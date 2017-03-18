$(function() {

    $(document)
        .bind("keyup", "a", function () { $("#add").click(); })
        .bind("keyup", "n", function () { $("#Name").focus(); })
        .bind("keyup", "p", function () { $("#Phone").focus(); })
        .bind("keyup", "1", function () { $("#AddressLine1").focus(); })
        .bind("keyup", "2", function () { $("#AddressLine2").focus(); })
        .bind("keyup", "t", function () { $("#City").focus(); })
        .bind("keyup", "s", function () { $("#State").focus(); })
        .bind("keyup", "o", function () { $("#PostalCode").focus(); })
    ;

    $("#add").on("click", addNewPayeeType);
    $("#payeetypes").on("click", ".remove", removePayeeType);
});

function removePayeeType(e) {
    $(e.target)
        .closest(".row")
        .remove();

    e.preventDefault();
}

function addNewPayeeType(e) {
    var data = { index: getNextIndex() };
    var template = $("#payeeTypeTmpl").html();

    $("#payeetypes").append(Mustache.to_html(template, data));

    var form = $("#payeeEditor")
            .removeData("validator")
            .removeData("unobtrusiveValidation");

    $.validator.unobtrusive.parse(form);

    e.preventDefault();
}

function getNextIndex() {
    var item = $("#payeetypes div.row:last input[type='hidden'][name='SelectedTypes.Index']");
    if (item.length > 0) {
        return 1 + parseInt($(item[0]).attr("value"));
    }
    return 0;
}