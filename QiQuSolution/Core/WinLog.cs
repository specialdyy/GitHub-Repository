using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GS = Core.GlobalSettings;

namespace Core
{
    [Serializable]
    public class WinLog : ICloneable
    {
        private WinLog()
        {
            this.firstTwoPlanForecast = new List<string>();
            this.lastTwoPlanForecast = new List<string>();
            this.isWin = null;
            this.currentForecastType = null;
        }

        /// <summary>
        /// 在还没有公布开奖结果的信息之前，根据指定的 “预测列表”、实际获取到的 “排除最新期数号” 的数量和 “定码类型” 来创建一个用来存储开奖记录数据的对象。
        /// 即根据：计划预测 “PlanForecast”、定码类型 “currentForecastType”、计划结果 “isWin” 来创建 WinLog 对象。
        /// </summary>
        /// <param name="planForecastNumberList">在当前中奖纪录开奖前所计算预测出的 “计划预测” 列表，“定码个数” 由该 planForecastNumberList 列表的元素个数来确定。</param>
        /// <param name="actuallyHistoryWinningNumberCount"> 实际上能够获取得到用来计算 “计划预测” 列表的 “排除最新期数号” 数量（因为有时候能获取到的 “排除最新期数号” 的数量并不一定能够达到所要求获取的数量）。</param>
        /// <param name="forecastType">定码的类型（前二码/后二码）。</param>
        public WinLog(List<string> planForecastNumberList, int actuallyHistoryWinningNumberCount, ForecastType forecastType) : this()
        {
            FillForecast(planForecastNumberList, actuallyHistoryWinningNumberCount, forecastType);
        }


        /// <summary>
        /// 在还没有公布开奖结果的信息之前，根据指定的 “历史中奖号码记录列表”、“定码数量” 和 “定码类型” 来创建一个用来存储开奖记录数据的对象。
        /// 即根据：计划预测 “PlanForecast”、定码类型 “currentForecastType”、计划结果 “isWin” 来创建 WinLog 对象。
        /// </summary>
        /// <param name="historyWinningNumber">历史中奖号码记录集合（集合可以没有任何元素，但本身不可以为 null，否则会抛出异常）。</param>
        /// <param name="dingMaCount">设定要保留的定码个数。</param>
        /// <param name="forecastType">设定的定码类型。</param>
        public WinLog(List<Number> historyWinningNumber, int dingMaCount, ForecastType forecastType) : this()
        {
            FillForecast(historyWinningNumber, dingMaCount, forecastType);
        }


        /// <summary>
        /// 在开奖结果的信息公布之后，根据开奖结果的源数据对象 sourceData 来创建一个用来存储开奖记录数据的对象。
        /// 即根据 SourceData 对象创建 WinLog 对象。
        /// </summary>
        /// <param name="sourceData">初始化当前的 WinLog 对象相关数据所需要的元数据对象。</param>
        public WinLog(SourceData sourceData) : this()
        {
            FillResult(sourceData);
        }

        /// <summary>
        /// 在开奖结果的信息公布之后，根据指定的 “预测列表”、“定码类型” 和开奖结果的源数据对象 sourceData 来创建一个用来存储开奖记录数据的对象。
        /// 即根据：计划预测 “PlanForecast”、定码类型 “currentForecastType”、计划结果 “isWin” 和 SourceData 对象来创建 WinLog 对象。
        /// </summary>
        /// <param name="planForecastNumberList"></param>
        /// <param name="actuallyHistoryWinningNumberCount"> 实际上能够获取得到用来计算 “计划预测” 列表的 “排除最新期数号” 数量（因为有时候能获取到的 “排除最新期数号” 的数量并不一定能够达到所要求获取的数量）。</param>
        /// <param name="forecastType"></param>
        /// <param name="sourceData"></param>
        public WinLog(List<string> planForecastNumberList, int actuallyHistoryWinningNumberCount, ForecastType forecastType, SourceData sourceData) : this(planForecastNumberList,actuallyHistoryWinningNumberCount, forecastType)
        {
            FillResult(sourceData);
        }



