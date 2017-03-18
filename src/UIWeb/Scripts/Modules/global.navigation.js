$(function() {
    var hash = document.location.hash;
    var query = document.location.search;
    var prefix = "tab-";

    if (query) {
        $(".nav-tabs a:last").tab("show");
    }

    if (hash) {
        $(".nav-tabs a[href='" + hash.replace(prefix, "") + "']").tab("show");
    }
    
    $(".nav-tabs a").on("shown.bs.tab", function(e) {
        window.location.hash = e.target.hash.replace("#", "#" + prefix);
    });

    $("[data-toggle='tooltip']").tooltip();
    $("[data-toggle='popover']").popover({
        html: true, content: function () {
            return $(this).data("content")
                || $(this).next(".popover-content").html();
        }
    });
    $("input.toggle[type='checkbox']").bootstrapSwitch();

    //clicking a table row will trigger the default button click
    $(".table tbody tr:visible").bind("mouseup", function(e) {
        if (e.target.tagName === "BUTTON" || e.target.parentElement.tagName === "BUTTON")
            return;
        var button = $(this).find("a.btn-primary");
        button.length && button[0].click();
    });

    //allow escape to drop focus of the current element
    $("input").keyup(function(event) {
        if (event.keyCode === 27) {
            $("input").blur();
        }
    });

    $(document).ajaxStart(function(e) {
        $(".form-control-feedback").removeClass("hidden");
    });
    $(document).ajaxStop(function(e) {
        $(".form-control-feedback").addClass("hidden");
    });

    $("ol.breadcrumb li.dropdown.pull-right, table div.dropdown").hover(function() {
        $(this).find('.dropdown-menu').stop(true, true).delay(200).fadeIn(500);
    }, function() {
        $(this).find(".dropdown-menu").stop(true, true).delay(200).fadeOut(500);
    });

    $("#cancel").bind("click", function() {
        window.history.back();
    });

    $("#continue").on("click", function(e) {
        $("#return_url").val($("#continue").data("url"));
    });

    $(document)
        //use keydown to prevent default save dialog
        .bind("keydown", "ctrl+s", function(event) {
            event.preventDefault();
            $("#save")[0].click();
        })
        .bind("keydown", "alt+s", function(event) {
            event.preventDefault();
            $("#search").focus();
        })
        .bind("keyup", "v", function() { $("#isactive").each(function() { this.checked = !this.checked; }); })
        .bind("keyup", "ctrl+e", function() { $("#edit")[0].click(); })
        .bind("keyup", "ctrl+c", function() { $("#cancel")[0].click(); });


    $.validator.setDefaults({ ignore: [] });

    $.validator.methods.date = function (value, element) {
        //This is not ideal but Chrome passes dates through in ISO1901 format regardless of locale 
        //and despite displaying in the specified format.

        return this.optional(element)
            || Globalize.parseDate(value, "dd/mm/yyyy")
            || Globalize.parseDate(value, "yyyy-mm-dd");
    };

    $.fn.clearValidation = function () {
        var v = $(this).validate();
        $('[name]', this).each(function() {
            v.successList.push(this);
            v.showErrors();
        });
        v.resetForm();
        v.reset();
    };


    $.fn.serializeObject = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name] !== undefined) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };

    function popupPrintWindow(e) {
        var url = $(e.target).attr("href");
        window.open(url, "Popup Window", 'scrollbars=1,height=860,width=860');
        e.preventDefault();
    }

    $(".print-check").click(popupPrintWindow);
    $(".print-receipt").click(popupPrintWindow);
});