(function ($) {
	$.fn.confirmdialog = function (options) {
		var opts = $.extend({}, $.fn.confirmdialog.defaults, options),
		confirmdialog = $('<div style="display: none"></div>')
			.addClass(opts.class)
			.appendTo(opts.insertSelector)
			.dialog({
				autoOpen: false,
				resizable: false,
				modal: true,
				buttons: {
					Oui: function () {
					    opts.onYes.call(confirmdialog, confirmdialog.data("link"));
					    confirmdialog.dialog("close")
					}, Non: function () {
					    opts.onNo.call(confirmdialog, confirmdialog.data("link"));
					    confirmdialog.dialog("close")
					}
				}
			});
		this.live(opts.eventType, function (event) {
			var self = $(this);
			event.preventDefault();
			confirmdialog.parent()
					.find(".ui-dialog-title")
					.html(self.attr(opts.titleAttr));
			confirmdialog
					.html(opts.formatMessage.call(self, self.attr(opts.messageAttr)))
					.data("link", opts.onGetUrl.call(self))
					.dialog("open");
		});
	}
	$.fn.confirmdialog.defaults = {
		messageAttr: "message",
		titleAttr: "title",
		insertSelector: "body",
		onGetUrl: function () { return this.closest("form") },
		onYes: function (url) { $(url).submit(); },
		onNo: function (url) { },
		class: "",
		formatMessage: function (message) { return "<p><span class='ui-icon ui-icon-alert' style='float: left; margin: 0 7px 20px 0;'></span>" + message + "</p>" },
		eventType: "click"
	}
})(jQuery);