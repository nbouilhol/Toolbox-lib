//Thead
(function(a){var k=navigator.userAgent,o=a(window),l=a(document),i=[],m=!(a.browser.msie&&parseFloat(k.substr(k.indexOf("MSIE")+4))<7),f=null,n=function(b){b=parseInt(b,10);return isNaN(b)?0:b},h=function(){a(i).each(function(){var b,c=a("table",this.parent().prev()).get(0),g=a("caption",c);a(c).css("border-collapse");var j=a("thead tr th, thead tr td",c),d=l.scrollTop()-a(c).offset().top+4;if(g.length)d-=g.get(0).clientHeight;a("thead tr th, thead tr td",this).each(function(e){e=j.eq(e).get(0);b=
a(e).css("width");a(this).css("width",b!="auto"?b:e.clientWidth-n(a(e).css("padding-left"))-n(a(e).css("padding-right"))+"px")});a(this).css({display:d>4&&d<c.clientHeight-a("tr:last",c).height()-8?a(c).css("display"):"none",left:a(c).offset().left-l.scrollLeft()+"px",width:a(c).get(0).offsetWidth})})};a(function(){if(m){o.scroll(h).resize(function(){f||(f=setInterval(function(){if(f)f=clearInterval(f);h()},50))});a(document).ajaxComplete(function(){a.thead.update()});a(".jquery-thead").thead()}});
a.thead=function(){return{update:function(){a(i).each(function(){var b=a("thead",a("table.jquery-thead, table",this.parent().prev()).get(0)),c=a("thead",this);if(c.html()!=b.html()){c.parent().append(b.clone(true));c.remove();h()}})}}}();a.fn.thead=function(){m&&a(this).each(function(){var b=this.tagName.toLowerCase()=="table"?a(this):a("table",this),c=b.parent(),g=a("thead",b);if(g.length){var j=b.attr("class"),d=b.attr("cellpadding");b=b.attr("cellspacing");i.push(a("<table />").attr("class",j).attr("cellpadding",
d?d:1).attr("cellspacing",b?b:2).css({position:"fixed",top:0}).appendTo(a("<"+c.get(0).tagName+"/>").attr("class",c.attr("class")).insertAfter(c)).append(a(g).clone(true)))}});h()}})(jQuery);
$(function () {
    init();
    $LAB.script("/Scripts/jquery.maskedinput.min.js")
    .script("/Scripts/jquery.pnotify.min.js")
    .script("/Scripts/jquery.colorbox.min.js")
    .script("/Scripts/jquery.ajaxselect.min.js")
    .script("/Scripts/jquery.cra.suivicra.async.min.js")
    .script("/Scripts/i18n/jquery.ui.datepicker-fr.min.js")
    .wait(function () {
        initAsync()
    })
});
var grid = $("#grid");
var pageFunctions = function (context) {
    context.init = function () {
        $(".needjs").removeClass("needjs");
        $("#grid-sub-bar").find("a.ajaxify-panel").removeClass("no-js ui-button-text").button({ icons: { secondary: "ui-icon-triangle-1-s"} });
        grid.init()
    };
    context.hidewarning = function (element) {
        $("div.error-panel", element).hide()
    };
    context.disableButton = function (element) {
        element.button("option", "disabled", true).attr("href", "#")
    }
} (this);
var gridFunctions = function (context) {
    var rows;
    context.init = function () {
        grid.rows = $("#gridtbody");
        $("#next").find("a").button({ icons: { primary: "ui-icon-seek-next" }, text: false });
        $("#prev-not-valid").find("a").button({ icons: { primary: "ui-icon-seek-start" }, text: false });
        $("#next-not-valid").find("a").button({ icons: { primary: "ui-icon-seek-end" }, text: false });
        $("#calendarContainer").find("a").button({ icons: { primary: "ui-icon-calendar" }, text: false });
        $("#today").find("a").button({ icons: { primary: "ui-icon-home"} });
        var prev = $("#prev").find("a");
        prev.button({ icons: { primary: "ui-icon-seek-prev" }, text: false });
        if (prev.data("forbidgoback") === "True") {
            disableButton(prev)
        }
        hidewarning($("#message-panel"));
        hidewarning($("#cra-validation-panel"));
        grid.rowInit(grid.rows)
    };
    context.initAndAsync = function () {
        grid.init();
        grid.initAsync()
    };
    context.rowInit = function () {
        grid.rows.fadeIn(200)
        grid.rows.find(".perimetre:even").removeClass("even").addClass("even");
        grid.rows.find(".tache:even").removeClass("even").addClass("even");
        grid.rows.find("a.delete-icon").button({ icons: { primary: "ui-icon-pin-s" }, text: false });
        grid.rows.find("tr.unfold").find(".ajaxexpander").button({ icons: { primary: "ui-icon-minus" }, text: false });
        grid.rows.find("tr.fold").find(".ajaxexpander").button({ icons: { primary: "ui-icon-plus" }, text: false })
    };
    context.rowInitAndAsync = function () {
        grid.rowInit();
        grid.rowInitAsync()
    }
} (grid);