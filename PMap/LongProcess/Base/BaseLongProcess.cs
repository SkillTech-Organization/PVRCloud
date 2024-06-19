﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using PMapCore.Common;

namespace PMapCore.LongProcess.Base
{
    /// <summary>
    /// Hosszú folyamat alaposztály
    /// </summary>
    public class BaseLongProcess
    {

        // Main thread sets this event to stop worker thread:
        public ManualResetEvent EventStop { get; private set; }
        // Worker thread sets this event when it is stopped:
        public ManualResetEvent EventStopped { get; private set; }

        protected Thread m_WorkingThread = null;


        protected ThreadPriority m_ThreadPriority;

        public BaseLongProcess(ThreadPriority p_ThreadPriority)
        {
            m_ThreadPriority = p_ThreadPriority;
            init();
        }


        private void init()
        {

            EventStop = new ManualResetEvent(false);
            EventStopped = new ManualResetEvent(false);

            // reset events
            //Sets the state of the specified event to nonsignaled,
            //which causes threads to block.
            if (EventStop != null)
                EventStop.Reset();

            if (EventStopped != null)
                EventStopped.Reset();

        }


        public void Run()
        {
            m_WorkingThread = new Thread(new ThreadStart(this.DoWorkWrapper));
            m_WorkingThread.Priority = m_ThreadPriority;

            m_WorkingThread.Name = "TH_" + DateTime.Now.ToString();
            m_WorkingThread.Start();
        }

        public void RunWait()
        {
            m_WorkingThread = new Thread(new ParameterizedThreadStart(delegate(object state)
            {
                ManualResetEvent handle = (ManualResetEvent)state;
                this.DoWorkWrapper();
                handle.Set();
            }));

            m_WorkingThread.Priority = m_ThreadPriority;

            ManualResetEvent waitHandle = new ManualResetEvent(false);
            m_WorkingThread.Start(waitHandle);
            waitHandle.WaitOne();
        }

        private void DoWorkWrapper()
        {

            DoWork();
            EventStopped.Set();
        }

        protected virtual void DoWork()
        {

            if (EventStop != null && EventStop.WaitOne(0, true))
            {
                EventStopped.Set();
                return;
            }
        }


        public virtual void Stop()
        {

            if (IsAlive())
            {
                //set event "Stop"
                //Sets the state of the specified event to signaled
                EventStop.Set();

                // wait when thread  will stop or finish
                while (IsAlive())
                {
                    if (WaitHandle.WaitAll((new ManualResetEvent[] { EventStopped }), 100, true))
                    {
                            break;
                    }
//                    Application.DoEvents();
                }
                System.GC.Collect();

                Process proc = Process.GetCurrentProcess();
                Util.Log2File("After killed thread:" + proc.PrivateMemorySize64.ToString());
            }
        }


        public bool IsAlive()
        {
            if (m_WorkingThread != null)
                return m_WorkingThread.IsAlive;
            else
                return false;
        }

    }
}
