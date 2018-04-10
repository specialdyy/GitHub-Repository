using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public delegate void SourceDataDownloadedHandler(IEnumerable<SourceData> newDatas, int oldDataCount);
    public delegate void SourceDataAddedHandler(SourceData newData, int oldDataCount);
    public class SourceDataManager
    {
        public event SourceDataDownloadedHandler OnNewSourceDataDownloaded;
        public event SourceDataAddedHandler OnNewSourceDataAdded;

        public static readonly Func<DateTime, TimeSpan> DefaultNextMinute0SecondCalculator = now => TimeSpan.FromSeconds(60.5 - now.Second);

        #region 测试之用。
        //public static readonly Func<DateTime, TimeSpan> DefaultNextMinute0SecondCalculator = now => TimeSpan.FromSeconds(60 - now.Second);
        #endregion

        private static readonly SourceDataManager singleObject = new SourceDataManager();

        /// <summary>
        /// 自从打开软件以来所有通过奇趣网站的接口下载获取到的历史数据集合。
        /// </summary>
        private static readonly List<SourceData> sourceDatas = new List<SourceData>();

        /// <summary>
        /// 用来记录当前的 sourceDatas 集合中时间最新（时间最新并不一定是最后添加）的 TimeId 值。
        /// </summary>
        private static int newestTimeId = -1;

        private static Timer downloadTimer;

        private SourceDataManager()
        {
        }

        public static SourceDataManager GetInstance()
        {
            return singleObject;
        }

        /// <summary>
        /// 把所传入的源数据对象添加到当前对象的源数据列表中，如果传入的源数据对象为空或者在当前对象中的源数据列表中已存在，则抛出异常。
        /// 该方法内部已经对 sourceDatas 集合进行了排序操作，而且出于对性能考虑，添加了 newestTimeId 字段来辅助判断是否需要对 sourceDatas 集合进行排序，而并不是每次添加都进行排序。
        /// </summary>
        /// <param name="source">要添加到当前对象中的源数据列表中的源数据对象。</param>
        /// <returns>返回当前对象，以便链式操作。</returns>
        private SourceDataManager AddSourceData(SourceData source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            List<SourceData> repeats = (from s in sourceDatas
                                        where s.TimeId == source.TimeId
                                        select s).ToList();
            if (repeats.Count > 0)
            {
                if (repeats.Exists(s => s.Equals(source)))   //估计不能用 Contains()，因为 Contains() 说明中说的是元素是否在 List<T> 中，而这里要判断的 source 对象并不是存在于 List<T> 中的。
                {
                    throw new ApplicationException("所要添加的 SourceData 对象已存在！");
                }
                else
                {
                    throw new ApplicationException("存在相同 TimeId 值的 SourceData 对象！");
                }
            }

            //因为由于业务的需要， sourceDatas 集合默认是按 TimeId 从新到旧的顺序来排序的，即排在最前的是最新的，而大多数情况下新添加的就是 TimeId 值最新的，所以
            //用 Insert() 来代替 Add() 会比较节省性能。
            sourceDatas.Insert(0, source);

            #region 通过一定的判断逻辑来尽量减少排序的操作次数，从而提高性能。
            //如果 newestTimeId 还没有值（初始值 -1 不算），则表示从来没有添加过 SourceData 元素。
            if (newestTimeId == -1)
            {
                newestTimeId = source.TimeId;
            }
            else
            {
                //如果当前添加的元素 source 在列表集合 sourceDatas 中不是最新的，则进行排序。
                if (newestTimeId > source.TimeId)
                {
                    SortSourceDatasByTimeIdDescending();
                }
                //更新 newestTimeId 的值。
                newestTimeId = Math.Max(newestTimeId, source.TimeId);
            }
            #endregion

            if (this.OnNewSourceDataAdded != null)
            {
                this.OnNewSourceDataAdded(source, sourceDatas.Count);
            }
            return this;
        }


        /// <summary>
        /// 把所传入的列表中的所有不为空且互不重复的源数据对象添加到当前对象的源数据列表中，并且对源数据列表进行由新到旧的顺序排序，如果要添加的源数据对象在当前对象中的源数据列表中已存在，则抛出异常。
        /// </summary>
        /// <param name="datas">要添加到当前对象的源数据集合。</param>
        /// <returns>返回当前对象，以便链式操作。</returns>
        private SourceDataManager AddSourceDatas(List<SourceData> datas)
        {
            if (datas == null || datas.Count() < 1)
            {
                throw new ArgumentNullException("datas");
            }

            SortSourceDatasByTimeId(datas, OrderType.Ascending);
            
            foreach (SourceData data in datas)
            {
                AddSourceData(data);
            }
            SortSourceDatasByTimeIdDescending();
            return this;
        }

        private void SortSourceDatasByTimeIdDescending()
        {
            SortSourceDatasByTimeId(sourceDatas, OrderType.Descending);
        }

        public static void SortSourceDatasByTimeId(List<SourceData> datas, OrderType orderType = OrderType.Descending)
        {
            if (datas == null || datas.Count < 1)
            {
                throw new ArgumentNullException("datas");
            }
            if (datas.Count == 1)
            {
                return;
            }

            if (orderType == OrderType.Ascending)
            {
                datas.Sort((d1, d2) => d1.TimeId - d2.TimeId);
            }
            else
            {
                datas.Sort((d1, d2) => d2.TimeId - d1.TimeId);
            }
        }

        #region 注意！有被外部修改内部集合元素状态的风险！
        /// <summary>
        /// 自从打开软件以来所有获取到的历史源数据集合。
        /// 注意！有被外部修改内部集合元素状态的风险！
        /// </summary>
        public ReadOnlyCollection<SourceData> SourceDatas
        {
            get
            {
                //#region 有待考证是深拷贝还是浅拷贝。
                //return new List<SourceData>(sourceDatas);   //复制一份给外部，以防止外部修改当前对象中的 sourceDatas 字段的元素。
                //#endregion
                return sourceDatas.AsReadOnly();
            }
        }
        #endregion




        public int IndexOf(SourceData data)
        {
            return sourceDatas.IndexOf(data);
        }

        public int IndexOf(int timeId)
        {
            SourceData data = sourceDatas.SingleOrDefault(d => d.TimeId == timeId);
            if (data == null)
            {
                return -1;
            }
            return sourceDatas.IndexOf(data);

        }



        /// <summary>
        /// 在所有历史源数据列表中从 startIndex 指定的源数据对象开始（包不包括这个 startIndex 指定的源数据对象本身则由 exceptStartItem 参数决定），获取最新的、指定数量的数据源对象，并以列表的方式返回。
        /// 如果开始获取的索引值 startIndex 小于 0，或者所要获取的最新源数据的数量值 rangeCount 小于 0 则抛出 ArgumentOutOfRangeException 异常。
        /// </summary>
        /// <param name="startIndex">源数据列表中将要开始获取的元素（或是该元素的前一元素，具体受 exceptStartItem 参数影响）的索引值，从此索引值指定的 SourceData 对象（或是该对象的下一个对象，具体受 exceptStartItem 参数影响）开始获取。 </param>
        /// <param name="rangeCount">所要获取的 SourceData 对象的个数。（对象的个数不受 exceptStartItem 参数影响）</param>
        /// <param name="getAsMuchAsPossibleIfNotEnougth">一个布尔值，用来指示如果当前对象的源数据列表中满足条件的源数据对象数量没有达到指定要获取的数量时，是否尽可能多地获取（即：不一定非得要满足指定的数量要求，尽管不够也有多少获取多少），如果是则为 true，否则为 false。</param>
        /// <param name="exceptStartItem">
        /// 一个布尔值，用来指示所要获取的 SourceData 对象集合中是否不包含 startIndex 指定的对象，如果是则为 true（此时开始获取的对象的索引值为 startIndex + 1），否则为 false。
        /// 注意！！！该布尔值不会影响 rangeCount 参数，即无论包不包含 startIndex 指定的对象，理论上要获取的个数都是 rangeCount 个。
        /// </param>
        /// <returns>
        /// 返回实际所获取到的源数据 SourceData 对象的集合列表。根据业务要求，该方法不会返回 null 值，只返回没有元素的空列表对象，或是抛出异常。
        /// 在 getAsMuchAsPossibleIfNotEnougth 参数为 true 的情况下，如果历史源数据中没有任何数据（sourceDatas.Count == 0），或是指定要获取的 SourceData 对象个数为 0（即：rangeCount == 0），则返回空的 List 集合对象，
        /// 在 getAsMuchAsPossibleIfNotEnougth 参数为 false 的情况下，如果所要获取的最新源数据的数量超过软件中满足指定条件的历史源数据的总数，则抛出 ApplicationException 异常。
        /// </returns>
        public List<SourceData> GetTopRangeHistorySourceData(int startIndex, int rangeCount, bool getAsMuchAsPossibleIfNotEnougth, bool exceptStartItem)
        {
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }
            if (rangeCount < 0)
            {
                throw new ArgumentOutOfRangeException("rangeCount");
            }
            //如果要求获取的 SourceData 对象数量为 0，则直接返回空集合。
            if (rangeCount == 0)
            {
                return new List<SourceData>(0);
            }
            //如果源数据列表没有任何数据且 getAsMuchAsPossibleIfNotEnougth 参数为 true（即调用方要求有多少获取多少，不一定非得要满足指定的数量要求），则直接返回 null。
            if (sourceDatas.Count < 1 && getAsMuchAsPossibleIfNotEnougth)
            {
                return new List<SourceData>(0);
            }

            if (exceptStartItem)
            {
                startIndex += 1;
            }

            if (false == getAsMuchAsPossibleIfNotEnougth && sourceDatas.Count < startIndex + rangeCount)
            {
                throw new ApplicationException("所要获取的最新源数据的数量超过软件中满足指定条件的历史源数据的总数！");
            }

            #region 感觉不太需要。因为在 AddSourceData() 方法中已经进行了排序，所以每添加一个 SourceData 对象，顺序都是排好了的，因此这里不需要再进行排序了。
            //SortSourceDatasByTimeIdDescending();  //要先排好序再进行下面的索引值获取，否则索引值会不准确。
            #endregion

            return sourceDatas.Skip(startIndex).Take(rangeCount).Clone().ToList();    //在这里进行了序列化，避免了 “引用关系” 导致的修改。
        }


        #region 备份
        ///// <summary>
        ///// 在所有历史源数据列表中从 startTimeId 指定的源数据对象开始（包括这个 startTimeId 指定的源数据对象本身），获取最新的、指定数量的数据源对象，并以列表的方式返回。
        ///// </summary>
        ///// <param name="startTimeId">当前对象的源数据列表中将要开始获取的元素的 TimeId 属性值，从此 TimeId 相应的 SourceData 对象开始获取。 </param>
        ///// <param name="rangeCount">所要连续获取的 SourceData 对象的个数。</param>
        ///// <param name="getAsMuchAsPossibleIfNotEnougth">一个布尔值，用来指示如果当前对象的源数据列表中满足条件的源数据对象数量没有达到指定要获取的数量时，是否尽可能多地获取（即：尽管不够也有多少获取多少）。</param>
        ///// <returns></returns>
        //public ReadOnlyCollection<SourceData> GetTopRangeHistorySourceData(int startTimeId, int rangeCount, bool getAsMuchAsPossibleIfNotEnougth)
        //{
        //    if (sourceDatas.Count < 1)
        //    {
        //        return null;
        //    }
        //    if (rangeCount < 1)
        //    {
        //        #region 感觉不至于要抛出异常，直接返回 null 即可。
        //        //throw new ArgumentOutOfRangeException("rangeCount");
        //        #endregion
        //        return null;
        //    }
        //    if (false == getAsMuchAsPossibleIfNotEnougth && sourceDatas.Count < rangeCount)
        //    {
        //        throw new ApplicationException("所要获取的最新源数据的数量超过软件中所收集到的历史源数据的总数！");
        //    }
        //    SourceData startData = (from d in sourceDatas
        //                            where d.TimeId == startTimeId
        //                            select d).SingleOrDefault() ?? throw new ApplicationException("没有找到和指定的 TimeId 相应的源数据对象！");
        //    SortSourceDatasByTimeId();  //要先排好序再进行下面的索引值获取，否则索引值会不准确。

        //    int startIndex = sourceDatas.IndexOf(startData);

        //    //在总的源数据中减去 startIndex 指定的元素之前的所有元素（即减去比 startIndex 要新的所有元素），但不包括 startIndex 指定的元素本身。
        //    int totalRemain = sourceDatas.Count - startIndex;

        //    //如果所要获取的源数据数量（rangeCount） > 满足指定范围条件的源数据总数（totalRemain）
        //    if (rangeCount > totalRemain)
        //    {
        //        //如果设定了 “有多少要多少”，则把要获取的数量（rangeCount）修改为能满足条件的源数据总数（totalRemain）。
        //        if (getAsMuchAsPossibleIfNotEnougth)
        //        {
        //            rangeCount = totalRemain;
        //        }
        //        //否则抛出异常。
        //        else
        //        {
        //            throw new ApplicationException("所要获取的最新源数据的数量超过软件中满足指定条件的历史源数据的总数！");
        //        }
        //    }

        //    #region 有待考证是深拷贝还是浅拷贝。
        //    return sourceDatas.Skip(startIndex).Take(rangeCount).ToList().AsReadOnly();
        //    #endregion
        //}

        ///// <summary>
        ///// 在所有历史源数据列表中从 startIndex 指定的源数据对象的下一个对象开始（即：不包括这个 startIndex 指定的源数据对象本身），获取最新的、指定数量（数量也不包括 startTimeId 指定的对象本身）的数据源对象，并以列表的方式返回。
        ///// 如果开始获取的索引值 startIndex 小于 0，或者所要获取的最新源数据的数量值 rangeCount 小于 1 则抛出 ArgumentOutOfRangeException 异常。
        ///// </summary>
        ///// <param name="startIndex">
        ///// 源数据列表中将要开始获取的元素的前一个元素的索引值，从此索引值相应的 SourceData 对象的下一个对象开始获取。</param>
        ///// <param name="rangeCount">所要获取的 SourceData 对象的个数。（不包含 startIndex 参数指向的 SourceData 对象）。</param>
        ///// <param name="getAsMuchAsPossibleIfNotEnougth">一个布尔值，用来指示如果当前对象的源数据列表中满足条件的源数据对象数量没有达到指定要获取的数量时，是否尽可能多地获取（即：不一定非得要满足指定的数量要求，尽管不够也有多少获取多少），如果是则为 true，否则为 false。</param>
        ///// <returns>
        ///// 返回实际所获取到的源数据 SourceData 对象的集合列表。
        ///// 在 getAsMuchAsPossibleIfNotEnougth 参数为 true 的情况下，如果历史源数据中没有任何数据（sourceDatas.Count == 0）则返回 null，
        ///// 在 getAsMuchAsPossibleIfNotEnougth 参数为 false 的情况下，如果所要获取的最新源数据的数量超过软件中满足指定条件的历史源数据的总数，则抛出 ApplicationException 异常。
        ///// </returns>
        //public List<SourceData> GetTopRangeHistorySourceDataExceptStartObject(int startIndex, int rangeCount, bool getAsMuchAsPossibleIfNotEnougth)
        //{
        //    //由于下面调用 GetTopRangeHistorySourceData() 方法时修改了 rangeCount 参数，所以为了参数的合理性，在修改 rangeCount 参数之前先对该参数进行合法性校验。
        //    if (rangeCount <= 0)
        //    {
        //        throw new ArgumentOutOfRangeException("rangeCount");
        //    }

        //    //“Skip(1)” 是为了把当前的 startIndex 所关联的 SourceData 对象排除在外。
        //    //“rangeCount + 1” 是为了解决由于排除了当前的 startIndex 所关联的 SourceData 对象之后所导致的获取到的历史 SourceData 对象数目少 1 的情况。
        //    return GetTopRangeHistorySourceData(startIndex, rangeCount + 1, getAsMuchAsPossibleIfNotEnougth).Skip(1).ToList();
        //}
        #endregion


        public SourceData GetSourceDataByTimeId(int timeId)
        {
            if (sourceDatas.Count < 1)
            {
                return null;
            }
            SourceData target = sourceDatas.SingleOrDefault(d => d.TimeId == timeId) ?? throw new ApplicationException("没有找到 TimeId 值为 " + timeId + " 的源数据对象！");
            return target;
        }


        public void ChangeTimerState(TimeSpan dueTime, TimeSpan period)
        {
            RepeatInfo info = new RepeatInfo(dueTime, period);
            info.ChangeTimer(downloadTimer);
        }

        public void StartRepeatDownload()
        {
            //为了预留一定的误差冗余，特意把触发间隔时间设为 60.5 秒而不是 60 秒。
            RepeatInfo infoNext = new RepeatInfo(DefaultNextMinute0SecondCalculator, TimeSpan.FromSeconds(60.5));

            //为了预留一定的误差冗余，特意把触发间隔时间设为 60.5 秒而不是 60 秒。
            RepeatInfo infoCurrent = new RepeatInfo(TimeSpan.FromSeconds(0), DefaultNextMinute0SecondCalculator);


            #region 测试之用：
            ////1
            //RepeatInfo infoNext = new RepeatInfo(TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(15));
            //RepeatInfo infoCurrent = new RepeatInfo(TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(15));

            ////2
            //RepeatInfo infoNext = new RepeatInfo(DefaultNextMinute0SecondCalculator, TimeSpan.FromSeconds(60));
            //RepeatInfo infoCurrent = new RepeatInfo(TimeSpan.FromSeconds(0), DefaultNextMinute0SecondCalculator);

            #endregion


            downloadTimer = new Timer(RepeatDownloadData, infoNext, infoCurrent.DueTime, infoCurrent.Period);
        }

        private void RepeatDownloadData(object repeatInfo)
        {
            if (repeatInfo == null || (false == repeatInfo is RepeatInfo))
            {
                throw new ArgumentException("参数 repeatInfo 为 null 或不是 RepeatInfo 类型的实例！", "repeatInfo");
            }
            RepeatInfo info = repeatInfo as RepeatInfo;
            List<SourceData> data = DataDownloader.DownloadData();
            bool isSuccess = CombineSourceData(data);
            //如果合并不成功（即：data 数据为空或者没有最新这一分钟的数据），则把触发时间改为 1 秒之后，即 1 秒后重新触发下载操作。
            if(false == isSuccess)
            {
                info.Change(TimeSpan.FromSeconds(1), DefaultNextMinute0SecondCalculator);
            }
            //否则，如果合并成功，则把触发时间设为正常的 “在下一分钟开始”。
            else
            {
                info.Change(DefaultNextMinute0SecondCalculator, TimeSpan.FromSeconds(60.5));
            }
            info.ChangeTimer(downloadTimer);    //通过 RepeatInfo 对象的参数来更新 Timer 对象的触发参数。
            //上面的这个 “info.ChangeTimer(downloadTimer);” 写法替代了下面的这种。
            //downloadTimer.Change(info.DueTime, info.Period);
        }


        /// <summary>
        /// 把从奇趣网站上获取到的最新 10 条源数据和当前对象中的 SourceDatas 集合中的源数据进行合并，相同的只保留一份，以防止出现重复。
        /// </summary>
        /// <param name="data">从网站上下载下来的最新 10 条源数据对象集合。</param>
        /// <returns>如果下载到的源数据集合为 null，或者源数据集合的条数为 0，或者最新的这一分钟的数据被丢失，则返回 false，否则返回 true。</returns>
        private bool CombineSourceData(List<SourceData> data)
        {
            if (data == null || data.Count < 1)
            {
                return false;
            }
            List<SourceData> unitData = data;
            //如果 sourceDatas 中存在数据，则先删除掉 data 中的和 sourceDatas 集合里的 SourceData 类型元素重复的元素对象。
            if (sourceDatas.Count > 0)
            {
                SourceData lastData = GetTopRangeHistorySourceData(0, 1, false, false).SingleOrDefault();
                //unitData 是用来存储 data 集合中比当前对象中的 sourceDatas 集合中最新的那条数据都还要新的所有数据的集合。
                unitData = (from d in data
                            where d.TimeId > lastData.TimeId
                            orderby d.TimeId descending
                            select d).ToList();

                //如果没有下载到数据，或者下载到的数据中没有当前这一分钟的数据（即：最新的这一分钟的数据被丢失），则隔一秒后重新发起下载操作。
                if (unitData == null || unitData.Count() < 1 || unitData.Count(d => d.OnlineTime.Minute == DateTime.Now.Minute) != 1)
                {
                    return false;
                }
            }
            if (this.OnNewSourceDataDownloaded != null)
            {
                this.OnNewSourceDataDownloaded(unitData, sourceDatas.Count);
            }
            AddSourceDatas(unitData);
            return true;
        }
    }
}