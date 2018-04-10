using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GS = Core.GlobalSettings;


namespace Core
{
    #region 事件委托类型定义。
    public delegate void WinLogAddedHandler(UIDataManager sender, WinLog newLog, SourceData sourceData, ReadOnlyCollection<WinLog> allLogs, int oldLogsCount);

    public delegate void NewestPeriodChangingHandler(UIDataManager sender, int oldValue, int newValue, int maxValidValue, int minValidValue);
    public delegate void NewestPeriodChangedHandler(UIDataManager sender, int oldValue, int newValue, int maxValidValue, int minValidValue);

    public delegate void DingMaCountChangingHandler(UIDataManager sender, int oldValue, int newValue, int maxValidValue, int minValidValue);
    public delegate void DingMaCountChangedHandler(UIDataManager sender, int oldValue, int newValue, int maxValidValue, int minValidValue);

    public delegate void ForecastTypeChangingHandler(UIDataManager sender, ForecastType oldValue, ForecastType newValue);
    public delegate void ForecastTypeChangedHandler(UIDataManager sender, ForecastType oldValue, ForecastType newValue);
    #endregion


    public class UIDataManager
    {
        #region 事件声明。
        public event WinLogAddedHandler OnNewWinLogAdded;

        public event NewestPeriodChangingHandler OnNewestPeriodChanging;
        public event NewestPeriodChangedHandler OnNewestPeriodChanged;

        public event DingMaCountChangingHandler OnDingMaCountChanging;
        public event DingMaCountChangedHandler OnDingMaCountChanged;

        public event ForecastTypeChangingHandler OnForecastTypeChanging;
        public event ForecastTypeChangedHandler OnForecastTypeChanged;
        #endregion

        #region 预定义常量。
        ///// <summary>
        ///// 保留定码（前二码/后二码）个数的最大值。
        ///// </summary>
        //public const int MAX_DING_MA_COUNT = 80;
        ///// <summary>
        ///// 保留定码（前二码/后二码）个数的最小值。
        ///// </summary>
        //public const int MIN_DING_MA_COUNT = 1;
        ///// <summary>
        ///// 排除最新期数号的最大值。
        ///// </summary>
        //public const int MAX_NEWEST_PERIOD_COUNT = 100;
        ///// <summary>
        ///// 排除最新期数号的最小值。
        ///// </summary>
        //public const int MIN_NEWEST_PERIOD_COUNT = 0;


        //public const int DEFAULT_NEWEST_PERIOD_COUNT = 30;

        //public const int DEFAULT_DING_MA_COUNT = 70;

        //public const ForecastType DEFAULT_FORECAST_TYPE = ForecastType.LastTwo;
        #endregion

        #region 字段和属性。
        private static readonly List<WinLog> winLogs = new List<WinLog>();
        //SourceDataManager 类型对象 mgr 的初始化必须要在 singleObject 对象的初始化之前，因为 singleObject 对象调用的本类构造函数中使用了 mgr 对象。
        private static readonly SourceDataManager mgr = SourceDataManager.GetInstance();
        private static readonly UIDataManager singleObject = new UIDataManager();

        private static readonly object syncObj = new object();
        private static WinLog nextNewestLog;
        //private static int nextNewestLogActuallyHistoryWinningNumberCount;

        /// <summary>
        /// 用来记录当前的 winLogs 集合中时间最新（时间最新并不一定是最后添加）的 TimeId 值。
        /// </summary>
        private static int newestTimeId = -1;

        public ReadOnlyCollection<WinLog> WinLogs
        {
            get
            {
                return winLogs.AsReadOnly();
            }
            //private set;

        }


        private int currentSelectedNewestPeriod;
        /// <summary>
        /// 当前用户设定的 “排除最新期数号”。
        /// </summary>
        public int CurrentSelectedNewestPeriod
        {
            get
            {
                return this.currentSelectedNewestPeriod;
            }
            private set
            {
                if (value < GS.MIN_NEWEST_PERIOD_COUNT || value > GS.MAX_NEWEST_PERIOD_COUNT)
                {
                    throw new ArgumentOutOfRangeException("CurrentSelectedNewestPeriod");
                }
                int oldValue = this.currentSelectedNewestPeriod;
                if (OnNewestPeriodChanging != null)
                {
                    OnNewestPeriodChanging(this, oldValue, value, GS.MAX_NEWEST_PERIOD_COUNT, GS.MIN_NEWEST_PERIOD_COUNT);
                }
                this.currentSelectedNewestPeriod = value;
                if (OnNewestPeriodChanged != null)
                {
                    OnNewestPeriodChanged(this, oldValue, value, GS.MAX_NEWEST_PERIOD_COUNT, GS.MIN_NEWEST_PERIOD_COUNT);
                }
            }
        }


        private ForecastType currentSelectedForecastType;
        /// <summary>
        /// 当前用户选择的 “定码类型（前二码/后二码）”。
        /// </summary>
        public ForecastType CurrentSelectedForecastType
        {
            get
            {
                return currentSelectedForecastType;
            }
            private set
            {
                ForecastType oldValue = this.currentSelectedForecastType;
                if (OnForecastTypeChanging != null)
                {
                    OnForecastTypeChanging(this, oldValue, value);
                }
                this.currentSelectedForecastType = value;
                if (OnForecastTypeChanged != null)
                {
                    OnForecastTypeChanged(this, oldValue, value);
                }
            }
        }


