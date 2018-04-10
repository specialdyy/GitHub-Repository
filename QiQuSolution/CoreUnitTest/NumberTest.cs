using System;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreUnitTest
{
    [TestClass]
    public class NumberTest
    {
        [TestMethod]
        public void TestNumber()
        {
            string numberString0 = null;            //null
            string numberString1 = string.Empty;    //空字符。
            string numberString2 = "abc*";          //非法的整数字符。
            string numberString3 = "1a2b3c";        //含有部分非法字符的整数。
            string numberString4 = "a+1234";        //以非法字符开头的整数。
            string numberString5 = "+1234……";       //以非法字符结尾的整数。
            string numberString6 = "-";             //只有符号没有数值的非法格式。
            string numberString7 = "+12.34";        //小数。
            string numberString8 = "+000000";       //带符号的全为 0 的整数。

            string numberString9 = "+003267829";    //带符号且以0开头的整数。
            string numberString10 = "-000043233";   //带符号且以0开头的整数。
            string numberString11 = "+32184";       //带符号的普通整数。
            string numberString12 = "-32184";       //带符号的普通整数。
            string numberString13 = "3233";         //无符号整数。
            string numberString14 = "000043233";    //以0开头的无符号整数。
            string numberString15 = "+9999999999999999999999999999999999999999999";  //大于 long.MaxValue。
            string numberString16 = "-9999999999999999999999999999999999999999999";  //小于 long.MinValue。
            string numberString17 = "+" + ((long)int.MaxValue + 200);  //在 long 类型的合法范围内，但大于 int.MaxValue。
            string numberString18 = "" + ((long)int.MinValue - 2000);  //在 long 类型的合法范围内，但小于 int.MinValue。

            long number1 = -000328;         //带符号且以 0 开头的整数。
            long number2 = +00723;          //带符号且以 0 开头的整数。
            long number3 = 00003827;        //以 0 开头的整数。

            //Assert.AreEqual<Number>("-3".ToNumber(), "-3".ToNumber());
            Assert.IsTrue("-3".ToNumber().Equals("-3".ToNumber()));
            Assert.IsFalse("-3".ToNumber().Equals(null));
            Assert.IsFalse("-3".ToNumber().Equals(-3));
            Assert.IsFalse("-3".ToNumber().Equals("-4".ToNumber()));



            //Assert.AreEqual<string>("-000328", number1.ToString());    // number1.ToString() 会转换成 “-328”，所以测试结果为错误。
            //Assert.AreEqual<string>("+00723", number2.ToString());      // number2.ToString() 会转换成 “723”，所以测试结果为错误。
            //Assert.AreEqual<string>("00003827", number3.ToString());    // number3.ToString() 会转换成 “3827”，所以测试结果为错误。


            Assert.ThrowsException<ArgumentNullException>(() => { numberString0.ToNumber(); });
            Assert.ThrowsException<ArgumentNullException>(() => { numberString1.ToNumber(); });
            Assert.ThrowsException<ArgumentException>(() => { numberString2.ToNumber(); }, "所传入的数值格式非法！必须为整数数值类型。");
            Assert.ThrowsException<ArgumentException>(() => { numberString3.ToNumber(); }, "所传入的数值格式非法！必须为整数数值类型。");
            Assert.ThrowsException<ArgumentException>(() => { numberString4.ToNumber(); }, "所传入的数值格式非法！必须为整数数值类型。");
            Assert.ThrowsException<ArgumentException>(() => { numberString5.ToNumber(); }, "所传入的数值格式非法！必须为整数数值类型。");
            Assert.ThrowsException<ArgumentException>(() => { numberString6.ToNumber(); }, "所传入的数值格式非法！必须为整数数值类型。");
            Assert.ThrowsException<ArgumentException>(() => { numberString7.ToNumber(); }, "所传入的数值格式非法！必须为整数数值类型。");

            //string numberString8 = "+000000";       //带符号的全为 0 的整数。

            Number number8;
            Assert.ThrowsException<ArgumentException>(() => { number8 = numberString8.ToNumber(); }, "整数 0 既不是正数也不是负数，不能带有符号！");
            //Assert.AreEqual("000000", number8.ToString());
            //Assert.AreEqual("+000000", number8.OriginalString);
            //Assert.AreEqual("000000", number8.AbsoluteValueString);
            //Assert.AreEqual(0, number8.ToInt64());
            //Assert.AreEqual(0, number8.ToInt32());
            //Assert.AreEqual<string>("0,0,0,0,0,0", string.Join(",", number8.ToBits(SortType.FromHighToLow)));
            //Assert.AreEqual<int>(6, number8.Length);
            //Assert.AreEqual("00", number8.FirstTwo());
            //Assert.AreEqual("00", number8.LastTwo());
            //Assert.IsFalse(number8.IsNegative);
            //Assert.IsFalse(number8.IsPositive);



            //string numberString9 = "+003267829";    //带符号且以0开头的整数。

            Number number9 = numberString9.ToNumber();
            Assert.AreEqual("003267829", number9.ToString());
            Assert.AreEqual("+003267829", number9.OriginalString);
            Assert.AreEqual("003267829", number9.AbsoluteValueString);
            Assert.AreEqual(3267829, number9.ToInt64());
            Assert.AreEqual(3267829, number9.ToInt32());
            Assert.AreEqual<string>("0,0,3,2,6,7,8,2,9", string.Join(",", number9.ToBits(SortType.FromHighToLow)));
            Assert.AreEqual<int>(9, number9.Length);
            Assert.AreEqual("00", number9.FirstTwo());
            Assert.AreEqual("29", number9.LastTwo());
            Assert.IsFalse(number9.IsNegative);
            Assert.IsTrue(number9.IsPositive);

            //string numberString10 = "-000043233";   //带符号且以0开头的整数。

            Number number10 = numberString10.ToNumber();
            Assert.AreEqual("-000043233", number10.ToString());
            Assert.AreEqual("-000043233", number10.OriginalString);
            Assert.AreEqual("000043233", number10.AbsoluteValueString);
            Assert.AreEqual(-43233, number10.ToInt64());
            Assert.AreEqual(-43233, number10.ToInt32());
            Assert.AreEqual<string>("0,0,0,0,4,3,2,3,3", string.Join(",", number10.ToBits(SortType.FromHighToLow)));
            Assert.AreEqual<int>(9, number10.Length);
            Assert.AreEqual("00", number10.FirstTwo());
            Assert.AreEqual("33", number10.LastTwo());
            Assert.IsTrue(number10.IsNegative);
            Assert.IsFalse(number10.IsPositive);
            
            //string numberString11 = "+32184";       //带符号的普通整数。

            Number number11 = numberString11.ToNumber();
            Assert.AreEqual("32184", number11.ToString());
            Assert.AreEqual("+32184", number11.OriginalString);
            Assert.AreEqual("32184", number11.AbsoluteValueString);
            Assert.AreEqual(32184, number11.ToInt64());
            Assert.AreEqual(32184, number11.ToInt32());
            Assert.AreEqual<string>("3,2,1,8,4", string.Join(",", number11.ToBits(SortType.FromHighToLow)));
            Assert.AreEqual<int>(5, number11.Length);
            Assert.AreEqual("32", number11.FirstTwo());
            Assert.AreEqual("84", number11.LastTwo());
            Assert.IsFalse(number11.IsNegative);
            Assert.IsTrue(number11.IsPositive);

            //string numberString12 = "-32184";       //带符号的普通整数。

            Number number12 = numberString12.ToNumber();
            Assert.AreEqual("-32184", number12.ToString());
            Assert.AreEqual("-32184", number12.OriginalString);
            Assert.AreEqual("32184", number12.AbsoluteValueString);
            Assert.AreEqual(-32184, number12.ToInt64());
            Assert.AreEqual(-32184, number12.ToInt32());
            Assert.AreEqual<string>("3,2,1,8,4", string.Join(",", number12.ToBits(SortType.FromHighToLow)));
            Assert.AreEqual<int>(5, number12.Length);
            Assert.AreEqual("32", number12.FirstTwo());
            Assert.AreEqual("84", number12.LastTwo());
            Assert.IsTrue(number12.IsNegative);
            Assert.IsFalse(number12.IsPositive);

            //string numberString13 = "3233";         //无符号整数。

            Number number13 = numberString13.ToNumber();
            Assert.AreEqual("3233", number13.ToString());
            Assert.AreEqual("3233", number13.OriginalString);
            Assert.AreEqual("3233", number13.AbsoluteValueString);
            Assert.AreEqual(3233, number13.ToInt64());
            Assert.AreEqual(3233, number13.ToInt32());
            Assert.AreEqual<string>("3,2,3,3", string.Join(",", number13.ToBits(SortType.FromHighToLow)));
            Assert.AreEqual<int>(4, number13.Length);
            Assert.AreEqual("32", number13.FirstTwo());
            Assert.AreEqual("33", number13.LastTwo());
            Assert.IsFalse(number13.IsNegative);
            Assert.IsTrue(number13.IsPositive);

            //string numberString14 = "000043233";    //以0开头的无符号整数。

            Number number14 = numberString14.ToNumber();
            Assert.AreEqual("000043233", number14.ToString());
            Assert.AreEqual("000043233", number14.OriginalString);
            Assert.AreEqual("000043233", number14.AbsoluteValueString);
            Assert.AreEqual(43233, number14.ToInt64());
            Assert.AreEqual(43233, number14.ToInt32());
            Assert.AreEqual<string>("0,0,0,0,4,3,2,3,3", string.Join(",", number14.ToBits(SortType.FromHighToLow)));
            Assert.AreEqual<int>(9, number14.Length);
            Assert.AreEqual("00", number14.FirstTwo());
            Assert.AreEqual("33", number14.LastTwo());
            Assert.IsFalse(number14.IsNegative);
            Assert.IsTrue(number14.IsPositive);

            //string numberString15 = "+9999999999999999999999999999999999999999999";  //大于 long.MaxValue。
            
            Assert.ThrowsException<ArgumentException>(() => { numberString15.ToNumber(); }, "所传入的数值格式非法！必须为整数数值类型。");
            
            //string numberString16 = "-9999999999999999999999999999999999999999999";  //小于 long.MinValue。
            
            Assert.ThrowsException<ArgumentException>(() => { numberString16.ToNumber(); }, "所传入的数值格式非法！必须为整数数值类型。");

            //string numberString17 = "+" + ((long)int.MaxValue + 200);  //在 long 类型的合法范围内，但大于 int.MaxValue。

            Assert.ThrowsException<ApplicationException>(() => { numberString17.ToNumber().ToInt32(); }, "数值超出范围！");

            //string numberString18 = "-" + ((long)int.MinValue - 2000);  //在 long 类型的合法范围内，但小于 int.MinValue。

            Assert.ThrowsException<ApplicationException>(() => { numberString18.ToNumber().ToInt32(); }, "数值超出范围！");




            //long number1 = -000328;         //带符号且以 0 开头的整数。

            Number num1 = number1.ToString().ToNumber();
            Assert.AreEqual("-328", num1.ToString());
            Assert.AreEqual("-328", num1.OriginalString);
            Assert.AreEqual("328", num1.AbsoluteValueString);
            Assert.AreEqual(-328, num1.ToInt64());
            Assert.AreEqual(-328, num1.ToInt32());
            Assert.AreEqual<string>("3,2,8", string.Join(",", num1.ToBits(SortType.FromHighToLow)));
            Assert.AreEqual<int>(3, num1.Length);
            Assert.AreEqual("32", num1.FirstTwo());
            Assert.AreEqual("28", num1.LastTwo());
            Assert.IsTrue(num1.IsNegative);
            Assert.IsFalse(num1.IsPositive);

            //long number2 = +00723;          //带符号且以 0 开头的整数。

            Number num2 = number2.ToString().ToNumber();
            Assert.AreEqual("723", num2.ToString());
            Assert.AreEqual("723", num2.OriginalString);
            Assert.AreEqual("723", num2.AbsoluteValueString);
            Assert.AreEqual(723, num2.ToInt64());
            Assert.AreEqual(723, num2.ToInt32());
            Assert.AreEqual<string>("7,2,3", string.Join(",", num2.ToBits(SortType.FromHighToLow)));
            Assert.AreEqual<int>(3, num2.Length);
            Assert.AreEqual("72", num2.FirstTwo());
            Assert.AreEqual("23", num2.LastTwo());
            Assert.IsFalse(num2.IsNegative);
            Assert.IsTrue(num2.IsPositive);

            //long number3 = 00003827;        //以 0 开头的整数。

            Number num3 = number3.ToString().ToNumber();
            Assert.AreEqual("3827", num3.ToString());
            Assert.AreEqual("3827", num3.OriginalString);
            Assert.AreEqual("3827", num3.AbsoluteValueString);
            Assert.AreEqual(3827, num3.ToInt64());
            Assert.AreEqual(3827, num3.ToInt32());
            Assert.AreEqual<string>("3,8,2,7", string.Join(",", num3.ToBits(SortType.FromHighToLow)));
            Assert.AreEqual<int>(4, num3.Length);
            Assert.AreEqual("38", num3.FirstTwo());
            Assert.AreEqual("27", num3.LastTwo());
            Assert.IsFalse(num3.IsNegative);
            Assert.IsTrue(num3.IsPositive);
        }
    }
}
