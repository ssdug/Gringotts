$(function() {

    //show tooltip for clipped table identifiers
    $(document).on("mouseenter", "td.identifiers", function() {
        var $this = $(this);
        if (this.offsetWidth < this.scrollWidth) {
            $this.tooltip({
                title: $this.text(),
                html: true,
                container: "body",
                placement: "top"
            });
            $this.tooltip("show");
        }
    });

    //keybindings
    $(document)
        .bind("keyup", "a", function () { $("#add")[0].click(); })
        .bind("keyup", "c", function () { $("#clear")[0].click(); });
});