        private int currentSelectedDingMaCount;
        /// <summary>
        /// 当前用户设定的 “定码个数”。
        /// </summary>
        public int CurrentSelectedDingMaCount
        {
            get
            {
                return this.currentSelectedDingMaCount;
            }
            private set
            {
                if (value < GS.MIN_DING_MA_COUNT || value > GS.MAX_DING_MA_COUNT)
                {
                    throw new ArgumentOutOfRangeException("CurrentSelectedDingMaCount");
                }
                int oldValue = this.currentSelectedDingMaCount;
                if (OnDingMaCountChanging != null)
                {
                    OnDingMaCountChanging(this, oldValue, value, GS.MAX_DING_MA_COUNT, GS.MIN_DING_MA_COUNT);
                }
                this.currentSelectedDingMaCount = value;
                if (OnDingMaCountChanged != null)
                {
                    OnDingMaCountChanged(this, oldValue, value, GS.MAX_DING_MA_COUNT, GS.MIN_DING_MA_COUNT);
                }
            }
        }



        /// <summary>
        /// 当前的 “最新在线人数/分钟” 值
        /// </summary>
        public Number CurrentOnlineNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// 当前的最新 “开奖号码” 值。
        /// </summary>
        public Number CurrentWinningNumber
        {
            get;
            private set;
        }



        /// <summary>
        /// 当前最新的中奖总次数。
        /// </summary>
        public int TotalWin
        {
            get;
            private set;
        }

        /// <summary>
        /// 当前最新的没中奖的总次数。
        /// </summary>
        public int TotalLose
        {
            get;
            private set;
        }



        /// <summary>
        /// 当前最新的最大连续中奖总次数。
        /// </summary>
        public int MaxContinueWinCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 当前最新的连续没中奖的总次数。
        /// </summary>
        public int MaxContinueLoseCount
        {
            get;
            private set;
        }
        #endregion







        private UIDataManager()
        {
            //最新在线人数
            this.CurrentOnlineNumber = null;
            //开奖号码
            this.CurrentWinningNumber = null;

            #region 由于所赋的值都能保证其合法性，因此可以直接为字段而不是属性赋值，减少校验代码的性能损耗。
            //定码类型
            this.currentSelectedForecastType = GS.DEFAULT_FORECAST_TYPE;
            //排除最新期数号
            this.currentSelectedNewestPeriod = GS.DEFAULT_NEWEST_PERIOD_COUNT;
            //定码个数
            this.currentSelectedDingMaCount = GS.DEFAULT_DING_MA_COUNT;
            #endregion

            //当前计划状态（“最大连中”、“最大连挂”）
            CalMaxContinueWinAndMaxContinueLose();
            //当前计划状态（“中奖期数”、“没中奖期数”）
            CalTotalWinAndTotalLose();


            #region 创建下一轮要用的 WinLog，并填充当前可以准备的所有预测数据。
            //List<string> planForecastNumberList = CalPlanForecastList(new List<Number>(), this.currentSelectedDingMaCount, this.currentSelectedForecastType);
            //nextNewestLog = new WinLog(planForecastNumberList, 0, this.currentSelectedForecastType);

            List<Number> historyData = mgr.GetTopRangeHistorySourceData(0, this.currentSelectedNewestPeriod, true, false).ExtractWinningNumber();
            nextNewestLog = new WinLog(historyData, this.currentSelectedDingMaCount, this.currentSelectedForecastType);
            //nextNewestLogActuallyHistoryWinningNumberCount = historyData.Count;
            #endregion


            //绑定事件
            mgr.OnNewSourceDataAdded += Mgr_OnNewSourceDataAdded;
        }

        #region 旧版，暂时没啥用。
        //private void Mgr_OnNewSourceDataAdded1(SourceData newData, int oldDataCount)
        //{
        //    #region 创建 WinLog 对象。
        //    //获取除了用来创建当前的 log 对象时使用的 newData 对象之外的 this.CurrentSelectedNewestPeriods 个最新的历史 SourceData 对象。
        //    List<SourceData> historyData = mgr.GetTopRangeHistorySourceDataExceptStartObject(newData.TimeId, this.currentSelectedNewestPeriod, true).ToList();
        //    List<string> planForecastNumberList = CalPlanForecastList(historyData.ExtractWinningNumber(), this.currentSelectedDingMaCount, this.currentSelectedForecastType);
        //    WinLog log = new WinLog(planForecastNumberList, this.currentSelectedForecastType, newData);
        //    #endregion

        //    winLogs.Add(log);
        //    SortWinLogsByTimeId(OrderType.Descending);


        //    nextNewestLog = log;
        //    #region 设置 “最新在线人数” 和 “最新的开奖号码” 值。
        //    ////由于最新添加到当前的源数据 newData 对象并不确保是时间上也是最新的，所以设置 “最新在线人数” 和 “最新的开奖号码” 不应该使用该对象的信息。
        //    //this.CurrentOnlineNumber = newData.OnlineNumber.ToNumber();
        //    //this.CurrentWinningNumber = newData.WinningNumber;

        //    //通过获取在本地 “数据池” 中最新的一个源数据对象 newestData 来设置 “最新在线人数” 和 “最新的开奖号码” 的值，这样就能保证信息是 “最新” 的。
        //    //如果源数据对象为空，则不做设置操作。
        //    SourceData newestData = mgr.GetTopNSourceData(1, true).SingleOrDefault();
        //    if (newestData != null)
        //    {
        //        this.CurrentOnlineNumber = newestData.OnlineNumber.ToNumber();
        //        this.CurrentWinningNumber = newestData.WinningNumber;
        //    }
        //    #endregion

        //    if (log.IsWin.Value)
        //    {
        //        this.TotalWin++;
        //    }
        //    else
        //    {
        //        this.TotalLose++;
        //    }

        //    //计算 “最大连中” 和 “最大连挂” 并设值。
        //    CalMaxContinueWinAndMaxContinueLose();

        //    if (this.OnNewWinLogAdded != null)
        //    {
        //        this.OnNewWinLogAdded(this, log, newData, this.WinLogs, winLogs.Count);
        //    }
        //}
        #endregion

