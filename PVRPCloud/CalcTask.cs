namespace PVRPCloud;

[Serializable]
public class CalcTask
{
    public CalcTask()
    {
        CalcTours = new List<CalcTour>();
    }

    public PVRPCloudTask Task { get; set; }
    public List<CalcTour> CalcTours { get; set; }


}
