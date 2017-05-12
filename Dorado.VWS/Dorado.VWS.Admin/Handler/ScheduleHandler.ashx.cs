#region using

using System.Web;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin.Handler
{
    /// <summary>
    ///     计划任务触发的Handler
    /// </summary>
    public class ScheduleHandler : IHttpHandler
    {
        //private readonly ILog _logger = LogManager.GetLogger(typeof (ScheduleHandler));
        private readonly ScheduleTaskProvider _scheduleProvider = new ScheduleTaskProvider();

        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            _scheduleProvider.DoWork();
            //_logger.Debug("ScheduleHandler invoke");
            LoggerWrapper.Logger.Info("VWS.Admin", "ScheduleHandler invoke");
            context.Response.ContentType = "text/plain";
            context.Response.Write(string.Empty);
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion IHttpHandler Members
    }
}