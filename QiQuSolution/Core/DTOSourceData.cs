using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    [Serializable]
    public class DTOSourceData
    {
        /// <summary>
        /// 数据的记录时间
        /// </summary>
        public DateTime OnlineTime
        {
            get;
            set;
        }

        /// <summary>
        /// 在线人数
        /// </summary>
        public string OnlineNumber
        {
            get;
            set;
        }
        
        /// <summary>
        /// 前后两期（本期和上一期）在线人数的变化人数值（本期人数 - 上期人数的差值）。
        /// </summary>
        public string OnlineChange
        {
            get;
            set;
        }
    }
}
