using System;
using System.Collections.Generic;
using System.Reflection;
using DotInsideLib;

/// <summary>
/// Assembly Encapsulation 
/// </summary>
public class Assembler
{
    Assembly assembly;
    public Type[] GetTypeList() => assembly.GetExportedTypes();

    public Assembler(string filePath)
    {
        Caller.Try(() =>
        {
            assembly = Assembly.LoadFrom(filePath);
        });
    }      

    public SortedDictionary<string, Type> GetTypeDict(bool emptyNamespace = true)
    {
        SortedDictionary<string, Type> sortDict = new SortedDictionary<string, Type>();

        Caller.Try(() =>
        {
            foreach (var i in GetTypeList())
            {
                if (!emptyNamespace)
                {
                    TryAddKey(i);
                }
                else if (i.Namespace == null)
                {
                    TryAddKey(i);
                }
            }
        });

        return sortDict;

        void TryAddKey(Type i)
        {
            if (sortDict.ContainsKey(i.Name) == false && i.IsGenericType == false)
                sortDict.Add(i.Name, i);
        }
    }

    public static Type[] GetNamespaceTypes()
    {
        return Assembly.GetExecutingAssembly().GetTypes();
    }

}

