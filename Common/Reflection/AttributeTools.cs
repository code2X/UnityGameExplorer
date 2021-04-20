using System;
using System.Collections.Generic;

class AttributeTools
{
    public static Dictionary<Type, Attribute> GetNamespaceCustomAttributes(Type attributeType)
    {
        Dictionary<Type, Attribute> type2attr = new Dictionary<Type, Attribute>();

        Type[] namespaceTypes = Assembler.GetNamespaceTypes();
        foreach (Type type in namespaceTypes)
        {
            Attribute attr = Attribute.GetCustomAttribute(type, attributeType);
            if (attr != null)
            {
                type2attr.Add(type, attr);
            }
        }

        return type2attr;
    }

    public static List<T> GetCustomAttributeClassList<T>(Type attributeType)
    {
        List<T> classList = new List<T>();
        var attrList = GetNamespaceCustomAttributes(attributeType);
        foreach (var pair in attrList)
        {
            Caller.Try(() =>
            {
                Type type = pair.Key;

                T containerClass = (T)ClassTools.CallDefaultConstructor(type);
                classList.Add(containerClass);
            });
        }

        return classList;
    }
}
