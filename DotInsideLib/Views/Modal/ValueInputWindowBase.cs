using ImGuiNET;
using System.Reflection;
using System;

namespace DotInsideLib
{
    public abstract class ValueInputWindowBase : IParamInputModalView
    {
        public override string GetPopupName() => "Set Value";
        protected object parentObj = null;
        protected bool errored = false;
        protected string inputText = "";

        public void doSuccess()
        {
            CloseWindow();
        }

        public void DrawTable(string tableName, Type type, string name, bool errored = false)
        {
            ImGuiEx.TableView(tableName, () =>
            {
                ImGui.TableNextRow();
                paramTable.DrawRow(type, name, ref inputText, errored);
            }, "Type", "Name", "Value", "Error");
        }

        public void Reset()
        {
            CloseWindow();
            this.parentObj = null;
            this.errored = false;
            this.inputText = "";
        }
    }
}
