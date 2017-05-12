<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestPage.aspx.cs" Inherits="Dorado.VWS.Admin.TestPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        input[type='text']
        {
            border: 0 none;
            color: #FFFFFF;
            cursor: pointer;
            font-size: 0.9em;
            height: 21px;
            background-color: #999999;
            border: 1px soild #00000;
            margin: 0em;
            font: -webkit-small-control;
            letter-spacing: normal;
            word-spacing: normal;
            line-height: normal;
            text-transform: none;
            text-indent: 0px;
            text-shadow: none;
            display: inline-block;
            text-align: -webkit-auto;
        }
        input[type="text"]:disabled
        {
            border: 0 none;
            color: #FFFFFF;
            cursor: pointer;
            font-size: 0.9em;
            height: 21px;
            background-color: #999999;
            border: 1px soild #00000;
            margin: 0em;
            font: -webkit-small-control;
            letter-spacing: normal;
            word-spacing: normal;
            line-height: normal;
            text-transform: none;
            text-indent: 0px;
            text-shadow: none;
            display: inline-block;
            text-align: -webkit-auto;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        信息输出:<br />
        <asp:Literal ID="ltResult" runat="server"></asp:Literal><hr />
        权限测试：<br />
        域名<asp:TextBox ID="tbDomain" runat="server"></asp:TextBox>
        用户<asp:TextBox ID="tbUserName" runat="server"></asp:TextBox>
        <asp:Button ID="btnPerssion" runat="server" Text="权限测试" OnClick="btnPerssion_Click" />
        <hr />
    </div>
    <asp:Button ID="Button1" runat="server" Text="ClearCache" UseSubmitBehavior="true"
        OnClick="Button1_Click" />
    <input type="text" value="1234567890" />
    <input type="text" value="1234567890" disabled="disabled" />
    </form>
</body>
</html>