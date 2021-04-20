using System.Collections.Generic;

namespace DotInsideNode
{
    class ExecFlowNode: ComNodeBase
    {
        protected ExecIC m_ExecIC = new ExecIC();
        protected ExecOC m_ExecOC = new ExecOC();

        public void AddExecComponent()
        {
            AddComponet(m_ExecIC);
            AddComponet(m_ExecOC);
        }
    }

    class FunctionCallNode : ExecFlowNode
    {
        diObjectTB m_TextTitleBar = null;

        IFunction m_Function = null;
        List<ParamIC> m_InputParams = new List<ParamIC>();
        List<ParamOC> m_OutputParams = new List<ParamOC>();

        public FunctionCallNode(IFunction function)
        {
            m_TextTitleBar = new diObjectTB(function);

            m_Function = function;
            m_TextTitleBar.Title = function.Name;
            m_ExecIC.Text = "";
            m_ExecOC.Text = "";

            AddComponet(m_TextTitleBar);
            AddExecComponent();

            function.InputParams.ExecuteForEachParam(OnAddInputParam);
            function.OutputParams.ExecuteForEachParam(OnAddOutputParam);
        }

        public void OnAddInputParam(IParam param)
        {
            ParamIC paramIC = new ParamIC(param);
            m_InputParams.Add(paramIC);
            AddComponet(paramIC);
        }

        public void OnAddOutputParam(IParam param)
        {
            ParamOC paramOC = new ParamOC(param);
            m_OutputParams.Add(paramOC);
            AddComponet(paramOC);
        }
    }
}
