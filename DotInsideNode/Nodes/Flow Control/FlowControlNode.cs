using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotInsideNode
{
    class FlowControlNode: ComNodeBase
    {
        protected static uint m_TitleBarColor = StyleManager.GetU32Color(160, 160, 160);

        protected TextTB m_TextTitleBar = new TextTB("");
        protected ExecIC m_ExecIC = new ExecIC();

        public FlowControlNode()
        {
            m_ExecIC.Text = "";
            AddComponet(m_TextTitleBar);
            AddComponet(m_ExecIC);

            Style.AddStyle(StyleManager.StyleType.TitleBar, m_TitleBarColor);
        }
    }
}
