$(document).ready(function () {
    formato();
});
var esFile = false;
function formato() {
    $('#table').DataTable({
        "order": [],
        "language": {
            "url": root + "Scripts/lang/" + spras + ".json",
            "zerorecords": "no hay registros",
            "infoempty": "registros no disponibles"
        },
        "paging": "full_numbers",
        "info": false,
        "searching": false,
        "scrollY": "200",
        "scrollX": "true",
        "scrollCollapse": true,

        "columns": [
            {
                "name": 'Borrar',
                "className": 'Borrar'
            },
            {
                "name": 'CoCode',
                "className": 'CoCode'
            },
            {
                "name": 'Pais',
                "className": 'Pais'
            },
            {
                "name": 'Cliente',
                "className": 'Cliente'
            },
            {
                "name": 'Nombre_cliente',
                "className": 'Nombre_cliente'
            },
            {
                "name": 'Nivel_0',
                "className": 'Nivel_0'
            },
            {
                "name": 'Nivel_1',
                "className": 'Nivel_1'
            },
            {
                "name": 'Nivel_2',
                "className": 'Nivel_2'
            },
            {
                "name": 'Nivel_3',
                "className": 'Nivel_3'
            },
            {
                "name": 'Nivel_4',
                "className": 'Nivel_4'
            },
            {
                "name": 'Nivel_5',
                "className": 'Nivel_5'
            },
            {
                "name": 'Nivel_6',
                "className": 'Nivel_6'
            },
            {
                "name": 'Nivel_7',
                "className": 'Nivel_7'
            },
            {
                "name": 'Vendor',
                "className": 'Vendor'
            },
            {
                "name": 'Banner',
                "className": 'Banner'
            },
            {
                "name": 'Banner_Agrupador',
                "className": 'Banner_Agrupador'
            },
            {
                "name": 'Canal',
                "className": 'Canal'
            },
            {
                "name": 'Exportacion',
                "className": 'Exportacion'
            },
            {
                "name": 'Contacto',
                "className": 'Contacto'
            },
            {
                "name": 'Email_Contacto',
                "className": 'Email_Contacto'
            },
            {
                "name": 'Mensaje',
                "className": 'Mensaje'
            }
        ]
    });
}

$("#files").on('change', function() {
    var filenum = $('#files').get(0).files.length;
    if (filenum > 0) {
        var file = document.getElementById("files").files[0];
        var filename = file.name;
        if (evaluarExt(filename)) {
            mostrarAlerta("info", "B", 'Archivo: ' + filename);
            loadExcel(file);
            onclick = checkoff();
            sessionStorage.setItem("num", filenum);
            $('#files').val("");
        } else {
            document.getElementById("files").value = "";
            mostrarAlerta("info", "E", 'Tipo de archivo incorrecto: ' + filename );
            sessionStorage.setItem("num", filenum);
            $('#files').val("");
        }
    } else {
        mostrarAlerta("info", "E", 'Seleccione un archivo');
        var table = $('#table').DataTable();
        table.clear().draw();
    }
});

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

