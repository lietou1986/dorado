<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ScheduleTaskInfo.aspx.cs" Inherits="Dorado.VWS.Admin.ScheduleTaskInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>计划任务信息</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: center">
        <a id="LinkBefore" runat="server" href="#">上一条</a> <a id="LinkAfter" runat="server"
            href="#">下一条</a>
        <table border="0" align="center" cellpadding="5" cellspacing="0" style="width: 500px;"
            class="listTable">
            <tr>
                <th align="right">
                    域名
                </th>
                <td align="left">
                    <asp:Label ID="LblDomain" runat="server" />
                </td>
            </tr>
            <tr>
                <th align="right">
                    创建人
                </th>
                <td align="left">
                    <asp:Label ID="LblUserName" runat="server" />
                </td>
            </tr>
            <tr>
                <th align="right">
                    时间
                </th>
                <td align="left">
                    <asp:Label ID="LblScheduleTime" runat="server" />
                </td>
            </tr>
            <tr>
                <th align="right">
                    日志
                </th>
                <td align="left">
                    <asp:Label ID="LblDescription" runat="server" />
                </td>
            </tr>
            <tr>
                <th align="right">
                    添加文件列表
                </th>
                <td align="left">
                    <asp:Literal ID="LtlAddFiles" runat="server" />
                </td>
            </tr>
            <tr>
                <th align="right">
                    删除文件列表
                </th>
                <td align="left">
                    <asp:Literal ID="LtlDeleteFiles" runat="server" />
                </td>
            </tr>
        </table>
        <div id="tip" style="text-align: center; color: #FF0000;">
        </div>
    </div>
</asp:Content>