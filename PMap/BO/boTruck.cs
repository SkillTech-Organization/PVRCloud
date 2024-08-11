using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using PMapCore.BLL.Base;
using PMapCore.Common;
using PMapCore.Common.Attrib;
using System.ComponentModel;

namespace PMapCore.BO
{



    [Serializable]
    public class boTruck
    {

        [WriteFieldAttribute(Insert = false, Update = false)]
        public int ID { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int CRR_ID { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int SPP_ID { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int WHS_ID { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int CPP_ID { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int TFP_ID { get; set; }

        [WriteFieldAttribute(Insert = false, Update = false)]
        private string _RZN_ID_LIST;
        [WriteFieldAttribute(Insert = false, Update = false)]
        public string RZN_ID_LIST
        {
            get { return _RZN_ID_LIST; }
            set
            {
                if (value != null)
                    _RZN_ID_LIST = value;
                else
                    _RZN_ID_LIST = "";
            }
        }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int TFP_ID_INC { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int TFP_ID_OUT { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string TRK_CODE { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string TRK_REG_NUM { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public string TRK_TRAILER { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public bool TRK_ACTIVE { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public bool TRK_GPS { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public bool TRK_BACKPANEL { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public bool TRK_LOGO { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int TRK_IDLETIME { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double TRK_BUNDTIME { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public double TRK_BUNDPOINT { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public DateTime TRK_BUNDDATE { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true, FieldName="TRK_COLOR")]
        public int intTRK_COLOR 
        {
            get { return Util.ConvertColourToWindowsRGB(TRK_COLOR); }
        }

        [WriteFieldAttribute(Insert = false, Update = false)]
        public Color TRK_COLOR { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int TRK_WEIGHT { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int TRK_XHEIGHT { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int TRK_XWIDTH { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int TRK_HEIGHT { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int TRK_WIDTH { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int TRK_LENGTH { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int TRK_AXLENUM { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public int TRK_ENGINEEURO { get; set; }

        [WriteFieldAttribute(Insert = true, Update = true)]
        public int TRK_ETOLLCAT { get; set; }                       //Jármű díjkategória
                                                                    // 0 => nem kell útdíjat számolni (3,5t alatti)
                                                                    // 2 => J2
                                                                    // 3 => J3
                                                                    // 4 => J4

        [WriteFieldAttribute(Insert = true, Update = true)]
        public string TRK_COMMENT { get; set; }
        [WriteFieldAttribute(Insert = true, Update = true)]
        public bool TRK_DELETED { get; set; }

        [WriteFieldAttribute(Insert = false, Update = true)]
        public DateTime LASTDATE { get; set; }
    }
}