function loadExcel(file) {
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
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data === "NO VALIDO") {
                Limpiar();
                M.toast({
                    html: "Revisar número de columnas y filas. Máximo 2,000 Registros."
                });
                document.getElementById("loader").style.display = "none";
            } else if ((data !== null || data !== "") && !data.isRedirect) {
                $('#table').DataTable().destroy();
                $('#table tbody').html(data);
                $('#table').css("font-size", "12px");
                $('#table').css("display", "table");
                formato();
                document.getElementById("loader").style.display = "none";
            }
            else {
                window.location.href = data.redirectUrl;
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

function Carga() {
    var rowNum = $('#table>tbody').find('.input_bor').length;
    if (rowNum === 0) {
        return;
    }

    document.getElementById("loader").style.display = "flex";
    var message = $('.input_mes').serialize();
    var cliente = $('.input_cli').serialize();
    var us1 = $('.input_ni1').serialize();
    var us6 = $('.input_ni6').serialize();
    var doc = sessionStorage.getItem("num");
    if (doc > 0 || cliente !== null || cliente !== "cli=") {
        if (message === "" || message === null || message.endsWith('mes=')) {
            if (us1 !== "" && us1 !== "ni1=" && us1 !== null && us6 !== "" && us6 !== "ni6=" && us6 !== null) {

                var table = $('#table').DataTable();
                table.destroy();
                habilitar();
                var datos = $('#tabla').serializeArray();
                $.ajax({
                    type: "POST",
                    url: 'Agregar',
                    data: datos,
                    dataType: "json",
                    success: function (data) {
                        if (data.isRedirect) {
                            window.location.href = data.redirectUrl;
                        } else {
                            mostrarAlerta("info", "A", "Se agregaron los nuevos registros");
                            if (esFile) {
                                window.location = root + "Clientes";
                            } else {
                                Limpiar();
                                document.getElementById("loader").style.display = "none";
                            }
                        }
                    },
                    error: function (request, status, error) {
                        document.getElementById("loader").style.display = "none";
                        console.log(request.responseText);
                    },
                    async: true
                });
            }
            else {
                document.getElementById("loader").style.display = "none";
                mostrarAlerta("info", "E", "Los niveles 1 y 6 no pueden quedar vacios");
            }
        }
        else {
            document.getElementById("loader").style.display = "none";
            mostrarAlerta("info", "E", "Hay errores por corregir");
        }
    }
    else {
        document.getElementById("loader").style.display = "none";
        mostrarAlerta("info", "E", "Seleccione un archivo");
    }
}

function Comprobar(me) {
    var indx = (me.parentNode.parentNode.rowIndex-1);
    var i = indx * 20;
    habilitar();
    var datos = $('#tabla').serializeArray();
    var row = [
        datos[i + 1], datos[i + 2], datos[i + 3], datos[i + 4], datos[i + 5], datos[i + 6], datos[i + 7], datos[i + 8], datos[i + 9], datos[i + 10],
        datos[i + 11],datos[i + 12], datos[i + 13], datos[i + 14], datos[i + 15], datos[i + 16], datos[i + 17], datos[i + 18], datos[i + 19], datos[i + 20]];
    creart('Comprobar', row, indx);
}

function Borrar() {
    var rowNum = $('#table>tbody').find('.input_bor').length;
    var check = false;
    var porBorrar = [];
    $('#table>tbody').find('.input_bor').each(function (indx, checkInput) {
        if (checkInput.checked) {
            check = checkInput.checked;
            porBorrar.push({ value: indx});
        }
    });
    if (rowNum === 0 || !check) {
        return;
    }
    var table = $('#table').DataTable();
    for (var i = porBorrar.length; i > 0; i--) {
        var num = porBorrar[i-1].value;
        table.row(num).remove().draw();
    }
    rowNum = $('#table>tbody').find('.input_bor').length;
    if (rowNum >0) {
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
        + '<td><input class="input_cli" style="font-size:12px;" type="text" id="" name="cli" value="" onkeyup="if(event.keyCode == 13) Actualizar()"/></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
        + '<td></td>'
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

function Actualizar() {
    var datos = $('#tabla').serializeArray();
    if (datos.length > 0 && datos[1].value.length === 10) {
        esFile = false;
        habilitar();
        creart('Actualizar', datos);
    }
}

function creart(metodo, datos, indx) {
    document.getElementById("loader").style.display = "flex";
    $.ajax({
        type: "POST",
        url: metodo,
        data: datos,
        success: function (data) {
            if ((data !== null || data !== "") && !data.isRedirect) {
                var table = $('#table').DataTable();
               
                if (metodo === "Comprobar") {
                    $('#table tbody tr').get(indx).innerHTML = data;
                } else {
                    table.clear().draw();
                    $('#table').DataTable().destroy();
                    $('#table tbody').html(data);
                    $('#table').css("font-size", "12px");
                    $('#table').css("display", "table");
                    formato();
                }
                document.getElementById("loader").style.display = "none";
            }
            else {
                window.location.href = data.redirectUrl;
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
    if ($("#borrar").prop('checked') && $('#table>tbody').find('.input_bor').length>0) {
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

function habilitar() {
    $(".input_cli").prop('disabled', false);
    $(".input_noc").prop('disabled', false);
    $(".input_ban").prop('disabled', false);
    habi = true;
}
$('body').on('keydown.autocomplete', '.input_pai', function () {
    
    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root + 'Listas/Paises',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: item.LAND + " | " + item.LANDX, value: item.LAND };
                    }));
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item) && $(".input_pai").val() === "") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
            //var label = ui.item.label;
            //var value = ui.item.value;
        }
    });
});
$('body').on('keydown.autocomplete', '.input_ven', function () {

    //var tr = $(this).closest('tr'); //Obtener el row
    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root+'Listas/Vendors',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: item.ID + " | " + item.NOMBRE, value: item.ID };
                    }));
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item) && $(".input_ven").val() === "") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
            //var label = ui.item.label;
            //var value = ui.item.value;
        }
    });
});

