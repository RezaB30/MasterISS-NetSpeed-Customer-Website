function EditPagedList() {
    var content = "";
    $('.pagination li a').addClass("btn btn-icon btn-sm border-0 btn-hover-primary mr-2 my-1");
    $('.pagination li').each(function () {
        var has = $(this).hasClass('active');
        if (has) {
            $(this).children('a').addClass('active');
        }
        content += $(this).html();
    });
    $('.pagination-container').remove();
    $('#paged').html(content);
}