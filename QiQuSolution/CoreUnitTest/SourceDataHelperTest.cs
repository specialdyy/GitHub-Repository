using System;
using System.Collections.Generic;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreUnitTest
{
    [TestClass]
    public class SourceDataHelperTest
    {
        [TestMethod]
        public void TestSourceDataHepler()
        {
            Number n1 = null;
            Assert.ThrowsException<ArgumentNullException>(() => { n1.FirstTwo(); });
            Assert.ThrowsException<ArgumentNullException>(() => { n1.LastTwo(); });
            Assert.ThrowsException<ArgumentNullException>(() => { n1.CalWinningNumber(); });


            Number n2 = "3".ToNumber();
            Assert.ThrowsException<ApplicationException>(() => { n2.FirstTwo(); }, "所传入的数值总长度不足 2 位，无法获取前二码！");
            Assert.ThrowsException<ApplicationException>(() => { n2.LastTwo(); }, "所传入的数值总长度不足 2 位，无法获取后二码！");
            Assert.ThrowsException<ApplicationException>(() => { n2.CalWinningNumber(); }, "所传入的数值总长度不足 4 位，无法计算中奖号码！");

            Number n3 = "-3214".ToNumber();
            Assert.AreEqual<string>("03214", n3.CalWinningNumberString());
            Assert.AreEqual<int>(03214, n3.CalWinningNumber().ToInt32());

            Number n4 = "+9824375214".ToNumber();
            Assert.AreEqual<string>("55214", n4.CalWinningNumberString());
            Assert.AreEqual<int>(55214, n4.CalWinningNumber().ToInt32());

            Number n5;

            Assert.ThrowsException<ArgumentException>(() => { n5 = "+0000".ToNumber(); }, "整数 0 既不是正数也不是负数，不能带有符号！");
            //Assert.AreEqual<string>("00000", n5.CalWinningNumberString());
            //Assert.AreEqual<int>(00000, n5.CalWinningNumber().ToInt32());

            Number n6;
            Assert.ThrowsException<ArgumentException>(() => { n6 = "-0000".ToNumber(); }, "整数 0 既不是正数也不是负数，不能带有符号！");
            //Assert.AreEqual<string>("00000", n6.CalWinningNumberString());
            //Assert.AreEqual<int>(00000, n6.CalWinningNumber().ToInt32());

            List<SourceData> datas = new List<SourceData>();
            datas.Add(new SourceData(DateTime.Now, "1233456456", "30121"));
            datas.Add(new SourceData(DateTime.Now, "1233456451", "30122"));
            datas.Add(new SourceData(DateTime.Now, "1233456453", "30123"));
            datas.Add(new SourceData(DateTime.Now, "1233456454", "30124"));
            datas.Add(new SourceData(DateTime.Now, "1233456458", "30125"));
            datas.Add(new SourceData(DateTime.Now, "1233456452", "30126"));
            datas.Add(new SourceData(DateTime.Now, "1233456459", "30127"));
            datas.Add(new SourceData(DateTime.Now, "1233456450", "30128"));
            List<Number> winningNumbers = datas.ExtractWinningNumber();
            List<string> firstTwo = datas.ExtractFirstTwo();
            List<string> lastTwo = datas.ExtractLastTwo();

            Assert.AreEqual<Number>("1233456456".ToNumber().CalWinningNumber(), winningNumbers[0]);
            Assert.AreEqual<string>("1233456456".ToNumber().CalWinningNumber().FirstTwo(), firstTwo[0]);
            Assert.AreEqual<string>("1233456456".ToNumber().CalWinningNumber().LastTwo(), lastTwo[0]);

        }
    }
}
