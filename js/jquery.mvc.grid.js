(function ($) {
    var jgrid, o;
    $.fn.grid = function (options) {
        var opts = $.extend({}, $.fn.grid.defaults, options);
        jgrid = $(this);
        o = $.meta ? $.extend({}, opts, jgrid.data()) : opts;

        init.call(jgrid, false, o);
        if (o.sort == true) { ajaxifyLive.call(jgrid, "th:has(a)"); }
        if (o.pagination == true) { ajaxifyLive.call(jgrid, "div.grid-footer span.fg-button"); ajaxifySelectLive.call(jgrid); }
        if (o.search == true) { ajaxifySearch.call(jgrid); }
        return jgrid;
    }
    function init(isAjaxRequest, o) {
        this.find('tbody tr:nth-child(odd)').addClass('odd');
        this.find('tbody tr:nth-child(even)').addClass('even');
        if (isAjaxRequest === true) {
            o.onReload.call(this);
        }
        unblock.call(this);
    }
    function ajaxifySearch() {
        var self = this;
        self.find("div.grid-header input").live("keydown", function (event) {
            if (event.keyCode == '13') {
                event.preventDefault();
                block.call(jgrid);
                var innerSelf = $(this);
                self.load(innerSelf.attr('href').replace("%7B0%7D", innerSelf.val()), function (response, status, xhr) { init.call(jgrid, true, o) });
            }
        });
    }
    function ajaxifySelectLive() {
        var self = this;
        self.find("div.grid-header select").live("change", function (event) {
            event.preventDefault();
            block.call(jgrid);
            self.load($(this).val(), function (response, status, xhr) { init.call(jgrid, true, o) });
        });
    }
    function ajaxifyLive(element) {
        var self = this;
        self.find(element).live("click", function (event) {
            event.preventDefault();
            block.call(jgrid);
            self.load($(this).find('a').attr("href"), function (response, status, xhr) { init.call(jgrid, true, o) });
        });
    }
    function block() {
        this.find("tbody").fadeTo(200, '0.33')
    }
    function unblock() {
        this.find("tbody").fadeIn(200)
    }
    $.fn.grid.defaults = {
        search: true,
        sort: true,
        pagination: true,
        onReload: function () { }
    }
})(jQuery);