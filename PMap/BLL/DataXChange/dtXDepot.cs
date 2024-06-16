using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL;
using PMapCore.DB.Base;
using PMapCore.BO;
using PMapCore.Common;
using PMapCore.Strings;
using GMap.NET;
using PMapCore.BLL.Base;
using PMapCore.BO.DataXChange;
using System.Runtime.ExceptionServices;

namespace PMapCore.BLL.DataXChange
{
    public class dtXDepot : bllBase
    {
        public dtXDepot(SQLServerAccess p_DBA)
            : base(p_DBA, "")
        {
        }

        public List<dtXResult> ImportDepots(List<boXDepot> p_depots, bool p_addAllTrucksToDepot = true)
        {

            List<dtXResult> result = new List<dtXResult>();
            bllDepot bllDepot = new bllDepot(DBA);
            bllWarehouse bllWarehouse = new bllWarehouse(DBA);
            bllZIP bllZIP = new bllZIP(DBA);
            bllRoute bllRoute = new bllRoute(DBA);
            boZIP zip = null;
            boWarehouse warehouse = null;

            int nItem = 0;
            foreach (boXDepot xDepot in p_depots)
            {
                try
                {

                    boDepot depot = new boDepot();
                    List<ObjectValidator.ValidationError> validationErros = ObjectValidator.ValidateObject(xDepot);
                    if (validationErros.Count == 0)
                    {
                        bool bValidated = true;

                        //validálások
                        if (bllDepot.GetDepotByDEP_CODE(xDepot.DEP_CODE) != null)
                        {
                            bValidated = false;
                            dtXResult itemRes = new dtXResult()
                            {
                                ItemNo = nItem,
                                Field = xDepot.GetType().Name + ".DEP_CODE",
                                Status = dtXResult.EStatus.ERROR,
                                ErrMessage = DXMessages.E_DPL_DEP_CODE
                            };
                            result.Add(itemRes);
                        }

                        warehouse = bllWarehouse.GetWarehouseByCODE(xDepot.WHS_CODE);
                        if (warehouse == null)
                        {
                            bValidated = false;
                            dtXResult itemRes = new dtXResult()
                            {
                                ItemNo = nItem,
                                Field = xDepot.GetType().Name + ".WHS_CODE",
                                Status = dtXResult.EStatus.ERROR,
                                ErrMessage = DXMessages.E_UNKWN_WHS_CODE
                            };
                            result.Add(itemRes);
                        }

                        zip = bllZIP.GetZIPbyNumAndCity(xDepot.ZIP_NUM, xDepot.ZIP_CITY);

                        if (zip == null)
                        {
                            
                            dtXResult itemRes = new dtXResult()
                            {
                                ItemNo = nItem,
                                Field = xDepot.GetType().Name + ".ZIP_NUM",
                                Status = xDepot.ZIP_CITY == "" ? dtXResult.EStatus.ERROR: dtXResult.EStatus.WARNING,
                                ErrMessage = DXMessages.E_UNKWN_ZIP_NUM
                            };

                            bValidated = bValidated && xDepot.ZIP_CITY != "";
                            result.Add(itemRes);
                        }
                        else
                        {
                            xDepot.ZIP_CITY = zip.ZIP_CITY;
                        }

                        //Geokódolás
                        int ZIP_ID = -1;
                        int NOD_ID = -1;
                        int EDG_ID = -1;
                        boDepot.EIMPADDRSTAT DEP_IMPADDRSTAT = boDepot.EIMPADDRSTAT.MISSADDR;
                        if (xDepot.Lat != 0 && xDepot.Lng != 0)
                        {
                            NOD_ID = bllRoute.GetNearestNOD_ID(new PointLatLng(xDepot.Lat, xDepot.Lng));
                            boEdge edg = bllRoute.GetEdgeByNOD_ID(NOD_ID);
                            if (edg != null)
                            {
                                EDG_ID = edg.ID;
                                DEP_IMPADDRSTAT = boDepot.EIMPADDRSTAT.AUTOADDR_LATLNG;
                            }
                            else
                            {
                                //ideiglenes megoldás!!
                                bValidated = false;
                                dtXResult itemRes = new dtXResult()
                                {
                                    ItemNo = nItem,
                                    Field = xDepot.GetType().Name + ".Lat, " + xDepot.GetType().Name + ".Lng",
                                    Status = dtXResult.EStatus.ERROR,
                                    ErrMessage = DXMessages.E_ERR_GEOCODING
                                };
                                result.Add(itemRes);

                            }
                        }
                        else
                        {
                            /*
                            string sCity = zip.ZIP_CITY;
                            if (sCity.ToUpper().IndexOf("BUDAPEST") >= 0)
                                sCity = "BUDAPEST";
                            */
                            string[] aStreet = xDepot.DEP_ADRSTREET.Split(' ');
                            string sAddr = (zip != null ? zip.ZIP_NUM.ToString() + " " : "" ) + xDepot.ZIP_CITY + " " + aStreet[0] + " . " + xDepot.DEP_ADRNUM;
                            if (!bllRoute.GeocodingByAddr(sAddr, out ZIP_ID, out NOD_ID, out EDG_ID, out DEP_IMPADDRSTAT))
                            {
                                //ideiglenes megoldás!!
                                bValidated = false;
                                dtXResult itemRes = new dtXResult()
                                {
                                    ItemNo = nItem,
                                    Field = xDepot.GetType().Name + ".ZIP_NUM, " + xDepot.GetType().Name + ".DEP_ADRSTREET",
                                    Status = dtXResult.EStatus.ERROR,
                                    ErrMessage = DXMessages.E_ERR_GEOCODING
                                };
                                result.Add(itemRes);
                            }


                            if (bValidated)
                            {
                                if (xDepot.DEP_OPEN > xDepot.DEP_CLOSE)
                                {
                                    int tmp = xDepot.DEP_OPEN;
                                    xDepot.DEP_OPEN = xDepot.DEP_CLOSE;
                                    xDepot.DEP_CLOSE = tmp;
                                }

                                //xDepot-->depot
                                depot.ZIP_ID = ZIP_ID;
                                depot.NOD_ID = NOD_ID;
                                depot.EDG_ID = EDG_ID;
                                depot.REG_ID = 0;
                                depot.WHS_ID = warehouse.ID;
                                depot.DEP_CODE = xDepot.DEP_CODE;
                                depot.DEP_NAME = xDepot.DEP_NAME;
                                depot.DEP_ADRSTREET = xDepot.DEP_ADRSTREET;
                                depot.DEP_ADRNUM = xDepot.DEP_ADRNUM;
                                depot.DEP_OPEN = xDepot.DEP_OPEN;
                                depot.DEP_CLOSE = xDepot.DEP_CLOSE;
                                depot.DEP_COMMENT = xDepot.DEP_COMMENT;
                                depot.DEP_SRVTIME = xDepot.DEP_QTYSRVTIME;
                                depot.DEP_QTYSRVTIME = xDepot.DEP_QTYSRVTIME;
                                depot.DEP_CLIENTNUM = "";
                                depot.DEP_IMPADDRSTAT = DEP_IMPADDRSTAT;
                                depot.DEP_LIFETIME = xDepot.DEP_LIFETIME;

                                using (TransactionBlock transObj = new TransactionBlock(DBA))
                                {
                                    try
                                    {

                                        int DEP_ID = bllDepot.AddDepot(depot);
                                        if(p_addAllTrucksToDepot)
                                            bllDepot.SetAllTruckToDep(DEP_ID);
                                        else
                                            bllDepot.SetRegTruckToDep(DEP_ID);

                                        boXDepotRes res = new boXDepotRes() { ID = DEP_ID, Lat = 0, Lng = 0 };
                                          boNode nod = bllRoute.GetNode(NOD_ID);
                                          if (nod != null)
                                          {
                                              res.Lat = nod.NOD_YPOS / Global.LatLngDivider;
                                              res.Lng = nod.NOD_XPOS / Global.LatLngDivider;
                                          }

                                        dtXResult itemRes = new dtXResult()
                                        {
                                            ItemNo = nItem,
                                            Status = dtXResult.EStatus.OK,
                                            Data = res
                                        };
                                        result.Add(itemRes);

                                    }

                                    catch (Exception e)
                                    {
                                        DBA.Rollback();
                                        ExceptionDispatchInfo.Capture(e).Throw();
                                        throw;
                                    }
                                }
                            }
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
                        Data = (object) e.StackTrace
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
