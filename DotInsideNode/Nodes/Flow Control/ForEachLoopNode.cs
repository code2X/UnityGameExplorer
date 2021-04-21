using System;
using imnodesNET;

namespace DotInsideNode
{
    [EditorNode("For Each Loop")]
    class ForEachLoopNode : FlowControlNode
    {
        ExecOC m_ExecOC_LoopBody = new ExecOC();
        ExecOC m_ExecOC_Completed = new ExecOC();
        ObjectIC m_Array = new ObjectIC();
        ObjectOC m_ArrayElement = new ObjectOC();
        ObjectOC m_ArrayIndex = new ObjectOC();

        public ForEachLoopNode(INodeGraph bp) : base(bp)
        {
            m_TextTitleBar.Title = "For Each Loop";
            m_ExecIC.Text = "".PadRight(12);
            m_ExecOC_LoopBody.Text = "Loop Body".PadLeft(12);
            m_Array.Text = "Array".PadRight(12);
            m_ArrayElement.Text = "Array Element".PadLeft(12);
            m_ArrayIndex.Text = "Array Index".PadLeft(24);
            m_ExecOC_Completed.Text = "Completed".PadLeft(24);

            AddComponet(m_ExecOC_LoopBody);
            AddComponet(m_Array);
            AddComponet(m_ArrayElement);
            AddComponet(m_ArrayIndex);
            AddComponet(m_ExecOC_Completed);

            Reset();
        }

        void Reset()
        {
            m_ArrayIndex.Object = (int)0;
        }

        protected override object ExecNode(int callerID, params object[] objects)
        {
            Array array = (Array)m_Array.Object;
            for(int i = 0; i< array.Length;++i)
            {
                m_ArrayElement.Object = array.GetValue(i);
                m_ArrayIndex.Object = i;

                m_ExecOC_LoopBody.Play();              
            }
            m_ExecOC_Completed.Play();

            return null;
        }
    }

    [EditorNode("For Loop with Break")]
    class ForLoopWithBreakNode : FlowControlNode
    {
        ExecOC m_ExecOC_LoopBody = new ExecOC();
        ExecOC m_ExecOC_Completed = new ExecOC();
        ExecIC m_ExecIC_Break = new ExecIC();
        IntIC m_FristIndex = new IntIC();
        ObjectOC m_Index = new ObjectOC();
        IntIC m_LastIndex = new IntIC();

        public ForLoopWithBreakNode(INodeGraph bp) : base(bp)
        {
            m_TextTitleBar.Title = "For Loop with Break";
            m_ExecOC_LoopBody.Text = "Loop Body".PadLeft(35);
            m_ExecOC_Completed.Text = "Completed".PadLeft(15);
            m_FristIndex.Text = "Frist Index";
            m_Index.Text = "Index".PadLeft(10);
            m_LastIndex.Text = "Last Index";
            m_ExecIC_Break.Text = "Break";

            AddComponet(m_ExecOC_LoopBody);
            AddComponet(m_FristIndex);
            AddComponet(m_Index);
            AddComponet(m_LastIndex);
            AddComponet(m_ExecOC_Completed);
            AddComponet(m_ExecIC_Break);
        }

        protected override object ExecNode(int callerID, params object[] objects)
        {
            int frist = m_FristIndex.Value;
            int last = m_LastIndex.Value;

            if(m_ExecIC_Break.ID != callerID)
            {
                for (; frist < last; ++frist)
                {
                    m_Index.Object = frist;

                    m_ExecOC_LoopBody.Play();
                }              
            }
            return m_ExecOC_Completed.Play();
        }
    }
}
