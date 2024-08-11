using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.Common.Attrib;

namespace PMapCore.BO
{
    [Serializable]
    public class boOrderType
    {
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int ID { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public string OTP_CODE { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true, FieldName = "OTP_NAME1")]
        public string OTP_NAME { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int OTP_VALUE { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public bool OTP_DELETED { get; set; }

        [WriteFieldAttribute(Insert = false, Update = true)]
        public DateTime LASTDATE { get; set; }


    }
}
