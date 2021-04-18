using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    public class Singleton<T> where T : new()
    {
        static T __instance = new T();
        public static T Instance
        {
            get => __instance;
            set => __instance = value;
        }
    }

    [Serializable]
    public class VarManager: Singleton<VarManager>
    { 

        TManager<VarBase> m_Manager = new TManager<VarBase>();
        VarListDrawer m_ListDrawer = null;

        public VarManager()
        {
            m_Manager.NewObjectBaseName = "NewVar_";

            var name2obj = m_Manager.Name2Object;
            m_ListDrawer = new VarListDrawer(ref name2obj);

            InitManagerEvent();
            diType.InitClassList();
            IContainer.InitClassList();
        }

        void InitManagerEvent()
        {
            m_ListDrawer.OnDeleteClick += new VarListDrawer.TAction(OnDeleteClick);
            m_ListDrawer.OnSelectClick += new VarListDrawer.TAction(OnSelectClick);
            m_ListDrawer.OnDuplicateClick += new VarListDrawer.TAction(OnDuplicateClick);

            m_Manager.OnObjectDelete += new TManager<VarBase>.ObjectAction(OnVarDelete);
        }

        //List Event
        void OnSelectClick(VarBase variable)
        {
            m_Manager.m_SelectedTObj = variable;
            PrintRightView.Instance.Submit(DrawVarInfo);
        }

        void OnDeleteClick(VarBase variable)
        {
            TryDeleteVar(variable.Name);
        }

        void OnDuplicateClick(VarBase variable)
        {
            VarBase dup = variable.Duplicate();
            AddVar(dup);
        }

        //Var Event
        void OnVarDelete(VarBase @var)
        {
            @var.OnVarDelete();     //Notify var delete action
        }

        public void AddVar() { AddVar(new Variable()); }
        public void AddVar(VarBase variable) => m_Manager.AddObject(variable);
        public bool ContainVar(int id) => m_Manager.ContainObject(id);
        public bool ContainVar(string name) => m_Manager.ContainObject(name);
        public bool SelectVar(int id) => m_Manager.SelectObject(id);
        public VarBase GetVarByID(int id) => m_Manager.GetObjectByID(id);
        public VarBase GetVarByName(string name) => m_Manager.GetObjectByName(name);
        public bool TryDeleteVar(string var_name) => m_Manager.TryDeleteObject(var_name);
        public bool TryDeleteVar(int var_id) => m_Manager.TryDeleteObject(var_id);
        public bool TryReplaceVar(VarBase old_var, VarBase new_var) => m_Manager.TryReplaceObject(old_var, new_var);

        public void DrawVarList() => m_ListDrawer.DrawList();
        void DrawVarInfo() => m_Manager.m_SelectedTObj?.DrawEditor();

    }

}
