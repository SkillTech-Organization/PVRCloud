namespace PVRPCloud;

[Serializable]
public class PVRPCloudCalcTask
{
    public PVRPCloudCalcTask()
    {
        CalcTours = new List<PVRPCloudCalcTour>();
    }

    public PVRPCloudTask Task { get; set; }
    public List<PVRPCloudCalcTour> CalcTours { get; set; }


}
