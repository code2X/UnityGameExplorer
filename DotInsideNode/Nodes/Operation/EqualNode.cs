using imnodesNET;
using System;
using System.Reflection;

namespace DotInsideNode
{
    [EditorNode("Equal")]
    class EqualNode : OperationNoExecNode
    {
        static uint m_PinColor = StyleManager.GetU32Color(255, 255, 255);

        public EqualNode()
        {
            m_TextTitleBar.Title = "==";
            AddBaseComponet();
            Style.AddStyle(ColorStyle.Pin, m_PinColor);    
        }

        protected bool DoEqual()
        {
            object left = m_ObjectIC_Left.Object;
            object right = m_ObjectIC_Right.Object;
            if (left == null)
            {
                Logger.Info("EqualNode left is null");
            }

            bool res = left.Equals(right);
            Logger.Info("EqualNode:" + res);

            m_ObjectOC.Object = null;

            return res;
        }

        public override object Request(ERequest type)
        {
            switch (type)
            {
                case ERequest.InstanceType:
                    return typeof(bool);
                case ERequest.InstanceObject:
                    return DoEqual();
            }
            throw new RequestTypeError(type);
        }
    }


    [EditorNode("Not Equal")]
    class NotEqualNode : EqualNode
    {
        public NotEqualNode()
        {
            m_TextTitleBar.Title = "!=";
        }

        public override object Request(ERequest type)
        {
            switch (type)
            {
                case ERequest.InstanceType:
                    return typeof(bool);
                case ERequest.InstanceObject:
                    return !DoEqual();
            }
            throw new RequestTypeError(type);
        }
    }


}
