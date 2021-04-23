using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotInsideNode
{
    class ParamManager
    {
        diObjectManager<IParam> m_DiObjectManager = new diObjectManager<IParam>();

        public ParamManager()
        {
            m_DiObjectManager.NewObjectBaseName = "NewParam";
        }

        //Param Manager 
        public int ParamCount => m_DiObjectManager.ID2Object.Count;
        public void AddParam(IParam variable) => m_DiObjectManager.AddObject(variable);
        public bool ContainParam(int id) => m_DiObjectManager.ContainObject(id);
        public bool ContainParam(string name) => m_DiObjectManager.ContainObject(name);
        public bool SelectParam(int id) => m_DiObjectManager.SelectObject(id);
        public IParam GetParamByID(int id) => m_DiObjectManager.GetObjectByID(id);
        public IParam GetParamByName(string name) => m_DiObjectManager.GetObjectByName(name);
        public bool TryDeleteVar(string var_name) => m_DiObjectManager.RemoveObject(var_name);
        public bool TryDeleteVar(int var_id) => m_DiObjectManager.RemoveObject(var_id);

        public void DrawParamList()
        {
            foreach(var param_pair in m_DiObjectManager.ID2Object)
            {
                param_pair.Value.DrawEditor();
            }
        }

        public delegate void ParamAction(IParam param);
        public void ExecuteForEachParam(ParamAction paramAction)
        {
            Assert.IsNotNull(paramAction);

            foreach (var param_pair in m_DiObjectManager.ID2Object)
            {
                paramAction(param_pair.Value);
            }
        }
    }
}
