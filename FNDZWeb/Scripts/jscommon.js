//easyui 日期列格式化
function getTime(/** timestamp=0 **/) {
    var ts = arguments[0] || 0;
    var t, y, m, d, h, i, s;
    t = ts ? new Date(ts * 1000) : new Date();
    y = t.getFullYear();
    m = t.getMonth() + 1;
    d = t.getDate();
    h = t.getHours();
    i = t.getMinutes();
    s = t.getSeconds();
    // 可根据需要在这里定义时间格式  
    return y + '-' + (m < 10 ? '0' + m : m) + '-' + (d < 10 ? '0' + d : d) + ' ' + (h < 10 ? '0' + h : h) + ':' + (i < 10 ? '0' + i : i) + ':' + (s < 10 ? '0' + s : s);
}

/**
 * Created by Ryan on 14-6-9.
 */
/**
 * 可编辑表格
 * **/
$.extend($.fn.datagrid.methods, {
    editCell: function (jq, param) {
        return jq.each(function () {
            var opts = $(this).datagrid('options');
            var fields = $(this).datagrid('getColumnFields', true).concat($(this).datagrid('getColumnFields'));
            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor1 = col.editor;
                if (fields[i] != param.field) {
                    col.editor = null;
                }
            }
            $(this).datagrid('beginEdit', param.index);
            for (var i = 0; i < fields.length; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor = col.editor1;
            }
        });
    }
});

/**
 * 序列化成json字符串
 * @param obj
 * @return
 */
function Serialize(obj) {
    switch (obj.constructor) {
        case Object:
            var str = "{";
            for (var o in obj) {
                str += o + ":" + Serialize(obj[o]) + ",";
            }
            if (str.substr(str.length - 1) == ",")
                str = str.substr(0, str.length - 1);
            return str + "}";
            break;
        case Array:
            var str = "[";
            for (var o in obj) {
                str += Serialize(obj[o]) + ",";
            }
            if (str.substr(str.length - 1) == ",")
                str = str.substr(0, str.length - 1);
            return str + "]";
            break;
        case Boolean:
            return "\"" + obj.toString() + "\"";
            break;
        case Date:
            return "\"" + obj.toString() + "\"";
            break;
        case Function:
            break;
        case Number:
            return "\"" + obj.toString() + "\"";
            break;
        case String:
            return "\"" + obj.toString() + "\"";
            break;
    }
}

$.extend($.fn.validatebox.defaults.rules, {
    minLength: {
        validator: function (value, param) {
            return value.length >= param[0];
        },
        message: 'Please enter at least {0} characters.'
    },
    maxLength: {
        validator: function (value, param) {
            return value.length <= param[0];
        },
        message: 'Please Enter a maximum {0} characters.'
    }
});
function fomatFloat(src, pos) {
    return Math.round(src * Math.pow(10, pos)) / Math.pow(10, pos);
}

function fomatNum(n) {
    var num = fomatFloat(parseFloat(n), 1);
    var alen = num.toString().split(".");
    var val = 0;
    if (parseInt(alen[1]) > 0 && parseInt(alen[1]) <= 5) {
        val = alen[0] + ".5";
    }
    if (parseInt(alen[1]) >= 6) {
        val = parseInt(alen[0]) + 1 + ".0";
    }
    if (alen.length == 1 || parseInt(alen[1]) == 0) {
        val = num;
    }
    return val;
}

function SEChargeTotal(url, jobno, AR, AP, PR) {
    $.ajax({
        url: url,
        data: { JobNo: jobno },
        success: function (data) {
            console.log(data);
            var result = eval('(' + data + ')');
            var msg = result.Msg;
            var baseCurr = result.BaseCurr;
            var AmountBaseCurr = 0, CostAmountOriginCurr = 0, BaseBalance = 0;
            if (msg == 'ok') {
                var d = result.data;
                var html = '';
                if (d.length > 0) {
                    var ar = new Array();
                    var ap = new Array();
                    var pr = new Array();
                    var arr = new Array(ar, ap, pr);
                    arr[0][0] = "<td class='lable he'>" + AR + "</td>";
                    arr[1][0] = "<td class='lable he'>" + AP + "</td>";
                    arr[2][0] = "<td class='lable he'>" + PR + "</td>";
                    var baseAR = 0;
                    var baseAP = 0;
                    var basePR = 0;
                    var BillTax = 0;
                    var CostTax = 0;
                    for (var j = 0; j < d.length; j++) {
                        arr[0][j + 2] = "<td class='lable he'>" + d[j].Currency + " " + d[j].Amount + "</td>";
                        arr[1][j + 2] = "<td class='lable he'>" + d[j].Currency + " " + d[j].CostAmount + "</td>";
                        if (d[j].Currency != baseCurr) {
                            arr[2][j + 2] = "<td class='lable he'>" + d[j].Currency + " " + fomatFloat(parseFloat(d[j].Amount - d[j].CostAmount), 2) + "</td>"
                        }
                        else {
                            arr[2][j + 2] = "<td class='lable he'>" + d[j].Currency + " " + parseFloat(d[j].Amount - d[j].CostAmount) + "</td>"
                        }
                        BillTax += parseFloat(d[j].BillTax);
                        CostTax += parseFloat(d[j].CostTax);
                        baseAR += parseFloat(d[j].AmountBaseCurr);
                        baseAP += parseFloat(d[j].CostAmountOriginCurr);
                        basePR += parseFloat(d[j].BaseBalance);
                    }
                    arr[0][1] = "<td class='lable he'>" + fomatFloat(baseAR, 0) + "</td>";
                    arr[1][1] = "<td class='lable he'>" + fomatFloat(baseAP, 0) + "</td>";
                    arr[2][1] = "<td class='lable he'>" + fomatFloat(basePR, 0) + "</td>";
                    if (baseAR != 0) {
                        arr[0][d.length + 2] = "<td class='lable he'>Tax " + fomatFloat(BillTax, 2) + "</td>";
                        arr[1][d.length + 2] = "<td class='lable he'>Tax " + fomatFloat(CostTax, 2) + "</td>";
                        arr[2][d.length + 2] = "<td class='lable he'>Tax " + fomatFloat(BillTax - CostTax, 2) + "</td>";
                    }
                    var html = "<table class ='default-tab1' style='width:auto'>"
                    for (var n = 0; n < arr.length; n++) {
                        html += "<tr>";
                        for (var k = 0; k < arr[n].length; k++) {
                            html += arr[n][k]
                        }
                        html += "</tr>"
                    }
                    html += "</table>";
                    $('#showAmount').html(html).show();
                } else {
                    html = "<table class ='default-tab1' style='width:20%'>" +
                                "<tr><td class='lable he'>" + AR + "</td><td class='lable he'>0</td></tr>" +
                                "<tr><td class='lable he'>" + AP + "</td><td class='lable he'>0</td></tr>" +
                                "<tr><td class='lable he'>" + PR + "</td><td class='lable he'>0</td></tr>" +
                            "</table>";
                }
                $('#showAmount').html(html).show();
            }
        }
    });
}


