#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class Role : WebBasePage
    {
        private readonly LogProvider _logProvider = new LogProvider();
        private readonly ServerProvider _serverProvider = new ServerProvider();
        private readonly RoleProvider _roleProvider = new RoleProvider();

        protected string JSON { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitEnvironmentData();
                InItDrop();
                InitData(int.Parse(ddlAll.SelectedValue));
            }
        }

        private void InItDrop()
        {
            //ddlAll.DataSource = GetUserDomians(EnumManageType.DailyManage); ;
            //ddlAll.DataValueField = "DomainId";
            //ddlAll.DataTextField = "DomainName";
            //ddlAll.DataBind();
            ddlAll.Items.Insert(0, new ListItem("请选择", "0"));

            if (string.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                lnkAddRole.NavigateUrl = "AddRole.aspx?domainid=" + Request.QueryString["id"];

                ddlAll.ClearSelection();
                try
                {
                    ddlAll.Items.FindByValue(Request.QueryString["id"]).Selected = true;
                }
                catch
                {
                    //ShowAlertAndBack("id参数错误");
                }
            }
            else
            {
                lnkAddRole.NavigateUrl = "AddRole.aspx?domainid=";
            }
        }

        private void InitData(int domainID)
        {
            IList<RoleEntity> ilist = _roleProvider.GetAllRole(domainID);
            if (CurUserIsAdmin)
            {
                rptRole.DataSource = ilist;
            }
            else
            {
                var query = from RoleEntity role in ilist
                            where (
                                    from DomainPermissionEntity permission in domainPermissions
                                    where permission.PermissionType == 1
                                    select permission.DomainID)
                                    .Contains(role.DomainId)
                            select role;
                rptRole.DataSource = query.ToList();
            }

            rptRole.DataBind();

            var templist = (from c in ilist
                            group c by new
                                           {
                                               DomainID = c.DomainId,
                                               c.Domain
                                           }
                                into g
                                select new
                                           {
                                               Id = g.Key.DomainID,
                                               Count = (from x in g
                                                        select x).Count()
                                           }).ToList();

            var sb = new StringBuilder();
            new JavaScriptSerializer().Serialize(templist, sb);
            JSON = sb.ToString();
        }

        protected void rptRole_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "del")
            {
                var hdfDomainName = e.Item.FindControl("hdfDomainName") as HiddenField;
                var hdfRoleName = e.Item.FindControl("hdfRoleName") as HiddenField;

                //定义操作日志
                var operateLogEntity = new OperationLogEntity
                                                          {
                                                              UserName = CurUserName,
                                                              DomainName = hdfDomainName.Value,
                                                              OperateType = EnumOperateType.PermissionManage,
                                                              Log = string.Format("删除角色，角色名:{0}", hdfRoleName.Value)
                                                          };

                RoleEntity role = _roleProvider.GetRoleByID(Convert.ToInt32(e.CommandArgument));

                if (!HasDomainPermission(role.DomainId, EnumManageType.DailyManage))
                {
                    operateLogEntity.Log += "你没有该域名(ID:" + role.DomainId + ")的管理权限！";
                    _logProvider.AddOperateLog(operateLogEntity);

                    ShowAlert("你没有该域名(ID:" + role.DomainId + ")的管理权限！");
                    return;
                }

                _roleProvider.Delete(Convert.ToInt32(e.CommandArgument));

                operateLogEntity.Result = true;
                _logProvider.AddOperateLog(operateLogEntity);

                InitData(Convert.ToInt32(ddlAll.SelectedValue));
            }
            if (e.CommandName == "delDomainRoles")
            {
                int domainID = Convert.ToInt32(e.CommandArgument);

                if (!HasDomainPermission(domainID, EnumManageType.DailyManage))
                {
                    ShowAlert("你没有该域名(ID:" + domainID + ")的管理权限！");
                    return;
                }

                IList<RoleEntity> ilist = _roleProvider.GetAllRole(domainID);
                foreach (RoleEntity dto in ilist)
                {
                    _roleProvider.Delete(dto.Id);
                }
                InitData(Convert.ToInt32(ddlAll.SelectedValue));
            }
        }

        private void InitEnvironmentData()
        {
            DdlEnvironment.Items.Insert(0, new ListItem("请选择", EnvironmentType.UnSelected));
            //DdlEnvironment.Items.Insert(1, new ListItem("开发", EnvironmentType.Development));
            //DdlEnvironment.Items.Insert(2, new ListItem("测试", EnvironmentType.Testing));
            DdlEnvironment.Items.Insert(1, new ListItem("验收", EnvironmentType.Acceptance));
            //DdlEnvironment.Items.Insert(4, new ListItem("预上线", EnvironmentType.Advanced));
            DdlEnvironment.Items.Insert(2, new ListItem("线上", EnvironmentType.Production));
        }

        protected void DdlEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlAll.DataSource = GetUserDomians(EnumManageType.DailyManage, DdlEnvironment.SelectedValue);

            ddlAll.DataValueField = "DomainId";
            ddlAll.DataTextField = "DomainName";
            ddlAll.DataBind();
            ddlAll.Items.Insert(0, new ListItem("请选择", "0"));
        }

        protected void ddlAll_SelectedIndexChanged(object sender, EventArgs e)
        {
            lnkAddRole.NavigateUrl = "AddRole.aspx?domainid=" + ddlAll.SelectedValue;
            InitData(Convert.ToInt32(ddlAll.SelectedValue));
        }
    }
}