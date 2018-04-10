using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreUnitTest
{
    [TestClass]
    public class WinLogTest
    {
        [TestMethod]
        public void TestWinLog()
        {
            //测试构造函数。
            Assert.ThrowsException<ArgumentNullException>(() => { new WinLog(null); });


            //构造一个 WinLog 对象，以供后续测试使用。
            SourceData data = new SourceData(DateTime.Now, "283948326", "-300");    //2 + 8 + 3 + 9 + 4 + 8 + 3 + 2 + 6 = 45，所以中奖号码为：58326。
            WinLog log = new WinLog(data);

            //填充数据，同时测试属性和字段。
            List<Number> planForecastNumbers = new List<Number>();
            string[] numbers = new string[] { "32", "48", "37", "29", "78", "05" };
            planForecastNumbers.ConvertAndFillNumbers(numbers);

            //测试在调用 UpdateResult 方法之前对 PlanForecast、IsWin 和 CurrentForecastType 三个属性的访问所引发的异常。
            Assert.ThrowsException<ApplicationException>(() => { ReadOnlyCollection<string> list = log.PlanForecast; });
            Assert.ThrowsException<ApplicationException>(() => { bool? result = log.IsWin; }, "本属性还没有被赋值！请务必确认使用本属性前 UpdateResult 方法已经被成功执行。");
            Assert.ThrowsException<ApplicationException>(() => { ForecastType? type = log.CurrentForecastType; }, "本属性还没有被赋值！请务必确认使用本属性前 UpdateResult 方法已经被成功执行。");


            Assert.IsTrue(log.FirstTwoPlanForecast.Count == 0);
            Assert.IsTrue(log.LastTwoPlanForecast.Count == 0);

            
            //Assert.ThrowsException<ArgumentException>(() => { log.UpdateResult(planForecastNumbers, planForecastNumbers.Count, ForecastType.FirstTwo); }, "所传入的 “历史中奖号码” 列表中含有格式非法的中奖号码！");
            List<Number> winNumbers = new List<Number>()
            {
                new Number("32811"),
                new Number("48812"),
                new Number("37813"),
                new Number("29814"),
                new Number("78815"),
                new Number("05816"),
            };
            //log.UpdateResult(winNumbers, 94, ForecastType.FirstTwo);


            //Assert.IsTrue(log.FirstTwoPlanForecast.Count > 0);
            //Assert.IsTrue(log.LastTwoPlanForecast.Count == 0);


            //测试在调用完 UpdateResult 方法之后对 CurrentForecastType 属性的访问。
            Assert.AreEqual<ForecastType>(ForecastType.FirstTwo, log.CurrentForecastType.Value);




            ////另外制造一个内容一样的用预测列表元素拼接而成的字符串，以供后面 “等价比较” 时使用。
            //string planForecast = null;
            //log.PlanForecast.ForEach(f => planForecast += " " + f.ToNumber().FirstTwo());


            Assert.AreEqual<DateTime>(data.OnlineTime, log.PlanningTime);
            Assert.AreEqual<Number>("58326".ToNumber(), log.WinningNumber);
            Assert.AreEqual<bool>(log.PlanForecast.Contains(log.WinningNumber.FirstTwo()), log.IsWin.Value);

            //Assert.AreNotEqual.AreEqual<string>(string.Join(" ", numbers), planForecast.Trim());


            //------------------------------------------------由于改变了测试对象的代码逻辑，所以有可能会导致以下的测试结果出错------------------------------------------------------
            #region 由于改变了测试对象的代码逻辑，所以有可能会导致以下的测试结果出错。
            //log.UpdateResult(planForecastNumbers, planForecastNumbers.Count, ForecastType.LastTwo);

            //Assert.ThrowsException<ArgumentNullException>(() => { log.UpdateResult(null, 0, ForecastType.FirstTwo); });

            //Assert.ThrowsException<ArgumentOutOfRangeException>(() => { log.UpdateResult(new List<Number>(), 0, ForecastType.FirstTwo); }, "计划预测的定码（前二码/后二码）数量超出允许的最大范围（1-100）！");

            //planForecastNumbers.AddRange(planForecastNumbers);
            //planForecastNumbers.AddRange(planForecastNumbers);
            //planForecastNumbers.AddRange(planForecastNumbers);
            //planForecastNumbers.AddRange(planForecastNumbers);
            //planForecastNumbers.AddRange(planForecastNumbers);
            //planForecastNumbers.AddRange(planForecastNumbers);
            //planForecastNumbers.AddRange(planForecastNumbers);
            //planForecastNumbers.AddRange(planForecastNumbers);
            //planForecastNumbers.AddRange(planForecastNumbers);
            //planForecastNumbers.AddRange(planForecastNumbers);
            //planForecastNumbers.AddRange(planForecastNumbers);
            //planForecastNumbers.AddRange(planForecastNumbers);
            //Assert.ThrowsException<ArgumentOutOfRangeException>(() => { log.UpdateResult(planForecastNumbers, planForecastNumbers.Count, ForecastType.FirstTwo); }, "计划预测的定码（前二码/后二码）数量超出允许的最大范围（1-100）！");



            //planForecastNumbers.Clear();
            //planForecastNumbers.ConvertAndFillNumbers(numbers);
            //planForecastNumbers.ConvertAndFillNumbers(numbers);
            //Assert.ThrowsException<ArgumentOutOfRangeException>(() => { log.UpdateResult(planForecastNumbers, 0, ForecastType.FirstTwo); }, "要保留的定码个数超出合法范围（最小值为 1，最大值为 “100 - 所要排除的最新期数号”）！");
            //Assert.ThrowsException<ArgumentOutOfRangeException>(() => { log.UpdateResult(planForecastNumbers, (100 - planForecastNumbers.Count + 3), ForecastType.FirstTwo); }, "要保留的定码个数超出合法范围（最小值为 1，最大值为 “100 - 所要排除的最新期数号”）！");
            //Assert.ThrowsException<ArgumentNullException>(() => { log.UpdateResult(planForecastNumbers, planForecastNumbers.Count, null); });


            //planForecastNumbers.Clear();
            //planForecastNumbers.ConvertAndFillNumbers(numbers);
            //Assert.ThrowsException<ApplicationException>(() => { log.UpdateResult(planForecastNumbers, 9, ForecastType.FirstTwo); }, "要求保留的元素数不能多于所提供的集合的元素总数！");
            //log.UpdateResult(planForecastNumbers, 2, ForecastType.FirstTwo);

            //log.Clone();
            #endregion
        }
    }
}