        private void Mgr_OnNewSourceDataAdded(SourceData newData, int oldDataCount)
        {
            #region 填充上一轮已经预创建好的 WinLog 对象。
            lock (syncObj)
            {
                //填充上一轮已经预创建好的 WinLog 对象。
                nextNewestLog.FillResult(newData);
            }
            #endregion

            WinLog currentLog = nextNewestLog;

            //因为由于业务的需要， winLogs 集合默认是按 TimeId 从新到旧的顺序来排序的，即排在最前的是最新的，而大多数情况下新添加的就是 TimeId 值最新的，所以
            //用 Insert() 来代替 Add() 会比较节省性能。
            winLogs.Insert(0, currentLog);
            
            #region 通过一定的判断逻辑来尽量减少排序的操作次数，从而提高性能。
            int currentTimeId = currentLog.TimeId;
            //如果 newestTimeId 还没有值（初始值 -1 不算），则表示从来没有添加过 WinLog 元素。
            if (newestTimeId == -1)
            {
                newestTimeId = currentTimeId;
            }
            else
            {
                //如果当前添加的元素 currentLog 在列表集合 winLogs 中不是最新的，则进行排序。
                if (newestTimeId > currentTimeId)
                {
                    SortWinLogsByTimeIdDescending();
                }
                //更新 newestTimeId 的值。
                newestTimeId = Math.Max(newestTimeId, currentTimeId);
            }
            #endregion
            

            #region 创建下一轮要用的 WinLog，并填充当前可以准备的所有预测数据。
            ////获取 this.currentSelectedNewestPeriods 个最新的历史 SourceData 对象。
            List<SourceData> historyData = mgr.GetTopRangeHistorySourceData(0, this.currentSelectedNewestPeriod, true, false);

            lock (syncObj)
            {
                nextNewestLog = new WinLog(historyData.ExtractWinningNumber(), this.currentSelectedDingMaCount, this.currentSelectedForecastType);
                //nextNewestLogActuallyHistoryWinningNumberCount = historyData.Count;
            }
            #endregion



            #region 设置 “最新在线人数” 和 “最新的开奖号码” 值。
            ////由于最新添加到当前的源数据 newData 对象并不确保是时间上也是最新的，所以设置 “最新在线人数” 和 “最新的开奖号码” 不应该使用该对象的信息。
            //this.CurrentOnlineNumber = newData.OnlineNumber.ToNumber();
            //this.CurrentWinningNumber = newData.WinningNumber;

            //通过获取在本地 “数据池” 中最新的一个源数据对象 newestData 来设置 “最新在线人数” 和 “最新的开奖号码” 的值，这样就能保证信息是 “最新” 的。
            //如果源数据对象为空，则不做设置操作。
            SourceData newestData = mgr.GetTopRangeHistorySourceData(0, 1, false, false).SingleOrDefault();
            if (newestData != null)
            {
                this.CurrentOnlineNumber = newestData.OnlineNumber.ToNumber();
                this.CurrentWinningNumber = newestData.WinningNumber;
            }
            #endregion

            #region 计算 “中奖总期数”、“没中奖总期数”、“最大连中”、“最大连挂”。
            if (currentLog.IsWin.Value)
            {
                this.TotalWin++;
            }
            else
            {
                this.TotalLose++;
            }

            //计算 “最大连中” 和 “最大连挂” 并设值。
            CalMaxContinueWinAndMaxContinueLose();
            #endregion

            if (this.OnNewWinLogAdded != null)
            {
                this.OnNewWinLogAdded(this, currentLog, newData, this.WinLogs, winLogs.Count);
            }
        }



        public void StartRepeatGetNewData()
        {
            mgr.StartRepeatDownload();
        }


        #region 更新多个或单个选项时更新全部历史中奖纪录数据。
        public void UpdateSelectedState(int newNewestPeriod = GS.DEFAULT_NEWEST_PERIOD_COUNT, int newDingMaCount = GS.DEFAULT_DING_MA_COUNT, ForecastType newForecastType = GS.DEFAULT_FORECAST_TYPE)
        {
            if (this.currentSelectedNewestPeriod == newNewestPeriod && this.currentSelectedDingMaCount == newDingMaCount && this.currentSelectedForecastType == newForecastType)
            {
                return;
            }

            #region 校验 “排除最新期数号” 和 “定码个数” 值的合法性。
            if (newNewestPeriod < GS.MIN_NEWEST_PERIOD_COUNT || newNewestPeriod > GS.MAX_NEWEST_PERIOD_COUNT)
            {
                throw new ApplicationException("“排除最新期数号” 的值超出允许的最大范围（" + GS.MIN_NEWEST_PERIOD_COUNT + "-" + GS.MAX_NEWEST_PERIOD_COUNT + "）");
            }
            if (newDingMaCount < GS.MIN_DING_MA_COUNT || newDingMaCount > GS.MAX_DING_MA_COUNT)
            {
                throw new ApplicationException("“定码个数” 的值超出允许的最大范围（" + GS.MIN_DING_MA_COUNT + "-" + GS.MAX_DING_MA_COUNT + "）");
            }
            if (newNewestPeriod + newDingMaCount > 100)
            {

                throw new ArgumentOutOfRangeException("newDingMaCount", "所设置的 “排除最新期数号” 和 “定码个数” 两个数的和不能大于 100！请重新设置！");
            }



            #endregion

            this.currentSelectedNewestPeriod = newNewestPeriod;
            this.currentSelectedDingMaCount = newDingMaCount;
            this.currentSelectedForecastType = newForecastType;

            //int actuallyCount;
            lock (syncObj)
            {
                List<SourceData> historySourceDatas = mgr.GetTopRangeHistorySourceData(0, this.currentSelectedNewestPeriod, true, false);
                nextNewestLog.FillForecast(historySourceDatas.ExtractWinningNumber(), this.currentSelectedDingMaCount, this.currentSelectedForecastType);
                //nextNewestLog.FillForecast(CalNextPlanForecastList(out actuallyCount), actuallyCount, this.currentSelectedForecastType);
            }

            UpdateAllWinLogs();
            CalTotalWinAndTotalLose();
            CalMaxContinueWinAndMaxContinueLose();
        }

