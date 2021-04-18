using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotInsideNode
{

    [AttributeUsage(AttributeTargets.Class)]
    class SingleConnect:System.Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Class)]
    class ConnectTypes : System.Attribute
    {
        HashSet<Type> m_Types = new HashSet<Type>();

        public ConnectTypes(params Type[] types)
        {
            foreach(Type type in types)
            {
                m_Types.Add(type);
            }
        }

        public bool Contains(Type type)
        {
            return m_Types.Contains(type);
        }
    }
}
