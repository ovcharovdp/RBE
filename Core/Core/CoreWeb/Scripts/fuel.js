var test, test1;
asu.fuel = {
    orderModel: function (a) {
        var today = new Date(); today.setMinutes(0); today.setHours(0); today.setSeconds(0);
        this._model = new kendo.observable({
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
                filter: { field: "DocDate", operator: "eq", value: today },
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
                    items: new kendo.data.DataSource({
                        type: "odata",
                        transport: {
                            read: { url: asu.Url("odata/FlOrders(" + id + ")/Items") + "?$expand=Station,Station/Organization,State,Product,Customer&$select=*,State/*,Customer/FullName,Product/Name,Station/Name,Station/Organization/ShortName", dataType: "json" }
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
                        //test = e;
                        test1 = this;
                    }
                });
                return _mm
            },
            detailInit: function (e) {
                var dataItem = e.sender.dataItem(e.masterRow);
                kendo.bind(e.detailCell, this.detailModel(dataItem.ID));
            }
        })
        var _m = this._model;
        //test = _m;
        return _m;
    }
}