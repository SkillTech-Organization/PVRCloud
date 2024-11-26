using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PMapCore.BO
{

    [Serializable]
    public class boRoute
    {
        public boRoute()
        {
            Edges = new List<boEdge>();
            Route = new MapRoute("");
        }

        public int ID { get; set; }
        [JsonPropertyName("NF")]
        public int NOD_ID_FROM { get; set; }
        [JsonPropertyName("NT")]
        public int NOD_ID_TO { get; set; }
        [JsonPropertyName("RZN")]
        public string RZN_ID_LIST { get; set; }
        [JsonPropertyName("WE")]
        public int DST_MAXWEIGHT { get; set; }
        [JsonPropertyName("HE")]
        public int DST_MAXHEIGHT { get; set; }
        [JsonPropertyName("WD")]
        public int DST_MAXWIDTH { get; set; }
        [JsonPropertyName("DST")]
        public int DST_DISTANCE { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public double CalcDistance                  //lehet, hogy nem hasznaljuk
        {
            get
            {
                if (Edges != null)
                    return Edges.Sum(e => e.EDG_LENGTH);
                else
                    return 0;
            }
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public MapRoute Route { get; set; }                 //Az útvonal GPS kordinátákkal

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public List<boEdge> Edges { get; set; }             //Az útvonal élekkel 

        [JsonPropertyName("EK")]
        public List<string> EdgeKeys { get; set; } = null;      //Az útvonal élek dictionary key-ek. Ezt fogjuk szerializálni
    }
}
