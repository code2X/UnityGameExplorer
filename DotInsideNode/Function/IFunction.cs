using ImGuiNET;
using System.Collections.Generic;

namespace DotInsideNode
{
    abstract class IFunction : diObject, IEditor
    {
        public enum EAccessSpecifier
        {
            Public,
            Private,
            Protected
        }

        //Base Field
        protected string m_Description = string.Empty;
        protected string m_Keywords = string.Empty;
        protected string m_CompactNodeTitle = string.Empty;
        protected EAccessSpecifier m_eAccessSpecifier = EAccessSpecifier.Public;
        protected bool m_Pure = false;
        protected bool m_CallInEditor = false;

        public IFunction()
        { }

        public string Description => m_Description;

        //Param Interface
        public abstract ParamManager InputParams
        {
            get;
        }

        public abstract ParamManager OutputParams
        {
            get;
        }

        //Node Interface       
        protected abstract List<ComNodeBase> EntryNodes
        {
            get;
        }
        protected abstract List<ComNodeBase> ReturnNodes
        {
            get;
        }
        protected abstract List<ComNodeBase> CallNodes
        {
            get;
        }
        public abstract int EntryNodeCount
        {
            get;
        }
        public abstract int ReturnNodeCount
        {
            get;
        }
        public abstract int CallNodeCount
        {
            get;
        }

        //Function Node Interface
        public abstract ComNodeBase GetNewFunctionEntry(INodeGraph bluePrint);
        public abstract ComNodeBase GetNewFunctionReturn(INodeGraph bluePrint);
        public abstract ComNodeBase GetNewFunctionCall(INodeGraph bluePrint);

        //Overridable Interface
        public virtual void DrawEditor() {}
        public virtual void Execute(int callerID, object[] inParams,out object[] outParams) { outParams = null; }

        public virtual void OpenGraph() { }
        public virtual void CloseGraph() { }
        public abstract IFunctionGraph GetGraph();

        protected abstract class IFunctionEditor : IEditor
        {
            public abstract void DrawEditor();
        }

        protected class FunctionDefaultEditor : IFunctionEditor
        {
            protected string m_EditName = string.Empty;
            protected IFunction m_Function = null;

            public FunctionDefaultEditor(IFunction function)
            {
                m_Function = function;
                m_EditName = m_Function.Name;

                m_Function.OnSetName += new NameEvent(SetNameProc);
            }

            void SetNameProc(string newName)
            {
                m_EditName = newName;
            }

            protected virtual void DrawBaseEditor()
            {
                ImGui.InputText("Name", ref m_EditName, 30);
                ImGui.InputText("Description", ref m_Function.m_Description, 30);
                ImGui.InputText("Keywords", ref m_Function.m_Keywords, 30);
                ImGui.InputText("Compact Node Title", ref m_Function.m_CompactNodeTitle, 30);
                DrawAccessSpecifier();
                ImGui.Checkbox("Pure", ref m_Function.m_Pure);
                ImGui.Checkbox("Call In Editor", ref m_Function.m_CallInEditor);
            }
            public void DrawAccessSpecifier()
            {
                if (ImGui.BeginCombo("Access Specifier", m_Function.m_eAccessSpecifier.ToString()))
                {
                    foreach (EAccessSpecifier type in System.Enum.GetValues(typeof(EAccessSpecifier)))
                    {
                        if (ImGui.Selectable(type.ToString()))
                        {
                            m_Function.m_eAccessSpecifier = type;
                        }
                    }

                    ImGui.EndCombo();
                }
            }

            public override void DrawEditor()
            {
                if (ImGui.CollapsingHeader("Graph", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    DrawBaseEditor();
                }
            }
        }

    }

}
