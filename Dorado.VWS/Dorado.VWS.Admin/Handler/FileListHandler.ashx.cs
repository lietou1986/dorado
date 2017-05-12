#region using

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;
using Dorado.VWS.Utils;

//using Dorado.VWS.Admin.PermissionWCF;

#endregion using

namespace Dorado.VWS.Admin.Handler
{
    /// <summary>
    /// 获取文件列表的Handler
    /// </summary>
    public class FileListHandler : IHttpHandler
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
            int domainid;
            int permission;
            string dir = context.Request.Params["id"];

            int roleId;
            if (!int.TryParse(context.Request.Params["roleId"], out roleId))
            {
                roleId = 0;
            }

            //获取角色权限
            IList<PermissionEntity> permissionList = _permissionProvider.GetPermissionByRoleId(roleId);

            if (int.TryParse(context.Request.Params["domainid"], out domainid) &&
                int.TryParse(context.Request.Params["permission"], out permission))
            {
                ServerEntity serverEntity = _serProvider.GetSourceServerByDomainId(domainid);
                if (serverEntity != null)
                {
                    sb.Append("[");
                    if (string.IsNullOrEmpty(dir))
                    {
                        dir = serverEntity.Root;
                    }
                    if (permission == 1)
                    {
                        if (!string.IsNullOrEmpty(CurUsername))
                        {
                            _listPermissionFiles =
                                _permissionProvider.GetPermissionByUserAndDomain(CurUsername, domainid).Select(c => c.Path).
                                    ToList();
                        }
                    }

                    //获取文件列表
                    List<VwsDirectoryInfo> filelist = _flProvider.GetFileList(serverEntity.ServerId, dir);

                    //bool hasDelPermission = _permissionProvider.HasResoucePermission(CurUserName, domainid, "VersionDirDelete");
                    bool hasDelPermission = _pomainPermissionProvider.HasPermission(domainid, CurUsername, EnumManageType.FileDelete);

                    //获取正在处理的文件列表
                    IList<SynctaskEntity> tasks = _syncProvider.GetProceTasksByDomain(domainid);
                    IList<string> procFiles = new List<string>();

                    foreach (var task in tasks)
                    {
                        IList<string> adds = CommonHelper.ConvertByComma(task.AddFiles);
                        IList<string> dels = CommonHelper.ConvertByComma(task.DelFiles);

                        foreach (var file in adds.Concat(dels))
                        {
                            if (!procFiles.Contains(serverEntity.Root + file))
                            {
                                procFiles.Add(serverEntity.Root + file);
                            }
                        }
                    }

                    //获取域名下的MD5列表
                    Hashtable hsFileMD5 = _flProvider.GetFileMd5ByDomainId(domainid);

                    //判断文件权限
                    foreach (VwsDirectoryInfo file in filelist)
                    {
                        bool hasPermission = false;
                        bool haveRole = false;

                        //判断用户文件权限
                        foreach (var permissionFile in _listPermissionFiles)
                        {
                            var fileCompare = serverEntity.Root + permissionFile;

                            if (!file.FullName.Contains(fileCompare)) continue;

                            hasPermission = true;
                            break;
                        }

                        //判断当前文件是否正在被使用
                        bool isProce = procFiles.Contains(file.FullName);

                        //判断文件是否已经加到指定角色
                        foreach (PermissionEntity permissionEntity in permissionList)
                        {
                            if (string.IsNullOrEmpty(permissionEntity.Path)) continue;

                            var permissionPath = serverEntity.Root + permissionEntity.Path;

                            if (file.FullName.Contains(permissionPath))
                            {
                                haveRole = true;
                                break;
                            }
                        }

                        bool nocheck = permission == 1 && !hasPermission;
                        bool lastHaveDelPermission = permission == 1 && (hasPermission && hasDelPermission);
                        bool hasChanged = !hsFileMD5.ContainsKey(file.FullName) || !file.MD5.Equals(hsFileMD5[file.FullName]);

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