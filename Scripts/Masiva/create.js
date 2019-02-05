var tsols = "";
var cargaInicialSpin = [];
cargaInicialSpin[0] = false;
cargaInicialSpin[1] = false;
cargaInicialSpin[2] = false;
cargaInicialSpin[3] = false;
cargaInicialSpin[4] = false;
//LEEMOS EL ARCHIVO UNA VEZ CARGADO AL ELEMENTO FILE INPUT
$("#miMas").change(function () {
    var filenum = $('#miMas').get(0).files.length;
    if (filenum > 0) {
        var file = document.getElementById("miMas").files[0];
        var filename = file.name;
        //EVALUAMOS LA EXTENSION PARA VER QUE SOLO PERMITA FORMATOS DE EXCEL
        if (evaluarExt(filename)) {
            mostrarAlerta("info", "B", 'Archivo: ' + filename);
            getExcelMasivas(file);
        } else {
            mostrarAlerta("info", "E", 'Tipo de archivo incorrecto: ' + filename);
        }
    } else {
        mostrarAlerta("info", "A", 'Seleccione un archivo');
    }
    clearErrors();
});

function evaluarExt(filename) {
    var exts = ['xls', 'xlsx'];
    var get_ext = filename.split('.');
    get_ext = get_ext.reverse();

    if ($.inArray(get_ext[0].toLowerCase(), exts) > -1) {
        return true;
    } else {
        return false;
    }
}

function getExcelMasivas(file) {
    var formData = new FormData();
    formData.append("FileUpload", file);
    document.getElementById("loader").style.display = "initial";
    $.ajax({
        type: "POST",
        url: 'loadExcelMasiva',
        data: formData,
        contentType: false,
        processData: false,
        async: true,
        success: function (data) {
            if (data !== null || data !== "") {
                if (data.isRedirect) {
                    window.onbeforeunload = false;
                    window.location = data.redirectUrl;
                } else {
                    InicializarTablas();
                    procesarHoja1();
                }
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            document.getElementById("loader").style.display = "none";
        }
    }).fail(function () {
    });
}

/////////////////////////////////////////////////////////HOJA 1 FUNCIONES Y ASIGNACIONES////////////////////////////////////////////////////////
function procesarHoja1() {
    document.getElementById("loader").style.display = "initial";
    var table = $('#tab_test1').DataTable({ language: { "url": "../Scripts/lang/" + ln + ".json" }, "order": [1, 'asc']  });

    $.ajax({
        type: "POST",
        url: 'validaHoja1',
        dataType: "json",
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data !== null || data !== "") {
                if (data.isRedirect) {
                    window.onbeforeunload = false;
                    window.location = data.redirectUrl;
                } else {
                    if (data.length > 1) {
                        var errores = data.pop();

                        //INICIO DEL CICLO FOR
                        $.each(data, function (i, dataj) {
                            //if (i % 2 == 0) {//}  CONDICION PARA SOLO PARES
                            var addedRow = addRowH1(table, dataj.NUM_DOC, dataj.TSOL_ID, dataj.TALL_ID, dataj.SOCIEDAD_ID, dataj.PAIS_ID, dataj.ESTADO, dataj.CIUDAD, dataj.CONCEPTO, dataj.NOTAS, dataj.PAYER_ID, dataj.PAYER_NOMBRE, dataj.CONTACTO_NOMBRE, dataj.CONTACTO_EMAIL, dataj.FECHAI_VIG, dataj.FECHAF_VIG, dataj.MONEDA_ID, dataj.VKORG, dataj.VTWEG, errores[i], dataj.PAIS_NAME, dataj.TALL_NAME, dataj.SPART, dataj.Decimales, dataj.Miles);
                            tsols += "." + dataj.NUM_DOC + "-" + dataj.TSOL_ID;//ADD RSG 05.11.2018
                        }); //FIN DEL FOR

                        $('#tab_test1').css("font-size", "10px");
                        $('#tab_test1').css("display", "table");
                        mostrarAlerta("info", "B", 'Hoja 1 procesada');
                        procesarHoja2();
                        procesarHoja3();
                        procesarHoja4();
                        procesarHoja5();
                        checkRelacionada();
                        var kk = checkRelacionadaMat();
                        clearErrors();
                        generarWarningH1();
                    }
                    else {
                        if (data) {
                            if (data.toString().length > 0)
                                mostrarAlerta("info", "A", data);
                        }
                        document.getElementById("loader").style.display = "none";
                    }
                }
            }

            cargaInicialSpin[0] = true;
            if (jQuery.inArray(false, cargaInicialSpin) === -1) {
                checkRelacionada();
                checkRelacionadaMat();
                clearErrors();
                document.getElementById("loader").style.display = "none";
            }
        },

        complete: function (data) { },

        error: function (xhr, httpStatusMessage, customErrorMessage) {
            document.getElementById("loader").style.display = "none";
            M.toast({ html: 'La solicitud no pudo ser procesada. Favor de intentarlo mas tarde. La causa ' + xhr.status + " : " + httpStatusMessage + " : " + customErrorMessage });
        }
    });
}

function addRowH1(t, NUM_DOC, TSOL_ID, TALL_ID, SOCIEDAD_ID, PAIS_ID, ESTADO, CIUDAD, CONCEPTO, NOTAS, PAYER_ID, PAYER_NOMBRE, CONTACTO_NOMBRE, CONTACTO_EMAIL, FECHAI_VIG, FECHAF_VIG, MONEDA_ID, VKORG, VTWEG, ERRORES, PAIS_NAME, TALL_NAME, SPART, Decimales, Miles) {

    //var clasificacion = "<select id=\"clas\" class=\"miSel\">";
    //$.each(arr1, function (i, data) {
    //    if (data == GALL_ID) {
    //        clasificacion += "<option value=\"" + i + "\" selected>" + data + "</option>";
    //    }
    //    else {
    //        clasificacion += "<option value=\"" + i + "\">" + data + "</option>";
    //    }
    //});
    //clasificacion = clasificacion + "</select>";
    var icono, clase = null;
    if (jQuery.inArray('red white-text rojo', ERRORES) !== -1) {
        icono = 'close';
        clase = 'red white-text';
    }
    else {
        icono = 'done';
        clase = 'green white-text';
    }
    
    NOTAS = NOTAS.replace("'", '&#39;');

    var r = t.row.add([
        "<div class='" + clase + "'></div>",
        "<input class='" + ERRORES[0] + " input_numdoc' style='font-size:10px; text-align:center;' type='text' id='' name='' disabled value='" + NUM_DOC + "' title='" + getWarning(ERRORES[0]) +"'><span hidden>" + NUM_DOC + "</span>",
        "<input class='" + ERRORES[1] + " input_tsol' style='font-size:10px; text-align:center;' type='text' id='' name='' disabled value='" + TSOL_ID + "' title='" + getWarning(ERRORES[1]) +"'><span hidden>" + TSOL_ID + "</span>",
        "<input class='" + ERRORES[2] + " input_clasificacion' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + TALL_NAME + "' title='" + getWarning(ERRORES[2]) +"'><span hidden>" + TALL_ID + "</span>",
        "<input class='" + ERRORES[3] + " input_sociedad' style='font-size:10px; text-align:center;' type='text' id='' name='' disabled value='" + SOCIEDAD_ID + "' title='" + getWarning(ERRORES[3]) +"'><span hidden>" + SOCIEDAD_ID + "</span>",
        //"<input class='" + ERRORES[4] + " input_pais' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + PAIS_ID + "'><span hidden class='span_pais'>" + PAIS_ID + "</span>",
        "<input class='" + ERRORES[4] + " input_pais' style='font-size:10px; text-align:center;' type='text' id='' name='' disabled value='" + PAIS_NAME + "' title='" + getWarning(ERRORES[4]) +"'><span hidden class='span_pais'>" + PAIS_ID + "</span>",
        "<input class='" + ERRORES[5] + " input_estado' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + ESTADO + "' title='" + getWarning(ERRORES[5]) +"'><span hidden>" + ESTADO + "</span>",
        "<input class='" + ERRORES[6] + " input_ciudad' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + CIUDAD + "' title='" + getWarning(ERRORES[6]) +"'><span hidden>" + CIUDAD + "</span>",
        "<input class='" + ERRORES[7] + " input_concepto' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + CONCEPTO + "' title='" + getWarning(ERRORES[7]) +"'><span hidden>" + CONCEPTO + "</span>",
        "<input class='" + ERRORES[8] + " input_notas' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + NOTAS + "' title='" + getWarning(ERRORES[8]) +"'><span hidden>" + NOTAS + "</span>",
        "<input class='" + ERRORES[9] + " input_cliente' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + PAYER_ID + "' title='" + getWarning(ERRORES[9]) +"'><span hidden>" + PAYER_ID + "</span>",
        "<input class='" + ERRORES[10] + "' style='font-size:10px; text-align:center;' type='text' id='' name='' disabled value='" + PAYER_NOMBRE + "' title='" + getWarning(ERRORES[10]) +"'><span hidden>" + PAYER_NOMBRE + "</span>",
        "<input class='" + ERRORES[11] + " input_contacto' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + CONTACTO_NOMBRE + "' title='" + getWarning(ERRORES[11]) +"'><span hidden>" + CONTACTO_NOMBRE + "</span>",
        "<input class='" + ERRORES[12] + " input_email' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + CONTACTO_EMAIL + "' title='" + getWarning(ERRORES[12]) +"'><span hidden>" + CONTACTO_EMAIL + "</span>",
        "<input class='" + ERRORES[13] + " input_fechai' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + FECHAI_VIG + "' title='" + getWarning(ERRORES[13]) +"'><span hidden>" + FECHAI_VIG + "</span>",
        "<input class='" + ERRORES[14] + " input_fechaf' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + FECHAF_VIG + "' title='" + getWarning(ERRORES[14]) +"'><span hidden>" + FECHAF_VIG + "</span>",
        "<input class='" + ERRORES[15] + " input_moneda' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + MONEDA_ID + "' title='" + getWarning(ERRORES[15]) +"'><span hidden>" + MONEDA_ID + "</span>",
        "<input id='txt_vkorg' value='" + VKORG + "' hidden>",
        "<input id='txt_vtweg' value='" + VTWEG + "' hidden>",
        "<input id='txt_vtspart' value='" + SPART + "' hidden>",
        "<input id='txt_vtmiles' value='" + Miles + "' hidden>",
        "<input id='txt_vtdecimales' value='" + Decimales + "' hidden>"
    ]).draw(false).node();

    var td = $(r).children()[0];
    $(td).css('text-align', 'center');

    return r;
}

$('#tab_test1').on('keydown.autocomplete', '.input_numdoc', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'numdoc',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    if (data) {
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                    } else {
                        $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                    }
                    clearErrors();
                }
            });
        },

        change: function () {
            if ($(this).val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            }
            else {
                if ($.isNumeric($(this).val())) {
                    $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                    $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                }
            }
            clearErrors();
        }
    });
});

$('#tab_test1').on('keydown.autocomplete', '.input_tsol', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'tipoSol',
                dataType: "json",
                data: { "Prefix": request.term },

                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: item.ID, value: item.ID };
                    }));
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item) && $(".input_tsol").val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                clearErrors();
                e.target.value = "";
            }
        },

        select: function (event, ui) {
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            clearErrors();
        }
    });
});

$('#tab_test1').on('keydown.autocomplete', '.input_clasificacion', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    var sociedad = $('.input_sociedad').val();
    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root + 'Listas/Talls',
                dataType: "json",
                data: { "Prefix": request.term, "sociedad_id": sociedad },
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: item.TXT50, value: item.TXT50 , TallId: item.TALL_ID };
                    }));
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item) && $(".input_clasificacion").val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                clearErrors();
                e.target.value = "";
            }
        },

        select: function (event, ui) {
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
            var idTall = tr.find("td:eq(" + col_index + ")").children('span');
            idTall.text(ui.item.TallId);
            clearErrors();
        }
    });
});

$('#tab_test1').on('keydown.autocomplete', '.input_sociedad', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    var us = $("#USUARIOC_ID").val(); //ADD RSG 01.11.2018

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'sociedad',
                dataType: "json",
                //data: { "Prefix": request.term },
                data: { "Prefix": request.term, user: us },//ADD RSG 01.11.2018
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: item.BUKRS, value: item.BUKRS };
                    }));
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item) && $(".input_sociedad").val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                clearErrors();
                e.target.value = "";
            }
        },

        select: function (event, ui) {
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            clearErrors();
        }
    });
});

$('#tab_test1').on('keydown.autocomplete', '.input_pais', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    var col_index2 = col_index - 1;
    var thisS = this;
    $(tr.find("td:eq(" + col_index2 + ")").children().addClass('' + row_index + 'sociedad' + col_index2));
    var sociedad = $('.' + row_index + 'sociedad' + col_index2).val();
    //ESTO SIRVE
    //var table = $('#tab_test1').DataTable();
    //var fila = table.row(row_index).data();
    //var kk = $('#tab_test1 tr').eq(row_index).find('td').eq(col_index).children;

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'pais',
                dataType: "json",
                data: { "Prefix": request.term, "Sociedad": sociedad.toUpperCase() },
                success: function (data) {
                    response(auto.map(data, function (item) {
                        //return { label: item.LANDX, value: item.LANDX };
                        return { label: item.LAND + " " + item.LANDX, value: item.LANDX };
                    }));
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item) && $(".input_pais").val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                clearErrors();
                e.target.value = "";
            }
        },

        select: function (event, ui) {
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
            $(thisS).parent().parent().find(".span_pais").text(ui.item.label.split(' ')[0]); //ADD RSG 01.11.2018
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            clearErrors();
        }
    });
});

$('#tab_test1').on('keydown.autocomplete', '.input_estado', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    var col_index2 = col_index - 1;

    $(tr.find("td:eq(" + col_index2 + ")").children().addClass('' + row_index + 'pais' + col_index2));
    var pais = $('.' + row_index + 'pais' + col_index2)[1].textContent;
    
    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root + "Listas/Estados",
                dataType: "json",
                data: { "Prefix": request.term, "pais": pais },
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: item.NAME, value: item.NAME };
                    }));
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item) && $(".input_estado").val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                clearErrors();
                e.target.value = "";
            } else if ($(".input_estado").val() !== "") {
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                clearErrors();
            }
        },

        select: function (event, ui) {
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
            clearErrors();
        }
    });
});

$('#tab_test1').on('keydown.autocomplete', '.input_ciudad', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    var col_index2 = col_index - 1;

    $(tr.find("td:eq(" + col_index2 + ")").children().addClass('' + row_index + 'estado' + col_index2));
    var estado = $('.' + row_index + 'estado' + col_index2).val();

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root + "Listas/Ciudades",
                dataType: "json",
                data: { "Prefix": request.term, "estado": estado },
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: item.NAME, value: item.NAME };
                    }));
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item) && $(".input_ciudad").val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                clearErrors();
                e.target.value = "";
            } else if ($(".input_ciudad").val() !== "") {
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                clearErrors();
            }
        },

        select: function (event, ui) {
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
            clearErrors();
        }
    });

});

$('#tab_test1').on('change', '.input_concepto', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var val = $(this).val();
    var indexConcepto = getTableIndex('#tab_test1', 'lbl_concepto');
    var colConcepto = tr.find('td:eq(' + indexConcepto + ') input');
    if (val === "") {
        colConcepto.addClass("red white-text rojo");
    } else {
        if (val.length > 100) {
            this.title = 'Se superó la cantidad maxima de caracteres(100)';
            colConcepto.addClass("red white-text rojo");
        } else {
            this.title = '';
            colConcepto.removeClass("red white-text rojo");
        }
    }
    clearErrors();

});

$('#tab_test1').on('change', '.input_notas', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var val = $(this).val();
    var indexnota = getTableIndex('#tab_test1', 'lbl_notas');
    var colNota = tr.find('td:eq(' + indexnota + ') input');
    if (val === "") {
        colNota.addClass("red white-text rojo");
    } else {
        if (val.length > 255) {
            this.title = 'Se superó la cantidad maxima de caracteres(255)';
            colNota.addClass("red white-text rojo");
        } else {
            this.title = '';
            colNota.removeClass("red white-text rojo");
        }
    }
    clearErrors();
});

$('body').on('keydown.autocomplete', '.input_cliente', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    var col_index2 = col_index + 1;
    var colVk_index = getTableIndex('#tab_test1', 'lbl_vkorg');
    var colVt_index = getTableIndex('#tab_test1', 'lbl_vtweg');
    var colSpartIdex = getTableIndex('#tab_test1', 'lbl_spart');
    var numTabla = $(this).parents()[3];
    numTabla = $(numTabla).attr('id');
    var us = $("#USUARIOC_ID").val(); //ADD RSG 01.11.2018
    var pais = $(this).parent().parent().find(".span_pais").text(); //ADD RSG 01.11.2018
    //pais = $(this).parent().parent().find(".input_pais").val(); //ADD RSG 01.11.2018

    var defClase = null;

    if (numTabla === 'tab_test1') {
        defClase = 'clienteNumH1';
    } else if (numTabla === 'tab_test3') {
        defClase = 'clienteNumH3';
    }

    $(tr.find("td:eq(" + col_index + ")").children().addClass('' + row_index + defClase + col_index));
    $(tr.find("td:eq(" + col_index2 + ")").children().addClass('' + row_index + defClase + col_index2));
    var clase2 = '.' + row_index + defClase + col_index2;

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root + "Listas/Clientes",
                dataType: "json",
                //data: { "Prefix": request.term },
                data: { "Prefix": request.term, "usuario": us, "pais": pais }, //ADD RSG 01.11.2018
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: trimStart('0', item.KUNNR) + '-' + item.NAME1, value: trimStart('0', item.KUNNR), vkorg: item.VKORG, vtweg: item.VTWEG, spart:item.SPART };
                    }));
                }
                , error: function (e, er) {
                    alert(e);
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item) && $(".input_cliente").val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                //$(tr.find("td:eq(" + col_index2 + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index2 + ")").children().val(""));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                clearErrors();
                e.target.value = "";
            }
        },

        select: function (event, ui) {
            var label = ui.item.label;
            label = label.split('-');
            $(clase2).val(label[1]);
            if (numTabla === 'tab_test1') {
                $(tr.find("td:eq(" + colVk_index + ")").children()).val(ui.item.vkorg);
                $(tr.find("td:eq(" + colVt_index + ")").children()).val(ui.item.vtweg);
                $(tr.find("td:eq(" + colSpartIdex + ")").children()).val(ui.item.spart);
            }
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
            //$(tr.find("td:eq(" + col_index2 + ")").children().removeClass("red white-text rojo"));
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
            clearErrors();
        }
    });
});

$('#tab_test1').on('keydown.autocomplete', '.input_contacto', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    var col = col_index - 2;
    var cliente = tr.find("td:eq(" + col + ")").children();
    var errorcliente = cliente.attr("class").indexOf('rojo');

    auto(this).autocomplete({
        source: function (request, response) {
            if (errorcliente >= 0) {
                auto.ajax({
                    type: "POST",
                    url: 'contactoNombre',
                    dataType: "json",
                    data: { "Prefix": request.term },
                    success: function (data) {
                        if (data) {
                            $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                            $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                        } else {
                            $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        }
                        clearErrors();
                    }
                });
            } else {
                auto.ajax({
                    url: root + "Listas/contactos",
                    type: "GET",
                    dataType: "json",
                    data: { Prefix: request.term, kunnr: cliente.val() },
                    success: function (data) {
                        if (data) {
                            $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                            $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                        } else {
                            $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        }
                        response(auto.map(data, function (item) {
                            return { label: item.NOMBRE + " - " + item.EMAIL, value: item.NOMBRE, mail: item.EMAIL };
                        }))
                    }
                });
            }
        },

        change: function () {
            if ($(this).val() === "" && $(".input_contacto").val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            }
            clearErrors();
        },

        select: function (event, ui) {
            var tr = $(this).closest('tr'); 
            var col_index = $(this).parent().index();
            var co = col_index + 1;
            var colemail = $(tr.find("td:eq(" + co + ")").children());
            if (errorcliente < 0) {
                colemail.val(ui.item.mail);
            }
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
            clearErrors();
        }
    });
});

