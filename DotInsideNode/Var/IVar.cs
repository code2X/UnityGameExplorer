using System;
using System.Collections.Generic;

namespace DotInsideNode
{
    public abstract class diObject
    {
        protected int m_ID = -1;
        protected string m_Name = "";

        public virtual int ID
        {
            get => m_ID;
            set => m_ID = value;
        }

        public virtual string Name
        {
            get => m_Name;
            set => m_Name = value;
        }
    }

    public abstract class IVar: diObject
    {
        //protected static ImGuiTableFlags TableFlags = ImGuiTableFlags.Borders | ImGuiTableFlags.Resizable;
        protected Type m_Type = typeof(bool);
        protected bool m_InstanceEditable = false;
        protected bool m_BluePrintReadOnly = false;
        protected string m_Tooltip = string.Empty;
        protected bool m_ExposeOnSpawn = false;
        protected bool m_Private = false;
        protected bool m_ExposeToCinematics = false;

        protected string m_EditName = string.Empty;

        public virtual bool BluePrintReadOnly
        {
            get => m_BluePrintReadOnly;
        }
        public virtual string Tooltip
        {
            get => m_Tooltip;
        }
        public override string Name
        {
            get => m_Name;
            set
            {
                m_Name = value;
                m_EditName = m_Name;
            }
        }
        public virtual Type VarType
        {
            get => m_Type;
        }
        public virtual Type VarBaseType
        {
            get => m_Type;
        }
        public abstract object VarValue
        {
            get;
            set;
        }
        public abstract List<VarSetter> Setters
        {
            get;
            protected set;
        }
        public abstract List<VarGetter> Getters
        {
            get;
            protected set;
        }

        public virtual VarBase BaseCopy(VarBase item)
        {
            var constructor = this.GetType().GetConstructor(System.Type.EmptyTypes);
            VarBase copy = (VarBase)constructor.Invoke(null);

            copy.m_ID = item.m_ID;
            copy.m_Name = item.m_Name;
            copy.m_EditName = item.m_EditName;
            copy.m_Type = item.m_Type;
            copy.m_InstanceEditable = item.m_InstanceEditable;
            copy.m_BluePrintReadOnly = item.m_BluePrintReadOnly;
            copy.m_Tooltip = item.m_Tooltip;
            copy.m_ExposeOnSpawn = item.m_ExposeOnSpawn;
            copy.m_Private = item.m_Private;
            copy.m_ExposeToCinematics = item.m_ExposeToCinematics;

            copy.Setters = item.Setters;
            copy.Getters = item.Getters;

            return copy;
        }
    }

}
