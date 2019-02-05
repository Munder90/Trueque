$(document).ready(function () {
    formato();
});
var esFile = false;
function formato() {
    $('#table').DataTable({
        "scrollY": "200",
        "scrollX": "true",
        "scrollCollapse": true,
        "order": false,
        "language": {
            "url": root + "Scripts/lang/" + spras + ".json",
            "zerorecords": "no hay registros",
            "infoempty": "registros no disponibles"
        },
        "paging": false,
        "info": false,
        "searching": false,

        "columns": [
            {
                "name": 'Borrar',
                "className": 'Borrar'
            },
            {
                "name": 'KUNNR',
                "className": 'KUNNR'
            },
            {
                "name": 'BUNIT',
                "className": 'BUNIT'
            },
            {
                "name": 'PUESTO_ID',
                "className": 'PUESTO_ID'
            },
            {
                "name": 'ID',
                "className": 'ID'
            },
            {
                "name": 'NOMBRE',
                "className": 'NOMBRE'
            },
            {
                "name": 'APELLIDO_P',
                "className": 'APELLIDO_P'
            },
            {
                "name": 'APELLIDO_M',
                "className": 'APELLIDO_M'
            },
            {
                "name": 'EMAIL',
                "className": 'EMAIL'
            },
            {
                "name": 'SPRAS_ID',
                "className": 'SPRAS_ID'
            },
            {
                "name": 'PASS',
                "className": 'PASS'
            },
            {
                "name": 'mess',
                "className": 'mess'
            }
        ]
    });
}
function subeExcel() {
    var filenum = $('#files').get(0).files.length;
    if (filenum > 0) {
        var file = document.getElementById("files").files[0];
        var filename = file.name;
        if (evaluarExt(filename)) {
            mostrarAlerta("info", "B", 'Archivo: ' + filename);
            loadExcelDis(file);
            onclick = checkoff();
            sessionStorage.setItem("num", filenum);
            $('#files').val("");
        } else {
            document.getElementById("files").value = "";
            mostrarAlerta("info", "E", 'Tipo de archivo incorrecto: ' + filename);
            sessionStorage.setItem("num", filenum);
            $('#files').val("");
        }
    } else {
        mostrarAlerta("info", "A", 'Seleccione un archivo');
        var table = $('#table').DataTable();
        table.clear().draw();
    }
}

function evaluarExt(filename) {

    var exts = ['xls', 'xlsx'];
    // split file name at dot
    var get_ext = filename.split('.');
    // reverse name to check extension
    get_ext = get_ext.reverse();
    // check file type is valid as given in 'exts' array
    if ($.inArray(get_ext[0].toLowerCase(), exts) > -1) {
        return true;
    } else {
        return false;
    }
}

function loadExcelDis(file) {
    esFile = true;
    document.getElementById("loader").style.display = "initial";
    var formData = new FormData();

    formData.append("FileUpload", file);

    var table = $('#table').DataTable();
    table.clear().draw();
    $.ajax({
        type: "POST",
        url: 'LoadExcel',
        data: formData,
        dataType: "json",
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {

            if ((data !== null || data !== "") && !data.isRedirect) {
                $.each(data, function (i, dataj) {

                    var cli = dataj.KUNNR;
                    var com = dataj.BUNIT;
                    var niv = dataj.PUESTO_ID;
                    var usc = dataj.ID;
                    var spr = dataj.SPRAS_ID;
                    var ema = dataj.EMAIL;
                    var bor = i;
                    var mes = dataj.mess;

                    if (mes === null)
                        mes = "";

                    //identificacion de error
                    if (cli.indexOf('?') !== -1) {
                        cli = cli.slice(0, -1);
                        var clix = true;
                    }
                    if (com.indexOf('?') !== -1) {
                        com = com.slice(0, -1);
                        var comx = true;
                    }
                    if (niv.indexOf('?') !== -1) {
                        niv = niv.slice(0, -1);
                        var nivx = true;
                    }
                    if (usc.indexOf('?') !== -1) {
                        usc = usc.slice(0, -1);
                        var uscx = true;
                    }
                    if (usc.indexOf('!') !== -1) {
                        usc = usc.slice(0, -1);
                        var uscy = true;
                    }
                    if (spr.indexOf('?') !== -1) {
                        spr = spr.slice(0, -1);
                        var sprx = true;
                    }
                    if (ema.indexOf('?') !== -1) {
                        ema = ema.slice(0, -1);
                        var emax = true;
                    }//Termina identificacion

                    var addedRow = addRow(table, dataj.POS, bor, cli, com, niv, usc, dataj.NOMBRE, dataj.APELLIDO_P, dataj.APELLIDO_M, ema, spr, dataj.PASS, mes);

                    var cols = addedRow.cells[1];
                    if (clix === true) {
                        $(cols).addClass("red");
                    }
                    cols = addedRow.cells[2];
                    if (comx === true) {
                        $(cols).addClass("red");
                    }
                    cols = addedRow.cells[3];
                    if (nivx === true) {
                        $(cols).addClass("red");
                    }
                    cols = addedRow.cells[4];
                    if (uscx === true) {
                        $(cols).addClass("red");
                    }
                    if (uscy === true) {
                        $(cols).addClass("yellow");
                    }
                    cols = addedRow.cells[8];
                    if (emax === true) {
                        $(cols).addClass("red");
                    }
                    cols = addedRow.cells[9];
                    if (sprx === true) {
                        $(cols).addClass("red");
                    }
                });
                $('#table_dis').css("font-size", "12px");
                $('#table_dis').css("display", "table");
                $('#tfoot_dis').css("display", "table-footer-group");
                document.getElementById("loader").style.display = "none";
            }
            else {
                window.location.href = data.redirectUrl;
            }
        },
        complete: function () {

            var num = $("#table tr").length - 1;
            addRow(table, num, num, "", "", "", "", "", "", "", "", "", "", "");
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({
                html: "Request couldn't be processed. Please try again later. the reason        " + xhr.status + " : " + httpStatusMessage + ": " + customErrorMessage
            });
            document.getElementById("loader").style.display = "none";
        },
        async: true
    });
    $("#table > tbody  > tr[role='row']").each(function () {
        ////alert("as");
    });
}

