function initListSort() {
    jQuery.fn.dataTableExt.oSort["fr_date-asc"] = function (a, b) {
        var x = convertDateForSort.call(a), y = convertDateForSort.call(b);
        return x < y ? -1 : x > y ? 1 : 0
    };
    jQuery.fn.dataTableExt.oSort["fr_date-desc"] = function (a, b) {
        var x = convertDateForSort.call(a), y = convertDateForSort.call(b);
        return x < y ? 1 : x > y ? -1 : 0
    };
    jQuery.fn.dataTableExt.oSort["progressbar-asc"] = function (a, b) {
        var x = convertProgressbarForSort.call(a), y = convertProgressbarForSort.call(b);
        return x < y ? -1 : x > y ? 1 : 0;
    };
    jQuery.fn.dataTableExt.oSort["progressbar-desc"] = function (a, b) {
        var x = convertProgressbarForSort.call(a), y = convertProgressbarForSort.call(b);
        return x < y ? 1 : x > y ? -1 : 0
    };
    jQuery.fn.dataTableExt.oSort['num-html-asc'] = function (a, b) {
        var x = convertLinkForSort.call(a), y = convertLinkForSort.call(b);
        return ((x < y) ? -1 : ((x > y) ? 1 : 0));
    };

    jQuery.fn.dataTableExt.oSort['num-html-desc'] = function (a, b) {
        var x = convertLinkForSort.call(a), y = convertLinkForSort.call(b);
        return ((x < y) ? 1 : ((x > y) ? -1 : 0));
    };
}
function convertLinkForSort() {
    return parseFloat(this.replace(/<.*?>/g, ""));
}
function convertDateForSort() {
    var frDate = this.split("/");
    return (frDate[2] + frDate[1] + frDate[0]) * 1;
}
function convertProgressbarForSort() {
    return parseFloat(this.replace(/<.*?>/g, "").replace(/,/, ".").split(" jour(s)")[0]);
}
function initProgressBar() {
    $("span.progressbar", "table").each(function () { $(this).progressBar({ boxImage: "/images/progressbar.gif", barImage: { 0: "/images/progressbg_red.gif", 30: "/images/progressbg_orange.gif", 70: "/images/progressbg_green.gif"} }) });
}