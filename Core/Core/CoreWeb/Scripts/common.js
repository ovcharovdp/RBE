var asu = {
    baseUrl: '',
    Url: function (e) { return this.baseUrl + e },
    getErr: function (r) {
        if (r.responseJSON) {
            return (r.responseJSON["odata.error"]) ? r.responseJSON["odata.error"].message.value : r.responseJSON.msg;
        }
        else if ($("<div>").html(r.responseText).find("#wrap_error").length > 0)
            return $("<div>").html(r.responseText).find("#wrap_error").html()
        else
            return r.responseText
    },
    notification: null
}
asu.common = {
    simpleTree: function (a) {
        var c = (a.controller == undefined) ? "" : a.controller + "/";
        var opt = {
            dataSource: new kendo.data.HierarchicalDataSource({
                transport: { read: { url: c + "GetGroups", dataType: "json", type: "POST", data: { baseID: a.baseGroup } } },
                schema: { model: { id: "ID", hasChildren: "HasCld" } },
                error: function (r) { showError(r.xhr); }
            }),
            dataTextField: "Name"
        }
        if (a.change != undefined) opt.change = a.change;
        var t = a.container.kendoTreeView(opt).data('kendoTreeView');
        t.curItem = function () {
            if (t.select().length == 0) return undefined;
            return t.dataItem(t.select());
        }
        t.curID = function () {
            if (t.select().length == 0) return -1;
            return t.dataItem(t.select()).ID;
        }
        t.newGroup = function (id) {
            saveEntity(asu.Url("Group/New"), function (r) { a.popup.close(); var x = t.dataItem(t.select()); var l = x.loaded(); x.append(r); x.loaded(l) }, { parentID: id });
        }
        t.editGroup = function (id) {
            saveEntity(asu.Url("Group/Edit"), function (r) { a.popup.close(); var x = t.select(); t.text(x, r.Name); t.dataItem(x).Name = r.Name }, { id: id });
        }
        t.add = function () {
            var g = this.curItem();
            if (g == undefined) { asu.notification.info("Необходимо выбрать группу."); return; }
            a.popup.show("Группа", asu.Url("Group/PopupNew"), { parentID: g.ID });
        }
        t.del = function () {
            var g = this.curItem();
            if (g == undefined) return;
            delObject("группу", asu.Url("Group/Del"), g.ID, function () { t.remove(t.select()) });
        }
        t.edit = function () {
            var g = this.curItem();
            if (g == undefined) return;
            a.popup.show("Редактировать группу", asu.Url("Group/PopupEdit"), { id: g.ID });
        }
        return t;
    },
    tree: function (e) {
        var c = (e.controller == undefined) ? "" : e.controller + "/";
        var dg = this.simpleTree(e);
        dg.newElement = function (id) {
            saveEntity(c + "New", function (r) { e.popup.close(); var c = dg.dataItem(dg.select()); var l = c.loaded(); c.append(r); c.loaded(l) }, { parentID: id });
        }
        dg.editElement = function (id) {
            saveEntity(c + "Edit", function (r) { e.popup.close(); var t = dg.select(); dg.text(t, r.Name); dg.dataItem(t).Name = r.Name }, { id: id });
        }
        dg.add = function () {
            var g = this.curItem();
            if (g == undefined || g.f != 0) { asu.notification.info("Необходимо выбрать группу."); return; }
            e.popup.show("Группа", asu.Url("Group/PopupNew"), { parentID: g.ID });
        }
        dg.add_o = function () {
            var g = this.curItem();
            if (g == undefined || g.f != 0) { asu.notification.info("Необходимо выбрать группу."); return; }
            e.popup.show("Добавить " + e.entityName, c + "PopupNew", { parentID: g.ID });
        }
        dg.del = function () {
            var g = this.curItem();
            if (g == undefined) return;
            if (g.f == undefined || g.f == 0)
                delObject("группу", asu.Url("Group/Del"), g.ID, function () { dg.remove(dg.select()) });
            else {
                delObject(e.entityName, c + "Del", g.ID, function () { dg.remove(dg.select()) });
            }
        }
        dg.edit = function () {
            var g = this.curItem();
            if (g == undefined) return;
            if (g.f == undefined || g.f == 0) {
                e.popup.show("Редактировать группу", asu.Url("Group/PopupEdit"), { id: g.ID });
            } else {
                e.popup.show("Редактировать " + e.entityName, c + "PopupEdit", { id: g.ID });
            }
        }
        return dg;
    },
    modal: function (a) {
        var c = (a.container == undefined) ? $("#editDialog") : a.container;
        var w = c.kendoWindow({
            width: "615px",
            modal: true,
            visible: false,
            open: function () { this.center() },
            error: function (e) { this.content(asu.getErr(e.xhr)) }
        }).data('kendoWindow');
        w.show = function (t, u, d) {
            this.title(t);
            this.content("");
            this.refresh({ url: u, type: "POST", data: d, complete: function () { this.open() }, context: this });
        }
        return w;
    },
    menuData: function (e) {
        if (typeof e != "string") e = "Объект";
        return [{ text: "", spriteCssClass: "i-add", items: [{ code: "add_o", text: e }, { code: "add", text: "Группу" }] },
                { code: "edit", text: "", spriteCssClass: "i-edit", title: "Редактировать" }, { code: "del", text: "", spriteCssClass: "i-del", title: "Удалить" }, { code: "role", text: "", spriteCssClass: "key", title: "Настройка доступа" }]
    },
    menu: function (a) {
        var t = (a.container == undefined) ? $("#menu") : a.container;
        return t.kendoExtMenu(
            {
                select: function onSelect(e) {
                    var item = $(e.item),
                        me = item.closest(".k-menu"),
                        di = this.options.dataSource,
                        index = item.parentsUntil(me, ".k-item").map(function () { return $(this).index() }).get().reverse();
                    index.push(item.index());
                    for (var i = -1, len = index.length; ++i < len;) {
                        di = di[index[i]];
                        di = i < len - 1 ? di.items : di;
                    }
                    if (di.code != undefined) { if (di.code == "menu") { showMainMenu(); } else { if (a.owner[di.code]) a.owner[di.code](); else if (a.owner.def) a.owner.def(di) } }
                },
                dataSource: a.items
            })
    },
    mainMenu: function (a) {
        if ($('#main-menu').length > 0) a.items.unshift({ code: "menu", text: "", spriteCssClass: "i-menu", title: "Главное меню" });
        return this.menu(a);
    },
    toolbar: function (a) {
        var t = (a.container) ? a.container : $("#menu");
        t.kendoToolBar({
            resizable: false,
            click: function (e) { if (e.id) { if (e.id == "menu") { showMainMenu(); } else { if (a.owner[e.id]) a.owner[e.id](e); else if (a.owner.def) a.owner.def(e) } } },
            toggle: function (e) { if (e.id) { if (a.owner[e.id]) a.owner[e.id](e); else if (a.owner.def) a.owner.def(e) } },
            items: a.items
        });
    },
    mainToolbar: function (a) {
        if ($('#main-menu').length > 0) a.items.unshift({ id: "menu", type: "button", spriteCssClass: "i-menu", attributes: { "title": "Главное меню" } });
        return this.toolbar(a);
    },
    flatGrid: function (a) {
        var w = (a.popup == undefined) ? asu.common.modal() : a.popup;
        var c = (a.controller == undefined) ? "" : a.controller + "/";
        var ds = {
            type: "odata",
            transport: {
                read: { url: asu.Url("odata/") + a.entity + ((a.expand) ? "?$expand=" + a.expand : ""), dataType: "json" }
            },
            schema: {
                model: { id: "ID", fields: a.fields },
                data: function (r) { if (r.value !== undefined) return r.value; else { delete r["odata.metadata"]; return r; } },
                total: function (r) { return r["odata.count"] }
            },
            push: function (e) {
                if (e.type == "update") {
                    var f = this.options.schema.model.fields;
                    var kk = Object.keys(f);
                    kk.forEach(function (ee) {
                        if (f[ee].type == "date" && e.items[0][ee]) e.items[0].set(ee, kendo.parseDate(e.items[0][ee]))
                    });
                }
            },
            pageSize: 50,
            serverPaging: true,
            serverFiltering: true,
            serverSorting: true
        };
        if (a.filter != undefined) ds.filter = a.filter;
        var opt = {
            dataSource: ds,
            pageable: true,
            sortable: { mode: "single" },
            filterable: { extra: false },
            excel: { allPages: true },
            //navigatable: true,
            //  resizable: true,
            selectable: true,
            columns: a.columns
            //,dataBound: function () {
            //    dataView = this.dataSource.view();
            //    for (var i = 0; i < dataView.length; i++) if (dataView[i].IsManual == 1) this.content.find("tbody tr[data-uid=" + dataView[i].uid + "]").addClass("is-manual");
            //}
        };
        if (a.dataBound != undefined) opt.dataBound = a.dataBound;
        if (a.toolbar != undefined) opt.toolbar = kendo.template(a.toolbar.html());
        var g = a.container.kendoGrid(opt).data("kendoGrid");
        g.del = function () {
            if (g.select().length > 0) delObject("элемент", c + "Del", g.curID(), function () { g.removeRow(g.select()) });
        }
        g.edit = function () {
            if (g.select().length > 0) w.show("Редактировать", c + "PopupEdit", { id: g.curID() });
        }
        g.add = function () {
            w.show("Новый элемент", c + "PopupNew");
        }
        g.reload = function () { g.dataSource.read() }
        $(".k-selectable").dblclick(g.edit);
        g.curID = function () { if (this.select().length == 0) return -1; return this.dataItem(this.select()).ID }
        g.curItem = function () {
            if (this.select().length == 0) return undefined;
            return this.dataItem(this.select());
        }
        g.saveAsExcel = function () {
            var n = g.options.excel || {},
                     i = new kendo.ExcelExporter({
                         columns: a.columnsExcel,
                         dataSource: g.dataSource,
                         allPages: n.allPages,
                         filterable: n.filterable,
                         hierarchy: n.hierarchy
                     });
            var e = window.kendo.jQuery;
            i.workbook().then(e.proxy(function (e, i) {
                if (!this.trigger("excelExport", {
                    workbook: e,
                    data: i
                })) {
                    var o = new kendo.ooxml.Workbook(e);
                    kendo.saveAs({
                        dataURI: o.toDataURL(),
                        fileName: e.fileName || n.fileName,
                        proxyURL: n.proxyURL,
                        forceProxy: n.forceProxy
                    })
                }
            }, g))
        }
        return g
    },
    linkGrid: function (a) {
        var ent = (a.entity == undefined) ? "" : a.entity;
        var w = (a.popup == undefined) ? asu.common.modal() : a.popup;
        var c = (a.controller == undefined) ? "" : a.controller + "/",
            opt = {
                sortable: true,
                filterable: { extra: false },
                selectable: true,
                dataSource: {
                    type: "json",
                    transport: { read: { url: c + "Get" + ((ent == "") ? "Data" : ent), dataType: "json", type: "POST", data: { groupID: function () { return a.group().ID } } } },
                    schema: { model: { id: "ID", fields: a.fields } },
                    error: function (r) { showError(r.xhr); }
                },
                columns: a.columns
            };
        if (a.toolbar != undefined) opt.toolbar = kendo.template(a.toolbar.html());
        var g = a.container.kendoGrid(opt).data("kendoGrid");
        g.del = function () {
            if (this.select().length == 0) return;
            extDelObject(a.entityName, c + "Del" + ent, { groupID: a.group().ID, id: g.dataItem(g.select()).ID }, function () { g.removeRow(g.select()) });
        }
        g.add = function () {
            if (a.group().ID == 0 || a.group().f == 0) return;
            w.show("Добавить " + a.entityName, c + "PopupAdd" + ent, { parentID: a.group().ID });
        }
        return g;
    },
    groupedGrid: function (a) {
        var w = a.popup;
        var en = (a.entityName == undefined) ? "объект" : a.entityName;
        var c = (a.controller == undefined) ? "" : a.controller + "/",
            opt = {
                sortable: true,
                filterable: { extra: false },
                selectable: true,
                dataSource: {
                    type: "json",
                    transport: { read: { url: c + "GetData", dataType: "json", type: "POST", data: { groupID: function () { return a.group().ID } } } },
                    schema: { model: { id: "ID", fields: a.fields } },
                    error: function (r) { showError(r.xhr); }
                },
                columns: a.columns
            };
        if (a.toolbar != undefined) opt.toolbar = kendo.template(a.toolbar.html());
        var g = a.container.kendoGrid(opt).data("kendoGrid");
        g.del = function () {
            if (this.select().length == 0) return;
            extDelObject(en, c + "Del", { id: g.dataItem(g.select()).ID }, function () { g.removeRow(g.select()) });
        }
        g.add = function () {
            if (a.group().ID == 0) return;
            w.show("Добавить " + en, c + "PopupNew", { parentID: a.group().ID });
        }
        g.edit = function () {
            if (a.group().ID == 0) return;
            w.show("Редактировать " + en, c + "PopupEdit", { id: g.dataItem(g.select()).ID });
        }
        g.newElement = function (i) {
            saveEntity(c + "New", function (r) { w.close(); g.dataSource.add(r) }, { parentID: i });
        }
        g.editElement = function (i) {
            saveEntity(c + "Edit", function (r) { w.close(); g.dataSource.pushUpdate(r) }, { id: i });
        }
        g.curID = function () { if (!g.select().length) return -1; return g.dataItem(g.select()).ID }
        g.curItem = function () {
            if (g.select().length == 0) return undefined;
            return g.dataItem(g.select());
        }
        return g;
    }
}

