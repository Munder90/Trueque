var div = $("#validaciones");
var app = "<script>";

app = setValidInputs(lista, app);

app = setValidTabs(app);

var appValidarTab = "";
appValidarTab += "function validarTab(e, tabid, div, rec) {";
appValidarTab += "var ban = true;";
for ( i = 0; i < lista.length; i++) {
    appValidarTab += "ban = true;";
    appValidarTab += "if ('" + lista[i].TAB + "' == tabid) {";
    campo = document.getElementById(lista[i].ID);
    if (campo !== null) {
        appValidarTab += "ban = ";
        for (j = 0; j < lista[i].COND.length; j++) {
            appValidarTab += lista[i].COND[j].andor + "warning($('#" + lista[i].ID + "').val(), '" + lista[i].COND[j].comp + "', '" + lista[i].COND[j].val2 + "')" + lista[i].COND[j].orand;
        }
        appValidarTab += ";";
        appValidarTab += "if($('#TSOL_ID').val() =='" + lista[i].TSOL + "' |'" + lista[i].TSOL + "'== ''){";
        appValidarTab += " validarN('" + lista[i].ID + "', " + lista[i].NUM + ", '" + lista[i].MSG + "', '" + lista[i].TIPO + "', '";
        appValidarTab += lista[i].COLOR + "', '" + lista[i].ELEM + "', " + i + ", ban);}else{ban=true;} ";

    }
    else {
        appValidarTab += "var val = $('." + lista[i].ID + "').find('input').val();";
        if (lista[i].COND.length > 0 & lista[i].COND[0].comp !== 'f') {
            appValidarTab += "if(val==undefined){ var li = document.querySelectorAll('." + lista[i].ID + "');";
            appValidarTab += "for(var j= 0; j<li.length;j++){ var par = $(li[j]).prop('tagName');if( par!= 'TH'){";
            appValidarTab += " val = $(li[j]).text(); ban = ";
            for (j = 0; j < lista[i].COND.length; j++) {
                appValidarTab += lista[i].COND[j].andor + "warning(val, '" + lista[i].COND[j].comp + "', '" + lista[i].COND[j].val2 + "')" + lista[i].COND[j].orand;
            }
            appValidarTab += ";if(!ban){j=li.length;}";
            appValidarTab += "}}}";
            appValidarTab += "else{";
            appValidarTab += "ban = ";
            for (j = 0; j < lista[i].COND.length; j++) {
                appValidarTab += lista[i].COND[j].andor + "warning(val, '" + lista[i].COND[j].comp + "', '" + lista[i].COND[j].val2 + "')" + lista[i].COND[j].orand;
            }
            appValidarTab += "}";
        } else {
            appValidarTab += "ban = ";
            for (j = 0; j < lista[i].COND.length; j++) {
                appValidarTab += lista[i].COND[j].andor + "warning(val, '" + lista[i].COND[j].comp + "', '" + lista[i].COND[j].val2 + "')" + lista[i].COND[j].orand;
            }
        }
        appValidarTab += ";";
        appValidarTab += "if($('#TSOL_ID').val() =='" + lista[i].TSOL + "' |'" + lista[i].TSOL + "'== ''){";
        appValidarTab += " validarNC('" + lista[i].ID + "', " + lista[i].NUM + ", '" + lista[i].MSG + "', '" + lista[i].TIPO + "', '";
        appValidarTab += lista[i].COLOR + "', '" + lista[i].ELEM + "', " + i + ", ban);}else{ban=true;}";
    }
    appValidarTab += "if('" + lista[i].TIPO + "'=='info'){ban=true;}";

    var tabb = "";
    if (lista[i].TAB === "tab_info") {
        tabb = "Informacion_cont";
    }
    if (lista[i].TAB === "tab_soporte") {
        tabb = "Soporte_cont";
    }
    if (lista[i].TAB === "tab_dis") {
        tabb = "Distribucion_cont";
    }
    appValidarTab += "if(!ban){selectTab('" + tabb + "', e);} activaSubmit('" + tabb + "');}";
    appValidarTab += "if(rec>0 & !ban){return ban;}";
}
appValidarTab += " return ban;}";
app += appValidarTab;

app += "</script>";
div.append(app);


