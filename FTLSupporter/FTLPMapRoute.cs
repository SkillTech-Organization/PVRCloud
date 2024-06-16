using PMapCore.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTLSupporter
{
    [Serializable]
    internal class FTLPMapRoute 
    {
        public class FTLToll
        {
            public int ETollCat { get; set; }                          //A díjszámításnál használandó járműkategória. 
            public int EngineEuro { get; set; }                        //Jármű motor EURO kategória
            public double Toll { get; set; }                           //Útdíj
        }

        public FTLPMapRoute()
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

        public override bool Equals(Object obj)
        {
            if (obj == null) return false;
            FTLPMapRoute rk = (FTLPMapRoute)obj;
            return (this.fromNOD_ID == rk.fromNOD_ID && this.toNOD_ID == rk.toNOD_ID &&
                this.RZN_ID_LIST == rk.RZN_ID_LIST && this.GVWR == rk.GVWR && this.Height == rk.Height && this.Width == rk.Width );
        }
        public override int GetHashCode()
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}", this.fromNOD_ID, this.toNOD_ID, this.RZN_ID_LIST, this.GVWR, this.Height, this.Width).GetHashCode();
        }


    }
}