asu.catalog = {
    catalogModel: function (a) {
        this._model = new kendo.observable({
            treeDS: new kendo.data.HierarchicalDataSource({
                transport: {
                    read: {
                        url: a.url + "/GetGroups", dataType: "json", type: "POST",
                        data: { baseID: a.rootID }
                    },
                },
                schema: { model: { id: "ID", hasChildren: "HasCld" } },
                error: function (r) { showError(r.xhr); }
            }),
            selectedGroup: null,
            changeGroup: function (e) {
                this.selectedGroup = e.sender.dataItem(e.sender.select());
                this.selectedElement = null
                this.elements.read({ groupID: this.selectedGroup.ID });
            },
            selectElement: function (e) {
                this.selectedElement = e.sender.dataItem(e.sender.select());
            },
            selectedElement: null,
            newElement: function () {
                saveEntity(a.url + "/New" + (this.selectedGroup.f == "0" ? "" : this.selectedGroup.f),
                    function (r) {
                        a.popup.close();
                        if (r.f) {
                            var c = this.selectedGroup;
                            var l = c.loaded(); c.append(r); c.loaded(l)
                        } else {
                            this.elements.add(r)
                        }
                    }, { parentID: this.selectedGroup.ID }, this);
            },
            editGroup: function () {
                if (this.selectedGroup)
                    saveEntity(asu.Url("Group/Edit"), function (r) { a.popup.close(); this.treeDS.pushUpdate(r) }, { id: this.selectedGroup.ID }, this);
            },
            editElement: function () {
                if (this.selectedElement) {
                    saveEntity(a.url + "/Edit", function (r) { a.popup.close(); this.elements.pushUpdate(r) }, { id: this.selectedElement.ID }, this);
                } else {
                    if (!this.selectedGroup || this.selectedGroup.f == "0") return;
                    saveEntity(a.url + "/Edit" + this.selectedGroup.f, function (r) { a.popup.close(); this.treeDS.pushUpdate(r) }, { id: this.selectedGroup.ID }, this);
                }
            },
            newGroup: function () {
                saveEntity(asu.Url("Group/New"), function (r) { a.popup.close(); var x = this.selectedGroup; var l = x.loaded(); x.append(r); x.loaded(l) }, { parentID: this.selectedGroup.ID }, this);
            },
            edit: function () {
                if (this.selectedElement) {
                    a.popup.show("Редактировать ", a.url + "/PopupEdit", { id: this.selectedElement.ID });
                } else {
                    if (!this.selectedGroup) return;
                    if (this.selectedGroup.f == "0") {
                        a.popup.show("Редактировать группу", asu.Url("Group/PopupEdit"), { id: this.selectedGroup.ID });
                    }
                    else {
                        a.popup.show("Редактировать ", a.url + "/PopupEdit" + this.selectedGroup.f, { id: this.selectedGroup.ID });
                    }
                }
            },
            delElement: function () {
                if (this.selectedElement) {
                    extDelObject("поле", a.url + "/Del", { id: this.selectedElement.ID }, function () { this.elements.remove(this.selectedElement) }, this);
                } else {
                    if (!this.selectedGroup) return;
                    if (this.selectedGroup.f == "0") {
                        delObject("группу", asu.Url("Group/Del"), this.selectedGroup.ID, function () { this.treeDS.remove(this.selectedGroup) }, this);
                    }
                    else {
                        delObject("журнал", a.url + "/Del" + this.selectedGroup.f, this.selectedGroup.ID, function () { this.treeDS.remove(this.selectedGroup); this.elements.data([]) }, this);
                    }
                }
            },
            addObject: function () {
                if (!this.selectedGroup) return;
                a.popup.show("Добавить", a.url + "/PopupNew" + (this.selectedGroup.f == "0" ? "" : this.selectedGroup.f), { parentID: this.selectedGroup.ID });
            },
            addGroup: function () {
                if (!this.selectedGroup || this.selectedGroup.f != "0") return;
                a.popup.show("Группа", asu.Url("Group/PopupNew"), { parentID: this.selectedGroup.ID });
            },
            cmd: function (e) {
                switch (e.id) {
                    case "menu": showMainMenu(); break;
                    case "add_o": this.addObject(); break;
                    case "add": this.addGroup(); break;
                    case "edit": this.edit(); break;
                    case "del": this.delElement(); break;
                    default:
                }
            },
            elements: new kendo.data.DataSource({
                transport: {
                    read: { type: "POST", url: a.url + "/GetData", dataType: "json" }
                },
                schema: { model: { id: "ID", fields: a.fields } },
                error: function (r) { showError(r.xhr); }
            })
        })
        var m = this._model;
        return m;
    }
}
asu.role = {
    access: {
        popup: function (id, name) {
            if (id != undefined && typeof id == "number" && id > 0)
                editWnd.show(name != undefined ? name : "", asu.Url("Role/PopupSetRoleObject"), { id: id });
        },
        model: function (id) {
            if (!Boolean(this._model)) {
                this._model = kendo.observable({
                    ID: id,
                    checked: true,
                    unchecked: true,
                    notifVisible: true,
                    roleName: "",
                    roles: new kendo.data.DataSource({
                        transport: {
                            read: { url: asu.Url("Role/GetRolesObject"), dataType: "json", type: "POST", data: { id: id } }
                        },
                        error: function (r) { showError(r.xhr); }
                    }),
                    setParamVal: function (s) {
                        this.checkChecked();
                        this.roles.filter(this.roles.filter());
                    },
                    checkChecked: function (s) {
                        if (this.roles._data.filter(function (ct) { return ct.Checked == true }).length == 0) this.set("notifVisible", true)
                        else this.set("notifVisible", false)
                    },
                    filterInput: function (e) { this.filterListView(e.target.value, this.checked, this.unchecked); },
                    filterChecked: function (e) { this.filterListView(this.roleName, e.target.checked, this.unchecked); },
                    filterUnchecked: function (e) { this.filterListView(this.roleName, this.checked, e.target.checked); },
                    filterListView: function (_input, _checked, _unchecked) {
                        this.roles.filter({
                            logic: "and",
                            filters: [{ field: "Name", operator: "contains", value: _input },
                                { logic: "or", filters: [{ field: "Checked", operator: "equals", value: _checked }, { field: "Checked", operator: "equals", value: !_unchecked }] }]
                        });
                    },
                    setObjectRoles: function () {
                        var ext = { id: this.ID, roles: this.roles._data.filter(function (ct) { return ct.Checked }).map(function (e) { return e.ID }) };
                        $.ajax({
                            url: asu.Url("Role/SetObjectRoles"),
                            type: "POST",
                            dataType: "json",
                            contentType: "application/json, charset=\"utf-8\"",
                            data: JSON.stringify(ext),
                            success: function (r) { editWnd.close() },
                            error: function (r) { showError(r) }
                        });
                    },
                    close: function () {
                        editWnd.close();
                    }
                });
            }
            else {
                this._model.ID = id;
                this._model.roles.transport.options.read.data.id = id;
                this._model.roles.read();
            }
            return this._model;
        }
    }
}
asu.docFlow = {
    docFlowModel: function (a) {
        this._model = new kendo.observable({
            _container: null,
            grid: function () {
                if (this._container == null)
                    this._container = a.container != null ? a.container.data("kendoGrid") : $("#grid").data("kendoGrid");
                return this._container;
            },
            _toolbar: null,
            toolbar: function () {
                if (this._toolbar == null)
                    this._toolbar = a.toolbar != null ? a.toolbar.data("kendoToolBar") : $("#toolbar").data("kendoToolBar");
                return this._toolbar;
            },
            dataBound: function (e) {
                var rows = e.sender.tbody.children(), state;
                for (var j = 0; j < rows.length; j++) {
                    state = e.sender.dataItem(rows[j]).get("State.ID");
                    if (state != null) {
                        $(rows[j]).addClass("gridDF_row_" + state);
                    }
                }
            },
            selectElement: function (e) {
                this.selectedElement = e.sender.dataItem(e.sender.select());
                if (this.selectedElement.State.ID != null) {
                    var _self = this;
                    $.get(asu.Url("odata/ObjStates(" + this.selectedElement.State.ID + ")/Rules"), null,
                        function (r) {
                            _self.updateDocFlowGroupBar(r.value);
                        }, "json").error(function (r) { showError(r) });
                }
            },
            selectedElement: null,
            updateDocFlowGroupBar: function (r) {
                var _btns = [], _self = this;
                this.toolbar().remove($("#docFlowBar"));
                for (var i = 0; i < r.length; i++) {
                    _btns.push({
                        text: r[i].Name,
                        id: 'btnDF_' + r[i].ID,
                        group: "docFlow",
                        imageUrl: asu.Url(r[i].ImageName),
                        confirmText: r[i].ConfirmText,
                        click: function (e) {
                            var optionsBtn;
                            for (var j = 0; j < this._groups.docFlow.buttons.length; j++) {
                                if (this._groups.docFlow.buttons[j].options.id == e.id) {
                                    optionsBtn = this._groups.docFlow.buttons[j].options;
                                    break;
                                }
                            }
                            if (optionsBtn.confirmText)
                                if (!confirm(optionsBtn.confirmText))
                                    return false;
                            _self.clickDocFlowBar(e.id.replace('btnDF_', ''))
                        }
                    });
                }
                if (_btns.length > 0)
                    this.toolbar().add({
                        type: "buttonGroup",
                        id: "docFlowBar",
                        buttons: _btns
                    });
            },
            newElement: function () {
                var _self = this;
                saveEntity(a.url + "/New",
                    function (r) {
                        a.popup.close();
                        _self.elements.add(r)
                    });
            },
            editElement: function () {
                if (this.selectedElement)
                    saveEntity(a.url + "/Edit", function (r) { a.popup.close(); this.elements.pushUpdate(r) }, { id: this.selectedElement.ID }, this);

            },
            edit: function () {
                if (this.selectedElement)
                    a.popup.show("Редактировать ", a.url + "/PopupEdit", { id: this.selectedElement.ID });
            },
            delElement: function () {
                if (this.selectedElement)
                    extDelObject("пользователя", a.url + "/Del", { id: this.selectedElement.ID }, function () { this.elements.remove(this.selectedElement) }, this);
            },
            addObject: function () {
                a.popup.show("Добавить", a.url + "/PopupNew");
            },
            saveAsExcel: function () {
                var n = this.grid().options.excel || {},
                         i = new kendo.ExcelExporter({
                             columns: a.columnsExcel,
                             dataSource: this.elements,
                             allPages: n.allPages,
                             filterable: n.filterable,
                             hierarchy: n.hierarchy
                         });
                var e = window.kendo.jQuery;
                i.workbook().then(e.proxy(function (e, i) {
                    if (!this.trigger("excelExport", {
                        workbook: e,
                        data: i
                    })) {
                        var o = new kendo.ooxml.Workbook(e);
                        kendo.saveAs({
                            dataURI: o.toDataURL(),
                            fileName: e.fileName || n.fileName,
                            proxyURL: n.proxyURL,
                            forceProxy: n.forceProxy
                        })
                    }
                }, this.grid()))
            },
            clickDocFlowBar: function (ruleID) {
                var _self = this;
                $.post(asu.Url("odata/" + a.entity + "(" + this.selectedElement.ID + ")/RunRule?$expand=State/Rules" + ((a.expand) ? "," + a.expand : "") + "&ruleID=" + ruleID), null,
                    function (r) {
                        _self.updateDocFlowGroupBar(r.State.Rules);
                        if (r.value !== undefined) return r.value; else { delete r["odata.metadata"] }
                        _self.elements.pushUpdate(r)
                    }, "json")
                    .error(function (r) { showError(r) })
            },
            cmd: function (e) {
                switch (e.id) {
                    case "menu": showMainMenu(); break;
                    case "add": this.addObject(); break;
                    case "edit": this.edit(); break;
                    case "del": this.delElement(); break;
                    case "saveAsExcel": this.saveAsExcel(); break;
                    case "reload": this.elements.read(); break;
                    default:
                }
            },
            elements: new kendo.data.DataSource({
                type: "odata",
                transport: {
                    read: { url: asu.Url("odata/") + a.entity + ((a.expand) ? "?$expand=" + a.expand : ""), dataType: "json" }
                },
                schema: {
                    model: { id: "ID", fields: a.fields },
                    data: function (r) { if (r.value !== undefined) return r.value; else { delete r["odata.metadata"]; return r; } },
                    total: function (r) { return r["odata.count"] }
                },
                push: function (e) {
                    if (e.type == "update") {
                        var f = this.options.schema.model.fields;
                        var kk = Object.keys(f);
                        kk.forEach(function (ee) {
                            if (f[ee].type == "date" && e.items[0][ee]) e.items[0].set(ee, kendo.parseDate(e.items[0][ee]))
                        });
                    }
                },
                pageSize: 50,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                error: function (r) { showError(r.xhr); }
            })
        })
        var m = this._model;
        return m;
    }
}

