using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core
{
    [Serializable]
    public class Number:ICloneable
    {


        #region 成员的引用关系
        //属性：
        //IsNegative	→	返回 this.number < 0
        //IsPositive	→	返回 this.number > 0
        //Length		→	返回 this.numberBits.Count;
        //OriginalString	→	返回 this.originalString
        //AbsoluteValueString	返回 this.numberString


        //字段：
        //3. number
        //5. numberBits
        //2. numberString
        //1. originalString
        //4. sign


        //索引器：
        //this[int bitIndex]	→	返回 this.numberBits[i];


        //构造函数：
        //Number()


        //方法：
        //ToBits()	→	通过 this.numberString 来设置 this.numberBits
        //ToInt32()	→	返回 this.number，如果溢出则抛异常
        //ToInt64()	→	返回 this.number
        //ToString()	→	返回 this.number
        //ToFullBitString()	→	返回 this.numberString
        //-1. Initial()
        //-1. HasSign()	→	通过 this.originalString 来判断。

        //-1. GetHashCode()
        //-1. Equals()
        #endregion










        /// <summary>
        /// 用来存储当前数字对象的（不带符号的）字符串形式值。
        /// </summary>
        private string numberString;

        /// <summary>
        /// 用来存储当创建前数字对象时所提供的原始字符串值（原始数值字符串是什么样子该属性值就是什么样子）。
        /// </summary>
        private string originalString;
        /// <summary>
        /// 获取创建当前数值对象时所提供的原始数值字符串（原始数值字符串是什么样子该属性值就是什么样子）。
        /// </summary>
        /// <returns></returns>
        public string OriginalString
        {
            get
            {
                return this.originalString;
            }
        }

        /// <summary>
        /// 用来存储当前数字对象的 long 形式值。
        /// </summary>
        private long number;

        /// <summary>
        /// 用来存储当前数字对象的前置符号（正号 “+” 或负号 “-”），如果没有则为默认值 null。
        /// </summary>
        private string sign;

        /// <summary>
        /// 用来存储当前数字对象数位从低往高的每一位数的列表，例如对象 “12345” 的列表元素依次为：5、4、3、2、1。
        /// </summary>
        //该字段的初始化在当前类下的 ToBits() 方法中进行，而 ToBits() 方法在当前类的构造函数中就被调用，所以当创建当前类的对象时该字段就已经被初始化了。
        private List<byte> numberBits;


        /// <summary>
        /// 显示整数值对象的绝对值字符串。
        /// </summary>
        public string AbsoluteValueString
        {
            get
            {
                return this.numberString;
            }
        }



        public Number(string numberString)
        {
            numberString.ValidateStringIsNumber();
            this.originalString = numberString;
            try
            {
                this.numberString = this.originalString;
                this.number = long.Parse(this.numberString);
                if (HasSign())
                {
                    this.sign = Convert.ToString(this.numberString[0]);
                    this.numberString = this.numberString.Substring(1);
                }
                //由于 numberBits 字段的初始化在 ToBits() 方法中进行，而 numberBits 字段的初始化又需要用到当前类的 numberString 字
                //段，所以在调用 ToBits() 方法之前必须先完成对 numberString 字段的初始化操作，因此特意在调用 ToBits() 方法之前先完
                //成对 numberString 的初始化赋值（见当前的 try 块里的第 1 行）。
                ToBits();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("所传入的数值格式非法！必须为整数数值类型。", ex);
            }
        }


        public void Initial(string numberString)
        {
            numberString.ValidateStringIsNumber();
            this.originalString = numberString.Trim();
            try
            {
                this.numberString = this.originalString;
                this.number = long.Parse(this.numberString);
                if (HasSign())
                {
                    this.sign = Convert.ToString(this.numberString[0]);
                    this.numberString = this.numberString.Substring(1);
                }
                //由于 numberBits 字段的初始化在 ToBits() 方法中进行，而 numberBits 字段的初始化又需要用到当前类的 numberString 字
                //段，所以在调用 ToBits() 方法之前必须先完成对 numberString 字段的初始化操作，因此特意在调用 ToBits() 方法之前先完
                //成对 numberString 的初始化赋值（见当前的 try 块里的第 1 行）。
                ToBits();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("所传入的数值格式非法！必须为整数数值类型。", ex);
            }
        }



        #region 业务上好像没用。
        //public Number(long numberLong)
        //{
        //    this.number = numberLong;
        //    this.numberString = Convert.ToString(numberLong);
        //    if (HasSign(this.numberString))
        //    {
        //        this.sign = Convert.ToString(this.numberString[0]);
        //        this.numberString = this.numberString.Substring(1);
        //    }
        //    ToBits();
        //}
        #endregion

        /// <summary>
        /// 获取从低位到高位的指定索引值所对应的位的数字。
        /// </summary>
        /// <param name="bitIndex"></param>
        /// <returns></returns>
        public int this[int bitIndex]
        {
            get
            {
                return this.numberBits[bitIndex];
            }
        }

        /// <summary>
        /// 用来获取当前数字对象的值是否为正。
        /// </summary>
        public bool IsPositive
        {
            get
            {
                return this.number > 0;
            }
        }

        /// <summary>
        /// 用来获取当前数字对象的值是否为负。
        /// </summary>
        public bool IsNegative
        {
            get
            {
                return this.number < 0;
            }
        }

        /// <summary>
        /// 当前数值对象（不包括符号位）的位数长度。
        /// </summary>
        public int Length
        {
            get
            {
                return this.numberBits.Count;
            }
        }

        /// <summary>
        /// 用来获取当前数字对象的原始字符串（this.originalString）是否含有前置的正号 “+” 或负号 “-”。
        /// </summary>
        private bool HasSign()
        {
            return this.originalString.StartsWith("+") || this.originalString.StartsWith("-");
        }

        /// <summary>
        /// 将当前的数值对象（假设值为 “-0012345”）按照 sortType 参数所指定的存储顺序转换为形如 “5 4 3 2 1 0 0”（或是 “0 0 1 2 3 4 5”）的 byte 类型列表（符号位不作任何处理，直接忽略）。
        /// 将当前的数值对象（假设值为 “-0012345”）转换为形如 5 4 3 2 1 0 0 的 byte 类型列表（符号位不作任何处理，直接忽略），顺序为随着列表索引值的递增，从数值的最低位开始，数位从低往高（即：个、十、百、千、万。。。）。
        /// </summary>
        /// <param name="sortType">用来设置按何种顺序存储整数中的每一位的枚举，默认为 FromLowToHigh，即把整数按从低位到高位的顺序依次存储到 byte 类型的列表对象中。</param>
        /// <returns></returns>
        public List<byte> ToBits(SortType sortType = SortType.FromLowToHigh)
        {
            if (this.numberBits == null)
            {
                this.numberBits = new List<byte>(this.numberString.Length);
                //从 numberString 从左到右的最后一位开始，逐位依次复制出来并依次添加到 this.numberBits 字段中的末尾。
                for (int i = this.numberString.Length - 1; i >= 0; i--)
                {
                    this.numberBits.Add(Convert.ToByte(this.numberString[i].ToString()));
                }
            }

            List<byte> result = new List<byte>(this.numberBits);       //复制一份返回到外部，以防止外部直接修改当前对象内的 numberBits 对象。
            if (sortType == SortType.FromHighToLow)
            {
                result.Reverse();
            }
            return result;
        }





        public long ToInt64()
        {
            return this.number;
        }

        public int ToInt32()
        {
            if (this.number > int.MaxValue || this.number < int.MinValue)
            {
                throw new ApplicationException("数值超出范围！");
            }
            return (int)this.number;
        }

        /// <summary>
        /// 显示 Number 对象的逻辑值（即省略了前导 0 和前导正号 + 的整数值）的字符串形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.number.ToString();
        }

        /// <summary>
        /// 显示 Number 对象的全位（即创建时的字符串所表示的数值部分是多少位就有多少位，就算有前导 0 也不例外）字符串形式，如果数值为负则还带有 “-” 符号。
        /// </summary>
        /// <returns></returns>
        public string ToFullBitString()
        {
            string sig = string.Empty;
            if (this.number != 0)
            {
                //只有符号位 sign 为 “-” 时才显示符号位，否则无论符号位是 null 还是 “+” 都不作显示。
                sig = (this.sign == null || this.sign == "+") ? sig : "-";
            }
            return sig + this.numberString;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Number)
            {
                Number that = obj as Number;
                if (this.originalString == that.originalString)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ToInt32();
        }

        public static Number operator ++(Number n1)
        {
            return n1 + "1".ToNumber();
        }

        public static Number operator +(Number n1, Number n2)
        {
            if (n1 == null)
            {
                throw new ArgumentNullException("n1");
            }
            if (n2 == null)
            {
                throw new ArgumentNullException("n2");
            }
            int length = Math.Max(n1.Length, n2.Length);
            long sum = n1.number + n2.number;
            string sumString = GetSpecifiedLengthNumberString(sum, length);
            return sumString.ToNumber();
        }

        /// <summary>
        /// 把一个 long 类型的整数转换成有指定长度 bitCount 的字符串，如果整数的长度不足以达到 bitCount 指定的长度，则以前导 0 来补足。
        /// </summary>
        /// <param name="sourceNumber">要将其转换成字符串的 long 类型整数。</param>
        /// <param name="bitCount">最终要求返回的字符串的长度。</param>
        /// <returns></returns>
        private static string GetSpecifiedLengthNumberString(long sourceNumber, int bitCount)
        {
            long tmpNumber = sourceNumber,bit = 10, tmpBit;
            StringBuilder sb = new StringBuilder(bitCount);
            if (sourceNumber != 0)
            {
                while ((tmpBit = tmpNumber % bit) != 0)
                {
                    sb.Insert(0, tmpBit);
                    tmpNumber /= 10;
                }
            }
            while(sb.Length < bitCount)
            {
                sb.Insert(0, '0');
            }
            if(sourceNumber < 0)
            {
                sb.Insert(0, '-');
            }
            return sb.ToString();
        }

        public object Clone()
        {
            return new Number(new string(this.originalString.ToArray()));
        }
    }
}
