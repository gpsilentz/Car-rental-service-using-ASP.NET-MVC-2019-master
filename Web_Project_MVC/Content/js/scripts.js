$(document).ready(function () {
    $(".product-item").hover(
        function () {
            $(this).addClass('shadow').css('cursor', 'pointer');
        }, function () {
            $(this).removeClass('shadow');
        }
    );
});

$(document).ready(function () {
    var container = $('.bootstrap-iso form').length > 0 ? $('.bootstrap-iso form').parent() : "body";

    var date = new Date();
    date.setDate(date.getDate());

    $('.datepicker').datepicker({
        format: 'mm/dd/yyyy',
        container: container,
        todayHighlight: true,
        autoclose: true,
        startDate: date
    });
});

function OnSuccess(response) {
    if (response.DisplayError) {
        new Noty({
            type: 'error',
            layout: 'topRight',
            theme: 'metroui',
            text: response.Message,
            timeout: 5000
        }).show();
    }

    if (response.DisplaySuccess) {
        new Noty({
            type: 'success',
            layout: 'topRight',
            theme: 'metroui',
            text: response.Message,
            timeout: 5000
        }).show();
    }

    if (response.NeedToRedirect)
        window.location.href = response.RedirectLink;
}