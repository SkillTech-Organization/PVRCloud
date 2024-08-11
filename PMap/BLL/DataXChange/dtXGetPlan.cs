using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.DB.Base;
using PMapCore.BO.DataXChange;
using PMapCore.BO;

namespace PMapCore.BLL.DataXChange
{
    public class dtXGetPlan : bllBase
    {
        public dtXGetPlan(SQLServerAccess p_DBA)
            : base(p_DBA, "")
        {
        }
        public boXFullPlan GetPlan(int p_PLN_ID)
        {
            boXFullPlan res = new boXFullPlan();

            bllPlan bllPlan = new bllPlan(DBA);
            boXPlan Plan = new boXPlan();
            Plan.fillFromObject(bllPlan.GetPlan(p_PLN_ID));

            List<boPlanTour> PlanTours = bllPlan.GetPlanTours(p_PLN_ID);
            foreach (boPlanTour pt in PlanTours)
            {
                boXPlanTour ptx = new boXPlanTour();
                ptx.fillFromObject(pt);
                res.PlanTours.Add(ptx);
            }

            List<boPlanOrder> PlanOrders = bllPlan.GetPlanOrders(p_PLN_ID);
            foreach (boPlanOrder po in PlanOrders)
            {
                boXPlanOrder pox = new boXPlanOrder();
                pox.fillFromObject(po);
                res.PlanOrders.Add(pox);
            }

            return res;
        }

        public List<boXFullPlan> GetPlans(int p_WHS_ID, DateTime p_PLN_DATE_B, DateTime p_PLN_DATE_E)
        {
            List<boXFullPlan> res = new List<boXFullPlan>();

            bllPlan pl = new bllPlan(DBA);

            List<boPlan> plans = pl.GetPlans(p_WHS_ID, p_PLN_DATE_B, p_PLN_DATE_E);
            foreach (boPlan plan in plans)
            {
                boXFullPlan planItem = new boXFullPlan();

                planItem.Plan.fillFromObject(plan);

                List<boPlanTour> PlanTours = pl.GetPlanTours(plan.ID);
                foreach (boPlanTour pt in PlanTours)
                {
                    boXPlanTour ptx = new boXPlanTour();
                    ptx.fillFromObject(pt);
                    planItem.PlanTours.Add(ptx);
                }

                List<boPlanOrder> PlanOrders = pl.GetPlanOrders(plan.ID);
                foreach (boPlanOrder po in PlanOrders)
                {
                    boXPlanOrder pox = new boXPlanOrder();
                    pox.fillFromObject(po);
                    planItem.PlanOrders.Add(pox);
                }

                res.Add(planItem);
            }
            return res;
        }
    }
}
