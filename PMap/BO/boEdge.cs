using GMap.NET;
using System;

namespace PMapCore.BO
{
    [Serializable]
    public class boEdge
    {
        public int ID;//; //{ get; set; }
        public int NOD_ID_FROM { get; set; }
        public int NOD_ID_TO { get; set; }
        public int RDT_VALUE { get; set; }
        // TODO boEdge méretcsökkentés miatt kiszedve     public string EDG_NAME ; //{ get; set; }
        public float EDG_LENGTH { get; set; }                       //futásidő miatt float
        public bool EDG_ONEWAY { get; set; }
        public bool EDG_DESTTRAFFIC { get; set; }
        // TODO boEdge méretcsökkentés miatt kiszedve     public string EDG_STRNUM1 ; //{ get; set; }                     //páratlan oldal számozás kezdet
        // TODO boEdge méretcsökkentés miatt kiszedve     public string EDG_STRNUM2 ; //{ get; set; }                     //páratlan oldal számozás vége
        //  TODO boEdge méretcsökkentés miatt kiszedve    public string EDG_STRNUM3 ; //{ get; set; }                     //páros oldal számozás kezdet
        //  TODO boEdge méretcsökkentés miatt kiszedve    public string EDG_STRNUM4 ; //{ get; set; }                     //páros oldal számozás vége
        public int RZN_ID { get; set; }
        public int RST_ID { get; set; }
        public string WZONE { get; set; }
        public string EDG_ETRCODE { get; set; }
        public float CalcSpeed; //{ get; set; }                      //idealizált sebességprofil sebesség (ez alapján számítjuk a leggyorsabb utat)
        public float CalcDuration; //{ get; set; }                   //menetidő (idealizált sebességprofil alapján) megj.:futásidő miatt float

        public PointLatLng fromLatLng { get; set; }                //LatLng kiemelése, hogy gyors lehessen a térképkivágás útvonalszámításnál

        public PointLatLng toLatLng { get; set; }
        public int ZIP_NUM_FROM; //{ get; set; }
        public int ZIP_NUM_TO; //{ get; set; }
        public int EDG_MAXWEIGHT { get; set; }
        public int EDG_MAXHEIGHT { get; set; }
        public int EDG_MAXWIDTH { get; set; }


    }


}
