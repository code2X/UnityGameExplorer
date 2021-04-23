using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotInsideNode
{
    class diObjectTB: TextTB
    {
        dnObject m_Object = null;

        public diObjectTB(dnObject obj)
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
