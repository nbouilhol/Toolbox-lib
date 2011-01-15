(function ($) {
    var jgrid, o;
    $.fn.grid = function (options) {
        var opts = $.extend({}, $.fn.grid.defaults, options);
        jgrid = $(this);
        o = $.meta ? $.extend({}, opts, jgrid.data()) : opts;

        reload.call(jgrid, o);
        jgrid.ajaxStart(manageAjaxStart).ajaxComplete(manageAjaxComplete);
        if (o.sort == true) {
            ajaxifyLive.call(jgrid, "th");
        }
        if (o.pagination == true) {
            ajaxifyLive.call(jgrid, "span.fg-button");
            ajaxifySelectLive.call(jgrid);
        }
        if (o.search == true) {
            ajaxifySearch.call(jgrid);
        }
        return jgrid;
    }
    function manageAjaxStart() {
        block.call(jgrid);
    }
    function manageAjaxComplete(e, status, settings) {
        unblock.call(jgrid);
        if (status == "error") {
            return
        }
        reload.call(jgrid, o);
    }
    function reload(o) {
        this.find('tbody tr:nth-child(odd)').addClass('odd');
        this.find('tbody tr:nth-child(even)').addClass('even');
        o.onReload.call(this);
    }
    function ajaxifySearch() {
        var self = this;
        self.find("input").live("keydown", function (event) {
            if (event.keyCode == '13') {
                event.preventDefault();
                var innerSelf = $(this);
                self.load(innerSelf.attr('href').replace("%7B0%7D", innerSelf.val()));
            }
        });
    }
    function ajaxifySelectLive() {
        var self = this;
        self.find("select").live("change", function (event) {
            event.preventDefault();
            self.load($(this).val());
        });
    }
    function ajaxifyLive(element) {
        var self = this;
        self.find(element).live("click", function (event) {
            event.preventDefault();
            self.load($(this).find('a').attr("href"));
        });
    }
    function block() {
        this.find("tbody").fadeTo(0, 0.5);
    }
    function unblock() {
        this.find("tbody").fadeTo(0, 1);
    }
    $.fn.grid.defaults = {
        search: true,
        sort: true,
        pagination: true,
        onReload: function () { }
    }
})(jQuery);