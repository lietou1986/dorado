namespace Dorado.Core.GlobalTimer.TimerStrategies
{
    /// <summary>
    /// 仅执行一次的定时策略
    /// </summary>
    [TimerStrategy("onlyOnce", typeof(Creator))]
    public class OnlyOnceTimerStrategy : ITimerStrategy
    {
        private bool isAlreadyRun = false;

        #region ITimerStrategy Members

        public bool IsTimeUp()
        {
            return !isAlreadyRun;
        }

        public virtual void RunNotify()
        {
            isAlreadyRun = true;
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
                return new OnlyOnceTimerStrategy();
            }
        }

        #endregion Class Creator ...
    }
}