$(function() {

    $("h4 > div.bootstrap-switch").addClass("pull-right");

    $(document)
        .bind("keyup", "a", function() { $("#add").click(); })
        .bind("keyup", "y", function() { $("#add-attorney").click(); })
        .bind("keyup", "g", function() { $("#add-guardian").click(); })
        .bind("keyup", "f", function() { $("#FirstName").focus(); })
        .bind("keyup", "m", function() { $("#MiddleName").focus(); })
        .bind("keyup", "l", function() { $("#LastName").focus(); })
        .bind("keyup", "u", function() { $("#LivingUnit").focus(); })
        .bind("keyup", "p", function() { $("#HasClientProperty").bootstrapSwitch("toggleState"); })
        .bind("keyup", "b", function() { $("#BankAccount").focus(); })
        .bind("keyup", "c", function() { $("#Comments").focus(); })
        .bind("keyup", "r", function() { $("#remove").click(); })
        .bind("keyup", "s", function() { $("#image").click(); });

    function revalidate() {
        var form = $("#clientEditor")
            .removeData("validator")
            .removeData("unobtrusiveValidation");

        $.validator.unobtrusive.parse(form);
    }

    $(document).on("form.change", revalidate);
    $("#FirstName").focus();

    new PayeesEditor({ container: "#attorneys" }).listen();
    new PayeesEditor({ container: "#guardians" }).listen();
    new IdentifiersEditor({ container: "#identifiers" }).listen();
    new LivingUnitEditor({ container: "#livingUnitEditor" }).listen();
    new ImageCropper({
        container: "#crop-image",
        maxFileSize: 1024 * 1024,
        defaultImage: "/Content/Images/no-profile.jpg"
    }).listen();
});

function ImageCropper(options) {
    var self = this;
    var container = $(options.container);
    var preview = container.find("img"),
        image = container.find("#image"),
        imageX = container.find("#ImageX"),
        imageY = container.find("#ImageY"),
        imageHeight = container.find("#ImageHeight"),
        imageWidth = container.find("#ImageWidth"),
        blobUrl = null;

    this.initValidation = function () {
        $.validator.unobtrusive.adapters.addBool("filesize");
        $.validator.addMethod("filesize", this.validate);
        $(document).trigger("form.change");
    };

    this.validate = function (value, element) {
        if (value && element.files[0].size <= options.maxFileSize) {
            $(element).closest(".form-group")
                .removeClass("has-error");
            return true;
        } else if (value.length > 0) {
            $(element).closest(".form-group")
                .addClass("has-error");
            return false;
        } else {
            $(element).closest(".form-group")
                .removeClass("has-error");
            return true;
        }
    };
    
    this.imageCropped = function(data) {
        imageX.val(Math.round(data.x));
        imageY.val(Math.round(data.y));
        imageHeight.val(Math.round(data.height));
        imageWidth.val(Math.round(data.width));
    };

    this.imageSelected = function() {
        if (!preview.data("cropper")) {
            preview.cropper(options);
        }

        if (this.files && this.files.length) {
            self.setImage(this.files[0]);
        } else {
            self.imageRemoved();
        }
    };

    this.setImage = function(file) {
        blobUrl = window.URL.createObjectURL(file);

        preview
            .cropper("reset")
            .cropper("replace", blobUrl);
    }

    this.imageRemoved = function() {
        if (preview.data("cropper")) {
            preview.cropper("destroy");
            image.replaceWith(image.clone(true));
            window.URL.revokeObjectURL(blobUrl);
        } else {
            $("#ImageId").val(" ");
            preview.attr("src", options.defaultImage);
        }
    };

    this.listen = function () {
        var defaults = {
            aspectRatio: 1,
            guides: false,
            rotatable: false,
            crop: this.imageCropped
        };
        options = _.defaults(options, defaults);
        self.initValidation();
        container
            .on("change", "#image", self.imageSelected)
            .on("click", "#remove", self.imageRemoved);

    };
}

function IdentifiersEditor(options) {
    var template = $("#identifierTmpl").html();
    var container = $(options.container);

    this.addIdentifier = function(e) {
        var index = container.children(".row").length;
        var data = _.extend({ index: index }, options);

        container.children(".row:last")
            .before(Mustache.to_html(template, data));

        $(document).trigger("form.change");
        e.preventDefault();
    };

    this.removeIdentifier = function(e) {
        $(e.target)
            .closest(".row")
            .remove();

        $(document).trigger("form.change");
        e.preventDefault();
    };

    this.listen = function () {
        container.on("click", "a.btn-default", this.addIdentifier)
            .on("click", "a.remove", this.removeIdentifier);
    };
}

function PayeesEditor(options) {
    var self = this;
    var template = $("#payeeTmpl").html();
    var container = $(options.container);

    options.remoteUrl = decodeURIComponent(container.data("remote-url"));
    options.collectionName = container.data("collection-name");
    options.labelName = container.data("label-name");

    this.source = new Bloodhound({
        remote: {
            url: options.remoteUrl,
            filter: options.filter || function (res) { return res.Items; }
        },
        datumTokenizer: options.datumTokenizer || function(datum) {
             return Bloodhound.tokenizers.whitespace(datum.Name);
        },
        queryTokenizer: Bloodhound.tokenizers.whitespace
    });

    this.validate = function(value, element) {
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

    this.initValidation = function() {
        $.validator.unobtrusive.adapters.addBool("payee");
        $.validator.addMethod("payee", this.validate);
    };

    this.addTypeahead = function (data) {
        $(".typeahead", container)
            .typeahead("destroy");

        $("input[type='text']", container)
        .typeahead({}, {
            name: data.label + "-list",
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
        .change(this.clearPayee)
        .focus();
    }

    this.clearPayee = function(e) {
        $("input[type=hidden]:first", $(e.target).closest(".row"))
            .val("");
    }

    this.addPayee = function (e) {
        var index = container.children(".row").length;
        var data = _.extend({ index: index }, options);
        container.children(".row:last")
            .before(Mustache.to_html(template, data));

        self.addTypeahead(data);
        $(document).trigger("form.change");
        e.preventDefault();
    };

    this.removePayee = function (e) {
        $(e.target)
            .closest(".row")
            .remove();

        $(document).trigger("form.change");
        e.preventDefault();
    };

    this.selectPayee = function (e, item) {
        $("input[type=hidden]:first", $(e.target).closest(".row"))
             .val(item.Id);
    };

    this.listen = function () {
        self.source.initialize();
        self.initValidation();
        container.on("click", "a.btn-default", this.addPayee)
            .on("click", "a.remove", this.removePayee)
            .on("typeahead:selected", this.selectPayee);
    }
}

function LivingUnitEditor(options) {
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
            name: "LivingUnitEditor",
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
        .change(this.clearLivingUnit);
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

    this.selectLivingUnit = function (e, item) {
        $("input[type=hidden]:first", $(e.target).closest(".form-group"))
             .val(item.Id);
    };

    this.clearLivingUnit = function (e) {
        $("input[type=hidden]:first", $(e.target).closest(".form-group"))
            .val("");
    }

    this.listen = function () {
        self.source.initialize();
        self.addTypeahead();
        $.validator.unobtrusive.adapters.addBool("livingunit");
        $.validator.addMethod("livingunit", this.validate);
        container.on("typeahead:selected", this.selectLivingUnit);
    }
}