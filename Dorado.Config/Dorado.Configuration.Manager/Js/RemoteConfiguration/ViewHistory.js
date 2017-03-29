function GetQuery(name) {
    if (!location.search)
        return '';
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = location.search.substr(1).match(reg);
    if (r != null) {
        return unescape(r[2]);
    }
}

$(function () {
    //        $("#easyui-tabs-0").bind('contextmenu', function (e) {
    //            $('#mm').menu('show', {
    //                left: e.pageX,
    //                top: e.pageY
    //            });
    //            return false;
    //        });
    $('#tt').datagrid({
        url: '/RemoteConfiguration/GetHistory',
        title: '配置文件列表',
        height: 'auto',
        fitColumns: true,
        queryParams: { SectionName: GetQuery("sectionName"), Application: GetQuery("application"), Major: GetQuery("major"), CurrentSetting: GetQuery("CurrentSetting") },
        sortName: 'SectionName',
        sortOrder: 'asc',
        columns: [[
					{ field: 'SectionName', title: '配置名称', width: 80, sortable: true,
					    formatter: function (value, rec, index) {
					        if (index == 0)
					            return value;
					        else
					            return "";
					    }
					},
					{ field: 'Application', title: '应用程序', width: 80, sortable: true },
					{ field: 'Major', title: '主版本', width: 60, align: 'right', sortable: true },
					{ field: 'Minor', title: '次版本', width: 60, align: 'right', sortable: true },
					{ field: 'FileName', title: '文件名称', width: 250, sortable: true,
					    formatter: function (value, rec) {
					        var temp = new Array(' ');
					        temp.push('SectionName="' + rec.SectionName + '"');
					        temp.push(' ');
					        temp.push('Application="' + rec.Application + '"');
					        temp.push(' ');
					        temp.push('Major="' + rec.Major + '"');
					        temp.push(' ');
					        temp.push('Minor="' + rec.Minor + '"');
					        temp.push(' ');
					        temp.push('FileName="' + rec.FileName + '"');
					        temp.push(' ');
					        temp.push('DownloadUrl="' + rec.DownloadUrl + '"');
					        var item = temp.join(' ');
					        return '<a ' + item + '" href="#" class="config_view">' + value + '</a>';
					    }
					},
                    	{ field: 'OperatorID', title: '操作人ID', width: 60, align: 'right', sortable: true },
                    	{ field: 'OperateTime', title: '操作时间', width: 120, align: 'right', sortable: true },
					{ field: 'Operation', title: '配置操作', width: 100, align: 'left', sortable: false,
					    formatter: function (value, rec) {
					        var temp = new Array(' ');
					        temp.push('SectionName="' + rec.SectionName + '"');
					        temp.push(' ');
					        temp.push('Application="' + rec.Application + '"');
					        temp.push(' ');
					        temp.push('Major="' + rec.Major + '"');
					        temp.push(' ');
					        temp.push('Minor="' + rec.Minor + '"');
					        temp.push(' ');
					        temp.push('FileName="' + rec.FileName + '"');
					        temp.push(' ');
					        temp.push('DownloadUrl="' + rec.DownloadUrl + '"');
					        var item = temp.join(' ');
					        var result = '<a href="#" class="config_rollback"' + item + '>[应用此版本]</a> ';
					        return result;
					    }
					}
				]],
        toolbar: [{
            text: '刷新列表',
            iconCls: 'icon-reload',
            handler: function () {
                RefreshConfigList();
            }
        }, '-', {
            text: '返回',
            iconCls: 'icon-back',
            handler: function () {
                GoBack();
            }
        }],
        onLoadSuccess: function () {
            $(".config_view").bind("click", function () {
                ViewConfig(this);
            })
            $(".config_rollback").bind("click", function () {
                RollbackConfig(this);
            })
        },
        onHeaderContextMenu: function (e, field) {
            e.preventDefault();
            if (!$('#tmenu').length) {
                createColumnMenu();
            }
            $('#tmenu').menu('show', {
                left: e.pageX,
                top: e.pageY
            });
        }
    });
});
function RollbackConfig(e) {
    var item = $(e);
    $.get("/RemoteConfiguration/GetConfigContent?downloadurl=" + item.attr("downloadurl"), function (result) {
        CreateVersion(result, item);
    })
}

function CreateVersion(result, item) {
    var params = {
        SectionName: item.attr('sectionname'),
        Application: item.attr('application'),
        Major: item.attr('major'),
        FileContent: encodeURIComponent(result)
    };
    $.post("/RemoteConfiguration/EditVersion", $.param(params), function (result) {
        CreateCallBack(result);
    })
}
function CreateCallBack(result) {
    if (result == 1) {
        $('#dd .dd-content').html("回滚版本成功!");
        $('#dd').dialog({
            modal: true, title: "提示",
            buttons: [{
                text: 'Ok',
                iconCls: 'icon-ok',
                handler: function () {
                    parent.RefreshConfigList();
                    $('#dd').dialog("close");
                    parent.$('#tabs').tabs('select', '配置文件管理');
                }
            }]
        });
    }
    else if (result == 0) {
        $('#dd .dd-content').html("不是有效的配置文件!");
        $('#dd').dialog({
            modal: true, title: "提示",
            buttons: [{
                text: 'Error',
                title: "提示",
                iconCls: 'icon-error',
                handler: function () {
                    $('#dd').dialog("close");
                }
            }]
        });
    }
}
function ViewConfig(e) {
    var title = '';
    var item = $(e);
    title = item.attr("filename");
    var query = "sectionName=" + item.attr('sectionname') + "&application=" + item.attr('application') + "&major=" + item.attr('major') + "&downloadurl=" + item.attr("downloadurl");
    parent.$('#tabs').tabs('add', {
        title: '查看配置文件[' + title + ']',
        content: '<iframe scrolling="yes" frameborder="0"  src="/RemoteConfiguration/ViewConfig?source=history&' + query + '"  style="width:100%;height:100%;"></iframe>',
        iconCls: 'icon-reload',
        closable: true
    });
}

function RefreshConfigList() {
    $('#tt').datagrid("reload");
}
function GoBack() {
    parent.$("#tabs").tabs('select', '配置文件管理');
}
function createColumnMenu() {
    var tmenu = $('<div id="tmenu" style="width:100px;"></div>').appendTo('body');
    var fields = $('#tt').datagrid('getColumnFields');
    for (var i = 0; i < fields.length; i++) {
        $('<div iconCls="icon-ok"/>').html(fields[i]).appendTo(tmenu);
    }
    tmenu.menu({
        onClick: function (item) {
            if (item.iconCls == 'icon-ok') {
                $('#tt').datagrid('hideColumn', item.text);
                tmenu.menu('setIcon', {
                    target: item.target,
                    iconCls: 'icon-empty'
                });
            } else {
                $('#tt').datagrid('showColumn', item.text);
                tmenu.menu('setIcon', {
                    target: item.target,
                    iconCls: 'icon-ok'
                });
            }
        }
    });
}
