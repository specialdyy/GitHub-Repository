using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class GlobalSettings
    {
        #region 预定义常量。
        /// <summary>
        /// 保留定码（前二码/后二码）个数的最大值。
        /// </summary>
        public const int MAX_DING_MA_COUNT = 80;
        /// <summary>
        /// 保留定码（前二码/后二码）个数的最小值。
        /// </summary>
        public const int MIN_DING_MA_COUNT = 1;
        
        /// <summary>
        /// 排除最新期数号的最大值。
        /// </summary>
        public const int MAX_NEWEST_PERIOD_COUNT = 100;
        /// <summary>
        /// 排除最新期数号的最小值。
        /// </summary>
        public const int MIN_NEWEST_PERIOD_COUNT = 0;


        public const int DEFAULT_NEWEST_PERIOD_COUNT = 30;

        public const int DEFAULT_DING_MA_COUNT = 70;

        public const ForecastType DEFAULT_FORECAST_TYPE = ForecastType.LastTwo;
        #endregion
    }
}
