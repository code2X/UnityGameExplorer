namespace DotInsideNode
{
    [EditorNode("Branch")]
    class BranchNode : FlowControlNode
    {
        ExecOC m_ExecOC_True = new ExecOC();
        ExecOC m_ExecOC_False = new ExecOC();
        BoolIC m_Condition = new BoolIC();

        public BranchNode()
        {
            m_TextTitleBar.Title = "Branch";
            m_ExecOC_True.Text = "True".PadLeft(20);
            m_ExecOC_False.Text = "False".PadLeft(8);
            m_Condition.Text = "Condition";

            AddComponet(m_ExecOC_True);
            AddComponet(m_Condition);
            AddComponet(m_ExecOC_False);
        }

        protected override object ExecNode(int callerID, params object[] objects)
        {
            bool condition = m_Condition.Value;
            if(condition)
            {
                return m_ExecOC_True.Play();
            }
            else
            {
                return m_ExecOC_False.Play();
            }
        }

    }

}
