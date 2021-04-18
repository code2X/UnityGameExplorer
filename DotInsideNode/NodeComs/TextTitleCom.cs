using ImGuiNET;

namespace DotInsideNode
{

    class TextTB : INodeTitleBar
    {
        string m_Text;
        string m_Tooltip = string.Empty;
        string m_TooltipDetails = string.Empty;

        public TextTB(string text = "")
        {
            Title = text;
        }

        public string Title
        {
            get => m_Text;
            set => m_Text = value;
        }

        public string Tooltip
        {
            get
            {
                return NodeTooltips.GetTooltip(ParentNode.GetType());
            }
            //set => m_Tooltip = value;
        }

        public string TooltipDetails
        {
            get => m_TooltipDetails;
            set => m_TooltipDetails = value;
        }

        void DrawTooltip()
        {
            if (ImGui.IsItemHovered())
            {

                if (ImGui.IsKeyDown((int)Keys.LeftControl) && ImGui.IsKeyDown((int)Keys.LeftAlt))
                {
                    ImGui.SetTooltip(m_TooltipDetails);
                }               
                else
                {
                    string tooltip = Tooltip;
                    if(tooltip != string.Empty)
                        ImGui.SetTooltip(Tooltip);
                }                 
            }
        }

        protected override void DrawContent()
        {
            ImGui.TextUnformatted(m_Text);
            DrawTooltip();
        }
    }

    class TextOC : INodeOutput
    {
        string m_Text;

        public TextOC(string text = "")
        {
            m_Text = text;
        }
        public void SetText(string text) => m_Text = text;

        protected override void DrawContent()
        {
            ImGui.TextUnformatted(m_Text);
        }
    }

    class TextIC : INodeInput
    {
        string m_Text;

        public TextIC(string text = "")
        {
            m_Text = text;
        }
        public void SetText(string text) => m_Text = text;

        protected override void DrawContent()
        {
            ImGui.TextUnformatted(m_Text);
        }
    }

    class TextSC : INodeStatic
    {
        string m_Text;

        public TextSC(string text = "")
        {
            m_Text = text;
        }
        public void SetText(string text) => m_Text = text;

        protected override void DrawContent()
        {
            ImGui.TextUnformatted(m_Text);
        }
    }

}
