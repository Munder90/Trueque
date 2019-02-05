function periodo() {
    var soc = $("#sociedad_id").val();
    var tsol = !$("#TSOL_ID").val() ? $("#tsol_id").val() : $("#TSOL_ID").val();
    var user = $("#USUARIOD_ID").val();
    $.ajax({
        type: "POST",
        url: root + 'Listas/periodo',
        dataType: "text",
        data: { sociedad_id: soc, tsol_id: tsol, usuario_id: user },

        success: function (data) {
            if (data === "0") {
                M.toast({
                    classes: "pWarnning",
                    displayLength: 1000000,
                    html: '<span style="padding-right:15px;"><i class="material-icons yellow-text">info</i></span> No existe periodo abierto '
                    + '<button class="btn-small btn-flat toast-action" onclick="dismiss(\'pWarnning\')">Aceptar</button>'
                });
                $("#PERIODO").val(null);
                $("#periodo").val(null);
            } else {
                $("#PERIODO").val(data);
                $("#periodo").val(data);
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });
}
function cierre() {
    //string sociedad_id, string tsol_id, int periodo_id, string usuario_id = null
    var soc = $("#sociedad_id").val();
    var tsol = !$("#TSOL_ID").val() ? $("#tsol_id").val() : $("#TSOL_ID").val();
    var periodo = $("#periodo").val() === "" ? 0 : $("#periodo").val();
    var user = $("#USUARIOD_ID").val();
    var bool = false;
    $.ajax({
        type: "POST",
        url: root + 'Listas/cierre',
        dataType: "text",
        data: { sociedad_id: soc, tsol_id: tsol, periodo_id: periodo, usuario_id: user },

        success: function (data) {
            if (data !== "X") {
                M.toast({
                    classes: "guardarWarnning",
                    displayLength: 1000000,
                    html: '<span style="padding-right:15px;"><i class="material-icons yellow-text">info</i></span> No existe periodo abierto '
                        + '<button class="btn-small btn-flat toast-action" onclick="dismiss(\'guardarWarnning\')">Aceptar</button>'
                });
                bool = true;
            } else {
                bool = false;
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });
    return bool;
}
function cuentas(num_doc, tsol_id, monto) {
        var categorias = [];
        if ($("#select_dis").val() === "C") {
            $("#table_dis > tbody  > tr[role='row']").each(function (indx, row) {
                var cat = row.cells.item(5).textContent;
                var _xxx = $.parseJSON($('#catmat').val());
                for (var i = 0; i < _xxx.length; i++) {
                    if (cat === _xxx[i].DESCRIPCION) {
                        categorias.push(_xxx[i].ID);
                    }
                }
            });
        } else {
            var categoriasAux = [];
            $("#table_dis > tbody  > tr[role='row']").each(function (indx, row) {
                var mat = row.cells.item(3).children[0].value,
                 cat = row.cells.item(4).textContent;
                if (categoriasAux.indexOf(cat) === -1) {
                    categorias.push(mat);
                    categoriasAux.push(cat);
                }
            });
        }

    var bool = false;
    clearing = null;
    var bukrs = $("#sociedad_id").val() ? $("#sociedad_id").val():$("#D_SOCIEDAD_ID").val(),
        land = $("#pais_id").val(),
        gall = $("#tall_id").val(),
        ejercicio = $("#ejercicio").val() ? $("#ejercicio").val():$("#D_EJERCICIO").val();
    $.ajax({
        type: "POST",
        url: root+'Listas/clearing',
        dataType: "json",
        data: { bukrs: bukrs, land: land, gall: gall, ejercicio: ejercicio, categorias: categorias, tsol_id: tsol_id, monto: monto, num_doc: num_doc },

        success: function (data) {
            if (data !== null && data !== "") {
                //data = {CARGO,NOMBREC,ABONO,NOMBREA,CLEARING,NOMBRECL,LIMITE,IMPUESTO} 12-12-2018
                clearing = data;
                bool = true;
            } else {
                M.toast({
                    classes: "cWarnning",
                    displayLength: 1000000,
                    html: '<span style="padding-right:15px;"><i class="material-icons yellow-text">info</i></span> Sin Cuentas relacionadas '
                    + '<button class="btn-small btn-flat toast-action" onclick="dismiss(\'cWarnning\')">Aceptar</button>'
                });
                bool = false;
            }

        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });
    if (!bool) {
        $("#btn_guardarBorr").addClass("disabled");
        $("#btn_guardarh").addClass("disabled");
    } else {
        $("#btn_guardarBorr").removeClass("disabled");
    }
}
function asignarPresupuesto(kunnr) {
    var periodo= $("#periodo").val();
    $.ajax({
        type: "POST",
        url: root+'Listas/getPresupuesto',
        dataType: "json",
        data: {
            kunnr: kunnr, periodo: periodo
        },
        success: function (data) {

            if (data !== null || data !== "") {
               
                //LEJ 09.07.18------------------------------------------
                var pcan = (data.P_CANAL / 1).toFixed(2);
                var pban = (data.P_BANNER / 1).toFixed(2);
                var pcc = (data.PC_C / 1).toFixed(2) * -1;
                var pca = (data.PC_A / 1).toFixed(2) * -1;
                var pcp = (data.PC_P / 1).toFixed(2) * -1;
                var pct = (data.PC_T / 1).toFixed(2) * -1;
                var consu = (data.CONSU / 1).toFixed(2);
                var _xdec = $("#dec").val();
                var _xm = $("#miles").val();
            
                $('#p_canal').text(toShowG(pcan.toString()));
                $('#p_banner').text(toShowG(pban.toString()));
                $('#pc_c').text(toShowG(pcc.toString()));
                $('#pc_a').text(toShowG(pca.toString()));
                $('#pc_p').text(toShowG(pcp.toString()));
                $('#pc_t').text(toShowG(pct.toString()));

                $('#consu').text(toShowG(consu.toString()));
            }
            //LEJ 09.07.18-----------------------------------------------
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: httpStatusMessage });
        },
        async: false
    });

}

function asignarSolicitud(num, num2,edit) {
    num = toNum(num);
    num2 = toNum(num2);
    var soc  = $("#sociedad_id").val() ? $("#sociedad_id").val() : $("#D_SOCIEDAD_ID").val();
    var tsol = !$("#TSOL_ID").val() ? $("#tsol_id").val() : $("#TSOL_ID").val();
    var land = $("#pais_id").val();
    var categorias = [];
    if ($("#select_dis").val() === "C") {
        $("#table_dis > tbody  > tr[role='row']").each(function (indx, row) {
            var cat = row.cells.item(5).textContent;
            var _xxx = $.parseJSON($('#catmat').val());
            for (var i = 0; i < _xxx.length; i++) {
                if (cat === _xxx[i].DESCRIPCION) {
                    categorias.push(_xxx[i].ID);
                }
            }
        });
    } else {
        var categoriasAux = [];
        $("#table_dis > tbody  > tr[role='row']").each(function (indx, row) {
            var mat = row.cells.item(3).children[0].value,
                cat = row.cells.item(4).textContent;
            if (categoriasAux.indexOf(cat) === -1) {
                categorias.push(mat);
                categoriasAux.push(cat);
            }
        });
    }
        $.ajax({
            type: "POST",
            url: 'getSolicitud',
            dataType: "json",
            data: { num: num, monto: num2, tsol_id: tsol, sociedad_id: soc, categorias: categorias, land: land, edit: edit },

            success: function (data) {

                if (data !== null || data !== "") {
                    if (data.S_NUM === "") {
                        var num2 = $('#monto_doc_md').val();
                        $('#s_montob').text(num2);
                        $('#s_montop').text("-");
                        $('#s_montoa').text("-");
                        $('#s_rema').text("-");
                        $('#s_impa').text("-");
                        if (data.S_IMPA !== "-"){
                            $('#s_impa').text(toShow(data.S_IMPA));
                        }
                        $('#s_impb').text("-");
                        $('#s_impc').text("-");
                        $('#s_ret').text("-");
                        $('#s_total').text(toShow(data.S_TOTAL));
                    } else if (edit) {
                        $('#s_montob').text(data.S_MONTOB);
                        $('#s_montop').text(data.S_MONTOP);
                        if (data.S_MONTOA.indexOf("(") > -1) {
                            document.getElementById("a3").classList.add("red");
                            document.getElementById("a3").classList.add("white-text");
                        }
                        $('#s_montoa').text(data.S_MONTOA);
                        if (data.S_REMA.indexOf("(")>-1){
                            document.getElementById("a4").classList.add("red");
                            document.getElementById("a4").classList.add("white-text");
                        }
                        $('#s_rema').text(data.S_REMA);
                        if (data.S_IMPA !== null && data.S_IMPA !== "-") {
                            $('#s_impa').text(toShow(data.S_IMPA));
                        } else {
                            $('#s_impa').text("-");
                        }
                        $('#s_impb').text("-");
                        $('#s_impc').text("-");
                        $('#s_ret').text("-");
                        $('#s_total').text(data.S_TOTAL);
                    }
                    else {
                        $('#s_montob').text(toShow(data.S_MONTOB));
                        $('#s_montop').text(toShow(data.S_MONTOP));
                        if (data.S_MONTOA !== "-") {
                            if (data.S_MONTOA < 0) {
                                $('#s_montoa').text(toShow(data.S_MONTOA));
                                document.getElementById("a3").classList.add("red");
                                document.getElementById("a3").classList.add("white-text");
                            }
                            else {
                                $('#s_montoa').text(toShow(data.S_MONTOA));
                                document.getElementById("a3").classList.remove("red");
                                document.getElementById("a3").classList.remove("white-text");
                            }
                        }
                        if (data.S_REMA !== "-") {
                            if (data.S_REMA < 0) {
                                $('#s_rema').text(toShow(data.S_REMA));
                                document.getElementById("a4").classList.add("red");
                                document.getElementById("a4").classList.add("white-text");
                            }
                            else {
                                $('#s_rema').text(toShow(data.S_REMA));
                                document.getElementById("a4").classList.remove("red");
                                document.getElementById("a4").classList.remove("white-text");
                            }
                        }
                        if (data.S_IMPA !== null && data.S_IMPA !== "-") {
                            $('#s_impa').text(toShow(data.S_IMPA));
                        } else {
                            $('#s_impa').text("-");
                        }
                        $('#s_impb').text(data.S_IMPB);
                        $('#s_impc').text(data.S_IMPC);
                        $('#s_ret').text(toShow(data.S_RET));
                        $('#s_total').text(toShow(data.S_TOTAL));
                    }
                }
            },
            error: function (xhr, httpStatusMessage, customErrorMessage) {
                M.toast({ html: httpStatusMessage });
            },
            async: false
        });
    
}

function cuentasPorTSol(tsol_id, monto, isReverso) {

    var tex_abono = document.querySelector(".lbl_cabono").innerHTML;
    var tex_cargo = document.querySelector(".lbl_ccargo").innerHTML;
    var tex_clearing = document.querySelector(".lbl_cclear").innerHTML;
    var tex_payer = document.querySelector(".lbl_cpayer").innerHTML;

    var prov = "PR",
        ncf = "NC",
        nci = "NCI";


    var div = "<ul class='collection'><li class='collection-item'>";
    if (isReverso) {
        monto = toShow(monto, $('#dec').val());
        div += "<div class='row' style='margin-bottom:0;'><div class='col s2'>" + tex_abono + "</div><div class='col s3'>" + clearing.CARGO + "</div><div class='col s4'>" + clearing.NOMBREC + "</div><div class='col s3 right-align'><span id='txt_clearing'>" + monto + "</span></div></div>";
        div += "</li><li class='collection-item'>";
        div += "<div class='row' style='margin-bottom:0;'><div class='col s2'>" + tex_cargo + "</div><div class='col s3'>" + clearing.ABONO + "</div><div class='col s4'>" + clearing.NOMBREA + "</div><div class='col s3 right-align'><span id='txt_clearing'>" + monto + "</span></div></div></div>";

    } else {
        if (tsol_id.indexOf(prov) === 0) {
            monto = toShow(monto, $('#dec').val());
            div += "<div class='row' style='margin-bottom:0;'><div class='col s2'>" + tex_abono + "</div><div class='col s3'>" + clearing.ABONO + "</div><div class='col s4'>" + clearing.NOMBREA + "</div><div class='col s3 right-align'><span id='txt_clearing'>" + monto + "</span></div></div>";
            div += "</li><li class='collection-item'>";
            div += "<div class='row' style='margin-bottom:0;'><div class='col s2'>" + tex_cargo + "</div><div class='col s3'>" + clearing.CARGO + "</div><div class='col s4'>" + clearing.NOMBREC + "</div><div class='col s3 right-align'><span id='txt_clearing'>" + monto + "</span></div></div></div>";

        } else if (tsol_id.indexOf(nci) === 0) {
            var kunnr = $("#payer_id").val(),
                nombrec = !$("#cli_name").val() ? $("#D_CLIENTE_NAME1").val() : $("#cli_name").val();
            var impuesto = toShow(clearing.IMPUESTO, $('#dec').val()),
                montot = toShow(monto + clearing.IMPUESTO, $('#dec').val());
            monto = toShow(monto);
            div += "<div class='row' style='margin-bottom:0;'><div class='col s2'>" + tex_payer + "</div><div class='col s3'>" + kunnr + "</div><div class='col s4'>" + nombrec + "</div><div class='col s3 right-align'>" + montot + "</div></div>";
            div += "</li><li class='collection-item'>";
            div += "<div class='row' style='margin-bottom:0;'><div class='col s2'>" + tex_cargo + "</div><div class='col s3'>" + clearing.CARGO + "</div><div class='col s4'>" + clearing.NOMBREC + "</div><div class='col s3 right-align'><span id='txt_clearing'>" + monto + "</span></div></div></div>";
            div += "</li><li class='collection-item'>";
            div += "<div class='row' style='margin-bottom:0;'><div class='col s2'>" + tex_clearing + "</div><div class='col s3'>" + clearing.CLEARING + "</div><div class='col s4'>" + clearing.NOMBRECL + "</div><div class='col s3 right-align'><span id='txt_clearing'>" + clearingV + "</span></div></div></div>";

        } else if (tsol_id.indexOf(ncf) === 0) {
            kunnr = $("#payer_id").val();
            nombrec = !$("#cli_name").val() ? $("#D_CLIENTE_NAME1").val() : $("#cli_name").val();

            monto += clearing.IMPUESTO;
            monto = toShow(monto);

            div += "<div class='row' style='margin-bottom:0;'><div class='col s2'>" + tex_payer + "</div><div class='col s3'>" + kunnr + "</div><div class='col s4'>" +nombrec + "</div><div class='col s3 right-align'>" + monto + "</div></div>";
            div += "</li><li class='collection-item'>";
            div += "<div class='row' style='margin-bottom:0;'><div class='col s2'>" + tex_cargo + "</div><div class='col s3'>" + clearing.CARGO + "</div><div class='col s4'>" + clearing.NOMBREC + "</div><div class='col s3 right-align'>" + monto + "</div></div></div>";

        }
        else {
            monto = toShow(monto);
            div += "<div class='row' style='margin-bottom:0;'><div class='col s2'>" + tex_abono + "</div><div class='col s3'>" + clearing.ABONO + "</div><div class='col s4'>" + clearing.NOMBREA + "</div><div class='col s3 right-align'>" + monto + "</div></div>";
            div += "</li><li class='collection-item'>";
            div += "<div class='row' style='margin-bottom:0;'><div class='col s2'>" + tex_cargo + "</div><div class='col s3'>" + clearing.CARGO + "</div><div class='col s4'>" + clearing.NOMBREC + "</div><div class='col s3 right-align'>" + monto + "</div></div></div>";

        }
    }
    div += "</li>";
    div += "</ul>";
    return div;


    //    if (clearing[2] != null) {
    //        if (monto <= clearing[5]) {

    //            div += "<div class='row' style='margin-bottom:0;'><div class='col s3'>" + tex_cargo + "</div><div class='col s3'>" + clearing[3] + "</div><div class='col s3'>" + clearing[2] + "</div><div class='col s3 right-align'>" + monto + "</div></div>";
    //            div += "</li><li class='collection-item'>";
    //            div += "<div class='row' style='margin-bottom:0;'><div class='col s4'>" + tex_clearing + "</div><div class='col s4'>" + clearing[4] + "</div><div class='col s4'>0.00</div></div>";
    //            div += "</li>";
    //        } else {
    //            div += "<div class='row' style='margin-bottom:0;'><div class='col s3'>" + tex_cargo + "</div><div class='col s3'>" + clearing[3] + "</div><div class='col s3'>" + clearing[2] + "</div><div class='col s3'>" + clearing[5] + "</div></div>";
    //            div += "</li><li class='collection-item'>";
    //            div += "<div class='row' style='margin-bottom:0;'><div class='col s4'>" + tex_clearing + "</div><div class='col s4'>" + clearing[4] + "</div><div class='col s4'>" + (monto - clearing[5]) + "</div></div>";
    //            div += "</li>";
    //        }
    //    }
    //}
}
