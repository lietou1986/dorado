using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Vancl.IC.VWS.BLL;
using Vancl.IC.VWS.Model;
using System.Web.Script.Serialization;

namespace Vancl.IC.VWS.SiteApp.Handler
{
    /// <summary>
    /// TaskHandler 的摘要说明
    /// </summary>
    public class TaskHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //ip
            string ip = context.Request.Params["ip"];
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(ip))
            {
                TaskBLL taskBll = new TaskBLL();
                IList<TaskEntity> tasks = taskBll.SeekTask(ip);

                if (tasks.Count > 0)
                {
                    new JavaScriptSerializer().Serialize(tasks, sb); 
                }
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(sb);
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