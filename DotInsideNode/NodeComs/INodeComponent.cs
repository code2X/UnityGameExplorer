using imnodesNET;

namespace DotInsideNode
{
    public enum ENodeComponent
    {
        Null,
        TitleBar,
        Input,
        Output,
        Static,
    }

    /// <summary>
    /// Node Component Abstract Interface
    /// </summary>
    public abstract class INodeComponent : diObject
    {
        INode m_Parent = new NullNode();

        public INode ParentNode
        {
            get => m_Parent;
            set => m_Parent = value;
        }
        public abstract ENodeComponent ComponentType
        {
            get;
        }

        //Control flow
        public abstract void DrawComponent();
        public virtual void DoComponentEnd() { }

        //Event
        public enum EEvent
        {
            Clicked,
            Selected,
            Hovered,
            Detroyed
        }
        public virtual void NodeComEventProc(EEvent eEvent) { }
    }

    /// <summary>
    /// Title Component: Show node title and node tooltip
    /// </summary>
    public abstract class INodeTitleBar : INodeComponent
    {
        public override ENodeComponent ComponentType => ENodeComponent.TitleBar;

        //Control flow
        public override void DrawComponent()
        {
            imnodes.BeginNodeTitleBar();
            DrawContent();
            imnodes.EndNodeTitleBar();
        }       
        protected virtual void DrawContent() { }
    }

    /// <summary>
    /// Static Component: Without inpubt and output pin 
    /// </summary>
    public abstract class INodeStatic : INodeComponent
    {
        public override ENodeComponent ComponentType => ENodeComponent.Static;

        //Control flow
        public override void DrawComponent()
        {
            imnodes.BeginStaticAttribute(this.ID);
            DrawContent();
            imnodes.EndStaticAttribute();
        }
        protected virtual void DrawContent() { }
    }

}
