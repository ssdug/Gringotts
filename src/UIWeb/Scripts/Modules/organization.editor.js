$(function () {

    $(document)
        .bind("keyup", "g", function () { $("#GroupName").focus(); })
        .bind("keyup", "n", function () { $("#Name").focus(); })
        .bind("keyup", "b", function () { $("#Abbreviation").focus(); })
        .bind("keyup", "p", function () { $("#Phone").focus(); })
        .bind("keyup", "f", function () { $("#FiscalContactSamAccountName").focus(); })
        .bind("keyup", "i", function () { $("#ITConactSamAccountName").focus(); })
        .bind("keyup", "1", function () { $("#AddressLine1").focus(); })
        .bind("keyup", "2", function () { $("#AddressLine2").focus(); })
        .bind("keyup", "t", function () { $("#City").focus(); })
        .bind("keyup", "s", function () { $("#State").focus(); })
        .bind("keyup", "o", function () { $("#PostalCode").focus(); });
});