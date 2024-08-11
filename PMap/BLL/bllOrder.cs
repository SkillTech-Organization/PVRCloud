using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.DB.Base;
using PMapCore.BO;
using System.Data;
using PMapCore.Common;
using System.Runtime.ExceptionServices;

namespace PMapCore.BLL
{
    public class bllOrder : bllBase
    {
        public bllOrder(SQLServerAccess p_DBA)
            : base(p_DBA, "ORD_ORDER")
        {
        }

        public List<boOrder> GetAllOrders(string p_where = "", params object[] p_pars)
        {
            string sSql = "select ORD.* " + Environment.NewLine +
                          "  from ORD_ORDER ORD " + Environment.NewLine;
            if (p_where != "")
                sSql += " where " + p_where;
            DataTable dt = DBA.Query2DataTable(sSql, p_pars);
            var linq = (from r in dt.AsEnumerable()
                        orderby r.Field<int>("ID")
                        select new boOrder
            {

                ID = Util.getFieldValue<int>(r, "ID"),

                OTP_ID = Util.getFieldValue<int>(r, "OTP_ID"),
                CTP_ID = Util.getFieldValue<int>(r, "CTP_ID"),
                DEP_ID = Util.getFieldValue<int>(r, "DEP_ID"),
                WHS_ID = Util.getFieldValue<int>(r, "WHS_ID"),

                ORD_NUM = Util.getFieldValue<string>(r, "ORD_NUM"),
                ORD_ORIGNUM = Util.getFieldValue<string>(r, "ORD_ORIGNUM"),

                ORD_DATE = Util.getFieldValue<DateTime>(r, "ORD_DATE"),
                ORD_CLIENTNUM = Util.getFieldValue<string>(r, "ORD_CLIENTNUM"),

                ORD_LOCKDATE = Util.getFieldValue<DateTime>(r, "ORD_LOCKDATE"),
                ORD_FIRSTDATE = Util.getFieldValue<DateTime>(r, "ORD_FIRSTDATE"),

                ORD_QTY = Util.getFieldValue<double>(r, "ORD_QTY"),
                ORD_ORIGQTY1 = Util.getFieldValue<double>(r, "ORD_ORIGQTY1"),
                ORD_ORIGQTY2 = Util.getFieldValue<double>(r, "ORD_ORIGQTY2"),
                ORD_ORIGQTY3 = Util.getFieldValue<double>(r, "ORD_ORIGQTY3"),
                ORD_ORIGQTY4 = Util.getFieldValue<double>(r, "ORD_ORIGQTY4"),
                ORD_ORIGQTY5 = Util.getFieldValue<double>(r, "ORD_ORIGQTY5"),

                ORD_SERVS = Util.getFieldValue<int>(r, "ORD_SERVS"),
                ORD_SERVE = Util.getFieldValue<int>(r, "ORD_SERVE"),

                ORD_VOLUME = Util.getFieldValue<double>(r, "ORD_VOLUME"),
                ORD_LENGTH = Util.getFieldValue<double>(r, "ORD_LENGTH"),
                ORD_WIDTH = Util.getFieldValue<double>(r, "ORD_WIDTH"),
                ORD_HEIGHT = Util.getFieldValue<double>(r, "ORD_HEIGHT"),

                ORD_LOCKED = Util.getFieldValue<bool>(r, "ORD_LOCKED"),
                ORD_ISOPT = Util.getFieldValue<bool>(r, "ORD_ISOPT"),

                ORD_GATE = Util.getFieldValue<string>(r, "ORD_GATE"),
                ORD_COMMENT = Util.getFieldValue<string>(r, "ORD_COMMENT"),

                ORD_UPDATED = Util.getFieldValue<bool>(r, "ORD_UPDATED"),

                ORD_ACTIVE = Util.getFieldValue<bool>(r, "ORD_ACTIVE"),

                LASTDATE = Util.getFieldValue<DateTime>(r, "LASTDATE")
            });
            return linq.ToList();
        }

        public boOrder GetOrderByID(int p_ORD_ID)
        {
            List<boOrder> lstOrder = GetAllOrders("ID = ? ", p_ORD_ID);
            if (lstOrder.Count == 0)
                return null;
            else
                return lstOrder[0];
        }

        public boOrder GetOrderByORD_NUM(string p_ORD_NUM)
        {
            List<boOrder> lstOrder = GetAllOrders("ORD_NUM = ? ", p_ORD_NUM);
            if (lstOrder.Count == 0)
                return null;
            else
                return lstOrder[0];
        }

        public int AddOrder(boOrder p_Order)
        {
            int ORD_ID = 0;
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    ORD_ID = AddItem(p_Order);
                }

                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }

                return ORD_ID;
            }
        }

        public void UpdateOrder(boOrder p_Order)
        {
            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    UpdateItem(p_Order);
                    UpdateOrderQtyInPlans(p_Order.ID);
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }

        public void UpdateOrderQtyInPlans(int p_ORD_ID)
        {

            /*MEGJ: A TOD_CUTQTY** mezők már nem használtak */
            string sSql = "update TOD " + Environment.NewLine +
                          "set TOD.TOD_QTY = ORDx.ORD_QTY, " + Environment.NewLine +
                          "TOD.TOD_CUTQTY1 = ORDx.ORD_ORIGQTY1, " + Environment.NewLine +
                          "TOD.TOD_CUTQTY2 = ORDx.ORD_ORIGQTY2, " + Environment.NewLine +
                          "TOD.TOD_CUTQTY3 = ORDx.ORD_ORIGQTY3, " + Environment.NewLine +
                          "/*TOD_CUTQTY4 - dohányáru nincs !! */ " + Environment.NewLine +
                          "TOD.TOD_CUTQTY5 = ORDx.ORD_ORIGQTY5 " + Environment.NewLine +
                          "from TOD_TOURORDER TOD " + Environment.NewLine +
                          "inner join ORD_ORDER ORDx on ORDx.ID = TOD.ORD_ID " + Environment.NewLine +
                          "where ORDx.ID = ?" ;

            using (TransactionBlock transObj = new TransactionBlock(DBA))
            {
                try
                {
                    DBA.ExecuteNonQuery(sSql, p_ORD_ID);
                }
                catch (Exception e)
                {
                    DBA.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }

        public void DeleteOrder(int p_ORD_ID)
        {
            try
            {
                string sSql = "delete ORD_ORDER where ID = ?";
                DBA.ExecuteNonQuery(sSql, p_ORD_ID);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
                throw;
            }
        }

        public List<boPlan> GetOrderPlans( int p_id)
        {
            var res = new List<boPlan>();

            return res;
        }


    }
}
