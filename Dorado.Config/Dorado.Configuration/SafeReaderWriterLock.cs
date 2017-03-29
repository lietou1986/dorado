using System;
using System.Threading;

namespace Dorado.Configuration
{
    public interface ISafeLock : IDisposable
    {
        void AcquireLock();
    }

    public class SafeReaderWriterLock
    {
        private SafeUpgradeReaderLock upgradableReaderLock;
        private SafeReaderLock readerLock;
        private SafeWriterLock writerLock;

        private ReaderWriterLockSlim locker;

        public SafeReaderWriterLock()
        {
            locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            upgradableReaderLock = new SafeUpgradeReaderLock(locker);
            readerLock = new SafeReaderLock(locker);
            writerLock = new SafeWriterLock(locker);
        }

        public ISafeLock AcquireReaderLock()
        {
            readerLock.AcquireLock();
            return readerLock;
        }

        public ISafeLock AcquireWriterLock()
        {
            writerLock.AcquireLock();
            return writerLock;
        }

        public ISafeLock AcquireUpgradeableReaderLock()
        {
            upgradableReaderLock.AcquireLock();
            return upgradableReaderLock;
        }
    }

    internal class SafeReaderLock : ISafeLock
    {
        private ReaderWriterLockSlim locker;

        public SafeReaderLock(ReaderWriterLockSlim locker)
        {
            this.locker = locker;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            locker.ExitReadLock();
        }

        #endregion IDisposable 成员

        #region ISafeLock 成员

        public void AcquireLock()
        {
            locker.EnterReadLock();
        }

        #endregion ISafeLock 成员
    }

    internal class SafeWriterLock : ISafeLock
    {
        private ReaderWriterLockSlim locker;

        public SafeWriterLock(ReaderWriterLockSlim locker)
        {
            this.locker = locker;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            locker.ExitWriteLock();
        }

        #endregion IDisposable 成员

        #region ISafeLock 成员

        public void AcquireLock()
        {
            locker.EnterWriteLock();
        }

        #endregion ISafeLock 成员
    }

    internal class SafeUpgradeReaderLock : ISafeLock
    {
        private ReaderWriterLockSlim locker;

        public SafeUpgradeReaderLock(ReaderWriterLockSlim locker)
        {
            this.locker = locker;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            locker.ExitUpgradeableReadLock();
        }

        #endregion IDisposable 成员

        #region ISafeLock 成员

        public void AcquireLock()
        {
            locker.EnterUpgradeableReadLock();
        }

        #endregion ISafeLock 成员
    }
}