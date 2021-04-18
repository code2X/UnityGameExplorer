using System.Collections.Generic;
using System.Reflection;
using ImGuiNET;

namespace DotInsideNode
{
    class VarListDrawer : TListDrawer<VarBase>
    {
        class RenameAction
        {
            public delegate void Action(VarBase variable, string new_name);
            VarBase m_RenameVar = null;
            public event Action OnAction;

            void DrawPopupMenuItem(VarBase @var)
            {
                if (ImGui.Selectable("Rename"))
                {
                    m_RenameVar = @var;
                }
            }

            void DoNotifyAction()
            {
                if (m_RenameVar != null)
                {
                    //OnRename?.Invoke(m_RenameVar);
                }
            }
        }

        public VarListDrawer(ref Dictionary<string, VarBase> name2vars)
            :
            base(ref name2vars)
        { }

        void DrawTooltip(VarBase @var)
        {
            if (var.Tooltip != string.Empty &&
                ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(var.Tooltip);
            }
        }

        protected override void DrawListItem(string name, VarBase tObj)
        {
            base.DrawListItem(name, tObj);
            DrawTooltip(tObj);
        }

        protected override string GetPayloadType() => "VAR_DRAG";
    }

    class FunctionListDrawer : TListDrawer<FunctionBase>
    {
        public FunctionListDrawer(ref Dictionary<string, FunctionBase> name2funcs)
        :
        base(ref name2funcs)
        { }

        void DrawTooltip(FunctionBase function)
        {
            if (function.Description != string.Empty &&
                ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(function.Description);
            }
        }

        protected override void DrawListItem(string name, FunctionBase tObj)
        {
            base.DrawListItem(name, tObj);
            DrawTooltip(tObj);
        }

        protected override string GetPayloadType() => "FUNCTION_DRAG";
    }
}
