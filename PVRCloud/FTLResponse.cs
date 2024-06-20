﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVRCloud;

public class PVRCloudResponse
{
    public string RequestID { get; set; }
    public List<PVRCloudTask> TaskList { get; set; }
    public List<PVRCloudTruck> TruckList { get; set; }
    public int MaxTruckDistance { get; set; } = 0;
    public List<PVRCloudResult> Result { get; set; } = new List<PVRCloudResult>();

    public bool HasError
    {
        get
        {
            return Result.Any(a =>
                 a.Status == PVRCloudResult.PVRCloudResultStatus.VALIDATIONERROR ||
                 a.Status == PVRCloudResult.PVRCloudResultStatus.EXCEPTION ||
                 a.Status == PVRCloudResult.PVRCloudResultStatus.ERROR);
        }
    }
}
