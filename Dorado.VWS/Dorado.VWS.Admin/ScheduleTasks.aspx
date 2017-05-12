<%@ Page Title="计划任务列表" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ScheduleTasks.aspx.cs" Inherits="Dorado.VWS.Admin.ScheduleTasks" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>计划任务列表</title>
    <script type="text/javascript" src="Scripts/jquery.ffpager.js"> </script>
    <script type="text/javascript">
        var domainId = 0;
        $(
            function () {
                $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "domains" }, function (json) {
                    $(json).each(function () {
                        if (this.selected) {
                            $("#domains").append("<option value='" + this.id + "' selected>" + this.name + "</option>");
                        } else {
                            $("#domains").append("<option value='" + this.id + "'>" + this.name + "</option>");
                        }
                    });
                    refreshTasks();
                });

                $("#domains").change(function () {
                    refreshTasks();
                });
            }
        );

        function refreshTasks() {

            $("#divversion").html("");
            $("#divpager").html("");
            // 当域名ID不为0时，刷新任务列表
            domainId = $("#domains").val();
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
            $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "scheduletask", "domainid": domainId, "begin": begin, "end": begin + pageSize - 1 }, function (data) {
                if (data.ScheduleTasks.length > 0) {
                    var str = "<table cellpadding='5' cellspacing='0' class='listTable'><tr><th align='center'>实施时间</th><th align='center'>同步任务信息</th><th align='center'>创建者</th><th align='center'>创建时间</th><th align='center'>操作</th></tr>";
                    for (var i = 0; i < data.ScheduleTasks.length; i++) {
                        var description = data.ScheduleTasks[i].Description.length > 30 ? data.ScheduleTasks[i].Description.substring(0, 30) : data.ScheduleTasks[i].Description;
                        str += "<tr><td align='center'>" + ConvertDatetime(data.ScheduleTasks[i].ScheduleTime) + "</td></td><td>" + description + "</td><td align='center'>" + data.ScheduleTasks[i].Creator + "</td><td align='center'>" + ConvertDatetime(data.ScheduleTasks[i].CreateTime) + "</td><td align='center'><a href='ScheduleTaskInfo.aspx?domainid=" + domainId + "&scheduleTaskId=" + data.ScheduleTasks[i].TimerSyncTaskId + "' target='_blank'>任务细节</a>&nbsp;&nbsp;<a href='javascript:;' onclick='DelTask(\"" + data.ScheduleTasks[i].TimerSyncTaskId + "\")'>删除</a></td><tr>";
                    }
                    str += "</table>";
                    $("#divversion").html(str);
                    $("#divpager").ffpager({ "pageIndex": pageIndex, "pageSize": pageSize, "totalRecord": data.Count, "pageIndexChange": getversion });
                }
            }
           );
        }

        function DelTask(id) {
            if (confirm("确定要删除？")) {
                $.getJSON("/Handler/SubmitHandler.ashx", { "action": "deleteSchedultTask", "taskid": id }, function (json) {
                    if (json.Result) {
                        refreshTasks();
                    } else {
                        alert("删除失败！");
                    }
                });
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: center">
        <p class="tip">
            显示计划任务，可以查看细节或者删除它。
        </p>
        <div class="tableTitle">
            计划任务</div>
        <table border="0" align="center" cellpadding="5" cellspacing="0" class="listTable">
            <tr>
                <th align="right" style="width: 20%;">
                    域名:
                </th>
                <td align="left">
                    <select id="domains">
                        <option value="0">请选择</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th align="right">
                    计划任务列表
                </th>
                <td align="left">
                    <div id="divversion">
                    </div>
                    <div id="divpager">
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>