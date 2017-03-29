using Dorado.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Dorado.Core.GlobalTimer.TimerStrategies
{
    /// <summary>
    /// 每天的定时任务
    /// </summary>
    [TimerStrategy("everyDay", typeof(Creator))]
    public class EveryDayTimerStrategy : ITimerStrategy
    {
        public EveryDayTimerStrategy(IEnumerable<TimeSpan> times)
        {
            Contract.Requires(times != null);

            _Times = times.OrderBy(t => t.Ticks).ToArray();
            DateTime now = DateTime.Now, today = now.Date;
            TimeSpan timeOfDay = now.TimeOfDay;
            _NextTime = _Times.Length == 0 ? DateTime.MaxValue :
                timeOfDay > _Times[_Times.Length - 1] ? (today.AddDays(1) + _Times[0]) :
                today + _Times[_Index = Array.FindIndex<TimeSpan>(_Times, t => t >= timeOfDay)];
        }

        private readonly TimeSpan[] _Times;
        private int _Index;
        private DateTime _NextTime;

        #region ITimerStrategy Members

        public bool IsTimeUp()
        {
            if (CommonExtension.QuickGetTime() >= _NextTime)
            {
                if (++_Index >= _Times.Length)
                {
                    _Index = 0;
                    _NextTime = DateTime.Today.AddDays(1) + _Times[_Index];
                }
                else
                {
                    _NextTime = DateTime.Today + _Times[_Index];
                }

                return true;
            }

            return false;
        }

        public virtual void RunNotify()
        {
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
                if (s == null)
                    throw new FormatException("The format of argument context is not correct");

                TimeSpan ts = default(TimeSpan);
                var list = from item in s.Split(',')
                           where TimeSpan.TryParse(item, out ts)
                           select ts;

                return new EveryDayTimerStrategy(list);
            }
        }

        #endregion Class Creator ...
    }
}