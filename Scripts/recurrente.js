function llenaCat(vkorg, vtweg, spart, kunnr) {
    var soc = document.getElementById("sociedad_id").value;
    $("#select_categoria").find('option').remove().end();
    $("#div_categoria").find('.select-dropdown.dropdown-trigger').addClass('ui-autocomplete-loading');
    $.ajax({
        type: "POST",
        url: root + 'Listas/categoriasCliente',
        dataType: "json",
        data: { vkorg: vkorg, spart: spart, kunnr: kunnr, soc_id: soc },
        success: function (data) {
            $("#select_categoria").find('option').remove().end();
            $("#div_categoria").find('.select-dropdown.dropdown-trigger').removeClass('ui-autocomplete-loading');
            for (var i = 0; i < data.length; i++) {
                var num = data[i].MATERIALGP_ID;
                var cat = data[i].TXT50;
                $("#select_categoria").append($("<option></option>")
                    .attr("value", num)
                    .text(cat));
            }
            var elem = document.getElementById("select_categoria");
            M.FormSelect.init(elem, []);
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: true
    });
}

function excedePresup() {
    var pres = toNum(document.getElementById("consu").innerHTML);
    var monto = toNum(document.getElementById("monto_doc_md").value);

    if (parseFloat(monto) > parseFloat(pres)) {
        document.getElementById("txtPres").value = 'X';
    }
}

$(document).ready(function () {
    var table = $('#table_rec').DataTable({
        "language": {
            "zerorecords": "no hay registros",
            "infoempty": "registros no disponibles",
            "decimal": ".",
            "thousands": ","
        },
        "paging": false,
        "info": false,
        "searching": false,
        "ordering": false,
        "columns": [
            {
                "name": 'POS',
                "className": 'POS'
            },
            {
                "name": 'PERIODO',
                "className": 'PERIODO'
            },
            {
                "name": 'TSOL',
                "className": 'TSOL'
            },
            {
                "name": 'NUM_DOC',
                "className": 'NUM_DOC'
            },
            {
                "name": 'FECHAV',
                "className": 'FECHAV'
            },
            {
                "name": 'MONTO',
                "className": 'MONTO'
            },
            {
                "name": 'PORCENTAJE',
                "className": 'PORCENTAJE'
            },
            {
                "name": 'ESTATUS',
                "className": 'ESTATUS'
            }
        ]
    });

    //LEJ 31.07.2018---------------------
    var _rec = $('#valRec').val();
    if (_rec !== undefined) {
        if (_rec > 0 || _rec !== '') {
            $('#check_recurrente').prop('checked', true);
            showPrTable();
        }
    }
    if ($('#chk_ligada').is(":checked")) {
        $("#div_objq").removeClass("hide");
        $("#txt_ligada").val("X");
    } else {
        $("#div_objq").addClass("hide");
        $("#txt_ligada").val("");
    }
    //LEJ 31.07.2018---------------------
    $('#table_rangos').DataTable({
        "language": {
            "zerorecords": "no hay registros",
            "infoempty": "registros no disponibles",
            "decimal": ".",
            "thousands": ","
        },
        "paging": false,
        ordering: false,
        order: [[1, "desc"]],
        "info": false,
        "searching": false,
        "columns": [
            {
                "name": 'POS',
                "className": 'POS'
            },
            {
                "name": 'LIN',
                "className": 'LIN'
            },
            {
                "name": 'OBJETIVO',
                "className": 'OBJETIVO'
            },
            {
                "name": 'PORCENTAJE',
                "className": 'PORCENTAJE'
            }
        ]
    });

    $("#table_objQ").DataTable({
        "paging": false,
        "ordering": false,
        "info": false,
        searching: false
    });

    $('#table_rec tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected') | listaRangos.length === 0) {
            $(this).removeClass('selected');
            $(".table_rangos").css("display", "none");
            $("#btnRango").css("display", "none");
            $("#btnDelRango").css("display", "none");
        }
        else {
            table.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
            $(".table_rangos").css("display", "table");
            $("#btnRango").css("display", "inline-block");
            $("#btnDelRango").css("display", "inline-block");
            var tableR = $('#table_rangos').DataTable();
            showRangos(tableR, $(this));
        }
    });

    if (!isDuplicado()) {
        cambiaCheckRec(false);
    }

    $('body').on('focusout', '#objPORC', function () {
        updateObjQ();
    });

});
//LEJ 31.07.2018---------------------
function showPrTable() {
    var _table = $('#table_rec').DataTable();
    _table.clear().draw(true);

    if ($('#check_recurrente').is(":checked")) {
        $('#table_rech > tbody  > tr').each(function () {
            var pos = $(this).find('td').eq(1).text();
            var fecha = $(this).find('td').eq(2).text().trim().split(' ');
            var mt = $(this).find('td').eq(3).text();
            var porc = $(this).find('td').eq(4).text();
            if ($('#miles').val() === ',') {
                var _mt = parseFloat(mt);
                var _porc = parseFloat(porc);
                fillTable(_table, pos, fecha[0], "$" + toShow(_mt), toShow(_porc));
            } else if ($('#miles').val() === '.') {
                mt = mt.toFixed(2).replace('.', ',');
                porc = porc.toFixed(2).replace('.', ',');
                fillTable(_table, pos, fecha[0], "$" + toShow(mt), toShow(porc));
            }
        });
        $(".table_rec").css("display", "table");

        if ($('#check_objetivoq').is(":checked")) {
            objetivoQ(true);
        }
    }
}
//LEJ 31.07.2018---------------------
function fillTable(t, no, fecha, mt, porc) {
    t.row.add([
        no, $("#TSOL_ID").val(), fecha,
        // mt,
        "<input class=\"numberd input_dc\" style=\"font-size:12px;height:2rem;\" type=\"text\" id=\"\" name=\"\" value=\"" + mt + "\" onchange='updateObjQ()'>",//LEJ 03.08.2018
        porc
    ]).draw(false).node();
}

