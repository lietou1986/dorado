<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SyncSelectFilesSimple.aspx.cs" Inherits="Dorado.VWS.Admin.SyncSelectFilesSimple" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>同步-选择文件</title>
    <link rel="stylesheet" href="Styles/zTreeStyle.css" type="text/css" />
    <script type="text/javascript" src="Scripts/jquery.ztree-2.6.min.js"> </script>
    <script type="text/javascript">
        var zTree1;
        var setting;
        var zNodes = [];
        var did;
        setting = {
            checkable: true,
            addDiyDom: addDiyDom,
            async: true,
            fontCss: setFont,
            asyncUrl: "Handler/FileListHandler.ashx",
            //获取节点数据的URL地址
            asyncParam: ["id"],
            callback: {
                asyncSuccess: refreshSuc
            }
        };

        $(
            function () {
                $("#tdFolderList").hide();
                $("#<%=BtnAdd.ClientID %>").hide();
                $("#<%=DdlDomains.ClientID%>").change(
                    function () {
                        // 当域名ID不为0时，刷新树
                        domainid = $("#<%=DdlDomains.ClientID%>").val();
                        if (domainid != '0') {
                            did = domainid;
                            refreshTree(domainid);
                        }
                    }
            );
                // 当域名ID不为0时，刷新树
                var domainid = $("#<%=DdlDomains.ClientID%>").val();
                if (domainid != '0') {
                    did = domainid;
                    refreshTree(domainid);
                }
            }
	);

        function setFont(treeId, treeNode) {
            if (!treeNode.isParent && treeNode.isProce) {
                var css = { color: "#f0742e" };

                return css;
            } else {
                return {};
            }
        }

        function refreshTree(domainid) {
            $("#tdFolderList").show();
            setting.asyncParamOther = {
                "permission": "1",
                "domainid": domainid
            };

            zTree1 = $("#tree").zTree(setting, zNodes);
        }

        function refreshSuc() {
            $("#<%=BtnAdd.ClientID %>").show();
        }

        function addDiyDom(treeId, treeNode) {
            var filepath = treeNode.id.replace(/\\/g, '\\\\');
            if ($("#diyChk_" + treeNode.id).length > 0 || treeNode.isParent) return;
            var aObj = $("#" + treeNode.tId + "_a");
            var downLink = "&nbsp;&nbsp;<a href='javascript:;' onclick='DownloadFile(\"" + filepath + "\")'>下载</a>";
            aObj.after(downLink);
            if (treeNode.hasDelPermission) {
                var editStr = "<span id='diyChk_space_" + treeNode.id + "'>&nbsp;</span>" + "<input type='checkbox' class='delcheckbox' id='diyChk_" + treeNode.id + "' value='" + treeNode.id + "'>删除</input>";
                aObj.after(editStr);
            }
        }

        ;

        function check() {
            if ($("#<%=TxtFileList.ClientID%>").length > 0 && $("#<%=TxtFileList.ClientID%>").val() != '') {
                return true;
            }
            var tmp = zTree1.getCheckedNodes();
            var chks = "";
            $.each(tmp, function (i, n) {
                if ($(n).attr("isParent") == false) {
                    chks = chks + $(n).attr("id") + "###";
                }
            });
            $("#<%=HfAddFiles.ClientID%>").val(chks);

            chks = "";
            var deltmp = $(".delcheckbox:checked");
            $.each(deltmp, function (i, n) {
                chks = chks + $(n).val() + "###";
            });
            $("#<%=HfDelFiles.ClientID%>").val(chks);

            if ($("#<%=HfAddFiles.ClientID%>").val() == '' && $("#<%=HfDelFiles.ClientID%>").val() == '') {
                alert("请选择文件");
                return false;
            }

            if ($("#<%=HfDelFiles.ClientID%>").val() != '') {
                return confirm("删除很危险，请确认！");
            }

            $('#divLoading').show();
            return true;
        }

        // 转化日期函数

        function ConvertDatetime(obj) {
            var s;
            if (obj.indexOf("Date") > -1) {
                obj = obj.substring(obj.indexOf("(") + 1, obj.lastIndexOf(")"));
                var d = new Date(parseInt(obj));
                s = d.toLocaleString();
            } else {
                s = obj;
            }
            return s;
        }

        //下载文件

        function DownloadFile(filePath) {
            $.getJSON("/Handler/DownloadFile.ashx", { "action": "remote", "filePath": filePath, "domainID": did, "isHistory": false }, function (json) {
                if (json.ret) {
                    //                    var fileName = filePath.substr(filePath.lastIndexOf("\\") + 1);
                    location.href = "/Handler/DownloadFile.ashx?action=local&fileName=" + json.filepath;
                } else {
                    $("#tip").text("下载失败！");
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="sm" runat="server">
    </asp:ScriptManager>
    <div id="divContainer" style="position: relative;">
        <div id="divLoading" style="width: 100%; height: 700px; position: absolute; z-index: 11;
            overflow: hidden; left: 0px; top: 0px; background: #939393; border: 0px solid #0000ff;
            display: none; text-align: center; vertical-align: middle; filter: Alpha(opacity=50);
            -moz-opacity: 0.5; opacity: 0.5; -khtml-opacity: 0.5;">
            <img src="styles/img/loading.gif" />
        </div>
        <div id="divContent" style="text-align: center">
            <p class="tip">
                <lable style="font-weight: bolder">文件目录选择：</lable>
                1.只有被打勾的文件才会被同步；2.<lable style="color: #f0742e">橙色</lable>文件表示正在被其它同事同步的文件。
                <lable style="font-weight: bolder">文件列表输入功能：</lable>
                1.需要向管理员申请；2.使用相对路径名；3.以”#”开头的行是注释。
            </p>
            <table border="0" align="center" cellpadding="0" cellspacing="5">
                <tr>
                    <td align="center" colspan="2">
                        <b>同步-选择文件</b>
                    </td>
                </tr>
                <tr id="trdomain">
                    <td align="center" valign="top" colspan="2">
                        域名：
                        <asp:DropDownList ID="DdlDomains" runat="server" AutoPostBack="True">
                        </asp:DropDownList>
                        <asp:UpdatePanel ID="upBtn" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="BtnAdd" TabIndex="0" OnClientClick="return check();" runat="server"
                                    Text="同 步" OnClick="BtnAdd_Click" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td align='left' valign="top" id="tdFilelist" runat="server">
                        文件列表输入<br />
                        <asp:TextBox ID="TxtFileList" runat="server" TextMode="MultiLine"></asp:TextBox>
                    </td>
                    <td align="left" valign="top" id="tdFolderList">
                        文件目录选择<br />
                        <asp:HiddenField ID="HfAddFiles" runat="server" />
                        <asp:HiddenField ID="HfDelFiles" runat="server" />
                        <ul id="tree" class="tree" style="overflow: auto;">
                        </ul>
                    </td>
                </tr>
            </table>
            <asp:UpdatePanel ID="upResult" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label runat="Server" ID="tip" ClientIDMode="Static" ForeColor="Red"></asp:Label>
                    <div id="divResult" style="text-align: left; padding-left: 300px;">
                        <asp:Literal ID="ltResult" runat="server"></asp:Literal>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="BtnAdd" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>