using System.Collections.Generic;
using System.Diagnostics;

namespace ExplorerSpace
{
    class ProcessTools
    {
        public static List<ProcessModule> GetProcessModule()
        {
            List<ProcessModule> processModule = new List<ProcessModule>();
            Process process = Process.GetCurrentProcess();

            for (int i = 0; i < process.Modules.Count; ++i)
            {
                processModule.Add(process.Modules[i]);
            }

            return processModule;
        }

        public static string GetDllPath(List<ProcessModule> processModules, string dllName)
        {
            foreach (ProcessModule i in processModules)
            {
                if (i.ModuleName == dllName)
                {
                    return i.FileName;
                }
            }

            return string.Empty;
        }
    }
}
