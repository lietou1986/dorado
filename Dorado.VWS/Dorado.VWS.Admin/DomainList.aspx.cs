using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

namespace Dorado.VWS.Admin
{
    public partial class DomainList : WebBasePage
    {
        private IList<DomainEntity> domainList;
        private DomainProvider _domainProvider = new DomainProvider();
        private ServerProvider _serverProvider = new ServerProvider();

        private readonly LogProvider _logProvider = new LogProvider();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitIdcData();
                InitEnvironmentData();
                InitDomainData();
                initList();
            }
        }

        protected void ddlIDC_SelectedIndexChanged(object sender, EventArgs e)
        {
            //InitDomainData();
            //InitEnvironmentData();
            initList();
        }

        protected void ddlEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlDomain.DataSource = _serverProvider.GetDomainsByIdcIdAndEnvironment(ddlEnvironment.SelectedValue, int.Parse(ddlIDC.SelectedValue));

            ddlDomain.DataValueField = "DomainId";
            ddlDomain.DataTextField = "DomainName";
            ddlDomain.DataBind();
            ddlDomain.Items.Insert(0, new ListItem("请选择", "0"));
        }

        protected void ddlDomain_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var domainList = _serverProvider.GetDomainsByIdcId(0);
            if (domainList == null)
            {
                domainList = _serverProvider.GetDomainsByIdcId(int.Parse(ddlIDC.SelectedValue));
            }
            var query = from DomainEntity domain in domainList
                        where (domain.DomainName == ddlDomain.SelectedItem.Text)
                        select domain;

            gvwDomains.DataSource = query;
            gvwDomains.DataBind();
        }

        private void InitEnvironmentData()
        {
            ddlEnvironment.Items.Clear();
            ddlEnvironment.Items.Insert(0, new ListItem("请选择", EnvironmentType.UnSelected));
            //ddlEnvironment.Items.Insert(1, new ListItem("开发", EnvironmentType.Development));
            //ddlEnvironment.Items.Insert(2, new ListItem("测试", EnvironmentType.Testing));
            ddlEnvironment.Items.Insert(1, new ListItem("验收", EnvironmentType.Acceptance));
            //ddlEnvironment.Items.Insert(4, new ListItem("预上线", EnvironmentType.Advanced));
            ddlEnvironment.Items.Insert(2, new ListItem("线上", EnvironmentType.Production));
        }

        protected void gvwDomains_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            int domainID = Convert.ToInt32(e.CommandArgument);
            string domainName = _serverProvider.GetDomainById(domainID).DomainName;
            //定义操作日志
            var operateLogEntity = new OperationLogEntity
            {
                UserName = CurUserName,
                DomainName = domainName
            };

            #region 删除域名

            if (e.CommandName == "delDomain")
            {
                operateLogEntity.Log = string.Format("启用域名（状态），域名:{0}", domainName);
                if (!_serverProvider.DeleteDoamin(Convert.ToInt32(e.CommandArgument)))
                {
                    operateLogEntity.Result = false;
                    ShowAlert("对不起，删除失败！");
                    return;
                }
                operateLogEntity.Result = true;
                _logProvider.AddOperateLog(operateLogEntity);

                ShowAlert("删除成功！");
                InitDomainData();
                initList();
                //InitServerData();
                return;
            }

            #endregion 删除域名

            #region 启用域名

            if (e.CommandName == "setEnable")
            {
                operateLogEntity.Log = string.Format("启用域名（状态），域名:{0}", domainName);

                if (!_serverProvider.SetDomainEnable(Convert.ToInt32(e.CommandArgument)))
                {
                    operateLogEntity.Result = false;
                    _logProvider.AddOperateLog(operateLogEntity);

                    ShowAlert("对不起，域名启用失败！");
                    return;
                }
                operateLogEntity.Result = true;
                _logProvider.AddOperateLog(operateLogEntity);

                ShowAlert("域名已经置为启用状态！");
                //InitDomainData();
                initList();
                return;
            }

            #endregion 启用域名

            #region 禁用域名

            if (e.CommandName == "setDisable")
            {
                operateLogEntity.OperateType = EnumOperateType.ServerManage;
                operateLogEntity.Log = string.Format("停用域名（状态），域名:{0}", domainName);

                if (!_serverProvider.SetDomainDisable(Convert.ToInt32(e.CommandArgument)))
                {
                    operateLogEntity.Result = false;
                    _logProvider.AddOperateLog(operateLogEntity);

                    ShowAlert("对不起，域名停用失败！");
                    return;
                }
                operateLogEntity.Result = true;
                _logProvider.AddOperateLog(operateLogEntity);

                ShowAlert("域名已经置为停用状态！");
                //InitDomainData();
                initList();
                //InitServerData();
                return;
            }

            #endregion 禁用域名
        }

        #region 数据处理

        protected void initList()
        {
            if (domainList == null)
            {
                domainList = _serverProvider.GetDomainsByIdcId(int.Parse(ddlIDC.SelectedValue));
            }

            gvwDomains.DataSource = domainList;
            gvwDomains.DataBind();
        }

        /// <summary>
        ///     初始化域名数据
        /// </summary>
        private void InitDomainData()
        {
            //domainList = _serverProvider.GetDomainsByIdcId(int.Parse(ddlIDC.SelectedValue));
            //ddlDomain.DataSource = domainList;
            //ddlDomain.DataValueField = "DomainId";
            //ddlDomain.DataTextField = "DomainName";
            //ddlDomain.DataBind();
            ddlDomain.Items.Insert(0, new ListItem("请选择", "0"));
        }

        /// <summary>
        ///     初始化Idc数据
        /// </summary>
        private void InitIdcData()
        {
            ddlIDC.DataSource = _serverProvider.GetAllIdcs();
            ddlIDC.DataValueField = "IdcId";
            ddlIDC.DataTextField = "IdcName";
            ddlIDC.DataBind();
            ddlIDC.Items.Insert(0, new ListItem("请选择", "0"));
        }

        #endregion 数据处理

        protected void gvwDomains_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (DataBinder.Eval(e.Row.DataItem, "DomainType").ToString() == "Linux")
                {
                    e.Row.BackColor = Color.Yellow;
                }
            }
        }
    }
}