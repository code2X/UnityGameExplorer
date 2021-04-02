using System;
using System.Collections.Generic;
using ImGuiNET;
using System.Runtime.InteropServices;

namespace ExplorerSpace
{
    class GIRerSurface
    {
        public delegate void RenderCallback();
        public const string GIRerPath = "GIRerSurface.dll";

        [DllImport(GIRerPath, EntryPoint = "AddRenderCallback")]
        public static extern void AddRenderCallback(RenderCallback callback);
    }
}
