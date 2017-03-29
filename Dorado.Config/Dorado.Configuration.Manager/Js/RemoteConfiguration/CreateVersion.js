function getQuery(name) {
    if (!location.search)
        return '';
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = location.search.substr(1).match(reg);
    if (r != null) {
        return unescape(r[2]);
    }
}

$("#formCancel").click(function () {
    parent.$('#w-createversion').window("close");
})

$("#formSubmit").click(function () {
    if (!$("#sectionName").validatebox("isValid"))
        return false;
    if (!$("#application").validatebox("isValid"))
        return false;
    if (!$("#major").validatebox("isValid"))
        return false;
    if (!$("#fileName").validatebox("isValid"))
        return false;
    $("#formCreateVersion").submit();
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