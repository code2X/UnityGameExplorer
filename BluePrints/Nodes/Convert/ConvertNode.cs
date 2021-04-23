using ImGuiNET;

namespace DotInsideNode
{
    abstract class IConvertNode: ComNodeBase
    {
        public IConvertNode(INodeGraph bp) : base(bp)
        {}

        public abstract bool InputConnect(INodeOutput outCom);
        public abstract bool OutputConnect(INodeInput inCom);
    }

    abstract class ConvertNode<T>: IConvertNode
    {
        protected ObjectIC m_ObjectIC = new ObjectIC();
        protected ObjectOC m_ObjectOC = new ObjectOC();

        public ConvertNode(INodeGraph bp):base(bp)
        {
            AddBaseComponet();
        }

        protected override void DrawContent()
        {
            ImGui.Text("To " + typeof(T).Name);
        }

        public void AddBaseComponet()
        {
            AddComponet(m_ObjectIC);
            AddComponet(m_ObjectOC);
            m_ObjectOC.ObjectType = typeof(T);
        }

        public override bool InputConnect(INodeOutput outCom)
        {
            NodeGraph.ngLinkManager.TryCreateLink(outCom.ID, m_ObjectIC.ID);
            return NodeGraph.ngLinkManager.IsConnect(outCom.ID, m_ObjectIC.ID);
        }
        public override bool OutputConnect(INodeInput inCom)
        {
            NodeGraph.ngLinkManager.TryCreateLink(m_ObjectOC.ID, inCom.ID);
            return NodeGraph.ngLinkManager.IsConnect(m_ObjectOC.ID, inCom.ID);
        }
    }

    [AConvertNode(typeof(bool))]
    [EditorNode("To Bool")]
    class ToBoolNode : ConvertNode<bool>
    {
        public ToBoolNode(INodeGraph bp) : base(bp)
        { }

        public override object Request(ERequest type)
        {
            switch (type)
            {
                case ERequest.InstanceType:
                    return typeof(bool);
                case ERequest.InstanceObject:
                    return System.Convert.ToBoolean(m_ObjectIC.Object);
            }
            throw new RequestTypeError(type);
        }
    }

    [AConvertNode(typeof(int))]
    [EditorNode("To Int")]
    class ToIntNode : ConvertNode<int>
    {
        public ToIntNode(INodeGraph bp) : base(bp)
        { }

        public override object Request(ERequest type)
        {          
            switch (type)
            {
                case ERequest.InstanceType:
                    return typeof(int);
                case ERequest.InstanceObject:
                    return System.Convert.ToInt32(m_ObjectIC.Object);
            }
            throw new RequestTypeError(type);
        }
    }

    [AConvertNode(typeof(float))]
    [EditorNode("To Float")]
    class ToFloatNode : ConvertNode<float>
    {
        public ToFloatNode(INodeGraph bp) : base(bp)
        {}

        public override object Request(ERequest type)
        {
            switch (type)
            {
                case ERequest.InstanceType:
                    return typeof(int);
                case ERequest.InstanceObject:
                    return System.Convert.ToSingle(m_ObjectIC.Object);
            }
            throw new RequestTypeError(type);
        }
    }

    [AConvertNode(typeof(double))]
    [EditorNode("To Double")]
    class ToDoubleNode : ConvertNode<double>
    {
        public ToDoubleNode(INodeGraph bp) : base(bp)
        {}

        public override object Request(ERequest type)
        {
            switch (type)
            {
                case ERequest.InstanceType:
                    return typeof(int);
                case ERequest.InstanceObject:
                    return System.Convert.ToDouble(m_ObjectIC.Object);
            }
            throw new RequestTypeError(type);
        }
    }

}
