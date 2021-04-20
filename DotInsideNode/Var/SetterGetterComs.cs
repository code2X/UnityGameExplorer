using ImGuiNET;

namespace DotInsideNode
{
    [SingleConnect]
    class VarIC : INodeInput
    {
        ComObject m_Object = new ComObject();
        IVar m_Var;
        INodeOutput m_Connect = null;

        public VarIC(IVar variable)
        {
            m_Var = variable;
        }

        public ComObject GetObject() => m_Object;

        protected override void DrawContent()
        {
            ImGui.TextUnformatted(m_Var.Name);
        }

        public override object Request(ERequest type)
        {
            switch (type)
            {
                case ERequest.InstanceType:
                    return m_Object.Type;
            }
            throw new RequestTypeError(type, m_Connect);
        }

        //Event
        public override bool TryConnectBy(INodeOutput component)
        {
            m_Connect = component;
            return true;
        }

        public void ChageVar(IVar variable)
        {
            m_Var = variable;
        }

        public void OnVarTypeChange()
        {

        }

    }

    class VarOC : INodeOutput
    {
        ComObject m_Object = new ComObject();
        IVar m_Var;
        bool m_ShowName;
        INodeInput m_Connect = new NullIC();

        public ComObject GetObject() => m_Object;

        public VarOC(IVar variable,bool show_name = true)
        {
            m_ShowName = show_name;
            m_Var = variable;
        }

        protected override void DrawContent()
        {
            if(m_ShowName)
                ImGui.TextUnformatted(m_Var.Name);
        }

        public override object Request(ERequest type)
        {
            switch (type)
            {
                case ERequest.InstanceType:
                    return m_Var.VarType;
                case ERequest.InstanceObject:
                    return m_Var.VarValue;
            }
            throw new RequestTypeError(type, m_Connect);
        }

        //Event
        public override bool TryConnectTo(INodeInput component)
        {
            m_Connect = component;
            return true;
        }

        public void ChageVar(IVar variable)
        {
            m_Var = variable;
            OnVarTypeChange();
        }

        public void OnVarTypeChange()
        {
            m_Connect.SendMessage(EMessage.InstanceTypeChange);
        }

    }

}
