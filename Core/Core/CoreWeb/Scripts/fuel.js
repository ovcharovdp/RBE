asu.fuel = {
    orderModel: function (a) {
        this._model = new kendo.observable({
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
                    read: { url: asu.Url("odata/") + a.entity + ((a.expand) ? "?$expand=Items,Items/Station,Items/Product," + a.expand : ""), dataType: "json" }
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
            }),
            detailInit: function (e) {
                console.log(e.data);
                var dataItem = $("#grid").data("kendoGrid").dataItem(e.masterRow);
                kendo.bind(e.detailCell, dataItem);
             //   $("<div/>").appendTo(e.detailCell)
            }
        })
        var _m = this._model;
        return _m;
    }
}