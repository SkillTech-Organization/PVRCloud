using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BO.Base;
using PMapCore.Common.Attrib;

namespace PMapCore.BO.DataXChange
{
    public class boXFullPlan : boXBase
    {
        [DisplayNameAttributeX(Name = "Terv fejadatok", Order = 1)]
        public boXPlan Plan { get; set; }

        [DisplayNameAttributeX(Name = "Járművek és azok túrái", Order = 2)]
        public List<boXPlanTour> PlanTours { get; set; }

        [DisplayNameAttributeX(Name = "Tervben szereplő megrendelések", Order = 2)]
        public List<boXPlanOrder> PlanOrders { get; set; }

        public boXFullPlan()
        {
            Plan = new boXPlan();
            PlanTours = new  List<boXPlanTour>() ;
            PlanOrders = new List<boXPlanOrder>();
        }

    }
}
