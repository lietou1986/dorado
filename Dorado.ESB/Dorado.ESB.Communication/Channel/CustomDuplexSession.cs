using System;
using System.ServiceModel.Channels;

namespace Dorado.ESB.Communication
{
    public class CustomDuplexSession : IDuplexSession
    {
        #region fields

        private CustomDuplexSessionChannel channel;
        private string id;

        #endregion fields

        #region ctor

        public CustomDuplexSession(CustomDuplexSessionChannel channel)
        {
            this.channel = channel;
            this.id = Guid.NewGuid().ToString();
        }

        #endregion ctor

        #region IDuplexSession Members

        public void CloseOutputSession(TimeSpan timeout)
        {
            this.channel.CloseOutput(timeout);
        }

        public void CloseOutputSession()
        {
            this.channel.CloseOutput();
        }

        public IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
        {
            CloseOutputSession(timeout);
            return new CompletedAsyncResult(callback, state);
        }

        public IAsyncResult BeginCloseOutputSession(AsyncCallback callback, object state)
        {
            CloseOutputSession();
            return new CompletedAsyncResult(callback, state);
        }

        public void EndCloseOutputSession(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        #endregion IDuplexSession Members

        #region ISession Members

        public string Id
        {
            get { return this.id; }
        }

        #endregion ISession Members
    }
}