function setValidInputs(lista,app) {
    for (var i = 0; i < lista.length; i++) {
        var campo = document.getElementById(lista[i].ID);
        if (campo !== null) {
            app += "$('#" + lista[i].ID + "').on('" + lista[i].ACTION + "', function (e) {";
            app += "var ban = ";
            for (var j = 0; j < lista[i].COND.length; j++) {
                app += lista[i].COND[j].andor + "warning($(this).val(), '" + lista[i].COND[j].comp + "', '" + lista[i].COND[j].val2 + "')" + lista[i].COND[j].orand;
            }
            app += ";";
            app += "if($('#TSOL_ID').val() =='" + lista[i].TSOL + "' |'" + lista[i].TSOL + "'== ''){";
            app += " validarN('" + lista[i].ID + "', " + lista[i].NUM + ", '" + lista[i].MSG + "', '" + lista[i].TIPO + "', '";
            app += lista[i].COLOR + "', '" + lista[i].ELEM + "', " + i + ", ban);";
            app += "}";
            app += "});";

        }
        else {
            app += "$('body').on('" + lista[i].ACTION + "','." + lista[i].ID + "', function (e) { ";
            app += "var val = $(this).find('input').val(); if(val==undefined){ val = $(this).val();}";
            app += "var ban = ";
            for (j = 0; j < lista[i].COND.length; j++) {
                app += lista[i].COND[j].andor + "warning(val, '" + lista[i].COND[j].comp + "', '" + lista[i].COND[j].val2 + "')" + lista[i].COND[j].orand;
            }
            app += ";";
            app += "if($('#TSOL_ID').val() =='" + lista[i].TSOL + "' |'" + lista[i].TSOL + "'== ''){";//Se agrego
            app += " validarNC('" + lista[i].ID + "', " + lista[i].NUM + ", '" + lista[i].MSG + "', '" + lista[i].TIPO + "', '";
            app += lista[i].COLOR + "', '" + lista[i].ELEM + "', " + i + ", ban);";
            // app += ";e.preventDefault(); e.stopPropagation();"; se quito
            app += "}";
            app += "});";
        }
    }
    return app;
}

