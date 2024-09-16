using PMapCore.BO;
using System.Text.Json.Serialization;

namespace PVRPCloud;

[Serializable]
public class PMapRoute : IEquatable<PMapRoute>
{
    public class PVRPCloudToll
    {
        public int ETollCat { get; set; }                          //A díjszámításnál használandó járműkategória.
        public int EngineEuro { get; set; }                        //Jármű motor EURO kategória
        public double Toll { get; set; }                           //Útdíj
    }

    public int fromNOD_ID { get; init; }
    public int toNOD_ID { get; init; }
    public string RZN_ID_LIST { get; init; } = string.Empty;

    public string TruckTypeId { get; init; } = string.Empty;
    public int GVWR { get; init; }
    public int Height { get; init; }
    public int Width { get; init; }

    public boRoute? route { get; set; } = null;

    [JsonIgnore]
    public string NODEList
    {
        get
        {
            return fromNOD_ID.ToString() + "," + string.Join(",", route?.Edges.Select(x => x.NOD_ID_TO.ToString()).ToArray() ?? []);
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        return Equals(obj as PMapRoute);
    }

    public bool Equals(PMapRoute? other)
    {
        return other is not null &&
            fromNOD_ID == other.fromNOD_ID &&
            toNOD_ID == other.toNOD_ID &&
            RZN_ID_LIST == other.RZN_ID_LIST &&
            GVWR == other.GVWR &&
            Height == other.Height &&
            Width == other.Width;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(fromNOD_ID);
        hash.Add(toNOD_ID);
        hash.Add(RZN_ID_LIST);
        hash.Add(GVWR);
        hash.Add(Height);
        hash.Add(Width);

        return hash.ToHashCode();
    }

}
