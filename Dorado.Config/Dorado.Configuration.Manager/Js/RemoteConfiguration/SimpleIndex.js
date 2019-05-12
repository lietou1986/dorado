$(document).ready(function () {
    $('#tabs').tabs({
        tools: [{
            iconCls: 'icon-add',
            handler: function () {
                CreateConfig();
            }
        }]
    });
    GridLoad();
});

function RefreshConfigList() {
    $('#tt').datagrid("reload");
}

function DeleteSelectConfig() {
    var selectRows = $('#tt').datagrid("getSelections");
    if (selectRows.length == 0) {
        ConfirmMessage("请选择要删除的配置！");

        return;
    }
    $('#dd .dd-content').html("您确定要删除所有选中的配置吗?");
    $('#dd').dialog({
        modal: true, title: "删除确认",
        buttons: [{
            text: 'Ok',
            iconCls: 'icon-ok',
            handler: function () {
                for (var i = 0, j = selectRows.length; i <= j; i++) {
                    var sectionName = selectRows[i].SectionName;
                    $.get("/RemoteConfiguration/DeleteSection?sectionName=" + sectionName, function (result) {
                        if (result) {
                            $('#dd').dialog("close");
                            RefreshConfigList();
                        }
                    })
                }
            }
        }, {
            text: 'Cancel',
            iconCls: "icon-cancel",
            handler: function () {
                $('#dd').dialog('close');
            }
        }]
    });
}

function DeleteSelectApp() {
    var selectRows = $('#tt').datagrid("getSelections");
    if (selectRows.length == 0) {
        ConfirmMessage("请选择要删除的应用！");
        return;
    }
    $('#dd .dd-content').html("您确定要删除所有选中的应用吗?");
    $('#dd').dialog({
        modal: true, title: "删除确认",
        buttons: [{
            text: 'Ok',
            iconCls: 'icon-ok',
            handler: function () {
                alert('ok');
            }
        }, {
            text: 'Cancel',
            iconCls: "icon-cancel",
            handler: function () {
                $('#dd').dialog('close');
            }
        }]
    });
}
function CreateConfig() {
    $('#w-create').window({
        title: '新增配置',
        width: 350,
        modal: true,
        content: '<iframe scrolling="no" frameborder="0" src="/RemoteConfiguration/Create" style="width: 100%; height: 100%;"></iframe>',
        shadow: true,
        closed: false,
        height: 300
    });
}
function ViewConfig(e) {
    var title = '';
    var item = $(e);
    title = item.attr("filename");
    var query = "sectionName=" + item.attr('sectionname') + "&application=" + item.attr('application') + "&major=" + item.attr('major') + "&downloadurl=" + item.attr("downloadurl");
    $('#tabs').tabs('add', {
        title: '查看配置文件[' + title + ']',
        content: '<iframe scrolling="auto" frameborder="0"  src="/RemoteConfiguration/ViewConfig?' + query + '"  style="width:100%;height:100%"></iframe>',
        iconCls: 'icon-reload',
        closable: true
    });
}
function CreateConfigVersion(e) {
    var item = $(e);
    var query = "sectionName=" + item.attr('sectionname') + "&application=" + item.attr('application') + "&major=" + item.attr('major');
    $('#w-createversion').window({
        title: '新增版本',
        width: 400,
        modal: true,
        content: '<iframe scrolling="no" frameborder="0" src="/RemoteConfiguration/CreateVersion?' + query + '" style="width: 100%; height: 100%;"></iframe>',
        shadow: true,
        closed: false,
        height: 400
    });
}
function EditConfigVersion(e) {
    var item = $(e);
    var query = "sectionName=" + item.attr('sectionname') + "&application=" + item.attr('application') + "&major=" + item.attr('major') + "&downloadUrl=" + item.attr('downloadurl');
    $('#w-editversion').window({
        title: '修改版本',
        width: 600,
        modal: true,
        content: '<iframe scrolling="no" frameborder="0" src="/RemoteConfiguration/EditVersion?' + query + '" style="width: 100%;height: 100%;"></iframe>',
        shadow: true,
        closed: false,
        height: 500
    });
}

