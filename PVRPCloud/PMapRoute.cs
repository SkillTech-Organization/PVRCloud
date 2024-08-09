using PMapCore.BO;

namespace PVRPCloud;

[Serializable]
internal class PMapRoute
{
    public class PVRPCloudToll
    {
        public int ETollCat { get; set; }                          //A díjszámításnál használandó járműkategória.
        public int EngineEuro { get; set; }                        //Jármű motor EURO kategória
        public double Toll { get; set; }                           //Útdíj
    }

    public PMapRoute()
    {
        route = null;
    }
    public int fromNOD_ID { get; set; }
    public int toNOD_ID { get; set; }
    public string RZN_ID_LIST { get; set; }

    public int GVWR { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }


    public boRoute route { get; set; }

    public string NODEList
    {
        get
        {
            return fromNOD_ID.ToString() + "," + string.Join(",", route.Edges.Select(x => x.NOD_ID_TO.ToString()).ToArray());

        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        PMapRoute rk = (PMapRoute)obj;
        return fromNOD_ID == rk.fromNOD_ID && toNOD_ID == rk.toNOD_ID &&
            RZN_ID_LIST == rk.RZN_ID_LIST && GVWR == rk.GVWR && Height == rk.Height && Width == rk.Width;
    }
    public override int GetHashCode()
    {
        return string.Format("{0}_{1}_{2}_{3}_{4}_{5}", fromNOD_ID, toNOD_ID, RZN_ID_LIST, GVWR, Height, Width).GetHashCode();
    }


}