var listaRangos = [];//RSG 17.09.2018

function cambiaRec() {
    $("#tabs_rec").addClass("disabled");
    var campo = document.getElementById("check_recurrente");
    listaRangos = [];//RSG 17.09.2018

    var radio = document.getElementById("btn-peri");
    var table = $('#table_rec').DataTable();
    table.clear().draw(true);
    var tipo = document.getElementById("select_neg").value + document.getElementById("select_dis").value;
    var montoo = toNum(document.getElementById("monto_dis").value);
    var tipoR = document.getElementById("txt_trec").value;//RSG 09.07.2018
    var porc = document.getElementById("bmonto_apoyo").value;//RSG 09.07.2018

    if (radio !== null) { //B20180625 MGC 2018.06.26 Marcaba error, por validación de null
        if (radio.checked & !isRelacionada()) {

            var pe1 = document.getElementById("periodoi_id").value;
            var pe2 = document.getElementById("periodof_id").value;
            var ej1 = document.getElementById("anioi_id").value;
            var ej2 = document.getElementById("aniof_id").value;
        }
    } else {

        //var pe1 = document.getElementById("periodoi_id").value;
        //var pe2 = document.getElementById("periodof_id").value;
        //var ej1 = document.getElementById("anioi_id").value;
        //var ej2 = document.getElementById("aniof_id").value;
    }
    if (campo.checked) {
        $("#tabs_rec").removeClass("disabled");
        if (montoo === "" | ligada()) {
            //var dist = $('#table_dis').DataTable();
            var montooo = 0.00;
            $('#table_dis > tbody  > tr').each(function () {
                var montot = $(this).find("td.total input").val();
                //montoo += parseInt(montot);
                //montooo += parseFloat(montot);//RSG 09.07.2018
                montooo += parseFloat(toNum(montot));
            });
            montoo = montooo;
        }
        if (montoo > 0 | ligada()) {
            //if (montoo > 0) { 
            $(".table_rec").css("display", "table");
            //Add row 
            ////var datei = document.getElementById("fechai_vig").value.split('/');
            ////var datef = document.getElementById("fechaf_vig").value.split('/');
            ////var dateii = new Date(datei[2], datei[1] - 1, datei[0]);
            ////var dateff = new Date(datef[2], datef[1] - 1, datef[0]);

            var anios = ej2 - ej1;
            ////var resdate = dateff - dateii;

            var meses = 1 + (pe2 - pe1) + anios * 12;
            if (meses > 1 & (montoo > 0 | ligada())) {
                for (var i = 1; i <= meses; i++) {
                    //var date = "";
                    var monto = "";
                    ////if (i === 1) {
                    ////if (tipo !== "P") {
                    if (tipoR !== "2" || tipoR === "2") {
                        if (tipo === "MM" | tipo === "MC") {
                            ////date = document.getElementById("fechai_vig").value;
                            monto = montoo;
                            //////addRowRec(table, i, date, monto, tipo);
                            //////primerDiaT(table, i, datei, monto, tipo);
                            primerDiaT(table, i, parseInt(pe1) + 1, ej1, monto, tipoR, meses);
                        } else if (ligada() & (tipo === "PM" | tipo === "PC")) {
                            monto = montoo;
                            ultimoDiaT(table, i, pe1, ej1, monto, tipoR, porc, meses);
                            listaRangos.push({ POS: i, LIN: 1, PERIODO: pe1, OBJ1: 0, OBJ2: 0, PORC: porc });
                        }
                    } else {
                        ////var dates = new Date(datei[2], datei[1] - 1 + i, 1);
                        //////date = date.addDays(-1);
                        ////dates.setDate(dates.getDate() - 1);
                        ////date = dates.getDate() + "/" + (dates.getMonth() + 1) + "/" + dates.getFullYear();
                        monto = montoo;
                        //////addRowRec(table, i, date, monto, tipo);
                        ////ultimoDiaT(table, i, datei, monto, tipo);
                        ultimoDiaT(table, i, pe1, ej1, monto, tipoR, porc, meses);
                    }
                    ////}
                    ////else {
                    ////    ////if (tipo !== "P") {
                    ////    if (tipoR !== "2") {
                    ////        if (tipo === "MM" | tipo === "MC") {
                    ////            //////var dates = new Date(datei[2], datei[1] - 2 + i, 1);
                    ////            //////date = dates.getDate() + "/" + (dates.getMonth() + 1) + "/" + dates.getFullYear();
                    ////            monto = montoo;
                    ////            //////primerDiaT(table, i, datei, monto, tipo);
                    ////            primerDiaT(table, i, parseInt(pe1) + 1, ej1, monto, tipoR, meses);
                    ////        } else if (ligada() & (tipo === "PM" | tipo === "PC")) {
                    ////            monto = montoo;
                    ////            ultimoDiaT(table, i, pe1, ej1, monto, tipoR, porc, meses);
                    ////            listaRangos.push({ POS: i, LIN: 1, PERIODO: pe1, OBJ1: 0, OBJ2: 0, PORC: porc });
                    ////        }
                    ////    } else {
                    ////        ////var dates = new Date(datei[2], datei[1] - 1 + i, 1);
                    ////        //////date = date.addDays(-1);
                    ////        ////dates.setDate(dates.getDate() - 1);
                    ////        ////date = dates.getDate() + "/" + (dates.getMonth() + 1) + "/" + dates.getFullYear();
                    ////        monto = montoo;
                    ////        //////addRowRec(table, i, date, monto, tipo);
                    ////        ////ultimoDiaT(table, i, datei, monto, tipo);
                    ////        ultimoDiaT(table, i, pe1, ej1, monto, tipoR, porc, meses);
                    ////    }
                    ////}
                }

            }
        }
        else {
            $(".table_rec").css("display", "none");
            //campo.checked = false;
        }
    } else {
        $(".table_rec").css("display", "none");
    }

    if (!ligada()) {
        $(".PORCENTAJE").css("display", "none");
    } else {
        $(".PORCENTAJE").css("display", "table-cell");
    }

    formaLiquida();
    $('#check_objetivoq').prop('checked', false);
}

