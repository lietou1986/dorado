<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="domainpermission.aspx.cs" Inherits="Dorado.VWS.Admin.domainpermission" %>

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
    <%#ddlDomains.ClientID%>
    <script type="text/javascript">
        $(

 function () {
     $('#MoveUser >input').click(
        function () {
            if ($('#<%=ddlDomains.ClientID%>').val() == 0 || $('#<%=ddlPermissions.ClientID%>').val() == 0) {
                alert("请选择正确的域名和权限")
                return false;
            }
        }
     );
 }
 )
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <table style="width: 504px">
        <tr>
            <td colspan="3">
                域名:<asp:DropDownList ID="ddlDomains" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlDomains_SelectedIndexChanged">
                </asp:DropDownList>
                权限：<asp:DropDownList ID="ddlPermissions" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPermissions_SelectedIndexChanged">
                </asp:DropDownList>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="Button5" type="button" value="返回" onclick="javascript:window.location.href='DomainList.aspx'" />
            </td>
        </tr>
        <tr>
            <td class="style1">
                <p>
                    可授权用户：</p>
                <asp:ListBox ID="lbxAllUsers" runat="server" Width="200px" Height="350px" SelectionMode="Multiple">
                </asp:ListBox>
            </td>
            <td style="text-align: center;" id="MoveUser">
                <asp:Button ID="Button1" runat="server" Text=">>" ToolTip="添加所有" OnClick="Button1_Click" />
                <br />
                <br />
                <asp:Button ID="Button2" runat="server" Text=">" ToolTip="添加选中" OnClick="Button2_Click" />
                <br />
                <br />
                <asp:Button ID="Button3" runat="server" Text="<" ToolTip="删除选中" OnClick="Button3_Click" />
                <br />
                <br />
                <asp:Button ID="Button4" runat="server" Text="<<" ToolTip="删除所有" OnClick="Button4_Click" />
            </td>
            <td class="style2">
                <p>
                    已授权用户：</p>
                <asp:ListBox ID="lbxPermissionUsers" runat="server" Height="350px" Width="200px"
                    SelectionMode="Multiple"></asp:ListBox>
            </td>
        </tr>
    </table>
</asp:Content>