        /// <summary>
        /// 根据开奖结果的源数据对象 sourceData 来初始化当前 WinLog 对象的相关字段数据。
        /// </summary>
        /// <param name="sourceData">初始化当前的 WinLog 对象相关数据所需要的元数据对象。</param>
        public void FillResult(SourceData sourceData)
        {
            this.sourceData = sourceData ?? throw new ArgumentNullException("sourceData");
            this.timeId = this.sourceData.TimeId;
            this.planningTime = this.sourceData.OnlineTime;
            this.winningNumber = this.sourceData.WinningNumber;
        }

        /// <summary>
        /// 判断 sourceData 对象是否已经初始化，如果已经初始化则返回 true，否则返回 false。
        /// </summary>
        private bool HasInitialedSourceData()
        {
            return this.sourceData != null;
        }

        /// <summary>
        /// 和 HasInitialedSourceData() 方法的功能类似，但 HasInitialedSourceData() 方法在没满足条件时只返回 false，而本方法则是抛出异常。
        /// </summary>
        private void ValidateSourceDataHasInitialed()
        {
            if (HasInitialedSourceData() == false)
            {
                throw new ApplicationException("源数据对象 sourceData 为空！请务必确认使用相关的关联属性前源数据对象 sourceData 已被初始化。");
            }
            return;
        }



        private SourceData sourceData;


        private int? timeId;

        public int TimeId
        {
            get
            {
                if (this.timeId.HasValue == false)
                {
                    ValidateSourceDataHasInitialed();
                    this.timeId = this.sourceData.TimeId;
                }
                return this.timeId.Value;
            }
        }


        private DateTime? planningTime;
        /// <summary>
        /// 获取当前开奖记录所对应的时间。
        /// </summary>
        public DateTime PlanningTime
        {
            get
            {
                if (this.planningTime.HasValue == false)
                {
                    ValidateSourceDataHasInitialed();
                    this.planningTime = this.sourceData.OnlineTime;
                }
                return this.planningTime.Value;
            }
        }


        private Number winningNumber;
        /// <summary>
        /// 获取中奖号码值。
        /// </summary>
        public Number WinningNumber
        {
            get
            {
                if (this.winningNumber == null)
                {
                    ValidateSourceDataHasInitialed();
                    this.winningNumber = this.sourceData.WinningNumber;
                }
                return this.winningNumber;
            }
        }
















        /// <summary>
        /// 判断 “计划预测” 列表是否已经完成初始化（即 FillForecast() 方法是否已经被成功执行，因为如果 FillForecast() 方法已执行，则 this.currentForecastType 就不会为 null），如果已经初始化则返回 true，否则返回 false。
        /// </summary>
        private bool HasFilledForecast()
        {
            return this.currentForecastType != null;
        }

        /// <summary>
        /// 和 HasFilledForecast() 方法的功能类似，但 HasFilledForecast() 方法在没满足条件时只返回 false，而本方法则是抛出异常。
        /// </summary>
        private void ValidateHasForecast()
        {
            if (HasFilledForecast() == false)
            {
                throw new ApplicationException("当前的 “定码类型”（即：currentForecastType）值还没被初始化！请务必确认使用相关的关联属性前 FillForecast 方法已经被成功执行。");
            }
            return;
        }




        private List<string> firstTwoPlanForecast;
        /// <summary>
        /// 获取前二码的计划预测列表。
        /// 本属性能使用的前提条件是（2 个前提条件任意选 1 个）：
        /// 1.必须要调用并执行过 FillForecast 方法。
        /// 2.当前对象是通过含有 planForecastNumberList 和 forecastType 两个参数的构造函数来创建的（满足该条件的构造函数有 2 个）。
        /// </summary>
        public List<string> FirstTwoPlanForecast
        {
            get
            {
                ValidateHasForecast();
                return this.firstTwoPlanForecast;
            }
        }


        private List<string> lastTwoPlanForecast;
        /// <summary>
        /// 获取后二码的计划预测列表。
        /// 本属性能使用的前提条件是（2 个前提条件任意选 1 个）：
        /// 1.必须要调用并执行过 FillForecast 方法。
        /// 2.当前对象是通过含有 planForecastNumberList 和 forecastType 两个参数的构造函数来创建的（满足该条件的构造函数有 2 个）。
        /// </summary>
        public List<string> LastTwoPlanForecast
        {
            get
            {
                ValidateHasForecast();
                return this.lastTwoPlanForecast;
            }
        }


