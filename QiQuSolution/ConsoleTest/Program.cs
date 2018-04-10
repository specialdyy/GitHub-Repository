using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 异步委托测试。
            //Func<List<SourceData>> fun = new Func<List<SourceData>>(DataDownloader.DownloadData);
            //IAsyncResult result = fun.BeginInvoke(DataDownloaded, "4");
            //Console.WriteLine("已执行完！");
            //Console.ReadKey();
            #endregion

            #region 值传递和引用传递测试。
            //Console.WriteLine("值传递和引用传递测试");
            //DateTime now = DateTime.Now;
            //List<SourceData> data = new List<SourceData>();
            //for (int i = 0; i < 8; i++)
            //{
            //    data.Add(new SourceData(now + TimeSpan.FromMinutes(i), (321495327 + i).ToString(), (24732 + i).ToString()));
            //}
            //Console.WriteLine("刚下载下来时，数据总条数：" + data.Count);

            //data.AddRange(data);
            //Console.WriteLine("添加了一次自身的所有元素之后，数据总条数：" + data.Count);

            //List<SourceData> d1 = data.Distinct().ToList();
            //Console.WriteLine("执行了 Distinct() 之后，源集合的数据总条数：" + data.Count + "，\t\tDistinct() 方法返回的集合的数据总条数：" + d1.Count);

            //Console.WriteLine("--------------------------源集合的数据-------------------------------");
            //data.ForEach(d => Console.WriteLine("TimeId=" + d.TimeId + "\t\tOnlineTime=" + d.OnlineTime.ToString() + "\t\tOnlineNumber=" + d.OnlineNumber + "\t\tOnlineChange=" + d.OnlineChange + "\t\tWinningNumber=" + d.WinningNumber + "\t\tFirstTwo=" + d.FirstTwo + "\t\tLastTwo=" + d.LastTwo));// + Environment.NewLine));

            //Console.WriteLine("--------------------------去重复后的集合的数据-------------------------------");
            //d1.ForEach(d => Console.WriteLine("TimeId=" + d.TimeId + "\t\tOnlineTime=" + d.OnlineTime.ToString() + "\t\tOnlineNumber=" + d.OnlineNumber + "\t\tOnlineChange=" + d.OnlineChange + "\t\tWinningNumber=" + d.WinningNumber + "\t\tFirstTwo=" + d.FirstTwo + "\t\tLastTwo=" + d.LastTwo));// + Environment.NewLine));

            //d1.ForEach(d =>
            //{
            //    d.InitialData(d.OnlineTime + TimeSpan.FromMinutes(2),
            //        (Convert.ToInt32(d.OnlineNumber) + 2).ToString(),
            //        (Convert.ToInt32(d.OnlineChange) + 1).ToString()
            //        );
            //});
            //Console.WriteLine("--------------------------把去重复后的集合的数据随机修改之后：-------------------------------");
            //Console.WriteLine("--------------------------源集合的数据-------------------------------");
            //data.ForEach(d => Console.WriteLine("TimeId=" + d.TimeId + "\t\tOnlineTime=" + d.OnlineTime.ToString() + "\t\tOnlineNumber=" + d.OnlineNumber + "\t\tOnlineChange=" + d.OnlineChange + "\t\tWinningNumber=" + d.WinningNumber + "\t\tFirstTwo=" + d.FirstTwo + "\t\tLastTwo=" + d.LastTwo));// + Environment.NewLine));

            //Console.WriteLine("--------------------------去重复后的集合的数据-------------------------------");
            //d1.ForEach(d => Console.WriteLine("TimeId=" + d.TimeId + "\t\tOnlineTime=" + d.OnlineTime.ToString() + "\t\tOnlineNumber=" + d.OnlineNumber + "\t\tOnlineChange=" + d.OnlineChange + "\t\tWinningNumber=" + d.WinningNumber + "\t\tFirstTwo=" + d.FirstTwo + "\t\tLastTwo=" + d.LastTwo));// + Environment.NewLine));
            //Console.WriteLine("总结：通过 List<T>（T 为自定义的引用类型）类型对象的 AddRange() 方法添加的还是元素对象的引用而不是添加一个完全独立的副本。");
            //Console.ReadKey();

















            //List<SourceData> d1 = data.Select(d => d).ToList();
            //for (int i = 0; i < d1.Count; i++)
            //{
            //    Console.WriteLine("object.ReferenceEquals(d1[i], data[i]) = " + object.ReferenceEquals(d1[i], data[i]));
            //    Console.WriteLine("object.ReferenceEquals(d1[i].TimeId, data[i].TimeId) = " + object.ReferenceEquals(d1[i].TimeId, data[i].TimeId));
            //    Console.WriteLine("object.ReferenceEquals(d1[i].OnlineTime, data[i].OnlineTime) = " + object.ReferenceEquals(d1[i].OnlineTime, data[i].OnlineTime));
            //    Console.WriteLine("object.ReferenceEquals(d1[i].OnlineNumber, data[i].OnlineNumber) = " + object.ReferenceEquals(d1[i].OnlineNumber, data[i].OnlineNumber));
            //    Console.WriteLine("object.ReferenceEquals(d1[i].OnlineChange, data[i].OnlineChange) = " + object.ReferenceEquals(d1[i].OnlineChange, data[i].OnlineChange));
            //    Console.WriteLine("object.ReferenceEquals(d1[i].WinningNumber, data[i].WinningNumber) = " + object.ReferenceEquals(d1[i].WinningNumber, data[i].WinningNumber));
            //    Console.WriteLine("object.ReferenceEquals(d1[i].FirstTwo, data[i].FirstTwo) = " + object.ReferenceEquals(d1[i].FirstTwo, data[i].FirstTwo));
            //    Console.WriteLine("object.ReferenceEquals(d1[i].LastTwo, data[i].LastTwo) = " + object.ReferenceEquals(d1[i].LastTwo, data[i].LastTwo));
            //}

            //Console.WriteLine();

            //d1 = new List<SourceData>(data);
            //for (int i = 0; i < d1.Count; i++)
            //{
            //    Console.WriteLine("object.ReferenceEquals(d1[i], data[i]) = " + object.ReferenceEquals(d1[i], data[i]));
            //    Console.WriteLine("object.ReferenceEquals(d1[i].TimeId, data[i].TimeId) = " + object.ReferenceEquals(d1[i].TimeId, data[i].TimeId));
            //    Console.WriteLine("object.ReferenceEquals(d1[i].OnlineTime, data[i].OnlineTime) = " + object.ReferenceEquals(d1[i].OnlineTime, data[i].OnlineTime));
            //    Console.WriteLine("object.ReferenceEquals(d1[i].OnlineNumber, data[i].OnlineNumber) = " + object.ReferenceEquals(d1[i].OnlineNumber, data[i].OnlineNumber));
            //    Console.WriteLine("object.ReferenceEquals(d1[i].OnlineChange, data[i].OnlineChange) = " + object.ReferenceEquals(d1[i].OnlineChange, data[i].OnlineChange));
            //    Console.WriteLine("object.ReferenceEquals(d1[i].WinningNumber, data[i].WinningNumber) = " + object.ReferenceEquals(d1[i].WinningNumber, data[i].WinningNumber));
            //    Console.WriteLine("object.ReferenceEquals(d1[i].FirstTwo, data[i].FirstTwo) = " + object.ReferenceEquals(d1[i].FirstTwo, data[i].FirstTwo));
            //    Console.WriteLine("object.ReferenceEquals(d1[i].LastTwo, data[i].LastTwo) = " + object.ReferenceEquals(d1[i].LastTwo, data[i].LastTwo));
            //}
            //Console.ReadKey();
            #endregion

            #region 定时下载测试。
            //SourceDataManager mgr = SourceDataManager.GetInstance();
            //mgr.StartRepeatDownload();
            //mgr.OnNewSourceDataDownloaded += Mgr_OnNewSourceDataDownloaded;
            //Console.ReadKey();
            #endregion

            #region 测试 Number 对象的 ToXXX 方法输出的格式。
            ////Console.WriteLine(" -000123 ".ToNumber().ToString());

            //Console.WriteLine("-000123".ToNumber().ToString());
            //Console.WriteLine("-000123".ToNumber().ToFullBitString());
            //Console.WriteLine("-000123".ToNumber().ToInt32());
            //Console.WriteLine("-000123".ToNumber().ToInt64());
            //Console.WriteLine(string.Join(" ", "-000123".ToNumber().ToBits()));
            //Console.WriteLine();

            //Console.WriteLine("+000123".ToNumber().ToString());
            //Console.WriteLine("+000123".ToNumber().ToFullBitString());
            //Console.WriteLine("+000123".ToNumber().ToInt32());
            //Console.WriteLine("+000123".ToNumber().ToInt64());
            //Console.WriteLine(string.Join(" ", "+000123".ToNumber().ToBits()));
            //Console.WriteLine();

            //Console.WriteLine("000123".ToNumber().ToString());
            //Console.WriteLine("000123".ToNumber().ToFullBitString());
            //Console.WriteLine("000123".ToNumber().ToInt32());
            //Console.WriteLine("000123".ToNumber().ToInt64());
            //Console.WriteLine(string.Join(" ", "000123".ToNumber().ToBits()));
            //Console.WriteLine();

            //Console.ReadKey();
            #endregion

            #region TimeSpan测试。
            //Console.WriteLine("TimeSpan.MaxValue = " + TimeSpan.MaxValue);
            //Console.WriteLine("TimeSpan.MinValue = " + TimeSpan.MinValue);
            //Console.WriteLine();

            //Console.WriteLine("TimeSpan.FromSeconds(0) = " + TimeSpan.FromSeconds(0));
            //Console.WriteLine("TimeSpan.FromSeconds(1) = " + TimeSpan.FromSeconds(1));
            //Console.WriteLine("TimeSpan.FromSeconds(-1) = " + TimeSpan.FromSeconds(-1));
            //Console.WriteLine();

            //Console.WriteLine("TimeSpan.FromMinutes(0) = " + TimeSpan.FromMinutes(0));
            //Console.WriteLine("TimeSpan.FromMinutes(1) = " + TimeSpan.FromMinutes(1));
            //Console.WriteLine("TimeSpan.FromMinutes(-1) = " + TimeSpan.FromMinutes(-1));
            //Console.WriteLine();

            //Console.WriteLine("TimeSpan.FromHours(0) = " + TimeSpan.FromHours(0));
            //Console.WriteLine("TimeSpan.FromHours(1) = " + TimeSpan.FromHours(1));
            //Console.WriteLine("TimeSpan.FromHours(-1) = " + TimeSpan.FromHours(-1));

            //Console.ReadKey();
            #endregion

            #region Exists 和 Sort 方法效率测试，经测试得知，Exists 方法还是要比 Sort 明显快很多，一百万条数据 Sort 的用时大概为 Exists 的几十倍左右。
            List<SourceData> datas = new List<SourceData>();
            DateTime now = DateTime.Now;
            Random r = new Random();
            for (int i = 0; i < 1000 * 1000 * 5; i++)
            {
                datas.Add(new SourceData(now + TimeSpan.FromMinutes(i), "298473256", "32748"));
            }

            datas.Reverse();
            DateTime time = now + TimeSpan.FromMinutes(2);
            SourceData tmp = new SourceData(time, "298473256", "32748");

            Stopwatch sw = new Stopwatch();
            sw.Start();
            datas.Exists(d => d.TimeId == tmp.TimeId);
            sw.Stop();
            Console.WriteLine("Exists() 方法耗时：" + sw.Elapsed);

            sw.Restart();
            datas.Sort((d1, d2) => { return d2.TimeId - d1.TimeId; });
            sw.Stop();
            Console.WriteLine("Sort() 方法耗时：" + sw.Elapsed);

            datas.Reverse();
            sw.Restart();
            List<SourceData> repeats = (from d in datas
                                        orderby d.TimeId descending
                                        where d.TimeId == tmp.TimeId
                                        select d).ToList();
            sw.Stop();
            Console.WriteLine("Linq 语句方法耗时：" + sw.Elapsed);

            ////输出结果：
            //Exists() 方法耗时：00:00:00.1018976
            //Sort() 方法耗时：00:00:02.3934203
            //Linq 语句方法耗时：00:00:00.9924494


            Console.ReadKey();
            #endregion

            #region ReadonlyCollection<T> 测试
            //List<SourceData> datas = new List<SourceData>();
            //DateTime now = DateTime.Now;
            //for (int i = 0; i < 100; i++)
            //{
            //    datas.Add(new SourceData(now + TimeSpan.FromMinutes(i), "298473256", "32748"));
            //}
            //ReadOnlyCollection<SourceData> readonlyDatas = datas.AsReadOnly();

            //////不能修改元素的属性
            ////readonlyDatas.First().OnlineNumber = "123";

            //////不能修改元素
            ////readonlyDatas[11] = new SourceData(DateTime.Now, "", "");
            ////SourceDataManager.GetInstance().SourceDatas[0] = null;

            //////可以执行一些操作，但像 Insert、Add、Remove 等这些操作在 ReadonlyCollection<T> 类型的对象中是没有的。
            //readonlyDatas.OrderBy<SourceData, int>(null);
            //Console.ReadKey();
            #endregion




        }

        //private static void Mgr_OnNewSourceDataAdded(SourceData newData, int oldDataCount)
        //{
        //    Console.WriteLine("TimeId=" + newData.TimeId + "\t\tOnlineTime=" + newData.OnlineTime.ToString() + "\t\tOnlineNumber=" + newData.OnlineNumber + "\t\tOnlineChange=" + newData.OnlineChange + "\t\tWinningNumber=" + newData.WinningNumber + "\t\tFirstTwo=" + newData.FirstTwo + "\t\tLastTwo=" + newData.LastTwo + Environment.NewLine);
        //}

        private static void Mgr_OnNewSourceDataDownloaded(IEnumerable<SourceData> newDatas, int oldDataCount)
        {
            newDatas.Reverse().ToList().ForEach(d => Console.WriteLine("TimeId=" + d.TimeId + "\t\tOnlineTime=" + d.OnlineTime.ToString() + "\t\tOnlineNumber=" + d.OnlineNumber + "\t\tOnlineChange=" + d.OnlineChange + "\t\tWinningNumber=" + d.WinningNumber + "\t\tFirstTwo=" + d.FirstTwo + "\t\tLastTwo=" + d.LastTwo + Environment.NewLine));
        }

        static void DataDownloaded(IAsyncResult iar)
        {
            AsyncResult ar = (AsyncResult)iar;
            Func<List<SourceData>> del = (Func<List<SourceData>>)ar.AsyncDelegate;
            List<SourceData> data = del.EndInvoke(iar);
            Console.WriteLine("参数：" + iar.AsyncState.GetType());
            Console.WriteLine(data.Count);
            data.ForEach(d =>
            {
                Console.WriteLine("TimeId = " + d.TimeId + ",\t\tOnlineTime = " + d.OnlineTime + ",\t\tOnlineNumber = " + d.OnlineNumber + ",\t\tOnlineChange = " + d.OnlineChange + ",\t\tWinningNumber = " + d.WinningNumber + ",\t\tFirstTwo = " + d.FirstTwo + ",\t\tLastTwo = " + d.LastTwo);
            });
        }
    }
}