        public void UpdateDingMaCount(int newDingMaCount)
        {
            UpdateSelectedState(this.currentSelectedNewestPeriod, newDingMaCount, this.currentSelectedForecastType);
        }

        public void UpdateNewestPeriods(int newNewestPeriods)
        {
            UpdateSelectedState(newNewestPeriods, this.currentSelectedDingMaCount, this.currentSelectedForecastType);
        }

        public void UpdateForecastType(ForecastType newForecastType)
        {
            UpdateSelectedState(this.currentSelectedNewestPeriod, this.currentSelectedDingMaCount, newForecastType);
        }
        #endregion


        /// <summary>
        /// 根据当前设置的 “排除最新期数号”（this.CurrentSelectedNewestPeriods）、“定码个数”（this.CurrentSelectedDingMaCount） 和 “定码类型”（this.CurrentSelectedForecastType）来重新计算所有中奖纪录对象的相关属性数据。
        /// </summary>
        public void UpdateAllWinLogs()
        {
            WinLog tmpLog;
            List<SourceData> historyData;
            int sourceDataIndex;
            for (int i = 0; i < winLogs.Count; i++)
            {
                tmpLog = winLogs[i];
                sourceDataIndex = mgr.IndexOf(tmpLog.TimeId);
                historyData = mgr.GetTopRangeHistorySourceData(sourceDataIndex, this.currentSelectedNewestPeriod, true, true);
                tmpLog.FillForecast(historyData.ExtractWinningNumber(), this.currentSelectedDingMaCount, this.currentSelectedForecastType);
            }
        }


        #region 多种计算 “最大连中”、“最大连挂” 的算法思路。

        #region 感觉相对来说思路较清晰的写法，正确性有待验证。
        /// <summary>
        /// 根据当前对象中的 “中奖纪录” 集合 winLogs 来从头开始计算出 “最大连中” 和 “最大连挂” 值，并赋值给相应的 “this.MaxContinueWinCount” 和 “this.MaxContinueNotWinCount” 属性。
        /// </summary>
        private void CalMaxContinueWinAndMaxContinueLose()
        {
            //如果当前对象中的 “中奖纪录” 集合 winLogs 没有任何数据，则说明没有任何 “最大连中” 和 “最大连挂” 数据，所以直接把 “最大连中” 和 “最大连挂” 值都初始化为 0 后返回。
            if (winLogs == null || winLogs.Count <= 0)
            {
                this.MaxContinueWinCount = 0;
                this.MaxContinueLoseCount = 0;
                return;
            }

            // 在当前对象的中奖纪录集合 winLogs 中提取出所有的 “是否中奖” 信息记录。
            List<bool> isWinLogList = (from h in winLogs
                                       orderby h.TimeId descending      //有待确认排序方向。
                                       select h.IsWin.Value).ToList();

            IEnumerable<bool> tmpWinState = isWinLogList;
            List<int> continueWinLog = new List<int>();     //连续中奖纪录集合。
            List<int> continueLoseLog = new List<int>();    //连续不中奖记录集合。
            bool isContinueWin;
            int tmpContinueWinOrLoseCount = 0;  //用来记录当前一轮的连续中奖或连续不中奖的次数。
            int tmpTotalSkipCount = 0;          //用来记录集合中已经统计并处理过的中奖状态元素个数。
            do
            {
                //获取第一个元素的中奖状态，以这个元素的状态为连续中奖（或连续不中奖）的 “连续开始点”，用来在下面的 TakeWhile() 中轮询判断。
                isContinueWin = tmpWinState.ToList()[tmpTotalSkipCount];
                //获取当前一轮的连续中奖（或不中奖）次数，至于状态是中奖还是不中奖则由 isLastStateWin 变量的值来确定。
                tmpContinueWinOrLoseCount = tmpWinState.Skip(tmpTotalSkipCount).TakeWhile(s => s == isContinueWin).Count();
                //把当前一轮统计出的连续中奖次数（或是连续不中奖次数）根据 isContinueWin 表示的 “中奖状态” 是 “中奖” 还是 “不中奖” 来累计到相对应的 “连续中奖记录集合” 或 “连续不中奖记录集合” 中。
                if (isContinueWin)
                {
                    continueWinLog.Add(tmpContinueWinOrLoseCount);
                }
                else
                {
                    continueLoseLog.Add(tmpContinueWinOrLoseCount);
                }
                //把本轮统计完的元素个数累加到 tmpTotalSkipCount 变量中，以让下一轮循环时会跳过这些已经统计过的元素。
                tmpTotalSkipCount += tmpContinueWinOrLoseCount;
            } while (tmpTotalSkipCount < tmpWinState.Count()); //如果已经统计过的元素总数 tmpTotalSkipCount 和集合 tmpWinState 中的元素总数一致，则表示统计处理完成。
            // 在 “连续中奖纪录集合” 中取出最大值（即 “最大连中” 值）并赋值给 this.MaxContinueWinCount 属性。
            this.MaxContinueWinCount = continueWinLog.Count > 0 ? continueWinLog.Max() : 0;
            // 在 “连续不中奖纪录集合” 中取出最大值（即 “最大连挂” 值）并赋值给 this.MaxContinueNotWinCount 属性。
            this.MaxContinueLoseCount = continueLoseLog.Count > 0 ? continueLoseLog.Max() : 0;
        }
        #endregion

