using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Dorado.VWS.Services;

namespace Dorado.VWS.Admin
{
    public partial class TestPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IDictionaryEnumerator CacheEnum = HttpContext.Current.Cache.GetEnumerator();
            var cachekey = new List<string>();
            while (CacheEnum.MoveNext())
            {
                cachekey.Add(CacheEnum.Key.ToString());
                //HttpContext.Current.Cache.Remove(CacheEnum.Key.ToString());
                Response.Write("<span sytle=\"Color:red;\">" + CacheEnum.Key.ToString() + "</span><br/>");
            }
            Response.Write("<span sytle=\"Color:red;\">cache count:" + cachekey.Count + "</span><br/>");
            if (!IsPostBack)
            {
                Session["clear"] = false;
            }
        }

        protected void btnPerssion_Click(object sender, EventArgs e)
        {
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (Session["clear"] == null || (bool)Session["clear"])
            {
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('不要重复提交！');__doPostBack('Refresh','');", true);
                Button1.Enabled = false;
                return;
            }
            Regex reg = new Regex("allowIPs_.*|allowservice_.*", RegexOptions.IgnoreCase);
            Response.Write("removed:" + WebCache.ClearAll(reg) + ";");
            Button1.Enabled = false;
            Session["clear"] = true;
        }
    }
}