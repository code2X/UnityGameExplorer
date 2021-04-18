using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class ClassTools
{
    public static object CallDefaultConstructor(System.Type type)
    {
        var constructor = type.GetConstructor(System.Type.EmptyTypes);
        Assert.IsNotNull(constructor);
        return constructor.Invoke(null);
    }
}

