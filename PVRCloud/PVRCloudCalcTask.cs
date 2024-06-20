﻿namespace PVRCloud;

[Serializable]
public class PVRCloudCalcTask
{
    public PVRCloudCalcTask()
    {
        CalcTours = new List<PVRCloudCalcTour>();
    }

    public PVRCloudTask Task { get; set; }
    public List<PVRCloudCalcTour> CalcTours { get; set; }


}
