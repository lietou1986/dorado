/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 时间： 2011/11/24 13:56:48
 * 作者：
 * 版本            时间                  作者                 描述
 * v 1.0    2011/11/24 13:56:48               创建
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

//using Dorado.VWS.Admin.PermissionWCF;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;
using Dorado.VWS.Utils;

namespace Dorado.VWS.Admin
{
    public partial class UserPermission : WebBasePage
    {
        private readonly LogProvider _logProvider = new LogProvider();
        private readonly DomainProvider _domianProvider = new DomainProvider();
        private readonly DomainPermissionProvider _domianPermissionProvider = new DomainPermissionProvider();
        //private readonly PermissionWCFClient permissionClient = new PermissionWCFClient();

        private IList<string> allUsers;
        private IList<string> permissionUsers = new List<string>();
        private int domainID;

        //private int permissionID;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //ddlDomains.DataSource = GetUserDomians(EnumManageType.DailyManage);
                //ddlDomains.DataTextField = "DomainName";
                //ddlDomains.DataValueField = "DomainId";
                //ddlDomains.DataBind();
                ddlDomains.Items.Insert(0, new ListItem("请选择", "0"));

                if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
                {
                    ddlDomains.ClearSelection();
                    ddlDomains.Items.FindByValue(Request.QueryString["id"]).Selected = true;
                }
                //EnumManageType[] ManageTypes = (EnumManageType[])Enum.GetValues(typeof(Model.Enum.EnumManageType));
                //foreach (EnumManageType permission in  ManageTypes)
                //{
                //    ddlPermissions.Items.Add(new ListItem(EnumHelper.GetDescription(permission), ((int)permission).ToString()));

                //}

                Permission1.Visible = CurUserIsAdmin;

