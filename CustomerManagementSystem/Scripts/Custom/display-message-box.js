var success = $('.generic-success-message').text();
var error = $('.generic-error-message').text();
if (success) {
    Swal.fire({
        title: success,
        icon: "success",
        buttonsStyling: true,
        confirmButtonText: "Tamam"
    });
} else {
    if (error) {
        Swal.fire({
            title: error,
            icon: "error",
            buttonsStyling: true,
            confirmButtonText: "Tamam"
        });
    }
}


//$('#quick-search-form').change(function () {
//    alert("sad");
//    //setTimeout(function () {
//    //    $.ajax({
//    //        url: navigator.language + '/Home/QuickSearch',
//    //        data: {
//    //            query: _query
//    //        },
//    //        dataType: 'html',
//    //        success: function (res) {
//    //            _hasResult = true;
//    //            _hideProgress();
//    //            KTUtil.addClass(_target, _resultClass);
//    //            KTUtil.setHTML(_resultWrapper, res);
//    //            _showDropdown();
//    //            KTUtil.scrollUpdate(_resultWrapper);
//    //        },
//    //        error: function (res) {
//    //            _hasResult = false;
//    //            _hideProgress();
//    //            KTUtil.addClass(_target, _resultClass);
//    //            KTUtil.setHTML(_resultWrapper, '<span class="font-weight-bold text-muted">???</div>');
//    //            _showDropdown();
//    //            KTUtil.scrollUpdate(_resultWrapper);
//    //        }
//    //    });
//    //}, 1000);
//});