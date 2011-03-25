(function ($) {
	$.fn.autocompletecached = function (url, options) {
		var opts = $.extend({}, $.fn.autocompletecached.defaults, options);
		var cache = {}, lastXhr;
		this.autocomplete({ source: function (request, response) {
			var term = request.term;
			if (term in cache) {
				response(cache[term]);
				return this
			}
			lastXhr = $.getJSON(url, request, function (data, textStatus, jqXHR) {
				cache[term] = data;
				if (jqXHR === lastXhr) {
					response(data)
				}
			})
		}, minLength: opts.minLength, select: opts.select});
		return this
	}
	$.fn.autocompletecached.defaults =
	{
		select: function (event, ui) { },
		minLength: 0
	}
})(jQuery);