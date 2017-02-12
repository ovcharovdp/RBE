var test, test1;
asu.fuel = {
    orderModel: function (a) {
        if (this._model != undefined) return this._model;
        var today = new Date(); today.setMinutes(0); today.setHours(0); today.setSeconds(0);
        this._model = new kendo.observable({
            //openDialog: function (e) {
            //    alert(2);
            //    test = e;
            //    e.center();
            //},
            cmd: function (e) {
                switch (e.id) {
                    case "menu": showMainMenu(); break;
                    case "add": this.addObject(); break;
                    case "edit": this.edit(); break;
                    case "del": this.cancelOrder(); break;// this.delElement(); break;
                    case "saveAsExcel": $("#grid").data("kendoGrid").saveAsExcel();// this.saveAsExcel();
                        break;
                    case "reload": this.elements.read(); break;
                    default:
                }
            },
            cancelOrder: function () {
                var g = $("#grid").data("kendoGrid");
                var r = g.dataItem(g.select()[0]);
                // вызов сервиса
                this.elements.pushUpdate(r);
            },
            elements: new kendo.data.DataSource({
                type: "odata",
                transport: {
                    read: { url: asu.Url("odata/") + a.entity + ((a.expand) ? "?$expand=State,Auto/Model," + a.expand : "") + "&$select=*,State/ID,State/Description,TankFarm/ID,Auto/RegNum,Auto/Model/Name,Auto/Organization/ID", dataType: "json" }
                },
                schema: {
                    model: {
                        id: "ID",
                        fields: a.fields
                    },
                    data: function (r) { if (r.value !== undefined) return r.value; else { delete r["odata.metadata"]; return r; } },
                    total: function (r) { return r["odata.count"] }
                },
                filter: [{ field: "DocDate", operator: "gte", value: today }, { field: "DocDate", operator: "lte", value: today }],
                push: function (e) {
                    if (e.type == "update") {
                        var f = this.options.schema.model.fields;
                        var kk = Object.keys(f);
                        kk.forEach(function (ee) {
                            if (f[ee].type == "date" && e.items[0][ee]) e.items[0].set(ee, kendo.parseDate(e.items[0][ee]))
                        });
                    }
                },
                sort: { field: "FillDatePlan", dir: "asc" },
                pageSize: 50,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                error: function (r) { showError(r.xhr); }
            }),
            changedOrder: null,
            changingItem: null,
            selectedStation: null,
            onSelectStation: function (e) {
                this.selectedStation = e.sender.dataItem();
            },
            onChangeStation: function (e) {
                if (this.changingItem.Station.ID != this.selectedStation.ID) {
                    var order = this.changedOrder;
                    $.post(asu.Url("odata/FlOrderItems(" + this.changingItem.ID + ")/SetStation?stationID=" + this.selectedStation.ID), null, function (d) { order.read() }, "json")
                        .error(function (r) { showError(r) });
                }
                editWnd.close();
            },
            stations: new kendo.data.DataSource({
                type: "odata",
                transport: {
                    read: { url: asu.Url("odata/FlStations?$expand=Organization&$select=*,Organization/Name"), dataType: "json" },
                    parameterMap: function (data, t) {
                        var d = kendo.data.transports.odata.parameterMap(data);
                        if (d.$format) delete d.$format;
                        if (data.filter.filters[0].value != "") {
                            d.$filter = "Number eq " + data.filter.filters[0].value;
                        }
                        return d;
                    }
                },
                schema: {
                    model: {
                        id: "ID"
                    },
                    data: function (r) { if (r.value !== undefined) return r.value; else { delete r["odata.metadata"]; return r; } },
                    total: function (r) { return r["odata.count"] }
                },
                //filter: { field: "DocDate", operator: "eq", value: today },
                //push: function (e) {
                //    if (e.type == "update") {
                //        var f = this.options.schema.model.fields;
                //        var kk = Object.keys(f);
                //        kk.forEach(function (ee) {
                //            if (f[ee].type == "date" && e.items[0][ee]) e.items[0].set(ee, kendo.parseDate(e.items[0][ee]))
                //        });
                //    }
                //},
                //sort: { field: "FillDatePlan", dir: "asc" },
                //pageSize: 50,
                //serverPaging: true,
                serverFiltering: true,
                //serverSorting: true,
                error: function (r) { showError(r.xhr); }
            }),
            dataBound: function (e) {
                var items = e.sender.items();
                items.each(function (index) {
                    var dataItem = e.sender.dataItem(this);
                    this.className += " " + dataItem.State.Description;
                });
                //this.expandRow(this.tbody.find("tr.k-master-row"));
            },
            detailModel: function (id) {
                var _mm = new kendo.observable({
                    mM: this,
                    items: new kendo.data.DataSource({
                        type: "odata",
                        transport: {
                            read: { url: asu.Url("odata/FlOrders(" + id + ")/Items") + "?$expand=Station,Station/Organization,State,Product,Customer&$select=*,State/*,Customer/FullName,Product/Name,Station/*", dataType: "json" }
                        },
                        schema: {
                            model: {
                                id: "ID"
                                //,fields: a.fields
                            },
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
                        sort: { field: "SectionNum", dir: "asc" },
                        serverPaging: false,
                        serverFiltering: false,
                        serverSorting: false,
                        error: function (r) { showError(r.xhr); }
                    }),
                    changeStation: function (e) {
                        this.mM.changingItem = e.data;
                        this.mM.changedOrder = this.items;
                        this.mM.set("selectedStation", e.data.Station);
                        editWnd.show("Смена объекта", a.url + "/ChangeStation");
                    }
                });
                return _mm
            },
            detailInit: function (e) {
                var dataItem = e.sender.dataItem(e.masterRow);
                kendo.bind(e.detailCell, this.detailModel(dataItem.ID));
            },
            visibleCS: true
        })
        //var _m = this._model;
        //test = _m;
        return this._model;
    },
    factModel: function (a) {
        if (this._factModel != undefined) return this._factModel;
        //var today = new Date(); today.setMinutes(0); today.setHours(0); today.setSeconds(0);
        this._factModel = new kendo.observable({
            cmd: function (e) {
                switch (e.id) {
                    case "menu": showMainMenu(); break;
                        //  case "add": this.addObject(); break;
                    case "edit": this.edit(); break;
                        //  case "del": this.cancelOrder(); break;// this.delElement(); break;
                    case "saveAsExcel": $("#grid").data("kendoGrid").saveAsExcel();// this.saveAsExcel();
                        break;
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
                    model: {
                        id: "ID",
                        fields: a.fields
                    },
                    data: function (r) { if (r.value !== undefined) return r.value; else { delete r["odata.metadata"]; return r; } },
                    total: function (r) { return r["odata.count"] }
                },
                //filter: { field: "FactDate", operator: "eq", value: today },
                push: function (e) {
                    if (e.type == "update") {
                        var f = this.options.schema.model.fields;
                        var kk = Object.keys(f);
                        kk.forEach(function (ee) {
                            if (f[ee].type == "date" && e.items[0][ee]) e.items[0].set(ee, kendo.parseDate(e.items[0][ee]))
                        });
                    }
                },
                sort: { field: "FactDate", dir: "desc" },
                pageSize: 50,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                error: function (r) { showError(r.xhr); }
            }),
            grid: null,
            dataBound: function (e) {
                this.grid = e.sender;
                var items = e.sender.items();
                items.each(function (index) {
                    var dataItem = e.sender.dataItem(this);
                    if (dataItem.State.Code == "00") {
                        this.className += " f-done";
                    }
                });
                //this.expandRow(this.tbody.find("tr.k-master-row"));
            },
            edit: function (e) {
                if (this.grid.select().length > 0) {
                    var id = this.grid.dataItem(this.grid.select()).ID
                    editWnd.show("Редактировать", a.url + "/PopupEdit", { id: id });
                }
            },
            detailModel: function (e) {
                var _startDate = new Date("2017-02-02T00:00:00");
                var _mm = new kendo.observable({
                    mM: this,
                    items: new kendo.data.DataSource({
                        type: "odata",
                        transport: {
                            read: { url: asu.Url("odata/FlOrderItems") + "?$expand=Order,Order/TankFarm,Station,Station/Organization,State,Product,Customer&$select=*,State/*,Customer/FullName,Product/Name,Station/*,Order/*,Order/TankFarm/ShortName", dataType: "json" }
                        },
                        schema: {
                            model: {
                                id: "ID",
                                order: function () {
                                    return kendo.toString(kendo.parseDate(this.Order.DocDate), "d")
                                        + ", рейс: " + this.Order.Order
                                        + ", " + this.Order.TankFarm.ShortName
                                }
                                //, fields: { "Order.LogID": { type: "number" } }
                                //,fields: a.fields
                            },
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
                        group: {
                            field: "order()",
                            dir: "asc"
                        },
                        // sort: { field: "SectionNum", dir: "asc" },
                        filter: [
                            { field: "Order.DocDate", operator: "ge", value: _startDate },
                            { field: "Order.Auto.RegNum", operator: "startswith", value: e.RegNum },
                            { field: "State.Code", operator: "neq", value: "2" }],
                        serverPaging: false,
                        serverFiltering: true,
                        serverSorting: false,
                        error: function (r) { showError(r.xhr); }
                    }),
                    changeStation: function (e) {
                        this.mM.changingItem = e.data;
                        this.mM.changedOrder = this.items;
                        this.mM.set("selectedStation", e.data.Station);
                        editWnd.show("Смена объекта", a.url + "/ChangeStation");
                    }
                });
                return _mm
            },
            detailInit: function (e) {
                var dataItem = e.sender.dataItem(e.masterRow);
                kendo.bind(e.detailCell, this.detailModel(dataItem));
            },
            handle: function (e) {
                var m = this;
                $.post(asu.Url("odata/FlFacts(" + e.data.ID + ")/Handle?$expand=" + a.expand), null, function (d) {
                    delete d["odata.metadata"];
                    m.elements.pushUpdate(d);
                }, "json");
            }
        })
        return this._factModel;
    }
}