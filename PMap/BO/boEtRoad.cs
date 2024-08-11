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

    public class boEtRoad
    {
        [DisplayNameAttributeX("ID")]
        [WriteFieldAttribute(Insert = false, Update = false)]
        public int ID { get; set; }

        [DisplayNameAttributeX("Útszelvény")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string ETR_CODE { get; set; }

        [DisplayNameAttributeX("Úttípus")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ETR_ROADTYPE { get; set; }                //1=gyorsforgalmi, 2=főút

        [DisplayNameAttributeX("Hossz (m)")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ETR_LEN_M { get; set; }

        /// <summary>
        /// https://cdn.kormany.hu/uploads/document/a/a5/a5b/a5b82b2fb1beb796f4ff52eef99fcbb1565c668b.pdf
        /// Az adott elemi útszakaszban a külvárosi útszakaszok hosszaránya. (Például ha egy elemi útszakasz teljes
        /// hosszában települési összekötő területjellegű, akkor az ahhoz tartozó külsőköltségdíj-tényező 0, ha 50%-ban
        /// külvárosi, 50%-ban pedig települési összekötő területjellegű, akkor a külsőköltségdíj-tényező 0,50, ha pedig
        /// teljes hosszában külvárosi területjellegű, akkor a külsőköltségdíj-tényező 1.
        /// 
        /// </summary>
        [DisplayNameAttributeX("külsőköltségdíj-tényező")]
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double ETR_COSTFACTOR { get; set; }
        public DateTime LASTDATE { get; set; }

    }
}
