using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace Dorado.DataExpress
{
    public class HttpBindModule : IHttpModule
    {
        public static bool Inited;
        private static readonly object InitLock = new object();

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            object initLock;
            Monitor.Enter(initLock = HttpBindModule.InitLock);
            try
            {
                HttpBindModule.Inited = true;
            }
            finally
            {
                Monitor.Exit(initLock);
            }
            context.EndRequest += new EventHandler(this.OnEndRequest);
            context.Error += new EventHandler(this.OnError);
        }

        private void OnError(object sender, EventArgs e)
        {
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.Items["DefaultDatabase"] != null)
            {
                (HttpContext.Current.Items["DefaultDatabase"] as Database).Close();
            }
            if (HttpContext.Current.Items["DatabaseContainer"] != null)
            {
                Dictionary<string, Database> cache = (Dictionary<string, Database>)HttpContext.Current.Items["DatabaseContainer"];
                foreach (KeyValuePair<string, Database> database in cache)
                {
                    database.Value.Close();
                }
            }
        }
    }
}