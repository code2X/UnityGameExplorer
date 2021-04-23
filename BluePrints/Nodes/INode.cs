using imnodesNET;
using ImGuiNET;
using System.Collections.Generic;

namespace DotInsideNode
{
    public abstract class INode: dnObject
    {
        StyleManager m_StyleManager = new StyleManager();

        public StyleManager Style
        {
            get => m_StyleManager;
        }
        public virtual bool Removable => true;

        public virtual void DrawNode()
        {
            Style.PushColorStyle();
            imnodes.BeginNode(this.ID);
            DrawNodeContent();
            imnodes.EndNode();
            Style.PopColorStyle();
        }

        //Logic Flow
        protected virtual void DrawNodeContent() { }
        public virtual void DoNodeEnd() { }

        //Event
        public enum EEvent
        {
            LClicked,
            RClicked,
            Selected,
            Hovered,
            Detroyed
        }

        public virtual void NodeEventProc(EEvent eEvent) { }
        protected void InfoComEvent(EEvent eEvent)
        {
            Logger.Info(GetType().Name + " " + eEvent.ToString());
        }

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
            public RequestTypeError(ERequest type, INodeComponent connect = null)
                :
                base("Error Request Type: " + type + ", Connect:" + connect)
            { }
        }

        public virtual object Request(ERequest type) => throw new System.NotImplementedException();
    }

    public class NullNode : INode
    {}

    public class NodeBase: INode
    {
        INodeGraph m_NodeGraph = null;
        NodeComponentManager m_ComManager = null;
        protected NodeComponentManager ComManager => m_ComManager;
        protected INodeGraph NodeGraph => m_NodeGraph;

        public NodeBase(INodeGraph ng)
        {
            Assert.IsNotNull(ng);
            m_NodeGraph = ng;
            m_ComManager = new NodeComponentManager(ng, this);
        }
        public int AddComponet(INodeComponent component) => m_ComManager.AddComponet(component);
        public int AddComponet(INodeInput component) => m_ComManager.AddComponet(component);
        public int AddComponet(INodeOutput component) => m_ComManager.AddComponet(component);

        //Node Logic
        protected sealed override void DrawNodeContent()
        {
            m_ComManager.DrawComponent();
            DrawContent();
        }
        public sealed override void DoNodeEnd()
        {
            m_ComManager.PostfixProc();
            DoEnd();
        }

        protected virtual void DrawContent() { }
        protected virtual void DoEnd() { }

        //Event
        public override void NodeEventProc(EEvent eEvent) => DefNodeEventProc(eEvent);
        protected virtual void DefNodeEventProc(EEvent eEvent)
        {
            switch(eEvent)
            {
                case EEvent.LClicked:
                case EEvent.RClicked:
                    InfoComEvent(eEvent);
                    break;
                case EEvent.Detroyed:
                    InfoComEvent(eEvent);
                    OnNodeDetroyed();
                    break;
            }
            
        }

        void OnNodeDetroyed() => m_ComManager.Clear();

        public static bool TryConnectCom<OUT,IN>(INodeGraph bp,NodeBase begin,NodeBase end) 
            where OUT: INodeOutput 
            where IN: INodeInput
        {
            OUT OutCom = null;
            IN InCom = null;

            foreach (var pair in begin.m_ComManager.m_RightComponents)
            {
                if(pair.Value is OUT)
                {
                    OutCom = (OUT)pair.Value;
                }
            }

            foreach (var pair in end.m_ComManager.m_LeftComponents)
            {
                if (pair.Value is IN)
                {
                    InCom = (IN)pair.Value;
                }
            }

            if(OutCom != null && InCom != null)
            {
                bp.ngLinkManager.TryCreateLink(OutCom.ID, InCom.ID);
                return bp.ngLinkManager.IsConnect(OutCom.ID, InCom.ID);
            }
            else
            {
                return false;
            }
        }
    }

    public class ComNodeBase : NodeBase
    {
        public ComNodeBase(INodeGraph bp):base(bp)
        {}

        public virtual INodeTitleBar GetTitleBarCom() => null;
        public virtual ExecIC GetExecInCom() => null;
        public virtual ExecOC GetExecOutCom() => null;
    }

}
