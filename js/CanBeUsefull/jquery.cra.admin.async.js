var panel = $("#message-panel");
/*function manageAjaxError(xhr, response) {
    var message = response.responseText.split("<title>")[1].split("</title>")[0];
    warning.call("Error", message)
}*/
function warning(title, message) {
    $.pnotify({ pnotify_title: title, pnotify_text: message, pnotify_type: "error" });
}
function info(title, message) {
    $.pnotify({ pnotify_title: title, pnotify_text: message });
}
function hover(selector) {
    selector.live('mouseenter mouseleave', function (event) {
        if (event.type === 'mouseenter') {
            $(this).addClass("hover")
        } else {
            $(this).removeClass("hover")
        }
    });
}
var pageEvents = function (context) {
    context.initAsync = function () {
        $.ajaxSetup({ cache: false });
        //adminForm.ajaxError(manageAjaxError);
        hover($("ul.ui-tabs-nav").find("li"));
        //$("#grid-sub-bar").css("background", "url('/images/subbar.gif') repeat 0% 100%");
    };
    context.ajaxmessage = function (data) {
        data = jQuery.parseJSON(data);
        if (data.Type === "Warning") {
            $.pnotify({ pnotify_title: "Attention :", pnotify_text: data.Message, pnotify_type: "error" })
        } else {
            $.pnotify({ pnotify_title: "Pour Info :", pnotify_text: data.Message })
        }
    }
} (this);
var listEvents = function (context) {
    context.initAsync = function () {
        hover(list.find("tr"));
        $('.colorbox-link').find('a').live('click', function () {
            $.fn.colorbox({ href: $(this).attr('href'), title: $(this).attr('title') });
            return false;
        });
    };
} (list);
var adminFormEvents = function (context) {
    context.initEditAsync = function () {
        adminForm.initValidation();
        $("textarea").expandingTextArea();
        adminForm.find(".dialog-confirm").confirmdialog();
    };
    context.initValidation = function () {
        var validationbox = $("#validationSummary:has(ul)");
        if (validationbox.length) {
            validationbox.find("ul").before("<span class='ui-icon ui-icon-alert'></span><strong>Attention: </strong>");
            validationbox.addClass("ui-state-error ui-corner-all");
        }
    };
    context.initDeleteAsync = function () {
        adminForm.initValidation();
        adminForm.find(".dialog-confirm").confirmdialog();
    };
    context.initUploadAsync = function () {
        $("#ajaxUploadForm").show().fileUploadUI({
            uploadTable: $("#files"),
            downloadTable: $("#files"),
            buildUploadRow: function (files, index) {
                return $('<tr><td>' + files[index].name + '<\/td>' +
                '<td class="file_upload_progress"><div><\/div><\/td>' +
                '<td class="file_upload_cancel">' +
                '<button class="ui-state-default ui-corner-all" title="Cancel">' +
                '<span class="ui-icon ui-icon-cancel">Cancel<\/span>' +
                '<\/button><\/td><\/tr>');
            },
            onError: function (response) {
                var message = response.split("<title>")[1].split("</title>")[0];
                warning("L'importation a échoué", message);
            },
            buildDownloadRow: function (file) {
                return $('<tr><td>' + file.name + '<\/td><\/tr>');
            }
        });
    }
} (adminForm);
var panelEvents = function (context) {
    context.info = function (message) {
        if (message === "") { return; }
        this.stop(true, true).show("pulsate");
        info("Pour info :", message);
    };
} (panel);