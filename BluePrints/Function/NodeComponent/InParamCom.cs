using ImGuiNET;

namespace DotInsideNode
{
    class ParamIC: ObjectIC
    {
        IParam m_Param;

        public ParamIC(IParam param)
        {
            m_Param = param;
        }

        protected override void  DrawContent()
        {
            ImGui.TextUnformatted(m_Param.Name);
        }
    }
}
