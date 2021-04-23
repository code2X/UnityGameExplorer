
using DotInsideLib;
using ImGuiNET;
using System;
using System.IO;
using System.Xml.Serialization;

namespace DotInsideNode
{
    [Serializable]
    [BlueprintClass]
    public class Structure: IStructure
    {
        string m_Tooltip;
        [NonSerialized]
        private StructureView m_StructureView;
        private StructItemManager m_StructItemManager = new StructItemManager();

        public string AssetPath
        {
            get; set;
        }
        public override string Tooltip 
        { 
            get => m_Tooltip; 
            set => m_Tooltip = value; 
        }
        public override int MemberCount => 0;

        public Structure()
        {
            m_StructureView = new StructureView(this);
        }

        public override void Draw()
        {
            m_StructureView.OnGUI();
        }

        public override void Save(string dirPath)
        {
            if (dirPath == null)
                dirPath = "Content/Struct.test";

            //string strobj = "test string for serialization";
            FileStream stream = new FileStream(dirPath, FileMode.Create, FileAccess.Write, FileShare.None);

            XmlSerializer writer =
            new XmlSerializer(typeof(Structure));
            writer.Serialize(stream, this);

            stream.Close();
            Console.WriteLine(dirPath);
        }

        public void AddItem() => m_StructItemManager.AddMemberItem(new IMember());
        protected StructItemManager ItemManager => m_StructItemManager;

        class StructureView : IWindowView
        {
            private Structure m_Controller = null;

            public override string WindowName => "Structure View";
            public override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.NoCollapse;
            bool m_Open = true;

            public StructureView(Structure enumertion)
            {
                m_Controller = enumertion;
                ShowWindow();
            }

            public override void DrawWindowContent()
            {
                if (!m_Open)
                    return;
                DrawEditor();
                UpdateEditor();
            }
            public bool IsOpen => m_Open;
            public void Open() => m_Open = true;
            public void Close() => m_Open = false;

            protected virtual void DrawEditor()
            {
                if (ImGui.Button("Save"))
                {
                    m_Controller.Save(m_Controller.AssetPath);
                }
                //Structure Collapsing Header
                if (ImGui.Button("+##Structure"))
                {
                    m_Controller.AddItem();
                }
                ImGui.SameLine();
                if (ImGui.CollapsingHeader("Structure"))
                {
                    m_Controller.ItemManager.DrawStructure();
                }

                //Default Values
                if (ImGui.CollapsingHeader("Default Values"))
                {
                    m_Controller.ItemManager.DrawDefaultValue();
                }

            }

            protected virtual void UpdateEditor() { }
        }


    }
}