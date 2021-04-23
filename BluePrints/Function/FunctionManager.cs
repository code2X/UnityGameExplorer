using System;

namespace DotInsideNode
{
    [Serializable]
    class FunctionManager: Singleton<FunctionManager>
    {
        class ListMenuView : TEnumMenuView<IFunction, ListMenuView.EItemEvent>
        {
            public enum EItemEvent
            {
                Open_Graph,
                Open_in_New_Tab,
                Call,
                Delete,
                Duplicate,
            }
        }

        diObjectManager<IFunction> m_DiObjectManager = new diObjectManager<IFunction>();
        [NonSerialized]
        FunctionListView m_ListView = null;
        [NonSerialized]
        ListMenuView m_ListMenuView = new ListMenuView();

        TabBarView m_TabBarView = null;
        NodeEditor m_NodeEditor = null;

        public TabBarView TabBar
        {
            set
            {
                m_TabBarView = value;
                m_TabBarView.OnTabEvent += new TabBarView.TabAction(TabEventProc);
            }
        }

        public NodeEditor NodeEditor
        {
            set => m_NodeEditor = value;
        }

        public FunctionManager()
        {
            m_DiObjectManager.NewObjectBaseName = "NewFunction_";

            m_ListView = new FunctionListView(m_DiObjectManager);
            InitManagerEvent();
        }

        void InitManagerEvent()
        {
            //List Menu Event
            m_ListMenuView.OnMenuEvent += new ListMenuView.TMenuAction(ListMenuItemEventProc);
            m_ListView.MenuDrawer = m_ListMenuView;

            //List Event
            m_ListView.OnListItemEvent += ListItemEventProc;
            m_DiObjectManager.OnObjectEvent += new diObjectManager<IFunction>.ObjectAction(ManagerEventProc);
        }

        void ListItemEventProc(FunctionListView.EListItemEvent eEvent, IFunction function)
        {
            switch (eEvent)
            {
                case FunctionListView.EListItemEvent.SelectItem:
                    m_DiObjectManager.m_SelectedTObj = function;
                    PrintRightView.Instance.Submit(DrawFunctionInfo);
                    break;
                case FunctionListView.EListItemEvent.DoubleSelectItem:
                    Logger.Info(function.Name);
                    m_TabBarView?.OpenTab(function);
                    break;
            }
        }

        void ListMenuItemEventProc(ListMenuView.EMenuEvent eMenuEvent,ListMenuView.EItemEvent eItemEvent, IFunction function)
        {
            object[] outParams = null;
            switch (eItemEvent)
            {
                case ListMenuView.EItemEvent.Open_Graph:
                    m_TabBarView?.OpenTab(function);
                    break;
                case ListMenuView.EItemEvent.Open_in_New_Tab:
                    m_TabBarView?.OpenNewTab(function);
                    break;
                case ListMenuView.EItemEvent.Call:
                    function.Execute(0,null,out outParams);
                    break;
                case ListMenuView.EItemEvent.Delete:
                    TryDeleteFunction(function.ID);
                    break;
            }
        }

        void TabEventProc(TabBarView.ETabEvent eTabEvent,dnObject obj)
        {
            IFunction function;
            switch (eTabEvent)
            {
                case TabBarView.ETabEvent.Open:
                    function = (IFunction)obj;
                    function.OpenGraph();
                    m_NodeEditor?.Open();
                    break;
                case TabBarView.ETabEvent.Close:
                    function = (IFunction)obj;
                    function.CloseGraph();
                    m_NodeEditor?.Close();
                    break;
            }
        }

        //Var Event
        void ManagerEventProc(diObjectManager<IFunction>.EObjectEvent eObjectEvent, IFunction function)
        {
            switch(eObjectEvent)
            {
                case diObjectManager<IFunction>.EObjectEvent.Delete:
                    break;
            }            
        }

        //Function Manager 
        public void AddFunction(IFunction variable) => m_DiObjectManager.AddObject(variable);
        public bool ContainFunction(int id) => m_DiObjectManager.ContainObject(id);
        public bool ContainFunction(string name) => m_DiObjectManager.ContainObject(name);
        public bool SelectFunction(int id) => m_DiObjectManager.SelectObject(id);
        public IFunction GetFunctionByID(int id) => m_DiObjectManager.GetObjectByID(id);
        public IFunction GetFunctionByName(string name) => m_DiObjectManager.GetObjectByName(name);
        public bool TryDeleteFunction(string var_name) => m_DiObjectManager.RemoveObject(var_name);
        public bool TryDeleteFunction(int var_id) => m_DiObjectManager.RemoveObject(var_id);

        //Drawer
        public void DrawFunctionList() => m_ListView.DrawList();
        public void DrawFunctionInfo() => m_DiObjectManager.m_SelectedTObj.DrawEditor();

    }
}
