using DotInsideLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using imnodesNET;

namespace DotInsideNode
{
    [EditorNode("Type of")]
    class TypeofNode : NodeBase
    {
        class SelectItemNoExist : Exception
        {
            public SelectItemNoExist(string item, int index)
                :
                base("Item: " + item + ", Index: " + index)
            { }
        }

        public static SortedList<string, CsharpClass> classList = DotInside.classListDetails;
        CsharpClass m_Class = null;

        TextTB m_TextTitleBar = new TextTB("Type of");
        ComboSC m_NodeCombo;
        TypeOC m_NodeTypeOutput;

        public TypeofNode(INodeGraph bp) : base(bp)
        {
            m_Class = classList.Values[0];

            m_NodeTypeOutput = new TypeOC(m_Class.type);
            m_NodeCombo = new ComboSC(classList.Keys);
            m_NodeCombo.OnSelected += new ComboSC.SelectAction(OnClassTypeSelected);

            AddComponet(m_TextTitleBar);
            AddComponet(m_NodeCombo);
            AddComponet(m_NodeTypeOutput);
        }

        void OnClassTypeSelected(string item,int index)
        {
            if(classList.TryGetValue(item,out m_Class))
            {
                m_NodeTypeOutput.Type = m_Class.type;
                Logger.Info("Type List Select:" + item);               
            }           
            else
            {
                throw new SelectItemNoExist(item, index);
            }
        }

        public override string Compile()
        {
            string res = "typeof(" + m_Class.typeName + ")";
            return res;
        }
    }
}