function addRow(t, POS, br, k, b, pi, id, n, ap, am, e, si, sp, me) {
    var ppr = "";
    var r = addRowl(
        t,
        POS,
        "<label><input class='input_bor' type='checkbox' id='' name='bor' onclick='checkoff();' value='" + br + "'><span></span></label>",
        "<input class='input_cli' style='font-size:12px;' type='text' id='' name='cli' value='" + k + "' >",
        "<input class='input_com' style='font-size:12px;' type='text' id='' name='com' value='" + b + "' >",
        "<input class='input_niv' style='font-size:12px;' type='text' id='' name='niv' value='" + pi + "' onchange='Comprobar()'>",
        "<input class='input_usc' readonly='readonly' style='font-size:12px;' type='text' id='' name='usc' value='" + id + "'>",
        "<input class='input_nom' style='font-size:12px;' type='text' id='' name='nom' value='" + n + "' onchange='Comprobar()'>",
        "<input class='input_app' style='font-size:12px;' type='text' id='' name='app' value='" + ap + "' onchange='Comprobar()'>",
        "<input class='input_apm' style='font-size:12px;' type='text' id='' name='apm' value='" + am + "' onchange='Comprobar()'>",
        "<input class='input_ema' style='font-size:12px;' type='text' id='' name='ema' value='" + e + "' onchange='Comprobar()'>",
        "<input class='input_idi' style='font-size:12px;' type='text' id='' name='idi' value='" + si + "' onchange='Comprobar()'>",
        "<input class='input_pas' style='font-size:12px;' type='text' id='' name='pas' value='" + sp + "' onchange='Comprobar()'>",
        "<input class='input_mes' type='hidden' name='mes' value='" + me + "'><span class='input_mes' style='font-size:12px;'>" + me + "</span>"
    );
    return r;
}

function addRowl(t, pos, br, k, b, pi, id, n, ap, am, e, si, sp, me) {
    var r = t.row.add([
        br,
        k,
        b,
        pi,
        id,
        n,
        ap,
        am,
        e,
        si,
        sp,
        me
    ]).draw(false).node();
    return r;
}


function Comprobar() {
    var datos = $('#tabla').serializeArray();
    document.getElementById("loader").style.display = "flex";
    creart('Comprobar', datos); 
    //mostrarAlerta("info", "A", "Vista Actualizada");
}

function Borrar() {
    var rowNum = $('#table>tbody').find('.input_bor').length;
    var check = false;
    $('#table>tbody').find('.input_bor').each(function (indx, checkInput) {
        if (checkInput.checked) {
            check = checkInput.checked;
        }
    });
    if (rowNum === 0 || !check) {
        return;
    }
    var table = $('#table').DataTable();
    var rows = $('.input_bor').serializeArray();
    for (var i = rows.length; i > 0; i--) {
        var num = rows[i - 1].value;
        table.row(num).remove().draw();
    }
    if (rowNum > 0) {
        Comprobar();
        $('#idBorrar').attr('disabled', 'disabled');
    } else {
        Iniciar();
    }
}
function Limpiar() {
    var table = $('#table').DataTable();
    table.clear().draw();
    Iniciar();
}

