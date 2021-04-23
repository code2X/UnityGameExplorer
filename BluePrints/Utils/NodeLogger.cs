namespace DotInsideNode
{
    class NodeLogger:Logger
    {
        public static void ExceStart()
        {
            Info("------------ Start ------------");
        }

        public static void ExceInfo(INode node)
        {
            Info(node.GetType().ToString());
        }

    }
}
