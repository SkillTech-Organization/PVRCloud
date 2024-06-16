using PMapCore.Common.Attrib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.BO.DataXChange
{
    public class boXChkRoute
    {
        [DisplayNameAttributeX(Name = "Indulás hosszúsági koordináta (lat)", Order = 1)]
        [ErrorIfConstAttrX(EvalMode.IsSmallerOrEqual, 0, "Kötelező mező:Lat")]
        public double FromLat { get; set; }

        [DisplayNameAttributeX(Name = "Indulás szélességi koordináta (lng)", Order = 2)]
        [ErrorIfConstAttrX(EvalMode.IsSmallerOrEqual, 0, "Kötelező mező:Lng")]
        public double FromLng { get; set; }

        [DisplayNameAttributeX(Name = "Érkezés hosszúsági koordináta (lat)", Order = 3)]
        [ErrorIfConstAttrX(EvalMode.IsSmallerOrEqual, 0, "Kötelező mező:Lat")]
        public double ToLat { get; set; }

        [DisplayNameAttributeX(Name = "Érkezés szélességi koordináta (lng)", Order = 4)]
        [ErrorIfConstAttrX(EvalMode.IsSmallerOrEqual, 0, "Kötelező mező:Lng")]
        public double ToLng { get; set; }


        [DisplayNameAttributeX(Name = "Behajtási övezetek kódok", Order = 5)]
        public string RZones { get; set; }


    }
}
