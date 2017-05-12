<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="FileRevert.aspx.cs" Inherits="Dorado.VWS.Admin.FileRevert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>同步-选择文件</title>
    <link rel="stylesheet" href="Styles/zTreeStyle.css" type="text/css" />
    <script type="text/javascript" src="Scripts/json.js"> </script>
    <script type="text/javascript" src="Scripts/jquery.ffpager.js"> </script>
    <script type="text/javascript" src="Scripts/jquery.ztree-2.6.min.js"> </script>
    <script type="text/javascript">
        var historyJson = null;
        var selectedJson = [];
        var domainID = 0;
        var selectPath = "";

        var zNodes = [];
        var setting = {
            checkable: false,
            addDiyDom: addDiyDom,
            async: true,
            asyncUrl: "Handler/FileListHandler.ashx", //获取节点数据的URL地址
            asyncParam: ["id"],
            callback: {
                click: zTreeNodeClick
            }
        };

        $(document).ready(function () {
            //$.getJSON("/Handler/GetInfoHandler.ashx", { "action": "domains" }, function(json) {
            //    $(json).each(function() {
            //        if (this.selected) {
            //            $("#domains").append("<option value='" + this.id + "' selected>" + this.name + "</option>");
            //        } else {
            //            $("#domains").append("<option value='" + this.id + "'>" + this.name + "</option>");
            //        }
            //    });
            //    refreshTree();
            //});

            $("#<%=DdlDomains.ClientID%>").change(function () {
                historyJson = null;
                selectedJson = [];
                $("#tableHistoryTitle").text("历史文件记录");
                $("#tableHistory,#pagerHistory").html("");
                $("#tableSelected").html("");
                $("#btnSubmit").attr("disabled", "disabled");
                refreshTree();
            });
        });

        function refreshTree() {

            // 当域名ID不为0时，刷新树
            domainID = $("#<%=DdlDomains.ClientID%>").val();
            if (domainID != '0') {

                $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "testConnByDomainId", "domainid": domainID }, function (json) {
                    if (json.ret) {
                        setting.asyncParamOther = {
                            "permission": "1",
                            "domainid": domainID
                        };

                        $("#tree").zTree(setting, zNodes);
                    } else {
                        alert("同步源连接失败！");
                        window.location.href = 'FileRevert.aspx';
                    }
                });

            }
        }

        function historyPageIndexChange(pageIndex, pageSize) {
            var begin = (pageIndex - 1) * pageSize + 1;
            $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "versionfiles", "begin": begin, "end": begin + pageSize - 1, "domainid": domainID, "filepath": selectPath }, function (json) {
                $("#tableHistory").html("");
                if (json.Version.length > 0) {
                    historyJson = json;
                    // 表头
                    $("#tableHistory").append("<tr><th>创建人</th><th>创建时间</th><th>备注</th><th>操作</th></tr>");
                    // 循环输出
                    for (var i = 0; i < json.Version.length; i++) {
                        $("#tableHistory").append("<tr><td>" + json.Version[i].Creator + "</td><td>" + ConvertDatetime(json.Version[i].CreateTime) + "</td><td>" + json.Version[i].Description + "</td><td><a href='javascript:;' onclick='SelectFile(" + i + ")'>选择</a> <a href='javascript:;' onclick='DownloadFile(\"" + json.Version[i].VersionPath.replace(/\\/ig, "\\\\") + "\", true)'>下载</a></td><tr>");
                    }
                } else {
                    $("#tableHistory").append("<tr><td style='text-align:center; color:#FF0000;'>目前没有历史文件记录</td></tr>");
                }

                $("#pagerHistory").ffpager({ "pageIndex": pageIndex, "pageSize": pageSize, "totalRecord": json.Count, "pageIndexChange": historyPageIndexChange });
            });
        }

        //在节点上显示自定义内容

        function addDiyDom(treeId, treeNode) {
            if (treeNode.isParent) return;
            var fullPath = treeNode.id.replace(/\\/ig, "\\\\");
            if (!treeNode.nocheck) {
                var aObj = $("#" + treeNode.tId + "_a");
                var html = " <a href='javascript:;' onclick='BindHistoryTable(\"" + fullPath + "\")'>历史</a> <a href='javascript:;' onclick='DownloadFile(\"" + fullPath + "\", false)'>下载</a>";
                aObj.after(html);
            }
        }

        ;

        //点击了树节点

        function zTreeNodeClick(event, treeId, treeNode) {
            if (!treeNode.isParent) BindHistoryTable(treeNode.id);
        }

        //绑定文件历史信息表格

        function BindHistoryTable(filePath) {
            $("#tableHistoryTitle").text("历史文件记录[" + filePath + "]");
            selectPath = filePath;
            historyPageIndexChange(1, 10);
        }

        //选择文件

        function SelectFile(i) {
            if (IsExist(historyJson.Version[i])) {
                $("#tip").text("此文件已经选择！");
                $("#tip").fadeIn().delay(1000).fadeOut();
                return;
            }
            selectedJson.push(historyJson.Version[i]);
            BindSelectedTable();
        }

        //移除文件

        function RemoveFile(i) {
            selectedJson.splice(i, 1);
            BindSelectedTable();
        }

        //绑定已选择文件表格

        function BindSelectedTable() {
            $("#tableSelected").html("");
            if (selectedJson.length > 0) {
                $("#btnSubmit").removeAttr("disabled");
                $("#tableSelected").append("<tr><th>文件名</th><th>创建人</th><th>创建时间</th><th>备注</th><th>操作</th></tr>");
                $(selectedJson).each(function (i) {
                    $("#tableSelected").append("<tr><td>" + this.FilePath + "</td><td>" + this.Creator + "</td><td>" + ConvertDatetime(this.CreateTime) + "</td><td>" + this.Description + "</td><td><a href='javascript:;' onclick='RemoveFile(" + i + ")'>移除</a></td><tr>");
                });
            } else {
                $("#btnSubmit").attr("disabled", "disabled");
                $("#tableSelected").append("<tr><td style='text-align:center; color:#FF0000;'>尚未选择任何文件</td></tr>");
            }
        }

        //判断文件是否已选择

        function IsExist(obj) {
            var i = selectedJson.length;
            while (i--)
                if (selectedJson[i].FilePath == obj.FilePath) return true;
            return false;
        }

        //提交回滚的文件

        function SubmitData() {
            if (!confirm("您的确需要回滚？")) {
                return;
            }

            $.ajaxSetup({
                beforeSend: null,
                complete: null
            });
            $("#btnSubmit").attr("disabled", "disabled");

            var data = $.toJSON(selectedJson);
            data = data.replace(/\/Date\(.+?\)/ig, "\\$&\\"); //使日期时间成为符合.net反序列化的格式
            $.getJSON("/Handler/SubmitHandler.ashx", { "action": "revertfiles", "data": data }, function (json) {
                if (json.errorMsg == '') {
                    $("#tip").text("已经成功添加回滚任务，任务ID如下： " + json.taskID);
                    var taskID = json.taskID.split(",");
                    for (var i in taskID) {
                        GetTaskResult(taskID[i]);
                    }
                } else {
                    $("#taskResult").text(json.errorMsg);
                }
            });
        }

        // 转化日期函数

        function ConvertDatetime(obj) {
            var str = obj.replace(/\//ig, "");
            return eval("new " + str).toLocaleString();
        }

        //获取回滚任务状态

        function GetTaskResult(taskID) {
            $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "task", "taskid": taskID }, function (data) {
                if (data.TaskStatus == 1 || data.TaskStatus == 2) {
                    $("#loadingImg").show();
                    $("#taskResult").text("回滚任务 " + taskID + " 状态：回滚中……");
                    setTimeout(function () { GetTaskResult(taskID); }, 2000);
                } else {
                    $("#loadingImg").hide();
                    $("#btnSubmit").removeAttr("disabled", "disabled");
                    $.ajaxSetup({
                        beforeSend: ShowProgress,
                        complete: HideProgress
                    });

                    if (data.TaskStatus == 3) {
                        $("#taskResult").text("回滚任务 " + taskID + " 状态：回滚成功！");
                    } else if (data.TaskStatus == 4) {
                        $("#taskResult").text("回滚任务 " + taskID + " 状态：回滚失败！");
                    }
                }
            });
        }

        //下载文件

        function DownloadFile(filePath, isHistory) {
            $.getJSON("/Handler/DownloadFile.ashx", { "action": "remote", "filePath": filePath, "domainID": domainID, "isHistory": isHistory }, function (json) {
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
    <div style="text-align: center">
        <p class="tip">
            <lable style="font-weight: bolder">文件回滚：</lable>
            同步源(demo环境)的文件修改为被选中的版本。<lable style="font-weight: bolder; color: Red">注意：</lable>文件历史默认只显示登录账号的同步历史，如须全部显示，请向管理员申请权限。
        </p>
        <div class="tableTitle">
            文件回滚</div>
        <table border="0" align="center" cellpadding="5" cellspacing="0" class="listTable">
            <tr>
                <td align="left" valign="top" style="width: 30%;">
                    环境：
                    <asp:DropDownList ID="DdlEnvironment" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlEnvironment_SelectedIndexChanged">
                    </asp:DropDownList>
                    域名:
                    <asp:DropDownList ID="DdlDomains" runat="server">
                    </asp:DropDownList>
                    <ul id="tree" class="tree" style="overflow: auto;">
                    </ul>
                </td>
                <td align="left" valign="top">
                    <div id="tableHistoryTitle" style="text-align: center; font-weight: bolder;">
                        历史文件记录</div>
                    <table id="tableHistory" class="listTable">
                    </table>
                    <div id="pagerHistory">
                    </div>
                    <div id="tip" class="tipMsg">
                    </div>
                    <hr />
                    <div style="text-align: center; font-weight: bolder;">
                        已选择文件</div>
                    <table id="tableSelected" class="listTable">
                    </table>
                    <div id="pagerSelected">
                    </div>
                    <br />
                    <div style="text-align: center;">
                        <input type="button" id="btnSubmit" onclick=" SubmitData(); " disabled="disabled"
                            value="回 滚" />
                    </div>
                    <div style="text-align: center;">
                        <img id="loadingImg" alt="loading" src="styles/img/loading2.gif" style="display: none;" />
                        <span id="taskResult" class="tipMsg"></span>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>