        #region 相对来说思路没有那么清晰，对每个环节都要严密把关，十分考验思维的慎密度。
        //用最原始的思路，逐个遍历来统计，正确性有待验证。
        private void CalMaxContinueWinAndMaxContinueLose2()
        {
            List<bool> isWinLogList = (from h in winLogs
                                       orderby h.TimeId descending      //有待确认排序方向。
                                       select h.IsWin.Value).ToList();
            int lastContinueWinCount = 0, lastContinueLoseCount = 0, maxContinueWinCount = 0, maxContinueLoseCount = 0;
            bool? isLastStateWin = null;

            foreach (bool isWin in isWinLogList)
            {
                //如果前一次的中奖状态值为空，则把本次的中奖状态作为下一轮的 “前一次的中奖状态” 值来赋值给 isLastStateWin 变量，同时也根据本次的中奖状态来做好相应的 “中奖次数” 或 “没中奖次数” 记录，以供下一次使用。
                if (isLastStateWin == null)
                {
                    isLastStateWin = isWin;
                    if (isWin)
                    {
                        lastContinueWinCount++;
                    }
                    else
                    {
                        lastContinueLoseCount++;
                    }
                    continue;
                }

                //如果是连续中或是连续不中
                if (isWin == isLastStateWin)
                {
                    //如果是连续中
                    if (isWin)
                    {
                        lastContinueWinCount++;
                    }
                    else    //否则如果是连续不中
                    {
                        lastContinueLoseCount++;
                    }
                }
                else    //否则如果不是连续（中或不中），则重置相关数据。
                {
                    //如果是前一次中（也就是本次不中），则把之前中奖的次数记录清零，因为连续中奖的情况已经被本次 “打断” 了。
                    //在把中奖次数记录清零之前，先和 “最大连续中奖次数记录” 进行比较，取其两者的最大值来保存，以记录 “到目前为止的最大连中次数” 值。
                    if (isLastStateWin.Value)
                    {
                        maxContinueWinCount = Math.Max(maxContinueWinCount, lastContinueWinCount);
                        lastContinueWinCount = 0;
                        lastContinueLoseCount++;
                    }
                    //如果是前一次不中（也就是本次中），则把之前没有中奖的次数记录清零，因为连续没有中奖的情况已经被本次 “打断” 了。
                    //在把没有中奖的次数记录清零之前，先和 “最大连续没有中奖的次数记录” 进行比较，取其两者的最大值来保存，以记录 “到目前为止的最大连挂次数” 值。
                    else
                    {
                        maxContinueLoseCount = Math.Max(maxContinueLoseCount, lastContinueLoseCount);
                        lastContinueLoseCount = 0;
                        lastContinueWinCount++;
                    }
                }
                isLastStateWin = isWin;
            }
            this.MaxContinueWinCount = maxContinueWinCount;
            this.MaxContinueLoseCount = maxContinueLoseCount;
        }


        //通过 SkipWhile 来实现统计，正确性有待验证。
        private void CalMaxContinueWinAndMaxContinueLose1()
        {
            List<bool> isWinLogList = (from h in winLogs
                                       orderby h.TimeId descending      //有待确认排序方向。
                                       select h.IsWin.Value).ToList();
            #region 方式一，正确性待确定。
            //int lastContinueWinCount = 0, lastContinueLoseCount = 0, maxContinueWinCount = 0, maxContinueLoseCount = 0, tmpRemainCount, totalCount;

            //totalCount = isWinLogList.Count;
            //IEnumerable<bool> tmpHistoryWinState = isWinLogList;
            //bool isLastStateWin = tmpHistoryWinState.ToList()[0];
            //// 1. while 头中的语句 “tmpHistoryWinState = tmpHistoryWinState.SkipWhile(n => n == isLastStateWin)” 表示从第一个元素开始，跳过元素值与变量 isLastStateWin 相等的若干个连续元素。
            //// 2. 语句 “(tmpHistoryWinState = tmpHistoryWinState.SkipWhile(n => n == isLastStateWin)).Count()” 表示获取跳过所有满足条件的元素之后，剩余的元素个数。
            //// 3. 语句 “tmpRemainCount = (tmpHistoryWinState = tmpHistoryWinState.SkipWhile(n => n == isLastStateWin)).Count()” 表示把第 2 行中返回的元素个数赋值给变量 tmpRemainCount。
            //while ((tmpRemainCount = (tmpHistoryWinState = tmpHistoryWinState.SkipWhile(n => n == isLastStateWin)).Count()) > 0)
            //{
            //    if (isLastStateWin)
            //    {
            //        maxContinueWinCount = Math.Max(maxContinueWinCount, lastContinueWinCount);
            //        lastContinueWinCount = totalCount - tmpRemainCount;
            //    }
            //    else
            //    {
            //        maxContinueLoseCount = Math.Max(maxContinueLoseCount, lastContinueLoseCount);
            //        lastContinueLoseCount = totalCount - tmpRemainCount;
            //    }

            //    #region 由于 bool 类型不是 true 就是 false，所以下面第一句可以写成 isLastStateWin = !isLastStateWin;
            //    //isLastStateWin = tmp.ToList()[0];
            //    isLastStateWin = !isLastStateWin;
            //    #endregion
            //}
            #endregion


            #region 方式二，也是有待验证。
            int totalCount = isWinLogList.Count;
            int lastContinueWinCount = 0, lastContinueLoseCount = 0, maxContinueWinCount = 0, maxContinueLoseCount = 0, tmpRemainCount;
            IEnumerable<bool> tmpHistoryWinState = isWinLogList;
            bool isLastStateWin;
            int tmpContinueCount = 0;   //用来临时存储刚记录完成的这一轮的连续中奖或连续没中奖的 “连续次数”。
            do
            {
                isLastStateWin = tmpHistoryWinState.ToList()[0];

                //从集合 tmpHistoryWinState 的第一个元素开始，跳过元素值与变量 isLastStateWin 相等的若干个连续元素。
                //也即：跳过了 “中奖状态连续不断且和 isLastStateWin 变量指定的中奖状态一致” 的多个元素，所以被跳过的这些元素总数即为 “连中数” 或 “连挂数”。
                tmpHistoryWinState = tmpHistoryWinState.SkipWhile(isWin => isWin == isLastStateWin);
                //记录经过了本轮 SkipWhile() 之后剩余的元素数。
                tmpRemainCount = tmpHistoryWinState.Count();
                //if(tmpRemainCount <= 0)     //防止第一次 SkipWhile() 之后就剩余 0 个元素（即跳过 tmpHistoryWinState 中的所有元素）的情况。
                //{
                //    break;
                //}
                tmpContinueCount = totalCount - tmpRemainCount;
                if (tmpContinueCount <= 0)
                {
                    throw new ApplicationException("获取连中数或连挂数出错！");
                }
                //如果检测到的是连续中奖
                if (isLastStateWin)
                {
                    maxContinueWinCount = Math.Max(maxContinueWinCount, lastContinueWinCount);
                    lastContinueWinCount = tmpContinueCount;
                }
                //否则，如果检测到的是连续没中奖
                else
                {
                    maxContinueLoseCount = Math.Max(maxContinueLoseCount, lastContinueLoseCount);
                    lastContinueLoseCount = tmpContinueCount;
                }

                #region 由于 bool 类型不是 true 就是 false，所以下面第一句可以写成 isLastStateWin = !isLastStateWin;
                //isLastStateWin = tmp.ToList()[0];
                isLastStateWin = !isLastStateWin;
                #endregion

            } while (tmpRemainCount > 0);
            #endregion
        }
        #endregion
        #endregion

