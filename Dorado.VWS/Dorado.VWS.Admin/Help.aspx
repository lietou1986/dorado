<%@ Page Title="帮助页面" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Help.aspx.cs" Inherits="Dorado.VWS.Admin.Help" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div align="center" style="margin-top: 10px">
        <div style="width: 90%; margin-top: 20px; border-bottom: 1px solid #000000" align="left">
            <span class="help">系统介绍</span><span class="helpen">&nbsp;&nbsp;About System </span>
        </div>
        <div style="width: 90%; margin-top: 5px;" align="left">
            <span class="helpcon">同步系统用于线上服务器文件分发、记录同步任务等场景。<br />
                本系统对各个线上应用的文件做统一处理，及时、高效的分发文件，保障了线上文件一致性。 </span>
        </div>
        <div style="width: 90%; margin-top: 10px; border-bottom: 1px solid #000000" align="left">
            <span class="help">帮助指南</span><span class="helpen">&nbsp;&nbsp;Help Service </span>
        </div>
        <div style="width: 90%; margin-top: 5px;" align="left">
            <span class="helpcon"><a href="readme/同步系统二期使用手册.docx">使用手册</a>&nbsp;&nbsp; <a href="readme/同步系统二期维护手册.docx">
                维护手册</a>&nbsp;&nbsp; <a href="readme/20111111 功能申请模板1.xltx">功能申请模板</a> </span>
        </div>
        <div style="width: 90%; margin-top: 10px; border-bottom: 1px solid #000000" align="left">
            <span class="help">后台服务</span><span class="helpen">&nbsp;&nbsp;Windows Service </span>
        </div>
        <div style="width: 90%; margin-top: 5px;" align="left">
            <span class="helpcon">同步系统的运行，需要在所有相关主机上安装一个名为ClientService的Windows服务，如果是新加的主机，需要安装。<br />
                安装方法：1、下载下面的文件并解压 2、复制到目标主机C:\clientservice2\目录下 3、以管理员权限运行下载的.bat文件<br />
                验证结果：1、查看目标主机的ClientService服务是否已经启动 2、在“<a href="ServerList.aspx">运维管理-服务器</a>”页面，对该主机进行连接测试。<br />
                点击后面的链接即可下载安装包:<a href="readme/update.zip">同步系统后台服务安装包</a> </span>
        </div>
    </div>
</asp:Content>