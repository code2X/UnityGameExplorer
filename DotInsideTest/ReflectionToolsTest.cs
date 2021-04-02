using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace ExplorerSpace
{
    [TestClass]
    public class ValueSetterTest
    {
        struct CustomClass
        { }

        [TestMethod]
        public void TestParse()
        {
            object outVal;

            //normal
            Assert.IsTrue(ValueSetter.Parse(typeof(int), "14", out outVal));
            Assert.IsTrue((int)outVal == 14);

            //Error input
            Assert.IsFalse(ValueSetter.Parse(typeof(int), "14.4", out outVal));
            Assert.IsFalse(ValueSetter.Parse(typeof(int), "14.4f", out outVal));
            Assert.IsFalse(ValueSetter.Parse(typeof(int), "1.4.4", out outVal));

            //Error type
            Assert.IsFalse(ValueSetter.Parse(typeof(CustomClass), "14.4", out outVal));
        }
    }

    [TestClass]
    public class FieldValueSetterTest
    {
        public class TestClass
        {
            public static int staticIntVal = 4;
            public int IntVal = 4;
        }

        [TestMethod]
        public void TestTrySetValue()
        {
            TestClass instance = new TestClass();
            FieldInfo staticIntInfo = typeof(TestClass).GetField("staticIntVal", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo IntInfo = typeof(TestClass).GetField("IntVal", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            //static normal
            for (int i = 0; i < 1000; ++i)
            {
                Assert.IsTrue(FieldValueSetter.TrySetValue(staticIntInfo, i.ToString(), null));
                Assert.IsTrue(TestClass.staticIntVal == i);
            }

            //null field
            for (int i = 0; i < 1000; ++i)
            {
                Assert.IsFalse(FieldValueSetter.TrySetValue(null, i.ToString(), null));
            }
            
            //static input text error
            for (float i = 0; i < 1000; ++i)
            {
                Assert.IsFalse(FieldValueSetter.TrySetValue(staticIntInfo, "2.0", null));
            }

            //normal instance
            for (int i = 0; i < 1000; ++i)
            {
                Assert.IsTrue(FieldValueSetter.TrySetValue(IntInfo, i.ToString(), instance));
                Assert.IsTrue(instance.IntVal == i);
            }

            //instance input error
            for (int i = 0; i < 100; ++i)
            {
                Assert.IsFalse(FieldValueSetter.TrySetValue(IntInfo, "7.0", instance));
            }

            //instance is null
            for (int i = 0; i < 100; ++i)
            {
                Assert.IsFalse(FieldValueSetter.TrySetValue(IntInfo, "5", null));
            }
        }
    }

    [TestClass]
    public class PropertyValueSetterTest
    {
        public class TestClass
        {
            static int staticIntVal = 4;
            int IntVal = 4;

            public static int staticIntProperty { 
                get 
                {
                    return staticIntVal;
                }
                set 
                {
                    staticIntVal = value;
                }
            }
            public int IntProperty
            {
                get
                {
                    return IntVal;
                }
                set
                {
                    IntVal = value;
                }
            }
        }

        [TestMethod]
        public void TestTrySetValue()
        {
            TestClass instance = new TestClass();
            PropertyInfo staticIntInfo = typeof(TestClass).GetProperty("staticIntProperty", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            PropertyInfo IntPropertyInfo = typeof(TestClass).GetProperty("IntProperty", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            //static
            for (int i = 0; i < 1000; ++i)
            {
                Assert.IsTrue(PropertyValueSetter.TrySetValue(staticIntInfo, i.ToString(), null));
                Assert.IsTrue(TestClass.staticIntProperty == i);
            }

            //static with instance
            for (int i = 0; i < 1000; ++i)
            {
                Assert.IsTrue(PropertyValueSetter.TrySetValue(staticIntInfo, i.ToString(), instance));
                Assert.IsTrue(TestClass.staticIntProperty == i);
            }

            //static input text error
            for (float i = 0; i < 1000; ++i)
            {
                Assert.IsFalse(PropertyValueSetter.TrySetValue(staticIntInfo, "2.0", null));
            }

            //null propery
            for (int i = 0; i < 1000; ++i)
            {
                Assert.IsFalse(PropertyValueSetter.TrySetValue(null, i.ToString(), null));
            }

            //normal instance
            for (int i = 0; i< 1000; ++i)
            {
                Assert.IsTrue(PropertyValueSetter.TrySetValue(IntPropertyInfo, i.ToString(), instance));
                Assert.IsTrue(instance.IntProperty == i);
            }

            //instance is null
            for (int i = 0; i < 1000; ++i)
            {
                Assert.IsFalse(PropertyValueSetter.TrySetValue(IntPropertyInfo, i.ToString(), null));
            }

            //instance input error
            Assert.IsFalse(PropertyValueSetter.TrySetValue(IntPropertyInfo, "7.0", instance));
            for (int i = 0; i < 1000; ++i)
            {
                Assert.IsFalse(PropertyValueSetter.TrySetValue(IntPropertyInfo, string.Empty, null));
            }
        }
    }

    [TestClass]
    public class MethodInvokeTest
    {
        public class TestClass
        {
            public static int staticInvokeInt(int val)
            {
                return val;
            }
            public int IntvokeInt(int val)
            {
                return val * 2;
            }

            public static int staticIntInt(int val,int val1)
            {
                return val;
            }
        }

        [TestMethod]
        public void TestInvoke()
        {
            TestClass instance = new TestClass();
            MethodInfo staticIntMethodInfo = typeof(TestClass).GetMethod("staticInvokeInt", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            MethodInfo IntMethodInfo = typeof(TestClass).GetMethod("IntvokeInt", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            MethodInvoker staticIntMethod = new MethodInvoker(staticIntMethodInfo, null);
            MethodInvoker IntMethodNull = new MethodInvoker(IntMethodInfo, null);
            MethodInvoker IntMethod = new MethodInvoker(IntMethodInfo, instance);
            MethodInvoker NullMethod = new MethodInvoker(null, instance);

            //static normal
            for (int i = 0; i < 1000; ++i)
            {
                object outval;
                Assert.IsTrue(staticIntMethod.Invoke(out outval,i.ToString()) == 0);
                Assert.IsTrue((int)outval == i);
            }

            for (int i = 0; i < 1000; ++i)
            {
                object outval;
                Assert.IsFalse(IntMethodNull.Invoke(out outval, i.ToString()) == 0);
            }

            for (int i = 0; i < 1000; ++i)
            {
                object outval;
                Assert.IsTrue(IntMethod.Invoke(out outval, i.ToString()) == 0);
                Assert.IsTrue((int)outval == i * 2);
            }

            for (int i = 0; i < 1000; ++i)
            {
                object outval;
                Assert.IsFalse(IntMethod.Invoke(out outval, "2.0") == 0);
            }

            for (int i = 0; i < 1000; ++i)
            {
                object outval;
                Assert.IsFalse(NullMethod.Invoke(out outval, i.ToString()) == 0);
            }

        }
    }


}