$('#tab_test1').on('keydown.autocomplete', '.input_email', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'contactoEmail',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    if (data) {
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                    } else {
                        $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                    }
                    clearErrors();
                }
            });
        },

        change: function () {
            if ($(this).val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            }
            clearErrors();
        }
    });
});

$('#tab_test1').on('keydown.autocomplete', '.input_fechai', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    var col_index2 = col_index + 1;

    var fecha2 = $(tr.find("td:eq(" + col_index2 + ")").children()).val();

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'validaFechaH1',
                dataType: "json",
                data: { "Fecha1": request.term, "Fecha2": fecha2 },
                success: function (data) {
                    if (data.status) {
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index2 + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                        $(tr.find("td:eq(" + col_index2 + ")").children().prop('title', ''));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                    } else {
                       var msj = "";
                        if (data.tipo === "Formato") {
                            msj = "Formato invalido";
                        } else {
                            msj = "Rango invalido";
                        }
                        $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index2 + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', msj));
                        $(tr.find("td:eq(" + col_index2 + ")").children().prop('title', msj));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                    }
                    clearErrors();
                }
            });
        }
    });
});

$('#tab_test1').on('keydown.autocomplete', '.input_fechaf', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    var col_index2 = col_index - 1;

    var fecha1 = $(tr.find("td:eq(" + col_index2 + ")").children()).val();

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'validaFechaH1',
                dataType: "json",
                data: { "Fecha1": fecha1, "Fecha2": request.term },
                success: function (data) {
                    if (data.status) {
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index2 + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title',''));
                        $(tr.find("td:eq(" + col_index2 + ")").children().prop('title', ''));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                    } else {
                        var msj = "";
                        if (data.tipo === "Formato") {
                            msj = "Formato invalido";
                        } else {
                            msj = "Rango invalido";
                        }
                        $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index2 + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', msj));
                        $(tr.find("td:eq(" + col_index2 + ")").children().prop('title', msj));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                    }
                    clearErrors();
                }
            });
        }
    });
});

$('#tab_test1').on('keydown.autocomplete', '.input_moneda', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    var sociedad = tr.find('td:eq(4)').children().val();

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'moneda',
                dataType: "json",
                data: { "Prefix": request.term, "SociedadH1": sociedad },
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: item.WAERS, value: item.WAERS };
                    }));
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item) && $(".input_moneda").val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                $(tr.find("td:eq(" + col_index + ")").children().prop('title', 'Moneda'));
                clearErrors();
                e.target.value = "";
            }
        },

        select: function (event, ui) {
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
            clearErrors();
        }
    });
});

$("#tab_info").click(function () {
    var tbody = $("#tab_test1 tbody");
    var arregloNumDoc = [];

    if (tbody.children().length !== 0) {
        arregloNumDoc = checkRelacionada();

        if (arregloNumDoc.length > 0) {
            var tablaH3 = $('#tab_test3').DataTable();

            for (var a = 0; a < tablaH3.rows().data().length; a++) {
                var rowH3 = tablaH3.row(a).node();
                var num_docH3 = $(rowH3).children().eq(1).children().val();

                if (jQuery.inArray(num_docH3, arregloNumDoc) !== -1) {
                    $(rowH3).children().eq(1).children().addClass("red white-text rojo");
                }
            }
        }
        checkRelacionadaMat();
    }
});

/////////////////////////////////////////////////////////HOJA 2 FUNCIONES Y ASIGNACIONES////////////////////////////////////////////////////////
function procesarHoja2() {
    //document.getElementById("loader").style.display = "initial";
    var table = $('#tab_test2').DataTable({ language: { "url": "../Scripts/lang/" + ln + ".json" }, "order": [1, 'asc']  });

    $.ajax({
        type: "POST",
        url: 'validaHoja2',
        dataType: "json",
        cache: false,
        contentType: false,
        processData: false,
        async: true,
        success: function (data) {
            if (data !== null || data !== "") {
                if (data.isRedirect) {
                    window.onbeforeunload = false;
                    window.location = data.redirectUrl;
                } else {
                    if (data.length > 1) {
                        var warnings = data.pop();
                        var errores = data.pop();

                        //INICIO DEL CICLO FOR
                        $.each(data, function (i, dataj) {
                            if (dataj.NUM_DOC !== undefined)
                                var addedRow = addRowH2(table, dataj.NUM_DOC, dataj.FACTURA, dataj.FECHA, dataj.PROVEEDOR, dataj.PROVEEDOR_NOMBRE, dataj.AUTORIZACION, dataj.VENCIMIENTO, dataj.FACTURAK, dataj.EJERCICIOK, errores[i], warnings[i]);

                        }); //FIN DEL FOR

                        $('#tab_test2').css("font-size", "10px");
                        $('#tab_test2').css("display", "table");
                        mostrarAlerta("info", "B", 'Hoja 2 procesada');
                        generarWarningH2();
                    }
                    else {
                        if (data) {
                            if (data.toString().length > 0)
                                mostrarAlerta("info", "A", data);
                        }
                    }
                }
            }
            cargaInicialSpin[1] = true;
            if (jQuery.inArray(false, cargaInicialSpin) === -1) {
                checkRelacionada();
                checkRelacionadaMat();
                clearErrors();
                document.getElementById("loader").style.display = "none";
            }
        },

        complete: function (data) { },

        error: function (xhr, httpStatusMessage, customErrorMessage) {
            document.getElementById("loader").style.display = "none";
            M.toast({ html: 'La solicitud no pudo ser procesada. Favor de intentarlo mas tarde. La causa ' + xhr.status + " : " + httpStatusMessage + " : " + customErrorMessage });
        }
    });

    //Actualizar los valores en la tabla
    //updateTable();
}

function addRowH2(t, NUM_DOC, FACTURA, FECHA, PROVEEDOR, PROVEEDOR_NOMBRE, AUTORIZACION, VENCIMIENTO, FACTURAK, EJERCICIOK, ERRORES, WARNINGS) {
    var icono, clase = null;
    var bloqueo = [];

    if (jQuery.inArray('red white-text rojo', ERRORES) !== -1) {
        icono = 'close';
        clase = 'red white-text';
    }
    else {
        icono = 'done';
        clase = 'green white-text';
    }

    if (WARNINGS[1] === "") { bloqueo[1] = "disabled"; }
    else { bloqueo[1] = ""; }

    if (WARNINGS[2] === "") { bloqueo[2] = "disabled"; }
    else { bloqueo[2] = ""; }

    if (WARNINGS[3] === "") { bloqueo[3] = "disabled"; }
    else { bloqueo[3] = ""; }

    if (WARNINGS[4] === "") { bloqueo[4] = "disabled"; }
    else { bloqueo[4] = ""; }

    if (WARNINGS[5] === "") { bloqueo[5] = "disabled"; }
    else { bloqueo[5] = ""; }

    if (WARNINGS[6] === "") { bloqueo[6] = "disabled"; }
    else { bloqueo[6] = ""; }

    if (WARNINGS[7] === "") { bloqueo[7] = "disabled"; }
    else { bloqueo[7] = ""; }

    if (WARNINGS[8] === "") { bloqueo[8] = "disabled"; }
    else { bloqueo[8] = ""; }

    var classProveedor = ERRORES[3] !== "" ? ERRORES[3] : WARNINGS[3];
    var r = t.row.add([
        "<div class='" + clase + "'></div>",
        "<input class='" + ERRORES[0] + WARNINGS[0] + " input_numdoc' style='font-size:12px; text-align:center;' type='text' id='' name='' disabled value='" + NUM_DOC + "'><span hidden>" + NUM_DOC + "</span>",
        "<input class='" + ERRORES[1] + WARNINGS[1] + " input_factura' style='font-size:10px; text-align:center;' type='text' id='' name='' " + bloqueo[1] + " value='" + FACTURA + "'><span hidden>" + FACTURA + "</span>",
        "<input class='" + ERRORES[2] + WARNINGS[2] + " input_fechaH2' style='font-size:10px; text-align:center;' type='text' id='' name='' " + bloqueo[2] + " value='" + FECHA + "'><span hidden>" + FECHA + "</span>",
        "<input class='" + classProveedor + " input_proveedor' style='font-size:10px; text-align:center;' type='text' id='' name='' " + bloqueo[3] + " value='" + PROVEEDOR + "'  title='" + getWarning(ERRORES[3]) +"'><span hidden>" + PROVEEDOR + "</span>",
        "<input class='" + ERRORES[4] + WARNINGS[4] + "' style='font-size:10px; text-align:center;' type='text' id='' name='' " + bloqueo[4] + " value='" + PROVEEDOR_NOMBRE + "'><span hidden>" + PROVEEDOR_NOMBRE + "</span>",
        "<input class='" + ERRORES[5] + WARNINGS[5] + " input_autorizacion' style='font-size:10px; text-align:center;' type='text' id='' name='' " + bloqueo[5] + " value='" + AUTORIZACION + "'><span hidden>" + AUTORIZACION + "</span>",
        "<input class='" + ERRORES[6] + WARNINGS[6] + " input_fechaH2' style='font-size:10px; text-align:center;' type='text' id='' name='' " + bloqueo[6] + " value='" + VENCIMIENTO + "'><span hidden>" + VENCIMIENTO + "</span>",
        "<input class='" + ERRORES[7] + WARNINGS[7] + " input_facturak' style='font-size:10px; text-align:center;' type='text' id='' name='' " + bloqueo[7] + " value='" + FACTURAK + "'><span hidden>" + FACTURAK + "</span>",
        "<input class='" + ERRORES[8] + WARNINGS[8] + " input_ejerciciok' style='font-size:10px; text-align:center;' type='text' id='' name='' " + bloqueo[8] + " value='" + EJERCICIOK + "'><span hidden>" + EJERCICIOK + "</span>"
    ]).draw(false).node();

    var td = $(r).children()[0];
    $(td).css('text-align', 'center');

    return r;
}

$('#tab_test2').on('keydown.autocomplete', '.input_factura', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();

    var amarillo = $(tr.find("td:eq(" + col_index + ")").children());
    amarillo = $(amarillo).hasClass("yelloww");

    if (amarillo) {
        amarillo = "blue";
    }
    else {
        amarillo = "";
    }

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'factura',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    if (data) {
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().addClass(amarillo));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                    }
                    else {
                        $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass(amarillo));
                    }
                    clearErrors();
                }
            });
        },
        change: function () {
            if ($(this).val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass(amarillo));
            }
            clearErrors();
        }
    });
});

$('#tab_test2').on('keydown.autocomplete', '.input_fechaH2', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();

    var amarillo = $(tr.find("td:eq(" + col_index + ")").children());
    amarillo = $(amarillo).hasClass("yelloww");

    if (amarillo) {
        amarillo = "blue";
    }
    else {
        amarillo = "";
    }

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'validaFechaH2',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    if (data) {
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().addClass(amarillo));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                    }
                    else {
                        $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass(amarillo));
                    }
                    clearErrors();
                }
            });
        },
        change: function () {
            if ($(this).val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass(amarillo));
            }
            clearErrors();
        }
    });
});//APLICA PARA AMBAS FECHAS EN H2 FECHA FACTURA Y FECHA VENCIMIENTO

$('#tab_test2').on('keydown.autocomplete', '.input_proveedor', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    var col_index2 = col_index + 1;
    var num_docH1 = null;
    var clienteH1 = null;
    var tablaH1 = $('#tab_test1').DataTable();

    var amarillo = $(tr.find("td:eq(" + col_index + ")").children());
    //amarillo = $(amarillo).hasClass("yelloww");
    amarillo.prop('disabled');

    if (amarillo) {
        amarillo = "yelloww blue";
    }
    else {
        amarillo = "";
    }

    $(tr.find('td:eq(1)').children().addClass('' + row_index + 'numDoc1'));
    $(tr.find("td:eq(" + col_index + ")").children().addClass('' + row_index + 'proveedorId' + col_index));
    $(tr.find("td:eq(" + col_index2 + ")").children().addClass('' + row_index + 'proveedorNom' + col_index2));
    var num_doc = $('.' + row_index + 'numDoc1').val();
    var clase2 = '.' + row_index + 'proveedorNom' + col_index2;

    for (var a = 0; a < tablaH1.rows().data().length; a++) {
        var rowH1 = tablaH1.row(a).node();
        num_docH1 = $(rowH1).children().eq(1).children().val();

        if (num_docH1 === num_doc) {
            clienteH1 = $(rowH1).find('td:eq(10)').children().val();
        }
    }

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'proveedor',
                dataType: "json",
                data: { "Prefix": request.term, "Cliente": clienteH1 },
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: trimStart('0', item.ID) + '-' + item.NOMBRE, value: trimStart('0', item.ID) };
                    }));
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item) && $(".input_proveedor").val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass(amarillo));
                clearErrors();
                e.target.value = "";
            }
        },

        select: function (event, ui) {
            var label = ui.item.label;
            label = label.split('-');
            $(clase2).val(label[1]);
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            $(tr.find("td:eq(" + col_index + ")").children().addClass(amarillo));
            $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
            clearErrors();
        }
    });
});

$('#tab_test2').on('keydown.autocomplete', '.input_autorizacion', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();

    var amarillo = $(tr.find("td:eq(" + col_index + ")").children());
    amarillo = $(amarillo).hasClass("yelloww");

    if (amarillo) {
        amarillo = "blue";
    }
    else {
        amarillo = "";
    }

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'autorizacion',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    if (data) {
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().addClass(amarillo));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                    }
                    else {
                        $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass(amarillo));
                    }
                    clearErrors();
                }
            });
        }
    });
});

$('#tab_test2').on('keydown.autocomplete', '.input_facturak', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();

    var amarillo = $(tr.find("td:eq(" + col_index + ")").children());
    amarillo = $(amarillo).hasClass("yelloww");

    if (amarillo) {
        amarillo = "blue";
    }
    else {
        amarillo = "";
    }

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'facturak',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    if (data) {
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().addClass(amarillo));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                    }
                    else {
                        $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass(amarillo));
                    }
                    clearErrors();
                }
            });
        },
        change: function () {
            if ($(this).val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass(amarillo));
            }
            clearErrors();
        }
    });
});

$('#tab_test2').on('keydown.autocomplete', '.input_ejerciciok', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();

    var amarillo = $(tr.find("td:eq(" + col_index + ")").children());
    amarillo = $(amarillo).hasClass("yelloww");

    if (amarillo) {
        amarillo = "blue";
    }
    else {
        amarillo = "";
    }

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'ejerciciok',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    if (data) {
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().addClass(amarillo));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                    } else {
                        $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass(amarillo));
                    }
                    clearErrors();
                }
            });
        },

        change: function () {
            if ($(this).val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass(amarillo));
            }
            clearErrors();
        }
    });
});

$("#tab_rel").click(function () {
    var tbody = $("#tab_test2 tbody");
    var arregloNumDoc = [];

    if (tbody.children().length !== 0) {
        arregloNumDoc = checkRelacionada();

        if (arregloNumDoc.length > 0) {
            var tablaH3 = $('#tab_test3').DataTable();

            for (var a = 0; a < tablaH3.rows().data().length; a++) {
                var rowH3 = tablaH3.row(a).node();
                var num_docH3 = $(rowH3).children().eq(1).children().val();

                if (jQuery.inArray(num_docH3, arregloNumDoc) !== -1) {
                    $(rowH3).children().eq(1).children().addClass("red white-text rojo");
                }
            }
        }
        checkRelacionadaMat();
    }
});
/////////////////////////////////////////////////////////HOJA 3 FUNCIONES Y ASIGNACIONES////////////////////////////////////////////////////////
function procesarHoja3() {
    var table = $('#tab_test3').DataTable({ language: { "url": "../Scripts/lang/" + ln + ".json" }, "order": [1, 'asc']  });

    $.ajax({
        type: "POST",
        url: 'validaHoja3',
        dataType: "json",
        cache: false,
        contentType: false,
        processData: false,
        async: true,
        success: function (data) {

            if (data !== null || data !== "") {
                if (data.isRedirect) {
                    window.onbeforeunload = false;
                    window.location = data.redirectUrl;
                } else {
                    if (data.length > 1) {
                        var errores = data.pop();

                        //INICIO DEL CICLO FOR
                        $.each(data, function (i, dataj) {
                            var addedRow = addRowH3(table, dataj.NUM_DOC, dataj.FACTURA, dataj.BILL_DOC, dataj.EJERCICIOK, dataj.PAYER, dataj.PAYER_NOMBRE, dataj.IMPORTE_FAC, dataj.BELNR, errores[i]);
                        }); //FIN DEL FOR

                        $('#tab_test3').css("font-size", "10px");
                        $('#tab_test3').css("display", "table");
                        mostrarAlerta("info", "B", 'Hoja 3 procesada');
                        generarWarningH3();
                    }
                    else {
                        if (data) {
                            if (data.toString().length > 0)
                                mostrarAlerta("info", "A", data);
                        }
                    }
                }
            }
            cargaInicialSpin[2] = true;
            if (jQuery.inArray(false, cargaInicialSpin) === -1) {
                checkRelacionada();
                checkRelacionadaMat();
                clearErrors();
                document.getElementById("loader").style.display = "none";
            }
        },

        complete: function (data) { },

        error: function (xhr, httpStatusMessage, customErrorMessage) {
            document.getElementById("loader").style.display = "none";
            M.toast({ html: 'La solicitud no pudo ser procesada. Favor de intentarlo mas tarde. La causa ' + xhr.status + " : " + httpStatusMessage + " : " + customErrorMessage });
        }
    });

    //Actualizar los valores en la tabla
    //updateTable();
}

function addRowH3(t, NUM_DOC, FACTURA, BILL_DOC, EJERCICIOK, PAYER, PAYER_NOMBRE, IMPORTE_FAC, BELNR, ERRORES) {
    var icono, clase = null;

    if (jQuery.inArray('red white-text rojo', ERRORES) !== -1) {
        icono = 'close';
        clase = 'red white-text';
    }
    else {
        icono = 'done';
        clase = 'green white-text';
    }

    var r = t.row.add([
        "<div class='" + clase + "'></div>",
        "<input class='" + ERRORES[0] + " input_numdoc' style='font-size:12px; text-align:center;' type='text' id='' name='' disabled value='" + NUM_DOC + "'><span hidden>" + NUM_DOC + "</span>",
        "<input class='" + ERRORES[1] + " input_facturaH3' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + FACTURA + "'><span hidden>" + FACTURA + "</span>",
        "<input class='" + ERRORES[2] + " input_bill' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + BILL_DOC + "'><span hidden>" + BILL_DOC + "</span>",
        "<input class='" + ERRORES[3] + " input_ejerciciokH3' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + EJERCICIOK + "'><span hidden>" + EJERCICIOK + "</span>",
        "<input class='" + ERRORES[4] + " input_cliente' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + PAYER + "'><span hidden>" + PAYER + "</span>",
        "<input class='" + ERRORES[5] + "' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + PAYER_NOMBRE + "'><span hidden>" + PAYER_NOMBRE + "</span>",
        "<input class='" + ERRORES[6] + " input_importe' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + IMPORTE_FAC + "'><span hidden>" + IMPORTE_FAC + "</span>",
        "<input class='" + ERRORES[7] + " input_belnr' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + BELNR + "'><span hidden>" + BELNR + "</span>"
    ]).draw(false).node();

    var td = $(r).children()[0];
    $(td).css('text-align', 'center');

    return r;
}

$('#tab_test3').on('keydown.autocomplete', '.input_facturaH3', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'factura',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    if (data) {
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                        clearErrors();
                        //validarErrores("tab_test2");
                    } else {
                        $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        clearErrors();
                        //validarErrores("tab_test2");
                    }
                }
            });
        },

        change: function () {
            if ($(this).val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            }
            clearErrors();
        }
    });
});

