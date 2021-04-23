using System;
using ImGuiNET;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Threading;

namespace DotInsideLib
{
    public class PropertyView : IClassView
    {
        protected static PropertyValueInputWindow valueInputWindow = PropertyValueInputWindow.GetInstance();

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
                ImGuiEx.TableSetupHeaders(
                    "Type",
                    "Name",
                    "Gettable",
                    "Settable");
                foreach (var property in property_list)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    propertyDrawer.DrawType(property.Value.PropertyType);

                    ImGuiEx.TableTextRow(1, property.Key, property.Value.CanRead.ToString(), property.Value.CanWrite.ToString());
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
                ImGuiEx.TableSetupHeaders(
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
                object instance = readable ? propertyPair.Value.GetValue(GetClassInstance(), null) : null;

                ImGui.TableSetColumnIndex(1);
                propertyDrawer.DrawName(
                    propertyPair.Key,
                    propertyPair.Value.PropertyType,
                    GetClass().type,
                    instance);
            });

            ImGui.TableSetColumnIndex(2);
            propertyDrawer.DrawPropertyValue(propertyPair.Value, GetClassInstance());

            ImGuiEx.TableTextRow(
                3,
                propertyPair.Value.CanRead.ToString(),
                propertyPair.Value.CanWrite.ToString());
        }
    }
}
