#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class SyncSelectFilesSimple : WebBasePage
    {
        private readonly ServerProvider _serverProvider = new ServerProvider();
        private readonly SyncProvider _syncProvider = new SyncProvider();

        protected void Page_Load(object sender, EventArgs e)
        {
            tip.Text = string.Empty;

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
            DdlDomains.DataSource = _serverProvider.GetDomainsByUser(CurUserName, 2);

            DdlDomains.DataValueField = "DomainId";
            DdlDomains.DataTextField = "DomainName";
            DdlDomains.DataBind();
            DdlDomains.Items.Insert(0, new ListItem("请选择", "0"));
        }

        protected void BtnAdd_Click(object sender, EventArgs e)
        {
            ltResult.Text = "";

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

            #region 简单同步

            string errorMsg;
            SyncTaskProcessor syncTaskProcessor = new SyncTaskProcessor();
            bool isSuccess = syncTaskProcessor.SyncUnverTask(CurUserName, DdlDomains.SelectedItem.Text, adds, dels, out errorMsg);
            tip.Text = errorMsg;
            if (isSuccess)
            {
                ShowAlert("同步成功！");
            }
            else
            {
                ShowAlert("同步失败！");
            }

            #endregion 简单同步

            #region 显示同步文件列表  雷斌添加

            int domainID = 0;
            if (int.TryParse(DdlDomains.SelectedValue, out domainID))
            {
                DomainProvider domainProvider = new DomainProvider();
                Model.DomainEntity domain = domainProvider.GetDomainById(domainID);
                string[] cacheUrls = domain.CacheUrl.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder sb = new StringBuilder();
                if (adds != null && adds.Length > 0)
                {
                    sb.Append("添加文件列表：<br />");

                    if (domain.DomainName.ToLower() == "image.dorado.com")
                    {
                        foreach (var file in adds)
                        {
                            sb.AppendFormat(string.Format("http://{0}/{1}<br/>", SyncTaskProcessor.GetServerIndex(file), file));
                        }
                    }

                    if (cacheUrls.Length > 0)
                    {
                        foreach (string url in cacheUrls)
                        {
                            foreach (string file in adds)
                            {
                                sb.AppendFormat("http://{0}/{1}{2}", url, file.Replace('\\', '/'), "<br/>");
                            }
                        }
                    }
                    else
                    {
                        sb.Append(string.Join("<br/>", adds));
                    }
                }
                if (dels != null && dels.Length > 0)
                {
                    sb.Append("删除文件列表：<br />");
                    if (domain.DomainName.ToLower() == "image.dorado.com")
                    {
                        foreach (var file in dels)
                        {
                            sb.AppendFormat(string.Format("http://{0}/{1}<br/>", SyncTaskProcessor.GetServerIndex(file), file));
                        }
                    }
                    if (cacheUrls.Length > 0)
                    {
                        foreach (string url in cacheUrls)
                        {
                            foreach (string file in dels)
                            {
                                sb.AppendFormat("http://{0}/{1}{2}", url, file.Replace('\\', '/'), "<br/>");
                            }
                        }
                    }
                    else
                    {
                        sb.Append(string.Join("<br/>", dels));
                    }
                }

                ltResult.Text = sb.ToString();
            }
            else
            {
                ltResult.Text = "域名不存在！";
            }

            #endregion 显示同步文件列表  雷斌添加
        }
    }
}