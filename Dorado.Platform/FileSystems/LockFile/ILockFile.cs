using System;

namespace Dorado.Platform.FileSystems.LockFile
{
    public interface ILockFile : IDisposable
    {
        void Release();
    }
}