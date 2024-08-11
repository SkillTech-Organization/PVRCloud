using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PMapCore.Common.Attrib
{
    public class DisplayNameAttributeX : DisplayNameAttribute
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public bool NoPrefix { get; set; }



        public DisplayNameAttributeX()
            : base()
        {
        }

        public DisplayNameAttributeX(string p_name)
            : base(p_name)
        {
            Name = p_name;
            NoPrefix = false;
        }
        public DisplayNameAttributeX(string p_name, int p_order)
            : base(p_name)
        {
            Name = p_name;
            Order = p_order;
            NoPrefix = false;
        }

        public DisplayNameAttributeX(string p_name, int p_order, bool p_noPrefix)
            : base(p_name)
        {
            Order = p_order;
            NoPrefix = p_noPrefix;
        }
        
        
        public override string DisplayName
        {
            get
            {
                if (NoPrefix)
                    return Name;
                else
                    return Order.ToString().PadLeft(2, ' ') + "\t" + Name;
            }
        }

    }
}
