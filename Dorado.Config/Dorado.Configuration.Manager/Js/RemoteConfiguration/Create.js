$("#formCancel").click(function () {
    parent.$('#w-create').window("close");
})

$("#formSubmit").click(function () {
    $("#formCreate").submit();
})

$("#application").focus(function () {
    $(this).val('').removeClass("watermark");
}).blur(function () {
    if ($(this).val() == "General" || $(this).val().length == 0)
        $(this).val("General").addClass("watermark");
})