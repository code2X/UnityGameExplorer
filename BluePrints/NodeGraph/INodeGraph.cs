
namespace DotInsideNode
{
    public abstract class INodeGraph
    {
        public abstract void OpenGraph();
        public abstract void CloseGraph();

        public abstract LinkManager ngLinkManager
        {
            get;
        }
        public abstract NodeManager ngNodeManager
        {
            get;
        }
        public abstract EditorNodeComponentManager ngNodeComponentManager
        {
            get;
        }

        public abstract void Draw();
        public abstract void Update();
    }
}
