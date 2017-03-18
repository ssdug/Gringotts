$(function() {

    //keybindings
    $(document)
        .bind("keyup", "d", function () { $(".nav-tabs a:first").tab("show"); })
        .bind("keyup", "s", function () { $(".nav-tabs a:last").tab("show"); })
        .bind("keyup", "l", function () { $(".nav-tabs a:last").tab("show"); })
        .bind("keyup", "e", function () { $("#edit")[0].click(); })
        .bind("keyup", "a", function () { $("#add")[0].click(); })
        .bind("keyup", "c", function () { $("#clear")[0].click(); });
});