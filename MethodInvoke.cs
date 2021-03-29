using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ExplorerSpace
{
    public class Parser
    {
        public abstract class IParser
        {
            public abstract bool Parse(string inStr, out object outVal);
            public abstract Type GetType();
        }

        public class INT : IParser
        {
            public override Type GetType()
            {
                return typeof(int);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                int value;
                if (int.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class FLOAT : IParser
        {
            public override Type GetType()
            {
                return typeof(float);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                float value;
                if (float.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class BYTE : IParser
        {
            public override Type GetType()
            {
                return typeof(byte);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                byte value;
                if (byte.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class DOUBLE : IParser
        {
            public override Type GetType()
            {
                return typeof(double);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                double value;
                if (double.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class ULONG : IParser
        {
            public override Type GetType()
            {
                return typeof(ulong);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                ulong value;
                if (ulong.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class UINT : IParser
        {
            public override Type GetType()
            {
                return typeof(uint);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                uint value;
                if (uint.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class BOOL : IParser
        {
            public override Type GetType()
            {
                return typeof(bool);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                bool value;
                if (bool.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class BOOLEAN : IParser
        {
            public override Type GetType()
            {
                return typeof(Boolean);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                Boolean value;
                if (Boolean.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class SHORT : IParser
        {
            public override Type GetType()
            {
                return typeof(int);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                short value;
                if (short.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class LONG : IParser
        {
            public override Type GetType()
            {
                return typeof(long);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                long value;
                if (long.TryParse(inStr, out value))
                {
                    outVal = value;
                    return true;
                }
                outVal = null;
                return false;
            }
        }

        public class STRING : IParser
        {
            public override Type GetType()
            {
                return typeof(string);
            }
            public override bool Parse(string inStr, out object outVal)
            {
                outVal = inStr;
                return true;
            }
        }
    }

    public class ParserFactory
    {
        List<Parser.IParser> parsers;

        public ParserFactory()
        {
            parsers = new List<Parser.IParser>();
            parsers.Add(new Parser.INT());
            parsers.Add(new Parser.FLOAT());
            parsers.Add(new Parser.BYTE());
            parsers.Add(new Parser.DOUBLE());
            parsers.Add(new Parser.ULONG());
            parsers.Add(new Parser.UINT());
            parsers.Add(new Parser.BOOL());
            parsers.Add(new Parser.BOOLEAN());
            parsers.Add(new Parser.SHORT());
            parsers.Add(new Parser.LONG());
            parsers.Add(new Parser.STRING());
        }

        public Parser.IParser GetParser(Type type)
        {
            foreach (Parser.IParser parser in parsers)
            {
                if (parser.GetType() == type)
                {
                    return parser;
                }
            }

            return null;
        }
    }

    class MethodInvoke
    {
        MethodInfo method;
        object target;
        object[] parameters;
        ParserFactory parserFactory = new ParserFactory();

        MethodInvoke(MethodInfo method, object target)
        {
            this.method = method;
            this.target = target;
        }

        public bool Invoke(out object res,params string[] strParams)
        {
            res = null;
            try
            {
                object[] parameters = new object[strParams.Length];
                for(int i = 0; i< strParams.Length; ++i)
                {
                    //GetParser
                    Parser.IParser parser = parserFactory.GetParser(method.GetParameters()[i].ParameterType);
                    if (parser == null)
                        return false;

                    //Parse
                    object outVal;
                    if (parser.Parse(strParams[i],out outVal))
                        parameters[i] = outVal;
                    else
                        return false;
                }

                res = method.Invoke(target, parameters);
                return true;
            }
            catch(Exception exp)
            {
                Console.WriteLine("Error: ", exp.Message);
                return false;
            }
        }

    }


}
