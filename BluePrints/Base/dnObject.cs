using System;
using System.Xml;
using System.Xml.Schema;

namespace DotInsideNode
{
    [Serializable]
    public class dnObject
    {      
        int m_ID = -1;        // Identification of the object
        string m_Name = "";   // Name of the object

        public delegate void NameEvent(string name);
        public event NameEvent OnSetName;

        public virtual int ID
        {
            get => m_ID;
            set => m_ID = value;
        }

        public virtual string Name
        {
            get => m_Name;
            set
            {
                OnSetName?.Invoke(value);
                m_Name = value;
            }
        }

        #region XmlSerializable
        public virtual XmlSchema GetSchema() => null;
        public virtual void ReadXml(XmlReader reader)
        {
            m_ID = reader.ReadElementContentAsInt();
            m_Name = reader.ReadElementContentAsString();
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("ID",ID.ToString());
            writer.WriteElementString("Name", m_Name);
        }
        #endregion

    }
}
