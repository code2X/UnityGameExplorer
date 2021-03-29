using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ExplorerSpace
{
    /// <summary>
    /// Assembly Encapsulation 
    /// </summary>
    class AssemblyClass
    {
        Assembly assembly;

        public AssemblyClass(string filePath)
        {
            try
            {
                assembly = Assembly.LoadFrom(filePath);
            }
            catch (Exception exp)
            {
                Debug.LogError(exp.Message);
            }
        }

        public SortedDictionary<string, Type> getTypeDict()
        {
            SortedDictionary<string, Type> sortDict = new SortedDictionary<string, Type>();

            try
            {
                foreach (var i in assembly.GetExportedTypes())
                {
                    if (i.Namespace == null)
                    {
                        if (sortDict.ContainsKey(i.Name) || i.IsGenericType)
                        { }
                        else
                        {
                            sortDict.Add(i.Name, i);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Debug.LogError(exp.Message);
            }

            return sortDict;
        }

        public static SortedDictionary<string, Type> GetUntiyClass(ref SortedDictionary<string, Type> classDict)
        {
            SortedDictionary<string, Type> sortUnityClass = new SortedDictionary<string, Type>();
            foreach (var i in classDict)
            {
                if (i.Value.IsSubclassOf(typeof(ManualBehaviour))
                    || i.Value.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    //Console.WriteLine(i.Key);
                    sortUnityClass.Add(i.Key, i.Value);
                }
            }
            return sortUnityClass;
        }

        public static SortedDictionary<string, Type> GetGeneralClass(ref SortedDictionary<string, Type> classDict)
        {
            SortedDictionary<string, Type> sortGeneralClass = new SortedDictionary<string, Type>();
            foreach (var i in classDict)
            {
                if (i.Value.IsSubclassOf(typeof(ManualBehaviour))
                    || i.Value.IsSubclassOf(typeof(MonoBehaviour)))
                {

                }
                else
                {
                    sortGeneralClass.Add(i.Key, i.Value);
                }
            }
            return sortGeneralClass;
        }
    }
}
