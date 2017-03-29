using Dorado.Configuration;
using Dorado.Core.Data;
using Dorado.Web.Extensions;
using System;
using System.Web.Mvc;

namespace Dorado.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class UsingDBAttribute : ActionFilterAttribute
    {
        public string[] Conns
        {
            get;
            set;
        }

        public UsingDBAttribute()
        {
            Conns = new[] { "Default" };
        }

        public UsingDBAttribute(params string[] conns)
            : this()
        {
            Conns = conns;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Conns == null) return;
            ConnContainer container = filterContext.Controller.GetConnContainer();
            foreach (var conn in Conns)
            {
                Conn connInstance = new Conn(ConnectionStringProvider.Get(conn));
                if (container.ContainsKey(conn)) continue;
                container.Add(conn, connInstance);
            }
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (Conns == null) return;
            ConnContainer container = filterContext.Controller.GetConnContainer();
            foreach (var conn in Conns)
            {
                if (!container.ContainsKey(conn)) continue;
                object svr = container[conn];
                Conn dispObj = svr as Conn;
                if (dispObj != null)
                    dispObj.Close();
                container.Remove(conn);
            }
        }
    }
}