using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    abstract class IParam: dnObject, IEditor
    {
        public abstract Type ParamType
        {
            get;
        }

        public abstract diContainer.EContainer ContainerType
        {
            get;
        }

        public virtual void DrawEditor() { }

        //Param Editor
        public abstract class IParamEditor : IContainerEditor, IEditor
        {
            public abstract void DrawEditor();
        }

        public class ParamDefaultDrawer : IParamEditor
        {
            IParam m_Param;

            public ParamDefaultDrawer(IParam param)
            {
                m_Param = param;
            }

            public override void DrawEditor()
            {
                ImGui.Text(m_Param.Name);
                ImGui.SameLine();
                DrawParamType(diType.TypeClassList);
                ImGui.SameLine();
                DrawContainerType(Enum.GetValues(typeof(diContainer.EContainer)));
                ImGui.SameLine();
                ImGui.Button("X##ParamButton" + m_Param.Name);
            }

            protected virtual void DrawParamType(List<diType> ditypeList)
            {
                DrawdiType(ditypeList, m_Param.ParamType, "##VariableType" + m_Param.Name);
            }
            protected void DrawContainerType(Array containerTypeArray)
            {
                DrawContainerType(containerTypeArray, m_Param.ContainerType, m_Param.ContainerType.ToString() + "##" + m_Param.Name);
            }
        }


    }
}
