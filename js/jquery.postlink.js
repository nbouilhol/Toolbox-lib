(function ($) {
    $.fn.postlink = function (options) {
        var opts = $.extend({}, $.fn.postlink.defaults, options);
        if (opts.hasConfirmDialog == true) {
            $(this).confirmdialog({ onGetUrl: function () { return this.attr("href") }, onYes: postForm });
        }
        else {
            $(this).live("click", function (event) {
                event.preventDefault();
                postForm.call($(this));
            });
        }
    }
    function postForm() {
        var linkForm = createPostForm.call(this, this.attr("href"));
        $('body').append(linkForm);
        linkForm.submit();
    }
    function createPostForm(href) {
        var linkForm = $('<form></form>')
                .attr("action", href)
                .attr("method", "post")
                .attr("id", "postlinkForm_" + new Date().getTime());
        return linkForm;
    }
    $.fn.postlink.defaults = {
        hasConfirmDialog: true
    };
})(jQuery);