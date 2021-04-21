using imnodesNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    [Serializable]
    public class LinkManager
    {
        LinkPool m_LinkPool = new LinkPool();

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

        //Event
        public enum ELinkEvent
        {
            Created,
            Started,
            Dropped,
            Destroyed,
            Hovered
        }
        public delegate void LinkAction(ELinkEvent eLinkEvent, int? beginID,int? endID,int? linkID);
        public event LinkAction OnLinkEvent;

        /// <summary>
        ///  Event Notify
        /// </summary>
        void NotifyLinkCreatedEvent(int start_attr, int end_attr) => OnLinkEvent?.Invoke(ELinkEvent.Created, start_attr, end_attr, null);
        void NotifyLinkStartedEvent(int start_attr) => OnLinkEvent?.Invoke(ELinkEvent.Started, start_attr, null,null);
        void NotifyLinkDroppedEvent(int start_attr) => OnLinkEvent?.Invoke(ELinkEvent.Dropped, start_attr, null, null);
        void NotifyLinkDestroyedEvent(int start_attr, int end_attr, int link_id) => OnLinkEvent?.Invoke(ELinkEvent.Destroyed, start_attr, end_attr, link_id);
        void NotifyLinkHoveredEvent(int start_attr, int end_attr,int link_id) => OnLinkEvent?.Invoke(ELinkEvent.Hovered, start_attr, end_attr, link_id);
        void NotifyLinkHoveredEvent(int link_id)
        {
            LinkPair link_pair;
            bool res = false;
            if(res = m_LinkPool.TryGetLink(link_id,out link_pair))
            {
                NotifyLinkHoveredEvent(link_pair.start, link_pair.end, link_id);
            }
            else
            {
                Logger.Info("Hovered link not exist");
            }           
        }

        /// <summary>
        ///  Event Check
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
            int link_id = -1;
            if (imnodes.IsLinkDestroyed(ref link_id))
            {
                Assert.IsTrue(RemoveLink(link_id));
            }
        }

        void CheckLinkHovered()
        {
            int link_id = -1;
            if (imnodes.IsLinkHovered(ref link_id))
            {
                NotifyLinkHoveredEvent(link_id);
            }
        }

/// <summary>
/// ------------ Link Manager
/// </summary>
        public void AddLink(LinkPair link_pair) => m_LinkPool.AddLink(link_pair);
        public bool IsConnect(LinkPair link_pair) => m_LinkPool.IsConnect(link_pair);
        public bool IsConnect(int start_attr, int end_attr) => IsConnect(new LinkPair(start_attr, end_attr));
        public void TryCreateLink(LinkPair linkPair) => TryCreateLink(linkPair.start, linkPair.end);
        public void TryCreateLink(int start_attr, int end_attr) => NotifyLinkCreatedEvent(start_attr, end_attr);
        public void TryCreateLink(INodeOutput start_node_com, INodeInput end_node_com) => NotifyLinkCreatedEvent(start_node_com.ID, end_node_com.ID);

        public bool RemoveLink(int link_id)
        {
            LinkPair link_pair;
            if (m_LinkPool.RemoveLink(link_id, out link_pair))
            {
                NotifyLinkDestroyedEvent(link_pair.start, link_pair.end,link_id);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveLinkByBegin(int begin_attr)
        {
            List<int> link_ids;
            if(m_LinkPool.TryGetLinkIDByBegin(begin_attr, out link_ids))
            {
                foreach(int link_id in link_ids)
                {
                    Assert.IsTrue(RemoveLink(link_id));
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveLinkByEnd(int end_attr)
        {
            List<int> links;
            if (m_LinkPool.TryGetLinkIDByEnd(end_attr, out links))
            {
                foreach (int link_id in links)
                {
                    Assert.IsTrue(RemoveLink(link_id));
                }
                return true;
            }
            else
            {
                return false;
            }           
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
