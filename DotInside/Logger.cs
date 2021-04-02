using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplorerSpace
{
    class Logger
    {
        public static void Error(params string[] error)
        {
            foreach(string err in error)
            {
                Console.WriteLine("Error: " + err);
            }
        }

        public static void Info(params string[] infos)
        {
            foreach (string info in infos)
            {
                Console.WriteLine("Info: " + info);
            }
        }

        public static void Error(Exception exp)
        {
            Console.WriteLine("Error: " + exp.Message);

            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);     //0为方法本身，1为调用的方法 
            Console.WriteLine(" File: {0}", sf.GetFileName());               
            Console.WriteLine(" Method: {0}", sf.GetMethod().Name);          
            Console.WriteLine(" Line Number: {0}", sf.GetFileLineNumber());  
            Console.WriteLine(" Column Number: {0}", sf.GetFileColumnNumber());
        }
    }
}
