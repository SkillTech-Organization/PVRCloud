using PMapCore.Common.Attrib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PMapCore.BO.DataXChange
{
    public class boXChkRes
    {
        public enum ResultStatus
        {
            [Description("OK")]
            OK,
            [Description("VALIDATIONERROR")]
            VALIDATIONERROR,
            [Description("EXCEPTION")]
            EXCEPTION,
            [Description("ERROR")]
            ERROR
        };

        [DisplayNameAttributeX(Name = "Eredmény státusza", Order = 1)]
        public ResultStatus Status { get; set; }

        [DisplayNameAttributeX(Name = "Hibaüzenet", Order = 2)]
        public string ErrMsg { get; set; }

        [DisplayNameAttributeX(Name = "Kezdőpont térképi hosszúsági koordináta (lat)", Order = 3)]
        public double fromLat { get; set; }

        [DisplayNameAttributeX(Name = "Kezdőpont térképi szélességi koordináta (lng)", Order = 4)]
        public double fromLng { get; set; }

        [DisplayNameAttributeX(Name = "Végpont térképi hosszúsági koordináta (lat)", Order = 5)]
        public double toLat { get; set; }

        [DisplayNameAttributeX(Name = "Végpont térképi szélességi koordináta (lng)", Order = 6)]
        public double toLng { get; set; }

        [DisplayNameAttributeX(Name = "Útvonal", Order = 7)]
        public string Route { get; set; }
    }
}
