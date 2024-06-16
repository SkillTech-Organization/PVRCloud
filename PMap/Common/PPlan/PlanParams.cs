using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.Common.PPlan
{
    
    public class PlanParams
    {
        public class CEnabledTruck
        {
            public int TRK_ID { get; set; }
            public DateTime AvailS { get; set; }       //elérhetőség kezdet
            public DateTime AvailE { get; set; }       //elérhetőség vége
        }
        public List<CEnabledTruck> EnabledTrucksInNewPlan { get; set; } 


        public PlanParams()
        {
            EnabledTrucksInNewPlan = new List<CEnabledTruck>();
        }
    }
}
