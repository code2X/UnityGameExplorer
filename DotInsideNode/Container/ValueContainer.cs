
namespace DotInsideNode
{
    [AValueContainer]
    class ValueContainer : ContainerBase
    {
        object m_Value;

        public ValueContainer()
        {
            m_Value = NewValueTypeObject();
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
                    m_Value = NewValueTypeObject();
                }
            }
        }

        public override void DrawContainerValue()
        {
            m_ValueType?.Draw(ref m_Value);
        }

        public override diContainer DuplicateContainer()
        {
            ValueContainer res = new ValueContainer();
            res.m_Value = DuplicateContainerValue();
            res.m_ValueType = m_ValueType;
            return res;
        }

        public override object DuplicateContainerValue()
        {
            return m_Value;
        }
    }
}