$('#tab_test3').on('keydown.autocomplete', '.input_bill', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'bill',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    if (data) {
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                    } else {
                        $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                    }
                    clearErrors();
                }
            });
        },

        change: function () {
            if ($(this).val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            }
            clearErrors();
        }
    });
});

$('#tab_test3').on('keydown.autocomplete', '.input_ejerciciokH3', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'ejerciciok',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    if (data) {
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                        clearErrors();
                        //validarErrores("tab_test2");
                    } else {
                        $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        clearErrors();
                        //validarErrores("tab_test2");
                    }
                }
            });
        },

        change: function () {
            if ($(this).val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            }
            clearErrors();
        }
    });
});

//PAYER Y NOMBRE PAYER APUNTA A INPUT CLIENTE EN ASIGNACIONES DE HOJA1

$('#tab_test3').on('keydown.autocomplete', '.input_importe', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    var num_docH1 = null;
    var pais = null;
    var tablaH1 = $('#tab_test1').DataTable();

    $(tr.find('td:eq(1)').children().addClass('' + row_index + 'numDoc3'));
    var num_doc = $('.' + row_index + 'numDoc3').val();

    for (var a = 0; a < tablaH1.rows().data().length; a++) {
        var rowH1 = tablaH1.row(a).node();
        num_docH1 = $(rowH1).children().eq(1).children().val();

        if (num_docH1 === num_doc) {
            pais = $(rowH1).find('td:eq(5)').children().val();
        }
    }

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'importe',
                dataType: "json",
                data: { "Prefix": request.term.replace('$', ''), "Pais": pais },
                success: function (data) {

                    var color = data.pop();

                    if (color) {
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().val(data));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                    } else {
                        $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', 'Importe'));
                    }
                    clearErrors();
                }
            });
        },

        change: function () {
            if ($(this).val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                $(tr.find("td:eq(" + col_index + ")").children().prop('title', 'Importe'));
            }
            clearErrors();
        }
    });
});

$('#tab_test3').on('keydown.autocomplete', '.input_belnr', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'belnr',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    if (data) {
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                        $(tr.find("td:eq(" + col_index + ")").children().prop('title', ''));
                    } else {
                        $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                        $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                    }
                    clearErrors();
                }
            });
        },

        change: function () {
            if ($(this).val() === "") {
                $(tr.find("td:eq(" + col_index + ")").children().addClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            }
            clearErrors();
        }
    });
});

$("#tab_mul").click(function () {
    var tbody = $("#tab_test3 tbody");
    var arregloNumDoc = [];

    if (tbody.children().length !== 0) {
        arregloNumDoc = checkRelacionada();

        if (arregloNumDoc.length > 0) {
            var tablaH3 = $('#tab_test3').DataTable();

            for (var a = 0; a < tablaH3.rows().data().length; a++) {
                var rowH3 = tablaH3.row(a).node();
                var num_docH3 = $(rowH3).children().eq(1).children().val();

                if (jQuery.inArray(num_docH3, arregloNumDoc) !== -1) {
                    $(rowH3).children().eq(1).children().addClass("red white-text rojo");
                    $(rowH3).children().eq(1).children().prop('title', 'El documento ya tiene facturas asignadas');
                }
            }
        }
        checkRelacionadaMat();
    }
});

/////////////////////////////////////////////////////////HOJA 4 FUNCIONES Y ASIGNACIONES////////////////////////////////////////////////////////
function procesarHoja4() {
    var table = $('#tab_test4').DataTable({
        language: { "url": "../Scripts/lang/" + ln + ".json" }, "order": [1, 'asc'],
        rowGroup: {
            startRender: null,
            endRender: function (rows, group) {
                var indexApoyo = getTableIndex('#tab_test4', 'lbl_apoyo');
                var ApoyoGroup = rows
                    .data()
                    .pluck(indexApoyo)
                    .reduce(function (a, b) {
                        var html = $.parseHTML(b);
                        var apoyo = 0;
                        if (html.length > 1) {
                            apoyo = toNum(html[0].value, ',', '.');
                        }
                        return a + apoyo;
                    }, 0);
                return $('<tr/>')
                    .append('<td colspan="14"><b>TOTAL</b></td>')
                    .append('<td><b>' + toShow(ApoyoGroup, '.') + '</b></td>');
            },
            dataSrc: 1
        }
       
    });

    $.ajax({
        type: "POST",
        url: 'validaHoja4',
        dataType: "json",
        cache: false,
        contentType: false,
        processData: false,
        async: true,
        success: function (data) {

            if (data !== null || data !== "") {
                if (data.isRedirect) {
                    window.onbeforeunload = false;
                    window.location = data.redirectUrl;
                } else {
                    if (data.length > 1) {
                        var errores = data.pop();

                        //INICIO DEL CICLO FOR
                        $.each(data, function (i, dataj) {
                            addRowH4(table, dataj.NUM_DOC, dataj.LIGADA, dataj.VIGENCIA_DE, dataj.VIGENCIA_AL, dataj.MATNR, dataj.MATKL, dataj.DESCRIPCION, dataj.MONTO, dataj.PORC_APOYO, dataj.APOYO_PIEZA, dataj.COSTO_APOYO, dataj.PRECIO_SUG, dataj.VOLUMEN_REAL, dataj.APOYO, errores[i], dataj.MATKL_ID);
                        }); //FIN DEL FOR

                        $('#tab_test4').css("font-size", "10px");
                        $('#tab_test4').css("display", "table");
                        mostrarAlerta("info", "B", 'Hoja 4 procesada');
                    }
                    else {
                        if (data) {
                            if (data.toString().length > 0)
                                mostrarAlerta("info", "A", data);
                        }
                    }
                }
            }
            cargaInicialSpin[3] = true;
            if (jQuery.inArray(false, cargaInicialSpin) === -1) {
                checkRelacionada();
                checkRelacionadaMat();
                clearErrors();
                document.getElementById("loader").style.display = "none";
            }
        },

        complete: function (data) { },

        error: function (xhr, httpStatusMessage, customErrorMessage) {
            document.getElementById("loader").style.display = "none";
            M.toast({ html: 'La solicitud no pudo ser procesada. Favor de intentarlo mas tarde. La causa ' + xhr.status + " : " + httpStatusMessage + " : " + customErrorMessage });
        }
    });

    //Actualizar los valores en la tabla
    //updateTable();
}

function addRowH4(t, NUM_DOC, LIGADA, VIGENCIA_DE, VIGENCIA_AL, MATNR, MATKL, DESCRIPCION, MONTO, PORC_APOYO, APOYO_PIEZA, COSTO_APOYO, PRECIO_SUG, VOLUMEN_REAL, APOYO, ERRORES, MATKL_ID) {
    var check, bloqueo, icono, clase = null, msj='';

    if (LIGADA !== "") {
        check = "checked='checked'";
        bloqueo = "disabled";
    }
    else {
        check = "";
    }

    if (jQuery.inArray('red white-text rojo', ERRORES) !== -1) {
        icono = 'close';
        clase = 'red white-text';
    }
    else {
        icono = 'done';
        clase = 'green white-text';
    }
    
    var r = t.row.add([
        "<div class='" + clase + "'></div>",
        "<input class='" + ERRORES[0] + " input_numdoc' style='font-size:12px; text-align:center;' type='text' id='' name='' disabled value='" + NUM_DOC + "' title='"+getWarning(ERRORES[0])+"'><span hidden>" + NUM_DOC + "</span>",
        "<p style='text-align:center;' class='" + ERRORES[1] + "'><label><input type='checkbox' class='filled-in ligada' " + check + " onchange='ligada(this);'/><span></span></label></p>",
        "<input class='" + ERRORES[2] + " input_fechaDis' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + VIGENCIA_DE + "' title='" + getWarning(ERRORES[2]) +"'><span hidden>" + VIGENCIA_DE + "</span>",
        "<input class='" + ERRORES[3] + " input_fechaDis' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + VIGENCIA_AL + "' title='" + getWarning(ERRORES[3]) +"'><span hidden>" + VIGENCIA_AL + "</span>",
        "<input class='" + ERRORES[4] + " input_material' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + MATNR + "' title='" + getWarning(ERRORES[4]) +"'><span hidden>" + MATNR + "</span>",
        "<input class='" + ERRORES[5] + " input_categoria' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + MATKL + "' title='" + getWarning(ERRORES[5]) +"'><span hidden>" + MATKL_ID + "</span>",
        "<input class='" + ERRORES[6] + "' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + DESCRIPCION + "'' title='" + getWarning(ERRORES[6]) +"'><span hidden>" + DESCRIPCION + "</span>",
        "<input class='" + ERRORES[7] + " input_cantidades' style='font-size:10px; text-align:center;' type='text' id='' name='' " + bloqueo + " value='" + MONTO + "' ' title='" + getWarning(ERRORES[7]) +"'><span hidden>" + MONTO + "</span>",
        "<input class='" + ERRORES[8] + " input_cantidades' style='font-size:10px; text-align:center;' type='text' id='' name='' value='" + PORC_APOYO + "' ' title='" + getWarning(ERRORES[8]) +"'><span hidden>" + PORC_APOYO + "</span>",
        "<input class='" + ERRORES[9] + " input_cantidades' style='font-size:10px; text-align:center;' type='text' id='' name='' disabled value='" + APOYO_PIEZA + "' ' title='" + getWarning(ERRORES[9]) +"'><span hidden>" + APOYO_PIEZA + "</span>",
        "<input class='" + ERRORES[10] + " input_cantidades' style='font-size:10px; text-align:center;' type='text' id='' name='' disabled value='" + COSTO_APOYO + "' ' title='" + getWarning(ERRORES[10]) +"'><span hidden>" + COSTO_APOYO + "</span>",
        "<input class='" + ERRORES[11] + " input_cantidades' style='font-size:10px; text-align:center;' type='text' id='' name='' " + bloqueo + " value='" + PRECIO_SUG + "'' title='" + getWarning(ERRORES[11]) +"'><span hidden>" + PRECIO_SUG + "</span>",
        "<input class='" + ERRORES[12] + " input_cantidades' style='font-size:10px; text-align:center;' type='text' id='' name='' " + bloqueo + " value='" + VOLUMEN_REAL + "' ' title='" + getWarning(ERRORES[12]) +"'><span hidden>" + VOLUMEN_REAL + "</span>",
        "<input class='" + ERRORES[13] + " input_apoyo' style='font-size:10px; text-align:center;' type='text' id='' name='' " + bloqueo + " value='" + APOYO + "' ' title='" + getWarning(ERRORES[13]) +"'><span hidden>" + APOYO + "</span>"
    ]).draw(false).node();

    var td = $(r).children()[0];
    $(td).css('text-align', 'center');

    return r;
}
$('#tab_test4').on('focusout', '.input_fechaDis', function () {
    var tr = $(this).closest('tr');

    var indexFI = getTableIndex('#tab_test4', 'lbl_fechaInicioH4');
    var indexFF = getTableIndex('#tab_test4', 'lbl_fechaFinH4');
    var indexnumDoc = getTableIndex('#tab_test4', 'lbl_numDocH4');
    var num_doc = $(tr).children().eq(indexnumDoc).children().val();
    var fechaInicio = $(tr).children().eq(indexFI).children().val();
    var fechaFin = $(tr).children().eq(indexFF).children().val();

    var fechaIH1 = "", fechaFH1 = "";
    var indexFechaIni = getTableIndex('#tab_test1', 'lbl_fechaInicio');
    var indexFechaFin = getTableIndex('#tab_test1', 'lbl_fechaFin');
    var indexH1numDoc = getTableIndex('#tab_test1', 'lbl_numDocH1');
    
    var tablaH1 = $('#tab_test1').DataTable();

    for (var c = 0; c < tablaH1.rows().data().length; c++) {
        var rowH1 = tablaH1.row(c).node();
        var num_docH1 = $(rowH1).children().eq(indexH1numDoc).children().val();
        if (num_doc === num_docH1) {
            fechaIH1 = $(rowH1).children().eq(indexFechaIni).children().val();
            fechaFH1 = $(rowH1).children().eq(indexFechaFin).children().val();
            break;
        }
    }
    $.ajax({
        type: "POST",
        url: 'validarFechasDis',
        dataType: "json",
        data: { "FechaIniDis": fechaInicio, "FechaFinDis": fechaFin, "FechaInfoIni": fechaIH1, "FechaInfoFin": fechaFH1 },
        success: function (data) {
            if (data !== null | data !== "") {
                if (data[0] === "") {
                    $(tr).children().eq(indexFI).children().removeClass("red white-text rojo");
                    $(tr).children().eq(indexFI).children().prop('title', '');
                } else {
                    $(tr).children().eq(indexFI).children().addClass("red white-text rojo");
                    $(tr).children().eq(indexFI).children().prop('title', getWarning(data[0]));
                }
                if (data[1] === "") {
                    $(tr).children().eq(indexFF).children().removeClass("red white-text rojo");
                    $(tr).children().eq(indexFF).children().prop('title','');
                } else {
                    $(tr).children().eq(indexFF).children().addClass("red white-text rojo");
                    $(tr).children().eq(indexFF).children().prop('title', getWarning(data[1]));
                }
                clearErrors();
            }
        }
    });
    
});
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
$('#tab_test4').on('keydown.autocomplete', '.input_material', function () {
            var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    col_index = col_index + 2;
    var num_doc = tr.find("td:eq(1)").children().val();

    $(tr.find("td:eq(" + (col_index - 1) + ")").children().addClass('' + row_index + 'categoria' + (col_index - 1)));
    var categoria = '.' + row_index + 'categoria' + (col_index - 1);

    $(tr.find("td:eq(" + col_index + ")").children().addClass('' + row_index + 'descripcion' + col_index));
    var materialDes = '.' + row_index + 'descripcion' + col_index;

    var vk = '', vt = '', indexvt, indexvk, indexnumDoc;

    
    indexvt= getTableIndex('#tab_test1', 'lbl_vtweg');
    indexvk = getTableIndex('#tab_test1', 'lbl_vkorg');
    indexnumDoc = getTableIndex('#tab_test1', 'lbl_numDocH1');

    var tablaH1 = $('#tab_test1').DataTable();
    for (var c = 0; c < tablaH1.rows().data().length; c++) {
        var rowH1 = tablaH1.row(c).node();
        var num_docH1 = $(rowH1).children().eq(indexnumDoc ).children().val();
        if (num_doc === num_docH1) {
            vk = $(rowH1).children().eq(indexvk).children().val();
            vt = $(rowH1).children().eq(indexvt).children().val();
            break;
        }
    }
    
    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: root + 'Listas/materiales',
                dataType: "json",
                data: { "Prefix": request.term, vkorg: vk, vtweg: vt},
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: trimStart('0', item.ID) + " - " + item.MAKTX, value: trimStart('0', item.ID) };
                    }))
                }
            })
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item)) {
                $(this).addClass("red white-text rojo");
                $(tr.find("td:eq(" + (col_index - 1) + ")").children('input').val(""));
                $(tr.find("td:eq(" + (col_index - 1) + ")").children('span').text(""));
                $(tr.find("td:eq(" + (col_index - 1) + ")").children().removeClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().val(""));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                clearErrors();
                e.target.value = "";
            }
        },

        select: function (event, ui) {
            var label = ui.item.label;
            var value = ui.item.value;
            var desc = label.split('-');
            $(materialDes).val(desc[1]);
            var cat = getCategoria(value);
            $(tr.find("td:eq(" + (col_index - 1) + ")").children('input').val(cat.DESCRIPCION));
            $(tr.find("td:eq(" + (col_index - 1) + ")").children('span').text(cat.CATEGORIA_ID));
            
            $(categoria).removeClass("red white-text rojo");
            $(this).removeClass("red white-text rojo");
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            var indexMaterial = getTableIndex('#tab_test4', 'lbl_material');
            tr.find("td:eq(" + indexMaterial + ")").children().val(value);
            healtyMaterialCategoria(num_doc);
            clearErrors();
        }
    });
});

$('#tab_test4').on('keydown.autocomplete', '.input_categoria', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    var num_doc = tr.find("td:eq(1)").children().val();

    $(tr.find("td:eq(" + col_index + ")").children().addClass('' + row_index + 'categoriaDes' + col_index));

    var vk = '', kunnr = '', spart = '', soc = '', indexvt, indexkunnr, indexnumDoc, indexspart, indexsoc;


    indexvk = getTableIndex('#tab_test1', 'lbl_vkorg');
    indexkunnr = getTableIndex('#tab_test1', 'lbl_payerId');
    indexnumDoc = getTableIndex('#tab_test1', 'lbl_numDocH1'); 
    indexspart = getTableIndex('#tab_test1', 'lbl_spart');
    indexsoc = getTableIndex('#tab_test1', 'lbl_sociedad');

    var tablaH1 = $('#tab_test1').DataTable();
    for (var c = 0; c < tablaH1.rows().data().length; c++) {
        var rowH1 = tablaH1.row(c).node();
        var num_docH1 = $(rowH1).children().eq(indexnumDoc).children().val();
        if (num_doc === num_docH1) {
            vk = $(rowH1).children().eq(indexvk).children().val();
            kunnr = $(rowH1).children().eq(indexkunnr).children().val();
            spart = $(rowH1).children().eq(indexspart).children().val();
            soc = $(rowH1).children().eq(indexsoc).children().val();
            break;
        }
    }

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: root + 'Listas/categoriasCliente',
                dataType: "json",
                data: { vkorg: vk, spart: spart, kunnr: kunnr, soc_id: soc },
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: trimStart('0', item.MATERIALGP_ID) + '-' + item.TXT50, value: item.TXT50, idCat: item.MATERIALGP_ID };
                    }));
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item)) {
                $(tr.find("td:eq(" + (col_index - 1) + ")").children().val(""));
                $(tr.find("td:eq(" + (col_index + 1) + ")").children().val(""));
                $(this).addClass("red white-text rojo");
                $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
                $(tr.find("td:eq(" + col_index + ")").children('span').text(""));
                clearErrors();
                e.target.value = "";
            }
        },

        select: function (event, ui) {
            $(tr.find("td:eq(" + (col_index - 1) + ")").children().val(""));
            $(tr.find("td:eq(" + (col_index + 1) + ")").children().val(""));
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("red white-text rojo"));
            $(tr.find("td:eq(" + col_index + ")").children().removeClass("ui-autocomplete-loading"));
            var idCategoria= tr.find("td:eq(" + col_index + ")").children('span');
            idCategoria.text(ui.item.idCat);
            var indexCat = getTableIndex('#tab_test4', 'lbl_categoriaH4');
            tr.find("td:eq(" + indexCat + ")").children().val(ui.item.idCat);
            healtyMaterialCategoria(num_doc);
            clearErrors();
        }
    });

});

