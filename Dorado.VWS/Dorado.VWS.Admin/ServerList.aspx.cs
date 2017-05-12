#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class ServerList : WebBasePage
    {
        private readonly LogProvider _logProvider = new LogProvider();

        /// <summary>
        ///     定义服务器业务逻辑类
        /// </summary>
        private readonly ServerProvider _serverProvider = new ServerProvider();

        private readonly UpdateClientProvider _updateProvider = new UpdateClientProvider();
        private readonly Dictionary<int, int> _domainDic = new Dictionary<int, int>();

        //开始记录数
        private int _beginIndex = 1;

        //当前页数
        private int _currentPage = 1;

        private const int PageSize = 20;
        private IList<ServerEntity> _serverList;

        //总共页数
        private int _totalPage = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (WebBasePage.CurUserIsAdmin)
            {
                inputUpdateClient.Visible = true;
            }
            else
            {
                inputUpdateClient.Visible = false;
            }

            if (!IsPostBack)
            {
                InitEnvironmentData();
                InitDomainData();
                InitPage();
                InitServerData();
            }
        }

        /// <summary>
        /// 更新客户端服务是否显示
        /// </summary>
        /// <returns></returns>
        protected string GetUpdateDisplayStyle()
        {
            return WebBasePage.CurUserIsAdmin ? "inline" : "none";
        }

        /// <summary>
        ///     初始化域名数据
        /// </summary>
        private void InitDomainData()
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                ddlDomain.DataSource = GetUserDomians(EnumManageType.DailyManage, DdlEnvironment.SelectedValue);

                ddlDomain.DataValueField = "DomainId";
                ddlDomain.DataTextField = "DomainName";
                ddlDomain.DataBind();
                ddlDomain.Items.Insert(0, new ListItem("请选择", "0"));

                ddlDomain.ClearSelection();
                try
                {
                    ddlDomain.Items.FindByValue(Request.QueryString["id"]).Selected = true;
                }
                catch
                {
                }
            }
            else
            {
                ddlDomain.Items.Clear();
                ddlDomain.Items.Insert(0, new ListItem("请选择", "0"));
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

        /// <summary>
        ///     初始服务器数据
        /// </summary>
        private void InitServerData()
        {
            int endIndex = _beginIndex + PageSize - 1;
            //获取服务器列表
            if (CurUserIsAdmin)
            {
                _serverList = _serverProvider.GetServersByDomainIdForServerList(Convert.ToInt32(ddlDomain.SelectedValue), _beginIndex, endIndex);
            }
            else
            {
                _serverList = _serverProvider.GetServersByDomainIdForServerList(CurUserName, Convert.ToInt32(ddlDomain.SelectedValue), _beginIndex, endIndex);
            }
            rptServer.DataSource = _serverList;
            rptServer.DataBind();
        }

        protected void DdlEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlDomain.DataSource = GetUserDomians(EnumManageType.DailyManage, DdlEnvironment.SelectedValue);

            ddlDomain.DataValueField = "DomainId";
            ddlDomain.DataTextField = "DomainName";
            ddlDomain.DataBind();
            ddlDomain.Items.Insert(0, new ListItem("请选择", "0"));
        }

        protected void ddlDomain_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitPage();
            InitServerData();
        }

        protected void rptServer_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var hdfDomainName = e.Item.FindControl("hdfDomainName") as HiddenField;
            //定义操作日志
            var operateLogEntity = new OperationLogEntity
                                                      {
                                                          UserName = CurUserName,
                                                          DomainName = hdfDomainName.Value,
                                                      };
            if (e.CommandName == "stopServer")
            {
                ServerEntity serverEntity = _serverProvider.GetServerByIdForServerList(Convert.ToInt32(e.CommandArgument));

                operateLogEntity.OperateType = EnumOperateType.ServerManage;
                operateLogEntity.Log = string.Format("停用服务器，服务器Id:{0}，服务器类型:{1}，IP:{2}", serverEntity.ServerId,
                                                     serverEntity.ServerTypeName, serverEntity.IP);

                if (!_serverProvider.DeleteServer(Convert.ToInt32(e.CommandArgument)))
                {
                    operateLogEntity.Result = false;
                    _logProvider.AddOperateLog(operateLogEntity);

                    ShowAlert("对不起，停用失败！");
                    return;
                }
                operateLogEntity.Result = true;
                _logProvider.AddOperateLog(operateLogEntity);

                ShowAlert("停用成功！");
                InitServerData();
                return;
            }
            if (e.CommandName == "startServer")
            {
                ServerEntity serverEntity = _serverProvider.GetServerByIdForServerList(Convert.ToInt32(e.CommandArgument));

                operateLogEntity.OperateType = EnumOperateType.ServerManage;
                operateLogEntity.Log = string.Format("启用服务器，服务器Id:{0}，服务器类型:{1}，IP:{2}", serverEntity.ServerId,
                                                     serverEntity.ServerTypeName, serverEntity.IP);

                if (!_serverProvider.UnDeleteServer(Convert.ToInt32(e.CommandArgument)))
                {
                    operateLogEntity.Result = false;
                    _logProvider.AddOperateLog(operateLogEntity);

                    ShowAlert("对不起，启用失败！");
                    return;
                }
                operateLogEntity.Result = true;
                _logProvider.AddOperateLog(operateLogEntity);

                ShowAlert("启用成功！");
                InitServerData();
                return;
            }
            if (e.CommandName == "delDomain")
            {
                if (!_serverProvider.DeleteDoamin(Convert.ToInt32(e.CommandArgument)))
                {
                    ShowAlert("对不起，删除失败！");
                    return;
                }
                ShowAlert("删除成功！");
                InitDomainData();
                InitServerData();
                return;
            }

            if (e.CommandName == "setEnable")
            {
                operateLogEntity.OperateType = EnumOperateType.ServerManage;
                operateLogEntity.Log = string.Format("启用域名（状态），域名:{0}", hdfDomainName.Value);

                if (!_serverProvider.SetDomainEnable(Convert.ToInt32(e.CommandArgument)))
                {
                    operateLogEntity.Result = false;
                    _logProvider.AddOperateLog(operateLogEntity);

                    ShowAlert("对不起，域名启用失败！");
                    return;
                }
                operateLogEntity.Result = true;
                _logProvider.AddOperateLog(operateLogEntity);

                ShowAlert("域名已经置为启用状态！");
                InitDomainData();
                InitServerData();
                return;
            }
            if (e.CommandName == "setDisable")
            {
                operateLogEntity.OperateType = EnumOperateType.ServerManage;
                operateLogEntity.Log = string.Format("停用域名（状态），域名:{0}", hdfDomainName.Value);

                if (!_serverProvider.SetDomainDisable(Convert.ToInt32(e.CommandArgument)))
                {
                    operateLogEntity.Result = false;
                    _logProvider.AddOperateLog(operateLogEntity);

                    ShowAlert("对不起，域名停用失败！");
                    return;
                }
                operateLogEntity.Result = true;
                _logProvider.AddOperateLog(operateLogEntity);

                ShowAlert("域名已经置为停用状态！");
                InitDomainData();
                InitServerData();
                return;
            }
        }

        protected void rptServer_ItemDataBound(object source, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var hdfDomainId = e.Item.FindControl("hdfDomainId") as HiddenField;

                var lbtnSetEnable = e.Item.FindControl("lbtnSetEnable") as LinkButton;
                var lbtnSetDisEnable = e.Item.FindControl("lbtnSetDisEnable") as LinkButton;

                DomainEntity domainEntity = _serverProvider.GetDomainById(int.Parse(hdfDomainId.Value));

                if (domainEntity != null)
                {
                    if (domainEntity.Enable)
                    {
                        lbtnSetEnable.Visible = false;
                        lbtnSetDisEnable.Visible = true;
                    }
                    else
                    {
                        lbtnSetEnable.Visible = true;
                        lbtnSetDisEnable.Visible = false;
                    }
                }
                else
                {
                    lbtnSetEnable.Visible = false;
                    lbtnSetDisEnable.Visible = false;
                }
            }

            var tdDomain = e.Item.FindControl("tdDomain") as HtmlTableCell;
            var tdEnvironment = e.Item.FindControl("tdEnvironment") as HtmlTableCell;
            if (tdDomain == null) return;
            var item = e.Item.DataItem as ServerEntity;
            if (item.DomainId == 0) return;
            if (_domainDic.ContainsKey(item.DomainId))
            {
                tdDomain.Visible = false;
                tdEnvironment.Visible = false;
            }
            else
            {
                int count = _serverList.Count(x => x.DomainId == item.DomainId);
                tdDomain.RowSpan = count;
                tdEnvironment.RowSpan = count;
                _domainDic.Add(item.DomainId, count);
            }
        }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            //获取服务器列表
            _serverList = ddlDomain.SelectedValue.Equals("0") ? _serverProvider.GetServersByIdcForServerList(0) : _serverProvider.GetServersByDomainIdForServerList(Convert.ToInt32(ddlDomain.SelectedValue));
            _updateProvider.UpdateClientService(_serverList, CurUserName);
        }

        #region 分页

        protected void lbtnFirst_Click(object sender, EventArgs e)
        {
            _beginIndex = 1;
            InitServerData();
            ViewState["CurrentPage"] = "1";
            InitPage();
        }

        protected void lbtnPre_Click(object sender, EventArgs e)
        {
            _currentPage = Convert.ToInt32(lblCurrentPage.Text);
            if (_currentPage > 1)
            {
                _beginIndex = (_currentPage - 2) * PageSize + 1;
                InitServerData();
                ViewState["CurrentPage"] = (_currentPage - 1).ToString();
                InitPage();
            }
        }

        protected void lbtnNext_Click(object sender, EventArgs e)
        {
            _currentPage = Convert.ToInt32(lblCurrentPage.Text);
            _totalPage = Convert.ToInt32(lblTotalPage.Text);
            if (_currentPage < _totalPage)
            {
                _beginIndex = _currentPage * PageSize + 1;
                InitServerData();
                ViewState["CurrentPage"] = (_currentPage + 1).ToString();
                InitPage();
            }
        }

        protected void lbtnLast_Click(object sender, EventArgs e)
        {
            _totalPage = Convert.ToInt32(lblTotalPage.Text);
            _beginIndex = (_totalPage - 1) * PageSize + 1;
            InitServerData();
            ViewState["CurrentPage"] = lblTotalPage.Text;
            InitPage();
        }

        /// <summary>
        ///     初始化分页
        /// </summary>
        public void InitPage()
        {
            if (ddlDomain.SelectedValue == "0")
            {
                int recordCount;
                if (CurUserIsAdmin)
                {
                    recordCount = _serverProvider.GetAllServersCountForServerList();
                }
                else
                {
                    //recordCount = _serverProvider.GetAllServersCount(CurUserName);
                    recordCount = _serverProvider.GetManageServerCountForServerList(CurUserName, EnumManageType.DailyManage, null);
                }
                _totalPage = Convert.ToInt32(Math.Ceiling(recordCount / (double)PageSize));

                lblTotalPage.Text = _totalPage.ToString();
                lblCurrentPage.Text = ViewState["CurrentPage"] == null ? "1" : ViewState["CurrentPage"].ToString();
                _currentPage = Convert.ToInt32(lblCurrentPage.Text);

                lbtnFirst.Enabled = true;
                lbtnPre.Enabled = true;
                lbtnNext.Enabled = true;
                lbtnLast.Enabled = true;

                if (_currentPage == 1)
                {
                    lbtnFirst.Enabled = false;
                    lbtnPre.Enabled = false;
                }
                if (_currentPage == _totalPage)
                {
                    lbtnNext.Enabled = false;
                    lbtnLast.Enabled = false;
                }
            }
            else
            {
                lblCurrentPage.Text = "1";
                lblTotalPage.Text = "1";
                lbtnFirst.Enabled = false;
                lbtnPre.Enabled = false;
                lbtnNext.Enabled = false;
                lbtnLast.Enabled = false;
            }
        }

        #endregion 分页

        protected void btnStartSelectedServers_Click(object sender, EventArgs e)
        {
            //定义操作日志
            var operateLogEntity = new OperationLogEntity
            {
                UserName = CurUserName,
                DomainName = "All",
            };

            string strServerIds = this.selectedServers.Value.ToString();
            foreach (string serverId in strServerIds.Split(','))
            {
                if (string.IsNullOrWhiteSpace(serverId) || serverId.Trim() == "0")
                    continue;
                ServerEntity serverEntity = _serverProvider.GetServerByIdForServerList(Convert.ToInt32(serverId));

                operateLogEntity.OperateType = EnumOperateType.ServerManage;
                operateLogEntity.Log = string.Format("启用服务器，服务器Id:{0}，服务器类型:{1}，IP:{2}", serverEntity.ServerId,
                                                     serverEntity.ServerTypeName, serverEntity.IP);

                if (!_serverProvider.UnDeleteServer(Convert.ToInt32(serverId)))
                {
                    operateLogEntity.Result = false;
                    _logProvider.AddOperateLog(operateLogEntity);

                    ShowAlert("对不起，启用失败！");
                    return;
                }
                operateLogEntity.Result = true;
                _logProvider.AddOperateLog(operateLogEntity);
            }
            ShowAlert("启用成功！");
            InitServerData();
            return;
        }

        protected void btnStopSelectedServers_Click(object sender, EventArgs e)
        {
            //定义操作日志
            var operateLogEntity = new OperationLogEntity
            {
                UserName = CurUserName,
                DomainName = "All",
            };
            string strServerIds = this.selectedServers.Value.ToString();
            foreach (string serverId in strServerIds.Split(','))
            {
                if (string.IsNullOrWhiteSpace(serverId) || serverId.Trim() == "0")
                    continue;
                ServerEntity serverEntity = _serverProvider.GetServerByIdForServerList(Convert.ToInt32(serverId));

                operateLogEntity.OperateType = EnumOperateType.ServerManage;
                operateLogEntity.Log = string.Format("停用服务器，服务器Id:{0}，服务器类型:{1}，IP:{2}", serverEntity.ServerId,
                                                     serverEntity.ServerTypeName, serverEntity.IP);

                if (!_serverProvider.DeleteServer(Convert.ToInt32(serverId)))
                {
                    operateLogEntity.Result = false;
                    _logProvider.AddOperateLog(operateLogEntity);

                    ShowAlert("对不起，停用失败！");
                    return;
                }
                operateLogEntity.Result = true;
                _logProvider.AddOperateLog(operateLogEntity);
            }
            ShowAlert("停用成功！");
            InitServerData();
            return;
        }
    }
}