using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace ExplorerSpace
{
    public abstract class IInstanceView
    {
        public abstract ClassInfo GetInfo();
        public virtual bool DrawLeftButton() { return false;  }
        public abstract void DrawMidView(CsharpClass instanceClass, object instance);
        public virtual void DrawRightView(CsharpClass instanceClass, object instance) { }
    }

    class InstanceFieldView : IInstanceView
    {
        Rect midArea = new Rect(190, 50, 450, 700);
        Vector2 midPos = new Vector2();

        public override bool DrawLeftButton()
        {
            if (GUILayout.Button("变量"))
            {
                return true;
            }
            return false;
        }

        public override void DrawMidView(CsharpClass instanceClass, object instance)
        {
            ExplorerUI.BeginSrcollArea(ref midArea, ref midPos);
            ExplorerUI.HorizontalLabel("Type Name", "Value");

            foreach (var i in instanceClass.FieldList)
            {
                ExplorerUI.BeginHorizontal();
                if (CsharpKeyword.GeneralTypes.Contains(i.Value.FieldType) && instance != null)
                {
                    object obj = i.Value.GetValue(instance);
                    GUILayout.Label(i.Value.FieldType.Name + "  " + i.Key);

                    if (GUILayout.Button(obj.ToString()))
                    {
                        ValueInputWindow.varName = i.Key;
                        ValueInputWindow.varInfo = i.Value;
                        ValueInputWindow.parentObj = instance;
                        ValueInputWindow.Show();
                    }
                }
                else if(i.Value.FieldType.IsEnum && instance != null)
                {
                    object obj = i.Value.GetValue(instance);
                    GUILayout.Label(i.Value.FieldType.Name + "  " + i.Key);
                    GUILayout.TextField(obj.ToString());
                }
                else
                {
                    
                    GUILayout.Button(i.Value.FieldType.Name + "  " + i.Key);                
                }
                ExplorerUI.EndHorizontal();
            }
            ExplorerUI.EndSrcollArea();
        }

        public override ClassInfo GetInfo() => ClassInfo.Field;
    }

    class InstanceMethodView : IInstanceView
    {
        Rect midArea = new Rect(190, 50, 450, 700);
        Vector2 midPos = new Vector2();

        public override bool DrawLeftButton()
        {
            if (GUILayout.Button("方法"))
            {
                return true;
            }
            return false;
        }

        public override void DrawMidView(CsharpClass instanceClass, object instance)
        {
            ExplorerUI.BeginSrcollArea(ref midArea, ref midPos);
            ExplorerUI.HorizontalLabel("ReturnType", "Name");

            foreach (var i in instanceClass.MethodList)
            {
                if(i.Key.StartsWith("get_"))
                {
                    ExplorerUI.BeginHorizontal();
                    ExplorerUI.TextField(i.Value.ReturnType.Name);
                    if (CsharpKeyword.GeneralTypes.Contains(i.Value.ReturnType) && i.Key.StartsWith("get_"))
                    {
                        ExplorerUI.HorizontalLabel(i.Value.ReturnType.Name + "  " + i.Key, i.Value.Invoke(instance, null).ToString());
                    }
                    else if (ExplorerUI.Button(i.Key) )
                    {
                        InstanceView.add(i.Key, i.Value.Invoke(instance,null));
                    }
                    ExplorerUI.EndHorizontal();
                }
                else
                {
                    ExplorerUI.HorizontalText(i.Value.ReturnType.Name, i.Key);
                }

            }
            ExplorerUI.EndSrcollArea();
        }

        public override ClassInfo GetInfo() => ClassInfo.Method;
    }

    class InstancePropView : IInstanceView
    {
        Rect midArea = new Rect(190, 50, 450, 700);
        Vector2 midPos = new Vector2();

        public override bool DrawLeftButton()
        {
            if (GUILayout.Button("属性"))
            {
                return true;
            }
            return false;
        }

        public override void DrawMidView(CsharpClass instanceClass, object instance)
        {
            ExplorerUI.BeginSrcollArea(ref midArea,ref midPos);
            ExplorerUI.HorizontalLabel("Type", "Name");

            foreach (var i in instanceClass.PropList)
            {
                ExplorerUI.HorizontalText(i.Value.PropertyType.Name, i.Key);
            }

            ExplorerUI.EndSrcollArea();
        }

        public override ClassInfo GetInfo() => ClassInfo.Property;
    }
}
