<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="TaskRevert.aspx.cs" Inherits="Dorado.VWS.Admin.TaskRevert" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>任务回滚</title>
    <script type="text/javascript" src="Scripts/jquery.ffpager.js"> </script>
    <script type="text/javascript">
        var domainId = 0;
        var syncTaskID = 0;

        $(
            function () {
                //$.getJSON("/Handler/GetInfoHandler.ashx", { "action": "domains" }, function (json) {
                //    $(json).each(function () {
                //        if (this.id == domainId) {
                //            $("#domains").append("<option value='" + this.id + "' selected>" + this.name + "</option>");
                //        } else {
                //            $("#domains").append("<option value='" + this.id + "'>" + this.name + "</option>");
                //        }
                //    });
                //    refreshTasks();
                //});

                $("#<%=DdlDomains.ClientID%>").change(function () {
                    $("#divversion").html("");
                    $("#divpager").html("");
                    refreshTasks();
                });
            }
        );

        function refreshTasks() {
            $("#taskResult").text("");

            // 当域名ID不为0时，刷新任务列表
            domainId = $("#<%=DdlDomains.ClientID%>").val();
            if (domainId != '0') {
                getversion(1, 10);
            }
        }

        // 转化日期函数

        function ConvertDatetime(obj) {
            var str = obj.replace(/\//ig, "");
            return eval("new " + str).toLocaleString();
        }

        function getversion(pageIndex, pageSize) {
            var begin = (pageIndex - 1) * pageSize + 1;
            $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "sucsynctasks", "domainid": domainId, "begin": begin, "end": begin + pageSize - 1 }, function (data) {
                if (data.Version.length > 0) {
                    var str = "<table cellpadding='5' cellspacing='0' class='listTable'><tr><th></th><th align='center'>任务编号</th><th align='center'>同步任务信息</th><th align='center'>创建者</th><th align='center'>创建时间</th><th align='center'>操作</th></tr>";
                    for (var i = 0; i < data.Version.length; i++) {
                        var description = data.Version[i].Description.length > 30 ? data.Version[i].Description.substring(0, 30) : data.Version[i].Description;
                        str += "<tr><td align='center'><input type='radio' id='r" + data.Version[i].TaskId + "' name='taskId' value='" + data.Version[i].TaskId + "' /><td align='center'>" + data.Version[i].TaskId + "</td></td><td>" + description + "</td><td>" + data.Version[i].UserName + "</td><td>" + ConvertDatetime(data.Version[i].CreateTime) + "</td><td align='center'><a href='TaskInfo.aspx?domainid=" + domainId + "&synctaskid=" + data.Version[i].TaskId + "' target='_blank'>任务细节</a></td><tr>";
                    }
                    str += "</table>";
                    $("#divversion").html(str);
                    $("#divpager").ffpager({ "pageIndex": pageIndex, "pageSize": pageSize, "totalRecord": data.Count, "pageIndexChange": getversion });
                }
            }
           );
        }

        //提交选择的文件

        function SubmitData(btnid, action, txt) {
            if ($(":radio[name='taskId'][checked]").length <= 0) {
                alert("请先选择任务！");
                return;
            }

            if (!confirm("您的确需要" + txt + "？")) {
                return;
            }

            $.ajaxSetup({
                beforeSend: null,
                complete: null
            });
            $("#" + btnid).attr("disabled", "disabled");

            $.getJSON("/Handler/SubmitHandler.ashx", { "action": action, "synctaskid": $(":radio[name='taskId'][checked]").val() }, function (json) {
                if (json.errorMsg == '') {
                    var taskID = json.taskID.split(",");
                    for (var i in taskID) {
                        GetTaskResult(btnid, taskID[i], txt);
                    }
                } else {
                    $("#taskResult").text(json.errorMsg);
                }
            });
        }

        //获取回滚任务状态

        function GetTaskResult(btnid, taskID, txt) {
            $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "task", "taskid": taskID }, function (data) {
                if (!data) {
                    $("#taskResult").text(txt + "任务 " + taskID + " 状态：服务器没有返回 状态!");
                    return;
                }
                if (data.TaskStatus == 1 || data.TaskStatus == 2) {
                    $("#loadingImg").show();
                    $("#taskResult").text(txt + "任务 " + taskID + " 状态：" + txt + "中……");
                    setTimeout(function () { GetTaskResult(btnid, taskID, txt); }, 2000);
                } else {
                    $("#loadingImg").hide();
                    $("#" + btnid).removeAttr("disabled", "disabled");
                    $.ajaxSetup({
                        beforeSend: ShowProgress,
                        complete: HideProgress
                    });

                    if (data.TaskStatus == 3) {
                        $("#taskResult").text(txt + "任务 " + taskID + " 状态：" + txt + "成功！");
                    } else if (data.TaskStatus == 4) {
                        $("#taskResult").text(txt + "任务 " + taskID + " 状态：" + txt + "失败！");
                    }
                }
            });
        }

        //验证提交
        function CheckRevertAndUpdate(txt) {
            if (domainId <= 0) {
                alert("请先选择域名！");
                return false;
            }

            if ($(":radio[name='taskId'][checked]").length <= 0) {
                alert("请先选择任务！");
                return false;
            }

            if (!confirm("您确定需要" + txt + "？")) {
                return false;
            }

            $("#<%=hfDomainID.ClientID%>").val(domainId);
            syncTaskID = $(":radio[name='taskId'][checked]").val();
            $("#<%=hfSyncTaskID.ClientID%>").val(syncTaskID);
            $('#divLoading').show();

            return true;
        }

        function SelectDomain(id) {
            domainId = id;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfDomainID" runat="server" Value="0" />
    <asp:HiddenField ID="hfSyncTaskID" runat="server" Value="0" />
    <asp:HiddenField ID="hfNewSyncTaskID" runat="server" Value="0" />
    <div id="divContainer" style="position: relative;">
        <div id="divLoading" style="width: 100%; height: 700px; position: absolute; z-index: 11;
            overflow: hidden; left: 0px; top: 0px; background: #939393; border: 0px solid #0000ff;
            display: none; text-align: center; vertical-align: middle; filter: Alpha(opacity=50);
            -moz-opacity: 0.5; opacity: 0.5; -khtml-opacity: 0.5;">
            <img src="styles/img/loading.gif" />
        </div>
        <div style="text-align: center">
            <p class="tip">
                <lable style="font-weight: bolder">回滚：</lable>
                把同步源(demo环境)的文件修改为被选中任务的上一版本。<lable style="font-weight: bolder">恢复：</lable>把同步源(demo环境)的文件修改为被选中任务的版本。
                <lable style="font-weight: bolder; color: Red">注意：</lable>
                任务列表默认显示登录账号的同步任务，如须全部显示，请向管理员申请权限。
            </p>
            <div class="tableTitle">
                任务回滚</div>
            <table border="0" align="center" cellpadding="5" cellspacing="0" class="listTable">
                <tr>
                    <th align="right" style="width: 20%;">
                        域名:
                    </th>
                    <td align="left">
                        环境：
                        <asp:DropDownList ID="DdlEnvironment" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlEnvironment_SelectedIndexChanged">
                        </asp:DropDownList>
                        域名:
                        <asp:DropDownList ID="DdlDomains" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <th align="right">
                        任务版本列表
                    </th>
                    <td align="left">
                        <div id="divversion">
                        </div>
                        <div id="divpager">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <input type="button" id="btnRevert" onclick="SubmitData('btnRevert', 'reverttask', '回滚'); "
                            value="回 滚" />
                        <input type="button" id="btnResume" onclick="SubmitData('btnResume', 'resumetask', '恢复'); "
                            value="恢 复" />
                        <asp:Button ID="btnRevertAndSync" runat="server" Text="回滚并更新" OnClientClick="return CheckRevertAndUpdate('回滚并更新');"
                            OnClick="BtnSyncRevert_Click" />
                        <asp:Button ID="btnResumeAndSync" runat="server" Text="恢复并更新" OnClientClick="return CheckRevertAndUpdate('恢复并更新');"
                            OnClick="BtnSyncResume_Click" />
                        <div>
                            <img id="loadingImg" alt="loading" src="styles/img/loading2.gif" style="display: none;" />
                            <span id="taskResult" class="tipMsg"></span>
                        </div>
                    </td>
                </tr>
            </table>
            <p>
                <label id="syncResult">
                </label>
                <br />
                <asp:Label ID="lbFiles" runat="server"></asp:Label>
                <div id="loading" style="display: none; text-align: center; color: #0000FF;">
                    <img alt="loading" src="styles/img/loading2.gif" />同步任务处理中……
                </div>
            </p>
            <p style="color: Red;">
                注意！</p>
            <p>
                <b>【回滚并更新】</b> <b>回滚</b>Demo上选定任务，同时将该任务中所包含的文件在同步宿上<b>回滚</b>到该任务同步之<b>前</b>的状态。
            </p>
            <p>
                <b>【恢复并更新】</b> <b>恢复</b>Demo上选定任务，同时将该任务中所包含的文件在同步宿上<b>恢复</b>到该任务同步之<b>后</b>的状态。
            </p>
            <p>
                <b>【回滚】</b> <b>把同步源(demo环境)的文件修改为被选中任务的上一版本。</b>
            </p>
            <p>
                <b>【恢复】</b> <b>把同步源(demo环境)的文件修改为被选中任务的版本。</b>
            </p>
        </div>
</asp:Content>