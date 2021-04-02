using System;
using UnityEngine;
using ImGuiNET;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

namespace ExplorerSpace
{

    public abstract class IClassView
    {
        public static ImGuiTableFlags TableFlags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable;
        public abstract ClassInfo GetInfo();
        protected abstract void Draw();

        public virtual void DrawView()
        {
            if (!show) return;
            Draw();
        }

        public virtual void Show(Type type, object instance = null)
        {
            if(type != curClassType)
            {
                string classTypeName = type.Name;

                if (UnityGameExplorer.classListDetails.ContainsKey(classTypeName) == false)
                {
                    UnityGameExplorer.classListDetails.Add(type.Name, new CsharpClass(type));
                }
                curClass = UnityGameExplorer.classListDetails[classTypeName];              
            }
            curInstance = instance;
            curClassType = type;
            show = true;
        }

        public virtual CsharpClass GetClass() => curClass;
        public virtual object GetClassInstance() => curInstance;

        protected bool show = false;
        CsharpClass curClass;
        Type curClassType;
        object curInstance;
    }

    public class ClassView : IClassView
    {
        List<IClassView> subViews;
        Dictionary<Type, bool> topTabs;

        static ClassView instance = new ClassView();
        public static ClassView GetInstance() => instance;
        public override ClassInfo GetInfo() => throw new NotImplementedException();

        ClassView()
        {
            subViews = new List<IClassView>();
            topTabs = new Dictionary<Type, bool>();
        }

        public void AddSubView(IClassView classView)
        {
            subViews.Add(classView);
        }

        public void Show(Type type)
        {
            TryAddTopTab(type);
            base.Show(type);
            ShowSubViews(type);
        }

        protected override void Draw()
        {
            ImGui.Begin("Class Information", ref show);
            DrawTopTab();
            DrawSubViews();
            ImGui.End();
        }

        void DrawTopTab()
        {
            //store tab state
            var typesState = new Dictionary<Type, bool>();
            if (ImGui.BeginTabBar("ClassInfoViewTopTab"))
            {
                foreach (var type in topTabs)
                {
                    bool opened = topTabs[type.Key];
                    if (opened && ImGui.BeginTabItem(type.Key.Name, ref opened, ImGuiTabItemFlags.None))
                    {
                        if(show) Show(type.Key);
                        ImGui.EndTabItem();
                    }
                    typesState[type.Key] = opened;
                }

                //chage orgianl tab state
                foreach (var type in typesState)
                {
                    if (typesState[type.Key] == false)
                        topTabs.Remove(type.Key);
                }

                ImGui.EndTabBar();
            }

        }

        void TryAddTopTab(Type type)
        {
            if (topTabs.ContainsKey(type) == false)
            {
                topTabs.Add(type, true);
            }
        }

        void ShowSubViews(Type type)
        {
            foreach (IClassView view in subViews)
            {
                view.Show(type);
            }
        }

        void DrawSubViews()
        {
            foreach (IClassView view in subViews)
            {
                view.DrawView();
            }
        }
    }

    public class PropertyView : IClassView
    {
        static PropertyValueInputWindow valueInputWindow = PropertyValueInputWindow.GetInstance();

        public bool PropertyCanRead(PropertyInfo property) => GetClassInstance() != null || GetClass().StaticPropList.ContainsKey(property.Name);
        public override ClassInfo GetInfo() => ClassInfo.Field;
        public IPropertyDrawer propertyDrawer = new DefaultPropertyDrawer();

        protected override void Draw()
        {
            if (ImGui.CollapsingHeader("Property"))
            {
                DrawPropertyTable(GetClass().PropList);
            }
            if (ImGui.CollapsingHeader("Static Property"))
            {
                DrawInstancePropertyTable(GetClass().StaticPropList);
            }
            valueInputWindow.OnGUI();
        }

        public void DrawPropertyTable(
            SortedList<string, PropertyInfo> property_list,
            string table_name = "PropertyTable"
            )
        {
            if (property_list.Count == 0) return;

            if (ImGui.BeginTable(table_name, 4, TableFlags))
            {
                ImGuiUtils.TableSetupHeaders(
                    "Type", 
                    "Name", 
                    "Gettable", 
                    "Settable");
                foreach (var property in property_list)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    propertyDrawer.DrawType(property.Value.PropertyType);

                    ImGuiUtils.TableTextRow(1, property.Key, property.Value.CanRead.ToString(), property.Value.CanWrite.ToString());
                }
                ImGui.EndTable();
            }
        }

        public void DrawInstancePropertyTable(
            SortedList<string, PropertyInfo> property_list,
            string table_name = "StaticPropertyTable"
            )
        {
            if (property_list.Count == 0) return;

            if (ImGui.BeginTable(table_name, 5, TableFlags))
            {
                ImGuiUtils.TableSetupHeaders(
                    "Type", 
                    "Name",
                    "Value", 
                    "Gettable", 
                    "Settable");

                foreach (var propertyPair in property_list)
                {
                    ImGui.TableNextRow();
                    DrawInstanceTableRow(propertyPair);
                }
                ImGui.EndTable();
            }
        }

