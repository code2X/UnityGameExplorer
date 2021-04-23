using ImGuiNET;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;

namespace DotInsideLib
{
    class ClusteringClass
    {
        public SortedDictionary<string, SortedDictionary<string, Type>> main;
        public SortedDictionary<string, SortedDictionary<string, Type>> auto;

        public void AutoCluster(SortedDictionary<string, Type> name2type)
        {
            main = ClassCluster.GetMainCluster(name2type);
            auto = ClassCluster.GetAutoCluster(name2type);
        }
    }

    public class AssemblyExplorerView : IWindowView
    {
        public override string WindowName => "Assembly Explorer";
        string processName;
        static ImGuiTableFlags tableFlags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable;

        ClassView classInfoView;
        ClusteringClass clusteringClass;
        string searchText = "";

        static AssemblyExplorerView instance = new AssemblyExplorerView();
        public static AssemblyExplorerView GetInstance() => instance;

        public void AutoCluster(SortedDictionary<string, Type> name2type)
        {
            clusteringClass.AutoCluster(name2type);
        }

        public void AddSubView(IClassView classView)
        {
            classInfoView.AddSubView(classView);
        }

        AssemblyExplorerView()
        {
            classInfoView = ClassView.GetInstance();
            AddSubView(new MethodView());
            AddSubView(new PropertyView());
            AddSubView(new FieldView());

            clusteringClass = new ClusteringClass();
            processName = Process.GetCurrentProcess().MainModule.ModuleName;
            showWindow = true;
        }

        public override void DrawWindowContent()
        {
            ImGui.Text("Proccess:" + processName);
            ImGui.InputTextWithHint("", "type to search", ref searchText, 20);

            ImGui.Text("Main Class");
            DrawClassList(clusteringClass.main, "MainClassTable");

            ImGui.Text("Auto Cluster Class");
            DrawClassList(clusteringClass.auto, "OtherClassTable");
            DrawClassInfo();
        }

        //public override void Update()
        //{
        //    showWindow = true;
        //    if (Input.GetKeyDown(KeyCode.End))
        //    {
        //        showWindow = !showWindow;
        //    }
        //}

        void DrawClassInfo()
        {
            classInfoView.DrawView();
        }

        void DrawClassList(SortedDictionary<string, SortedDictionary<string, Type>> class_dict, string table_name)
        {
            foreach (var state in class_dict)
            {
                if (ImGui.CollapsingHeader(state.Key))
                {
                    ImGuiEx.TableView(table_name, () =>
                    {
                        foreach (var class2type in class_dict[state.Key])
                        {
                            if (class2type.Key.IndexOf(searchText) != -1)
                            {
                                ImGui.TableNextRow();
                                DrawTableRow(class2type);
                            }
                        }
                    }, tableFlags, "Class Name", "Parent Class");
                }
            }
        }

        void DrawTableRow(KeyValuePair<string, Type> class2type)
        {
            ImGuiEx.TableColumns(() =>
            {
                if (ImGui.Button(class2type.Key))
                {
                    classInfoView.Show(class2type.Value);
                }
            }, () =>
            {
                ImGui.Text(class2type.Value.BaseType.Name);
            });
        }
    }
}
