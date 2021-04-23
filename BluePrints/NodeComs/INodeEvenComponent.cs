using imnodesNET;

namespace DotInsideNode
{
    public enum ELinkEvent
    {
        Created,
        Started,
        Hovered,
        Destroyed,
        Dropped
    }
    public interface ILinkEvent
    {
        void LinkEventProc(ELinkEvent eEvent);
    }

    public enum ERequest
    {
        InstanceObject,
        InstanceType
    }

    public enum EMessage
    {
        InstanceObjectChange,
        InstanceTypeChange
    }

    public abstract class INodeLinkEventComponent : INodeComponent, ILinkEvent
    {
        protected class RequestTypeError : System.Exception
        {
            public RequestTypeError(ERequest type, INodeComponent connect = null)
                :
                base("Error Request Type: " + type + ", Connect:" + connect)
            { }
        }

        protected class MessageTypeError : System.Exception
        {
            public MessageTypeError(EMessage type, INodeComponent connect = null)
                :
                base("Error Message Type: " + type + ", Connect:" + connect)
            { }
        }

        public virtual object Request(ERequest type) => null;
        public virtual object SendMessage(params string[] msgs) => null;
        public virtual object SendMessage(EMessage type) => null;

        //Link Event
        public virtual void LinkEventProc(ELinkEvent eEvent) => DefLinkEventProc(eEvent);
        protected virtual void DefLinkEventProc(ELinkEvent eEvent)
        {
            if (eEvent == ELinkEvent.Hovered)
                return;
            Logger.Info(GetType().Name + " Link " + eEvent);
        }

    }

    /// <summary>
    /// Input Component: with a input pin
    /// </summary>
    public abstract class INodeInput : INodeLinkEventComponent
    {
        protected virtual PinShape GetPinShape() => PinShape.Circle;
        public override ENodeComponent ComponentType => ENodeComponent.Input;

        PinStyle m_Style = new PinStyle();
        public PinStyle Style => m_Style;

        //Drawer
        public override void DrawComponent()
        {
            Style.PushColorStyle();
            imnodes.BeginInputAttribute(this.ID, GetPinShape());
            DrawContent();
            imnodes.EndInputAttribute();
            Style.PopColorStyle();
        }
        protected virtual void DrawContent() { }

        //Event
        public virtual bool TryConnectBy(INodeOutput component) { return false; }
        public override void NodeComEventProc(EEvent eEvent) => DefComEventProc(eEvent);
        protected virtual void DefComEventProc(EEvent eEvent)
        {
            switch (eEvent)
            {
                case EEvent.Detroyed:
                    NodeGraph.ngLinkManager.RemoveLinkByEnd(this.ID);
                    break;
            }
            InfoComEvent(eEvent);
        }

        //Runtime
        public virtual object Play(params object[] objects) => null;
        public virtual object Compile(params object[] objects) => null;
    }

    /// <summary>
    /// Output Component: with a output pin
    /// </summary>
    public abstract class INodeOutput : INodeLinkEventComponent
    {
        protected virtual PinShape GetPinShape() => PinShape.Circle;
        public override ENodeComponent ComponentType => ENodeComponent.Output;

        PinStyle m_Style = new PinStyle();
        public PinStyle Style => m_Style;

        //Drawer
        public override void DrawComponent()
        {
            Style.PushColorStyle();
            imnodes.BeginOutputAttribute(this.ID, GetPinShape());
            DrawContent();
            imnodes.EndOutputAttribute();
            Style.PopColorStyle();
        }
        protected virtual void DrawContent() { }

        //Event
        public virtual bool TryConnectTo(INodeInput component) { return false; }
        public override void NodeComEventProc(EEvent eEvent) => DefComEventProc(eEvent);
        protected virtual void DefComEventProc(EEvent eEvent)
        {           
            switch (eEvent)
            {
                case EEvent.Detroyed:
                    NodeGraph.ngLinkManager.RemoveLinkByBegin(this.ID);
                    break;
            }
            InfoComEvent(eEvent);
        }

        //Runtime
        public virtual object Play(params object[] objects) => null;
        public virtual object Compile(params object[] objects) => null;
    }    

    //Null Node Component Object
    public class NullIC : INodeInput { }
    public class NullOC : INodeOutput { }    
}
