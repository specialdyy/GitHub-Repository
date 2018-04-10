using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class SourceDataHelper
    {
        /// <summary>
        /// 根据 “在线人数” 来计算出中奖号码，其中中奖号码位数为 5 位，格式为 “32478”，从左往右的顺序分别为 “第1位” 至 “第5位”。
        /// 中奖号码的计算逻辑为：假设在线人数为 “327,659,428”，则中奖号码的第 1 位的值为 “3 + 2 + 7 + 6 + 5 + 9 + 4 + 2 + 8” 的
        /// 和（即 46）的个位数（即 6），第 2 ~ 5 位则是在线人数 “327,659,428” 的后四位（即右边的四位）“9428”，所以完整的中奖号码为：“69428”。
        /// </summary>
        /// <param name="number">当前的 Number 类型的 “在线人数” 对象。</param>
        /// <returns>所计算出的 string 类型的中奖号码。</returns>
        public static string CalWinningNumberString(this Number number)
        {
            if (number == null)
            {
                throw new ArgumentNullException("number");
            }
            if (number.Length < 4)
            {
                throw new ApplicationException("所传入的数值总长度不足 4 位，无法计算中奖号码！");
            }

            int sum = 0;    //number 中所有位相加后得出的总和值。
            List<byte> result = number.ToBits();
            int length = result.Count;
            result.ForEach(n => sum += n);
            sum %= 10;
            return sum.ToString() + result[3] + result[2] + result[1] + result[0];
        }

        /// <summary>
        /// 根据 “在线人数” 来计算出中奖号码，其中中奖号码位数为 5 位，格式为 “32478”，从左往右的顺序分别为 “第1位” 至 “第5位”。
        /// 中奖号码的计算逻辑为：假设在线人数为 “327,659,428”，则中奖号码的第 1 位的值为 “3 + 2 + 7 + 6 + 5 + 9 + 4 + 2 + 8” 的
        /// 和（即 46）的个位数（即 6），第 2 ~ 5 位则是在线人数 “327,659,428” 的后四位（即右边的四位）“9428”，所以完整的中奖号码为：“69428”。
        /// </summary>
        /// <param name="number">当前的 Number 类型的 “在线人数” 对象。</param>
        /// <returns>所计算出的 Number 类型的中奖号码。</returns>
        public static Number CalWinningNumber(this Number number)
        {
            return number.CalWinningNumberString().ToNumber();
        }

        /// <summary>
        /// 获取当前数值对象的（从左往右的）最前（最左侧）两位。
        /// </summary>
        /// <param name="number">Number 类型的当前数值对象。</param>
        /// <returns></returns>
        public static string FirstTwo(this Number number)
        {
            if (number == null)
            {
                throw new ArgumentNullException("number");
            }
            int length = number.Length;
            if (length < 2)
            {
                throw new ApplicationException("所传入的数值总长度不足 2 位，无法获取前二码！");
            }
            return number[length - 1].ToString() + number[length - 2];
        }

        /// <summary>
        /// 获取当前数值对象的（从左往右的）最前（最左侧）两位。
        /// </summary>
        /// <param name="number">Number 类型的当前数值对象。</param>
        /// <returns></returns>
        public static int FirstTwoInt(this Number number)
        {
            return Convert.ToInt32(number.FirstTwo());
        }

        /// <summary>
        /// 获取当前数值对象的（从左往右的）最前（最左侧）两位。
        /// </summary>
        /// <param name="number">Number 类型的当前数值对象。</param>
        /// <returns></returns>
        public static Number FirstTwoNumber(this Number number)
        {
            return number.FirstTwo().ToNumber();
        }



        /// <summary>
        /// 获取当前数值对象的（从左往右的）最后（最右侧）两位。
        /// </summary>
        /// <param name="number">Number 类型的当前数值对象。</param>
        /// <returns></returns>
        public static string LastTwo(this Number number)
        {
            if (number == null)
            {
                throw new ArgumentNullException("number");
            }
            if (number.Length < 2)
            {
                throw new ApplicationException("所传入的数值总长度不足 2 位，无法获取后二码！");
            }
            return number[1].ToString() + number[0];
        }

        /// <summary>
        /// 获取当前数值对象的（从左往右的）最后（最右侧）两位。
        /// </summary>
        /// <param name="number">Number 类型的当前数值对象。</param>
        /// <returns></returns>
        public static int LastTwoInt(this Number number)
        {
            return Convert.ToInt32(number.LastTwo());
        }

        /// <summary>
        /// 获取当前数值对象的（从左往右的）最后（最右侧）两位。
        /// </summary>
        /// <param name="number">Number 类型的当前数值对象。</param>
        /// <returns></returns>
        public static Number LastTwoNumber(this Number number)
        {
            return number.LastTwo().ToNumber();
        }



        /// <summary>
        /// 提取出传入的源数据列表中每个源数据的中奖号码并以列表的形式返回。
        /// </summary>
        /// <param name="sourceDatas">要从中提取出中奖号码的源数据列表。</param>
        /// <returns></returns>
        public static List<Number> ExtractWinningNumber(this List<SourceData> sourceDatas)
        {
            #region 有待考证是深拷贝还是浅拷贝。
            return new List<Number>(from d in sourceDatas
                                    select d.WinningNumber);
            #endregion
        }



        /// <summary>
        /// 提取出传入的源数据列表中每个源数据的前二码并以列表的形式返回。
        /// </summary>
        /// <param name="sourceDatas">要从中提取出前二码的源数据列表。</param>
        /// <returns></returns>
        public static List<string> ExtractFirstTwo(this List<SourceData> sourceDatas)
        {
            #region 有待考证是深拷贝还是浅拷贝。
            return new List<string>(from d in sourceDatas
                                    select d.FirstTwo);
            #endregion
        }
        
        /// <summary>
        /// 提取出传入的源数据列表中每个源数据的后二码并以列表的形式返回。
        /// </summary>
        /// <param name="sourceDatas">要从中提取出后二码的源数据列表。</param>
        /// <returns></returns>
        public static List<string> ExtractLastTwo(this List<SourceData> sourceDatas)
        {
            #region 有待考证是深拷贝还是浅拷贝。
            return new List<string>(from d in sourceDatas
                                    select d.LastTwo);
            #endregion
        }


        /// <summary>
        /// 根据源数据的在线时间属性 OnlineTime 所记录的时间计算出 TimeId 值。
        /// 计算逻辑为：计算 onlineTime 参数所记录的时间（除掉秒值和毫秒值后）的总分钟数。大致推算，就算到了公
        /// 元 3000 年，该 TimeId 值的最大值也只是 60 * 24 * 365 * 3000 = 1,576,800,000，还未到 int 类型的最大值 2147483647。
        /// </summary>
        /// <param name="onlineTime"></param>
        /// <returns></returns>
        public static int ToTimeId(this DateTime onlineTime)
        {
            int year = onlineTime.Year - 1900 - 1;  //去掉 1900 年之前的年份值，让计算出的结果尽量小。
            int dayOfYear = onlineTime.DayOfYear - 1;   //计算出的结果还包含了没有完全过完的当前这一天（即运算代码时的 “今天”），所以要 -1 来去除这个 “当天”。
            int hour = onlineTime.Hour;
            int minute = onlineTime.Minute;
            int totalMinutes = (year * 365 * 24 * 60) + (dayOfYear * 24 * 60) + (hour * 60) + minute;
            return totalMinutes;
        }

        public static SourceData ToSourceData(this DTOSourceData data)
        {
            return new SourceData(data.OnlineTime, data.OnlineNumber, data.OnlineChange);
        }
    }
}
