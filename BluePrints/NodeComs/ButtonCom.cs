using ImGuiNET;
using System;

namespace DotInsideNode
{
    class ButtonCom : INodeStatic
    {
        string m_Text = "";
        public event Action OnButtonClick;

        public string Text
        {
            get => m_Text;
            set => m_Text = value;
        }

        protected override void DrawContent()
        {
            if(ImGui.Button(m_Text + "##" + ID))
            {
                OnButtonClick?.Invoke();
            }
        }
    }
}
