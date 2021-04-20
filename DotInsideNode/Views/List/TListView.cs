using System.Collections.Generic;
using ImGuiNET;

namespace DotInsideNode
{
    abstract class TListView<T> where T : diObject
    {
        protected Dictionary<string, T> m_Name2Obj = new Dictionary<string, T>();

        public TListView(ref Dictionary<string, T> name2vars)
        {
            m_Name2Obj = name2vars;
        }

        //MenuDrawer
        public ITMenuView<T> MenuDrawer
        {
            get; set;
        }

        //List Item Event
        public enum EListItemEvent
        {
            SelectItem,
            DoubleSelectItem,
        }
        public delegate void TListAction(EListItemEvent eListItemEvent, T obj);
        public event TListAction OnListItemEvent;

        protected virtual void NotifyEvent(EListItemEvent eListItemEvent,T tObj)
        {
            OnListItemEvent?.Invoke(eListItemEvent, tObj);
        }

        public virtual int MaxItemCount => 10;

        /// <summary>
        /// Interface
        /// </summary>
        public virtual void DrawList()
        {
            int lineNum = MATH.Utils.Clamp(m_Name2Obj.Count, 0, MaxItemCount);
            if (lineNum == 0)
                return;

            bool onMenuEvent = false;

            ImGui.BeginListBox("##ListBox" + typeof(T),
                new Vector2(ImGui.GetColumnWidth(), ImGui.GetTextLineHeightWithSpacing() * lineNum)
                );
            foreach (var varPair in m_Name2Obj)
            {
                DrawListItem(varPair.Key,varPair.Value, out onMenuEvent);
                if (onMenuEvent) break;

                DragVarProc(varPair.Value, out onMenuEvent);
                if (onMenuEvent) break;

                //Call menu drawer when drawer is setted
                MenuDrawer?.DrawMenuView(varPair.Value, out onMenuEvent);
                if (onMenuEvent) break;
            }
            ImGui.EndListBox();
        }

        protected virtual void DrawListItem(string name, T tObj,out bool onEvent)
        {
            onEvent = false;
            if (ImGui.Selectable(name))
            {
                NotifyEvent(EListItemEvent.SelectItem, tObj);
            }

            // Do stuff on Selectable() double click.                    
            if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(0))
            {                                                                                                                                                                                                                       
                NotifyEvent(EListItemEvent.DoubleSelectItem, tObj);
            }
        }

        /// <summary>
        /// process drag item action
        /// </summary>
        int dragVarID;
        protected virtual unsafe void DragVarProc(T tObj, out bool onEvent)
        {
            onEvent = false;
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
