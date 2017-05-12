#region using

using System;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class EditServer : WebBasePage
    {
        private readonly LogProvider _logProvider = new LogProvider();

        /// <summary>
        ///     定义服务器业务逻辑类
        /// </summary>
        private readonly ServerProvider _serverProvider = new ServerProvider();

        private int _serverId;

        protected void Page_Load(object sender, EventArgs e)
        {
            _serverId = Convert.ToInt32(Request.QueryString["id"]);
            if (!IsPostBack)
            {
                InitData();
            }
        }

        protected void ddlIDC_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitDomainData();
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            ServerEntity serverEntity = _serverProvider.GetServerById(_serverId);

            //定义操作日志
            var operateLogEntity = new OperationLogEntity
                                                      {
                                                          UserName = CurUserName,
                                                          DomainName = ddlDomain.SelectedItem.Text,
                                                          OperateType = EnumOperateType.ServerManage,
                                                          Log =
                                                              string.Format(
                                                                  "修改服务器，服务器类型:{0}，修改前IP:{1}，修改后IP:{2}，修改前域名:{3}，修改后域名:{4}",
                                                                  serverEntity.ServerTypeName, serverEntity.IP,
                                                                  txtIP.Text.Trim(), serverEntity.DomainName,
                                                                  ddlDomain.SelectedItem.Text)
                                                      };

            serverEntity.IdcId = Convert.ToInt32(ddlIDC.SelectedValue);
            serverEntity.DomainId = Convert.ToInt32(ddlDomain.SelectedValue);
            serverEntity.ServerType = (EnumServerType)Convert.ToInt32(ddlServerType.SelectedValue);
            serverEntity.IP = txtIP.Text.Trim();
            serverEntity.Root = txtRoot.Text.Trim();
            serverEntity.IISStatus = EnumIISStatus.Running;
            serverEntity.Updator = CurUserName;
            serverEntity.IsAdvanced = chkIsAdvanced.Checked;

            if (_serverProvider.UpdateServer(serverEntity))
            {
                operateLogEntity.Result = true;
                ShowAlertAndRedirect("修改成功！", "ServerList.aspx?id=" + serverEntity.DomainId);
                return;
            }
            else
            {
                operateLogEntity.Result = false;
                if (ddlServerType.SelectedValue == "1")
                {
                    ShowAlert("同一域名下不能出现多台宿主一个IP，修改失败！");
                }
                else if (ddlServerType.SelectedValue == "2")
                {
                    ShowAlert("一个IDC下同域名的同步中继只能有一个，修改失败！");
                }
                else
                {
                    ShowAlert("一个域名只能有一个同步源，修改失败！");
                }
            }
            _logProvider.AddOperateLog(operateLogEntity);
        }

        /// <summary>
        ///     初始化Idc数据
        /// </summary>
        private void InitIdcData()
        {
            ddlIDC.DataSource = _serverProvider.GetAllIdcs();
            ddlIDC.DataValueField = "IdcId";
            ddlIDC.DataTextField = "IdcName";
            ddlIDC.DataBind();
        }

        /// <summary>
        ///     初始化域名数据
        /// </summary>
        private void InitDomainData()
        {
            //IList<DomainEntity> domainList = _serverProvider.GetDomainsByIdcId(0);
            //var query = from DomainEntity domain in domainList
            //            where (
            //                    from DomainPermissionEntity permission in domainPermissions
            //                    select permission.DomainID).Contains(domain.DomainId
            //                   )
            //            select domain;
            ddlDomain.DataSource = GetUserDomians(EnumManageType.DailyManage, ddlEnvironment.SelectedValue.ToString());
            ddlDomain.DataValueField = "DomainId";
            ddlDomain.DataTextField = "DomainName";
            ddlDomain.DataBind();
        }

        /// <summary>
        ///     初始服务器数据
        /// </summary>
        private void InitData()
        {
            ServerEntity serverEntity = _serverProvider.GetServerById(_serverId);
            if (!HasDomainPermission(serverEntity.DomainId, EnumManageType.DailyManage))
            {
                ShowAlertAndBack("你没有该域名的管理权限！");
                return;
            }

            InitIdcData();
            ddlIDC.SelectedValue = serverEntity.IdcId.ToString();

            InitEnvironmentData();
            ddlEnvironment.SelectedValue = serverEntity.Environment.ToString();

            InitDomainData();
            ddlDomain.SelectedValue = serverEntity.DomainId.ToString();
            ddlServerType.SelectedValue = Convert.ToInt32(serverEntity.ServerType).ToString();

            txtIP.Text = serverEntity.IP;
            txtRoot.Text = serverEntity.Root;
            chkIsAdvanced.Checked = serverEntity.IsAdvanced;
        }

        protected void ddlEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitDomainData();
        }

        private void InitEnvironmentData()
        {
            ddlEnvironment.Items.Insert(0, new ListItem("请选择", EnvironmentType.UnSelected));
            //ddlEnvironment.Items.Insert(1, new ListItem("开发", EnvironmentType.Development));
            //ddlEnvironment.Items.Insert(2, new ListItem("测试", EnvironmentType.Testing));
            ddlEnvironment.Items.Insert(1, new ListItem("验收", EnvironmentType.Acceptance));
            //ddlEnvironment.Items.Insert(4, new ListItem("预上线", EnvironmentType.Advanced));
            ddlEnvironment.Items.Insert(2, new ListItem("线上", EnvironmentType.Production));
        }
    }
}