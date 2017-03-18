$(function() {
    //keybindings
    $(document)
    .bind("keyup", "w", function () { });

    function revalidate() {
        var form = $("form")
        .removeData("validator")
        .removeData("unobtrusiveValidation");

        $.validator.unobtrusive.parse(form);
        form.validateBootstrap();
    }

    $(document).on("form.change", revalidate);

    function filter(res) {
        return res.alignItems || res.Accounts;
    }

    new FromAccountEditor({ container: "#fromAccountEditor", filter:filter }).listen();
    new ToAccountEditor({ container: "#toAccountEditor", filter:filter }).listen();

    $("form").on("click", "#save", function (e) {
        e && e.preventDefault();
        confirm()
            .done(function (response) {
                if (response) {
                    $("form")[0].submit();
                }
            });
    });
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

function FromAccountEditor(options) {
    var self = this;

    this.selectAccount = function (e, item) {
        $("input[type=hidden]:first", $(e.target).closest(".form-group"))
        .val(item.Id);
        $("#Available")
        .val(item.Available);
    };

    this.clearAccount = function (e) {
        $("input[type=hidden]:first", $(e.target).closest(".form-group"))
            .val("");
    };

    this.listen = function () {
        var config = {
            name: "fromAccountEditor",
            selected: self.selectAccount,
            change: self.clearAccount
        };
        new TypeaheadEditor(_.extend(config, options)).listen();
    };
}

function ToAccountEditor(options) {
    var self = this;

    this.selectAccount = function (e, item) {
        $("input[type=hidden]:first", $(e.target).closest(".form-group"))
        .val(item.Id);
    };

    this.clearAccount = function (e) {
        $("input[type=hidden]:first", $(e.target).closest(".form-group"))
            .val("");
    };

    this.listen = function () {
        var config = {
            name: "toAccountEditor",
            selected: self.selectAccount,
            change: self.clearAccount
        };
        new TypeaheadEditor(_.extend(config, options)).listen();
    };
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
            message: "The entered Amount for this expense exceeds the available balance. Do you want to continue?",
            type: BootstrapDialog.TYPE_WARNING,
            callback: deferred.resolve
        });
    }

    return deferred.promise();
}