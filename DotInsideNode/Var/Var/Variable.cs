using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    public class Variable : VarBase
    {
        protected List<VarSetter> m_Setters = new List<VarSetter>();
        protected List<VarGetter> m_Getters = new List<VarGetter>();

        IContainer m_Container;

        public Variable()
        {
            m_Container = new ValueContainer();
        }

        public override object VarValue
        {
            get => m_Container.Value;
            set => m_Container.Value = value;
        }

        public override Type VarType
        {
            get => m_Container.ValueType.ValueType;
        }

        public override List<VarSetter> Setters
        {
            get => m_Setters;
            protected set => m_Setters = value;
        }

        public override List<VarGetter> Getters
        {
            get => m_Getters;
            protected set => m_Getters = value;
        }

        public override IContainer.EContainer ContainerType
        {
            get => IContainer.EContainer.Value;
        }

        protected VarBase BaseDuplicate()
        {
            //var constructor = this.GetType().GetConstructor(System.Type.EmptyTypes);
            //VarBase newVar = (VarBase)constructor.Invoke(null);
            //newVar.BaseCopy(this);
            //newVar.VarValue = m_Value;
            //
            //return newVar;
            return null;
        }

        public override VarBase Duplicate()
        {
            return BaseDuplicate();
        }

        public override void OnChangeType(VarBase template_var)
        {
            VarBase newVar = template_var.BaseCopy(this);
            Assert.IsNotNull(newVar);
            newVar.CopyType(template_var);

            foreach (VarSetter setter in m_Setters)
            {
                setter.ChageVar(newVar);
            }
            foreach (VarGetter getter in m_Getters)
            {
                getter.ChageVar(newVar);
            }
            Assert.IsTrue(VarManager.Instance.TryReplaceVar(this, newVar));
            Assert.IsTrue(VarManager.Instance.SelectVar(newVar.ID));
        }

        public override void OnChangeType(diType newType)
        {
            //m_Container.ValueType = newType;
            if(SelectContainerType != m_Container.ContainerType)
            {
                foreach (IContainer container in IContainer.ContainerClassList)
                {
                    if(container.ContainerType == SelectContainerType)
                    {
                        m_Container = container.DuplicateContainer();
                        m_Container.ValueType = newType;
                        Logger.Info(container.ContainerType.ToString());
                        break;
                    }
                }
                Logger.Info("ContainerTypeChange");
            }
            else
            {
                m_Container.ValueType = newType;
            }
        }

        public override void OnVarDelete()
        {
            foreach (VarSetter setter in m_Setters)
            {
                NodeManager.Instance.TryDestroyNode(setter.ID);
            }
            foreach (VarGetter getter in m_Getters)
            {
                NodeManager.Instance.TryDestroyNode(getter.ID);
            }
        }

        public override ComNodeBase NewGetter()
        {
            VarGetter getter = new VarGetter(this);
            m_Getters.Add(getter);
            return getter;
        }

        public override ComNodeBase NewSetter()
        {
            VarSetter setter = new VarSetter(this);
            m_Setters.Add(setter);
            return setter;
        }

        public override void DrawEditor()
        {
            if (ImGui.CollapsingHeader("Variable", ImGuiTreeNodeFlags.DefaultOpen))
            {
                DrawBaseEditor();
            }
            if (ImGui.CollapsingHeader("Default Value", ImGuiTreeNodeFlags.DefaultOpen))
            {
                m_Container.DrawContainerValue();
                ImGui.SameLine();
                ImGui.Text(m_Name);
            }
        }

    }


}