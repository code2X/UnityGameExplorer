using imnodesNET;
using ImGuiNET;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace DotInsideNode
{
    [EditorNode("Set Field")]
    class FieldSetterNode : ComNodeBase
    {
        public SortedList<string, FieldInfo> FieldDict = new SortedList<string, FieldInfo>();
        FieldInfo m_FieldInfo = null;

        ExecIC m_ExecIC = new ExecIC();
        ExecOC m_ExecOC = new ExecOC();
        TextTB m_TextTitleBar = new TextTB("Set Field");
        ComboSC m_FieldCombo = new ComboSC();
        TargetIC m_TargetIC = new TargetIC();
        ObjectIC m_ObjectIC = new ObjectIC();
        ObjectOC m_ObjectOC = new ObjectOC();

        public FieldSetterNode(INodeGraph bp) : base(bp)
        {
            m_TargetIC.OnSetTargetType += new TargetIC.TypeHandler(OnTargetTypeSet);
            m_FieldCombo.OnSelected += new ComboSC.SelectAction(OnFieldSelected);

            AddComponet(m_TextTitleBar);
            AddComponet(m_ExecIC);
            AddComponet(m_ExecOC);
            AddComponet(m_FieldCombo);
            AddComponet(m_TargetIC);
            AddComponet(m_ObjectOC);
            AddComponet(m_ObjectIC);
        }

        protected override void DrawContent()
        {
        }

        void OnFieldSelected(string item,int index)
        {
            m_FieldInfo = FieldDict[item];
            m_ObjectOC.ObjectType = FieldDict[item].FieldType;
            m_ObjectIC.SetObjectType(FieldDict[item].FieldType);
        }

        void OnTargetTypeSet(Type type)
        {
            if (type == null)
                return;
            //Logger.Info(type.IsClass.ToString());
            FieldInfo[] allFileds = FieldTools.GetAllField(type);
            FieldDict.Clear();
            foreach (FieldInfo field in allFileds)
            {
                FieldDict.Add(field.Name, field);
            }

            m_FieldCombo.ItemList = FieldDict.Keys;

            foreach (var pair in FieldDict)
            {
                m_FieldInfo = pair.Value;
                m_ObjectOC.ObjectType = pair.Value.FieldType;
                m_ObjectIC.SetObjectType(pair.Value.FieldType);
                break;
            }
            //Console.WriteLine(type.Name + "ds");
        }

        protected override object ExecNode(int callerID, params object[] objects)
        {
            if(m_FieldInfo == null)
            {
                Logger.Warn("FieldInfo is null");
                return null;
            }
            m_FieldInfo.SetValue(m_TargetIC.TargetObject, m_ObjectIC.Object);
            return null;
        }

    }
}
