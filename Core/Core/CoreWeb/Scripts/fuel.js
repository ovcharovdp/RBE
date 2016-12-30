asu.fuel = {
    orderModel: function (a) {
        var today = new Date(); today.setMinutes(0); today.setHours(0); today.setSeconds(0);
        this._model = new kendo.observable({
            cmd: function (e) {
                switch (e.id) {
                    case "menu": showMainMenu(); break;
                    case "add": this.addObject(); break;
                    case "edit": this.edit(); break;
                    case "del": this.delElement(); break;
                    case "saveAsExcel": $("#grid").data("kendoGrid").saveAsExcel();// this.saveAsExcel();
                        break;
                    case "reload": this.elements.read(); break;
                    default:
                }
            },
            elements: new kendo.data.DataSource({
                type: "odata",
                transport: {
                    read: { url: asu.Url("odata/") + a.entity + ((a.expand) ? "?$expand=Items,Items/Station,State,Items/Station/Organization,Items/State,Items/Product,Auto/Model,Items/Customer," + a.expand : "") + "&$select=*,State/ID,State/Description,TankFarm/ID,Auto/RegNum,Auto/Model/Name,Auto/Organization/ID,Items/*,Items/State/*,Items/Customer/FullName,Items/Product/Name,Items/Station/Name,Items/Station/Organization/ShortName", dataType: "json" }
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
                })
            },
            detailInit: function (e) {
                var dataItem = e.sender.dataItem(e.masterRow);
                kendo.bind(e.detailCell, dataItem);
            }
        })
        var _m = this._model;
        return _m;
    }
}