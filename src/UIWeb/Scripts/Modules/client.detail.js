$(function() {
    $(document)
        .bind("keyup", "c", function() { $('.nav-tabs a[href="#client-info"]').tab('show'); })
        .bind("keyup", "t", function() { $('.nav-tabs a[href="#attorneys"]').tab('show'); })
        .bind("keyup", "g", function() { $('.nav-tabs a[href="#guardians"]').tab('show'); })
        .bind("keyup", "i", function () { $('.nav-tabs a[href="#identifiers"]').tab('show'); })
        .bind("keyup", "r", function () { $('.nav-tabs a[href="#restitution"]').tab('show'); })
        .bind("keyup", "a", function () { $("#add")[0].click(); });
});