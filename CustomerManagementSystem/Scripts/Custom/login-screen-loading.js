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