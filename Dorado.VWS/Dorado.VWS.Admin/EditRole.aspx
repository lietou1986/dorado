<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EditRole.aspx.cs" Inherits="Dorado.VWS.Admin.EditRole" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>修改角色</title>
    <link rel="stylesheet" href="Styles/zTreeStyle.css" type="text/css" />
    <script type="text/javascript" src="Scripts/jquery.ztree-2.6.min.js"> </script>
    <script type="text/javascript">
        function check() {
            if ($("#<%=txtName.ClientID%>").val() == "") {
                alert("名称不能为空！");
                $("#<%=txtName.ClientID%>").focus();
                return false;
            }

            if ($("#<%=ChkAll.ClientID%>").attr("checked") != "checked") {
                var tmp = zTree1.getCheckedNodes();
                var chks = "";
                $.each(tmp, function (i, n) {
                    chks = chks + $(n).attr("id") + "###";
                });
                $("#<%=HfChecks.ClientID%>").val(chks);
                if ($("#<%=HfChecks.ClientID%>").val() == '') {
                    alert("请选择文件");
                    return false;
                }
            }
            return true;
        }

        var zTree1;
        var setting;
        var zNodes = [];
        var did;

        setting = {
            checkable: true,
            checkType: { "Y": "", "N": "p" },
            async: true,
            asyncUrl: "Handler/FileListHandler.ashx",
            //获取节点数据的URL地址
            asyncParam: ["id"]
        };

        $(
            function () {
                $("#<%=ddlSource.ClientID%>").change(
                    function () {
                        // 当域名ID不为0时，刷新树
                        domainid = $("#<%=ddlSource.ClientID%>").val();
                        roleId = GetRequest()["id"];
                        if (domainid != '0') {
                            did = domainid;
                            refreshTree(domainid, roleId);
                        }
                    }
                );
                // 当域名ID不为0时，刷新树
                var domainid = $("#<%=ddlSource.ClientID%>").val();
                var roleId = GetRequest()["id"];
                if (domainid != '0') {
                    did = domainid;
                    refreshTree(domainid, roleId);
                }
            }
	    );

        function refreshTree(domainid, roleId) {
            setting.asyncParamOther = {
                "permission": "0",
                "domainid": domainid,
                "roleId": roleId
            };

            zTree1 = $("#tree").zTree(setting, zNodes);
        }

        function GetRequest() {
            var url = location.search; //获取url中"?"符后的字串
            var theRequest = new Object();
            if (url.indexOf("?") != -1) {
                var str = url.substr(1);
                var strs = str.split("&");
                for (var i = 0; i < strs.length; i++) {
                    theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
                }
            }
            return theRequest;

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: center">
        <table border="0" align="center" cellpadding="0" cellspacing="5">
            <tr>
                <td align="center" colspan="2">
                    <b>修改角色</b>
                </td>
            </tr>
            <tr>
                <td align="right">
                    名称：
                </td>
                <td align="left">
                    <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                    <asp:Button ID="btnEdit" OnClientClick="return check();" runat="server" Text="修 改"
                        OnClick="btnEdit_Click" />
                </td>
            </tr>
            <tr>
                <td align="right">
                    环境：
                </td>
                <td align="left">
                    <asp:DropDownList ID="DdlEnvironment" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlEnvironment_SelectedIndexChanged"
                        Enabled="false">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td align="right">
                    域名：
                </td>
                <td align="left">
                    <asp:DropDownList ID="ddlSource" runat="server" Enabled="false">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td align="right" valign="top">
                    目录：
                </td>
                <td align="left">
                    <asp:CheckBox ID="ChkAll" Text="All" runat="server" />
                    <ul id="tree" class="tree" style="overflow: auto;">
                    </ul>
                    <asp:HiddenField ID="HfChecks" runat="server" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>