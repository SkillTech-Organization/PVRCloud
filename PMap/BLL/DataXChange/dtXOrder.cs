using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.DB.Base;
using PMapCore.BO;
using PMapCore.Common;
using PMapCore.Strings;
using PMapCore.BO.DataXChange;
using System.Runtime.ExceptionServices;

namespace PMapCore.BLL.DataXChange
{
    public class dtXOrder : bllBase
    {

        public dtXOrder(SQLServerAccess p_DBA)
            : base(p_DBA, "")
        {
        }

        public List<dtXResult> ImportOrders(List<boXOrder> p_orders)
        {

            List<dtXResult> result = new List<dtXResult>();

            bllDepot bllDepot = new bllDepot(DBA);
            bllWarehouse bllWarehouse = new bllWarehouse(DBA);
            bllOrder bllOrder = new bllOrder(DBA);
            bllCargoType bllCargoType = new bllCargoType(DBA);
            bllOrderType bllOrderType = new bllOrderType(DBA);

            int nItem = 0;
            foreach (boXOrder xOrder in p_orders)
            {
                try
                {
                    List<ObjectValidator.ValidationError> validationErros = ObjectValidator.ValidateObject(xOrder);
                    if (validationErros.Count == 0)
                    {

                        bool bValidated = true;
                        boWarehouse warehouse = null;
                        boCargoType ct = null;
                        boOrderType ot = null;
                        int DEP_ID = 0;

                        //Létező megrendelés?
                        if (bllOrder.GetOrderByORD_NUM(xOrder.ORD_NUM) != null)
                        {
                            bValidated = false;
                            dtXResult itemRes = new dtXResult()
                            {
                                ItemNo = nItem,
                                Field = xOrder.GetType().Name + ".ORD_NUM",
                                Status = dtXResult.EStatus.ERROR,
                                ErrMessage = DXMessages.E_ORD_NUM_EXISTS
                            };
                            result.Add(itemRes);
                        }

                        //raktár ellenőrzés
                        warehouse = bllWarehouse.GetWarehouseByCODE(xOrder.WHS_CODE);
                        if (warehouse == null)
                        {
                            bValidated = false;
                            dtXResult itemRes = new dtXResult()
                            {
                                ItemNo = nItem,
                                Field = xOrder.GetType().Name + ".WHS_CODE",
                                Status = dtXResult.EStatus.ERROR,
                                ErrMessage = DXMessages.E_UNKWN_WHS_CODE
                            };
                            result.Add(itemRes);
                        }

                        //árutípus ellenőrzés
                        ct = bllCargoType.GetCargoTypeByCODE(xOrder.CTP_CODE);
                        if (ct == null)
                        {
                            bValidated = false;
                            dtXResult itemRes = new dtXResult()
                            {
                                ItemNo = nItem,
                                Field = xOrder.GetType().Name + ".CTP_CODE",
                                Status = dtXResult.EStatus.ERROR,
                                ErrMessage = DXMessages.E_UNKWN_CTP_CODE
                            };
                            result.Add(itemRes);
                        }


                        //megrendeléstípus ellenőrzés
                        ot = bllOrderType.GetOrderTypeByCODE(xOrder.OTP_CODE);
                        if (ot == null)
                        {
                            bValidated = false;
                            dtXResult itemRes = new dtXResult()
                            {
                                ItemNo = nItem,
                                Field = xOrder.GetType().Name + ".OTP_CODE",
                                Status = dtXResult.EStatus.ERROR,
                                ErrMessage = DXMessages.E_UNKWN_OTP_CODE
                            };
                            result.Add(itemRes);
                        }

                        if (bValidated)
                        {
                            boDepot depot = bllDepot.GetDepotByDEP_CODE(xOrder.DEP_CODE);
                            if (depot == null)
                            {
                                boXDepot xDepot = new boXDepot()
                                {
                                    DEP_CODE = xOrder.DEP_CODE,
                                    DEP_NAME = xOrder.DEP_NAME != "" ? xOrder.DEP_NAME : xOrder.DEP_CODE,
                                    ZIP_NUM = xOrder.ZIP_NUM,
                                    ZIP_CITY = xOrder.ZIP_CITY,
                                    DEP_ADRSTREET = xOrder.DEP_ADRSTREET,
                                    DEP_ADRNUM = xOrder.DEP_ADRNUM,
                                    DEP_OPEN = xOrder.DEP_OPEN != 0 ? xOrder.DEP_OPEN : xOrder.ORD_SERVS,
                                    DEP_CLOSE = xOrder.DEP_CLOSE != 0 ? xOrder.DEP_CLOSE : xOrder.ORD_SERVE,
                                    DEP_COMMENT = xOrder.DEP_COMMENT,
                                    DEP_SRVTIME = xOrder.DEP_SRVTIME,
                                    DEP_QTYSRVTIME = xOrder.DEP_QTYSRVTIME,
                                    DEP_LIFETIME = xOrder.DEP_LIFETIME,
                                    WHS_CODE = xOrder.WHS_CODE
                                };
                                dtXDepot xdep = new dtXDepot(DBA);
                                List<boXDepot> lstDep = new List<boXDepot>();
                                lstDep.Add(xDepot);
                                List<dtXResult> resDep = xdep.ImportDepots(lstDep);
                                if (resDep.Count > 0)
                                {
                                    dtXResult depOkRes = resDep.Where(i => i.Status == dtXResult.EStatus.OK).FirstOrDefault();

                                    if (depOkRes != null)
                                    {
                                        DEP_ID = (int)depOkRes.Data;
                                    }
                                    else
                                    {
                                        bValidated = false;
                                        result.AddRange(resDep);
                                    }
                                }
                                else
                                    bValidated = false;

                            }
                            else
                            {
                                DEP_ID = depot.ID;
                            }
                        }
                        if (bValidated)
                        {

                            boOrder ord = new boOrder()
                                            {
                                                OTP_ID = ot.ID,
                                                CTP_ID = ct.ID,
                                                DEP_ID = DEP_ID,
                                                WHS_ID = warehouse.ID,
                                                ORD_NUM = xOrder.ORD_NUM,
                                                ORD_ORIGNUM = xOrder.ORD_NUM,
                                                ORD_DATE = xOrder.ORD_DATE != null ? ((DateTime)(xOrder.ORD_DATE)).Date : DateTime.Now.Date,
                                                ORD_CLIENTNUM = xOrder.ORD_CLIENTNUM,
                                                ORD_FIRSTDATE = xOrder.ORD_FIRSTDATE != null ? ((DateTime)(xOrder.ORD_FIRSTDATE)).Date : DateTime.Now.Date,
                                                ORD_QTY = xOrder.ORD_QTY,
                                                ORD_ORIGQTY1 = xOrder.ORD_QTY,
                                                ORD_ORIGQTY2 = 0,
                                                ORD_ORIGQTY3 = 0,
                                                ORD_ORIGQTY4 = 0,
                                                ORD_ORIGQTY5 = 0,
                                                ORD_SERVS = xOrder.ORD_SERVS,
                                                ORD_SERVE = xOrder.ORD_SERVE,
                                                ORD_VOLUME = xOrder.ORD_VOLUME,
                                                ORD_LENGTH = xOrder.ORD_LENGTH,
                                                ORD_WIDTH = xOrder.ORD_WIDTH,
                                                ORD_HEIGHT = xOrder.ORD_HEIGHT,
                                                ORD_LOCKED = false,
                                                ORD_GATE = "",
                                                ORD_COMMENT = xOrder.ORD_COMMENT,
                                                ORD_UPDATED = false,
                                                ORD_ACTIVE = true
                                            };

                            using (TransactionBlock transObj = new TransactionBlock(DBA))
                            {
                                try
                                {
                                    ord.ID = bllOrder.AddOrder(ord);
                                }

                                catch (Exception e)
                                {
                                    DBA.Rollback();
                                    ExceptionDispatchInfo.Capture(e).Throw();
                                    throw;
                                }
                            }


                            dtXResult itemRes = new dtXResult()
                            {
                                ItemNo = nItem,
                                Status = dtXResult.EStatus.OK,
                                Data = ord.ID
                            };
                            result.Add(itemRes);
                        }


                    }
                    else
                    {
                        foreach (var err in validationErros)
                        {
                            dtXResult itemRes = new dtXResult()
                            {
                                ItemNo = nItem,
                                Field = err.Field,
                                Status = dtXResult.EStatus.VALIDATIONERROR,
                                ErrMessage = err.Message

                            };
                            result.Add(itemRes);
                        }
                    }
                }

                catch (Exception e)
                {
                    dtXResult itemRes = new dtXResult()
                    {
                        ItemNo = nItem,
                        Status = dtXResult.EStatus.EXCEPTION,
                        ErrMessage = Util.GetExceptionText(e),
                        Data = (object)e.StackTrace
                    };
                    result.Add(itemRes);
                    Util.ExceptionLog(e);
                }
                nItem++;
            }
            return result;
        }
    }
}

