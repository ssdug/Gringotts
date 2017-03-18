$(function () {
    $(document)
        .bind("keyup", "a", function() { $("#Amount").focus(); })
        .bind("keyup", "t", function() { $("#ReceiptTypeId").focus(); })
        .bind("keyup", "f", function() { $("#ReceivedFrom").focus(); })
        .bind("keyup", "r", function() { $("#ReceivedFor").focus(); })
        .bind("keyup", "n", function () { $("#ReceiptNumber").focus(); });

    new ReceiptSourceEditor({ container: "#receiptSourceEditor" }).listen();
});

function ReceiptSourceEditor(options) {
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
            name: "ReceiptSourceEditor",
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
        .change(this.clearSource);
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

    this.selectSource = function (e, item) {
        $("input[type=hidden]:first", $(e.target).closest(".row"))
             .val(item.Id);
    };

    this.clearSource = function (e) {
        $("input[type=hidden]:first", $(e.target).closest(".row"))
            .val("");
    }

    this.listen = function () {
        self.source.initialize();
        self.addTypeahead();
        $.validator.unobtrusive.adapters.addBool("source");
        $.validator.addMethod("source", this.validate);
        container.on("typeahead:selected", this.selectSource);
    };
}