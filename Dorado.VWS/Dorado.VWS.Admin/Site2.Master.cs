using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Vancl.IC.VWS.SiteApp.PermissionWCF;

namespace Vancl.IC.VWS.SiteApp
{
    public partial class SiteMaster2 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string token = "";
                if (HttpContext.Current.Request.Cookies["SSOToken"] != null)
                    token = HttpContext.Current.Request.Cookies["SSOToken"].Value;
                PermissionWCFClient client = new PermissionWCFClient();
                string userId = client.GetUserId(token);                
                LblName.Text = userId; 
                limgr.Visible = true;                
            }
        }

        protected void LbtnLogonOut_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            Response.Cookies["UserType"].Value = null;
            Response.Cookies["UserType"].Expires = DateTime.Now;
            Response.Cookies["SSOToken"].Value = null;
            Response.Cookies["SSOToken"].Expires = DateTime.Now;
            Response.Cookies["SSOCurrentUser"].Expires = DateTime.Now;
            Response.Cookies["SSOTokenuserid"].Expires = DateTime.Now;
            Response.Cookies["SSOToken1"].Expires = DateTime.Now;
            Response.Clear();
            Response.Redirect("http://pms.vancl.com/SSO/Login.aspx?AppCode=VIDE&BackUrl=http://vws.vancl.com/Default.aspx", true);
        }
    }
}