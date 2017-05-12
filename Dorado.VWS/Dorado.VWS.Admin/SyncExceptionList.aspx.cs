#region using

using System;
using System.Web.Services;
using System.Web.UI.WebControls;
using Dorado.VWS.Model.Enum;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class SyncExceptionList : WebBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitEnvironmentData();
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

        protected void DdlEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlDomains.DataSource = GetUserDomians(EnumManageType.DailyManage, DdlEnvironment.SelectedValue);

            ddlDomains.DataValueField = "DomainName";
            ddlDomains.DataTextField = "DomainName";
            ddlDomains.DataBind();
            ddlDomains.Items.Insert(0, new ListItem("请选择", "0"));
        }

        /// <summary>
        /// 非安全的把同步任务置为结束
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [WebMethod]
        public bool ClearSyncException(string taskId)
        {
            return true;
        }
    }
}