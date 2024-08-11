using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.Common.Attrib;

namespace PMapCore.BO
{
    [Serializable]
    public class boSpeedProf
    {
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int ID { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true, FieldName="SPP_NAME1")]
        public string SPP_NAME { get; set; }

        [WriteFieldAttribute(Insert = false, Update = false)]
        public int SPEED1 { get; set; }
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int SPEED2 { get; set; }
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int SPEED3 { get; set; }
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int SPEED4 { get; set; }
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int SPEED5 { get; set; }
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int SPEED6 { get; set; }
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int SPEED7 { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public bool SPP_DELETED { get; set; }
        [WriteFieldAttribute(Insert = false, Update = true)]
        public DateTime LASTDATE { get; set; }
    }
}
