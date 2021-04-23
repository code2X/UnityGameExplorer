using ImGuiNET;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;

namespace DotInsideLib
{
    public class InstanceView : IWindowView
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

        public override string WindowName => "Instance Explorer";
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

            //Add("GameMain", "none", typeof(GameMain), null);
        }

        void UpdateView(InstanceInfo instanceInfo)
        {
            curInstance = instanceInfo;
            foreach (IClassView i in subViews)
            {
                i.Show(curInstance.type, curInstance.instance);
            }
        }

        public void Add(string parent, string name, Type type, object instance = null)
        {
            InstanceInfo instanceInfo = new InstanceInfo();
            instanceInfo.instance = instance;
            instanceInfo.name = name;
            instanceInfo.type = type;
            instanceInfo.parent = parent;

            if (instanceList.Add(instanceInfo))
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
            var color = new Vector4(0.6f, 0.6f, 0.6f, 0.65f);
            ImGui.BeginChild("InstanceTableChlid", new Vector2(ImGui.GetWindowContentRegionWidth() * 0.35f, ImGui.GetWindowHeight()));
            ImGui.Text("Instance List");

            InstanceInfo removeInfo = new InstanceInfo();
            ImGuiEx.TableView("InstanceTable", () =>
            {
                foreach (InstanceInfo instance in instanceList)
                {
                    ImGui.TableNextRow();

                    if (instance == curInstance)
                        ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0 + 1, ImGui.GetColorU32(color));

                    ImGuiEx.TableTextRow(0, instance.parent, instance.type.Name);

                    ImGui.TableSetColumnIndex(2);
                    if (ImGui.Button(instance.name.ToString()))
                    {
                        UpdateView(instance);
                        break;  //prevent error
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
            ImGui.BeginChild("InstanceInfoChlid", new Vector2(0, ImGui.GetWindowHeight()));

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