        public void DrawInstanceTableRow(KeyValuePair<string, PropertyInfo> propertyPair)
        {
            ImGui.TableSetColumnIndex(0);
            propertyDrawer.DrawType(propertyPair.Value.PropertyType);

            Caller.Try(() =>
            {
                bool readable = PropertyCanRead(propertyPair.Value);
                object instance = readable ? propertyPair.Value.GetValue(GetClassInstance()) : null;

                ImGui.TableSetColumnIndex(1);
                propertyDrawer.DrawName(
                    propertyPair.Key,
                    propertyPair.Value.PropertyType,
                    GetClass().type,
                    instance);
            });

            ImGui.TableSetColumnIndex(2);
            propertyDrawer.DrawPropertyValue(propertyPair.Value, GetClassInstance());

            ImGuiUtils.TableTextRow(
                3,
                propertyPair.Value.CanRead.ToString(),
                propertyPair.Value.CanWrite.ToString());
        }
    }

    class FieldView : IClassView
    {
        protected FieldValueInputWindow valueInputWindow = FieldValueInputWindow.GetInstance();

        public override ClassInfo GetInfo() => ClassInfo.Field;
        public IFieldDrawer fieldDrawer = new DefaultFieldDrawer();
        bool FieldCanRead(FieldInfo field) => GetClassInstance() != null || field.IsStatic;

        protected override void Draw()
        {
            if (ImGui.CollapsingHeader("Field"))
            {
                DrawFieldTable(GetClass().FieldList);
            }
            if (ImGui.CollapsingHeader("Static Field"))
            {
                DrawInstanceFieldTable(GetClass().StaticFieldList);
            }
            valueInputWindow.OnGUI();
        }

        public void DrawFieldTable(
            SortedList<string, FieldInfo> field_list,
            string table_name = "FieldTable"
        )
        {
            if (field_list.Count == 0) return;

            if (ImGui.BeginTable(table_name, 2, TableFlags))
            {
                ImGuiUtils.TableSetupHeaders("Type", "Name");

                foreach (var fieldPair in field_list)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    fieldDrawer.DrawType(fieldPair.Value.FieldType);

                    ImGuiUtils.TableTextRow(1, fieldPair.Key);
                }
                ImGui.EndTable();
            }
        }

        public void DrawInstanceFieldTable(
            SortedList<string,FieldInfo> field_list,
            string table_name = "InstanceFieldTable"
            )
        {
            if (field_list.Count == 0) return;

            if (ImGui.BeginTable(table_name, 3, TableFlags))
            {
                ImGuiUtils.TableSetupHeaders("Type", "Name", "Value");

                foreach (var fieldPair in field_list)
                {
                    ImGui.TableNextRow();
                    DrawInstanceTableRow(fieldPair);
                }
                ImGui.EndTable();
            }
        }

        public void DrawInstanceTableRow(KeyValuePair<string, FieldInfo> fieldPair)
        {
            ImGui.TableSetColumnIndex(0);
            fieldDrawer.DrawType(fieldPair.Value.FieldType);

            ImGui.TableSetColumnIndex(1);         
            Caller.Try(() =>
            {
                bool readable = FieldCanRead(fieldPair.Value);
                object instance = readable ? fieldPair.Value.GetValue(GetClassInstance()) : null;

                fieldDrawer.DrawName(
                fieldPair.Key,
                fieldPair.Value.FieldType,
                GetClass().type,
                instance);
            });

            ImGui.TableSetColumnIndex(2);
            fieldDrawer.DrawFieldValue(fieldPair.Value, GetClassInstance());
        }
    }

    public class MethodView : IClassView
    {
        public bool CanInvoke(MethodInfo method) => method.IsStatic == true || GetClassInstance() != null;
        public override ClassInfo GetInfo() => ClassInfo.Method;
        public IMethodDrawer methodDrawer = new DefaultMethodDrawer();

        protected override void Draw( )
        {
            if (ImGui.CollapsingHeader("Method"))
            {
                DrawMethodTable(GetClass().MethodList, "ClassMethod");
                DrawMethodTable(GetClass().GetMethodList, "ClassGetMethod");
                DrawMethodTable(GetClass().SetMethodList, "ClassGetMethod");
            }
            if (ImGui.CollapsingHeader("Static Method"))
            {
                DrawMethodTable(GetClass().StaticMethodList, "InstanceClassMethod");
                DrawMethodTable(GetClass().StaticGetMethodList, "InstanceGetMethod");
                DrawMethodTable(GetClass().StaticSetMethodList, "InstanceGetMethod");
            }
        }

        public void DrawMethodTable(
            SortedList<string,MethodInfo> table,
            string TableName = "ClassMethodTable"
            )
        {
            if (table.Count == 0) return;

            if (ImGui.BeginTable(TableName, 3, TableFlags))
            {
                ImGuiUtils.TableSetupHeaders("Return Type", "Name", "Param");

                foreach (var method in table)
                {
                    ImGui.TableNextRow();
                    DrawTableRow(method);
                }
                ImGui.EndTable();
            }
        }

        public void DrawTableRow(KeyValuePair<string,MethodInfo> methodPair)
        {
            ImGui.TableSetColumnIndex(0);
            methodDrawer.DrawType(methodPair.Value.ReturnType);

            ImGui.TableSetColumnIndex(1);
            methodDrawer.DrawMethodName(methodPair.Value, GetClassInstance());

            ImGui.TableSetColumnIndex(2);
            methodDrawer.DrawMethodParams(methodPair.Value, GetClassInstance());
        }
    }

/// <summary>
/// Instance View
/// </summary>
    public class InstanceMethodView:MethodView
    {
        protected override void Draw()
        {
            if (ImGui.CollapsingHeader("Method"))
            {
                DrawMethodTable(GetClass().MethodList);
                DrawMethodTable(GetClass().GetMethodList);
                DrawMethodTable(GetClass().SetMethodList);
            }
        }
    }

    class InstanceFieldView : FieldView
    {
        protected override void Draw()
        {
            if (ImGui.CollapsingHeader("Field"))
            {
                DrawInstanceFieldTable(GetClass().FieldList);
            }
            valueInputWindow.OnGUI();
        }
    }

    class InstancePropView : PropertyView
    {
        protected override void Draw()
        {
            if (ImGui.CollapsingHeader("Property"))
            {
                DrawInstancePropertyTable(GetClass().PropList);
            }
        }
    }

}
