using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMapCore.Common.Attrib
{
    /*Csoportképző kulcs (Ha nem kell particionálni az adatokat, akkor is érdemes ezt használni a rekordazonosításhoz, mert
     gyorsabb:
     http://blog.maartenballiauw.be/post/2012/10/08/What-PartitionKey-and-RowKey-are-for-in-Windows-Azure-Table-Storage.aspx
    */

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AzureTablePartitionKeyAttr : Attribute
    {
    }
}
