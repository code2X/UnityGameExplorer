using System;
using ImGuiNET;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Threading;

namespace DotInsideLib
{
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
                ImGuiEx.TableSetupHeaders("Type", "Name");

                foreach (var fieldPair in field_list)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    fieldDrawer.DrawType(fieldPair.Value.FieldType);

                    ImGuiEx.TableTextRow(1, fieldPair.Key);
                }
                ImGui.EndTable();
            }
        }

        public void DrawInstanceFieldTable(
            SortedList<string, FieldInfo> field_list,
            string table_name = "InstanceFieldTable"
            )
        {
            if (field_list.Count == 0) return;

            if (ImGui.BeginTable(table_name, 3, TableFlags))
            {
                ImGuiEx.TableSetupHeaders("Type", "Name", "Value");

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
}
