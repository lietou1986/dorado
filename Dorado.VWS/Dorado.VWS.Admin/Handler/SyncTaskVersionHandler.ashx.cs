using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Vancl.IC.VWS.BLL;
using System.Web.Script.Serialization;

using Vancl.IC.VWS.SiteApp.PermissionWCF;
using Vancl.IC.VWS.Model;


namespace Vancl.IC.VWS.SiteApp.Handler
{
    /// <summary>
    /// SyncTaskVersionHandler 的摘要说明
    /// </summary>
    public class SyncTaskVersionHandler : IHttpHandler
    {
        protected string CurUserName
        {
            get
            {
                if (HttpContext.Current.Request.Cookies["SSOToken"] == null) return "";
                string ssoToken = HttpContext.Current.Request.Cookies["SSOToken"].Value;
                PermissionWCFClient client = new PermissionWCFClient();
                return client.GetUserId(ssoToken);
            }
        }


        public void ProcessRequest(HttpContext context)
        {
            StringBuilder sb = new StringBuilder();
            SyncBLL syncBll = new SyncBLL();
            //domainid, begin, end
            int domainid, begin, end;
            int count = 0;
            if (int.TryParse(context.Request.Params["domainid"], out domainid)
                && int.TryParse(context.Request.Params["begin"], out begin)
                && int.TryParse(context.Request.Params["end"], out end))
            {
                if (IsHaveAllSelectTaskPermission(domainid))
                {
                    var obj = syncBll.GetSucessSyncTaskByDomain(domainid, begin, end);
                    new JavaScriptSerializer().Serialize(obj, sb);
                    count = syncBll.GetSucessSyncTaskCount(domainid);
                }
                else
                {
                    if (!string.IsNullOrEmpty(CurUserName))
                    {
                        var obj = syncBll.GetSucessSyncTaskByDomain(domainid, CurUserName, begin, end);
                        new JavaScriptSerializer().Serialize(obj, sb);
                        count = syncBll.GetSucessSyncTaskCount(domainid, CurUserName);
                    }
                }
            }
            context.Response.ClearContent();
            context.Response.ContentType = "text/plain";
            context.Response.Write("{\"Version\":" + sb.ToString() + ",\"Count\":" + count + "}");
            context.Response.End();
        }

        /// <summary>
        /// 判断是否拥有所有任务回滚列表查看权限
        /// </summary>
        /// <param name="domainId"></param>
        /// <returns></returns>
        private bool IsHaveAllSelectTaskPermission(int domainId)
        {
            //判断是否拥有查看所有任务回滚列表权限
            UserRoleBLL userRoleBll = new UserRoleBLL();
            List<int> userRoleIDList = userRoleBll.GetUserRoleId(CurUserName, domainId);

            UserResourceBLL userResourceBll = new UserResourceBLL();
            if (userRoleIDList.Count > 0)
            {
                IList<UserResoureEntity> listUserResource = userResourceBll.GetUserResourcePermission(userRoleIDList);

                foreach (UserResoureEntity userResoureEntity in listUserResource)
                {
                    //拥有查看所有任务回滚列表权限
                    if (userResoureEntity.ResourceValue.Equals("TaskRevertOwner"))
                    {
                        return true;
                    }
                }
            }
            return false;
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