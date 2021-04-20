using imnodesNET;
using System;
using System.Reflection;

namespace DotInsideNode
{
    abstract class ArithmeticNode : OperationNoExecNode
    {
        protected object DoArithmetic()
        {
            object left = m_ObjectIC_Left.Object;
            object right = m_ObjectIC_Right.Object;

            var context = new OperationContext(GetStrategy());
            return context.DoOperation(left, right);
        }
        
        protected override object ExecNode(int callerID, params object[] objects)
        {
            object left = m_ObjectIC_Left.Object;
            object right = m_ObjectIC_Right.Object;

            var context = new OperationContext(GetStrategy());
            m_ObjectOC.Object = context.DoOperation(left, right);

            return null;
        }

        public override object Request(ERequest type)
        {
            switch (type)
            {
                case ERequest.InstanceType:
                    return typeof(object);
                case ERequest.InstanceObject:
                    return DoArithmetic();
            }
            throw new RequestTypeError(type);
        }

        protected abstract OperationStrategy GetStrategy();
    }

    [EditorNode("object - object")]
    class SubStractionNode : ArithmeticNode
    {
        static OperationStrategy m_Strategy = new OperationSubtract();

        public SubStractionNode()
        {
            m_TextTitleBar.Title = "-";
            AddBaseComponet();
        }

        protected override OperationStrategy GetStrategy() => m_Strategy;
    }

    [EditorNode("object + object")]
    class AdditionNode : ArithmeticNode
    {
        static OperationStrategy m_Strategy = new OperationAddition();

        public AdditionNode()
        {
            m_TextTitleBar.Title = "+";
            AddBaseComponet();
        }

        protected override OperationStrategy GetStrategy() => m_Strategy;
    }

    [EditorNode("object × object")]
    class MutiplyNode : ArithmeticNode
    {
        static OperationStrategy m_Strategy = new OperationMultiply();

        public MutiplyNode()
        {
            m_TextTitleBar.Title = "×";
            AddBaseComponet();
        }

        protected override OperationStrategy GetStrategy() => m_Strategy;
    }

    [EditorNode("object / object")]
    class DivisionNode : ArithmeticNode
    {
        static OperationStrategy m_Strategy = new OperationDivision();

        public DivisionNode()
        {
            m_TextTitleBar.Title = "/";
            AddBaseComponet();
        }

        protected override OperationStrategy GetStrategy() => m_Strategy;
    }

    [EditorNode("object % object")]
    class ModuloNode : ArithmeticNode
    {
        static OperationStrategy m_Strategy = new OperationModulo();

        public ModuloNode()
        {
            m_TextTitleBar.Title = "%";
            AddBaseComponet();
        }

        protected override OperationStrategy GetStrategy() => m_Strategy;
    }

}
