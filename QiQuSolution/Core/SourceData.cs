using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Core
{
    [Serializable]
    public class SourceData : ICloneable
    {
        public static readonly TimeSpan ONLINE_TIME_RANGE = TimeSpan.FromDays(20);



        /// <summary>
        /// 该构造函数仅提供给 DataDownloader 类的 DownloadData 方法使用，程序集外部需要使用带参数的构造函数，
        /// 这样才能防止 SourceData 对象初始化方式不同所造成的混乱。
        /// </summary>
        private SourceData()
        {
        }

        public SourceData(DateTime onlineTime, string onlineNumber, string onlineChange)
        {
            InitialData(onlineTime, onlineNumber, onlineChange);
        }


        #region 经测试确认不可行！因为 JavaScriptSerializer 的 Deserialize() 方法也是在当前类以外的程序集执行，所以本质上也不是在当前类内部创建 SourceData 对象，所以报错。
        ///// <summary>
        ///// 将 Json 格式的若干条源数据转换成 SourceData 类型的对象，并以泛型列表的方式来返回。
        ///// 
        ///// 注意！该方法主要是为了解决 “外部的 Json 数据必须要调用 SourceData 类的无参构造函数才能创建对象” 和 “如果让外部调用无
        ///// 参构造函数来创建 SourceData 对象有可能会导致 SourceData 对象的成员没有被初始化就被使用” 这两个问题之间的矛盾。
        ///// 
        ///// 通过在 SourceData 类内部实现 “把 Json 数据解析并转换成 SourceData 对象” 的功能，来解决上面说到的矛盾，即：既可以不
        ///// 让外部调用本类的无参构造函数，导致对象的成员有可能没有被初始化，又可以让 Json 数据成功转换成 SourceData 对象。
        ///// </summary>
        ///// <param name="jsonData">需要解析转换成 SourceData 对象的 Json 格式数据。</param>
        ///// <returns></returns>
        //public static List<SourceData> CreateSourceDataListByJsonData(string jsonData)
        //{
        //    if (string.IsNullOrEmpty(jsonData))
        //    {
        //        throw new ArgumentNullException("jsonData");
        //    }

        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    List<SourceData> data;

        //    try
        //    {
        //        data = (List<SourceData>)js.Deserialize(jsonData, typeof(List<SourceData>));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("把 Json 类型数据转换成 SourceData 类型的对象失败！", ex);
        //    }
        //    if (data == null || data.Count <= 0)
        //    {
        //        throw new ApplicationException("获取指定地址的数据失败！");
        //    }
        //    return data;
        //}
        #endregion


        /// <summary>
        /// 初始化当前的对象。
        /// 如果是通过默认的无参构造函数来创建 SourceData 对象，则创建出来的对象是没有被初始化的，此时的对象不能使用，
        /// 必须先手动调用 InitialData() 来完成初始化后才能使用对象。
        /// </summary>
        /// <param name="onlineTime"></param>
        /// <param name="onlineNumber"></param>
        /// <param name="onlineChange"></param>
        public void InitialData(DateTime onlineTime, string onlineNumber, string onlineChange)
        {
            this.OnlineTime = onlineTime;   //必须赋值给 this.OnlineTime，这样才有校验时间合理性的效果。
            //this.TimeId = this.OnlineTime.ToTimeId();     //此操作已经在对上一行的 this.OnlineTime 属性的设置器（setter）内部操作代码中完成了。
            this.OnlineNumber = onlineNumber;
            this.OnlineChange = onlineChange;

            //上面的 “this.OnlineNumber = onlineNumber;” 语句触发了 this.OnlineNumber 属性的 setter 设置器，从
            //而间接调用了下面的 “CalculateWinningInfoByOnlineNumber()” 方法，因此在下面这里不需要再重复调用了。
            //CalculateWinningInfoByOnlineNumber(this.onlineNumber.ToNumber());
        }

        /// <summary>
        /// 有记录时间先后顺序功能的时间Id，Id值越大当前的对象就越新。
        /// </summary>
        public int TimeId { get; private set; }


        private DateTime onlineTime;
        /// <summary>
        /// 数据的记录时间
        /// </summary>
        public DateTime OnlineTime
        {
            get
            {
                return this.onlineTime;
            }
            private set
            {
                if (onlineTime.Equals(value))
                {
                    return;
                }
                DateTime today = DateTime.Now;
                DateTime tenDaysAgo = today - ONLINE_TIME_RANGE;
                DateTime tenDaysLater = today + ONLINE_TIME_RANGE;
                if (value < tenDaysAgo || value > tenDaysLater)
                {
                    throw new ArgumentOutOfRangeException("OnlineTime", "所要设置的源数据的 “在线时间” 范围必须要在今天之前和之后的 " + ONLINE_TIME_RANGE.Days + " 天之内！");
                }
                this.onlineTime = value;
                this.TimeId = value.ToTimeId();     //连带地为 TimeId 属性赋值，以防止调用默认无参构造函数时不会对 TimeId 进行赋值的问题。
            }
        }

        private string onlineNumber;
        /// <summary>
        /// 在线人数
        /// </summary>
        public string OnlineNumber
        {
            get
            {
                return onlineNumber;
            }
            private set
            {
                Number tmp = value.ToNumber();
                if (tmp.IsNegative)
                {
                    throw new ArgumentOutOfRangeException("OnlineNumber", "在线人数值只能为非 0 的正整数！");
                }

                if (value != this.onlineNumber)
                {
                    this.onlineNumber = value;
                    CalculateWinningInfoByOnlineNumber(value.ToNumber());
                }
            }
        }


        private string onlineChange;
        /// <summary>
        /// 前后两期（本期和上一期）在线人数的变化人数值（本期人数 - 上期人数的差值）。
        /// </summary>
        public string OnlineChange
        {
            get
            {
                return this.onlineChange;
            }
            private set
            {
                value.ValidateStringIsNumber();
                this.onlineChange = value.Trim();
            }
        }


        /// <summary>
        /// 获取通过当前在线人数计算出的中奖号码。
        /// </summary>
        public Number WinningNumber
        {
            //get { return winningNumber; }
            get;
            private set;
        }


        /// <summary>
        /// 获取通过当前在线人数计算出的中奖号码的前二码。
        /// </summary>
        public string FirstTwo
        {
            get;
            private set;
        }


        /// <summary>
        /// 获取通过当前在线人数计算出的中奖号码的后二码。
        /// </summary>
        public string LastTwo
        {
            get;
            private set;
        }













        /// <summary>
        /// 根据所传入的在线人数重新计算并刷新当前对象的中奖号码、中奖号码的前二码、后二码等数据信息。
        /// </summary>
        private void CalculateWinningInfoByOnlineNumber(Number onlineNumber)
        {
            this.WinningNumber = onlineNumber.CalWinningNumber();
            this.FirstTwo = this.WinningNumber.FirstTwo();
            this.LastTwo = this.WinningNumber.LastTwo();
        }


        public override bool Equals(object obj)
        {
            if (obj != null && obj is SourceData)
            {
                SourceData that = obj as SourceData;
                if (this.OnlineChange == that.OnlineChange && this.OnlineNumber == that.OnlineNumber && this.OnlineTime == that.OnlineTime)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.TimeId;
        }


        public object Clone()
        {
            #region 旧方法。
            return new SourceData(this.OnlineTime, new string(this.onlineNumber.ToCharArray()), new string(this.OnlineChange.ToCharArray()));
            #endregion

            #region 新方法。
            //return this.BinaryClone();
            #endregion
        }
    }
}
