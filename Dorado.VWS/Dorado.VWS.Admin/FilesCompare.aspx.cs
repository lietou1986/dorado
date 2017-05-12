#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class FilesCompare : WebBasePage, ISyncTaskParams
    {
        private readonly ServerProvider _serverProvider = new ServerProvider();
        private readonly SyncProvider _syncProvider = new SyncProvider();

        #region ISyncTaskParams Members

        public SyncTaskParams Params { get; set; }

        #endregion ISyncTaskParams Members

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitEnvironmentData();
                InitDomainData();
            }
            //判断用户是否有文件列表权限
            if ("0".Equals(DdlDomains.SelectedValue))
            {
                trFileInput.Visible = false;
                trFileSelect.Visible = false;
            }
            else
            {
                trFileSelect.Visible = HasDomainPermission(int.Parse(DdlDomains.SelectedValue), EnumManageType.SyncByList);
                //trFileInput.Visible = HasResoucePermission(int.Parse(DdlDomains.SelectedValue), "VersionFileListBulid");
                trFileInput.Visible = HasDomainPermission(int.Parse(DdlDomains.SelectedValue), EnumManageType.SyncByList);
            }
        }

        private void InitEnvironmentData()
        {
            DdlEnvironment.Items.Insert(0, new ListItem("请选择", EnvironmentType.UnSelected));
            //DdlEnvironment.Items.Insert(1, new ListItem("开发", EnvironmentType.Development));
            //DdlEnvironment.Items.Insert(2, new ListItem("测试", EnvironmentType.Testing));
            DdlEnvironment.Items.Insert(1, new ListItem("验收", EnvironmentType.Acceptance));
            //DdlEnvironment.Items.Insert(4, new ListItem("预上线", EnvironmentType.Advanced));
            DdlEnvironment.Items.Insert(2, new ListItem("线上", EnvironmentType.Production));
        }

        private void InitDomainData()
        {
            //DdlDomains.DataSource = _serverProvider.GetDomainsByUser(CurUserName, 1);

            //DdlDomains.DataValueField = "DomainId";
            //DdlDomains.DataTextField = "DomainName";
            //DdlDomains.DataBind();
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
            string randid = RandomStr.GetRndStrOnlyFor(16);
            Session[randid + "_SyncTaskParams"] = Params;
            Response.Redirect("SyncSelectTargets.aspx?param=" + randid, true);
            Response.Cookies["tmpdomainid"].Value = DdlDomains.SelectedValue;
            //Server.Transfer("SyncSelectTargets.aspx");
            //Response.Redirect("SyncSelectTargets.aspx?id="+Params.DomainId);
            //string param= new JavaScriptSerializer().Serialize(Params);
            //param = AESHelper.AESEncrypt(param);
            //Response.Redirect("SyncSelectTargets.aspx?param=" + Server.UrlEncode(param), true);
        }

        protected void DdlEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            DdlDomains.DataSource = _serverProvider.GetDomainsByUser(CurUserName, 1, DdlEnvironment.SelectedValue);

            DdlDomains.DataValueField = "DomainId";
            DdlDomains.DataTextField = "DomainName";
            DdlDomains.DataBind();
            DdlDomains.Items.Insert(0, new ListItem("请选择", "0"));
        }

        protected void DdlDomains_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DdlDomains.SelectedValue != "0")
            {
                //SynctaskEntity syncTaskEntity = _syncProvider.GetUnFinishTask(int.Parse(DdlDomains.SelectedValue),
                //                                                         CurUserName);
                //if (syncTaskEntity != null)
                //{
                //    Params = new SyncTaskParams
                //    {
                //        DomainId = int.Parse(DdlDomains.SelectedValue),
                //        TaskId = syncTaskEntity.TaskId,
                //        AddFiles = Regex.Split(syncTaskEntity.AddFiles, ","),
                //        DelFiles = Regex.Split(syncTaskEntity.DelFiles, ",")
                //    };
                //    //使用随机数标记数据，防止同时同步多个任务时，数据被覆盖
                //    string randid = RandomStr.GetRndStrOnlyFor(16);
                //    Session[randid + "_SyncTaskParams"] = Params;
                //    Response.Redirect("SyncSelectTargets.aspx?param=" + randid, true);
                //    //Response.Redirect("SyncSelectTargets.aspx?id=" + Params.DomainId);
                //    //Server.Transfer("SyncSelectTargets.aspx");
                //    //string param = new JavaScriptSerializer().Serialize(Params);
                //    //param = AESHelper.AESEncrypt(param);
                //    //Response.Redirect("SyncSelectTargets.aspx?param=" + Server.UrlEncode(param), true);
                //}
                int domid = 0;
                if (int.TryParse(DdlDomains.SelectedValue, out domid))
                {
                    DdlDomains_server.DataSource = _serverProvider.GetServersByDomainId(domid);
                    DdlDomains_server.DataValueField = "ServerId";
                    DdlDomains_server.DataTextField = "IP";
                    DdlDomains_server.DataBind();
                    DdlDomains_server.Items.Insert(0, new ListItem("请选择", "0"));
                }
            }
        }

        //protected void DomainServer_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    //if (DdlDomains.SelectedValue != "0")
        //    //{
        //    //    //SynctaskEntity syncTaskEntity = _syncProvider.GetUnFinishTask(int.Parse(DdlDomains.SelectedValue),
        //    //    //                                                         CurUserName);
        //    //    //if (syncTaskEntity != null)
        //    //    //{
        //    //    //    Params = new SyncTaskParams
        //    //    //    {
        //    //    //        DomainId = int.Parse(DdlDomains.SelectedValue),
        //    //    //        TaskId = syncTaskEntity.TaskId,
        //    //    //        AddFiles = Regex.Split(syncTaskEntity.AddFiles, ","),
        //    //    //        DelFiles = Regex.Split(syncTaskEntity.DelFiles, ",")
        //    //    //    };
        //    //    //    //使用随机数标记数据，防止同时同步多个任务时，数据被覆盖
        //    //    //    string randid = RandomStr.GetRndStrOnlyFor(16);
        //    //    //    Session[randid + "_SyncTaskParams"] = Params;
        //    //    //    Response.Redirect("SyncSelectTargets.aspx?param=" + randid, true);
        //    //    //    //Response.Redirect("SyncSelectTargets.aspx?id=" + Params.DomainId);
        //    //    //    //Server.Transfer("SyncSelectTargets.aspx");
        //    //    //    //string param = new JavaScriptSerializer().Serialize(Params);
        //    //    //    //param = AESHelper.AESEncrypt(param);
        //    //    //    //Response.Redirect("SyncSelectTargets.aspx?param=" + Server.UrlEncode(param), true);
        //    //    //}
        //    //    int domid = 0;
        //    //    if (int.TryParse(DdlDomains.SelectedValue, out domid))
        //    //    {
        //    //        DdlDomains_server.DataSource = _serverProvider.GetServersByDomainId(domid);
        //    //        DdlDomains_server.DataValueField = "ServerId";
        //    //        DdlDomains_server.DataTextField = "IP";
        //    //        DdlDomains_server.DataBind();
        //    //        DdlDomains_server.Items.Insert(0, new ListItem("请选择", "0"));
        //    //    }
        //    //}
        //}
    }
}