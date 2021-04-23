using System;
using imnodesNET;

namespace DotInsideNode
{
    [EditorNode("Print String")]
    class PrintNode : ComNodeBase
    {
        TextTB m_TextTitleBar = new TextTB("Print String");
        ExecIC m_ExecIC = new ExecIC();
        ExecOC m_ExecOC = new ExecOC();
        ObjectIC m_ObjectIC = new ObjectIC();

        public PrintNode(INodeGraph bp) : base(bp)
        {
            m_ObjectIC.Text = "Object";

            AddComponet(m_TextTitleBar);
            AddComponet(m_ExecIC);
            AddComponet(m_ExecOC);
            AddComponet(m_ObjectIC);
        }

        protected override object ExecNode(int callerID, params object[] objects)
        {
            Console.WriteLine(m_ObjectIC.Object);

            return null;
        }

    }

}
