#region using

using System;
using Dorado.VWS.Model;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class AddIDC : WebBasePage
    {
        /// <summary>
        ///     定义服务器业务逻辑类
        /// </summary>
        private readonly ServerProvider serverProvider = new ServerProvider();

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            var idcEntity = new IdcEntity { IdcName = txtName.Text.Trim(), Description = txtDescription.Text.Trim() };

            if (serverProvider.InsertIdc(idcEntity))
            {
                ShowAlertAndRedirect("添加成功！", "ServerList.aspx");
            }
            else
            {
                ShowAlert("存在同名的名称或者服务器异常，请重新添加！");
                txtName.Focus();
            }
        }
    }
}