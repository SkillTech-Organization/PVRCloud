using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMapCore.BLL.Base;
using PMapCore.Common.Attrib;

namespace PMapCore.BO
{
    [Serializable]
    public class boCarrier
    {
        [DisplayNameAttributeX("ID")]
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int ID { get; set; }

        [DisplayNameAttributeX("Szállítókód")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string CRR_CODE { get; set; }

        [DisplayNameAttributeX("Szállítónév")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string CRR_NAME { get; set; }


        //a többi mezőt majd később kezeljük

        [DisplayNameAttributeX("Törölt?")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public bool CRR_DELETED { get; set; }

        [DisplayNameAttributeX("Utolsó módosítás")]
        [WriteFieldAttribute(Insert = false, Update = true)]
        public DateTime LASTDATE { get; set; }

    }
}
