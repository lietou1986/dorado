#region using

using System;
using Dorado.VWS.Model;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class EditResource : WebBasePage
    {
        private readonly UserResourceProvider _userResourceProvider = new UserResourceProvider();
        private int resourceId;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                resourceId = Convert.ToInt32(Request.QueryString["id"]);
            }
            catch
            {
                ShowAlert("请选择您要编辑的资源！");
                return;
            }
            if (!IsPostBack)
            {
                InitData();
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            ResourceEntity resourceEntity = _userResourceProvider.GetResourceById(resourceId);

            resourceEntity.ResourceValue = txtValue.Text.Trim();
            resourceEntity.ResourceDescription = txtDescription.Text.Trim();

            if (_userResourceProvider.UpdateResource(resourceEntity))
            {
                ShowAlertAndRedirect("修改成功！", "ResourceList.aspx");
            }
            else
            {
                ShowAlert("可能存在相同的资源值或者服务器异常，修改失败！");
            }
        }

        /// <summary>
        ///     初始资源数据
        /// </summary>
        private void InitData()
        {
            ResourceEntity resourceEntity = _userResourceProvider.GetResourceById(resourceId);
            if (resourceEntity != null)
            {
                txtValue.Text = resourceEntity.ResourceValue;
                txtDescription.Text = resourceEntity.ResourceDescription;
            }
            else
            {
                ShowAlert("您选择的资源不存在！");
                return;
            }
        }
    }
}