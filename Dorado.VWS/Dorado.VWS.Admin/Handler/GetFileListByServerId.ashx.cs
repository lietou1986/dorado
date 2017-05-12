#region using

using System.Collections.Generic;
using System.Text;
using System.Web;
using Dorado.VWS.Model;
using Dorado.VWS.Services;
using Dorado.VWS.Utils;

//using Dorado.VWS.Admin.PermissionWCF;

#endregion using

namespace Dorado.VWS.Admin.Handler
{
    /// <summary>
    /// GetFileListByServerId 的摘要说明
    /// </summary>
    public class GetFileListByServerId : IHttpHandler
    {
        private readonly FileListProvider _flProvider = new FileListProvider();
        private readonly PermissionProvider _permissionProvider = new PermissionProvider();
        private readonly DomainPermissionProvider _pomainPermissionProvider = new DomainPermissionProvider();
        private readonly ServerProvider _serProvider = new ServerProvider();
        private readonly SyncProvider _syncProvider = new SyncProvider();
        private List<string> _listPermissionFiles = new List<string>();

        private WebBasePage basePage = new WebBasePage();

        protected string CurUsername
        {
            get
            {
                return WebBasePage.GetUserName();
            }
        }

        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            //参数domainid, id==dir, permission(0:不判断权限， 1：判断权限)
            var sb = new StringBuilder();
            int serverid;
            int permission = 1;
            //string dir = context.Request.Params["id"];
            int roleId;
            if (!int.TryParse(context.Request.Params["roleId"], out roleId))
            {
                roleId = 0;
            }
            string dir = context.Request.Params["id"];
            //获取域名下的MD5列表
            //List<VwsDirectoryInfo> hsFileMD5 = _flProvider.GetFileListNoMd5(serverid, dir);
            if (int.TryParse(context.Request.Params["serverid"], out serverid))
            {
                bool hasPermission = true;
                bool haveRole = true;
                //判断当前文件是否正在被使用
                bool isProce = false;
                bool hasDelPermission = false;
                bool nocheck = permission == 1 && !hasPermission;
                bool lastHaveDelPermission = permission == 1 && (hasPermission && hasDelPermission);

                if (string.IsNullOrEmpty(dir))
                {
                    ServerEntity server = _serProvider.GetServerById(serverid);

                    dir = server != null ? server.Root : "";
                }
                //获取文件列表
                List<VwsDirectoryInfo> filelist = _flProvider.GetFileList(serverid, dir);
                sb.Append("[");
                //判断文件权限
                foreach (VwsDirectoryInfo file in filelist)
                {
                    bool hasChanged = false;//!hsFileMD5.ContainsKey(file.FullName) || !file.MD5.Equals(hsFileMD5[file.FullName]);
                    if (!file.IsFolder)
                    {
                        sb.Append(
                                            string.Format(
                                                "{{\"id\":\"{0}\",\"name\":\"{1}\",\"pId\":\"{2}\",\"isParent\":{3},\"nocheck\":{4},\"hasDelPermission\":{5},\"checked\":{6}, \"isProce\":{7}, \"icon\":\"{8}\"}},",
                                                file.FullName.Replace("\\", "\\\\"),
                                                file.Name + " (" + file.UpdateTime.ToLocalTime() + ")",
                                                dir.Replace("\\", "\\\\"), file.IsFolder.ToString().ToLower(),
                                                nocheck.ToString().ToLower(), lastHaveDelPermission.ToString().ToLower(),
                                                haveRole.ToString().ToLower(), isProce.ToString().ToLower(),
                                                hasChanged ? "styles/img/edit.png" : "styles/img/page.png"));
                    }
                    else
                    {
                        sb.Append(
                                            string.Format(
                                                "{{\"id\":\"{0}\",\"name\":\"{1}\",\"pId\":\"{2}\",\"isParent\":{3},\"nocheck\":{4},\"hasDelPermission\":{5},\"checked\":{6}, \"isProce\":{7}}},",
                                                file.FullName.Replace("\\", "\\\\"),
                                                file.Name + " (" + file.UpdateTime.ToLocalTime() + ")",
                                                dir.Replace("\\", "\\\\"), file.IsFolder.ToString().ToLower(),
                                                nocheck.ToString().ToLower(), lastHaveDelPermission.ToString().ToLower(),
                                                haveRole.ToString().ToLower(), isProce.ToString().ToLower()));
                    }
                }
                if (sb.Length > 1)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append("]");
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(sb);
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion IHttpHandler Members
    }
}