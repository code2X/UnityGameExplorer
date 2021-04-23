using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DotInsideNode
{
    [Serializable]
    public class IEnumItem : dnObject
    {
        string m_Description = "";   // Description of the object

        public int Value => ID;
        public bool CustomValue = false;
        public virtual string Description
        {
            get => m_Description;
            set => m_Description = value;
        }
    }

    [Serializable]
    abstract public class IEnumeration : IBluePrint
    {
        public virtual string AssetPath
        {
            get; set;
        }
        public abstract string Description
        {
            get;
            set;
        }
        public abstract List<IEnumItem> Enumerators
        {
            get;
        }
        public abstract void AddEnumItem();

        public virtual int EnumCount => Enumerators.Count;
        public override string NewBaseName => "NewUserDefinedEnum";
    }
}
