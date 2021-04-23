using imnodesNET;
using ImGuiNET;
using System.Collections.Generic;

namespace DotInsideNode
{
    class NodeEditor: NodeEditorBase
    {
        public override string WindowName => "NodeEditorTest";
        //MethodEntryNode m_EntryNode = new MethodEntryNode();
        //ReturnNode m_ReturnNode = null;

        List<string> m_SelectList = new List<string>();
        SortedDictionary<string, System.Type> m_SelectDict = new SortedDictionary<string, System.Type>();
        PopupSelectList m_PopupSelectList = PopupSelectList.GetInstance();
        PopupVarGetSet m_PopupVarGetSet = PopupVarGetSet.Instance;

        public NodeEditor() 
        {
            InitBluePrintNodeClasses(m_SelectDict);
            OnDropEvent += new DropAction(OnVarDragDrop);
            ShowWindow();
        }


        void InitBluePrintNodeClasses(SortedDictionary<string, System.Type> nodeDict)
        {
            if (nodeDict.Count != 0)
                return;
            var attrList = AttributeTools.GetNamespaceCustomAttributes(typeof(EditorNode));
            foreach (var pair in attrList)
            {
                EditorNode node = (EditorNode)pair.Value;
                nodeDict.Add(node.Text, pair.Key);
                Logger.Info("Editor Node: " + pair.Key);
            }
        }

        protected override void DrawContent()
        {
        }

        public void CreateMethod()
        {
            //m_ReturnNode = new ReturnNode();
            //AddNode(m_EntryNode,false);
            //AddNode(m_ReturnNode, false);
            //m_LinkManager.TryCreateLink(m_EntryNode.GetExecOutCom(), m_ReturnNode.GetExecInCom());
        }

        public string Compile()
        {
            //return m_EntryNode.Compile();
            return "";
        }

        public void Play()
        {
            NodeLogger.ExceStart();
            //m_EntryNode.Play(0);
        }

        protected override void DoEnd()
        {
            OnRightMenu();
            //OnVarDragDrop();
            //OnFunctionDragDrop();
            //DragDropProc();
            m_PopupVarGetSet.Draw();
        }

        void OnRightMenu()
        {
            //Select List Menu
            ImGui.OpenPopupOnItemClick(m_PopupSelectList.GetPopupID());
            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                m_PopupSelectList.Reset(m_SelectDict, OnRightMenuSelected);
            }
            m_PopupSelectList.DrawTypeDict();
        }

        unsafe void OnVarDragDrop(INodeGraph bp)
        {
            ImGuiPayloadPtr pPayload = ImGui.AcceptDragDropPayload("VAR_DRAG");
            if (pPayload.NativePtr == null)
                return;

            int varID = *(int*)pPayload.Data;
            m_PopupVarGetSet.Show(varID, OnSelectVarGetSet);
            Logger.Info("Var Drop. ID:" + varID);
        }

        void OnSelectVarGetSet(IVar variable, PopupVarGetSet.MenuType selectType)
        {
            Logger.Info("Var Menu Select:" + selectType);
            if (NodeGraph == null)
                return;

            switch (selectType)
            {
                case PopupVarGetSet.MenuType.Get:
                    AddNode(variable.GetNewGetter(NodeGraph));
                    break;
                case PopupVarGetSet.MenuType.Set:
                    AddNode(variable.GetNewSetter(NodeGraph));
                    break;
            }
        }

        void OnRightMenuSelected(string selected,int index)
        {
            Logger.Info("Right Menu Select. Name:" + selected + ", ID:"+ index);
            if (NodeGraph == null)
                return;

            System.Type type = m_SelectDict[selected];
            INode nodeView = (INode)ClassTools.CallConstructor(type, NodeGraph);
            AddNode(nodeView);
        }

    }
}
