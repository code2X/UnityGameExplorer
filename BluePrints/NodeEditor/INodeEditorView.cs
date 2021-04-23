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
        static INodeGraph m_NodeGraph = null;
        public static void SubmitGraph(INodeGraph nodeGraph) => m_NodeGraph = nodeGraph;
        protected INodeGraph NodeGraph => m_NodeGraph;

        public override string WindowName => "NodeEditorBase";

        public delegate void DropAction(INodeGraph bp);
        public static event DropAction OnDropEvent;

        public void AddNode(INode nodeView,bool atMousePos = true)
        {
            m_NodeGraph?.ngNodeManager.AddNode(nodeView, atMousePos);
        }

        protected sealed override void DrawNodeEditorContent()
        {
            m_NodeGraph?.Draw();
            DrawContent();
        }

        protected override void DoNodeEditorStart()
        {
            DoStart();
        }

        protected override void DoNodeEditorEnd() 
        {
            m_NodeGraph?.Update();
            DragDropProc();
            DoEnd();
        }

        protected virtual void DoStart() { }
        protected virtual void DrawContent() { }
        protected virtual void DoEnd() { }

        protected virtual void DragDropProc()
        {
            if (ImGui.BeginDragDropTarget())
            {
                if (m_NodeGraph != null)
                    OnDropEvent?.Invoke(m_NodeGraph);
            }
        }
    }

}
