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
        public static void BeginTableWithHeaders(string tableName,params string[] strs)
        {
            ImGui.BeginTable(tableName, strs.Length);

            ImGui.TableSetupScrollFreeze(0, 1); // Make top row always visible
            foreach (string str in strs)
            {
                ImGui.TableSetupColumn(str);
            }
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
    }

    class ExplorerUI
    {

        //public static void BeginHorizontal(int size)
        //{
        //    GUILayout.BeginHorizontal();
        //    GUILayout.Space(size);
        //}
        //
        //public static void BeginHorizontal()
        //{
        //    GUILayout.BeginHorizontal();
        //}
        //
        //public static void EndHorizontal()
        //{
        //    GUILayout.EndHorizontal();
        //}
        //
        //public static void BeginSrcollArea(ref Rect rect, ref Vector2 pos)
        //{
        //    GUILayout.BeginArea(rect);
        //    pos = GUILayout.BeginScrollView(pos);
        //}
        //
        //public static void EndSrcollArea()
        //{
        //    GUILayout.EndScrollView();
        //    GUILayout.EndArea();
        //}

        public static void Label(params string[] textList)
        {
            foreach (string text in textList)
            {
                ImGui.LabelText(text, "");
            }
        }

        public static string TextField(string txt)
        {
            ImGui.Text(txt);
            return txt;
        }

        public static bool Button(string txt)
        {
            return ImGui.Button(txt);
        }
    }
}