                //ddlPermissions.Items.Insert(0, new ListItem("请选择", "0"));
                //int.TryParse(ddlDomains.SelectedValue, out domainID);
                //permissionID = Convert.ToInt32(ddlPermissions.SelectedValue);
                InitEnvironmentData();
                BindUser();
                //BindPermission(0, "");
            }
            else
            {
                int.TryParse(ddlDomains.SelectedValue, out domainID);
                //permissionID = Convert.ToInt32(ddlPermissions.SelectedValue);
            }
        }

        protected void ddlDomains_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BindUser();
            BindPermission(domainID, lbxAllUsers.SelectedValue);
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
            ddlDomains.DataSource = GetUserDomians(EnumManageType.DailyManage, DdlEnvironment.SelectedValue);

            ddlDomains.DataValueField = "DomainId";
            ddlDomains.DataTextField = "DomainName";
            ddlDomains.DataBind();
            ddlDomains.Items.Insert(0, new ListItem("请选择", "0"));
        }

        protected void BindUser()
        {
            //Todo:
            //allUsers = permissionClient.GetAllUsers(AppSettingProvider.Get("Dorado.Permission.AppCode"]);

            ActivateUserProvider userProvider = new ActivateUserProvider();

            allUsers = userProvider.GetAllUsers().Select(x => x.UserName).ToList();
            lbxAllUsers.DataSource = allUsers;
            //lbxAllUsers.DataTextField = "";
            lbxAllUsers.DataBind();
            //lbxAllUsers.Items.Clear();

            //if (!string.IsNullOrWhiteSpace(ddlPermissions.SelectedValue))
            //{
            //    allUsers = permissionClient.GetAllUsers(AppSettingProvider.Get("Dorado.Permission.AppCode"]);
            //    //IList<DomainPermissionEntity> domainPermissions = _domianPermissionProvider.GetUsersByDomainAndPermissionType(domainID, permissionID);
            //    //foreach (DomainPermissionEntity p in domainPermissions)
            //    //{
            //    //    permissionUsers.Add(p.UserName);
            //    //}

            //    //foreach (string user in allUsers)
            //    //{
            //    //    if (!permissionUsers.Contains(user))
            //    //    {
            //    //        lbxAllUsers.Items.Add(user);
            //    //    }
            //    //}

            //}
        }

        protected void loadPermission()
        {
        }

        protected void BindPermission(int domianID, string userName)
        {
            //IList<DomainPermissionEntity> userPermissions;
            //if(domainID>0 &&!string.IsNullOrWhiteSpace(userName)){
            var userPermissions = _domianPermissionProvider.GetPermission(domainID, userName);

            //}

            clearPermisson();
            foreach (var p in userPermissions)
            {
                string id = "Permission" + p.PermissionType;
                var cbx = (CheckBox)PermissionContainter.FindControl(id);
                if (cbx != null)
                {
                    cbx.Checked = true;
                }
            }
        }

        protected void clearPermisson()
        {
            foreach (Control control in PermissionContainter.Controls)
            {
                ((CheckBox)control).Checked = false;
                //Response.Write(control.ID);
            }
        }

        protected void permissionAdd(int domianID, string userName, int permissionID)
        {//定义操作日志
            var operateLogEntity = new OperationLogEntity
            {
                UserName = CurUserName,
                DomainName = ddlDomains.SelectedItem.Text,
                OperateType = EnumOperateType.PermissionManage,
                Log = string.Format("添加权限成功，用户名:{0}，权限{1}",
                    userName,
                    EnumHelper.GetDescription((EnumManageType)permissionID)
                )
            };

            try
            {
                _domianPermissionProvider.Add(new DomainPermissionEntity()
                {
                    DomainID = domainID,
                    PermissionType = permissionID,
                    UserName = userName,
                    AddTime = DateTime.Now,
                    AddUserName = CurUserName,
                    UpdateTime = DateTime.Now,
                    UpdateUserName = CurUserName,
                    DeleteFlag = false
                });

                operateLogEntity.Result = true;
            }
            catch (Exception ex)
            {
                operateLogEntity.Result = false;
                operateLogEntity.Log = string.Format("添加权限失败，用户名:{0}，权限{1}，error:{2}",
                    userName,
                    EnumHelper.GetDescription((EnumManageType)permissionID),
                    ex.ToString()
                );
            }

            _logProvider.AddOperateLog(operateLogEntity);
        }

        protected void permissionDelete(int domianID, string userName, int permissionID)
        {
            //定义操作日志
            var operateLogEntity = new OperationLogEntity
            {
                UserName = CurUserName,
                DomainName = ddlDomains.SelectedItem.Text,
                OperateType = EnumOperateType.PermissionManage,
                Log = string.Format("删除权限成功，用户名:{0}，权限{1}",
                    userName,
                    EnumHelper.GetDescription((EnumManageType)permissionID)
                )
            };
            try
            {
                _domianPermissionProvider.Delete(new DomainPermissionEntity()
                {
                    DomainID = domainID,
                    PermissionType = permissionID,
                    UserName = userName,
                    UpdateTime = DateTime.Now,
                    UpdateUserName = CurUserName,
                    DeleteFlag = true
                });
                operateLogEntity.Result = true;
            }
            catch (Exception ex)
            {
                operateLogEntity.Result = false;
                operateLogEntity.Log = string.Format("删除权限失败，用户名:{0}，权限{1}，error:{2}",
                    userName,
                    EnumHelper.GetDescription((EnumManageType)permissionID),
                    ex.ToString()
                );
            }

            _logProvider.AddOperateLog(operateLogEntity);
        }

        protected bool checkPMS()
        {
            if (int.Parse(ddlDomains.SelectedValue) > 0 && !string.IsNullOrWhiteSpace(lbxAllUsers.SelectedValue))
            {
                return true;
            }
            else
            {
                ShowAlert("请选择正确的域名和用户");
                return false;
            }
        }

        protected void lbxAllUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (domainID > 0 && !string.IsNullOrWhiteSpace(lbxAllUsers.SelectedValue))
            {
                BindPermission(domainID, lbxAllUsers.SelectedValue);
            }
        }

        protected void cbxlstPermission_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void PermissionCbx_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkPMS())
            {
                return;
            }
            CheckBox cbx = (CheckBox)sender;
            if (cbx.Checked)
            {
                permissionAdd(domainID, lbxAllUsers.SelectedValue, int.Parse(cbx.CssClass));
            }
            else
            {
                permissionDelete(domainID, lbxAllUsers.SelectedValue, int.Parse(cbx.CssClass));
            }
        }

        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (ddlDomains.SelectedValue != "0")
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = true;
            }
        }
    }
}