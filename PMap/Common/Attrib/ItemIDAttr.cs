using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.Common.Attrib
{
    //ezzel az attributummal annotáljuk az azonosító mezőket
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ItemIDAttr : Attribute
    {
    }
}
