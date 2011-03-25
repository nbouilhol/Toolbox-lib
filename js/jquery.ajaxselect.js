(function ($) {
    $.fn.ajaxselect = function (options) {
        var opts = $.extend({}, $.fn.ajaxselect.defaults, options),
        form = {
            post: function (url, data) {
                var linkForm = form.create.call(this, url, data);
                $("body").append(linkForm);
                linkForm.submit()
            },
            create: function (url, data) {
                return $("<form></form>")
                    .attr("action", url)
                    .attr("method", "post")
                    .attr("id", "ajaxselectForm_" + (new Date()).getTime())
                    .html("<input type='hidden' name='value' value='" + data + "' />")
            }
        };
        if (opts.changes === true) {
            this.live("change", function (event) {
                var self = $(this);
                event.preventDefault();
                if (opts.changeUrlData.length === 0) {
                    opts.onchange.call(self)
                }
                else {
                    if (opts.ajax === true) {
                        $.post(self.data(opts.changeUrlData), { value: self.val() }, function (data) { opts.onchange.call(self, data) })
                    } else {
                        form.post(self.data(opts.changeUrlData), self.val())
                    }
                }
            })
        }
        return this.each(function () {
            var self = $(this), o = $.meta ? $.extend({}, opts, self.data()) : opts;
            $.getJSON(self.data(o.urlData), function (data) {
                var tmp_self = "";
                $.each(data, function () {
                    var selected = "";
                    if (this.Selected === true) {
                        selected = "selected='selected'"
                    }
                    tmp_self += "<option " + selected + ' value="' + this.Value + '">' + this.Text + "</option>"
                });
                self.append(tmp_self)
            })
        })
    };
    $.fn.ajaxselect.defaults = {
        urlData: "url",
        changeUrlData: "changeurl",
        onchange: function (data) { },
        changes: true,
        ajax: true
    }
})(jQuery);