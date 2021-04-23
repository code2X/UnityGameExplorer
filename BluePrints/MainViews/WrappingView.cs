using DotInsideLib;
using ImGuiNET;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;

namespace DotInsideNode
{
    class WrappingView
    {
        public void Draw(string dirPath)
        {
            string[] subFiles = Directory.GetFiles(dirPath);

            foreach(string filePath in subFiles)
            {
                string filename = Path.GetFileName(filePath);
                if (ImGui.Selectable(filename))
                {
                }
                if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(0))
                {
                    FileStream readstream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    BinaryFormatter reader = new BinaryFormatter();
                    Enumeration readdata = (Enumeration)reader.Deserialize(readstream);                 
                    readdata.Name = filename;
                    readdata.AssetPath = filePath;

                    BluePrintWindow.Instance.Submit(readdata);
                    Logger.Info("Double Clicked");
                }
            }

        }
    }




}
