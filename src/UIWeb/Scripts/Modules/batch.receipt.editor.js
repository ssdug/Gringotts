$(function () {
    $(document)
    .bind("keyup", "w", function () { $("a.btn.btn-xs.pull-right:visible")[0].click(); })
    .bind("keyup", "h", function () { $("a.btn.btn-xs.pull-right:visible")[0].click(); })

    .bind("keyup", "t", function () { $("#ReceiptTypeId").focus(); })
    .bind("keyup", "x", function () { $("#ExpectedAmount").focus(); })
    .bind("keyup", "e", function () { $("#EffectiveDate").focus(); })

    .bind("keyup", "s", function () { $("#ReceiptSourceName").focus(); })
    .bind("keyup", "n", function () { $("#AccountName").focus(); })
    .bind("keyup", "o", function () { $("#Amount").focus(); })
    .bind("keyup", "r", function () { $("#ReceiptNumber").focus(); })
    .bind("keyup", "f", function () { $("#ReceivedFrom").focus(); })
    .bind("keyup", "c", function () { $("#Comments").focus(); })
    .bind("keyup", "l", function () { $("#ClientName").focus(); })
    .bind("keyup", "u", function () { $("#AccountId").focus(); })
    ;

    new BatchTransactionEditor({ container: "#batchReceiptEditor" }).listen();
});