var panel = $("#slide-panel");
function warning(message, animation, notify) {
	var displayinfo = $("div.error-panel", this);
	if (!displayinfo.length) {
		displayinfo = $('<div class="ui-widget wrapper ui-state-error ui-corner-all error-panel"><p><span class="ui-icon ui-icon-alert"></span><strong>Attention: </strong><span class="error-message"></span></p></div>').appendTo(this)
	}
	$("span.error-message", displayinfo).html(message);
	displayinfo.stop(true, true);
	if (animation === true) {
		displayinfo.show("pulsate")
	}
	if (notify === true) {
		$.pnotify({ pnotify_title: "Attention :", pnotify_text: message, pnotify_type: "error" })
	}
	return displayinfo
}
function info(message, animation, notify) {
	var displayinfo = $("div.info-panel", this);
	if (!displayinfo.length) {
		displayinfo = $('<div class="ui-widget wrapper ui-state-highlight ui-corner-all info-panel"><p><span class="ui-icon ui-icon-info"></span><strong>Pour Info: </strong><span class="info-message"></span></p></div>').appendTo(this)
	}
	$("span.info-message", displayinfo).text(message);
	displayinfo.stop(true, true);
	if (animation === true) {
		displayinfo.show("pulsate")
	}
	if (notify === true) {
		$.pnotify({ pnotify_title: "Pour info :", pnotify_text: message })
	}
	return displayinfo
}
function manageAjaxError(xhr, response) {
	var message = response.responseText.split("<title>")[1].split("</title>")[0];
	warning.call($("#message-panel"), message, true, true);
	grid.afterLoad()
}
var pageEvents = function (context) {
	context.ajaxifyLive = function () {
		grid.find("a.ajaxify").live("click", function (event) {
			event.preventDefault();
			grid.beforeLoad();
			grid.load($(this).attr("href"), grid.initAndAsync)
		})
	};
	context.confirm = function (title, text, href, callbacktext) {
		var dialoghtml = $("#dialog");
		if (!dialoghtml.length) {
			dialoghtml = $('<div id="dialog" style="display: none;"><p><span class="ui-icon ui-icon-alert confirm-icon"></span><span id="confirm-text"></span></p></div>').appendTo(grid)
		}
		$("#confirm-text").text(text);
		dialoghtml.dialog({ resizable: false, modal: true, title: title, buttons: { Oui: function () {
			grid.beforeLoad();
			$.post(href, function (data) {
				grid.html(data);
				grid.initAndAsync();
				info.call($("#message-panel"), callbacktext, true, true)
			});
			$(this).dialog("close");
			return false
		}, Non: function () {
			$(this).dialog("close")
		}
		}
		})
	};
	context.attachTache = function () {
		$("#attach-taches").click(function (event) {
			event.preventDefault();
			confirm("Rattacher toutes les t\u00e2ches ?", "Etes-vous s\u00fbr de vouloir rattacher toutes les t\u00e2ches ?", $(this).attr("href"), "Toutes les t\u00e2ches ont \u00e9t\u00e9 rattach\u00e9es.")
		})
	};
	context.dettachTacheLive = function () {
		grid.rows.find("td.cell-delete a").live("click", function (event) {
			event.preventDefault();
			var self = $(this);
			var title = self.parent().prevAll("td.cell-libelle").text();
			confirm("D\u00e9tacher la t\u00e2che " + title + " ?", "Etes-vous s\u00fbr de vouloir d\u00e9tacher la t\u00e2che " + title + " ?", self.attr("href"), "La t\u00e2che " + title + " a \u00e9t\u00e9 d\u00e9tach\u00e9e.")
		})
	};
	context.attachColorbox = function () {
		grid.rows.find("a.colorbox-link").live("click", function (event) {
			event.preventDefault();
			$.fn.colorbox({ href: $(this).attr("href"), title: $(this).attr("title") });
			return false
		})
	};
	context.updateChargeLive = function () {
		grid.rows.find("td.edit").live("change", function (event) {
			event.preventDefault();
			var self = $(this);
			var val = self.find("input").val();
			if (val.length === 0) {
				grid.reload();
				return false
			}
			grid.beforeLoad();
			var tacheid = self.parent().data("id"), date = self.data("date");
			$.post(grid.rows.data("update"), { tacheid: tacheid, date: date, value: val }, function (data) {
				grid.html(data);
				grid.initAndAsync()
			})
		})
	};
	context.gridKeybordNavLive = function () {
		grid.rows.live("keydown", function (event) {
			var self = $(event.target);
			switch (event.which) {
				case 37:
					self.blur();
					var nextcell = self.parent().prev("td.cell").children("input");
					if (nextcell.length == 0) {
						nextcell = self
					}
					nextcell.select();
					grid.storeSelect(nextcell);
					break;
				case 39:
					self.blur();
					var prevcell = self.parent().next("td.cell").children("input");
					if (prevcell.length == 0) {
						prevcell = self
					}
					prevcell.select();
					grid.storeSelect(prevcell);
					break;
				case 40:
					self.blur();
					var prevrowcell = self.parent().parent().nextAll("tr.tache").first().children("td.cell[data-date=" + self.parent().data("date") + "]").children();
					if (prevrowcell.length == 0) {
						prevrowcell = self
					}
					prevrowcell.select();
					grid.storeSelect(prevrowcell);
					break;
				case 38:
					self.blur();
					var nextrowcell = self.parent().parent().prevAll("tr.tache").first().children("td.cell[data-date=" + self.parent().data("date") + "]").children();
					if (nextrowcell.length == 0) {
						nextrowcell = self
					}
					nextrowcell.select();
					grid.storeSelect(nextrowcell);
					break
			}
		})
	};
	context.foldingRowsLive = function () {
		grid.rows.find("a.ajaxexpander").live("click", function (event) {
			event.preventDefault();
			var self = $(this), parentrow = self.parent().parent(), level = parentrow.data("level"), parentLevel = level - 1, childs = parentrow.nextUntil("tr[data-level=" + level + "]").filter("tr[data-level!='" + parentLevel + "']");
			if (parentrow.hasClass("fold")) {
				manageIconUncollapsed(self);
				parentrow.removeClass("fold");
				parentrow.addClass("unfold");
				if (childs.length == 0) {
					$.get(self.attr("href"), function (data) {
						parentrow.after(data);
						grid.rowInitAndAsync()
					})
				} else {
					if (grid.rows.find("tr.groupe").length > 0 && (childs.hasClass("fold") || childs.hasClass("unfold"))) {
						childs.each(function () {
							var rowChild = $(this);
							if (rowChild.hasClass("fold") || rowChild.hasClass("unfold")) {
								rowChild.show()
							}
							if (rowChild.hasClass("unfold")) {
								rowChild.nextUntil("tr.fold tr.unfold").show();
							}
						});
					}
					else {
						childs.show()
					}
					//$.get(grid.rows.data("state"), { state: "unfold", id: parentrow.data("id"), level: level })
				}
			} else {
				manageIconCollapsed(self);
				parentrow.removeClass("unfold");
				parentrow.addClass("fold");
				childs.hide();
				//$.get(grid.rows.data("state"), { state: "fold", id: parentrow.data("id"), level: level })
			}
		})
	};
	context.manageIconCollapsed = function (link) {
		link.text("D\u00e9plier");
		link.button("option", "icons", { primary: "ui-icon-plus" });
		link.button("refresh")
	};
	context.manageIconUncollapsed = function (link) {
		link.text("Plier");
		link.button("option", "icons", { primary: "ui-icon-minus" });
		link.button("refresh")
	};
	context.navCalendarLive = function () {
		$("#calendar").live("click", function (event) {
			event.preventDefault();
			var self = $(this), datepickerSelector = $("#datepicker");
			datepickerSelector.datepicker({ dateFormat: "dd-mm-yy", firstDay: 1, minDate: datepickerSelector.data("mindate"), onSelect: function (date, instance) {
				grid.beforeLoad();
				grid.load(self.attr("href") + "/" + date, grid.initAndAsync)
			}
			}, $.datepicker.regional["fr"]).datepicker("show")
		})
	};
	context.initAsync = function () {
		$.ajaxSetup({ cache: false });
		grid.ajaxError(manageAjaxError);
		//$("#grid-sub-bar").css("background", "url('/images/subbar.gif') repeat 0% 100%");
		$("select", "#login").ajaxselect({ ajax: false });
		grid.initAsync();
		panel.initAsync();
		ajaxifyLive();
		navCalendarLive();
		gridKeybordNavLive();
		foldingRowsLive();
		attachColorbox();
		updateChargeLive();
		dettachTacheLive();
		attachTache()
	}
} (this);
var gridEvents = function (context) {
	context.initAsync = function () {
		grid.validateWeek();
		grid.validateCra();
		grid.rowInitAsync();
		grid.selectTache()
	};
	context.storeSelect = function (input) {
		var td = input.parent(), tr = td.parent(), tacheid = tr.data("id"), index = tr.children("td").index(td);
		grid.data("focusindex", index);
		grid.data("focustacheid", tacheid)
	};
	context.selectTache = function () {
		var index = grid.data("focusindex") + 1, tacheid = grid.data("focustacheid");
		if (index !== undefined && tacheid !== undefined) {
			grid.find("tr.tache[data-id=" + tacheid + "] td:nth-child(" + index + ")").children().select()
		} else {
			grid.find("input.edit-input").first().select()
		}
	};
	context.validateWeek = function () {
		var body = $("#row-total");
		body.find("td.validate-by-day").each(function () {
			var self = $(this);
			if (self.text() != "1") {
				self.addClass("error-cell")
			}
		});
		if (body.find("td.error-cell").length > 0) {
			body.addClass("error")
		}
	};
	context.validateCra = function () {
		$.getJSON(grid.rows.data("invalidsdays"), function (data) {
			if (data.hasprev == false) {
				disableButton($("#prev-not-valid").find("a"))
			}
			if (data.hasnext == false) {
				disableButton($("#next-not-valid").find("a"))
			}
			if (data.count > 0) {
				var validationPanel = $("#cra-validation-panel"), message = " jours ne sont pas valides.";
				if (data.count == 1) {
					message = " jour n'est pas valide."
				}
				validationPanel.addClass("had-errors");
				warning.call(validationPanel, data.count + message, false, false).show("slide", { direction: $("#grid-outer-header").data("direction") })
			} else {
				$("#cra-validation-panel").removeClass("had-errors")
			}
		})
	};
	context.beforeLoad = function (data) {
		grid.rows.fadeTo(200, '0.33')
	};
	context.afterLoad = function (data) {
		grid.rows.fadeIn(200)
	};
	context.reload = function () {
		grid.beforeLoad();
		grid.load(grid.rows.data("reload"), grid.initAndAsync)
	};
	context.rowInitAsync = function () {
		grid.maskForInput()
	};
	context.maskForInput = function () {
		$.mask.definitions["#"] = "[012345]";
		grid.rows.find("input.edit-input").mask("#?,99")
	}
} (grid);
var panelEvents = function (context) {
	var bar = $("#grid-sub-bar");
	context.initAsync = function () {
		panel.ajaxify()
	};
	context.intiBtCancel = function () {
		panel.find(".bt-cancel").click(function (event) {
			event.preventDefault();
			event.stopPropagation();
			bar.find(".ui-state-activated").removeClass("ui-state-activated");
			panel.hideAndEmpty()
		})
	};
	context.init = function (callback) {
		panel.find(".button").button();
		$LAB.script("/Scripts/jquery.validate.min.js").wait().script("/Scripts/jquery.validate.unobtrusive.min.js").script("/Scripts/jquery.confirm-dialog.min.js").script("/Scripts/jquery.autocompletecached.min.js").script("/Scripts/localization/messages_fr.js").wait(function () {
			panel.intiBtCancel();
			callback.call(this)
		})
	};
	context.hideAndEmpty = function () {
		panel.slideUp("slow", function () {
			panel.empty()
		})
	};
	context.ajaxify = function () {
		bar.find("a.ajaxify-panel").click(function (event) {
			var self = $(this), isOpen = self.hasClass("ui-state-activated") === true, other = bar.find(".ui-state-activated"), otherOpen = other.length !== 0;
			event.preventDefault();
			event.stopPropagation();
			panel.stop(true, true);
			if (isOpen) {
				panel.hideAndEmpty();
				self.removeClass("ui-state-activated");
				return
			}
			if (otherOpen) {
				panel.fadeTo("slow", 0, function () {
					$.get(self.attr("href"), function (data) {
						panel.html(data).fadeTo("slow", 1);
						other.removeClass("ui-state-activated");
						self.addClass("ui-state-activated")
					})
				});
				return
			}
			$.get(self.attr("href"), function (data) {
				panel.html(data).slideDown("slow");
				self.removeClass("ui-state-activated");
				self.addClass("ui-state-activated")
			})
		})
	};
	context.autocomplete = function (selector) {
		selector.autocompletecached(selector.data("search"));
		return this
	};
	context.validate = function (form) {
		var validationInfo = $(form).data("unobtrusiveValidation");
		return !validationInfo || !validationInfo.validate || validationInfo.validate()
	};
	context.confirmAndPost = function () {
		var bt = $("#add"), form = panel.find("form");
		bt.confirmdialog({ validation: function () {
			return panel.validate(form)
		}, noLive: true, onYes: function (url) {
			grid.beforeLoad();
			$.post(form.attr("action"), form.serialize(), function (data) {
				if (data.success === true) {
					grid.reload();
					panel.hideAndEmpty();
					bar.find(".ui-state-activated").removeClass("ui-state-activated");
					info.call($("#message-panel"), data.message, true, true)
				} else {
					grid.afterLoad();
					warning.call($("#message-panel"), data.message, true, true)
				}
			})
		} 
		});
		return this
	}
} (panel);
var perimetre = $("#add-perimetre");
var perimetreEvents = function (context) {
	context.init = function () {
		panel.init(perimetre.initWithScripts)
	};
	context.initWithScripts = function () {
		panel.autocomplete($("#auto-groupe")).autocomplete($("#auto-domaine")).autocomplete($("#auto-projet")).autocomplete($("#auto-client")).confirmAndPost()
	}
} (perimetre);
var charge = $("#add-charge");
var chargeEvents = function (context) {
	context.init = function () {
		$("#add-perimetre-select").ajaxselect({ ajax: false, changes: false });
		panel.find(".datepicker").datepicker({ firstDay: 1, minDate: $("#datepicker").data("mindate") }, $.datepicker.regional["fr"]);
		panel.init(charge.initWithScripts)
	};
	context.initWithScripts = function () {
		panel.autocomplete($("#auto-activite")).confirmAndPost()
	}
} (charge);
var activite = $("#add-activite");
var activiteEvents = function (context) {
	context.init = function () {
		$("#add-perimetre-select").ajaxselect({ ajax: false, changes: false });
		panel.init(activite.initWithScripts)
	};
	context.initWithScripts = function () {
		panel.autocomplete($("#auto-activite")).confirmAndPost()
	}
} (activite);