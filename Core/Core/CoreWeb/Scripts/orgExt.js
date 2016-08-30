asu.org = {
    tree: function (e) {
        e.entityName = "организацию";
        var dg = asu.common.tree(e);
        dg.add_o = function () {
            var g = this.curItem();
            if (g == undefined) return;
            e.popup.show("Организация", e.controller + "/PopupNew", { parentID: g.ID })
        }
        return dg;
    }
}
function oilOrg(element, _value, orgGroupID) {
    element.kendoExtDropDownTreeView({
        height: "400px",
        dropDownList:
            {
                animation: false,
                dataSource: [{ text: "", value: "" }],
                dataTextField: "text",
                dataValueField: "value",
                optionLabel: (_value != null && _value != '') ? _value : "--Выберите значение--",
            },
        treeview: {
            animation: false,
            dataSource: new kendo.data.HierarchicalDataSource({
                transport: { read: { url: asu.Url("Org/Department/GetGroups"), dataType: "json", type: "POST", data: { baseID: orgGroupID } } },
                schema: { model: { id: "ID", hasChildren: "HasCld" } },
                error: function (r) { showError(r.xhr); }
            }),
            dataTextField: "Name",
            dataValueField: "ID"
        }
    });
}