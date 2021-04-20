using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotInsideNode
{
    class diObjectTB: TextTB
    {
        diObject m_Object = null;

        public diObjectTB(diObject obj)
        {
            m_Object = obj;
        }

        public override string Title
        {
            get => m_Object != null?m_Object.Name:"";
            set { }
        }
    }
}
