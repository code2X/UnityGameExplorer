using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ImGuiNET;

namespace ExplorerSpace
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

        public delegate void VoidFunc();

        public static void TableView(string name, VoidFunc callback, params string[] headers)
        {
            TableView(name, callback, ImGuiTableFlags.None, headers);
        }

        public static void TableView(string name, VoidFunc callback, ImGuiTableFlags flags, params string[] headers)
        {
            int len = headers.Length;
            if (ImGui.BeginTable(name, len, flags))
            {
                ImGuiUtils.TableSetupHeaders(headers);

                callback();
                ImGui.EndTable();
            }
        }

        public static void TableColumns(params VoidFunc[] callbacks)
        {
            int len = callbacks.Length;
            for(int i=0; i < len; ++i)
            {
                ImGui.TableSetColumnIndex(i);
                callbacks[i]();
            }
        }
    }
}