        /// <summary>
        /// 自动地根据字段 currentForecastType 所指定的定码类型来获取或设置相对应的（前二码或是后二码）计划预测列表。
        /// 本属性能使用的前提条件是（2 个前提条件任意选 1 个）：
        /// 1.必须要调用并执行过 FillForecast 方法。
        /// 2.当前对象是通过含有 planForecastNumberList 和 forecastType 两个参数的构造函数来创建的（满足该条件的构造函数有 2 个）。
        /// </summary>
        public ReadOnlyCollection<string> PlanForecast
        {
            get
            {
                ValidateHasForecast();
                List<string> planForecast = this.currentForecastType == ForecastType.FirstTwo ? this.firstTwoPlanForecast : this.lastTwoPlanForecast;
                return planForecast.AsReadOnly();
            }

            private set
            {
                #region 注意！这里 value.Count == 0 的情况是允许的，就相当于为当前属性创建一个没有任何元素的列表对象。
                if (value == null)
                {
                    return;
                    ////由于当前属性的 setter 是 private 的，所以感觉给当前属性赋 null 值也不至于抛异常，直接返回即可。
                    //throw new ArgumentNullException("PlanForecast");
                }
                if (value.Count > 100)
                {
                    throw new ArgumentOutOfRangeException("PlanForecast");
                }
                #endregion
                ValidateHasForecast();

                List<string> currentForecastContainer = this.currentForecastType == ForecastType.FirstTwo ? this.firstTwoPlanForecast : this.lastTwoPlanForecast;
                currentForecastContainer = value.ToList();
            }
        }


        private bool? isWin;
        /// <summary>
        /// 获取计划结果。
        /// 本属性能使用的前提条件是（2 个前提条件任意选 1 个）：
        /// 1.必须要调用并执行过 FillForecast 方法。
        /// 2.当前对象是通过含有 planForecastNumberList 和 forecastType 两个参数的构造函数来创建的（满足该条件的构造函数有 2 个）。
        /// </summary>
        public bool? IsWin
        {
            get
            {
                if (this.isWin.HasValue == false && HasFilledForecast() && HasInitialedSourceData())
                {
                    bool isFirstTwo = this.currentForecastType == ForecastType.FirstTwo;
                    List<string> container = isFirstTwo ? this.firstTwoPlanForecast : this.lastTwoPlanForecast;
                    string winNum = isFirstTwo ? this.winningNumber.FirstTwo() : this.winningNumber.LastTwo();
                    this.isWin = container.Contains(winNum);
                }
                return this.isWin;
            }
        }


        private ForecastType? currentForecastType;
        /// <summary>
        /// 获取当前所设定的定码类型（前二码/后二码）。
        /// 本属性能使用的前提条件是（2 个前提条件任意选 1 个）：
        /// 1.必须要调用并执行过 FillForecast 方法。
        /// 2.当前对象是通过含有 planForecastNumberList 和 forecastType 两个参数的构造函数来创建的（满足该条件的构造函数有 2 个）。
        /// </summary>
        public ForecastType? CurrentForecastType
        {
            get
            {
                ValidateHasForecast();
                return currentForecastType;
            }
        }


        private int actuallyHistoryWinningNumberCount;
        /// <summary>
        /// 实际上能够获取得到用来计算 “计划预测” 列表的 “排除最新期数号” 数量（因为有时候能获取到的 “排除最新期数号” 的数量并不一定能够达到所要求获取的数量）。
        /// 本属性能使用的前提条件是（2 个前提条件任意选 1 个）：
        /// 1.必须要调用并执行过 FillForecast 方法。
        /// 2.当前对象是通过含有 planForecastNumberList 和 forecastType 两个参数的构造函数来创建的（满足该条件的构造函数有 2 个）。
        /// </summary>
        public int ActuallyHistoryWinningNumberCount
        {
            get
            {
                ValidateHasForecast();
                return actuallyHistoryWinningNumberCount;
            }
        }