        /// <summary>
        /// 根据当前对象中的 “中奖纪录” 集合 winLogs 来从头开始计算出 “中奖的总期数” 和 “不中奖的总期数” 值，并赋值给相应的 “this.MaxContinueWinCount” 和 “this.MaxContinueNotWinCount” 属性。
        /// </summary>
        private void CalTotalWinAndTotalLose()
        {
            this.TotalWin = 0;
            this.TotalLose = 0;
            //如果 “中奖纪录” 集合 winLogs 中没有任何数据，则直接返回。
            if (winLogs == null || winLogs.Count < 1)
            {
                return;
            }

            WinLog tmp;
            for (int i = 0; i < winLogs.Count; i++)
            {
                tmp = winLogs[i];
                if (tmp.IsWin.HasValue == false)
                {
                    continue;
                }
                if (tmp.IsWin.Value)
                {
                    this.TotalWin++;
                }
                else
                {
                    this.TotalLose++;
                }
            }
        }


        public static UIDataManager GetInstance()
        {
            return singleObject;
        }

        public List<WinLog> GetTopNWinLogs(int topCount)
        {
            if (topCount < 1)
            {
                throw new ArgumentOutOfRangeException("topCount", "所要获取的最新中奖纪录条数必须为正整数！");
            }
            if (winLogs.Count < 1)
            {
                return null;
            }
            if (winLogs.Count < topCount)
            {
                throw new ApplicationException("所要获取的最新中奖记录的数量超过软件中所收集到的历史中奖记录总数！");
            }
            SortWinLogsByTimeIdDescending();
            return winLogs.Take(topCount).ToList();
        }

        public WinLog GetWinLogByTimeId(int timeId)
        {
            return (from l in winLogs
                    where l.TimeId == timeId
                    select l).SingleOrDefault();
        }

        public int GetWinLogCountByTimeId(int timeId)
        {
            return winLogs.Count(l => l.TimeId == timeId);
        }

        /// <summary>
        /// 根据 TimeId 属性的值来对当前对象中的 WinLogs 集合进行降序排序。
        /// </summary>
        /// <param name="logs"></param>
        /// <param name="orderType"></param>
        public static void SortWinLogsByTimeIdDescending()
        {
            SortWinLogsByTimeId(winLogs, OrderType.Descending);
        }

        public static void SortWinLogsByTimeId(List<WinLog> logs, OrderType orderType = OrderType.Descending)
        {
            if (logs == null || logs.Count < 1)
            {
                throw new ArgumentNullException("logs");
            }
            if (logs.Count == 1)
            {
                return;
            }

            if (orderType == OrderType.Ascending)
            {
                logs.Sort((l1, l2) => l1.TimeId - l2.TimeId);
            }
            else
            {
                logs.Sort((l1, l2) => l2.TimeId - l1.TimeId);
            }
        }

        

        #region 功能需求。
        //× 从打开软件开始，最早15条的计划预测不作计算填充。

        //如果选中的 “排除最新期数号” 大于 “历史中奖纪录条数”，则弹出警告提示：“排除最新期数号的值最大只能选择历史中奖记录条数的最大值”
        //假如 “排除最新期数号” 为 10，则此时最早的 10 条之前的 “历史中奖纪录条数” 是不够 10 条的，但也按 “排除最新10期” 来处理，就是最多能排除多少期是多少期。


        //定码个数：
        //最多80个，（最少1个）
        //但是有个硬性前提：定码个数 + 排除最新期数号 <= 100

        //复制计划按钮：
        //复制将要开奖的计划预测。


        //一旦修改了 “前二码/后二码”、“排除最新期数号”、“定码个数” 中的任何一个，所有显示的历史记录都同时同步刷新结果，“计划预测” 中的结果集合也重新随机生成，而不是在原来的结果中再次筛选。
        //（初步预计应该写在 UIDataManager 类中）


        //“排除最新期数号” 的最大值不能超过当前 “历史中奖记录” 的总条数，也就是：
        //“排除最新期数号” 的最大值 <= 当前 “历史中奖记录” 的总条数


        //添加一个选项：用来设置打开软件之后等到后台获取到的 “历史中奖记录” 大于等于 15 条后才做计划预测操作。


