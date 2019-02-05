
$(document).ready(function () {
    $('#btn_guardarBorr').on("click", function (e) {
        document.getElementById("loader").style.display = "flex";
        var _miles = $("#miles").val(); //LEJ 09.07.18
        var _decimales = $("#dec").val(); //LEJ 09.07.18
        var msg = 'Verificar valores en los campos: ';
        var campos = '';
        var res = true;
        var elem = document.querySelector(".tabs");
        var instance = M.Tabs.getInstance(elem);
        var index = instance.index;


        for (var i = 0; i < lista.length; i++) {
            if (lista[i].ID === "tsol_id") {
                var tsol = $("#TSOL_ID").val();
                if (!warning(tsol, "!=", "")) {
                    toast(lista[i].ID, 1000000, "error", lista[i].MSG, "red");
                    res = false;
                }
            }
            if (lista[i].ID === "payer_id") {
                var payer = $("#payer_id").val();
                if (!warning(payer, "!=", "")) {
                    toast(lista[i].ID, 1000000, "error", lista[i].MSG, "red");
                    res = false;
                }
            }
            if (lista[i].ID === "tall_id") {
                var tall = $("#tall_id").val();
                if (!warning(tall, "!=", "")) {
                    toast(lista[i].ID, 1000000, "error", lista[i].MSG, "red");
                    res = false;
                }
            }
        }


        //Evaluar TabInfo values
        if (index > 0) {
            var InfoTab = evalInfoTab(true, e);
            if (!InfoTab) {
                campos += 'Información';
                res = InfoTab;
            }
        }

        if (index > 0) {
            //Evaluar SoporteTab values
            var SoporteTab = evalSoporteTab(true, e);
            if (!SoporteTab) {
                campos += (campos === '' ? '' : ',') + ' Soporte';
                res = SoporteTab;
            }
        }
        if (index > 2) {
            //Evaluar DistribucionTab values
            var DistTab = evalDistribucionTab(true, e);
            if (!DistTab) {
                campos += (campos === '' ? '' : ',') + ' Distribución';
                res = DistTab;
            }
        }
        if (index > 3) {
            //Evaluar FinTab values
            var FinancieraTab = evalFinancieraTab(true, e);
            if (!FinancieraTab) {
                campos += (campos === '' ? '' : ',') + ' Financiera';
                res = FinancieraTab;
            }
            //jemo inicio 24-07-2018
            //validacion de importe de facturas contra monto de distribucion
            var checkf = $('#check_factura').is(':checked');
            if (checkf) {
                var monto = parseFloat(toNum($('#monto_dis').val()));
                if (importe_fac !== monto) {
                    campos += (campos === '' ? '' : ',') +' Importe total de las facturas sea igual al monto en Distribución';
                    res = false;
                }
            }
        }
        if (index < 4) {
            $('#monto_doc_md').val($("#monto_dis").val());
        }
        msg += campos;
        msg += '!';
        if (res) {
            //loadFilesf();
            //LEJ 10.07.18--------------------------------------------------
            //Provisional
            //var tipo_cambio = $('#tipo_cambio').val().replace('$', '');
            var tipo_cambio = toNum($('#tipo_cambio').val());
            //if (_decimales === '.') {
            //    tipo_cambio = tipo_cambio.replace(',', '');
            //}
            //else if (_decimales === ',') {
            //    var tc = tipo_cambio.replace('.', '');
            //    tc = tc.replace(',', '.');
            //    tipo_cambio = tc;
            //}
            //LEJ 10.07.18--------------------------------------------------
            //Para que el controlador no tenga problema
            $('#tipo_cambio').val(tipo_cambio);
            ////var tipo_cambio = $('#tipo_cambio').val();
            //var iNum = parseFloat(tipo_cambio.replace(',', '.')).toFixed(2);
            var iNum = parseFloat(tipo_cambio);

            if (iNum > 0) {
                //var num = "" + iNum;
                //num = num.replace('.', ',');
                //var numexp = num;//* 60000000000;
                //$('#tipo_cambio').val(numexp);
            } else {
                $('#tipo_cambio').val(0);
            }
            //var tipo_cambio = $('#monto_doc_ml2').val();
            //LEJ 10.07.18---------------------------------------------------
            //var tipo_cambiod = $('#monto_doc_ml2').val().replace('$', '');
            var tipo_cambiod = toNum($('#monto_doc_ml2').val());
            //if (_decimales === '.') {
            //    tipo_cambiod = tipo_cambiod.replace(',', '');
            //}
            //else if (_decimales === ',') {
            //    var tc = tipo_cambiod.replace('.', '');
            //    tc = tc.replace(',', '.');
            //    tipo_cambiod = tc;
            //}
            //LEJ 10.07.18--------------------------------------------------
            //Para que el controlador no tenga problema
            $('#monto_doc_ml2').val(tipo_cambiod);

            //var iNum2 = parseFloat(tipo_cambio.replace(',', '.')).toFixed(2);
            var iNum2 = parseFloat(toNum(tipo_cambio));
            //var iNum2 = parseFloat(tipo_cambio.replace('.', ','));
            if (iNum2 > 0) {
                //var nums = "" + iNum2;
                //nums = nums.replace('.', ',');
                //var numexp2 = nums;// * 60000000000;
                //$('#monto_doc_ml2').val(numexp2);
            } else {
                $('#monto_doc_ml2').val(0);
            }

            $('#monto_doc_ml2').val(toNum($('#montos_doc_ml2').val()));

            //Monto
            monto = $('#monto_doc_md').val();
            //var numm = parseFloat(monto.replace(',', '.')).toFixed(2);   
            //var numm = parseFloat(monto.replace(',', ''));
            var numm = parseFloat(toNum(monto));
            if (numm > 0) {
                $('#MONTO_DOC_MD').val(numm);
            } else {
                $('#MONTO_DOC_MD').val(0);
                $('#monto_doc_md').val(0);
            }
            //bmonto_apoyo
            $('#bmonto_apoyo').val(toNum($('#bmonto_apoyo').val()));

            //objq
            $('#objPORC').val(toNum($('#objPORC').val()));//RSG 01.08.2018


            $('#select_negi').prop('disabled', false); //B20180618 v1 MGC 2018.06.18
            $('#select_disi').prop('disabled', false); //B20180618 v1 MGC 2018.06.18

            //Guardar los valores de la tabla en el modelo para enviarlos al controlador
            copiarTableControl("");//Distribución //B20180625 MGC 2018.07.03
            copiarSopTableControl(""); //Soporte ahora en información //B20180625 MGC 2018.07.03
            enviaRec("");//RSG 28.05.2018 //B20180625 MGC 2018.07.03
            enviaRan();//RSG 26.09.2018
            excedePresup();

            //B20180625 MGC2 2018.07.04
            //enviar borrador
            var borrador = "false";
            if ($("#borradore").length) {
                borrador = $('#borradore').val();
            }
            $('#borrador_param').val(borrador);//B20180625 MGC2 2018.07.04

            $("#txt_flujo").val("B");
            //Termina provisional
            $('#btn_guardar').click();
        } else {
            dismiss('guardarWarnning');
            M.toast({
                classes: "guardarWarnning",
                displayLength: 1000000,
                html: '<span style="padding-right:15px;"><i class="material-icons yellow-text">info</i></span>  ' + msg
                    + '<button class="btn-small btn-flat toast-action" onclick="dismiss(\'guardarWarnning\')">Aceptar</button>'
            });
            document.getElementById("loader").style.display = "none";//RSG 26.04.2018
        }

    });
});