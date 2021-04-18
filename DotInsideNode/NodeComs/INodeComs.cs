using imnodesNET;
using ImGuiNET;

namespace DotInsideNode
{
    public abstract class INodeComponent
    {
        int m_ID;
        INode m_Parent = new NullNode();

        public int ID
        {
            get => m_ID;
            set => m_ID = value;
        }
        public INode ParentNode
        {
            get => m_Parent;
            set => m_Parent = value;
        }
        public abstract ComponentType ComponentType
        {
            get;
        }

        //Logic
        public abstract void DrawComponent();
        public virtual void DoComponentEnd() { }

        //Event
        public virtual void OnComponentDestroyed() { }
    }

    public enum ComponentType
    {
        Null,
        TitleBar,
        Input,
        Output,
        Static,
    }

    public abstract class INodeTitleBar : INodeComponent
    {
        public override ComponentType ComponentType 
        { 
            get => ComponentType.TitleBar;
        } 
        public override void DrawComponent()
        {
            imnodes.BeginNodeTitleBar();
            DrawContent();
            imnodes.EndNodeTitleBar();
        }
        
        protected abstract void DrawContent();
    }

    public abstract class INodeStatic : INodeComponent
    {
        public override ComponentType ComponentType
        {
            get => ComponentType.Static;
        }
        public override void DrawComponent()
        {
            imnodes.BeginStaticAttribute(this.ID);
            DrawContent();
            imnodes.EndStaticAttribute();
        }

        protected abstract void DrawContent();
    }

    public interface ILinkEvent
    {
        void OnLinkDropped();
        void OnLinkDestroyed();
        void OnLinkHovered();
        void OnLinkStart();
    }

    public enum RequestType
    {
        InstanceObject,
        InstanceType
    }

    public enum MessageType
    {
        InstanceObjectChange,
        InstanceTypeChange
    }

    public abstract class INodeEventComponent : INodeComponent, ILinkEvent
    {
        protected class RequestTypeError : System.Exception
        {
            public RequestTypeError(RequestType type,INodeComponent connect = null)
                :
                base("Error Request Type: " + type +", Connect:" + connect)
            { }
        }

        protected class MessageTypeError : System.Exception
        {
            public MessageTypeError(MessageType type, INodeComponent connect = null)
                :
                base("Error Message Type: " + type + ", Connect:" + connect)
            { }
        }

        public virtual object Request(RequestType type) => throw new System.NotImplementedException();
        public virtual object SendMessage(params string[] msgs) => throw new System.NotImplementedException();
        public virtual object SendMessage(MessageType type) => throw new System.NotImplementedException();

        //Event
        public virtual void OnLinkDropped() { }
        public virtual void OnLinkDestroyed() { }
        public virtual void OnLinkHovered() { }
        public virtual void OnLinkStart() { }
    }

    public abstract class INodeOutput : INodeEventComponent
    {

        public override ComponentType ComponentType
        {
            get => ComponentType.Output;
        }
        public override void DrawComponent()
        {
            imnodes.BeginOutputAttribute(this.ID, GetPinShape());
            DrawContent();
            imnodes.EndOutputAttribute();
        }

        protected abstract void DrawContent();
        protected virtual PinShape GetPinShape() => PinShape.Circle;

        public virtual bool TryConnectTo(INodeInput component) { return false; }
        public override void OnComponentDestroyed()
        {
            LinkManager.Instance.TryRemoveLinkByStart(this.ID);
        }

        public virtual object Play(params object[] objects) => null;
        public virtual object Compile(params object[] objects) => null;
    }

    public abstract class INodeInput : INodeEventComponent
    {

        public override ComponentType ComponentType
        {
            get => ComponentType.Input;
        }
        public override void DrawComponent()
        {
            imnodes.BeginInputAttribute(this.ID, GetPinShape());
            DrawContent();
            imnodes.EndInputAttribute();
        }

        protected abstract void DrawContent();
        protected virtual PinShape GetPinShape() => PinShape.Circle;

        public virtual bool TryConnectBy(INodeOutput component) { return false; }
        public override void OnComponentDestroyed()
        {
            LinkManager.Instance.TryRemoveLinkByEnd(this.ID);
        }

        public virtual object Play(params object[] objects) => null;
        public virtual object Compile(params object[] objects) => null;
    }

    public class NullOC : INodeOutput
    {
        protected override void DrawContent() { }
    }

    public class NullIC : INodeInput
    {
        protected override void DrawContent() { }
    }
}
