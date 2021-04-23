using ImGuiNET;

namespace DotInsideNode
{
    [AValueContainer]
    class ArrayContainer : ContainerBase
    {
        object[] m_Array = new object[0];

        public override object Value
        {
            get => m_Array;
            set => m_Array = (object[])value;
        }

        public override EContainer ContainerType => EContainer.Array;
        public override diType ValueType
        {
            get => m_ValueType;
            set
            {
                if (m_ValueType != value)
                {
                    m_ValueType = value;
                    m_Array = new object[0];
                }
            }
        }

        public override void DrawContainerValue(string lable)
        {
            DrawArrayEditor();
        }

        public override diContainer DuplicateContainer()
        {
            ArrayContainer res = new ArrayContainer();
            res.m_Array = (object[])DuplicateContainerValue();
            return res;
        }

        public override object DuplicateContainerValue()
        {
            if(m_Array == null)
                return new object[0];

            object[] res = new object[m_Array.Length];
            for (int i = 0; i < m_Array.Length; ++i)
            {
                res[i] = m_Array[i];
            }

            return res;
        }

        protected void ResetVar()
        {
            m_Array = NewArray(0);
        }

        protected void AddArrayElement()
        {
            object[] newArray = NewArray(m_Array.Length + 1);
            m_Array.CopyTo(newArray, 0);
            m_Array = newArray;
        }

        public void InsertItem(int index)
        {
            Assert.IsTrue(0 <= index && index < m_Array.Length);
            object[] newArray = NewArray(m_Array.Length + 1);

            //Copy head
            System.Array.ConstrainedCopy(m_Array, 0, newArray, 0, index);

            //Copy back
            int backlen = newArray.Length - (index + 1);
            System.Array.ConstrainedCopy(m_Array, index, newArray, index + 1, backlen);

            m_Array = newArray;
        }

        public void DeleteItem(int index)
        {
            Assert.IsTrue(0 <= index && index < m_Array.Length);
            if (m_Array.Length <= 1)
            {
                ResetVar();
                return;
            }

            object[] newArray = NewArray(m_Array.Length - 1);

            //Copy head
            System.Array.ConstrainedCopy(m_Array, 0, newArray, 0, index);

            //Copy back
            int backlen = newArray.Length - index;
            System.Array.ConstrainedCopy(m_Array, index + 1, newArray, index, backlen);

            m_Array = newArray;
        }

        public void DuplicateItem(int index)
        {
            Assert.IsTrue(0 <= index && index < m_Array.Length);

            //Get duplicate item
            object[] array = m_Array;
            object itemVal = array[index];

            //Create new array
            object[] newArray = NewArray(m_Array.Length + 1);
            m_Array.CopyTo(newArray, 0);
            newArray[m_Array.Length] = itemVal;
            m_Array = newArray;
        }

        public virtual object[] NewArray(int size)
        {
            object[] newArray = new object[size];
            for(int i =0; i< size;++i)
            {
                newArray[i] = NewValueTypeObject();
            }
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

        protected void DrawArrayEditor()
        {
            object[] TArray = m_Array;
            int len = TArray.Length;

            DrawAddItem();
            ImGui.SameLine();
            DrawRemoveAll();

            ImGuiEx.TableView("ArrayVar", () =>
            {
                for (int i = 0; i < len; ++i)
                {
                    ImGui.TableNextColumn();
                    ImGui.Text(i.ToString());

                    ImGui.TableNextColumn();
                    TArray[i] = m_ValueType.Draw(ref TArray[i],i.ToString());

                    ImGui.SameLine();
                    if (DrawArrowMenu(i))
                        break;
                    if (i > 500)
                        break;
                }

            }, "Array", len.ToString());
        }


    }

}
