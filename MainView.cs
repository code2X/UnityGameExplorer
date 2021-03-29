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
using System.Diagnostics;

namespace ExplorerSpace
{
    public abstract class IMainView
    {
        public abstract void OnGUI();
        public virtual void Update() { }
    }

    public abstract class InputWindow<T> : IMainView
    {
        public static bool showWindow = false;
        public static Rect windowArea;

        protected static void SetWindowPos(float x, float y)
        {
            windowArea = new Rect(x, Screen.height - y, 200, 150);
        }
    }

    class MethodInvokeWindow : InputWindow<MethodInvokeWindow>
    {
        public static Rect valueWindowArea;
        public static string valueWindowName = "函数调用";

        public override void OnGUI()
        {
            if (showWindow)
            {
                valueWindowArea = GUI.Window(10002, valueWindowArea, DragValueWindow, valueWindowName);
            }
        }

        void DragValueWindow(int windowID)
        {
        }
    }


    class ValueInputWindow : IMainView
    {
        Rect dragArea = new Rect(0, 0, 10000, 20);
        public static bool showWindow = false;
        public static Rect valueWindowArea;
        public static string valueWindowName = "设置值";
        public static string varName = "";
        public static string varValue = "";
        public static FieldInfo varInfo;
        public static object parentObj = null;
        public static ParserFactory parserFactory = new ParserFactory();

        public ValueInputWindow()
        {
            SetWindowPos(Input.mousePosition.x, Input.mousePosition.y);
        }

        public static void Show()
        {
            varValue = "";
            SetWindowPos(Input.mousePosition.x, Input.mousePosition.y);
            showWindow = true;
        }

        private static void SetWindowPos(float x, float y)
        {
            valueWindowArea = new Rect(x, Screen.height - y, 200, 150);
        }

        public override void OnGUI()
        {
            if (showWindow)
            {
                valueWindowArea = GUI.Window(10001, valueWindowArea, DragValueWindow, valueWindowName);
            }
        }

        void DragValueWindow(int windowID)
        {
            GUI.DragWindow(dragArea);  //Dragable
            GUILayout.Label(varName);
            varValue = GUILayout.TextArea(varValue);

            try
            {
                if (GUILayout.Button("确认"))
                {
                    if (varValue == "")
                    {
                        showWindow = false;
                        return;
                    }

                    bool res = false;

                    var parserFactory = new ParserFactory();
                    var parser = parserFactory.GetParser(varInfo.FieldType);
                    if(parser != null)
                    {
                        object outVal;
                        if(parser.Parse(varValue,out outVal))
                        {
                            varInfo.SetValue(parentObj, outVal);
                            res = true;
                        }
                        else
                        {
                            res = false;
                        }
                    }
                    else
                    {
                        res = false;
                    }

                    if (res == false)
                        Console.WriteLine("Error: Parse failed!!");
                    else
                        Console.WriteLine("Succes: " + varValue);
                }

                if (GUILayout.Button("关闭"))
                    showWindow = false;
            }
            catch (UnityException exp)
            {
                Console.WriteLine("Error: " + exp.Message);
            }
            finally
            {

            }

        }
    }

    class ExplorerView : IMainView
    {
        string FilterText = "";

        ArrayList subViews;
        Rect dragArea = new Rect(0, 0, 10000, 20);
        Rect searchLabelArea = new Rect(10, 30, 50, 20);
        Rect filterTextArea = new Rect(60, 30, 110, 20);
        Rect leftArea = new Rect(10, 50, 180, 700);
        Vector2 leftPos = new Vector2();

        ClassInfo curInfo = ClassInfo.Null;
        CsharpClass curClass = null;
        string curClassName = string.Empty;

        public bool showWindow = false;
        Rect windowRect;
        string windowTitle;

        SortedDictionary<string, SortedDictionary<string, bool>> classCluster;
        SortedList<string, bool> categoryState;

        public ExplorerView()
        {
            subViews = new ArrayList();
            subViews.Add(new MethodView());
            subViews.Add(new PropView());
            subViews.Add(new FieldView());
            subViews.Add(new StaticFieldView());
            subViews.Add(new StaticPropView());
            subViews.Add(new StaticMethodView());

            categoryState = new SortedList<string, bool>();
            classCluster = new SortedDictionary<string, SortedDictionary<string, bool>>();
            windowRect = new Rect(10, 20, 700, 800);
            windowTitle = "Unity Game Explorer : " + Process.GetCurrentProcess().MainModule.ModuleName;
        }

