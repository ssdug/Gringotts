$(function () {

    //keybindings
    $(document)
        .bind("keyup", "a", function () { $("#add").click(); })
        .bind("keyup", "c", function () { $("#clear").click(); });
});