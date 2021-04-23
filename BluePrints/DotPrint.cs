using imnodesNET;
using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    class DotPrint : DotInsideLib.IWindowView
    {
        public override string WindowName => "DotPrint";
        public static ImGuiTableFlags TableFlags = ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable;

        NodeEditor m_NodeEditor = new NodeEditor();
        VarManager m_VarManager = VarManager.Instance;
        FunctionManager m_FunctionManager = FunctionManager.Instance;

        TabBarView m_TabBarView = new TabBarView(); 
        PrintRightView m_RightView = PrintRightView.Instance;

        DotPrint()
        {
            m_FunctionManager.TabBar = m_TabBarView;
            m_FunctionManager.NodeEditor = m_NodeEditor;

            m_NodeEditor.CreateMethod();
            ShowWindow();
        }
        static DotPrint instance = new DotPrint();
        public static DotPrint GetInstance() => instance;

        public override void DrawWindowContent()
        {
            if (ImGui.BeginTable("EditorTable", 3, TableFlags))
            {
                ImGuiEx.TableSetupHeaders();

                //Left
                ImGui.TableSetColumnIndex(0);
                DrawLeft();
                DrawConsoleOutput();

                //Middle
                ImGui.TableSetColumnIndex(1);
                DrawEditorTop();
                m_TabBarView.Draw();
                m_NodeEditor.DrawWindowContent();
                
                //Right
                ImGui.TableSetColumnIndex(2);
                DrawRight();                
                ImGui.EndTable();
                
            }
            
        }

        void DrawEditorTop()
        {
            if(ImGui.Button("Compile"))
            {
                compileText = m_NodeEditor.Compile();
            }
            ImGui.SameLine();
            ImGui.Button("Save");
            ImGui.SameLine();
            if(ImGui.Button("Play"))
            {
                try
                {
                    m_NodeEditor.Play();
                }
                catch(Exception exp)
                {
                    compileText = exp.ToString();
                }
            }
        }

        void DrawLeft()
        {
            DrawFunctionList();
            DrawVaribaleList();
        }

        void DrawFunctionList()
        {
            //Function
            if (ImGui.Button("+##Function Create"))
            {
                //m_NodeEditor.CreateMethod();
                m_FunctionManager.AddFunction(new Function());
                Console.WriteLine("IFunction Create");
            }
            ImGui.SameLine();
            if (ImGui.CollapsingHeader("Functions"))
            {
                m_FunctionManager.DrawFunctionList();
            }
        }

        void DrawVaribaleList()
        {
            //Variables
            if (ImGui.Button("+##Variables Create"))
            {
                m_VarManager.AddVar();
                Console.WriteLine("Variable Create");
            }
            ImGui.SameLine();
            if (ImGui.CollapsingHeader("Variables"))
            {
                m_VarManager.DrawVarList();
            }
        }

        
        void DrawRight()
        {
            m_RightView.Draw();            
        }

        string compileText = "";
        void DrawConsoleOutput()
        {
            if (ImGui.CollapsingHeader("Console Output"))
            {
                ImGui.InputTextMultiline("", ref compileText, 10000, new Vector2(ImGui.GetColumnWidth(), ImGui.GetTextLineHeight() * 16));
            }
        }
    }
}
