﻿using Dorado.Core.SuperCache;

namespace Dorado.Platform.FileSystems.VirtualPath
{
    /// <summary>
    /// Enable monitoring changes over virtual path
    /// </summary>
    public interface IVirtualPathMonitor : IVolatileProvider
    {
        IVolatileToken WhenPathChanges(string virtualPath);
    }
}