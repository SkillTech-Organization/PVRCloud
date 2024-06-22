using PMapCore.Common.Attrib;

namespace PVRPCloud;

//Eredmény útvonalak
[Serializable]
public class PVRPCloudCalcRoute
{

    public PVRPCloudCalcRoute()
    {
        TPoint = null;
        Arrival = DateTime.MinValue;
        Departure = DateTime.MinValue;
        Completed = false;
        PMapRoute = null;
        DrivingDuration = 0;
        RestDuration = 0;
        WaitingDuration = 0;
        SrvDuration = 0;
        Distance = 0;
        Toll = 0;
        Current = false;
        RoutePoints = "";
    }

    [DisplayNameAttributeX(Name = "Túrapont", Order = 1)]
    public PVRPCloudPoint TPoint { get; set; }

    [DisplayNameAttributeX(Name = "Érkezés", Order = 2)]
    public DateTime Arrival { get; set; }

    [DisplayNameAttributeX(Name = "Indulás", Order = 3)]
    public DateTime Departure { get; set; }

    [DisplayNameAttributeX(Name = "Teljesítve", Order = 4)]
    public bool Completed { get; set; }

    [DisplayNameAttributeX(Name = "Vezetés időtartama", Order = 5)]
    public int DrivingDuration { get; set; }

    [DisplayNameAttributeX(Name = "Pihenőidő", Order = 6)]
    public int RestDuration { get; set; }

    [DisplayNameAttributeX(Name = "Várakozási idő", Order = 7)]
    public int WaitingDuration { get; set; }

    [DisplayNameAttributeX(Name = "Kiszolgálási idő", Order = 8)]
    public int SrvDuration { get; set; }

    [DisplayNameAttributeX(Name = "Távolság (m)", Order = 9)]
    public double Distance { get; set; }

    [DisplayNameAttributeX(Name = "Útdíj", Order = 10)]
    public double Toll { get; set; }

    [DisplayNameAttributeX(Name = "Aktuális pont?", Order = 11)]
    public bool Current { get; set; }

    [DisplayNameAttributeX(Name = "Útvonal pontok", Order = 12)]
    public string RoutePoints { get; set; }

    /* munkamező */
    internal PVRPCloudPMapRoute PMapRoute { get; set; }
    internal bool ErrDriveTime { get; set; } = false;

}
