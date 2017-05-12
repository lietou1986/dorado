#region using

using System;
using System.Web.UI.WebControls;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class FileRevert : WebBasePage
    {
        private readonly ServerProvider _serverProvider = new ServerProvider();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                InitEnvironmentData();
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

        protected void DdlEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            DdlDomains.DataSource = _serverProvider.GetDomainsByUser(CurUserName, 1, DdlEnvironment.SelectedValue);

            DdlDomains.DataValueField = "DomainId";
            DdlDomains.DataTextField = "DomainName";
            DdlDomains.DataBind();
            DdlDomains.Items.Insert(0, new ListItem("请选择", "0"));
        }
    }
}