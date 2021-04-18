using imnodesNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{

    class NodeComponentAttributeProcesser
    {
        void ProcessInComConnect(INodeInput com)
        {
            object[] attrs = com.GetType().GetCustomAttributes(true);
            for (int i = 0; i < attrs.Length; i++)
            {
                if (attrs[i].GetType() == typeof(SingleConnect))
                {
                    LinkManager.Instance.TryRemoveLinkByEnd(com.ID);
                }
            }
        }

        void ProcessSingleConncect(INodeInput inCom)
        {
            SingleConnect singleConnect = (SingleConnect)Attribute.GetCustomAttribute(inCom.GetType(), typeof(SingleConnect));
            if (singleConnect != null)
            {
                LinkManager.Instance.TryRemoveLinkByEnd(inCom.ID);
            }
        }
        void ProcessSingleConncect(INodeOutput outCom)
        {
            SingleConnect singleConnect = (SingleConnect)Attribute.GetCustomAttribute(outCom.GetType(), typeof(SingleConnect));
            if (singleConnect != null)
            {
                LinkManager.Instance.TryRemoveLinkByStart(outCom.ID);
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
                    return false;
            }

            connectTypes = (ConnectTypes)Attribute.GetCustomAttribute(outComType, typeof(ConnectTypes));
            if (connectTypes != null)
            {
                if (connectTypes.Contains(inComType) == false)
                    return false;
            }

            return true;
        }

        public bool ComCanConnect(INodeInput inCom, INodeOutput outCom)
        {
            if (ProcessConncectTypes(inCom, outCom) == false)
                return false;

            ProcessSingleConncect(inCom);
            ProcessSingleConncect(outCom);

            return true;
        }
    }

    [Serializable]
    class NodeComponentManager: ILinkEventObserver
    {
        Random s_Random = new Random();

        Dictionary<int, INodeComponent> g_Components = new Dictionary<int, INodeComponent>();
        Dictionary<int, INodeInput> g_InComponents = new Dictionary<int, INodeInput>();
        Dictionary<int, INodeOutput> g_OutComponents = new Dictionary<int, INodeOutput>();

        NodeComponentAttributeProcesser m_ComAttrProcesser = new NodeComponentAttributeProcesser();

        public NodeComponentManager() 
        {
            LinkManager.Instance.AttachEventObserver(this);
        }
        public NodeComponentManager(LinkManager linkManager)
        {
            linkManager.AttachEventObserver(this);
        }
        static NodeComponentManager __instance = new NodeComponentManager();
        public static NodeComponentManager Instance 
        {
            get => __instance;
            set => __instance = value;
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
            return false;
        }

        public override void NotifyLinkCreated(int start,int end)
        {
            if (TryConnectComponet(start, end))
            {
                LinkManager.Instance.AddLink(new LinkPair(start, end));
            }
        }

        public override void NotifyLinkStarted(int start)
        {
            INodeInput inCom;
            INodeOutput outCom;

            if ( g_InComponents.TryGetValue(start, out inCom) )
            {
                inCom.OnLinkStart();
            }
            if( g_OutComponents.TryGetValue(start, out outCom) )
            {
                outCom.OnLinkStart();
            }
        }

        public override void NotifyLinkDropped(int start)
        {
            INodeInput inCom;
            INodeOutput outCom;

            if (g_InComponents.TryGetValue(start, out inCom))
            {
                inCom.OnLinkDropped();
            }
            if (g_OutComponents.TryGetValue(start, out outCom))
            {
                outCom.OnLinkDropped();
            }
        }

        public override void NotifyLinkDestroyed(int start)
        {
            INodeInput inCom;
            INodeOutput outCom;

            if (g_InComponents.TryGetValue(start, out inCom))
            {
                inCom.OnLinkDestroyed();
            }
            if (g_OutComponents.TryGetValue(start, out outCom))
            {
                outCom.OnLinkDestroyed();
            }
        }

        public override void NotifyLinkHovered(int start)
        {
            INodeInput inCom;
            INodeOutput outCom;

            if (g_InComponents.TryGetValue(start, out inCom))
            {
                inCom.OnLinkHovered();
            }
            if (g_OutComponents.TryGetValue(start, out outCom))
            {
                outCom.OnLinkHovered();
            }
        }

        void AddInComponet(int id, INodeInput inCom)
        {
            g_InComponents.Add(id, inCom);
        }

        void AddOutComponet(int id, INodeOutput outCom)
        {
            g_OutComponents.Add(id, outCom);
        }

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
            g_Components.Remove(comID);
            g_InComponents.Remove(comID);
            g_OutComponents.Remove(comID);

            component.OnComponentDestroyed();
        }
    }
}
