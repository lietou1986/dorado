using System.Diagnostics.Contracts;
using System.Threading;

namespace Dorado.Core
{
    /// <summary>
    /// 自定义用于任务调度的信号量(还有bug)
    /// </summary>
    public class Semaphore
    {
        private ManualResetEvent _waitEvent = new ManualResetEvent(false);
        private static object _syncObjWait = new object();
        private int _maxCount = 1;
        private int _currentCount = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="max">信号个数</param>
        public Semaphore(int max)
        {
            Contract.Requires(max > 0);

            _maxCount = max;
        }

        #region ITaskSemaphore Members

        public bool Wait()
        {
            lock (_syncObjWait)//只能一个线程进入下面代码
            {
                bool waitResult = _waitEvent.WaitOne();//在此等待资源数大于零
                if (waitResult)
                {
                    lock (this)
                    {
                        if (_currentCount > 0)
                        {
                            _currentCount--;
                            if (_currentCount == 0)
                            {
                                _waitEvent.Reset();
                            }
                        }
                        else
                        {
                            Contract.Requires(_currentCount >= 0, " not allow current count < 0");
                        }
                    }
                }
                return waitResult;
            }
        }

        /// 允许超时返回的 Wait 操作
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        public bool Wait(int millisecondsTimeout)
        {
            lock (_syncObjWait) // Monitor 确保该范围类代码在临界区内
            {
                bool waitResult = _waitEvent.WaitOne(millisecondsTimeout, false);
                if (waitResult)
                {
                    lock (this)
                    {
                        if (_currentCount > 0)
                        {
                            _currentCount--;
                            if (_currentCount == 0)
                            {
                                _waitEvent.Reset();
                            }
                        }
                        else
                        {
                            Contract.Requires(_currentCount >= 0, " not allow current count < 0");
                        }
                    }
                }
                return waitResult;
            }
        }

        public bool Release(int count = 1)
        {
            lock (this) // Monitor 确保该范围类代码在临界区内
            {
                _currentCount += count;
                if (_currentCount > _maxCount)
                {
                    _currentCount = _maxCount;
                    return false;
                }
                _waitEvent.Set();//允许调用Wait的线程进入
                return true;
            }
        }

        #endregion ITaskSemaphore Members

        #region IDisposable Members

        public void Dispose()
        {
            _waitEvent.Dispose();
        }

        #endregion IDisposable Members
    }
}