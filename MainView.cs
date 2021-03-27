using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System;
using System.Threading;

namespace ExplorerSpace
{
    public abstract class IMainView
    {
        public abstract void OnGUI();
        public virtual void Update() { }
    }

    class ExplorerView : IMainView
    {
        string FilterText = "";

        ArrayList subViews;
        Rect explorerLeftArea = new Rect(10, 50, 180, 700);
        Vector2 explorerLeftPos = new Vector2();

        ClassInfo curInfo = ClassInfo.Null;
        CsharpClass curClass = null;
        string curClassName = string.Empty;

        public SortedList<string, bool> categoryState;

        public bool showWindow = false;
        Rect windowRect;

        public ExplorerView(ArrayList viewList)
        {
            subViews = viewList;
            categoryState = new SortedList<string, bool>();
            windowRect = new Rect(10, 20, 700, 800);
        }

        public override void OnGUI()
        {
            if (showWindow)
            {
                windowRect = GUI.Window(9988, windowRect, DrawWindow, "Unity Game Explorer");
            }
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.End))
            {
                showWindow = !showWindow;
            }
        }

        private void DrawWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));  //Dragable
            FilterText = GUI.TextField(new Rect(10, 30, 170, 20), FilterText);

            DrawLeftView();
            DrawMidView();
            DrawRightView();
        }

        private void DrawLeftView()
        {
            GUILayout.BeginArea(explorerLeftArea);
            explorerLeftPos = GUILayout.BeginScrollView(explorerLeftPos);

            //Draw category
            foreach (var category2state in categoryState)
            {
                bool result = GUILayout.Toggle(category2state.Value, category2state.Key);
                categoryState[category2state.Key] = result;
                if (result)
                {
                    //Draw class
                    foreach (var class2state in RuntimeExplorer.classCluster[category2state.Key])
                    {
                        //Filter class name
                        if (class2state.Key.IndexOf(FilterText) != -1)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            bool res = GUILayout.Toggle(class2state.Value, class2state.Key);
                            GUILayout.EndHorizontal();

                            RuntimeExplorer.classCluster[category2state.Key][class2state.Key] = res;
                            if (res)
                                DrawClassStruct(class2state.Key);
                        }
                    }
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawMidView()
        {
            foreach (IView view in subViews)
            {
                if (view.GetInfo() == curInfo)
                {
                    view.DrawMidView(curClassName, curClass);
                }
            }
        }

        private void DrawRightView()
        {
            foreach (IView view in subViews)
            {
                if (view.GetInfo() == curInfo)
                {
                    view.DrawRightView(curClassName, curClass);
                }
            }
        }

        static Dictionary<string, ClassInfo> classNameInfo = new Dictionary<string, ClassInfo> {
            { "属性", ClassInfo.Property },
            { "变量", ClassInfo.Field },
            { "方法", ClassInfo.Method },
            { "静态属性", ClassInfo.StaticProp },
            { "静态变量", ClassInfo.StaticField },
            { "静态方法", ClassInfo.StaticMethod },
        };
        private void DrawClassStruct(string className)
        {
            //Enum class
            if (RuntimeExplorer.classListDetails[className].type.IsEnum)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(40);
                if (GUILayout.Button("枚举类型"))
                {
                    curInfo = ClassInfo.StaticField;
                    curClassName = className;
                    curClass = RuntimeExplorer.classListDetails[className];
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                foreach (var info in classNameInfo)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40);
                    if (GUILayout.Button(info.Key))
                    {
                        curInfo = info.Value;
                        curClassName = className;
                        curClass = RuntimeExplorer.classListDetails[className];
                    }
                    GUILayout.EndHorizontal();
                }
            }

        }
    }
}
