using System.Collections.Generic;

namespace DotInsideNode
{
    class FunctionEntryNode : ComNodeBase
    {
        static uint m_TitleBarColor = StyleManager.GetU32Color(148, 0, 211);
        diObjectTB m_TextTitleBar = null;
        ExecOC m_ExecOC = new ExecOC();

        IFunction m_Function;

        public override bool Removable => false;
        List<ParamOC> m_OutputParams = new List<ParamOC>();

        public FunctionEntryNode(INodeGraph ng, IFunction function) : base(ng)
        {           
            m_Function = function;
            m_ExecOC.Text = "";
            m_TextTitleBar = new diObjectTB(function);

            AddComponet(m_TextTitleBar);
            AddComponet(m_ExecOC);

            Style.AddStyle(StyleManager.StyleType.TitleBar, m_TitleBarColor);

            function.OutputParams.ExecuteForEachParam(OnAddOutputParam);
        }

        public void OnAddOutputParam(IParam param)
        {
            ParamOC paramOC = new ParamOC(param);
            m_OutputParams.Add(paramOC);
            AddComponet(paramOC);
        }

        protected override object ExecNode(int callerID, params object[] inParams)
        {
            //Fill input params into out component
            if(inParams != null)
            {
                Assert.IsTrue(inParams.Length <= m_OutputParams.Count);
                for (int i = 0; i < inParams.Length; ++i)
                {
                    m_OutputParams[i].Object = inParams[i];
                }
            }

            object res = m_ExecOC.Play(callerID, null);

            ClearExecLocalState();
            return res;
        }

        public void ClearExecLocalState()
        {
            foreach(ParamOC param in m_OutputParams)
            {
                param.Object = null;
            }
        }
    }
}
