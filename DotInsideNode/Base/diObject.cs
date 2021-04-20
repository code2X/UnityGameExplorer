namespace DotInsideNode
{
    public class diObject
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
    }
}
