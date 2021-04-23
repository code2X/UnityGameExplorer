using System;
using System.Collections.Generic;
using DotInsideLib;

namespace DotInsideNode
{
    class ClassTest
    {
        static SortedDictionary<string, Type> m_List = new SortedDictionary<string, Type>();

        public static SortedDictionary<string, Type> classList
        {
            get
            {
                if(m_List.Count == 0)
                {
                    m_List.Add("Test1", typeof(Test1));
                }
                return m_List;
            }
        }
    }

    class Test1
    {
        static Test1 __instance = new Test1();

        public static int s_int_var = 10;
        public static bool s_bool_var = false;
        public static float s_float_var = 10;
        public static double s_double_var = 10;
        public static string s_string_var = "";

        public int m_int_var = 10;
        public bool m_bool_var = false;
        public float m_float_var = 10;
        public double m_double_var = 10;
        public string m_string_var = "";

        public static Test1 s_pInstatnce
        {
            get
            {
                return __instance;
            }
        }
    }
}
