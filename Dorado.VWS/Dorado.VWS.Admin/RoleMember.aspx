<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="RoleMember.aspx.cs" Inherits="Dorado.VWS.Admin.RoleMember" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>角色成员管理</title>
    <meta http-equiv="pragma " content="no-cache " />
    <meta http-equiv="Cache-Control " content="no-cache,   must-revalidate " />
    <style type="text/css">
        .style1
        {
            width: 137px;
        }
        .style2
        {
            width: 205px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="padding: 10px;">
        注意 权限操作立即生效，请谨慎使用！<input id="Button5" type="button" value="返回" onclick="javascript:window.location.href='role.aspx'" /><br />
        如果用户不在“可授权用户”也不在“已授权用户”列表，需要先向PMS系统申请对该用户授权，可以联系【<a href="mailto:len@dorado.com">len</a>
        len@dorado.com】申请授权
        <table style="width: 90%; border: 0px #aeaecc solid; margin-top: 5px;">
            <tr>
                <td colspan="5">
                    环境：<asp:DropDownList ID="DdlEnvironment" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlEnvironment_SelectedIndexChanged">
                    </asp:DropDownList>
                    域名:<asp:DropDownList ID="ddlDomains" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlDomains_SelectedIndexChanged">
                    </asp:DropDownList>
                    角色：<asp:DropDownList ID="ddlRoles" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlRoles_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="width: 200px;">
                    可授权用户
                </td>
                <td style="width: 50px;">
                </td>
                <td style="width: 200px;">
                    已授权用户
                </td>
                <td style="width: 20px;">
                </td>
                <td style="width: auto;">
                    角色信息<asp:Literal ID="ltGotoRoleDetail" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:ListBox ID="lbxAllUsers" runat="server" Width="200px" Height="300px" SelectionMode="Multiple">
                    </asp:ListBox>
                </td>
                <td style="text-align: center;">
                    <asp:Button ID="Button1" runat="server" Text=">>" Visible="false" />
                    <br />
                    <br />
                    <asp:Button ID="Button2" runat="server" Text=">" OnClick="Button2_Click" />
                    <br />
                    <br />
                    <asp:Button ID="Button3" runat="server" Text="<" OnClick="Button3_Click" />
                    <br />
                    <br />
                    <asp:Button ID="Button4" runat="server" Text="<<" Visible="false" />
                </td>
                <td class="style2">
                    <asp:ListBox ID="lbxRoleUsers" runat="server" Height="300px" Width="200px" SelectionMode="Multiple">
                    </asp:ListBox>
                </td>
                <td>
                </td>
                <td style="vertical-align: top; border: 1px solid #aeaecd;">
                    <asp:Literal ID="ltRoleInfo" runat="server"></asp:Literal>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>