function cambiaCheckRec(load) {
    if (load == undefined)
        load = false;
    var anioi = $('#anioi_id'), anioiV = anioi.val() * 1, aniof = $('#aniof_id'), aniofV = aniof.val() * 1,
        periodoi = $('#periodoi_id'), periodoiV = periodoi.val() * 1, periodof = $('#periodof_id'), periodofV = periodof.val() * 1;

    var campo = document.getElementById("check_recurrente");
    document.getElementById("btn-date").disabled = false;
    document.getElementById("btn-peri").disabled = false;

    if (campo.checked) {
        if (periodoiV === periodofV) {
            var periodo = periodoiV + 1 === 13 ? 1 : periodoiV + 1;
            if (periodo === 1 && anioiV === aniofV) {
                aniof.val(anioiV + 1);
                aniof.formSelect();
            }
            periodof.val(periodo);
            periodof.formSelect();
        }
        document.getElementById("btn-peri").checked = true;
        document.getElementById("btn-date").disabled = true;
        document.getElementById("btn-peri").disabled = true;
        if (!load)
            $("#btn-peri").trigger("click");

    } else {
        document.getElementById("btn-date").disabled = false;
        document.getElementById("btn-peri").disabled = false;
        $("#tabs_rec").addClass("disabled");

    }
    if (campo.checked) {
        if (!ligada()) {
            $("#select_neg").val("M");
            $("#select_negi").val("M");
        }
        $("#select_neg").prop("disabled", "disabled");
        $("#select_neg").formSelect();
        if (!load)
            $("#select_neg").change();
    } else {
        $("#select_neg").prop("disabled", false);
        if (!load)
            $("#select_neg").change();
    }
    var elem = document.getElementById('select_neg');
    M.FormSelect.init(elem, []);

}

