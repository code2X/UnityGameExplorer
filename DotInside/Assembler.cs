using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ExplorerSpace
{
    /// <summary>
    /// Assembly Encapsulation 
    /// </summary>
    class Assembler
    {
        Assembly assembly;

        public Assembler(string filePath)
        {
            try
            {
                assembly = Assembly.LoadFrom(filePath);
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }
        }

        public Type[] GetTypeList() => assembly.GetExportedTypes();

        public SortedDictionary<string, Type> getTypeDict(bool emptyNamespace = true)
        {
            SortedDictionary<string, Type> sortDict = new SortedDictionary<string, Type>(); 

            try
            {
                foreach (var i in GetTypeList())
                {
                    if(!emptyNamespace)
                    {
                        TryAddKey(i);
                    }
                    else if (i.Namespace == null)
                    {
                        TryAddKey(i);
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }

            return sortDict;

            void TryAddKey(Type i)
            {
                if (sortDict.ContainsKey(i.Name) == false && i.IsGenericType == false)
                    sortDict.Add(i.Name, i);
            }
        }
    }
}
