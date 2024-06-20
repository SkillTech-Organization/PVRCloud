using PVRCloud.Shared;

namespace PVRCloud.Response;

[Serializable]
public class FTLCalcTask
{
    public FTLCalcTask()
    {
        CalcTours = new List<FTLCalcTour>();
    }

    public FTLTask Task { get; set; }
    public List<FTLCalcTour> CalcTours { get; set; }


}
