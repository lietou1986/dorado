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
using System.Web.UI.WebControls;

//using Dorado.VWS.Admin.PermissionWCF;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

namespace Dorado.VWS.Admin
{
    public partial class domainpermission : WebBasePage
    {
        private readonly DomainProvider _domianProvider = new DomainProvider();
        private readonly DomainPermissionProvider _domianPermissionProvider = new DomainPermissionProvider();
        //private readonly PermissionWCFClient permissionClient = new PermissionWCFClient();

        private IList<string> allUsers;
        private IList<string> permissionUsers = new List<string>();
        private int domainID;
        private int permissionID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlDomains.DataSource = GetUserDomians(EnumManageType.DailyManage);
                ddlDomains.DataTextField = "DomainName";
                ddlDomains.DataValueField = "DomainId";
                ddlDomains.DataBind();
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
                //if (!CurUserIsAdmin)
                //{
                //    ddlPermissions.Items.Remove(ddlPermissions.Items.FindByValue(((int)EnumManageType.DailyManage).ToString()));
                //}
                ddlPermissions.DataSource = GetManagePermission();
                ddlPermissions.DataTextField = "value";
                ddlPermissions.DataValueField = "key";
                ddlPermissions.DataBind();
                ddlPermissions.Items.Insert(0, new ListItem("请选择", "0"));
                int.TryParse(ddlDomains.SelectedValue, out domainID);
                permissionID = Convert.ToInt32(ddlPermissions.SelectedValue);

                BindUser();
            }
            else
            {
                int.TryParse(ddlDomains.SelectedValue, out domainID);
                permissionID = Convert.ToInt32(ddlPermissions.SelectedValue);
            }
        }

        protected void ddlDomains_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int.TryParse(ddlDomains.SelectedValue, out domainID);
            BindUser();
        }

        protected void ddlPermissions_SelectedIndexChanged(object sender, EventArgs e)
        {
            //permissionID = Convert.ToInt32(Enum.Parse(typeof(EnumManageType), ddlPermissions.SelectedValue, false));
            BindUser();
        }

        protected void BindUser()
        {
            lbxAllUsers.Items.Clear();
            lbxPermissionUsers.Items.Clear();

            if (!string.IsNullOrWhiteSpace(ddlPermissions.SelectedValue))
            {
                ActivateUserProvider userProvider = new ActivateUserProvider();

                allUsers = userProvider.GetAllUsers().Select(x => x.UserName).ToList();
                //TODO:已激活的平台用户
                //allUsers = permissionClient.GetAllUsers(AppSettingProvider.Get("Dorado.Permission.AppCode"]);

                IList<DomainPermissionEntity> domainPermissions = _domianPermissionProvider.GetUsersByDomainAndPermissionType(domainID, permissionID);
                foreach (DomainPermissionEntity p in domainPermissions)
                {
                    permissionUsers.Add(p.UserName);
                }

                foreach (string user in allUsers)
                {
                    if (!permissionUsers.Contains(user))
                    {
                        lbxAllUsers.Items.Add(user);
                    }
                }

                foreach (string user in permissionUsers)
                {
                    lbxPermissionUsers.Items.Add(user);
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (!checkPMS())
            {
                return;
            }

            if (lbxAllUsers.Items.Count <= 0)
            {
                return;
            }
            List<ListItem> items = new List<ListItem>();
            for (int i = 0; i < lbxAllUsers.Items.Count; i++)
            {
                items.Add(lbxAllUsers.Items[i]);

                lbxPermissionUsers.Items.Add(lbxAllUsers.Items[i]);
            }

            lbxAllUsers.Items.Clear();

            permissionAdd(items);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (!checkPMS())
            {
                return;
            }

            int[] arrayIndex = lbxAllUsers.GetSelectedIndices();

            if (arrayIndex.Length <= 0)
            {
                return;
            }

            List<ListItem> items = new List<ListItem>();
            for (int i = arrayIndex.Length - 1; i >= 0; i--)
            {
                items.Add(lbxAllUsers.Items[arrayIndex[i]]);

                lbxPermissionUsers.Items.Add(lbxAllUsers.Items[arrayIndex[i]]);
                lbxAllUsers.Items.Remove(lbxAllUsers.Items[arrayIndex[i]]);
            }

            permissionAdd(items);
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            if (!checkPMS())
            {
                return;
            }

            int[] arrayIndex = lbxPermissionUsers.GetSelectedIndices();

            if (arrayIndex.Length <= 0)
            {
                return;
            }

            List<ListItem> items = new List<ListItem>();

            for (int i = arrayIndex.Length - 1; i >= 0; i--)
            {
                items.Add(lbxPermissionUsers.Items[arrayIndex[i]]);

                lbxAllUsers.Items.Add(lbxPermissionUsers.Items[arrayIndex[i]]);
                lbxPermissionUsers.Items.Remove(lbxPermissionUsers.Items[arrayIndex[i]]);
            }
            permissionDelete(items);
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            if (!checkPMS())
            {
                return;
            }

            if (lbxPermissionUsers.Items.Count <= 0)
            {
                return;
            }

            List<ListItem> items = new List<ListItem>();

            for (int i = 0; i < lbxPermissionUsers.Items.Count; i++)
            {
                items.Add(lbxPermissionUsers.Items[i]);
                lbxAllUsers.Items.Add(lbxPermissionUsers.Items[i]);
            }

            lbxPermissionUsers.Items.Clear();

            permissionDelete(items);
        }

        protected void permissionAdd(List<ListItem> items)
        {
            foreach (ListItem item in items)
            {
                _domianPermissionProvider.Add(new DomainPermissionEntity()
                {
                    DomainID = domainID,
                    PermissionType = permissionID,
                    UserName = item.Text,
                    AddTime = DateTime.Now,
                    AddUserName = CurUserName,
                    UpdateTime = DateTime.Now,
                    UpdateUserName = CurUserName,
                    DeleteFlag = false
                });
            }
        }

        protected void permissionDelete(IList<ListItem> items)
        {
            foreach (ListItem item in items)
            {
                _domianPermissionProvider.Delete(new DomainPermissionEntity()
                {
                    DomainID = domainID,
                    PermissionType = permissionID,
                    UserName = item.Text,
                    UpdateTime = DateTime.Now,
                    UpdateUserName = CurUserName,
                    DeleteFlag = true
                });
            }
        }

        protected bool checkPMS()
        {
            if (int.Parse(ddlDomains.SelectedValue) > 0 && int.Parse(ddlPermissions.SelectedValue) > 0)
            {
                return true;
            }
            else
            {
                ShowAlert("请选择正确的域名和权限");
                return false;
            }
        }
    }
}