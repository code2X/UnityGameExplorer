using ImGuiNET;

namespace DotInsideNode
{
    /*
    public abstract class ArrayVar<T> : ValueVar<T[]>
    {
        public ArrayVar()
        {
            ResetVar();
        }

        public override EContainer ContainerType
        {
            get => EContainer.Array;
        }

        protected void ResetVar()
        {
            m_Value = NewArray(0);
        }

        protected void AddArrayElement()
        {
            T[] newArray = NewArray(m_Value.Length + 1);
            m_Value.CopyTo(newArray, 0);
            m_Value = newArray;
        }

        protected void InsertItem(int index)
        {
            Assert.IsTrue(0 <= index && index < m_Value.Length);
            T[] newArray = NewArray(m_Value.Length + 1);

            //Copy head
            System.Array.ConstrainedCopy(m_Value, 0, newArray, 0, index);

            //Copy back
            int backlen = newArray.Length - (index + 1);
            System.Array.ConstrainedCopy(m_Value, index, newArray, index + 1, backlen);           

            m_Value = newArray;
        }

        protected void DeleteItem(int index)
        {
            Assert.IsTrue(0 <= index && index < m_Value.Length);
            if (m_Value.Length <= 1)
            {
                ResetVar();
                return;
            }
                
            T[] newArray = NewArray(m_Value.Length - 1);

            //Copy head
            System.Array.ConstrainedCopy(m_Value, 0, newArray, 0, index);

            //Copy back
            int backlen = newArray.Length - index;
            System.Array.ConstrainedCopy(m_Value, index + 1, newArray, index, backlen);

            m_Value = newArray;
        }

        protected void DuplicateItem(int index)
        {
            Assert.IsTrue(0 <= index && index < m_Value.Length);

            //Get duplicate item
            T[] array = m_Value;
            T itemVal = array[index];

            //Create new array
            T[] newArray = NewArray(m_Value.Length + 1);
            m_Value.CopyTo(newArray, 0);
            newArray[m_Value.Length] = itemVal;
            m_Value = newArray;
        }

        protected virtual T[] NewArray(int size)
        {
            T[] newArray = new T[size];
            return newArray;
        }

        protected void DrawAddItem()
        {
            if (ImGui.Button("Add Item##BoolArrayVar"))
                AddArrayElement();
        }

        protected void DrawRemoveAll()
        {
            if (ImGui.Button("Remove All##BoolArrayVar"))
                ResetVar();
        }

        protected bool DrawArrowMenu(int index)
        {
            bool block = false;
            if (ImGui.BeginCombo("##ArrayVarArrowButton" + index.ToString(), "", ImGuiComboFlags.NoPreview))
            {
                if (ImGui.Selectable("Insert"))
                {
                    InsertItem(index);
                    block = true;
                }
                else if (ImGui.Selectable("Delete"))
                {
                    DeleteItem(index);
                    block = true;
                }
                else if (ImGui.Selectable("Duplicate"))
                {
                    DuplicateItem(index);
                    block = true;
                }
                ImGui.EndCombo();
            }
            return block;
        }

        protected override void DrawValueEditor()
        {
            T[] TArray = m_Value;
            int len = TArray.Length;

            DrawAddItem();
            ImGui.SameLine();
            DrawRemoveAll();

            ImGuiUtils.TableView("ArrayVar", () =>
            {
                for (int i = 0; i < len; ++i)
                {
                    ImGui.TableNextColumn();
                    ImGui.Text(i.ToString());

                    ImGui.TableNextColumn();
                    DrawItem(i);

                    ImGui.SameLine();
                    if (DrawArrowMenu(i))
                        break;
                    if (i > 500)
                        break;
                }

            }, Name, len.ToString());
        }

        protected abstract void DrawItem(int index);
    }
    
    [DotPrintVar]
    public class BoolArrayVar : ArrayVar<bool>
    {
        protected override void DrawItem(int index)
        {
            string lable = "##BoolArrayVarCheckbox" + index.ToString();

            ImGui.Checkbox(lable, ref m_Value[index]);
        }
    }

    [DotPrintVar]
    public class IntArrayVar : ArrayVar<int>
    {
        int[] m_Range = new int[2];

        protected override void DrawItem(int index)
        {
            string lable = "##IntArrayVarInputInt" + index.ToString();

            ImGui.InputInt(lable, ref m_Value[index]);
        }
    }

    [DotPrintVar]
    public class FloatArrayVar : ArrayVar<float>
    {
        protected override void DrawItem(int index)
        {
            string lable = "##FloatArrayVarInputFloat" + index.ToString();

            ImGui.InputFloat(lable, ref m_Value[index], 0.1f);
        }
    }

    [DotPrintVar]
    public class DoubleArrayVar : ArrayVar<double>
    {
        protected override void DrawItem(int index)
        {
            string lable = "##DoubleArrayVarInputDouble" + index.ToString();

            ImGui.InputDouble(lable, ref m_Value[index], 0.1f);
        }
    }

    [DotPrintVar]
    public class StringArrayVar : ArrayVar<string>
    {
        public StringArrayVar()
        {
            //m_Value = "";
        }

        protected override string[] NewArray(int index)
        {
            string[] newArray = base.NewArray(index);
            for(int i= 0; i < newArray.Length; ++i)
            {
                newArray[i] = "";
            }
            return newArray;
        }
 
        protected override void DrawItem(int index)
        {
            string lable = "##StringArrayVarInputText" + index.ToString();

            ImGui.InputText(lable, ref m_Value[index], 30);
        }
    }

    [DotPrintVar]
    public class VectorArrayVar : ArrayVar<Vector3>
    {
        protected override void DrawItem(int index)
        {
            string lable = "##VectorArrayVarInputFloat3" + index.ToString();

            ImGui.InputFloat3(lable, ref m_Value[index]);
        }
    }
    */

}