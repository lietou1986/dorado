using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;
using Dorado.VWS.Utils;

namespace Dorado.VWS.Admin
{
    public partial class SystemPermission : WebBasePage
    {
        private readonly LogProvider _logProvider = new LogProvider();
        private readonly DomainProvider _domianProvider = new DomainProvider();
        private readonly DomainPermissionProvider _domianPermissionProvider = new DomainPermissionProvider();
        private readonly SysytemRoleProvider _systemRoleProvider = new SysytemRoleProvider();
        //private readonly PermissionWCFClient permissionClient = new PermissionWCFClient();

        private IList<string> allUsers;
        private IList<string> permissionUsers = new List<string>();
        private int domainID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUser();
            }
        }

        protected void BindUser()
        {
            ActivateUserProvider userProvider = new ActivateUserProvider();

            allUsers = userProvider.GetAllUsers().Select(x => x.UserName).ToList();
            lbxAllUsers.DataSource = allUsers;
            lbxAllUsers.DataBind();
        }

        protected void lbxAllUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindPermission(lbxAllUsers.SelectedValue);
        }

        protected void BindPermission(string userName)
        {
            List<SystemRoleEntity> userPermissions = _systemRoleProvider.GetSysytemRoleByUser(userName).ToList();
            clearPermisson();
            foreach (var p in userPermissions)
            {
                string id = "Permission" + p.ID;
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
            }
        }

        protected void permissionAdd(string userName, int permissionID)
        {//定义操作日志
            var operateLogEntity = new OperationLogEntity
            {
                UserName = CurUserName,
                OperateType = EnumOperateType.SystemPermissionManage,
                Log = string.Format("添加系统权限成功，用户名:{0}，权限{1}",
                    userName,
                    EnumHelper.GetDescription((SysytemRoleEnumType)permissionID)
                )
            };

            try
            {
                _systemRoleProvider.Add(new UserRoleEntity()
                {
                    UserName = userName,
                    roleId = permissionID
                });

                operateLogEntity.Result = true;
            }
            catch (Exception ex)
            {
                operateLogEntity.Result = false;
                operateLogEntity.Log = string.Format("添加系统权限失败，用户名:{0}，权限{1}，error:{2}",
                    userName,
                    EnumHelper.GetDescription((SysytemRoleEnumType)permissionID),
                    ex.ToString()
                );
            }

            _logProvider.AddOperateLog(operateLogEntity);
        }

        protected void permissionDelete(string userName, int permissionID)
        {
            //定义操作日志
            var operateLogEntity = new OperationLogEntity
            {
                UserName = CurUserName,
                OperateType = EnumOperateType.PermissionManage,
                Log = string.Format("删除系统权限成功，用户名:{0}，权限{1}",
                    userName,
                    EnumHelper.GetDescription((SysytemRoleEnumType)permissionID)
                )
            };
            try
            {
                _systemRoleProvider.DeleteByUserRoleId(userName, permissionID);

                operateLogEntity.Result = true;
            }
            catch (Exception ex)
            {
                operateLogEntity.Result = false;
                operateLogEntity.Log = string.Format("删除系统权限失败，用户名:{0}，权限{1}，error:{2}",
                    userName,
                    EnumHelper.GetDescription((SysytemRoleEnumType)permissionID),
                    ex.ToString()
                );
            }

            _logProvider.AddOperateLog(operateLogEntity);
        }

        protected void PermissionCbx_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbx = (CheckBox)sender;

            foreach (Control control in PermissionContainter.Controls)
            {
                if (((CheckBox)control).Checked == true && ((CheckBox)control) != cbx)
                {
                    ((CheckBox)control).Checked = false;
                    permissionDelete(lbxAllUsers.SelectedValue, int.Parse(((CheckBox)control).CssClass));
                }
            }

            //cbx.Checked = true;
            if (cbx.Checked)
            {
                permissionAdd(lbxAllUsers.SelectedValue, int.Parse(cbx.CssClass));
            }
            else
            {
                permissionDelete(lbxAllUsers.SelectedValue, int.Parse(cbx.CssClass));
            }
        }
    }
}