$('#tab_test4').on('focusout', '.input_cantidades', function () {
    var tr = $(this).closest('tr'); //Obtener el row
    var row_index = $(this).parent().parent().index();
    var col_index = $(this).parent().index();
    var num_docH1 = null, pais = null, getDec = null, getMiles = null;
    var num1 = null, num2 = null;
    var tablaH1 = $('#tab_test1').DataTable();
    var num_docGlobal = tr.find("td:eq(1)").children().val();

    var monto = null, porApoyo = null, pieApoyo = null, cosApoyo = null, preSugerido = null, volReal = null, apoyo = null;

    var indexMonto = getTableIndex('#tab_test4', 'lbl_monto'),
        indexPorApoyo = getTableIndex('#tab_test4', 'lbl_porcentaje'),
        indexPieApoyo = getTableIndex('#tab_test4', 'lbl_apoyoPieza'),
        indexCosApoyo = getTableIndex('#tab_test4', 'lbl_costo'),
        indexPreSugerido = getTableIndex('#tab_test4', 'lbl_precio'),
        indexVolReal = getTableIndex('#tab_test4', 'lbl_volumen'),
        indexApoyo = getTableIndex('#tab_test4', 'lbl_apoyo');

    var colMonto = tr.find('td:eq(' + indexMonto + ') input');//8
    var colPorApoyo = tr.find('td:eq(' + indexPorApoyo + ') input'); //9
    var colPieApoyo = tr.find('td:eq(' + indexPieApoyo + ') input');//10
    var colCosApoyo = tr.find('td:eq(' + indexCosApoyo + ') input');//11
    var colPreSugerido = tr.find('td:eq(' + indexPreSugerido + ') input');//12
    var colVolReal = tr.find('td:eq(' + indexVolReal + ') input');//13
    var colApoyo = tr.find('td:eq(' + indexApoyo + ') input');//14

    $(tr.find('td:eq(1)').children().addClass('' + row_index + 'numDoc4'));
    var num_doc = $('.' + row_index + 'numDoc4').val();

    ///OBTENER PAIS , FORMATO DECIMALES Y MILES POR  NUMERO DE DOCUMENTO 
    for (var a = 0; a < tablaH1.rows().data().length; a++) {
        var rowH1 = tablaH1.row(a).node();
        num_docH1 = $(rowH1).children().eq(1).children().val();
        var indexPais = getTableIndex('#tab_test1', 'lbl_pais');
        var indexDecimal = getTableIndex('#tab_test1', 'lbl_decimales');
        var indexMiles = getTableIndex('#tab_test1', 'lbl_miles');

        if (num_docH1 === num_doc) {
            pais = $(rowH1).find('td:eq(' + indexPais + ')').children().val();//5
            getDec = $(rowH1).find('td:eq(' + indexDecimal + ')').children().val();
            getMiles = $(rowH1).find('td:eq(' + indexMiles + ')').children().val();
        }
    }

    if (getDec === '.') {
        monto = tr.find('td:eq(' + indexMonto+') input').val().replace('$', '').replace(',', ''); //8
        porApoyo = tr.find('td:eq(' + indexPorApoyo+') input').val().replace('%', '').replace(',', '');//9
        pieApoyo = tr.find('td:eq('+indexPieApoyo+') input').val().replace('$', '').replace(',', '');//10
        cosApoyo = tr.find('td:eq(' + indexCosApoyo+') input').val().replace('$', '').replace(',', '');//11
        preSugerido = tr.find('td:eq(' + indexPreSugerido+') input').val().replace('$', '').replace(',', '');//12
        volReal = tr.find('td:eq(' + indexVolReal+') input').val().replace('$', '').replace(',', '');//13
        apoyo = tr.find('td:eq(' + indexApoyo+') input').val().replace('$', '').replace(',', '');//14
    }
    else {
        monto = tr.find('td:eq(' + indexMonto +') input').val().replace('$', '').replace('.', '').replace(',', '.');
        porApoyo = tr.find('td:eq(' + indexPorApoyo +') input').val().replace('%', '').replace('.', '').replace(',', '.');
        pieApoyo = tr.find('td:eq(' + indexPieApoyo +') input').val().replace('$', '').replace('.', '').replace(',', '.');
        cosApoyo = tr.find('td:eq(' + indexCosApoyo +') input').val().replace('$', '').replace('.', '').replace(',', '.');
        preSugerido = tr.find('td:eq(' + indexPreSugerido +') input').val().replace('$', '').replace('.', '').replace(',', '.');
        volReal = tr.find('td:eq(' + indexVolReal +') input').val().replace('$', '').replace('.', '').replace(',', '.');
        apoyo = tr.find('td:eq(' + indexApoyo +') input').val().replace('$', '').replace('.', '').replace(',', '.');
    }

            //VISTA PARA EL MONTO
            if ($.isNumeric(monto)) {
                if (monto > 0) {
                    colMonto.val(toShow(monto, getDec));
                    colMonto.removeClass("red white-text rojo");
                }
                else {
                    colMonto.val(toShow(monto, getDec));
                    colMonto.removeClass("red white-text rojo");
                }
            }
            else if (monto === '') {
                colMonto.val(toShow('0', getDec));
                colMonto.removeClass("red white-text rojo");
            }
            else {
                colMonto.addClass("red white-text rojo");
            }

            //VISTA PARA EL PORCENTAJE DE APOYO
            if ($.isNumeric(porApoyo)) {
                if (porApoyo > 0) {
                    colPorApoyo.val(toShowPorc(porApoyo, getDec));
                    colPorApoyo.removeClass("red white-text rojo");
                    validaLigada(num_docGlobal);
                }
                else {
                    var varValidaLigada = tr.find('td:eq(2) input').prop('checked');

                    if (varValidaLigada) {
                        colPorApoyo.val(toShowPorc(porApoyo, getDec));
                        colPorApoyo.addClass("red white-text rojo");
                        validaLigada(num_docGlobal);
                    }
                    else {
                        colPorApoyo.val(toShowPorc(porApoyo, getDec));
                        colPorApoyo.removeClass("red white-text rojo");
                        validaLigada(num_docGlobal);
                    }
                }
            }
            else if (porApoyo === '') {
                colPorApoyo.val(toShowPorc('0', getDec));
                colPorApoyo.removeClass("red white-text rojo");
                validaLigada(num_docGlobal);
            }
            else {
                colPorApoyo.addClass("red white-text rojo");
            }

            ////VISTA PARA EL PORCENTAJE DE APOYO
            //if ($.isNumeric(porApoyo)) {
            //    if (porApoyo > 0) {
            //        colPorApoyo.val(toShowPorc(porApoyo, getDec));
            //        colPorApoyo.removeClass("red white-text rojo");
            //    }
            //    else {
            //        colPorApoyo.val(toShowPorc(porApoyo, getDec));
            //        colPorApoyo.removeClass("red white-text rojo");
            //    }
            //}
            //else if (porApoyo == '') {
            //    colPorApoyo.val(toShowPorc('0', getDec));
            //    colPorApoyo.removeClass("red white-text rojo");
            //}
            //else {
            //    colPorApoyo.addClass("red white-text rojo");
            //}

            //VISTA PARA EL APOYO POR PIEZA
            if ($.isNumeric(pieApoyo)) {
                if (pieApoyo > 0) {
                    colPieApoyo.val(toShow(pieApoyo, getDec));
                    colPieApoyo.removeClass("red white-text rojo");
                }
                else {
                    colPieApoyo.val(toShow(pieApoyo, getDec));
                    colPieApoyo.removeClass("red white-text rojo");
                }
            }
            else if (pieApoyo === '') {
                colPieApoyo.val(toShow('0', getDec));
                colPieApoyo.removeClass("red white-text rojo");
            }
            else {
                colPieApoyo.addClass("red white-text rojo");
            }

            //VISTA PARA EL COSTO CON APOYO
            if ($.isNumeric(cosApoyo)) {
                if (cosApoyo > 0) {
                    colCosApoyo.val(toShow(cosApoyo, getDec));
                    colCosApoyo.removeClass("red white-text rojo");
                }
                else {
                    colCosApoyo.val(toShow(cosApoyo, getDec));
                    colCosApoyo.removeClass("red white-text rojo");
                }
            }
            else if (cosApoyo === '') {
                colCosApoyo.val(toShow('0', getDec));
                colCosApoyo.removeClass("red white-text rojo");
            }
            else {
                colCosApoyo.addClass("red white-text rojo");
            }

            //VISTA PARA EL PRECIO SUGERIDO
            if ($.isNumeric(preSugerido)) {
                if (preSugerido > 0) {
                    colPreSugerido.val(toShow(preSugerido, getDec));
                    colPreSugerido.removeClass("red white-text rojo");
                }
                else {
                    colPreSugerido.val(toShow(preSugerido, getDec));
                    colPreSugerido.removeClass("red white-text rojo");
                }
            }
            else if (preSugerido === '') {
                colPreSugerido.val(toShow('0', getDec));
                colPreSugerido.removeClass("red white-text rojo");
            }
            else {
                colPreSugerido.addClass("red white-text rojo");
            }

            //VISTA PARA EL VOLUMEN REAL
            if ($.isNumeric(volReal)) {
                if (volReal > 0) {
                    colVolReal.val(toShowNum(volReal, getDec));
                    colVolReal.removeClass("red white-text rojo");
                }
                else {
                    colVolReal.val(toShowNum(volReal, getDec));
                    colVolReal.removeClass("red white-text rojo");
                }
            }
            else if (volReal === '') {
                colVolReal.val(toShowNum('0', getDec));
                colVolReal.removeClass("red white-text rojo");
            }
            else {
                colVolReal.addClass("red white-text rojo");
            }

            //VISTA PARA EL APOYO
            if ($.isNumeric(apoyo)) {
                if (apoyo > 0) {
                    if (($.isNumeric(monto) & monto !== '0.00') & ($.isNumeric(porApoyo) & porApoyo !== '0.00') & ($.isNumeric(volReal) & volReal !== '0.00')) {
                        num1 = monto * (porApoyo / 100);
                        num2 = num1 * volReal;

                        colPieApoyo.val(toShow(num1, getDec));
                        colCosApoyo.val(toShow(monto - num1, getDec));
                        colApoyo.val(toShow(num2, getDec));

                        validaApoyo(num2, colApoyo);
                    }
                    else if ((monto === "" || porApoyo === "" || volReal === "") || (monto === "0.00" || porApoyo === "0.00" || volReal === "0.00")) {
                        if ((monto === "" && porApoyo === "" && volReal === "") || (monto === "0.00" && porApoyo === "0.00" && volReal === "0.00") && apoyo !== "") {
                            colMonto.val(toShow('0', getDec));
                            colPorApoyo.val(toShowPorc('0', getDec));
                            colPieApoyo.val(toShow('0', getDec));
                            colCosApoyo.val(toShow('0', getDec));
                            colPreSugerido.val(toShow('0', getDec));
                            colVolReal.val(toShowNum('0', getDec));
                            colApoyo.val(toShow(apoyo, getDec));
                            checkRelacionadaMat();
                        }
                        else {
                            num1 = monto * (porApoyo / 100);
                            num2 = num1 * volReal;

                            colPieApoyo.val(toShow(num1, getDec));
                            colCosApoyo.val(toShow(monto - num1, getDec));
                            colApoyo.val(toShow(num2, getDec));

                            validaApoyo(num2, colApoyo);
                        }
                    }
                }
                else {
                    if ($.isNumeric(monto) & $.isNumeric(porApoyo) & $.isNumeric(volReal)) {
                        num1 = monto * (porApoyo / 100);
                        num2 = num1 * volReal;

                        colPieApoyo.val(toShow(num1, getDec));
                        colCosApoyo.val(toShow(monto - num1, getDec));
                        colApoyo.val(toShow(num2, getDec));

                        validaApoyo(num2, colApoyo);
                    }
                }
            }
            else if (apoyo === '') {
                if ($.isNumeric(monto) & $.isNumeric(porApoyo) & $.isNumeric(volReal)) {
                    num1 = monto * (porApoyo / 100);
                    num2 = num1 * volReal;

                    colPieApoyo.val(toShow(num1, getDec));
                    colCosApoyo.val(toShow(monto - num1, getDec));
                    colApoyo.val(toShow(num2, getDec));

                    validaApoyo(num2, colApoyo);
                }
            }
    getTotalApoyo(num_doc);
});

$('#tab_test4').on('keydown', '.input_apoyo', function (e) {

    if (e.keyCode === 13) {
        var tr = $(this).closest('tr'); //Obtener el row
        var row_index = $(this).parent().parent().index();
        var num_docH1 = null, pais = null, getDec = null, getMiles = null;
        var tablaH1 = $('#tab_test1').DataTable();
        var apoyo = null;


        var indexMonto = getTableIndex('#tab_test4', 'lbl_monto'),
            indexPorApoyo = getTableIndex('#tab_test4', 'lbl_porcentaje'),
            indexPieApoyo = getTableIndex('#tab_test4', 'lbl_apoyoPieza'),
            indexCosApoyo = getTableIndex('#tab_test4', 'lbl_costo'),
            indexPreSugerido = getTableIndex('#tab_test4', 'lbl_precio'),
            indexVolReal = getTableIndex('#tab_test4', 'lbl_volumen'),
            indexApoyo = getTableIndex('#tab_test4', 'lbl_apoyo');

        var colMonto = tr.find('td:eq(' + indexMonto + ') input');//8
        var colPorApoyo = tr.find('td:eq(' + indexPorApoyo + ') input'); //9
        var colPieApoyo = tr.find('td:eq(' + indexPieApoyo + ') input');//10
        var colCosApoyo = tr.find('td:eq(' + indexCosApoyo + ') input');//11
        var colPreSugerido = tr.find('td:eq(' + indexPreSugerido + ') input');//12
        var colVolReal = tr.find('td:eq(' + indexVolReal + ') input');//13
        var colApoyo = tr.find('td:eq(' + indexApoyo + ') input');//14


        $(tr.find('td:eq(1)').children().addClass(row_index + 'numDoc4'));
        var num_doc = $('.' + row_index + 'numDoc4').val();

        ///OBTENER PAIS , FORMATO DECIMALES Y MILES POR  NUMERO DE DOCUMENTO 
        for (var a = 0; a < tablaH1.rows().data().length; a++) {
            var rowH1 = tablaH1.row(a).node();
            num_docH1 = $(rowH1).children().eq(1).children().val();
            var indexPais = getTableIndex('#tab_test1', 'lbl_pais');
            var indexDecimal = getTableIndex('#tab_test1', 'lbl_decimales');
            var indexMiles = getTableIndex('#tab_test1', 'lbl_miles');

            if (num_docH1 === num_doc) {
                pais = $(rowH1).find('td:eq(' + indexPais + ')').children().val();//5
                getDec = $(rowH1).find('td:eq(' + indexDecimal + ')').children().val();
                getMiles = $(rowH1).find('td:eq(' + indexMiles + ')').children().val();
            }
        }
        if (getDec === '.') {
            apoyo = tr.find('td:eq(14) input').val().replace('$', '').replace(',', '');
        }
        else {
            apoyo = tr.find('td:eq(14) input').val().replace('$', '').replace('.', '').replace(',', '.');
        }

        //VISTA PARA EL APOYO
        e.preventDefault();

        if ($.isNumeric(apoyo)) {
            colMonto.val(toShow('0', getDec));
            colPorApoyo.val(toShowPorc('0', getDec));
            colPieApoyo.val(toShow('0', getDec));
            colCosApoyo.val(toShow('0', getDec));
            colPreSugerido.val(toShow('0', getDec));
            colVolReal.val(toShowNum('0', getDec));
            colApoyo.val(toShow(apoyo, getDec));
        }

        colApoyo.blur();
        checkRelacionadaMat();
        getTotalApoyo(num_doc);


        validaApoyo(apoyo, colApoyo);
    }
});


function healtyMaterialCategoria(num_doc) {
    var tablaH4 = $('#tab_test4').DataTable();
    var rowsMaterial = [];
    var contador = 0, indexNumDoc, indexMaterial, indexCategoria;
    indexNumDoc = getTableIndex('#tab_test4', 'lbl_numDocH4');
    indexMaterial = getTableIndex('#tab_test4', 'lbl_material');
    indexCategoria = getTableIndex('#tab_test4', 'lbl_categoriaH4');

    for (var a = 0; a < tablaH4.rows().data().length; a++) {
        var rowH4 = tablaH4.row(a).node();
        var num_docH4 = $(rowH4).children().eq(indexNumDoc).children().val();
        var material = $(rowH4).children().eq(indexMaterial).children().val();
        var categoria = $(rowH4).children().eq(indexCategoria).children('span').text();

        if (num_doc === num_docH4) {
            rowsMaterial.push({ material: material, categoria: categoria });
            contador++;
        }
    }

    $.ajax({
        type: "POST",
        url: 'ValidarCategoriasMateriles',
        dataType: "json",
        data: { "materialCategorias": rowsMaterial },
        async: true,
        success: function (data) {
            categorias = false;
            if (data !== null | data !== "") {
                data.forEach(function (element) {
                    if ((element.Material === "" || element.Material === null) && (element.Categoria !== "" && element.Categoria !== null)) {
                        categorias = true;
                    }
                });

                var tablaH4 = $('#tab_test4').DataTable();

                for (var a = 0; a < tablaH4.rows().data().length; a++) {
                    var rowH4 = tablaH4.row(a).node();
                    var num_docH4 = $(rowH4).children().eq(1).children().val();

                    if (num_doc === num_docH4) {
                        indexMaterial = getTableIndex('#tab_test4', 'lbl_material');
                        indexCategoria = getTableIndex('#tab_test4', 'lbl_categoriaH4');
                        categoria = $(rowH4).children().eq(indexCategoria).children('span').text();
                        material = $(rowH4).children().eq(indexMaterial).children().val();
                        material = material === null ? "" : material;
                        for (i = 0; i < data.length; i++) {
                            var obj = data[i];
                            var mat = (obj.Material !== "" && obj.Material !== null) ? trimStart('0', obj.Material) : "";
                            var cat = (obj.Categoria !== "" && obj.Categoria !== null) ? obj.Categoria : "";
                            if (categoria === cat && material === mat) {
                                var colCategoria = $(rowH4).children().eq(indexCategoria);
                                var colMaterial = $(rowH4).children().eq(indexMaterial);
                                var inputCategoria = colCategoria.children('input');
                                var inputMaterial = colMaterial.children('input');

                                if (obj.Error) {
                                    if ((obj.Categoria !== "" && obj.Categoria !== null) && (obj.Material !== "" && obj.Material !== null)) {
                                        inputMaterial.prop('title', obj.Msj);
                                        inputCategoria.prop('title', "");
                                        colMaterial.children().addClass('red white-text rojo');
                                        colCategoria.children().removeClass('red white-text rojo');
                                    } else {
                                        inputMaterial.prop('title', "");
                                        inputCategoria.prop('title', obj.Msj);
                                        colCategoria.children().addClass('red white-text rojo');
                                        colMaterial.children().removeClass('red white-text rojo');
                                    }
                                } else {
                                    if ((obj.Categoria === "" || obj.Categoria === null) && (obj.Material === "" || obj.Material === null)) {

                                        if (categorias) {
                                            colCategoria.children().addClass('red white-text rojo');
                                            colMaterial.children().removeClass('red white-text rojo');
                                        } else {
                                            colMaterial.children().addClass('red white-text rojo');
                                            colCategoria.children().removeClass('red white-text rojo');
                                        }
                                    }
                                    else {
                                        inputMaterial.prop('title', "");
                                        inputCategoria.prop('title', "");
                                        colMaterial.children().removeClass('red white-text rojo');
                                        colCategoria.children().removeClass('red white-text rojo');
                                    }
                                }
                            }
                        }
                    }
                }
                clearErrors();
            }
        }
    });
}

function validaApoyo(apoyo, colApoyo) {
    var tr = $(colApoyo).closest('tr'); //Obtener el row
    var errorSumMateriales = tr.find('td:eq(14) input').hasClass("errorCantidades");
    //var validaLigada = tr.find('td:eq(2) input').attr('checked');
    var validaLigada = tr.find('td:eq(2) input').prop('checked');

    if (validaLigada) {
        colApoyo.removeClass("red white-text rojo");
        clearErrors();
    }
    else {
        if (apoyo !== "") {
            if ($.isNumeric(apoyo)) {
                if (apoyo !== '0.00' & apoyo > '0.00') {
                    //if (!errorSumMateriales) {

                    //}
                    colApoyo.removeClass("red white-text rojo");
                    clearErrors();
                }
                else {
                    colApoyo.addClass("red white-text rojo");
                    clearErrors();
                }
            }
            else {
                colApoyo.addClass("red white-text rojo");
                clearErrors();
            }
        }
        else {
            colApoyo.addClass("red white-text rojo");
            clearErrors();
        }
    }

    checkRelacionadaMat();
}

