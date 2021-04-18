using System;
using System.Collections.Generic;
using imnodesNET;
using ImGuiNET;

namespace DotInsideNode
{
    [Serializable]
    class NodeManager
    {
        static NodeManager __instance = new NodeManager();
        public static NodeManager Instance
        {
            get => __instance;
            set => __instance = value;
        }

        static Random s_Rand = new Random();
        Dictionary<int, INode> m_Nodes = new Dictionary<int, INode>();

        public void AddNode(INode nodeView, bool mousePos = true)
        {
            int id;
            while (m_Nodes.ContainsKey(id = s_Rand.Next())) ;

            nodeView.ID = id;
            m_Nodes.Add(id, nodeView);

            if(mousePos)
                imnodes.SetNodeScreenSpacePos(id, ImGuiNET.ImGui.GetMousePos());
        }

        public Dictionary<int, Vector2> NodeEditorPostions
        {
            get
            {
                var positionDict = new Dictionary<int, ImGuiNET.Vector2>();

                foreach (var id2node in m_Nodes)
                {
                    positionDict.Add(id2node.Key, imnodes.GetNodeEditorSpacePos(id2node.Key));
                }

                return positionDict;
            }
            set
            {
                foreach (var id2link in value)
                {
                    imnodes.SetNodeEditorSpacePos(id2link.Key, id2link.Value);
                }
            }
        }

        public void Draw()
        {
            foreach (var nodeView in m_Nodes)
            {
                nodeView.Value.DrawNode();
            }
        }

        public void Update()
        {
            CheckNodeHovered();
            CheckNodeSelectedDetroy();

            DoEnd();
        }

        void CheckNodeHovered()
        {
            int nodeID = -1;
            if(imnodes.IsNodeHovered(ref nodeID))
            {
                NotifyNodeHovered(nodeID);
            }
        }

        void CheckNodeSelectedDetroy()
        {
            int selectNum = imnodes.NumSelectedNodes();
            if (selectNum <= 0) 
                return;

            int[] nodeIDs = new int[selectNum];
            imnodes.GetSelectedNodes(ref nodeIDs[0]);

            //Destroy nodes
            if(ImGui.IsKeyReleased((int)Keys.Delete))
            {
                Logger.Info("Delete Node Number:" + selectNum);
                for (int i = 0; i < selectNum; ++i)
                {
                    TryDestroyNode(nodeIDs[i]);                       
                }
            }
            //Select nodes
            else
            {
                for (int i = 0; i < selectNum; ++i)
                {
                    NotifyNodeSelected(nodeIDs[i]);
                }
            }
        }

        void NotifyNodeHovered(int nodeID)
        {
            INode node;
            if (m_Nodes.TryGetValue(nodeID, out node))
            {
                node.OnNodeHovered();
            }
        }

        void NotifyNodeSelected(int nodeID)
        {
            INode node;
            if (m_Nodes.TryGetValue(nodeID, out node))
            {
                node.OnNodeSelected();
            }
        }

        public bool TryDestroyNode(int nodeID)
        {
            INode node;
            if (m_Nodes.TryGetValue(nodeID, out node))
            {
                if (node is MethodEntryNode)
                    return false;

                node.OnNodeDetroyed();
                m_Nodes.Remove(nodeID);
                return true;
            }

            return false;
        }

        void DoEnd()
        {
            foreach (var nodeView in m_Nodes)
            {
                nodeView.Value.DoNodeEnd();
            }
        }

    }
}