function addRowRec(t, num, date, monto, tipo, porc, periodo, meses) {
    //<<<<<<< 20181206-RSG
    //var tsol = document.getElementsByClassName("k-state-selected");
    //var tsoll = "";
    //if (tsol.length > 0)
    //    tsoll = [0].innerText;
    //else
    //    tsoll = document.getElementById("tsol_idi").value;
    //=======
    var tsoll = ($("#tsol_idi").val() ? $("#tsol_idi").val() : $("#tsol_id").val());

    var m = "";
    var p = "";
    if (tipo !== "2") {
        if (!ligada()) {
            m = toShow(monto);
            p = toShowPorc(0.00);
        } else {
            m = toShow(monto);
            p = toShowPorc(porc);
        }
    } else {
        p = toShowPorc(porc);
        //if (num !== 1) {
        //    m = "<input class=\"MONTO input_rec numberd input_dc monto \" style=\"font-size:12px;height:2rem;\" type=\"text\" id=\"\" name=\"\" value=\"" + toShow(monto) + "\" onchange='updateObjQ()'>";
        //} else {
        ////m = "<input class=\"MONTO input_rec numberd input_dc monto \" style=\"font-size:12px;height:2rem;\" type=\"text\" id=\"\" name=\"\" value=\"" + toShow(monto) + "\" onchange='updateObjQ()'>";
        //}
        m = toShow(monto);
    }

    addRowRecl(
        t,
        num + "/" + meses, //POS
        tsoll,
        date,
        m,
        p
        , periodo
    );

    ////if (tipo !== "2") {
    ////    if (!ligada()) {
    ////        addRowRecl(
    ////            t,
    ////            num + "/" + meses, //POS
    ////            tsoll,
    ////            date,
    ////            toShow(monto),
    ////            toShowPorc(0.00)
    ////            , periodo
    ////        );
    ////    } else {
    ////        addRowRecl(
    ////            t,
    ////            num + "/" + meses, //POS
    ////            tsoll,
    ////            date,
    ////            toShow(monto),
    ////            toShowPorc(porc)
    ////            , periodo
    ////        );
    ////    }
    ////} else {
    ////    if (num !== 1) {
    ////        addRowRecl(
    ////            t,
    ////            num + "/" + meses, //POS
    ////            tsoll,
    ////            date,
    ////            "<input class=\"MONTO input_rec numberd input_dc monto \" style=\"font-size:12px;height:2rem;\" type=\"text\" id=\"\" name=\"\" value=\"" + toShow(monto) + "\" onchange='updateObjQ()'>",
    ////            toShowPorc(porc)
    ////            , periodo
    ////        );
    ////    } else {
    ////        addRowRecl(
    ////            t,
    ////            //num, //POS
    ////            num + "/" + meses, //POS
    ////            tsoll,
    ////            date,
    ////            "<input class=\"MONTO input_rec numberd input_dc monto \" style=\"font-size:12px;height:2rem;\" type=\"text\" id=\"\" name=\"\" value=\"" + toShow(monto) + "\" onchange='updateObjQ()'>",
    ////            toShowPorc(porc)
    ////            , periodo
    ////        );
    ////    }
    ////}
}

function addRowRecl(t, pos, tsol, fecha, monto, porc, periodo) {
    //var t = $('#table_rec').DataTable();

    t.row.add([
        pos
        , periodo
        , tsol
        , "No generado"
        , "<span style='display:none;'>" + fecha + "</span>" + "No generado"
        , monto
        , porc
        , "Pendiente"
    ]).draw(false);
}

