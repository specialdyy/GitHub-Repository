using System;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreUnitTest
{
    [TestClass]
    public class SourceDataTest
    {
        [TestMethod]
        public void TestSourceData()
        {
            DateTime currTime = DateTime.Now;
            SourceData data = new SourceData(DateTime.Now, "283948326", "-300");    //2 + 8 + 3 + 9 + 4 + 8 + 3 + 2 + 6 = 45，所以中奖号码为：58326。
            //data.OnlineTime = currTime;
            //data.OnlineTime = currTime;     //测试重复赋相同的值的情况。
            data.InitialData(currTime, data.OnlineNumber, data.OnlineChange);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { data.InitialData(currTime.AddDays(-15), data.OnlineNumber, data.OnlineChange); }, "所要设置的源数据的 “在线时间” 范围必须要在今天之前和之后的 10 天之内！");
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { data.InitialData(currTime.AddDays(13), data.OnlineNumber, data.OnlineChange); }, "所要设置的源数据的 “在线时间” 范围必须要在今天之前和之后的 10 天之内！");

            Assert.AreEqual<string>("-300", data.OnlineChange);
            Assert.AreEqual<string>("283948326", data.OnlineNumber);
            Assert.AreEqual<DateTime>(currTime, data.OnlineTime);

            Assert.IsTrue(data.Equals(data));
            //Assert.IsFalse(data.Equals(new SourceData()));
            Assert.IsFalse(data.Equals(null));

            data = new SourceData(DateTime.Now, "283948327", "-301");
            Number winNumber = "68327".ToNumber();
            Assert.AreEqual<Number>(winNumber, data.WinningNumber);
            Assert.AreEqual<string>(winNumber.FirstTwo(), data.FirstTwo);
            Assert.AreEqual<string>(winNumber.LastTwo(), data.LastTwo);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => { new SourceData(DateTime.Now, "-123456789", "-301"); }, "在线人数值只能为非 0 的正整数！");


            #region 与业务逻辑关系不大，测试值传递和引用传递。
            data = new SourceData(DateTime.Now, "123456789", "-301");
            string f1 = data.FirstTwo;
            string l1 = data.LastTwo;
            string c1 = data.OnlineChange;
            SourceData data1 = (SourceData)data.Clone();
            Assert.IsFalse(object.ReferenceEquals(data, data1));

            string f2 = data1.FirstTwo;
            string l2 = data1.LastTwo;
            string c2 = data1.OnlineChange;

            Assert.IsFalse(object.ReferenceEquals(data.FirstTwo, data1.FirstTwo));      //为 false，因为 LastTwo 和 FirstTwo 属性都是通过 SourceData 对象的 OnlineNmuber 来重新计算并创建的。
            Assert.IsFalse(object.ReferenceEquals(data.LastTwo, data1.LastTwo));        //为 false，因为 LastTwo 和 FirstTwo 属性都是通过 SourceData 对象的 OnlineNmuber 来重新计算并创建的。
            Assert.IsTrue(object.ReferenceEquals(data.OnlineChange, data1.OnlineChange));   //比较不解，为什么结果是 True？而上面同样是 string 类型的属性却为 False。
            Assert.IsTrue(object.ReferenceEquals(data.OnlineNumber, data1.OnlineNumber));   //估计结果和 OnlineChange 属性类似。
            Assert.IsFalse(object.ReferenceEquals(data.OnlineTime, data1.OnlineTime));      //估计是因为 DateTime 是值类型，赋值的是拷贝，所以两个不是同一个引用？
            Assert.IsFalse(object.ReferenceEquals(data.WinningNumber, data1.WinningNumber));
            #endregion
        }
    }
}
