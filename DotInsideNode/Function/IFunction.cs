using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    class IFunction : diObject
    {
        public enum EAccessSpecifier
        {
            Public,
            Private,
            Protected
        }

        public struct Variable
        {

        }

        protected string m_Description = string.Empty;
        protected string m_Keywords = string.Empty;
        protected string m_CompactNodeTitle = string.Empty;
        protected EAccessSpecifier m_eAccessSpecifier = EAccessSpecifier.Public;
        protected bool m_Pure = false;
        protected bool m_CallInEditor = false;

        protected string m_EditName = string.Empty;

        public override string Name
        {
            get => m_Name;
            set
            {
                m_Name = value;
                m_EditName = m_Name;
            }
        }

        public string Description
        {
            get => m_Description;
        }
    }

    class FunctionBase: IFunction
    {
        LinkManager m_LinkManager;
        NodeManager m_NodeManager;
        VarManager m_LocalVarManager;
        NodeComponentManager m_NodeComponentManager;
        Dictionary<int, Vector2> m_NodePositions = null;

        public FunctionBase()
        {
            m_LinkManager = new LinkManager();
            m_NodeManager = new NodeManager();
            m_LocalVarManager = new VarManager();
            m_NodeComponentManager = new NodeComponentManager(m_LinkManager);
        }

        public void OpenGraph()
        {
            LinkManager.Instance = m_LinkManager;
            NodeComponentManager.Instance = m_NodeComponentManager;
            NodeManager.Instance = m_NodeManager;

            if(m_NodePositions != null)
                m_NodeManager.NodeEditorPostions = m_NodePositions;
        }

        public void CloseGraph()
        {
            m_NodePositions = m_NodeManager.NodeEditorPostions;
        }

        public void OnFunctionDelete()
        {

        }

        protected virtual void DrawBaseEditor()
        {
            ImGui.InputText("Name", ref m_EditName, 30);
            ImGui.InputText("Description", ref m_Description, 30);
            ImGui.InputText("Keywords", ref m_Keywords, 30);
            ImGui.InputText("Compact Node Title", ref m_CompactNodeTitle, 30);
            DrawAccessSpecifier();
            ImGui.Checkbox("Pure", ref m_Pure);
            ImGui.Checkbox("Call In Editor", ref m_CallInEditor);
        }
        public void DrawAccessSpecifier()
        {
            if (ImGui.BeginCombo("Access Specifier", m_eAccessSpecifier.ToString()))
            {
                foreach (EAccessSpecifier type in Enum.GetValues(typeof(EAccessSpecifier)))
                {
                    if (ImGui.Selectable(type.ToString()))
                    {
                        m_eAccessSpecifier = type;
                    }
                }

                ImGui.EndCombo();
            }
        }

        public virtual void DrawEditor() 
        {
            if (ImGui.CollapsingHeader("Graph", ImGuiTreeNodeFlags.DefaultOpen))
            {
                DrawBaseEditor();
            }

            //Function
            if (ImGui.Button("+##Input Create"))
            {
                Console.WriteLine("Input Create");
            }
            ImGui.SameLine();
            if (ImGui.CollapsingHeader("Input", ImGuiTreeNodeFlags.DefaultOpen))
            {
            }

            //Variables
            if (ImGui.Button("+##Outputs Create"))
            {
                Console.WriteLine("Outputs Create");
            }
            ImGui.SameLine();
            if (ImGui.CollapsingHeader("Outputs", ImGuiTreeNodeFlags.DefaultOpen))
            {
            }
        }
    }
}
