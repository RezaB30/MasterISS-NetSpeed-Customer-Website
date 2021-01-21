function GetSupportAttachments(url, supportId, downloadUrl) {
    $.ajax({
        url: url,
        method: 'POST',
        data: { supportId: supportId },
        complete: function (data, status) {
            if (status == "success") {
                //FileName , ServerName,StageId
                var files = data.responseJSON;
                for (var i = 0; i < files.length; i++) {
                    var html = '<a href="' + downloadUrl + '?supportId=' + supportId + '&fileName=' + files[i].ServerName + '" class="d-flex align-items-center text-primary text-hover-primary py-1">' +
                        '<span class="flaticon2-clip-symbol text-warning icon-1x mr-2">' + files[i].FileName + '</span></a>';

                    $('.support-file-content').each(function () {
                        var stage = $(this).attr("data-stage");
                        if (stage == files[i].StageId) {
                            $(this).append(html);
                        }
                    })
                }
            }
        }
    });
}
$('#support-attachment-id').click(function () {
    $('.upload-attachments').trigger("click");
});
$('.upload-attachments').change(function () {
    var items = "";
    //for (var i = 0; i < $(this).files.length; i++) {
    //    items += '<li>' + $(this).files[i].name + '</li>';
    //}
    var inp = document.getElementsByClassName('upload-attachments')[0];
    for (var i = 0; i < inp.files.length; ++i) {
        var name = inp.files.item(i).name;
        items += '<li class="list-item">' + name + '</li>';
    }
    $('.upload-attachment-list').html(items);
});