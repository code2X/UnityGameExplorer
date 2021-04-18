using ImGuiNET;

namespace DotInsideNode
{
    class ConvertNode<T>: ComNodeBase
    {
        protected ObjectIC m_ObjectIC = new ObjectIC();
        protected ObjectOC m_ObjectOC = new ObjectOC();

        public ConvertNode()
        {
            AddBaseComponet();
        }

        public void AddBaseComponet()
        {
            AddComponet(m_ObjectIC);
            AddComponet(m_ObjectOC);
            m_ObjectOC.ObjectType = typeof(T);
        }


    }

    [EditorNode("To Int")]
    class ToIntNode : ConvertNode<int>
    {
        protected override void DrawContent() 
        {
            ImGui.Text("To Int");
        }

        public override object Request(RequestType type)
        {
            
            switch (type)
            {
                case RequestType.InstanceType:
                    return typeof(int);
                case RequestType.InstanceObject:
                    return System.Convert.ToInt32(m_ObjectIC.Object);
            }
            throw new RequestTypeError(type);
        }
    }

    [EditorNode("To Float")]
    class ToFloatNode : ConvertNode<int>
    {
        protected override void DrawContent()
        {
            ImGui.Text("To Float");
        }

        public override object Request(RequestType type)
        {

            switch (type)
            {
                case RequestType.InstanceType:
                    return typeof(int);
                case RequestType.InstanceObject:
                    return System.Convert.ToSingle(m_ObjectIC.Object);
            }
            throw new RequestTypeError(type);
        }
    }

    [EditorNode("To Double")]
    class ToDoubleNode : ConvertNode<int>
    {
        protected override void DrawContent()
        {
            ImGui.Text("To Double");
        }

        public override object Request(RequestType type)
        {

            switch (type)
            {
                case RequestType.InstanceType:
                    return typeof(int);
                case RequestType.InstanceObject:
                    return System.Convert.ToDouble(m_ObjectIC.Object);
            }
            throw new RequestTypeError(type);
        }
    }
}
