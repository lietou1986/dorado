<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Role.aspx.cs" Inherits="Dorado.VWS.Admin.Role" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>角色维护</title>
    <script type="text/javascript">
        $(document).ready(function () {
            var data = '<%=JSON%>';
            data = eval("(" + data + ")");
            for (var i = 0; i < data.length; i++) {
                $(".idc[title=" + data[i].Id + "]").each(function (index) {
                    if (data[i].Count == 1) {

                    } else if (data[i].Count > 1 && index == 0) {
                        $(this).attr("rowspan", data[i].Count);
                        $(this).parent().children(".a" + data[i].Id + "").attr("rowspan", data[i].Count).attr("class", "");
                    } else {
                        $(this).remove();
                        $(".a" + data[i].Id + "").remove();
                    }
                });
            }

            for (var i = 0; i < data.length; i++) {
                $(".idc1[title=" + data[i].Id + "]").each(function (index) {
                    if (data[i].Count == 1) {

                    } else if (data[i].Count > 1 && index == 0) {
                        $(this).attr("rowspan", data[i].Count);
                        $(this).parent().children(".a" + data[i].Id + "").attr("rowspan", data[i].Count).attr("class", "");
                    } else {
                        $(this).remove();
                        $(".a" + data[i].Id + "").remove();
                    }
                });
            }

            $("[title=b0]>.c").remove();
            $(".v0").html("&nbsp;");
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: center">
        <table border="1" align="center" cellpadding="5" cellspacing="0" style="width: 50%;"
            class="listTable">
            <tr>
                <td align="center" colspan="5" class="tableTitle">
                    <b>角色列表</b>
                </td>
            </tr>
            <tr>
                <td align="left" colspan="5">
                    环境：<asp:DropDownList ID="DdlEnvironment" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlEnvironment_SelectedIndexChanged">
                    </asp:DropDownList>
                    域名：<asp:DropDownList ID="ddlAll" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlAll_SelectedIndexChanged">
                    </asp:DropDownList>
                    <asp:HyperLink ID="lnkAddRole" runat="server">添加角色</asp:HyperLink>
                </td>
            </tr>
            <asp:Repeater ID="rptRole" OnItemCommand="rptRole_ItemCommand" runat="server">
                <HeaderTemplate>
                    <tr>
                        <th align="center">
                            环境
                        </th>
                        <th align="center">
                            域名
                        </th>
                        <th align="center">
                            角色名称
                        </th>
                        <th align="center">
                            操作
                        </th>
                        <th align="center">
                            角色操作
                        </th>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="idc1" title='<%#Eval("DomainId")%>' align="left">
                            <%#Eval("Environment")%>
                            &nbsp;
                        </td>
                        <td class="idc" title='<%#Eval("DomainId")%>' align="left">
                            <%#Eval("Domain")%>
                            &nbsp;
                        </td>
                        <td align="left">
                            <%#Eval("RoleName")%>
                            &nbsp;
                        </td>
                        <td align="center" class='v<%#Eval("ID")%>'>
                            <a href='EditRole.aspx?id=<%#Eval("ID")%>'>修改</a> &nbsp; <a href='RoleMember.aspx?roleId=<%#Eval("ID")%>'>
                                成员管理</a>
                            <asp:LinkButton ID="lbtnDel" CommandName="del" CommandArgument='<%#Eval("ID")%>'
                                runat="server" OnClientClick="return confirm('确定要删除吗？')">删除</asp:LinkButton>
                            <asp:HiddenField ID="hdfDomainName" Value='<%#Eval("Domain")%>' runat="server" />
                            <asp:HiddenField ID="hdfRoleName" Value='<%#Eval("RoleName")%>' runat="server" />
                        </td>
                        <td align="center" title='b<%#Eval("ID")%>' class='a<%#Eval("DomainId")%>'>
                            <a href='AddRole.aspx?DomainID=<%#Eval("DomainId")%>'>添加</a>
                            <asp:LinkButton CssClass="c" ID="lbtndelIDC" CommandName="delDomainRoles" CommandArgument='<%#Eval("DomainId")%>'
                                runat="server" OnClientClick="return confirm('此删除要删除该域名下的所有角色和用户吗，确定要删除吗？')">删除</asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
        </table>
    </div>
</asp:Content>