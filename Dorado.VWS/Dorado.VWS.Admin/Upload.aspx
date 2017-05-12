<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Upload.aspx.cs" Inherits="Dorado.VWS.Admin.Upload" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    域名:<asp:Label ID="lbDomainName" runat="server"></asp:Label>
    上传路径:<asp:Label ID="lbRelativePath" runat="server"></asp:Label><br />
    <asp:FileUpload ID="fuUpload" runat="server" Visible="true" />&nbsp;&nbsp;
    <asp:Button ID="btnUpload" runat="server" Text="上传" OnClick="btnUpload_Click" />
    <asp:LinkButton ID="lbtnClose" runat="server" Text="返回" OnClientClick="go.history(-1);"></asp:LinkButton><br />
    <asp:Label ID="lbInfo" runat="server"></asp:Label>
</asp:Content>