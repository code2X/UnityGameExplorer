using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{

    [AttributeUsage(AttributeTargets.Class)]
    class ABluePrintType : Attribute
    { }

    public abstract class diType
    {
        public static List<diType> TypeClassList = new List<diType>();

        public static void InitClassList()
        {
            if (TypeClassList.Count != 0)
                return;

            var attrList = AttributeTools.GetNamespaceCustomAttributes(typeof(ABluePrintType));
            foreach (var pair in attrList)
            {
                Caller.Try(() =>
                {
                    Type type = pair.Key;

                    diType @var = (diType)ClassTools.CallDefaultConstructor(type);
                    TypeClassList.Add(@var);
                    Logger.Info("BluePrint Type: " + type);
                });
            }
        }

        public abstract Type ValueType
        {
            get;
        }
        public abstract object NewObject
        {
            get;
        }
        public abstract object Draw(ref object obj);
        public abstract object Draw(ref object obj, string label);
    }

    abstract class TType<T> : diType
    {
        public override Type ValueType
        {
            get => typeof(T);
        }

        public override object Draw(ref object obj)
        {
            T tmp = (T)obj;
            obj = Draw(ref tmp,"");
            return tmp;
        }

        public override object Draw(ref object obj, string label)
        {
            T tmp = (T)obj;
            obj = Draw(ref tmp,"##" + ValueType + obj + label);
            return tmp;
        }

        public abstract T Draw(ref T obj,string label);
    }

    [ABluePrintType]
    class BoolType : TType<bool>
    {
        public override bool Draw(ref bool obj, string label)
        {
            ImGui.Checkbox(label, ref obj);
            return obj;
        }

        public override object NewObject
        {
            get => false;
        }
    }

    [ABluePrintType]
    class IntType : TType<int>
    {
        public override int Draw(ref int obj, string label)
        {
            ImGui.InputInt(label, ref obj);
            return obj;
        }
        public override object NewObject
        {
            get => 0;
        }
    }

    [ABluePrintType]
    class FloatType : TType<float>
    {
        public override float Draw(ref float obj, string label)
        {
            ImGui.InputFloat(label, ref obj);
            return obj;
        }
        public override object NewObject
        {
            get => 0.0f;
        }
    }

    [ABluePrintType]
    class DoubleType : TType<double>
    {
        public override double Draw(ref double obj, string label)
        {
            ImGui.InputDouble(label, ref obj);
            return obj;
        }
        public override object NewObject
        {
            get => 0.0;
        }
    }

    [ABluePrintType]
    class StringType : TType<string>
    {
        public override string Draw(ref string obj, string label)
        {
            ImGui.InputText(label, ref obj,1000);
            return obj;
        }
        public override object NewObject
        {
            get => "";
        }
    }

    [ABluePrintType]
    class Vector3Type : TType<Vector3>
    {
        public override object NewObject
        {
            get => new Vector3();
        }

        public override Vector3 Draw(ref Vector3 obj, string label)
        {
            ImGui.InputFloat3(label, ref obj);
            return obj;
        }
    }


}
