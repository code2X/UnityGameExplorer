
namespace DotInsideNode
{
    public abstract class IStructure: IBluePrint
    {
        public abstract string Tooltip
        {
            get;
            set;
        }

        public abstract int MemberCount
        {
            get;
        }

        public override string NewBaseName => "NewUserDefinedStruct";
    }
}
