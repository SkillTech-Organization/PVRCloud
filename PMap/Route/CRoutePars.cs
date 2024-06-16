using PMapCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.Route
{
    public class CRoutePars
    {

        private string m_RZN_ID_LIST;
        public string RZN_ID_LIST
        {
            get
            {
                return m_RZN_ID_LIST;
            }
            set
            {
                if (value != null)
                    m_RZN_ID_LIST = value;
                else
                    m_RZN_ID_LIST = "";

            }
        }

        /* ha a CT-t súly- és méretkorlátozások nélkül akarjuk használni, itt kell a korlátozásokat 0-ra venni */
        public int Weight { get; set; } = 0;
        public int Height { get; set; } = 0;
        public int Width { get; set; } = 0;

        public override string ToString()
        {
            return String.Format("RZN_ID_LIST:{0}, Weight:{1}, Height:{2}, Width:{3}", RZN_ID_LIST, Weight, Height, Width);
        }

        public override bool Equals(Object obj)
        {
            if (obj == null) return false;
            CRoutePars rp = (CRoutePars)obj;
            return (this.RZN_ID_LIST == rp.RZN_ID_LIST && this.Weight == rp.Weight &&
                this.Height == rp.Height && this.Width == rp.Width);
        }
        public override int GetHashCode()
        {
            return string.Format("{0}_{1}_{2}_{3}", this.RZN_ID_LIST, this.Weight, this.Height, this.Width).GetHashCode();
        }


        public string Hash
        {
            get
            {
                return Util.GenerateHashCode(
                   string.Format("{0}_{1}_{2}_{3}", this.RZN_ID_LIST, this.Weight, this.Height, this.Width));
            }
        }

    }

}
