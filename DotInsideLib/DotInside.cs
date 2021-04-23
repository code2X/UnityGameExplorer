using System;
using System.Collections.Generic;
using ImGuiNET;
using imnodesNET;
using System.Runtime.InteropServices;
using DotInsideNode;

namespace DotInsideLib
{
    public class DotInside
    {
        static InstanceView instanceView;
        static AssemblyExplorerView explorerView;
        public static SortedList<string, CsharpClass> classListDetails = new SortedList<string, CsharpClass>();
        static private IBluePrint enumPrint = new Enumeration();
        static private IBluePrint structPrint = new Structure();

        static unsafe void DotInsideRender()
        {
            Caller.Try(() =>
            {
                enumPrint.Draw();
                structPrint.Draw();

                ContentBrowser.Instance.OnGUI();
                BluePrintWindow.Instance.OnGUI();
                //DotInsideNode.DotPrint.GetInstance().OnGUI();
                //enumPrint.Draw();
                //explorerView.OnGUI();
                //instanceView.OnGUI();
                ImGui.ShowDemoWindow();
                //ArrayInfoWindow.GetInstance().OnGUI();
                //ArrayElementInputWindow.GetInstance().OnGUI();
            });
        }

        static void TestArray()
        {
            int[] array = new int[5];
            array[0] = 1;
            array[1] = 4;
            array[2] = 3;
            array[3] = 3;
            array[4] = 5;

            object obj = array;

            ArrayInfoWindow.GetInstance().Show(obj);
        }

        static bool AddRenderCallback()
        {
            return Caller.Try(() =>
            {
                GIRerSurface.AddRenderCallback(DotInsideRender);
            });
        }

        public static bool AddAssembly(string path)
        {
            bool res;
            res = Caller.Try(() =>
            {
                Assembler assembly = new Assembler(path);
                SortedDictionary<string, Type> className2Type = assembly.GetTypeDict();
                className2Type.Clear();
                className2Type = DotInsideNode.ClassTest.classList;

                explorerView = AssemblyExplorerView.GetInstance();
                explorerView.AutoCluster(className2Type);
            
                foreach (var cls in className2Type)
                {
                    classListDetails.Add(cls.Key, new CsharpClass(cls.Value));
                }
            });
            if (!res) return res;
            
            instanceView = InstanceView.GetInstance();
            AddRenderCallback();            
            return res;
        }

        static void Main(string[] args)
        {
            AddAssembly("Assembly-CSharp.dll");
            TestArray();
            Console.ReadLine();
        }

    }
}
