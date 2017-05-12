#region using

using System;
using System.Web;
using System.Web.UI;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

//using Dorado.VWS.Admin.PermissionWCF;

using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class SiteMaster : MasterPage
    {
        private string _userId = string.Empty;

        protected bool IsAdmin = WebBasePage.CurUserIsAdmin;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["IsLogin"] == null)
            {
                string defaultPage = WebHttpHelper.GetUrl(Request, "Login.aspx");

                Response.Write("<script language=javascript>parent.location.href='" + defaultPage + "'</script>");

                return;
            }
            if (!IsPostBack)
            {
                string token = string.Empty;
                //TODO:去掉Permission服务
                //_userId = client.GetUserId(token);
                _userId = WebBasePage.GetUserName();
                LblName.Text = _userId;
                limgr.Visible = false;

                //DomainInfo[] infos = client.GetUserRolesByDomain(_userId, Config.AppCode);
                ////DomainInfo[] infos = new DomainInfo[] { };

                //if (infos == null || infos.Length == 0)
                //{
                //    ////注销PMS服务器凭据
                //    //new PermissionHelper().SignOut(Request["SSOToken"]);
                //    //// 返回页面
                //    //string defaultPage = string.Format(@"{0}://{1}/Default.aspx", Request.Url.Scheme, Request.Url.Host);
                //    //// 跳回登录页面
                //    //string backurl = Config.LoginUrl + "?AppCode=" + Config.AppCode + "&BackUrl=" + defaultPage;
                //    //Response.Write("<script language=javascript>parent.location.href='" + backurl + "'</script>");
                //    //return;
                //}
                limgr.Visible = true;
            }
        }

        protected void LbtnLogonOut_Click(object sender, EventArgs e)
        {
            Session.Clear();
            string defaultPage = WebHttpHelper.GetUrl(Request, "Login.aspx");
            Response.Write("<script language=javascript>parent.location.href='" + defaultPage + "'</script>");
        }

        protected bool CheckSysPermission(string username, SysytemRoleEnumType[] permissionTypes)
        {
            SysytemRoleProvider _systemRoleProvider = new SysytemRoleProvider();
            var sysrole = _systemRoleProvider.GetSysytemRoleByUser(username);
            if (sysrole.Count == 0)
                return false;
            foreach (var roleEnum in permissionTypes)
            {
                if (sysrole[0].ID == (int)roleEnum)
                    return true;
            }
            return false;
        }

        protected bool CheckSysUser(string username)
        {
            if (username.ToLower() == "len" || username.ToLower() == "yifan.lin" || username.ToLower() == "shanfeng.han" || username.ToLower() == "hector.ma")
                return true;
            return false;
        }
    }
}