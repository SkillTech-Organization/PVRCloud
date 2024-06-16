using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.Common.Attrib
{


    public class WriteFieldAttribute : Attribute
    {
        public bool Insert {get; set;}
        public bool Update { get; set; }
        public bool ID { get; set; }
        public string FieldName { get; set; }
        public object FixValue { get; set; }

        public WriteFieldAttribute()
        {
            Insert = false;
            Update = false;
            ID = false;
            FieldName = null;
            FixValue = null;
        }
        public  WriteFieldAttribute( bool p_insert, bool p_update)
        {
            Insert = p_insert;
            Update = p_update;
            ID = false;
            FieldName = null;
            FixValue = null;
        }

        public WriteFieldAttribute(bool p_insert, bool p_update, bool p_ID)
        {
            Insert = p_insert;
            Update = p_update;
            ID = p_ID;
            FieldName = null;
            FixValue = null;
        }

        public WriteFieldAttribute(bool p_insert, bool p_update, string p_FieldName)
        {
            Insert = p_insert;
            Update = p_update;
            ID = false;
            FieldName = p_FieldName;
            FixValue = null;
        }

        public WriteFieldAttribute(bool p_insert, bool p_update, bool p_ID, string p_FieldName)
        {
            Insert = p_insert;
            Update = p_update;
            ID = p_ID;
            FieldName = p_FieldName;
            FixValue = null;
        }

        public WriteFieldAttribute(bool p_insert, bool p_update, string p_FieldName, object p_fixValue)
        {
            Insert = p_insert;
            Update = p_update;
            ID = false;
            FieldName = p_FieldName;
            FixValue = p_fixValue;
        }

        public WriteFieldAttribute(bool p_insert, bool p_update, bool p_ID, string p_FieldName, object p_fixValue)
        {
            Insert = p_insert;
            Update = p_update;
            ID = p_ID;
            FieldName = p_FieldName;
            FixValue = p_fixValue;
        }
    }
}
