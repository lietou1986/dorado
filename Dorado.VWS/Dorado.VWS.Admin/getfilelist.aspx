<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="getfilelist.aspx.cs"
    Inherits="Dorado.VWS.Admin.getfilelist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function Clear() {
            $("#<%=Label1.ClientID%>").val("");
            $("#<%=tbResult.ClientID%>").val("");
        }
    </script>
    <div style="padding-top: 10px; padding-left: 50px; padding-right: 50px;">
        <asp:ScriptManager ID="sm" runat="server">
        </asp:ScriptManager>
        <asp:UpdateProgress ID="up" runat="server">
            <ProgressTemplate>
                查询中，请稍后
            </ProgressTemplate>
        </asp:UpdateProgress>
        <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
            </ContentTemplate>
        </asp:UpdatePanel>
        请选择域名:
        <asp:DropDownList ID="ddlDomain" runat="server">
        </asp:DropDownList>
        &nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" Text="获取文件列表" OnClientClick="Clear();" OnClick="Button1_Click" />&nbsp;&nbsp;
        <asp:Label ID="Label1" runat="server"></asp:Label>
        <br />
        <asp:TextBox ID="tbResult" runat="server" Height="350px" TextMode="MultiLine" Width="100%"></asp:TextBox>
    </div>
</asp:Content>