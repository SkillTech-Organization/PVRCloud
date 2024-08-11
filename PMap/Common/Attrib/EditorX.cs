using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.Common.Attrib
{

    [AttributeUsage(AttributeTargets.All)]
    public class EditorX : Attribute
    {
        private Type m_editorType;
        private object[] m_parameterlist;

        public EditorX(Type p_editorType, params object[] p_parameterlist)
        {
            m_editorType = p_editorType;
            m_parameterlist = p_parameterlist;
        }
        public Type EditorType
        {
            get { return m_editorType; }
        }
        public object[] Parameterlist
        {
            get { return m_parameterlist; }
        }
    }

}
