using System.Collections.Generic;
using ImGuiNET;

namespace DotInsideNode
{
    abstract class TListDrawer<T> where T:diObject
    {
        //Event
        public delegate void TAction(T obj);
        public delegate void RenameAction(T obj, string new_name);
        public event TAction OnDeleteClick;
        public event TAction OnSelectClick;
        public event TAction OnSelectDoubleClick;
        //public event RenameAction OnRename;
        public event TAction OnDuplicateClick;

        //Event Target Object
        protected T m_DeleteItem = default(T);          
        protected T m_SelectedItem = default(T);
        protected T m_DoubleSelectedItem = default(T);
        protected T m_RenameItem = default(T);
        protected T m_DuplicateItem = default(T);

        //Object Dict
        protected Dictionary<string, T> m_Name2Obj = new Dictionary<string, T>();

        public TListDrawer(ref Dictionary<string, T> name2vars)
        {
            m_Name2Obj = name2vars;
        }

        void Reset()
        {
            m_DeleteItem = default(T);
            m_SelectedItem = default(T);
            m_DoubleSelectedItem = default(T);
            m_RenameItem = default(T);
            m_DuplicateItem = default(T);
        }

        /// <summary>
        /// Do notify action when event happened
        /// </summary>
        protected virtual void DoEventNotify()
        {
            if (m_DeleteItem != null)
            {
                OnDeleteClick?.Invoke(m_DeleteItem);
            }
            if (m_SelectedItem != null)
            {
                OnSelectClick?.Invoke(m_SelectedItem);
            }
            if (m_DoubleSelectedItem != null)
            {
                OnSelectDoubleClick?.Invoke(m_DoubleSelectedItem);
            }
            if (m_DuplicateItem != null)
            {
                OnDuplicateClick?.Invoke(m_DuplicateItem);
            }
            Reset();
        }

        /// <summary>
        /// Interface of draw list
        /// </summary>
        public virtual void DrawList()
        {
            int lineNum = MATH.Utils.Clamp(m_Name2Obj.Count, 0, 10);
            if (lineNum == 0)
                return;

            ImGui.BeginListBox("##ListBox" + typeof(T),
                new Vector2(ImGui.GetColumnWidth(), ImGui.GetTextLineHeightWithSpacing() * lineNum)
                );
            foreach (var varPair in m_Name2Obj)
            {
                DrawListItem(varPair.Key,varPair.Value);
                DoDragVar(varPair.Value);
                DrawRightClickView(varPair.Value);
            }
            ImGui.EndListBox();

            DoEventNotify();
        }

        protected virtual void DrawListItem(string name, T tObj)
        {
            if (ImGui.Selectable(name))
            {
                m_SelectedItem = tObj;
            }

            if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(0))
            {
                // Do stuff on Selectable() double click.                                                                                                                                                                                                                           
                m_DoubleSelectedItem = tObj;
            }
        }

        void DrawRightClickView(T tObj)
        {
            ImGuiUtils.PopupContextItemView(() =>
            {
                DrawRightClickMenu(tObj);
            });
        }

        protected virtual void DrawRightClickMenu(T tObj)
        {
            if (ImGui.Selectable("Delete"))
            {
                m_DeleteItem = tObj;
            }
            if (ImGui.Selectable("Duplicate"))
            {
                m_DuplicateItem = tObj;
            }
        }

        /// <summary>
        /// process drag list item action
        /// </summary>
        int dragVarID;
        protected virtual unsafe void DoDragVar(T tObj)
        {
            dragVarID = tObj.ID;
            fixed (int* p = &dragVarID)
            {
                System.IntPtr intPtr = new System.IntPtr(p);
                if (ImGui.BeginDragDropSource(ImGuiDragDropFlags.None))
                {
                    // Set payload to carry the index of our item (could be anything)
                    ImGui.SetDragDropPayload(GetPayloadType(), intPtr, sizeof(int));

                    // Display preview (could be anything, e.g. when dragging an image we could decide to display
                    // the filename and a small preview of the image, etc.)
                    ImGui.Text(tObj.Name + ":" + dragVarID);
                    ImGui.EndDragDropSource();
                }
            }
        }

        protected abstract string GetPayloadType();
    }

}
