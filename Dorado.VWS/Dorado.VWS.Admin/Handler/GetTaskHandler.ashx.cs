using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vancl.IC.VWS.BLL;
using Vancl.IC.VWS.Model;
using System.Text;
using System.Web.Script.Serialization;

namespace Vancl.IC.VWS.SiteApp.Handler
{
    /// <summary>
    /// GetTaskHandler 的摘要说明
    /// </summary>
    public class GetTaskHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            StringBuilder sb = new StringBuilder();
            int taskid = 0;
            if (int.TryParse(context.Request.QueryString["taskid"], out taskid))
            {
                TaskBLL taskBll = new TaskBLL();
                TaskEntity taskEntity = taskBll.GetTask(taskid);

                if (taskEntity != null)
                {
                    new JavaScriptSerializer().Serialize(taskEntity, sb);
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