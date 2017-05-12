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
    public partial class AddRole : WebBasePage
    {
        private readonly LogProvider _logProvider = new LogProvider();

        /// <summary>
        ///     定义服务器业务逻辑类
        /// </summary>
        private readonly ServerProvider _serverProvider = new ServerProvider();

        private readonly RoleProvider _roleProvider = new RoleProvider();
        private readonly PermissionProvider _pmsProvider = new PermissionProvider();
        private int domainID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["DomainID"]))
            {
                domainID = Convert.ToInt32(Request.QueryString["DomainID"]);
                if (!HasDomainPermission(domainID, EnumManageType.DailyManage))
                {
                    ShowAlertAndRedirect("你没有该域名(ID:" + domainID + ")的管理权限！", "role.aspx");
                    return;
                }
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
            InitEnvironmentData();
            //ddlSource.DataSource = GetUserDomians(EnumManageType.DailyManage);
            //ddlSource.DataValueField = "DomainId";
            //ddlSource.DataTextField = "DomainName";
            //ddlSource.DataBind();
            //ddlSource.Items.Insert(0, new ListItem("请选择", "0"));

            if (domainID >= 1)
            {
                var dataSource = GetUserDomians(EnumManageType.DailyManage);
                var domain = dataSource.First(item => item.DomainId == domainID);
                if (DdlEnvironment.Items.FindByValue(domain.Environment.ToString()) != null)
                {
                    DdlEnvironment.SelectedIndex = -1;
                    DdlEnvironment.Items.FindByValue(domain.Environment.ToString()).Selected = true;
                }

                ddlSource.DataSource = (from item in dataSource where (item.Environment == domain.Environment) select item);

                ddlSource.DataValueField = "DomainId";
                ddlSource.DataTextField = "DomainName";
                ddlSource.DataBind();
                ddlSource.Items.Insert(0, new ListItem("请选择", "0"));
                ddlSource.Items.FindByValue(domainID.ToString()).Selected = true;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
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

                    if (!resultList.Contains(sel))
                    {
                        resultList.Add(sel);
                        ilist.Add(new PermissionEntity
                                      {
                                          Path = sel.Substring(nIndex),
                                          Type = sel.EndsWith("\\") ? 0 : 1
                                      });
                    }
                }

                //保存最大化文件，对于子目录和子文件不插入，只插入父级节点目录和文件
                IEnumerable<PermissionEntity> exceptFileList = _pmsProvider.GetExceptFile(ilist);

                foreach (PermissionEntity exceptFile in exceptFileList)
                {
                    ilist.Remove(exceptFile);
                }
            }
            //添加所有文件权限
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
                //定义操作日志
                var operateLogEntity = new OperationLogEntity
                                                          {
                                                              UserName = CurUserName,
                                                              DomainName = ddlSource.SelectedItem.Text,
                                                              OperateType = EnumOperateType.PermissionManage,
                                                              Log = string.Format("添加角色，角色名:{0}", txtName.Text.Trim())
                                                          };

                var dto = new RoleEntity { RoleName = txtName.Text.Trim(), DomainId = domainid };
                bool flag = _roleProvider.Add(dto, ilist);
                if (flag)
                {
                    operateLogEntity.Result = true;
                    ShowAlertAndRedirect("添加成功！", "Role.aspx");
                }
                else
                {
                    operateLogEntity.Result = false;
                    ShowAlert("添加失败！");
                }
                _logProvider.AddOperateLog(operateLogEntity);
            }
            else
            {
                ShowAlert("添加失败,请选择一个目录或文件！");
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