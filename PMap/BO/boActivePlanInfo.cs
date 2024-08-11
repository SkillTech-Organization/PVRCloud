using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.BO
{
    [Serializable]
    public class boActivePlanInfo
    {
        public int ID { get; set; }
        public string WHS_CODE { get; set; }
        public string WHS_NAME { get; set; }
        public int WHS_SRVTIME { get; set; }
        public int WHS_SRVTIME_UNLOAD { get; set; }
        public int WHS_OPEN { get; set; }
        public int WHS_CLOSE { get; set; }
        public DateTime OPEN
        {
            get
            {
                if (PLN_USEINTERVAL)
                    return PLN_INTERVAL_B.Date.AddMinutes(WHS_OPEN);
                else
                    return PLN_DATE_B.Date.AddMinutes(WHS_OPEN);
            }
        }

        public DateTime CLOSE
        {
            get
            {
                if (PLN_USEINTERVAL)
                    return PLN_INTERVAL_E.Date.AddMinutes(WHS_CLOSE);
                else
                    return PLN_DATE_E.Date.AddMinutes(WHS_CLOSE);
            }
        }

        public bool PLN_USEINTERVAL { get; set; }
        public DateTime PLN_DATE_B { get; set; }
        public DateTime PLN_DATE_E { get; set; }
        public DateTime PLN_INTERVAL_B { get; set; }
        public DateTime PLN_INTERVAL_E { get; set; }
        public int NOD_ID { get; set; }
        public int WHS_ID { get; set; }

    }

}
