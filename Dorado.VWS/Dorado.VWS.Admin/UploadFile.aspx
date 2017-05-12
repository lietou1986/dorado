<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="UploadFile.aspx.cs" Inherits="Dorado.VWS.Admin.UploadFile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>同步-选择文件</title>
    <link rel="stylesheet" href="Styles/zTreeStyle.css" type="text/css" />
    <script type="text/javascript" src="Scripts/jquery.ztree-2.6.min.js"> </script>
    <script type="text/javascript" src="Scripts/jquery.MultiFile.pack.js"> </script>
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
            asyncParam: ["id"]
        };

        var uploadCount = 0;

        $(
            function () {
                $("#tdFolderList").hide();
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

                //上传控件
                $("#upCtrl").MultiFile({
                    "afterFileSelect": function () { uploadCount++; },
                    "afterFileRemove": function () { uploadCount--; },
                    "STRING": { "duplicate": "此文件已选择！" }
                });
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

        ;

        function addDiyDom(treeId, treeNode) {
            var filepath = treeNode.id.replace(/\\/g, '\\\\');
            var aObj = $("#" + treeNode.tId + "_a");

            if (treeNode.isParent) {
                var upLink = "<a href='javascript:;' onclick='UploadFile(\"" + filepath + "\")'>上传</a>";
                aObj.after(upLink);
            }

            if ($("#diyChk_" + treeNode.id).length > 0 || treeNode.isParent) return;
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

        //上传文件

        function UploadFile(uploadPath) {
            $("#upDiv").css("left", ($(window).width() - $("#upDiv").width()) / 2);
            $("#upDiv").css("top", ($(window).height() - $("#upDiv").height()) / 2);
            $("#mask").show();
            $("#upDiv").show("fast");

            $("#hfUploadPath").val(uploadPath);
            $("#upTip").text("上传到：" + uploadPath);
            $("#upCtrl").show();
        }

        function UploadCheck() {
            if ($("#hfUploadPath").val() == "") {
                $("#upTip").text("请选择上传的目录！");
                return false;
            }
            if (uploadCount == 0) {
                alert("请选择上传的文件！");
                return false;
            }
            return true;
        }

        function CanclUpload() {
            $("#upDiv").hide("fast");
            $("#mask").hide();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: left;">
        <p class="tip">
            文件目录选择: 选择需要同步的文件。因为是异步刷新的目录树，所以只有被打勾的文件才会被同步。其中<lable style="color: #f0742e">橙色</lable>文件表示正在被其它同事同步的文件。<br />
            文件列表输入(此权限需要向管理员申请): 输入文件的相对路径进行同步。‘#’开头的行为注释，相对路径前不加斜线。例如：ABC目录中的Test.xml，输入格式为“ABC\Test.xml”。
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
                    <asp:DropDownList ID="DdlDomains" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlDomains_SelectedIndexChanged">
                    </asp:DropDownList>
                    <asp:Button ID="BtnAdd" TabIndex="0" OnClientClick="return check();" runat="server"
                        Text="下一步" OnClick="BtnAdd_Click" />
                    <asp:HiddenField ID="HfAddFiles" runat="server" />
                    <asp:HiddenField ID="HfDelFiles" runat="server" />
                </td>
            </tr>
            <tr>
                <td align='left' valign="top" id="tdFilelist" runat="server">
                    文件列表输入<br />
                    <asp:TextBox ID="TxtFileList" runat="server" TextMode="MultiLine"></asp:TextBox>
                </td>
                <td align="left" valign="top" id="tdFolderList">
                    文件目录选择<br />
                    根目录 <a href='javascript:;' onclick=' UploadFile("根目录"); '>上传</a>
                    <ul id="tree" class="tree" style="overflow: auto;">
                    </ul>
                </td>
            </tr>
        </table>
        <div id="tip" class="tipMsg">
        </div>
        <div style="text-align: center;">
            <asp:Label runat="Server" ID="lResult" ForeColor="Red"></asp:Label></div>
    </div>
    <div id="upDiv" style="position: fixed; background-color: #999999; line-height: 150%;
        padding: 10px; border: 1px solid #555555; display: none;">
        <div id="upTip" style="text-align: left; color: #FFFFFF;">
        </div>
        <input id="upCtrl" type="file" />
        <asp:Button runat="Server" ID="bUpload" OnClick="bUpload_Click" Text="上传" OnClientClick="return UploadCheck();" />
        <input type="button" value="取消" onclick=" CanclUpload(); " />
        <asp:HiddenField runat="Server" ID="hfUploadPath" ClientIDMode="Static" />
    </div>
</asp:Content>