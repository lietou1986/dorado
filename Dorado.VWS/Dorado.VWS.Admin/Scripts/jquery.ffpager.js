/*
* JQuery ffpager 0.1
*
* Copyright (c) 2011 fufeng
*
* Licensed same as jquery - under the terms of either the MIT License or the GPL Version 2 License
*
*/

(function ($) {
    $.fn.ffpager = function (setting /*pageIndex, pageSize, totalRecord, pageIndexChange*/) {
        this.totalRecord = setting.totalRecord;

        var _this = this;
        var str = ["", "", "", ""];
        var totalPage = Math.ceil(this.totalRecord / setting.pageSize);
        if (totalPage == 0) totalPage = 1;

        if (totalPage > 1) {
            str[1] = "<a href='javascript:;' id='_ffpager_prePage'>上一页</a>";
            str[2] = "<a href='javascript:;' id='_ffpager_nextPage'>下一页</a>"
        }
        else {
            str[1] = "上一页";
            str[2] = "下一页";
        }
        if (setting.pageIndex == 1) {
            str[0] = "首页";
            str[1] = "上一页";
        }
        else {
            str[0] = "<a href='javascript:;' id='_ffpager_firstPage'>首页</a>";
            str[1] = "<a href='javascript:;' id='_ffpager_prePage'>上一页</a>";
        }
        if (setting.pageIndex == totalPage) {
            str[2] = "下一页";
            str[3] = "尾页";
        }
        else {
            str[2] = "<a href='javascript:;' id='_ffpager_nextPage'>下一页</a>";
            str[3] = "<a href='javascript:;' id='_ffpager_lastPage'>尾页</a>";
        }

        str.push("[" + setting.pageIndex + "/" + totalPage + "]");

        this.html(str.join(" "));
        var page = 0;
        var links = this.children("a");
        $(links).each(function (page) {
            switch ($(this).attr("id")) {
                case "_ffpager_firstPage":
                    page = 1;
                    break;
                case "_ffpager_prePage":
                    page = setting.pageIndex - 1;
                    break;
                case "_ffpager_nextPage":
                    page = setting.pageIndex + 1;
                    break;
                case "_ffpager_lastPage":
                    page = totalPage;
                    break;
            }
            $(this).bind("click", function () {
                setting.pageIndex = page;
                setting.pageIndexChange(setting.pageIndex, setting.pageSize);
                _this.ffpager(setting);
            });
        });
    }
})(jQuery);