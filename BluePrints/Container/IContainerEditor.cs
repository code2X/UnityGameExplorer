using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    public class IContainerEditor
    {
        public delegate void EContainerAction(diContainer.EContainer eContainer);
        public delegate void diTypeAction(diType type);

        public event EContainerAction OnContainerTypeChange;
        public event diTypeAction OnObjectTypeChange;

        protected void NotifyContainerTypeChange(diContainer.EContainer eContainer)
        {
            OnContainerTypeChange?.Invoke(eContainer);
        }

        protected void NotifyObjectTypeChange(diType type)
        {
            OnObjectTypeChange?.Invoke(type);
        }

        /// <summary>
        /// Draw ditype such as [Bool, Int, Float ...] 
        /// </summary>
        protected virtual void DrawdiType(List<diType> ditypeList,Type curType,string label)
        {
            ImGui.SetNextItemWidth(100);
            ImGuiEx.ComboView(label, () =>
            {
                foreach (diType type in ditypeList)
                {
                    //Selected and type not equal
                    if (ImGui.Selectable(type.ValueType.Name) &&
                        Equals(type.ValueType, curType) == false)
                    {
                        NotifyObjectTypeChange(type);
                    }
                }

            }, curType.Name);
        }

        /// <summary>
        /// Draw container type such as [Value, Array, Set ...] 
        /// </summary>
        protected void DrawContainerType(Array containerTypeArray,diContainer.EContainer eContainer,string label)
        {
            ImGuiEx.ComboView(label, () =>
            {
                foreach (diContainer.EContainer type in containerTypeArray)
                {
                    if (ImGui.Selectable(type.ToString()) &&
                    eContainer != type)
                    {
                        NotifyContainerTypeChange(type);
                    }
                }

            }, "", ImGuiComboFlags.NoPreview);
        }
    }

}
