using ImGuiNET;
using System.Collections.Generic;

namespace DotInsideNode
{
    abstract class IFunctionGraph
    {
        public abstract void OpenGraph();
        public abstract void CloseGraph();
        public abstract void Execute(object[] objects);
    }

    abstract class IBluePrint
    {
        public abstract void OpenGraph();
        public abstract void CloseGraph();

        public abstract LinkManager BP_LinkManager
        {
            get;
        }
        public abstract NodeManager BP_NodeManager
        {
            get;
        }
        public abstract NodeComponentManager BP_NodeComponentManager
        {
            get;
        }
    }

    class FunctionGraph: IFunctionGraph
    {
        LinkManager m_LinkManager = null;
        NodeManager m_NodeManager = null;
        VarManager m_LocalVarManager = null;
        NodeComponentManager m_NodeComponentManager = null;
        Dictionary<int, Vector2> m_NodePositions = null;
        bool isInit = false;
        ComNodeBase m_EntryPoint = null;
        //List<ComNodeBase> m_ReturnPoint = new List<ComNodeBase>();

        Function m_Function = null;

        public FunctionGraph(Function function)
        {
            Assert.IsNotNull(function);
            m_Function = function;
            m_Function.OnOutputParamEvent += new Function.OnputParamAction(OutputParamEventProc);

            m_LinkManager = new LinkManager();
            m_NodeManager = new NodeManager();
            m_LocalVarManager = new VarManager();
            m_NodeComponentManager = new NodeComponentManager(m_LinkManager);           
        }

        public void OutputParamEventProc(Function.EOutputParamEvent eOutputParamEvent, IParam param)
        {
            switch(eOutputParamEvent)
            {
                case Function.EOutputParamEvent.AddParam:
                    if(m_Function.ReturnNodeCount == 0)
                    {
                        AddFristReturnNode();
                    }
                    break;
            }
        }

        void AddFristReturnNode()
        {
            if (m_EntryPoint == null)
                return;

            ComNodeBase returnPoint = m_Function.GetNewFunctionReturn();
            NodeManager.Instance.AddNode(returnPoint, false);
            
            //Set Position
            Vector2 entryNodePosition = NodeManager.Instance.GetNodeEditorPostion(m_EntryPoint);
            Vector2 returnPointPosition = new Vector2(entryNodePosition);
            returnPointPosition.x += 200;
            NodeManager.Instance.SetNodeEditorPostion(returnPoint, returnPointPosition);

            //Connect Exec
            NodeBase.TryConnectCom<ExecOC, ExecIC>(m_EntryPoint, returnPoint);
        }

        public override void OpenGraph()
        {
            LinkManager.Instance = m_LinkManager;
            NodeComponentManager.Instance = m_NodeComponentManager;
            NodeManager.Instance = m_NodeManager;

            if (m_NodePositions != null)
                m_NodeManager.NodeEditorPostions = m_NodePositions;

            TryInit();
        }

        public void TryInit()
        {
            if (isInit == false)
            {
                m_EntryPoint = m_Function.GetNewFunctionEntry();
                NodeManager.Instance.AddNode(m_EntryPoint, false);
                isInit = true;
            }
        }

        public override void CloseGraph()
        {
            m_NodePositions = m_NodeManager.NodeEditorPostions;
        }

        public void OnFunctionDelete()
        {

        }

        public override void Execute(params object[] inParams) 
        {
            m_EntryPoint?.Play(0, inParams);
        }
    }
}
