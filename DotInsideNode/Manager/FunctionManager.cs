using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    [Serializable]
    class FunctionManager
    {
        static FunctionManager __instance = new FunctionManager();
        public static FunctionManager Instance
        {
            get => __instance;
        }

        TManager<FunctionBase> m_Manager = new TManager<FunctionBase>();
        [NonSerialized]
        FunctionListDrawer m_ListDrawer = null;

        TabBarView m_TabBarView = null;
        NodeEditor m_NodeEditor = null;

        public TabBarView TabBar
        {
            set
            {
                m_TabBarView = value;
                m_TabBarView.OnTabOpen += new TabBarView.TabAction(OnTabOpen);
                m_TabBarView.OnTabClose += new TabBarView.TabAction(OnTabClose);
            }
        }

        public NodeEditor NodeEditor
        {
            set
            {
                m_NodeEditor = value;
            }
        }

        FunctionManager()
        {
            m_Manager.NewObjectBaseName = "NewFunction_";

            var name2obj = m_Manager.Name2Object;
            m_ListDrawer = new FunctionListDrawer(ref name2obj);

            InitManagerEvent();
        }

        void InitManagerEvent()
        {
            m_ListDrawer.OnDeleteClick += new FunctionListDrawer.TAction(OnDeleteClick);
            m_ListDrawer.OnSelectClick += new FunctionListDrawer.TAction(OnSelectClick);
            m_ListDrawer.OnSelectDoubleClick += new FunctionListDrawer.TAction(OnSelectDoubleClick);
            m_ListDrawer.OnDuplicateClick += new FunctionListDrawer.TAction(OnDuplicateClick);

            m_Manager.OnObjectDelete += new TManager<FunctionBase>.ObjectAction(OnFunctionDelete);
        }

        void OnSelectClick(FunctionBase function)
        {
            m_Manager.m_SelectedTObj = function;
            PrintRightView.Instance.Submit(DrawFunctionInfo);
        }

        void OnSelectDoubleClick(FunctionBase function)
        {
            Logger.Info(function.Name);
            m_TabBarView?.OpenTab(function);
        }

        void OnTabOpen(diObject obj)
        {
            FunctionBase function = (FunctionBase)obj;
            function.OpenGraph();
            m_NodeEditor?.Open();
        }

        void OnTabClose(diObject obj)
        {
            FunctionBase function = (FunctionBase)obj;
            function.CloseGraph();
            m_NodeEditor?.Close();
        }

        void OnDeleteClick(FunctionBase function)
        {
            TryDeleteVar(function.Name);
        }
        
        void OnDuplicateClick(FunctionBase variable)
        {
        //    IFunction dup = variable.Duplicate();
        //    AddVar(dup);
        }

        //Var Event
        void OnFunctionDelete(FunctionBase function)
        {
            function.OnFunctionDelete();     //Notify var delete action
        }

        //Function Manager 
        public void AddFunction(FunctionBase variable) => m_Manager.AddObject(variable);
        public bool ContainFunction(int id) => m_Manager.ContainObject(id);
        public bool ContainFunction(string name) => m_Manager.ContainObject(name);
        public bool SelectFunction(int id) => m_Manager.SelectObject(id);
        public FunctionBase GetFunctionByID(int id) => m_Manager.GetObjectByID(id);
        public FunctionBase GetFunctionByName(string name) => m_Manager.GetObjectByName(name);
        public bool TryDeleteVar(string var_name) => m_Manager.TryDeleteObject(var_name);
        public bool TryDeleteVar(int var_id) => m_Manager.TryDeleteObject(var_id);

        //Drawer
        public void DrawFunctionList() => m_ListDrawer.DrawList();
        public void DrawFunctionInfo() => m_Manager.m_SelectedTObj.DrawEditor();

    }
}
