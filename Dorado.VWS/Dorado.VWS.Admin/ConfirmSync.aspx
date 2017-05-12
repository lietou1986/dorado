<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ConfirmSync.aspx.cs" Inherits="Dorado.VWS.Admin.ConfirmSync" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>确认同步</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: center">
        <table border="1" align="center" cellpadding="5" cellspacing="0">
            <tr>
                <td align="center" colspan="5">
                    <b>同步文件版本列表确认</b>
                </td>
            </tr>
            <asp:Repeater ID="rptVersionFile" runat="server">
                <HeaderTemplate>
                    <tr>
                        <td align="center">
                            版本号
                        </td>
                        <td align="center">
                            文件名
                        </td>
                        <td align="center">
                            版本创建人
                        </td>
                        <td align="center">
                            创建时间
                        </td>
                        <td align="center">
                            历史版本链接
                        </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td align="center">
                            <%#Eval("Version")%>
                        </td>
                        <td align="left">
                            <%#Eval("FilePath")%>
                        </td>
                        <td align="left">
                            <%#Eval("Creator")%>
                        </td>
                        <td align="left">
                            <%#Eval("CreateTime")%>
                        </td>
                        <td align="left">
                            <a href='<%#Eval("VersionPath")%>' id="LinkVersionFile" target="_blank" runat="server">
                                历史版本文件</a>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr>
                <td align="right">
                    任务信息描述
                </td>
                <td align="left" colspan="8">
                    <asp:TextBox ID="TxtLog" runat="server" Height="57px" TextMode="MultiLine" Width="269px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <asp:Button ID="BtnSync" runat="server" Text="确认同步" OnClick="BtnSync_Click" />
    </div>
</asp:Content>