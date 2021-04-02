using ImGuiNET;
using System.Reflection;
using System;

namespace ExplorerSpace
{
    public abstract class IWindowView: IView
    {
        protected bool showWindow = false;

        public override void OnGUI()
        {
            if (showWindow)
            {
                ImGui.Begin(GetWindowName(), ref showWindow);
                DrawWindowContent();
                ImGui.End();
            }
        }

        public virtual string GetWindowName() => "ModalWindow";
        public abstract void DrawWindowContent();
    }

    public abstract class IModalView : IView
    {
        protected bool showWindow = false;

        public override void OnGUI()
        {
            if (showWindow)
            {
                if (!ImGui.IsPopupOpen(GetPopupName()))
                    ImGui.OpenPopup(GetPopupName());
                if (ImGui.BeginPopupModal(GetPopupName(), ref showWindow))
                {
                    DrawPopupContent();
                    ImGui.EndPopup();
                }
            }
        }

        public virtual string GetPopupName() => "ModalWindow";
        public abstract void DrawPopupContent();
    }

    public abstract class IParamInputModalView : IModalView
    {
        public static ParamInputTable paramTable = new ParamInputTable();
    }

    public abstract class IValueInputWindow : IParamInputModalView
    {
        public override string GetPopupName() => "Set Value";
    }

    public class ArrayInfoWindow : IWindowView
    {
        public override string GetWindowName() => "Array Element";
        static IArrayDrawer arrayDrawer = new DefaultArrayDrawer();

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
            this.showWindow = true;
            this.arrayInstance = instance;
            this.arrayName = name;
        }

