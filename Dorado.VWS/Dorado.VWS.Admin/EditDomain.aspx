<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EditDomain.aspx.cs" Inherits="Dorado.VWS.Admin.EditDomain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>添加域名</title>
    <script type="text/javascript">
        function check() {
            if ($("#<%=txtDomain.ClientID%>").val() == "") {
                alert("域名不能为空！");
                $("#<%=txtDomain.ClientID%>").focus();
                return false;
            }
            if ($("#<%=ddlEnvironment.ClientID%>").val() == "0") {
                alert("环境不能为空！");
                $("#<%=ddlEnvironment.ClientID%>").focus();
                return false;
            }
            return true;
        }
        function domainTypeChange() {
            var packageType = $('input[name="domainType"]:checked').val();
            if (packageType == '1') {
                $("#domainRoot").html("请输入Shell脚本路径：");
            }
            else {
                $("#domainRoot").html("请输入Tomcat根目录：");
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: center">
        <table border="0" align="center" cellpadding="0" cellspacing="5">
            <tr>
                <td align="center" colspan="2">
                    <b>修改域名</b>
                </td>
            </tr>
            <tr>
                <td align="right">
                    IDC名称：
                </td>
                <td align="left">
                    <asp:DropDownList ID="ddlIDC" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td align="right">
                    环境：
                </td>
                <td align="left">
                    <asp:DropDownList ID="ddlEnvironment" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr id="trdomain">
                <td align="right">
                    域名：
                </td>
                <td align="left">
                    <asp:TextBox ID="txtDomain" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    同步类型：
                </td>
                <td align="left">
                    <asp:DropDownList ID="ddlSyncType" runat="server">
                        <asp:ListItem Value="1">普通同步</asp:ListItem>
                        <asp:ListItem Value="2">简单同步</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td align="right">
                    html文件压缩：
                </td>
                <td align="left">
                    <asp:RadioButtonList ID="RadlHtml" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem>是</asp:ListItem>
                        <asp:ListItem Selected="True">否</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td align="right">
                    JsCss文件压缩：
                </td>
                <td align="left">
                    <asp:RadioButtonList ID="RadlJsCss" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem>是</asp:ListItem>
                        <asp:ListItem Selected="True">否</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td align="right">
                    WinService名称：
                </td>
                <td align="left">
                    <asp:TextBox ID="txtWinServerName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    IIS配置网站名称：
                </td>
                <td align="left">
                    <asp:TextBox ID="txtIISSiteName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    缓冲Url：
                </td>
                <td align="left">
                    <asp:TextBox ID="txtCacheUrl" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    域名类型：
                </td>
                <td align="left">
                    <asp:RadioButtonList ID="rdlDomainType" runat="server" RepeatDirection="Horizontal"
                        OnSelectedIndexChanged="rdlDomainType_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Selected="True">Windows</asp:ListItem>
                        <asp:ListItem>Linux</asp:ListItem>
                    </asp:RadioButtonList>
                    <%--<asp:RadioButtonList ID="rdlDomainType" runat="server"></asp:RadioButtonList>--%>
                    <%-- <input type="radio" value="1" id="windows" name="domainType" onclick="domainTypeChange()" />Windows
                    <input type="radio" value="2" id="linux" name="domainType" onclick="domainTypeChange()" />Linux--%>
                </td>
            </tr>
            <tr id="trPathType" runat="server" visible="false">
                <td align="right">
                    服务类型：
                </td>
                <td align="left">
                    <asp:RadioButtonList ID="rdlOperatePathType" runat="server" RepeatDirection="Horizontal"
                        OnSelectedIndexChanged="rdlOperatePathType_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Selected="True">Tomcat</asp:ListItem>
                        <asp:ListItem>Jar包</asp:ListItem>
                    </asp:RadioButtonList>
                    <%--<asp:RadioButtonList ID="rdlDomainType" runat="server"></asp:RadioButtonList>--%>
                    <%-- <input type="radio" value="1" id="windows" name="domainType" onclick="domainTypeChange()" />Windows
                    <input type="radio" value="2" id="linux" name="domainType" onclick="domainTypeChange()" />Linux--%>
                </td>
            </tr>
            <tr id="trPath" runat="server" visible="false">
                <td align="right">
                    <asp:Label ID="lblDomainRoot" runat="server">请输入Tomcat路径：</asp:Label>
                </td>
                <td align="left">
                    <asp:TextBox ID="txtdomainRoot" runat="server"></asp:TextBox>
                </td>
            </tr>
            <%--<tr>
                <td align="right">
                    域名类型：
                </td>
                <td align="left">
                    <input type="radio" value="1" id="windows" name="domainType" onclick="domainTypeChange()" />Windows
                    <input type="radio" value="2" id="linux" name="domainType" onclick="domainTypeChange()" />Linux
                    <asp:RadioButtonList ID="rblDomainType" runat="server"></asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <span id="domainRoot"></span>
                </td>
                <td align="left">
                    <asp:TextBox ID="txtdomainRoot" runat="server"></asp:TextBox>
                </td>
            </tr>--%>
            <tr>
                <td align="right" colspan="2">
                    <asp:Button ID="btnEdit" TabIndex="0" OnClientClick="return check();" runat="server"
                        Text="修 改" OnClick="btnEdit_Click" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>