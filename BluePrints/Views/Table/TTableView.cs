
using System.Collections.Generic;
using ImGuiNET;

namespace DotInsideNode
{
    public abstract class TTableView<T> where T : dnObject
    {
        protected Dictionary<int, T> m_ID2Obj = new Dictionary<int, T>();

        public TTableView(diObjectManager<T> diObjectManager)
        {
            m_ID2Obj = diObjectManager.ID2Object;
        }

        public void Draw()
        {
            ImGuiEx.TableView("##TableView" + typeof(T).Name, () =>
            {
                foreach (var objPair in m_ID2Obj)
                {
                    ImGui.TableNextRow();
                    DrawItem(objPair.Value, out bool onEvent);
                    if(onEvent)
                        break;
                }
            }, Titles);
        }

        protected abstract string[] Titles
        {
            get;
        }
        protected abstract void DrawItem(T obj, out bool onEvent);
    }

    public abstract class TListTableView<T> where T : dnObject
    {
        protected List<T> m_KeyNameList = new List<T>();

        public TListTableView(KeyNameList<T> keyNameList)
        {
            m_KeyNameList = keyNameList.ObjList;
        }

        public void Draw()
        {
            ImGuiEx.TableView(TableLable, () =>
            {
                for (int i = 0; i< m_KeyNameList.Count; ++i)
                {
                    ImGui.TableNextRow();
                    DrawItem(i,m_KeyNameList[i], out bool onEvent);
                    if (onEvent)
                        break;
                }
            }, Titles);
        }

        protected abstract string TableLable
        {
            get;
        }
        protected abstract string[] Titles
        {
            get;
        }
        protected abstract void DrawItem(int index,T obj, out bool onEvent);
    }
}