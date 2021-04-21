
namespace DotInsideNode
{
    class OperationNode : ComNodeBase
    {
        protected static uint m_TitleBarColor = StyleManager.GetU32Color(67,205,128);

        protected ExecIC m_ExecIC = new ExecIC();
        protected ExecOC m_ExecOC = new ExecOC();
        protected TextTB m_TextTitleBar = new TextTB("");
        protected ObjectIC m_ObjectIC_Left = new ObjectIC();
        protected ObjectIC m_ObjectIC_Right = new ObjectIC();
        protected ObjectOC m_ObjectOC = new ObjectOC();

        public OperationNode(INodeGraph bp) : base(bp)
        {
            m_ExecIC.Text = m_ExecOC.Text = "";
            m_ObjectIC_Left.Text = "Left".PadRight(10);
            m_ObjectOC.Text = "Result";
            m_ObjectIC_Right.Text = "Right".PadRight(10);

            Style.AddStyle(StyleManager.StyleType.TitleBar, m_TitleBarColor);
        }

        public void AddBaseComponet()
        {
            AddComponet(m_TextTitleBar);
            AddComponet(m_ExecIC);
            AddComponet(m_ExecOC);
            AddComponet(m_ObjectIC_Left);
            AddComponet(m_ObjectOC);
            AddComponet(m_ObjectIC_Right);
        }
    }

    class OperationNoExecNode : ComNodeBase
    {
        protected static uint m_TitleBarColor = StyleManager.GetU32Color(67, 205, 128);

        protected TextTB m_TextTitleBar = new TextTB("");
        protected ObjectIC m_ObjectIC_Left = new ObjectIC();
        protected ObjectIC m_ObjectIC_Right = new ObjectIC();
        protected ObjectOC m_ObjectOC = new ObjectOC();

        public OperationNoExecNode(INodeGraph bp) : base(bp)
        {
            m_ObjectIC_Left.Text = "Left".PadRight(8);
            m_ObjectOC.Text = "Result";
            m_ObjectIC_Right.Text = "Right".PadRight(8);

            Style.AddStyle(StyleManager.StyleType.TitleBar, m_TitleBarColor);
        }

        public void AddBaseComponet()
        {
            AddComponet(m_TextTitleBar);
            AddComponet(m_ObjectIC_Left);
            AddComponet(m_ObjectOC);
            AddComponet(m_ObjectIC_Right);
        }
    }
}