function validaLigada(num_doc) {
    var tablaH4 = $('#tab_test4').DataTable();
    var contador = 0, contienePalabra = -1;;
    var ligadaPorcentaje = [], ligadaPorcentaje2 = [];
    var porcentaje = "";

    for (var a = 0; a < tablaH4.rows().data().length; a++) {
        var rowH4 = tablaH4.row(a).node();
        var num_docH4 = $(rowH4).children().eq(1).children().val();
        var check = $(rowH4).children().eq(2).children().eq(0).children().eq(0).children().eq(0);
        porcentaje = $(rowH4).children().eq(9).children().val();

        if ($(check).is(":checked") && num_docH4 === num_doc) {
            ligadaPorcentaje[contador] = num_docH4 + true + porcentaje;
            ligadaPorcentaje2[contador] = num_docH4 + true;
            //ligadaPorcentaje[contador] = num_docH4 + "-" + true + "-" + porcentaje;
            contador++;
        }
        else {
            if (num_docH4 === num_doc) {
                ligadaPorcentaje[contador] = num_docH4 + false + porcentaje;
                ligadaPorcentaje2[contador] = num_docH4 + false;
                //ligadaPorcentaje[contador] = num_docH4 + "-" + false + "-" + porcentaje;
                contador++;
            }
        }
    }

    Array.prototype.allValuesSame = function () {

        for (var i = 1; i < this.length; i++) {
            if (this[i] !== this[0])
                return false;
        }

        return true;
    }

    var igualesCompleto = ligadaPorcentaje.allValuesSame();
    var igualesCompleto2 = ligadaPorcentaje2.allValuesSame();

    if (ligadaPorcentaje2.length > 0) {
        contienePalabra = ligadaPorcentaje2[0].indexOf("false");
    }
    
    if (igualesCompleto) {
        var tablaH4 = $('#tab_test4').DataTable();

        for (var a = 0; a < tablaH4.rows().data().length; a++) {
            var rowH4 = tablaH4.row(a).node();
            var num_docH4 = $(rowH4).children().eq(1).children().val();
            var check = $(rowH4).children().eq(2).children().eq(0).children().eq(0).children().eq(0);

            if (num_docH4 === num_doc) {
                $(rowH4).children().eq(2).children().removeClass("red white-text rojo");
                $(rowH4).children().eq(9).children().removeClass("red white-text rojo");
            }
        }
    }
    else {
        if (igualesCompleto2 && contienePalabra === 1) {
            var tablaH4 = $('#tab_test4').DataTable();

            for (var a = 0; a < tablaH4.rows().data().length; a++) {
                var rowH4 = tablaH4.row(a).node();
                var num_docH4 = $(rowH4).children().eq(1).children().val();
                var check = $(rowH4).children().eq(2).children().eq(0).children().eq(0).children().eq(0);

                if (num_docH4 === num_doc) {
                    $(rowH4).children().eq(2).children().removeClass("red white-text rojo");
                    $(rowH4).children().eq(9).children().removeClass("red white-text rojo");
                }
            }
        }
        else {
            var tablaH4 = $('#tab_test4').DataTable();

            for (var a = 0; a < tablaH4.rows().data().length; a++) {
                var rowH4 = tablaH4.row(a).node();
                var num_docH4 = $(rowH4).children().eq(1).children().val();
                var check = $(rowH4).children().eq(2).children().eq(0).children().eq(0).children().eq(0);

                if (num_docH4 === num_doc) {
                    $(rowH4).children().eq(2).children().addClass("red white-text rojo");
                    $(rowH4).children().eq(9).children().addClass("red white-text rojo");
                }
            }
        }
    }

    //checa si la columna ligada esta bien 
    var arrLigada = [], arrPorcentaje = [];
    var contadorLigada = 0, contadorPorcentaje = 0;

    for (var a = 0; a < tablaH4.rows().data().length; a++) {
        var rowH4 = tablaH4.row(a).node();
        var num_docH4 = $(rowH4).children().eq(1).children().val();
        var porcentajeH4 = $(rowH4).children().eq(9).children().val();
        var check = $(rowH4).children().eq(2).children().eq(0).children().eq(0).children().eq(0);

        if ($(check).is(":checked") && num_docH4 === num_doc) {
            arrLigada[contadorLigada] = num_docH4 + true;
            arrPorcentaje[contadorLigada] = num_docH4 + porcentajeH4;
            contadorLigada++;
        }
        else {
            if (num_docH4 === num_doc) {
                arrLigada[contadorLigada] = num_docH4 + false;
                arrPorcentaje[contadorLigada] = num_docH4 + porcentajeH4;
                contadorLigada++;
            }
        }
    }

    var igualesLigada = arrLigada.allValuesSame();

    for (var b = 0; b < tablaH4.rows().data().length; b++) {
        var rowbH4 = tablaH4.row(b).node();
        var num_docbH4 = $(rowbH4).children().eq(1).children().val();
        var check = $(rowbH4).children().eq(2).children().eq(0).children().eq(0).children().eq(0);
        var checkeado = $(check).is(":checked");

        if (num_docbH4 === num_doc) {
            if (igualesLigada) {
                $(rowbH4).children().eq(2).children().removeClass("red white-text rojo");
                revisaPorcentajeLigada(num_docbH4, checkeado);
            }
            else {
                $(rowbH4).children().eq(2).children().addClass("red white-text rojo");
                revisaPorcentajeLigada(num_docbH4, checkeado);
            }
        }
    }

    clearErrors();
}

function revisaPorcentajeLigada(num_doc, checked) {
    var tablaH4 = $('#tab_test4').DataTable();
    var arrPorcentajes = [];
    var contador = 0;

    for (var a = 0; a < tablaH4.rows().data().length; a++) {
        var rowH4 = tablaH4.row(a).node();
        var num_docH4 = $(rowH4).children().eq(1).children().val();
        var porcentaje = $(rowH4).children().eq(9).children().val();

        if (num_docH4 === num_doc) {
            arrPorcentajes[contador] = porcentaje;
            contador++;
        }
    }

    Array.prototype.allValuesSame = function () {

        for (var i = 1; i < this.length; i++) {
            if (this[i] !== this[0])
                return false;
        }

        return true;
    }

    var igualesPorcentaje = arrPorcentajes.allValuesSame();

    for (var b = 0; b < tablaH4.rows().data().length; b++) {
        var rowH4 = tablaH4.row(b).node();
        var num_docH4 = $(rowH4).children().eq(1).children().val();
        var porcentaje = $(rowH4).children().eq(9).children().val();

        if (num_docH4 === num_doc & checked & igualesPorcentaje) {
            $(rowH4).children().eq(9).children().removeClass("red white-text rojo");
        }
        else if (num_docH4 === num_doc & checked & !igualesPorcentaje) {
            $(rowH4).children().eq(9).children().addClass("red white-text rojo");
        }
    }
}

function ligada(check) {
    var tr = $(check).closest('tr');//Obtener el row
    var tablaH1 = $('#tab_test1').DataTable();
    var num_docH1 = null, pais = null, getDec = null, getMiles= null;
    var num_docGlobal = tr.find("td:eq(1)").children().val();

    var colMonto = tr.find('td:eq(8) input');
    var colPorcentaje = tr.find('td:eq(9) input');
    var colPieApoyo = tr.find('td:eq(10) input');
    var colCosApoyo = tr.find('td:eq(11) input');
    var colPreSugerido = tr.find('td:eq(12) input');
    var colVolReal = tr.find('td:eq(13) input');
    var colApoyo = tr.find('td:eq(14) input');
    var num_doc = tr.find('td:eq(1) input').val();

    for (var a = 0; a < tablaH1.rows().data().length; a++) {
        var rowH1 = tablaH1.row(a).node();
        num_docH1 = $(rowH1).children().eq(1).children().val();
        var indexPais = getTableIndex('#tab_test1', 'lbl_pais');
        var indexDecimal = getTableIndex('#tab_test1', 'lbl_decimales');
        var indexMiles = getTableIndex('#tab_test1', 'lbl_miles');

        if (num_docH1 === num_doc) {
            pais = $(rowH1).find('td:eq(' + indexPais + ')').children().val();//5
            getDec = $(rowH1).find('td:eq(' + indexDecimal + ')').children().val();
            getMiles = $(rowH1).find('td:eq(' + indexMiles + ')').children().val();
        }
    }
    if (getDec === '.') {
        apoyo = tr.find('td:eq(14) input').val().replace('$', '').replace(',', '');
    }
    else {
        apoyo = tr.find('td:eq(14) input').val().replace('$', '').replace('.', '').replace(',', '.');
    }

    var porcentajeApoyo = colPorcentaje.val().replace('%', '');

    if ($.isNumeric(porcentajeApoyo)) {
        if (porcentajeApoyo > 0) {
            colPorcentaje.val(toShowPorc(porcentajeApoyo, getDec));
        }
        else {
            colPorcentaje.addClass("red white-text rojo");
        }
    }
    else {
        colPorcentaje.addClass("red white-text rojo");
    }

    if ($(check).is(":checked")) {
        colMonto.val(toShow('0', getDec)).attr("disabled", true);
        colPieApoyo.val(toShow('0', getDec)).attr("disabled", true);
        colCosApoyo.val(toShow('0', getDec)).attr("disabled", true);
        colPreSugerido.val(toShow('0', getDec)).attr("disabled", true);
        colVolReal.val(toShowNum('0', getDec)).attr("disabled", true);
        colApoyo.val(toShow('0', getDec)).attr("disabled", true);
        colApoyo.removeClass("red white-text rojo");
        validaLigada(num_docGlobal);
        clearErrors();
    } else {
        colMonto.val(toShow('0', getDec)).attr("disabled", false);
        colPorcentaje.removeClass("red white-text rojo");
        colPieApoyo.val(toShow('0', getDec)).attr("disabled", false);
        colCosApoyo.val(toShow('0', getDec)).attr("disabled", false);
        colPreSugerido.val(toShow('0', getDec)).attr("disabled", false);
        colVolReal.val(toShowNum('0', getDec)).attr("disabled", false);
        colApoyo.val(toShow('0', getDec)).attr("disabled", false);
        colApoyo.addClass("red white-text rojo");
        validaLigada(num_docGlobal);
        clearErrors();
    }

}

$("#tab_dis").click(function () {
    var tbody = $("#tab_test4 tbody");
    var arregloNumDoc = [];

    if (tbody.children().length !== 0) {
        arregloNumDoc = checkRelacionada();

        if (arregloNumDoc.length > 0) {
            var tablaH3 = $('#tab_test3').DataTable();

            for (var a = 0; a < tablaH3.rows().data().length; a++) {
                var rowH3 = tablaH3.row(a).node();
                var num_docH3 = $(rowH3).children().eq(1).children().val();

                if (jQuery.inArray(num_docH3, arregloNumDoc) !== -1) {
                    $(rowH3).children().eq(1).children().addClass("red white-text rojo");
                    $(rowH3).children().eq(1).children().prop('title', 'El documento ya tiene facturas asignadas');
                }
            }
        }
        checkRelacionadaMat();
    }
});

/////////////////////////////////////////////////////////HOJA 5 FUNCIONES Y ASIGNACIONES////////////////////////////////////////////////////////
function procesarHoja5() {
    var table = $('#tab_test5').DataTable({ language: { "url": "../Scripts/lang/" + ln + ".json" }, "paging": false, "order":[ 1, 'asc' ] });

    $.ajax({
        type: "POST",
        url: 'validaHoja5',
        dataType: "json",
        cache: false,
        contentType: false,
        processData: false,
        async: true,
        success: function (data) {

            if (data !== null || data !== "") {
                if (data.isRedirect) {
                    window.onbeforeunload = false;
                    window.location = data.redirectUrl;
                } else {
                    if (data.length > 1) {
                        var textos = data.pop();
                        var tsol = data.pop();
                        //INICIO DEL CICLO FOR
                        for (var i = 0; i < data.length; i++) {
                            addRowH5(table, data[i], textos[i], tsol[i]);
                        }
                        //FIN DEL FOR

                        $('#tab_test5').css("font-size", "10px");
                        $('#tab_test5').css("display", "table");
                        mostrarAlerta("info", "B", 'Hoja 5 procesada');
                    }
                }
            }
            cargaInicialSpin[4] = true;
            if (jQuery.inArray(false, cargaInicialSpin) === -1) {
                checkRelacionada();
                checkRelacionadaMat();
                clearErrors();
                document.getElementById("loader").style.display = "none";
            }
        },

        complete: function (data) {
            $(document).ready(function () {
                $('#global_filter').removeAttr('disabled');
                var tablaH5 = $('#tab_test5').DataTable();
                $("#excelBtn").removeAttr("disabled");
                
                $("#tab_test5").on("change", ".requiredfile", function () {
                    var data = table.row(this).data();
                    var val = $(this).val();
                    var indexTipo = getTableIndex('#tab_test5', 'lbl_tipo');
                    if ($(this).hasClass("valid") || val !== "") {//ADD RSG 29.10.2018
                        $(this).closest('tr').children().eq(0).children().removeClass("red");
                        $(this).closest('tr').children().eq(0).children().addClass("green");
                        $(this).closest('tr').children().eq(0).children().text("Ok");
                        $(this).closest('tr').children().eq(indexTipo).children().removeClass("red white-text rojo");
                        clearErrors();
                    } else {
                        $(this).closest('tr').children().eq(0).children().removeClass("green");
                        $(this).closest('tr').children().eq(0).children().addClass("red");
                        $(this).closest('tr').children().eq(0).children().text("Error");
                        $(this).closest('tr').children().eq(indexTipo).children().addClass("red white-text rojo");
                        clearErrors();//ADD RSG 29.10.2018
                    }
                });

                $("#tab_test5").on("change", ".outRequiredfile", function () {
                    var val = $(this).val();
                    if ($(this).hasClass("valid") || val !== "") {
                        var id = $(this).closest('tr').children().eq(1).children().val();
                        var tipo = $(this).closest('tr').children().eq(2).children().val();
                        $(this).closest('tr').children().eq(3).children().children().eq(0).children().eq(1).attr('id', id + tipo);
                    } else {
                        $(this).closest('tr').children().eq(3).children().children().eq(0).children().eq(1).removeAttr('id');
                    }
                });
                $("#tablesToexcel").prop("disabled", "false");
                $("#tablesToexcel").removeAttr("disabled");
            });
        },

        error: function (xhr, httpStatusMessage, customErrorMessage) {
            document.getElementById("loader").style.display = "none";
            M.toast({ html: 'La solicitud no pudo ser procesada. Favor de intentarlo mas tarde. La causa ' + xhr.status + " : " + httpStatusMessage + " : " + customErrorMessage });
        }
    });
}

function addRowH5(t, NUM_DOC, TIPO, OBLIGATORIO) {
    var CHECK = null;
    var CLASE = null;
    var claseCell = null;

    if (OBLIGATORIO) {
        claseCell = 'red white-text rojo';
        CHECK = " <div class='red white-text'></div>";
        TIPO = '*' + TIPO;//RMG
        CLASE = 'rojo';
        fileInpt = "<div class='file-field input-field isrequired'><div class='btn-small' style='float: left;'><span>Examinar</span><input type='file' name='" + NUM_DOC + TIPO + "' id='" + NUM_DOC + TIPO + "'></div><div class='file-path-wrapper'><input class='file-path validate requiredfile' type='text'></div></div>";

    }
    else {
        claseCell = '';
        CHECK = "<div class='green white-text'></div>";//RMG
        fileInpt = "<div class='file-field input-field'><div class='btn-small' style='float: left;'><span>Examinar</span><input type='file'></div><div class='file-path-wrapper'><input class='file-path validate outRequiredfile' type='text'></div></div>";

    }

    var r = t.row.add([
        CHECK,
        "<input class='' style='font-size:12px; text-align:center;' type='text' id='' name='' disabled value='" + NUM_DOC + "'><span hidden>" + NUM_DOC + "</span>",
        "<input class='" + claseCell+"' style='font-size:12px; text-align:center;' type='text' id='' name='' value='" + TIPO + "' disabled><span hidden>" + TIPO + "</span>",
        fileInpt, //RMG
        "<input class='NOTAS" + NUM_DOC + TIPO + "' style='font-size:12px; text-align:center;' type='text' id='' name='' value=''><span hidden>NOTAS" + NUM_DOC + TIPO + "</span>",
        //"<input class='' style='font-size:10px; text-align:center;' type='text' id='' name='' value=''>",
        //"<div class='file-field input-field col s8 offset-s3'><div class='btn'><span>Seleccionar archivo</span><input type='file' class='btnF' name='' id='"+NUM_DOC+TIPO+"'></div></div>"
    ]).draw(false).node();

    var td = $(r).children()[0];
    $(td).css('text-align', 'center');

    return r;
}

$("#tab_arc").click(function () {
    var tbody = $("#tab_test5 tbody");
    var arregloNumDoc = [];

    if (tbody.children().length !== 0) {
        arregloNumDoc = checkRelacionada();

        if (arregloNumDoc.length > 0) {
            var tablaH3 = $('#tab_test3').DataTable();

            for (var a = 0; a < tablaH3.rows().data().length; a++) {
                var rowH3 = tablaH3.row(a).node();
                var num_docH3 = $(rowH3).children().eq(1).children().val();

                if (jQuery.inArray(num_docH3, arregloNumDoc) !== -1) {
                    $(rowH3).children().eq(1).children().addClass("red white-text rojo");
                }
            }
        }
        checkRelacionadaMat();
    }
});

/////////////////////////////////////////////FUNCIONES GENERALES////////////////////////////
function checkRelMul() {
    var tablaH1 = $('#tab_test1').DataTable();
    var tablaH2 = $('#tab_test2').DataTable();
    var tablaH3 = $('#tab_test3').DataTable();

    for (var a = 0; a < tablaH1.rows().data().length; a++) {
        var rowH1 = tablaH1.row(a).node();
        var num_docH1 = $(rowH1).children().eq(1).children().val();
        var banderaH2 = false, banderaH3 = false;

        for (var b = 0; b < tablaH2.rows().data().length; b++) {
            var rowH2 = tablaH2.row(b).node();
            var num_docH2 = $(rowH2).children().eq(1).children().val();

            if (num_docH1 === num_docH2) {
                banderaH2 = true;
                break;
            }
        }

        for (var c = 0; c < tablaH3.rows().data().length; c++) {
            var rowH3 = tablaH3.row(c).node();
            var num_docH3 = $(rowH3).children().eq(1).children().val();

            if (num_docH1 === num_docH3) {
                banderaH3 = true;
                break;
            }
        }

        if (!banderaH2 & !banderaH3) {
            $(rowH1).children().eq(1).children().addClass("red white-text rojo");
        }
    }
}

function checkRelacionada() {
    var tablaH2 = $('#tab_test2').DataTable();
    var tablaH3 = $('#tab_test3').DataTable();
    var arregloNumDoc = [];
    var contador = 0;

    for (var a = 0; a < tablaH2.rows().data().length; a++) {
        var rowH2 = tablaH2.row(a).node();
        var num_docH2 = $(rowH2).children().eq(1).children().val();

        for (var b = 0; b < tablaH3.rows().data().length; b++) {
            var rowH3 = tablaH3.row(b).node();
            var num_docH3 = $(rowH3).children().eq(1).children().val();

            if (num_docH2 === num_docH3) {
                if (jQuery.inArray(num_docH2, arregloNumDoc) === -1) {
                    arregloNumDoc[contador] = num_docH2;
                    contador++;
                }
            }
        }
    }

    if (arregloNumDoc.length > 0) {
        for (var c = 0; c < tablaH3.rows().data().length; c++) {
            var rowH33 = tablaH3.row(c).node();
            var num_docH33 = $(rowH33).children().eq(1).children().val();

            if (jQuery.inArray(num_docH33, arregloNumDoc) !== -1) {
                $(rowH33).children().eq(1).children().addClass("red white-text rojo");
                $(rowH33).children().eq(1).children().prop('title', 'El documento ya tiene facturas asignadas');
            }
        }
    }
    return arregloNumDoc;
}

