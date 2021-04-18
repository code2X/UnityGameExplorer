
namespace DotInsideNode
{

    public abstract class OperationStrategy
    {
        //Request
        protected class OperationNotImplemented : System.Exception
        {
        }

        public abstract object DoOperation(object left, object right);
    }

    public class OperationSubtract : OperationStrategy
    {
        public override object DoOperation(object left, object right)
        {
            if(left is int && right is int)
            {
                return (int)left - (int)right;
            }
            else if(left is float && right is float)
            {
                return (float)left - (float)right;
            }
            else if (left is double && right is double)
            {
                return (double)left - (double)right;
            }
            throw new OperationNotImplemented();
        }
    }

    public class OperationMultiply : OperationStrategy
    {
        public override object DoOperation(object left, object right)
        {
            if (left is int && right is int)
            {
                return (int)left * (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left * (float)right;
            }
            else if (left is double && right is double)
            {
                return (double)left * (double)right;
            }
            throw new OperationNotImplemented();
        }
    }

    public class OperationAddition : OperationStrategy
    {
        public override object DoOperation(object left, object right)
        {
            if (left is int && right is int)
            {
                return (int)left + (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left + (float)right;
            }
            else if (left is double && right is double)
            {
                return (double)left + (double)right;
            }
            else if (left is string && right is string)
            {
                return (string)left + (string)right;
            }
            throw new OperationNotImplemented();
        }
    }

    public class OperationDivision : OperationStrategy
    {
        public override object DoOperation(object left, object right)
        {
            if (left is int && right is int)
            {
                return (int)left / (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left / (float)right;
            }
            else if (left is double && right is double)
            {
                return (double)left / (double)right;
            }
            throw new OperationNotImplemented();
        }
    }

    public class OperationModulo : OperationStrategy
    {
        public override object DoOperation(object left, object right)
        {
            if (left is int && right is int)
            {
                return (int)left % (int)right;
            }
            else if (left is float && right is float)
            {
                return (float)left % (float)right;
            }
            else if (left is double && right is double)
            {
                return (double)left % (double)right;
            }
            throw new OperationNotImplemented();
        }
    }

    public class OperationContext
    {
        private OperationStrategy strategy;

        public OperationContext(OperationStrategy strategy)
        {
            this.strategy = strategy;
        }

        public object DoOperation(object left, object right)
        {
            return strategy.DoOperation(left, right);
        }
    }


}
