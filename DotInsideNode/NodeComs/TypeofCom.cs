using System;
using System.Reflection;

namespace DotInsideNode
{
    class TypeOC : INodeOutput
    {
        Type m_Type = null;
        INodeInput m_ConnectTo = new NullIC();

        public TypeOC(Type type)
        {
            m_Type = type;
        }
        public Type Type
        {
            set => m_Type = value;
        }

        protected override void DrawContent()
        {
            //ImGui.TextUnformatted(m_Type.Name);
        }

        public override void LinkEventProc(ELinkEvent eEvent)
        {
            switch (eEvent)
            {
                case ELinkEvent.Dropped:
                    PopupSelectList.GetInstance().Show(MethodTools.GetMethodList(m_Type), OnListSelected);
                    break;
            }
        }

        public override void DoComponentEnd()
        {
            PopupSelectList.GetInstance().Draw();
        }

        public override bool TryConnectTo(INodeInput component)
        {
            //if (component.GetType().Name != typeof(ExecOC).Name)
            //    return false;
            //
            m_ConnectTo = component;
            //component.OnLinkStart();
            //Console.WriteLine("ExecIC ConnectBy");
            return true;
        }

        void OnListSelected(string selected,int index)
        {
            Logger.Info("TypeOC modal list select:" + selected);

            MethodInfo[] allMethods = MethodTools.GetAllMethod(m_Type);
            if(index >= allMethods.Length)
            {
                Logger.Error("TypeOC method index out range");
                return;
            }

            MethodInfo methodInfo = allMethods[index];
            if (methodInfo == null)
            {
                Logger.Error("TypeOC methodInfo is null");
                return;
            }

            MethodNode endNode = new MethodNode(methodInfo);
            //NodeManager.Instance.AddNode(endNode);
            //LinkManager.Instance.TryCreateLink(this, endNode.GetTarget());
        }

        public override object Request(ERequest type)
        {
            switch (type)
            {
                case ERequest.InstanceType:
                    return m_Type;
                case ERequest.InstanceObject:
                    return null;
            }
            throw new RequestTypeError(type);
        }
    }
}