function enviaRec(borrador) { //B20180625 MGC 2018.07.03

    var lengthT = $("table#table_rec tbody tr[role='row']").length;
    var tipo = document.getElementById("select_neg").value;
    //var tipoR = document.getElementById("txt_trec").value;

    var jsonObjDocs = [];

    if (lengthT > 0) {
        //var i = 1;
        //var sol = $("#TSOL_ID").val();
        //var mostrar = isFactura(sol);
        //if (mostrar) {
        //    vol = "real";
        //} else {
        //    vol = "estimado";
        //}

        var poss = 0;
        $("#table_rec > tbody  > tr[role='row']").each(function () { //B20180625 MGC 2018.07.03
            poss++;

            var pos = $(this).find("td.POS").text().split("/")[0];
            var tsol = $(this).find("td.TSOL").text();
            var fecha = $(this).find("td.FECHAV span").text();
            var monto = ""; var porcentaje = "";
            if (tipo === "P") {
                if (ligada()) {
                    monto = toNum($(this).find("td.MONTO input").val());
                    porcentaje = toNum($(this).find("td.PORCENTAJE").text());
                } else {
                    //if (poss === 1) {
                    //    monto = toNum($(this).find("td.MONTO").text());
                    //} else {
                    //    //monto = toNum($(this).find("td.MONTO input").val());
                    monto = toNum($(this).find("td.MONTO").text());
                    //}
                    porcentaje = toNum($(this).find("td.PORCENTAJE input").val());
                }
            } else {
                monto = toNum($(this).find("td.MONTO").text());
                porcentaje = toNum($(this).find("td.PORCENTAJE").text());
            }


            ////Obtener el id de la categoría            
            //var t = $('#table_rec').DataTable();
            //var tr = $(this);

            var item = {};

            item["NUM_DOC"] = 0;
            item["POS"] = pos;
            item["TSOL"] = tsol;
            //item["FECHAF"] = fecha + " 12:00:00 p.m.";
            var horaServer = $("#horaServer").val();
            item["FECHAF"] = fecha + " " + horaServer;

            item["MONTO_BASE"] = monto;
            item["PORC"] = porcentaje;

            jsonObjDocs.push(item);
            //i++;
            //item = "";

            if (borrador !== "X") { //B20180625 MGC 2018.07.03
                $(this).addClass('selected');
            }

        });

        var docsenviar = JSON.stringify({ 'docs': jsonObjDocs });

        $.ajax({
            type: "POST",
            url: 'getPartialRec',
            contentType: "application/json; charset=UTF-8",
            data: docsenviar,
            success: function (data) {
                if (data !== null /*|| data !== ""*/) {

                    $("table#table_rech tbody").append(data);
                    if (borrador !== "X") { //B20180625 MGC 2018.07.03
                        $('#delRow').click();
                    }
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
    }

}

function copiarTableVistaRec() {

    var lengthT = $("table#table_rech tbody tr").length;
    //var tipo = document.getElementById("select_neg").value;

    if (lengthT > 0) {
        if (lengthT > 1)
            document.getElementById("check_recurrente").checked = true;
        $(".table_rec").css("display", "table");
        ////var rowsn = 0;

        ////var tsol = "";
        ////var sol = $("#TSOL_ID").val();
        var sol = ($("#tsol_idi").val() ? $("#tsol_idi").val() : $("#tsol_id").val());
        var tipoR = document.getElementById("txt_trec").value;//RSG 09.07.2018

        //var i = 1;
        $('#table_rech > tbody  > tr').each(function () {

            //var pos = $(this).find("td.POS").text();
            var pos = $(this).find("td:eq(1)").text();
            //var fecha = $(this).find("td.FECHAV").text();
            var fecha = $(this).find("td:eq(2)").text().trim();

            var ffecha = fecha.split(' ');

            //var prov = $(this).find("td.PROVEEDOR").text();
            var monto = "";
            if (tipoR !== "1") {
                monto = $(this).find("td:eq(3)").text();
            } else {
                monto = toNum($("#MONTO_DOC_MD").val());
            }
            //var control = $(this).find("td.CONTROL").text();
            var porc = $(this).find("td:eq(4)").text();
            var per = $(this).find("td:eq(11)").text();

            monto = monto.trim();
            porc = porc.trim();
            ////}
            var t = $('#table_rec').DataTable();

            ////addRowRecl(t, pos.trim(), sol.trim(), ffecha[0], toShow(monto), toShowPorc(porc), per);
            addRowRec(t, pos.trim(), ffecha[0], monto, tipoR, "", "P" + per.trim() + "-" + ffecha[0].split('/')[2], lengthT);
            if (tipoR !== "1") {
                var o = { POS: parseInt(pos.trim()), LIN: 1, PERIODO: per, OBJ1: monto, OBJ2: 0, PORC: porc };
                listaRangos.push(o);
            }

            //Quitar el row
            $(this).remove();
        });
        ////Hide columns
        //ocultarColumnasTablaSoporteDatos();
        //$('.input_sop_f').trigger('focusout');
    }

    if (!ligada()) {
        $(".PORCENTAJE").css("display", "none");
    } else {
        $(".PORCENTAJE").css("display", "table-cell");
    }
}

//function primerDiaT(t, num, date, monto, tipo) {
function primerDiaT(t, num, periodo, ejercicio, monto, tipo, meses) {
    document.getElementById("loader").style.display = "initial";


    $.ajax({
        type: "POST",
        //url: '../Listas/getPrimerDia',
        url: '../Listas/getPrimerLunes',
        dataType: "json",
        data: { ejercicio: ejercicio, periodo: periodo - 1 + num },
        success: function (data) {
            document.getElementById("loader").style.display = "none";
            var dd = data.split('/');
            var dates = new Date(dd[2], dd[1] - 1, dd[0]);
            var datee = dates.getDate() + "/" + (dates.getMonth() + 1) + "/" + dates.getFullYear();
            if (periodo - 2 + num > 12) {
                ejercicio = parseInt(ejercicio) + Math.floor((periodo - 2 + num) / 12);
                periodo = periodo - 2 + num - 12;
            } else
                periodo = periodo - 2 + num;
            addRowRec(t, num, datee, monto, tipo, "", "P" + periodo + "-" + ejercicio, meses);
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
            document.getElementById("loader").style.display = "none";
        },
        async: false
    });
}

//function ultimoDiaT(t, num, date, monto, tipo) {
function ultimoDiaT(t, num, periodo, ejercicio, monto, tipo, porc, meses) {
    document.getElementById("loader").style.display = "initial";


    $.ajax({
        type: "POST",
        url: '../Listas/getUltimoDia',
        //url: '../Listas/getPrimerLunes',
        dataType: "json",
        data: { ejercicio: ejercicio, periodo: periodo - 1 + num },
        //data: { ejercicio: ejercicio, periodo: (periodo - 1 + 1 + num) },
        success: function (data) {
            document.getElementById("loader").style.display = "none";
            var dd = data.split('/');
            var dates = new Date(dd[2], dd[1] - 1, dd[0]);
            var datee = dates.getDate() + "/" + (dates.getMonth() + 1) + "/" + dates.getFullYear();
            if (periodo - 1 + num > 12) {
                ejercicio = parseInt(ejercicio) + Math.floor((periodo - 1 + num) / 12);
                periodo = periodo - 1 + num - 12;
            } else
                periodo = periodo - 1 + num;

            addRowRec(t, num, datee, monto, tipo, porc, "P" + periodo + "-" + ejercicio, meses);
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
            document.getElementById("loader").style.display = "none";
        },
        async: false
    });
}

function setDates(tipo) {

    var fechaIa = document.getElementById('fechai_vig').value;
    var fechaFa = document.getElementById('fechaf_vig').value;

    if (tipo === "date") {
        ////var fechI = document.getElementById("fechai_vig2"),
        ////    fechF = document.getElementById("fechaf_vig2");

        ////fechI.value = document.getElementById('fechai_vig').value;
        ////fechF.value = document.getElementById('fechaf_vig').value;

        ////var instanceI = M.Datepicker.getInstance(fechI);
        ////var dI = fechI.value.split('/');
        ////var datesI = new Date(dI[2], dI[1] - 1, dI[0]);
        ////instanceI.setDate(datesI);
        ////var instanceF = M.Datepicker.getInstance(fechF);
        ////var dF = fechF.value.split('/');
        ////var datesF = new Date(dF[2], dF[1] - 1, dF[0]);
        ////instanceF.setDate(datesF);

        document.getElementById("lbl_fechade").setAttribute('class', 'active');
        document.getElementById("lbl_fechahasta").setAttribute('class', 'active');

        pickerFecha2(".format_date");
    } else {

        var anioi = document.getElementById('anioi_id').value,
            aniof = document.getElementById('aniof_id').value,
            periodoi = document.getElementById('periodoi_id').value,
            periodof = document.getElementById('periodof_id').value;
        if (anioi * 1 > aniof * 1) {
            var af = $("#aniof_id");
            af.val("");
            af.formSelect();
            var fechaf_vig = $("#fechaf_vig");
            fechaf_vig.val("");
            M.toast({
                classes: "pFechas",
                displayLength: 1000000,
                html: '<span style="padding-right:15px;"><i class="material-icons yellow-text">info</i></span> Los años no tienen una secuencia correcta.'
                    + '<button class="btn-small btn-flat toast-action" onclick="dismiss(\'pFechas\')">Aceptar</button>'
            });
            return;
        }
        if (periodoi == "") {
            periodoi = $("#fechai_vig").val().split('/')[1];
            document.getElementById('periodoi_id').value = periodoi;
        }
        if (periodof == "") {
            periodof = $("#fechaf_vig").val().split('/')[1];
            document.getElementById('periodof_id').value = periodof;
        }
        if (periodoi * 1 > periodof * 1 && anioi * 1 === aniof * 1) {
            var pf = $("#periodof_id");
            pf.val("");
            pf.formSelect();
            fechaf_vig = $("#fechaf_vig");
            fechaf_vig.val("");
            M.toast({
                classes: "pFechas",
                displayLength: 1000000,
                html: '<span style="padding-right:15px;"><i class="material-icons yellow-text">info</i></span> Los meses no tienen una secuencia correcta.'
                    + '<button class="btn-small btn-flat toast-action" onclick="dismiss(\'pFechas\')">Aceptar</button>'
            });
            return;
        }
        if (anioi && periodoi) {
            $.ajax({
                type: "POST",
                url: root + 'Listas/getPrimerDia',
                dataType: "json",
                data: { ejercicio: anioi, periodo: periodoi },
                success: function (data) {
                    var dd = data.split('/');
                    var dates = new Date(dd[2], dd[1] - 1, dd[0]);
                    var datee = dates.getDate() + "/" + (dates.getMonth() + 1) + "/" + dates.getFullYear();

                    document.getElementById('fechai_vig').value = datee;
                },
                error: function (xhr, httpStatusMessage, customErrorMessage) {
                    M.toast({ html: httpStatusMessage });
                },
                async: false
            });
        }
        if (aniof && periodof) {
            $.ajax({
                type: "POST",
                url: root + 'Listas/getUltimoDia',
                dataType: "json",
                data: { ejercicio: aniof, periodo: periodof },
                success: function (data) {
                    var dd = data.split('/');
                    var dates = new Date(dd[2], dd[1] - 1, dd[0]);
                    var datee = dates.getDate() + "/" + (dates.getMonth() + 1) + "/" + dates.getFullYear();

                    document.getElementById('fechaf_vig').value = datee;
                },
                error: function (xhr, httpStatusMessage, customErrorMessage) {
                    M.toast({ html: httpStatusMessage });
                },
                async: false
            });
        }
    }
    var fechI = document.getElementById("fechai_vig2"),
        fechF = document.getElementById("fechaf_vig2");

    fechI.value = document.getElementById('fechai_vig').value;
    fechF.value = document.getElementById('fechaf_vig').value;

    var instanceI = M.Datepicker.getInstance(fechI);
    var dI = fechI.value.split('/');
    var datesI = new Date(dI[2], dI[1] - 1, dI[0]);
    instanceI.setDate(datesI);
    var instanceF = M.Datepicker.getInstance(fechF);
    var dF = fechF.value.split('/');
    var datesF = new Date(dF[2], dF[1] - 1, dF[0]);
    instanceF.setDate(datesF);


    var fechaIb = document.getElementById('fechai_vig').value;
    var fechaFb = document.getElementById('fechaf_vig').value;
    if (fechaIa != fechaIb || fechaFa != fechaFb)
        if (mesesMult(3, periodoi, anioi, periodof, aniof))
            cambiaRec();
}

//Evaluar la extensión y tamaño del archivo a cargar
function changeFile(campo) {
    var length = $(campo).length;
    var message = "";
    var namefile = "";
    if (length > 0) {
        //Validar tamaño y extensión
        var file = $(campo).get(0).files;
        if (file.length > 0) {
            var sizefile = file[0].size;
            namefile = file[0].name;
            if (sizefile > 20971520) {
                message = 'Error! Tamaño máximo del archivo 20 M --> Archivo ' + namefile + " sobrepasa el tamaño";

            }

            if (!evaluarExtSoporte(namefile)) {
                //message = "Error! Tipos de archivos aceptados 'xlsx', 'doc', 'pdf', 'png', 'msg', 'zip', 'jpg', 'docs' --> Archivo " + namefile + " no es compatible";
                message = "Error! Tipos de archivos rechazados 'exe', 'bak' --> Archivo " + namefile + " no es compatible";

            }
        }
    } else {
        message = "No selecciono archivo";
    }

    if (message !== "") {
        $(campo).val("");
        M.toast({ html: message });
    } else {
        //Verificar los nombres
        var id = $(campo).attr('id');
        var res = evaluarFilesName(id, namefile);

        if (res) {
            //Nombre duplicado
            M.toast({ html: 'Ya existe un archivo con ese mismo nombre' });
        } else {
            if (campo.id == "file_rev") {
                $("#fileinput_rev").removeClass("invalid");
                $("#fileinput_rev").addClass("valid");
            }
        }
    }
}

function ligada() {
    return $("#chk_ligada").is(":checked");
}

function isObjetivoQ() {
    return $("#check_objetivoq").is(":checked");
}

function isRecurrente() {
    return $("#check_recurrente").is(":checked");
}

function isDuplicado() {
    return $("#duplicate").val() !== "";
}
function isMultiple() {
    return $("#check_facturas").val() === "true";
}
function isCaso1() {
    return $("#txt_caso1").val() === "X";
}

var liquida = [];
function formaLiquida() {
    var mensual = true;
    var trimestral = false;
    var semestral = false;
    var anual = false;
    var periodoi = parseInt($("#periodoi_id").val());
    var periodof = parseInt($("#periodof_id").val());
    var anioi = parseInt($("#anioi_id").val());
    var aniof = parseInt($("#aniof_id").val());
    if (!(periodoi === periodof && anioi == aniof)) {
        var resta = 0;
        if (aniof - anioi == 0)
            resta = periodof - periodoi + 1;
        else {
            resta = 13 - periodoi + periodof;
            resta += 12 * ((aniof - anioi) - 1)
        }
        trimestral = resta % 3 == 0;
        semestral = resta % 6 == 0;
        anual = resta % 12 == 0;
    }
    $("#sel_nn").find('option').remove().end();
    ////$("#div_categoria").find('.select-dropdown.dropdown-trigger').removeClass('ui-autocomplete-loading');
    var val = "";
    var label = "";
    if (mensual) {
        val = 1;
        label = "Mensual";
        $("#sel_nn").append($("<option></option>")
            .attr("value", val)
            .text(label));
    }
    if (trimestral) {
        val = 3;
        label = "Trimestral";
        $("#sel_nn").append($("<option></option>")
            .attr("value", val)
            .text(label));
    }
    if (semestral) {
        val = 6;
        label = "Semestral";
        $("#sel_nn").append($("<option></option>")
            .attr("value", val)
            .text(label));
    }
    if (anual) {
        val = 12;
        label = "Anual";
        $("#sel_nn").append($("<option></option>")
            .attr("value", val)
            .text(label));
    }
    var elem = document.getElementById("sel_nn");
    M.FormSelect.init(elem, []);
}

function mesesMult(num, p1, a1, p2, a2) {
    if (!isRecurrente())
        return false;
    var mult = true;
    ////var per1 = parseInt(p1);
    ////var ani1 = parseInt(a1);
    ////var per2 = parseInt(p2);
    ////var ani2 = parseInt(a2);

    ////var anios = ani2 - ani1;
    ////var meses = 1 + (per2 - per1) + anios * 12;
    ////mult = meses % num === 0;

    ////if (!mult)
    ////    toast("perR", 100000, "error", "Cambiar periodos", "yellow");
    ////else
    ////    dismiss("perR")

    return mult;
}