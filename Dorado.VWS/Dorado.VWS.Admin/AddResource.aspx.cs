#region using

using System;
using Dorado.VWS.Model;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class AddResource : WebBasePage
    {
        private readonly UserResourceProvider _userResourceProvider = new UserResourceProvider();

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            var resourceEntity = new ResourceEntity
                                     {
                                         ResourceValue = txtValue.Text.Trim(),
                                         ResourceDescription = txtDescription.Text.Trim()
                                     };

            if (_userResourceProvider.InsertResource(resourceEntity))
            {
                ShowAlertAndRedirect("添加成功！", "ResourceList.aspx");
            }
            else
            {
                ShowAlert("存在同名的资源值或者服务器异常，请重新添加！");
                txtValue.Focus();
            }
        }
    }
}