using System;
using System.Collections.Generic;
using ImGuiNET;
using System.Runtime.InteropServices;

namespace ExplorerSpace
{
    public enum ClassInfo
    {
        Null,
        Field,
        Property,
        Method,
    }

    class UnityGameExplorer
    {
        static InstanceView instanceView;
        static AssemblyExplorerView explorerView;
        static Assembler g_Assembly;
        public static SortedDictionary<string, Type> g_ClassName2Type;
        public static SortedList<string, CsharpClass> classListDetails;

        static unsafe void MyRender()
        {
            explorerView.OnGUI();
            instanceView.OnGUI();
            ImGui.ShowDemoWindow();
            MethodInvokeWindow.GetInstance().OnGUI();
            ArrayInfoWindow.GetInstance().OnGUI();
            ArrayElementInputWindow.GetInstance().OnGUI();
        }

        static void Cout(object val)
        {
            Console.WriteLine(val);
        }

        static void Test()
        {
            int[] array = new int[5];
            array[0] = 1;
            array[1] = 4;
            array[2] = 3;
            array[3] = 3;
            array[4] = 5;

            //Type type = array.GetType();
            object obj = array;
            ////object[] objArray = (object[])array;
            //Cout(obj);
            //foreach(var i in (Array)obj)
            //{
            //    Cout(i);
            //}

            ArrayInfoWindow.GetInstance().Show(obj);
        }

        static void Main(string[] args)
        {
            explorerView = AssemblyExplorerView.GetInstance();
            classListDetails = new SortedList<string, CsharpClass>();

            try
            {
                g_Assembly = new Assembler("Assembly-CSharp.dll");
                g_ClassName2Type = g_Assembly.getTypeDict();

                explorerView.AutoCluster(g_ClassName2Type);

                foreach (var cls in g_ClassName2Type)
                {
                    classListDetails.Add(cls.Key, new CsharpClass(cls.Value));
                }
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }
            instanceView = InstanceView.GetInstance();

            
            try
            {
                GIRerSurface.AddRenderCallback(MyRender);
                Test();
                Console.ReadLine();
            }
            catch(Exception exp)
            {
                Logger.Error(exp);
            }

        }


    }
}
