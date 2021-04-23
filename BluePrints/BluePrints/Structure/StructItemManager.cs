using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotInsideNode
{
    public class IMember: dnObject
    {
        IContainerEditor containerEditor = new IContainerEditor();
        diContainer m_Member = new ValueContainer();
        IMemberEditor m_Editor;

        public diContainer Container => m_Member;

        public Type MemberType
        {
            get => m_Member.ValueType.ValueType;
        }

        public diContainer.EContainer ContainerType
        {
            get => m_Member.ContainerType;
        }
        public IMemberEditor Editor => m_Editor;

        public IMember()
        {
            m_Editor = new DefaultMemberEditor(this);
            m_Editor.OnObjectTypeChange += OnVariableTypeChange;
            m_Editor.OnContainerTypeChange += OnContainerTypeChange;
        }

        public void OnContainerTypeChange(diContainer.EContainer SelectContainerType)
        {
            if (SelectContainerType != m_Member.ContainerType)
            {
                foreach (diContainer container in diContainer.ContainerClassList)
                {
                    if (container.ContainerType == SelectContainerType)
                    {
                        m_Member = container.DuplicateContainer();
                        Logger.Info(container.ContainerType.ToString());
                        break;
                    }
                }
                Logger.Info("ContainerTypeChange");
            }
        }

        public void OnVariableTypeChange(diType newType)
        {
            m_Member.ValueType = newType;
        }

        //Param Editor
        public abstract class IMemberEditor : IContainerEditor
        {
            public abstract void DrawMemberType();
            public abstract void DrawContainerType();
        }

        public class DefaultMemberEditor : IMemberEditor
        {
            IMember m_Param;

            public DefaultMemberEditor(IMember param)
            {
                m_Param = param;
            }

            protected virtual void DrawParamType(List<diType> ditypeList)
            {
                DrawdiType(ditypeList, m_Param.MemberType, "##VariableType" + m_Param.Name);
            }
            protected void DrawContainerType(Array containerTypeArray)
            {
                DrawContainerType(containerTypeArray, m_Param.ContainerType, m_Param.ContainerType.ToString() + "##" + m_Param.Name);
            }

            public override void DrawMemberType()
            {
                DrawParamType(diType.TypeClassList);
            }

            public override void DrawContainerType()
            {
                DrawContainerType(Enum.GetValues(typeof(diContainer.EContainer)));
            }
        }
    }

    public class StructureView : TListTableView<IMember>
    {
        private KeyNameList<IMember> m_keyNameList;
        Timer m_Timer = new Timer();

        public StructureView(KeyNameList<IMember> keyNameList) : base(keyNameList)
        {
            diContainer.InitClassList();
            m_keyNameList = keyNameList;
            m_keyNameList.NewObjectBaseName = "MenmberVar_";

            m_Timer.Reset();
        }
        System.DateTime m_PrevSelectTime = System.DateTime.Now;
        protected override void DrawItem(int index, IMember tObj, out bool onEvent)
        {
            string objName = tObj.Name;
            //string objDescription = tObj.Description;

            onEvent = false;
            ImGui.TableNextColumn();
            ImGui.InputText("##IEnumItemName" + tObj.ID, ref objName, 30);

            ImGui.TableNextColumn();
            tObj.Editor.DrawMemberType();
            ImGui.SameLine();
            tObj.Editor.DrawContainerType();

            ImGui.TableNextColumn();

            if (ImGui.Button("X##" + tObj.ID))
            {
                Assert.IsTrue(m_keyNameList.RemoveObject(tObj));
                onEvent = true;
            }
            ImGui.SameLine();
            ImGui.Selectable("##EnumeratorsViewArrowButton" + tObj.Name);
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

        private string[] m_Titles = new[] { "Member Name", "Type && Container", "Close && Drag" };
        protected override string[] Titles => m_Titles;
        protected override string TableLable => "StructureViewTable";
    }

    public class DefaultValuesView : TListTableView<IMember>
    {
        private KeyNameList<IMember> m_keyNameList;

        public DefaultValuesView(KeyNameList<IMember> keyNameList) : base(keyNameList)
        {
            m_keyNameList = keyNameList;
        }

        protected override void DrawItem(int index, IMember obj, out bool onEvent)
        {
            onEvent = false;

            ImGui.TableNextColumn();
            ImGui.Text(obj.Name);

            ImGui.TableNextColumn();
            obj.Container.DrawContainerValue(obj.Name);
        }

        private string[] m_Titles = new[] { "Member Name", "Default Value" };
        protected override string[] Titles => m_Titles;
        protected override string TableLable => "DefaultValuesViewTable";
    }

    public class StructItemManager
    {
        KeyNameList<IMember> m_diObjectManager = new KeyNameList<IMember>();
        StructureView m_EnumeratorsView;
        DefaultValuesView m_DefaultValuesView;

        public StructItemManager()
        {
            m_EnumeratorsView = new StructureView(m_diObjectManager);
            m_DefaultValuesView = new DefaultValuesView(m_diObjectManager);
        }

        //Function Manager 
        public void AddMemberItem(IMember enumItem) => m_diObjectManager.AddObject(enumItem);
        public void RemoveMemberItem(IMember enumItem) => m_diObjectManager.RemoveObject(enumItem);
        public List<IMember> EnumItems => m_diObjectManager.ObjList;

        public void DrawStructure() => m_EnumeratorsView.Draw();
        public void DrawDefaultValue() => m_DefaultValuesView.Draw();
    }
}
