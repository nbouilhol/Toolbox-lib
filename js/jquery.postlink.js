(function ($) {
    $.fn.postlink = function (options) {
        var opts = $.extend({}, $.fn.postlink.defaults, options);
        if (opts.hasConfirmDialog == true) {
            this.confirmdialog({ onGetUrl: function () { return this.attr("href") }, onYes: function (url) { postForm(url) } });
        }
        else {
            this.live("click", function (event) {
                event.preventDefault();
                postForm(this.attr("href"));
            });
        }
    }
    function postForm(url) {
        var linkForm = createPostForm.call(this, url);
        $('body').append(linkForm);
        linkForm.submit();
    }
    function createPostForm(url) {
        var linkForm = $('<form></form>')
                .attr("action", url)
                .attr("method", "post")
                .attr("id", "postlinkForm_" + new Date().getTime());
        return linkForm;
    }
    $.fn.postlink.defaults = {
        hasConfirmDialog: true
    };
})(jQuery);