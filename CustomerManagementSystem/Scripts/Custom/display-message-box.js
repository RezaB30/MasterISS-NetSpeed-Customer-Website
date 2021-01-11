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