$('body').on('keydown.autocomplete', '.input_can', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root+'Listas/Canales',
                dataType: "json",
                data: { "Prefix": request.term },
                success: function (data) {
                    response(auto.map(data, function (item) {
                        return { label: item.CANAL1 + " | " + item.CDESCRIPCION, value: item.CANAL1 };
                    }));
                }
            });
        },

        messages: {
            noResults: '',
            results: function (resultsCount) { }
        },

        change: function (e, ui) {
            if (!(ui.item) && $(".input_can").val() === "") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
        }
    });
});

$('body').on('keydown.autocomplete', '.input_cli', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root + 'Listas/Clientes',
                dataType: "json",
                data: { "Prefix": request.term },
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
            if (!(ui.item) && $(".input_cli").val()==="") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
        }
    });
});

$('body').on('keydown.autocomplete', '.input_coc', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root+'Listas/Sociedades',
                dataType: "json",
                data: { Prefix: request.term },
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
            if (!(ui.item) && $(".input_coc").val()==="") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
        }
    });
});

$('body').on('keydown.autocomplete', '.input_ni0', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root+'Listas/Usuarios',
                dataType: "json",
                data: { "Prefix": request.term, autorizador:2 },
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
            if (!(ui.item) && $(".input_ni0").val() === "") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
        }
    });
});

$('body').on('keydown.autocomplete', '.input_ni1', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root + 'Listas/Usuarios',
                dataType: "json",
                data: { "Prefix": request.term, autorizador: 2 },
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
            if (!(ui.item) && $(".input_ni1").val()==="") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
        }
    });
});

$('body').on('keydown.autocomplete', '.input_ni2', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root + 'Listas/Usuarios',
                dataType: "json",
                data: { "Prefix": request.term, autorizador: 2 },
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
            if (!(ui.item) && $(".input_ni2").val() === "") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
        }
    });
});

$('body').on('keydown.autocomplete', '.input_ni3', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root + 'Listas/Usuarios',
                dataType: "json",
                data: { "Prefix": request.term, autorizador: 2 },
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
            if (!(ui.item) && $(".input_ni3").val() === "") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
        }
    });
});

$('body').on('keydown.autocomplete', '.input_ni4', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root + 'Listas/Usuarios',
                dataType: "json",
                data: { "Prefix": request.term, autorizador: 2 },
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
            if (!(ui.item) && $(".input_ni4").val() === "") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
        }
    });
});

$('body').on('keydown.autocomplete', '.input_ni5', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "GET",
                url: root + 'Listas/Usuarios',
                dataType: "json",
                data: { "Prefix": request.term, autorizador: 2 },
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
            if (!(ui.item) && $(".input_ni5").val() === "") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
        }
    });
});

$('body').on('keydown.autocomplete', '.input_ni6', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'Usuario8',
                dataType: "json",
                data: { "Prefix": request.term },
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
            if (!(ui.item) && $(".input_ni6").val() === "") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
        }
    });
});

$('body').on('keydown.autocomplete', '.input_ni7', function () {

    auto(this).autocomplete({
        source: function (request, response) {
            auto.ajax({
                type: "POST",
                url: 'Usuario9',
                dataType: "json",
                data: { "Prefix": request.term },
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
            if (!(ui.item) && $(".input_ni7").val() === "") {
                e.target.value = "";
            }
        },

        select: function (event, ui) {
        }
    });
});


