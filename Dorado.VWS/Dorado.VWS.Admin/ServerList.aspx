<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ServerList.aspx.cs" Inherits="Dorado.VWS.Admin.ServerList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>服务器列表维护</title>
    <script language="javascript" type="text/javascript">
        $(
            function () {
                $("#chkall").click(
            function () {
                if ($(this).attr("checked")) {
                    $('.chkclass').attr('checked', true);
                } else {
                    $('.chkclass').attr('checked', null);
                }
            }
        );
            }
            );
        //函数说明：合并指定表格（表格id为_w_table_id）指定列（列数为_w_table_colnum）的相同文本的相邻单元格
        //参数说明：_w_table_id 为需要进行合并单元格的表格的id。如在HTMl中指定表格 id="data" ，此参数应为 #data
        //参数说明：_w_table_colnum 为需要合并单元格的所在列。为数字，从最左边第一列为1开始算起。
        function _w_table_rowspan(_w_table_id, _w_table_colnum) {
            var _w_table_firsttd = "";
            var _w_table_currenttd = "";
            var _w_table_SpanNum = 0;
            var _w_table_Obj = $(_w_table_id + " tr td:nth-child(" + _w_table_colnum + ")");

            _w_table_Obj.each(function (i) {
                if (i == 0) {
                    _w_table_firsttd = $(this);
                    _w_table_SpanNum = 1;

                } else {
                    _w_table_currenttd = $(this);

                    if (_w_table_firsttd.text() == _w_table_currenttd.text()) {
                        _w_table_SpanNum++;
                        _w_table_currenttd.hide(); //remove();
                        _w_table_firsttd.attr("rowSpan", _w_table_SpanNum);

                    } else {
                        _w_table_firsttd = $(this);
                        _w_table_SpanNum = 1;
                    }
                }
            });
        }

        //测试连接

        function TestConn(tagObj) {
            var tests = tagObj ? $(tagObj) : $(".testConn");
            $.each(tests, function (i) {
                $(this).html('<img src="styles/img/loading2.gif" />');
                $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "testConn", "ip": $(this).attr("ip") }, function (json) {
                    $(tests[i]).html(json.ret ? "连接成功" : "<font color='Red'>连接失败</font>");
                });
            });
        }
        function IsThereAnySelected() {
            return !$('.chkclass').is(":checked");
        }
        function GetAllTheCheckedServers() {
            var s = '';
            $('input[name="chkserver"]:checked').each(function () {
                s += $(this).val() + ',';
            });
            $("#MainContent_selectedServers").val(s);
        }
        //更新客户端

        function UpdateClient(ip) {
            $.getJSON("/Handler/GetInfoHandler.ashx", { "action": "updateClient", "ip": ip, "domainID": $("#ddlDomain").val() });
            alert("更新服务命令已发出！");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="selectedServers" runat="server" />
    <div style="text-align: center;">
        <p class="tip">
            逻辑关系：域名->同步源->同步宿。
        </p>
        <div style="text-align: left; margin: 5px;">
            环境：<asp:DropDownList ID="DdlEnvironment" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlEnvironment_SelectedIndexChanged">
            </asp:DropDownList>
            域名：<asp:DropDownList ID="ddlDomain" runat="server" ClientIDMode="Static" AutoPostBack="True"
                OnSelectedIndexChanged="ddlDomain_SelectedIndexChanged" />
            <a href="javascript:;" onclick=" TestConn(); ">测试所有服务器连接</a>如果提示“连接失败”，表示该主机尚未安装几步系统后台服务程序，或者该主机已经下架。<br />
            通常情况下，由系统部负责安装后台服务程序ClientService，如果情况紧急，可以自行下载安装<a href="Help.aspx">进入帮助下面下载</a>
        </div>
        <table id="tab" border="0" align="center" cellpadding="5" cellspacing="0" class="listTable">
            <tr>
                <td align="center" colspan="10">
                    <b>服务器列表</b>
                </td>
            </tr>
            <asp:Repeater ID="rptServer" runat="server" OnItemCommand="rptServer_ItemCommand"
                OnItemDataBound="rptServer_ItemDataBound">
                <HeaderTemplate>
                    <tr>
                        <th align="center">
                            环境
                        </th>
                        <th align="center">
                            域名
                        </th>
                        <th align="center">
                            服务器类型
                        </th>
                        <th align="center">
                            IP
                        </th>
                        <th align="center">
                            根目录
                        </th>
                        <th align="center">
                            主机名
                        </th>
                        <th align="center">
                            服务版本
                        </th>
                        <th align="center">
                            状态
                        </th>
                        <th align="center">
                            <input type="checkbox" class="chkclass" id='chkall' value='chkall' />
                        </th>
                        <th align="center" style="width: auto;">
                            操作
                        </th>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td runat="server" align="left" id="tdEnvironment" title='<%#Eval("DomainName")%>'>
                            <asp:Panel ID="Panel1" runat="server" Visible='<%#(int) Eval("DomainId") != 0%>'>
                                <%#Eval("Environment")%>
                            </asp:Panel>
                        </td>
                        <td runat="server" id="tdDomain" align="left" title='<%#Eval("DomainName")%>'>
                            <asp:Panel runat="server" Visible='<%#(int) Eval("DomainId") != 0%>'>
                                <%#Eval("DomainName")%>
                                <a href='AddServer.aspx?IDCID=<%#Eval("IdcId")%>&DomainID=<%#Eval("DomainId")%>'>添加服务器</a>
                                <asp:HiddenField ID="hdfDomainId" Value='<%#Eval("DomainId")%>' runat="server" />
                                <asp:HiddenField ID="hdfDomainName" Value='<%#Eval("DomainName")%>' runat="server" />
                                <a href="Role.aspx?id=<%#Eval("DomainId")%>">设置权限</a>
                                <asp:LinkButton ID="lbtnSetEnable" CommandName="setEnable" CommandArgument='<%#Eval("DomainId")%>'
                                    runat="server" ForeColor="Red">启用</asp:LinkButton>
                                <asp:LinkButton ID="lbtnSetDisEnable" CommandName="setDisable" CommandArgument='<%#Eval("DomainId")%>'
                                    runat="server">停用</asp:LinkButton>
                            </asp:Panel>
                        </td>
                        <td align="left" style="<%#Eval("DeleteFlag").ToString()=="False"?((string)Eval("ServerTypeName")=="同步源"?"background-color:greenyellow": ""):"background-color:#CDCDCD"%>">
                            <%#Eval("ServerTypeName")%>
                        </td>
                        <td align="left" style="<%#Eval("DeleteFlag").ToString()=="False"?((string)Eval("ServerTypeName")=="同步源"?"background-color:greenyellow": ""):"background-color:#CDCDCD"%>">
                            <%#Eval("IP")%><%#Eval("IsAdvanced").ToString()=="True"?"<br />(预上线)":""%><div id="<%#"ip" + Container.ItemIndex%>"
                                class="testConn" ip='<%#Eval("IP")%>'>
                            </div>
                        </td>
                        <td align="left" style="<%#Eval("DeleteFlag").ToString()=="False"?((string)Eval("ServerTypeName")=="同步源"?"background-color:greenyellow": ""):"background-color:#CDCDCD"%>">
                            <%#Eval("Root")%>
                        </td>
                        <td align="left" style="<%#Eval("DeleteFlag").ToString()=="False"?((string)Eval("ServerTypeName")=="同步源"?"background-color:greenyellow": ""):"background-color:#CDCDCD"%>">
                            <%#Eval("HostName")%>
                        </td>
                        <td align="left" style="<%#Eval("DeleteFlag").ToString()=="False"?((string)Eval("ServerTypeName")=="同步源"?"background-color:greenyellow": ""):"background-color:#CDCDCD"%>">
                            <%#Eval("ClientVersion")%>
                        </td>
                        <td align="center" title="<%#Eval("LastHeartBeatDate")%>" style="<%#Eval("DeleteFlag").ToString()=="False"?((string)Eval("ServerTypeName")=="同步源"?"background-color:greenyellow": ""):"background-color:#CDCDCD"%>">
                            <%#((bool)Eval("ServerStatus")==false)?"<font color='Red' size='4px'>●</font>":"<font size='4px' color='green'>●</font>"%>
                        </td>
                        <td align="center" style="<%#Eval("DeleteFlag").ToString()=="False"?((string)Eval("ServerTypeName")=="同步源"?"background-color:greenyellow": ""):"background-color:#CDCDCD"%>">
                            <input type="checkbox" name="chkserver" class="chkclass" id='<%#Eval("ServerId")%>'
                                value='<%#Eval("ServerId")%>' />
                        </td>
                        <td align="center" style="<%#Eval("DeleteFlag").ToString()=="False"?((string)Eval("ServerTypeName")=="同步源"?"background-color:greenyellow": ""):"background-color:#CDCDCD"%>">
                            <asp:Panel runat="server" Visible='<%#(int) Eval("ServerId") != 0%>'>
                                <a href='EditServer.aspx?id=<%#Eval("ServerId")%>'>修改</a>
                                <asp:LinkButton ID="lbtnStop" CommandName="stopServer" CommandArgument='<%#Eval("ServerId")%>'
                                    runat="server" OnClientClick="return confirm('谨慎操作\n确定要停用吗？')" Visible='<%#Eval("DeleteFlag").ToString()=="True"?false:true%>'>停用</asp:LinkButton>
                                <asp:LinkButton ID="LinkStart" CommandName="startServer" CommandArgument='<%#Eval("ServerId")%>'
                                    runat="server" OnClientClick="return confirm('谨慎操作\n确定要启用吗？')" Visible='<%#Eval("DeleteFlag").ToString()=="True"?true:false%>'>启用</asp:LinkButton>
                                <a href="javascript:;" onclick='<%#"TestConn(\"#ip" + Container.ItemIndex + "\")"%>'>
                                    测试连接</a> <a href="javascript:;" style="display: <%# GetUpdateDisplayStyle() %>;"
                                        onclick='<%#"UpdateClient(\"" + Eval("IP") + "\")"%>'>更新</a>
                            </asp:Panel>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <div id="divpager" align="center">
            <asp:LinkButton ID="lbtnFirst" runat="server" OnClick="lbtnFirst_Click">首页</asp:LinkButton>
            &nbsp;<asp:LinkButton ID="lbtnPre" runat="server" OnClick="lbtnPre_Click">上一页</asp:LinkButton>
            &nbsp;<asp:LinkButton ID="lbtnNext" runat="server" OnClick="lbtnNext_Click">下一页</asp:LinkButton>
            &nbsp;<asp:LinkButton ID="lbtnLast" runat="server" OnClick="lbtnLast_Click">尾页</asp:LinkButton>
            [
            <asp:Label ID="lblCurrentPage" runat="server" Text="Label"></asp:Label>
            /
            <asp:Label ID="lblTotalPage" runat="server" Text="Label"></asp:Label>
            ]
        </div>
        <br />
        <asp:Button ID="btnStartSelectedServers" runat="server" Text="启用选中的服务器" OnClick="btnStartSelectedServers_Click"
            OnClientClick='if (IsThereAnySelected()) { alert("请选择服务器"); return false; } else if (confirm("确定更新吗？")) {GetAllTheCheckedServers(); return true;} ' />
        <asp:Button ID="btnStopSelectedServers" runat="server" Text="停用选中的服务器" OnClick="btnStopSelectedServers_Click"
            OnClientClick='if (IsThereAnySelected()) { alert("请选择服务器"); return false;} else if (confirm("确定更新吗？")) {GetAllTheCheckedServers(); return true; }' />
        <input type="button" id="inputUpdateClient" runat="server" value="更新客户端服务" onclick='if($("#ddlDomain").val() == "0") {alert("请选择域名");} else if (confirm("确定更新吗？")) UpdateClient(""); ' />
    </div>
</asp:Content>