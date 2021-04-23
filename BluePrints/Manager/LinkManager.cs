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
        public delegate void LinkAction(ELinkEvent eLinkEvent, int? beginId,int? endId,int? linkId);
        public event LinkAction OnLinkEvent;

        /// <summary>
        ///  Event Notify
        /// </summary>
        void NotifyLinkCreatedEvent(int startAttr, int endAttr) => OnLinkEvent?.Invoke(ELinkEvent.Created, startAttr, endAttr, null);
        void NotifyLinkStartedEvent(int startAttr) => OnLinkEvent?.Invoke(ELinkEvent.Started, startAttr, null,null);
        void NotifyLinkDroppedEvent(int startAttr) => OnLinkEvent?.Invoke(ELinkEvent.Dropped, startAttr, null, null);
        void NotifyLinkDestroyedEvent(int startAttr, int endAttr, int linkId) => OnLinkEvent?.Invoke(ELinkEvent.Destroyed, startAttr, endAttr, linkId);
        void NotifyLinkHoveredEvent(int startAttr, int endAttr,int linkId) => OnLinkEvent?.Invoke(ELinkEvent.Hovered, startAttr, endAttr, linkId);
        void NotifyLinkHoveredEvent(int linkId)
        {
            bool res = false;
            if(res = m_LinkPool.TryGetLink(linkId,out LinkPair linkPair))
            {
                NotifyLinkHoveredEvent(linkPair.start, linkPair.end, linkId);
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
            int startAttr = -1, endAttr = -1;
            if (imnodes.IsLinkCreated(ref startAttr, ref endAttr))
            {
                NotifyLinkCreatedEvent(startAttr, endAttr);
            }
        }

        void CheckLinkStarted()
        {
            int startAttr = -1;
            if(imnodes.IsLinkStarted(ref startAttr))
            {
                NotifyLinkStartedEvent(startAttr);
            }
        }

        void CheckLinkDropped()
        {
            int startAttr = -1;
            if (imnodes.IsLinkDropped(ref startAttr))
            {
                NotifyLinkDroppedEvent(startAttr);
            }
        }

        void CheckLinkDestroyed()
        {
            int linkId = -1;
            if (imnodes.IsLinkDestroyed(ref linkId))
            {
                Assert.IsTrue(RemoveLink(linkId));
            }
        }

        void CheckLinkHovered()
        {
            int linkId = -1;
            if (imnodes.IsLinkHovered(ref linkId))
            {
                NotifyLinkHoveredEvent(linkId);
            }
        }

/// <summary>
/// ------------ Link Manager
/// </summary>
        public void AddLink(LinkPair linkPair) => m_LinkPool.AddLink(linkPair);
        public bool IsConnect(LinkPair linkPair) => m_LinkPool.IsConnect(linkPair);
        public bool IsConnect(int startAttr, int endAttr) => IsConnect(new LinkPair(startAttr, endAttr));
        public void TryCreateLink(LinkPair linkPair) => TryCreateLink(linkPair.start, linkPair.end);
        public void TryCreateLink(int startAttr, int endAttr) => NotifyLinkCreatedEvent(startAttr, endAttr);
        public void TryCreateLink(INodeOutput startNodeCom, INodeInput endNodeCom) => NotifyLinkCreatedEvent(startNodeCom.ID, endNodeCom.ID);

        public bool RemoveLink(int linkId)
        {
            if (m_LinkPool.RemoveLink(linkId, out LinkPair linkPair))
            {
                NotifyLinkDestroyedEvent(linkPair.start, linkPair.end,linkId);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveLinkByBegin(int beginAttr)
        {
            if(m_LinkPool.TryGetLinkIDByBegin(beginAttr, out List<int> linkIds))
            {
                foreach(int linkId in linkIds)
                {
                    Assert.IsTrue(RemoveLink(linkId));
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveLinkByEnd(int endAttr)
        {
            if (m_LinkPool.TryGetLinkIDByEnd(endAttr, out List<int> links))
            {
                foreach (int linkId in links)
                {
                    Assert.IsTrue(RemoveLink(linkId));
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
