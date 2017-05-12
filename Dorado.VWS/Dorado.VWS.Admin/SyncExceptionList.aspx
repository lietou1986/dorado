<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SyncExceptionList.aspx.cs" Inherits="Dorado.VWS.Admin.SyncExceptionList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>任务回滚</title>
    <script type="text/javascript" src="Scripts/jquery.ffpager.js"> </script>
    <script src="My97DatePicker/WdatePicker.js" type="text/javascript"> </script>
    <script type="text/javascript">
        var domainName;
        var operateType;
        var username;
        var startDate;
        var endDate;
        $(
            function () {
                //$.getJSON("/Handler/GetInfoHandler.ashx", { "action": "alldomains" }, function(json) {
                //    $(json).each(function() {
                //        $("#domains").append("<option value='" + this.name + "'>" + this.name + "</option>");
                //    });
                //});

                $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "operateTypes" }, function (json) {
                    $(json).each(function () {
                        $("#operateTypes").append("<option value='" + this.id + "'>" + this.name + "</option>");
                    });
                    refreshTasks();
                });

                $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "getvws2user" }, function (json) {
                    $(json).each(function () {
                        $("#ddluserlist").append("<option value='" + this.name + "'>" + this.name + "</option>");
                    });
                });

                $("#domains").change(function () {
                    $("#divoperatelog").html("");
                    $("#divpager").html("");
                    refreshTasks();
                });

                $("#operateTypes").change(function () {
                    $("#divoperatelog").html("");
                    $("#divpager").html("");
                    refreshTasks();
                });

                $("#ddluserlist").change(function () {
                    $("#divoperatelog").html("");
                    $("#divpager").html("");
                    refreshTasks();
                });
            }
        );

        // 转化日期函数
        function tdclick(i) {
            //if ($("#detail" + i).style.display == "none")
            $("#detail" + i).toggle(100);
            // else
            //     $("#detail" + i).css('display', 'none');
        }
        function ConvertDatetime(obj) {
            var str = obj.replace(/\//ig, "");
            return eval("new " + str).toLocaleString();
        }

        function refreshTasks() {
            $("#divoperatelog").html("");
            $("#divpager").html("");
            // 当域名ID不为0时，刷新任务列表
            domainName = $("#MainContent_ddlDomains").val();
            operateType = $("#operateTypes").val();
            username = $("#ddluserlist").val();
            startDate = $("#txtStartDate").val();
            endDate = $("#txtEndDate").val();

            if (domainName == '0' || domainName == null) {
                domainName = "";
            }
            if (operateType == '0') {
                operateType = "";
            }
            if (username == '0') {
                username = "";
            }
            getversion(1, 10);
        }
        //"domainname": domainName, "operatetype": operateType, "username": username, "startdate": startDate, "enddate": endDate,
        function getversion(pageIndex, pageSize) {
            var begin = (pageIndex - 1) * pageSize + 1;
            $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "SyncExceptionList", "domainname": domainName, "operatetype": operateType, "queryusername": username, "startdate": startDate, "enddate": endDate, "begin": begin, "end": begin + pageSize - 1 }, function (data) {
                if (data.SyncExceptionList.length > 0) {
                    var str = "<table cellpadding='6' cellspacing='0' class='listTable' style='width:100%; table-layout:fixed;'><tr><th align='center' width='60px'>任务编号</th><th align='center'>域名</th><th align='center'>详情</th><th align='center' width='160px'>操作人</th><th align='center' width='60px'>结果</th><th align='center' width='120px'>时间</th><th align='center' width='120px'>操作</th></tr>";
                    for (var i = 0; i < data.SyncExceptionList.length; i++) {

                        var result = "";
                        var status = data.SyncExceptionList[i].SyncStatus;
                        if (status == 1) {
                            result = "同步中";
                        }
                        else if (status == 2) {
                            result = "同步挂起";
                        }
                        var log = "添加文件:";
                        log += data.SyncExceptionList[i].AddFiles.length > 45 ? data.SyncExceptionList[i].AddFiles.substring(0, 45) + "&nbsp;&nbsp;&nbsp;&nbsp;......" : data.SyncExceptionList[i].AddFiles;
                        str += "<tr><td align='center'>" + data.SyncExceptionList[i].TaskId +
                                "</td><td align='center'>" + data.SyncExceptionList[i].DomainName +
                                "</td><td onclick='return tdclick(" + i + ")'>" + log +
                                "</td><td align='center'>" + data.SyncExceptionList[i].UserName +
                                "</td><td>" + result + "</td><td align='center'>" + ConvertDatetime(data.SyncExceptionList[i].CreateTime) +
                                "</td><td><input type='button' value='强制结束任务' onclick='clearSyncException(\"" + data.SyncExceptionList[i].DomainName +
                                "\",\"" + data.SyncExceptionList[i].TaskId + "\",\"" + data.SyncExceptionList[i].UserName +
                                "\")' /></td></tr>";
                        str += "<tr id='detail" + i + "' style='display: none'><td>操作详情：</td><td colspan='6' >添加文件：" + data.SyncExceptionList[i].AddFiles + "\n删除文件：" + data.SyncExceptionList[i].DelFiles + "\n同步日志：" + data.SyncExceptionList[i].LogInfo + "</td></tr>";
                    }
                    str += "</table>";
                    $("#divoperatelog").html(str);
                    $("#divpager").ffpager({ "pageIndex": pageIndex, "pageSize": pageSize, "totalRecord": data.Count, "pageIndexChange": getversion });
                }
            }
           );
        }
        function clearSyncException(domaiName1, taskId, userName) {
            if (confirm("谨慎操作！此操作非安全的把任务置为结束。")) {
                $.ajax({
                    type: 'post',
                    url: 'Handler/SyncException.ashx?action=ClearSyncException&domainname=' + domaiName1 + '&userName=' + userName + '&taskId=' + taskId, //url  action是方法的名称
                    dataType: "text", //可以是text，如果用text，返回的结果为字符串；如果需要json格式的，可是设置为json
                    ContentType: "application/json; charset=utf-8",
                    success: function (data) {
                        if (data == "true") {
                            alert("操作成功！");
                        }
                        else {
                            alert("操作失败！");
                        }
                        refreshTasks();
                    },
                    error: function (msg) {
                        alert("操作失败！");
                        refreshTasks();
                    }

                });

            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: center">
        <div class="tableTitle">
            操作日志</div>
        <table border="0" align="center" cellpadding="5" cellspacing="0" class="listTable">
            <tr>
                <th align="right" style="width: 80px;">
                </th>
                <td align="left">
                    环境：<asp:DropDownList ID="DdlEnvironment" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlEnvironment_SelectedIndexChanged">
                    </asp:DropDownList>
                    域名:<asp:DropDownList ID="ddlDomains" runat="server" AutoPostBack="false">
                    </asp:DropDownList>
                    操作类型:
                    <select id="operateTypes">
                        <option value="0">请选择</option>
                    </select>
                    操作人:
                    <select id="ddluserlist">
                        <option value="0">请选择</option>
                    </select>
                </td>
            </tr>
            <tr>
                <th align="right">
                </th>
                <td align="left">
                    开始时间:
                    <input id="txtStartDate" class="Wdate" type="text" onclick=" WdatePicker(); " />
                    结束时间:
                    <input id="txtEndDate" class="Wdate" type="text" onclick=" WdatePicker(); " />
                    <input type="button" value="   [查   询]   " onclick=" refreshTasks(); " />
                </td>
            </tr>
            <tr>
                <th align="right">
                    异常任务列表
                </th>
                <td align="left">
                    <div id="divoperatelog">
                    </div>
                    <div id="divpager" align="center">
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>