function setValidTabs(app) {
    var clickTab = "";
    clickTab += "$('#tab_soporte').on('click', function (e) {var ban = validarTab(e, 'tab_info','Informacion_cont',0); }); ";
    clickTab += "$('#tab_dis').on('click', function (e) { var ban = validarTab(e, 'tab_info', 'Informacion_cont',0);" +
        " if(ban){ ban = validarTab(e, 'tab_soporte', 'Soporte_cont',0);} }); ";
    clickTab += "$('#tab_rec').on('click', function (e) { var ban = validarTab(e, 'tab_info', 'Informacion_cont',0);" +
        " if(ban){ ban = validarTab(e, 'tab_soporte', 'Soporte_cont',0);} " +
        " if(ban){ ban = validarTab(e, 'tab_dis', 'Distribucion_cont',0);} }); ";
    clickTab += "$('#tab_fin').on('click', function (e) { var ban = validarTab(e, 'tab_info', 'Informacion_cont',0);" +
        " if(ban){ ban = validarTab(e, 'tab_soporte', 'Soporte_cont',0);} " +
        " if(ban){ ban = validarTab(e, 'tab_dis', 'Distribucion_cont',0);} " +
        " if(isRecurrente()){  if(ban){ ban = validarTab(e, 'tab_rec', 'Recurrente_cont',1);}} " +
        " if(ban){ activaSubmit('Financiera_cont') } }); ";
   

    app += clickTab;
    return app;
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


function toast(clases, dura, icon, mensaje, color) {
    dismiss(clases);
    M.toast({
        classes: clases,
        displayLength: dura,
        html: '<span style="padding-right:15px;"><i class="material-icons ' + color + '-text">' + icon + '</i></span>  ' + mensaje
        + '<button class="btn-small btn-flat toast-action" onclick="dismiss(\'toast\')">Aceptar</button>'
    });
}

function dismiss(clase) {
    var toastElement = document.querySelectorAll('.' + clase);
    for (var i = 0; i < toastElement.length; i++) {
        var toastInstance = M.Toast.getInstance(toastElement[i]);
        toastInstance.dismiss();
    }
}

function selectTab(tab, e) {
    e.preventDefault();
    e.stopPropagation();
    if (tab === "Financiera_cont") {
        if (!cierre())
            $("#btn_guardarh").removeClass("disabled");
        else
            $("#btn_guardarh").addClass("disabled");
    }
    else
        $("#btn_guardarh").addClass("disabled");
    var ell = document.getElementById("tabs");
    var instances = M.Tabs.getInstance(ell);
   // instances.select(tab); Se quita para no ejecutar validacion otra vez
}
function activaSubmit(tab) {
    if (tab === "Financiera_cont") {
        if (!cierre())
            $("#btn_guardarh").removeClass("disabled");
        else
            $("#btn_guardarh").addClass("disabled");
    }
    else
        $("#btn_guardarh").addClass("disabled");
}

function esValido(campo) {
    campo.classList.add("valid");
    campo.classList.remove("invalid");
    campo.classList.remove("warn");
}
function esInvalido(campo) {
    campo.classList.add("invalid");
    campo.classList.remove("valid");
    campo.classList.remove("warn");
}
function esWarning(campo) {
    campo.classList.add("warn");
    campo.classList.remove("valid");
    campo.classList.remove("invalid");
}
function esValidoC(campo) {
    $(campo).find('input').addClass("valid");
    $(campo).find('input').removeClass("invalid");
    $(campo).find('input').removeClass("warn");
}
function esInvalidoC(campo) {
    $(campo).find('input').addClass("invalid");
    $(campo).find('input').removeClass("valid");
    $(campo).find('input').removeClass("warn");
}
function esWarningC(campo) {
    campo.classList.add("warn");
    campo.classList.remove("valid");
    campo.classList.remove("invalid");
}



function validarN(id, num, mensaje, icon, color, elem, ii, ban) {
    var campoVal = document.getElementById(id);
    var campo = document.getElementById(elem);

    if (campo !== null & campoVal !== null) {
        if (condiciones(ii)) {
            
            var valid = ban;
            var tipo_condicion = num;

            if (!valid) {
                toast(id + "-" + tipo_condicion, 1000000, icon, mensaje, color);
                if (color === "red")
                    esInvalido(campo);
                else
                    esWarning(campo);
            } else {
                esValido(campo);
                dismiss(id + "-" + tipo_condicion);
            }
        }
    }
}
function validarNC(id, num, mensaje, icon, color, elem, ii, ban) {
    var isTabla = true;
    var campoVal = document.querySelectorAll("td>." + id);
    var campo = document.querySelectorAll("td>." + elem);
    if (campoVal.length===0) {
        isTabla = false;
        campoVal=document.querySelectorAll("." + id);
        campo=document.querySelectorAll("." + elem);
    }
    
    for (var i = 0; i < campoVal.length; i++) {
        if (campo.length === campoVal.length) {
            var campoInput = $(campoVal[i]).find('input');
            var value = campoInput.val();
            if (lista[i].COND.length > 0 & lista[i].COND[0].comp !== 'f') {
                if (value === undefined)
                    value = $(campoVal[i]).text();
            }
            var valid = ban;
            var tipo_condicion = num;
            if (condiciones(ii) & value !== null) {

                if (!valid) {
                    toast(id + "-" + num, 1000000, icon, mensaje, color);
                    if (color === "red") {
                        esInvalidoC(campo[i]);
                        if (isTabla) {
                            campo[i].classList.add('invalid');
                            campo[i].classList.remove('valid');
                        }
                    }
                    else
                        esWarningC(campo[i]);
                    i = campoVal.length;
                } else {
                    esValidoC(campo[i]);
                    if (isTabla) {
                        campo[i].classList.add('valid');
                        campo[i].classList.remove('invalid');
                    }
                    dismiss(id + "-" + tipo_condicion);

                }
            }
        }
    }
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
    else if (comp === "f") {
        ban = evaluarFiles();
    }
    else if (comp === "d") {
        ban = isDate(val1);
    }
    else if (comp === "M") {
        ban = ($("#select_neg").val() === "M");
    }
    else if (comp === "P") {
        ban = ($("#select_neg").val() === "P");
    }
    else if (comp === "MA") {
        ban = ($("#select_dis").val() === "M");
    }
    else if (comp === "CA") {
        ban = ($("#select_dis").val() === "C");
    }
    else if (comp === "TOT") {
        ban = (toNum(val1) === toNum($("#total_dis").text()));
    }
    else if (comp === "T") {
        ban = (parseFloat(toNum($("#total_dis").text())) > 0);
    }
    else if (comp === "DIS") {
        evaluarDisTable();
        var len = $("#table_dis > tbody  > tr[role='row']").length;
        if (len > 0) {
            var cont = 0;
            $('#table_dis > tbody  > tr').each(function () {
                var mat = $(this).find("td:eq(" + 3 + ") input").val();
                var cat = $(this).find("td:eq(" + 4 + ")").text();
                if (mat !== undefined & mat !== "")
                    cont++;
                else if (cat !== undefined & cat !== "")
                    cont++;

            });
        }
        ban = cont > 0;
    }
    else if (comp === "INI") {
        ban = (val1.startsWith(val2));
    }
    else if (comp === "mail") {
        ban = validateEmail(val1);
    }
    else if (comp === "L") {
        ban = ligada();
    }
    else if (comp === "nL") {
        ban = !ligada();
    }
    else if (comp === "r1") {
        if (isRecurrente() & !ligada()) {
            len = $("#table_rec > tbody  > tr[role='row']").length;
            ban = len > 1;
        }
    }
    else if (comp === "r2") {
        if (isRecurrente() & ligada() & !isObjetivoQ()) {
            len = $("#table_rec > tbody  > tr[role='row']").length;
            if (listaRangos.length > 0)
                for (var j = 0; j < listaRangos.length; j++) {
                    ban = parseFloat(listaRangos[j].OBJ1) > 0 & parseFloat(listaRangos[j].PORC) > 0;
                    if (!ban)
                        j = listaRangos.length;
                }
            ban = (len > 1) & ban;
        }
    }
    else if (comp === "r3") {
        if (isRecurrente() & ligada() & isObjetivoQ()) {
            len = $("#table_rec > tbody  > tr[role='row']").length;
            if (listaRangos.length > 0)
                for (j = 0; j < listaRangos.length; j++) {
                    ban = parseFloat(listaRangos[j].OBJ1) > 0 & parseFloat(listaRangos[j].PORC) > 0;
                    if (!ban)
                        j = listaRangos.length;
                }
            ban = (len > 1) & ban & parseFloat(toNum($("#objPORC").val())) > 0;
        }
    } else if (comp === "FAC") {
        if (isMultiple()) {
            len = $("#table_sop > tbody  > tr[role='row']").length;
            ban = len > 1;
        } else {
            ban = true;
        }
    }
    return ban;
}

function validateEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

                



