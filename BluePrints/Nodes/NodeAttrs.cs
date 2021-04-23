using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotInsideNode
{
    [AttributeUsage(AttributeTargets.Class)]
    class EditorNode : System.Attribute
    {
        string m_Text;

        public string Text
        {
            get => m_Text;
        }

        public EditorNode(string text)
        {
            m_Text = text;
        }
    }
}