function checkRelacionadaMat() {
    var tablaH1 = $('#tab_test1').DataTable();
    var tablaH3 = $('#tab_test3').DataTable();
    var tablaH4 = $('#tab_test4').DataTable();
    var pais, miles, decimales = null;
    var cantidadesH3 = [], cantidadesH3Num = [], cantidadesUniH3 = [];
    var contadorH1 = 0, contadorH3 = 0, cantidad = 0, cantidadRel = 0;


    ///////////////////////////////////////////PROCEDIMIENTO PARA SACAR LAS CANTIDADES DE MULTIPLES HOJA 3////////////////////////////////////////////////
    for (var a = 0; a < tablaH3.rows().data().length; a++) {
        var rowH3 = tablaH3.row(a).node();
        var num_docH3 = $(rowH3).children().eq(1).children().val();
        cantidad = $(rowH3).children().eq(7).children().val();
        cantidadRel = cantidad;

        for (var b = 0; b < tablaH1.rows().data().length; b++) {
            var rowH1 = tablaH1.row(b).node();
            var num_docH1 = $(rowH1).children().eq(1).children().val();

            if (num_docH1 === num_docH3) {
                pais = $(rowH1).find('td:eq(5)').children().val();
            }
        }

        cantidad = cantidad.substr(cantidad.length - 3);
        decimales = cantidad.charAt(0);

        if (decimales === ".") {
            miles = ",";
        }
        else if (decimales === ",") {
            miles = ".";
        }

        cantidadRel = toNum(cantidadRel, miles, decimales);

        if (jQuery.inArray(num_docH3, cantidadesH3) === -1) {
            cantidadesH3[contadorH3] = cantidadRel;
            cantidadesH3Num[contadorH3] = num_docH3;
            contadorH3++;
        }
    }

    for (var c = 0; c < tablaH1.rows().data().length; c++) {
        var rowH11 = tablaH1.row(c).node();
        var num_docH11 = $(rowH11).children().eq(1).children().val();
        var sumH3 = 0;

        for (var d = 0; d < cantidadesH3.length; d++) {
            if (num_docH11 === cantidadesH3Num[d]) {
                sumH3 += cantidadesH3[d];
            }
        }

        if (jQuery.inArray(num_docH11, cantidadesH3Num) !== -1) {
            cantidadesUniH3[contadorH1] = num_docH11 + '-' + sumH3;
            contadorH1++;
        }
    }
    /////////////////////////////////////////////////////////////////////////FIN//////////////////////////////////////////////////////////////////////////
    pais, miles, decimales = null;
    var contadorH4 = 0;
    contadorH1 = 0, cantidad = 0, cantidadRel = 0;
    var cantidadesH4 = [], cantidadesH4Num = [], cantidadesUniH4 = [];

    ///////////////////////////////////////////PROCEDIMIENTO PARA SACAR LAS CANTIDADES DE MATERIALES HOJA 4////////////////////////////////////////////////

    for (var e = 0; e < tablaH4.rows().data().length; e++) {
        var rowH4 = tablaH4.row(e).node();
        var num_docH4 = $(rowH4).children().eq(1).children().val();
        cantidad = $(rowH4).children().eq(14).children().val();
        cantidadRel = cantidad;

        for (var f = 0; f < tablaH1.rows().data().length; f++) {
            var rowH111 = tablaH1.row(f).node();
            var num_docH111 = $(rowH111).children().eq(1).children().val();

            if (num_docH111 === num_docH4) {
                pais = $(rowH111).find('td:eq(5)').children().val();
            }
        }

        cantidad = cantidad.substr(cantidad.length - 3);
        decimales = cantidad.charAt(0);

        if (decimales === ".") {
            miles = ",";
        }
        else if (decimales === ",") {
            miles = ".";
        }

        cantidadRel = toNum(cantidadRel, miles, decimales);

        if (jQuery.inArray(num_docH4, cantidadesH4) === -1) {
            cantidadesH4[contadorH4] = cantidadRel;
            cantidadesH4Num[contadorH4] = num_docH4;
            contadorH4++;
        }
    }

    for (var g = 0; g < tablaH1.rows().data().length; g++) {
        var rowH1111 = tablaH1.row(g).node();
        var num_docH1111 = $(rowH1111).children().eq(1).children().val();
        var sumH4 = 0;

        for (var h = 0; h < cantidadesH4.length; h++) {
            if (num_docH1111 === cantidadesH4Num[h]) {
                sumH4 += cantidadesH4[h];
            }
        }

        if (jQuery.inArray(num_docH1111, cantidadesH4Num) !== -1) {
            cantidadesUniH4[contadorH1] = num_docH1111 + '-' + sumH4;
            contadorH1++;
        }
    }
    /////////////////////////////////////////////////////////////////////////FIN/////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////PROCEDIMIENTO PARA SACAR LOS ID CON ERROR DE LAS CANTIDADES QUE NO CUADRAN////////////////////////////////////////////////
    var cantidadesError = [], idError = [];
    var contadorErr = 0;

    for (var i = 0; i < cantidadesUniH3.length; i++) {
        if (jQuery.inArray(cantidadesUniH3[i], cantidadesUniH4) === -1) {
            cantidadesError[contadorErr] = cantidadesUniH3[i];
            contadorErr++;
        }
    }

    for (j = 0; j < cantidadesError.length; j++) {
        //idError[j] = cantidadesError[j].charAt(0);
        index = cantidadesError[j].indexOf('-');
        idError[j] = cantidadesError[j].substr(0, index);
    }
    /////////////////////////////////////////////////////////////////////////FIN/////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////PINTAMOS DE COLOR ROJO LOS MONTOS DE APOYO EN MATERIALES POR NO CUADRAR EN CANTIDADES///////////////////////////////////////////
    for (var k = 0; k < tablaH4.rows().data().length; k++) {
        var rowH44 = tablaH4.row(k).node();
        var num_docH44 = $(rowH44).children().eq(1).children().val();
        var tieneRojo = $(rowH44).children().eq(14).children().hasClass("red white-text rojo");

        for (var l = 0; l < idError.length; l++) {
            if (num_docH44 === idError[l]) {
                $(rowH44).children().eq(1).children().addClass("red white-text rojo errorCantidades");
                $(rowH44).children().eq(1).children().prop('title', 'El total de distribución es diferente a el total de facturas multiples.');
                //if (tieneRojo) {
                //    $(rowH44).children().eq(14).children().addClass("errorCantidades");
                //}
                //else {
                //    $(rowH44).children().eq(14).children().addClass("red white-text rojo errorCantidades");
                //}
            }
        }
    }
    /////////////////////////////////////////////////////////////////////////FIN/////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////DESPINTAMOS DE COLOR ROJO LOS MONTOS DE APOYO EN MATERIALES QUE CUADRAR EN CANTIDADES///////////////////////////////////////////
    var correctos = [];
    var contadorCorrectos = 0;

    for (var m = 0; m < tablaH1.rows().data().length; m++) {
        var rowH1C = tablaH1.row(m).node();
        var num_docH1C = $(rowH1C).children().eq(1).children().val();

        if (jQuery.inArray(num_docH1C, idError) === -1) {
            correctos[contadorCorrectos] = num_docH1C;
            contadorCorrectos++;
        }
    }

    for (var n = 0; n < tablaH4.rows().data().length; n++) {
        var rowH4C = tablaH4.row(n).node();
        var num_docH4C = $(rowH4C).children().eq(1).children().val();
        //var tieneRojo = $(rowH4C).children().eq(14).children().hasClass("red white-text rojo");

        for (var o = 0; o < correctos.length; o++) {
            if (num_docH4C === correctos[o]) {
                $(rowH4C).children().eq(1).children().removeClass("red white-text rojo errorCantidades");
            }
        }
    }

    return correctos;
}

function obtenDecimalMil(pais, cantidad) {
    var mil, decimal, total = null;

    $.ajax({
        type: "POST",
        url: 'getDecimalMil',
        dataType: "json",
        data: { "Pais": pais },
        success: function (data) {
            if (data !== null | data !== "") {
                decimal = data.pop();
                mil = data;
            }
            total = toNum(cantidad, mil, decimal);
        },
        async: false
    });

    return total;
}

function erroresH1() {
    var tablaH1 = $('#tab_test1').DataTable();
    var banderaH1 = false;
    var tabla1 = [];

    for (var a = 0; a < tablaH1.rows().data().length; a++) {
        var rowH1 = tablaH1.row(a).node();
        var num_docH1 = $(rowH1).children().eq(1).children().val();
        var statusH1 = $(rowH1).children().children().hasClass('rojo');
        var tsol = $(rowH1).children().eq(2).children().hasClass('rojo');
        var sociedad = $(rowH1).children().eq(4).children().hasClass('rojo');
        var pais = $(rowH1).children().eq(5).children().hasClass('rojo');

        if (!$.isNumeric(num_docH1) | tsol | sociedad | pais) {
            tabla1[a] = num_docH1;
            banderaH1 = true;
        }
        else {
            banderaH1 = false;
        }
    }

    ////console.log(tabla1);
    return tabla1;
}

function trimStart(character, string) {//RSG 07.06.2018
    var startIndex = 0;

    while (string[startIndex] === character) {
        startIndex++;
    }

    return string.substr(startIndex);
}

function clearErrors() {
    var tablaH1 = $('#tab_test1').DataTable();
    var tablaH2 = $('#tab_test2').DataTable();
    var tablaH3 = $('#tab_test3').DataTable();
    var tablaH4 = $('#tab_test4').DataTable();
    //var tablaH5 = $('#tab_test5').DataTable();//DELETE RSG 29.10.2018
    var erroresTab1 ,erroresTab2,erroresTab3,erroresTab4, erroresTab5 = false;



    var tabla1 = [], tabla2 = [], tabla3 = [], tabla4 = [], tabla5 = [], archivos = [];

    for (var a = 0; a < tablaH1.rows().data().length; a++) {
        var rowH1 = tablaH1.row(a).node();
        var num_docH1 = $(rowH1).children().eq(1).children().val();
        var statusH1 = $(rowH1).children().children().hasClass('rojo');

        tabla1[a] = num_docH1 + statusH1;
    }

    for (var b = 0; b < tablaH2.rows().data().length; b++) {
        var rowH2 = tablaH2.row(b).node();
        var num_docH2 = $(rowH2).children().eq(1).children().val();
        var statusH2 = $(rowH2).children().children().hasClass('rojo');

        tabla2[b] = num_docH2 + statusH2;
    }

    for (var c = 0; c < tablaH3.rows().data().length; c++) {
        var rowH3 = tablaH3.row(c).node();
        var num_docH3 = $(rowH3).children().eq(1).children().val();
        var statusH3 = $(rowH3).children().children().hasClass('rojo');

        tabla3[c] = num_docH3 + statusH3;
    }

    for (var d = 0; d < tablaH4.rows().data().length; d++) {
        var rowH4 = tablaH4.row(d).node();
        var num_docH4 = $(rowH4).children().eq(1).children().val();
        var statusH4 = $(rowH4).children().children().hasClass('rojo');

        tabla4[d] = num_docH4 + statusH4;
    }

    //for (var e = 0; e < tablaH5.rows().data().length; e++) {//DELETE RSG 29.10.2018
    //    var rowH5 = tablaH5.row(e).node();
    //    var num_docH5 = $(rowH5).children().eq(1).children().val();
    //    var statusH5 = $(rowH5).children().children().hasClass('rojo');

    //    tabla5[e] = num_docH5 + statusH5;
    //}

    var banderaH1, banderaH2, banderaH3, banderaH4, banderaH5 = false;

    for (var aa = 0; aa < tablaH1.rows().data().length; aa++) {
        var rowH11 = tablaH1.row(aa).node();
        var num_docH11 = $(rowH11).children().eq(1).children().val();
        clearErrorsN(num_docH11);//ADD RSG 29.10.2018

        var tablaH5 = $('#tab_test5').DataTable();                      //ADD RSG 29.10.2018
        for (var e = 0; e < tablaH5.rows().data().length; e++) {        //ADD RSG 29.10.2018
            var rowH5 = tablaH5.row(e).node();
            var num_docH5 = $(rowH5).children().eq(1).children().val();
            var statusH5 = $(rowH5).children().children().hasClass('rojo');

            tabla5[e] = num_docH5 + statusH5;
        }

        //SI TIENE TRUE TIENE ERROR
        if (jQuery.inArray(num_docH11 + true, tabla1) !== -1) {
            banderaH1 = true;
            erroresTab1 = true;
        }
        else {
            banderaH1 = false;
        }

        for (var bb = 0; bb < tablaH2.rows().data().length; bb++) {
            var rowH22 = tablaH2.row(bb).node();
            var num_docH22 = $(rowH22).children().eq(1).children().val();

            if (num_docH11 === num_docH22) {
                //SI TIENE TRUE TIENE ERROR
                if (jQuery.inArray(num_docH22 + true, tabla2) !== -1) {
                    banderaH2 = true;
                    erroresTab2 = true;
                    break;
                }
                else {
                    banderaH2 = false;
                }
            }
            else {
                banderaH2 = false;
            }
        }

        for (var cc = 0; cc < tablaH3.rows().data().length; cc++) {
            var rowH33 = tablaH3.row(cc).node();
            var num_docH33 = $(rowH33).children().eq(1).children().val();

            if (num_docH11 === num_docH33) {
                //SI TIENE TRUE TIENE ERROR
                if (jQuery.inArray(num_docH33 + true, tabla3) !== -1) {
                    banderaH3 = true;
                    erroresTab3 = true;
                    break;
                }
                else {
                    banderaH3 = false;
                }
            }
            else {
                banderaH3 = false;
            }
        }

        for (var dd = 0; dd < tablaH4.rows().data().length; dd++) {
            var rowH44 = tablaH4.row(dd).node();
            var num_docH44 = $(rowH44).children().eq(1).children().val();

            if (num_docH11 === num_docH44) {
                //SI TIENE TRUE TIENE ERROR
                if (jQuery.inArray(num_docH44 + true, tabla4) !== -1) {
                    banderaH4 = true;
                    erroresTab4 = true;
                    break;
                }
                else {
                    banderaH4 = false;
                }
            }
            else {
                banderaH4 = false;
            }
        }

        for (var ee = 0; ee < tablaH5.rows().data().length; ee++) {
            var rowH55 = tablaH5.row(ee).node();
            var num_docH55 = $(rowH55).children().eq(1).children().val();

            if (num_docH11 === num_docH55) {
                //SI TIENE TRUE TIENE ERROR
                if (jQuery.inArray(num_docH55 + true, tabla5) !== -1) {
                    banderaH5 = true;
                    erroresTab5 = true;
                    break;
                }
                else {
                    banderaH5 = false;
                }
            }
            else {
                banderaH5 = false;
            }
        }

        erroresTab1 ? $('#tab_info').addClass("red white-text rojo") : $('#tab_info').removeClass("red white-text rojo");
        erroresTab2 ? $('#tab_rel').addClass("red white-text rojo") : $('#tab_rel').removeClass("red white-text rojo");
        erroresTab3 ? $('#tab_mul').addClass("red white-text rojo") : $('#tab_mul').removeClass("red white-text rojo");
        erroresTab4 ? $('#tab_dis').addClass("red white-text rojo") : $('#tab_dis').removeClass("red white-text rojo");
        erroresTab5 ? $('#tab_arc').addClass("red white-text rojo") : $('#tab_arc').removeClass("red white-text rojo");
        //console.log(banderaH1, banderaH2, banderaH3, banderaH4, banderaH5, num_docH11);
        //console.log(validaErrores(banderaH1, banderaH2, banderaH3, banderaH4, banderaH5, num_docH11));
        validaErrores(banderaH1, banderaH2, banderaH3, banderaH4, banderaH5, num_docH11);
    }
}

function validaErrores(h1, h2, h3, h4, h5, num_doc) {
    var tablaH1 = $('#tab_test1').DataTable();
    var tablaH2 = $('#tab_test2').DataTable();
    var tablaH3 = $('#tab_test3').DataTable();
    var tablaH4 = $('#tab_test4').DataTable();

    if (!h1 && !h2 && !h3 && !h4 && !h5) {

        for (var a = 0; a < tablaH1.rows().data().length; a++) {
            var rowH1 = tablaH1.row(a).node();
            var num_docH1 = $(rowH1).children().eq(1).children().val();

            if (num_docH1 === num_doc) {
                $(rowH1).children().eq(0).children().removeClass("red");
                $(rowH1).children().eq(0).children().addClass("green");
                $(rowH1).children().eq(0).children().text("Ok");
            }
        }

        for (var b = 0; b < tablaH2.rows().data().length; b++) {
            var rowH2 = tablaH2.row(b).node();
            var num_docH2 = $(rowH2).children().eq(1).children().val();

            if (num_docH2 === num_doc) {
                $(rowH2).children().eq(0).children().removeClass("red");
                $(rowH2).children().eq(0).children().addClass("green");
                $(rowH2).children().eq(0).children().text("Ok");
            }
        }

        for (var c = 0; c < tablaH3.rows().data().length; c++) {
            var rowH3 = tablaH3.row(c).node();
            var num_docH3 = $(rowH3).children().eq(1).children().val();

            if (num_docH3 === num_doc) {
                $(rowH3).children().eq(0).children().removeClass("red");
                $(rowH3).children().eq(0).children().addClass("green");
                $(rowH3).children().eq(0).children().text("Ok");
            }
        }

        for (var d = 0; d < tablaH4.rows().data().length; d++) {
            var rowH4 = tablaH4.row(d).node();
            var num_docH4 = $(rowH4).children().eq(1).children().val();

            if (num_docH4 === num_doc) {
                $(rowH4).children().eq(0).children().removeClass("red");
                $(rowH4).children().eq(0).children().addClass("green");
                $(rowH4).children().eq(0).children().text("Ok");
            }
        }
        return "SE MODIFICARON LOS REGISTROS: " + num_doc;
    }
    else {
        for (var aa = 0; aa < tablaH1.rows().data().length; aa++) {
            var rowH11 = tablaH1.row(aa).node();
            var num_docH11 = $(rowH11).children().eq(1).children().val();

            if (num_docH11 === num_doc) {
                $(rowH11).children().eq(0).children().removeClass("green");
                $(rowH11).children().eq(0).children().addClass("red");
                $(rowH11).children().eq(0).children().text("Error");
            }
        }

        for (var bb = 0; bb < tablaH2.rows().data().length; bb++) {
            var rowH22 = tablaH2.row(bb).node();
            var num_docH22 = $(rowH22).children().eq(1).children().val();

            if (num_docH22 === num_doc) {
                $(rowH22).children().eq(0).children().removeClass("green");
                $(rowH22).children().eq(0).children().addClass("red");
                $(rowH22).children().eq(0).children().text("Error");
            }
        }

        for (var cc = 0; cc < tablaH3.rows().data().length; cc++) {
            var rowH33 = tablaH3.row(cc).node();
            var num_docH33 = $(rowH33).children().eq(1).children().val();

            if (num_docH33 === num_doc) {
                $(rowH33).children().eq(0).children().removeClass("green");
                $(rowH33).children().eq(0).children().addClass("red");
                $(rowH33).children().eq(0).children().text("Error");
            }
        }

        for (var dd = 0; dd < tablaH4.rows().data().length; dd++) {
            var rowH44 = tablaH4.row(dd).node();
            var num_docH44 = $(rowH44).children().eq(1).children().val();

            if (num_docH44 === num_doc) {
                $(rowH44).children().eq(0).children().removeClass("green");
                $(rowH44).children().eq(0).children().addClass("red");
                $(rowH44).children().eq(0).children().text("Error");
            }
        }

        //for (var ee = 0; ee < tablaH5.rows().data().length; ee++) {
        //    var rowH55 = tablaH5.row(ee).node();
        //    var num_docH55 = $(rowH55).children().eq(1).children().val();
        //    var esRequerido2 = $(rowH55).children().eq(3).children().hasClass("isrequired");

        //    if (num_docH55 == num_doc & esRequerido2) {
        //        //console.log($(rowH55).children().eq(0).children().attr("class"));
        //        $(rowH55).children().eq(0).children().removeClass("green");
        //        $(rowH55).children().eq(0).children().addClass("red");
        //        $(rowH55).children().eq(0).children().text("close");
        //    }
        //}
    }
    return "WRANGGGGGGGGGGGGGGGGGGGG ERROR";
}

