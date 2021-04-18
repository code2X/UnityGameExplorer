using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotInsideNode
{
    class NodeTooltips
    {
        static Dictionary<Type, string> nodeTooltips = new Dictionary<Type, string>
        {
        //StellarAutoNavigation
            { typeof(SubStractionNode),"SubStraction (Left - Right)" },
            { typeof(AdditionNode),"Addition (Left + Right)" },
            { typeof(MutiplyNode),"Mutiplication (Left × Right)" },
            { typeof(DivisionNode),"Division (Left / Right)" },
            { typeof(ModuloNode),"Modulo (Left % Right)" },
            { typeof(FlipFlopNode),"Alternates between A and B outputs, starting with A"},
            { typeof(ORNode),"Returns the logical OR of two values(Left OR Right)" },
            { typeof(NOTNode),"Returns the logical complement of value(NOT In)" },
            { typeof(ANDNode),"Returns the logical AND of two values(Left AND Right)" },
            { typeof(SequenceNode),"Executes a series of pins in order" },
            { typeof(WhileLoopNode),"Repeatedly executes LoopBody as long as Condition it true" },
            { typeof(BranchNode),"Branch Statement\nIf Condition is true,execution goes to True,otherwise it goes to False" },
        };

        public static string GetTooltip(Type type)
        {
            string res;
            if(nodeTooltips.TryGetValue(type, out res))
            {
                return res;
            }
            return string.Empty;           
        }

    }
}
