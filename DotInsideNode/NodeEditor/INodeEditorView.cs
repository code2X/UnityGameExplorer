using imnodesNET;
using System;
using System.Collections.Generic;
using DotInsideLib;

namespace DotInsideNode
{
    abstract class INodeEditorView : IWindowView
    {
        bool m_Open = false;

        public override void DrawWindowContent()
        {
            if (!m_Open)
                return;
            DoNodeEditorStart();
            imnodes.BeginNodeEditor();
            DrawNodeEditorContent();        
            imnodes.EndNodeEditor();
            DoNodeEditorEnd();
        }
        public bool IsOpen
        {
            get => m_Open;
        }
        public void Open() => m_Open = true;
        public void Close() => m_Open = false;

        protected virtual void DoNodeEditorStart() { }
        protected abstract void DrawNodeEditorContent();
        protected virtual void DoNodeEditorEnd() { }
    }

    class NodeEditorBase: INodeEditorView
    {
        public void AddNode(INode nodeView,bool atMosuePos = true)
        {
            NodeManager.Instance.AddNode(nodeView, atMosuePos);
        }

        protected sealed override void DrawNodeEditorContent()
        {
            NodeManager.Instance.Draw();
            LinkManager.Instance.Draw();
            DrawContent();
        }

        protected override void DoNodeEditorStart()
        {
            DoStart();
        }

        protected override void DoNodeEditorEnd() 
        {
            NodeManager.Instance.Update();
            LinkManager.Instance.Update();
            DoEnd();
        }

        protected virtual void DoStart() { }
        protected virtual void DrawContent() { }
        protected virtual void DoEnd() { }
    }

}
