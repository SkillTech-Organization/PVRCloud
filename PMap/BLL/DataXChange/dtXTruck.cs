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
    public class dtXTruck : bllBase
    {
        public dtXTruck(SQLServerAccess p_DBA)
            : base(p_DBA, "")
        {
        }

        public List<dtXResult> ImportTrucks(List<boXTruck> p_trucks)
        {
            List<dtXResult> result = new List<dtXResult>();
            bllTruck bllTruck = new bllTruck(DBA);
            bllCarrier bllCarrier = new bllCarrier(DBA);
            bllWarehouse bllWarehouse = new bllWarehouse(DBA);
            bllSpeedProf bllSpeedProf = new bllSpeedProf(DBA);
            bllCapacityProf bllCapacityProf = new bllCapacityProf(DBA);
            bllTariffProf bllTariffProf = new bllTariffProf(DBA);

            int nItem = 0;
            foreach (boXTruck xTruck in p_trucks)
            {
                try
                {

                    boCarrier carrier = null;
                    boWarehouse warehouse = null;
                    List<ObjectValidator.ValidationError> validationErros = ObjectValidator.ValidateObject(xTruck);
                    if (validationErros.Count == 0)
                    {
                        bool bValidated = true;


                        carrier = bllCarrier.GetCarrierByCODE(xTruck.CRR_CODE);
                        if (carrier == null)
                        {
                            bValidated = false;
                            dtXResult itemRes = new dtXResult()
                            {
                                ItemNo = nItem,
                                Field = xTruck.GetType().Name + ".CRR_CODE",
                                Status = dtXResult.EStatus.ERROR,
                                ErrMessage = DXMessages.E_UNKWN_CRR_CODE
                            };
                            result.Add(itemRes);
                        }

                        warehouse = bllWarehouse.GetWarehouseByCODE(xTruck.WHS_CODE);
                        if (warehouse == null)
                        {
                            bValidated = false;
                            dtXResult itemRes = new dtXResult()
                            {
                                ItemNo = nItem,
                                Field = xTruck.GetType().Name + ".WHS_CODE",
                                Status = dtXResult.EStatus.ERROR,
                                ErrMessage = DXMessages.E_UNKWN_WHS_CODE
                            };
                            result.Add(itemRes);
                        }


                        if (bValidated)
                        {



                            //xDepot-->depot

                            using (TransactionBlock transObj = new TransactionBlock(DBA))
                            {
                                try
                                {

                                    //sebességprofil létrehozása, amennyiben szükésges
                                    boSpeedProf speedprof = bllSpeedProf.GetSpeedProfByValues(xTruck.SPV_VALUE1, xTruck.SPV_VALUE2, xTruck.SPV_VALUE3, xTruck.SPV_VALUE4, xTruck.SPV_VALUE5, xTruck.SPV_VALUE6, xTruck.SPV_VALUE7);
                                    if (speedprof == null)
                                    {
                                        speedprof = new boSpeedProf();
                                        speedprof.SPP_NAME = "TRKImport";
                                        speedprof.SPEED1 = xTruck.SPV_VALUE1;
                                        speedprof.SPEED2 = xTruck.SPV_VALUE2;
                                        speedprof.SPEED3 = xTruck.SPV_VALUE3;
                                        speedprof.SPEED4 = xTruck.SPV_VALUE4;
                                        speedprof.SPEED5 = xTruck.SPV_VALUE5;
                                        speedprof.SPEED6 = xTruck.SPV_VALUE6;
                                        speedprof.SPEED7 = xTruck.SPV_VALUE7;
                                        speedprof.ID = bllSpeedProf.AddSpeedProf(speedprof);
                                    }

                                    //kapacitásprofil létrehozása amennyiben szükséges
                                    boCapacityProf capacityprof = bllCapacityProf.GetCapacityProfByValues(xTruck.CPP_LOADQTY, xTruck.CPP_LOADVOL);
                                    if (capacityprof == null)
                                    {
                                        capacityprof = new boCapacityProf();
                                        capacityprof.CPP_NAME = "TRKImport";
                                        capacityprof.CPP_LOADQTY = xTruck.CPP_LOADQTY;
                                        capacityprof.CPP_LOADVOL = xTruck.CPP_LOADVOL;
                                        capacityprof.ID = bllCapacityProf.AddCapacityProf(capacityprof);
                                    }

                                    //tarifaprofil létrehozása amennyiben szükséges
                                    boTariffProf tariffprof = bllTariffProf.GetTariffProfByValues(xTruck.TFP_FIXCOST, xTruck.TFP_KMCOST, xTruck.TFP_HOURCOST);
                                    if (tariffprof == null)
                                    {
                                        tariffprof = new boTariffProf();
                                        tariffprof.TFP_NAME = "TRKImport";
                                        tariffprof.TFP_FIXCOST = xTruck.TFP_FIXCOST;
                                        tariffprof.TFP_KMCOST = xTruck.TFP_KMCOST;
                                        tariffprof.TFP_HOURCOST = xTruck.TFP_HOURCOST;

                                        tariffprof.ID = bllTariffProf.AddTariffProf(tariffprof);
                                    }

                                    bool bNew = false;
                                    boTruck truck = bllTruck.GetTruckByCODE(xTruck.TRK_CODE);
                                    if (truck == null)
                                    {
                                        truck = new boTruck();
                                        bNew = true;
                                    }


                                    truck.CRR_ID = carrier.ID;
                                    truck.SPP_ID = speedprof.ID;
                                    truck.WHS_ID = warehouse.ID;
                                    truck.CPP_ID = capacityprof.ID;
                                    truck.TFP_ID = tariffprof.ID;
                                    truck.TFP_ID_INC = tariffprof.ID;
                                    truck.TFP_ID_OUT = tariffprof.ID;
                                    truck.TRK_CODE = xTruck.TRK_CODE;
                                    truck.TRK_REG_NUM = xTruck.TRK_REG_NUM;
                                    truck.TRK_TRAILER = xTruck.TRK_TRAILER;
                                    truck.TRK_ACTIVE = xTruck.TRK_ACTIVE;
                                    truck.TRK_GPS = xTruck.TRK_GPS;
                                    truck.TRK_BACKPANEL = xTruck.TRK_BACKPANEL;
                                    truck.TRK_LOGO = xTruck.TRK_LOGO;
                                    truck.TRK_IDLETIME = xTruck.TRK_IDLETIME;
                                    truck.TRK_BUNDTIME = 0;      //TODO:Kivezetni?
                                    truck.TRK_BUNDPOINT = 0;
                                    truck.TRK_BUNDDATE = DateTime.Now;
                                    truck.TRK_COLOR = xTruck.TRK_COLOR;
                                    truck.TRK_WEIGHT = xTruck.TRK_WEIGHT;
                                    truck.TRK_XHEIGHT = xTruck.TRK_XHEIGHT;
                                    truck.TRK_XWIDTH = xTruck.TRK_XWIDTH;
                                    truck.TRK_WIDTH = xTruck.TRK_WIDTH;
                                    truck.TRK_HEIGHT = xTruck.TRK_HEIGHT;
                                    truck.TRK_LENGTH = xTruck.TRK_LENGTH;
                                    truck.TRK_AXLENUM = xTruck.TRK_AXLENUM;
                                    truck.TRK_ENGINEEURO = xTruck.TRK_ENGINEEURO;

                                    truck.TRK_ETOLLCAT = xTruck.TRK_ETOLLCAT;
                                    truck.TRK_COMMENT = xTruck.TRK_COMMENT;

                                    if (bNew)
                                    {
                                        truck.TRK_DELETED = false;
                                        truck.ID = bllTruck.AddTruck(truck);
                                        bllTruck.SeTruckToAllRegions(truck.ID);
                                        bllTruck.SeTruckToAllDepots(truck.ID);
                                    }
                                    else
                                    {
                                        bllTruck.UpdateTruck(truck);
                                    }

                                    bllTruck.SeTruckToAllCargoTypes(truck.ID);
                                    bllTruck.UpdateRestrZonesByWeight(truck.ID, true);

                                    dtXResult itemRes = new dtXResult()
                                    {
                                        ItemNo = nItem,
                                        Status = dtXResult.EStatus.OK,
                                        Data = truck.ID
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
