using imnodesNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    public struct LinkPair
    {
        public int start, end;

        public LinkPair(int start, int end)
        {
            this.start = start;
            this.end = end;
        }
    }

    class LinkPool
    {
        Dictionary<int, LinkPair> s_Links = new Dictionary<int, LinkPair>();
        Dictionary<int, List<int>> s_Start2Link = new Dictionary<int, List<int>>();
        Dictionary<int, List<int>> s_End2Link = new Dictionary<int, List<int>>();

        static Random s_Rand = new Random();

        public void AddLink(LinkPair linkPair)
        {
            int id;
            while (s_Links.ContainsKey(id = s_Rand.Next())) ;
            s_Links.Add(id, linkPair);

            //add to Start2Link
            if (s_Start2Link.ContainsKey(linkPair.start) == false)
            {               
                s_Start2Link.Add(linkPair.start,new List<int>());
            }
            s_Start2Link[linkPair.start].Add(id);

            //add to End2Link
            if (s_End2Link.ContainsKey(linkPair.end) == false)
            {
                s_End2Link.Add(linkPair.end, new List<int>());
            }
            s_End2Link[linkPair.end].Add(id);
        }

        public void Draw()
        {
            foreach (var id2link in s_Links)
            {
                imnodes.Link(id2link.Key, id2link.Value.start, id2link.Value.end);
            }
        }

        public bool RemoveLink(int link_id)
        {
            LinkPair pair;
            if (s_Links.TryGetValue(link_id, out pair))
            {
                return s_Links.Remove(link_id) &&
                        s_Start2Link.Remove(pair.start) &&
                        s_End2Link.Remove(pair.end);
            }
            return false;
        }

        public bool RemoveLink(int link_id,out LinkPair pair)
        {
            if (s_Links.TryGetValue(link_id, out pair))
            {
                return s_Links.Remove(link_id) &&
                        s_Start2Link.Remove(pair.start) &&
                        s_End2Link.Remove(pair.end);
            }
            return false;
        }

        bool RemoveLinks(List<int> links)
        {
            foreach (int link_id in links)
            {
                RemoveLink(link_id);
            }
            return false;
        }

        public bool RemoveLinkByStart(int start)
        {
            List<int> links;
            if (s_Start2Link.TryGetValue(start, out links))
            {
                return RemoveLinks(links);
            }
            return false;
        }

        public bool RemoveLinkByEnd(int end)
        {
            List<int> links;
            if (s_End2Link.TryGetValue(end, out links))
            {
                return RemoveLinks(links);
            }
            return false;
        }

        public bool TryGetLinkIDByStart(int start, out List<int> id)
        {
            return s_Start2Link.TryGetValue(start, out id);
        }
        
        public bool TryGetLinkIDByEnd(int end, out List<int> id)
        {
            return s_End2Link.TryGetValue(end, out id);
        }

        public bool TryGetLink(int id,out LinkPair link)
        {
            return s_Links.TryGetValue(id, out link);
        }

        public bool IsConnect(LinkPair queryLink)
        {
            List<int> linkIDs = new List<int>();

            if(TryGetLinkIDByStart(queryLink.start,out linkIDs))
            {
                foreach(int id in linkIDs)
                {
                    LinkPair linkPair;
                    if(TryGetLink(id,out linkPair) && linkPair.end == queryLink.end)
                    {
                        return true;
                    }
                }              
            }

            return false;
        }
    }

}
