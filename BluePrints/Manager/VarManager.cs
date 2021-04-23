using ImGuiNET;
using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    [Serializable]
    public class VarManager: Singleton<VarManager>
    {
        class ListMenuView : TEnumMenuView<IVar, ListMenuView.EItemEvent>
        {
            public enum EItemEvent
            {
                Delete,
                Duplicate,
            }
        }

        diObjectManager<IVar> m_DiObjectManager = new diObjectManager<IVar>();
        VarListView m_ListDrawer = null;
        ListMenuView m_ListMenuView = new ListMenuView();

        public VarManager()
        {
            m_DiObjectManager.NewObjectBaseName = "NewVar_";

            m_ListDrawer = new VarListView(m_DiObjectManager);

            InitManagerEvent();
            diType.InitClassList();
            diContainer.InitClassList();
            ConnectValueType.InitClassList();
        }

        void InitManagerEvent()
        {
            //List Menu Event
            m_ListMenuView.OnMenuEvent += new ListMenuView.TMenuAction(ListMenuItemEventProc);
            m_ListDrawer.MenuDrawer = m_ListMenuView;

            //List Event
            m_ListDrawer.OnListItemEvent += new VarListView.TListAction(ListItemEventProc);

            m_DiObjectManager.OnObjectEvent += new diObjectManager<IVar>.ObjectAction(ManagerEventProc);
        }

        //List Event
        void ListItemEventProc(VarListView.EListItemEvent eEvent, IVar variable)
        {
            switch (eEvent)
            {
                case VarListView.EListItemEvent.SelectItem:
                    m_DiObjectManager.m_SelectedTObj = variable;
                    PrintRightView.Instance.Submit(DrawVarInfo);
                    break;
            }
        }
        void ListMenuItemEventProc(ListMenuView.EMenuEvent eMenuEvent, ListMenuView.EItemEvent eItemEvent, IVar variable)
        {
            switch (eItemEvent)
            {
                case ListMenuView.EItemEvent.Delete:
                    TryDeleteVar(variable.Name);
                    break;
                case ListMenuView.EItemEvent.Duplicate:
                    IVar dup = (IVar)variable.Duplicate();
                    AddVar(dup);
                    break;
            }
        }

        //Var Event
        void ManagerEventProc(diObjectManager<IVar>.EObjectEvent eObjectEvent, IVar @var)
        {
            switch (eObjectEvent)
            {
                case diObjectManager<IVar>.EObjectEvent.Delete:
                    @var.OnVarDelete();     //Notify var delete action
                    break;
            }
        }

        public void AddVar() { AddVar(new Variable()); }
        public void AddVar(IVar variable) => m_DiObjectManager.AddObject(variable);
        public bool ContainVar(int id) => m_DiObjectManager.ContainObject(id);
        public bool ContainVar(string name) => m_DiObjectManager.ContainObject(name);
        public bool SelectVar(int id) => m_DiObjectManager.SelectObject(id);
        public IVar GetVarByID(int id) => m_DiObjectManager.GetObjectByID(id);
        public IVar GetVarByName(string name) => m_DiObjectManager.GetObjectByName(name);
        public bool TryDeleteVar(string var_name) => m_DiObjectManager.RemoveObject(var_name);
        public bool TryDeleteVar(int var_id) => m_DiObjectManager.RemoveObject(var_id);
        public bool TryReplaceVar(IVar old_var, IVar new_var) => m_DiObjectManager.ReplaceObject(old_var, new_var);
        public bool TryRenameVar(int obj_id, string new_name) => m_DiObjectManager.RenameObject(obj_id,new_name);

        public void DrawVarList() => m_ListDrawer.DrawList();
        void DrawVarInfo() => m_DiObjectManager.m_SelectedTObj?.DrawEditor();

    }

}
