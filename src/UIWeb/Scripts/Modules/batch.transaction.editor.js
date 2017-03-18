$(function() {
    function revalidate() {
        var form = $("form")
        .removeData("validator")
        .removeData("unobtrusiveValidation");

        $.validator.unobtrusive.parse(form);
        form.validateBootstrap();
    }

    $(document).on("form.change", revalidate);
});

function TypeaheadEditor(options) {
    var self = this;
    options = _.extend({
        name: "Editor",
        displayKey: "Name",
        templates: {
            empty: ["<div class=\"empty-message\">",
                        "<p>No Matches Found</p>",
                    "</div>"].join("\n"),
            suggestion: function (context) {
                return Mustache.render("<p><strong>{{Name}}</strong></p>", context);
            }
        },
        selected: function (e, item) {
            $("input[type=hidden]:first", $(e.target).closest(".form-group"))
            .val(item.Id);
        },
        change: function (e) {
            $("input[type=hidden]:first", $(e.target).closest(".form-group"))
                .val("");
        },
        searchParser: function (term) {
            return term;
        }
    }, options);

    var container = $(options.container);

    options.remoteUrl = decodeURIComponent(container.data("remote-url"));

    this.source = new Bloodhound({
        remote: {
            url: options.remoteUrl,
            filter: options.filter || function (res) {
                return res.Items;
            }
        },
        datumTokenizer: options.datumTokenizer || function (datum) {
            return Bloodhound.tokenizers.whitespace(datum.Name);
        }
        ,
        queryTokenizer: Bloodhound.tokenizers.whitespace
    });

    this.addTypeahead = function () {
        $("input[type='text']", container)
            .typeahead({}, {
                name: options.name,
                displayKey: options.displayKey,
                source: this.source.ttAdapter(),
                templates: options.templates
            })
            .change(options.change);
    };

    this.validate = function (value, element) {
        var id = parseInt($("input[type=hidden]", $(element).closest(".form-group")).val());

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

    this.initialize = function () {
        var name = $("input[type='text']", container).val();
        var id = $("input[type=hidden]:first", container).val();
        if (name && id) {
            self.source.get(options.searchParser(name), function (d) {
                var item = _.find(d, function (i) {
                    return i.Id == id;
                });
                if (item != null) {
                    $("input[type='text']", container)
                    .trigger("typeahead:selected", item);
                }
            });
        }
    };

    this.listen = function () {
        self.source.initialize().then(function () {
            self.initialize();
            self.addTypeahead();
            $.validator.unobtrusive.adapters.addBool("typeahead");
            $.validator.addMethod("typeahead", self.validate);

            container.on("typeahead:selected", options.selected);
        });
    };
}

function ExpenseCategoryEditor(options) {
    this.listen = function () {
        var config = {
            name: "ExpenseCategoryEditor"
        };
        new TypeaheadEditor(_.extend(config, options)).listen();
    };
}

function ReceiptSourceEditor(options) {
    this.listen = function () {
        var config = {
            name: "ReceiptSourceEditor"
        };
        new TypeaheadEditor(_.extend(config, options)).listen();
    };
}

function PayeeEditor(options) {
    this.listen = function () {
        var config = {
            name: "PayeeEditor",
            templates: {
                suggestion: function (context) {
                    return Mustache.render("<p><strong>{{Name}}</strong><br/><small>{{DisplayAddress}}</small></p>", context);
                }
            }
        };
        new TypeaheadEditor(_.extend(config, options)).listen();
    }
}

function SubsidiaryAccountEditor(options) {
    var self = this;

    this.selectSubsidiaryAccount = function (e, item) {
        $("input[type=hidden]:first", $(e.target).closest(".form-group"))
        .val(item.Id);
        $("#Available", $(e.target).closest("#batch-line-item-editor"))
        .val(item.Available);
    };

    this.clearSubsidiaryAccount = function (e) {
        $("input[type=hidden]:first", $(e.target).closest(".form-group"))
            .val("");
    };

    this.listen = function () {
        var config = {
            name: "subsidiaryAccountEditor",
            selected: self.selectSubsidiaryAccount,
            change: self.clearSubsidiaryAccount
        };
        new TypeaheadEditor(_.extend(config, options)).listen();
    };
}

function ClientAccountEditor(options) {
    var self = this;

    this.selectClient = function (e, client) {
        $("input[type=hidden]:first", $(e.target).closest(".form-group"))
        .val(client.Id);

        self.setAccounts(client);
    };

    this.setAccounts = function (item) {
        var residency = _.first(item.Residencies);
        var select = $("#AccountId").data("accounts", residency.Accounts);
        select.find('option').remove();
        _.forEach(residency.Accounts, function (account) {
            if (account.Id == options.accountId) {
                select.append(new Option(account.Name, account.Id, false, true));
            } else {
                select.append(new Option(account.Name, account.Id));
            }
        });
        select.attr("readonly", false);
        select.trigger("change");
    };

    this.selectAccount = function (e) {
        var select = $(e.target);
        var selectedId = select.val();
        var accounts = select.data("accounts");
        var account = _.first(accounts.filter(function (i) {
            return i.Id == selectedId;
        }
        ));
        $("input[type=hidden]:first", $(e.target).closest(".form-group"))
        .val(account.Name);
        $("#Available", $(e.target).closest("#batch-line-item-editor"))
        .val(account.Available);
    };

    this.clearClient = function (e) {
        $("input[type=hidden]:first", $(e.target).closest(".form-group"))
        .val("");

        $("#AccountId").attr("readonly", true)
        .children("option").remove();
        $("#Available").val("");
    };

    this.listen = function () {
        var config = {
            name: "clientAccountEditor",
            displayKey: "DisplayName",
            selected: self.selectClient,
            change: self.clearClient,
            searchParser: function (term) {
                return term.split(",")[0];
            },
            templates: {
                suggestion: function (context) {
                    return Mustache.render("<p><strong>{{DisplayName}}</strong></p>", context);
                }
            }
        };

        new TypeaheadEditor(_.extend(config, options)).listen();
        $("#AccountId").change(self.selectAccount);
    };
}

function BatchTransactionEditor(options) {
    var self = this;
    var transactionTmpl = $("#transactionTmpl").html();
    var itemEditorTmpl = $("#itemEditorTmpl").html();
    var noItemsDisplay = $("#batch-items tbody tr:has(td[colspan])").clone();
    var container = $(options.container);

    this.showHeader = function (duration, e) {
        return $.when.apply($, [
            $(".tally-expected, .tally-total", container).slideUp(duration),
            $("#batch-header", container).slideDown(duration),
            $("#show-batch-header").hide(),
            $("#hide-batch-header").show()
        ]);
    };

    this.hideHeader = function (duration) {
        return $.when.apply($, [
            $(".tally-expected, .tally-total", container).slideDown(duration),
            $("#batch-header", container).slideUp(duration),
            $("#show-batch-header").show(),
            $("#hide-batch-header").hide()
        ]);
    };

    this.syncAmount = function (e) {
        var value = $(e.target).val();
        if (isNaN(parseFloat(value)) || !isFinite(value))
            return;

        value = parseFloat(value).toFixed(2);

        $(e.data.target, container)
            .text(value);

        self.checkAmounts();
    };

    this.checkAmounts = function () {
        var expected = $("#ExpectedAmount").val();
        var total = $("#TotalAmount").val();

        if (parseFloat(expected) === parseFloat(total)) {
            $(".tally-total").removeClass("text-danger").addClass("text-success");
        } else {
            $(".tally-total").addClass("text-danger").removeClass("text-danger");
        }
    };

    this.addToTotalAmount = function (amount) {
        var current = $("#TotalAmount").val();
        var updated = parseFloat(current) + parseFloat(amount);

        $(".total-amount").val(updated.toFixed(2))
            .trigger("change");
    };

    this.subtractFromTotalAmount = function (amount) {
        var current = $("#TotalAmount").val();
        var updated = parseFloat(current) - parseFloat(amount);

        $(".total-amount").val(updated.toFixed(2))
            .trigger("change");
    };

    this.addItemFromEditor = function (e) {
        if (!$("form").valid()) {
            return;
        } else {
            var form = $("form").serializeToJSON();
            var available = $("#Available").val();

            self.confirmItemAmount(form.Amount, available)
            .done(function(response){
                if(response){
                    self.addItemToTable(form);
                    self.addToTotalAmount(form.Amount);

                    self.hideHeader(400)
                        .done(self.resetItem);
                }
            });
        }
    };

    this.confirmItemAmount = function (amount, available) {
        var deferred = $.Deferred(),
            amount = parseFloat(amount),
        available = parseFloat(available),
        type = $("#ConcreteModelType").val();

        if(amount <= available || _.includes(type, "Receipt")){
            deferred.resolve(true);
        } else {
            BootstrapDialog.confirm({
                title: "Warning",
                message: "The amount entered is more than the available balance of the selected account. Do you want to continue?",
                type: BootstrapDialog.TYPE_WARNING,
                callback:deferred.resolve
            });
        }

        return deferred.promise();
    };

    this.showItemInEditor = function (item) {
        $("#batch-line-item-editor")
            .html(Mustache.to_html(itemEditorTmpl, item));

        new PayeeEditor({ container: "#payeeEditor" }).listen();
        new ExpenseCategoryEditor({ container: "#expenseCategoryEditor" }).listen();
        new ReceiptSourceEditor({ container: "#receiptSourceEditor" }).listen();
        new SubsidiaryAccountEditor({ container: "#subsidiaryAccountEditor" }).listen();
        new ClientAccountEditor({ container: "#clientAccountEditor", accountId: item.AccountId }).listen();

        $(document).trigger("form.change");
    };

    this.addItemToTable = function (item) {
        var rows = $("#batch-items tbody tr:not(:has(td[colspan]))", container);
        if (rows.length === 0 || item.Index == 0) {
            $("#batch-items tbody")
                .prepend(Mustache.to_html(transactionTmpl, item));
        } else {
            rows.eq(item.Index - 1)
                .after(Mustache.to_html(transactionTmpl, item));
        }
        $("#batch-items tbody tr:has(td[colspan]), #batch-items tbody tr.text-success")
            .remove();
    };

    this.editItem = function (e) {
        var record = $(e.target).closest("tr");
        var index = $("#Transactions_Index", record).val();
        var form = $("form").serializeToJSON();
        var item = form.Transactions[index];
        item["Index"] = index;

        self.subtractFromTotalAmount(item.Amount);
        self.showItemInEditor(item);

        record.addClass("text-success");

        self.hideHeader(400);

    };

    this.removeItem = function (e) {
        e && e.preventDefault();
        var record = $(e.target).closest("tr");
        var amount = $(".item-amount", record).val();
        self.subtractFromTotalAmount(amount);
        record.remove();
        self.resetItemsIndex();
        self.resetItem();
    };

    this.resetItem = function () {
        self.showItemInEditor({
            Index: $("#batch-items tbody tr:not(:has(td[colspan]))").length
        });
        self.focusFirst();
    };

    this.resetItemsIndex = function () {
        var form = $("form").serializeToJSON();
        $("#batch-items tbody tr").remove();
        var items = _.filter(_.valuesIn(form.Transactions), function (i) {
            return !_.isString(i);
        });
        _.forEach(items, function (item, i) {
            item.Index = i;
            self.addItemToTable(item);
        });
    };

    this.focusFirst = function () {
        $(".first-input", container)
            .filter(":visible")
            .filter(":first")
            .focus();
    };

    this.confirmExpectedMatchesTotal = function() {
        var deferred = $.Deferred(),
            expected = parseFloat($("#ExpectedAmount",container).val()),
            total = parseFloat($("#TotalAmount", container).val());

        if (Math.abs(total - expected) < 0.00001) {
            deferred.resolve(true);
        } else {
            BootstrapDialog.confirm({
                title: "Warning",
                message: "The Expected Amount for this batch does not match the Total Amount. Do you want to continue?",
                type: BootstrapDialog.TYPE_WARNING,
                callback:deferred.resolve
            });
        }
        
        return deferred.promise();
    };

    this.onSave = function (e) {
        e && e.preventDefault();
        $("form").validate();
        if (!$("#batch-header input:not(.tt-hint), #batch-header select").valid()) {
            return false;
        }

        if (_.isEmpty($(".batch-item"))) {
            BootstrapDialog.alert("Please enter some transactions before saving.");
            return false;
        }

        self.confirmExpectedMatchesTotal()
        .done(function(response){
            if(response){
                $("form")[0].submit();
            }
        });
    };

    this.initialize = function () {
        self.checkAmounts();
        self.showItemInEditor({
            Index: $("#batch-items tbody tr:not(:has(td[colspan]))").length
        });
        self.showHeader(0)
            .then(self.focusFirst);
    };

    this.listen = function () {
        self.initialize();
        container.on("change", "#ExpectedAmount", {
            target: ".tally-expected span.value"
        }, self.syncAmount);
        container.on("change", "#TotalAmount", {
            target: ".tally-total span.value"
        }, self.syncAmount);
        container.on("click", "#save, #continue", self.onSave);
        container.on("click", "#show-batch-header", self.showHeader.bind(400));
        container.on("click", "#hide-batch-header", self.hideHeader.bind(400));
        container.on("click", "#add-batch-item", self.addItemFromEditor);
        container.on("click", ".edit", self.editItem);
        container.on("click", ".remove", self.removeItem);
        container.on("keydown", "#Memo", function (e) {
            if (e.which == 9) {
                e.preventDefault();
                self.hideHeader(400);
                $("#ExpenseCategoryName", container).focus();
            }
        });
        container.on("keydown", "#ExpenseCategoryName", function (e) {
            if (e.which == 9 && e.shiftKey) {
                e.preventDefault();
                self.showHeader(400);
                $("#Memo").focus();
            }
        });
    };
}