using imnodesNET;
using ImGuiNET;
using System.Collections.Generic;

namespace DotInsideNode
{
    class NodeEditor: NodeEditorBase
    {
        public override string GetWindowName() => "NodeEditorTest";
        MethodEntryNode m_EntryNode = new MethodEntryNode();
        //ReturnNode m_ReturnNode = null;

        List<string> m_SelectList = new List<string>();
        SortedDictionary<string, System.Type> m_SelectDict = new SortedDictionary<string, System.Type>();
        PopupSelectList m_PopupSelectList = PopupSelectList.GetInstance();
        PopupVarGetSet m_PopupVarGetSet = PopupVarGetSet.Instance;

        public NodeEditor() 
        {
            InitBluePrintNodeClasses(m_SelectDict);

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
            AddNode(m_EntryNode,false);
            //AddNode(m_ReturnNode, false);
            //m_LinkManager.TryCreateLink(m_EntryNode.GetExecOutCom(), m_ReturnNode.GetExecInCom());
        }

        public string Compile()
        {
            return m_EntryNode.Compile();
        }

        public void Play()
        {
            NodeLogger.ExceStart();
            m_EntryNode.Play(0);
        }

        protected override void DoEnd()
        {
            OnRightMenu();
            //OnVarDragDrop();
            //OnFunctionDragDrop();
            OnDragDrop();
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

        unsafe void OnDragDrop()
        {
            if (ImGui.BeginDragDropTarget())
            {
                ImGuiPayloadPtr pPayload = ImGui.AcceptDragDropPayload("VAR_DRAG");
                if (pPayload.NativePtr != null)
                {
                    OnVarDragDrop(pPayload);
                }
                pPayload = ImGui.AcceptDragDropPayload("FUNCTION_DRAG");
                if (pPayload.NativePtr != null)
                {
                    OnFunctionDragDrop(pPayload);
                }
            }
            m_PopupVarGetSet.Draw();
        }

        unsafe void OnVarDragDrop(ImGuiPayloadPtr pPayload)
        {
            int varID = *(int*)pPayload.Data;
            m_PopupVarGetSet.Show(varID, OnSelectVarGetSet);
            Logger.Info("Var Drop. ID:" + varID);
        }

        unsafe void OnFunctionDragDrop(ImGuiPayloadPtr pPayload)
        {
            int fucntionID = *(int*)pPayload.Data;
            CreateFunctionCallNode(fucntionID);
        }

        void CreateFunctionCallNode(int functionID)
        {
            IFunction function = FunctionManager.Instance.GetFunctionByID(functionID);
            Assert.IsNotNull(function);
            AddNode(function.GetNewFunctionCall());
        }

        void OnSelectVarGetSet(IVar variable, PopupVarGetSet.MenuType select_type)
        {
            Logger.Info("Var Menu Select:" + select_type);

            switch(select_type)
            {
                case PopupVarGetSet.MenuType.Get:
                    AddNode(variable.GetNewGetter());
                    break;
                case PopupVarGetSet.MenuType.Set:
                    AddNode(variable.GetNewSetter());
                    break;
            }
        }

        void OnRightMenuSelected(string selected,int index)
        {
            Logger.Info("Right Menu Select. Name:" + selected + ", ID:"+ index);

            System.Type type = m_SelectDict[selected];
            INode nodeView = (INode)ClassTools.CallDefaultConstructor(type);
            AddNode(nodeView);
        }

    }
}
