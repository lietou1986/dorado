function getQuery(name) {
    if (!location.search)
        return '';
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = location.search.substr(1).match(reg);
    if (r != null) {
        return unescape(r[2]);
    }
}
$("#formSubmit").click(function () {
    if (!$("#sectionName").validatebox("isValid"))
        return false;
    if (!$("#application").validatebox("isValid"))
        return false;
    if (!$("#major").validatebox("isValid"))
        return false;
    if (!$("#fileContent").validatebox("isValid"))
        return false;
    var params = {
        SectionName: $('#sectionName').combobox('getValue'),
        Application: $('#application').combobox('getValue'),
        Major: $("#major").val(),
        CurrentSetting: $("#CurrentSetting").val(),
        FileContent: encodeURIComponent($("#fileContent").val())
    };
    $.post("/RemoteConfiguration/EditVersion", $.param(params), function (result) {
        CreateCallBack(result);
    })
})
function CreateCallBack(result) {
    if (result == 1) {
        $('#dd .dd-content').html("修改版本成功!");
        $('#dd').dialog({
            modal: true, title: "提示",
            buttons: [{
                text: 'Ok',
                iconCls: 'icon-ok',
                handler: function () {
                    parent.RefreshConfigList();
                    $('#dd').dialog("close");
                    $("#formCancel").click();
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
$("#formCancel").click(function () {
    parent.$('#w-editversion').window("close");
})
$('#sectionName').combobox({
    url: '/RemoteConfiguration/GetAllConfigs',
    valueField: 'id',
    textField: 'text',
    formatter: function (rec) {
        if (rec.id == -1)
            return '<span style="background-color:red">' + rec.text + '</span>';
        else
            return rec.text;
    },
    onChange: function (newValue, oldValue) {
        $('#application').combobox({
            url: '/RemoteConfiguration/GetApplications?sectionName=' + newValue,
            valueField: 'id',
            textField: 'text',
            onLoadSuccess: function () {
            }
        });
    },
    onLoadSuccess: function () {
        $('#sectionName').combobox('setValue', getQuery("sectionName"));
        $('#application').combobox('setValue', getQuery("application"));
        $('#major').numberspinner('setValue', getQuery("major"));
    }
});
