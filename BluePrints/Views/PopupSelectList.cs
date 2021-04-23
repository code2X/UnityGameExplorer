using ImGuiNET;
using imnodesNET;
using System;
using System.Collections.Generic;
using DotInsideLib;
using System.Reflection;

namespace DotInsideNode
{
    class PopupSelectList
    {
        PopupSelectList() { }
        static PopupSelectList instance = new PopupSelectList();
        public static PopupSelectList GetInstance() => instance;

        string m_ID = "PopupSelectList";
        string m_InputText = "";
        IList<string> m_StringList = new List<string>();
        SortedDictionary<string, System.Type> m_StringTypeDict;
        SelectAction m_Handler;

        public string GetPopupID() => m_ID;

        public delegate void SelectAction(string select,int index);

        public void Show(IList<string> strList, SelectAction selectHandler = null)
        {
            Reset(strList, selectHandler);
            ImGui.OpenPopup(m_ID);
        }

        public void Reset(IList<string> strList, SelectAction selectHandler = null)
        {
            m_Handler = selectHandler;
            m_StringList = strList;
        }

        public void Reset(SortedDictionary<string, System.Type> strTypeDict, SelectAction selectHandler = null)
        {
            m_Handler = selectHandler;
            m_StringTypeDict = strTypeDict;
        }

        public void Draw()
        {
            if (ImGui.BeginPopup(m_ID, ImGuiWindowFlags.AlwaysAutoResize))
            {
                DrawList();

                ImGui.EndPopup();
            }
        }

        public void DrawTypeDict()
        {
            if (ImGui.BeginPopup(m_ID, ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.InputTextWithHint("##seach text", "seach", ref m_InputText, 20);
                int i = 0;
                foreach (var pair in m_StringTypeDict)
                {
                    if (ImGui.Selectable(pair.Key) && m_Handler != null)
                    {
                        m_Handler(pair.Key, i);
                        ++i;
                    }
                }
                //DrawList();

                ImGui.EndPopup();
            }
        }

        void DrawList()
        {
            ImGui.InputTextWithHint("##seach text", "seach", ref m_InputText, 20);
            for(int i =0;i<m_StringList.Count;++i)
            {
                if (ImGui.Selectable(m_StringList[i]) && m_Handler != null)
                {
                    m_Handler(m_StringList[i],i);
                }
            }
        }

    }

}
