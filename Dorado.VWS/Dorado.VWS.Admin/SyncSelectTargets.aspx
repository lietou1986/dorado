<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SyncSelectTargets.aspx.cs" Inherits="Dorado.VWS.Admin.SyncSelectTargets" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>同步-选择宿主同步</title>
    <script src="My97DatePicker/WdatePicker.js" type="text/javascript"> </script>
    <script type="text/javascript" src="Scripts/servicecontrol.js"></script>
    <script type="text/javascript">
        $(
            function () {
                $.ajaxSetup({
                    beforeSend: null,
                    complete: null
                });
                init();

                $('.changeall').click(
                function () {
                    var domain = $('#<%=hdfDomain.ClientID%>').val();
                    serviceCtrl(this, domain);
                    //init();
                }
            );

                $("#<%=BtnSync.ClientID%>").attr("disabled", true);
                //全选，或者全不选
                $('.checkall').click(
                    function () {
                        if ($(this).attr('checked')) {
                            //alert('true');
                            $(':checkbox').attr('checked', true);
                        } else {
                            $(':checkbox').attr('checked', null);
                        }
                        if ($(':checkbox:checked:not(.checkall)').length == 0) {
                            $("#<%=BtnSync.ClientID%>").attr("disabled", true);
                        } else {
                            $("#<%=BtnSync.ClientID%>").attr("disabled", false);
                        }
                    }
                );

                //checkbox变更时，设置“同步”按钮可用性
                $("input[type='checkbox']:not(.checkall)'").change(
                    function () {
                        //alert($(".ckbSync:checked").length);
                        if ($(':checkbox:checked:not(.checkall)').length == 0) {
                            $("#<%=BtnSync.ClientID%>").attr("disabled", true);
                        } else {
                            $("#<%=BtnSync.ClientID%>").attr("disabled", false);
                        }
                    }
            );

                $("#aSyncTargets").click(
                    function () {
                        $("#trSyncTarget1").show();
                        $("#trSyncTarget2").show();
                        $(".trSyncTargets").show();
                        $(".trScheduleTask").hide();
                    }
            );

                $("#aScheduleTask").click(
                    function () {
                        $("#trSyncTarget1").hide();
                        $("#trSyncTarget2").hide();
                        $(".trSyncTargets").hide();
                        $(".trScheduleTask").show();
                    }
            );

                $("#trSyncTarget1").show();
                $("#trSyncTarget2").show();
                $(".trSyncTargets").show();
                $(".trScheduleTask").hide();
            }
    );

        function check() {
            if ($("#<%=TxtDescription.ClientID%>").val() == '') {
                alert("请填写同步原因");
                return false;
            }
            return true;
        }

        function checkschedule() {
            if ($("#<%=TxtDescription.ClientID%>").val() == '') {
                alert("请填写同步原因");
                return false;
            }
            if ($("#<%=txtScheduleTime.ClientID%>").val() == '') {
                alert("请选择计划同步时间");
                return false;
            }
            return true;
        }

        function getsyncresult() {
            var hasTaskRunning = false;
            var taskid = $("#<%=HfTaskid.ClientID%>").val();
            if (taskid != '0') {
                $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "synctask", "taskid": taskid }, function (data) {
                    //同步结果
                    $("#syncResult").html(data.Msg);
                    $.each(data.Subtasks, function (i, n) {
                        var serverid = n.ServerId;
                        var status = n.Status;
                        $("input:hidden[value=" + serverid + "]").siblings("#targetstatus").html(status);
                        $("input:hidden[value=" + serverid + "]").parent().siblings().find(".ckbSync").find(':checkbox').hide().attr("disabled", true);

                        if (status == '同步中') {
                            hasTaskRunning = true;
                            $("#<%=BtnSync.ClientID%>").attr("disabled", "disabled");
                            $("#<%=BtnCancel.ClientID%>").attr("disabled", "disabled");
                        } else {
                            $("#<%=BtnSync.ClientID%>").removeAttr("disabled");
                            $("#<%=BtnCancel.ClientID%>").removeAttr("disabled");
                        }
                    });

                    // 任务完成时，显示同步完成，并设置按钮状态
                    //if (data.Status != "Running") {
                    if (!hasTaskRunning) {
                        $("#loading").hide();
                        $("#<%=BtnSync.ClientID%>").attr("disabled", "disabled");
                        if (data.Status != "Suspend") {
                            $("#<%=BtnCancel.ClientID%>").attr("disabled", "disabled");
                            $(".ckbSync").hide();
                            $("#chkall").hide();
                        }

                        switch (data.Status) {
                            case "Succeed":
                                alert("同步成功");
                                break;
                            case "Failed":
                                alert("同步失败");
                                break;
                            case "Rollback":
                                alert("同步回滚成功");
                                break;
                            case "RollbackFailed":
                                alert("同步回滚失败");
                                break;
                            case "Suspend":
                                break;
                            default:
                                $("#loading").show();
                                setTimeout("getsyncresult()", 2000);
                                break;
                        }
                    } else {
                        $("#loading").show();
                        setTimeout("getsyncresult()", 2000);
                    }
                });
            }
        }

        //        function serviceCtrl(index, ip, servicename, domainname, isStart, isIis) {
        //            $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "serviceCtrl", "ip": ip, "servicename": servicename, "domainname": domainname, "isStart": isStart, "isIis": isIis }, function (json) {
        //                if (json.ret) {
        //                    var objID = (isIis ? "#iis" : "#other") + index;
        //                    $(objID).text(isStart ? $(objID).text().replace("启动", "停止") : $(objID).text().replace("停止", "启动"));
        //                    $(objID).removeAttr("onclick");
        //                    $(objID).one("click", function () {
        //                        serviceCtrl(index, ip, servicename, domainname, !isStart, isIis);
        //                    });
        //                }
        //            });
        //        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hdfDomain" runat="server" />
    <div style="text-align: center">
        <p class="tip">
            <lable style="font-weight: bolder; color: Red">注意：</lable>
            站点的dll文件可能会有延迟。如在高峰期同步，建议先停止部分服务器站点，再同步这些服务器。
        </p>
        <div id="loading" style="display: none; text-align: center; color: #0000FF;">
            <img alt="loading" src="styles/img/loading2.gif" style="width: 25px; height: 25px" /><span
                style="font-size: x-large;">同步任务处理中……</span></div>
        <div class="tableTitle">
            同步-选择宿主同步</div>
        <table border="0" align="center" cellpadding="5" cellspacing="0" style="width: 50%;"
            class="listTable">
            <tr id="trdomain">
                <th align="right" valign="top" style="width: 10%;">
                    添加列表
                </th>
                <td align="left" style="width: 90%;">
                    <asp:Literal ID="LtlAddFiles" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <th align="right" valign="top">
                    删除列表
                </th>
                <td align="left">
                    <asp:Literal ID="LtlDelFiles" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <th align="right" valign="top">
                    同步结果
                </th>
                <td align="left">
                    <asp:HiddenField ID="HfTaskid" runat="server" />
                    <div id="syncResult">
                    </div>
                </td>
            </tr>
            <tr>
                <th align="right" valign="top">
                    同步原因
                </th>
                <td align="left">
                    <asp:TextBox ID="TxtDescription" runat="server" Height="56px" MaxLength="128" TextMode="MultiLine"
                        Width="90%"></asp:TextBox>
                </td>
            </tr>
        </table>
        <table border="0" align="center" cellpadding="5" cellspacing="0" style="width: 50%;"
            class="listTable">
            <tr>
                <td align="left" colspan="6">
                    <a href="#" id="aSyncTargets">同步</a>
                </td>
            </tr>
            <tr id="trSyncTarget1">
                <th align="center">
                    <input type="checkbox" class="checkall" />
                </th>
                <th align="center">
                    IP
                </th>
                <th align="center">
                    机器名
                </th>
                <th align="center">
                    状态
                </th>
                <asp:Panel runat="server" ID="pTH" Visible="false">
                    <th align="center">
                        站点操作（IIS或Tomcat）
                    </th>
                    <th align="center">
                        WinService操作（Shell命令或JAR包）
                    </th>
                </asp:Panel>
            </tr>
            <asp:Repeater ID="RptServers" runat="server">
                <ItemTemplate>
                    <tr class="trSyncTargets" serverid="<%#Eval("ServerId")%>">
                        <td align="center" serverid="<%#Eval("ServerId")%>" serverip=" <%#Eval("IP")%>">
                            <%--<input class='ckbSync' id="ckbSync" type="checkbox"
                        />--%>
                            <asp:CheckBox runat="server" ID="ckbSync" class="ckbSync" />
                        </td>
                        <td align="center" style="width: 50px">
                            <%#Eval("IP")%><%#Eval("IsAdvanced").ToString()=="True"?"<br />(预上线)":""%>
                        </td>
                        <td align="left">
                            <%#Eval("HostName")%>
                        </td>
                        <td align="center">
                            <asp:HiddenField ID="HfServerId" runat="server" Value='<%#Eval("ServerId")%>' />
                            <div id='targetstatus'>
                                <%#Eval("Status")%></div>
                        </td>
                        <asp:Panel runat="Server" OnLoad="pTD_Load">
                            <td>
                                <span class="iisctrl" serverid="<%#Eval("ServerId")%>" serverip="<%#Eval("IP")%>"
                                    style="cursor: pointer"></span>
                            </td>
                            <td>
                                <span class="svcctrl" serverid="<%#Eval("ServerId")%>" serverip="<%#Eval("IP")%>"
                                    style="cursor: pointer"></span>
                            </td>
                        </asp:Panel>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
            <tr id="tr1">
                <th align="center">
                    <input type="checkbox" class="checkall" />
                </th>
                <th align="center">
                </th>
                <th align="center">
                </th>
                <th align="center">
                </th>
                <asp:Panel runat="server" ID="pTH1" Visible="false">
                    <th align="center">
                        <input type="button" svc="iisCtrl" cmd="start" class="changeall" value="启动" style="margin: 2px;" />
                        <input type="button" svc="iisCtrl" cmd="restart" class="changeall" value="重启" style="margin: 2px;" />
                        <input type="button" svc="iisCtrl" cmd="stop" class="changeall" value="停止" style="margin: 2px;" />
                    </th>
                    <th align="center">
                        <input type="button" svc="svcCtrl" cmd="start" class="changeall" value="启动" style="margin: 2px;" />
                        <input type="button" svc="svcCtrl" cmd="restart" class="changeall" value="重启" style="margin: 2px;" />
                        <input type="button" svc="svcCtrl" cmd="stop" class="changeall" value="停止" style="margin: 2px;" />
                    </th>
                </asp:Panel>
            </tr>
            <tr id="trSyncTarget2">
                <td align="right" colspan='<%=pTH.Visible ? 6 : 4%>'>
                    <asp:Button ID="BtnSync" TabIndex="0" OnClientClick="return check();" runat="server"
                        Text="同 步" OnClick="BtnSync_Click" />
                    <asp:Button ID="BtnCancel" runat="server" Text="撤 销" OnClick="BtnCancel_Click" />
                </td>
            </tr>
            <tr id="trScheduleTaskLink" runat="server">
                <td align="left" colspan="5">
                    <a href="#" id="aScheduleTask">计划任务</a>
                </td>
            </tr>
            <tr id="trScheduleTask" runat="server" class="trScheduleTask">
                <td align="right" colspan="4">
                    <input id="txtScheduleTime" class="Wdate" type="text" onfocus="WdatePicker({dateFmt:'yyyy-MM-dd HH:mm:ss',minDate:'%y-%M-%d {%H+1}'})"
                        style="width: 200px" runat="server" />
                    <asp:Button ID="BtnScheduleSync" runat="server" Text="计划任务" OnClick="BtnScheduleSync_Click"
                        OnClientClick="return checkschedule();" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>