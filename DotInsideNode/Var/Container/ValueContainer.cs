using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    [AValueContainer]
    class ValueContainer : ContainerBase
    {
        object m_Value;

        public ValueContainer()
        {
            m_Value = m_ValueType.NewObject;
        }

        public override object Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        public override EContainer ContainerType
        {
            get => EContainer.Value;
        }
        public override diType ValueType
        {
            get => m_ValueType;
            set
            {
                if (m_ValueType != value)
                {
                    m_ValueType = value;
                    m_Value = m_ValueType.NewObject;
                }
            }
        }

        public override void DrawContainerValue()
        {
            m_ValueType?.Draw(ref m_Value);
        }

        public override IContainer DuplicateContainer()
        {
            return new ValueContainer();
        }

        public override object DuplicateContainerValue()
        {
            throw new NotImplementedException();
        }
    }
}
