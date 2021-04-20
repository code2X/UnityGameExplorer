using imnodesNET;

namespace DotInsideNode
{
    public enum ELinkEvent
    {
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

    public abstract class INodeEventComponent : INodeComponent, ILinkEvent
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

        //Event
        public virtual void LinkEventProc(ELinkEvent eEvent) { }
    }

    /// <summary>
    /// Input Component: with a input pin
    /// </summary>
    public abstract class INodeInput : INodeEventComponent
    {
        protected virtual PinShape GetPinShape() => PinShape.Circle;
        public override ENodeComponent ComponentType => ENodeComponent.Input;

        //Drawer
        public override void DrawComponent()
        {
            imnodes.BeginInputAttribute(this.ID, GetPinShape());
            DrawContent();
            imnodes.EndInputAttribute();
        }
        protected virtual void DrawContent() { }

        //Event
        public virtual bool TryConnectBy(INodeOutput component) { return false; }
        public override void NodeComEventProc(EEvent eEvent) => DefEventProc(eEvent);
        protected virtual void DefEventProc(EEvent eEvent)
        {
            switch (eEvent)
            {
                case EEvent.Detroyed:
                    LinkManager.Instance.TryRemoveLinkByEnd(this.ID);
                    break;
            }
        }

        //Runtime
        public virtual object Play(params object[] objects) => null;
        public virtual object Compile(params object[] objects) => null;
    }

    /// <summary>
    /// Output Component: with a output pin
    /// </summary>
    public abstract class INodeOutput : INodeEventComponent
    {
        protected virtual PinShape GetPinShape() => PinShape.Circle;
        public override ENodeComponent ComponentType => ENodeComponent.Output;

        //Drawer
        public override void DrawComponent()
        {
            imnodes.BeginOutputAttribute(this.ID, GetPinShape());
            DrawContent();
            imnodes.EndOutputAttribute();
        }
        protected virtual void DrawContent() { }

        //Event
        public virtual bool TryConnectTo(INodeInput component) { return false; }
        public override void NodeComEventProc(EEvent eEvent) => DefEventProc(eEvent);
        protected virtual void DefEventProc(EEvent eEvent)
        {
            switch (eEvent)
            {
                case EEvent.Detroyed:
                    LinkManager.Instance.TryRemoveLinkByStart(this.ID);
                    break;
            }
        }

        //Runtime
        public virtual object Play(params object[] objects) => null;
        public virtual object Compile(params object[] objects) => null;
    }    

    //Null Node Component Object
    public class NullIC : INodeInput { }
    public class NullOC : INodeOutput { }    
}
