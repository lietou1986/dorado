using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vancl.IC.VWS.BLL;
using System.Text;
using Vancl.IC.VWS.Model;

namespace Vancl.IC.VWS.SiteApp.Handler
{
    /// <summary>
    /// SyncTaskResultHandler 的摘要说明
    /// </summary>
    public class SyncTaskResultHandler : IHttpHandler
    {
        private SyncBLL _syncBll = new SyncBLL();

        public void ProcessRequest(HttpContext context)
        {
            //参数taskid
            StringBuilder sb = new StringBuilder();
            int taskid = 0;
            if (int.TryParse(context.Request.Params["taskid"], out taskid))
            {
                SynctaskEntity synctask = _syncBll.GetSyncTaskById(taskid);

                if (synctask != null)
                {
                    IList<SynctaskSubEntity> syncsubtasks = _syncBll.GetSubTask(taskid);
                    sb.Append("{");
                    sb.AppendFormat("\"Id\":{0},", synctask.TaskId);
                    sb.AppendFormat("\"Status\":\"{0}\",", synctask.SyncStatus);
                    sb.AppendFormat("\"Msg\":\"{0}\"", synctask.LogInfo.Replace("\n\r", "<br/>"));
                    if (syncsubtasks != null && syncsubtasks.Count != 0)
                    {
                        sb.Append(",\"Subtasks\":[");
                        bool bFirst = true;
                        foreach (var item in syncsubtasks)
                        {
                            if (!bFirst)
                            {                                
                                sb.Append(",");
                            }
                            
                            bFirst = false;
                            sb.Append("{");
                            sb.AppendFormat("\"ServerId\":{0},", item.SyncServerId);
                            sb.AppendFormat("\"Status\":\"{0}\"", item.SyncStatus);
                            sb.Append("}");
                        }
                        sb.Append("]");
                    }
                    sb.Append("}");
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