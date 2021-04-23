using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    class VarListView : TListView<IVar>
    {
        public VarListView(diObjectManager<IVar> diObjectManager):base(diObjectManager)
        { }

        void DrawTooltip(IVar @var)
        {
            if (var.Tooltip != string.Empty &&
                ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(var.Tooltip);
            }
        }

        protected override void DrawListItem(IVar tObj, out bool onEvent)
        {
            onEvent = false;
            base.DrawListItem(tObj, out onEvent);
            DrawTooltip(tObj);
        }

        protected override string GetPayloadType() => "VAR_DRAG";
    }

    class FunctionListView : TListView<IFunction>
    {
        public FunctionListView(diObjectManager<IFunction> diObjectManager) : base(diObjectManager)
        {
            NodeEditorBase.OnDropEvent += new NodeEditorBase.DropAction(OnFunctionDragDrop);
        }

        void DrawTooltip(IFunction function)
        {
            if (function.Description != string.Empty &&
                ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(function.Description);
            }
        }

        protected override void DrawListItem(IFunction tObj, out bool onEvent)
        {
            onEvent = false;
            base.DrawListItem(tObj,out onEvent);
            DrawTooltip(tObj);
        }

        unsafe void OnFunctionDragDrop(INodeGraph bp)
        {
            ImGuiPayloadPtr pPayload = ImGui.AcceptDragDropPayload("FUNCTION_DRAG");
            if (pPayload.NativePtr != null)
            {
                int fucntionID = *(int*)pPayload.Data;
                CreateFunctionCallNode(bp,fucntionID);
            }
        }

        void CreateFunctionCallNode(INodeGraph bp,int functionID)
        {
            IFunction function = FunctionManager.Instance.GetFunctionByID(functionID);
            Assert.IsNotNull(function);
            bp.ngNodeManager.AddNode(function.GetNewFunctionCall(bp));
        }

        protected override string GetPayloadType() => "FUNCTION_DRAG";
    }
}
