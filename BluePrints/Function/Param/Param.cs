using System;

namespace DotInsideNode
{
    class Param : IParam
    {
        diContainer m_Container;
        IParamEditor m_BaseEditor;

        public Param()
        {
            m_Container = new ValueContainer();
            m_BaseEditor = new ParamDefaultDrawer(this);
            m_BaseEditor.OnContainerTypeChange += new IContainerEditor.EContainerAction(OnContainerTypeChange);
            m_BaseEditor.OnObjectTypeChange += new IContainerEditor.diTypeAction(OnVariableTypeChange);
        }

        public override Type ParamType
        {
            get => m_Container.ValueType.ValueType;
        }
        public override diContainer.EContainer ContainerType
        {
            get => m_Container.ContainerType;
        }

        public void OnContainerTypeChange(diContainer.EContainer SelectContainerType)
        {
            if (SelectContainerType != m_Container.ContainerType)
            {
                foreach (diContainer container in diContainer.ContainerClassList)
                {
                    if (container.ContainerType == SelectContainerType)
                    {
                        m_Container = container.DuplicateContainer();
                        Logger.Info(container.ContainerType.ToString());
                        break;
                    }
                }
                Logger.Info("ContainerTypeChange");
            }
        }

        public void OnVariableTypeChange(diType newType)
        {
            m_Container.ValueType = newType;
        }

        public override void DrawEditor()
        {
            m_BaseEditor.DrawEditor();
        }
    }
}