        /// <summary>
        /// 根据指定的 “预测列表” 和 “定码类型” 来填充或更新当前对象的未确定属性（例如：计划预测 “PlanForecast”、计划结果 “isWin”、定码类型 “currentForecastType”、实际的排除最新期数号“actuallyHistoryWinningNumberCount” 等）的值。
        /// </summary>
        /// <param name="planForecastNumberList">在当前中奖纪录开奖前所计算预测出的 “计划预测” 列表，“定码个数” 由该 planForecastNumberList 列表的元素个数来确定。</param>
        /// <param name="actuallyHistoryWinningNumberCount"> 实际上能够获取得到用来计算 “计划预测” 列表的 “排除最新期数号” 数量（因为有时候能获取到的 “排除最新期数号” 的数量并不一定能够达到所要求获取的数量）。</param>
        /// <param name="forecastType">定码的类型（前二码/后二码）。</param>
        public void FillForecast(List<string> planForecastNumberList, int actuallyHistoryWinningNumberCount, ForecastType forecastType)
        {
            if (planForecastNumberList == null)
            {
                throw new ArgumentNullException("planForecastNumberList");
            }

            //如果 “计划预测” 列表 planForecastNumberList 中出现格式非法（即号码长度不等于 2）的预测号码值，则抛出异常。
            if (planForecastNumberList.Exists(n => n.Length != 2))
            {
                throw new ArgumentException("所传入的 “计划预测” 列表中含有格式非法的预测号码！", "planForecastNumberList");
            }


            //填充当前选中的 “定码类型”（前二码/后二码）。
            this.currentForecastType = forecastType;

            this.actuallyHistoryWinningNumberCount = actuallyHistoryWinningNumberCount;

            bool isFirstTwo = this.currentForecastType == ForecastType.FirstTwo;

            #region 旧思路
            ////如果当前的定码类型为 “前二码”。
            //if (isFirstTwo)
            //{
            //    this.firstTwoPlanForecast.Clear();
            //    //填充 “计划预测” 列表。
            //    this.firstTwoPlanForecast.AddRange(planForecastNumberList);
            //    //计算 “计划结果” 值（即 this.isWin 字段值），判断是否中奖。
            //    this.isWin = this.firstTwoPlanForecast.Contains(this.winningNumber.FirstTwo());
            //}
            ////否则，如果当前的定码类型为 “后二码”。
            //else
            //{
            //    this.lastTwoPlanForecast.Clear();
            //    //填充 “计划预测” 列表。
            //    this.lastTwoPlanForecast.AddRange(planForecastNumberList);
            //    //计算 “计划结果” 值（即 this.isWin 字段值），判断是否中奖。
            //    this.isWin = this.lastTwoPlanForecast.Contains(this.winningNumber.LastTwo());
            //}
            #endregion


            #region 新思路

            #region 严重错误！！！下面第一行的把 firstTwoPlanForecast 或 lastTwoPlanForecast（假设为变量 a 或 b，以 a 为例）赋值给变量 currentForecastContainer（假设为 c）的行为本质上是把 a 变量中所存储的对象的引用赋值给变量 c，所以此时 c 中存储的是对 a 中存储的对象的引用。而又因为下面第 2 行是给 c 重新赋值，而不是给 c 所指向的集合对象添加元素，所以执行完之后变量 a 中的集合还是没有任何元素。
            ////先根据当前的定码类型来设定当前用来存储的存储容器字段是 “前二码”（this.firstTwoPlanForecast）还是 “后二码”（this.lastTwoPlanForecast）的。
            //List<string> currentForecastContainer = isFirstTwo ? this.firstTwoPlanForecast : this.lastTwoPlanForecast;

            ////在当前的存储容器字段 currentForecast 中填充 “计划预测” 列表。
            //currentForecastContainer = planForecastNumberList;
            #endregion
            #region 修改之后解决了上面的严重错误，只要把 “直接给变量 currentForecastContainer 重新赋值” 的操作改成 “直接给变量 currentForecastContainer 所引用的集合对象添加元素” 即可。
            //先根据当前的定码类型来设定当前用来存储的存储容器字段是 “前二码”（this.firstTwoPlanForecast）还是 “后二码”（this.lastTwoPlanForecast）的。
            List<string> currentForecastContainer = isFirstTwo ? this.firstTwoPlanForecast : this.lastTwoPlanForecast;

            currentForecastContainer.Clear();
            //在当前的存储容器字段 currentForecast 中填充 “计划预测” 列表。
            currentForecastContainer.AddRange(planForecastNumberList);
            #endregion

            //如果 sourceData 对象已经初始化，则顺带初始化 this.isWin 字段。
            if (this.sourceData != null)
            {
                string dingMa = isFirstTwo ? this.winningNumber.FirstTwo() : this.winningNumber.LastTwo();
                //计算 “计划结果” 值（即 this.isWin 字段值），判断是否中奖。
                this.isWin = currentForecastContainer.Contains(dingMa);
            }
            #endregion
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="historyWinningNumber">最新的某个范围的历史中奖号码的集合，范围由该 historyWinningNumber 集合的元素个数来确定，集合中的每个中奖号码之间没有顺序关系，中奖号码有可能会出现重复。</param>
        /// <param name="dingMaCount">要经过筛选后最后保留的定码（前二码/后二码）个数。</param>
        /// <param name="forecastType">定码的类型（前二码/后二码）。</param>
        public void FillForecast(List<Number> historyWinningNumber, int dingMaCount, ForecastType forecastType)
        {
            List<string> planForecastList = CalPlanForecastList(historyWinningNumber, dingMaCount, forecastType);
            FillForecast(planForecastList, historyWinningNumber.Count, forecastType);
        }



        #region 计算 “计划预测”
        /// <summary>
        /// 根据传入的最新历史中奖号码集合（historyWinningNumber）、保留的定码个数（dingMaCount）和定码类型（forecastType）来计算出下一个即将公布的中奖号码的定码 “计划预测” 列表。
        /// 如果 historyWinningNumber 为 null 则抛出异常。
        /// 注意：该方法目的是计算即将公布（但还没公布）的未来一期的 “计划预测” 值；
        /// historyWinningNumber 参数的 Count 属性值其实就是界面上选择的 “排除最新期数号”；
        /// dingMaCount 参数其实就是界面上选择的 “定码个数”；
        /// forecastType 参数就是界面上选择的定码类型 “前二码/后二码”；
        /// </summary>
        /// <param name="historyWinningNumber">最新的某个期数范围的历史中奖号码的集合（集合可以没有任何元素，但本身不可以为 null），期数范围由该 historyWinningNumber 集合的元素个数来确定，集合中的每个中奖号码之间没有顺序关系，中奖号码有可能会出现重复。</param>
        /// <param name="dingMaCount">要经过筛选后最后保留的定码（前二码/后二码）个数。</param>
        /// <param name="forecastType">定码的类型（前二码/后二码）。</param>
        /// <returns>
        /// 返回 “计划预测” 列表。如果 historyWinningNumber 为 null 则抛出异常。
        /// </returns>
        private List<string> CalPlanForecastList(List<Number> historyWinningNumber, int dingMaCount, ForecastType forecastType)
        {
            if (historyWinningNumber == null)
            {
                throw new ArgumentNullException("historyWinningNumber");
            }
            //如果 “排除最新期数” 的数量超出允许的最大范围。
            if (historyWinningNumber.Count < GS.MIN_NEWEST_PERIOD_COUNT || historyWinningNumber.Count > GS.MAX_NEWEST_PERIOD_COUNT)
            {
                throw new ArgumentOutOfRangeException("historyWinningNumber", "“排除最新期数” 的数量超出允许的最大范围（" + GS.MIN_NEWEST_PERIOD_COUNT + "-" + GS.MAX_NEWEST_PERIOD_COUNT + "）！");
            }
            if (dingMaCount < GS.MIN_DING_MA_COUNT || dingMaCount > (100 - historyWinningNumber.Count))
            {
                throw new ArgumentOutOfRangeException("dingMaCount", "所设置的 “排除最新期数号” 和 “定码个数” 两个数的和不能大于 100！请重新设置！");
            }
            //dingMaCount 参数就算满足了上面的限定，也有可能会超过所允许的最大值 MAX_DING_MA_COUNT，所以需要在此继续补充判断。
            if (dingMaCount > GS.MAX_DING_MA_COUNT)
            {
                throw new ArgumentOutOfRangeException("dingMaCount", "要保留的定码个数超出所允许的最大值（" + GS.MAX_DING_MA_COUNT + "）！");
            }
            //如果历史中奖号码列表 historyWinningNumber 中出现格式非法（即号码长度不等于 5）的中奖号码值，则抛出异常。
            if (historyWinningNumber.Exists(n => n.Length != 5))
            {
                throw new ArgumentException("所传入的 “历史中奖号码” 列表中含有格式非法的中奖号码！", "historyWinningNumber");
            }

            bool isFirstTwo = forecastType == ForecastType.FirstTwo;
            List<string> planForecastListWaitForRemove = (from n in historyWinningNumber
                                                          select isFirstTwo ? n.FirstTwo() : n.LastTwo()).Distinct().ToList();
            
            ////由于该方法的功能只是负责计算出 “计划预测” 列表，并不需要设置当前对象的相关数据，填充数据的工作交给相关的方法去实现。
            //this.actuallyHistoryWinningNumberCount = historyWinningNumber.Count;
            
            #region 用后面的几行代码代替第 1 行的 GetRandoms() 方法，降低耦合度。
            //return GetRandoms(planForecastListWaitForRemove, dingMaCount).ToList();

            List<string> fullItems = GetFullForecastNumberList();
            RemoveSpecifiedItems(fullItems, planForecastListWaitForRemove);
            return RandomSelectSomeItems(fullItems, dingMaCount).ToList();
            #endregion
        }

        #region 在当前类中感觉不合理，不合理的地方在于：不应该在当前方法内部去主动获取 SourceDataManager 中的 SourceDatas 数据。
        ///// <summary>
        ///// 当前方法（A）的功能与它的重载方法（B）相同，区别在于当前方法 A 是在方法内部通过前 4 个参数间接地计算得出方法 B 中的 historyWinningNumber 参数的数据，然后再进行方法 B 的后续流程。
        ///// 简单来说就是，B 中的 historyWinningNumber 拆分成了 A 中的 4 个参数，通过 4 个参数重新求得 B 中的 historyWinningNumber 参数的数据。
        ///// </summary>
        ///// <param name="historySourceDataStartIndex">源数据列表中将要开始获取的元素（或是该元素的下一元素，具体由 exceptStartSourceData 参数决定）的索引值，从此索引值指定的 SourceData 对象（的下一个对象，具体有 exceptStartSourceData 参数决定）开始获取。</param>
        ///// <param name="historySourceDataRangeCount">所要获取的 SourceData 对象的个数。（个数已包含 historySourceDataStartIndex 参数指向的 SourceData 对象）</param>
        ///// <param name="getAsMuchAsPossibleIfHistorySourceDataNotEnougth">一个布尔值，用来指示如果当前对象的源数据列表中满足条件的源数据对象数量没有达到指定要获取的数量时，是否尽可能多地获取（即：不一定非得要满足指定的数量要求，尽管不够也有多少获取多少），如果是则为 true，否则为 false。</param>
        ///// <param name="exceptStartSourceData">
        ///// 一个布尔值，用来指示所要获取的 SourceData 对象集合中是否不包含 historySourceDataStartIndex 指定的对象，如果是则为 true，否则为 false。
        ///// 注意！！！该布尔值不会影响 historySourceDataRangeCount 参数，即无论包不包含 historySourceDataStartIndex 指定的对象，理论上要获取的个数都是 historySourceDataRangeCount 个。
        ///// </param>
        ///// <param name="dingMaCount">要经过筛选后最后保留的定码（前二码/后二码）个数。</param>
        ///// <param name="forecastType">定码的类型（前二码/后二码）。</param>
        ///// <returns>返回 “计划预测” 列表。
        ///// 如果 historyWinningNumber 为 null 则抛出异常；
        ///// 如果 historyWinningNumber.Count 为 0，则直接返回 null。</returns>
        //private List<string> CalPlanForecastList(int historySourceDataStartIndex, int historySourceDataRangeCount, bool getAsMuchAsPossibleIfHistorySourceDataNotEnougth, bool exceptStartSourceData, int dingMaCount, ForecastType forecastType)
        //{


        //    List<SourceData> historyData = mgr.GetTopRangeHistorySourceData(historySourceDataStartIndex, this.currentSelectedNewestPeriod, true, );
        //    int historyCount = historyData.Count;
        //    List<string> planForecastNumberList = CalPlanForecastList(historyData.ExtractWinningNumber(), this.CurrentSelectedDingMaCount, this.CurrentSelectedForecastType);

        //    if (planForecastNumberList == null || planForecastNumberList.Count != this.currentSelectedDingMaCount)
        //    {
        //        throw new ApplicationException("所计算出的即将要开奖的未来一期 “预测列表” 数据个数和设置值不一致！");
        //    }
        //    //在保证了所有计算都成功了之后才把 historyCount 赋值给输出参数。
        //    actuallyHistoryWinningNumberCount = historyCount;
        //    return planForecastNumberList;
        //}
        #endregion
        
        #region GetRandoms() 方法，已没用，已被下面的 “RandomSelectSomeItems()”、“RemoveSpecifiedItems()” 和 “GetFullForecastNumberList()” 方法替代。
        ///// <summary>
        ///// 在一个以 "00"-"99" 为元素的新集合中排除掉已在泛型列表 itemsWaitForExclusion 中存在的所有元素后，从剩余的元素集合中随机抽取指定的 count 个元素组成新的泛型列表并返回。
        ///// </summary>
        ///// <param name="itemsWaitForExclusion">将要在新集合中排除掉的所有元素组成的泛型列表。</param>
        ///// <param name="count">将要最终保留的元素个数。</param>
        ///// <returns></returns>
        //private IEnumerable<string> GetRandoms(IEnumerable<string> itemsWaitForExclusion, int count)
        //{
        //    if (itemsWaitForExclusion == null || itemsWaitForExclusion.Count() < 1)
        //    {
        //        throw new ArgumentNullException("itemsWaitForExclusion");
        //    }

        //    if (count < 1)
        //    {
        //        throw new ArgumentOutOfRangeException("count", "要求保留的元素个数不能少于 1 个！");
        //    }

        //    int remainingItemCount = 100 - itemsWaitForExclusion.Count();
        //    if (remainingItemCount < count)
        //    {
        //        throw new ApplicationException("要求保留的元素数不能多于所能提供的剩余定码（前二码/后二码）的总个数！");
        //    }

        //    List<string> forecastNumbers = new List<string>()
        //    {
        //        "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
        //        "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
        //        "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
        //        "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
        //        "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
        //        "50", "51", "52", "53", "54", "55", "56", "57", "58", "59",
        //        "60", "61", "62", "63", "64", "65", "66", "67", "68", "69",
        //        "70", "71", "72", "73", "74", "75", "76", "77", "78", "79",
        //        "80", "81", "82", "83", "84", "85", "86", "87", "88", "89",
        //        "90", "91", "92", "93", "94", "95", "96", "97", "98", "99",
        //    };

        //    IEnumerable<string> waitForRemoveItems = itemsWaitForExclusion.Distinct();
        //    forecastNumbers.RemoveAll(n => waitForRemoveItems.Contains(n));

        //    Random r = new Random();
        //    while (forecastNumbers.Count() > count)
        //    {
        //        forecastNumbers.RemoveAt(r.Next(0, forecastNumbers.Count));
        //    }
        //    return forecastNumbers;
        //}
        #endregion

        #region 将上面的 GetRandoms() 方法拆分成这几个方法，以降低耦合度。
        /// <summary>
        /// 从 fullItems 集合中删除掉在 itemsWaitForExclusion 集合中存在的所有元素。
        /// </summary>
        /// <param name="fullItems">将要从中删除元素的集合。</param>
        /// <param name="itemsWaitForExclusion">将要在 fullItems 集合中排除的所有元素组成的泛型集合。</param>
        /// <returns>fullItems 集合中被删除掉的元素总数。</returns>
        private int RemoveSpecifiedItems(List<string> fullItems, IEnumerable<string> itemsWaitForExclusion)
        {
            if (fullItems == null || fullItems.Count() < 1)
            {
                throw new ArgumentNullException("fullItems");
            }
            if (itemsWaitForExclusion == null || itemsWaitForExclusion.Count() < 1)
            {
                #region 究竟是抛出异常还是正常返回，有待商议。
                //throw new ArgumentNullException("itemsWaitForExclusion");
                return 0;
                #endregion
            }

            //从 fullItems 集合中删除在 itemsWaitForExclusion 集合中存在的元素。
            return fullItems.RemoveAll(i => itemsWaitForExclusion.Contains(i));
        }

        /// <summary>
        /// 获取没有进行过任何筛选操作的完整的定码（前二码/后二码）“预测列表” 可选值集合。
        /// </summary>
        /// <returns></returns>
        private List<string> GetFullForecastNumberList()
        {
            List<string> forecastNumbers = new List<string>()
            {
                "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
                "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
                "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
                "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
                "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
                "50", "51", "52", "53", "54", "55", "56", "57", "58", "59",
                "60", "61", "62", "63", "64", "65", "66", "67", "68", "69",
                "70", "71", "72", "73", "74", "75", "76", "77", "78", "79",
                "80", "81", "82", "83", "84", "85", "86", "87", "88", "89",
                "90", "91", "92", "93", "94", "95", "96", "97", "98", "99",
            };
            return forecastNumbers;
        }

        /// <summary>
        /// 从传入的元素集合 fullItems 中随机选择出 count 个元素，并组成新的泛型列表来返回。
        /// </summary>
        /// <param name="fullItems">将要从中随机选择元素的泛型集合。</param>
        /// <param name="count">最终要保留的元素个数。</param>
        /// <returns></returns>
        private IEnumerable<string> RandomSelectSomeItems(IEnumerable<string> fullItems, int count)
        {
            if (fullItems == null || fullItems.Count() < 1)
            {
                throw new ArgumentNullException("fullItems");
            }
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException("count", "要求保留的元素个数不能少于 1 个！");
            }
            if (count > fullItems.Count())
            {
                throw new ArgumentOutOfRangeException("count", "要求保留的元素个数不能超过源集合的元素总数！");
            }

            List<string> remainItems = fullItems.Clone().ToList();// new List<string>(fullItems);
            Random r = new Random();
            while (remainItems.Count() > count)
            {
                remainItems.RemoveAt(r.Next(0, remainItems.Count));
            }
            return remainItems;
        }
        #endregion

