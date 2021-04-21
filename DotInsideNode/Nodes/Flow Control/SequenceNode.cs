using System.Collections.Generic;
using ImGuiNET;

namespace DotInsideNode
{
    [EditorNode("Sequence")]
    class SequenceNode : FlowControlNode
    {
        protected static uint m_Color = StyleManager.GetU32Color(255, 255, 255);
        static string m_OutPinBaseName = "Then ";
        static string m_AddPinText = "Add pin +";

        int m_OutPinIndex = 0;     
        List<ExecOC> m_ExecOC_List = new List<ExecOC>();

        public SequenceNode(INodeGraph bp) : base(bp)
        {
            m_TextTitleBar.Title = "Sequence";

            m_ExecOC_List.Add(NewOutPin());
            m_ExecOC_List.Add(NewOutPin());

            foreach(ExecOC oc in m_ExecOC_List)
            {
                AddComponet(oc);
            }

            Style.AddStyle(StyleManager.StyleType.Pin, m_Color);
            Style.AddStyle(StyleManager.StyleType.Link, m_Color);
        }

        protected override void DrawContent()
        {
            if(ImGui.Button(m_AddPinText + "##" + ID))
            {
                ExecOC newPin = NewOutPin();
                m_ExecOC_List.Add(newPin);
                AddComponet(newPin);
            }
        }

        public ExecOC NewOutPin()
        {
            ExecOC newExecOC = new ExecOC();
            newExecOC.Text = m_OutPinBaseName + m_OutPinIndex;
            if(m_OutPinIndex == 0)
                newExecOC.Text = newExecOC.Text.PadLeft(9);
            else
                newExecOC.Text = newExecOC.Text.PadLeft(10);
            ++m_OutPinIndex;
            return newExecOC;
        }

        protected override object ExecNode(int callerID, params object[] objects)
        {
            object res = null;
            foreach(ExecOC oc in m_ExecOC_List)
            {
                res = oc.Play();
            }
            return res;
        }

    }
}
