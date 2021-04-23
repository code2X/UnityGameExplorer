using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DotInsideNode
{
    [Serializable]
    public class KeyNameList<T> where T : dnObject
    {
        [NonSerialized]
        public HashSet<string> m_NameSet = new HashSet<string>();
        List<T> m_ObjList = new List<T>();
        string m_NewObjectBaseName = "";

        public List<T> ObjList => m_ObjList;
        public string NewObjectBaseName
        {
            get => m_NewObjectBaseName;
            set => m_NewObjectBaseName = value;
        }

        #region Deserialized
        [OnDeserializedAttribute]
        private void OnDeserialized(StreamingContext sc)
        {
            m_NameSet = new HashSet<string>();
            foreach (T obj in m_ObjList)
            {
                Assert.IsTrue(m_NameSet.Add(obj.Name));
            }
        }
        #endregion

        string GetNewName()
        {
            int index = 0;
            string varName = NewObjectBaseName + index;
            while (ContainName(varName))
            {
                ++index;
                varName = NewObjectBaseName + index;
            }

            return varName;
        }

        public virtual void AddObject(T obj)
        {
            string name = GetNewName();
            obj.Name = name;
            obj.ID = m_ObjList.Count;

            m_NameSet.Add(name);
            m_ObjList.Add(obj);
        }

        public virtual bool ContainName(string name)
        {
            return m_NameSet.Contains(name);
        }

        public bool RemoveObject(T tObj)
        {
            bool res = m_ObjList.Remove(tObj);
            if (res)
            {
                m_NameSet.Remove(tObj.Name);
                ResetObjID();
            }
            return res;
        }

        void ResetObjID()
        {
            for (int i = 0; i < m_ObjList.Count; ++i)
            {
                m_ObjList[i].ID = i;
            }
        }

        public bool Rename(int index,string newName)
        {
            if(m_NameSet.Contains(newName))
            {
                return false;
            }
            else
            {
                m_NameSet.Remove(m_ObjList[index].Name);
                m_ObjList[index].Name = newName;
                Assert.IsTrue(m_NameSet.Add(newName));
                return true;
            }
        }

        public bool Exchange(int indexL, int indexR)
        {
            if (0 <= indexL && indexL < m_ObjList.Count &&
                0 <= indexR && indexR < m_ObjList.Count)
            {
                T tmp = m_ObjList[indexL];
                m_ObjList[indexL] = m_ObjList[indexR];
                m_ObjList[indexR] = tmp;
                ResetObjID();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
