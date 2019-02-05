function showRangos(table, tr) {
    var pos = parseInt(tr.find("td.POS").text().split("/")[0]);
    table.clear().draw(true);
    for (var i = 0; i < listaRangos.length; i++) {
        if (listaRangos[i].POS == pos) {
            addRowRan(table, listaRangos[i].POS, listaRangos[i].LIN, listaRangos[i].OBJ1, listaRangos[i].OBJ2, listaRangos[i].PORC);
        }
    }
}


function addRowRan(t, pos, lin, obj1, obj2, porc) {
    if (lin != 1) {
        addRowRanl(t,
            pos,
            lin,
            "<input type='text' style='font-size:12px;height:2rem;' value='" + toShow(obj1) + "' onblur='cambiaRango(this, \"o1\", " + pos + "," + lin + ", this.value)'/>",
            "<input type='text' style='font-size:12px;height:2rem;' value='" + toShow(obj2) + "' onblur='cambiaRango(this, \"o2\", " + pos + "," + lin + ", this.value)'/>",
            "<input type='text' style='font-size:12px;height:2rem;' value='" + toShowPorc(porc) + "' onblur='cambiaRango(this, \"p1\", " + pos + "," + lin + ", this.value)'/>"
            , ""
        );
    } else {
        addRowRanl(t,
            pos,
            lin,
            "<input type='text' style='font-size:12px;height:2rem;' value='" + toShow(obj1) + "' onblur='cambiaRango(this, \"o1\", " + pos + "," + lin + ", this.value)'/>",
            "<input type='text' style='font-size:12px;height:2rem;' value='" + toShow(obj2) + "' onblur='cambiaRango(this, \"o2\", " + pos + "," + lin + ", this.value)'/>",
            toShowPorc(porc)
            , "X"
        );
    }
}

function addRowRanl(t, pos, lin, obj1, obj2, porc, prim) {
    //var t = $('#table_rec').DataTable();

    t.row.add([
        //"",
        pos
        , lin
        , obj1
        //, obj2
        , porc
    ]).draw(false);
}

function cambiaRango(e, tipo, pos, lin, val) {
    //for (var i = 0; i < listaRangos.length; i++) {
    //    if (listaRangos[i].LIN == lin) {
    //      if (tipo == "p1") {
    //            listaRangos[i].PORC = toNum(val);
    //        }
    //    }
    //}
    val = toNum(val);
    if (lin > 1) {

        $('#table_rangos > tbody  > tr').each(function () {
            pos = parseInt($(this).find("td.POS").text());

            for (var i = 0; i < listaRangos.length; i++) {
                if (listaRangos[i].POS == pos & listaRangos[i].LIN == 1) {

                    if (tipo == "o1") {
                        if (parseFloat(toNum(listaRangos[i].OBJ1)) <= parseFloat(toNum(val))) {
                            val = 0;
                            e.classList.add("invalid");
                            e.classList.remove("valid");
                        } else {

                            e.classList.add("valid");
                            e.classList.remove("invalid");
                            for (var j = 0; j < listaRangos.length; j++) {
                                if (listaRangos[j].POS == pos & listaRangos[j].LIN == lin) {
                                    listaRangos[j].OBJ1 = toNum(val);
                                }
                            }
                        }
                        e.value = toShow(val);
                    }
                    if (tipo == "p1") {
                        if (parseFloat(toNum(listaRangos[i].PORC)) <= parseFloat(toNum(val))) {
                            val = 0;
                            e.classList.add("invalid");
                            e.classList.remove("valid");
                        } else {
                            e.classList.add("valid");
                            e.classList.remove("invalid");
                            for (var j = 0; j < listaRangos.length; j++) {
                                if (listaRangos[j].LIN == lin) {
                                    listaRangos[j].PORC = toNum(val);
                                }
                            }
                        }
                        e.value = toShowPorc(val);
                    }
                }
            }

            return false;
        });
    } else {
        for (var j = 0; j < listaRangos.length; j++) {
            if (listaRangos[j].POS == pos & listaRangos[j].LIN == lin) {
                if (tipo == "o1") {
                    listaRangos[j].OBJ1 = parseFloat(toNum(val));
                }
            }
        }
        e.value = toShow(val);
        if (isObjetivoQ())
            updateObjQ();
    }
}

function addRango() {
    var lin = 1;
    for (var i = 0; i < listaRangos.length; i++) {
        if (listaRangos[i].POS == 1)
            lin++;
    }
    var cont = 1;
    var tableR = $('#table_rangos').DataTable();

    $('#table_rec > tbody  > tr').each(function () {
        var o = { POS: cont, LIN: lin, PERIODO: 0, OBJ1: 0, OBJ2: 0, PORC: 0 };
        if (cont == 1)
            addRowRan(tableR, 0, o.LIN, 0, 0, 0);
        listaRangos.push(o);

        cont++;
    });
}


