using DotInsideLib;
using DotInsideLib.BluePrints.MainViews;
using ImGuiNET;

namespace DotInsideNode
{
    class ContentBrowser : IWindowView
    {
        DirView m_DirView = new DirView();
        WrappingView m_WrapView = new WrappingView();

        string m_SelectDir = string.Empty;

        static ContentBrowser __instance = new ContentBrowser();
        public static ContentBrowser Instance
        {
            get => __instance;
            set => __instance = value;
        }
        public ContentBrowser()
        {
            ShowWindow();

            m_DirView.OnItemEvent += OnDirItemEvent;
        }

        private void OnDirItemEvent(DirView.EItemEvent eItemEvent, string dirPath)
        {
            switch (eItemEvent)
            {
                case DirView.EItemEvent.Clicked:
                    m_SelectDir = dirPath;
                    Logger.Info(dirPath);
                    break;
            }
        }

        public override void DrawWindowContent()
        {
            ImGuiEx.TableView("ContentBrowserTable", () =>
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                m_DirView.Draw();

                ImGui.TableNextColumn();
                DrawRight();
            }, ImGuiTableFlags.Resizable, "Dir", "File");

        }
        void DrawRight()
        {
            if (m_SelectDir != string.Empty)
                m_WrapView.Draw(m_SelectDir);

            /*
            ImGui.Text("Manually wrapping:");
            Vector2 button_sz = new Vector2(40, 40);
            var style = ImGui.GetStyle();
            int buttons_count = 20;
            float window_visible_x2 = ImGui.GetWindowPos().x + ImGui.GetWindowContentRegionMax().x;
            for (int n = 0; n < buttons_count; n++)
            {
                ImGui.PushID(n);
                ImGui.Button("Manually wrapping", button_sz);
                float last_button_x2 = ImGui.GetItemRectMax().x;
                float next_button_x2 = last_button_x2 + style.ItemSpacing.x + button_sz.x; // Expected position if next button was on same line
                if (n + 1 < buttons_count && next_button_x2 < window_visible_x2)
                    ImGui.SameLine();
                ImGui.PopID();
            }
            */


            if (ImGui.BeginPopupContextWindow())
            {
                if (ImGui.BeginMenu("BluePrint"))
                {
                    ImGui.Selectable("BluePrint Class");
                    if (ImGui.Selectable("Enumeration"))
                    {
                        BluePrintWindow.Instance.Submit(new Enumeration());
                    }
                    ImGui.Selectable("Structure");
                    ImGui.EndMenu();
                }
                ImGui.EndPopup();
            }

        }

        public override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.NoCollapse;
        public override string WindowName => "Content Browser";
    }
}
