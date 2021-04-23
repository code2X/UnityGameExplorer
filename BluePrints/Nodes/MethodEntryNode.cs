
namespace DotInsideNode
{
    class MethodEntryNode : ComNodeBase
    {
        string m_Name = "Method";
        TextTB m_TextTitleBar = new TextTB("Method Entry");
        ExecOC m_ExecOC = new ExecOC();
        static uint m_TitleBarColor = StyleManager.GetU32Color(148, 0, 211);
        //static uint m_PinColor = StyleManager.GetU32Color(255, 255, 255);

        public override INodeTitleBar GetTitleBarCom() => m_TextTitleBar;
        public override ExecOC GetExecOutCom() => m_ExecOC;

        public MethodEntryNode(INodeGraph bp) : base(bp)
        {
            AddComponet(m_TextTitleBar);
            AddComponet(m_ExecOC);

            Style.AddStyle(StyleManager.StyleType.TitleBar, m_TitleBarColor);
            //Style.AddStyle(ColorStyle.Pin, m_PinColor);
        }

        public override string Compile()
        {
            string res = "private void " + m_Name + "() \n{\n";
            res += m_ExecOC.Compile();
            res += "}\n";
            return res;
        }

        protected override object ExecNode(int callerID, params object[] objects)
        {
            return m_ExecOC.Play();
        }

        protected override void DrawContent()
        {

        }
    }
}
