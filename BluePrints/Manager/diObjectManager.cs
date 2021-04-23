using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotInsideNode
{
    public class diObjectManager<T> where T : dnObject
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
            int index = 1;
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
            return m_ID2Objs.TryGetValue(id, out T tObj) ? tObj : null;
        }

        public virtual T GetObjectByName(string name)
        {
            return m_Name2Objs.TryGetValue(name, out T tObj) ? tObj : null;
        }

        public virtual bool RemoveObject(string name)
        {
            if (name == string.Empty)
                return false;

            return m_Name2Objs.TryGetValue(name, out T obj) ? RemoveObject(obj) : false;
        }

        public virtual bool RemoveObject(int id)
        {
            if (id == 0)
                return false;

            return m_ID2Objs.TryGetValue(id, out T obj) ? RemoveObject(obj) : false;
        }

        bool RemoveObject(T tObj)
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

        public bool ReplaceObject(T oldObj, T newObj)
        {
            if (m_Name2Objs.TryGetValue(oldObj.Name, out T var1) == false ||
                m_ID2Objs.TryGetValue(oldObj.ID, out T var2) == false)
                return false;

            if (var1.ID != var2.ID || var1.Name != var2.Name)
                return false;

            m_ID2Objs[oldObj.ID] = newObj;
            m_Name2Objs[oldObj.Name] = newObj;
            return true;
        }

        public bool RenameObject(int objId,string newName)
        {
            if (ContainObject(objId) == false)
                return false;
            if (ContainObject(newName) == true)
                return false;

            T obj = GetObjectByID(objId);
            Assert.IsNotNull(obj);
            Assert.IsTrue(m_Name2Objs.Remove(obj.Name));
            m_Name2Objs.Add(newName, obj);

            return true;
        }

    }

}
