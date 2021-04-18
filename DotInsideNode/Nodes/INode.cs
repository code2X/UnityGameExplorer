using imnodesNET;
using ImGuiNET;
using System.Collections.Generic;

namespace DotInsideNode
{
    public abstract class INode
    {
        int m_ID;
        StyleManager m_StyleManager = new StyleManager();

        public int ID
        {
            get => m_ID;
            set => m_ID = value;
        }
        public StyleManager Style
        {
            get => m_StyleManager;
        }

        public virtual void DrawNode()
        {
            m_StyleManager.PushColorStyle();
            imnodes.BeginNode(this.ID);
            DrawNodeContent();
            imnodes.EndNode();
            m_StyleManager.PopColorStyle();
        }

        //Logic Flow
        protected virtual void DrawNodeContent() { }
        public virtual void DoNodeEnd() { }

        //Event
        public virtual void OnNodeSelected() { }
        public virtual void OnNodeHovered() { }
        public virtual void OnNodeDetroyed() { }

        //Runtime
        public virtual string Compile() { return string.Empty; }
        public virtual object Play(int callerID, params object[] objects)
        {
            NodeLogger.ExceInfo(this);
            return ExecNode(callerID, objects);
        }
        protected virtual object ExecNode(int callerID, params object[] objects) => null; 

        //Request
        protected class RequestTypeError : System.Exception
        {
            public RequestTypeError(RequestType type, INodeComponent connect = null)
                :
                base("Error Request Type: " + type + ", Connect:" + connect)
            { }
        }

        public virtual object Request(RequestType type) => throw new System.NotImplementedException();
    }

    public class NullNode : INode
    {}

    public class NodeBase: INode
    {
        Dictionary<int,INodeComponent> m_Components = new Dictionary<int, INodeComponent>();
        NodeComponentManager m_NodeComManager = NodeComponentManager.Instance;

        public int AddComponet(INodeComponent component)
        {
            int id = m_NodeComManager.AddComponet(component);
            AddMyComponent(id, component);
            return id;
        }

        public int AddComponet(INodeInput component)
        {
            int id = m_NodeComManager.AddComponet(component);
            AddMyComponent(id, component);
            return id;
        }

        public int AddComponet(INodeOutput component)
        {
            int id = m_NodeComManager.AddComponet(component);
            AddMyComponent(id, component);
            return id;
        }

        void AddMyComponent(int id, INodeComponent component)
        {
            component.ID = id;
            component.ParentNode = this;
            m_Components.Add(id, component);
        }

        protected sealed override void DrawNodeContent() 
        {
            ComponentType prevType = ComponentType.Null;
            m_Components.GetEnumerator();
            foreach (var component in m_Components)
            {
                if (component.Value.ComponentType is ComponentType.Input && prevType == ComponentType.Input)
                    ImGui.NewLine();

                component.Value.DrawComponent();

                if (component.Value.ComponentType == ComponentType.Input)
                    ImGui.SameLine();
                prevType = component.Value.ComponentType;
            }
            DrawContent();
        }

        public sealed override void DoNodeEnd()
        {
            foreach (var component in m_Components)
            {
                component.Value.DoComponentEnd();
            }
            DoEnd();
        }

        protected virtual void DrawContent() { }
        protected virtual void DoEnd() { }

        public override void OnNodeDetroyed()
        {
            foreach(var com in m_Components)
            {
                m_NodeComManager.RemoveComponent(com.Value);
            }
        }

    }

    public class ComNodeBase : NodeBase
    {
        public virtual INodeTitleBar GetTitleBarCom() => throw new System.NotImplementedException();
        public virtual ExecIC GetExecInCom() => throw new System.NotImplementedException();
        public virtual ExecOC GetExecOutCom() => throw new System.NotImplementedException();
    }
}
