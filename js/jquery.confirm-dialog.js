(function ($) {
    $.fn.confirmdialog = function (options) {
        var opts = $.extend({}, $.fn.confirmdialog.defaults, options),
        confirmdialog = $('<div style="display: none"></div>')
        .addClass(opts.dialogClass)
        .appendTo(opts.insertSelector)
        .dialog({
            autoOpen: false,
            resizable: false,
            modal: true,
            buttons: {
                Oui: function () {
                    confirmdialog.dialog("close");
                    opts.onYes.call(confirmdialog, confirmdialog.data("link"))
                },
                Non: function () {
                    confirmdialog.dialog("close");
                    opts.onNo.call(confirmdialog, confirmdialog.data("link"))
                }
            }
        });
        if (opts.noLive === true) {
            this.click(function (event) {
                var self = $(this);
                event.preventDefault();
                if (opts.validation() === true) {
                    open($(this), confirmdialog, opts)
                }
            })
        } else {
            this.live(opts.eventType, function (event) {
                var self = $(this);
                event.preventDefault();
                if (opts.validation() === true) {
                    open($(this), confirmdialog, opts)
                }
            })
        }
        return this
    };
    function open(element, confirmdialog, opts) {
        confirmdialog.parent().find(".ui-dialog-title").html(element.attr(opts.titleAttr));
        confirmdialog.html(opts.formatMessage.call(element, element.data(opts.messageData))).data("link", opts.onGetUrl.call(element)).dialog("open")
    }
    $.fn.confirmdialog.defaults = {
        messageData: "message",
        titleAttr: "title",
        insertSelector: "body",
        onGetUrl: function () {
            return this.closest("form")
        },
        onYes: function (url) {
            $(url).submit()
        },
        onNo: function (url) { },
        dialogClass: "",
        formatMessage: function (message) {
            return "<p><span class='ui-icon ui-icon-alert' style='float: left; margin: 0 7px 20px 0;'></span>" + message + "</p>"
        },
        noLive: false,
        validation: function () { return true },
        eventType: "click"
    }
})(jQuery);