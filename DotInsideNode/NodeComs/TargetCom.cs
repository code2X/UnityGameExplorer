using ImGuiNET;
using imnodesNET;
using System;
using System.Collections.Generic;
using DotInsideLib;
using System.Reflection;

namespace DotInsideNode
{
    [SingleConnect]
    [ConnectTypes(typeof(ObjectOC),typeof(TypeOC))]
    class TargetIC : INodeInput
    {
        INodeOutput m_ConnectBy = new NullOC();
        Type m_TargetType;
        public delegate void TypeHandler(Type type);
        public event TypeHandler OnSetTargetType;

        public INodeOutput Target
        {
            get => m_ConnectBy;
        }
        public object TargetObject
        {
            get => m_ConnectBy.Request(RequestType.InstanceObject);
        }
        public Type TargetType
        {
            get => m_TargetType;
            set
            {
                if (OnSetTargetType != null)
                    OnSetTargetType(value);
                m_TargetType = value;
            }
        }

        protected override void DrawContent()
        {
            ImGui.TextUnformatted("Target");
        }

        public override void DoComponentEnd()
        {

        }

        public override bool TryConnectBy(INodeOutput component)
        {
            m_ConnectBy = component;
            TargetType = m_ConnectBy.Request(RequestType.InstanceType) as Type;
            return true;
        }

        public string Compile()
        {
            return m_ConnectBy.ParentNode.Compile();
        }

    }
}
