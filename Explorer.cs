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
    public enum ClassInfo
    {
        Null,
        Field,
        Property,
        Method,
        StaticField,
        StaticProp,
        StaticMethod
    }

    //class ClassInstance
    //{
    //    public static Dictionary<string, object> InstanceList = new Dictionary<string, object>();
    //}

    

    [BepInPlugin("UnityGameExplorer", "Unity Game Explorer", "1.0")]
    public class RuntimeExplorer : BaseUnityPlugin
    {
        //public static bool showExplorerWindow = false;
        public static bool showInstanceWindow = false;
        public static object curInstance = null;
        public static string curInstanceName;

        //ClassInfo curInfo = ClassInfo.Null;
        //CsharpClass curClass = null;
        //string curClassName = string.Empty;

        ArrayList views;
        ArrayList instanceViews;

        ExplorerView explorerView;

        AssemblyClass g_Assembly;
        //SortedList<string, bool> classCategoryState;
        SortedDictionary<string, Type> g_ClassName2Type;

        public static SortedList<string, CsharpClass> classListDetails;
        public static SortedDictionary<string, SortedDictionary<string, bool>> classCluster;

        public static ConfigEntry<string> g_DllPathConfig;

        void Start()
        {
            //classCategoryState = new SortedList<string, bool>();
            classListDetails = new SortedList<string, CsharpClass>();

            //Add Explorer View

            views = new ArrayList();
            views.Add(new MethodView());
            views.Add(new PropView());
            views.Add(new FieldView());
            views.Add(new StaticFieldView());
            views.Add(new StaticPropView());
            views.Add(new StaticMethodView());

            explorerView = new ExplorerView(views);

            //Add Instance View
            instanceViews = new ArrayList();
            instanceViews.Add(new InstanceFieldView());
            instanceViews.Add(new InstancePropView());
            instanceViews.Add(new InstanceMethodView());

            Thread t = new Thread(Init);
            t.Start();
        }

        void Update()
        {
            explorerView.Update();

            //if (Input.GetKeyDown(KeyCode.End))
            //{
            //    showExplorerWindow = !showExplorerWindow;
            //}
        }

        void Init()
        {
            g_DllPathConfig = Config.Bind<string>("Explorer", "DllPath", "DSPGAME_Data\\Managed\\Assembly-CSharp.dll", "Assembly Path");

            try
            {
                g_Assembly = new AssemblyClass(g_DllPathConfig.Value);
                g_ClassName2Type = g_Assembly.getTypeDict();
                classCluster = DspClassCluster.GetCluster(g_ClassName2Type);
                foreach (var i in classCluster)
                {
                    explorerView.categoryState.Add(i.Key, false);
                    //classCategoryState.Add(i.Key, false);
                }

                foreach (var cls in g_ClassName2Type)
                {
                    classListDetails.Add(cls.Key, new CsharpClass(cls.Value));
                }
            }
            catch(Exception exp)
            {
                Debug.LogError(exp.Message);
            }
        }

        public Rect explorerRect = new Rect(10, 20, 700, 800);
        public Rect instanceRect = new Rect(100, 20, 700, 800);
        void OnGUI()
        {
            explorerView.OnGUI();
            //if (showExplorerWindow)
            //{
            //    explorerRect = GUI.Window(9988, explorerRect, DrawExplorerWindow, "Unity Game Explorer");
            //}         
            if (showInstanceWindow)
            {
                instanceRect = GUI.Window(9999, instanceRect, DrawInstanceWindow, "Instance Explorer");
            }
        }

        //string curText = "";
        //void DrawExplorerWindow(int windowID)
        //{
        //    GUI.DragWindow(new Rect(0, 0, 10000, 20));
        //    curText = GUI.TextField(new Rect(10, 30, 170, 20), curText);
        //
        //    DrawLeftView();
        //    DrawMidView();
        //    DrawRightView();
        //}
        //
        void DrawInstanceWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            if (curInstance != null)
            {
                GUI.Button(new Rect(10, 30, 170, 20), curInstance.GetType().Name + " " + curInstanceName);
            }
            else
            {
                GUI.Button(new Rect(10, 30, 170, 20), "nullptr");
            }

            if (GUI.Button(new Rect(600, 30, 80, 20), "关闭"))
            {
                showInstanceWindow = false;
            }
            DrawInstanceLeft();
            DrawInstanceMid();
            DrawInstanceRight();
        }

        ClassInfo instanceInfo = ClassInfo.Null;
        Rect instanceLeftArea = new Rect(10, 50, 180, 700);
        Vector2 instanceLeftPos = new Vector2();
        void DrawInstanceLeft()
        {
            GUILayout.BeginArea(instanceLeftArea);
            instanceLeftPos = GUILayout.BeginScrollView(instanceLeftPos);
            if (GUILayout.Button("属性"))
            {
                instanceInfo = ClassInfo.Property;
            }
            else if (GUILayout.Button("变量"))
            {
                instanceInfo = ClassInfo.Field;
            }
            else if (GUILayout.Button("方法"))
            {
                instanceInfo = ClassInfo.Method;
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        void DrawInstanceMid()
        {
            foreach (IInstanceView view in instanceViews)
            {
                if (view.GetInfo() == instanceInfo)
                {
                    view.DrawMidView(classListDetails[curInstance.GetType().Name], curInstance);
                }
            }
        }

        void DrawInstanceRight()
        {
            foreach (IInstanceView view in instanceViews)
            {
                if (view.GetInfo() == instanceInfo)
                {
                    view.DrawRightView(classListDetails[curInstance.GetType().Name], curInstance);
                }
            }
        }
        //struct NameInfo
        //{
        //    string name;
        //    ClassInfo info;
        //}
        //static Dictionary<string, ClassInfo> classNameInfo = new Dictionary<string, ClassInfo> {
        //    { "属性", ClassInfo.Property },
        //    { "变量", ClassInfo.Field },
        //    { "方法", ClassInfo.Method },
        //    { "静态属性", ClassInfo.StaticProp },
        //    { "静态变量", ClassInfo.StaticField },
        //    { "静态方法", ClassInfo.StaticMethod },
        //};
        //void DrawClassStruct(string className)
        //{
        //    //Enum class
        //    if (classListDetails[className].type.IsEnum)
        //    {
        //        GUILayout.BeginHorizontal();
        //        GUILayout.Space(40);
        //        if (GUILayout.Button("枚举类型"))
        //        {
        //            curInfo = ClassInfo.StaticField;
        //            curClassName = className;
        //            curClass = classListDetails[className];
        //        }
        //        GUILayout.EndHorizontal();
        //    }
        //    else
        //    {
        //        foreach (var info in classNameInfo)
        //        {
        //            GUILayout.BeginHorizontal();
        //            GUILayout.Space(40);
        //            if (GUILayout.Button(info.Key))
        //            {
        //                curInfo = info.Value;
        //                curClassName = className;
        //                curClass = classListDetails[className];
        //            }
        //            GUILayout.EndHorizontal();
        //        }
        //    }
        //
        //}

        //Rect explorerLeftArea = new Rect(10, 50, 180, 700);
        //Vector2 explorerLeftPos = new Vector2();
        //void DrawLeftView()
        //{
        //    GUILayout.BeginArea(explorerLeftArea);
        //    explorerLeftPos = GUILayout.BeginScrollView(explorerLeftPos);
        //
        //    //Draw category
        //    foreach (var category2state in classCategoryState)
        //    {
        //        bool result = GUILayout.Toggle(category2state.Value, category2state.Key);
        //        classCategoryState[category2state.Key] = result;
        //        if(result)
        //        {
        //            //Draw class
        //            foreach (var class2state in classCluster[category2state.Key])
        //            {
        //                //Filter class name
        //                if ( class2state.Key.IndexOf(curText) != -1)
        //                {
        //                    GUILayout.BeginHorizontal();
        //                    GUILayout.Space(20);
        //                    bool res = GUILayout.Toggle(class2state.Value, class2state.Key);
        //                    GUILayout.EndHorizontal();
        //
        //                    classCluster[category2state.Key][class2state.Key] = res;
        //                    if (res)
        //                        DrawClassStruct(class2state.Key);
        //                }
        //            }
        //        }
        //    }
        //
        //    GUILayout.EndScrollView();
        //    GUILayout.EndArea();
        //}

        //void DrawMidView()
        //{
        //    foreach(IView view in views)
        //    {
        //        if(view.GetInfo() == curInfo)
        //        {
        //            view.DrawMidView(curClassName, curClass);
        //        }
        //    }
        //}
        //
        //void DrawRightView()
        //{
        //    foreach (IView view in views)
        //    {
        //        if (view.GetInfo() == curInfo)
        //        {
        //            view.DrawRightView(curClassName, curClass);
        //        }
        //    }
        //}


    }
}

