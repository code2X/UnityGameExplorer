using System;
using System.Collections.Generic;

class AttributeTools
{
    public static Dictionary<Type, Attribute> GetNamespaceCustomAttributes(Type attributeType)
    {
        Dictionary<Type, Attribute> type2attr = new Dictionary<Type, Attribute>();

        Type[] namespaceTypes = Assembler.GetNamespaceTypes();
        foreach (System.Type type in namespaceTypes)
        {
            Attribute attr = System.Attribute.GetCustomAttribute(type, attributeType);
            if (attr != null)
            {
                type2attr.Add(type, attr);
            }
        }

        return type2attr;
    }
}
