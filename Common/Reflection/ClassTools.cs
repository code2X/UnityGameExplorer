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

    public static object CallConstructor<T>(System.Type type,T value)
    {
        Type[] types = new Type[1];
        types[0] = typeof(T);
        object[] param = new object[1];
        param[0] = value;

        var constructor = type.GetConstructor(types);
        Assert.IsNotNull(constructor);
        return constructor.Invoke(param);
    }
}

