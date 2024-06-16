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

        [DisplayNameAttributeX("Azonosítókód")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string ETL_CODE { get; set; }

        [DisplayNameAttributeX("Hosszúság KM")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ETL_LEN_KM { get; set; }

        [DisplayNameAttributeX("J2 útdíj KM")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ETL_J2_TOLL_KM { get; set; }

        [DisplayNameAttributeX("J3 útdíj KM")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ETL_J3_TOLL_KM { get; set; }

        [DisplayNameAttributeX("J4 útdíj KM")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ETL_J4_TOLL_KM { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]

        [DisplayNameAttributeX("J2 útdíj szakasz összesen")]
        public double ETL_J2_TOLL_FULL { get; set; }

        [DisplayNameAttributeX("J3 útdíj szakasz összesen")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ETL_J3_TOLL_FULL { get; set; }

        [DisplayNameAttributeX("J4 útdíj szakasz összesen")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ETL_J4_TOLL_FULL { get; set; }

        [DisplayNameAttributeX("Utolsó módosítás")]
        [WriteFieldAttribute(Insert = false, Update = true)]
        public DateTime LASTDATE { get; set; }

        [WriteFieldAttribute(Insert = false, Update = false)]
        public Dictionary<int, double> TollsToDict()
        {
            Dictionary<int, double> retTolls = new Dictionary<int, double>();
            retTolls.Add(Global.ETOLLCAT_J0, 0);
            retTolls.Add(Global.ETOLLCAT_J2, ETL_J2_TOLL_FULL);
            retTolls.Add(Global.ETOLLCAT_J3, ETL_J3_TOLL_FULL);
            retTolls.Add(Global.ETOLLCAT_J4, ETL_J4_TOLL_FULL);
            return retTolls;
        }

    }
}
