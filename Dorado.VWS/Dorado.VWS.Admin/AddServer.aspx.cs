#region using

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;

#endregion using

namespace Dorado.VWS.Admin
{
    public partial class AddServer : WebBasePage
    {
        private readonly LogProvider _logProvider = new LogProvider();

        /// <summary>
        ///     定义服务器业务逻辑类
        /// </summary>
        private readonly ServerProvider _serverProvider = new ServerProvider();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitIdcData();

                if (!string.IsNullOrWhiteSpace(Request.QueryString["IDCID"]))
                {
                    ddlIDC.SelectedValue = Request.QueryString["IDCID"];
                }

                InitEnvironmentData();

                if (!string.IsNullOrWhiteSpace(Request.QueryString["Environment"]))
                {
                    ddlEnvironment.SelectedValue = Request.QueryString["Environment"];
                }

                InitDomainData();

                if (!string.IsNullOrWhiteSpace(Request.QueryString["DomainID"]))
                {
                    ddlDomain.SelectedValue = Request.QueryString["DomainID"];
                }
            }
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
        }

        /// <summary>
        ///     初始化域名数据
        /// </summary>
        private void InitDomainData()
        {
            ddlDomain.DataSource = GetUserDomians(EnumManageType.DailyManage, ddlEnvironment.SelectedValue.ToString());
            ddlDomain.DataValueField = "DomainId";
            ddlDomain.DataTextField = "DomainName";
            ddlDomain.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            var serverEntity = new ServerEntity
                                   {
                                       IdcId = Convert.ToInt32(ddlIDC.SelectedValue),
                                       DomainId = Convert.ToInt32(ddlDomain.SelectedValue),
                                       ServerType = (EnumServerType)Convert.ToInt32(ddlServerType.SelectedValue),
                                       IP = txtIP.Text.Trim(),
                                       Root = txtRoot.Text.Trim(),
                                       IISStatus = EnumIISStatus.Running,
                                       Creator = CurUserName,
                                       IsAdvanced = chkIsAdvanced.Checked
                                   };

            switch (int.Parse(ddlServerType.SelectedValue))
            {
                case (int)EnumServerType.Host:
                    if (!_serverProvider.ExistsSource(serverEntity))
                    {
                        ShowAlert("必须首先添加一个同步源，才能添加同步宿。添加失败！");
                        return;
                    }
                    if (_serverProvider.ExistsHostCommonIp(serverEntity))
                    {
                        ShowAlert("同一域名下不能出现多台宿主一个IP，添加失败！");
                        return;
                    }
                    break;

                case (int)EnumServerType.Relay:
                    if (_serverProvider.ExistsRelay(serverEntity))
                    {
                        ShowAlert("一个IDC下同域名的同步中继只能添加一个，添加失败！");
                        return;
                    }
                    break;

                case (int)EnumServerType.Source:
                    if (_serverProvider.ExistsSource(serverEntity))
                    {
                        ShowAlert("一个域名只能有一个同步源，添加失败！");
                        return;
                    }
                    break;
            }

            //定义操作日志
            var operateLogEntity = new OperationLogEntity
                                                      {
                                                          UserName = CurUserName,
                                                          DomainName = ddlDomain.SelectedItem.Text,
                                                          OperateType = EnumOperateType.ServerManage,
                                                          Log =
                                                              string.Format("添加服务器，服务器类型:{0}，IP:{1}",
                                                                            serverEntity.ServerTypeName, serverEntity.IP),
                                                      };

            if (_serverProvider.InsertServer(serverEntity))
            {
                operateLogEntity.Result = true;
                ShowAlertAndRedirect("添加成功！", "ServerList.aspx?id=" + ddlDomain.SelectedValue);
            }
            else
            {
                operateLogEntity.Result = false;
                lTip.Text = "添加失败！";
            }
            _logProvider.AddOperateLog(operateLogEntity);
        }

        protected void ddlIDC_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitDomainData();
        }

        #region 针对导入操作的相关方法

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImport_Click(object sender, EventArgs e)
        {
            int idcID = 0;
            try
            {
                idcID = Convert.ToInt32(Request["IDCID"]);
            }
            catch
            {
                lbInfo.Text = "无法从URL中获取IDC机房ID，无法导入！";
                return;
            }
            int domainID = 0;
            try
            {
                domainID = Convert.ToInt32(Request["DomainID"]);
            }
            catch
            {
                lbInfo.Text = "无法从URL中获取域名ID，无法导入！";
                return;
            }

            if (!fuImport.HasFile)
            {
                lbInfo.Text = "未选择文件！";
                return;
            }

            String fileExtension = Path.GetExtension(fuImport.FileName).ToLower();
            if (!fileExtension.EndsWith(".txt"))
            {
                lbInfo.Text = "只能上传类型为.txt的文件！";
                return;
            }

            Regex regexIP = new Regex(@"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
            Regex regexRoot = new Regex(@"^[a-zA-Z]\:[\\](([^\\])?[\w\.\-]+\\)*$");

            int lineIndex = 0;
            int totalCount = 0;
            int succCount = 0;
            string[] txtAllLine = Encoding.UTF8.GetString(fuImport.FileBytes).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sbInfo = new StringBuilder();
            foreach (string line in txtAllLine)
            {
                lineIndex++;
                if (string.IsNullOrEmpty(line) || line.Length < 5)
                {
                    continue;
                }
                if (line.StartsWith("#") || line.Substring(0, 2).Contains("#"))
                {
                    continue;
                }

                totalCount++;

                string[] fields = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (fields.Length < 2 || fields.Length > 3)
                {
                    sbInfo.Append("第" + lineIndex + "行[" + line + "]字段数量不正确</br>");
                    continue;
                }

                EnumServerType serverType = EnumServerType.Relay;
                string ip = fields[1];
                string root = string.Empty;

                if (!regexIP.Match(ip).Success)
                {
                    sbInfo.Append("第" + lineIndex + "行[" + line + "]IP地址不正确</br>");
                    continue;
                }

                switch (fields[0])
                {
                    case "1":
                        serverType = EnumServerType.Host;
                        if (fields.Length != 3)
                        {
                            sbInfo.Append("第" + lineIndex + "行[" + line + "]字段数量与服务器类型不符</br>");
                            continue;
                        }
                        root = fields[2];
                        break;

                    case "2":
                        if (fields.Length != 2)
                        {
                            sbInfo.Append("第" + lineIndex + "行[" + line + "]字段数量与服务器类型不符</br>");
                            continue;
                        }
                        break;

                    case "3":
                        serverType = EnumServerType.Source;
                        if (fields.Length != 3)
                        {
                            sbInfo.Append("第" + lineIndex + "行[" + line + "]字段数量与服务器类型不符</br>");
                            continue;
                        }
                        root = fields[2];
                        break;

                    default:
                        sbInfo.Append("第" + lineIndex + "行[" + line + "]服务器类型不正确</br>");
                        continue;
                }

                if (!string.IsNullOrEmpty(root) && !regexRoot.Match(root).Success)
                {
                    sbInfo.Append("第" + lineIndex + "行[" + line + "]根目录不正确</br>");
                    continue;
                }

                //准备添加
                var serverEntity = new ServerEntity
                {
                    IdcId = idcID,
                    DomainId = domainID,
                    ServerType = serverType,
                    IP = fields[1],
                    Root = root,
                    IISStatus = EnumIISStatus.Running,
                    Creator = CurUserName
                };

                string info = "第" + lineIndex + "行,IP[" + serverEntity.IP + "]  ";
                switch (serverType)
                {
                    case EnumServerType.Host:
                        if (!_serverProvider.ExistsSource(serverEntity))
                        {
                            lbInfo.Text = info + "必须首先添加一个同步源，才能添加同步宿。添加失败！已经成功添加" + succCount + "行";
                            return;
                        }
                        if (_serverProvider.ExistsHostCommonIp(serverEntity))
                        {
                            //lbInfo.Text = info + "同一域名下不能出现多台宿主一个IP，添加失败！已经成功添加" + succCount + "行";
                            //return;
                            sbInfo.Append(info + "]同步宿IP重复</br>");
                            continue;
                        }
                        break;

                    case EnumServerType.Relay:
                        if (_serverProvider.ExistsRelay(serverEntity))
                        {
                            //lbInfo.Text = info + "一个IDC下同域名的同步中继只能添加一个，添加失败！已经成功添加" + succCount + "行";
                            //return;
                            sbInfo.Append(info + "]一个域名只能有一个同步中继</br>");
                            continue;
                        }
                        break;

                    case EnumServerType.Source:
                        if (_serverProvider.ExistsSource(serverEntity))
                        {
                            //lbInfo.Text = info + "一个域名只能有一个同步源，添加失败！已经成功添加" + succCount + "行";
                            //return;
                            sbInfo.Append(info + "]一个域名只能有一个同步源</br>");
                            continue;
                        }
                        break;
                }
                string ddldomain = ddlDomain.SelectedItem == null ? "" : ddlDomain.SelectedItem.Text;
                //定义操作日志
                var operateLogEntity = new OperationLogEntity
                {
                    UserName = CurUserName,
                    DomainName = ddldomain,
                    OperateType = EnumOperateType.ServerManage,
                    Log = string.Format("添加服务器，服务器类型:{0}，IP:{1}", serverEntity.ServerTypeName, serverEntity.IP),
                };

                if (_serverProvider.InsertServer(serverEntity))
                {
                    succCount++;
                }
                else
                {
                    lbInfo.Text = info + "添加失败！";
                    continue;
                }
            }

            if (succCount == 0)
            {
                lbInfo.Text = "无任何信息上传成功</br>" + sbInfo.ToString();
            }
            else if (succCount == totalCount)
            {
                lbInfo.Text = "成功上传" + succCount + "个服务器信息";
            }
            else
            {
                lbInfo.Text = "部分失败成功" + succCount + "个，失败" + (totalCount - succCount) + "个</br>" + sbInfo.ToString();
            }

            //lbInfo.Text += "</br>-------原文件信息---------</br>" + Encoding.UTF8.GetString(fuImport.FileBytes).Replace("\r\n", "</br>");
        }

        /// <summary>
        /// 导出样本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportDemo_Click(object sender, EventArgs e)
        {
            string fileName = "服务器上传样本.txt";
            string content = @"#文本编码保证为UTF-8格式
#符号“#”表示注释,忽略该行
#文本文件包含2到3个字段,字段间以空格或制表符分隔,分别为:
#服务器类型	IP	根目录
#服务器类型为数字(1代表同步宿  2代表同步中继 3代表同步源  如果有同步源,必须在第一行),当类型为同步中继时,根目录字段为空, 如:
3	10.21.0.100	E:\Web\
1	10.21.0.101	E:\Web\
2	10.21.0.102";
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            Response.Clear();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
            Response.AppendHeader("Content-Length", buffer.Length.ToString());
            Response.BinaryWrite(buffer);
            Response.Flush();
            Response.End();
        }

        #endregion 针对导入操作的相关方法

        protected void ddlEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitDomainData();
        }

        private void InitEnvironmentData()
        {
            ddlEnvironment.Items.Insert(0, new ListItem("请选择", EnvironmentType.UnSelected));
            //ddlEnvironment.Items.Insert(1, new ListItem("开发", EnvironmentType.Development));
            //ddlEnvironment.Items.Insert(2, new ListItem("测试", EnvironmentType.Testing));
            ddlEnvironment.Items.Insert(1, new ListItem("验收", EnvironmentType.Acceptance));
            //ddlEnvironment.Items.Insert(4, new ListItem("预上线", EnvironmentType.Advanced));
            ddlEnvironment.Items.Insert(2, new ListItem("线上", EnvironmentType.Production));
        }
    }
}