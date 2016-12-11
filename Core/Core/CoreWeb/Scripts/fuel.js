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
                    read: { url: asu.Url("odata/") + a.entity + ((a.expand) ? "?$expand=Items,Items/Station,Items/Station/Organization,Items/State,Items/Product,Auto/Model," + a.expand : "") + "&$select=*,TankFarm/ID,Auto/RegNum,Auto/Model/Name,Auto/Organization/ID,Items/*,Items/State/Name,Items/Product/Name,Items/Station/Name,Items/Station/Organization/ShortName", dataType: "json" }
                    //read: { url: asu.Url("odata/") + a.entity + ((a.expand) ? "?$expand=" + a.expand : ""), data: { $orderby: "Order/ID,SectionNum" }, dataType: "json" }
                },
                schema: {
                    model: { id: "ID", fields: a.fields },
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
                pageSize: 50,
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
                error: function (r) { showError(r.xhr); }
            }),
            detailInit: function (e) {
                console.log(e.data);
                var dataItem = e.sender.dataItem(e.masterRow);
                kendo.bind(e.detailCell, dataItem);
            }
        })
        var _m = this._model;
        return _m;
    }
}