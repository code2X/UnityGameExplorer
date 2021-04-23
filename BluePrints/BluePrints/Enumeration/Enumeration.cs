using DotInsideLib;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DotInsideNode
{
    class CT
    {
        List<Line> lineList = new List<Line>();

        public class Line
        {
            string m_Text = "";

            public Line(string text)
            {
                m_Text = text;
            }

            public void LSpace(int size) => m_Text = m_Text.PadLeft(m_Text.Length + size);

            public string Text => m_Text + "\n";
        }

        public class EnumClass
        {
            string m_Text = "";
            List<Line> itemList = new List<Line>();

            public EnumClass(string name)
            {
                m_Text += 
                    new Line("enum " + name).Text + 
                    new Line("{").Text;
            }

            public void AddItem(IEnumItem item)
            {
                itemList.Add(new Line(item.Name + " = " + item.ID + ","));
            }

            public string Text
            {
                get
                {           
                    foreach (Line item in itemList)
                    {
                        item.LSpace(5);
                        m_Text += item.Text;
                    }
                    m_Text += 
                        new Line("}").Text;
                    return m_Text;
                }
            }
        }

        public void EnterBracket()
        {

        }

        public void LeaveBracket()
        {

        }

        void AddLine(Line line)
        {
            lineList.Add(line);
        }

        public static CT operator +(CT left, Line b)
        {
            left.AddLine(b);
            return left;
        }

        public string Compile()
        {
            string result = "";

            foreach (Line item in lineList)
            {
                result += item.Text;
            }

            return result;
        }
    }
    
    [Serializable]
    [BlueprintClass]
    public class Enumeration: IEnumeration
    {
        [NonSerialized]
        DefaultEnumerationView m_EnumerationView = null;
        KeyNameList<IEnumItem> m_EnumKeys = new KeyNameList<IEnumItem>();
        string m_Description = "";

        #region Property
        public override string Description
        {
            get => m_Description;
            set => m_Description = value;
        }

        public override List<IEnumItem> Enumerators => m_EnumKeys.ObjList;
        protected KeyNameList<IEnumItem> Model => m_EnumKeys;
        #endregion

        #region Deserialized
        [OnDeserializedAttribute]
        private void OnDeserialized(StreamingContext sc)
        {
            m_EnumerationView = new DefaultEnumerationView(this);
        }
        #endregion

        #region Method
        public override void Compile() 
        {
            CT.EnumClass enumClass = new CT.EnumClass("Keycode");
            foreach (IEnumItem item in Enumerators)
            {
                enumClass.AddItem(item);
            }
            Console.WriteLine(enumClass.Text);
        }
        public override void Save(string dirPath)
        {
            if (dirPath == null)
                dirPath = "Content/Enum.test";

            using (FileStream stream = new FileStream(dirPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BinaryFormatter writer = new BinaryFormatter();
                writer.Serialize(stream, this);
                Console.WriteLine(dirPath);
            }           
        }

        public Enumeration Read(string dirPath)
        {
            if (dirPath == null)
                dirPath = "Content/Enum.test";

            FileStream readstream = new FileStream(dirPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryFormatter reader = new BinaryFormatter();
            Enumeration readdata = (Enumeration)reader.Deserialize(readstream);
            return readdata;
        }

        public Enumeration() => m_EnumerationView = new DefaultEnumerationView(this);
        public override void AddEnumItem() => m_EnumKeys.AddObject(new IEnumItem());
        public override void Draw() => m_EnumerationView.DrawWindowContent();
        #endregion

        class DefaultEnumerationView : IWindowView
        {            
            private EnumeratorsView m_EnumeratorsView = null;
            private Enumeration m_Controller = null;          

            public override string WindowName => "Enumeration View";
            public override ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.NoCollapse;
            bool m_Open = true;

            public DefaultEnumerationView(Enumeration enumertion)
            {
                ShowWindow();
                m_Controller = enumertion;
                m_EnumeratorsView = new EnumeratorsView(m_Controller.Model);
            }

            public override void DrawWindowContent()
            {
                if (!m_Open)
                    return;
                DrawTop();
                DrawEnumerators();
                DrawDescription();
            }
            public bool IsOpen => m_Open;
            public void Open() => m_Open = true;
            public void Close() => m_Open = false;

            protected virtual void DrawTop()
            {
                if (ImGui.Button("Save"))
                {
                    m_Controller.Save(m_Controller.AssetPath);
                }
                ImGui.SameLine();
                if (ImGui.Button("Read"))
                {
                    m_Controller.Read(m_Controller.AssetPath);
                }
                ImGui.SameLine();
                if (ImGui.Button("Compile"))
                {
                    m_Controller.Compile();
                }
            }

            protected virtual void DrawEnumerators()
            {
                //Enumerators Collapsing Header
                if (ImGui.Button("+##EnumeratorsButton"))
                {
                    m_Controller.AddEnumItem();
                }
                ImGui.SameLine();
                if (ImGui.CollapsingHeader("Enumerators"))
                {
                    m_EnumeratorsView.Draw();
                }
            }

            string TableTitle => "EnumeratorsDescriptionTable" + m_Controller.ID;
            string DescriptionTitle => "##EnumeratorsDescriptionTable";
            protected virtual void DrawDescription()
            {
                if (ImGui.CollapsingHeader("Description"))
                {
                    ImGuiEx.TableView(TableTitle, () =>
                    {
                        ImGui.TableNextColumn();
                        ImGui.Text("Enum Description");

                        ImGui.TableNextColumn();
                        string description = m_Controller.Description;
                        ImGui.InputText(DescriptionTitle, ref description, 50);
                        m_Controller.Description = description;

                    }, 2);
                }
            }
        }
    }

    public class EnumeratorsView : TListTableView<IEnumItem>
    {
        private KeyNameList<IEnumItem> m_keyNameList;
        Timer m_Timer = new Timer();

        public EnumeratorsView(KeyNameList<IEnumItem> keyNameList) : base(keyNameList)
        {
            m_keyNameList = keyNameList;
            m_keyNameList.NewObjectBaseName = "NewEnumertor";

            m_Timer.Reset();
        }

        protected override void DrawItem(int index, IEnumItem tObj, out bool onEvent)
        {
            string objDescription = tObj.Description;

            onEvent = false;
            //Name
            ImGui.TableNextColumn();
            DrawName(index, tObj, out onEvent);
            //Description
            ImGui.TableNextColumn();
            DrawDescription(index, tObj, out onEvent);
            //Remove Button
            ImGui.TableNextColumn();
            DrawRemoveButton(index, tObj, out onEvent);
            ImGui.SameLine();
            //Drag Button
            ImGui.Selectable("##EnumeratorsViewDrag" + tObj.Name);
            DragProc(index, tObj, out onEvent);
        }

        protected void DrawName(int index, IEnumItem tObj, out bool onEvent)
        {
            onEvent = false;
            string objName = tObj.Name;
            ImGui.InputText("##IEnumItemName" + tObj.ID, ref objName, 30);
            if (objName != tObj.Name)
            {
                m_keyNameList.Rename(index, objName);
            }
        }

        protected void DrawDescription(int index, IEnumItem tObj, out bool onEvent)
        {
            onEvent = false;
            string objDescription = tObj.Description;
            ImGui.InputText("##IEnumItemDescription" + tObj.ID, ref objDescription, 30);
            tObj.Description = objDescription;
        }

        protected void DrawRemoveButton(int index, IEnumItem tObj, out bool onEvent)
        {
            onEvent = false;
            if (ImGui.Button("X##" + tObj.ID))
            {
                Assert.IsTrue(m_keyNameList.RemoveObject(tObj));
                onEvent = true;
            }
        }

        protected void DragProc(int index, IEnumItem tObj, out bool onEvent)
        {
            onEvent = false;
            if (ImGui.IsItemActive() && !ImGui.IsItemHovered())
            {
                int n_next = index + (ImGui.GetMouseDragDelta(0).y < 0.0f ? -1 : 1);
                if (m_Timer.Span.TotalMilliseconds < 200)
                    return;
                else
                    m_Timer.Reset();

                if (m_keyNameList.Exchange(index, n_next))
                {
                    Logger.Info(tObj.Name + ":" + n_next.ToString());
                    ImGui.ResetMouseDragDelta();
                }
            }
        }

        private string[] m_Titles = new[] { "Display Name", "Description", "Close && Drag" };
        protected override string[] Titles => m_Titles;
        protected override string TableLable => "EnumeratorsViewTable";
    }

}