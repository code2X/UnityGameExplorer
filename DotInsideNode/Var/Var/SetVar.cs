using ImGuiNET;
using System.Collections.Generic;

namespace DotInsideNode
{
    /*
    public abstract class SetVar<T> : ValueVar<HashSet<T>>
    {
        public SetVar()
        {
            ResetVar();
        }

        public override TContainer ContainerType
        {
            get => TContainer.Set;
        }

        public override System.Type VarBaseType
        {
            get => typeof(T);
        }

        protected void ResetVar()
        {
            m_Value = new HashSet<T>();
        }

        protected void DrawAddItem()
        {
            if (ImGui.Button("Add Item##BoolArrayVar"))
                m_Value.Add(NewItem());
        }

        protected void DrawRemoveAll()
        {
            if (ImGui.Button("Remove All##BoolArrayVar"))
                ResetVar();
        }

        protected void DeleteItem(T item)
        {
            m_Value.Remove(item);
        }

        protected bool DrawArrowMenu(T item)
        {
            bool block = false;
            if (ImGui.BeginCombo("##SetVarArrowButton" + item.ToString(), "", ImGuiComboFlags.NoPreview))
            {
                if (ImGui.Selectable("Delete"))
                {
                    DeleteItem(item);
                    block = true;
                }
                ImGui.EndCombo();
            }
            return block;
        }

        protected override void DrawValueEditor()
        {
            HashSet<T> TSet = m_Value;
            int len = TSet.Count;
            int index = 0;

            DrawAddItem();
            ImGui.SameLine();
            DrawRemoveAll();

            ImGuiUtils.TableView("SetVar", () =>
            {
                foreach(T item in TSet)
                {
                    ImGui.TableNextColumn();
                    ImGui.Text(index.ToString());

                    ImGui.TableNextColumn();
                    if (DrawSetItem(item))
                        break;
                    ImGui.SameLine();
                    if (DrawArrowMenu(item))
                        break;                    
                    if (index > 500)
                        break;

                    ++index;
                }

            }, Name, len.ToString() + " Set elements");
        }

        public virtual bool DrawSetItem(T item)
        {
            T copy = item;
            bool isEqual = DrawItem(ref copy, item);
            if (isEqual == false && m_Value.Contains(copy) == false)
            {
                m_Value.Remove(item);
                m_Value.Add(copy);
                return true;
            }

            return false;
        }

        protected abstract T NewItem();
        protected abstract bool DrawItem(ref T copy,T item);
    }

    [DotPrintVar]
    public class IntSetVar : SetVar<int>
    {
        protected override bool DrawItem(ref int copy, int item)
        {
            string lable = "##IntSetVarInputInt" + item;
            ImGui.InputInt(lable, ref copy);
            return copy == item;
        }

        protected override int NewItem()
        {
            return 0;
        }
    }

    [DotPrintVar]
    public class FloatSetVar : SetVar<float>
    {
        protected override bool DrawItem(ref float copy, float item)
        {
            string lable = "##FloatArrayVarInputFloat" + item;

            ImGui.InputFloat(lable, ref copy, 0.1f);
            return copy == item;
        }

        protected override float NewItem()
        {
            return 0.0f;
        }
    }

    [DotPrintVar]
    public class DoubleSetVar : SetVar<double>
    {
        protected override bool DrawItem(ref double copy, double item)
        {
            string lable = "##DoubleArrayVarInputDouble" + item;

            ImGui.InputDouble(lable, ref copy, 0.1f);
            return copy == item;
        }

        protected override double NewItem()
        {
            return 0.0;
        }
    }

    [DotPrintVar]
    public class StringSetVar : SetVar<string>
    {
        protected override bool DrawItem(ref string copy, string item)
        {
            string lable = "##StringArrayVarInputText" + item;

            ImGui.InputText(lable, ref copy, 30);
            return copy == item;
        }

        protected override string NewItem()
        {
            return "";
        }
    }

    [DotPrintVar]
    public class VectorSetVar : SetVar<Vector3>
    {
        protected override bool DrawItem(ref Vector3 copy, Vector3 item)
        {
            string lable = "##VectorArrayVarInputFloat3" + item.ToString();

            ImGui.InputFloat3(lable, ref item);
            return copy.Equals(item);
        }

        protected override Vector3 NewItem()
        {
            return new Vector3();
        }
    }
    */
}
