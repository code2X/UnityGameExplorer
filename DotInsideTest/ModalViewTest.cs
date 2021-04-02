using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExplorerSpace
{
    [TestClass]
    public class ArrayElementInputWindowTest
    {
        [TestMethod]
        public ArrayElementInputWindow TestGetInstance()
        {
            ArrayElementInputWindow arrayInstance = ArrayElementInputWindow.GetInstance();
            Assert.IsTrue(arrayInstance != null);
            return arrayInstance;
        }

        [TestMethod]
        public void TestSetElementValue()
        {
            int[] array = new int[5];
            array[0] = 10;
            array[1] = 10;
            array[2] = 10;
            array[3] = 10;
            array[4] = 10;

            ArrayElementInputWindow arrayInstance = TestGetInstance();

            int number = 11;

            //Null array
            for (int i = 0; i < array.Length; ++i)
            {
                Assert.IsFalse(arrayInstance.SetArrayElementValue(null, typeof(int), i, 11.ToString()));
                Assert.IsTrue(array[i] == 10);
            }

            //Null string
            for (int i = 0; i < array.Length; ++i)
            {
                Assert.IsFalse(arrayInstance.SetArrayElementValue(null, typeof(int), i, string.Empty));
                Assert.IsTrue(array[i] == 10);
            }

            //Normal
            for (int i = 0; i< array.Length; ++i)
            {
                Assert.IsTrue( arrayInstance.SetArrayElementValue(array, typeof(int), i, 11.ToString()) );
                Assert.IsTrue(array[i] == number);
            }

            //Error Type
            for (int i = 0; i < array.Length; ++i)
            {
                Assert.IsFalse(arrayInstance.SetArrayElementValue(array, typeof(float), i, i.ToString()));
                Assert.IsTrue(array[i] == number);
            }
            
            //Error Index
            for (int i = array.Length; i < array.Length; ++i)
            {
                Assert.IsFalse(arrayInstance.SetArrayElementValue(array, typeof(int), i, i.ToString()));
                Assert.IsTrue(array[i] == number);
            }
            for (int i = 1; i < array.Length; ++i)
            {
                Assert.IsFalse(arrayInstance.SetArrayElementValue(array, typeof(int), -i, i.ToString()));
                Assert.IsTrue(array[i] == number);
            }
        }



    }
}
