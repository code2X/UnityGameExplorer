using ImGuiNET;
using System.Collections.Generic;

namespace DotInsideNode
{
    public class TabBarView
    {
        public class TabItem
        {
            public diObject obj = null;
            public bool state = false;

            public TabItem(diObject obj, bool state)
            {
                this.obj = obj;
                this.state = state;
            }
        }

        protected Dictionary<int, TabItem> m_ID2Item = new Dictionary<int, TabItem>();

        //MenuDrawer
        public ITMenuView<diObject> MenuDrawer
        {
            get; set;
        }

        //Event
        public enum ETabEvent
        {
            Open,
            Close,
            Remove
        }
        public delegate void TabAction(ETabEvent eTabEvent, diObject obj);
        public event TabAction OnTabEvent;

        protected virtual int GetNewVarID()
        {
            int index = 0;
            while (m_ID2Item.ContainsKey(index))
            {
                ++index;
            }

            return index;
        }

        public int? OpenTab(diObject obj)
        {
            if(ContainObject(obj))
            {
                return null;
            }
            else
            {
                return OpenNewTab(obj);
            }               
        }

        public bool ContainObject(diObject obj)
        {
            foreach (var pair in m_ID2Item)
            {
                if (pair.Value.obj == obj)
                    return true;
            }
            return false;
        }

        public int? OpenNewTab(diObject obj)
        {
            int id = GetNewVarID();
            m_ID2Item.Add(id, new TabItem(obj, true));
            return id;
        }      

        public void Draw()
        {
            if (m_ID2Item.Count == 0)
                return;
            bool onMenuEvent = false;

            if (ImGui.BeginTabBar("##TabBarViewTopTab",ImGuiTabBarFlags.AutoSelectNewTabs | ImGuiTabBarFlags.Reorderable))
            {
                foreach (var pair in m_ID2Item)
                {
                    if (pair.Value.state && ImGui.BeginTabItem(pair.Value.obj.Name + "##" + pair.Key,ref pair.Value.state))
                    {
                        ProcessSelectTab(pair.Key,pair.Value.obj);
                        ImGui.EndTabItem();
                    }

                    //Call menu drawer when drawer is setted
                    MenuDrawer?.DrawMenuView(pair.Value.obj, out onMenuEvent);
                    if (onMenuEvent) break;
                }
                ImGui.EndTabBar();

                ProcessTabState();
            }
        }

        protected virtual void ProcessTabState()
        {
            //Process tab whether closed
            foreach (var pair in m_ID2Item)
            {
                if (pair.Value.state == false)
                {
                    if (TryRemoveTab(pair.Key))
                    {
                        break;
                    }
                }
            }
        }

        public virtual bool ContainTab(int tabID)
        {
            return m_ID2Item.ContainsKey(tabID);
        }

        public virtual bool TryRemoveTab(int tabID)
        {
            if(ContainTab(tabID))
            {
                bool res;
                NotifyTabEvent(ETabEvent.Remove, tabID, m_ID2Item[tabID].obj);
                Assert.IsTrue(res = m_ID2Item.Remove(tabID));
                return res;
            }
            return false;
        }

        int? m_PrevSelectID = null;
        protected virtual void ProcessSelectTab(int tabID,diObject obj)
        {
            if(tabID != m_PrevSelectID)
            {
                if (m_PrevSelectID != null)
                {
                    NotifyTabEvent(ETabEvent.Close, m_PrevSelectID.Value, m_ID2Item[m_PrevSelectID.Value].obj);
                }
                NotifyTabEvent(ETabEvent.Open,tabID, obj);               
            }
            m_PrevSelectID = tabID;
        }

        /// <summary>
        /// Tab Event
        /// </summary>
        protected virtual void NotifyTabEvent(ETabEvent eTabEvent, int tabID, diObject obj)
        {
            Logger.Info(eTabEvent + " Tab Actoin:" + obj.Name);
            OnTabEvent?.Invoke(eTabEvent, obj);
        }
    }

}
