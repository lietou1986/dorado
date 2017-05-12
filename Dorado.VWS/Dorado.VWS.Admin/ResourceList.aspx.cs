#region using

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class ResourceList : WebBasePage
    {
        private readonly UserResourceProvider _userResourceProvider = new UserResourceProvider();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        protected void rptResource_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "del")
            {
                _userResourceProvider.DeleteResource(Convert.ToInt32(e.CommandArgument));
                InitData();
            }
        }

        private void InitData()
        {
            IList<ResourceEntity> ilist = _userResourceProvider.GetResourceList();

            rptResource.DataSource = ilist;
            rptResource.DataBind();
        }
    }
}