function generarScriptValidaciones(table, fields) {
    var div = $("#validacionesMasivas");
    var app = "<script>";
    app += "var tb = $('" + table + "').DataTable();";
    for (var f = 0; f < fields.length; f++) {
        var fiels = fields[f];
        for (var i = 0; i < lista.length; i++) {
            if (fiels.elementId === lista[i].ID) {
                app += "for (var t = 0; t < tb.rows().data().length; t++) {";
                app += " var row = tb.row(t).node();";
                app += "$(row).children().eq('" + fiels.elementIndex + "').children('input').on('" + lista[i].ACTION + "', function (e) {";
                app += "var ban = ";
                for (var j = 0; j < lista[i].COND.length; j++) {
                    app += lista[i].COND[j].andor + "warning(this.value, '" + lista[i].COND[j].comp + "', '" + lista[i].COND[j].val2 + "')" + lista[i].COND[j].orand;
                }
                app += ";";
                app += "if('" + lista[i].TSOL + "'== ''){";
                app += " validar('" + lista[i].ID + "', " + lista[i].NUM + ", '" + lista[i].MSG + "', '" + lista[i].TIPO + "', '";
                app += lista[i].COLOR + "', this, " + i + ", ban);";
                app += "}";
                app += "});";
                app += "$(row).children().eq('" + fiels.elementIndex + "').children('input').trigger('" + lista[i].ACTION + "')";
                app += "}";
            }
        }
    }
    app += "</script>";
    div.append(app);
}
function validar(id, num, mensaje, icon, color, campo, ii, ban) {
    if (campo !== null) {
        if (condiciones(ii)) {

            var valid = ban;
            var tipo_condicion = num;

            if (!valid) {
                campo.title = mensaje;
                if (color === "red")
                    esInvalido(campo);
                else
                    esWarning(campo);
            } else {
                esValido(campo);
            }
        }
    }
}
function condiciones(ii) {
    var ban = true;
    var cont = 0;
    if (lista[ii].CONDICION !== undefined && lista[ii].CONDICION !== "undefined")
        if (lista[ii].CONDICION.length > 0) {
            for (var i = 0; i < lista[ii].CONDICION.length; i++) {
                var ccc = lista[ii].CONDICION;
                if (document.getElementById(ccc[i].id).value === ccc[i].val)
                    cont++;
            }
            if (cont !== lista[ii].CONDICION.length)
                ban = false;
        }
    return ban;
}
function esValido(campo) {

    //campo.classList.remove("red");
    //campo.classList.remove("white-text");
    //campo.classList.remove("rojo");
    campo.classList.remove("yellow");
}
function esInvalido(campo) {
    if (!campo.disabled) {
        campo.classList.add("red");
        campo.classList.add("white-text");
        campo.classList.add("rojo");
    }
}
function esWarning(campo) {
    campo.classList.add("yellow");
    campo.classList.add("white-text");
}

function warning(val1, comp, val2) {
    var ban = false;
    if (comp === '=') {
        if (val1 === val2)
            return true;
    } if (comp === '!=') {
        if (val1 !== val2)
            return true;
    } if (comp === '&') {
        if (val1 & val2)
            return true;
        else
            return false;
    } if (comp === '|') {
        if (val1 | val2)
            return true;
        else
            return false;
    } if (comp === 'n') {
        if ($.isNumeric(toNum(val1)))
            return true;
    } if (comp === 'e') {
        if ($.isNumeric(parseFloat(toNum(val1))) & Number.isInteger(parseFloat(toNum(val1))))
            return true;
    } if (comp === 'dec') {
        if (!Number.isInteger(parseFloat(toNum(val1))) & $.isNumeric(parseFloat(toNum(val1))))
            return true;
    }

    if ($.isNumeric(toNum(val1))) {
        if (comp === '>') {
            if (parseFloat(toNum(val1)) > parseFloat(toNum(val2)))
                return true;
        } if (comp === '<') {
            if (parseFloat(toNum(val1)) < parseFloat(toNum(val2)))
                return true;
        } if (comp === '>=') {
            if (parseFloat(toNum(val1)) >= parseFloat(toNum(val2)))
                return true;
        } if (comp === '<=') {
            if (parseFloat(toNum(val1)) <= parseFloat(toNum(val2)))
                return true;
        }
    }
    if (comp === "l") {
        var longitud = parseInt(val2);
        if (longitud > 0) {
            if (val1.length === longitud)
                return true;
        }
    }
    else if (comp === "d") {
        ban = isDate(val1);
    }
    
    else if (comp === "TOT") {
        ban = (toNum(val1) === toNum($("#total_dis").text()));
    }
    else if (comp === "T") {
        ban = (parseFloat(toNum($("#total_dis").text())) > 0);
    }
    else if (comp === "INI") {
        ban = (val1.startsWith(val2));
    }
    else if (comp === "mail") {
        ban = validateEmail(val1);
    }
    
    return ban;
}
function validateEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}
function isDate(xx) {
    var currVal = xx;
    if (currVal === '' || currVal === undefined)
        return false;

    var rxDatePattern = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/; //Declare Regex
    var dtArray = currVal.match(rxDatePattern); // is format OK?

    if (dtArray === null)
        return false;

    //Checks for mm/dd/yyyy format.
    dtMonth = dtArray[3];
    dtDay = dtArray[1];
    dtYear = dtArray[5];

    if (dtMonth < 1 || dtMonth > 12) return false;

    else if (dtDay < 1 || dtDay > 31) return false;
    else if ((dtMonth === 4 || dtMonth === 6 || dtMonth === 9 || dtMonth === 11) && dtDay === 31) return false;
    else if (dtMonth === 2) {
        var isleap = (dtYear % 4 === 0 && (dtYear % 100 !== 0 || dtYear % 400 === 0));
        if (dtDay > 29 || (dtDay === 29 && !isleap)) return false;
    }
    return true;
}