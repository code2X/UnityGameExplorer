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

    public abstract class IWindowView : IView
    {
        protected bool showWindow = false;

        public override void OnGUI()
        {
            if (!showWindow) return;

            ImGui.Begin(GetWindowName(), ref showWindow, GetWindowFlags());
            DrawWindowContent();
            ImGui.End();
        }

        public virtual ImGuiWindowFlags GetWindowFlags() => ImGuiWindowFlags.None;
        public virtual string GetWindowName() => "ModalWindow";
        public abstract void DrawWindowContent();
    }

    public class ISingletonWindowView<T>: IWindowView
    {
        static ISingletonWindowView<T> instance = new ISingletonWindowView<T>();
        public static ISingletonWindowView<T> GetInstance() => instance;

        public override void DrawWindowContent() => throw new NotImplementedException();
    }

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

    class AssemblyExplorerView : IWindowView
    {
        public override string GetWindowName() => "Assembly Explorer";
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
            ImGui.InputTextWithHint("","type to search", ref searchText, 20);

            ImGui.Text("Main Class");
            DrawClassList(clusteringClass.main, "MainClassTable");

            ImGui.Text("Auto Cluster Class");
            DrawClassList(clusteringClass.auto, "OtherClassTable");
            DrawClassInfo();
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
                if (ImGui.CollapsingHeader(state.Key))
                {
                    ImGuiUtils.TableView(table_name, () =>
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

        void DrawTableRow(KeyValuePair<string,Type> class2type)
        {
            ImGuiUtils.TableColumns(() =>
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

    class InstanceView : IWindowView
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

        public override string GetWindowName() => "Instance Explorer";
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

        public override void DrawWindowContent()
        {
            DrawLeft();
            ImGui.SameLine();
            DrawRight();
        }

        public void DrawLeft()
        {
            var color = new System.Numerics.Vector4(0.6f, 0.6f, 0.6f, 0.65f);

            ImGui.BeginChild("InstanceTableChlid", new System.Numerics.Vector2(ImGui.GetWindowContentRegionWidth() * 0.35f, ImGui.GetWindowHeight()));
            ImGui.Text("Instance List");

            InstanceInfo removeInfo = new InstanceInfo();
            ImGuiUtils.TableView("InstanceTable", () =>
            {
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
            }, tableFlags, "Parent", "Type", "Name", "Close");

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
