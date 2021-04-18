using ImGuiNET;
using System;

namespace DotInsideNode
{
    public abstract class VarBase : IVar
    {
        public abstract IContainer.EContainer ContainerType
        {
            get;
        }

        public virtual void CopyType(VarBase item)
        {
            //this.VarType = item.VarType;
        }

        public abstract void OnVarDelete();
        public abstract void OnChangeType(VarBase newvar);
        public abstract void OnChangeType(diType newType);
        public abstract VarBase Duplicate();

        protected virtual void DrawBaseEditor()
        {
            ImGui.InputText("Name", ref m_EditName, 30);
            DrawType();
            ImGui.SameLine();
            DrawContainerType();
            //ImGui.Checkbox("Instance Editable", ref m_InstanceEditable);
            ImGui.Checkbox("BluePrint Read Only", ref m_BluePrintReadOnly);
            ImGui.InputText("ToolTip", ref m_Tooltip, 30);
            //ImGui.Checkbox("Expose On Spawn", ref m_ExposeOnSpawn);
            //ImGui.Checkbox("Private", ref m_Private);
            //ImGui.Checkbox("Expose To Cinematics", ref m_ExposeToCinematics);
        }

        public virtual void DrawEditor() { }
        public virtual ComNodeBase NewGetter() => throw new System.NotImplementedException();
        public virtual ComNodeBase NewSetter() => throw new System.NotImplementedException();

        IContainer.EContainer m_SelectContainer = IContainer.EContainer.Value;
        public IContainer.EContainer SelectContainerType => m_SelectContainer;

        public void DrawContainerType()
        {
            if (ImGui.BeginCombo(m_SelectContainer.ToString(), "", ImGuiComboFlags.NoPreview))
            {
                foreach (IContainer.EContainer type in Enum.GetValues(typeof(IContainer.EContainer)))
                {
                    if (ImGui.Selectable(type.ToString()))
                    {
                        m_SelectContainer = type;
                    }
                }

                ImGui.EndCombo();
            }
        }

        public virtual void DrawType()
        {
            if (ImGui.BeginCombo("Type", VarType.Name))
            {
                foreach (diType type in diType.TypeClassList)
                {
                    //Selected and type not equal
                    if (ImGui.Selectable(type.ValueType.Name) &&
                        Equals(type.ValueType, VarType) == false)
                    {
                        this.OnChangeType(type);
                    }
                }
                ImGui.EndCombo();
            }

            /*
            if (ImGui.BeginCombo("Type", m_Type.Name))
            {
                foreach (VarBase @var in VarManager.VarClassList)
                {
                    //Current container type
                    if (var.ContainerType == m_SelectContainer)
                    {
                        //Selected and type not equal
                        if (ImGui.Selectable(@var.VarBaseType.Name) &&
                            Equals(@var.VarType, VarType) == false)
                        {
                            this.OnChangeType(@var);
                        }
                    }
                }
                ImGui.EndCombo();
            }
            */
        }

    }
}
