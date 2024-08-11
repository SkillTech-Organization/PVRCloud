using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.Common.Attrib
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyOrderX : Attribute
    {
        private int order;

        public PropertyOrderX(int order)
        {
            this.order = order;
        }

        public int Order
        {
            get { return this.order; }
        }
    }

 
}
