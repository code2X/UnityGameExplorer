using System;
using ImGuiNET;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Threading;

namespace DotInsideLib
{
    public class MethodView : IClassView
    {
        protected static MethodInvokeWindow methodInvokeWindow = MethodInvokeWindow.GetInstance();

        public bool CanInvoke(MethodInfo method) => method.IsStatic == true || GetClassInstance() != null;
        public override ClassInfo GetInfo() => ClassInfo.Method;
        public IMethodDrawer methodDrawer = new DefaultMethodDrawer();

        protected override void Draw()
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

            methodInvokeWindow.OnGUI();
        }

        public void DrawMethodTable(
            SortedList<string, MethodInfo> table,
            string TableName = "ClassMethodTable"
            )
        {
            if (table.Count == 0) return;

            if (ImGui.BeginTable(TableName, 3, TableFlags))
            {
                ImGuiEx.TableSetupHeaders("Return Type", "Name", "Param");

                foreach (var method in table)
                {
                    ImGui.TableNextRow();
                    DrawTableRow(method);
                }
                ImGui.EndTable();
            }
        }

        public void DrawTableRow(KeyValuePair<string, MethodInfo> methodPair)
        {
            ImGui.TableSetColumnIndex(0);
            methodDrawer.DrawType(methodPair.Value.ReturnType);

            ImGui.TableSetColumnIndex(1);
            methodDrawer.DrawMethodName(methodPair.Value, GetClassInstance());

            ImGui.TableSetColumnIndex(2);
            methodDrawer.DrawMethodParams(methodPair.Value, GetClassInstance());
        }
    }
}
