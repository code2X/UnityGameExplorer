namespace DotInsideNode
{
    [EditorNode("While Loop")]
    class WhileLoopNode : FlowControlNode
    {
        ExecOC m_ExecOC_LoopBody = new ExecOC();
        ExecOC m_ExecOC_Completed = new ExecOC();
        BoolIC m_Condition = new BoolIC();

        public WhileLoopNode(INodeGraph bp) : base(bp)
        {
            m_TextTitleBar.Title = "While Loop";
            m_ExecOC_LoopBody.Text = "Loop Body".PadLeft(21);
            m_ExecOC_Completed.Text = "Completed";
            m_Condition.Text = "Condition";

            AddComponet(m_ExecOC_LoopBody);
            AddComponet(m_Condition);
            AddComponet(m_ExecOC_Completed);
        }

        protected override object ExecNode(int callerID, params object[] objects)
        {
            bool condition = m_Condition.Value;
            while (condition)
            {
                m_ExecOC_LoopBody.Play();
            }

            return m_ExecOC_Completed.Play();
        }
    }
}
