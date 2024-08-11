using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.Common.Attrib;
using PMapCore.Common;

namespace PMapCore.BO
{
    [Serializable]
    public class boEtoll
    {
        [DisplayNameAttributeX("ID")]
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int ID { get; set; }

        [DisplayNameAttributeX("Járműkategória")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int ETL_ETOLLCAT { get; set; }

        [DisplayNameAttributeX("Jármű emissziós kategória")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int ETL_ENGINEEURO { get; set; }

        [DisplayNameAttributeX("Gyorsforgalmi km díj")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ETL_TOLL_SPEEDWAY { get; set; }

        [DisplayNameAttributeX("Főút km díj")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ETL_TOLL_ROAD { get; set; }

        [DisplayNameAttributeX("Zajártalmi költség külvárosi")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ETL_NOISE_CITY { get; set; }

        [DisplayNameAttributeX("Zajártalmi költség telepüésközi")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ETL_NOISE_OUTER { get; set; }

        [DisplayNameAttributeX("Co2 költség")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ETL_CO2 { get; set; }
        public DateTime LASTDATE { get; set; }

    }
}
