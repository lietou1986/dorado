#region using

using System;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class AddDomain : WebBasePage
    {
        /// <summary>
        ///     定义服务器业务逻辑类
        /// </summary>
        private readonly ServerProvider _serverProvider = new ServerProvider();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitEnvironmentData();
                InitIdcData();
            }
        }

        private void InitEnvironmentData()
        {
            ddlEnvironment.Items.Insert(0, new ListItem("请选择", EnvironmentType.UnSelected));
            ddlEnvironment.Items.Insert(1, new ListItem("验收", EnvironmentType.Acceptance));
            ddlEnvironment.Items.Insert(2, new ListItem("线上", EnvironmentType.Production));
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            var domainEntity = new DomainEntity
                                   {
                                       DomainName = txtDomain.Text.Trim(),
                                       IdcId = Convert.ToInt32(ddlIDC.SelectedValue),
                                       Environment = ddlEnvironment.SelectedValue,
                                       WinServiceName = txtWinServerName.Text.Trim(),
                                       IISSiteName = txtIISSiteName.Text.Trim(),
                                       CacheUrl = txtCacheUrl.Text.Trim(),
                                       SyncType = Convert.ToInt32(ddlSyncType.SelectedValue),
                                       HtmlCompress = RadlHtml.SelectedValue.Equals("是"),
                                       JsCssCompress = RadlJsCss.SelectedValue.Equals("是"),
                                       DomainType = rdlDomainType.SelectedValue.Equals("Windows") ?
                                                    EnumDomainType.Windows : EnumDomainType.Linux,
                                       OperatePathType = rdlOperatePathType.SelectedValue.Equals("Tomcat") ?
                                                                            EnumOperatePathType.Tomcat :
                                                                            EnumOperatePathType.Jar,
                                       OperatePath = txtdomainRoot.Text.Trim(),
                                   };

            if (_serverProvider.InsertDomain(domainEntity))
            {
                ShowAlertAndRedirect("添加成功！", "DomainList.aspx");
            }
            else
            {
                ShowAlert("存在相同的域名或者服务器异常，请重新添加！");
                txtDomain.Focus();
            }
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