#region using

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web.UI.WebControls;
using Vancl.IC.VWS.BLL;
using Vancl.IC.VWS.Model;
using Vancl.IC.VWS.Model.Enum;
using Vancl.IC.VWS.SiteApp.PermissionWCF;

#endregion

namespace Vancl.IC.VWS.SiteApp
{
    public partial class UserList : WebBasePage
    {
        private readonly LogBLL _logBLL = new LogBLL();

        private readonly RoleBLL bll = new RoleBLL();
        private readonly ServerBLL serverBll = new ServerBLL();
        private readonly UserRoleBLL userbll = new UserRoleBLL();
        private string userid = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            var client = new PermissionWCFClient();
            IList<string> listResource = new List<string>();

            if (!string.IsNullOrEmpty(Request.QueryString["userid"]))
            {
                userid = Request.QueryString["userid"];

                listResource = client.GetAllUsers(ConfigurationManager.AppSettings["Vancl.Permission.AppCode"]);
                if (!listResource.Contains(userid))
                {
                    ShowAlert("对不起，用户不存在");
                }
                else
                {
                    ltUser.Text = userid;
                    tabQuan.Visible = true;
                }
            }
            if (!IsPostBack)
            {
                if (listResource.Count == 0)
                {
                    listResource = client.GetAllUsers(ConfigurationManager.AppSettings["Vancl.Permission.AppCode"]);
                }

                var userRoleBll = new UserRoleBLL();
                IList<string> listUserRole = userRoleBll.GetVwsUserName();

                IList<string> notPermissionUsers = new List<string>();

                foreach (string userRole in listUserRole)
                {
                    //查询vws系统中给配置过角色的用户是否已经在PMS权限系统中解除vws权限
                    bool isPermission = false;
                    foreach (string vwsUserName in listResource)
                    {
                        if (userRole.Equals(vwsUserName))
                        {
                            isPermission = true;
                            break;
                        }
                    }
                    if (!isPermission)
                    {
                        notPermissionUsers.Add(userRole);
                    }
                }
                //删除无效的vws系统用户
                //userRoleBll.DeleteByUserName(notPermissionUsers);

                var sb = new StringBuilder();
                foreach (var str in listResource)
                {
                    sb.Append("<tr><td align='left'>" + str + "</td><td align='center'><a href='/UserList.aspx?userid=" +
                              str + "'>修改</a></td></tr>");
                }
                ltUserlist.Text = sb.ToString();
                if (!string.IsNullOrEmpty(userid))
                {
                    InItData();
                }
            }
        }

