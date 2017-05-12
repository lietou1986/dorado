<%@ Page Title="登录" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs"
    Inherits="Dorado.VWS.Admin.Login" %>

<!DOCTYPE html>
<html lang="zh">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>同步系统管理平台--登录</title>
    <link href="/Styles/style.css" rel="Stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.1.min.js" type="text/javascript"> </script>
    <style type="text/css">
        body
        {
            background-color: #F0F0F0;
            font-size: 12px;
        }
        .container
        {
            margin-top: 10%;
            width: 80%;
        }
        .head h2
        {
            font-size: 24px;
            padding: 15px;
        }
        .body
        {
            padding: 20px;
        }
        .container li
        {
            color: red;
            list-style: none;
            text-align: left;
            padding-left: 50px;
        }
        .container .failureNotification
        {
            color: red;
        }
    </style>
</head>
<body>
    <form id="Form1" runat="server">
    <center>
        <div class="container">
            <div class="head">
                <h2>
                    同步系统2.0-Beta
                </h2>
            </div>
            <hr size="1" />
            <div class="body">
                <asp:Login ID="LoginUser" runat="server" EnableViewState="false" RenderOuterTable="false"
                    OnAuthenticate="LoginUser_Authenticate">
                    <LayoutTemplate>
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">用户名：</asp:Label>
                        <asp:TextBox ID="UserName" runat="server" CssClass="textEntry"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                            CssClass="failureNotification" ErrorMessage="必须填写“用户名”。" ToolTip="必须填写“用户名”。"
                            ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">密码：</asp:Label>
                        <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                            CssClass="failureNotification" ErrorMessage="必须填写“密码”。" ToolTip="必须填写“密码”。" ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                        <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="登录" ValidationGroup="LoginUserValidationGroup"
                            OnClick="LoginButton_Click" />
                        <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification"
                            ValidationGroup="LoginUserValidationGroup" />
                        <span class="failureNotification">
                            <asp:Literal ID="FailureText" runat="server"></asp:Literal>
                        </span>
                    </LayoutTemplate>
                </asp:Login>
            </div>
        </div>
    </center>
    </form>
    <script type="text/javascript">
        function OnSuccess(status) {
            if (status) {
                //alert(window.location.host);
                window.location.href = "http://" + window.location.host;
            } else {
                alert("用户名或密码错误！");
            }
        }

        function formsubmit() {
            var loginname = $("#loginname").val();
            var password = $("#password").val();
            if (loginname === "") {
                alert("登录名不能为空！")
                return;
            }
            if (password === "") {
                alert("密码不能为空！");
                return;
            }
            $("#form0").submit();

        }

        $("input").keydown(function (event) {
            if (event.keyCode == 13) {
                formsubmit();
            }
        });
    </script>
</body>
</html>