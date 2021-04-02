using System;
using UnityEngine;
using ImGuiNET;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

namespace ExplorerSpace
{
    public abstract class ITypeDrawer
    {
        public bool IsGeneralType(Type type) => CsharpKeyword.GeneralTypes.Contains(type);
        public virtual void DrawType(Type type) { }
        public virtual void DrawName(string name, Type type, Type parent, object instance = null) { }
    }

    public abstract class IFieldDrawer : ITypeDrawer
    {
        public virtual void DrawFieldValue(FieldInfo field, object instance = null) { }
    }

    public abstract class IPropertyDrawer : ITypeDrawer
    {
        public virtual void DrawPropertyValue(PropertyInfo property, object instance = null) { }
    }

    public abstract class IMethodDrawer : ITypeDrawer
    {
        public virtual void DrawMethodName(MethodInfo method, object instance = null) { }
        public virtual void DrawMethodParams(MethodInfo method, object instance = null) { }
    }

    public abstract class IArrayDrawer : ITypeDrawer
    {
        public virtual void DrawArrayIndex(int index) { }
        public virtual void DrawArrayValue(Array array, object element, int index) { }
    }

    public class DefaultTypeDrawer : ITypeDrawer
    {
        System.Numerics.Vector4 color = new System.Numerics.Vector4(0.1f, 0.1f, 0.1f, 0.65f);

        public override void DrawType(Type type)
        {
            if (type.IsArray)
            {
                ImGui.Button(type.Name);
            }
            else if (IsGeneralType(type) == false)
            {
                ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0 + 1, ImGui.GetColorU32(color));
                if (ImGui.Button(type.Name))
                {
                    ClassView.GetInstance().Show(type);
                }
            }
            else
            {
                ImGui.Text(type.Name);
            }
        }

        public override void DrawName(string name, Type type, Type parent, object instance = null)
        {
            if(type.IsArray)
            {
                
                if (ImGui.Button(name) && instance != null)
                {
                    //instance = new Mesh[5];
                    ArrayInfoWindow.GetInstance().Show(instance, name);
                }
            }
            else if (IsGeneralType(type) == false && type.IsEnum == false)
            {
                if (ImGui.Button(name) && instance != null)
                {
                    InstanceView.GetInstance().Add(parent.Name, name, type, instance);
                }
            }
            else
            {
                ImGui.Text(name);
            }
        }
    }

    public class DefaultArrayDrawer : IArrayDrawer
    {
        ITypeDrawer typeDrawer = new DefaultTypeDrawer();

        public override void DrawType(Type type)
        {
            typeDrawer.DrawType(type);
        }

        public override void DrawName(string name, Type type, Type parent, object instance = null)
        {
            typeDrawer.DrawName(name, type, parent, instance);
        }

        public override void DrawArrayIndex(int index) 
        {
            ImGui.Text(index.ToString());
        }

        public override void DrawArrayValue(Array array, object element, int index)
        {
            if (ImGui.Button(element.ToString()))
            {
                ArrayElementInputWindow.GetInstance().Show(array, element, index);
            }
        }
    }

    public class DefaultFieldDrawer : IFieldDrawer
    {
        ITypeDrawer typeDrawer = new DefaultTypeDrawer();
        static FieldValueInputWindow valueInputWindow = FieldValueInputWindow.GetInstance();

        public override void DrawType(Type type)
        {
            typeDrawer.DrawType(type);
        }

        public override void DrawName(string name, Type type, Type parent, object instance = null)
        {
            typeDrawer.DrawName(name, type, parent, instance);
        }

        public override void DrawFieldValue(FieldInfo field, object instance = null)
        {
            if (IsGeneralType(field.FieldType) == false)
                return;
            if (instance == null && field.IsStatic == false)
                return;

            Caller.Try(() =>
            {
                string value = field.GetValue(instance).ToString();

                if (field.IsLiteral)
                {
                    ImGui.Text(value);
                }
                else if (ImGui.Button(value))
                {
                    Console.WriteLine("Showsdd");

                    valueInputWindow.Show(field, instance);
                }
            });
        }

    }

    public class DefaultPropertyDrawer : IPropertyDrawer
    {
        ITypeDrawer typeDrawer = new DefaultTypeDrawer();
        static PropertyValueInputWindow valueInputWindow = PropertyValueInputWindow.GetInstance();
        public bool IsPropertyStatic(PropertyInfo property) => property.GetAccessors().Length > 0 && property.GetAccessors()[0].IsStatic;

        public override void DrawType(Type type)
        {
            typeDrawer.DrawType(type);
        }

        public override void DrawName(string name, Type type, Type parent, object instance = null)
        {
            typeDrawer.DrawName(name, type, parent, instance);
        }

        public override void DrawPropertyValue(PropertyInfo property, object instance = null)
        {
            if (IsGeneralType(property.PropertyType) == false)
                return;

            if (property.CanRead && (instance != null || IsPropertyStatic(property)))
            {
                Caller.Try(() =>
                {
                    ImGui.TableSetColumnIndex(2);
                    string value = property.GetValue(instance).ToString();
                    if( property.CanWrite == false)
                    {
                        ImGui.Text(value);
                    }
                    else if (ImGui.Button(value))
                    {
                        valueInputWindow.Show(property, instance);
                    }
                });
            }
        }
    }

    public class DefaultMethodDrawer : IMethodDrawer
    {
        ITypeDrawer typeDrawer = new DefaultTypeDrawer();
        public bool CanInvoke(MethodInfo method,object instance) => method.IsStatic == true || instance != null;

        public override void DrawType(Type type)
        {
            typeDrawer.DrawType(type);
        }

        public override void DrawName(string name, Type type, Type parent, object instance = null)
        {
            typeDrawer.DrawName(name, type, parent, instance);
        }

        public override void DrawMethodName(MethodInfo method, object instance = null)
        {
            string methodName = method.Name;

            if (ImGui.Button(methodName))
            {
                ImGui.OpenPopup(methodName);
            }

            DrawMethodMenu(methodName, method, instance);
        }

        void DrawMethodMenu(string name, MethodInfo method, object instance = null)
        {
            if (ImGui.BeginPopup(name))
            {
                if (ImGui.Button("Call"))
                {
                    if (CanInvoke(method,instance))
                    {
                        MethodInvokeWindow.GetInstance().Show(method, method.GetParameters(), instance);
                    }
                    else
                    {
                        ImGui.CloseCurrentPopup();
                    }
                }
                ImGui.Button("Hook");
                ImGui.EndPopup();
            }
        }

        public override void DrawMethodParams(MethodInfo method, object instance = null)
        {
            ImGui.Text("(");
            var parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; ++i)
            {
                ImGui.SameLine();
                if (i != parameters.Length - 1)
                {
                    ImGui.Text(parameters[i].ParameterType.Name + " " + parameters[i].Name + ",");
                }
                else
                {
                    ImGui.Text(parameters[i].ParameterType.Name);
                }
            }
            ImGui.SameLine();
            ImGui.Text(")");
        }


        public void DrawRightView(string className, CsharpClass csharpClass)
        {
            //if (curHookFuncName != String.Empty)
            //    GUI.TextField(new Rect(430, 30, 190, 20), curHookFuncName);
            //ExplorerUI.BeginSrcollArea(ref rightArea, ref rightPos);
            //ExplorerUI.Label("调用次数: " + SinglePatch.count);
            //SinglePatch.block = GUILayout.Toggle(SinglePatch.block, "拦截");
            //ExplorerUI.EndSrcollArea();
        }
    }


}
