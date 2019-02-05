

var precio = document.getElementsByClassName("precio");
for (var i = 0; i < precio.length; i++) {
    var val = precio[i].innerHTML.trim();
    if (val == "") val = precio[i].value;
    val = toShow(val);
    precio[i].innerHTML = val;
    precio[i].value = val;
}
var porc = document.getElementsByClassName("porcentaje");
for (var i = 0; i < porc.length; i++) {
    var val = porc[i].innerHTML.trim();
    if (val == "") val = porc[i].value;
    val = toShowPorc(val);
    porc[i].innerHTML = val;
    porc[i].value = val;
}

var numb = document.getElementsByClassName("numero");
for (var i = 0; i < numb.length; i++) {
    var val = numb[i].innerHTML.trim();
    if (val == "") val = numb[i].value;
    val = toShowNum(val);
    numb[i].innerHTML = val;
    numb[i].value = val;
}
function toNum(string) {
    if (!$.isNumeric(string)) {
        if (string !== "" && string != undefined) {
            var _miles = $("#miles").val();
            var _decimales = $("#dec").val();
            var _i = 1;
            string = string.replace('$', '');
            string = string.replace('%', '');
            if (string.indexOf("(") >= 0)
                _i = -1;
            string = string.replace('(', '');
            string = string.replace(')', '');
            string = string.replace('-', '0');
            //string = string.replace(_miles, '');
            if (_miles === '.')
                string = string.replace(/\./g, '')
            else
                string = string.replace(new RegExp(_miles, 'g'), '');
            string = string.replace(_decimales, '.');

            if (!$.isNumeric(string))
                string = "0";

            string = string * _i;
        } else {
            string = "0.00";
        }
    } else {
        string = parseFloat(string) + "";
    }
    return string;
}

function toShow(string) {
    string = toNum(string);
    var _miles = $("#miles").val();
    var _decimales = $("#dec").val();
    var xx = parseFloat(string).toFixed(2);
    var _i = 1; if (xx < 0) _i = -1;
    xx = (xx * _i) + "";
    xx = xx.replace('.', _decimales);
    //string = xx.toString().replace(/\B(?=(\d{3})+(?!\d))/g, _miles) + '%';
    if (string != '') {
        if (_decimales === '.') {
            //Hace la conversion a 2 decimales
            var _xv = xx.replace(',', '');
            xx = _xv;
            string = ("$" + parseFloat(xx).toFixed(2).toString().replace(/\B(?=(?=\d*\.)(\d{3})+(?!\d))/g, ","));
        } else if (_decimales === ',') {
            var _xv = xx.replace('.', '');
            xx = _xv.replace(',', '.');
            var _xpf = parseFloat(xx.replace(',', '.')).toFixed(2);
            _xpf = _xpf.replace('.', ',');
            string = ("$" + _xpf.toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, "."));
        }
    }
    else {
        $(this).val("$0" + _decimales + "00");
    }
    if (_i < 0) string = string.replace('$', '$(') + ')';
    return string;
}
function toShow5(string) {
    string = toNum(string);
    var _miles = $("#miles").val();
    var _decimales = $("#dec").val();
    var xx = parseFloat(string).toFixed(5);
    var _i = 1; if (xx < 0) _i = -1;
    xx = (xx * _i) + "";
    xx = xx.replace('.', _decimales);
    //string = xx.toString().replace(/\B(?=(\d{3})+(?!\d))/g, _miles) + '%';
    if (string != '') {
        if (_decimales === '.') {
            //Hace la conversion a 2 decimales
            var _xv = xx.replace(',', '');
            xx = _xv;
            string = ("$" + parseFloat(xx).toFixed(5).toString().replace(/\B(?=(?=\d*\.)(\d{3})+(?!\d))/g, ","));
        } else if (_decimales === ',') {
            var _xv = xx.replace('.', '');
            xx = _xv.replace(',', '.');
            var _xpf = parseFloat(xx.replace(',', '.')).toFixed(5);
            _xpf = _xpf.replace('.', ',');
            string = ("$" + _xpf.toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, "."));
        }
    }
    else {
        $(this).val("$0" + _decimales + "00");
    }
    if (_i < 0) string = string.replace('$', '$(') + ')';
    return string;
}

