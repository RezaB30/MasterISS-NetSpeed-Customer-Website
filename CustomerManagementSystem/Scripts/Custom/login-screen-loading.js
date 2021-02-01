$('form').submit(function () {
    EnableLoadingScreen();
});
$(document).ajaxStart(function () {
    EnableLoadingScreen();
})
$(document).ajaxStop(function () {
    DisableLoadingScreen();
});

function EnableLoadingScreen() {
    $('.loading-screen').removeClass("d-none");
}
function DisableLoadingScreen() {
    $('.loading-screen').addClass("d-none");
}

function GetCustomCaptcha(url) {
    $.ajax({
        url: url,
        method: 'POST',
        complete: function (data, status) {
            if (status == "success") {
                var content = data.responseText;
                var hasGRecaptcha = $('form').find('.g-recaptcha');
                if (hasGRecaptcha.length == 0) {
                    $('#customCaptcha').parent('div').remove();
                    $('.custom-captcha').parent('div').html(content);

                } else {
                    $('.g-recaptcha').parent('div').after(content);
                    $('.g-recaptcha').parent('div').remove();
                }

            }
        }
    });
}