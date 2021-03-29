using System;
using UnityEngine;

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
            ExplorerUI.BeginSrcollArea(ref midArea, ref midPos);
            foreach (var method in curClass.MethodList)
            {
                ExplorerUI.BeginHorizontal();
                ExplorerUI.Label(method.Value.ReturnType.Name);
                if (ExplorerUI.Button(method.Key))
                {
                    curHookFuncName = method.Key;
                    var methdInfo = curClass.MethodList[method.Key];
                    SinglePatch.Patch(ref methdInfo);

                    //method.Value.GetParameters()
                }

                foreach (var param in method.Value.GetParameters())
                {
                    ExplorerUI.Label(param.ToString());
                }
                ExplorerUI.EndHorizontal();
            }
            ExplorerUI.EndSrcollArea();
        }

        public override void DrawRightView(string className, CsharpClass csharpClass)
        {
            if (curHookFuncName != String.Empty)
                GUI.TextField(new Rect(430, 30, 190, 20), curHookFuncName);
            ExplorerUI.BeginSrcollArea(ref rightArea, ref rightPos);
            ExplorerUI.Label("调用次数: " + SinglePatch.count);
            SinglePatch.block = GUILayout.Toggle(SinglePatch.block, "拦截");
            ExplorerUI.EndSrcollArea();
        }

        public override ClassInfo GetInfo() => ClassInfo.Method;
    }

    class PropView : IView
    {
        Rect midArea = new Rect(190, 50, 450, 700);
        Vector2 midPos = new Vector2();

        public override void DrawMidView(string curclassName, CsharpClass curClass)
        {
            GUI.TextField(new Rect(210, 30, 190, 20), curclassName + "属性");

            ExplorerUI.BeginSrcollArea(ref midArea, ref midPos);
            ExplorerUI.HorizontalLabel("Type", "Name");
            foreach (var i in curClass.PropList)
            {
                ExplorerUI.HorizontalText(i.Value.PropertyType.Name, i.Key);
            }
            ExplorerUI.EndSrcollArea();
        }

        public override ClassInfo GetInfo() => ClassInfo.Property;
    }

    class FieldView : IView
    {
        Rect midArea = new Rect(IView.midx, 50, 450, 700);
        Vector2 midPos = new Vector2();

        public override void DrawMidView(string curclassName, CsharpClass curClass)
        {
            GUI.TextField(new Rect(210, 30, 190, 20), curclassName + "变量");

            ExplorerUI.BeginSrcollArea(ref midArea, ref midPos);
            ExplorerUI.HorizontalLabel("Type", "Name");
            foreach (var i in curClass.FieldList)
            {
                ExplorerUI.HorizontalText(i.Value.FieldType.Name, i.Key);
            }
            ExplorerUI.EndSrcollArea();
        }

        public override ClassInfo GetInfo() => ClassInfo.Field;
    }

    class StaticMethodView : IView
    {
        Rect midArea = new Rect(IView.midx, 50, 340, 700);
        Vector2 midPos = new Vector2();

        static CsharpClass curClass;
        static string curFuncName = string.Empty;

        public override void DrawMidView(string curclassName, CsharpClass curClass)
        {
            StaticMethodView.curClass = curClass;
            GUI.TextField(new Rect(210, 30, 190, 20), curclassName + "静态函数");

            //midScrollview.Begin();
            ExplorerUI.BeginSrcollArea(ref midArea, ref midPos);
            foreach (var i in curClass.StaticMethodList)
            {
                if (ExplorerUI.Button(i.Key))
                {
                    curFuncName = i.Key;
                }
            }
            ExplorerUI.EndSrcollArea();
        }

        public override void DrawRightView(string className, CsharpClass csharpClass)
        {
            if (curFuncName != string.Empty)
            {
                var func = curClass.StaticMethodList[curFuncName];

                if (curFuncName != String.Empty)
                    GUI.TextField(new Rect(430, 30, 190, 20), curFuncName);

                ExplorerUI.BeginSrcollArea(ref midArea, ref midPos);
                ExplorerUI.Label("返回类型: " + func.ReturnType.Name);
                ExplorerUI.EndSrcollArea();
            }
        }

        public override ClassInfo GetInfo() => ClassInfo.StaticMethod;
    }

    class StaticPropView : IView
    {
        Rect midArea = new Rect(190, 50, 450, 700);
        Vector2 midPos = new Vector2();

        public override void DrawMidView(string curclassName, CsharpClass curClass)
        {
            GUI.TextField(new Rect(210, 30, 190, 20), curclassName + "静态属性");

            ExplorerUI.BeginSrcollArea(ref midArea, ref midPos);

            //Title
            ExplorerUI.HorizontalLabel("Static Property", "Value");

            foreach (var i in curClass.StaticPropList)
            {
                var func = curClass.StaticPropList[i.Key];
                if (CsharpKeyword.GeneralTypes.Contains(func.ReturnType) && i.Key.StartsWith("get_"))
                {
                    ExplorerUI.HorizontalLabel(i.Value.ReturnType.Name + "  " + i.Key, func.Invoke(null, null).ToString());
                }
                else
                {
                    ExplorerUI.BeginHorizontal();
                    ExplorerUI.TextField(i.Value.ReturnType.Name);
                    if (i.Key.StartsWith("get_"))
                    {
                        if(GUILayout.Button(i.Key))
                            InstanceView.show(i.Key, func.Invoke(null, null));
                    }
                    else
                    {
                        ExplorerUI.TextField(i.Key);
                    }
                    ExplorerUI.EndHorizontal();
                }
            }

            ExplorerUI.EndSrcollArea();
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

            ExplorerUI.BeginSrcollArea(ref midArea, ref midPos);
            foreach (var i in curClass.StaticFieldList)
            {
                //General Type
                if (CsharpKeyword.GeneralTypes.Contains(i.Value.FieldType))
                {
                    ExplorerUI.BeginHorizontal();
                    ExplorerUI.Label(i.Value.FieldType.Name, i.Key);

                    //Const value
                    if (i.Value.IsLiteral)
                    {
                        ExplorerUI.TextField(i.Value.GetValue(null).ToString());
                    }
                    //General value
                    else if(ExplorerUI.Button(i.Value.GetValue(null).ToString()))
                    {                     
                        ValueInputWindow.varName = i.Key;
                        ValueInputWindow.varInfo = i.Value;
                        ValueInputWindow.parentObj = null;
                        ValueInputWindow.Show();
                    }

                    ExplorerUI.EndHorizontal();
                }
                else if(i.Value.FieldType.IsEnum)
                {
                    ExplorerUI.HorizontalText(i.Value.FieldType.Name, i.Key);
                }
                //Custom Type
                else
                {
                    ExplorerUI.BeginHorizontal();
                    ExplorerUI.Button(i.Value.FieldType.Name);
                    if (ExplorerUI.Button( i.Key))
                    {
                        InstanceView.show(i.Key, i.Value.GetValue(null));
                    }
                    ExplorerUI.EndHorizontal();
                }

            }
            ExplorerUI.EndSrcollArea();
        }
        public override ClassInfo GetInfo() =>  ClassInfo.StaticField;
    }

}
