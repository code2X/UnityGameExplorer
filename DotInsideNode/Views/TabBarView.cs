using ImGuiNET;
using System.Collections.Generic;

namespace DotInsideNode
{
    class TabBarView
    {
        protected Dictionary<int, diObject> m_ID2Objs = new Dictionary<int, diObject>();
        protected Dictionary<int, bool> m_ID2OpenState = new Dictionary<int, bool>();

        //Event
        public delegate void TabAction(diObject obj);
        public event TabAction OnTabOpen;
        public event TabAction OnTabClose;
        public event TabAction OnTabRemove;

        int GetNewVarID()
        {
            int index = 0;
            while (m_ID2Objs.ContainsKey(index))
            {
                ++index;
            }

            return index;
        }

        public void OpenTab(diObject obj)
        {
            foreach(var pair in m_ID2Objs)
            {
                if (pair.Value == obj)
                    return;
            }

            OpenNewTab(obj);
        }

        public void OpenNewTab(diObject obj)
        {
            int id = GetNewVarID();
            m_ID2Objs.Add(id, obj);
            m_ID2OpenState.Add(id, true);
        }

        int? m_PrevSelectID = null;

        public void Draw()
        {
            if (m_ID2Objs.Count == 0)
                return;

            if (ImGui.BeginTabBar("##TabBarViewTopTab",ImGuiTabBarFlags.AutoSelectNewTabs | ImGuiTabBarFlags.Reorderable))
            {
                foreach (var pair in m_ID2Objs)
                {
                    bool opened = m_ID2OpenState[pair.Key];
                    if (opened && ImGui.BeginTabItem(pair.Value.Name + "##" + pair.Key,ref opened))
                    {
                        ProcessOpenTab(pair.Key,pair.Value);
                        ImGui.EndTabItem();
                    }
                    m_ID2OpenState[pair.Key] = opened;
                }
                ImGui.EndTabBar();

                ProcessTabState();
            }
        }

        void ProcessTabState()
        {
            //Process tab whether closed
            foreach (var pair in m_ID2OpenState)
            {
                if (pair.Value == false)
                {
                    if (TryRemoveTab(pair.Key))
                    {
                        break;
                    }
                }
            }
        }


        bool ContainTab(int id)
        {
            return m_ID2Objs.ContainsKey(id) && m_ID2OpenState.ContainsKey(id);
        }

        bool TryRemoveTab(int id)
        {
            if(ContainTab(id))
            {
                OnTabRemove?.Invoke(m_ID2Objs[id]);
                return m_ID2Objs.Remove(id) && m_ID2OpenState.Remove(id);
            }
            return false;
        }

/// <summary>
/// Open Tab Event
/// </summary>
        void ProcessOpenTab(int id,diObject obj)
        {
            if(id != m_PrevSelectID)
            {
                if(m_PrevSelectID != null)
                    NotifyCloseTab(m_PrevSelectID.Value, m_ID2Objs[m_PrevSelectID.Value]);
                NotifyOpenTab(id, obj);               
            }
            m_PrevSelectID = id;
        }

        void NotifyOpenTab(int id, diObject obj)
        {
            Logger.Info("Open Tab Actoin:" + obj.Name);
            OnTabOpen?.Invoke(obj);
        }

/// <summary>
/// Close Tab Event
/// </summary>

        void NotifyCloseTab(int id, diObject obj)
        {
            Logger.Info("Close Tab Actoin:" + obj.Name);
            OnTabClose?.Invoke(obj);
        }
    }

}
