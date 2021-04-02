using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExplorerSpace
{
    class ClassCluster
    {
        static SortedDictionary<string, Type> NewClassDict() => new SortedDictionary<string, Type>();

        /// <summary>
        /// Main Cluster: UI, Enum, All, Root(include main class number)
        /// </summary>
        /// <param name="classDict"></param>
        /// <returns>Cluster result</returns>
        public static SortedDictionary<string, SortedDictionary<string, Type>> GetMainCluster(SortedDictionary<string, Type> classDict)
        {
            var classCluster = new SortedDictionary<string, SortedDictionary<string, Type>>();
            classCluster.Add("UI", NewClassDict());
            classCluster.Add("Enum Class", NewClassDict());
            classCluster.Add("All Class", NewClassDict());

            //PreProcess UI And Enum
            foreach (var i in classDict)
            {              
                if (i.Value.IsEnum)
                {
                    classCluster["Enum Class"].Add(i.Key, i.Value);
                }
                else if (i.Key.StartsWith("UI"))
                {
                    classCluster["UI"].Add(i.Key, i.Value);
                }
                classCluster["All Class"].Add(i.Key, i.Value);
            }

            RootClassCluster(ref classCluster, classDict);

            return classCluster;
        }

        /// <summary>
        /// Auto Cluster: cluster by start name and end name
        /// </summary>
        /// <param name="classDict"></param>
        /// <returns>Cluster result</returns>
        public static SortedDictionary<string, SortedDictionary<string, Type>> GetAutoCluster(SortedDictionary<string, Type> classDict)
        {
            var classCluster = new SortedDictionary<string, SortedDictionary<string, Type>>();
            var OtherDict = NewClassDict();

            foreach (var i in classDict)
            {
                if (i.Value.IsEnum == false)
                {
                    OtherDict.Add(i.Key, i.Value);
                }
            }

            StartNameCluster(ref classCluster, classDict, OtherDict);
            EndNameCluster(ref classCluster, classDict, OtherDict);

            return classCluster;
        }

        static void RootClassCluster(
            ref SortedDictionary<string, SortedDictionary<string, Type>> classCluster,
            SortedDictionary<string, Type> allClass,
            uint limit = 10
            )
        {
            classCluster.Add("Root Class", NewClassDict());

            foreach(var class2type in allClass)
            {
                int num = CountClassInclude(class2type.Value, allClass);
                if(num > limit)
                {
                    classCluster["Root Class"].Add(class2type.Key, class2type.Value);
                }
            }
        }

        /// <summary>
        /// count class include other main class 
        /// </summary>
        /// <param name="type">class type</param>
        /// <param name="allClass"></param>
        /// <returns></returns>
        static int CountClassInclude(
            Type type,
            SortedDictionary<string, Type> allClass
        )
        {
            HashSet<Type> types = new HashSet<Type>();
            int bias = 0;     //

            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            //count field class include number
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if(allClass.ContainsKey(fieldInfo.FieldType.Name))
                {
                    //static class field
                    if (fieldInfo.IsStatic && fieldInfo.FieldType.IsEnum == false)
                    {
                        bias += 1;
                    }                      
                    types.Add(fieldInfo.FieldType);
                }
            }

            //count property class include number
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (allClass.ContainsKey(propertyInfo.PropertyType.Name))
                {
                    //static class property
                    if (propertyInfo.GetAccessors().Length > 0 && 
                        propertyInfo.GetAccessors()[0].IsStatic &&
                        propertyInfo.PropertyType.IsEnum == false)
                    {
                        bias += 1;
                    }                      
                    types.Add(propertyInfo.PropertyType);
                }
            }

            return types.Count + bias;
        }

        static void StartNameCluster<T>(
            ref SortedDictionary<string, SortedDictionary<string, Type>> classCluster,
            SortedDictionary<string, Type> allClass,
            SortedDictionary<string, T> classDict
            )
        {
            Dictionary<string, int> startNameCount = StartNameCount(classDict);

            foreach (var i in startNameCount)
            {
                if (i.Value > 4 &&
                    i.Key.Length > 1 &&
                    classCluster.ContainsKey(i.Key) == false)
                {
                    classCluster.Add(i.Key, new SortedDictionary<string, Type>());
                    foreach (var str2type in allClass)
                    {
                        if (str2type.Key.StartsWith(i.Key))
                        {
                            classCluster[i.Key].Add(str2type.Key, str2type.Value);
                        }
                    }
                }
            }
        }

        static void EndNameCluster<T>(
            ref SortedDictionary<string, SortedDictionary<string, Type>> classCluster,
            SortedDictionary<string, Type> allClass,
            SortedDictionary<string, T> classDict
            )
        {
            Dictionary<string, int> endNameCount = EndNameCount(classDict);

            foreach (var i in endNameCount)
            {
                if (i.Value > 4 &&
                    i.Key.Length > 1 &&
                    classCluster.ContainsKey(i.Key) == false)
                {
                    classCluster.Add(i.Key, new SortedDictionary<string, Type>());
                    foreach (var str2type in allClass)
                    {
                        if (str2type.Key.EndsWith(i.Key))
                        {
                            classCluster[i.Key].Add(str2type.Key, str2type.Value);
                        }
                    }
                }
            }
        }

        static Dictionary<string, int> StartNameCount<T>(SortedDictionary<string, T> dict)
        {
            Dictionary<string, int> startNameCount = new Dictionary<string, int>();

            foreach (var i in dict)
            {
                string word = fristWord(i.Key);
                if (startNameCount.ContainsKey(word))
                {
                    ++startNameCount[word];
                }
                else
                {
                    startNameCount[word] = 1;
                }
            }

            return startNameCount;
        }

        static Dictionary<string, int> EndNameCount<T>(SortedDictionary<string, T> dict)
        {
            Dictionary<string, int> endNameCount = new Dictionary<string, int>();

            foreach (var i in dict)
            {
                string word = endWord(i.Key);
                if (endNameCount.ContainsKey(word))
                {
                    ++endNameCount[word];
                }
                else
                {
                    endNameCount[word] = 1;
                }
            }

            return endNameCount;
        }

        static string fristWord(string word)
        {
            if (word.Length > 2)
            {
                for (int i = 1; i < word.Length; ++i)
                {
                    if ('A' <= word[i] && 'Z' >= word[i])
                    {
                        return word.Substring(0, i);
                    }
                }
            }
            return word;
        }

        static string endWord(string word)
        {
            for (int i = word.Length - 1; i >= 0; --i)
            {
                if ('A' <= word[i] && 'Z' >= word[i])
                {
                    return word.Substring(i, word.Length - i);
                }
            }
            return word;
        }
    }
}
