using System.Collections.Generic;

namespace DotInsideNode
{
    class FunctionReturnNode : ComNodeBase
    {
        static uint m_TitleBarColor = StyleManager.GetU32Color(148, 0, 211);
        TextTB m_TextTitleBar = new TextTB("Return Node");
        ExecIC m_ExecIC = new ExecIC();

        IFunction m_Function = null;

        List<ParamIC> m_InputParams = new List<ParamIC>();

        public FunctionReturnNode(INodeGraph bp, IFunction function) : base(bp)
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
            //Fill out params
            object[] outParams = new object[m_InputParams.Count];
            for (int i = 0; i < m_InputParams.Count; ++i)
            {
                outParams[i] = m_InputParams[i].Object;
            }
            Assert.IsNotNull(m_Function);
            Assert.IsNotNull(m_Function.GetGraph());

            m_Function.GetGraph().ExecuteReturn(outParams);

            //Out params event
            return outParams;
        }
    }
}
