using DotInsideLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using imnodesNET;

namespace DotInsideNode
{
    class MethodNode : NodeBase
    {
        //public static SortedList<string, CsharpClass> classList = DotInside.classListDetails;
        MethodInfo m_Method = null;

        TextTB m_TextTitleBar = new TextTB();
        ExecIC m_ExecIC = new ExecIC();
        ExecOC m_ExecOC = new ExecOC();
        TargetIC m_TargetIC = new TargetIC();
        ObjectOC m_ObjectOC = new ObjectOC();
        //ComboCompo m_NodeCombo;

        public TargetIC GetTarget() => m_TargetIC;

        public MethodNode(MethodInfo method)
        {
            if (method == null) return;
            m_Method = method;
            m_TextTitleBar.Title = method.Name;          

            AddComponet(m_TextTitleBar);
            AddComponet(m_ExecIC);
            AddComponet(m_ExecOC);
            AddComponet(m_TargetIC);

            //if (method.ReturnType.IsValueType)
            //{
                m_ObjectOC.ObjectType = method.ReturnType;
                AddComponet(m_ObjectOC);
            //}          
        }

        void OnClassSelect(string item)
        {
            //if (classList.TryGetValue(item, out m_Class))
            //{
            //    m_NodeTypeOutput.SetType(m_Class.type);
            //    System.Console.WriteLine(item);
            //}
        }

        protected override void DrawContent()
        {
            //imnodesNET.imnodes.Lin
            //System.Console.WriteLine(classListDetails.Count);
        }

        public override string Compile()
        {
            string res = GetTarget().Compile() + "."+ m_Method.Name + "(" + ")"+ "\n";
            res += m_ExecOC.Compile();
            return res;
        }

        protected override object ExecNode(int callerID, params object[] objects)
        {
            if (m_Method == null)
                return null;
            if(m_Method.GetParameters().Length == 0)
            {
                object res = m_Method.Invoke(m_TargetIC.TargetObject, null);
                m_ObjectOC.Object = res;
            }           
            return m_ExecOC.Play();
        }

        public override object Request(RequestType type)
        {
            if (m_Method == null)
                return null;
            return m_Method.Invoke(m_TargetIC.TargetObject,null);
        }
    }
}