function enviaRan(borrador) { //B20180625 MGC 2018.07.03

    var lengthT = listaRangos.length;
    var tipo = document.getElementById("select_neg").value;
    var tipoR = document.getElementById("txt_trec").value;

    if (lengthT > 1) {
        var indext = 0;
        jsonObjDocs = [];
        var j = 1;

        var poss = 0;
        for (var i = 0; i < lengthT; i++) {

            var item = {};

            item["NUM_DOC"] = 0;
            item["POS"] = listaRangos[i].POS;
            item["LIN"] = listaRangos[i].LIN;
            item["OBJETIVOI"] = parseFloat(listaRangos[i].OBJ1);
            item["PORCENTAJE"] = parseFloat(toNum(listaRangos[i].PORC));

            jsonObjDocs.push(item);
            item = "";

        }

        docsenviar = JSON.stringify({ 'docs': jsonObjDocs });

        $.ajax({
            type: "POST",
            url: 'getPartialRan',
            contentType: "application/json; charset=UTF-8",
            data: docsenviar,
            success: function (data) {

                if (data !== null || data !== "") {

                    $("table#table_ranh tbody").append(data);
                    //if (borrador != "X") { //B20180625 MGC 2018.07.03
                    //    $('#delRow').click();
                    //}
                }

            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
    }

}

function delRango() {
    var t = $('#table_rangos').DataTable();
    var lin = listaRangos[listaRangos.length - 1].LIN;
    if (lin != 1 & lin != "1") {
        var tempList = [];
        for (var i = 0; i < listaRangos.length; i++) {
            if (listaRangos[i].LIN != lin) {
                tempList.push(listaRangos[i]);
            }
        }
        listaRangos = tempList;
    }

    $('#table_rangos > tbody  > tr').each(function () {
        var linn = $(this).find('td.LIN').text();
        if (linn != 1 & linn == lin) {
            $(this).addClass("selected");
        }
    });

    t.rows('.selected').remove().draw(false);
}


function updateObjQ() {
    $(".objqT").remove();
    var lengthT = $("table#table_rec tbody tr[role='row']").length;
    if (lengthT > 0) {
        var total = 0;
        var porc = 0;
        var fecha = "";
        var tsol = "";
        $("#table_rec > tbody  > tr[role='row']").each(function () {
            //total += parseFloat(toNum($(this).find("td.MONTO input").val()));
            //porc = toNum($(this).find("td.PORCENTAJE").text());
            fecha = ($(this).find("td.FECHA").text());
            tsol = $(this).find("td.TSOL").text();


            var pos = $(this).find("td.POS").text().split("/")[0];
            if (pos != "1") {
                var tr = "";
                tr += "<tr class='objqT'>"
                tr += "<td>Q " + pos + "</td>"
                tr += "<td>" + tsol + "</td>";
                tr += "<td>" + fecha + "</td>";
                total = 0;
                for (var i = 0; i < listaRangos.length; i++) {
                    var poss = listaRangos[i].LIN;
                    if (poss == 1 & pos == listaRangos[i].POS)
                        total += parseFloat(toNum(listaRangos[i].OBJ1));
                }
                tr += "<td>" + toShow(total) + "</td>";
                tr += "<td>" + toShowPorc($('#objPORC').val()) + "</td>";
                tr += "</tr>"
                $("#table_objQ tbody").append(tr);
            } else {
                total = 0;
                for (var i = 0; i < listaRangos.length; i++) {
                    var poss = listaRangos[i].LIN;
                    if (poss == 1 & pos == listaRangos[i].POS)
                        total += parseFloat(toNum(listaRangos[i].OBJ1));
                }
                document.getElementById("obqTSOL").innerText = tsol;
                document.getElementById("obqFECHA").innerText = fecha;
                document.getElementById("obqMONTO").innerText = toShow(total);
            }
        });
    }
    $("#table_objQ").DataTable();
}


function copiarTableVistaRan() {

    var lengthT = $("table#table_ranh tbody tr").length;
    var tipo = document.getElementById("select_neg").value;
    listaRangos = [];

    if (lengthT > 0) {
        //Obtener los valores de la tabla para agregarlos a la tabla de la vista en información
        //Se tiene que jugar con los index porque las columnas (ocultas) en vista son diferentes a las del plugin
        //$('#check_recurrente').trigger('change');
        var rowsn = 0;

        var tsol = "";
        var sol = $("#TSOL_ID").val();

        var i = 1;
        $('#table_ranh > tbody  > tr').each(function () {
            var pos = parseInt($(this).find("td:eq(1) input").val().trim());
            var lin = parseInt($(this).find("td:eq(2) input").val().trim());
            var porc = parseFloat($(this).find("td:eq(4) input").val().trim());
            var obj = parseFloat($(this).find("td:eq(3) input").val().trim());
            
            var o = { POS: pos, LIN: lin, PERIODO: 0, OBJ1: obj, OBJ2: 0, PORC: porc };
            listaRangos.push(o);
            $(this).remove();
        });
    }

}