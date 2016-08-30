kendo.ui.plugin(kendo.ui.Menu.extend({
    init: function (element, options) {
        //BASE CALL TO WIDGET INITIALIZATION
        kendo.ui.Menu.fn.init.call(this, element, options);
        var dataItem = this.options.dataSource,
			items = $(element).children();
        this.setAttr(items, dataItem);
    },
    setAttr: function (items, dataItem) {
        for (var i = 0; i < items.length; i++) {
            var di = dataItem[i];
            if (di != undefined && di != null) {
                if (di.items != undefined && di.items.length > 0) {
                    if (di.title != undefined) {
                        $(items[i]).attr("title", dataItem[i].title);
                    }
                    this.setAttr($(items).children("ul").children("li"), di.items);
                }
                else {
                    if (di.title != undefined) {
                        $(items[i]).attr("title", dataItem[i].title);
                    }
                }
            }
        }
    },
    options: {
        name: 'ExtMenu'
    }
}));
/*
*
* ExtDropDownTreeView
*
*/
var ExtDropDownTreeView = kendo.ui.Widget.extend({
    _uid: null,
    _treeview: null,
    _dropdown: null,

    init: function (element, options) {
        var that = this;

        kendo.ui.Widget.fn.init.call(that, element, options);

        that._uid = new Date().getTime();

        if ($(that.element).is("input")) {
            $(that.element).after(kendo.format("<div id='extTreeView{0}' class='k-ext-treeview' style='z-index:10;'/>", that._uid));
        }
        else {
            $(that.element).append(kendo.format("<input id='extDropDown{0}' class='k-ext-dropdown'/>", that._uid));
            $(that.element).append(kendo.format("<div id='extTreeView{0}' class='k-ext-treeview' style='z-index:10;'/>", that._uid));
        }

        // Создание dropdown.
        that._dropdown = $(that.element).kendoDropDownList(options.dropDownList).data("kendoDropDownList");
        that._dropdown.bind("open", function (e) {
            e.preventDefault();
            var element = that._dropdown.popup.wrapper[0] ? that._dropdown.popup.wrapper : that._dropdown.popup.element;
            element.remove();
            // Если treeview невидимо, то отобразить.
            if (!$treeviewRootElem.hasClass("k-custom-visible")) {
                $(that._treeview.element).css("width", "auto");
                // Отобразить treeview.
                $treeviewRootElem.slideToggle('fast', function () {
                    that._dropdown.close();
                    $treeviewRootElem.addClass("k-custom-visible");
                });
            }
            return false;
        }
        );

        if (options.dropDownWidth) {
            that._dropdown._inputWrapper.width(options.dropDownWidth);
        }

        var $dropdownRootElem = $(that._dropdown.element).closest("span.k-dropdown");

        // Создание treeview.
        that._treeview = $(kendo.format("#extTreeView{0}", that._uid)).kendoTreeView(options.treeview).data("kendoTreeView");
        that._treeview.bind("select", function (e) {
            // По нажатию на узел дерева, отобразить текст и задать значени выпадающего списка. После скрыть дерево.
            $dropdownRootElem.find("span.k-input").text(that._treeview.dataItem(e.node).Name);
            $dropdownRootElem.find("span.k-input").val(that._treeview.dataItem(e.node).ID);

            that.value(that._treeview.dataItem(e.node).ID);
            $treeviewRootElem.slideToggle('fast', function () {
                $treeviewRootElem.removeClass("k-custom-visible");
                that.trigger("select", e);
                that.trigger("change");
            });
            $treeviewRootElem.removeClass("k-custom-visible");
            that.trigger("select", e);
            that.element.trigger("change")
        });
        if (options.height) {
            $(that._treeview.element).css("max-height", options.height);
        }
        if (options.width) {
            $(that._treeview.element).css("min-width", options.width);
        }

        var $treeviewRootElem = $(that._treeview.element).closest("div.k-treeview");

        // Спрятать treeview.
        $treeviewRootElem
            .width($dropdownRootElem.width())
            .css({
                "border": "1px solid grey",
                "display": "none",
                "margin": "auto",
                "position": "fixed",
                "background-color": that._dropdown.list.css("background-color")
            });

        //Событие очистки фильтра
        $(that.element.parent()).parent().find("[type='reset']").click(function () {
            $dropdownRootElem.find("span.k-input").text(options.dropDownList.optionLabel);
            that._treeview.select($());
        });

        $(document).click(function (e) {
            // Игнорировать нажатия в treeview.
            if ($(e.target).closest("div.k-treeview").length == 0) {
                // Если видим, то закрыть treeview.
                if ($treeviewRootElem.hasClass("k-custom-visible")) {
                    $treeviewRootElem.slideToggle('fast', function () {
                        $treeviewRootElem.removeClass("k-custom-visible");
                    });
                }
            }
        });
    },
    dropDownList: function () {
        return this._dropdown;
    },
    treeview: function () {
        return this._treeview;
    },
    options: {
        name: "ExtDropDownTreeView"
    },
    _update: function (value) {
        var that = this;
        that._value = value;
        that.element.val(value);
    },
    events: ["change"],
    value: function (value) {
        var that = this;

        if (value === undefined) {
            return that._value;
        }
        that._update(value);
        that._old = that._value;
    }

});
kendo.ui.plugin(ExtDropDownTreeView);