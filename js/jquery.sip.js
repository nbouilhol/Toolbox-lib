$(function () {
    $(".button-ui").button();
    $("a.button-ui-alert").button({ icons: { primary: "ui-icon-alert"} });
});
$(window).load(function () {
    $.ajaxSetup({ cache: false, error: manageAjaxError });
});
function manageAjaxError(xhr, status) {
    var message = xhr.responseText.split("<title>");
    if (message.length > 1) {
        message = message[1].split("</title>");
        if (message.length > 0) {
            message = message[0];
            $.pnotify({ pnotify_title: "Attention :", pnotify_text: message, pnotify_type: "error" });
        }
    }
}
function Notify(message, type) {
    this.stop(true, true).show("pulsate");
    if (type == "Warning") {
        warning.call(this, "Attention :", message);
    } else {
        info.call(this, "Pour Info :", message);
    }
}
function warning(title, message) {
    $.pnotify({ pnotify_title: title, pnotify_text: message, pnotify_type: "error" });
}
function info(title, message) {
    $.pnotify({ pnotify_title: title, pnotify_text: message });
}
function showNotification(notificationType, title, body, url) {
    if (window.webkitNotifications.checkPermission() == 0) {
        var notification;
        if (notificationType == "simple") {
            notification = window.webkitNotifications.createNotification(null, title, body)
        } else {
            if (notificationType == "html") {
                notification = window.webkitNotifications.createHTMLNotification(url)
            }
        }
        notification.show();
        setTimeout(function () {
            notification.cancel()
        }, 5E3)
    } else {
        window.webkitNotifications.requestPermission()
    }
}
function tooltip() {
    this.qtip({
        position: {
            my: 'top left',
            target: 'mouse',
            viewport: $(window),
            adjust: {
                x: 10, y: 10
            }
        },
        hide: {
            fixed: true
        },
        style: {
            classes: 'ui-tooltip-light ui-tooltip-shadow ui-tooltip-rounded',
            widget: true
        }
    });
}
$.fn.autocompletecached = function (url, select) {
    var cache = {}, lastXhr;
    this.autocomplete({ source: function (request, response) {
        var term = request.term;
        if (term in cache) {
            response(cache[term]);
            return
        }
        lastXhr = $.getJSON(url, request, function (data, status, xhr) {
            cache[term] = data;
            if (xhr === lastXhr) {
                response(data)
            }
        })
    }, minLength: 1, select: function (event, ui) {
        if (select)
            select.call(this)
    }
    })
};