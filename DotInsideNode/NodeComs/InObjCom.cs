using ImGuiNET;
using imnodesNET;
using System;
using System.Reflection;

namespace DotInsideNode
{
    [SingleConnect]
    class ObjectIC : INodeInput
    {
        protected ComObject m_Object = new ComObject();
        protected INodeOutput m_ConnectBy = new NullOC();

        public ObjectIC(Type type = null, object obj = null)
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
                Assert.IsNotNull(m_ConnectBy);
                return m_ConnectBy.Request(ERequest.InstanceObject);
            }           
        }
        public void SetObject(object obj) => m_Object.Object = obj;
        public Type GetObjectType() => m_Object.Type;
        public void SetObjectType(Type type) => m_Object.Type = type;

        protected override void DrawContent()
        {
            if(m_Object.Type != null && Text != string.Empty)
            {
                ImGui.TextUnformatted(m_Object.Type.Name + "  " + Text);
            }
            else if (m_Object.Type == null)
            {
                ImGui.TextUnformatted(Text);
            }
            else if (Text == string.Empty)
            {
                ImGui.TextUnformatted(m_Object.Type.Name);
            }
            else
            {
                ImGui.TextUnformatted("");
            }
        }

        public override bool TryConnectBy(INodeOutput component)
        {
            m_ConnectBy = component;
            m_Object.Type = (Type)m_ConnectBy.Request(ERequest.InstanceType);
            return true;
        }

        public override void LinkEventProc(ELinkEvent eEvent)
        {
            switch (eEvent)
            {
                case ELinkEvent.Started:
                    Logger.Info("ObjectIC Link Started");
                    break;
                case ELinkEvent.Dropped:
                    Logger.Info("ObjectIC Link Dropped");
                    break;
                case ELinkEvent.Destroyed:
                    Logger.Info("ObjectIC Link Destroyed");
                    m_ConnectBy = new NullOC();
                    break;
            }
        }

        public override object Request(ERequest type)
        {
            switch (type)
            {
                case ERequest.InstanceType:
                    return m_Object.Type;
            }
            throw new RequestTypeError(type, m_ConnectBy);
        }

        public override object SendMessage(EMessage type)
        {
            switch (type)
            {
                case EMessage.InstanceTypeChange:
                    m_Object.Type = (Type)m_ConnectBy.Request(ERequest.InstanceType);
                    return m_Object.Type;
            }
            throw new MessageTypeError(type, m_ConnectBy);
        }
    }

    class InputObjectIC: ObjectIC
    {
        string m_InputText = string.Empty;

        protected override void DrawContent()
        {
            base.DrawContent();
            if(Object is null)
            {
                ImGui.SameLine();
                ImGui.InputTextMultiline("##sae", ref m_InputText, 20, new Vector2(30, ImGui.GetTextLineHeightWithSpacing()));
            }

        }
    }

    class ValueIC<T>: ObjectIC
    {
        public T Value
        {
            get
            {
                if (m_ConnectBy is NullOC)
                {
                    return (T)m_Object.Object;
                }
                else
                {
                    return (T)Object;
                }
            }
        }
    }

    class BoolIC : ValueIC<bool>
    {
        public BoolIC()
        {
            m_Object.Object = false;
        }

        protected override void DrawContent()
        {
            ImGui.TextUnformatted(Text);
            ImGui.SameLine();
            bool condition = (bool)m_Object.Object;
            ImGui.Checkbox("##"+ ID.ToString(), ref condition);
            m_Object.Object = condition;
        }
    }

    class IntIC : ValueIC<int>
    {
        public IntIC()
        {
            m_Object.Object = 0;
        }

        protected override void DrawContent()
        {
            ImGui.TextUnformatted(Text);
            ImGui.SameLine();
            int value = (int)m_Object.Object;
            ImGui.SetNextItemWidth(60);
            ImGui.InputInt("##" + ID.ToString(), ref value);
            m_Object.Object = value;
        }
    }

}
