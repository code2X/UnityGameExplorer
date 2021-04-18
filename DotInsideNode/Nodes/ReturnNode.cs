using imnodesNET;
using ImGuiNET;
using System.Reflection;
using System.Collections.Generic;

namespace DotInsideNode
{
    [EditorNode("Return")]
    class ReturnNode : ComNodeBase
    {
        TextTB m_TextTitleBar = new TextTB("Return");
        ExecIC m_ExecIC = new ExecIC();

        public override INodeTitleBar GetTitleBarCom() => m_TextTitleBar;
        public override ExecIC GetExecInCom() => m_ExecIC;

        public ReturnNode()
        {
            AddComponet(m_TextTitleBar);
            AddComponet(m_ExecIC);
        }

        protected override void DrawContent()
        {
        }

        public override string Compile()
        {
            return "return;\n";
        }

        protected override object ExecNode(int callerID, params object[] objects)
        {
            return null;
        }
    }

}