        //
        #endregion
        
        
        
        
        
        
        public ReadOnlyCollection<string> GetNextPlanForecastList()
        {
            if (nextNewestLog.PlanForecast == null)
            {
                //int actuallyCount;
                //List<string> planForecastList = CalNextPlanForecastList(out actuallyCount);

                List<Number> winningNumberList = mgr.GetTopRangeHistorySourceData(0, this.currentSelectedNewestPeriod, true, false).ExtractWinningNumber();

                lock (syncObj)
                {
                    //nextNewestLog.FillForecast(planForecastList, actuallyCount, this.currentSelectedForecastType);
                    //nextNewestLogActuallyHistoryWinningNumberCount = actuallyCount;

                    nextNewestLog.FillForecast(winningNumberList, this.currentSelectedDingMaCount, this.currentSelectedForecastType);
                    //nextNewestLogActuallyHistoryWinningNumberCount = winningNumberList.Count;
                }
            }
            return nextNewestLog.PlanForecast;
        }



        #region 计算 “计划预测”，估计没用了。
        ///// <summary>
        ///// 根据传入的最新历史中奖号码集合（historyWinningNumber）、保留的定码个数（dingMaCount）和定码类型（forecastType）来计算出下一个即将公布的中奖号码的定码 “计划预测” 列表。
        ///// 如果 historyWinningNumber 为 null 则抛出异常，如果 historyWinningNumber 不为 null 但是一个空集合，则直接返回 null。
        ///// 注意：该方法目的是计算即将公布（但还没公布）的未来一期的 “计划预测” 值；
        ///// historyWinningNumber 参数的 Count 属性值其实就是界面上选择的 “排除最新期数号”；
        ///// dingMaCount 参数其实就是界面上选择的 “定码个数”；
        ///// forecastType 参数就是界面上选择的定码类型 “前二码/后二码”；
        ///// </summary>
        ///// <param name="historyWinningNumber">最新的某个期数范围的历史中奖号码的集合，期数范围由该 historyWinningNumber 集合的元素个数来确定，集合中的每个中奖号码之间没有顺序关系，中奖号码有可能会出现重复。</param>
        ///// <param name="dingMaCount">要经过筛选后最后保留的定码（前二码/后二码）个数。</param>
        ///// <param name="forecastType">定码的类型（前二码/后二码）。</param>
        ///// <returns>返回 “计划预测” 列表。
        ///// 如果 historyWinningNumber 为 null 则抛出异常；
        ///// 如果 historyWinningNumber.Count 为 0，则直接返回 null。</returns>
        //public List<string> CalPlanForecastList(List<Number> historyWinningNumber, int dingMaCount, ForecastType forecastType)
        //{
        //    if (historyWinningNumber == null)
        //    {
        //        throw new ArgumentNullException("historyWinningNumber");
        //    }
        //    //如果 “排除最新期数” 的数量超出允许的最大范围。
        //    if (historyWinningNumber.Count < GS.MIN_NEWEST_PERIOD_COUNT || historyWinningNumber.Count > GS.MAX_NEWEST_PERIOD_COUNT)
        //    {
        //        throw new ArgumentOutOfRangeException("historyWinningNumber", "“排除最新期数” 的数量超出允许的最大范围（" + GS.MIN_NEWEST_PERIOD_COUNT + "-" + GS.MAX_NEWEST_PERIOD_COUNT + "）！");
        //    }
        //    if (dingMaCount < GS.MIN_DING_MA_COUNT || dingMaCount > (100 - historyWinningNumber.Count))
        //    {
        //        throw new ArgumentOutOfRangeException("dingMaCount", "所设置的 “排除最新期数号” 和 “定码个数” 两个数的和不能大于 100！请重新设置！");
        //    }
        //    //dingMaCount 参数就算满足了上面的限定，也有可能会超过所允许的最大值 MAX_DING_MA_COUNT，所以需要在此继续补充判断。
        //    if (dingMaCount > GS.MAX_DING_MA_COUNT)
        //    {
        //        throw new ArgumentOutOfRangeException("dingMaCount", "要保留的定码个数超出所允许的最大值（" + GS.MAX_DING_MA_COUNT + "）！");
        //    }
        //    //如果历史中奖号码列表 historyWinningNumber 中出现格式非法（即号码长度不等于 5）的中奖号码值，则抛出异常。
        //    if (historyWinningNumber.Exists(n => n.Length != 5))
        //    {
        //        throw new ArgumentException("所传入的 “历史中奖号码” 列表中含有格式非法的中奖号码！", "historyWinningNumber");
        //    }

        //    bool isFirstTwo = forecastType == ForecastType.FirstTwo;
        //    List<string> planForecastListWaitForRemove = (from n in historyWinningNumber
        //                                                  select isFirstTwo ? n.FirstTwo() : n.LastTwo()).Distinct().ToList();

        //    //nextNewestLogActuallyHistoryWinningNumberCount = historyWinningNumber.Count;
        //    #region 用后面的几行代码代替第 1 行的 GetRandoms() 方法，降低耦合度。
        //    //return GetRandoms(planForecastListWaitForRemove, dingMaCount).ToList();

        //    List<string> fullItems = GetFullForecastNumberList();
        //    RemoveSpecifiedItems(fullItems, planForecastListWaitForRemove);
        //    return RandomSelectSomeItems(fullItems, dingMaCount).ToList();
        //    #endregion
        //}


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
        //public List<string> CalPlanForecastList(int historySourceDataStartIndex, int historySourceDataRangeCount, bool getAsMuchAsPossibleIfHistorySourceDataNotEnougth, bool exceptStartSourceData, int dingMaCount, ForecastType forecastType)
        //{
        //    List<SourceData> historyData = mgr.GetTopRangeHistorySourceData(historySourceDataStartIndex, historySourceDataRangeCount, getAsMuchAsPossibleIfHistorySourceDataNotEnougth, exceptStartSourceData);
        //    int historyCount = historyData.Count;
        //    List<string> planForecastNumberList = CalPlanForecastList(historyData.ExtractWinningNumber(), this.CurrentSelectedDingMaCount, this.CurrentSelectedForecastType);

