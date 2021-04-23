using ImGuiNET;
using System.Reflection;
using System;

namespace DotInsideLib
{
    public class ArrayInfoWindow : IWindowView
    {
        public override string WindowName => "Array Element";
        IArrayDrawer arrayDrawer = new DefaultArrayDrawer();

        object arrayInstance;
        string arrayName;

        private ArrayInfoWindow() { Reset(); }
        static ArrayInfoWindow instance = new ArrayInfoWindow();
        public static ArrayInfoWindow GetInstance() => instance;

        public void Reset()
        {
            this.showWindow = false;
            this.arrayInstance = null;
        }

        public void Show(object instance = null, string name = "")
        {
            this.showWindow = false;
            this.arrayInstance = instance;
            this.arrayName = name;
            this.showWindow = true;
        }

        public override void DrawWindowContent()
        {
            if (instance == null)
                return;

            ImGui.Text(arrayInstance.GetType().ToString() + " " + arrayName);

            var array = (Array)arrayInstance;
            ImGui.Text("Length:" + array.Length);

            ImGuiEx.TableView("ArrayInfo", () =>{
                int index = 0;
                foreach (var i in (Array)arrayInstance)
                {
                    if (i == null)
                        continue;

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    arrayDrawer.DrawType(i.GetType());

                    ImGui.TableSetColumnIndex(1);
                    arrayDrawer.DrawArrayIndex(index);

                    ImGui.TableSetColumnIndex(2);
                    arrayDrawer.DrawArrayValue((Array)arrayInstance, i, index);

                    ++index;

                    //Array too large
                    if (index > 100)
                        break;
                }
            }, "Type", "Index", "Value");


        }
    }
}
