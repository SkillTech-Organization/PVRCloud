using PMapCore.Common.Attrib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMapCore.BO
{
    [Serializable]
    public class boUser
    {
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int ID { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int UST_ID { get; set; }

        [WriteFieldAttribute(Insert = false, Update = false)]
        public string UST_NAME { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public string USR_NAME { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string USR_LOGIN { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string USR_PASSWD { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string USR_PPANEL { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string USR_PGRID { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public bool USR_DELETED { get; set; }

        [WriteFieldAttribute(Insert = false, Update = true)]
        public DateTime LASTDATE { get; set; }
    }
}
