using System;
using System.Collections.Generic;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreUnitTest
{
    [TestClass]
    public class DataDownloaderTest
    {
        [TestMethod]
        public void TestDataDownloader()
        {
            List<SourceData> data = DataDownloader.DownloadData();
            Assert.AreEqual<int>(data.Count, 10);
            //下面 3 个主要是用来测试 SourceData 各个属性的 get 访问器，没有实际作用。
            Assert.IsInstanceOfType(data[0].OnlineChange, typeof(string));
            Assert.IsInstanceOfType(data[0].OnlineTime, typeof(DateTime));
            Assert.IsInstanceOfType(data[0].OnlineNumber, typeof(string));

            Assert.IsTrue(data[0].Equals(data[0]));
            Assert.IsFalse(data[0].Equals(data[1]));
        }
    }
}
