using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    interface IEditor
    {
        void DrawEditor();
    }

    /// <summary>
    /// Variable base field
    /// </summary>
    public abstract class IVar: diObject, IEditor
    {
        bool m_InstanceEditable = false;
        bool m_BluePrintReadOnly = false;
        string m_Tooltip = string.Empty;
        bool m_ExposeOnSpawn = false;
        bool m_Private = false;

        //Event Interface
        public virtual void OnVarDelete() { }
        public virtual void OnVarRename(string newName) { }

        //Abstract Interface
        public abstract Type VarType
        {
            get;
        }
        public abstract diContainer.EContainer ContainerType
        {
            get;
        }
        public abstract object VarValue
        {
            get;
            set;
        }
        public abstract IVar Duplicate();
        public virtual void DrawEditor() { }
        public abstract List<ComNodeBase> Setters
        {
            get;
            protected set;
        }

        public abstract List<ComNodeBase> Getters
        {
            get;
            protected set;
        }
        public abstract ComNodeBase GetNewGetter();
        public abstract ComNodeBase GetNewSetter();

        //Overridable Field Interface
        public virtual bool InstanceEditable
        {
            get => m_InstanceEditable;
            protected set => m_InstanceEditable = value;
        }
        public virtual bool BluePrintReadOnly
        {
            get => m_BluePrintReadOnly;
            protected set => m_BluePrintReadOnly = value;
        }
        public virtual string Tooltip
        {
            get => m_Tooltip;
            protected set => m_Tooltip = value;
        }
        public virtual bool ExposeOnSpawn
        {
            get => m_ExposeOnSpawn;
            protected set => m_ExposeOnSpawn = value;
        }
        public virtual bool Private
        {
            get => m_Private;
            protected set => m_Private = value;
        }

        //Copy base class field
        protected virtual IVar CopyBaseField(IVar item)
        {
            var constructor = this.GetType().GetConstructor(Type.EmptyTypes);
            IVar copy = (IVar)constructor.Invoke(null);

            copy.ID = item.ID;
            copy.Name = item.Name;
            copy.InstanceEditable = item.InstanceEditable;
            copy.BluePrintReadOnly = item.BluePrintReadOnly;
            copy.Tooltip = item.Tooltip;
            copy.ExposeOnSpawn = item.ExposeOnSpawn;
            copy.Private = item.Private;

            copy.Setters = item.Setters;
            copy.Getters = item.Getters;

            return copy;
        }

        public abstract class IVarEditor : IContainerEditor, IEditor
        {
            public abstract void DrawEditor();
        }

        public class VarDefaultEditor : IVarEditor
        {
            //Editor Event
            public virtual void OnSubvariableTypeChange(diType newType) { }

            IVar m_Var;

            protected string m_EditName = string.Empty;

            public VarDefaultEditor(IVar @var)
            {
                m_Var = @var;
                m_EditName = m_Var.Name;

                m_Var.OnSetName += new NameEvent(SetNameProc);
            }

            void SetNameProc(string newName)
            {
                m_EditName = newName;
            }

            /// <summary>
            /// Editor Interface
            /// </summary>
            public override void DrawEditor()
            {
                if (ImGui.CollapsingHeader("Variable", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    DrawEditorBase();
                }
            }

            public virtual bool TryRenameVar(ref string newName)
            {
                //Manager have the var name
                if (VarManager.Instance.ContainVar(newName))
                {
                    Logger.Info("Have var name:" + newName + ":" + m_Var.Name);
                    newName = m_Var.Name;
                    return false;
                }
                else
                {
                    //OnVarRename(newName);
                    Assert.IsTrue(VarManager.Instance.TryRenameVar(m_Var.ID, newName));
                    m_Var.Name = newName;
                    return true;
                }
            }

            protected unsafe int InputTextCompleteCallback(ImGuiInputTextCallbackData* data)
            {
                if (data->EventFlag == ImGuiInputTextFlags.CallbackCompletion)
                {
                    TryRenameVar(ref m_EditName);
                }

                return 0;
            }

            protected unsafe virtual void DrawEditorBase()
            {
                ImGui.InputText("Name##" + m_Var.Name, ref m_EditName, 30, ImGuiInputTextFlags.CallbackCompletion, InputTextCompleteCallback);

                DrawVariableType(diType.TypeClassList);
                ImGui.SameLine();
                DrawContainerType(Enum.GetValues(typeof(diContainer.EContainer)));
                //ImGui.Checkbox("Instance Editable", ref m_InstanceEditable);
                ImGui.Checkbox("BluePrint Read Only", ref m_Var.m_BluePrintReadOnly);
                ImGui.InputText("ToolTip", ref m_Var.m_Tooltip, 30);

                //ImGui.Checkbox("Expose On Spawn", ref m_ExposeOnSpawn);
                //ImGui.Checkbox("Private", ref m_Private);
            }

            protected virtual void DrawVariableType(List<diType> ditypeList)
            {
                DrawdiType(ditypeList, m_Var.VarType, "Type");
            }
            protected void DrawContainerType(Array containerTypeArray)
            {
                DrawContainerType(containerTypeArray, m_Var.ContainerType, m_Var.ContainerType.ToString());
            }
        }

    }

}
