using imnodesNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{

    class NodeComponentAttributeProcesser
    {
        //LinkManager m_LinkManager;
        INodeGraph m_NodeGraph;

        public NodeComponentAttributeProcesser(INodeGraph nodeGraph)
        {
            m_NodeGraph = nodeGraph;
            //m_LinkManager = linkManager;
        }

        void ProcessInComConnect(INodeInput com)
        {
            object[] attrs = com.GetType().GetCustomAttributes(true);
            for (int i = 0; i < attrs.Length; i++)
            {
                if (attrs[i].GetType() == typeof(SingleConnect))
                {
                    m_NodeGraph.ngLinkManager.RemoveLinkByEnd(com.ID);
                }
            }
        }

        void ProcessSingleConncect(INodeInput inCom)
        {
            SingleConnect singleConnect = (SingleConnect)Attribute.GetCustomAttribute(inCom.GetType(), typeof(SingleConnect));
            if (singleConnect != null)
            {
                singleConnect.Process(m_NodeGraph.ngLinkManager, inCom);
            }
        }
        void ProcessSingleConncect(INodeOutput outCom)
        {
            SingleConnect singleConnect = (SingleConnect)Attribute.GetCustomAttribute(outCom.GetType(), typeof(SingleConnect));
            if (singleConnect != null)
            {
                singleConnect.Process(m_NodeGraph.ngLinkManager, outCom);
            }
        }

        bool ProcessConncectTypes(INodeInput inCom, INodeOutput outCom)
        {
            ConnectTypes connectTypes;
            Type inComType = inCom.GetType();
            Type outComType = outCom.GetType();

            connectTypes = (ConnectTypes)Attribute.GetCustomAttribute(inComType, typeof(ConnectTypes));
            if (connectTypes != null)
            {
                if (connectTypes.Contains(outComType) == false)
                {
                    Logger.Info(inCom.Name + " Can't Connect Type: " + outCom.Name);
                    return false;
                }
                    
            }

            connectTypes = (ConnectTypes)Attribute.GetCustomAttribute(outComType, typeof(ConnectTypes));
            if (connectTypes != null)
            {
                if (connectTypes.Contains(inComType) == false)
                {
                    Logger.Info(outComType.Name + " Can't Connect Type: " + inComType.Name);
                    return false;
                }
            }

            return true;
        }

        public bool ConncectValueTypeProc(INodeInput inCom, INodeOutput outCom)
        {
            Type inComType = inCom.GetType();

            ConnectValueType connectValueType = (ConnectValueType)Attribute.GetCustomAttribute(inComType, typeof(ConnectValueType));
            if (connectValueType != null)
            {
                return connectValueType.Process(m_NodeGraph, inCom, outCom);                 
            }

            return true;
        }

        public bool ComCanConnect(INodeInput inCom, INodeOutput outCom)
        {
            if (ProcessConncectTypes(inCom, outCom) == false)
                return false;

            if (ConncectValueTypeProc(inCom,outCom) == false)
                return false;

            ProcessSingleConncect(inCom);
            ProcessSingleConncect(outCom);

            return true;
        }
    }

    [Serializable]
    public class EditorNodeComponentManager
    {
        Random s_Random = new Random();

        Dictionary<int, INodeComponent> g_Components = new Dictionary<int, INodeComponent>();
        Dictionary<int, INodeInput> g_InComponents = new Dictionary<int, INodeInput>();
        Dictionary<int, INodeOutput> g_OutComponents = new Dictionary<int, INodeOutput>();

        NodeComponentAttributeProcesser m_ComAttrProcesser;
        INodeGraph m_NodeGraph;

        public EditorNodeComponentManager(INodeGraph nodeGraph)
        {
            m_NodeGraph = nodeGraph;
            m_ComAttrProcesser = new NodeComponentAttributeProcesser(m_NodeGraph);
            m_NodeGraph.ngLinkManager.OnLinkEvent += new LinkManager.LinkAction(LinkEventProc);
        }

        bool TryConnectComponet(int start, int end)
        {
            INodeInput inCom;
            INodeOutput outCom;
            if (g_InComponents.TryGetValue(end, out inCom) && g_OutComponents.TryGetValue(start, out outCom))
            {
                if(m_ComAttrProcesser.ComCanConnect(inCom,outCom) == false)
                {
                    return false;
                }

                return inCom.TryConnectBy(outCom) && outCom.TryConnectTo(inCom);
            }
            else
            {
                throw new Exception("inCom or outCom not found!!!");
            }                    
        }

        //Event
        public virtual void LinkEventProc(LinkManager.ELinkEvent eLinkEvent,int? beginID,int? endID,int? linkID)
        {
            switch(eLinkEvent)
            {
                case LinkManager.ELinkEvent.Created:
                    NotifyLinkCreated(beginID.Value, endID.Value);
                    break;
                case LinkManager.ELinkEvent.Started:
                    NotifyLinkStarted(beginID.Value);
                    break;
                case LinkManager.ELinkEvent.Dropped:
                    NotifyLinkDropped(beginID.Value);
                    break;
                case LinkManager.ELinkEvent.Hovered:
                    NotifyLinkHovered(beginID.Value, endID.Value);
                    break;
                case LinkManager.ELinkEvent.Destroyed:
                    NotifyLinkDestroyed(beginID.Value, endID.Value);
                    break;
            }
        }

        public virtual void NotifyLinkCreated(int start,int end)
        {
            if (TryConnectComponet(start, end))
            {
                m_NodeGraph.ngLinkManager.AddLink(new LinkPair(start, end));
                NotifyLinkEvent(ELinkEvent.Created, start);
                NotifyLinkEvent(ELinkEvent.Created, end);
            }
        }

        public virtual void NotifyLinkStarted(int comID) => NotifyLinkEvent(ELinkEvent.Started, comID);
        public virtual void NotifyLinkDropped(int comID) => NotifyLinkEvent(ELinkEvent.Dropped, comID);
        public virtual void NotifyLinkDestroyed(int begin,int end) 
        {
            NotifyLinkEvent(ELinkEvent.Destroyed, begin);
            NotifyLinkEvent(ELinkEvent.Destroyed, end);
        }
        public virtual void NotifyLinkHovered(int begin, int end) 
        {
            NotifyLinkEvent(ELinkEvent.Hovered, begin);
            NotifyLinkEvent(ELinkEvent.Hovered, end);
        }

        protected virtual void NotifyLinkEvent(ELinkEvent eLinkEvent,int comID)
        {
            INodeInput inCom;
            INodeOutput outCom;

            if (g_InComponents.TryGetValue(comID, out inCom))
            {
                inCom.LinkEventProc(eLinkEvent);
            }
            if (g_OutComponents.TryGetValue(comID, out outCom))
            {
                outCom.LinkEventProc(eLinkEvent);
            }
        }

        //Component Operation
        void AddInComponet(int id, INodeInput inCom) => g_InComponents.Add(id, inCom);
        void AddOutComponet(int id, INodeOutput outCom) => g_OutComponents.Add(id, outCom);

        public int AddComponet(INodeComponent component)
        {
            int id;
            while (g_Components.ContainsKey(id = s_Random.Next())) ;
            g_Components.Add(id, component);

            return id;
        }

        public int AddComponet(INodeInput component)
        {
            int id = AddComponet((INodeComponent)component);
            AddInComponet(id, component);
            return id;
        }

        public int AddComponet(INodeOutput component)
        {
            int id = AddComponet((INodeComponent)component);
            AddOutComponet(id, component);
            return id;
        }

        public void RemoveComponent(INodeComponent component)
        {
            int comID = component.ID;
            Assert.IsTrue( g_Components.Remove(comID) );
            g_InComponents.Remove(comID);
            g_OutComponents.Remove(comID);

            component.NodeComEventProc(INodeComponent.EEvent.Detroyed);
        }
    }
}
