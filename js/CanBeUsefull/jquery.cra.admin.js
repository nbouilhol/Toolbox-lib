var listFunctions = function () {
    function convertLinkForSort(link) {
        return parseFloat(link.replace(/<.*?>/g, ""))
    }
    function convertDateForSort(date) {
        var frDate = date.split("/");
        return (frDate[2] + frDate[1] + frDate[0]) * 1
    }
    function convertProgressbarForSort(progressbar) {
        return parseFloat(progressbar.replace(/<.*?>/g, "").replace(/,/, ".").split(" jour(s)")[0])
    }
    this.init = function () {
        this.find(".button-plus").removeClass("hidden").button({ icons: { primary: "ui-icon-plus" }, text: false });
        this.find(".button-minus").removeClass("hidden").button({ icons: { primary: "ui-icon-minus" }, text: false });
        this.find(".button-delete").removeClass("hidden").button({ icons: { primary: "ui-icon-close" }, text: false });
        this.find(".button-email").removeClass("hidden").button({ icons: { primary: "ui-icon-mail-closed" }, text: false })
    };
    this.sort = function () {
        if (jQuery.fn.dataTableExt === undefined)
            return;
        jQuery.fn.dataTableExt.oSort["fr_date-asc"] = function (a, b) {
            var x = convertDateForSort(a), y = convertDateForSort(b);
            return x < y ? -1 : x > y ? 1 : 0
        };
        jQuery.fn.dataTableExt.oSort["fr_date-desc"] = function (a, b) {
            var x = convertDateForSort(a), y = convertDateForSort(b);
            return x < y ? 1 : x > y ? -1 : 0
        };
        jQuery.fn.dataTableExt.oSort["num-html-asc"] = function (a, b) {
            var x = convertLinkForSort(a), y = convertLinkForSort(b);
            return x < y ? -1 : x > y ? 1 : 0
        };
        jQuery.fn.dataTableExt.oSort["num-html-desc"] = function (a, b) {
            var x = convertLinkForSort(a), y = convertLinkForSort(b);
            return x < y ? 1 : x > y ? -1 : 0
        };
        jQuery.fn.dataTableExt.oSort["progressbar-asc"] = function (a, b) {
            var x = convertProgressbarForSort(a), y = convertProgressbarForSort(b);
            return x < y ? -1 : x > y ? 1 : 0
        };
        jQuery.fn.dataTableExt.oSort["progressbar-desc"] = function (a, b) {
            var x = convertProgressbarForSort(a), y = convertProgressbarForSort(b);
            return x < y ? 1 : x > y ? -1 : 0
        }
    };
    this.progressBar = function () {
        this.find("span.progressbar").each(function () {
            $(this).progressBar({ boxImage: "/images/progressbar.gif", barImage: { 0: "/images/progressbg_red.gif", 30: "/images/progressbg_orange.gif", 70: "/images/progressbg_green.gif"} })
        })
    };
};
var list = $("table.list-table");
listFunctions.call(list);
var adminFormFunctions = function () {
    this.initEdit = function () {
        $(":submit").button()
    };
    this.initDelete = function () {
        $(":submit").button()
    };
    this.initUpload = function () {
        $("#ajaxUploadForm").hide();
    }
};
var adminForm = $("div.form-wrapper");
adminFormFunctions.call(adminForm);
var pageFunctions = function () {
    this.init = function () {
        $(".needjs").removeClass("needjs")
    }
};
pageFunctions.call(this);
function datatable_fr() {
    return {
        sProcessing: "Traitement en cours...",
        sLengthMenu: "Afficher _MENU_ \u00e9l\u00e9ments",
        sZeroRecords: "Aucun \u00e9l\u00e9ment \u00e0 afficher",
        sInfo: "Affichage de l'\u00e9lement _START_ \u00e0 _END_ sur _TOTAL_ \u00e9l\u00e9ments",
        sInfoEmpty: "Affichage de l'\u00e9lement 0 \u00e0 0 sur 0 \u00e9l\u00e9ments",
        sInfoFiltered: "(filtr\u00e9 de _MAX_ \u00e9l\u00e9ments au total)",
        sInfoPostFix: "",
        sSearch: "Rechercher&nbsp;:",
        sUrl: "",
        oPaginate: {
            sFirst: "Premier",
            sPrevious: "Pr\u00e9c\u00e9dent",
            sNext: "Suivant", 
            sLast: "Dernier"
        }
    }
};
$(function () {
    init();
    if (list.length !== 0) {
        list.sort()
    }
});