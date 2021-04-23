using imnodesNET;
using ImGuiNET;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace DotInsideNode
{
    [EditorNode("Get Field")]
    class FieldNode : ComNodeBase
    {
        public SortedList<string, FieldInfo> FieldDict = new SortedList<string, FieldInfo>();
        FieldInfo m_FieldInfo = null;

        TextTB m_TextTitleBar = new TextTB("Field");
        ComboSC m_FieldCombo = new ComboSC();
        TargetIC m_TargetIC = new TargetIC();
        ObjectOC m_ObjectOC = new ObjectOC();

        public FieldNode(INodeGraph bp) : base(bp)
        {
            m_TargetIC.OnSetTargetType += new TargetIC.TypeHandler(OnTargetTypeSet);
            m_FieldCombo.OnSelected += new ComboSC.SelectAction(OnFieldSelected);

            AddComponet(m_TextTitleBar);
            AddComponet(m_FieldCombo);
            AddComponet(m_TargetIC);
            AddComponet(m_ObjectOC);
        }

        protected override void DrawContent()
        {
        }

        void OnFieldSelected(string item, int index)
        {
            m_FieldInfo = FieldDict[item];
            m_ObjectOC.ObjectType = FieldDict[item].FieldType;
        }

        void OnTargetTypeSet(Type type)
        {
            FieldInfo[] allFileds;
            if (m_TargetIC.Target is TypeOC)
            {
                allFileds = FieldTools.GetStaticField(type);
            }
            else
            {
                allFileds = FieldTools.GetAllField(type);
            }
            
            //Fill field dict
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
                break;
            }
        }

        public override object Request(ERequest type)
        {
            Assert.IsNotNull(m_FieldInfo);
            switch (type)
            {
                case ERequest.InstanceType:
                    return m_FieldInfo.FieldType;
                case ERequest.InstanceObject:
                    return m_FieldInfo.GetValue(m_TargetIC.TargetObject);
            }

            throw new RequestTypeError(type);
        }
    }
}