        public override void DrawWindowContent()
        {
            ImGui.Text(arrayInstance.GetType().ToString() + " " + arrayName);
            ImGui.BeginTable("ArrayInfo", 3);
            ImGuiUtils.TableSetupHeaders("Type", "Index", "Value");

            int index = 0;
            foreach (var i in (Array)arrayInstance)
            {
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                arrayDrawer.DrawType(i.GetType());

                ImGui.TableSetColumnIndex(1);
                arrayDrawer.DrawArrayIndex(index);

                ImGui.TableSetColumnIndex(2);
                arrayDrawer.DrawArrayValue((Array)arrayInstance,i,index);

                ++index;
            }
            ImGui.EndTable();

        }
    }

    public class ArrayElementInputWindow : IParamInputModalView
    {
        public override string GetPopupName() => "Array Element Value";

        //method
        Array arrayObj;
        object elementObj;
        int elementIndex;

        //error
        bool inputErrored = false;

        string inputText;

        private ArrayElementInputWindow() { Reset(); }
        static ArrayElementInputWindow instance = new ArrayElementInputWindow();
        public static ArrayElementInputWindow GetInstance() => instance;

        void Reset()
        {
            this.elementObj = null;
            this.elementIndex = 0;
            this.inputText = string.Empty;
            this.inputErrored = false;
        }

        public void Show(Array array, object elementObj, int elementIndex)
        {
            Reset();
            this.arrayObj = array;
            this.elementObj = elementObj;
            this.elementIndex = elementIndex;
            this.inputText = elementObj.ToString();
            this.showWindow = true;
        }

        public override void DrawPopupContent()
        {
            DrawTable();

            if (ImGui.Button("OK"))
            {
                bool res = SetArrayElementValue(arrayObj, elementObj.GetType(), elementIndex, inputText);
                if(res)
                {
                    inputErrored = false;
                    showWindow = false;
                }
                else
                {
                    inputErrored = true;
                }
            }
        }

        public bool SetArrayElementValue(Array array, Type type, int index, string text)
        {
            if (array == null) 
                return false;
            if (index >= array.Length || index < 0) 
                return false;

            object parsedValue;
            if (ValueSetter.Parse(type, text, out parsedValue))
            {
                bool res = Caller.Try(() =>
                {
                    array.SetValue(parsedValue, index);
                });
                return res;
            }
            else
            {
                return false;
            }
        }

        void DrawTable()
        {
            ImGuiUtils.BeginTableWithHeaders("ArrayElementInputTable","Type", "Index", "Value");
            ImGui.TableNextRow();
            paramTable.DrawParamRow(elementObj.GetType(), elementIndex.ToString(), ref inputText, inputErrored);
            ImGui.EndTable();
        }
    }

    public class ParamInputTable
    {
        static System.Numerics.Vector4 errorColor = new System.Numerics.Vector4(0.4f, 0.1f, 0.1f, 0.65f);

        public void DrawHeader()
        {
            ImGui.TableSetupScrollFreeze(0, 1); // Make top row always visible
            ImGuiUtils.TableSetupColumn("Type", "Name", "Value");
            ImGui.TableHeadersRow();
        }

        public void DrawParamRow(Type type,string name, ref string inputText, bool error = false)
        {
            if (error)
                ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0 + 1, ImGui.GetColorU32(errorColor));
            ImGui.TableSetColumnIndex(0);
            ImGui.Text(type.Name);
            ImGui.TableSetColumnIndex(1);
            ImGui.Text(name);
            ImGui.TableSetColumnIndex(2);
            ImGui.InputText("", ref inputText, 20);
        }
    }

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
            inputText = new string[this.methodParameters.Length];
            for (int i = 0; i < inputText.Length; ++i)
            {
                inputText[i] = "";
            }
        }

        public void Show(MethodInfo method, ParameterInfo[] parameters, object parentObj = null)
        {
            Reset();
            this.methodInfo = method;
            this.methodParameters = parameters;
            this.methodParentObj = parentObj;
            this.showWindow = true;

            ResetInputText(parameters);
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
            ImGui.BeginTable("MethodInvokeTable", 3);
            paramTable.DrawHeader();
            for (int i = 0; i < methodParameters.Length; ++i)
            {
                ImGui.TableNextRow();
                if (errorRow == i)
                    paramTable.DrawParamRow(methodParameters[i].ParameterType,methodParameters[i].Name, ref inputText[i], true);
                else
                    paramTable.DrawParamRow(methodParameters[i].ParameterType, methodParameters[i].Name, ref inputText[i]);
            }
            ImGui.EndTable();
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
            showWindow = false;
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

    class FieldValueInputWindow: IValueInputWindow
    {
        FieldInfo varInfo;
        object parentObj = null;
        bool errored = false;
        string InputText = "";

        public void Reset()
        {
            this.showWindow = false;
            this.varInfo = null;
            this.parentObj = null;
            this.errored = false;
            this.InputText = "";
        }

        public void Show(FieldInfo varInfo, object parentObj = null)
        {
            try
            {
                this.InputText = varInfo.GetValue(parentObj).ToString();
            }
            catch (System.Exception exp)
            {
                this.InputText = "";
                Logger.Error(exp);
            }
            this.errored = false;
            this.showWindow = true;
            this.varInfo = varInfo;
            this.parentObj = parentObj;
        }

        public override void DrawPopupContent()
        {
            ImGui.BeginTable("ValueInputTable", 3);
            paramTable.DrawHeader();
            ImGui.TableNextRow();
            paramTable.DrawParamRow(varInfo.FieldType,varInfo.Name, ref InputText, errored);
            ImGui.EndTable();

            if (ImGui.Button("OK"))
            {
                errored = !FieldValueSetter.TrySetValue(varInfo, InputText, parentObj);
                if (errored == false)
                {
                    showWindow = false;
                }
            }
        }
    }

    class ValueInputWindow : IParamInputModalView
    {
        enum SetType
        {
            Null,
            Property,
            Field
        }

        public static string windowName = "Set Value";
        static ParamInputTable paramTable = new ParamInputTable();

        FieldInfo varInfo;
        object parentObj = null;
        bool errored = false;
        string InputText = "";
        SetType setType = SetType.Null;

        public void Reset()
        {
            this.showWindow = false;
            this.varInfo = null;
            this.parentObj = null;
            this.errored = false;
            this.InputText = "";
            this.setType = SetType.Null;
        }

        public void Show(PropertyInfo propInfo, object parentObj = null)
        {
            try
            {
                this.InputText = propInfo.GetValue(parentObj).ToString();
            }
            catch (System.Exception exp)
            {
                this.InputText = "";
                Logger.Error(exp);
            }
            this.errored = false;
            this.showWindow = true;
            this.parentObj = parentObj;
            this.setType = SetType.Property;
        }

        public void Show(FieldInfo varInfo, object parentObj = null)
        {
            try
            {
                this.InputText = varInfo.GetValue(parentObj).ToString();
            }
            catch(System.Exception exp)
            {
                this.InputText = "";
                Logger.Error(exp);
            }
            this.errored = false;         
            this.showWindow = true;
            this.varInfo = varInfo;
            this.parentObj = parentObj;
            this.setType = SetType.Field;
        }

        public override void DrawPopupContent()
        {
            ImGui.BeginTable("ValueInputTable", 3);
            paramTable.DrawHeader();
            ImGui.TableNextRow();
            paramTable.DrawParamRow(varInfo.FieldType, varInfo.Name, ref InputText, errored);
            ImGui.EndTable();

            if (ImGui.Button("OK"))
            {
                errored = !FieldValueSetter.TrySetValue(varInfo,InputText, parentObj);
                if(errored == false)
                {
                    showWindow = false;
                }
            }
        }


    }

}
