using System;

namespace Dorado.Core.SuperCache
{
    public interface IAsyncTokenProvider
    {
        IVolatileToken GetToken(Action<Action<IVolatileToken>> task);
    }
}