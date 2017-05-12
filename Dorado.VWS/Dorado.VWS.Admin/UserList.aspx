<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserList.aspx.cs" Inherits="Vancl.IC.VWS.SiteApp.UserList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>用户列表</title>

    <script type="text/javascript">
        function allchk() {
            if ($("#chkall").attr("checked") == true) {
                $("input[type='checkbox']").each(function() {
                    if ($(this).parent().parent().parent().parent().attr('class') != 'chkResourceListCss') {
                        $(this).attr("checked", true);
                    }
                });
            } else {
                $("input[type='checkbox']").each(function() {
                    if ($(this).parent().parent().parent().parent().attr('class') != 'chkResourceListCss') {
                        $(this).attr("checked", false);
                    }
                });
            }
        }

        function canclcheck(id) {
            if ($("#" + id).attr("checked") == true) {
                var flag = true;
                $("input[type='checkbox'][id!='chkall']").each(function() {
                    if ($(this).attr("checked") == false) {
                        flag = false;
                        return;
                    }
                });
                if (flag) {
                    $("#chkall").attr("checked", true);
                }
            } else {
                $("#chkall").attr("checked", false);
            }
        }


        function showhiddenResource() {
            if ($('.resourceShowHidden').css("display") == "block") {
                $('.resourceShowHidden').css("display", "none");
            } else {
                $('.resourceShowHidden').css("display", "block");
            }
        }


        function validate() {
            var userName = $.trim($('.userNameCss').val());
            if (userName == "") {
                alert("请输入用户名！");
                return false;
            } else {
                return true;
            }

        }


        $(
            function() {
                $('.rolelist').change(
                    function() {
                        if ($(this).prev().prev().attr('Value') != $(this).val()) {

                            $(this).parent().prev().prev().find(':checkbox').attr('checked', true);

                        } else {
                            $(this).parent().prev().prev().find(':checkbox').attr('checked', false);
                        }
                    }
                );
            }
        );        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: center">
        <table align="center" cellpadding="5" cellspacing="0">
            <tr>
                <td valign="top">
                    <div align="left">
                        <table>
                            <tr>
                                <td> 
                                    <asp:Label ID="lblUserName" runat="server" Text="用户名"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUserName" runat="server" CssClass="userNameCss"></asp:TextBox>
                               
                                </td>
                                <td>
                                    <asp:Button ID="btnSeek" runat="server" Text="查询" OnClientClick="return validate()" onclick="btnSeek_Click" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div align="left">
                        <table border="1" align="left" cellpadding="5" cellspacing="0" width="230px"  class="listTable">
                            <tr>
                                <th align="center" colspan="2">
                                    <b>权限维护</b>
                                </th>
                            </tr>
                            <tr>
                                <th align="center">
                                    用户名
                                </th>
                                <th align="center">
                                    操作
                                </th>
                            </tr>
                            <asp:Literal ID="ltUserlist" runat="server"></asp:Literal>
                        </table>
                    </div> 
                </td>
                <td valign="top" width="600px">
                    <table id="tabQuan" runat="server" visible="false" border="0" align="center" cellpadding="5"
                           cellspacing="0">
                        <tr>
                            <td align="left">
                                用户：<asp:Literal ID="ltUser" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <td align="left">
                                <input id="btnShow" type="button" value="显示/隐藏—资源权限"  onclick=" showhiddenResource(); "/>
                            </td>
                        </tr>
                        <tr>
                            <td align="left" valign="top">
                                <asp:Repeater ID="rptRole" OnItemCommand="rptRole_ItemCommand" OnItemDataBound="rptRole_ItemDataBound"
                                              runat="server">
                                    <HeaderTemplate>
                                        <table border="1" align="center" cellpadding="5" cellspacing="0">
                                        <tr>
                                            <td align="center">
                                                域名
                                            </td>
                                            <td align="center">
                                                角色
                                            </td>
                                            <td align="center" class="resourceShowHidden" style="display: none">
                                                资源权限
                                            </td>                                                
                                            <td align="center">
                                                操作
                                            </td>
                                        </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td align="center">
                                                <%#Eval("DomainName")%>
                                                <asp:HiddenField ID="hdfUserID" runat="server"  />
                                            </td>
                                            <td align="left">
                                                <asp:HiddenField ID="hdfDomainID" runat="server" Value='<%#Eval("DomainID")%>' />
                                                <asp:HiddenField ID="hdfUser" runat="server" />
                                                <asp:HiddenField ID="hdfRoleID" runat="server" />
                                                <asp:HiddenField ID="hdfDomain" runat="server" Value='<%#Eval("DomainName")%>' />
                                                <asp:DropDownList ID="ddlRole" runat="server" CssClass="rolelist" Visible="false">
                                                </asp:DropDownList>
                                                <asp:CheckBoxList ID="chkRoleList" runat="server" RepeatDirection="Vertical" CssClass="chkResourceListCss">
                                                </asp:CheckBoxList>
                                            </td>
                                            <td align="left" class="resourceShowHidden" style="display: none">
                                                <asp:CheckBoxList ID="chkResourceList" runat="server" RepeatDirection="Vertical" CssClass="chkResourceListCss">
                                                </asp:CheckBoxList>
                                            </td>
                                            <td align="center">
                                                <asp:LinkButton ID="lbtndelIDC" CommandName="edit" CommandArgument=''
                                                                runat="server">修 改</asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                    </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" colspan="2">
                                <asp:Button ID="btnEdit" TabIndex="0" UseSubmitBehavior="false" runat="server" Text="修 改"
                                            OnClick="btnEdit_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>