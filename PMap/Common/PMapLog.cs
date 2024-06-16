using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using PMapCore.Common.Azure;
using PMapCore.Common.Attrib;

namespace PMapCore.Common
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class PMapLog : AzureTableObjBase
    {
        /*****************/
        /* partition key */
        /*****************/

        public PMapLog() { m_ID = Guid.NewGuid(); DateTimeKind = DateTimeKind.Local; }

        public PMapLog ShallowCopy()
        {
            return (PMapLog)this.MemberwiseClone();
        }

        private Guid m_ID;
        [DataMember]
        [AzurePartitionAttr]
        [AzureTablePartitionKeyAttr]
        public Guid ID
        {
            get { return m_ID; }
            set { m_ID = value; NotifyPropertyChanged("ID"); }
        }

        [DataMember]
        [AzurePartitionAttr]
        [DisplayNameAttributeX(Name = "Alkalmazás példány", Order = 1, NoPrefix = true)]
        public string AppInstance { get; set; }


        [DataMember]
        [AzurePartitionAttr]
        [DisplayNameAttributeX(Name = "Bejegyzés típusa", Order = 2, NoPrefix = true)]
        public string Type { get; set; }

        [DataMember]
        [AzurePartitionAttr]
        [DisplayNameAttributeX(Name = "Szöveg", Order = 3, NoPrefix = true)]
        public string Text { get; set; }

        [DataMember]
        [AzurePartitionAttr]
        [DisplayNameAttributeX(Name = "Időpont", Order = 4, NoPrefix = true)]
        public string PMapTimestamp { get; set; }

        [DataMember]
        [AzurePartitionAttr]
        [DisplayNameAttributeX(Name = "Érték", Order = 5, NoPrefix = true)]
        public string Value { get; set; }
    }
}
