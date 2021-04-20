using System;

namespace ImGuiNET
{
    public class ImGuiUtils
    {
        //public static void BeginTableWithHeaders(string tableName,params string[] strs)
        //{
        //    ImGui.BeginTable(tableName, strs.Length);
        //
        //    ImGui.TableSetupScrollFreeze(0, 1); // Make top row always visible
        //    foreach (string str in strs)
        //    {
        //        ImGui.TableSetupColumn(str);
        //    }
        //    ImGui.TableHeadersRow();
        //}

        public static void TableSetupHeaders()
        {
            ImGui.TableSetupScrollFreeze(0, 1); // Make top row always visible
            ImGui.TableHeadersRow();
        }

        public static void TableSetupHeaders(params string[] strs)
        {
            ImGui.TableSetupScrollFreeze(0, 1); // Make top row always visible
            foreach (string str in strs)
            {
                ImGui.TableSetupColumn(str);
            }
            ImGui.TableHeadersRow();
        }

        public static void TableTextRow(int beginColumn, params string[] strs)
        {
            for(int i = 0; i < strs.Length; ++i)
            {
                ImGui.TableSetColumnIndex(beginColumn + i);
                ImGui.Text(strs[i]);
            }
        }

        public static void TableSetupColumn(params string[] strs)
        {
            foreach (string str in strs)
            {
                ImGui.TableSetupColumn(str);
            }
        }

        public static void TableView(string name, Action callback, params string[] headers)
        {
            TableView(name, callback, ImGuiTableFlags.None, headers);
        }

        public static void PopupContextItemView(Action callback)
        {
            if (ImGui.BeginPopupContextItem())
            {
                callback?.Invoke();
                ImGui.EndPopup();
            }
        }

        public static void TableColumns(params Action[] callbacks)
        {
            int len = callbacks.Length;
            for(int i=0; i < len; ++i)
            {
                ImGui.TableNextColumn();
                callbacks[i]?.Invoke();
            }
        }

        public static void TableView(string str_id, Action callback, ImGuiTableFlags flags, params string[] headers)
        {
            int len = headers.Length;
            if (ImGui.BeginTable(str_id, len, flags))
            {
                ImGuiUtils.TableSetupHeaders(headers);

                callback?.Invoke();
                ImGui.EndTable();
            }
        }

        public static void ComboView(string str_id, Action callback,string preview_value)
        {
            if (ImGui.BeginCombo(str_id, preview_value))
            {
                callback?.Invoke();
                ImGui.EndCombo();
            }
        }

        public static void ComboView(string str_id, Action callback, string preview_value, ImGuiComboFlags flags)
        {
            if (ImGui.BeginCombo(str_id, preview_value, flags))
            {
                callback?.Invoke();
                ImGui.EndCombo();
            }
        }
    }
}
