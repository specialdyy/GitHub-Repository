using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core
{
    public static class NumberHelper
    {
        public static Number ToNumber(this string numberString)
        {
            return new Number(numberString);
        }

        /// <summary>
        /// 批量地把若干个字符串类型的整数转换成 Number 类型的对象并存储到传入的 list 列表容器对象中。
        /// </summary>
        /// <param name="list">用来存储转换后的所有 Number 对象的列表对象。</param>
        /// <param name="numbers">要转换为 Number 类型对象的字符串类型的若干个整数值。</param>
        public static void ConvertAndFillNumbers(this List<Number> list, params string[] numbers)
        {
            if(list == null)
            {
                throw new ArgumentNullException("list", "所传入的 List<Number> 类型的列表对象不能为空！");
            }
            if(numbers == null || numbers.Length < 1)
            {
                return;
            }

            list.AddRange(from n in numbers
                          select n.ToNumber());
            return;
        }


        /// <summary>
        /// 判断一个字符串是否为合法的数值类型，如果是则返回 true，否则返回 false。
        /// </summary>
        /// <param name="numberString"></param>
        /// <returns></returns>
        public static bool IsNumber(this string numberString)
        {
            if (string.IsNullOrEmpty(numberString))
            {
                return false;
            }
            if (false == Regex.IsMatch(numberString, @"^[\+\-]?[0-9]+$"))
            {
                //所传入的数值格式非法！必须为整数数值类型。
                return false;
            }
            if (Regex.IsMatch(numberString, @"^[\+\-]0+$"))
            {
                //整数 0 既不是正数也不是负数，不能带有符号！
                return false;
            }
            return true;
        }


        /// <summary>
        /// 和 IsNumber() 方法的功能类似，校验一个字符串是否为合法的数值类型。
        /// 但该方法只有当检验全部通过之后才返回 true，否则直接抛出异常，而不是返回 false。
        /// </summary>
        /// <param name="numberString"></param>
        /// <returns></returns>
        public static bool ValidateStringIsNumber(this string numberString)
        {
            if (string.IsNullOrEmpty(numberString))
            {
                throw new ArgumentNullException("numberString");
            }
            
            if (false == Regex.IsMatch(numberString, @"^[\+\-]?[0-9]+$"))
            {
                throw new ArgumentException("所传入的数值格式非法！必须为整数数值类型。");
            }
            if (Regex.IsMatch(numberString, @"^[\+\-]0+$"))
            {
                throw new ArgumentException("整数 0 既不是正数也不是负数，不能带有符号！");
            }
            return true;
        }
    }
}
