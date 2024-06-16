using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMapCore.Common.Attrib
{
    //this attribute indicates header type fields
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AzurePartitionAttr : Attribute
    {

    }
}
