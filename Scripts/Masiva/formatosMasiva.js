function toNum(string, _miles, _decimales) {
    if (!$.isNumeric(string)) {
        if (string !== "" && string != undefined) {
            var _i = 1;
            string = string.replace('$', '');
            string = string.replace('%', '');
            if (string.indexOf("(") >= 0)
                _i = -1;
            string = string.replace('(', '');
            string = string.replace(')', '');
            //string = string.replace(_miles, '');
            if (_miles === '.')
                string = string.replace(/\./g, '')
            else
                string = string.replace(new RegExp(_miles, 'g'), '');
            string = string.replace(_decimales, '.');
            string = string * _i;
        } else {
            string = "0.00";
        }
    } else {
        string = parseFloat(string) + "";
    }
    return string;
}

function toShow(string, _decimales) {
    string = toNum(string);
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

function toShowG(string, _decimales) {//LGPP 21.08.2018----------------
    string = toNum(string);
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
            xpf = _xpf.replace('.', ',');
            string = ("$" + _xpf.toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, "."));
        }
    }
    else {
        string = ("$ -" + " ");
    }
    if (i < 0) string = string.replace('$', '$(') + ')';
    return string;
}

function toShowPorc(string, _decimales) {
    string = toNum(string);
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

function toShowPorc5(string, _decimales) {
    string = toNum(string);
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

function toShowNum(string, _decimales) {
    string = toNum(string);
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