using System.Collections.Generic;

namespace DotInsideNode
{
    class FunctionReturnNode : ComNodeBase
    {
        static uint m_TitleBarColor = StyleManager.GetU32Color(148, 0, 211);
        TextTB m_TextTitleBar = new TextTB("Return Node");
        ExecIC m_ExecIC = new ExecIC();

        IFunction m_Function;

        List<ParamIC> m_InputParams = new List<ParamIC>();

        public FunctionReturnNode(IFunction function)
        {
            m_Function = function;

            AddComponet(m_TextTitleBar);
            AddComponet(m_ExecIC);

            Style.AddStyle(StyleManager.StyleType.TitleBar, m_TitleBarColor);
        }

        public void OnAddInputParam(IParam param)
        {
            ParamIC paramOC = new ParamIC(param);
            m_InputParams.Add(paramOC);
            AddComponet(paramOC);
        }

        protected override object ExecNode(int callerID, params object[] objects)
        {
            return null;
        }
    }
}
