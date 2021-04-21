using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    public class Variable : IVar
    {
        diContainer m_Container;

        //Node setters and getters
        List<ComNodeBase> m_Setters = new List<ComNodeBase>();
        List<ComNodeBase> m_Getters = new List<ComNodeBase>();

        IVarEditor m_BaseEditor;

        public Variable()
        {
            m_Container = new ValueContainer();
            m_BaseEditor = new VarDefaultEditor(this);
            m_BaseEditor.OnContainerTypeChange += new IContainerEditor.EContainerAction(OnContainerTypeChange);
            m_BaseEditor.OnObjectTypeChange += new IContainerEditor.diTypeAction(OnVariableTypeChange);
        }

        //Node
        public override List<ComNodeBase> Setters
        {
            get => m_Setters;
            protected set => m_Setters = value;
        }

        public override List<ComNodeBase> Getters
        {
            get => m_Getters;
            protected set => m_Getters = value;
        }

        public override ComNodeBase GetNewGetter(INodeGraph bp)
        {
            VarGetter getter = new VarGetter(bp, this);
            m_Getters.Add(getter);
            return getter;
        }
        public override ComNodeBase GetNewSetter(INodeGraph bp)
        {
            VarSetter setter = new VarSetter(bp,this);
            m_Setters.Add(setter);
            return setter;
        }

        //Field Interface
        public override object VarValue
        {
            get => m_Container.Value;
            set => m_Container.Value = value;
        }

        public override Type VarType
        {
            get => m_Container.ValueType.ValueType;
        }
        public override diContainer.EContainer ContainerType
        {
            get => m_Container.ContainerType;
        }

        protected IVar DuplicateBase()
        {
            Variable newVar = (Variable)ClassTools.CallDefaultConstructor(this.GetType());
            newVar.CopyBaseField(this);
            newVar.m_Container = m_Container.DuplicateContainer();

            return newVar;
        }

        public override IVar Duplicate()
        {
            return DuplicateBase();
        }

        public override void DrawEditor()
        {
            m_BaseEditor.DrawEditor();
            if (ImGui.CollapsingHeader("Default Value", ImGuiTreeNodeFlags.DefaultOpen))
            {
                m_Container.DrawContainerValue();
                ImGui.SameLine();
                ImGui.Text(Name);
            }
        }

/// <summary>
/// Event
/// </summary>
        void OnContainerTypeChange(diContainer.EContainer SelectContainerType)
        {
            if (SelectContainerType != m_Container.ContainerType)
            {
                foreach (diContainer container in diContainer.ContainerClassList)
                {
                    if (container.ContainerType == SelectContainerType)
                    {
                        m_Container = container.DuplicateContainer();
                        Logger.Info(container.ContainerType.ToString());
                        NotifyVariableTypeChange();
                        break;
                    }
                }
                Logger.Info("ContainerTypeChange");
            }
        }

        void OnVariableTypeChange(diType newType)
        {
            m_Container.ValueType = newType;
            NotifyVariableTypeChange();
        }

        public void NotifyVariableTypeChange()
        {
            foreach (VarSetter setter in Setters)
            {
                setter.OnVarTypeChange();
            }
            foreach (VarGetter getter in Getters)
            {
                getter.OnVarTypeChange();
            }
        }

        public override void OnVarDelete()
        {
            //foreach (VarSetter setter in Setters)
            //{
            //    Assert.IsTrue(NodeManager.Instance.TryDestroyNode(setter.ID));
            //}
            //foreach (VarGetter getter in Getters)
            //{
            //    Assert.IsTrue(NodeManager.Instance.TryDestroyNode(getter.ID));
            //}

            Setters.Clear();
            Getters.Clear();
        }

    }

}