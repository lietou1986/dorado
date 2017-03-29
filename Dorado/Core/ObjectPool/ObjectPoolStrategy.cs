using System.Diagnostics.Contracts;
using System.Threading;

namespace Dorado.Core.ObjectPool
{
    /// <summary>
    /// 默认的对象池策略
    /// </summary>
    internal class ObjectPoolStrategy : IObjectPoolStrategy
    {
        public ObjectPoolStrategy(int maxCount)
        {
            Contract.Requires(maxCount > 0);
            _MaxCount = maxCount;
        }

        private readonly int _MaxCount;
        private volatile int _TotalCount, _ReadyCount;
        private readonly AutoResetEvent _SyncEvent = new AutoResetEvent(true);
        private readonly AutoResetEvent _WaitForResourceEvent = new AutoResetEvent(false);

        #region IObjectPoolStrategy Members

        public bool Wait(int count, int timeoutMillsecounds)
        {
            if (!_SyncEvent.WaitOne(timeoutMillsecounds))
                return false;

            while (_TotalCount + (count - _ReadyCount) > _MaxCount)
            {
                _WaitForResourceEvent.WaitOne();
            }

            return true;
        }

        public void AccquireNotify(int count, int newCount)
        {
            _TotalCount += newCount;
            _ReadyCount -= count - newCount;

            _SyncEvent.Set();
        }

        public void ReleaseNotify(int count)
        {
            _ReadyCount += count;
            _WaitForResourceEvent.Set();
        }

        public int TrimNotify()
        {
            return _MaxCount;
        }

        #endregion IObjectPoolStrategy Members
    }
}