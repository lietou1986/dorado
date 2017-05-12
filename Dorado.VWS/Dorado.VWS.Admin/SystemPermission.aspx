<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SystemPermission.aspx.cs" Inherits="Dorado.VWS.Admin.SystemPermission" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>系统角色成员管理</title>
    <meta http-equiv="pragma " content="no-cache " />
    <meta http-equiv="Cache-Control " content="no-cache,   must-revalidate " />
    <style type="text/css">
        .style1
        {
            vertical-align: top;
        }

        .style2
        {
            width: 205px;
            vertical-align: top;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--<input type="checkbox" onmousedown />--%>
    <br />
    <table style="width: 504px">
        <tr>
            <td class="style1">
                <p>
                    可授权用户：</p>
                <asp:ListBox ID="lbxAllUsers" runat="server" Width="200px" Height="350px" SelectionMode="Single"
                    AutoPostBack="True" OnSelectedIndexChanged="lbxAllUsers_SelectedIndexChanged"
                    ValidationGroup="2"></asp:ListBox>
            </td>
            <td style="text-align: center;" id="MoveUser">
                &nbsp;
            </td>
            <td class="style2">
                <p>
                    权限：</p>
                <div id="PermissionContainter" runat="server">
                    <%if (CurUserIsAdmin)
                      { %>
                    <asp:CheckBox ID="Permission1" Text="超级管理员" runat="server" OnCheckedChanged="PermissionCbx_CheckedChanged"
                        AutoPostBack="True" value="1" CausesValidation="False" ValidationGroup="1" CssClass="1" /><span
                            style="color: Red;">&nbsp; 谨慎授权</span><br />
                    <%} %>
                    <asp:CheckBox ID="Permission2" Text="开发" runat="server" value="2" OnCheckedChanged="PermissionCbx_CheckedChanged"
                        AutoPostBack="True" CausesValidation="False" ValidationGroup="1" CssClass="2" /><br />
                    <asp:CheckBox ID="Permission3" Text="测试" runat="server" value="3" OnCheckedChanged="PermissionCbx_CheckedChanged"
                        AutoPostBack="True" CausesValidation="False" ValidationGroup="1" CssClass="3" /><br />
                    <asp:CheckBox ID="Permission4" Text="运维" runat="server" value="4" OnCheckedChanged="PermissionCbx_CheckedChanged"
                        AutoPostBack="True" CausesValidation="False" ValidationGroup="1" CssClass="4" /><br />
                </div>
            </td>
        </tr>
    </table>
</asp:Content>