using ImGuiNET;
using System.Reflection;
using System;

namespace ExplorerSpace
{

    public abstract class IModalView : IView
    {
        bool showWindow = false;

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

        public virtual void ShowWindow() => showWindow = true;
        public virtual void CloseWindow() => showWindow = false;
        public virtual string GetPopupName() => "ModalWindow";
        public abstract void DrawPopupContent();
    }

    public abstract class IParamInputModalView : IModalView
    {
        public static ParamInputTable paramTable = new ParamInputTable();
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

            ImGui.BeginTable("ArrayInfo", 3);
            ImGuiUtils.TableSetupHeaders("Type", "Index", "Value");

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
                arrayDrawer.DrawArrayValue((Array)arrayInstance,i,index);

                ++index;
            }
            ImGui.EndTable();

        }
    }

    public class ParamInputTable
    {
        static System.Numerics.Vector4 errorColor = new System.Numerics.Vector4(0.4f, 0.1f, 0.1f, 0.65f);

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
            ShowWindow();

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
            ImGuiUtils.TableView("MethodInvokeTable", () =>
            {
                for (int i = 0; i < methodParameters.Length; ++i)
                {
                    ImGui.TableNextRow();
                    if (errorRow == i)
                        paramTable.DrawParamRow(methodParameters[i].ParameterType, methodParameters[i].Name, ref inputText[i], true);
                    else
                        paramTable.DrawParamRow(methodParameters[i].ParameterType, methodParameters[i].Name, ref inputText[i]);
                }
            }, "Type", "Name", "Value");
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
            ShowWindow();
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

    public abstract class ValueInputWindowBase : IParamInputModalView
    {
        public override string GetPopupName() => "Set Value";
        protected object parentObj = null;
        protected bool errored = false;
        protected string inputText = "";

        public void doSuccess()
        {
            CloseWindow();
        }

        public void DrawTable(string tableName,Type type,string name)
        {
            ImGuiUtils.TableView(tableName, () =>
            {
                ImGui.TableNextRow();
                paramTable.DrawParamRow(type, name, ref inputText, errored);
            }, "Type", "Name", "Value");
        }

        public void Reset()
        {
            CloseWindow();
            this.parentObj = null;
            this.errored = false;
            this.inputText = "";
        }
    }

    class FieldValueInputWindow: ValueInputWindowBase
    {
        FieldInfo varInfo;

        private FieldValueInputWindow() { Reset(); }
        static FieldValueInputWindow instance = new FieldValueInputWindow();
        public static FieldValueInputWindow GetInstance() => instance;

        public new void Reset()
        {
            base.Reset();
            this.varInfo = null;
        }

        public void Show(FieldInfo varInfo, object parentObj = null)
        {
            Reset();
            Caller.Try(() =>
            {
                this.inputText = varInfo.GetValue(parentObj).ToString();
            });
            ShowWindow();
            this.varInfo = varInfo;
            this.parentObj = parentObj;
        }

        public override void DrawPopupContent()
        {
            DrawTable("FieldValueInputTable", varInfo.FieldType, varInfo.Name);

            if (ImGui.Button("OK"))
            {
                errored = !FieldValueSetter.TrySetValue(varInfo, inputText, parentObj);
                if (errored == false)
                {
                    doSuccess();
                }
            }
        }
    }

    class PropertyValueInputWindow : ValueInputWindowBase
    {
        PropertyInfo propertyInfo;

        private PropertyValueInputWindow() { Reset(); }
        static PropertyValueInputWindow instance = new PropertyValueInputWindow();
        public static PropertyValueInputWindow GetInstance() => instance;

        public new void Reset()
        {
            base.Reset();
            this.propertyInfo = null;
        }

        public void Show(PropertyInfo propertyInfo, object parentObj = null)
        {
            Reset();
            Caller.Try(() =>
            {
                this.inputText = propertyInfo.GetValue(parentObj).ToString();
            });
            ShowWindow();
            this.propertyInfo = propertyInfo;
            this.parentObj = parentObj;
        }

        public override void DrawPopupContent()
        {
            DrawTable("propertyValueInputTable", propertyInfo.PropertyType, propertyInfo.Name);

            if (ImGui.Button("OK"))
            {
                errored = !PropertyValueSetter.TrySetValue(propertyInfo, inputText, parentObj);
                if (errored == false)
                {
                    doSuccess();
                }
            }
        }

    }

    public class ArrayElementInputWindow : ValueInputWindowBase
    {
        public override string GetPopupName() => "Array Element Value";

        //method
        Array arrayObj;
        object elementObj;
        int elementIndex;

        private ArrayElementInputWindow() { Reset(); }
        static ArrayElementInputWindow instance = new ArrayElementInputWindow();
        public static ArrayElementInputWindow GetInstance() => instance;

        public new void Reset()
        {
            base.Reset();
            this.elementObj = null;
            this.elementIndex = 0;
        }

        public void Show(Array array, object elementObj, int elementIndex)
        {
            Reset();
            this.arrayObj = array;
            this.elementObj = elementObj;
            this.elementIndex = elementIndex;
            Caller.Try(() =>
            {
                this.inputText = elementObj.ToString();
            });
            ShowWindow();
        }

        public override void DrawPopupContent()
        {
            DrawTable("ArrayElementInputTable", elementObj.GetType(), elementIndex.ToString());

            if (ImGui.Button("OK"))
            {
                bool res = ArrayElementSetter.TrySetValue(arrayObj, elementObj.GetType(), elementIndex, inputText);
                if (res)
                {
                    errored = false;
                    CloseWindow();
                }
                else
                {
                    errored = true;
                }
            }
        }
    }

}
