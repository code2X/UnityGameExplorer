using System;
using System.Collections.Generic;
using System.Reflection;
using DotInsideNode;

namespace DotInsideLib
{
    class CsharpKeyword
    {
        public static HashSet<Type> GeneralTypes = new HashSet<Type>
        {
            typeof(int),
            typeof(float),
            typeof(byte),
            typeof(double),
            typeof(ulong),
            typeof(uint),
            typeof(bool),
            typeof(Boolean),
            typeof(short),
            typeof(long),
            typeof(string)
        };
    }

    public class CsharpClass
    {
        MethodInfo[] methodInfos;
        PropertyInfo[] propertyInfos;
        FieldInfo[] fieldInfos;
        EventInfo[] eventInfos;

        SortedList<string, FieldInfo> sortField;
        SortedList<string, PropertyInfo> sortProperty;
        SortedList<string, MethodInfo> sortMethod;
        SortedList<string, MethodInfo> sortGetMethod;
        SortedList<string, MethodInfo> sortSetMethod;

        SortedList<string, FieldInfo> sortStaticField;
        SortedList<string, PropertyInfo> sortStaticProperty;
        SortedList<string, MethodInfo> sortStaticMethod;
        SortedList<string, MethodInfo> sortStaticGetMethod;
        SortedList<string, MethodInfo> sortStaticSetMethod;

        public string typeName = "";
        public Type type;

        public SortedList<string, FieldInfo> FieldList
        {
            get
            {
                return sortField;
            }
        }

        //[NodeCompAttr(ComponentType.Output, "Propertys",typeof(PropertyInfo))]
        public SortedList<string, PropertyInfo> PropList
        {
            get
            {
                return sortProperty;
            }
        }

        public SortedList<string, MethodInfo> MethodList
        {
            get
            {
                return sortMethod;
            }
        }

        public SortedList<string, MethodInfo> GetMethodList
        {
            get
            {
                return sortGetMethod;
            }
        }
        public SortedList<string, MethodInfo> SetMethodList
        {
            get
            {
                return sortSetMethod;
            }
        }
        public SortedList<string, FieldInfo> StaticFieldList
        {
            get
            {
                return sortStaticField;
            }
        }
        public SortedList<string, MethodInfo> StaticGetMethodList
        {
            get
            {
                return sortStaticGetMethod;
            }
        }
        public SortedList<string, MethodInfo> StaticSetMethodList
        {
            get
            {
                return sortStaticSetMethod;
            }
        }
        public SortedList<string, MethodInfo> StaticMethodList
        {
            get
            {
                return sortStaticMethod;
            }
        }

        public SortedList<string, PropertyInfo> StaticPropList
        {
            get
            {
                return sortStaticProperty;
            }
        }

        public CsharpClass(Type classType)
        {
            try
            {
                type = classType;
                typeName = type.Name;

                sortField = new SortedList<string, FieldInfo>();
                sortProperty = new SortedList<string, PropertyInfo>();
                sortMethod = new SortedList<string, MethodInfo>();
                sortGetMethod = new SortedList<string, MethodInfo>();
                sortSetMethod = new SortedList<string, MethodInfo>();

                sortStaticField = new SortedList<string, FieldInfo>();
                sortStaticProperty = new SortedList<string, PropertyInfo>();
                sortStaticMethod = new SortedList<string, MethodInfo>();
                sortStaticGetMethod = new SortedList<string, MethodInfo>();
                sortStaticSetMethod = new SortedList<string, MethodInfo>();

                methodInfos = classType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                propertyInfos = classType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                fieldInfos = classType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                eventInfos = classType.GetEvents();

                AddNameList();
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }
        }

        //Get a name new in sortlist
        private string GetName<T>(SortedList<string, T> sortList, string name)
        {
            while (sortList.ContainsKey(name))
            {
                name += "#";
            }
            return name;
        }

        private void AddNameList()
        {
            //Method
            foreach (MethodInfo m in methodInfos)
            {             
                if(m.IsStatic)
                {
                    //Property Function
                    if (m.Name.StartsWith("get_"))
                    {
                        string name = GetName(sortStaticGetMethod, m.Name);
                        sortStaticGetMethod.Add(name, m);
                    }
                    else if (m.Name.StartsWith("set_"))
                    {
                        string name = GetName(sortStaticSetMethod, m.Name);
                        sortStaticSetMethod.Add(name, m);
                    }
                    //General Function
                    else
                    {
                        string name = GetName(sortStaticMethod, m.Name);
                        sortStaticMethod.Add(name, m);
                    }
                }
                else
                {
                    //Unity Function
                    if (m.Name.StartsWith("BroadcastMessage") ||
                      m.Name.StartsWith("CancelInvoke") ||
                      m.Name.StartsWith("GetComponent") ||
                      m.Name.StartsWith("SendMessage") ||
                      m.Name.StartsWith("StartCoroutine") ||
                      m.Name.StartsWith("StopAllCoroutine") ||
                      m.Name.StartsWith("StopCoroutine") ||
                      m.Name.StartsWith("Invoke") ||
                      m.Name.StartsWith("ToString") ||
                      m.Name.StartsWith("GetType") ||
                      m.Name.StartsWith("Get") ||
                      m.Name.StartsWith("IsInvok") ||
                      m.Name == "CompareTo" ||
                      m.Name == "Equals")
                        continue;

                    //Property Function
                    if (m.Name.StartsWith("get_"))
                    {
                        string name = GetName(sortGetMethod, m.Name);
                        sortGetMethod.Add(name, m);
                    }
                    else if (m.Name.StartsWith("set_"))
                    {
                        string name = GetName(sortSetMethod, m.Name);
                        sortSetMethod.Add(name, m);
                    }
                    //General Function
                    else
                    {
                        string name = GetName(sortMethod, m.Name);
                        sortMethod.Add(name, m);
                    }
                }
          
            }

            //Property
            foreach (PropertyInfo m in propertyInfos)
            {
                MethodInfo[] methodInfo = m.GetAccessors();
                if(methodInfo.Length > 0 && methodInfo[0].IsStatic)
                {
                    string name = GetName(sortStaticProperty, m.Name);
                    sortStaticProperty.Add(name, m);
                }
                else
                {
                    string name = GetName(sortProperty, m.Name);
                    sortProperty.Add(name, m);
                }
            }

            //Field
            foreach (FieldInfo m in fieldInfos)
            {
                //Static Field
                if (m.IsStatic)
                {
                    string name = GetName(sortStaticField, m.Name);
                    sortStaticField.Add(name, m);
                }
                //General Field
                else
                {
                    string name = GetName(sortField, m.Name);
                    sortField.Add(name, m);
                }
            }

        }
    }
}