function myconfirm(title, msg, fn) {
    $.messager.defaults = { ok: "cancel", cancel: "ok" };
    $.messager.confirm(title, msg, function (r) {
        if (!r) {
            if (fn) {
                fn(true);
                return false;
            }
        }
    });
    $.messager.defaults = { ok: "ok", cancel: "cancel" };
}


//时间格式  yyyy/mm/dd
function dateformatter(str) {
    if (Date.parse(str) == Date.parse(str)) {
        var date = new Date(str);
        var y = date.getFullYear();
        var m = date.getMonth() + 1;
        var d = date.getDate();
        if (y == 1970 && m == 1 && d == 1)
            return "";

        return y + '/' + (m < 10 ? ('0' + m) : m) + '/' + (d < 10 ? ('0' + d) : d);
    }
    else
        return str;
}


Date.prototype.format = function (format) {
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(),    //day
        "h+": this.getHours(),   //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter
        "S": this.getMilliseconds() //millisecond
    }
    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
            (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
                RegExp.$1.length == 1 ? o[k] :
                        ("00" + o[k]).substr(("" + o[k]).length));
    return format;
}

function setSeaCode(obj, data) {
    var r = data.rows;
    if (r.length == 1) {
        var v = (obj.combogrid('getValue')).toUpperCase();
        var code5 = (r[0].Code5).toUpperCase();
        if (v == code5) {
            var g = obj.combogrid('grid');
            g.datagrid('selectRow', 0);
        }
    }
}

function setAirCode(obj, data) {
    var r = data.rows;
    if (r.length == 1) {
        var v = (obj.combogrid('getValue')).toUpperCase();
        var code3 = (r[0].Code3).toUpperCase();
        if (v == code3) {
            var g = obj.combogrid('grid');
            g.datagrid('selectRow', 0);
        }
    }
    else {
        var g = obj.combogrid('grid');
        g.datagrid('unselectAll', r);
    }
}

var loadData = $.extend({}, $.fn.datagrid.defaults.view, {
    onBeforeRender: function (target, rows) {
        //console.log('加载中....');
        $(target).datagrid("loading");
    }, onAfterRender: function (target) {
        //setTimeout(function () {
        $(target).datagrid("loaded");
        //}, 10000);
        //$(target).datagrid("loading");
    }
});

function loading() {
    var win = $.messager.progress({
        title: 'Please waiting',
        msg: 'Loading data...'
    });

}

function SetInfoShowPosi() {
    var iheight = 55;
    var iheighth = 30;
    //初始化时时展开的
    $('#dg').datagrid('resize', {
        height: $(window).height() - (2 * (iheight) + $('#pInfoShow').height() - iheighth)
    }).datagrid('resize', {
        height: $(window).height() - (2 * (iheight) + $('#pInfoShow').height() - iheighth)
    });

    $('#pInfoShow').panel({
        onCollapse: function () {
            $('#dg').datagrid('resize', {
                height: $(window).height() - iheight
            }).datagrid('resize', {
                height: $(window).height() - iheight
            });
        },
        onExpand: function () {
            $('#dg').datagrid('resize', {
                height: $(window).height() - (2 * (iheight) + $('#pInfoShow').height() - iheighth)
            }).datagrid('resize', {
                height: $(window).height() - (2 * (iheight) + $('#pInfoShow').height() - iheighth)
            });
        }


    });
}

function SetInfoShowPosiPen() {
    var iheight = 55;
    var iheighth = 30;
    //初始化时时展开的
    $('#dg').datagrid('resize', {
        height: $(window).height() - (2 * (iheight) + $('#pInfoShow').height() - iheighth)
    }).datagrid('resize', {
        height: $(window).height() - (2 * (iheight) + $('#pInfoShow').height() - iheighth)
    });

    $('#pInfoShow').panel({
        onCollapse: function () {
            $('#dg').datagrid('resize', {
                height: $(window).height() - iheight
            }).datagrid('resize', {
                height: $(window).height() - iheight
            });
        },
        onExpand: function () {
            $('#dg').datagrid('resize', {
                height: $(window).height() - (2 * (iheight) + $('#pInfoShow').height() - iheighth)
            }).datagrid('resize', {
                height: $(window).height() - (2 * (iheight) + $('#pInfoShow').height() - iheighth)
            });
        }


    });
}




