<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ReadLog.aspx.cs" Inherits="Dorado.VWS.Admin.ReadLog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="My97DatePicker/WdatePicker.js" type="text/javascript"> </script>
    <script type="text/javascript">
        function Clear() {
            $("#<%=ltInfo.ClientID%>").val("");
            $("#<%=tbResult.ClientID%>").val("");
        }
    </script>
    <div style="padding-top: 10px; padding-left: 50px; padding-right: 50px;">
        本页面用来查看Web站点后台日志<br />
        <asp:ScriptManager ID="sm" runat="server">
        </asp:ScriptManager>
        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
            <ProgressTemplate>
                查询中，请稍后
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:UpdatePanel ID="up" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                日期:<asp:TextBox ID="tbDate" runat="server" onclick="WdatePicker();"></asp:TextBox>
                时间: 从<asp:DropDownList ID="ddlBegin" runat="server">
                </asp:DropDownList>
                到<asp:DropDownList ID="ddlEnd" runat="server">
                </asp:DropDownList>
                <asp:Button ID="btnRead" runat="server" Text="查看" OnClientClick="Clear();" OnClick="btnRead_Click" />
                <asp:Literal ID="ltInfo" runat="server"></asp:Literal><br />
                <asp:TextBox ID="tbResult" runat="server" Height="350px" TextMode="MultiLine" Width="100%"></asp:TextBox>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>