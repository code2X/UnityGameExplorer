using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System;
using System.Threading;

namespace ExplorerSpace
{
    public enum ClassInfo
    {
        Null,
        Field,
        Property,
        Method,
        StaticField,
        StaticProp,
        StaticMethod
    }

    [BepInPlugin("UnityGameExplorer", "Unity Game Explorer", "1.0")]
    public class UnityGameExplorer : BaseUnityPlugin
    {

        ExplorerView explorerView;
        List<IMainView> mainViews = new List<IMainView>();

        AssemblyClass g_Assembly;
        SortedDictionary<string, Type> g_ClassName2Type;
        public ConfigEntry<string> g_DllPathConfig;
        public static SortedList<string, CsharpClass> classListDetails;

        void Start()
        {
            InitView();
            new Thread(Init).Start();
        }

        void InitView()
        {          
            mainViews.Add(new ValueInputWindow());
            //Add Explorer View
            explorerView = new ExplorerView();
            mainViews.Add(explorerView);

            //Add Instance View
            ArrayList views = new ArrayList();
            views.Add(new InstanceFieldView());
            views.Add(new InstancePropView());
            views.Add(new InstanceMethodView());

            mainViews.Add(new InstanceView(views));
        }

        void Init()
        {
            classListDetails = new SortedList<string, CsharpClass>();
            g_DllPathConfig = Config.Bind<string>("Explorer", "DllPath", "DSPGAME_Data\\Managed\\Assembly-CSharp.dll", "Assembly Path");

            try
            {
                g_Assembly = new AssemblyClass(g_DllPathConfig.Value);
                g_ClassName2Type = g_Assembly.getTypeDict();

                explorerView.cluster(g_ClassName2Type);

                foreach (var cls in g_ClassName2Type)
                {
                    classListDetails.Add(cls.Key, new CsharpClass(cls.Value));
                }
            }
            catch(Exception exp)
            {
                Console.WriteLine("Error: " + exp.Message);
            }
        }

        void Update()
        {
            foreach (IMainView view in mainViews)
                view.Update();
        }

        void OnGUI()
        {
            GUI.backgroundColor = Color.black;

            foreach (IMainView view in mainViews)
                view.OnGUI();
        }
    }
}

