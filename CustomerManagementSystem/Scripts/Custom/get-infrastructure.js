function GetValues(requestId, responseId, url) {
    var ID = $(requestId).val();
    var token = $("[name='__RequestVerificationToken']").val();
    ClearList(responseId);
    $(responseId).trigger('change');
    if (ID == '') {

    }
    else {
        DisableSelectList();
        $.ajax({
            dataType: 'json',
            method: 'POST',
            url: url,
            data: { code: ID, __RequestVerificationToken: token },
            complete: function (data, status) {
                if (status == "success") {
                    var results = data.responseJSON;
                    for (var i = 0; i < results.length; i++) {
                        $(responseId).append('<option value="' + results[i].value + '">' + results[i].text + '</option>');
                    }
                    EnableSelectList();
                }
            }
        });
    }
}
function DisableSelectList() {
    $('div').each(function () {
        $(this).find("select").attr("disabled", "disabled");
    })
}
function EnableSelectList() {
    $('div').each(function () {
        $(this).find("select").removeAttr("disabled");
    })
}
function ClearList(responseId) {    
    var container = $(responseId);
    container.find('option').each(function () {
        if ($(this).val() != '') {
            $(this).remove();
        }
    });
}