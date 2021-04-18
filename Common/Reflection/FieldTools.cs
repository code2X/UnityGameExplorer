using System;
using System.Collections.Generic;
using System.Reflection;

class FieldTools
{
    public static FieldInfo[] GetAllField(Type type)
    {
        return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
    }

    public static FieldInfo[] GetStaticField(Type type)
    {
        return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
    }
}
