using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    [AttributeUsage(AttributeTargets.Class)]
    class AValueContainer : Attribute
    {}

    public abstract class diContainer
    {
        /// <summary>
        /// All IContainer Classes 
        /// </summary>
        public static List<diContainer> ContainerClassList = new List<diContainer>();

        public static void InitClassList()
        {
            if (ContainerClassList.Count != 0)
                return;

            ContainerClassList = AttributeTools.GetCustomAttributeClassList<diContainer>(typeof(AValueContainer));
            foreach(var container in ContainerClassList)
            {
                Logger.Info("BluePrint Container: " + container);
            }
        }

        public enum EContainer
        {
            Value,
            Array,
            Set,
            Dict
        }

        public abstract diType ValueType
        {
            get;set;
        }

        public abstract object Value
        {
            get;set;
        }

        public abstract EContainer ContainerType
        {
            get;
        }

        public abstract diContainer DuplicateContainer();
        public abstract object DuplicateContainerValue();
        public abstract void DrawContainerValue();
    }

    abstract class ContainerBase : diContainer
    {
        protected diType m_ValueType;

        public ContainerBase()
        {
            m_ValueType = diType.TypeClassList[0];
        }

        protected object NewValueTypeObject()
        {
            return m_ValueType.NewTypeObject;
        }
    }

}
