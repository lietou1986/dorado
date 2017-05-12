<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ServerControl.aspx.cs" Inherits="Dorado.VWS.Admin.ServerControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>同步-选择文件</title>
    <script type="text/javascript" src="Scripts/servicecontrol.js"></script>
    <script type="text/javascript">
        $(function () {

            init();
            $('.changeall').click(
                function () {
                    //console.log($(this));

                    var domain = $('#<%=ddlDomain.ClientID %> option:selected').text();
                    serviceCtrl(this, domain);
                    //init();

                }
            );
            $('span.iisctrl,span.svcctrl').click(
                function () {

                    var domain = $('#<%=ddlDomain.ClientID %> option:selected').text();
                    var isIIS = $(this).attr('class') == 'iisctrl';
                    var isStart = $(this).text() != "已启动";
                    var serverip = $(this).attr('serverip');
                    var serverid = $(this).attr('serverid');

                    serverOpreate(domain, serverip, serverid, isIIS, isStart);
                    //init();

                }
            );

            $('.checkall').click(
                function () {
                    if ($(this).attr('checked')) {
                        //alert('true');
                        $(':checkbox').attr('checked', true);
                    } else {
                        $(':checkbox').attr('checked', null);
                    }
                }
            );
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: center;">
        <p class="tip">
            控制相应机器的站点或服务的启用状态。
        </p>
        <div class="tableTitle">
            服务控制</div>
        <hr />
        环境：
        <asp:DropDownList ID="ddlEnvironment" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlEnvironment_SelectedIndexChanged">
        </asp:DropDownList>
        域名：<asp:DropDownList ID="ddlDomain" ClientIDMode="Static" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="ddlDomain_SelectedIndexChanged">
        </asp:DropDownList>
        <br />
        <input type="button" svc="iisCtrl" cmd="start" class="changeall" value="启动站点" />
        <input type="button" svc="iisCtrl" cmd="stop" class="changeall" value="停止站点" />
        <input type="button" svc="iisCtrl" cmd="restart" class="changeall" value="重启站点" />&nbsp;&nbsp;
        &nbsp;&nbsp;
        <input type="button" svc="svcCtrl" cmd="start" class="changeall" value="启动服务" />
        <input type="button" svc="svcCtrl" cmd="stop" class="changeall" value="停止服务" />
        <input type="button" svc="svcCtrl" cmd="restart" class="changeall" value="重启服务" /><br />
        <asp:ListView runat="server" ID="lvServer" OnItemCommand="lvServer_ItemCommand" OnItemDataBound="lvServer_DataBound">
            <EmptyDataTemplate>
                <asp:Label runat="server" ForeColor="Red">没有相关数据</asp:Label></EmptyDataTemplate>
            <LayoutTemplate>
                <table border="0" cellspacing="0" cellpadding="5" align="center" style="width: 50%;"
                    class="listTable">
                    <tr>
                        <th>
                            全选
                            <input type="checkbox" class="checkall" />
                        </th>
                        <th>
                            主机名
                        </th>
                        <th>
                            主机IP
                        </th>
                        <th>
                            IIS操作
                        </th>
                        <th>
                            WinService操作
                        </th>
                        <th>
                            压缩操作
                        </th>
                    </tr>
                    <tr runat="Server" id="itemPlaceholder">
                    </tr>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr serverid="<%#Eval("ServerId")%>">
                    <td serverid="<%#Eval("ServerId")%>" serverip=" <%#Eval("IP")%>">
                        <input type="checkbox" id="checkserver" />
                    </td>
                    <td>
                        <%#Eval("HostName")%>
                    </td>
                    <td class="ip">
                        <%#Eval("IP")%>
                    </td>
                    <td>
                        <span class="iisctrl" serverid="<%#Eval("ServerId")%>" serverip="<%#Eval("IP")%>"
                            style="cursor: pointer"></span>
                        <!--                        1<asp:Label runat="Server" ID="lError" Visible="false" ForeColor="Red">无法连接到客户端</asp:Label>
                        <asp:LinkButton runat="server" ID="lbIis" CommandName="iisCtrl" CommandArgument='<%#Eval("IP")%>'></asp:LinkButton>
-->
                    </td>
                    <td>
                        <span class="svcctrl" serverid="<%#Eval("ServerId")%>" serverip="<%#Eval("IP")%>"
                            style="cursor: pointer"></span>
                        <!--                        2
                        <asp:LinkButton runat="server" ID="lbOther" CommandName="otherCtrl" CommandArgument='<%#Eval("IP")%>'></asp:LinkButton>
-->
                    </td>
                    <td>
                        <%if (CurUserIsAdmin)
                          { %>
                        <asp:LinkButton runat="server" ID="lbHtmlCompress" CommandName="htmlCompressCtrl"
                            CommandArgument='<%#Eval("IP")%>'></asp:LinkButton>
                        <asp:LinkButton runat="server" ID="lbJSCssCompress" CommandName="jsCssCompressCtrl"
                            CommandArgument='<%#Eval("IP")%>'></asp:LinkButton>
                        <%} %>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
        <asp:Label runat="Server" ID="tip" class="tipMsg"></asp:Label>
    </div>
</asp:Content>