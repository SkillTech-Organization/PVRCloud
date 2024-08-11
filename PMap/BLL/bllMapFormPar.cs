using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PMapCore.DB.Base;
using PMapCore.Common;
using System.Runtime.ExceptionServices;

namespace PMapCore.BLL
{
    public static class bllMapFormPar
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_PLN_ID"></param>
        /// <param name="p_USR_ID"></param>
        /// <param name="p_MPP_WINDOW"></param>
        /// <param name="p_MPP_DOCK"></param>
        /// <param name="p_MPP_PARAM"></param>
        /// <param name="p_MPP_TGRID"></param>
        /// <param name="p_MPP_PGRID"></param>
        /// <param name="p_MPP_UGRID"></param>
        public static void SaveParameters(int p_PLN_ID, int p_USR_ID, string p_MPP_WINDOW, string p_MPP_DOCK, string p_MPP_PARAM, string p_MPP_TGRID, string p_MPP_PGRID, string p_MPP_UGRID)
        {
            /*
                p_PLN_ID > 0 : Tervezés képernyő  (Egyenlőre nincs tervenkénti paraméterbeállítás, ezért ez esetben a
                                p_PLN_ID-t 0-ra vesszük.
                p_PLN_ID < 0 : Más képernyők elrendezésének mentése
                                -1: frmMPorder layout
            */
            if (p_PLN_ID > 0) 
                p_PLN_ID = 0;


            using (TransactionBlock transObj = new TransactionBlock(PMapCommonVars.Instance.CT_DB))
            {
                try
                {

                    string sSQL = "select * from MPP_MAPPLANPAR where PLN_ID = ? and USR_ID = ? ";

                    DataTable dt = PMapCommonVars.Instance.CT_DB.Query2DataTable(sSQL, p_PLN_ID, p_USR_ID);
                    if (dt.Rows.Count == 0)
                    {
                        sSQL = "insert into MPP_MAPPLANPAR (PLN_ID, USR_ID, MPP_WINDOW, MPP_DOCK, MPP_PARAM, MPP_TGRID, MPP_PGRID, MPP_UGRID) " +
                               "values( ?, ?, ?, ?, ?, ?, ?, ?)";
                        PMapCommonVars.Instance.CT_DB.ExecuteNonQuery(sSQL,
                            p_PLN_ID, p_USR_ID, p_MPP_WINDOW, p_MPP_DOCK, p_MPP_PARAM, p_MPP_TGRID, p_MPP_PGRID, p_MPP_UGRID);
                    }
                    else
                    {

                        sSQL = "update MPP_MAPPLANPAR set MPP_WINDOW=?, MPP_DOCK=?, MPP_PARAM=?, MPP_TGRID=?, MPP_PGRID=?, MPP_UGRID=? where PLN_ID = ? and USR_ID=? ";
                        PMapCommonVars.Instance.CT_DB.ExecuteNonQuery(sSQL,
                                p_MPP_WINDOW, p_MPP_DOCK, p_MPP_PARAM, p_MPP_TGRID, p_MPP_PGRID, p_MPP_UGRID, p_PLN_ID, p_USR_ID);
                    }
                }
                catch (Exception e)
                {
                    PMapCommonVars.Instance.CT_DB.Rollback();
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }
        }

        public static bool RestoreParameters(int p_PLN_ID, int p_USR_ID, out string o_MPP_WINDOW, out string o_MPP_DOCK, out string o_MPP_PARAM, out string o_MPP_TGRID, out string o_MPP_PGRID, out string o_MPP_UGRID)
        {

            /*
                p_PLN_ID > 0 : Tervezés képernyő  (Egyenlőre nincs tervenkénti paraméterbeállítás, ezért ez esetben a
                                p_PLN_ID-t 0-ra vesszük.
                p_PLN_ID < 0 : Más képernyők elrendezésének mentése
                                -1: frmMPorder layout
            */
            if (p_PLN_ID > 0)
                p_PLN_ID = 0;

            try
            {
                string sSQL = "select * from MPP_MAPPLANPAR where PLN_ID = ? and USR_ID = ? ";

                DataTable dt = PMapCommonVars.Instance.CT_DB.Query2DataTable(sSQL, p_PLN_ID, p_USR_ID);
                if (dt.Rows.Count > 0)
                {
                    o_MPP_WINDOW = dt.Rows[0].Field<string>("MPP_WINDOW");
                    o_MPP_DOCK = dt.Rows[0].Field<string>("MPP_DOCK");
                    o_MPP_PARAM = dt.Rows[0].Field<string>("MPP_PARAM");
                    o_MPP_TGRID = dt.Rows[0].Field<string>("MPP_TGRID");
                    o_MPP_PGRID = dt.Rows[0].Field<string>("MPP_PGRID");
                    o_MPP_UGRID = dt.Rows[0].Field<string>("MPP_UGRID");

                    return true;
                }
                else
                {
                    o_MPP_WINDOW = "";
                    o_MPP_DOCK = "";
                    o_MPP_PARAM = "";
                    o_MPP_TGRID = "";
                    o_MPP_PGRID = "";
                    o_MPP_UGRID = "";
                    return false;

                }
            }
            catch (Exception e)
            {
                PMapCommonVars.Instance.CT_DB.Rollback();
                ExceptionDispatchInfo.Capture(e).Throw();
                throw;
            }
        }

        public static void RemoveParameters(int p_PLN_ID, int p_USR_ID)
        {
            //Egyenlőre nincs tervenkénti paraméterbeállítás
            p_PLN_ID = 0;
            string sSQL = "delete MPP_MAPPLANPAR where PLN_ID = ? and USR_ID = ? ";
            PMapCommonVars.Instance.CT_DB.ExecuteNonQuery(sSQL, p_PLN_ID, p_USR_ID);
        }
    }
}