        //    if (planForecastNumberList == null || planForecastNumberList.Count != this.currentSelectedDingMaCount)
        //    {
        //        throw new ApplicationException("所计算出的即将要开奖的未来一期 “预测列表” 数据个数和设置值不一致！");
        //    }
        //    return planForecastNumberList;
        //}



        //#region GetRandoms() 方法，已没用，已被下面的 “RandomSelectSomeItems()”、“RemoveSpecifiedItems()” 和 “GetFullForecastNumberList()” 方法替代。
        /////// <summary>
        /////// 在一个以 "00"-"99" 为元素的新集合中排除掉已在泛型列表 itemsWaitForExclusion 中存在的所有元素后，从剩余的元素集合中随机抽取指定的 count 个元素组成新的泛型列表并返回。
        /////// </summary>
        /////// <param name="itemsWaitForExclusion">将要在新集合中排除掉的所有元素组成的泛型列表。</param>
        /////// <param name="count">将要最终保留的元素个数。</param>
        /////// <returns></returns>
        ////private IEnumerable<string> GetRandoms(IEnumerable<string> itemsWaitForExclusion, int count)
        ////{
        ////    if (itemsWaitForExclusion == null || itemsWaitForExclusion.Count() < 1)
        ////    {
        ////        throw new ArgumentNullException("itemsWaitForExclusion");
        ////    }

        ////    if (count < 1)
        ////    {
        ////        throw new ArgumentOutOfRangeException("count", "要求保留的元素个数不能少于 1 个！");
        ////    }

        ////    int remainingItemCount = 100 - itemsWaitForExclusion.Count();
        ////    if (remainingItemCount < count)
        ////    {
        ////        throw new ApplicationException("要求保留的元素数不能多于所能提供的剩余定码（前二码/后二码）的总个数！");
        ////    }

        ////    List<string> forecastNumbers = new List<string>()
        ////    {
        ////        "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
        ////        "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
        ////        "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
        ////        "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
        ////        "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
        ////        "50", "51", "52", "53", "54", "55", "56", "57", "58", "59",
        ////        "60", "61", "62", "63", "64", "65", "66", "67", "68", "69",
        ////        "70", "71", "72", "73", "74", "75", "76", "77", "78", "79",
        ////        "80", "81", "82", "83", "84", "85", "86", "87", "88", "89",
        ////        "90", "91", "92", "93", "94", "95", "96", "97", "98", "99",
        ////    };

        ////    IEnumerable<string> waitForRemoveItems = itemsWaitForExclusion.Distinct();
        ////    forecastNumbers.RemoveAll(n => waitForRemoveItems.Contains(n));

        ////    Random r = new Random();
        ////    while (forecastNumbers.Count() > count)
        ////    {
        ////        forecastNumbers.RemoveAt(r.Next(0, forecastNumbers.Count));
        ////    }
        ////    return forecastNumbers;
        ////}
        //#endregion

        //#region 将上面的 GetRandoms() 方法拆分成这几个方法，以降低耦合度。
        ///// <summary>
        ///// 从 fullItems 集合中删除掉在 itemsWaitForExclusion 集合中存在的所有元素。
        ///// </summary>
        ///// <param name="fullItems">将要从中删除元素的集合。</param>
        ///// <param name="itemsWaitForExclusion">将要在 fullItems 集合中排除的所有元素组成的泛型集合。</param>
        ///// <returns>fullItems 集合中被删除掉的元素总数。</returns>
        //private int RemoveSpecifiedItems(List<string> fullItems, IEnumerable<string> itemsWaitForExclusion)
        //{
        //    if (fullItems == null || fullItems.Count() < 1)
        //    {
        //        throw new ArgumentNullException("fullItems");
        //    }
        //    if (itemsWaitForExclusion == null || itemsWaitForExclusion.Count() < 1)
        //    {
        //        #region 究竟是抛出异常还是正常返回，有待商议。
        //        //throw new ArgumentNullException("itemsWaitForExclusion");
        //        return 0;
        //        #endregion
        //    }

        //    //从 fullItems 集合中删除在 itemsWaitForExclusion 集合中存在的元素。
        //    return fullItems.RemoveAll(i => itemsWaitForExclusion.Contains(i));
        //}

        ///// <summary>
        ///// 获取没有进行过任何筛选操作的完整的定码（前二码/后二码）“预测列表” 可选值集合。
        ///// </summary>
        ///// <returns></returns>
        //private List<string> GetFullForecastNumberList()
        //{
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
        //    return forecastNumbers;
        //}

        ///// <summary>
        ///// 从传入的元素集合 fullItems 中随机选择出 count 个元素，并组成新的泛型列表来返回。
        ///// </summary>
        ///// <param name="fullItems">将要从中随机选择元素的泛型集合。</param>
        ///// <param name="count">最终要保留的元素个数。</param>
        ///// <returns></returns>
        //private IEnumerable<string> RandomSelectSomeItems(IEnumerable<string> fullItems, int count)
        //{
        //    if (fullItems == null || fullItems.Count() < 1)
        //    {
        //        throw new ArgumentNullException("fullItems");
        //    }
        //    if (count < 1)
        //    {
        //        throw new ArgumentOutOfRangeException("count", "要求保留的元素个数不能少于 1 个！");
        //    }
        //    if (count > fullItems.Count())
        //    {
        //        throw new ArgumentOutOfRangeException("count", "要求保留的元素个数不能超过源集合的元素总数！");
        //    }

        //    List<string> remainItems = new List<string>(fullItems);
        //    Random r = new Random();
        //    while (remainItems.Count() > count)
        //    {
        //        remainItems.RemoveAt(r.Next(0, remainItems.Count));
        //    }
        //    return remainItems;
        //}
        //#endregion

        #endregion
    }
}
