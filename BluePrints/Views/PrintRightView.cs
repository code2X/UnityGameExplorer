using System;

namespace DotInsideNode
{
    class PrintRightView: Singleton<PrintRightView>
    {
        Action m_Drawer;

        public void Draw()
        {
            m_Drawer?.Invoke();
        }

        public void Reset()
        {
            m_Drawer = null;
        }

        public void Submit(Action drawer)
        {
            m_Drawer = drawer;
        }
    }
}
