using System;

namespace DotInsideNode
{
    class PrintRightView
    {
        static PrintRightView __instance = new PrintRightView();
        Action m_Drawer;

        public static PrintRightView Instance
        {
            get => __instance;
        }

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
