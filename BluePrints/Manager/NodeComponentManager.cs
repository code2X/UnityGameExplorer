using ImGuiNET;
using System.Collections.Generic;

namespace DotInsideNode
{
    public class NodeComponentManager
    {
        public Dictionary<int, INodeComponent> m_Components = new Dictionary<int, INodeComponent>();
        public Dictionary<int, INodeComponent> m_LeftComponents = new Dictionary<int, INodeComponent>();
        public Dictionary<int, INodeComponent> m_RightComponents = new Dictionary<int, INodeComponent>();

        INodeGraph m_NodeGraph = null;
        INode m_Node = null;

        public NodeComponentManager(INodeGraph nodeGraph, INode node)
        {
            m_NodeGraph = nodeGraph;
            m_Node = node;
        }

        public int AddComponet(INodeComponent component)
        {
            int id = m_NodeGraph.ngNodeComponentManager.AddComponet(component);
            FillComponent(id, component);
            m_Components.Add(id, component);
            return id;
        }

        public int AddComponet(INodeInput component)
        {
            int id = m_NodeGraph.ngNodeComponentManager.AddComponet(component);
            FillComponent(id, component);
            m_LeftComponents.Add(id, component);
            return id;
        }

        public int AddComponet(INodeOutput component)
        {
            int id = m_NodeGraph.ngNodeComponentManager.AddComponet(component);
            FillComponent(id, component);
            m_RightComponents.Add(id, component);
            return id;
        }

        void FillComponent(int id, INodeComponent component)
        {
            Assert.IsNotNull(m_Node);

            component.ID = id;
            component.ParentNode = m_Node;
            component.NodeGraph = m_NodeGraph;
        }

        public void DrawComponent()
        {
            foreach (var component in m_Components)
            {
                component.Value.DrawComponent();
            }

            int sameLineCount = m_LeftComponents.Count < m_RightComponents.Count ? m_LeftComponents.Count : m_RightComponents.Count;
            var leftEnumerator = m_LeftComponents.GetEnumerator();
            var rightEnumerator = m_RightComponents.GetEnumerator();

            for (int i = 0; i < sameLineCount; ++i)
            {
                leftEnumerator.MoveNext();
                rightEnumerator.MoveNext();

                leftEnumerator.Current.Value.DrawComponent();
                ImGui.SameLine();
                rightEnumerator.Current.Value.DrawComponent();
            }

            for (int i = 0; i < m_LeftComponents.Count - sameLineCount; ++i)
            {
                leftEnumerator.MoveNext();
                leftEnumerator.Current.Value.DrawComponent();
            }

            for (int i = 0; i < m_RightComponents.Count - sameLineCount; ++i)
            {
                rightEnumerator.MoveNext();
                rightEnumerator.Current.Value.DrawComponent();
            }
        }

        public void PostfixProc()
        {
            foreach (var component in m_Components)
            {
                component.Value.DoComponentEnd();
            }
        }

        public void Clear()
        {
            foreach (var com in m_Components)
            {
                m_NodeGraph.ngNodeComponentManager.RemoveComponent(com.Value);
            }
            foreach (var com in m_LeftComponents)
            {
                m_NodeGraph.ngNodeComponentManager.RemoveComponent(com.Value);
            }
            foreach (var com in m_RightComponents)
            {
                m_NodeGraph.ngNodeComponentManager.RemoveComponent(com.Value);
            }
            m_Components.Clear();
            m_LeftComponents.Clear();
            m_RightComponents.Clear();
        }
    }
}
