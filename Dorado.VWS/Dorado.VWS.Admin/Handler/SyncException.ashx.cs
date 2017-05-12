using System;
using System.Web;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

namespace Dorado.VWS.Admin.Handler
{
    /// <summary>
    /// SyncException 的摘要说明
    /// </summary>
    public class SyncException : IHttpHandler
    {
        protected string CurUserName
        {
            get
            {
                return WebBasePage.GetUserName();
            }
        }

        private LogProvider logProvider = new LogProvider();

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string taskid = context.Request.Params["taskId"];
            string domainName = context.Request.QueryString["domainname"];
            string userName = context.Request.QueryString["userName"];
            bool result = false;
            switch (context.Request.QueryString["action"])
            {
                //清除异常任务
                case "ClearSyncException":
                    {
                        int inttaskid = 0;
                        if (!string.IsNullOrEmpty(taskid) && int.TryParse(taskid, out inttaskid))
                        {
                            result = ClearSyncException(inttaskid);
                            //定义操作日志
                            var operateLogEntity = new OperationLogEntity
                            {
                                UserName = CurUserName,
                                DomainName = domainName,
                                OperateType = EnumOperateType.ServerControl,
                            };
                            //操作成功，写入日志
                            if (result)
                            {
                                operateLogEntity.Log = CurUserName + "强制结束了" + CurUserName + "操作域名：" + domainName + "的同步任务";
                                operateLogEntity.Result = result;
                                //var logProvider = new LogProvider();
                                logProvider.AddOperateLog(operateLogEntity);
                            }
                        }
                    }
                    break;
            }
            context.Response.Write(result.ToString().ToLower());
        }

        /// <summary>
        /// 强制结束异常任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private bool ClearSyncException(int taskId)
        {
            SyncProvider syncProvider = new SyncProvider();
            try
            {
                return syncProvider.SyncExceptionByTaskId(taskId);
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS.Site.ClearSync", "清除任务失败\n" + ex.Message);
                return false;
            }
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