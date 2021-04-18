using ImGuiNET;
using imnodesNET;
using System;
using System.Collections.Generic;
using DotInsideLib;
using System.Reflection;

namespace DotInsideNode
{
    [ConnectTypes(typeof(ExecOC))]
    public class ExecIC : INodeInput
    {
        INodeOutput m_ConnectBy = new NullOC();
        string m_Text = "Exec";
        bool m_IsRuntime = false;

        public INodeOutput GetConnect() => m_ConnectBy;
        public INode ConnectNode
        {
            get
            {
                return m_ConnectBy.ParentNode;
            }
        }

        public string Text
        {
            get => m_Text;
            set => m_Text = value;
        }

        public bool IsRuntime
        {
            get => m_IsRuntime;
            set => m_IsRuntime = value;
        }

        protected override void DrawContent()
        {
            ImGui.TextUnformatted(m_Text);
        }

        public override bool TryConnectBy(INodeOutput component) 
        {
            m_ConnectBy = component;
            Logger.Info("ExecIC ConnectBy");
            return true; 
        }

        public override object Play(params object[] objects)
        {
            m_IsRuntime = true;
            ParentNode.Play(ID);
            m_IsRuntime = false;

            return null;
        }
        public override object Compile(params object[] objects)
        {
            m_IsRuntime = true;
            ParentNode.Compile();
            m_IsRuntime = false;

            return null;
        }

        public override void OnLinkDestroyed()
        {
            Logger.Info("ExecIC Link Destroyed");
            m_ConnectBy = new NullOC();
        }

        public override void OnLinkStart()
        {
          
            Logger.Info("ExecIC Link Started");
        }

        protected override PinShape GetPinShape() => m_ConnectBy is NullOC ? PinShape.Triangle : PinShape.TriangleFilled;
    }

    [SingleConnect]
    [ConnectTypes(typeof(ExecIC))]
    public class ExecOC : INodeOutput
    {
        INodeInput m_ConnectTo = new NullIC();
        string m_Text = "Exec";
        bool m_IsRuntime = false;

        INodeInput ConnectCom
        {
            get => m_ConnectTo;
        }
        INode ConnectNode
        {
            get => m_ConnectTo.ParentNode;
        }

        public bool IsRuntime
        {
            get => m_IsRuntime;
            set => m_IsRuntime = value;
        }

        public string Text
        {
            get => m_Text;
            set => m_Text = value;
        }

        protected override void DrawContent()
        {
            ImGui.TextUnformatted(m_Text);
        }

        public override bool TryConnectTo(INodeInput component) 
        {
            m_ConnectTo = component;
            Logger.Info("ExecOC ConnectTo");
            return true; 
        }

        public override object Play(params object[] objects)
        {
            m_IsRuntime = true;
            object res = ConnectCom.Play();
            m_IsRuntime = false;

            return res;
        }
        public override object Compile(params object[] objects)
        {
            m_IsRuntime = true;
            object res = ConnectCom.Compile();
            m_IsRuntime = false;

            return res;
        }

        public override void OnLinkDestroyed()
        {
            Console.WriteLine("ExecOC Link Destroyed");
            m_ConnectTo = new NullIC();
        }

        public override void OnLinkStart() 
        {
            Logger.Info("ExecOC Link Started");
        }

        public override void OnLinkDropped()
        {           
            Logger.Info("ExecOC Link Dropped");
        }

        protected override PinShape GetPinShape() => m_ConnectTo is NullIC ? PinShape.Triangle:PinShape.TriangleFilled;
    }

}
