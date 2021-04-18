using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotInsideNode
{
    abstract class BoolOperationNode : ComNodeBase
    {
        protected static uint m_TitleBarColor = StyleManager.GetU32Color(67, 205, 128);
        protected static uint m_PinColor = StyleManager.GetU32Color(255, 0, 0);

        protected TextTB m_TextTitleBar = new TextTB("");
        List<BoolIC> m_BoolIC_List = new List<BoolIC>();
        protected BoolOC m_ObjectOC = new BoolOC();

        public BoolOperationNode()
        {
            m_ObjectOC.Text = "Result".PadLeft(9);

            Style.AddStyle(StyleManager.StyleType.TitleBar, m_TitleBarColor);
            Style.AddStyle(StyleManager.StyleType.Pin, m_PinColor);
        }

        public void AddBaseComponet()
        {
            AddComponet(m_TextTitleBar);
            NewPin();
            AddComponet(m_ObjectOC);
            NewPin();
        }

        protected override void DrawContent()
        {
            if (ImGui.Button("Add Pin +##" + ID))
            {
                NewPin();
            }
        }

        void NewPin()
        {
            BoolIC newPin = new BoolIC();
            m_BoolIC_List.Add(newPin);
            AddComponet(newPin);
        }

        protected object DoBoolOperation()
        {
            List<bool> objectList = new List<bool>();

            foreach(var obj in m_BoolIC_List)
            {
                objectList.Add(obj.Value);
            }

            return DoBoolOperation(objectList);
        }

        public override object Request(RequestType type)
        {
            switch (type)
            {
                case RequestType.InstanceType:
                    return typeof(object);
                case RequestType.InstanceObject:
                    return DoBoolOperation();
            }
            throw new RequestTypeError(type);
        }

        protected abstract bool DoBoolOperation(List<bool> objects);
    }

    [EditorNode(m_Name)]
    class ANDNode : BoolOperationNode
    {
        public const string  m_Name = "AND";

        public ANDNode()
        {
            m_TextTitleBar.Title = m_Name;
            AddBaseComponet();
        }

        protected override bool DoBoolOperation(List<bool> objects)
        {
            bool res = true;
            foreach(bool obj in objects)
            {
                res = res && (bool)obj;
            }

            return res;
        }
    }

    [EditorNode("OR")]
    class ORNode : BoolOperationNode
    {
        public ORNode()
        {
            m_TextTitleBar.Title = "OR";
            AddBaseComponet();
        }

        protected override bool DoBoolOperation(List<bool> objects)
        {
            bool res = false;
            foreach (bool obj in objects)
            {
                res = res || (bool)obj;
            }

            return res;
        }
    }

    [EditorNode("NOT")]
    class NOTNode : OperationNoExecNode
    {
        protected static uint m_PinColor = StyleManager.GetU32Color(255, 0, 0);

        public NOTNode()
        {
            m_TextTitleBar.Title = "NOT";
            m_ObjectIC_Left.Text = "In";
            m_ObjectOC.Text = "Out";

            AddComponet(m_TextTitleBar);
            AddComponet(m_ObjectIC_Left);
            AddComponet(m_ObjectOC);

            Style.AddStyle(StyleManager.StyleType.Pin, m_PinColor);
        }

        public override object Request(RequestType type)
        {
            switch (type)
            {
                case RequestType.InstanceType:
                    return typeof(object);
                case RequestType.InstanceObject:
                    return !(bool)m_ObjectIC_Left.Object;
            }
            throw new RequestTypeError(type);
        }
    }


}
