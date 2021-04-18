using imnodesNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    abstract class ILinkEventObserver
    {
        public virtual void NotifyLinkCreated(int start,int end) { }
        public virtual void NotifyLinkStarted(int start) { }
        public virtual void NotifyLinkDropped(int start) { }
        public virtual void NotifyLinkDestroyed(int start) { }
        public virtual void NotifyLinkHovered(int start) { }
    }

    [Serializable]
    class LinkManager
    {
        LinkPool m_LinkPool = new LinkPool();
        List<ILinkEventObserver> m_EventObservers = new List<ILinkEventObserver>();

        static LinkManager __instance = new LinkManager();
        public static LinkManager Instance
        {
            get => __instance;
            set => __instance = value;
        }

        public void Draw()
        {
            m_LinkPool.Draw();
        }

        public void Update()
        {
            CheckLinkCreated();
            CheckLinkStarted();
            CheckLinkDropped();
            CheckLinkDestroyed();
            CheckLinkHovered();
        }

        public void AttachEventObserver(ILinkEventObserver observer)
        {
            m_EventObservers.Add(observer);
        }

/// <summary>
/// ------------ Event Notify
/// </summary>
        void NotifyLinkCreatedEvent(int start_attr, int end_attr)
        {
            foreach (var observer in m_EventObservers)
            {
                observer.NotifyLinkCreated(start_attr, end_attr);
            }
        }

        void NotifyLinkStartedEvent(int start_attr)
        {
            foreach (var observer in m_EventObservers)
            {
                observer.NotifyLinkStarted(start_attr);
            }
        }

        void NotifyLinkDroppedEvent(int start_attr)
        {
            foreach (var observer in m_EventObservers)
            {
                observer.NotifyLinkDropped(start_attr);
            }
        }

        void NotifyLinkDestroyedEvent(int start_attr)
        {
            foreach (var observer in m_EventObservers)
            {
                observer.NotifyLinkDestroyed(start_attr);
            }
        }

        void NotifyLinkHoveredEvent(int start_attr)
        {
            foreach (var observer in m_EventObservers)
            {
                observer.NotifyLinkHovered(start_attr);
            }
        }

/// <summary>
/// ------------ Event Check
/// </summary>
        void CheckLinkCreated()
        {
            int start_attr = -1, end_attr = -1;
            if (imnodes.IsLinkCreated(ref start_attr, ref end_attr))
            {
                NotifyLinkCreatedEvent(start_attr, end_attr);
            }
        }

        void CheckLinkStarted()
        {
            int start_attr = -1;
            if(imnodes.IsLinkStarted(ref start_attr))
            {
                NotifyLinkStartedEvent(start_attr);
            }
        }

        void CheckLinkDropped()
        {
            int start_attr = -1;
            if (imnodes.IsLinkDropped(ref start_attr))
            {
                NotifyLinkDroppedEvent(start_attr);
            }
        }

        void CheckLinkDestroyed()
        {
            int start_attr = -1;
            if (imnodes.IsLinkDestroyed(ref start_attr))
            {
                NotifyLinkDestroyedEvent(start_attr);
            }
        }

        void CheckLinkHovered()
        {
            int start_attr = -1;
            if (imnodes.IsLinkHovered(ref start_attr))
            {
                NotifyLinkHoveredEvent(start_attr);
            }
        }

/// <summary>
/// ------------ Link Manager
/// </summary>
        public void AddLink(LinkPair linkPair)
        {
            m_LinkPool.AddLink(linkPair);
        }

        public void TryCreateLink(LinkPair linkPair)
        {
            TryCreateLink(linkPair.start, linkPair.end);
        }

        public void TryCreateLink(int start_attr, int end_attr)
        {
            NotifyLinkCreatedEvent(start_attr, end_attr);
        }

        public void TryCreateLink(INodeOutput start_node, INodeInput end_node)
        {
            NotifyLinkCreatedEvent(start_node.ID, end_node.ID);
        }

        public bool TryRemoveLinkByStart(int start)
        {
            List<int> links;
            if(m_LinkPool.TryGetLinkIDByStart(start, out links))
            {
                foreach(int link_id in links)
                {
                    LinkPair linkPair;
                    if (m_LinkPool.RemoveLink(link_id,out linkPair))
                    {
                        NotifyLinkDestroyedEvent(linkPair.start);
                        NotifyLinkDestroyedEvent(linkPair.end);
                    }
                }
                return true;
            }
            return false;
        }

        public bool TryRemoveLinkByEnd(int start)
        {
            List<int> links;
            if (m_LinkPool.TryGetLinkIDByEnd(start, out links))
            {
                foreach (int link_id in links)
                {
                    LinkPair linkPair;
                    if (m_LinkPool.RemoveLink(link_id, out linkPair))
                    {
                        NotifyLinkDestroyedEvent(linkPair.start);
                        NotifyLinkDestroyedEvent(linkPair.end);
                    }
                }
                return true;
            }
            return false;
        }

        //void RemoveLink()
        //{
        //    int link = -1;
        //    imnodes.GetSelectedLinks(ref link);
        //    if (link != -1 && ImGuiNET.ImGui.IsKeyPressed((int)Keys.A))
        //    {
        //        m_LinkPool.RemoveLink(link);
        //        imnodes.ClearLinkSelection();
        //    }
        //}
    }

}
