using ImGuiNET;
using System.Reflection;
using System;

namespace DotInsideLib
{
    public class MethodInvokeWindow : IParamInputModalView
    {
        public override string GetPopupName() => "Method Invoke";

        //method
        ParameterInfo[] methodParameters;
        object methodParentObj;
        MethodInfo methodInfo;

        //error
        bool invokeErrored = false;
        int errorRow = -1;

        string[] inputText;

        private MethodInvokeWindow() { Reset(); }
        static MethodInvokeWindow instance = new MethodInvokeWindow();
        public static MethodInvokeWindow GetInstance() => instance;

        void Reset()
        {
            this.methodInfo = null;
            this.methodParameters = null;
            this.methodParentObj = null;
            this.inputText = null;

            this.invokeErrored = false;
            this.errorRow = -1;
        }

        void ResetInputText(ParameterInfo[] parameters)
        {
            inputText = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                inputText[i] = "";
            }
        }

        public void Show(MethodInfo method, ParameterInfo[] parameters, object parentObj = null)
        {
            Reset();
            ResetInputText(parameters);
            this.methodInfo = method;
            this.methodParameters = parameters;
            this.methodParentObj = parentObj;
            ShowWindow();

        }

        public override void DrawPopupContent()
        {
            DrawTable();

            if (ImGui.Button("Call"))
            {
                CallMethod();
            }
        }

        void DrawTable()
        {
            ImGuiEx.TableView("MethodInvokeTable", () =>
            {
                for (int i = 0; i < inputText.Length; ++i)
                {
                    ImGui.TableNextRow();
                    if (errorRow == i)
                        paramTable.DrawRow(methodParameters[i].ParameterType, methodParameters[i].Name, ref inputText[i], true);
                    else
                        paramTable.DrawRow(methodParameters[i].ParameterType, methodParameters[i].Name, ref inputText[i]);
                }
            }, "Type", "Name", "Value", "Error");
        }

        void CallMethod()
        {
            MethodInvoker invoke = new MethodInvoker(methodInfo, methodParentObj);
            object outObj;
            int res = invoke.Invoke(out outObj, inputText);
            if (res == 0)
            {
                InvokeSuccess(outObj);
            }
            else if (res == -1)
            {
                InvokeError();
            }
            else
            {
                InputError(res - 1);
            }
        }

        void InvokeSuccess(object outObj)
        {
            CloseWindow();
        }

        void InvokeError()
        {
            invokeErrored = true;

            ImGui.SameLine();
            ImGui.Text("Invoke Error");
        }

        void InputError(int line)
        {
            errorRow = line;
        }
    }
}
