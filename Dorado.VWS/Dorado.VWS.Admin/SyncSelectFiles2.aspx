<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SyncSelectFiles2.aspx.cs" Inherits="Vancl.IC.VWS.SiteApp.SyncSelectFiles2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>同步-选择文件</title>
    <link rel="stylesheet" href="Styles/zTreeStyle.css" type="text/css" />
    <script type="text/javascript" src="Scripts/jquery.ffpager.js"></script>
    <script type="text/javascript" src="Scripts/json.js"></script>
    <script type="text/javascript" src="Scripts/jquery.ztree-2.6.min.js"></script>
    <script type="text/javascript">
        var historyJson = null;
        var selectedJson = [];
        var domainID = 0;
        var selectPath="";

        var zNodes = [];
        var setting = {
            checkable: false,
            addDiyDom: addDiyDom,
            async: true,
            asyncUrl: "Handler/FileListHandler.ashx", //获取节点数据的URL地址
            asyncParam: ["id"]
        };

        $(document).ready(function () {
            $.getJSON("/Handler/SyncSelectFiles.ashx", { "action": "getdomains" }, function (json) {
                $(json).each(function (i) {
                    $("#domains").append("<option value='" + this.id + "'>" + this.name + "</option>");
                });
            });

            $("#domains").change(function () {
                // 当域名ID不为0时，刷新树
                domainID = $("#domains").val();
                if (domainID != '0') {
                    setting.asyncParamOther = {
                        "permission": "0",
                        "domainid": domainID
                    };

                    $("#tree").zTree(setting, zNodes);
                }
            });

        });

        function historyPageIndexChange(pageIndex,pageSize)
        {
            var begin=(pageIndex-1)*pageSize;
            $.getJSON("/Handler/VersionFilesHandler.ashx", { "begin": begin, "end": begin+pageSize, "domainid": domainID, "filepath": selectPath }, function (json) {
                $("#tableHistory").html("");
                if (json.Version.length > 0) {
                    historyJson = json;
                    // 表头
                    $("#tableHistory").append("<tr><th>文件名</th><th>创建人</th><th>创建时间</th><th>备注</th><th>操作</th></tr>");
                    // 循环输出
                    for (i = 0; i < json.Version.length; i++) {
                        $("#tableHistory").append("<tr><td>" + json.Version[i].VersionPath + "</td><td>" + json.Version[i].Creator + "</td><td>" + ConvertDatetime(json.Version[i].CreateTime) + "</td><td>" + json.Version[i].Description + "</td><td><a href='javascript:;' onclick='SelectFile(" + i + ")'>选择</a></td><tr>");
                    }
                }
                else {
                    $("#tableHistory").append("<tr><td style='text-align:center; color:#FF0000;'>目前没有历史文件记录</td></tr>");
                }

                $("#pagerHistory").ffpager({"pageIndex":pageIndex, "pageSize":pageSize, "totalRecord":json.Count, "pageIndexChange":historyPageIndexChange});
            });
         }

        //在节点上显示自定义内容
        function addDiyDom(treeId, treeNode) {
            var aObj = $("#" + treeNode.tId + "_a");
            var html = " <a href='javascript:;' onclick='BindHistoryTable(\"" + treeNode.id.replace(/\\/ig, "\\\\")  + "\")'>历史</a>";
            aObj.append(html);
        };

        //绑定文件历史信息表格
        function BindHistoryTable(filePath) {
            selectPath=filePath;
            historyPageIndexChange(1,10);
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
                $("#submit").removeAttr("disabled");
                $("#tableSelected").append("<tr><th>文件名</th><th>创建人</th><th>创建时间</th><th>备注</th><th>操作</th></tr>");
                $(selectedJson).each(function (i) {
                    $("#tableSelected").append("<tr><td>" + this.VersionPath + "</td><td>" + this.Creator + "</td><td>" + ConvertDatetime(this.CreateTime) + "</td><td>" + this.Description + "</td><td><a href='javascript:;' onclick='RemoveFile(" + i + ")'>移除</a></td><tr>");
                });
            }
            else {
                $("#submit").attr("disabled", "disabled");
                $("#tableSelected").append("<tr><td style='text-align:center; color:#FF0000;'>尚未选择任何文件</td></tr>");
            }
        }

        //判断文件是否已选择
        function IsExist(obj) {
            var i = selectedJson.length;
            while (i--)
                if (selectedJson[i].VersionFileId == obj.VersionFileId) return true;
            return false;
        }

        //提交选择的文件
        function SubmitData() {
            var data = $.toJSON(selectedJson);
            $.getJSON("/Handler/SyncSelectFiles.ashx", { "action": "submit", "data": data }, function (json) {
                
            });
        }

        // 转化日期函数
        function ConvertDatetime(obj) {
           var str=obj.replace(/[/]/ig, "");
           return eval("new " + str).toLocaleString();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="text-align: center">
        <table border="0" align="center" cellpadding="5" cellspacing="0" class="listTable">
            <tr>
                <td align="center" colspan="2" style="font-size:14px; font-weight:bolder;">同步-选择文件</td>
            </tr>
            <tr>
                <td align="left" valign="top" style="width:30%;">
                    域名：<select id="domains"><option value="0">请选择</option></select>
                    <ul id="tree" class="tree" style="overflow:auto;"></ul>
                </td>
                <td align="left" valign="top">
                    <div style="text-align:center; font-weight:bolder;">历史文件记录</div>
                    <table id="tableHistory" class="listTable"></table>
                    <div id="pagerHistory"></div>
                    <div id="tip" style="text-align:center; color:#FF0000;"></div>
                    <hr />
                    <div style="text-align:center; font-weight:bolder;">已选择文件</div>
                    <table id="tableSelected" class="listTable"></table>
                    <div id="pagerSelected"></div>
                    <br />
                    <div style="text-align:center;">
                        <input type="button" id="submit" onclick="SubmitData()" disabled="disabled" value="回 滚" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
