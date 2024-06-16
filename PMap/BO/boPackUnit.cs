using PMapCore.Common.Attrib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMapCore.BO
{
    [Serializable]
    public class boPackUnit
    {
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int ID { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public string PCU_NAME1 { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public string PCU_NAME2 { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public string PCU_NAME3 { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public double PCU_EXCVALUE { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public bool PCU_DELETED { get; set; }

        [WriteFieldAttribute(Insert = false, Update = true)]
        public DateTime LASTDATE { get; set; }

    }
}
