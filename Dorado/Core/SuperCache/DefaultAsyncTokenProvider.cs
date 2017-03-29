using Dorado.Core.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Dorado.Core.SuperCache
{
    public class DefaultAsyncTokenProvider : IAsyncTokenProvider
    {
        public IVolatileToken GetToken(Action<Action<IVolatileToken>> task)
        {
            var token = new AsyncVolativeToken(task);
            token.QueueWorkItem();
            return token;
        }

        public class AsyncVolativeToken : IVolatileToken
        {
            private readonly Action<Action<IVolatileToken>> _task;
            private readonly List<IVolatileToken> _taskTokens = new List<IVolatileToken>();
            private volatile Exception _taskException;
            private volatile bool _isTaskFinished;

            public AsyncVolativeToken(Action<Action<IVolatileToken>> task)
            {
                _task = task;
            }

            public void QueueWorkItem()
            {
                // Start a work item to collect tokens in our internal array
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        _task(token => _taskTokens.Add(token));
                    }
                    catch (Exception e)
                    {
                        LoggerWrapper.Logger.Error("Error while monitoring extension files. Assuming extensions are not current.", e);
                        _taskException = e;
                    }
                    finally
                    {
                        _isTaskFinished = true;
                    }
                });
            }

            public bool IsCurrent
            {
                get
                {
                    // We are current until the task has finished
                    if (_taskException != null)
                    {
                        return false;
                    }
                    if (_isTaskFinished)
                    {
                        return _taskTokens.All(t => t.IsCurrent);
                    }
                    return true;
                }
            }
        }
    }
}