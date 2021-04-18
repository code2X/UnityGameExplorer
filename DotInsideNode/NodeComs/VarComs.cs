using ImGuiNET;

namespace DotInsideNode
{
    [SingleConnect]
    class VarIC : INodeInput
    {
        ComObject m_Object = new ComObject();
        VarBase m_Var;
        INodeOutput m_Connect = null;

        public VarIC(VarBase variable)
        {
            m_Var = variable;
        }

        public ComObject GetObject() => m_Object;

        protected override void DrawContent()
        {
            ImGui.TextUnformatted(m_Var.Name);
        }

        public override bool TryConnectBy(INodeOutput component)
        {
            m_Connect = component;            
            return true;
        }

        public override void OnLinkDropped()
        {
            
        }

        public void ChageVar(VarBase variable)
        {
            m_Var = variable;
        }

        public override object Request(RequestType type)
        {
            switch (type)
            {
                case RequestType.InstanceType:
                    return m_Object.Type;
            }
            throw new RequestTypeError(type, m_Connect);
        }
    }

    class VarOC : INodeOutput
    {
        ComObject m_Object = new ComObject();
        VarBase m_Var;
        bool m_ShowName;
        INodeInput m_Connect = new NullIC();

        public VarOC(VarBase variable,bool show_name = true)
        {
            m_ShowName = show_name;
            m_Var = variable;
        }

        public ComObject GetObject() => m_Object;

        protected override void DrawContent()
        {
            if(m_ShowName)
                ImGui.TextUnformatted(m_Var.Name);
        }

        public override bool TryConnectTo(INodeInput component)
        {
            m_Connect = component;
            return true;
        }

        public void ChageVar(VarBase variable)
        {
            m_Var = variable;
            m_Connect.SendMessage(MessageType.InstanceTypeChange);
        }

        public override object Request(RequestType type)
        {
            switch (type)
            {
                case RequestType.InstanceType:
                    return m_Var.VarType;
                case RequestType.InstanceObject:
                    return m_Var.VarValue;
            }
            throw new RequestTypeError(type, m_Connect);
        }
    }

}
