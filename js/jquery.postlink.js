(function ($) {
    $.fn.postlink = function (options) {
        var opts = $.extend({}, $.fn.postlink.defaults, options),
        form = {
            post: function (url, data) {
                var linkForm = form.create.call(this, url, data);
                $("body").append(linkForm);
                linkForm.submit()
            },
            create: function (url, data) {
                return $("<form></form>").attr("action", url).attr("method", "post").attr("id", "ajaxselectForm_" + (new Date()).getTime())
            }
        };
        if (opts.hasConfirmDialog === true) {
            this.confirmdialog(
            {
                onGetUrl: function () {
                    return this.attr("href")
                },
                onYes: function (url) {
                    form.post(url)
                }
            })
        } else {
            this.live("click", function (event) {
                event.preventDefault();
                form.post(this.attr("href"))
            })
        }
        return this;
    };
    $.fn.postlink.defaults = { hasConfirmDialog: true }
})(jQuery);