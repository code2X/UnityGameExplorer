using UnityEngine;
using ImGuiNET;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Reflection;

namespace ExplorerSpace
{
    public abstract class IView
    {
        public abstract void OnGUI();
        public virtual void Update() { }
    }

    class ClusteringClass
    {
        public SortedDictionary<string, SortedDictionary<string, Type>> main;
        public SortedDictionary<string, SortedDictionary<string, Type>> auto;

        public ClusteringClass(SortedDictionary<string, Type> name2type)
        {
            main = ClassCluster.GetMainCluster(name2type);
            auto = ClassCluster.GetAutoCluster(name2type);
        }
    }

    class AssemblyExplorerView : IView
    {
        string windowTitle;
        string processName;
        static ImGuiTableFlags tableFlags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable;

        ClassView classInfoView;
        SortedDictionary<string, SortedDictionary<string, Type>> mainClusterClass, autoClusterClass;
        string searchText = "";
        bool showWindow = true;

        static AssemblyExplorerView instance = new AssemblyExplorerView();
        public static AssemblyExplorerView GetInstance() => instance;

        public void AutoCluster(SortedDictionary<string, Type> name2type)
        {
            mainClusterClass = ClassCluster.GetMainCluster(name2type);
            autoClusterClass = ClassCluster.GetAutoCluster(name2type);
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
            
            mainClusterClass = new SortedDictionary<string, SortedDictionary<string, Type>>();
            autoClusterClass = new SortedDictionary<string, SortedDictionary<string, Type>>();
            windowTitle = "Assembly Explorer";
            processName = Process.GetCurrentProcess().MainModule.ModuleName;
        }

        public override void OnGUI()
        {
            if (!showWindow) return;

            ImGui.Begin(windowTitle, ref showWindow,ImGuiWindowFlags.NoCollapse);
            ImGui.Text("Proccess:" + processName);
            ImGui.InputTextWithHint("","type to search", ref searchText, 20);

            ImGui.Text("Main Class");
            DrawClassList(mainClusterClass, "MainClassTable");

            ImGui.Text("Auto Cluster Class");
            DrawClassList(autoClusterClass, "OtherClassTable");
            DrawClassInfo();
            ImGui.End();
        }

        public override void Update()
        {
            showWindow = true;
            if (Input.GetKeyDown(KeyCode.End))
            {
                showWindow = !showWindow;
            }
        }

        void DrawClassInfo()
        {
            classInfoView.DrawView();
        }

        void DrawClassList(SortedDictionary<string, SortedDictionary<string, Type>> class_dict, string table_name)
        {
            foreach (var state in class_dict)
            {
                if (ImGui.CollapsingHeader(state.Key) &&
                    ImGui.BeginTable(table_name, 2, tableFlags))
                {
                    ImGuiUtils.TableSetupHeaders("Class Name", "Parent Class");

                    foreach (var class2type in class_dict[state.Key])
                    {                      
                        if (class2type.Key.IndexOf(searchText) != -1)
                        {
                            ImGui.TableNextRow();
                            DrawTableRow(class2type);
                        }
                    }
                    ImGui.EndTable();
                }
            }
        }

        void DrawTableRow(KeyValuePair<string,Type> class2type)
        {
            ImGui.TableSetColumnIndex(0);
            if (ImGui.Button(class2type.Key))
            {
                classInfoView.Show(class2type.Value);
            }
            ImGui.TableSetColumnIndex(1);
            ImGui.Text(class2type.Value.BaseType.Name);
        }
    }

    class InstanceView : IView
    {
        public class InstanceInfo
        {
            public string parent;
            public Type type;
            public object instance;
            public string name;

            public InstanceInfo()
            {
                name = string.Empty;
                parent = string.Empty;
                instance = null;
                type = null;
            }
        }

        static string windowTitle = "Instance Explorer";
        static bool showWindow = false;
        static InstanceInfo curInstance;
        static ArrayList subViews;
        static HashSet<InstanceInfo> instanceList = new HashSet<InstanceInfo>();

        static InstanceView instance = new InstanceView();
        public static InstanceView GetInstance() => instance;

        public void AddSubView(IClassView view)
        {
            subViews.Add(view);
        }

        InstanceView()
        {
            subViews = new ArrayList();       
            AddSubView(new InstanceFieldView());
            AddSubView(new InstancePropView());
            AddSubView(new InstanceMethodView());

            Add("GameMain", "none", typeof(GameMain), null);
        }

        void UpdateView(InstanceInfo instanceInfo)
        {
            curInstance = instanceInfo;
            foreach (IClassView i in subViews)
            {
                i.Show(curInstance.type);
            }
        }

        public void Add(string parent,string name,Type type, object instance = null)
        {
            InstanceInfo instanceInfo = new InstanceInfo();
            instanceInfo.instance = instance;
            instanceInfo.name = name;
            instanceInfo.type = type;
            instanceInfo.parent = parent;
           
            if(instanceList.Add(instanceInfo))
            {
                curInstance = instanceInfo;
                UpdateView(curInstance);
            }

            showWindow = true;
        }
        public static ImGuiTableFlags tableFlags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable;

        public override void OnGUI()
        {
            ImGui.Begin(windowTitle);

            DrawLeft();
            ImGui.SameLine();
            DrawRight();
            ImGui.End();
            if (showWindow)
            {
                //windowRect = GUI.Window(9999, windowRect, DrawInstanceWindow, "Instance Explorer");
            }
        }

        public void DrawLeft()
        {
            var color = new System.Numerics.Vector4(0.6f, 0.6f, 0.6f, 0.65f);

            ImGui.BeginChild("InstanceTableChlid", new System.Numerics.Vector2(ImGui.GetWindowContentRegionWidth() * 0.35f, ImGui.GetWindowHeight()));
            ImGui.Text("Instance List");

            InstanceInfo removeInfo = new InstanceInfo();
            if (ImGui.BeginTable("InstanceTable", 4, tableFlags))
            {
                ImGuiUtils.TableSetupHeaders("Parent", "Type", "Name", "Close");

                foreach (InstanceInfo instance in instanceList)
                {
                    ImGui.TableNextRow();

                    if (instance == curInstance)
                        ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0 + 1, ImGui.GetColorU32(color));

                    ImGuiUtils.TableTextRow(0, instance.parent, instance.type.Name);
                    ImGui.TableSetColumnIndex(2);
                    if (ImGui.Button(instance.name.ToString()))
                    {
                        UpdateView(instance);
                    }

                    ImGui.TableSetColumnIndex(3);
                    if (ImGui.ArrowButton(instance.GetHashCode().ToString(), ImGuiDir.Down))
                    {
                        removeInfo = instance;
                    }
                }
                ImGui.EndTable();
            }
            if (removeInfo != null)
                instanceList.Remove(removeInfo);

            ImGui.EndChild();
        }

        public void DrawRight()
        {
            ImGui.BeginChild("InstanceInfoChlid", new System.Numerics.Vector2(0, ImGui.GetWindowHeight()));

            ImGui.Text(curInstance.type.Name);
            ImGui.SameLine();
            ImGui.Text(curInstance.name);
            foreach (IClassView i in subViews)
            {
                i.DrawView();
            }
            ImGui.EndChild();           
        }

    }
    
}
