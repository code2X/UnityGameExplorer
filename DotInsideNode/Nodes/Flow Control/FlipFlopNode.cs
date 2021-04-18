namespace DotInsideNode
{
    [EditorNode("Flip Flop")]
    class FlipFlopNode : FlowControlNode
    {
        ExecOC m_ExecOC_A = new ExecOC();
        ExecOC m_ExecOC_B = new ExecOC();
        BoolOC m_IsA_OC = new BoolOC();

        bool m_IsA = true;

        public FlipFlopNode()
        {
            m_TextTitleBar.Title = "Flip Flop";
            m_ExecOC_A.Text = "A".PadLeft(9);
            m_ExecOC_B.Text = "B".PadLeft(10);
            m_IsA_OC.Text = "Is A".PadLeft(10);

            AddComponet(m_ExecOC_A);         
            AddComponet(m_ExecOC_B);
            AddComponet(m_IsA_OC);
        }

        protected override object ExecNode(int callerID, params object[] objects)
        {
            object res;
            m_IsA_OC.Object = m_IsA;
            if (m_IsA)
            {
                res = m_ExecOC_A.Play();
            }
            else
            {
                res = m_ExecOC_B.Play();
            }

            m_IsA = !m_IsA;

            return res;
        }

    }

}
