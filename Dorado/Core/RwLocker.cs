using System;
using System.Diagnostics;
using System.Threading;

namespace Dorado.Core
{
    /// <summary>
    /// 读写锁
    /// </summary>
    public class RwLocker
    {
        public RwLocker()
        {
            _ReadLocker = new ReadLocker(_Locker);
            _WriteLocker = new WriteLocker(_Locker);
            _UpgradeLocker = new UpgradeLocker(_Locker);
        }

        private readonly ReaderWriterLockSlim _Locker = new ReaderWriterLockSlim();

        /// <summary>
        /// 进入读模式
        /// </summary>
        /// <returns></returns>
        public IDisposable Read()
        {
            _Locker.EnterReadLock();
            return _ReadLocker;
        }

        private readonly IDisposable _ReadLocker, _WriteLocker, _UpgradeLocker;

        /// <summary>
        /// 尝试进入读模式
        /// </summary>
        /// <param name="stub"></param>
        /// <param name="millsecondsTimeout"></param>
        /// <returns></returns>
        public bool TryRead(out IDisposable stub, int millsecondsTimeout = 0)
        {
            if (_Locker.TryEnterReadLock(millsecondsTimeout))
            {
                stub = _ReadLocker;
                return true;
            }

            stub = null;
            return false;
        }

        /// <summary>
        /// 进入写模式
        /// </summary>
        /// <returns></returns>
        public IDisposable Write()
        {
            _Locker.EnterWriteLock();
            return _WriteLocker;
        }

        /// <summary>
        /// 尝试进入写模式
        /// </summary>
        /// <param name="stub"></param>
        /// <param name="millsecondsTimeout"></param>
        /// <returns></returns>
        public bool TryWrite(out IDisposable stub, int millsecondsTimeout = 0)
        {
            if (_Locker.TryEnterWriteLock(millsecondsTimeout))
            {
                stub = _WriteLocker;
                return true;
            }

            stub = null;
            return false;
        }

        /// <summary>
        /// 从读模式升级到写模式
        /// </summary>
        /// <returns></returns>
        public IDisposable Upgrade()
        {
            _Locker.EnterUpgradeableReadLock();
            return _UpgradeLocker;
        }

        /// <summary>
        /// 尝试从读模式升级到写模式
        /// </summary>
        /// <param name="stub"></param>
        /// <param name="millsecondsTimeout"></param>
        /// <returns></returns>
        public bool TryUpgrade(out IDisposable stub, int millsecondsTimeout = 0)
        {
            if (_Locker.TryEnterUpgradeableReadLock(millsecondsTimeout))
            {
                stub = _UpgradeLocker;
                return true;
            }

            stub = null;
            return false;
        }
    }

    internal class ReadLocker : IDisposable
    {
        public ReadLocker(ReaderWriterLockSlim locker)
        {
            _Locker = locker;
        }

        private readonly ReaderWriterLockSlim _Locker;

        #region IDisposable Members

        public void Dispose()
        {
            _Locker.ExitReadLock();
        }

        #endregion IDisposable Members
    }

    internal class WriteLocker : IDisposable
    {
        public WriteLocker(ReaderWriterLockSlim locker)
        {
            _Locker = locker;
        }

        private readonly ReaderWriterLockSlim _Locker;

        #region IDisposable Members

        public void Dispose()
        {
            _Locker.ExitWriteLock();
        }

        #endregion IDisposable Members
    }

    internal class UpgradeLocker : IDisposable
    {
        public UpgradeLocker(ReaderWriterLockSlim locker)
        {
            _Locker = locker;
        }

        private readonly ReaderWriterLockSlim _Locker;

        #region IDisposable Members

        public void Dispose()
        {
            _Locker.ExitUpgradeableReadLock();
        }

        #endregion IDisposable Members
    }

    public static class LockExtensions
    {
        /// <summary>
        /// Acquires a disposable reader lock that can be used with a using statement.
        /// </summary>
        [DebuggerStepThrough]
        public static IDisposable Read(this ReaderWriterLockSlim rwLock)
        {
            return rwLock.TryRead(-1);
        }

        /// <summary>
        /// Acquires a disposable reader lock that can be used with a using statement.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or -1 to wait indefinitely.
        /// </param>
        [DebuggerStepThrough]
        public static IDisposable TryRead(this ReaderWriterLockSlim rwLock, int millisecondsTimeout)
        {
            bool acquire = rwLock.IsReadLockHeld == false ||
                           rwLock.RecursionPolicy == LockRecursionPolicy.SupportsRecursion;

            if (acquire)
            {
                if (rwLock.TryEnterReadLock(millisecondsTimeout))
                {
                    return new ReadLocker(rwLock);
                }
            }

            return ActionDisposable.Empty;
        }

        /// <summary>
        /// Acquires a disposable and upgradeable reader lock that can be used with a using statement.
        /// </summary>
        [DebuggerStepThrough]
        public static IDisposable Upgrade(this ReaderWriterLockSlim rwLock)
        {
            return rwLock.TryUpgrade(-1);
        }

        /// <summary>
        /// Acquires a disposable and upgradeable reader lock that can be used with a using statement.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or -1 to wait indefinitely.
        /// </param>
        [DebuggerStepThrough]
        public static IDisposable TryUpgrade(this ReaderWriterLockSlim rwLock, int millisecondsTimeout)
        {
            bool acquire = rwLock.IsUpgradeableReadLockHeld == false ||
                           rwLock.RecursionPolicy == LockRecursionPolicy.SupportsRecursion;

            if (acquire)
            {
                if (rwLock.TryEnterUpgradeableReadLock(millisecondsTimeout))
                {
                    return new UpgradeLocker(rwLock);
                }
            }

            return ActionDisposable.Empty;
        }

        /// <summary>
        /// Acquires a disposable writer lock that can be used with a using statement.
        /// </summary>
        [DebuggerStepThrough]
        public static IDisposable Write(this ReaderWriterLockSlim rwLock)
        {
            return rwLock.TryWrite(-1);
        }

        /// <summary>
        /// Tries to enter a disposable write lock that can be used with a using statement.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or -1 to wait indefinitely.
        /// </param>
        [DebuggerStepThrough]
        public static IDisposable TryWrite(this ReaderWriterLockSlim rwLock, int millisecondsTimeout)
        {
            bool acquire = rwLock.IsWriteLockHeld == false ||
                           rwLock.RecursionPolicy == LockRecursionPolicy.SupportsRecursion;

            if (acquire)
            {
                if (rwLock.TryEnterWriteLock(millisecondsTimeout))
                {
                    return new WriteLocker(rwLock);
                }
            }

            return ActionDisposable.Empty;
        }
    }
}