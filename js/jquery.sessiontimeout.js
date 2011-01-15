(function($) {
    $.fn.sessiontimeout = function(element, resume, options) {
        var opts = $.extend({}, $.fn.sessiontimeout.defaults, options),
		o = $.meta ? $.extend({}, opts, $(element).data()) : opts,
		SessionTimeout = {
		    init: function() {
		        var self = this;
		        if (location.href.toLowerCase().indexOf(o.logOutUrl.toLowerCase()) == -1)
		            setTimeout(function() { self._idle(); }, (o.idleAfter * 1000) - o.warningLength);
		    },
		    _createHtml: function() {
		        var self = this,
                warning = $("<div id='idletimeout'></div>")
                    .html(o.message)
                    .appendTo(element);
		        $(resume).bind("click", { warning: warning }, self._resume);
		        return warning;
		    },
		    _resume: function(e) {
		        var self = this;

		        e.preventDefault();
		        window.clearInterval(countdown);
		        o.onResume.call(e.data.warning);
		    },
		    _idle: function() {
		        var self = this,
				counter = o.warningLength,
				warning = self._createHtml();

		        o.onIdle.call(warning);
		        o.onCountdown.call(warning, counter);

		        countdown = window.setInterval(function() {
		            counter -= 1;

		            if (counter === 0) {
		                window.clearInterval(countdown);
		                o.onTimeout.call(warning, o.logOutUrl);
		            } else {
		                o.onCountdown.call(warning, counter);
		            }
		        }, 1000);
		    }
		};

        SessionTimeout.init();
    };

    $.fn.sessiontimeout.defaults = {
        warningLength: 30,
        idleAfter: 1200,
        message: "You will be logged off in <span></span>&nbsp;seconds due to inactivity.<a href='#'>Click here to continue using this web page</a>.",
        logOutUrl: "",
        onTimeout: function(logOutUrl) { $(this).slideToggle(); window.location = logOutUrl; },
        onIdle: function() { $(this).slideToggle(); },
        onCountdown: function(counter) { $(this).find('span').html(String(counter)); },
        onResume: function() { $(this).slideToggle(); window.location.reload(); }
    };
})(jQuery);