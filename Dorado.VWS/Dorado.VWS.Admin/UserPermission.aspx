<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="UserPermission.aspx.cs" Inherits="Dorado.VWS.Admin.UserPermission" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>角色成员管理</title>
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
    <%#ddlDomains.ClientID%>
    <script type="text/javascript">
        function CheckDomain(source, args) {
            //return ($('#<%=ddlDomains.ClientID%>').val() == 0);

            //CustomValidator的ID和ErrorMessage
            //alert(source.id);
            //alert(source.errormessage);
            //alert($('#<%=ddlDomains.ClientID%>').val() > 0)
            args.IsValid = ($('#<%=ddlDomains.ClientID%>').val() > 0);

        }
        function Check(source, args) {
            //alert($('#<%=ddlDomains.ClientID%>').val() > 0);
            //alert($('#<%=lbxAllUsers.ClientID%>').val());
            //alert($('#<%=ddlDomains.ClientID%>').val() > 0 && $('#<%=lbxAllUsers.ClientID%>').val() != null);
            //alert($('#<%=Permission1. ClientID%>').attr('checked'));

            args.IsValid = ($('#<%=ddlDomains.ClientID%>').val() > 0 && $('#<%=lbxAllUsers.ClientID%>').val() != null);
        }
        function CheckP(obj) {
            //alert($('#<%=ddlDomains.ClientID%>').val() > 0);
            //alert($('#<%=lbxAllUsers.ClientID%>').val());
            //alert($('#<%=ddlDomains.ClientID%>').val() > 0 && $('#<%=lbxAllUsers.ClientID%>').val() != null);
            if (!($('#<%=ddlDomains.ClientID%>').val() > 0 && $('#<%=lbxAllUsers.ClientID%>').val() != null)) {
                //console.log($(obj));
                if ($(obj).attr('checked') == "checked") {
                    $(obj).removeAttr('checked');
                } else {
                    $(obj).attr('checked', 'checked');
                }
                alert("请选择正确的域名和用户");
                return false;
            }
            return true;
        }
        function CheckP1() {
            //alert($('#<%=ddlDomains.ClientID%>').val() > 0);
            //alert($('#<%=lbxAllUsers.ClientID%>').val());
            //alert($('#<%=ddlDomains.ClientID%>').val() > 0 && $('#<%=lbxAllUsers.ClientID%>').val() != null);
            if (!($('#<%=ddlDomains.ClientID%>').val() > 0 && $('#<%=lbxAllUsers.ClientID%>').val() != null)) {

                if ($('#<%=Permission1. ClientID%>').attr('checked') == "checked") {
                    $('#<%=Permission1. ClientID%>').removeAttr('checked');
                } else {
                    $('#<%=Permission1. ClientID%>').attr('checked', 'checked');
                }
                alert("请选择正确的域名和用户");
                return false;
            }
            //alert($('#<%=Permission1. ClientID%>').attr('checked'));
            if ($('#<%=Permission1. ClientID%>').attr('checked') == "checked") {

                if (confirm('确认要授权日常管理(域管理员)权限')) {
                    return true;
                }
                else {

                    $('#<%=Permission1. ClientID%>').removeAttr('checked');
                    return false;
                }

            }
            else {
                return true;
            }

        }
        $.ready(function () {
            //         $('input:checkbox').click(
            //        function () {
            //            //alert($('#<%=ddlDomains.ClientID%>').val());
            //            //alert($('#<%=lbxAllUsers.ClientID%>').val());
            //            if ($('#<%=ddlDomains.ClientID%>').val() == 0) {
            //                alert("请选择正确的域名");
            //                return false;
            //            }
            //            if (!$('#<%=lbxAllUsers.ClientID%>').val()) {
            //                alert("请选择正确的用户")
            //                return false;
            //            }
            //        }
            //     );
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--<input type="checkbox" onmousedown />--%>
    <br />
    <table style="width: 504px">
        <tr>
            <td colspan="3">
                环境：<asp:DropDownList ID="DdlEnvironment" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlEnvironment_SelectedIndexChanged">
                </asp:DropDownList>
                域名:<asp:DropDownList ID="ddlDomains" runat="server" OnSelectedIndexChanged="ddlDomains_SelectedIndexChanged"
                    AutoPostBack="True">
                </asp:DropDownList>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="Button5" type="button" value="返回" onclick="javascript:window.location.href='ServerList.aspx'" />
            </td>
        </tr>
        <tr>
            <td class="style1" colspan="3">
                *用户应先分配至角色，然后再授予个权限！
            </td>
        </tr>
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
                    <asp:CheckBox ID="Permission1" Text="日常管理" runat="server" onclick="if(!CheckP1()){return;};"
                        OnCheckedChanged="PermissionCbx_CheckedChanged" AutoPostBack="True" value="1"
                        CausesValidation="False" ValidationGroup="1" CssClass="1" /><span style="color: Red;">&nbsp;
                            即域管理员，谨慎授权</span><br />
                    <%} %>
                    <asp:CheckBox ID="Permission2" Text="服务控制" runat="server" onclick="if(!CheckP(this)){return;};"
                        value="2" OnCheckedChanged="PermissionCbx_CheckedChanged" AutoPostBack="True"
                        CausesValidation="False" ValidationGroup="1" CssClass="2" /><br />
                    <asp:CheckBox ID="Permission3" Text="列表同步" runat="server" onclick="if(!CheckP(this)){return;};"
                        value="3" OnCheckedChanged="PermissionCbx_CheckedChanged" AutoPostBack="True"
                        CausesValidation="False" ValidationGroup="1" CssClass="3" /><br />
                    <asp:CheckBox ID="Permission4" Text="全部回滚" runat="server" onclick="if(!CheckP(this)){return;};"
                        value="4" OnCheckedChanged="PermissionCbx_CheckedChanged" AutoPostBack="True"
                        CausesValidation="False" ValidationGroup="1" CssClass="4" /><br />
                    <asp:CheckBox ID="Permission5" Text="文件删除" runat="server" onclick="if(!CheckP(this)){return;};"
                        value="4" OnCheckedChanged="PermissionCbx_CheckedChanged" AutoPostBack="True"
                        CausesValidation="False" ValidationGroup="1" CssClass="5" /><br />
                </div>
            </td>
        </tr>
    </table>
</asp:Content>