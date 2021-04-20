namespace DotInsideNode
{
    public class VarSetter : ComNodeBase
    {
        //Component
        TextTB m_TextTitleBar = new TextTB("Set");
        ExecIC m_ExecIC = new ExecIC();
        ExecOC m_ExecOC = new ExecOC();
        VarIC m_VarIC;
        VarOC m_VarOC;

        //Variable
        IVar m_Var;

        public virtual IVar BaseVar => m_Var;

        public VarSetter(IVar variable)
        {
            Assert.IsNotNull(variable);
            m_Var = variable;

            AddBaseComponet();
        }

        public void AddBaseComponet()
        {
            m_VarIC = new VarIC(BaseVar);
            m_VarOC = new VarOC(BaseVar, false);

            AddComponet(m_TextTitleBar);
            AddComponet(m_ExecIC);
            AddComponet(m_ExecOC);
            AddComponet(m_VarIC);
            AddComponet(m_VarOC);
        }

        public void OnVarChage(IVar variable)
        {
            m_Var = variable;
            NotifyVarChange();
        }

        //Event
        public override void EventProc(EEvent eEvent)
        {
            switch(eEvent)
            {
                case EEvent.Selected:
                    VarManager.Instance.SelectVar(BaseVar.ID);
                    break;
            }

            DefEventProc(eEvent);
        }

        protected void NotifyVarChange()
        {
            m_VarIC.ChageVar(BaseVar);
            m_VarOC.ChageVar(BaseVar);
        }

        public void OnVarTypeChange()
        {

        }
    }

    public class VarGetter : ComNodeBase
    {
        //Component
        VarOC m_VarOC;

        //Variable
        IVar m_Var;

        public virtual IVar BaseVar => m_Var;

        public VarGetter(IVar variable)
        {
            Assert.IsNotNull(variable);
            m_Var = variable;

            AddBaseComponet();
        }

        public void AddBaseComponet()
        {
            m_VarOC = new VarOC(BaseVar);
            AddComponet(m_VarOC);
        }        

        public void OnVarChage(IVar variable)
        {
            m_Var = variable;
            NotifyVarChange();
        }

        System.DateTime m_PrevSelectTime = System.DateTime.Now;
        //Event
        public override void EventProc(EEvent eEvent)
        {
            System.DateTime timeNow = System.DateTime.Now;
            System.TimeSpan span = timeNow - m_PrevSelectTime;
            m_PrevSelectTime = System.DateTime.Now;

            switch (eEvent)
            {

                case EEvent.Selected:
                    if (span.TotalMilliseconds > 200)
                    {
                        Logger.Info("Select");
                        VarManager.Instance.SelectVar(BaseVar.ID);
                    }
                    break;
                case EEvent.Clicked:
                    VarManager.Instance.SelectVar(BaseVar.ID);
                    break;
            }

            DefEventProc(eEvent);
        }

        protected void NotifyVarChange()
        {
            m_VarOC.ChageVar(BaseVar);
        }

        public void OnVarTypeChange()
        {
            m_VarOC.OnVarTypeChange();
        }
    }
}
