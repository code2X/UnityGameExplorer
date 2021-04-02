using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ExplorerSpace
{
    public class ValueSetter
    {
        static DefaultParserFactory parserFactory = new DefaultParserFactory();

        public static bool Parse(Type type,string InputText, out object outVal)
        {
            outVal = null;
            var parser = parserFactory.GetParser(type);
            if (parser == null)
                return false;

            if (parser.Parse(InputText, out outVal))
                return true;
            else
                return false;
        }
    }

    public class FieldValueSetter: ValueSetter
    {
        public static bool TrySetValue(FieldInfo fieldInfo, string inputText, object instance = null)
        {
            if (fieldInfo == null || fieldInfo.IsLiteral)
                return false;
            if (instance == null && fieldInfo.IsStatic == false)
                return false;

            object outVal;
            bool res = Parse(fieldInfo.FieldType, inputText, out outVal);
            if (res == true)
            {
                res = Caller.Try(() =>
                {
                    fieldInfo.SetValue(instance, outVal);
                });
            }
            return res;
        }

        public static bool TrySetValue(FieldInfo fieldInfo, object inputObj, object parent = null)
        {
            bool res = Caller.Try(() =>
            {
                fieldInfo.SetValue(parent, inputObj);
            });
            return res;
        }
    }

    public class PropertyValueSetter: ValueSetter
    {
        public static bool TrySetValue(PropertyInfo propertyInfo, string inputText, object instance = null)
        {
            if (propertyInfo == null || propertyInfo.CanWrite == false)
                return false;
            if (instance == null && 
                propertyInfo.GetAccessors().Length > 0 &&
                propertyInfo.GetAccessors()[0].IsStatic == false)
                return false;

            object outVal;
            bool res = Parse(propertyInfo.PropertyType, inputText, out outVal);
            if (res == true)
            {
                res = Caller.Try(() =>
                {
                    propertyInfo.SetValue(instance, outVal);
                });
            }
            return res;
        }

        public static bool TrySetValue(PropertyInfo propertyInfo, object inputObj, object parent = null)
        {
            if (propertyInfo.CanWrite == false)
                return false;

            bool res = Caller.Try(() =>
            {
                propertyInfo.SetValue(parent, inputObj);
            });
            return res;
        }
    }

    public class MethodInvoker
    {
        DefaultParserFactory parserFactory = new DefaultParserFactory();
        MethodInfo method;
        object target;    

        public MethodInvoker(MethodInfo method, object target)
        {
            this.method = method;
            this.target = target;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="res">1.Parse error return row num 2.Success return true 3.Invoke error return -1</param>
        /// <param name="strParams"></param>
        /// <returns></returns>
        public int Invoke(out object res,params string[] strParams)
        {
            res = null;
            if (this.method == null)
                return -1;
            if (this.method.IsStatic == false && this.target == null)
                return -1;
            try
            {
                object[] parameters = new object[strParams.Length];
                for(int i = 0; i< strParams.Length; ++i)
                {
                    //GetParser
                    Parser.IParser parser = parserFactory.GetParser(method.GetParameters()[i].ParameterType);
                    if (parser == null)
                        return i + 1;

                    //Parse
                    object outVal;
                    if (parser.Parse(strParams[i],out outVal))
                        parameters[i] = outVal;
                    else
                        return i + 1;
                }

                res = method.Invoke(target, parameters);
                return 0;
            }
            catch(Exception exp)
            {
                Logger.Error(exp);
                return -1;
            }
        }

    }


}
