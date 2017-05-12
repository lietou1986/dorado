<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageSql.aspx.cs" Inherits="Dorado.VWS.Admin.ManageSql" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Web页执行SQL</title>
    <style type="text/css">
        *
        {
            padding: 0;
            margin: 0;
        }

        body
        {
            font-size: 12px;
            color: #333;
        }

        .navPath
        {
            background: #A4B6D7;
            padding: 4px;
        }
    </style>
</head>
<body>
    <form id="form" runat="server">
    <div class="navPath">
        您的当前位置：系统管理&raquo;&raquo;执行SQL</div>
    <div style="vertical-align: top; margin-top: 10px; margin-left: 10px;">
        <asp:TextBox ID="txtSQL" runat="server" TextMode="MultiLine" Height="80px" Width="90%"
            BorderStyle="Inset" OnTextChanged="txtSQL_TextChanged"></asp:TextBox>
        <br />
        <asp:Button ID="btnExeSql" runat="server" CssClass="button" Text="执行SQL" OnClick="btnExeSql_Click" />
        <asp:Label ID="lblExeNum" runat="server"></asp:Label>
        <a href="javascript:void(0)" onclick=" Open(document.getElementById('table').innerHTML); ">
            查看表结构</a>
        <div id="table" style="display: none;">
            <asp:GridView ID="grdTable" runat="server" Font-Size="12px" Width="100%">
                <RowStyle HorizontalAlign="Center" CssClass="tItem" />
                <PagerStyle CssClass="tPage" />
                <HeaderStyle CssClass="tHeader" />
                <AlternatingRowStyle CssClass="tAlter" />
                <SelectedRowStyle BackColor="#F1F5FB" />
            </asp:GridView>
        </div>
        <div class="tipInfo" style="width: 90%; display: none;" id="tip">
            此功能建议仅供系统管理员在十分熟悉SQL语句的情况下操作，否则可能造成数据库数据丢失。</div>
        <hr style="border-collapse: collapse; width: 90%; text-align: left;" />
    </div>
    <asp:GridView ID="grdSQL" runat="server" Width="100%" Visible="False">
        <RowStyle HorizontalAlign="Center" CssClass="tItem" />
        <PagerStyle CssClass="tPage" />
        <HeaderStyle CssClass="tHeader" />
        <AlternatingRowStyle CssClass="tAlter" />
        <SelectedRowStyle BackColor="#F1F5FB" />
    </asp:GridView>
    <script type="text/javascript">
var Osel = document.form;
Osel.onsubmit = function() {
    if (Osel. <%=txtSQL.ClientID%> .value == "") {
        alert("输入不可为空");
        Osel. <%=txtSQL.ClientID%> .focus();
        return false;
    } else if (Osel. <%=txtSQL.ClientID%> .value.indexOf("update") != -1 || Osel. <%=txtSQL.ClientID%> .value.indexOf("delete") != -1 || Osel. <%=txtSQL.ClientID%> .value.indexOf("truncate") != -1) {
        if (confirm("即将执行的操作带有一定的风险，是否继续？"))
            return true;
        else
            return false;
    }
};

function Open(value) {
    var TestWin = open('', '', 'toolbar=no, scrollbars=yes, menubar=no, location=no, resizable=no');
    TestWin.document.title = "数据库表结构";
    TestWin.document.write(value);
}

function clear() {
    document.getElementById("tip").style.display = "none";
}

window.setInterval("clear()", 3000);
    </script>
    </form>
</body>
</html>