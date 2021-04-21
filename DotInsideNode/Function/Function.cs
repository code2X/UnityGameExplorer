using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    class Function : IFunction
    {
        IFunctionGraph m_FunctionGraph = null;
        IFunctionEditor m_BaseEditor = null;

        //Input and output params
        ParamManager m_InputParams = new ParamManager();
        ParamManager m_OutputParams = new ParamManager();

        //Instance Function Entry Node 
        List<ComNodeBase> m_EntryNodes = new List<ComNodeBase>();
        List<ComNodeBase> m_ReturnNodes = new List<ComNodeBase>();
        List<ComNodeBase> m_CallNodes = new List<ComNodeBase>();

        //Param and node interface
        public override ParamManager InputParams => m_InputParams;
        public override ParamManager OutputParams => m_OutputParams;
        protected override List<ComNodeBase> EntryNodes => m_EntryNodes;
        protected override List<ComNodeBase> ReturnNodes => m_ReturnNodes;
        protected override List<ComNodeBase> CallNodes => m_CallNodes;
        public override int EntryNodeCount => m_EntryNodes.Count;
        public override int ReturnNodeCount => m_ReturnNodes.Count;
        public override int CallNodeCount => m_CallNodes.Count;

        public Function()
        {
            FunctionGraph functionGraph = new FunctionGraph(this);
            m_FunctionGraph = functionGraph;
            functionGraph.AddEntryNode();            

            m_BaseEditor = new FunctionDefaultEditor(this);
        }

        //Function Node Interface
        public override ComNodeBase GetNewFunctionEntry(INodeGraph bluePrint)
        {
            Assert.IsNotNull(m_FunctionGraph);

            FunctionEntryNode entryNode = new FunctionEntryNode(bluePrint, this);
            m_EntryNodes.Add(entryNode);
            return entryNode;
        }

        public override ComNodeBase GetNewFunctionReturn(INodeGraph bluePrint)
        {
            Assert.IsNotNull(m_FunctionGraph);

            FunctionReturnNode returnNode = new FunctionReturnNode(bluePrint, this);
            m_ReturnNodes.Add(returnNode);
            return returnNode;
        }

        public override ComNodeBase GetNewFunctionCall(INodeGraph bluePrint)
        {
            Assert.IsNotNull(m_FunctionGraph);

            FunctionCallNode callNode = new FunctionCallNode(bluePrint, this);
            m_CallNodes.Add(callNode);
            return callNode;
        }

        public enum EInputParamEvent
        {
            AddParam,
        }

        public enum EOutputParamEvent
        {
            AddParam,
        }

        public delegate void InputParamAction(EInputParamEvent eInputParamEvent,IParam param);
        public delegate void OnputParamAction(EOutputParamEvent eOutputParamEvent,IParam param);
        public event InputParamAction OnInputParamEvent;
        public event OnputParamAction OnOutputParamEvent;

        public virtual void NotifyAddInputParam(IParam param)
        {
            OnInputParamEvent?.Invoke(EInputParamEvent.AddParam, param);

            foreach (FunctionCallNode callNode in CallNodes)
            {
                callNode.OnAddInputParam(param);
            }
            foreach (FunctionEntryNode entryNode in EntryNodes)
            {
                entryNode.OnAddOutputParam(param);
            }
        }

        public virtual void NotifyAddOutputParam(IParam param)
        {
            OnOutputParamEvent?.Invoke(EOutputParamEvent.AddParam,param);

            foreach (FunctionCallNode callNode in CallNodes)
            {
                callNode.OnAddOutputParam(param);
            }
            foreach (FunctionReturnNode returnNode in ReturnNodes)
            {
                returnNode.OnAddInputParam(param);
            }
        }

        public override void OpenGraph()
        {
            m_FunctionGraph?.OpenGraph();
        }

        public override void CloseGraph()
        {
            m_FunctionGraph?.CloseGraph();
        }

        public override void Execute(int callerID, object[] inParams, out object[] outParams)
        {
            outParams = null;
            m_FunctionGraph?.Execute(callerID, inParams, out outParams);
        }

        public override void DrawEditor()
        {
            m_BaseEditor.DrawEditor();
            //Inputs
            if (ImGui.Button("+##Inputs Create"))
            {
                Param newParam = new Param();
                InputParams.AddParam(newParam);
                NotifyAddInputParam(newParam);
                Console.WriteLine("Inputs Create");
            }
            ImGui.SameLine();
            if (ImGui.CollapsingHeader("Inputs", ImGuiTreeNodeFlags.DefaultOpen))
            {
                InputParams.DrawParamList();
            }

            //Outputs
            if (ImGui.Button("+##Outputs Create"))
            {
                Param newParam = new Param();
                OutputParams.AddParam(newParam);
                NotifyAddOutputParam(newParam);
                Console.WriteLine("Outputs Create");
            }
            ImGui.SameLine();
            if (ImGui.CollapsingHeader("Outputs", ImGuiTreeNodeFlags.DefaultOpen))
            {
                OutputParams.DrawParamList();
            }
        }

        public override IFunctionGraph GetGraph() => m_FunctionGraph;
    }
}
