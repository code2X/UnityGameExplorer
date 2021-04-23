using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotInsideNode
{
    public class Timer
    {
        System.DateTime m_PrevSelectTime = System.DateTime.Now;

        public System.TimeSpan Span
        {
            get
            {
                System.DateTime timeNow = System.DateTime.Now;
                return timeNow - m_PrevSelectTime;
            }
        }

        public void Reset()
        {
            m_PrevSelectTime = System.DateTime.Now;
        }
    }
}
