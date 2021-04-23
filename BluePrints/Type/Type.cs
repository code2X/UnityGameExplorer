using ImGuiNET;
using System;

namespace DotInsideNode
{
    abstract class TType<T> : diType
    {
        public override Type ValueType
        {
            get => typeof(T);
        }

        public override object Draw(ref object obj)
        {
            T tmp = (T)obj;
            obj = Draw(ref tmp, "");
            return tmp;
        }

        public override object Draw(ref object obj, string label)
        {
            T tmp = (T)obj;
            obj = Draw(ref tmp, "##" + ValueType + obj + label);
            return tmp;
        }

        protected abstract T Draw(ref T obj, string label);
    }

    [AdiType]
    class BoolType : TType<bool>
    {
        public override object NewTypeObject => false;

        protected override bool Draw(ref bool obj, string label)
        {
            ImGui.Checkbox(label, ref obj);
            return obj;
        }       
    }

    [AdiType]
    class IntType : TType<int>
    {
        public override object NewTypeObject => 0;

        protected override int Draw(ref int obj, string label)
        {
            ImGui.InputInt(label, ref obj);
            return obj;
        }      
    }

    [AdiType]
    class FloatType : TType<float>
    {
        public override object NewTypeObject => 0.0f;

        protected override float Draw(ref float obj, string label)
        {
            ImGui.InputFloat(label, ref obj);
            return obj;
        }        
    }

    [AdiType]
    class DoubleType : TType<double>
    {
        public override object NewTypeObject => 0.0;

        protected override double Draw(ref double obj, string label)
        {
            ImGui.InputDouble(label, ref obj);
            return obj;
        }        
    }

    [AdiType]
    class StringType : TType<string>
    {
        public override object NewTypeObject => "";

        protected override string Draw(ref string obj, string label)
        {
            ImGui.InputText(label, ref obj, 1000);
            return obj;
        }       
    }

    [AdiType]
    class Vector3Type : TType<Vector3>
    {
        public override object NewTypeObject => new Vector3();

        protected override Vector3 Draw(ref Vector3 obj, string label)
        {
            ImGui.InputFloat3(label, ref obj);
            return obj;
        }
    }

}