        private void InItData()
        {
            IList<DomainEntity> ilist = serverBll.GetDomainsByIdcId(0);

            rptRole.DataSource = ilist;
            rptRole.DataBind();
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in rptRole.Items)
            {
                var chkRoleList = item.FindControl("chkRoleList") as CheckBoxList;
                var hdfuserId = (HiddenField) item.FindControl("hdfUserID");
                var hdfDomainID = item.FindControl("hdfDomainID") as HiddenField;
                var hdfDomain = item.FindControl("hdfDomain") as HiddenField;
                var chkResourceList = item.FindControl("chkResourceList") as CheckBoxList;

                int domainId = Convert.ToInt32(hdfDomainID.Value);
                //获取老用户资源权限ID
                List<int> oldUserRoleIDList = userbll.GetUserRoleId(userid, domainId);

                var userRoleList = new List<UserRoleEntity>();

                bool isSelected = false;

                //定义操作日志
                var operateLogEntity = new OperationLogEntity
                                                          {
                                                              UserName = CurUserName,
                                                              DomainName = hdfDomain.Value,
                                                              OperateType = EnumOperateType.PermissionManage,
                                                              Log = string.Format("更新用户权限，用户：{0}，", ltUser.Text.Trim())
                                                          };
                var logBuilder = new StringBuilder("拥有的角色列表：");

                foreach (ListItem itemRole in chkRoleList.Items)
                {
                    if (itemRole.Selected)
                    {
                        isSelected = true;

                        var dto = new UserRoleEntity();

                        if (!string.IsNullOrEmpty(hdfuserId.Value))
                        {
                            dto.ID = Convert.ToInt32(hdfuserId.Value);
                        }
                        dto.RoleID = Convert.ToInt32(itemRole.Value);
                        dto.UserName = ltUser.Text.Trim();

                        //更新权限
                        userRoleList.Add(dto);

                        logBuilder.Append(string.Format("{0}#", itemRole.Text.Split('<')[0]));
                    }
                }

                if (isSelected)
                {
                    userbll.UpdateUserRole(userRoleList, domainId);
                }
                    //没有选择角色（即删除角色）
                else
                {
                    userbll.DeleteUserRole(userid, domainId);
                }

                //获取更新后用户资源权限ID
                List<int> newUserRoleIDList = userbll.GetUserRoleId(userid, domainId);

                var userResourceBll = new UserResourceBLL();

                var listResourceId = new List<int>();

                logBuilder.Append("，拥有的资源列表：");
                //修改资源权限
                foreach (ListItem itemResource in chkResourceList.Items)
                {
                    if (itemResource.Selected)
                    {
                        int resourceId = Convert.ToInt32(itemResource.Value);
                        listResourceId.Add(resourceId);

                        logBuilder.Append(string.Format("{0}#", itemResource.Text));
                    }
                }
                userResourceBll.UpdateUserResource(oldUserRoleIDList, newUserRoleIDList, listResourceId);

                operateLogEntity.Result = true;
                operateLogEntity.Log += logBuilder.ToString();
                _logBLL.AddOperateLog(operateLogEntity);
            }
            InItData();
            ShowAlert("修改成功！");
        }

        protected void rptRole_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem &&
                e.Item.ItemType != ListItemType.EditItem) return;

            var ddl = e.Item.FindControl("ddlRole") as DropDownList;
            var chkRoleList = e.Item.FindControl("chkRoleList") as CheckBoxList;
            var hdfDomainID = e.Item.FindControl("hdfDomainID") as HiddenField;
            var hdfuser = e.Item.FindControl("hdfUser") as HiddenField;
            var chkResourceList = e.Item.FindControl("chkResourceList") as CheckBoxList;
            var hdfroleid = e.Item.FindControl("hdfRoleID") as HiddenField;

            int domainId = Convert.ToInt32(hdfDomainID.Value);
            IList<RoleEntity> rolelist = bll.GetAllRole(domainId);

            foreach (RoleEntity roleEntity in rolelist)
            {
                chkRoleList.Items.Add(
                    new ListItem(
                        roleEntity.RoleName + "<A href='EditRole.aspx?id=" + roleEntity.Id.ToString() +
                        "'target=\"_BLANK\"> 编辑</A>", roleEntity.Id.ToString()));
            }
            //获取权限
            IList<RoleEntity> roleList = userbll.GetUserRole(domainId, userid);

            if (roleList.Count > 0)
            {
                foreach (ListItem item in chkRoleList.Items)
                {
                    foreach (RoleEntity RoleEntity in roleList)
                    {
                        if (item.Value.Equals(RoleEntity.Id.ToString()))
                        {
                            hdfroleid.Value = hdfroleid.Value + string.Format("{0}#", RoleEntity.Id);
                            item.Selected = true;
                            break;
                        }
                    }
                }

                var hdfuserId = (HiddenField) e.Item.FindControl("hdfUserID");
                hdfuserId.Value = roleList[0].UserId.ToString();
                hdfuser.Value = roleList[0].UserName;
            }

            ddl.DataSource = rolelist;
            ddl.DataTextField = "RoleName";
            ddl.DataValueField = "ID";
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("请选择", "0"));


            if (string.IsNullOrEmpty(hdfuser.Value))
            {
                hdfroleid.Value = "0";
            }

            var userResourceBll = new UserResourceBLL();

            List<int> userRoleIDList = userbll.GetUserRoleId(userid, domainId);

            //获取用户资源权限
            IList<UserResoureEntity> listUserResource = userResourceBll.GetUserResourcePermission(userRoleIDList);

            //获取资源列表
            IList<ResourceEntity> ResoureEntityList = userResourceBll.GetResourceList();


            chkResourceList.DataSource = ResoureEntityList;
            chkResourceList.DataTextField = "ResourceDescription";
            chkResourceList.DataValueField = "ResourceId";
            chkResourceList.DataBind();

            foreach (ListItem item in chkResourceList.Items)
            {
                foreach (UserResoureEntity UserResoureEntity in listUserResource)
                {
                    if (item.Value.Equals(UserResoureEntity.ResourceId.ToString()))
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
        }


        protected void rptRole_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "edit")
            {
                var hdfDomainID = e.Item.FindControl("hdfDomainID") as HiddenField;
                var hdfDomain = e.Item.FindControl("hdfDomain") as HiddenField;
                var chkResourceList = e.Item.FindControl("chkResourceList") as CheckBoxList;
                var hdfuserId = (HiddenField) e.Item.FindControl("hdfUserID");

                int domainId = Convert.ToInt32(hdfDomainID.Value);
                //获取老用户资源权限ID
                List<int> oldUserRoleIDList = userbll.GetUserRoleId(userid, domainId);

                var chkRoleList = e.Item.FindControl("chkRoleList") as CheckBoxList;

                var userRoleList = new List<UserRoleEntity>();

                bool isSelected = false;

                //定义操作日志
                var operateLogEntity = new OperationLogEntity
                                                          {
                                                              UserName = CurUserName,
                                                              DomainName = hdfDomain.Value,
                                                              OperateType = EnumOperateType.PermissionManage,
                                                              Log = string.Format("更新用户权限，用户：{0}，", ltUser.Text.Trim())
                                                          };
                var logBuilder = new StringBuilder("拥有的角色列表：");

                foreach (ListItem itemRole in chkRoleList.Items)
                {
                    if (itemRole.Selected)
                    {
                        isSelected = true;
                        var dto = new UserRoleEntity
                                      {RoleID = Convert.ToInt32(itemRole.Value), UserName = ltUser.Text.Trim()};

                        if (!string.IsNullOrEmpty(hdfuserId.Value))
                        {
                            dto.ID = Convert.ToInt32(hdfuserId.Value);
                        }
                        
                        userRoleList.Add(dto);

                        logBuilder.Append(string.Format("{0}#", itemRole.Text.Split('<')[0]));
                    }
                }
                if (isSelected)
                {
                    userbll.UpdateUserRole(userRoleList, domainId);
                }
                    //没有选择角色（即删除角色）
                else
                {
                    userbll.DeleteUserRole(userid, domainId);
                }

                //获取更新后用户资源权限ID
                List<int> newUserRoleIDList = userbll.GetUserRoleId(userid, domainId);

                var userResourceBll = new UserResourceBLL();

                var listResourceId = new List<int>();

                logBuilder.Append("，拥有的资源列表：");
                //修改资源权限
                foreach (ListItem itemResource in chkResourceList.Items)
                {
                    if (!itemResource.Selected) continue;

                    int resourceId = Convert.ToInt32(itemResource.Value);
                    listResourceId.Add(resourceId);

                    logBuilder.Append(string.Format("{0}#", itemResource.Text));
                }
                if (!userResourceBll.UpdateUserResource(oldUserRoleIDList, newUserRoleIDList, listResourceId))
                {
                    ShowAlert("对不起，修改失败！");

                    operateLogEntity.Result = false;
                    operateLogEntity.Log += logBuilder.ToString();
                    _logBLL.AddOperateLog(operateLogEntity);

                    return;
                }

                operateLogEntity.Result = true;
                operateLogEntity.Log += logBuilder.ToString();
                _logBLL.AddOperateLog(operateLogEntity);
            }

            InItData();
            ShowAlert("修改成功！");
        }

        protected void btnSeek_Click(object sender, EventArgs e)
        {
            string userName = txtUserName.Text.Trim();

            if (!string.IsNullOrEmpty(userName))
            {
                Response.Redirect("UserList.aspx?userid=" + userName);
            }
        }
    }
}