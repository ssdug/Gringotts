$(function() {
    $(document)
        .bind("keyup", "a", function() { $("#Amount").focus(); })
        .bind("keyup", "e", function() { $("#EffectiveDate").focus(); })
        .bind("keyup", "c", function() { $("#ExpenseCategoryName").focus(); })
        .bind("keyup", "t", function() { $("#ExpenseTypeId").focus(); })
        .bind("keyup", "p", function() { $("#PayeeName").focus(); })
        .bind("keyup", "m", function () { $("#Memo").focus(); });

    new PayeeEditor({ container: "#payeeEditor" }).listen();
    new ExpenseCategoryEditor({ container: "#expenseCategoryEditor" }).listen();

    var form = $("#expenseEditor")
            .removeData("validator")
            .removeData("unobtrusiveValidation");

    $.validator.unobtrusive.parse(form);

    form.on("click", "#save", function (e) {
        e && e.preventDefault();
        confirm()
            .done(function(response) {
                if (response) {
                    form[0].submit();
                }
            });
    });
});

function ExpenseCategoryEditor(options) {
    var self = this;
    var container = $(options.container);

    options.remoteUrl = decodeURIComponent(container.data("remote-url"));

    this.source = new Bloodhound({
        remote: {
            url: options.remoteUrl,
            filter: options.filter || function (res) { return res.Items; }
        },
        datumTokenizer: options.datumTokenizer || function (datum) {
            return Bloodhound.tokenizers.whitespace(datum.Name);
        },
        queryTokenizer: Bloodhound.tokenizers.whitespace
    });

    this.addTypeahead = function () {
        $("input[type='text']", container)
        .typeahead({}, {
            name: "ExpenseCategoryEditor",
            displayKey: "Name",
            source: this.source.ttAdapter(),
            templates: {
                empty: ["<div class=\"empty-message\">",
                        "<p>No Matches Found</p>",
                        "</div>"
                ].join("\n"),
                suggestion: function (context) {
                    return Mustache.render("<p><strong>{{Name}}</strong></p>", context);
                }
            }
        })
        .change(this.clearCategory);
    }

    this.validate = function (value, element) {
        var id = parseInt($("input[type=hidden]", $(element).closest(".row")).val());

        if (_.isFinite(id) && _.isString(value)) {
            $(element).closest(".form-group")
                .removeClass("has-error");
            return true;
        } else {
            $(element).closest(".form-group")
                .addClass("has-error");
            return false;
        }
    };

    this.selectCategory = function (e, item) {
        $("input[type=hidden]:first", $(e.target).closest(".row"))
             .val(item.Id);
    };

    this.clearCategory = function (e) {
        $("input[type=hidden]:first", $(e.target).closest(".row"))
            .val("");
    }

    this.listen = function() {
        self.source.initialize();
        self.addTypeahead();
        $.validator.unobtrusive.adapters.addBool("category");
        $.validator.addMethod("category", this.validate);
        container.on("typeahead:selected", this.selectCategory);
    };
}

function PayeeEditor(options) {
    var self = this;
    var container = $(options.container);

    options.remoteUrl = decodeURIComponent(container.data("remote-url"));
    
    this.source = new Bloodhound({
        remote: {
            url: options.remoteUrl,
            filter: options.filter || function (res) { return res.Items; }
        },
        datumTokenizer: options.datumTokenizer || function (datum) {
            return Bloodhound.tokenizers.whitespace(datum.Name);
        },
        queryTokenizer: Bloodhound.tokenizers.whitespace
    });

    this.addTypeahead = function () {
        $("input[type='text']", container)
        .typeahead({}, {
            name: "PayeeEditor",
            displayKey: "Name",
            source: this.source.ttAdapter(),
            templates: {
                empty: ["<div class=\"empty-message\">",
                        "<p>No Matches Found</p>",
                        "</div>"
                ].join("\n"),
                suggestion: function (context) {
                    return Mustache.render("<p><strong>{{Name}}</strong><br/><small>{{DisplayAddress}}</small></p>", context);
                }
            }
        })
        .change(this.clearPayee);
    }

    this.validate = function (value, element) {
        var id = parseInt($("input[type=hidden]", $(element).closest(".row")).val());

        if (_.isFinite(id) && _.isString(value)) {
            $(element).closest(".form-group")
                .removeClass("has-error");
            return true;
        } else {
            $(element).closest(".form-group")
                .addClass("has-error");
            return false;
        }
    };

    this.selectPayee = function (e, item) {
        $("input[type=hidden]:first", $(e.target).closest(".row"))
             .val(item.Id);
    };

    this.clearPayee = function (e) {
        $("input[type=hidden]:first", $(e.target).closest(".row"))
            .val("");
    }

    this.listen = function() {
        self.source.initialize();
        self.addTypeahead();
        $.validator.unobtrusive.adapters.addBool("payee");
        $.validator.addMethod("payee", this.validate);
        container.on("typeahead:selected", this.selectPayee);
    }
}

function confirm() {
    var deferred = $.Deferred(),
            available = parseFloat($("#Available").val()),
            amount = parseFloat($("#Amount").val());

    if (Math.abs(available - amount) < 0.00001) {
        deferred.resolve(true);
    } else {
        BootstrapDialog.confirm({
            title: "Warning",
            message: "The entered Amount for this expense does exceeds the available balance. Do you want to continue?",
            type: BootstrapDialog.TYPE_WARNING,
            callback: deferred.resolve
        });
    }

    return deferred.promise();
}