function guardaDatos() {
    document.getElementById("loader").style.display = "initial";

    var tablaH1 = $('#tab_test1').DataTable();
    var tablaH2 = $('#tab_test2').DataTable();
    var tablaH3 = $('#tab_test3').DataTable();
    var tablaH4 = $('#tab_test4').DataTable();
    var tablaH5 = $('#tab_test5').DataTable();

    var tabla1 = [], tabla2 = [], tabla3 = [], tabla4 = [], tabla5 = [], notasArr = [];
    var rowsH1 = [], rowsH2 = [], rowsH3 = [], rowsH4 = [], rowsH5 = [], rowsArc = [];
    var a = 0, b = 0, c = 0, d = 0, e = 0;
    var formData = new FormData();

    for (var aa = 0; aa < tablaH1.rows().data().length; aa++) {
        var rowH1 = tablaH1.row(aa).node();
        var status = $(rowH1).children().eq(0).children().text();

        if (status === "Ok") {
            var num_doc = $(rowH1).children().eq(1).children().val();
            var t_sol = $(rowH1).children().eq(2).children().val();
            var tall_id = $(rowH1).children().eq(3).children('span').text();
            var bukrs = $(rowH1).children().eq(4).children().val();
            var land = $(rowH1).children().eq(5).children('span').text();
            var estado = $(rowH1).children().eq(6).children().val();
            var ciudad = $(rowH1).children().eq(7).children().val();
            var concepto = $(rowH1).children().eq(8).children().val();
            var notas = $(rowH1).children().eq(9).children().val();
            var payer_id = $(rowH1).children().eq(10).children().val();
            var payer_nombre = $(rowH1).children().eq(11).children().val();
            var contacto_nombre = $(rowH1).children().eq(12).children().val();
            var contacto_email = $(rowH1).children().eq(13).children().val();
            var fechai_vig = $(rowH1).children().eq(14).children().val();
            var fechaf_vig = $(rowH1).children().eq(15).children().val();
            var moneda_id = $(rowH1).children().eq(16).children().val();

            rowsH1[a] = [num_doc, t_sol, tall_id, bukrs, land, estado, ciudad, concepto, notas, payer_id, payer_nombre, contacto_nombre, contacto_email, fechai_vig, fechaf_vig, moneda_id];
            a++;
        }
    }

    for (var bb = 0; bb < tablaH2.rows().data().length; bb++) {
        var rowH2 = tablaH2.row(bb).node();
        var statusH2 = $(rowH2).children().eq(0).children().text();

        if (statusH2 === "Ok") {
            var num_docH2 = $(rowH2).children().eq(1).children().val();
            var facturaH2 = $(rowH2).children().eq(2).children().val();
            var fecha_facturaH2 = $(rowH2).children().eq(3).children().val();
            var proveedorH2 = $(rowH2).children().eq(4).children().val();
            var proveedor_nombreH2 = $(rowH2).children().eq(5).children().val();
            var autorizacionH2 = $(rowH2).children().eq(6).children().val();
            var vencimientoH2 = $(rowH2).children().eq(7).children().val();
            var facturakH2 = $(rowH2).children().eq(8).children().val();
            var ejerciciokH2 = $(rowH2).children().eq(9).children().val();

            rowsH2[b] = [num_docH2, facturaH2, fecha_facturaH2, proveedorH2, proveedor_nombreH2, autorizacionH2, vencimientoH2, facturakH2, ejerciciokH2];
            b++;
        }
    }

    for (var cc = 0; cc < tablaH3.rows().data().length; cc++) {
        var rowH3 = tablaH3.row(cc).node();
        var statusH3 = $(rowH3).children().eq(0).children().text();

        if (statusH3 === "Ok") {
            var num_docH3 = $(rowH3).children().eq(1).children().val();
            var facturaH3 = $(rowH3).children().eq(2).children().val();
            var bill_docH3 = $(rowH3).children().eq(3).children().val();
            var ejerciciokH3 = $(rowH3).children().eq(4).children().val();
            var payer_idH3 = $(rowH3).children().eq(5).children().val();
            var payer_nombreH3 = $(rowH3).children().eq(6).children().val();
            var importe_facH3 = $(rowH3).children().eq(7).children().val();
            var belnrH3 = $(rowH3).children().eq(8).children().val();

            rowsH3[c] = [num_docH3, facturaH3, bill_docH3, ejerciciokH3, payer_idH3, payer_nombreH3, importe_facH3, belnrH3];
            c++;
        }
    }

    for (var dd = 0; dd < tablaH4.rows().data().length; dd++) {
        var rowH4 = tablaH4.row(dd).node();
        var statusH4 = $(rowH4).children().eq(0).children().text();

        if (statusH4 === "Ok") {
            var num_docH4 = $(rowH4).children().eq(1).children().val();
            var checkH4 = $(rowH4).children().eq(2).children().children().children().is(':checked');
            var vigencia_deH4 = $(rowH4).children().eq(3).children().val();
            var vigencia_alH4 = $(rowH4).children().eq(4).children().val();
            var matnrH4 = $(rowH4).children().eq(5).children().val();
            var matklH4 = $(rowH4).children().eq(6).children('span').text();
            var descripcionH4 = $(rowH4).children().eq(7).children().val();
            var montoH4 = $(rowH4).children().eq(8).children().val();
            var porc_apoyoH4 = $(rowH4).children().eq(9).children().val();
            var apoyo_piezaH4 = $(rowH4).children().eq(10).children().val();
            var costo_apoyoH4 = $(rowH4).children().eq(11).children().val();
            var precio_sugH4 = $(rowH4).children().eq(12).children().val();
            var volumen_realH4 = $(rowH4).children().eq(13).children().val();
            var apoyoH4 = $(rowH4).children().eq(14).children().val();

            rowsH4[d] = [num_docH4, checkH4, vigencia_deH4, vigencia_alH4, matnrH4, matklH4, descripcionH4, montoH4, porc_apoyoH4, apoyo_piezaH4, costo_apoyoH4, precio_sugH4, volumen_realH4, apoyoH4];
            d++;
        }
    }

    for (var ee = 0; ee < tablaH5.rows().data().length; ee++) {
        var rowH5 = tablaH5.row(ee).node();
        var statusH5 = $(rowH5).children().eq(0).children().text();

        if (statusH5 === "Ok") {
            var num_docH5 = $(rowH5).children().eq(1).children().val();
            var descripcionH5 = $(rowH5).children().eq(2).children().val();
            var idArchivoH5 = $(rowH5).children().eq(3).children().children().eq(0).children().eq(1).attr('id');


            if (idArchivoH5 === (num_docH5 + descripcionH5)) {
                var archivo = document.getElementById(num_docH5 + descripcionH5).files[0];

                if (idArchivoH5.indexOf("*")>-1) {
                    rowsH5[e] = [num_docH5 + descripcionH5 + "*" + archivo.name + archivo.size];
                    rowsArc[e] = archivo;
                    formData.append("archivos[]", archivo);
                    e++;
                }
                else {
                    rowsH5[e] = [num_docH5 + "*" + descripcionH5 + "*" + archivo.name + archivo.size];
                    rowsArc[e] = archivo;
                    formData.append("archivos[]", archivo);
                    e++;
                }
            }
        }
    }

    var contadorNota = 0;
    for (var f = 0; f < tablaH1.rows().data().length; f++) {
        var rowH1 = tablaH1.row(f).node();
        var num_docH1 = $(rowH1).children().eq(1).children().val();
        var notaFinal = "";

        for (var g = 0; g < tablaH5.rows().data().length; g++) {
            var rowH5 = tablaH5.row(g).node();
            var statusH5 = $(rowH5).children().eq(0).children().text();

            if (statusH5 === "Ok") {
                var num_docH5 = $(rowH5).children().eq(1).children().val();
                var nota = $(rowH5).children().eq(4).children().val();


                if (num_docH1 === num_docH5) {
                    if (notaFinal === "") {
                        notaFinal = notaFinal + nota;
                    }
                    else if (nota !== "") {
                        notaFinal = notaFinal + "," + nota;
                    }
                }
            }
        }

        if (notaFinal !== "") {
            notasArr[contadorNota] = num_docH1 + "*" + notaFinal;
            contadorNota++;
        }
    }


    tabla1 = rowsH1;
    tabla2 = rowsH2;
    tabla3 = rowsH3;
    tabla4 = rowsH4;
    tabla5 = rowsH5;

    $.ajax({
        type: "POST",
        url: 'setArchivos',
        dataType: "json",
        processData: false,
        cache: false,
        contentType: false,
        data: formData,
        success: function (data) {
            if (data !== null || data !== "") {
                if (data.isRedirect) {
                    window.onbeforeunload = false;
                    window.location = data.redirectUrl;
                } else {
                    setDatos(tabla1, tabla2, tabla3, tabla4, tabla5, notasArr);
                }
            }
        },
        error: function (xhr, status, p3, p4) {
            var err = "Error " + " " + status + " " + p3 + " " + p4;
            if (xhr.responseText && xhr.responseText[0] === "{")
                err = JSON.parse(xhr.responseText).Message;
            document.getElementById("loader").style.display = "none";
            //console.log(err);
        }
    });

   
}
function setDatos(tabla1, tabla2, tabla3, tabla4, tabla5, notasArr) {
    $.ajax({
        type: "POST",
        url: 'setDatos',
        dataType: "json",
        data: { "h1": tabla1, "h2": tabla2, "h3": tabla3, "h4": tabla4, "h5": tabla5, "notas": notasArr },
        async: true,
        success: function (data) {
            if (data !== null | data !== "") {
                if (data.isRedirect) {
                    window.onbeforeunload = false;
                    window.location = data.redirectUrl;
                } else {
                    var eliminarId = [];
                    var listaIds = [];
                    var listIdsRechazados = [];
                    listaIds = data.pop();
                    //eliminarId = data;
                    listaIds.forEach(function (element) {
                        if ((element.indexOf('<') >= 0)) {
                            var DocId = element.split('<')[1];
                            listIdsRechazados.push({ idDoc: DocId, msj: "No contiene materiales." });
                        } else if ((element.indexOf('periodoCerrado') >= 0)) {
                            var DocIdPeriodo = element.split('periodoCerrado')[1];
                            listIdsRechazados.push({ idDoc: DocIdPeriodo, msj: "No existe periodo abierto" });
                        }
                    });

                    data.forEach(function (numDoc) {
                        var existe = false;
                        numDoc = numDoc.toString();
                        listIdsRechazados.forEach(function (Doc) {
                            if (Doc.idDoc === numDoc) {
                                existe = true;
                            }
                        });
                        if (!existe)
                            eliminarId.push(numDoc);
                    });

                    var tablaH1 = $('#tab_test1').DataTable();
                    var tablaH2 = $('#tab_test2').DataTable();
                    var tablaH3 = $('#tab_test3').DataTable();
                    var tablaH4 = $('#tab_test4').DataTable();
                    var tablaH5 = $('#tab_test5').DataTable();

                    for (var a = 0; a < tablaH1.rows().data().length; a++) {
                        var rowH1 = tablaH1.row(a).node();
                        var rowH11 = tablaH1.row(a);
                        var num_docH1 = $(rowH1).children().eq(1).children().val();

                        for (var b = 0; b < eliminarId.length; b++) {
                            num_doc = eliminarId[b];

                            if (num_doc === num_docH1) {
                                rowH11.remove().draw();
                                a--;
                            }
                        }

                        listIdsRechazados.forEach(function (Doc) {
                            if (Doc.idDoc === num_docH1) {
                                $(rowH1).children().eq(1).children().addClass("yellow white-text");
                                $(rowH1).children().eq(1).children().prop('title', Doc.msj);

                            }
                        });
                    }

                    for (var c = 0; c < tablaH2.rows().data().length; c++) {
                        var rowH2 = tablaH2.row(c).node();
                        var rowH22 = tablaH2.row(c);
                        var num_docH2 = $(rowH2).children().eq(1).children().val();

                        for (var d = 0; d < eliminarId.length; d++) {
                            num_doc = eliminarId[d];

                            if (num_doc === num_docH2) {
                                rowH22.remove().draw();
                                c--;
                            }
                        }
                    }

                    for (var e = 0; e < tablaH3.rows().data().length; e++) {
                        var rowH3 = tablaH3.row(e).node();
                        var rowH33 = tablaH3.row(e);
                        var num_docH3 = $(rowH3).children().eq(1).children().val();

                        for (var f = 0; f < eliminarId.length; f++) {
                            num_doc = eliminarId[f];

                            if (num_doc === num_docH3) {
                                rowH33.remove().draw();
                                e--;
                            }
                        }
                    }

                    for (var g = 0; g < tablaH4.rows().data().length; g++) {
                        var rowH4 = tablaH4.row(g).node();
                        var rowH44 = tablaH4.row(g);
                        var num_docH4 = $(rowH4).children().eq(1).children().val();

                        for (var h = 0; h < eliminarId.length; h++) {
                            num_doc = eliminarId[h];

                            if (num_doc === num_docH4) {
                                rowH44.remove().draw();
                                g--;
                            }
                        }
                    }

                    for (var i = 0; i < tablaH5.rows().data().length; i++) {
                        var rowH5 = tablaH5.row(i).node();
                        var rowH55 = tablaH5.row(i);
                        var num_docH5 = $(rowH5).children().eq(1).children().val();

                        for (var j = 0; j < eliminarId.length; j++) {
                            num_doc = eliminarId[j];

                            if (num_doc === num_docH5) {
                                rowH55.remove().draw();
                                i--;
                            }
                        }
                    }

                    document.getElementById("loader").style.display = "none";

                    for (var k = 0; k < listaIds.length; k++) {
                        if (listaIds[k].indexOf('<') >= 0) {
                            var texto = listaIds[k].split('<');
                            M.toast({ html: 'Documento ' + texto[1] + ' no fue creado, porque no contiene materiales.' });

                        } else if (listaIds[k].indexOf('periodoCerrado') >= 0) {
                            var msjTexto = listaIds[k].split('periodoCerrado');
                            M.toast({ html: 'Documento ' + msjTexto[1] + ' no fue creado, porque no existe periodo abierto.' });

                        }
                        else {
                            M.toast({ html: 'Documento ' + listaIds[k] + ' fue creado' });
                        }
                    }

                    //var table = $('#tab_test1').DataTable();

                    //if (!table.data().any()) {
                    //    location.reload();
                    //}
                }
            }
        },

        error: function (xhr, httpStatusMessage, customErrorMessage) {
            document.getElementById("loader").style.display = "none";
            M.toast({ html: 'Error al guardar archivo ' + xhr.status + " : " + httpStatusMessage + " : " + customErrorMessage });
        }
    });
}
function cloneTables() {
    var tablaH1c = $('#tab_test1').DataTable();
    var tablaH2c = $('#tab_test2').DataTable();
    var tablaH3c = $('#tab_test3').DataTable();
    var tablaH4c = $('#tab_test4').DataTable();

    ////TAB1Data////
    $('#tab_test1').append("<table id='tab_test1clond' style='display:none'><thead id='tabclon1hd'></thead><tbody id='tabclon1bd'></tbody></table>");
    $('#tabclon1hd').append("<tr id='titles1d'></tr>");

    $('#tab_test1 > thead > tr > th').each(function () {
        var id = $(this).prop('id');
        if (id !== "lbl_estatusH1" && id !== "lbl_vkorg" && id !== "lbl_vtweg" && id !== "lbl_spart" && id !== "lbl_miles" && id !== "lbl_decimales") {
            $('#titles1d').append("<th>" + $(this).text() + "</th>");
        }
    });
    $('#tabclon1bd').append("<tr><td colspan='16'></td></tr>");
    for (var aa = 0; aa < tablaH1c.rows().data().length; aa++) {
        var rowH1c = tablaH1c.row(aa).node();
        $('#tabclon1bd').append("<tr id='trd" + aa + "'></tr>");
        $(rowH1c).children().each(function (td) {
            if (td !== 18 && td !== 19 && td !== 0) {
                $("#trd" + aa).append("<td>" + $(this).find('input:first').val().replace(/[^a-z0-9-/\s]/gi, '') + "</td>");
            }
        });
    }

    ////TAB2////
    $('#tab_test1').append("<table id='tab_test2clon' style='display:none'><thead id='tabclon2h'></thead><tbody id='tabclon2b'></tbody></table>");
    $('#tabclon2h').append("<tr id='titles2'></tr>");

    $('#tab_test2 > thead > tr > th').each(function () {
        var id = $(this).prop('id');
        if (id !== "lbl_estatusH2") {
            $('#titles2').append("<th>" + $(this).text() + "</th>");
        }
    });
    $('#tabclon2b').append("<tr><td colspan='9'></td></tr>");
    for (var bb = 0; bb < tablaH2c.rows().data().length; bb++) {
        var rowH2c = tablaH2c.row(bb).node();
        $('#tabclon2b').append("<tr id='tr2" + bb + "'></tr>");
        $(rowH2c).children().each(function (td2) {
            if (td2 !== 0) {
                $("#tr2" + bb).append("<td>" + $(this).find('input:first').val().replace(/[^a-z0-9-/\s]/gi, '') + "</td>");
            }
        });
    }
    ////TAB3////
    $('#tab_test1').append("<table id='tab_test3clon' style='display:none'><thead id='tabclon3h'></thead><tbody id='tabclon3b'></tbody></table>");
    $('#tabclon3h').append("<tr id='titles3'></tr>");

    $('#tab_test3 > thead > tr > th').each(function () {
        var id = $(this).prop('id');
        if (id !== "lbl_estatusH3") {
            $('#titles3').append("<th>" + $(this).text() + "</th>");
        }
    });
    $('#tabclon3b').append("<tr><td colspan='8'></td></tr>");
    for (var cc = 0; cc < tablaH3c.rows().data().length; cc++) {
        var rowH3c = tablaH3c.row(cc).node();
        $('#tabclon3b').append("<tr id='tr3" + cc + "'></tr>");
        $(rowH3c).children().each(function (td3) {
            if (td3 !== 0) {
                $("#tr3" + cc).append("<td>" + $(this).find('input:first').val().replace(/[^a-z0-9-/\s]/gi, '') + "</td>");
            }
        });
    }
    ////TAB4////
    $('#tab_test1').append("<table id='tab_test4clon' style='display:none'><thead id='tabclon4h'></thead><tbody id='tabclon4b'></tbody></table>");
    $('#tabclon4h').append("<tr id='titles4'></tr>");

    $('#tab_test4 > thead > tr > th').each(function () {
        var id = $(this).prop('id');
        if (id !== "lbl_estatusH4") {
            $('#titles4').append("<th>" + $(this).text() + "</th>");
        }
    });
    $('#tabclon4b').append("<tr><td colspan='14'></td></tr>");
    for (var dd = 0; dd < tablaH4c.rows().data().length; dd++) {
        var rowH4c = tablaH4c.row(dd).node();
        $('#tabclon4b').append("<tr id='tr4" + dd + "'></tr>");
        $(rowH4c).children().each(function (td4) {
            if (td4 !== 0) {
                if (td4 === 9) {
                    $("#tr4" + dd).append("<td>" + $(this).find('input:first').val().replace('%', '') + "</td>");
                }
                else {
                    $("#tr4" + dd).append("<td>" + $(this).find('input:first').val().replace(/[^a-z0-9-/\s]/gi, '') + "</td>");
                }
            }
        });
    }
}

