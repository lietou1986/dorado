using System;
using System.Diagnostics;

namespace Dorado.Windows.SingleInstancing
{
    [Serializable]
    public abstract class SingleAppController
    {
        #region Fields

        protected SingleInstanceTracker Tracker { get; private set; }

        #endregion Fields

        #region Method

        protected abstract ISingleInstanceEnforcer GetSingleInstanceEnforcer();

        protected abstract void FirstInit(string[] args);

        protected virtual void SecondInit(string[] args)
        {
            // This is not the first instance of the application, so do nothing but send a message to the first instance
            if (args.Length > 0)
            {
                Tracker.SendMessageToFirstInstance(args);
            }
        }

        protected virtual void PreInit(string[] args)
        {
        }

        protected virtual string SingleInstanceName
        {
            get
            {
                return Process.GetCurrentProcess().ProcessName;
            }
        }

        public void Dispose()
        {
        }

        #endregion Method

        public void Start(string[] args)
        {
            PreInit(args);
            try
            {
                // Attempt to create a tracker
                Tracker = new SingleInstanceTracker(SingleInstanceName, new SingleInstanceEnforcerRetriever(GetSingleInstanceEnforcer));

                // If this is the first instance of the application, run the main form
                if (Tracker.IsFirstInstance)
                {
                    try
                    {
                        FirstInit(args);
                    }
                    finally
                    {
                        Dispose();
                    }
                }
                else
                {
                    SecondInit(args);
                }
            }
            catch (SingleInstancingException)
            {
                throw;
            }
            finally
            {
                if (Tracker != null)
                    Tracker.Dispose();
            }
        }
    }
}