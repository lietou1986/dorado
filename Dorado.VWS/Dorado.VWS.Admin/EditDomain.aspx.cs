#region using

using System;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class EditDomain : WebBasePage
    {
        /// <summary>
        ///     定义服务器业务逻辑类
        /// </summary>
        private readonly ServerProvider _serverProvider = new ServerProvider();

        private int domainId;

        protected void Page_Load(object sender, EventArgs e)
        {
            domainId = Convert.ToInt32(Request.QueryString["id"]);
            if (!IsPostBack)
            {
                InitData();
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            DomainEntity domainEntity = _serverProvider.GetDomainById(domainId);

            domainEntity.DomainName = txtDomain.Text.Trim();
            domainEntity.IdcId = Convert.ToInt32(ddlIDC.SelectedValue);
            domainEntity.Environment = ddlEnvironment.SelectedValue.ToString();
            domainEntity.WinServiceName = txtWinServerName.Text.Trim();
            domainEntity.IISSiteName = txtIISSiteName.Text.Trim();
            domainEntity.CacheUrl = txtCacheUrl.Text.Trim();
            domainEntity.SyncType = Convert.ToInt32(ddlSyncType.SelectedValue);
            domainEntity.HtmlCompress = RadlHtml.SelectedValue.Equals("是");

            domainEntity.JsCssCompress = RadlJsCss.SelectedValue.Equals("是");
            domainEntity.DomainType = rdlDomainType.SelectedValue.Equals("Windows") ?
                                      EnumDomainType.Windows : EnumDomainType.Linux;
            if (rdlDomainType.SelectedValue.Equals("Linux"))
            {
                domainEntity.OperatePathType = rdlOperatePathType.SelectedValue.Equals("Tomcat") ? EnumOperatePathType.Tomcat : EnumOperatePathType.Jar;
                domainEntity.OperatePath = txtdomainRoot.Text.Trim();
            }
            else
            {
                domainEntity.OperatePathType = EnumOperatePathType.other;
                domainEntity.OperatePath = string.Empty;
            }

            if (_serverProvider.UpdateDomain(domainEntity))
            {
                ShowAlertAndRedirect("修改成功！", "DomainList.aspx");
            }
            else
            {
                ShowAlert("可能存在相同的域名或者服务器异常，修改失败！");
            }
        }

        /// <summary>
        ///     初始域名数据
        /// </summary>
        private void InitData()
        {
            DomainEntity domainEntity = _serverProvider.GetDomainById(domainId);

            InitIdcData();
            InitEnvironmentData();
            ddlIDC.SelectedValue = domainEntity.IdcId.ToString();
            ddlEnvironment.SelectedValue = domainEntity.Environment.ToString();
            ddlSyncType.SelectedValue = domainEntity.SyncType.ToString();
            txtDomain.Text = domainEntity.DomainName;
            txtCacheUrl.Text = domainEntity.CacheUrl;
            txtWinServerName.Text = domainEntity.WinServiceName;
            txtIISSiteName.Text = domainEntity.IISSiteName;

            RadlHtml.SelectedValue = domainEntity.HtmlCompress ? "是" : "否";

            RadlJsCss.SelectedValue = domainEntity.JsCssCompress ? "是" : "否";
            rdlDomainType.SelectedValue = domainEntity.DomainType.ToString();
            txtdomainRoot.Text = domainEntity.OperatePath;
            rdlOperatePathType.SelectedValue = domainEntity.OperatePathType.ToString();
            if (domainEntity.DomainType == EnumDomainType.Windows || domainEntity.DomainType == EnumDomainType.other)
            {
                trPath.Visible = false;
                trPathType.Visible = false;
            }
            else
            {
                trPath.Visible = true;
                trPathType.Visible = true;
                if (rdlOperatePathType.SelectedValue.Equals("Tomcat"))
                {
                    lblDomainRoot.Text = "请输入Tomcat路径：";
                }
                else
                {
                    lblDomainRoot.Text = "请输入启动脚本路径：";
                }
            }
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

        protected void rdlDomainType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rdlDomainType.SelectedValue.Equals("Windows"))
            {
                trPath.Visible = false;
                trPathType.Visible = false;
            }
            else
            {
                trPath.Visible = true;
                trPathType.Visible = true;
            }
        }

        protected void rdlOperatePathType_SelectedIndexChanged(object sender, EventArgs e)
        {
            trPath.Visible = true;
            if (rdlOperatePathType.SelectedValue.Equals("Tomcat"))
            {
                lblDomainRoot.Text = "请输入Tomcat路径：";
            }
            else
            {
                lblDomainRoot.Text = "请输入启动脚本路径：";
            }
        }
    }
}