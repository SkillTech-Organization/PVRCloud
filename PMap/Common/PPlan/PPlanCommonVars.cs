using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMap.NET;
using PMapCore.DB.Base;
using System.ComponentModel;
using PMapCore.DB;
using System.Xml.Serialization;
using GMap.NET.MapProviders;
using PMapCore.Route;
using PMapCore.BO;


namespace PMapCore.Common.PPlan
{
    public sealed class PPlanCommonVars
    {

        public class PPlanDragObject
        {
            public enum ESourceDataObjectType
            {
                [Description("TourPoint")]
                TourPoint = 0,
                [Description("Order")]
                Order = 1
            };

            public PPlanDragObject(ESourceDataObjectType p_SourceDataObjectType)
            {
                SourceDataObjectType = p_SourceDataObjectType;
            }

            public int ID { get; set; }
            public ESourceDataObjectType SourceDataObjectType { get; private set; }
            public object DataObject { get; set; }
            public object SrcGridControl { get; set; }  //DevExpress.XtraGrid.GridControl -re kell castolni !!!
        }

        // *** Lock ***
        private object m_propertyLock = new object();

        private int m_zoom;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public int Zoom
        {
            get
            {
                lock (m_propertyLock)
                {
                    return m_zoom;
                }
            }
            set
            {
                lock (m_propertyLock)
                {
                    m_zoom = value;
                }

            }
        }

        private PointLatLng m_currentPosition;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public PointLatLng CurrentPosition
        {
            get
            {
                lock (m_propertyLock)
                {
                    return m_currentPosition;
                }
            }
            set
            {
                lock (m_propertyLock)
                {
                    m_currentPosition = value;
                }

            }
        }

        public int m_PLN_ID;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public int PLN_ID
        {
            get
            {
                lock (m_propertyLock)
                {
                    return m_PLN_ID;
                }
            }
            set
            {
                lock (m_propertyLock)
                {
                    m_PLN_ID = value;
                }

            }
        }

        private List<boPlanTour> m_tourList;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public List<boPlanTour> TourList
        {
            get
            {
                lock (m_propertyLock)
                {
                    return m_tourList;
                }
            }
            set
            {
                lock (m_propertyLock)
                {
                    m_tourList = value;
                }

            }
        }


        private List<boPlanOrder> m_planOrderList;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public List<boPlanOrder> PlanOrderList
        {
            get
            {
                lock (m_propertyLock)
                {
                    return m_planOrderList;
                }
            }
            set
            {
                lock (m_propertyLock)
                {
                    m_planOrderList = value;
                }

            }
        }



        private PPlanDragObject m_draggedObj;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public PPlanDragObject DraggedObj
        {
            get
            {
                lock (m_propertyLock)
                {
                    return m_draggedObj;
                }
            }
            set
            {
                lock (m_propertyLock)
                {
                    m_draggedObj = value;
                }

            }
        }

        private boPlanTour m_focusedTour;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public boPlanTour FocusedTour
        {
            get
            {
                lock (m_propertyLock)
                {
                    return m_focusedTour;
                }
            }
            set
            {
                lock (m_propertyLock)
                {
                    m_focusedTour = value;
                }

            }
        }

        private boPlanTourPoint m_focusedPoint;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public boPlanTourPoint FocusedPoint
        {
            get
            {
                lock (m_propertyLock)
                {
                    return m_focusedPoint;
                }
            }
            set
            {
                lock (m_propertyLock)
                {
                    m_focusedPoint = value;
                }

            }
        }

        private boPlanOrder m_focusedUnplannedOrder;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public boPlanOrder FocusedUnplannedOrder
        {
            get
            {
                lock (m_propertyLock)
                {
                    return m_focusedUnplannedOrder;
                }
            }
            set
            {
                lock (m_propertyLock)
                {
                    m_focusedUnplannedOrder = value;
                }

            }
        }

    



        /// <summary>
        /// serilizálandó paraméterek
        /// </summary>
        private bool m_showPlannedDepots;
        public bool ShowPlannedDepots
        {
            get
            {
                lock (m_propertyLock)
                {
                    return m_showPlannedDepots;
                }
            }
            set
            {
                lock (m_propertyLock)
                {
                    m_showPlannedDepots = value;
                }

            }
        }

        private bool m_showUnPlannedDepots;
        public bool ShowUnPlannedDepots
        {
            get
            {
                lock (m_propertyLock)
                {
                    return m_showUnPlannedDepots;
                }
            }
            set
            {
                lock (m_propertyLock)
                {
                    m_showUnPlannedDepots = value;
                }

            }
        }


        private bool m_zoomToSelectedPlan;
        public bool ZoomToSelectedPlan
        {
            get
            {
                lock (m_propertyLock)
                {
                    return m_zoomToSelectedPlan;
                }
            }
            set
            {
                lock (m_propertyLock)
                {
                    m_zoomToSelectedPlan = value;
                }

            }
        }

        private bool m_zoomToSelectedUnPlanned;
        public bool ZoomToSelectedUnPlanned
        {
            get
            {
                lock (m_propertyLock)
                {
                    return m_zoomToSelectedUnPlanned;
                }
            }
            set
            {
                lock (m_propertyLock)
                {
                    m_zoomToSelectedUnPlanned = value;
                }

            }
        }

        private bool m_showAllOrdersInGrid;
        public bool ShowAllOrdersInGrid
        {
            get
            {
                lock (m_propertyLock)
                {
                    return m_showAllOrdersInGrid;
                }
            }
            set
            {
                lock (m_propertyLock)
                {
                    m_showAllOrdersInGrid = value;
                }

            }
        }

        public boPlanTour GetTourByID(int p_ID)
        {
            var linq = (from o in TourList
                        where o.ID == p_ID
                        select o);

            if (linq.Count<boPlanTour>() > 0)
                return linq.First<boPlanTour>();
            else
                return null;
        }

        public boPlanOrder GetPlannedOrderByID(int p_ID)
        {
            var linq = (from o in PlanOrderList
                        where o.ID == p_ID
                        select o);

            if (linq.Count<boPlanOrder>() > 0)
                return linq.First<boPlanOrder>();
            else
                return null;
        }

        public boPlanTourPoint GetTourPointByID(int p_ID)
        {
            foreach (boPlanTour tour in TourList)
            {

                var linq = (from o in tour.TourPoints
                            where o.ID == p_ID
                            select o);
                if (linq.Count<boPlanTourPoint>() > 0)
                {
                    return linq.First<boPlanTourPoint>();
                }
            }
            return null;
        }

        public boPlanTourPoint GetTourPointByORD_NUM(string p_ORD_NUM)
        {
            boPlanTourPoint oTp = null;
            p_ORD_NUM = p_ORD_NUM.ToUpper();
            foreach (boPlanTour tour in TourList)
            {
                if (tour.TourPoints != null)
                {
                    foreach (boPlanTourPoint tp in tour.TourPoints)
                    {
                        if (tp.ORD_NUM.ToUpper() == p_ORD_NUM)
                        {
                            oTp = tp;
                            break;
                        }
                    }
                    if (oTp != null)
                        break;
                }
            }
            return oTp;
        }

        public boPlanOrder GetOrderByORD_NUM(string p_ORD_NUM)
        {
            boPlanOrder oUpOrder = null;
            p_ORD_NUM = p_ORD_NUM.ToUpper();
            foreach (boPlanOrder upt in PlanOrderList)
            {
                if (upt.ORD_NUM.ToUpper() == p_ORD_NUM)
                {
                    oUpOrder = upt;
                    break;
                }
            }
            return oUpOrder;
        }

    }
}
