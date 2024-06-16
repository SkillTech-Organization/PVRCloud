using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PMapCore.Common;
using System.Reflection;

namespace PMapCore.BO.Base
{
    public class boXBase
    {

        public void fillFromObject (object p_objSrc)
        {

            PropertyInfo[] thisProps = this.GetType().GetProperties();
            foreach( PropertyInfo prop in thisProps)
            {
                var srcProp = p_objSrc.GetType().GetProperty( prop.Name);
                if( srcProp != null)
                {
                    prop.SetValue(this, srcProp.GetValue(p_objSrc, null), null);
                }
            }
        }
    }
}
