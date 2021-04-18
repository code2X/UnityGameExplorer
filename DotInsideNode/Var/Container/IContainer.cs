using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    [AttributeUsage(AttributeTargets.Class)]
    class AValueContainer : Attribute
    {}

    public abstract class IContainer
    {
        public static List<IContainer> ContainerClassList = new List<IContainer>();

        public static void InitClassList()
        {
            if (ContainerClassList.Count != 0)
                return;

            var attrList = AttributeTools.GetNamespaceCustomAttributes(typeof(AValueContainer));
            foreach (var pair in attrList)
            {
                Caller.Try(() =>
                {
                    Type type = pair.Key;

                    IContainer @var = (IContainer)ClassTools.CallDefaultConstructor(type);
                    ContainerClassList.Add(@var);
                    Logger.Info("BluePrint Type: " + type);
                });
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
            get;
            set;
        }

        public abstract object Value
        {
            get;
            set;
        }
        public abstract EContainer ContainerType
        {
            get;
        }

        public abstract IContainer DuplicateContainer();
        public abstract object DuplicateContainerValue();
        public abstract void DrawContainerValue();
    }

    abstract class ContainerBase : IContainer
    {
        protected diType m_ValueType;

        public ContainerBase()
        {
            m_ValueType = diType.TypeClassList[0];
        }
    }

}
