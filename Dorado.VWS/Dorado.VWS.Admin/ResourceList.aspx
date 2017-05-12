<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ResourceList.aspx.cs" Inherits="Dorado.VWS.Admin.ResourceList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>权限资源维护</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: center">
        <table border="1" align="center" cellpadding="5" cellspacing="0" style="width: 50%;"
            class="listTable">
            <tr>
                <td align="center" colspan="3" class="tableTitle">
                    <b>权限资源列表</b>
                </td>
            </tr>
            <tr>
                <td align="right" colspan="3">
                    <a href="AddResource.aspx">添加资源</a>
                </td>
            </tr>
            <asp:Repeater ID="rptResource" runat="server" OnItemCommand="rptResource_ItemCommand">
                <HeaderTemplate>
                    <tr>
                        <th align="center">
                            资源值
                        </th>
                        <th align="center">
                            资源描述
                        </th>
                        <th align="center">
                            操作
                        </th>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td align="left">
                            <%#Eval("ResourceValue")%>
                            &nbsp;
                        </td>
                        <td align="left">
                            <%#Eval("ResourceDescription")%>
                            &nbsp;
                        </td>
                        <td align="center">
                            <a href='EditResource.aspx?id=<%#Eval("ResourceId")%>'>修改</a> &nbsp;
                            <asp:LinkButton ID="lbtnDel" CommandName="del" CommandArgument='<%#Eval("ResourceId")%>'
                                runat="server" OnClientClick="return confirm('确定要删除吗？')">删除</asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
        </table>
    </div>
</asp:Content>