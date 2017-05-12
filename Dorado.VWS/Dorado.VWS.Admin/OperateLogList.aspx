<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="OperateLogList.aspx.cs" Inherits="Dorado.VWS.Admin.OperateLogList" %>

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
            $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "operatelog", "domainname": domainName, "operatetype": operateType, "queryusername": username, "startdate": startDate, "enddate": endDate, "begin": begin, "end": begin + pageSize - 1 }, function (data) {
                if (data.OperateLogList.length > 0) {
                    var str = "<table cellpadding='6' cellspacing='0' class='listTable' style='width:100%; table-layout:fixed;'><tr><th align='center' width='60px'>日志编号</th><th align='center'>域名</th><th align='center'>操作人</th><th align='center' width='100px'>操作类型</th><th align='center' width='500px'>日志</th><th align='center' width='30px'>结果</th><th align='center' width='150px'>时间</th></tr>";
                    for (var i = 0; i < data.OperateLogList.length; i++) {
                        var log = data.OperateLogList[i].Log.length > 45 ? data.OperateLogList[i].Log.substring(0, 45) + "&nbsp;&nbsp;&nbsp;&nbsp;......" : data.OperateLogList[i].Log;
                        var result = data.OperateLogList[i].Result == true ? "成功" : "失败";
                        str += "<tr><td align='center'>" + data.OperateLogList[i].OperationLogId + "</td><td align='center'>" + data.OperateLogList[i].DomainName + "</td><td align='center'>" + data.OperateLogList[i].UserName + "</td><td align='center'>" + data.OperateLogList[i].OperateTypeName + "</td><td onclick='return tdclick(" + i + ")'>" + log + "</td><td>" + result + "</td><td align='center'>" + ConvertDatetime(data.OperateLogList[i].CreateTime) + "</td></tr>";
                        str += "<tr id='detail" + i + "' style='display: none'><td>操作详情：</td><td colspan='6' >" + data.OperateLogList[i].Log + "</td></tr>";
                    }
                    str += "</table>";
                    $("#divoperatelog").html(str);
                    $("#divpager").ffpager({ "pageIndex": pageIndex, "pageSize": pageSize, "totalRecord": data.Count, "pageIndexChange": getversion });
                }
            }
           );
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
                    操作日志列表
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