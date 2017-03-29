namespace Dorado.Core.GlobalTimer.TimerStrategies
{
    /// <summary>
    /// 基于定时器频率的定时策略
    /// </summary>
    [TimerStrategy("globalTimerInterval", typeof(Creator))]
    public class GlobalTimerIntervalTimerStrategy : ITimerStrategy
    {
        #region ITimerStrategy Members

        public bool IsTimeUp()
        {
            return true;
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
                return new GlobalTimerIntervalTimerStrategy();
            }
        }

        #endregion Class Creator ...
    }
}