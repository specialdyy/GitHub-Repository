using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    [Serializable]
    public class RepeatInfo
    {
        public static readonly TimeSpan MaxDueTime = TimeSpan.FromHours(10);
        public static readonly TimeSpan MinDueTime = TimeSpan.FromMilliseconds(-1);     //-1 是 Timeout.Infinity 的值，在 Threading.Timer 类中有特殊用途。
        public static readonly TimeSpan MaxPeriod = TimeSpan.FromHours(10);
        public static readonly TimeSpan MinPeriod = TimeSpan.FromMilliseconds(-1);      //-1 是 Timeout.Infinity 的值，在 Threading.Timer 类中有特殊用途。

        #region 构造函数和初始化函数。
        public RepeatInfo(TimeSpan dueTime, TimeSpan period)
        {
            Change(dueTime, period);
        }

        public RepeatInfo(Func<DateTime, TimeSpan> dueTimeCalculator, TimeSpan period)
        {
            Change(dueTimeCalculator, period);
        }

        public RepeatInfo(TimeSpan dueTime, Func<DateTime, TimeSpan> periodCalculator)
        {
            Change(dueTime, periodCalculator);
        }

        public RepeatInfo(Func<DateTime, TimeSpan> dueTimeCalculator, Func<DateTime, TimeSpan> periodCalculator)
        {
            Change(dueTimeCalculator, periodCalculator);
        }


        public void Change(TimeSpan dueTime, TimeSpan period)
        {
            ////不需要校验 dueTime，因为在给 this.DueTime 属性赋值的操作中，内部已经进行了同样的校验了。
            //ValidateTimeSpanRange(dueTime, MinDueTime, MaxDueTime, "dueTime");
            ////不需要校验 period，因为在给 this.Period 属性赋值的操作中，内部已经进行了同样的校验了。
            //ValidateTimeSpanRange(period, MinPeriod, MaxPeriod, "period");
            this.DueTime = dueTime;
            this.Period = period;
        }

        public void Change(Func<DateTime, TimeSpan> dueTimeCalculator, TimeSpan period)
        {
            ////不需要校验 period，因为在给 this.Period 属性赋值的操作中，内部已经进行了同样的校验了。
            //ValidateTimeSpanRange(period, MinPeriod, MaxPeriod, "period");
            this.DueTimeCalculator = dueTimeCalculator ?? throw new ArgumentNullException("dueTimeCalculator");
            this.Period = period;
        }

        public void Change(TimeSpan dueTime, Func<DateTime, TimeSpan> periodCalculator)
        {
            ////不需要校验 dueTime，因为在给 this.DueTime 属性赋值的操作中，内部已经进行了同样的校验了。
            //ValidateTimeSpanRange(dueTime, MinDueTime, MaxDueTime, "dueTime");
            this.DueTime = dueTime;
            this.PeriodCalculator = periodCalculator ?? throw new ArgumentNullException("periodCalculator");
        }

        public void Change(Func<DateTime, TimeSpan> dueTimeCalculator, Func<DateTime, TimeSpan> periodCalculator)
        {
            this.DueTimeCalculator = dueTimeCalculator ?? throw new ArgumentNullException("dueTimeCalculator");
            this.PeriodCalculator = periodCalculator ?? throw new ArgumentNullException("periodCalculator");
        }
        #endregion

        private bool ValidateTimeSpanRange(TimeSpan timeSpan, TimeSpan minValue, TimeSpan maxValue, string parameterName)
        {
            if (dueTime < minValue || dueTime > maxValue)
            {
                throw new ArgumentOutOfRangeException(parameterName, "参数或属性 " + parameterName + " 的值超出所允许的最大范围（" + minValue.TotalSeconds + "s-" + maxValue.TotalHours + "h）！");
            }
            return true;
        }


        /// <summary>
        /// 通过当前的 RepeatInfo 类型对象的属性来更新 Timer 对象的触发状态和参数。
        /// </summary>
        /// <param name="timer"></param>
        public void ChangeTimer(Timer timer)
        {
            if(timer == null)
            {
                throw new ArgumentNullException("timer");
            }
            timer.Change(this.DueTime, this.Period);
        }


        private TimeSpan dueTime;
        /// <summary>
        /// 下一次触发定时器之前的延迟时间。
        /// 如果 “延迟时间计算器” 属性 DueTimeCalculator 不为空，则无论 dueTime 字段是否已经设置值，都优先通过该属性中的委托来计算出延迟时间值。
        /// 即 DueTimeCalculator 会强行覆盖 dueTime 的值。
        /// </summary>
        public TimeSpan DueTime
        {
            get
            {
                if(this.DueTimeCalculator != null)
                {
                    this.dueTime = this.DueTimeCalculator(DateTime.Now);
                    if(this.dueTime < MinDueTime || this.dueTime > MaxDueTime)
                    {
                        throw new ArgumentOutOfRangeException("DueTimeCalculator", "通过属性 DueTimeCalculator 计算出的延迟时间值超出所允许的最大范围（" + MinDueTime.TotalSeconds + "s-" + MaxDueTime.TotalHours + "h）！");
                    }
                }
                return this.dueTime;
            }
            set
            {
                ValidateTimeSpanRange(value, MinDueTime, MaxDueTime, "DueTime");
                this.dueTime = value;
                //一旦 dueTime 的值被修改，则说明使用者有意修改该值，所以在此清空 DueTimeCalculator 属性中的委托，以防该委托的赋值逻辑继续覆盖当前的修改操作。
                this.DueTimeCalculator = null;
            }
        }

        private TimeSpan period;
        /// <summary>
        /// 每一次触发定时器之间的时间间隔。
        /// 如果 “间隔时间计算器” 属性 PeriodCalculator 不为空，则无论 period 字段是否已经设置值，都优先通过该属性中的委托来计算出间隔时间值。
        /// 即 PeriodCalculator 会强行覆盖 period 的值。
        /// </summary>
        public TimeSpan Period
        {
            get
            {
                if (this.PeriodCalculator != null)
                {
                    this.period = this.PeriodCalculator(DateTime.Now);
                    if (this.period < MinPeriod || this.period > MaxPeriod)
                    {
                        throw new ArgumentOutOfRangeException("PeriodCalculator", "通过属性 PeriodCalculator 计算出的间隔时间值超出所允许的最大范围（" + MinPeriod.TotalSeconds + "s-" + MaxPeriod.TotalHours + "h）！");
                    }
                }
                return this.period;
            }
            set
            {
                ValidateTimeSpanRange(value, MinPeriod, MaxPeriod, "Period");
                this.period = value;
                //一旦 period 的值被修改，则说明使用者有意修改该值，所以在此清空 PeriodCalculator 属性中的委托，以防该委托的赋值逻辑继续覆盖当前的修改操作。
                this.PeriodCalculator = null;
            }
        }


        public Func<DateTime, TimeSpan> DueTimeCalculator
        {
            private get;
            set;
        }


        public Func<DateTime, TimeSpan> PeriodCalculator
        {
            private get;
            set;
        }
    }
}