function toShowG(string) {//LGPP 21.08.2018----------------
    string = toNum(string);
    var _miles = $("#miles").val();
    var _decimales = $("#dec").val();
    var xx = parseFloat(string);
    var i = 1;
    if (xx <= 0) {
        i = -1;
    }
    xx = xx * i;

    xx = xx.toFixed(2);
    xx = xx.replace('.', _decimales);
    if (string != '0') {
        if (_decimales === '.') {
            //Hace la conversion a 2 decimales
            var _xv = xx.replace(',', '');
            xx = _xv;
            string = ("$" + parseFloat(xx).toFixed(2).toString().replace(/\B(?=(?=\d*\.)(\d{3})+(?!\d))/g, ","));
        } else if (_decimales === ',') {
            var _xv = xx.replace('.', '');
            xx = _xv.replace(',', '.');
            var _xpf = parseFloat(xx.replace(',', '.')).toFixed(2);
            _xpf = _xpf.replace('.', ',');
            string = ("$" + _xpf.toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, "."));
        }
    }
    else {
        string = ("$ -" + " ");
    }
    if (i < 0) string = string.replace('$', '$(') + ')';
    return string;
}

function toShowPorc(string) {
    string = toNum(string);
    var _miles = $("#miles").val();
    var _decimales = $("#dec").val();
    var xx = parseFloat(string).toFixed(2);
    var _i = 1; if (xx < 0) _i = -1;
    xx = (xx * _i) + "";
    xx = xx.replace('.', _decimales);
    //string = xx.toString().replace(/\B(?=(\d{3})+(?!\d))/g, _miles) + '%';
    if (string != '') {
        if (_decimales === '.') {
            //Hace la conversion a 2 decimales
            var _xv = xx.replace(',', '');
            xx = _xv;
            string = (parseFloat(xx).toFixed(2).toString().replace(/\B(?=(?=\d*\.)(\d{3})+(?!\d))/g, ",") + '%');
        } else if (_decimales === ',') {
            var _xv = xx.replace('.', '');
            xx = _xv.replace(',', '.');
            var _xpf = parseFloat(xx.replace(',', '.')).toFixed(2);
            _xpf = _xpf.replace('.', ',');
            string = (_xpf.toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, ".") + '%');
        }
    }
    else {
        $(this).val("0" + _decimales + "00%");
    }
    if (_i < 0) string = '(' + string + ')';
    return string;
}

function toShowPorc5(string) {
    string = toNum(string);
    var _miles = $("#miles").val();
    var _decimales = $("#dec").val();
    var xx = parseFloat(string).toFixed(5);
    var _i = 1; if (xx < 0) _i = -1;
    xx = (xx * _i) + "";
    xx = xx.replace('.', _decimales);
    //string = xx.toString().replace(/\B(?=(\d{3})+(?!\d))/g, _miles) + '%';
    if (string != '') {
        if (_decimales === '.') {
            //Hace la conversion a 2 decimales
            var _xv = xx.replace(',', '');
            xx = _xv;
            string = (parseFloat(xx).toFixed(5).toString().replace(/\B(?=(?=\d*\.)(\d{3})+(?!\d))/g, ",") + '%');
        } else if (_decimales === ',') {
            var _xv = xx.replace('.', '');
            xx = _xv.replace(',', '.');
            var _xpf = parseFloat(xx.replace(',', '.')).toFixed(5);
            _xpf = _xpf.replace('.', ',');
            string = (_xpf.toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, ".") + '%');
        }
    }
    else {
        $(this).val("0" + _decimales + "00%");
    }
    if (_i < 0) string = '(' + string + ')';
    return string;
}

function toShowNum(string) {
    string = toNum(string);
    var _miles = $("#miles").val();
    var _decimales = $("#dec").val();
    var xx = parseFloat(string).toFixed(2);
    var _i = 1; if (xx < 0) _i = -1;
    xx = (xx * _i) + "";
    xx = xx.replace('.', _decimales);
    //string = xx.toString().replace(/\B(?=(\d{3})+(?!\d))/g, _miles) + '%';
    if (string != '') {
        if (_decimales === '.') {
            //Hace la conversion a 2 decimales
            var _xv = xx.replace(',', '');
            xx = _xv;
            string = (parseFloat(xx).toFixed(2).toString().replace(/\B(?=(?=\d*\.)(\d{3})+(?!\d))/g, ","));
        } else if (_decimales === ',') {
            var _xv = xx.replace('.', '');
            xx = _xv.replace(',', '.');
            var _xpf = parseFloat(xx.replace(',', '.')).toFixed(2);
            _xpf = _xpf.replace('.', ',');
            string = (_xpf.toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, "."));
        }
    }
    else {
        $(this).val("0" + _decimales + "00");
    }
    if (_i < 0) string = '(' + string + ')';
    return string;
}