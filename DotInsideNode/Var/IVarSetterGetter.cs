using System.Collections.Generic;
using ImGuiNET;

namespace DotInsideNode
{
    public class VarSetter : ComNodeBase
    {
        VarManager m_Manager = VarManager.Instance;

        TextTB m_TextTitleBar = new TextTB("Set");
        ExecIC m_ExecIC = new ExecIC();
        ExecOC m_ExecOC = new ExecOC();
        VarIC m_VarIC;
        VarOC m_VarOC;

        VarBase m_Var;

        public void AddBaseComponet()
        {
            m_VarIC = new VarIC(GetBaseVar());
            m_VarOC = new VarOC(GetBaseVar(), false);

            AddComponet(m_TextTitleBar);
            AddComponet(m_ExecIC);
            AddComponet(m_ExecOC);
            AddComponet(m_VarIC);
            AddComponet(m_VarOC);
        }

        //public abstract VarBase GetBaseVar();

        public override void OnNodeSelected()
        {
            m_Manager.SelectVar(GetBaseVar().ID);
        }

        public override void OnNodeHovered()
        {
        }

        protected void OnVarChange()
        {
            m_VarIC.ChageVar(GetBaseVar());
            m_VarOC.ChageVar(GetBaseVar());
        }

        public VarSetter(VarBase variable)
        {
            Assert.IsNotNull(variable);
            m_Var = variable;

            AddBaseComponet();
        }

        public virtual VarBase GetBaseVar()
        {
            return m_Var;
        }

        public void ChageVar(VarBase variable)
        {
            m_Var = variable;
            OnVarChange();
        }
    }

    public class VarGetter : ComNodeBase
    {
        VarManager m_Manager = VarManager.Instance;
        VarOC m_VarOC;

        VarBase m_Var;

        public void AddBaseComponet()
        {
            m_VarOC = new VarOC(GetBaseVar());
            AddComponet(m_VarOC);
        }

        public override void OnNodeSelected()
        {
            m_Manager.SelectVar(GetBaseVar().ID);
        }

        public override void OnNodeHovered()
        {
        }

        protected void OnVarChange()
        {
            m_VarOC.ChageVar(GetBaseVar());
        }

        public VarGetter(VarBase variable)
        {
            Assert.IsNotNull(variable);
            m_Var = variable;

            AddBaseComponet();
        }

        public virtual VarBase GetBaseVar()
        {
            return m_Var;
        }

        public void ChageVar(VarBase variable)
        {
            m_Var = variable;
            OnVarChange();
        }
    }
}
