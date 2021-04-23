using ImGuiNET;
using System.Collections.Generic;

namespace DotInsideNode
{
    abstract class IFunctionGraph: INodeGraph
    {
        public abstract void Execute(int callerID,object[] inParams, out object[] outParams);
        public abstract void ExecuteReturn(object[] inParams);
    }

    class FunctionGraph: IFunctionGraph
    {
        LinkManager m_LinkManager = null;
        NodeManager m_NodeManager = null;
        VarManager m_LocalVarManager = null;
        EditorNodeComponentManager m_NodeComponentManager = null;

        Dictionary<int, Vector2> m_NodePositions = null;

        public bool IsOpen { get; private set; } = false;

        ComNodeBase m_EntryPoint = null;
        //List<ComNodeBase> m_ReturnPoint = new List<ComNodeBase>();

        Function m_Function = null;

        public override LinkManager ngLinkManager => m_LinkManager;
        public override NodeManager ngNodeManager => m_NodeManager;
        public override EditorNodeComponentManager ngNodeComponentManager => m_NodeComponentManager;

        public override void Draw()
        {
            ngNodeManager.Draw();
            ngLinkManager.Draw();
        }

        public override void Update()
        {
            ngNodeManager.Update();
            ngLinkManager.Update();
        }

        public FunctionGraph(Function function)
        {
            Assert.IsNotNull(function);
            m_Function = function;
            m_Function.OnOutputParamEvent += new Function.OnputParamAction(OutputParamEventProc);

            m_LinkManager = new LinkManager();
            m_NodeManager = new NodeManager();
            m_LocalVarManager = new VarManager();
            m_NodeComponentManager = new EditorNodeComponentManager(this);
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

        public void AddEntryNode()
        {
            m_EntryPoint = m_Function.GetNewFunctionEntry(this);
            m_NodeManager.AddNode(m_EntryPoint, false);
        }

        void AddFristReturnNode()
        {
            if (m_EntryPoint == null)
                return;

            ComNodeBase firstReturnPoint = m_Function.GetNewFunctionReturn(this);
            m_NodeManager.AddNode(firstReturnPoint, false);

            if(IsOpen)
            {
                //Set Position
                Vector2 entryNodePosition = m_NodeManager.GetNodeEditorPostion(m_EntryPoint);
                Vector2 returnPointPosition = new Vector2(entryNodePosition);
                returnPointPosition.x += 200;
                m_NodeManager.SetNodeEditorPostion(firstReturnPoint, returnPointPosition);
            }
            else           
            {
                //Connect Exec
                NodeBase.TryConnectCom<ExecOC, ExecIC>(this,m_EntryPoint, firstReturnPoint);
            }
        }

        public override void OpenGraph()
        {
            NodeEditorBase.SubmitGraph(this);

            if (m_NodePositions != null)
                m_NodeManager.NodeEditorPostions = m_NodePositions;
            IsOpen = true;
        }

        public override void CloseGraph()
        {
            IsOpen = false;
            m_NodePositions = m_NodeManager.NodeEditorPostions;
        }

        public void OnFunctionDelete()
        {

        }

        public override void Execute(int callerID, object[] inParams, out object[] outParams)
        {
            outParams = null;
            m_EntryPoint?.Play(callerID, inParams);

            if (m_ExecResult != null)
                outParams = m_ExecResult;
        }

        object[] m_ExecResult = null;
        public override void ExecuteReturn(object[] inParams)
        {
            m_ExecResult = inParams;
        }
    }
}
