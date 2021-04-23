using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DotInsideLib.BluePrints.MainViews
{
    class DirView
    {
        string m_Dir = "Content";

        public DirView()
        {
            if (Directory.Exists("Content") == false)
            {
                Directory.CreateDirectory("Content");
            }

            ListFiles("Content");
        }

        public void ListFiles(string pathname)
        {
            // 所有目录与文件
            string[] subDirs = Directory.GetDirectories(pathname);
            string[] subFiles = Directory.GetFiles(pathname);

            foreach (string subDir in subDirs)
            {
                string dirname = Path.GetFileName(subDir);
                System.Console.WriteLine(dirname);
            }
        }

        public enum EItemEvent
        {
            Clicked,
        }
        public delegate void EItemAction(EItemEvent eItemEvent, string dirPath);
        public event EItemAction OnItemEvent;

        public void CheckCliked(string dirPath)
        {
            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(0))
            {
                OnItemEvent?.Invoke(EItemEvent.Clicked, dirPath);
            }
        }

        public void Draw()
        {
            if (ImGui.TreeNodeEx(m_Dir + "##" + m_Dir, treeNodeFlags | ImGuiTreeNodeFlags.DefaultOpen))
            {
                CheckCliked("Content");
                DrawDir(Directory.GetDirectories(m_Dir));
                ImGui.TreePop();
            }
            CheckCliked("Content");
        }
        ImGuiTreeNodeFlags treeNodeFlags = ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.OpenOnArrow;
        public void DrawDir(string[] dirSubDirs)
        {
            foreach (string subDir in dirSubDirs)
            {
                string dirname = Path.GetFileName(subDir);
                Assert.IsNotNull(dirname);
                string[] subItemDirs = Directory.GetDirectories(subDir);

                if (Directory.GetDirectories(subDir).Length == 0)
                {
                    if (ImGui.Selectable(dirname + "##" + subDir))
                    {
                        OnItemEvent?.Invoke(EItemEvent.Clicked, subDir);
                    }
                }
                else if (ImGui.TreeNodeEx(dirname + "##" + subDir, treeNodeFlags))
                {
                    CheckCliked(subDir);
                    DrawDir(subItemDirs);
                    ImGui.TreePop();
                }
                CheckCliked(subDir);
            }
        }

    }
}
