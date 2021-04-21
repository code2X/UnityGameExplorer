using imnodesNET;
using System;
using System.Collections.Generic;
using DotInsideLib;
using ImGuiNET;

namespace DotInsideNode
{
    abstract class INodeEditorView : IWindowView
    {
        bool m_Open = true;

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
        static INodeGraph m_BP = null;
        public static void SubmitGraph(INodeGraph bp) => m_BP = bp;
        protected INodeGraph BP => m_BP;

        public delegate void DropAction(INodeGraph bp);
        public static event DropAction OnDropEvent;

        public void AddNode(INode nodeView,bool atMosuePos = true)
        {
            m_BP?.ngNodeManager.AddNode(nodeView, atMosuePos);
        }

        protected sealed override void DrawNodeEditorContent()
        {
            m_BP?.ngNodeManager.Draw();
            m_BP?.ngLinkManager.Draw();
            DrawContent();
        }

        protected override void DoNodeEditorStart()
        {
            DoStart();
        }

        protected override void DoNodeEditorEnd() 
        {
            m_BP?.ngNodeManager.Update();
            m_BP?.ngLinkManager.Update();
            DragDropProc();
            DoEnd();
        }

        protected virtual void DoStart() { }
        protected virtual void DrawContent() { }
        protected virtual void DoEnd() { }

        unsafe void DragDropProc()
        {
            if (ImGui.BeginDragDropTarget())
            {
                if (BP != null)
                    OnDropEvent?.Invoke(BP);
            }
        }
    }

}
