<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="DomainList.aspx.cs" Inherits="Dorado.VWS.Admin.DomainList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .listTable
        {
            line-height: 25px;
            font-size: 11px;
            text-align: center;
        }

        .listTable tr.altrow
        {
            background-color: #E6F2FF; /*隔行变色  */
        }
        .red
        {
            color: Red;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            $("table.listTable tr:nth-child(odd)").addClass("altrow");
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    IDC：<asp:DropDownList ID="ddlIDC" runat="server" OnSelectedIndexChanged="ddlIDC_SelectedIndexChanged"
        AutoPostBack="True">
    </asp:DropDownList>
    环境：<asp:DropDownList ID="ddlEnvironment" runat="server" OnSelectedIndexChanged="ddlEnvironment_SelectedIndexChanged"
        AutoPostBack="True">
    </asp:DropDownList>
    域名：<asp:DropDownList ID="ddlDomain" runat="server" OnSelectedIndexChanged="ddlDomain_SelectedIndexChanged"
        AutoPostBack="True">
    </asp:DropDownList>
    <a href="AddDomain.aspx">添加新域名</a>
    <asp:GridView ID="gvwDomains" runat="server" CssClass="listTable" AutoGenerateColumns="False"
        OnRowCommand="gvwDomains_RowCommand" OnRowDataBound="gvwDomains_RowDataBound">
        <Columns>
            <%-- <asp:BoundField DataField="IDCName" HeaderText="IDC名称">
            <ItemStyle Width="80px" />
            </asp:BoundField>--%>
            <asp:BoundField DataField="Environment" HeaderText="环境">
                <ItemStyle Width="80px" />
            </asp:BoundField>
            <asp:BoundField DataField="DomainName" HeaderText="域名">
                <ItemStyle Width="180px" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="同步类型">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("SyncType") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%#  ((int)Eval("SyncType")==1)?"普通同步":"简单同步"  %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="60px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="HTML压缩">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("HtmlCompress") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <input type="checkbox" disabled="disabled" <%# (bool)Eval("HtmlCompress")?"checked":"" %> />
                </ItemTemplate>
                <ItemStyle Width="60px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="JSCSS压缩">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("JsCssCompress") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <input type="checkbox" disabled="disabled" <%# (bool)Eval("JsCssCompress")?"checked":"" %> />
                </ItemTemplate>
                <ItemStyle Width="60px" />
            </asp:TemplateField>
            <asp:BoundField DataField="WinServiceName" HeaderText="windows service名称" />
            <asp:BoundField DataField="IISSiteName" HeaderText="IIS站点名称" />
            <asp:BoundField DataField="CacheUrl" HeaderText="缓冲url" />
            <asp:BoundField DataField="DomainType" HeaderText="域名类型" />
            <asp:TemplateField HeaderText="操作">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DmainName") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:HiddenField ID="hdfDomainId" Value='<%#Eval("DomainId")%>' runat="server" />
                    <asp:HiddenField ID="hdfDomainName" Value='<%#Eval("DomainName")%>' runat="server" />
                    <a href='EditDomain.aspx?ID=<%#Eval("DomainId")%>'>修改</a>
                    <asp:LinkButton ID="lbtnSetEnable" CommandName='<%# ((bool)Eval("Enable"))?"setDisable":"setEnable" %>'
                        CommandArgument='<%#Eval("DomainId")%>' Text='<%# ((bool)Eval("Enable"))?"停用":"启用" %>'
                        runat="server" CssClass='<%# ((bool)Eval("Enable"))?"":"red" %>'><%# ((bool)Eval("Enable"))?"停用":"启用" %></asp:LinkButton>
                    <asp:LinkButton ID="LbtndelDomain" CommandName="delDomain" CommandArgument='<%#Eval("DomainId")%>'
                        runat="server" OnClientClick="return confirm('此删除要删除该域名下的所有服务器，确定要删除吗？')">删除</asp:LinkButton>
                    <a href='ServerList.aspx?ID=<%#Eval("DomainId")%>'>服务器</a> <a href='domainpermission.aspx?ID=<%#Eval("DomainId")%>'>
                        管理员</a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>