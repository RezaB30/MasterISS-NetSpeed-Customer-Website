function DisplayResourceText(text) {
    $.ajax({
        url: '/Initialize/Init',
        method: 'POST',
        data: { text: text },
        complete: function (data, status) {
            try {
                if (status == "success") {
                    var display = data.responseJSON;
                    return display.displayText;
                }
            } catch (e) {
                console.log(e);
                return "???";
            }
        }
    })
}