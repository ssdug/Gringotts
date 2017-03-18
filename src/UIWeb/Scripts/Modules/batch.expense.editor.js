$(function () {
    //keybindings
    $(document)
    .bind("keyup", "w", function () {$("a.btn.btn-xs.pull-right:visible")[0].click();})
    .bind("keyup", "h", function () { $("a.btn.btn-xs.pull-right:visible")[0].click(); })

    .bind("keyup", "t", function () {$("#ExpenseTypeId").focus();})
    .bind("keyup", "p", function () {$("#PayeeName").focus();})
    .bind("keyup", "x", function () { $("#ExpectedAmount").focus(); })
    .bind("keyup", "e", function () {$("#EffectiveDate").focus();})
    .bind("keyup", "m", function () { $("#Memo").focus(); })

    .bind("keyup", "a", function () {$("#ExpenseCategoryName").focus();})
    .bind("keyup", "n", function () {$("#AccountName").focus();})
    .bind("keyup", "o", function () { $("#Amount").focus(); })
    .bind("keyup", "c", function () { $("#Comments").focus(); })
    .bind("keyup", "l", function () { $("#ClientName").focus(); })
    .bind("keyup", "u", function () { $("#AccountId").focus(); })
    ;

    function revalidate() {
        var form = $("form")
        .removeData("validator")
        .removeData("unobtrusiveValidation");

        $.validator.unobtrusive.parse(form);
        form.validateBootstrap();
    }

    $(document).on("form.change", revalidate);

    new BatchTransactionEditor({ container: "#batchExpenseEditor" }).listen();
});