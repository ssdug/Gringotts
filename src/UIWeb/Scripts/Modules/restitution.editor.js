$(function () {

    $(document)
        .bind("keyup", "d", function () { $("#OrderNumber").focus(); })
        .bind("keyup", "f", function () { $("#IsPropertyDamage").bootstrapSwitch("toggleState"); })
        .bind("keyup", "a", function () { $("#Amount").focus(); })
        .bind("keyup", "b", function () { $("#Ballance").focus(); })
        .bind("keyup", "w", function () { $("#Withholding").focus(); })
        .bind("keyup", "p", function () { $("#PayeeName").focus(); })
        .bind("keyup", "c", function () { $("#Comments").focus(); })
        .bind("keyup", "r", function () { $("#SatisfiedReason").focus(); })
        .bind("keyup", "t", function () { $("#IsSatisfied").bootstrapSwitch("toggleState"); });

    new PayeeEditor({ container: "#payeeEditor" }).listen();

    var form = $("#expenseEditor")
            .removeData("validator")
            .removeData("unobtrusiveValidation");

    $.validator.unobtrusive.parse(form);
});

function PayeeEditor(options) {
    var self = this;
    var container = $(options.container);

    options.remoteUrl = decodeURIComponent(container.data("remote-url"));

    this.filterPayees = function (res) {
        var isPropertyDammage = $("#IsPropertyDamage").bootstrapSwitch("state");
        return _.filter(res.Items, function (i) {
            return isPropertyDammage
                ? _.some(i.Types, { "Name": "Facility" })
                : _.some(i.Types, { "Name": "Court" });
        });
    };

    this.source = new Bloodhound({
        remote: {
            url: options.remoteUrl,
            filter: options.filter || this.filterPayees
        },
        datumTokenizer: options.datumTokenizer || function (datum) {
            return Bloodhound.tokenizers.whitespace(datum.Name);
        },
        queryTokenizer: Bloodhound.tokenizers.whitespace
    });

    this.addTypeahead = function() {
        $("input[type='text']", container)
            .typeahead({}, {
                name: "PayeeEditor",
                displayKey: "Name",
                source: this.source.ttAdapter(),
                templates: {
                    empty: [
                        "<div class=\"empty-message\">",
                        "<p>No Matches Found</p>",
                        "</div>"
                    ].join("\n"),
                    suggestion: function(context) {
                        return Mustache.render("<p><strong>{{Name}}</strong><br/><small>{{DisplayAddress}}</small></p>", context);
                    }
                }
            })
            .change(this.clearPayee);
    };

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
        $("input[type=hidden]:first", $(e.target).closest(".form-group"))
             .val(item.Id);
    };

    this.clearPayee = function (e) {
        $("input[type=hidden]:first", $(e.target).closest(".form-group"))
            .val("");
    }

    this.listen = function () {
        self.source.initialize();
        self.addTypeahead();
        $.validator.unobtrusive.adapters.addBool("payee");
        $.validator.addMethod("payee", this.validate);
        container.on("typeahead:selected", this.selectPayee);
    }
}