#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class UploadFile : WebBasePage, ISyncTaskParams
    {
        private readonly ServerProvider _serverProvider = new ServerProvider();
        private readonly SyncProvider _syncProvider = new SyncProvider();

        #region ISyncTaskParams Members

        public SyncTaskParams Params { get; set; }

        #endregion ISyncTaskParams Members

        protected void Page_Load(object sender, EventArgs e)
        {
            lResult.Text = string.Empty;

            if (!IsPostBack)
            {
                InitDomainData();
            }
            //判断用户是否有文件列表权限
            //tdFilelist.Visible = HasResoucePermission(int.Parse(DdlDomains.SelectedValue), "VersionFileListBulid");
            tdFilelist.Visible = HasDomainPermission(int.Parse(DdlDomains.SelectedValue), EnumManageType.SyncByList);
        }

        private void InitDomainData()
        {
            DdlDomains.DataSource = _serverProvider.GetDomainsByUser(CurUserName, 1);

            DdlDomains.DataValueField = "DomainId";
            DdlDomains.DataTextField = "DomainName";
            DdlDomains.DataBind();
            DdlDomains.Items.Insert(0, new ListItem("请选择", "0"));
        }

        protected void BtnAdd_Click(object sender, EventArgs e)
        {
            //文件列表控件不为空，则判断文件是否存在，且按照文件列表方式同步
            IList<string> sellist = new List<string>();
            if (!string.IsNullOrEmpty(TxtFileList.Text.Trim()))
            {
                IList<string> sellisttemp = Regex.Split(TxtFileList.Text, Environment.NewLine);
                foreach (var item in sellisttemp)
                {
                    if (!string.IsNullOrEmpty(item) && !item.StartsWith("#") && !item.StartsWith("\\"))
                    {
                        sellist.Add(item);
                    }
                }

                if (sellist.Count == 0)
                {
                    ShowAlert("文件列表格式错误!");
                    return;
                }

                var filelistProvider = new FileListProvider();
                string nonExist;
                if (!filelistProvider.FilesExist(int.Parse(DdlDomains.SelectedValue), sellist, out nonExist))
                {
                    ShowAlert(string.Format("文件不存在:\\r\\n{0}", nonExist));
                    return;
                }
            }

            ServerEntity sourceServer = _serverProvider.GetSourceServerByDomainId(int.Parse(DdlDomains.SelectedValue));
            // 把添加删除列表改为相对路径
            int rootLength = sourceServer.Root.Length;

            string[] adds = Regex.Split(HfAddFiles.Value, "###");
            string[] dels = Regex.Split(HfDelFiles.Value, "###");
            adds = adds.Except(dels).Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Remove(0, rootLength)).ToArray();
            dels = dels.Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Remove(0, rootLength)).ToArray();

            if (sellist.Count > 0)
            {
                adds = sellist.ToArray();
            }

            Params = new SyncTaskParams
                         {
                             DomainId = int.Parse(DdlDomains.SelectedValue),
                             AddFiles = adds,
                             DelFiles = dels
                         };

            Response.Cookies["tmpdomainid"].Value = DdlDomains.SelectedValue;
            Server.Transfer("SyncSelectTargets.aspx");
        }

        protected void DdlDomains_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DdlDomains.SelectedValue != "0")
            {
                SynctaskEntity syncTaskEntity = _syncProvider.GetUnFinishTask(int.Parse(DdlDomains.SelectedValue),
                                                                         CurUserName);
                if (syncTaskEntity != null)
                {
                    Params = new SyncTaskParams
                                 {
                                     DomainId = int.Parse(DdlDomains.SelectedValue),
                                     TaskId = syncTaskEntity.TaskId,
                                     AddFiles = Regex.Split(syncTaskEntity.AddFiles, ","),
                                     DelFiles = Regex.Split(syncTaskEntity.DelFiles, ",")
                                 };
                    Server.Transfer("SyncSelectTargets.aspx");
                }
            }
        }

        /// <summary>
        ///     判断是否有文件列表填写权限
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns></returns>
        private bool IsHaveFileListPermission(int domainId)
        {
            //判断是否有文件列表填写权限
            var userRoleProvider = new UserRoleProvider();
            List<int> userroleIdList = userRoleProvider.GetUserRoleId(CurUserName, domainId);

            var userResourceProvider = new UserResourceProvider();
            if (userroleIdList.Count > 0)
            {
                IList<UserResoureEntity> listUserResource = userResourceProvider.GetUserResourcePermission(userroleIdList);

                foreach (UserResoureEntity userResoureEntity in listUserResource)
                {
                    //判断是否有文件列表填写权限
                    if (userResoureEntity.ResourceValue.Equals("VersionFileListBulid"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected void bUpload_Click(object sender, EventArgs e)
        {
            HttpFileCollection files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                if (string.IsNullOrEmpty(files[i].FileName)) continue;
                string upFilePath = Server.MapPath("~\\tmp\\") + files[i].FileName;
                files[i].SaveAs(upFilePath);

                ServerEntity sourceServer = _serverProvider.GetSourceServerByDomainId(int.Parse(DdlDomains.SelectedValue));
                string uploadPath = (hfUploadPath.Value == "根目录" ? sourceServer.Root : hfUploadPath.Value) +
                                    files[i].FileName;

                var client = new SocketFileClient();
                bool ret = client.SendFile(IPAddress.Parse(sourceServer.IP), uploadPath, upFilePath);
                if (ret)
                    lResult.Text += files[i].FileName + " 上传成功！<br/>";
                else
                    lResult.Text += files[i].FileName + " 上传失败！<br/>";
            }
        }
    }
}