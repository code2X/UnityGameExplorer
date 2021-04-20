using ImGuiNET;

namespace DotInsideNode
{
    class PopupVarGetSet
    {
        PopupVarGetSet() { }
        static PopupVarGetSet __instance = new PopupVarGetSet();
        public static PopupVarGetSet Instance
        {
            get
            {
                return __instance;
            }
        }

        VarManager m_VarManager = VarManager.Instance;

        string m_PopupID = "PopupVarGetSet";
        SelectAction m_Handler;

        IVar m_Var = null;
        public string GetPopupID() => m_PopupID;

        public enum MenuType
        {
            Get,
            Set
        }

        public delegate void SelectAction(IVar variable, MenuType select_type);

        public void Show(int var_id, SelectAction select_handler = null)
        {
            Reset(var_id, select_handler);

            m_Var = m_VarManager.GetVarByID(var_id);
            ImGui.OpenPopup(m_PopupID);
        }

        public void Reset(int var_id,SelectAction select_handler = null)
        {
            m_Var = m_VarManager.GetVarByID(var_id);
            m_Handler = select_handler;
        }

        public void Draw()
        {
            if (ImGui.BeginPopup(m_PopupID, ImGuiWindowFlags.AlwaysAutoResize))
            {
                DrawList();

                ImGui.EndPopup();
            }
        }

        void DrawList()
        {
            if (m_Handler == null || m_Var == null)
                return;
            string varName = m_Var.Name;

            ImGui.Text(varName);
            if(ImGui.Selectable("Get " + varName))
            {
                m_Handler(m_Var, MenuType.Get);
            }
            if (ImGui.Selectable("Set " + varName))
            {
                m_Handler(m_Var, MenuType.Set);
            }

        }

    }

}