var tablesToExcel = (function () {
    var uri = 'data:application/vnd.ms-excel;base64,'
        , tmplWorkbookXML = ' <xml version="1.0"><?mso-application progid="Excel.Sheet"?><Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">'
            + '<DocumentProperties xmlns="urn:schemas-microsoft-com:office:office"><Author>Axel Richter</Author><Created>{created}</Created></DocumentProperties>'
            + '<Styles>'
            + '<Style ss:ID="Currency"><NumberFormat ss:Format="Currency"></NumberFormat></Style>'
            + '<Style ss:ID="Date"><NumberFormat ss:Format="Medium Date"></NumberFormat></Style>'
            + '</Styles>'
            + '{worksheets}</Workbook>'
        , tmplWorksheetXML = '<Worksheet ss:Name="{nameWS}"><Table>{rows}</Table></Worksheet>'
        , tmplCellXML = '<Cell{attributeStyleID}{attributeFormula}><Data ss:Type="{nameType}">{data}</Data></Cell>'
        , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
        , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) }
    return function (tables, wsnames, wbname, appname) {
        var ctx = "";
        var workbookXML = "";
        var worksheetsXML = "";
        var rowsXML = "";

        for (var i = 0; i < tables.length; i++) {
            if (!tables[i].nodeType) tables[i] = document.getElementById(tables[i]);
            for (var j = 0; j < tables[i].rows.length; j++) {
                rowsXML += '<Row>';
                for (var k = 0; k < tables[i].rows[j].cells.length; k++) {
                    var dataType = tables[i].rows[j].cells[k].getAttribute("data-type");
                    var dataStyle = tables[i].rows[j].cells[k].getAttribute("data-style");
                    var dataValue = tables[i].rows[j].cells[k].getAttribute("data-value");
                    dataValue = (dataValue) ? dataValue : tables[i].rows[j].cells[k].innerHTML;
                    var dataFormula = tables[i].rows[j].cells[k].getAttribute("data-formula");
                    dataFormula = (dataFormula) ? dataFormula : (appname === 'Calc' && dataType === 'DateTime') ? dataValue : null;
                    ctx = {
                        attributeStyleID: (dataStyle === 'Currency' || dataStyle === 'Date') ? ' ss:StyleID="' + dataStyle + '"' : ''
                        , nameType: (dataType === 'Number' || dataType === 'DateTime' || dataType === 'Boolean' || dataType === 'Error') ? dataType : 'String'
                        , data: (dataFormula) ? '' : dataValue
                        , attributeFormula: (dataFormula) ? ' ss:Formula="' + dataFormula + '"' : ''
                    };
                    rowsXML += format(tmplCellXML, ctx);
                }
                rowsXML += '</Row>';
            }
            ctx = { rows: rowsXML, nameWS: wsnames[i] || 'Sheet' + i };
            worksheetsXML += format(tmplWorksheetXML, ctx);
            rowsXML = "";
        }

        ctx = { created: (new Date()).getTime(), worksheets: worksheetsXML };
        workbookXML = format(tmplWorkbookXML, ctx);



        var link = document.createElement("A");
        link.href = uri + base64(workbookXML);
        link.download = wbname || 'TestBook.xls';
        link.target = '_blank';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
})();
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//function mandaArc(file) {
//    var formData = new FormData();
//    formData.append("FileUpload", file);
//    $.ajax({
//        type: "POST",
//        url: 'getDatosMasiva',
//        data: formData,
//    contentType: false,
//    processData: false
//}).fail(function () {
//    alert("error");
//});
//};

//function validaHoja1() {
//    $.ajax({
//        url: 'validaHoja1',
//    }).done(function (data) {
//        //$("table#test1 tbody").append(data);
//        $("#test1").append(data);
//    }).fail(function () {
//        alert("error");
//    });
//};

//function validaHoja2() {
//    $.ajax({
//        url: 'validaHoja2',
//    }).done(function (data) {
//        $('#test2').html(data);
//    }).fail(function () {
//        alert("error");
//    });
//};

//function validaHoja3() {
//    $.ajax({
//        url: 'validaHoja3',
//    }).done(function (data) {
//        $('#test3').html(data);
//    }).fail(function () {
//        alert("error");
//    });
//};

//function validaHoja4() {
//    $.ajax({
//        url: 'validaHoja4',
//    }).done(function (data) {
//        $('#test4').html(data);
//    }).fail(function () {
//        alert("error");
//    });
//};

//function validaHoja5() {
//    $.ajax({
//        url: 'validaHoja5',
//    }).done(function (data) {
//        $('#test5').html(data);
//    }).fail(function () {
//        alert("error");
//    });
//};

function InicializarTablas() {
    var table = $('#tab_test1').DataTable();
    table.clear().draw();
    table.destroy();

    var table2 = $('#tab_test2').DataTable();
    table2.clear().draw();
    table2.destroy();

    var table3 = $('#tab_test3').DataTable();
    table3.clear().draw();
    table3.destroy();

    var table4 = $('#tab_test4').DataTable();
    table4.clear().draw();
    table4.destroy();

    var table5 = $('#tab_test5').DataTable();
    table5.clear().draw();
    table5.destroy();

    cargaInicialSpin[0] = false;
    cargaInicialSpin[1] = false;
    cargaInicialSpin[2] = false;
    cargaInicialSpin[3] = false;
    cargaInicialSpin[4] = false;

    $('#tab_info').removeClass("red white-text rojo");
    $('#tab_rel').removeClass("red white-text rojo");
    $('#tab_mul').removeClass("red white-text rojo");
    $('#tab_dis').removeClass("red white-text rojo");
    $('#tab_arc').removeClass("red white-text rojo");
}
function getCategoria(mat) {
    
    var categoriamaterial = "";
    if (mat !== "") {
        $.ajax({
            type: "POST",
            url: root + 'Solicitudes/getCategoria',
            data: { "material": mat },
            dataType: "json",

            success: function (data) {

                if (data !== null || data !== "") {
                    categoriamaterial =data;
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
    }
    return categoriamaterial;

}
function getTableIndex(table, idColumna) {
    var index;
    $(table + ' thead>tr').each(function () {
        $('th', this).each(function (i) {
            var column = $(this);
            if (column.context.id === idColumna) {
                index = i;
            }
        });
    });  

    return index;
}
function getWarning(texto) {
    msj = "";
    if (texto) {
        error = texto.split('|');
        if (error.length > 1) {
            msj = error[1];
        }
    }
    return msj;
}

function mostrarAlerta(warning_id, tipo, mensaje) {
    var dura = 1000000,
        color = 'yellow',
        icon = 'info',
        classe = 'toast';
    if (tipo === "E") {
        color = 'red';
        icon = 'error';
    }
    else if (tipo === "B") {
        color = 'green';
        icon = 'done';
    }
    M.toast({
        classes: classe,
        displayLength: dura,
        html: '<span style="padding-right:15px;"><i class="material-icons ' + color + '-text">' + icon + '</i></span>  ' + mensaje
            + '<button class="btn-small btn-flat toast-action" onclick="dismiss(\'toast\')">Aceptar</button>'
    });
}
function dismiss(classe) {
    var toastElement = document.querySelectorAll('.' + classe);
    for (var i = 0; i < toastElement.length; i++) {
        var toastInstance = M.Toast.getInstance(toastElement[i]);
        toastInstance.dismiss();
    }
}
function generarExcel() {
    document.getElementById("loader").style.display = "initial";

    var tablaH1 = $('#tab_test1').DataTable();
    var tablaH2 = $('#tab_test2').DataTable();
    var tablaH3 = $('#tab_test3').DataTable();
    var tablaH4 = $('#tab_test4').DataTable();
    var tablaH5 = $('#tab_test5').DataTable();
    var tabla1 = [], tabla2 = [], tabla3 = [], tabla4 = [];
    var rowsH1 = [], rowsH2 = [], rowsH3 = [], rowsH4 = [];
    var a = 0, b = 0, c = 0, d = 0, e = 0;
    for (var aa = 0; aa < tablaH1.rows().data().length; aa++) {
        var rowH1 = tablaH1.row(aa).node();
        var status = $(rowH1).children().eq(0).children().text();
        var num_doc = $(rowH1).children().eq(1).children().val();
        var t_sol = $(rowH1).children().eq(2).children().val();
        var tall_id = $(rowH1).children().eq(3).children().val();;
        var bukrs = $(rowH1).children().eq(4).children().val();
        var land = $(rowH1).children().eq(5).children().val();;
        var estado = $(rowH1).children().eq(6).children().val();
        var ciudad = $(rowH1).children().eq(7).children().val();
        var concepto = $(rowH1).children().eq(8).children().val();
        var notas = $(rowH1).children().eq(9).children().val();
        var payer_id = $(rowH1).children().eq(10).children().val();
        var payer_nombre = $(rowH1).children().eq(11).children().val();
        var contacto_nombre = $(rowH1).children().eq(12).children().val();
        var contacto_email = $(rowH1).children().eq(13).children().val();
        var fechai_vig = $(rowH1).children().eq(14).children().val();
        var fechaf_vig = $(rowH1).children().eq(15).children().val();
        var moneda_id = $(rowH1).children().eq(16).children().val();

        rowsH1[a] = [num_doc, t_sol, tall_id, bukrs, land, estado, ciudad, concepto, notas, payer_id, payer_nombre, contacto_nombre, contacto_email, fechai_vig, fechaf_vig, moneda_id];
        a++;
    }

    for (var bb = 0; bb < tablaH2.rows().data().length; bb++) {
        var rowH2 = tablaH2.row(bb).node();
        var statusH2 = $(rowH2).children().eq(0).children().text();
            var num_docH2 = $(rowH2).children().eq(1).children().val();
            var facturaH2 = $(rowH2).children().eq(2).children().val();
            var fecha_facturaH2 = $(rowH2).children().eq(3).children().val();
            var proveedorH2 = $(rowH2).children().eq(4).children().val();
            var proveedor_nombreH2 = $(rowH2).children().eq(5).children().val();
            var autorizacionH2 = $(rowH2).children().eq(6).children().val();
            var vencimientoH2 = $(rowH2).children().eq(7).children().val();
            var facturakH2 = $(rowH2).children().eq(8).children().val();
            var ejerciciokH2 = $(rowH2).children().eq(9).children().val();

            rowsH2[b] = [num_docH2, facturaH2, fecha_facturaH2, proveedorH2, proveedor_nombreH2, autorizacionH2, vencimientoH2, facturakH2, ejerciciokH2];
            b++;
    }

    for (var cc = 0; cc < tablaH3.rows().data().length; cc++) {
        var rowH3 = tablaH3.row(cc).node();
        var statusH3 = $(rowH3).children().eq(0).children().text();
            var num_docH3 = $(rowH3).children().eq(1).children().val();
            var facturaH3 = $(rowH3).children().eq(2).children().val();
            var bill_docH3 = $(rowH3).children().eq(3).children().val();
            var ejerciciokH3 = $(rowH3).children().eq(4).children().val();
            var payer_idH3 = $(rowH3).children().eq(5).children().val();
            var payer_nombreH3 = $(rowH3).children().eq(6).children().val();
            var importe_facH3 = $(rowH3).children().eq(7).children().val();
            var belnrH3 = $(rowH3).children().eq(8).children().val();

            rowsH3[c] = [num_docH3, facturaH3, bill_docH3, ejerciciokH3, payer_idH3, payer_nombreH3, importe_facH3, belnrH3];
            c++;
    }

    for (var dd = 0; dd < tablaH4.rows().data().length; dd++) {
        var rowH4 = tablaH4.row(dd).node();
        var statusH4 = $(rowH4).children().eq(0).children().text();
        var num_docH4 = $(rowH4).children().eq(1).children().val();
        var checkH4 = $(rowH4).children().eq(2).children().children().children().is(':checked');
        var vigencia_deH4 = $(rowH4).children().eq(3).children().val();
        var vigencia_alH4 = $(rowH4).children().eq(4).children().val();
        var matnrH4 = $(rowH4).children().eq(5).children().val();
        var matklH4 = $(rowH4).children().eq(6).children().val();
        var descripcionH4 = $(rowH4).children().eq(7).children().val();
        var montoH4 = $(rowH4).children().eq(8).children().val();
        var porc_apoyoH4 = $(rowH4).children().eq(9).children().val();
        var apoyo_piezaH4 = $(rowH4).children().eq(10).children().val();
        var costo_apoyoH4 = $(rowH4).children().eq(11).children().val();
        var precio_sugH4 = $(rowH4).children().eq(12).children().val();
        var volumen_realH4 = $(rowH4).children().eq(13).children().val();
        var apoyoH4 = $(rowH4).children().eq(14).children().val();

        rowsH4[d] = [num_docH4, checkH4, vigencia_deH4, vigencia_alH4, matnrH4, matklH4, descripcionH4, montoH4, porc_apoyoH4, apoyo_piezaH4, costo_apoyoH4, precio_sugH4, volumen_realH4, apoyoH4];
        d++;
    }
    tabla1 = rowsH1;
    tabla2 = rowsH2;
    tabla3 = rowsH3;
    tabla4 = rowsH4;
    $.ajax({
        type: "POST",
        url: 'DescargarExcel',
        dataType: "json",
        data: { "h1": tabla1, "h2": tabla2, "h3": tabla3, "h4": tabla4},
        async: true,
        success: function (data) {
            document.getElementById("loader").style.display = "none";
            if (data) {
                if (data.isRedirect) {
                    window.onbeforeunload = false;
                    window.location = data.redirectUrl;
                } else {
                    var strXML = data.split("PdfTemp");
                    var strNombre = data.split("\\");
                    var direccion = strXML[strXML.length - 1];
                    direccion = direccion.replace(/ \\/gi, "\\");
                    var mod = direccion.replace(/\\/gi, "/");
                    mod.replace('/', "");
                    mod = "PdfTemp" + mod;
                    var dir = root + mod;
                    var link = document.createElement("a");
                    link.download = "Template Masivas";
                    link.target = '_blank';
                    link.href = dir;
                    document.body.appendChild(link);
                    link.click();
                    link.parentNode.removeChild(link);
                    InicializarTablas();
                }
            }
        },

        error: function (xhr, httpStatusMessage, customErrorMessage) {
            document.getElementById("loader").style.display = "none";
            M.toast({ html: 'Error al guardar archivo ' + xhr.status + " : " + httpStatusMessage + " : " + customErrorMessage });
        }
    });

}
function generarWarningH1() {
    var elementos = [];
    var numDoc_index = getTableIndex('#tab_test1', 'lbl_numDocH1');
    var tSol_index = getTableIndex('#tab_test1', 'lbl_tipoSolicitud');
    var clasificacion_idex = getTableIndex('#tab_test1', 'lbl_clasificacion');
    var soc_index = getTableIndex('#tab_test1', 'lbl_sociedad');
    var pais_index = getTableIndex('#tab_test1', 'lbl_pais');
    var estado_idex = getTableIndex('#tab_test1', 'lbl_estado');
    var ciudad_index = getTableIndex('#tab_test1', 'lbl_ciudad');
    var concepto_index = getTableIndex('#tab_test1', 'lbl_concepto');
    var notas_index = getTableIndex('#tab_test1', 'lbl_notas');
    var payer_index = getTableIndex('#tab_test1', 'lbl_payerId');
    var payerName_index = getTableIndex('#tab_test1', 'lbl_payerNombre');
    var contato_index = getTableIndex('#tab_test1', 'lbl_contactoNombre');
    var email_index = getTableIndex('#tab_test1', 'lbl_contactoEmail');
    var fechaI_index = getTableIndex('#tab_test1', 'lbl_fechaInicio');
    var fechaF_index = getTableIndex('#tab_test1', 'lbl_fechaFin');
    var moneda_index = getTableIndex('#tab_test1', 'lbl_moneda');

    elementos.push({ elementIndex: ciudad_index, elementId: 'city_id' });
    elementos.push({ elementIndex: concepto_index, elementId: 'concepto' });
    elementos.push({ elementIndex: fechaI_index, elementId: 'fechai_vig' });
    elementos.push({ elementIndex: fechaF_index, elementId: 'fechaf_vig' });
    elementos.push({ elementIndex: notas_index, elementId: 'notas_txt' });
    elementos.push({ elementIndex: email_index, elementId: 'payer_email' });
    elementos.push({ elementIndex: payer_index, elementId: 'payer_id' });
    elementos.push({ elementIndex: contato_index, elementId: 'payer_nombre' });
    elementos.push({ elementIndex: estado_idex, elementId: 'state_id' });
    elementos.push({ elementIndex: clasificacion_idex, elementId: 'tall_id' });
    elementos.push({ elementIndex: tSol_index, elementId: 'tsol_id' });

    generarScriptValidaciones('#tab_test1', elementos);

}
function generarWarningH2() {

    var elementos = [];
    var numDoc_index = getTableIndex('#tab_test2', 'lbl_numDocH2');
    var factura_index = getTableIndex('#tab_test2', 'lbl_factura');
    var facturaFecha_index = getTableIndex('#tab_test2', 'lbl_facturaFecha');
    var proveedor_index = getTableIndex('#tab_test2', 'lbl_proveedor');
    var proveedorNombre_index = getTableIndex('#tab_test2', 'lbl_proveedorNombre');
    var autorizacion_index = getTableIndex('#tab_test2', 'lbl_autorizacion');
    var vencimiento_index = getTableIndex('#tab_test2', 'lbl_vencimiento');
    var facturak_index = getTableIndex('#tab_test2', 'lbl_facturak');
    var ejerciciok_index = getTableIndex('#tab_test2', 'lbl_ejerciciok');

    elementos.push({ elementIndex: factura_index, elementId: 'FACTURA' });
    elementos.push({ elementIndex: facturaFecha_index, elementId: 'FECHA' });
    elementos.push({ elementIndex: proveedor_index, elementId: 'PROVEEDOR' });
    elementos.push({ elementIndex: facturak_index, elementId: 'FACTURAK' });
    elementos.push({ elementIndex: ejerciciok_index, elementId: 'EJERCICIOK' });
    generarScriptValidaciones('#tab_test2', elementos);
}
function generarWarningH3() {
    var elementos = [];
    var numDoc_index = getTableIndex('#tab_test3', 'lbl_numDocH3');
    var facturaFiscal_index = getTableIndex('#tab_test3', 'lbl_facturaFiscal');
    var bill_index = getTableIndex('#tab_test3', 'lbl_bill');
    var ejerciciokH3_index = getTableIndex('#tab_test3', 'lbl_ejerciciokH3');
    var payerIdH3_index = getTableIndex('#tab_test3', 'lbl_payerIdH3');
    var payerNombreH3_index = getTableIndex('#tab_test3', 'lbl_payerNombreH3');
    var importeFac_index = getTableIndex('#tab_test3', 'lbl_importeFac');
    var folio_index = getTableIndex('#tab_test3', 'lbl_folio');

    elementos.push({ elementIndex: facturaFiscal_index, elementId: 'FACTURA' });
    elementos.push({ elementIndex: bill_index, elementId: 'BELNR' });
    elementos.push({ elementIndex: ejerciciokH3_index, elementId: 'EJERCICIOK' });
    elementos.push({ elementIndex: folio_index, elementId: 'BELNR' });

    generarScriptValidaciones('#tab_test3', elementos);


}

function getTotalApoyo(numDoc) {
    var total = 0;
    var tablaH4 = $('#tab_test4').DataTable();
    var indexNumDoc = getTableIndex('#tab_test4', 'lbl_numDocH4');
    var lbl_apoyo = getTableIndex('#tab_test4', 'lbl_apoyo');

    for (var a = 0; a < tablaH4.rows().data().length; a++) {
        var rowH4 = tablaH4.row(a).node();
        var num_docH4 = $(rowH4).children().eq(indexNumDoc).children().val();
        var apoyo = $(rowH4).children().eq(lbl_apoyo).children().val();
        if (numDoc === num_docH4) {
            total = total + toNum(apoyo, ',', '.');
        }
    }
    var sum = toShow(total, '.');
    $('#grupo' + numDoc).children().eq(1).children().text(sum);
}

