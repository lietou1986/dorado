using System;
using System.Web.UI.WebControls;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Services;
using Dorado.VWS.Utils;

namespace Dorado.VWS.Admin
{
    public partial class Login : WebBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
        }

        protected void LoginUser_Authenticate(object sender, AuthenticateEventArgs e)
        {
            try
            {
                LoginProvider provider = new LoginProvider();
                bool isAdmin = false;
                //bool loginPass = provider.Login(LoginUser.UserName, LoginUser.Password, ref isAdmin);

                if (true)
                {
                    CookieManager cookieMgr = new CookieManager();
                    cookieMgr.setCookie("UserName", LoginUser.UserName);
                    Session["IsLogin"] = true;
                    CurUserIsAdmin = isAdmin;

                    string defaultPage = WebHttpHelper.GetUrl(Request, "Default.aspx");
                    Response.Write("<script language=javascript>parent.location.href='" + defaultPage + "'</script>");
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(ex);
            }
        }
    }
}