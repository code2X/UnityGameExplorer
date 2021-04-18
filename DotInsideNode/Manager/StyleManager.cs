using imnodesNET;
using ImGuiNET;
using System.Collections.Generic;

namespace DotInsideNode
{
    public class StyleManager
    {
        struct Style
        {
            public ColorStyle style;
            public uint color;

            public Style(ColorStyle style, uint color)
            {
                this.style = style;
                this.color = color;
            }
        }

        public enum StyleType
        {
            TitleBar,
            Pin,
            Link
        }

        List<Style> m_StyleList = new List<Style>();

        public static uint GetU32Color(float r, float g, float b, float a = 1.0f)
        {
            return ImGui.GetColorU32(new Vector4(r, g, b, a));
        }
        public static uint GetU32Color(int r, int g, int b, int a = 255)
        {
            float rf = (float)r/255f;
            float gf = (float)g/255f;
            float bf = (float)b/255f;
            float af = (float)a/255f;

            return ImGui.GetColorU32(new Vector4(rf, gf, bf, af));
        }

    //ColorStyle
        public void AddStyle(ColorStyle style, uint color)
        {
            Style nodeStyle = new Style(style, color);
            m_StyleList.Add(nodeStyle);
        }

        public void AddStyle(ColorStyle style, Vector4 color)
        {
            AddStyle(style, ImGui.GetColorU32(color));
        }

        public void AddStyle(ColorStyle style, float r, float g, float b, float a = 1.0f)
        {
            Vector4 color = new Vector4(r, g, b, a);
            AddStyle(style, color);
        }

    //StyleType
        public void AddStyle(StyleType style, uint color)
        {
            switch(style)
            {
                case StyleType.TitleBar:
                    AddStyle(ColorStyle.TitleBar, color);
                    AddStyle(ColorStyle.TitleBarHovered, color);
                    AddStyle(ColorStyle.TitleBarSelected, color);
                    break;
                case StyleType.Pin:
                    AddStyle(ColorStyle.Pin, color);
                    AddStyle(ColorStyle.PinHovered, color);
                    break;
                case StyleType.Link:
                    AddStyle(ColorStyle.Link, color);
                    AddStyle(ColorStyle.LinkHovered, color);
                    AddStyle(ColorStyle.LinkSelected, color);
                    break;
            }
        }

        public void AddStyle(StyleType style, Vector4 color)
        {
            AddStyle(style, ImGui.GetColorU32(color));
        }

        public void AddStyle(StyleType style, float r, float g, float b, float a = 1.0f)
        {
            Vector4 color = new Vector4(r, g, b, a);
            AddStyle(style, color);
        }

        public void PushColorStyle()
        {
            foreach (Style style in m_StyleList)
                imnodes.PushColorStyle(style.style, style.color);
        }
        public void PopColorStyle()
        {
            foreach (Style style in m_StyleList)
                imnodes.PopColorStyle();
        }

        public void DrawStyle(System.Action action)
        {
            PushColorStyle();
            action?.Invoke();
            PopColorStyle();
        }
    }
}
