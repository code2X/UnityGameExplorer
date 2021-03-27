using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExplorerSpace
{
    public abstract class IView
    {
        public static float midx = 190;
        public abstract ClassInfo GetInfo();
        public abstract void DrawMidView(string className, CsharpClass csharpClass);
        public virtual void DrawRightView(string className, CsharpClass csharpClass) { }
    }

    class MethodView : IView
    {
        Rect midArea = new Rect(IView.midx, 50, 340, 700);
        Vector2 midPos = new Vector2();
        Rect rightArea = new Rect(530, 50, 140, 700);
        Vector2 rightPos = new Vector2();

        string curHookFuncName = string.Empty;

        public override void DrawMidView(string curclassName, CsharpClass curClass)
        {
            GUI.TextField(new Rect(210, 30, 190, 20), curclassName + "函数");

            GUILayout.BeginArea(midArea);
            midPos = GUILayout.BeginScrollView(midPos);
            foreach (var keyValue in curClass.MethodList)
            {
                if (GUILayout.Button(keyValue.Key))
                {
                    curHookFuncName = keyValue.Key;
                    var methdInfo = curClass.MethodList[keyValue.Key];
                    SinglePatch.Patch(ref methdInfo);
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public override void DrawRightView(string className, CsharpClass csharpClass)
        {
            if (curHookFuncName != String.Empty)
                GUI.TextField(new Rect(430, 30, 190, 20), curHookFuncName);
            GUILayout.BeginArea(rightArea);
            rightPos = GUILayout.BeginScrollView(rightPos);
            GUILayout.Label("调用次数: " + SinglePatch.count);
            SinglePatch.block = GUILayout.Toggle(SinglePatch.block, "拦截");
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public override ClassInfo GetInfo()
        {
            return ClassInfo.Method;
        }
    }

    class PropView : IView
    {
        Rect midArea = new Rect(190, 50, 450, 700);
        Vector2 midPos = new Vector2();

        public override void DrawMidView(string curclassName, CsharpClass curClass)
        {
            GUI.TextField(new Rect(210, 30, 190, 20), curclassName + "属性");

            GUILayout.BeginArea(midArea);
            midPos = GUILayout.BeginScrollView(midPos);
            //Title
            GUILayout.BeginHorizontal();
            GUILayout.Label("Type");
            GUILayout.Label("Name");
            GUILayout.EndHorizontal();

            //List
            foreach (var i in curClass.PropList)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Button(i.Value.PropertyType.Name);
                GUILayout.TextField(i.Key);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public override ClassInfo GetInfo()
        {
            return ClassInfo.Property;
        }
    }

    class FieldView : IView
    {
        Rect midArea = new Rect(IView.midx, 50, 450, 700);
        Vector2 midPos = new Vector2();

        public override void DrawMidView(string curclassName, CsharpClass curClass)
        {
            GUI.TextField(new Rect(210, 30, 190, 20), curclassName + "变量");

            GUILayout.BeginArea(midArea);
            midPos = GUILayout.BeginScrollView(midPos);

            //Title
            GUILayout.BeginHorizontal();
            GUILayout.Label("Type");
            GUILayout.Label("Name");
            GUILayout.EndHorizontal();

            //List
            foreach (var i in curClass.FieldList)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Button(i.Value.FieldType.Name);
                GUILayout.TextField(i.Key);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public override ClassInfo GetInfo()
        {
            return ClassInfo.Field;
        }
    }

    public abstract class IInstanceView
    {
        public abstract ClassInfo GetInfo();
        public abstract void DrawMidView(CsharpClass instanceClass, object instance);
        public virtual void DrawRightView(CsharpClass instanceClass, object instance) { }
    }

    class InstanceFieldView : IInstanceView
    {
        Rect midArea = new Rect(190, 50, 450, 700);
        Vector2 midPos = new Vector2();

        public override void DrawMidView(CsharpClass instanceClass, object instance)
        {
            GUI.TextField(new Rect(210, 30, 190, 20), instance.ToString());

            GUILayout.BeginArea(midArea);
            midPos = GUILayout.BeginScrollView(midPos);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Type Name");
            GUILayout.Label("Value");
            GUILayout.EndHorizontal();

            foreach (var i in instanceClass.FieldList)
            {
                if ((CsharpKeyword.GeneralTypes.Contains(i.Value.FieldType) || i.Value.FieldType.IsEnum) && instance != null)
                {
                    //Debug.Log("Instance");
                    object obj = i.Value.GetValue(instance);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(i.Value.FieldType.Name + "  " + i.Key);
                    GUILayout.Button(obj.ToString());
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Button(i.Value.FieldType.Name + "  " + i.Key);
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public override ClassInfo GetInfo() => ClassInfo.Field;
    }

    class InstanceMethodView : IInstanceView
    {
        Rect midArea = new Rect(190, 50, 450, 700);
        Vector2 midPos = new Vector2();

        public override void DrawMidView(CsharpClass instanceClass, object instance)
        {
            GUI.TextField(new Rect(210, 30, 190, 20), instance.ToString());

            GUILayout.BeginArea(midArea);
            midPos = GUILayout.BeginScrollView(midPos);
            GUILayout.BeginHorizontal();
            GUILayout.TextField("ReturnType");
            GUILayout.TextField("Name");
            GUILayout.EndHorizontal();

            foreach (var i in instanceClass.MethodList)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Button(i.Value.ReturnType.Name);
                GUILayout.Button(i.Key);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public override ClassInfo GetInfo() => ClassInfo.Method;
    }

    class InstancePropView : IInstanceView
    {
        Rect midArea = new Rect(190, 50, 450, 700);
        Vector2 midPos = new Vector2();

        public override void DrawMidView(CsharpClass instanceClass, object instance)
        {
            GUI.TextField(new Rect(210, 30, 190, 20), instance.ToString());

            GUILayout.BeginArea(midArea);
            midPos = GUILayout.BeginScrollView(midPos);
            GUILayout.BeginHorizontal();
            GUILayout.TextField("Type");
            GUILayout.TextField("Name");
            GUILayout.EndHorizontal();

            foreach (var i in instanceClass.PropList)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Button(i.Value.PropertyType.Name);
                GUILayout.Button(i.Key);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public override ClassInfo GetInfo() => ClassInfo.Property;
    }

    class StaticMethodView : IView
    {
        Rect midArea = new Rect(190, 50, 240, 700);
        Vector2 midPos = new Vector2();
        Rect rightArea = new Rect(430, 50, 240, 700);
        Vector2 rightPos = new Vector2();

        static CsharpClass curClass;
        static string curFuncName = string.Empty;

        public override void DrawMidView(string curclassName, CsharpClass curClass)
        {
            StaticMethodView.curClass = curClass;
            GUI.TextField(new Rect(210, 30, 190, 20), curclassName + "静态函数");

            //midScrollview.Begin();
            GUILayout.BeginArea(midArea);
            midPos = GUILayout.BeginScrollView(midPos);
            foreach (var i in curClass.StaticMethodList)
            {
                if (GUILayout.Button(i.Key))
                {
                    curFuncName = i.Key;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            //midScrollview.End();
        }

        public override void DrawRightView(string className, CsharpClass csharpClass)
        {
            if (curFuncName != string.Empty)
            {
                var func = curClass.StaticMethodList[curFuncName];

                if (curFuncName != String.Empty)
                    GUI.TextField(new Rect(430, 30, 190, 20), curFuncName);

                GUILayout.BeginArea(rightArea);
                rightPos = GUILayout.BeginScrollView(rightPos);
                GUILayout.Label("返回类型: " + func.ReturnType.Name);
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
        }

        public override ClassInfo GetInfo() => ClassInfo.StaticMethod;
    }

    class StaticPropView : IView
    {
        Rect midArea = new Rect(190, 50, 450, 700);
        Vector2 midPos = new Vector2();
        Rect rightArea = new Rect(430, 50, 240, 700);
        Vector2 rightPos = new Vector2();
        static CsharpClass curClass;
        static string curFuncName = string.Empty;

        public override void DrawMidView(string curclassName, CsharpClass curClass)
        {
            StaticPropView.curClass = curClass;
            curFuncName = string.Empty;
            GUI.TextField(new Rect(210, 30, 190, 20), curclassName + "静态属性");

            //midScrollview.Begin();
            GUILayout.BeginArea(midArea);
            midPos = GUILayout.BeginScrollView(midPos);

            //Title
            GUILayout.BeginHorizontal();
            GUILayout.Label("Static Property");
            GUILayout.Label("Value");
            GUILayout.EndHorizontal();

            foreach (var i in curClass.StaticPropList)
            {
                var func = curClass.StaticPropList[i.Key];
                if (CsharpKeyword.GeneralTypes.Contains(func.ReturnType) && i.Key.StartsWith("get_"))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(i.Value.ReturnType.Name + "  " + i.Key);
                    GUILayout.TextField(func.Invoke(null, null).ToString());
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.TextField(i.Value.ReturnType.Name);
                    if (GUILayout.Button(i.Key))
                    {
                        curFuncName = i.Key;
                        RuntimeExplorer.showInstanceWindow = true;
                        RuntimeExplorer.curInstance = func.Invoke(null, null);
                        RuntimeExplorer.curInstanceName = i.Key;
                    }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public override void DrawRightView(string className, CsharpClass csharpClass)
        {
            if (curFuncName != string.Empty)
            {
                var func = curClass.StaticPropList[curFuncName];

                if (curFuncName != String.Empty)
                    GUI.TextField(new Rect(430, 30, 190, 20), curFuncName);
                if (func.GetParameters().Length == 0)
                {
                    GUILayout.BeginArea(rightArea);
                    rightPos = GUILayout.BeginScrollView(rightPos);
                    GUILayout.Label("返回类型: " + func.ReturnType.Name);
                    GUILayout.Label("值: " + func.Invoke(null, null));
                    GUILayout.EndScrollView();
                    GUILayout.EndArea();
                }
            }
        }

        public override ClassInfo GetInfo() => ClassInfo.StaticProp;
    }

    class StaticFieldView : IView
    {
        Rect midArea = new Rect(190, 50, 450, 700);
        Vector2 midPos = new Vector2();

        public override void DrawMidView(string curclassName, CsharpClass curClass)
        {
            GUI.TextField(new Rect(210, 30, 190, 20), curclassName + "静态变量");

            GUILayout.BeginArea(midArea);
            midPos = GUILayout.BeginScrollView(midPos);

            foreach (var i in curClass.StaticFieldList)
            {
                if (CsharpKeyword.GeneralTypes.Contains(i.Value.FieldType))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Button(i.Value.FieldType.Name + "  " + i.Key);
                    GUILayout.Button(i.Value.GetValue(null).ToString());
                    GUILayout.EndHorizontal();
                }
                else
                {
                    //Enum
                    if(i.Value.FieldType.IsEnum == true)
                    {
                        GUILayout.TextField(i.Key);
                    }
                    else 
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(i.Value.FieldType.Name);
                        if (GUILayout.Button( i.Key))
                            RuntimeExplorer.showInstanceWindow = true;
                        RuntimeExplorer.curInstance = i.Value.GetValue(null);
                        RuntimeExplorer.curInstanceName = i.Key;
                        GUILayout.EndHorizontal();
                    }
                }

            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public override ClassInfo GetInfo() =>  ClassInfo.StaticField;
    }

}