        public override void OnGUI()
        {
            if (showWindow)
            {
                windowRect = GUI.Window(9988, windowRect, DrawWindow, windowTitle);
            }
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.End))
            {
                showWindow = !showWindow;
            }
        }

        public void cluster(SortedDictionary<string, Type> name2type)
        {
            classCluster = DspClassCluster.GetCluster(name2type);
            foreach (var i in classCluster)
            {
                categoryState.Add(i.Key, false);
            }
        }

        private void DrawWindow(int windowID)
        {
            GUI.DragWindow(dragArea);  //Dragable
            GUI.Label(searchLabelArea, "search:");
            FilterText = GUI.TextField(filterTextArea, FilterText);

            DrawLeftView();
            DrawMidView();
            DrawRightView();
        }

        private void DrawLeftView()
        {
            ExplorerUI.BeginSrcollArea(ref leftArea, ref leftPos);

            //Draw category
            GUILayout.Label("自动分类");
            foreach (var state in categoryState)
            {
                bool result = GUILayout.Toggle(state.Value, state.Key);
                categoryState[state.Key] = result;
                if (result)
                {
                    //Draw class
                    foreach (var class2state in classCluster[state.Key])
                    {
                        //Filter class name
                        if (class2state.Key.IndexOf(FilterText) != -1)
                        {
                            ExplorerUI.BeginHorizontal(20);
                            bool res = GUILayout.Toggle(class2state.Value, class2state.Key);
                            ExplorerUI.EndHorizontal();

                            classCluster[state.Key][class2state.Key] = res;
                            if (res)
                                DrawClassStruct(class2state.Key);
                        }
                    }
                }
            }

            ExplorerUI.EndSrcollArea();
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
            if (UnityGameExplorer.classListDetails[className].type.IsEnum)
            {
                ExplorerUI.BeginHorizontal(40);
                if (GUILayout.Button("枚举类型"))
                {
                    curInfo = ClassInfo.StaticField;
                    curClassName = className;
                    curClass = UnityGameExplorer.classListDetails[className];
                }
                ExplorerUI.EndHorizontal();
            }
            else
            {
                foreach (var info in classNameInfo)
                {
                    ExplorerUI.BeginHorizontal(40);
                    if (GUILayout.Button(info.Key))
                    {
                        curInfo = info.Value;
                        curClassName = className;
                        curClass = UnityGameExplorer.classListDetails[className];
                    }
                    ExplorerUI.EndHorizontal();
                }
            }

        }

    }

    class InstanceView : IMainView
    {
        //UI Info
        Rect windowRect = new Rect(100, 20, 700, 800);
        Rect dragRect = new Rect(0, 0, 10000, 20);
        Rect leftArea = new Rect(10, 55, 180, 100);
        Vector2 leftPos = new Vector2();

        Rect instanceArea = new Rect(10, 155, 180, 400);
        Vector2 instancePos = new Vector2();

        struct InstanceInfo
        {
            public object instance;
            public string name;
            public int index;
        }

        static bool showWindow = false;
        static int curIndex = 0;
        ClassInfo curClassInfo = ClassInfo.Null;

       InstanceInfo curInstance
       {
           get
           {
               return instanceList[curIndex];
           }
       }

        ArrayList instanceViews;
        static List<InstanceInfo> instanceList = new List<InstanceInfo>();

        public InstanceView(ArrayList instanceViews)
        {
            this.instanceViews = instanceViews;
        }

        public static void show(string name,object instance)
        {
            instanceList.Clear();
            InstanceInfo instanceInfo = new InstanceInfo();
            instanceInfo.instance = instance;
            instanceInfo.name = name;
            instanceInfo.index = 0;

            instanceList.Add(instanceInfo);

            curIndex = 0;
            showWindow = true;
        }

        public static void add(string name, object instance)
        {
            InstanceInfo instanceInfo = new InstanceInfo();
            instanceInfo.instance = instance;
            instanceInfo.name = name;
            instanceInfo.index = instanceList.Count;

            instanceList.Add(instanceInfo);

            curIndex = instanceInfo.index;
            showWindow = true;
        }

        public override void OnGUI()
        {
            if (showWindow)
            {
                windowRect = GUI.Window(9999, windowRect, DrawInstanceWindow, "Instance Explorer");
            }
        }

        void DrawInstanceWindow(int windowID)
        {
            GUI.DragWindow(dragRect);
            if (instanceList[curIndex].instance != null)
            {
                ExplorerUI.BeginSrcollArea(ref instanceArea, ref instancePos);
                ExplorerUI.Label("实例列表");
                foreach (var i in instanceList)
                {
                    if (ExplorerUI.Button(i.instance.GetType().Name + " " + i.name))
                    {
                        curIndex = i.index;
                    }
                }
                ExplorerUI.EndSrcollArea();

                DrawInstanceLeft();
                DrawInstanceMid();
                DrawInstanceRight();
            }
            else
            {
                GUI.TextField(new Rect(10, 30, 500, 20), "当前对象为空");
            }

            if (GUI.Button(new Rect(600, 30, 80, 20), "关闭"))
            {
                showWindow = false;
            }
        }

        void DrawInstanceLeft()
        {
            ExplorerUI.BeginSrcollArea(ref leftArea, ref leftPos);
            foreach(IInstanceView view in instanceViews)
            {
                if (view.DrawLeftButton())
                {
                    curClassInfo = view.GetInfo();
                }
            }
            ExplorerUI.EndSrcollArea();
        }

        void DrawInstanceMid()
        {
            foreach (IInstanceView view in instanceViews)
                if (view.GetInfo() == curClassInfo)
                    view.DrawMidView(UnityGameExplorer.classListDetails[curInstance.instance.GetType().Name], curInstance.instance);
        }

        void DrawInstanceRight()
        {
            foreach (IInstanceView view in instanceViews)
                if (view.GetInfo() == curClassInfo)
                    view.DrawRightView(UnityGameExplorer.classListDetails[curInstance.instance.GetType().Name], curInstance.instance);
        }

    }

}
