$(function () {

    if ($("#child-organizations tbody tr").length > 0) {
        $("#child-organizations").dataTable({
            info: false,
            paging: false,
            searching: false
        });
    }

    $("a[href='#users']").on("shown.bs.tab", function (e) {
        if ($("#organization-users tbody tr").length > 0) {
            $("#organization-users").dataTable({});
        }
    });

    



    $(document)
        .bind("keyup", "d", function() { $(".nav-tabs a[href=\"#organization-detail\"]").tab("show"); })
        .bind("keyup", "u", function () { $(".nav-tabs a[href=\"#users\"]").tab("show"); })
        .bind("keyup", "e", function () { $("#edit").click(); })
        .bind("keyup", "a", function() { $("#add").click(); });

});