function showError(error) {
    if (asu.notification)
        asu.notification.error(asu.getErr(error));
    else
        alert(asu.getErr(error));
}
function extDelObject(name, delURI, data, e, cntx) {
    if (!confirm("Вы действительно хотите удалить " + name + "?")) return;
    var opt = { url: delURI, type: "POST", dataType: "json", data: data, success: function (r) { if (e) e.call(this) }, error: function (r) { showError(r) } };
    if (cntx != undefined) opt.context = cntx;
    $.ajax(opt);
}
// базовая функция для удаления объекта
function delObject(name, delURI, id, e, cntx) {
    extDelObject(name, delURI, { id: id }, e, cntx)
}
function cancelSave(s) {
    if (paramVal.length > 0) {
        if (!confirm("Отменить внесенные изменения?")) return;
        paramVal = [];
    }
    s.close();
}
//Базовая функция для замены текста внутри другого
String.format = function () {
    var theString = arguments[0];
    for (var i = 1; i < arguments.length; i++) {
        var regEx = new RegExp("\\{" + (i - 1) + "\\}", "gm");
        theString = theString.replace(regEx, arguments[i]);
    }
    return theString;
}
//Функция для скачивания файла
window.downloadFile = function (sUrl, fileName) {

    //IOS устройства не поддерживают загрузку.
    if (/(iP)/g.test(navigator.userAgent)) {
        asu.notification.info("Ваше устройство не поддерживает скачивание файлов. Пожалуйста, попробуйте снова в настольном браузере.");
        return false;
    }

    //Если в Chrome или Safari, то скачать через виртуальный щелчек мыши
    if (window.downloadFile.isChrome || window.downloadFile.isSafari) {
        //Создание новой ссылки
        var link = document.createElement('a');
        link.href = sUrl;

        if (link.download !== undefined) {
            if (fileName == undefined)
                fileName = sUrl.substring(sUrl.lastIndexOf('/') + 1, sUrl.length);
            link.download = fileName;
        };

        //Диспетчеризация события нажатия
        if (document.createEvent) {
            var e = document.createEvent('MouseEvents');
            e.initEvent('click', true, true);
            link.dispatchEvent(e);
            return true;
        }
    }

    if (sUrl.indexOf('?') === -1) {
        sUrl += '?download';
    }

    window.open(sUrl, '_self');
    return true;
}

window.downloadFile.isChrome = navigator.userAgent.toLowerCase().indexOf('chrome') > -1;
window.downloadFile.isSafari = navigator.userAgent.toLowerCase().indexOf('safari') > -1;

//Обновление параметров url
function updateQueryStringParameter(uri, key, value) {
    var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
    var separator = uri.indexOf('?') !== -1 ? "&" : "?";
    if (uri.match(re)) {
        return uri.replace(re, '$1' + key + "=" + value + '$2');
    }
    else {
        return uri + separator + key + "=" + value;
    }
}

//Получение значения параметра по его имени
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? null : decodeURIComponent(results[1].replace(/\+/g, " "));
}