using ImGuiNET;
using imnodesNET;
using System;
using System.Reflection;

namespace DotInsideNode
{
    class ComObject
    {
        object m_Object = null;
        Type m_Type = null;
        string m_Name = string.Empty;

        public string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        public object Object
        {
            get => m_Object;
            set
            {
                if (m_Object == null)
                {
                    Logger.Warn("Set null object");
                }

                m_Object = value;
            }
        }

        public Type Type
        {
            get => m_Type;
            set
            {
                if (m_Type == null)
                {
                    Logger.Warn("Set null type");
                }

                m_Type = value;
            }
        }
    }

    class ObjectOC : INodeOutput
    {
        ComObject m_Object = new ComObject();

        public ObjectOC(Type type = null, object obj = null)
        {
            m_Object.Object = obj;
            m_Object.Type = type;
        }

        public string Text
        {
            get => m_Object.Name;
            set => m_Object.Name = value;
        }
        public object Object
        {
            get
            { 
                if(m_Object.Object != null)
                {
                    return m_Object.Object;
                }                    
                else
                {
                    Assert.IsNotNull(ParentNode);
                    return ParentNode.Request(RequestType.InstanceObject);
                }
            }
            set => m_Object.Object = value;
        }
        public Type ObjectType
        {
            get => m_Object.Type;
            set => m_Object.Type = value;
        }

        protected override void DrawContent()
        {
            if (m_Object.Type != null && m_Object.Name != string.Empty)
            {
                ImGui.TextUnformatted(m_Object.Type.Name + "  " + m_Object.Name);
            }
            else if (m_Object.Type == null)
            {
                ImGui.TextUnformatted(m_Object.Name);
            }
            else if (m_Object.Name == string.Empty)
            {
                ImGui.TextUnformatted(m_Object.Type.Name);
            }
            else
            {
                ImGui.TextUnformatted("");
            }
        }

        public override bool TryConnectTo(INodeInput component)
        {
            return true;
        }

        public override void OnLinkDropped()
        {
            if(m_Object.Type != null)
                PopupSelectList.GetInstance().Show(MethodTools.GetMethodList(m_Object.Type), OnListSelected);
        }

        void OnListSelected(string selected, int index)
        {
            Logger.Info(selected);
            MethodInfo methodInfo = MethodTools.GetAllMethod(m_Object.Type)[index];
            if (methodInfo == null) return;

            MethodNode endNode = new MethodNode(methodInfo);
            NodeManager.Instance.AddNode(endNode);
            LinkManager.Instance.TryCreateLink(this, endNode.GetTarget());
        }

        public override object Request(RequestType type) 
        { 
            switch(type)
            {
                case RequestType.InstanceType:
                    return ObjectType;
                case RequestType.InstanceObject:
                    return Object;
            }
            throw new RequestTypeError(type);
        }
    }

    class BoolOC : ObjectOC
    {
    }

}
