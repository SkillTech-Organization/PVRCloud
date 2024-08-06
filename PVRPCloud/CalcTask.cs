namespace PVRPCloud;

[Serializable]
public class CalcTask
{
    public CalcTask()
    {
        CalcTours = new List<PVRPCloudCalcTour>();
    }

    public PVRPCloudTask Task { get; set; }
    public List<PVRPCloudCalcTour> CalcTours { get; set; }


}
