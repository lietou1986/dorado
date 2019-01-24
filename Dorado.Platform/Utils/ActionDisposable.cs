using System;

namespace Dorado.Platform.Utils
{
    /// <summary>
    /// Allows action to be executed, when it is disposed
    /// </summary>
    internal struct ActionDisposable : IDisposable
    {
        private readonly Action _action;

        public static readonly ActionDisposable Empty = new ActionDisposable(() => { });

        public ActionDisposable(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}