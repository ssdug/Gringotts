$(function() {
    //keybindings
    $(document)
        .bind("keyup", "d", function () { $(".nav-tabs a:first").tab("show"); })
        .bind("keyup", "e", function () { $("#edit")[0].click(); })
        .bind("keyup", "p", function () { $("#receipt")[0].click(); })
        .bind("keyup", "x", function () { $("#expense")[0].click(); });

    $(document).on("click", ".void-receipt", function (e) {
        if (e != null) {
            e.preventDefault();
        }

        var dialogTmpl = $("#void-receipt-confirm-tmpl").html();

        BootstrapDialog.show({
            title: "Warning",
            message: Mustache.to_html(dialogTmpl, {id: $(this).data("id")}),
            type: BootstrapDialog.TYPE_WARNING,
            spinicon: "glyphicon glyphicon-cog",
            buttons:[
                {
                    label: "Save",
                    cssClass: "btn-primary",
                    autospin: true,
                    action: function (dialog) {
                        var form = $("#voidExpenseEditor");
                        form.validate();
                        if (form.valid()) {
                            form.submit();
                        }
                    }
                },
                {
                    label: "Cancel",
                    action: function(dialog) {
                        dialog.close();
                }
                }],
            onshown: function(dialog) {
                var form = $("#voidExpenseEditor");
                $.validator.unobtrusive.parse(form);
                form.validateBootstrap();
            }
        });
    });
});