function DeleteConfig(e) {
    var item = $(e);
    var sectionName = item.attr('sectionname');
    $('#dd .dd-content').html('您确定要删除配置[' + sectionName + ']吗?');
    $('#dd').dialog({
        modal: true, title: "删除确认",
        buttons: [{
            text: 'Ok',
            iconCls: 'icon-ok',
            handler: function () {
                $.get("/RemoteConfiguration/DeleteSection?sectionName=" + encodeURIComponent(sectionName), function (result) {
                    if (result != "False") {
                        ConfirmMessage('删除配置[' + sectionName + ']成功！');
                    }
                })
            }
        }, {
            text: 'Cancel',
            iconCls: "icon-cancel",
            handler: function () {
                $('#dd').dialog('close');
            }
        }]
    });
}
function DeleteApp(e) {
    var item = $(e);
    var sectionName = item.attr('sectionname');
    var application = item.attr('application');
    $('#dd .dd-content').html("您确定要删除此应用吗?");
    $('#dd').dialog({
        modal: true, title: "删除确认",
        buttons: [{
            text: 'Ok',
            iconCls: 'icon-ok',
            handler: function () {
                $.get("/RemoteConfiguration/DeleteApplication?sectionName=" + encodeURIComponent(sectionName) + "&application=" + encodeURIComponent(application), function (result) {
                    if (result != "False") {
                        ConfirmMessage('删除配置[' + application + ']成功！');
                    }
                })
            }
        }, {
            text: 'Cancel',
            iconCls: "icon-cancel",
            handler: function () {
                $('#dd').dialog('close');
            }
        }]
    });
}
function ViewHistory(e) {
    var item = $(e);
    var query = "sectionName=" + item.attr('sectionname') + "&application=" + item.attr('application') + "&major=" + item.attr('major');
    $('#tabs').tabs('add', {
        title: '版本历史' + item.attr('filename'),
        content: '<iframe scrolling="yes" frameborder="0"  src="/RemoteConfiguration/ViewHistory?' + query + '"  style="width:100%;height:100%"></iframe>',
        iconCls: 'icon-reload',
        closable: true
    });
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

function CreateCallBack(result) {
    if (result == 1) {
        ConfirmMessage("新增配置成功!");
        $('#w-create').window("close");
    }
    else if (result == 0) {
        ConfirmMessage("不是有效的配置文件!");
    }
}

function GridLoad() {
    var toolbar = [{
        text: '新增配置',
        iconCls: 'icon-add',
        handler: function () {
            CreateConfig();
        }
    }, '-', {
        text: '刷新配置',
        iconCls: 'icon-reload',
        handler: function () {
            RefreshConfigList();
        }
    }];

    $('#tt').datagrid({
        url: '/RemoteConfiguration/GetAllLastVersion',
        title: '配置文件列表',
        height: 'auto',
        fitColumns: true,
        sortName: 'SectionName',
        sortOrder: 'asc',
        columns: [[
            { field: 'ck', checkbox: true },
            { field: 'Application', title: '应用程序', width: 200, sortable: true },
            {
                field: 'SectionName', title: '配置名称', width: 200, sortable: true,
                formatter: function (value, rec) {
                    var temp = new Array(' ');
                    temp.push('Application="' + rec.Application + '"');
                    temp.push(' ');
                    temp.push('SectionName="' + rec.SectionName + '"');
                    temp.push(' ');
                    temp.push('Major="' + rec.Major + '"');
                    temp.push(' ');
                    temp.push('Minor="' + rec.Minor + '"');
                    temp.push(' ');
                    temp.push('FileName="' + rec.FileName + '"');
                    temp.push(' ');
                    temp.push('DownloadUrl="' + rec.DownloadUrl + '"');
                    var item = temp.join(' ');
                    return '<a ' + item + '" href="#" class="config_view"><strong>' + value + '</a></strong>';
                }
            },
            { field: 'Major', title: '主版本', width: 60, align: 'right', sortable: true },
            { field: 'Minor', title: '次版本', width: 60, align: 'right', sortable: true },
            {
                field: 'Operation', title: '配置操作', width: 350, align: 'left', sortable: true,
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
                    var result = '<a href="#" class="config_history"' + item + '>[历史]</a> <a href="#" class="config_create"' + item + '>[新增版本]</a> <a href="#" class="config_edit"' + item + '>[修改版本]</a>';
                    if (rec.CanDelete == "True")
                        result += ' <a href="#" class="config_delete"' + item + '>[删除配置]</a>';
                    if (rec.CanDeleteApp == "True")
                        result += ' <a href="#" class="app_delete"' + item + '>[删除应用]</a>';
                    return result;
                }
            }
        ]],
        toolbar: toolbar,
        onLoadSuccess: function () {
            $("#easyui-tabs-0 .datagrid-body").bind('contextmenu', function (e) {
                $('#ctmenu').menu('show', {
                    left: e.pageX,
                    top: e.pageY
                });
                return false;
            });
            $(".config_view").bind("click", function () {
                ViewConfig(this);
            })
            $(".config_create").bind("click", function () {
                CreateConfigVersion(this);
            })
            $(".config_edit").bind("click", function () {
                EditConfigVersion(this);
            })
            $(".config_delete").bind("click", function () {
                DeleteConfig(this);
            })
            $(".config_history").bind("click", function () {
                ViewHistory(this);
            })
            $(".app_delete").bind("click", function () {
                DeleteApp(this);
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
}

function ConfirmMessage(message) {
    $('#dd .dd-content').html(message);
    $('#dd').dialog({
        modal: true, title: "提示",
        buttons: [{
            text: 'Ok',
            title: "提示",
            iconCls: 'icon-ok',
            handler: function () {
                RefreshConfigList();
                $('#dd').dialog("close");
            }
        }]
    });
}