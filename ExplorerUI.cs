using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ExplorerSpace
{
    class ExplorerUI
    {

        public static void BeginHorizontal(int size)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(size);
        }

        public static void BeginHorizontal()
        {
            GUILayout.BeginHorizontal();
        }

        public static void EndHorizontal()
        {
            GUILayout.EndHorizontal();
        }

        public static void BeginSrcollArea(ref Rect rect, ref Vector2 pos)
        {
            GUILayout.BeginArea(rect);
            pos = GUILayout.BeginScrollView(pos);
        }

        public static void EndSrcollArea()
        {
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public static void Label(params string[] textList)
        {
            foreach (string text in textList)
            {
                GUILayout.Label(text);
            }
        }

        public static string TextField(string txt)
        {
            return GUILayout.TextField(txt);
        }

        public static bool Button(string txt)
        {
            return GUILayout.Button(txt);
        }

        public static void HorizontalText(params string[] textList)
        {
            BeginHorizontal();
            Label(textList);
            EndHorizontal();
        }

        public static void HorizontalLabel(params string[] textList)
        {
            BeginHorizontal();
            foreach (string text in textList)
            {
                GUILayout.Label(text);
            }
            EndHorizontal();
        }
    }
}
