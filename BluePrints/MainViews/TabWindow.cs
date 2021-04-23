using DotInsideLib;
using ImGuiNET;
using System;

namespace DotInsideNode
{
    class BluePrintWindow : IWindowView
    {
        TabBarView m_TabBarView = new TabBarView();
        Action drawCall = null;

        static BluePrintWindow __instance = new BluePrintWindow();
        public static BluePrintWindow Instance
        {
            get => __instance;
            set => __instance = value;
        }

        public BluePrintWindow()
        {
            ShowWindow();
            m_TabBarView.OnTabEvent += OnTabEvent;
            IBluePrint.InitClassList();
        }

        private void OnTabEvent(TabBarView.ETabEvent eTabEvent, dnObject obj)
        {
            IBluePrint bp = (IBluePrint)obj;
            switch (eTabEvent)
            {
                case TabBarView.ETabEvent.Open:
                    drawCall = bp.Draw;
                    break;
                case TabBarView.ETabEvent.Close:
                    break;
            }
        }

        public override void DrawWindowContent()
        {
            m_TabBarView.Draw();
            //ImGui.Button("Save");
            //ImGui.SameLine();
            //ImGui.Button("Browse");
            drawCall?.Invoke();
        }

        public void Submit(IBluePrint bp)
        {
            Assert.IsNotNull(bp);
            m_TabBarView.OpenTab(bp);
        }
        public override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.NoCollapse;
        public override string WindowName => "BluePrint Window";
    }

}
