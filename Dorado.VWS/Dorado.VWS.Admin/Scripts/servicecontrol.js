/// <reference path="/Scripts/jquery-1.7.1.js" />

function serviceCtrl(obj, domain) {
    $(obj).attr('disabled', 'disabled');
    var isIIS = $(obj).attr('svc') == 'iisCtrl'
    var cls = isIIS ? 'iisctrl' : 'svcctrl';
    var isStart = $(obj).attr('cmd') == "start";
    var isReStart = $(obj).attr('cmd') == "restart";
    //alert(isIIS + "_" + isStart);
    var checkedbox = $(':checkbox:checked:not(.checkall)')
    //console.log(checkedbox);
    //alert(checkedbox.length);

    //     checkedbox.each(function (i) {
    //
    //         //console.log(i);
    //         var serverip = $(this).attr('serverip');
    //         var serverid = $(this).attr('serverid');

    //         serverOpreate(domain,serverip, serverid, isIIS, isStart);
    //         //console.log( $(this).parents('tr').find('.ip'));
    //     });

    //     for (var i = 0; i < checkedbox.length; i++) {
    //         var serverip = $(checkedbox[i]).parents('td').attr('serverip');
    //         var serverid = $(checkedbox[i]).parents('td').attr('serverid');

    //
    //         //setTimeout('serverOpreate(\'' + domain + '\', \'' + serverip + '\', ' + serverid + ', ' + isIIS + ', ' + isStart + ');', 500);
    //         serverOpreate(domain, serverip, serverid, isIIS, isStart);
    //     }
    if (checkedbox.length < 1) {
        alert('请先选择要操作的服务器！');
        $(obj).removeAttr('disabled');
        return;
    }
    var i = 0;

    opreate();
    function opreate() {
        if (i >= checkedbox.length) {
            $(obj).removeAttr('disabled');
            return;
        }
        var serverip = $(checkedbox[i]).parents('td').attr('serverip');
        var serverid = $(checkedbox[i]).parents('td').attr('serverid');
        if (isReStart) {
            restart(domain, serverip, serverid, isIIS, opreate);
        }
        else {
            serverOpreate(domain, serverip, serverid, isIIS, isStart, opreate);
        }
        i++

        //         if (i < checkedbox.length) {
        //             setTimeout(opreate, 2000);
        //         }
        //         else {
        //             $(obj).removeAttr('disabled');
        //             return;
        //         }
    }
    //$(obj).removeAttr('disabled');
}

function serverOpreate(domain, serverip, serverid, isIIS, isStart, callback) {
    var cls = isIIS ? 'iisctrl' : 'svcctrl';

    //console.log(domain + ' ' + serverip + " " + (isIIS ? 'IIS站点' : 'WinServeice') + (isStart ? '启动' : '停止'));

    var status = $('span[serverid="' + serverid + '"].' + cls).text();
    if (isStart && status != "已停止") {
        getStatus(serverid, callback);
        return;
    }
    if (!isStart && status != "已启动") {
        getStatus(serverid, callback);
        return;
    }

    $('span[serverid="' + serverid + '"].' + cls).text(isStart ? 'starting…' : 'stoping...');

    $.ajax({
        url: "/Handler/GetInfoHandler.ashx",
        dataType: 'json',
        //async: false,
        global: false,
        data: {
            "action": "serviceCtrl",
            "serverid": serverid,
            "domainname": domain,
            "isStart": isStart,
            "isIis": isIIS
        },
        beforeSend: function () {
        },
        success: function (json) {
            //getStatus(serverid, callback);
            //setTimeout(startGetStatus(serverid, callback), 5000);
        },
        complete: function (json) {
            setTimeout(startGetStatus(serverid, callback), 5000);
            //            if (callback && typeof(callback)!='string') {
            //                callback();
            //            }
            //            //            $.ajaxSetup({
            //                async: true
            //            });
        }
    })

    //delay(3);
    //setTimeout('getStatus(' + serverid + ');', 2000);
}

function restart(domain, serverip, serverid, isIIS, callback) {
    serverOpreate(domain, serverip, serverid, isIIS, false, function () {
        serverOpreate(domain, serverip, serverid, isIIS, true, callback);
    }
     );
}
function startGetStatus(serverid, callback) {
    return function () {
        getStatus(serverid, callback);
    }
}
function init() {
    $('tr').each(function () {
        var serverid = $(this).attr('serverid');
        if (serverid) {
            //console.log(serverid);
            getStatus(serverid)
        }
    })
}

function getStatus(serverid, callback) {
    //console.log("更新状态" + serverid);

    $.ajax({
        url: "/Handler/GetInfoHandler.ashx",
        dataType: 'json',
        //        async: true,
        global: false,
        data: { "action": "getserverstatus",
            "serverid": serverid
        },
        beforeSend: function () {
        },
        success: function (json) {
            //console.log(serverid);
            //console.log(json);
            if (json.ret) {
                //                           $(this).find('.iisctrl').text(json.result[0]);
                //                           $(this).find('.svcctrl').text(json.result[1]);

                $('span[serverid="' + serverid + '"].iisctrl').text(json.result[0]);
                if (json.result[0] == '已启动') {
                    $('span[serverid="' + serverid + '"].iisctrl').css('color', 'green');
                }
                else {
                    $('span[serverid="' + serverid + '"].iisctrl').css('color', 'red');
                }

                $('span[serverid="' + serverid + '"].svcctrl').text(json.result[1]);
                if (json.result[1] == '已启动') {
                    $('span[serverid="' + serverid + '"].svcctrl').css('color', 'green');
                }
                else {
                    $('span[serverid="' + serverid + '"].svcctrl').css('color', 'red');
                }
            }
            else {
                $('span[serverid="' + serverid + '"].iisctrl').text('获取状态失败');
                $('span[serverid="' + serverid + '"].iisctrl').css('color', 'red');
                $('span[serverid="' + serverid + '"].svcctrl').text('获取状态失败');
                $('span[serverid="' + serverid + '"].svcctrl').css('color', 'red');
                //alert();
            }
        },
        complete: function (json) {
            //            $.ajaxSetup({
            //                async: true
            //            });
            if (callback && typeof (callback) != 'string') {
                callback();
            }
        }
    })
}