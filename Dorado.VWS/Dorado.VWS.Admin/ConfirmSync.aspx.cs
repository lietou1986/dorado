using System;
using System.Collections.Generic;

using Dorado.VWS.Services;

namespace Dorado.VWS.Admin
{
    public partial class ConfirmSync : System.Web.UI.Page
    {
        /// <summary>
        /// 定义同步业务逻辑类
        /// </summary>
        private SyncProvider _syncProvider = new SyncProvider();

        private readonly string SessionVersionFileIdList = "SessionVersionFiles";
        private readonly string SessionDescription = "SessionDescription";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitFileData();
            }
        }

        private void InitFileData()
        {
            if (Session[SessionVersionFileIdList] != null)
            {
                IList<int> versionFileIdList = Session[SessionVersionFileIdList] as List<int>;

                //IList<VersionFileEntity> versionFileList = _syncProvider.GetVersionFile(versionFileIdList);

                //rptVersionFile.DataSource = versionFileList;
                //rptVersionFile.DataBind();
            }
            if (Session[SessionDescription] != null)
            {
                TxtLog.Text = Session[SessionDescription].ToString();
            }
        }

        protected void BtnSync_Click(object sender, EventArgs e)
        {
        }
    }
}