function GetQuery(name) {
    if (!location.search)
        return '';
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = location.search.substr(1).match(reg);
    if (r != null) {
        return unescape(r[2]);
    }
}

$(document).ready(function () {
    var styleList = new Array("ide-eclipse", "ide-kdev", "ide-codewarrior", "matlab", "peachpuff", "rand01", "vim", "zellner", "berries-dark", "acid", "berries-light", "bright", "emacs");
    var random = Math.ceil(Math.random() * 14) - 1
    $("pre.xml").snippet("xml", { style: styleList[random], transparent: false, showNum: true });
    if (GetQuery("source") && GetQuery("source").length >= 0)
        $("#btnEdit").hide();
    else {
        $("#btnEdit").click(function () {
            $("pre.xml").slideUp(500);
            $("#fileContent").slideDown(500);
            $(".config-edit").show();
            $(this).hide();
        })
        $("#btnCancelEdit").click(function () {
            $("pre.xml").slideDown(500);
            $("#fileContent").slideUp(500);
            $("#btnEdit").show();
            $(".config-edit").hide();
        })
        $("#btnSaveEdit").click(function () {
            if (!$("#fileContent").validatebox("isValid"))
                return false;
            var params = {
                SectionName: GetQuery("sectionName"),
                Application: GetQuery("application"),
                Major: GetQuery("major"),
                FileContent: encodeURIComponent($("#fileContent").val())
            };
            $.post("/RemoteConfiguration/EditConfig", $.param(params), function (result) {
                CreateCallBack(result);
            })
        })
    }
})

function CreateCallBack(result) {
    if (result == 1) {
        $('#dd .dd-content').html("修改配置成功!");
        $('#dd').dialog({
            modal: true, title: "提示",
            buttons: [{
                text: 'Ok',
                iconCls: 'icon-ok',
                handler: function () {
                    $("#btnCancelEdit").click();
                    $("pre.xml").remove();
                    $("#fileContainer").html('<pre class="xml"></pre>');
                    $("pre.xml").text($("#fileContent").val()).snippet("xml", { style: "random", transparent: false, showNum: true });
                    parent.RefreshConfigList();
                    $('#dd').dialog("close");
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