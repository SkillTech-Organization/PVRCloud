using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.Common.Attrib;

namespace PMapCore.BO
{
    [Serializable]
    public class boCargoType
    {
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int ID { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public string CTP_CODE { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true, FieldName = "CTP_NAME1")]
        public string CTP_NAME { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int CTP_VALUE { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public bool CTP_DELETED { get; set; }

        [WriteFieldAttribute(Insert = false, Update = true)]
        public DateTime LASTDATE { get; set; }

    }
}
