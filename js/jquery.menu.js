(function($) {
    $.fn.menu = function() {
        return this.each(function() {
            var self = $(this),
            nodeLink = self.find("a[href]").filter(function() {
                return stripQueryString(location.href) == stripQueryString(this.href)
            }).parent(),
            currenth3 = nodeLink.closest("h3 ~ div", self).prev("h3");

            if (nodeLink.length > 1) {
                nodeLink = nodeLink.children("a[href]").filter(function() {
                    return location.href == this.href
                }).parent();
            }

            nodeLink.addClass("ui-tabs-selected ui-state-active");
            self.accordion({ header: "h3", clearStyle: true, autoHeight: false, active: currenth3 });

            return self;
        });
    };
	function stripQueryString(url) {
		var lowerUrl = url.toLowerCase();
		var hasQueryString = lowerUrl.indexOf("?");
		return hasQueryString >= 0 ? lowerUrl.substring(0, hasQueryString) : lowerUrl;
	}
})(jQuery);