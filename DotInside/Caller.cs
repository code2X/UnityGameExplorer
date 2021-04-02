using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplorerSpace
{
    class Caller
    {
        public delegate void VoidFunc();

        public static bool Try(VoidFunc voidFunc)
        {
            try
            {
                voidFunc();
                return true;
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
                return false;
            }
        }
    }
}
