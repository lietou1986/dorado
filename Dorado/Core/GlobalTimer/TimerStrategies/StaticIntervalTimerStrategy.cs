using System;
using System.Diagnostics.Contracts;

namespace Dorado.Core.GlobalTimer.TimerStrategies
{
    /// <summary>
    /// 固定周期定时方式
    /// </summary>
    [TimerStrategy("staticInterval", typeof(Creator))]
    public class StaticIntervalTimerStrategy : ITimerStrategy
    {
        public StaticIntervalTimerStrategy(TimeSpan interval)
        {
            Contract.Requires(interval.Ticks > 0);

            _Interval = interval;
            _NextRunTime = DateTime.Now + _Interval;
        }

        private readonly TimeSpan _Interval;
        private DateTime _NextRunTime;

        #region ITimerStrategy Members

        public bool IsTimeUp()
        {
            return DateTime.Now >= _NextRunTime;
        }

        public void RunNotify()
        {
            _NextRunTime = DateTime.Now + _Interval;
        }

        public virtual void CompletedNotify()
        {
        }

        #endregion ITimerStrategy Members

        #region Class Creator ...

        private class Creator : IObjectCreator
        {
            public object CreateObject(object context = null, IObjectCreator innerCreator = null)
            {
                string s = context as string;
                TimeSpan ts;
                if (s == null || !TimeSpan.TryParse(s, out ts))
                    throw new FormatException("The format of argument 'context' is not correct");

                return new StaticIntervalTimerStrategy(ts);
            }
        }

        #endregion Class Creator ...
    }
}