function Iniciar() {
    var htmlTr = '<tr>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td><input class="input_usc" style="font-size:12px;" type="text" id="" name="usc" value="" onkeyup="if(event.keyCode == 13) AgregarU()" /></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
        + '</tr>';
    $('#table').DataTable().destroy();
    $('#table').append(htmlTr);
    formato();
    $('#idBorrar').attr('disabled', 'disabled');
    $("#borrar").prop('checked', false);

}

function Guardar() {
    var rowNum = $('#table>tbody').find('.input_bor').length;
    if (rowNum === 0) {
        return;
    }

    var rowNumConError = $('#table>tbody').find('.red').length;
    if (rowNumConError===0) {
            document.getElementById("loader").style.display = "flex";
            $.ajax({
                type: "POST",
                url: 'Guardar',
                data: null,
                dataType: "json",
                success: function (data) {
                    mostrarAlerta("info", "A", "Se agregaron los nuevos registros");
                    if (esFile) {
                        window.location = root + "Usuarios";
                    } else {
                        Limpiar();
                        document.getElementById("loader").style.display = "none";
                    }
                },
                error: function (request, status, error) {
                    document.getElementById("loader").style.display = "none";
                    //alert(request.responseText);
                },
                async: true
            });
        
    } else
        mostrarAlerta("info", "E", "Hay errores por corregir");
}