        #endregion












        #region 重写父类的相关实现。
        public object Clone()
        {
            #region 旧方法。
            WinLog tmpLog = new WinLog(this.sourceData);
            tmpLog.firstTwoPlanForecast = new List<string>(this.firstTwoPlanForecast);
            tmpLog.lastTwoPlanForecast = new List<string>(this.lastTwoPlanForecast);
            tmpLog.isWin = this.isWin;
            tmpLog.currentForecastType = this.currentForecastType;
            tmpLog.actuallyHistoryWinningNumberCount = this.actuallyHistoryWinningNumberCount;
            return tmpLog;
            #endregion

            #region 新方法，BinaryClone() 方法有问题，List<> 列表没有实现 Serialize 特性，不能序列化。
            //return this.BinaryClone();
            #endregion
        }

        public override int GetHashCode()
        {
            return this.TimeId;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is WinLog)
            {
                WinLog that = obj as WinLog;
                if (this.sourceData == that.sourceData &&
                    this.isWin == that.isWin &&
                    this.currentForecastType == that.currentForecastType &&
                    IsStringListEquals(this.firstTwoPlanForecast, that.firstTwoPlanForecast) &&
                    IsStringListEquals(this.lastTwoPlanForecast, that.lastTwoPlanForecast))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsStringListEquals(List<string> listA, List<string> listB)
        {
            if (listA == null && listB == null)
            {
                return true;
            }
            //在前面排除了 A 和 B 都为 null 的情况之后，剩下的就只有 “仅有 A 为 null”、“仅有 B 为 null” 和 “A 和 B 都不为 null” 这三种情况，
            //所以剩下的情况中，只要 A 为 null，B 就肯定不为 null，反之也一样，所以只要是 A 为 null 或是 B 为 null，结果都为 false。
            if (listA == null || listB == null)
            {
                return false;
            }
            else    //剩下的就只有 A 和 B 都不为 null 了，在此可以直接判断里面的所有元素的值是否相等了。
            {
                if (listA.Count != listB.Count)
                {
                    return false;
                }
                for (int i = 0; i < listA.Count; i++)
                {
                    if (listA[i] != listB[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        #endregion
    }
}
