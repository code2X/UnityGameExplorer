using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotInsideNode
{
    public class TManager<T> where T : diObject
    {
        Dictionary<string, T> m_Name2Objs = new Dictionary<string, T>();
        Dictionary<int, T> m_ID2Objs = new Dictionary<int, T>();
        string m_NewObjectBaseName = "";

        public T m_SelectedTObj = null;

        //Event
        public enum EObjectEvent
        {
            Delete
        }
        public delegate void ObjectAction(EObjectEvent eObjectEvent,T obj);
        public event ObjectAction OnObjectEvent;

        //Field Interface
        public Dictionary<string, T> Name2Object
        {
            get => m_Name2Objs;
        }

        public Dictionary<int, T> ID2Object
        {
            get => m_ID2Objs;
        }

        public string NewObjectBaseName
        {
            get => m_NewObjectBaseName;
            set => m_NewObjectBaseName = value;
        }

        string GetNewName()
        {
            int index = 0;
            string varName = NewObjectBaseName + index;
            while (m_Name2Objs.ContainsKey(varName))
            {
                ++index;
                varName = NewObjectBaseName + index;
            }

            return varName;
        }

        int GetNewVarID()
        {
            int index = 0;
            while (m_ID2Objs.ContainsKey(index))
            {
                ++index;
            }

            return index;
        }

        public virtual void AddObject(T variable)
        {
            string name = GetNewName();
            int id = GetNewVarID();
            variable.Name = name;
            variable.ID = id;

            m_Name2Objs.Add(name, variable);
            m_ID2Objs.Add(id, variable);
        }

        public virtual bool ContainObject(int id)
        {
            return GetObjectByID(id) != null ? true : false;
        }

        public virtual bool ContainObject(string name)
        {
            return GetObjectByName(name) != null ? true : false;
        }

        public virtual bool SelectObject(int id)
        {
            T tObj;
            if ((tObj = GetObjectByID(id)) != null)
            {
                m_SelectedTObj = tObj;
                return true;
            }
            return false;
        }

        public virtual T GetObjectByID(int id)
        {
            T tObj;
            return m_ID2Objs.TryGetValue(id, out tObj) ? tObj : null;
        }

        public virtual T GetObjectByName(string name)
        {
            T tObj;
            return m_Name2Objs.TryGetValue(name, out tObj) ? tObj : null;
        }

        public virtual bool TryDeleteObject(string name)
        {
            if (name == string.Empty)
                return false;
        
            T obj;
            return m_Name2Objs.TryGetValue(name, out obj) ? TryDeleteObject(obj) : false;
        }

        public virtual bool TryDeleteObject(int id)
        {
            if (id == 0)
                return false;
        
            T obj;
            return m_ID2Objs.TryGetValue(id, out obj) ? TryDeleteObject(obj) : false;
        }

        bool TryDeleteObject(T tObj)
        {
            if (tObj == null)
                return false;

            //Delete Event
            OnObjectEvent?.Invoke(EObjectEvent.Delete,tObj);

            if (m_ID2Objs.Remove(tObj.ID) == false)
            {
                Logger.Warn("Try delete var not in id dict");
                return false;
            }
            if (m_Name2Objs.Remove(tObj.Name) == false)
            {
                Logger.Warn("Try delete var not in name dict");
                return false;
            }
            return true;
        }

        public bool TryReplaceObject(T old_obj, T new_obj)
        {
            T var1, var2;
            if (m_Name2Objs.TryGetValue(old_obj.Name, out var1) == false ||
                m_ID2Objs.TryGetValue(old_obj.ID, out var2) == false)
                return false;

            if (var1.ID != var2.ID || var1.Name != var2.Name)
                return false;

            m_ID2Objs[old_obj.ID] = new_obj;
            m_Name2Objs[old_obj.Name] = new_obj;
            return true;
        }

        public bool TryRenameObject(int obj_id,string new_name)
        {
            if (ContainObject(obj_id) == false)
                return false;
            if (ContainObject(new_name) == true)
                return false;

            T obj = GetObjectByID(obj_id);
            Assert.IsNotNull(obj);
            Assert.IsTrue(m_Name2Objs.Remove(obj.Name));
            m_Name2Objs.Add(new_name, obj);

            return true;
        }

    }

}
