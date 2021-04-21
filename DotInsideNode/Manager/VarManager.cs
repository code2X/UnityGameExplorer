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

        TManager<IVar> m_Manager = new TManager<IVar>();
        VarListView m_ListDrawer = null;
        ListMenuView m_ListMenuView = new ListMenuView();

        public VarManager()
        {
            m_Manager.NewObjectBaseName = "NewVar_";

            var name2obj = m_Manager.Name2Object;
            m_ListDrawer = new VarListView(ref name2obj);

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

            m_Manager.OnObjectEvent += new TManager<IVar>.ObjectAction(ManagerEventProc);
        }

        //List Event
        void ListItemEventProc(VarListView.EListItemEvent eEvent, IVar variable)
        {
            switch (eEvent)
            {
                case VarListView.EListItemEvent.SelectItem:
                    m_Manager.m_SelectedTObj = variable;
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
        void ManagerEventProc(TManager<IVar>.EObjectEvent eObjectEvent, IVar @var)
        {
            switch (eObjectEvent)
            {
                case TManager<IVar>.EObjectEvent.Delete:
                    @var.OnVarDelete();     //Notify var delete action
                    break;
            }
        }

        public void AddVar() { AddVar(new Variable()); }
        public void AddVar(IVar variable) => m_Manager.AddObject(variable);
        public bool ContainVar(int id) => m_Manager.ContainObject(id);
        public bool ContainVar(string name) => m_Manager.ContainObject(name);
        public bool SelectVar(int id) => m_Manager.SelectObject(id);
        public IVar GetVarByID(int id) => m_Manager.GetObjectByID(id);
        public IVar GetVarByName(string name) => m_Manager.GetObjectByName(name);
        public bool TryDeleteVar(string var_name) => m_Manager.TryDeleteObject(var_name);
        public bool TryDeleteVar(int var_id) => m_Manager.TryDeleteObject(var_id);
        public bool TryReplaceVar(IVar old_var, IVar new_var) => m_Manager.TryReplaceObject(old_var, new_var);
        public bool TryRenameVar(int obj_id, string new_name) => m_Manager.TryRenameObject(obj_id,new_name);

        public void DrawVarList() => m_ListDrawer.DrawList();
        void DrawVarInfo() => m_Manager.m_SelectedTObj?.DrawEditor();

    }

}
