<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="RefuseService.aspx.cs" Inherits="Dorado.VWS.Admin.RefuseService" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        拒绝操作的列表（每行一个）：<br />
        <asp:TextBox ID="TextBox1" runat="server" Height="334px" TextMode="MultiLine" Width="320px"></asp:TextBox>
        <br />
        <asp:Label ID="Label1" runat="server" Style="color: #FF0000" Text=""></asp:Label>
        <br />
        &nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="保存" />
    </div>
</asp:Content>