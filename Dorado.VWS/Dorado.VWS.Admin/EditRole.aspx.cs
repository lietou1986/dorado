#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class EditRole : WebBasePage
    {
        private readonly LogProvider _logProvider = new LogProvider();

        /// <summary>
        ///     定义服务器业务逻辑类
        /// </summary>
        private readonly ServerProvider _serverProvider = new ServerProvider();

        private readonly PermissionProvider _pmsProvider = new PermissionProvider();
        private readonly RoleProvider _roleProvider = new RoleProvider();
        private int _id;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["ID"]))
            {
                _id = Convert.ToInt32(Request.QueryString["ID"]);
            }
            if (!IsPostBack)
            {
                InitSource();
            }
        }

        /// <summary>
        ///     初始化数据
        /// </summary>
        private void InitSource()
        {
            RoleEntity role = _roleProvider.GetRoleByID(_id);
            if (role == null)
            {
                ShowAlertAndRedirect("错误的角色！", "role.aspx");
                return;
            }
            if (!HasDomainPermission(role.DomainId, EnumManageType.DailyManage))
            {
                ShowAlertAndRedirect("你没有该域名(ID:" + role.DomainId + ")的管理权限！", "role.aspx");
                return;
            }
            InitEnvironmentData();
            var dataSource = GetUserDomians(EnumManageType.DailyManage);
            var domain = dataSource.First(item => item.DomainId == role.DomainId);
            if (DdlEnvironment.Items.FindByValue(domain.Environment.ToString()) != null)
            {
                DdlEnvironment.SelectedIndex = -1;
                DdlEnvironment.Items.FindByValue(domain.Environment.ToString()).Selected = true;
            }

            ddlSource.DataSource = (from item in dataSource where (item.Environment == domain.Environment) select item);

            //ddlSource.DataSource = GetUserDomians(EnumManageType.DailyManage);
            ddlSource.DataValueField = "DomainId";
            ddlSource.DataTextField = "DomainName";
            ddlSource.DataBind();

            ddlSource.SelectedValue = role.DomainId.ToString();

            txtName.Text = role.RoleName;
            ChkAll.Checked = false;

            //获取角色权限
            IList<PermissionEntity> permissionList = _pmsProvider.GetPermissionByRoleId(_id);
            //判断是否拥有所有文件权限
            foreach (PermissionEntity permissionEntity in permissionList)
            {
                if (permissionEntity.Path.Trim() == string.Empty)
                {
                    ChkAll.Checked = true;
                    break;
                }
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            IList<PermissionEntity> ilist = new List<PermissionEntity>();
            IList<string> resultList = new List<string>();
            //TreeNodeCollection tnc = TreeView1.CheckedNodes;
            int domainid = Convert.ToInt32(ddlSource.SelectedValue);

            if (!HasDomainPermission(domainid, EnumManageType.DailyManage))
            {
                ShowAlert("你没有该域名(ID:" + domainid + ")的管理权限！");
                return;
            }

            RoleEntity roleDTO = _roleProvider.GetRoleByID(_id);

            //定义操作日志
            var operateLogEntity = new OperationLogEntity
                                                      {
                                                          UserName = CurUserName,
                                                          DomainName = ddlSource.SelectedItem.Text,
                                                          OperateType = EnumOperateType.PermissionManage,
                                                          Log =
                                                              string.Format("修改角色，修改前角色名:{0}，修改后角色名:{1}",
                                                                            roleDTO.RoleName, txtName.Text.Trim())
                                                      };

            roleDTO.RoleName = txtName.Text.Trim();

            if (!ChkAll.Checked)
            {
                string root = _serverProvider.GetSourceServerByDomainId(domainid).Root;
                int nIndex = root.Length;
                string[] selStrs = Regex.Split(HfChecks.Value, "###");

                foreach (string sel in selStrs)
                {
                    if (string.IsNullOrEmpty(sel.Trim()))
                    {
                        continue;
                    }

                    if (resultList.Contains(sel)) continue;

                    resultList.Add(sel);
                    ilist.Add(new PermissionEntity
                                  {
                                      Path = sel.Substring(nIndex),
                                      Type = sel.EndsWith("\\") ? 0 : 1
                                  });
                }

                //保存最大化文件，对于子目录和子文件不插入，只插入父级节点目录和文件
                IEnumerable<PermissionEntity> exceptFileList = _pmsProvider.GetExceptFile(ilist);

                foreach (PermissionEntity exceptFile in exceptFileList)
                {
                    ilist.Remove(exceptFile);
                }
            }
            else
            {
                ilist.Add(new PermissionEntity
                              {
                                  Path = string.Empty,
                                  Type = 0
                              });
            }

            if (ilist.Count > 0)
            {
                bool flag = _roleProvider.Edit(roleDTO, ilist);
                if (flag)
                {
                    operateLogEntity.Result = true;
                    ShowAlertAndRedirect("修改成功！", "Role.aspx");
                }
                else
                {
                    operateLogEntity.Result = false;
                    ShowAlert("修改失败！");
                }
                _logProvider.AddOperateLog(operateLogEntity);
            }
            else
            {
                ShowAlert("请选择一个目录或文件，修改失败！");
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
            ddlSource.DataSource = GetUserDomians(EnumManageType.DailyManage, DdlEnvironment.SelectedValue);

            ddlSource.DataValueField = "DomainId";
            ddlSource.DataTextField = "DomainName";
            ddlSource.DataBind();
            ddlSource.Items.Insert(0, new ListItem("请选择", "0"));
        }
    }
}