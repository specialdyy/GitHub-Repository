using System;
using System.Collections.Generic;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreUnitTest
{
    [TestClass]
    public class NumberHelperTest
    {
        [TestMethod]
        public void TestNumberHelper()
        {
            Number n1 = "-3".ToNumber();

            List<Number> numberList = new List<Number>();
            string[] numbers = new string[] { "32", "48", "37", "29", "78", "05" };
            numberList.ConvertAndFillNumbers(numbers);

            numberList.Clear();
            numbers = new string[] { };
            numberList.ConvertAndFillNumbers(numbers);
            Assert.AreEqual<int>(0, numberList.Count);

            numbers = null;
            numberList.ConvertAndFillNumbers(numbers);
            Assert.AreEqual<int>(0, numberList.Count);

            numbers = new string[] { "32", "48", "37", "29", "78", "05" };
            numberList = null;
            Assert.ThrowsException<ArgumentNullException>(() => { numberList.ConvertAndFillNumbers(numbers); }, "所传入的 List<Number> 类型的列表对象不能为空！");

        }
    }
}
