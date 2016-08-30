var paramOrder = [];
var paramVal = [];
function initOrders(param, value) {
    paramOrder[param] = value;
}
//v - значение
//p - идентификатор объекта-владельца значений
//i - идентификатор параметра
//o - порядок
//c - код параметра
function setValue(v, p, i, o, c) {
    if (v == null) delParam(null, p, i, o, c);
    var f;
    if (i[0] == 'c') i[0] = '-';
    for (var r in paramVal) {
        if ((paramVal[r].p == p) && (paramVal[r].i == i) && (paramVal[r].o == o) && (paramVal[r].c == c)) f = paramVal[r];
    }
    if (f == null) {
        paramVal.push({ p: p, i: i, o: o, s: "c", v: v, c: c });
    } else {
        f.v = v;
    }
}
function addParam(s, p) {
    var o = paramOrder[p]
    var t = s.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
    var r = t.insertRow(t.rows.length);
    var td = r.insertCell(0);
    var id = t.id + "_" + o;
    td.id = id;
    $.ajax({
        url: asu.Url("Param/GetMultirowControl"),
        type: "POST",
        dataType: "html",
        data: { parentID: p.split('_')[0], id: p.split('_')[1], order: o },
        success: function (r) {
            $('#' + id).html(r);
            paramVal.push({ p: p.split('_')[0], i: p.split('_')[1], o: o, s: "n", v: null });
            paramOrder[p]++;
        },
        error: function () { asu.notification.error('Ошибка') }
    });
}
function delParam(s, p, i, o, c) {
    var f;
    for (var r in paramVal) {
        if ((paramVal[r].p == p) && (paramVal[r].i == i) && (paramVal[r].o == o) && (paramVal[r].c == c)) f = paramVal[r];
    }
    if (f != null) {
        if (f.s == "c") {
            f.s = "d";
        }
        else {
            delete paramVal[paramVal.indexOf(f)];
        }
    } else {
        paramVal.push({ p: p, i: i, o: o, s: "d", v: null, c: c });
    }
    if (s == null) return;
    var tr = s.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
    var t = tr.parentNode.parentNode;
    if (t.rows.length > 1) {
        t.deleteRow(tr.sectionRowIndex);
    }
}
function baseSaveParams(u, e, ext, dt, c) {
    if (paramVal.length > 0) {
        if (ext == undefined)
            ext = paramVal;
        else
            ext.data = paramVal;
        $.ajax({
            context: c,
            url: u,
            type: "POST",
            dataType: dt,
            contentType: "application/json, charset=\"utf-8\"",
            data: JSON.stringify(ext),
            success: function (r) { paramVal = []; if (e) { e.call(this, r); } },
            error: function (r) { showError(r); }
        });
    } else {
        if (e) e();
    }
}
function saveParams(u, e, ext) {
    baseSaveParams(asu.Url(u), e, ext, "json");
}
function saveParamsUrl(u, successURI) {
    saveParams(asu.Url(u), function () {
        window.location = successURI
    });
}
function saveEntity(u, e, ext, context) {
    baseSaveParams(u, e, ext, "jsonp", context);
}