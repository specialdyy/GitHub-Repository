using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// 用来描述把整数的字符串转换为字节列表时的对应顺序的枚举。
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// 表示随着字节列表中索引值的递增，所对应存储的整数位数依次为从低位到高位（把整数按从低位到高位的顺序依次存储到 byte 类型的列表对象中）。
        /// 例如：整数 “12345”（5 为低位 1 为高位）会被存储为 { 5, 4, 3, 2, 1 }。
        /// </summary>
        FromLowToHigh,

        /// <summary>
        /// 表示随着字节列表中索引值的递增，所对应存储的整数位数依次为从高位到低位（把整数按从高位到低位的顺序依次存储到 byte 类型的列表对象中）。
        /// 例如：整数 “12345”（5 为低位 1 为高位）会被存储为 { 1, 2, 3, 4, 5 }。
        /// </summary>
        FromHighToLow,
    }
}
