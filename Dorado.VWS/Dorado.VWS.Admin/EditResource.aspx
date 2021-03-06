﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EditResource.aspx.cs" Inherits="Dorado.VWS.Admin.EditResource" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>修改资源</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: center">
        <table border="0" align="center" cellpadding="0" cellspacing="5">
            <tr>
                <td align="center" colspan="2">
                    <b>修改资源</b>
                </td>
            </tr>
            <tr>
                <td align="right">
                    资源值：
                </td>
                <td align="left">
                    <asp:TextBox ID="txtValue" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                        ControlToValidate="txtValue" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td align="right">
                    资源描述：
                </td>
                <td align="left">
                    <asp:TextBox ID="txtDescription" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*"
                        ControlToValidate="txtDescription" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td align="right" colspan="2">
                    <asp:Button ID="btnEdit" runat="server" Text="修 改" OnClick="btnEdit_Click" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>