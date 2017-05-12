using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vancl.IC.VWS.BLL;
using log4net;

namespace Vancl.IC.VWS.SiteApp.Handler
{
    /// <summary>
    /// TaskResultHandler 的摘要说明
    /// </summary>
    public class TaskResultHandler : IHttpHandler
    {
        ILog _logger = LogManager.GetLogger(typeof(TaskResultHandler));
        public void ProcessRequest(HttpContext context)
        {
            //ip, taskid, result, msg
            string ip = context.Request.Params["ip"];
            string msg = context.Request.Params["msg"];
            int reslut = -1;
            int taskid = -1;
            if (!string.IsNullOrEmpty(ip) && int.TryParse(context.Request.Params["taskid"], out taskid) && int.TryParse(context.Request.Params["result"], out reslut))
            {
                //任务失败时发异常邮件
                if (reslut > 0)
                {
                    string errMsg = string.Format("IP[{0}] Task[{1}] Error:{2}", ip, taskid, msg);
                    _logger.Error(errMsg);
                    //MailSender.SendErrorEmail(errMsg);
                }

                TaskBLL taskBll = new TaskBLL();
                taskBll.ProceTaskResult(taskid, ip, reslut == 0);
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write("1");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}