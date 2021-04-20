using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotInsideNode
{
    class ParamManager
    {
        TManager<IParam> m_Manager = new TManager<IParam>();

        public ParamManager()
        {
            m_Manager.NewObjectBaseName = "NewParam";
        }

        //Param Manager 
        public int ParamCount => m_Manager.ID2Object.Count;
        public void AddParam(IParam variable) => m_Manager.AddObject(variable);
        public bool ContainParam(int id) => m_Manager.ContainObject(id);
        public bool ContainParam(string name) => m_Manager.ContainObject(name);
        public bool SelectParam(int id) => m_Manager.SelectObject(id);
        public IParam GetParamByID(int id) => m_Manager.GetObjectByID(id);
        public IParam GetParamByName(string name) => m_Manager.GetObjectByName(name);
        public bool TryDeleteVar(string var_name) => m_Manager.TryDeleteObject(var_name);
        public bool TryDeleteVar(int var_id) => m_Manager.TryDeleteObject(var_id);

        public void DrawParamList()
        {
            foreach(var param_pair in m_Manager.ID2Object)
            {
                param_pair.Value.DrawEditor();
            }
        }

        public delegate void ParamAction(IParam param);
        public void ExecuteForEachParam(ParamAction paramAction)
        {
            Assert.IsNotNull(paramAction);

            foreach (var param_pair in m_Manager.ID2Object)
            {
                paramAction(param_pair.Value);
            }
        }
    }
}
