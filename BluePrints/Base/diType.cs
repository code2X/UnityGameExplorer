using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    [AttributeUsage(AttributeTargets.Class)]
    class AdiType : Attribute {}

    public abstract class diType
    {
        //All diType Class Instance
        public static List<diType> TypeClassList = new List<diType>();

        public static void InitClassList()
        {
            if (TypeClassList.Count != 0)
                return;

            TypeClassList = AttributeTools.GetCustomAttributeClassList<diType>(typeof(AdiType));
            foreach (var container in TypeClassList)
                Logger.Info("BluePrint Type: " + container);
        }

        public abstract Type ValueType
        {
            get;
        }

        //Create a new object of value type
        public abstract object NewTypeObject
        {
            get;
        }

        //Draw object as value type
        public abstract object Draw(ref object obj);
        public abstract object Draw(ref object obj, string label);
    }

}
