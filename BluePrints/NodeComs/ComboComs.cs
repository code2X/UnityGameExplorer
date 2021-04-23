using System.Collections.Generic;
using ImGuiNET;

namespace DotInsideNode
{
    class ComboSC : INodeStatic
    {
        public delegate void SelectAction(string item,int index);
        public event SelectAction OnSelected;

        int m_CurItem;
        IList<string> m_Items;
        string m_SearchText = "";

        public ComboSC(IList<string> items = null, int current_item = 0)
        {
            m_Items = items;
            m_CurItem = current_item;
        }

        public IList<string> ItemList
        {
            set => m_Items = value;
        }
        string CurItem
        { 
            get => m_Items [m_CurItem];
        }

        protected override void DrawContent()
        {
            if (m_Items == null || m_Items.Count == 0) return;

            if (ImGui.BeginCombo(m_Items[m_CurItem], "", ImGuiComboFlags.NoPreview))
            {
                ImGui.InputTextWithHint("##select class", "search", ref m_SearchText, 20);
                for (int n = 0; n < m_Items.Count; n++)
                {
                    //seach text
                    if (m_Items[n].IndexOf(m_SearchText) == -1)
                        continue;

                    //select
                    bool is_selected = (m_CurItem == n);
                    if (ImGui.Selectable(m_Items[n], is_selected))
                        DoSelect(n);

                    // Set the initial focus when opening the combo (scrolling + keyboard navigation focus)
                    if (is_selected)
                        ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
        }

        void DoSelect(int index)
        {
            m_CurItem = index;
            OnSelected?.Invoke(CurItem, index);
        }
    }
}
