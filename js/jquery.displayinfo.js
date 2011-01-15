(function($) {
    $.fn.displayinfo = function(options) {
        var opts = $.extend({}, $.fn.displayinfo.defaults, options);

        return this.each(function() {
            var self = this,
            o = $.meta ? $.extend({}, opts, self.data()) : opts,
            displayinfo = $(".DisplayInfo", self);

            if (!displayinfo.length && o.hide == false) {
                displayinfo = (self.displayinfo = $('<div></div>'))
		            .appendTo(self)
		            .addClass('DisplayInfo');
            }

            if (o.hide == true) {
                if (displayinfo.data('validation') == true) {
                    displayinfo.hide();
                }
                return self;
            }

            displayinfo.data('validation', o.validation);
            displayInfoCreated(displayinfo, o);

            return self;
        });
    };

    function displayInfoCreated(selfParent, options) {
        selfParent.removeClass("DisplayInfoAlert DisplayInfoInfo DisplayInfoConfirm DisplayInfoError DisplayInfoSuccess")
                .html(options.message)
                .stop(true, true)
                .show("pulsate");

        switch (options.type) {
            case "alert":
                selfParent.addClass("DisplayInfoAlert");
                break;
            case "confirm":
                selfParent.addClass("DisplayInfoConfirm");
                break;
            case "error":
                selfParent.addClass("DisplayInfoError");
                break;
            case "info":
                selfParent.addClass("DisplayInfoInfo");
                break;
            case "success":
                selfParent.addClass("DisplayInfoSuccess");
                break;
        };

        return selfParent;
    };

    $.fn.displayinfo.defaults = {
        message: "",
        type: "error",
        hide: false,
        validation: false
    };
})(jQuery);
function ValidationSummaryOnSubmit(validationGroup) {
    var container = $('#aspnetForm').find('span.DisplayInfoContainer');
    if (Page_IsValid) {
        container.displayinfo({ hide: true });
        return;
    }
    var messageInHtml = "", displayinfoheader = container.attr('displayinfoheader');
    if (displayinfoheader) {
        messageInHtml += "<span class='DisplayInfoHeader'>" + displayinfoheader + "</span>";
    }
    messageInHtml += "<ul>";
    $.each(Page_Validators, function() {
        var self = this;
        if (!self.isvalid && typeof (self.errormessage) == "string") {
            messageInHtml += "<li>" + self.errormessage + "</li>";
        }
    });
    messageInHtml += "</ul>";
    container.displayinfo({ message: messageInHtml, type: "error", validation: true });
}
function ValidatorOnChange(event) {
    if (!event) {
        event = window.event
    }
    Page_InvalidControlToBeFocused = null;
    var targetedControl;
    if (typeof event.srcElement != "undefined" && event.srcElement != null) {
        targetedControl = event.srcElement
    } else {
        targetedControl = event.target
    }
    var vals;
    if (typeof targetedControl.Validators != "undefined") {
        vals = targetedControl.Validators
    } else {
        if (targetedControl.tagName.toLowerCase() == "label") {
            targetedControl = document.getElementById(targetedControl.htmlFor);
            vals = targetedControl.Validators
        }
    }
    var i;
    for (i = 0; i < vals.length; i++) {
        ValidatorValidate(vals[i], null, event)
    }
    ValidatorUpdateIsValid()
    ValidationSummaryOnSubmit(null)
}