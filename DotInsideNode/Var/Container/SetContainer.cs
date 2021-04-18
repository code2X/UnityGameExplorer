using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    [AValueContainer]
    class SetContainer : ContainerBase
    {
        HashSet<object> m_Set;

        public SetContainer()
        {
            ResetSet();
        }

        public override diType ValueType
        {
            get => m_ValueType;
            set
            {
                if (m_ValueType != value)
                {
                    m_ValueType = value;
                    m_Set = new HashSet<object>();
                }
            }
        }
        public override object Value
        {
            get => m_Set;
            set => m_Set = (HashSet<object>)value;
        }

        public override EContainer ContainerType => EContainer.Set;

        public override void DrawContainerValue()
        {
            DrawSetEditor();
        }

        public override IContainer DuplicateContainer()
        {
            SetContainer res = new SetContainer();
            res.m_Set = (HashSet<object>)DuplicateContainerValue();
            return res;
        }

        public override object DuplicateContainerValue()
        {
            HashSet<object> res = new HashSet<object>();
            foreach(var item in m_Set)
            {
                res.Add(item);
            }
            return res;
        }

        protected void ResetSet()
        {
            m_Set = new HashSet<object>();
        }

        protected void DrawAddItem()
        {
            if (ImGui.Button("Add Item##BoolArrayVar"))
                m_Set.Add(m_ValueType.NewObject);
        }

        protected void DrawRemoveAll()
        {
            if (ImGui.Button("Remove All##BoolArrayVar"))
                ResetSet();
        }

        protected void DeleteItem(object item)
        {
            m_Set.Remove(item);
        }

        protected bool DrawArrowMenu(object item)
        {
            bool block = false;
            if (ImGui.BeginCombo("##SetVarArrowButton" + item.ToString(), "", ImGuiComboFlags.NoPreview))
            {
                if (ImGui.Selectable("Delete"))
                {
                    DeleteItem(item);
                    block = true;
                }
                ImGui.EndCombo();
            }
            return block;
        }

        protected void DrawSetEditor()
        {
            HashSet<object> TSet = m_Set;
            int len = TSet.Count;
            int index = 0;

            DrawAddItem();
            ImGui.SameLine();
            DrawRemoveAll();

            ImGuiUtils.TableView("SetVar", () =>
            {
                foreach (object item in TSet)
                {
                    ImGui.TableNextColumn();
                    ImGui.Text(index.ToString());

                    ImGui.TableNextColumn();
                    if (DrawSetItem(item))
                        break;
                    ImGui.SameLine();
                    if (DrawArrowMenu(item))
                        break;
                    if (index > 500)
                        break;

                    ++index;
                }

            }, "Set", len.ToString() + " Set elements");
        }

        public virtual bool DrawSetItem(object item)
        {
            object copy = item;
            copy = m_ValueType.Draw(ref copy,item.ToString());
            if (item.Equals(copy) == false && m_Set.Contains(copy) == false)
            {
                m_Set.Remove(item);
                m_Set.Add(copy);
                return true;
            }

            return false;
        }

    }
}