function creart(metodo, datos) {
    var table = $('#table').DataTable();
    $.ajax({
        type: "POST",
        url: metodo,
        dataType: "json",
        data: datos,
        success: function (data) {
            if ((data !== null || data !== "") && !data.isRedirect) {

                table.clear().draw();

                $.each(data, function (i, dataj) {

                    var cli = dataj.KUNNR;
                    var com = dataj.BUNIT;
                    var niv = dataj.PUESTO_ID;
                    var usc = dataj.ID;
                    var spr = dataj.SPRAS_ID;
                    var ema = dataj.EMAIL;
                    var bor = i;
                    var clix = false;
                    var mes = dataj.mess;

                    if (mes === null)
                        mes = "";

                    //identificacion de error
                    if (cli.indexOf('?') !== -1) {
                        cli = cli.slice(0, -1);
                        clix = true;
                    }
                    if (com.indexOf('?') !== -1) {
                        com = com.slice(0, -1);
                        var comx = true;
                    }
                    if (niv.indexOf('?') !== -1) {
                        niv = niv.slice(0, -1);
                        var nivx = true;
                    }
                    if (usc.indexOf('?') !== -1) {
                        usc = usc.slice(0, -1);
                        var uscx = true;
                    }
                    if (usc.indexOf('!') !== -1) {
                        usc = usc.slice(0, -1);
                        var uscy = true;
                    }
                    if (spr.indexOf('?') !== -1) {
                        spr = spr.slice(0, -1);
                        var sprx = true;
                    }
                    if (ema.indexOf('?') !== -1) {
                        ema = ema.slice(0, -1);
                        var emax = true;
                    }//Termina identificacion

                    var addedRow = addRow(table, dataj.POS, bor, cli, com, niv, usc, dataj.NOMBRE, dataj.APELLIDO_P, dataj.APELLIDO_M, ema, spr, dataj.PASS, mes);

                    var cols = addedRow.cells[1];
                    if (clix === true) {
                        $(cols).addClass("red");
                    }
                    cols = addedRow.cells[2];
                    if (comx === true) {
                        $(cols).addClass("red");
                    }
                    cols = addedRow.cells[3];
                    if (nivx === true) {
                        $(cols).addClass("red");
                    }
                    cols = addedRow.cells[4];
                    if (uscx === true) {
                        $(cols).addClass("red");
                    }
                    if (uscy === true) {
                        $(cols).addClass("yellow");
                    }
                    cols = addedRow.cells[8];
                    if (emax === true) {
                        $(cols).addClass("red");
                    }
                    cols = addedRow.cells[9];
                    if (sprx === true) {
                        $(cols).addClass("red");
                    }
                });
                $('#table_dis').css("font-size", "12px");
                $('#table_dis').css("display", "table");
                $('#tfoot_dis').css("display", "table-footer-group");
                document.getElementById("loader").style.display = "none";
            }
            else {
                window.location.href = data.redirectUrl;
            }
        },
        complete: function () {
            if (!existeRowSinDatos()) {
                var num = $("#table tr").length - 1;
                addRow(table, num, num, "", "", "", "", "", "", "", "", "", "", "");
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({
                html: "Request couldn't be processed. Please try again later. the reason        " + xhr.status + " : " + httpStatusMessage + ": " + customErrorMessage
            });
            document.getElementById("loader").style.display = "none";
        },
        async: true
    });
}

function check() {
    if ($("#borrar").prop('checked') && $('#table>tbody').find('.input_bor').length > 0) {
        $('#idBorrar').removeAttr('disabled');
        $(".input_bor").prop('checked', true);
    } else {
        $(".input_bor").prop('checked', false);
        $('#idBorrar').attr('disabled', 'disabled');
    }

}

function checkoff() {
    var check = false;
    $('#table>tbody').find('.input_bor').each(function (indx, checkInput) {
        if (checkInput.checked) {
            check = checkInput.checked;
        }
    });
    if (!check) {
        $("#borrar").prop('checked', false);
        $('#idBorrar').attr('disabled', 'disabled');
    } else {
        $('#idBorrar').removeAttr('disabled');
    }
}

function AgregarU() {
    esFile = false;
    var datos = $('.input_usc').serializeArray();
    document.getElementById("loader").style.display = "flex";
    creart('AgregarT', datos);
}

function AgregarC() {
    
    var table = $('#table').DataTable(),
    num = $("#table tr").length - 1;
    addRow(table, num, num, "", "", "", "", "", "", "", "", "", "", "");
}

function foco() {
    $("input").blur();
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
    dismiss(classe);
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
function existeRowSinDatos() {
    var conDatos = false;
    var table = $("#table").DataTable();
    var row = table.row(table.rows()[0].length - 1).node();
    if (row) {
        for (var j = 0; j < row.cells.length; j++) {
            var children = row.cells.item(j).children;
            if (children.length > 0 && (children.item(0).value !== undefined && children.item(0).value !== "")) {
                conDatos = true;
            }

        }
    }
    return !conDatos;

}
function obtenerSociedadId(rowIndx) {
    var sociedad_id = null;
    var table = $("#table").DataTable();
    for (var i = rowIndx; i >= 0; i--) {
        var row = table.row(i).node();
        for (var j = 0; j < row.cells.length; j++) {
            var children = row.cells.item(j).children;
            if (children.length > 0 && children.item(0).name === "com" && children.item(0).value !== "" && sociedad_id === null) {
                sociedad_id = children.item(0).value;
            }
        }
    }
    return sociedad_id;

}
$('body').on('keydown.autocomplete', '.input_cli', function () {
    var rowIndx = this.parentElement._DT_CellIndex.row;
    var sociedad_id = obtenerSociedadId(rowIndx);
        auto(this).autocomplete({
            source: function (request, response) {
                auto.ajax({
                    type: "GET",
                    url: root +'Listas/Clientes',
                    dataType: "json",
                    data: {
                        Prefix: request.term, sociedad_id: sociedad_id },
                    success: function (data) {
                        response(auto.map(data, function (item) {
                            return { label: item.KUNNR + " | " + item.NAME1, value: item.KUNNR };
                        }));
                    }
                });
            },

            messages: {
                noResults: '',
                results: function (resultsCount) { }
            },

            change: function (e, ui) {
                if (this.value === "") {
                    Comprobar();
                }  

                  
                
            },

            select: function (event, ui) {
                if (!existeRowSinDatos()) {
                    AgregarC();
                }
                var value = ui.item.value;
                this.value = value;
                Comprobar();
            }
        });
   
});

$('body').on('keydown.autocomplete', '.input_usc', function () {
        auto(this).autocomplete({
            source: function (request, response) {
                auto.ajax({
                    type: "GET",
                    url: root+'Listas/Usuarios',
                    dataType: "json",
                    data: {Prefix: request.term },
                    success: function (data) {
                        response(auto.map(data, function (item) {
                            return { label: item.ID + " | " + item.NOMBRE + " " + item.APELLIDO_P, value: item.ID };
                        }));
                    }
                });
            },

            messages: {
                noResults: '',
                results: function (resultsCount) { }
            },

            change: function (e, ui) {
                if (!(ui.item) && $(".input_usc").val() === "") {
                    e.target.value = "";
                }
            },

            select: function (event, ui) {
            }
        });
});

$('body').on('keydown.autocomplete', '.input_idi', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'Idioma',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: item.ID + " | " + item.DESCRIPCION, value: item.ID };
                    }));
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item) && $(".input_idi").val() === "") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
        }
    });
});

$('body').on('keydown.autocomplete', '.input_com', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root+'Listas/Sociedades',
                dataType: "json",
                data: { "Prefix": request.term },
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
            if (this.value==="") {
                Comprobar();
            }
        },

        select: function (event, ui) {
            if (!existeRowSinDatos()) {
                AgregarC();
            }
            var value = ui.item.value;
            this.value = value;
            Comprobar();
        }
    });
});

$('body').on('keydown.autocomplete', '.input_niv', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'Nivel',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: item.ID + " | " + item.TEXTO, value: item.ID };
                    }));
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item) && $(".input_niv").val() === "") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
        }
    });
});