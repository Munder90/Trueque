
$(document).ready(function () {
    //Validar que los labels esten activos
    //Información
    $("label[for='notas_txt']").addClass("active");
    //Nombre    
    if ($('#cli_name').val() !== "") {
        $("label[for='cli_name']").addClass("active");
    }
    //Razón social
    if ($('#parvw').val() !== "") {
        $("label[for='parvw']").addClass("active");
    }
    //Razón social
    if ($('#vkorg').val() !== "") {
        $("label[for='vkorg']").addClass("active");
    }
    //Tax ID
    if ($('#stcd1').val() !== "") {
        $("label[for='stcd1']").addClass("active");
    }
    //Canal
    if ($('#vtweg').val() !== "") {
        $("label[for='vtweg']").addClass("active");
    }
    //Payer nombre
    if ($('#payer_nombre').val() !== "") {
        $("label[for='payer_nombre']").addClass("active");
    }
    //Email nombre
    if ($('#PAYER_EMAIL').val() !== "") {
        $("label[for='PAYER_EMAIL']").addClass("active");
    }

    //Soporte
    $('#table_sop').DataTable({
        "language": {
            "zerorecords": "no hay registros",
            "infoempty": "registros no disponibles",
            "decimal": ".",
            "thousands": ","
        },
        "paging": false,
        //        "ordering": false,
        "info": false,
        "searching": false,
        "columns": [
            //{
            //    "classname": 'select_row',
            //    "orderable": false,
            //    "data": null,
            //    "defaultcontent": ''
            //},
            {
                "name": 'POS',
                "className": 'POS',
            },
            //jemo 10-07-2018 inicio
            {
                "name": 'SOCIEDAD',
                "className": 'SOCIEDAD'
            },
            //jemo 10-07-2018 fin
            {
                "name": 'FACTURA',
                "className": 'FACTURA'
            },
            {
                "name": 'FECHA',
                "className": 'FECHA'
            },
            //jemo 10-07-2018 fin
            {
                "name": 'BILL_DOC',
                "className": 'BILL_DOC'
            },
            //jemo 10-07-2018 inicio
            {
                "name": 'PROVEEDOR',
                "className": 'PROVEEDOR'
            },
            {
                "name": 'PROVEEDOR_TXT',
                "className": 'PROVEEDOR_TXT'
            },
            {
                "name": 'CONTROL',
                "className": 'CONTROL'

            },
            {
                "name": 'AUTORIZACION',
                "className": 'AUTORIZACION'
            },
            {
                "name": 'VENCIMIENTO',
                "className": 'VENCIMIENTO'
            },
            {
                "name": 'FACTURAK',
                "className": 'FACTURAK'
            },
            {
                "name": 'EJERCICIOK',
                "className": 'EJERCICIOK'
            },
            //jemo 10-07-2018 inicio
            {
                "name": 'PAYER',
                "className": 'PAYER'
            },
            {
                "name": 'DESCRIPCION',
                "className": 'DESCRIPCION'
            },
            {
                "name": 'IMPORTE_FAC',
                "className": 'IMPORTE_FAC'
            },
            //jemo 10-07-2018 inicio
            {
                "name": 'BELNR',
                "className": 'BELNR'
            }
        ]
    });

    $('#matcat').click(function (e) {

        var kunnr = $('#payer_id').val();
        definirTipoCliente(kunnr);
        event.returnvalue = false;
        event.cancel = true;
    });

    //Evaluar la extensión y tamaño del archivo a cargar
    $('.file_soporte').change(function () {
        var length = $(this).length;
        var message = "";
        var namefile = "";
        if (length > 0) {
            //Validar tamaño y extensión
            var file = $(this).get(0).files;
            if (file.length > 0) {
                var sizefile = file[0].size;
                namefile = file[0].name;
                if (sizefile > 20971520) {
                    message = 'Error! Tamaño máximo del archivo 20 M --> Archivo ' + namefile + " sobrepasa el tamaño";

                }

                if (!evaluarExtSoporte(namefile)) {
                    message = "Error! Tipos de archivos aceptados 'xlsx', 'doc', 'pdf', 'png', 'msg', 'zip', 'jpg', 'docs' --> Archivo " + namefile + " no es compatible";

                }
            }
        } else {
            message = "No selecciono archivo";
        }

        if (message !== "") {
            $(this).val("");
            M.toast({ html: message });
        } else {
            //Verificar los nombres
            var id = $(this).attr('id');
            var res = evaluarFilesName(id, namefile);

            if (res) {
                //Nombre duplicado
                M.toast({ html: 'Ya existe un archivo con ese mismo nombre' });
            }
        }
    });

    //Negociación
    if ($('#notas_soporte').val() !== "") {
        $("label[for='notas_soporte']").addClass("active");
    }

    //Distribución    
    $('#table_dis').DataTable({
        destroy: true,
        "language": {
            "zeroRecords": "No hay registros",
            "infoEmpty": "Registros no disponibles",
            "decimal": ".",
            "thousands": ","
        },
        "paging": false,
        //        "ordering": false,
        "info": false,
        "searching": false,
        "columns": [
            {
                "className": 'id_row',
                "orderable": false,
                "defaultContent": ''

            },
            {
                "className": 'detail_row',
                "orderable": false,
                "data": null,
                "defaultContent": ''
            },
            {
                "className": 'select_row',
                "orderable": false,
                "data": null,
                "defaultContent": ''
            },
            {},
            {},
            {},
            {},
            {},
            {},
            {
                "className": 'PORC'//RSG 11.06.2018
            },
            {},
            {},
            {},
            {},
            {
                "className": 'total'//RSG 11.06.2018
            }

        ]
    });

    $('#table_dis tbody').on('click', 'td.select_row', function () {
        var tr = $(this).closest('tr');
        //Add MGC B20180705 2018.07.05 ne no eliminar
        if ($(tr).hasClass('ne')) {
            M.toast({ html: 'Los materiales originales de la provisión no se pueden eliminar' });
            $(tr).removeClass('selected');
        } else {
            $(tr).toggleClass('selected');
        }
    });


    $('#delRowB').click(function (e) {
        var t = $('#table_dis').DataTable();
        t.rows('.selected').remove().draw(false);
        //Validar si es categoría por porcentaje
        //Obtener el tipo de negociación
        var neg = $("#select_neg").val();
        //Obtener la distribución
        var dis = $("#select_dis").val();
        if (neg === "P" && dis === "C") {
            //Actualizar la tabla con los porcentajes
            updateTableCat();
        } else {
            updateFooter();
        }
        event.returnValue = false;
        event.cancel = true;
    });

    //Mostrar los materiales (detalle) de la categoria 
    $('#table_dis tbody').on('click', 'td.detail_row', function () {
        var t = $('#table_dis').DataTable();
        var tr = $(this).closest('tr');
        var row = t.row(tr);

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('details');
        }
        else {
            document.getElementById("loader").style.display = "initial";//RSG 26.04.2018
            //Obtener el id de la categoría
            var index = t.row(tr).index();
            var catid = t.row(index).data()[0];
            //Obtener las fechas del row de la categoría
            var indext = getIndex();
            var vd = (3 + indext);
            var va = (4 + indext);
            var vigencia_de = tr.find("td:eq(" + vd + ") input").val();
            var vigencia_al = tr.find("td:eq(" + va + ") input").val();
            var tot = parseFloat(toNum(tr.find("td.total input").val()));//ADD RSG 03.11.2018

            //row.child(format(catid, vigencia_de, vigencia_al)).show();
            row.child(format(catid, vigencia_de, vigencia_al, tot)).show();//ADD RSG 03.11.2018
            tr.addClass('details');
            document.getElementById("loader").style.display = "none";//RSG 26.04.2018
        }
    });

    $('#addRowB').on('click', function () {
        if ($("#catmat").val() === "" && $("#select_dis").val() === "C") {
            return;
        }
        var relacionada = "";
        if ($('#table_dis > tbody > tr').length === 1 && $('table_dis').find(' tbody tr:eq(0)').attr('class') !== "row") {
            catsArr = new Array();
            uniqueArr = new Array();
            unica1 = false;
            unica = false;
        }
        if ($("#txt_rel").length) {
            var vrelacionada = $('#txt_rel').val();
            if (vrelacionada !== "") {
                relacionada = "prelacionada";
            }
        }

        //Add MGC B20180705 2018.07.05 permitir editar el material 
        var relacionadaed = "";
        if (isAddt()) {
            relacionadaed = "prelacionadaed";
        }

        var reversa = "";
        if ($("#txt_rev").length) {
            var vreversa = $('#txt_rev').val();
            if (vreversa === "preversa") {
                reversa = vreversa;
            }
        }

        //Obtener el tipo de negociación
        var neg = $("#select_neg").val();

        if (neg !== "") {
            //Obtener los valores que se van a utilizar
            var t = $('#table_dis').DataTable();
            //Obtener las fechas de temporalidad para agregarlas a los items
            var val_de = $('#fechai_vig').val();
            var val_al = $('#fechaf_vig').val();

            var adate = formatDate(val_al);
            var ddate = formatDate(val_de);

            adate = formatDatef(adate);
            ddate = formatDatef(ddate);

            //Obtener la distribución
            var dis = $("#select_dis").val();

            if (dis === "") {
                M.toast({ html: 'Seleccione distribución' });
                return false;
            }
            var addedRow;
            var cat = "";
            var catExist;
            var opt;
            var catt;
            //Negociación Monto
            if (neg === "M") {
                //Distribución por categoría
                if (dis === "C") {
                    //Obtener la categoría
                    cat = $('#select_categoria').val();

                    //Validar si la categoría ya había sido agregada
                    catExist = valcategoria(cat);

                    //if (catExist != true) {
                    if (catExist === 0) {
                        if (cat !== "" & cat !== null) {

                            opt = $("#select_categoria option:selected").text();
                            addedRow = addRowCat(t, cat, ddate, adate, opt, "", relacionada, reversa, "", "");
                        } else {
                            M.toast({ html: 'Seleccione una categoría' });
                        }
                    } else {
                        if (catExist === 1) {
                            //M.toast({ html: 'La categoría ya había sido agregada' });
                            msj("toast", 'La categoría no puede ser agregada en la misma solicitud');//16-11-2018
                        } else {
                            catt = categoriaUnica(cat);
                            msj("toast", catt + ' no pueden mezclarse con otras categorías y/ o materiales'); //16-11-2018
                        }
                    }

                } else if (dis === "M") {//falta validar
                    var classtd = $("#table_dis tbody tr:first td").attr("class");
                    indext = getIndex();

                    //Distribución por material 
                    addedRow = addRowMat(t, "", "", "", "", "", "", "", "", "", "", "", relacionada, relacionadaed, reversa, ddate, adate, "", "");//Add MGC B20180705 2018.07.05 ne no eliminar //Add MGC B20180705 2018.07.05 relacionadaed editar el material en los nuevos renglones
                    $('#table_dis').css("font-size", "12px");
                    $('#table_dis').css("display", "table");
                    t.column(0).visible(false);
                    t.column(1).visible(false);
                }
                updateFooter();


            } else if (neg === "P") {
                var p_apoyo = toNum($('#bmonto_apoyo').val());
                p_apoyo = parseFloat(p_apoyo) | 0;

                var m_apoyo = toNum($('#monto_dis').val());
                m_apoyo = parseFloat(m_apoyo) | 0;

                //Distribución por categoría
                if (dis === "C") {
                    //if (m_apoyo > 0) {//RSG 09.07.2018
                    if (m_apoyo > 0 | $("#chk_ligada").is(':checked')) {
                        //Obtener la categoría
                        cat = $('#select_categoria').val();

                        //Validar si la categoría ya había sido agregada
                        catExist = valcategoria(cat);
                        //if (catExist != true) {
                        if (catExist === 0) {
                            if (cat !== "" & cat !== null) {
                                opt = $("#select_categoria option:selected").text();
                                porcentaje_cat = "<input class=\"" + reversa + " input_oper numberd porc_cat pc\" style=\"font-size:12px;\" type=\"text\" id=\"\" name=\"\" value=\"\">";
                                addedRow = addRowCat(t, cat, ddate, adate, opt, "", relacionada, reversa, porcentaje_cat, "pc");
                                $(".pc").prop('disabled', true);
                                $('.pc').trigger('click');
                                //Actualizar la tabla con los porcentajes
                                updateTableCat();
                            } else {
                                M.toast({ html: 'Seleccione una categoría' });

                            }
                        } else {
                            if (catExist === 1) {
                                //M.toast({ html: 'La categoría ya había sido agregada' });
                                msj("toast", 'La categoría no puede ser agregada en la misma solicitud'); //16-11-2018
                            } else {
                                catt = categoriaUnica(cat);
                                msj("toast", catt + ' no pueden mezclarse con otras categorías y/ o materiales');//16-11-2018
                            }
                        }
                    } else {
                        msj("toast", 'El monto base debe de ser mayor a cero'); //16-11-2018
                    }
                } else if (dis === "M") {
                    //Distribución por material  
                    var por_apoyo = 0;
                    por_apoyo = p_apoyo;
                    if (por_apoyo > 0) {
                        if (ligada()) //RSG 17.09.2018
                            por_apoyo = 0;
                        //var addedRow = addRowMat(t, "", "", "", "", "", por_apoyo, "", "", "", "", "", relacionada, reversa, ddate, adate, "", "pm");
                        addedRow = addRowMat(t, "", "", "", "", "", toShowPorc(por_apoyo), "", "", "", "", "", relacionada, "", reversa, ddate, adate, "", "pm", "");//Add MGC B20180705 2018.07.05 ne no eliminar después de pm //Add MGC B20180705 2018.07.05 relacionadaed editar el material en los nuevos renglones
                        //Si el porcentaje de apoyo es mayor a cero bloquear la columna de porcentaje de apoyo
                        //Eliminar los renglones que no contienen el mismo porcentaje
                        $(".pm").prop('disabled', true);
                        $('.pm').trigger('click');
                        //eliminarRowsDistribucion(por_apoyo);
                    } else {
                        //Si el porcentage es 0 desbloquear la columna de porcentaje de apoyo
                        msj("toast", 'Porcentaje de apoyo base debe de ser mayor a cero');//16-11-2018
                        return false;
                    }


                    //Inhabilitar la modificación del total



                    $('#table_dis').css("font-size", "12px");
                    $('#table_dis').css("display", "table");

                    t.column(0).visible(false);
                    t.column(1).visible(false);
                }
                updateFooter();
                //} else {
                //    M.toast({ html: 'Porcentaje de apoyo base debe de ser mayor a cero' });
                //}
            }
        } else {
            M.toast({ html: 'Seleccione negociación' });
        }

        //Set val a DatePicker
        var fe = document.querySelectorAll('.input_fe');
        for (var i = 0; i < fe.length; i++) {
            var datePicker = fe[i];
            if (datePicker.value) {
                var dateVal = moment(datePicker.value, 'DD/MM/YYYY');
                M.Datepicker.getInstance(datePicker).setDate(dateVal._d);
            }}
        //fe.forEach(function (datePicker) {
        //    if (datePicker.value) {
        //        ////var dateVal = moment(datePicker.value, 'DD/MM/YYYY');
        //        ////M.Datepicker.getInstance(datePicker).setDate(dateVal._d);
        //    }
        //});

        event.returnValue = false;
        event.cancel = true;

    });

    //Archivo para tabla de distribución
    $("#file_dis").change(function () {
        var filenum = $('#file_dis').get(0).files.length;
        if (filenum > 0) {
            var file = document.getElementById("file_dis").files[0];
            var filename = file.name;
            if (evaluarExt(filename)) {
                M.toast({ html: 'Cargando ' + filename });
                loadExcelDis(file);
                updateFooter();
            } else {
                M.toast({ html: 'Tipo de archivo incorrecto: ' + filename });
            }
        } else {
            M.toast({ html: 'Seleccione un archivo' });
        }
    });

    //jemo 18-07-2018 inicio
    $('#check_factura').change(function () {
        var table = $('#table_sop').DataTable();
        table.clear().draw(true);
        if ($(this).is(":checked")) {
            $(".table_sop").css("display", "none");
            $("#file_facturat").css("display", "block");
            $("#check_facturas").val("true"); //B20180625 MGC 2018.06.27

        } else {
            $(".table_sop").css("display", "table");
            $("#file_facturat").css("display", "none");
            $("#check_facturas").val("false"); //B20180625 MGC 2018.06.27
            //$("#check_facturas").val("true"); //B20180625 MGC 2018.06.27
            //Add row 
            addRowSop(table);
            //Hide columns
            ocultarColumnasTablaSoporteDatos();
        }

        $('.file_sop').val('');
    });
    //jemo 18-07-2018 fin

    $('#file_sop').on('click touchstart', function () {
        $('.file_sop').val('');
    });

    //Archivo para facturas en soporte ahora información
    $("#file_sop").change(function () {
        var filenum = $('#file_sop').get(0).files.length;
        if (filenum > 0) {
            var file = document.getElementById("file_sop").files[0];
            var filename = file.name;
            if (evaluarExt(filename)) {
                M.toast({ html: 'Cargando ' + filename });
                loadExcelSop(file);
            } else {
                M.toast({ html: 'Tipo de archivo incorrecto: ' + filename });
            }
        } else {
            M.toast({ html: 'Seleccione un archivo' });
        }
    });


    //Temporalidad
    if ($('#monto_doc_md').val() !== "") {
        $("label[for='monto_doc_md']").addClass("active");
    }

    $('#tabs').tabs();

    var elem = document.querySelectorAll('select');
    var instance = M.FormSelect.init(elem, []);

    $('#tab_tempp').on("click", function (e) {
        //$('#gall_id').change();
        evalInfoTab(false, e);
    });

    $('#tab_soportee').on("click", function (e) {

        evalTempTab(false, e);

    });
    $('#tab_fin').on("click", function (e) {
        //LEJ 09.07.18----------------------------
        var _miles = $("#miles").val();
        var _decimales = $("#dec").val();
        //LEJ 09.07.18----------------------------
        $("label[for='montos_doc_ml2']").addClass("active");//RSG 29.07.2018
        var res = evalDistribucionTab(true, e);
        if (res) {

            //Activar el botón de guardar
            //$("#btn_guardarh").removeClass("disabled");

            //Copiar el monto de distribución de la tabla footer al monto financiera
            //LEJ 09.07.18-------------------------------------------------------------Inicia
            //var total_dis = $('#total_dis').text();
            var total_dis = $('#total_dis').text().replace("$", '');
            //if (_decimales === '.') {
            //    total_dis = total_dis.replace(',', '');
            //}
            //else if (_decimales === ',') {
            //    var _xtot = total_dis.replace('.', '');
            //    _xtot = _xtot.replace(',', '.');
            //    total_dis = _xtot;
            //}
            total_dis = toNum(total_dis);
            //LEJ 09.07.18-------------------------------------------------------------Termina
            var basei = convertI(total_dis);

            //Obtiene el id del tipo de negociación, default envía vacío
            var select_neg = $('#select_neg').val();
            //Validar el monto base vs monto tabla
            if (select_neg === "M") {
                //Tiene que tener una moneda
                //Obtener la moneda de distribución y de financiera
                var monedadis_id = $('#monedadis_id').val();
                var monedafin_id = $('#moneda_id').val();

                //Si las monedas son iguales, se pasa el monto
                if (monedadis_id === monedafin_id) {
                    //LEJ 09.07.18------------------------------------------------------------------------------Inicia
                    //$('#monto_doc_md').val(basei);
                    //Adaptacion para monedas . y ,
                    if (_decimales === '.') {
                        //$('#monto_doc_md').val("$" + basei.toFixed(2).toString().replace(/\B(?=(?=\d*\.)(\d{3})+(?!\d))/g, ","));
                        $('#monto_doc_md').val(toShow(basei.toFixed(2).toString()));
                    } else if (_decimales === ',') {
                        //var _xbasei = basei.toFixed(2).replace('.', ',');
                        //$('#monto_doc_md').val("$" + _xbasei.toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, "."));
                        $('#monto_doc_md').val(toShow(basei.toFixed(2).toString()));
                    }
                    //LEJ 09.07.18------------------------------------------------------------------------------Termina
                } else {
                    //Realizar conversión de monedas
                    var newMonto = cambioCurr(monedadis_id, monedafin_id, basei);
                    // $('#monto_doc_md').val(newMonto);
                    //LEJ 09.07.18------------------------------------------------------------------------------Inicia
                    if (_decimales === '.') {
                        //$('#monto_doc_md').val("$" + newMonto.toFixed(2).toString().replace(/\B(?=(?=\d*\.)(\d{3})+(?!\d))/g, ","));
                        $('#monto_doc_md').val(toShow(newMonto.toFixed(2).toString()));
                    }
                    else if (_decimales === ',') {
                        //var x_newm = newMonto.toFixed(2).replace('.', ',');
                        //$('#monto_doc_md').val("$" + x_newm.toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, "."));
                        $('#monto_doc_md').val(toShow(newMonto.toFixed(2).toString()));
                    }
                    //LEJ 09.07.18------------------------------------------------------------------------------Termina
                }

            } else if (select_neg === "P") {
                //Si no es por monto solo se copia la cantidad

                //  $('#monto_doc_md').val(basei);
                //LEJ 09.07.18------------------------------------------------------------------------------Inicia
                //Adaptacion para monedas . y ,
                if (_decimales === '.') {
                    //$('#monto_doc_md').val("$" + basei.toFixed(2).toString().replace(/\B(?=(?=\d*\.)(\d{3})+(?!\d))/g, ","));
                    $('#monto_doc_md').val(toShow(basei.toFixed(2).toString()));
                } else if (_decimales === ',') {
                    //var _xbasei = basei.toFixed(2).replace('.', ',');
                    //$('#monto_doc_md').val("$" + _xbasei.toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, "."));
                    $('#monto_doc_md').val(toShow(basei.toFixed(2).toString()));
                }
                //LEJ 09.07.18------------------------------------------------------------------------------Termina
            }

            //Emular un focus out para actualizar los campos
            $('#monto_doc_md').focusout();//B20180625 MGC 2018.07.02
            //focusoutmonto("");//B20180625 MGC 2018.07.02

            $("label[for='monto_doc_md']").addClass("active");

            //Obtener los valores para asignar persupuesto
            //Obtener canal desc
            var canal = $('#vtweg').val();
            canal = canal.split('-');
            canal[1] = $.trim(canal[1]);
            $('#p_vtweg').text(canal[1]);
            //Obtener cliente id
            var kunnr = $('#payer_id').val();
            //$('#cli_name').val();
            $('#p_kunnr').text(kunnr);
            $('#p_kunnr').text($("#cli_name").val());


            var num = $('#txt_rel').val();//RSG 12.06.2018
            var num2 = $('#monto_doc_md').val();//RSG 12.06.2018

            asignarPresupuesto(kunnr);
            if ($('#NUM_DOC').val() !== undefined) {
                asignarSolicitud($('#NUM_DOC').val(), num2.replace("$", ""), true);//RSG 12.06.2018 //LEJ 09.07.18
            } else {
                asignarSolicitud(num, num2.replace("$", ""));//RSG 12.06.2018 //LEJ 09.07.18
            }

        } else {
            msj("toast", 'Verificar valores en los campos de Distribución!'); //16-11-2018
            e.preventDefault();
            e.stopPropagation();
            //var active = $('ul.tabs .active').attr('href');
            //$('ul.tabs').tabs('select_tab', active);
            var ell = document.getElementById("tabs");
            var instances = M.Tabs.getInstance(ell);
            instances.select('Distribucion_cont');
        }

        formaClearing();//RSG 18.06.2018
    });

    //Financiera   
    $('#monto_doc_md').focusout(function (e) {
        //LEJ 09.07.18-----------------------------------------------Inicia
        var _miles = $("#miles").val();
        var _decimales = $("#dec").val();
        var monto_doc_md = $('#monto_doc_md').val().replace("$", '');
        //var is_num = $.isNumeric(monto_doc_md);
        // var mt = parseFloat(monto_doc_md.replace(',', '')).toFixed(2);
        if (_decimales === '.') {
            //monto_doc_md = monto_doc_md.replace(',', '');
            monto_doc_md = toNum(monto_doc_md);
        }
        else if (_decimales === ',') {
            //monto_doc_md = monto_doc_md.replace('.', '');
            //monto_doc_md = monto_doc_md.replace(',', '.');
            monto_doc_md = toNum(monto_doc_md);
        }
        var is_num = $.isNumeric(monto_doc_md);
        //var mt = parseFloat(monto_doc_md.replace(',', '')).toFixed(2);
        var mt = parseFloat(toNum(monto_doc_md)).toFixed(2);
        //LEJ 09.07.18----------------------------------------------Termina
        //if (mt > 0 & is_num == true) {//RSG 09.07.2018
        if ((mt > 0 | ligada()) & is_num === true) {
            //Obtener la moneda en la lista
            //var MONEDA_ID = $('#moneda_id').val();
            //$('#monto_doc_md').val(mt);
            //LEJ 09.07.18---------------------------------------------------------------------Inicia
            if (_decimales === '.') {
                //$('#monto_doc_md').val("$" + mt.toString().replace(/\B(?=(?=\d*\.)(\d{3})+(?!\d))/g, ","));
                //$('#monto_doc_md').val(toShow(mt.toString()));
                $('#monto_doc_md').val(toShow(mt.toString()));
            }
            else if (_decimales === ',') {
                //var _mtx = mt.replace('.', ',').toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, ".");
                //$('#monto_doc_md').val("$" + _mtx);
                $('#monto_doc_md').val(toShow(mt.toString()));
            }
            //LEJ 09.07.18---------------------------------------------------------------------Termina
            //selectTcambio(MONEDA_ID, mt);
            //var tipo_cambio = $('#tipo_cambio').val();
            var tipo_cambio = $('#tipo_cambio').val().replace('$', '');//LEJ 09.07.18
            //var tc = parseFloat(tipo_cambio.replace(',', '')).toFixed(2);
            var tc = 0;//LEJ 09.07.18
            if (_decimales === ',') {
                //tc = parseFloat(tipo_cambio.replace(',', '.')).toFixed(2);
                //tipo_cambio = tipo_cambio.replace('.', '');
                //tipo_cambio = tipo_cambio.replace(',', '.');
                tc = parseFloat(toNum(tipo_cambio));
                tipo_cambio = toNum(tipo_cambio);
            }//LEJ 09.07.18
            else if (_decimales === '.') {
                //tc = parseFloat(tipo_cambio.replace(',', '')).toFixed(2);//LEJ 09.07.18
                tc = parseFloat(toNum(tipo_cambio));
            }
            //Validar el monto en tipo de cambio
            var is_num2 = $.isNumeric(tipo_cambio);
            if (tc > 0 & is_num2 === true) {
                // $('#tipo_cambio').val(tc);
                //LEJ 09.07.18--------------------
                //if (_decimales === '.') {
                //    $('#tipo_cambio').val("$" + tc);
                //}
                //else if (_decimales === ',') {
                //    $('#tipo_cambio').val("$" + tc.replace('.', ','));
                //}

                //$('#tipo_cambio').val(toShow(tc));
                $('#tipo_cambio').val(toShow5(tc));

                var monto = mt / tc;
                monto = parseFloat(monto).toFixed(2);
                //$('#monto_doc_ml2').val(monto);
                // $('#montos_doc_ml2').val(monto);
                //$("label[for='montos_doc_ml2']").addClass("active");
                //LEJ 09.07.18-------------------------------------------------------------------------------------------Inicia
                if (_decimales === '.') {
                    //$('#monto_doc_ml2').val("$" + monto.toString().replace(/\B(?=(?=\d*\.)(\d{3})+(?!\d))/g, ","));
                    //$('#montos_doc_ml2').val("$" + monto.toString().replace(/\B(?=(?=\d*\.)(\d{3})+(?!\d))/g, ","));
                    $('#montos_doc_ml2').val(toShow(monto.toString()));
                }
                else if (_decimales === ',') {
                    //$('#monto_doc_ml2').val("$" + monto.replace('.', ',').toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, "."));
                    //$('#montos_doc_ml2').val("$" + monto.replace('.', ',').toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, "."));
                    $('#montos_doc_ml2').val(toShow(monto.toString()));
                }
                //LEJ 09.07.18-------------------------------------------------------------------------------------------Termina
            } else {
                $('#monto_doc_ml2').val(monto);
                $('#montos_doc_ml2').val(monto);
                $("label[for='montos_doc_ml2']").addClass("active");
                var msg = 'Tipo de cambio incorrecto';
                M.toast({ html: msg });
                e.preventDefault();
            }

        } else {
            $('#monto_doc_ml2').val(monto_doc_md);
            $('#montos_doc_ml2').val(monto_doc_md);
            $("label[for='montos_doc_ml2']").addClass("active");
            msg = 'Monto incorrecto';
            msj("toast", msg); //16-11-2018
            e.preventDefault();
        }

    });

    $('body').on('keydown', '#tipo_cambio', function (e) {
        var _miles = $("#miles").val(); //LEJ 09.07.18
        var _decimales = $("#dec").val(); //LEJ 09.07.18
        if (_decimales === ".") {
            if (e.keyCode === 110 || e.keyCode === 190) {
                if ($(this).val().indexOf('.') !== -1) {
                    e.preventDefault();
                }
            }
            else {  // Allow: backspace, delete, tab, escape, enter and .
                if (e.keyCode === 13) {
                    $("#tipo_cambio").focusout();  //OCG Se agrega para quitar el focus y no redireccionar a la pestaña de informacioin
                }
                if ($.inArray(e.keyCode, [46, 8, 9, 27, /*13,*/ 110, 190]) !== -1 ||
                    // Allow: Ctrl+A, Command+A
                    (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                    // Allow: home, end, left, right, down, up
                    (e.keyCode >= 35 && e.keyCode <= 40)) {
                    // let it happen, don't do anything

                    return;
                }

                // Ensure that it is a number and stop the keypress
                if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                    e.preventDefault();
                }
            }
        }
        else if (_decimales === ",") {
            if (e.keyCode === 188) {
                if ($(this).val().indexOf(',') !== -1) {
                    e.preventDefault();
                }
            }
            else {  // Allow: backspace, delete, tab, escape, enter and ','
                if (e.keyCode === 13) {
                    $("#tipo_cambio").focusout(); //OCG Se agrega para quitar el focus y no redireccionar a la pestaña de informacioin
                }
                if ($.inArray(e.keyCode, [46, 8, 9, 27, /*13,*/ 188]) !== -1 ||
                    // Allow: Ctrl+A, Command+A
                    (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                    // Allow: home, end, left, right, down, up
                    (e.keyCode >= 35 && e.keyCode <= 40)) {
                    // let it happen, don't do anything

                    return;
                }

                // Ensure that it is a number and stop the keypress
                if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                    e.preventDefault();
                }
            }
        }
    });

    //LEJ 09.07.18---------------------------------------------Inicia
    //--Cambio de formato monetario
    var _Tc = toNum($("#tipo_cambio").val());
    var _mil = $("#miles").val();
    var _dec = $("#dec").val();
    //if (_dec === '.') {
    //    $("#tipo_cambio").val("$" + _Tc);
    //}
    //else if (_dec === ',') {
    //    var _xtc = _Tc.replace('.', ',');
    //    _Tc = _xtc;
    //    $("#tipo_cambio").val("$" + _Tc);
    //}
    $("#tipo_cambio").val(toShow5(_Tc));
    //LEJ 09.07.18---------------------------------------------Termina

    $('#tipo_cambio').focusout(function (e) {
        var _miles = $("#miles").val(); //LEJ 09.07.18
        var _decimales = $("#dec").val(); //LEJ 09.07.18
        //var tipo_cambio = $('#tipo_cambio').val();
        var tipo_cambio = $('#tipo_cambio').val().replace('$', '').replace(_miles, ''); //LEJ 09.07.18
        tipo_cambio = tipo_cambio.replace(',', '.');
        if (tipo_cambio !== "") {
            //LEJ 09.07.18------------------------I
            //if (_decimales === '.') {
            //    tipo_cambio = tipo_cambio.replace(',', '');
            //}
            //else if (_decimales === ',') {
            //    var _xtc = tipo_cambio.replace('.', '');
            //    _xtc = _xtc.replace(',', '.');
            //    tipo_cambio = _xtc;
            //}
            //////////////tipo_cambio = toNum(tipo_cambio);
            //LEJ 09.07.18------------------------T
            var is_num = $.isNumeric(tipo_cambio);
            //var tc = parseFloat(tipo_cambio.replace(',', '')).toFixed(2);
            var tc = parseFloat(tipo_cambio);
            //Validar el monto en tipo de cambio
            if (tc > 0 & is_num === true) {
                //Validar el monto
                // $('#tipo_cambio').val(tc)
                //LEJ 10.07.18----------------------------------I
                //if (_decimales === '.') {
                //    $('#tipo_cambio').val("$" + tc.toString().replace(/\B(?=(?=\d*\.)(\d{3})+(?!\d))/g, ","));
                //}
                //else if (_decimales === ',') {
                //    tc = tc.replace('.', ',');
                //    $('#tipo_cambio').val("$" + tc.toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, "."));
                //}
                //$('#tipo_cambio').val(toShow(tc.toString()));
                $('#tipo_cambio').val(toShow5(tc.toString()));
                //LEJ 10.07.18----------------------------------T
                // var monto_doc_md = $('#monto_doc_md').val();
                //var mt = parseFloat(monto_doc_md.replace(',', '')).toFixed(2);
                //LEJ 09.07.18-----------------------------------------------I
                var monto_doc_md = $('#monto_doc_md').val().replace('$', '');
                var mt = 0;
                ////Cambiosde formato para moneda
                //if (_decimales === '.') {
                //    mt = parseFloat(toNum(monto_doc_md)).toFixed(2);
                //    monto_doc_md = mt;
                //}
                //else if (_decimales === ',') {
                //    var _xmt = toNum(monto_doc_md);
                //    //_xmt = _xmt.replace(',', '.');
                //    mt = parseFloat(_xmt).toFixed(2);
                //    monto_doc_md = mt;
                //}
                mt = parseFloat(toNum(monto_doc_md)).toFixed(2);
                monto_doc_md = mt;
                //LEJ 09.07.18-----------------------------------------------T
                var is_num2 = $.isNumeric(monto_doc_md);
                //if (mt > 0 & is_num2 == true) {//RSG 09.07.2018
                if ((mt > 0 | ligada()) & is_num === true) {
                    //$('#monto_doc_md').val(mt);//LEJ 09.07.18

                    //Validar la moneda                    
                    var moneda_id = $('#moneda_id').val();
                    if (moneda_id !== null && moneda_id !== "") {
                        $('#monto_doc_ml2').val();
                        $('#montos_doc_ml2').val();

                        //Los valores son correctos, proceso para generar nuevo monto
                        //var monto = mt / tc;
                        var monto = mt / tipo_cambio;
                        monto = parseFloat(monto).toFixed(2);
                        //LEJ 09.07.18----------------------------I
                        //if (_decimales === '.') {
                        //    monto = monto;
                        //}
                        //else if (_decimales === ',') {
                        //    var _xmonto = monto.replace('.', ',');
                        //    //compruebo los millares
                        //    var _arrM = _xmonto.split(',');
                        //    _arrM[0] = _arrM[0].toString().replace(/\B(?=(?=\d*\,)(\d{3})+(?!\d))/g, ".");
                        //    _xmonto = _arrM[0] + ',' + _arrM[1];
                        //    monto = _xmonto;
                        //}
                        ////$('#monto_doc_ml2').val(monto);
                        ////$('#montos_doc_ml2').val(monto);
                        //$('#monto_doc_ml2').val("$" + monto);
                        //$('#montos_doc_ml2').val("$" + monto);
                        $('#montos_doc_ml2').val(toShow(monto));
                        //LEJ 09.07.18-----------------------------T
                        $("label[for='montos_doc_ml2']").addClass("active");
                    }
                    else {
                        $('#monto_doc_md').val();

                        // $('#monto_doc_ml2').val(monto);
                        // $('#montos_doc_ml2').val(monto);
                        //LEJ 09.07.18
                        //$('#monto_doc_ml2').val("$" + monto);
                        $('#monto_doc_ml2').val(toShow(monto));
                        //$('#montos_doc_ml2').val("$" + monto);
                        $('#montos_doc_ml2').val(toShow(monto));
                        var msg = 'Moneda incorrecta';
                        M.toast({ html: msg });
                    }
                } else {
                    $('#monto_doc_md').val();

                    $('#tipo_cambio').val("");
                    $('#monto_doc_ml2').val(toShow(monto));
                    $('#montos_doc_ml2').val(toShow(monto));
                    $("label[for='montos_doc_ml2']").addClass("active");
                    msg = 'Monto incorrecto';
                    M.toast({ html: msg });
                    e.preventDefault();
                }

            } else {
                //$('#monto_doc_ml2').val(monto);
                // $('#montos_doc_ml2').val(monto);
                //$('#monto_doc_ml2').val("$" + monto);//LEJ 09.07.18
                //$('#montos_doc_ml2').val("$" + monto);//LEJ 09.07.18
                $('#monto_doc_ml2').val(toShow(monto));
                $('#montos_doc_ml2').val(toShow(monto));
                $("label[for='montos_doc_ml2']").addClass("active");
                msg = 'Tipo de cambio incorrecto';
                M.toast({ html: msg });
                e.preventDefault();
            }
        } else {
            //$('#monto_doc_ml2').val("$0.00");//LEJ 09.07.18
            //$('#montos_doc_ml2').val("$0.00");//LEJ 09.07.18
            $('#monto_doc_ml2').val(toShow(0));
            $('#montos_doc_ml2').val(toShow(0));
        }
    });

    var monto_doc_md = $('#monto_doc_md').val();
    var is_num = $.isNumeric(monto_doc_md);
    //var mt = parseFloat(monto_doc_md.replace(',', '')).toFixed(2);
    var mt = parseFloat(toNum(monto_doc_md)).toFixed(2);
    if (mt > 0 & is_num === true) {
        //Obtener la moneda en la lista
        //var MONEDA_ID = $('#moneda_id').val();
        $('#monto_doc_md').val(mt);

        //selectTcambio(MONEDA_ID, mt);
        var tipo_cambio = toNum($('#tipo_cambio').val());
        //var tc = parseFloat(tipo_cambio.replace(',', '')).toFixed(2);
        var tc = parseFloat(toNum(tipo_cambio));
        //Validar el monto en tipo de cambio
        var is_num2 = $.isNumeric(tipo_cambio);
        if (tc > 0 & is_num2 === true) {
            $('#tipo_cambio').val(tc);
            var monto = mt / tc;
            monto = parseFloat(monto).toFixed(2);
            $('#monto_doc_ml2').val(toShow(monto));
            $('#montos_doc_ml2').val(toShow(monto));
            $("label[for='montos_doc_ml2']").addClass("active");
        } else {
            //$('#monto_doc_ml2').val(monto);
            //$('#montos_doc_ml2').val(monto);
            $('#monto_doc_ml2').val(toShow(monto));
            $('#montos_doc_ml2').val(toShow(monto));
            $("label[for='montos_doc_ml2']").addClass("active");
        }

    } else {
        //$('#monto_doc_ml2').val(monto_doc_md);
        //$('#montos_doc_ml2').val(monto_doc_md);
        $('#monto_doc_ml2').val(toShow(monto_doc_md));
        $('#montos_doc_ml2').val(toShow(monto_doc_md));
        $("label[for='montos_doc_ml2']").addClass("active");
    }
    //Delay for a number of milliseconds
    function sleep(delay) {
        var start = new Date().getTime();
        while (new Date().getTime() < start + delay);
    }
    $('#btn_guardarh').on("click", function (e) {
        var _miles = $("#miles").val(); //LEJ 09.07.18
        var _decimales = $("#dec").val(); //LEJ 09.07.18
        document.getElementById("loader").style.display = "flex";//RSG 26.04.2018
        var msg = 'Verificar valores en los campos: ';
        var campos = '';
        var res = true;
        //Evaluar TabInfo values
        var InfoTab = evalInfoTab(true, e);
        if (!InfoTab) {
            campos += 'Información';
            res = InfoTab;
        }
        //Evaluar SoporteTab values
        var SoporteTab = evalSoporteTab(true, e);
        if (!SoporteTab) {
            campos += (campos === '' ? '' : ',') +' Soporte';
            res = SoporteTab;
        }

        //Evaluar FinancieraTab values
        var FinancieraTab = evalFinancieraTab(true, e);
        if (!FinancieraTab) {
            campos += (campos === '' ? '' : ',') +' Financiera';
            res = FinancieraTab;
        }
        //jemo inicio 24-07-2018
        //validacion de importe de facturas contra monto de distribucion
        var checkf = $('#check_factura').is(':checked');
        if (checkf) {
            var monto = parseFloat(toNum($('#monto_dis').val()));
            importe_fac = parseFloat(importe_fac.toFixed(2));
            if (importe_fac !== monto) {
                campos += (campos === '' ? '' : ',') +' Importe total de las facturas sea igual al monto en Distribución';
                res = false;
            }
        }
        msg  += campos;
        //jemo fin 24-07-2018
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


            //Termina provisional
            $('#btn_guardar').click();
            M.toast({ html: "Guardando" });
        } else {
            M.toast({
                classes: "guardarWarnning",
                displayLength: 1000000,
                html: '<span style="padding-right:15px;"><i class="material-icons yellow-text">info</i></span>  ' + msg
                    + '<button class="btn-small btn-flat toast-action" onclick="dismiss(\'guardarWarnning\')">Aceptar</button>'
            });
            document.getElementById("loader").style.display = "none";//RSG 26.04.2018
        }

    });
    $('#btn_guardarr').on("click", function (e) {
        document.getElementById("loader").style.display = "initial";//RSG 26.04.2018
        var msg = 'Verificar valores en los campos: ';
        var campos = '';
        var res = true;
        //Evaluar TabInfo values
        var InfoTab = evalInfoTab(true, e);
        if (!InfoTab) {
            campos += (campos === '' ? '' : ',') +'Información';
            res = InfoTab;
        }
        //Evaluar SoporteTab values
        var SoporteTab = evalSoporteTab(true, e);
        if (!SoporteTab) {
            campos += (campos === '' ? '' : ',')+' Soporte';
            res = SoporteTab;
        }

        //Evaluar FinancieraTab values
        var FinancieraTab = evalFinancieraTab(true, e);
        if (!FinancieraTab) {
            campos += (campos === '' ? '' : ',') + ' Financiera';
            res = FinancieraTab;
        }
        msg += campos;

        msg += '!';
        if (res) {
            //loadFilesf();
            //Provisional
            var tipo_cambio = $('#tipo_cambio').val();
            //var iNum = parseFloat(tipo_cambio.replace(',', '.')).toFixed(2);
            //var iNum = parseFloat(tipo_cambio.replace(',', ''));
            var iNum = parseFloat(toNum(tipo_cambio));

            if (iNum > 0) {
                //var num = "" + iNum;
                //num = num.replace('.', ',');
                //var numexp = num;//* 60000000000;
                //$('#tipo_cambio').val(numexp);
            } else {
                $('#tipo_cambio').val(0);
            }
            tipo_cambio = $('#monto_doc_ml2').val();
            //var iNum2 = parseFloat(tipo_cambio.replace(',', '.')).toFixed(2);
            //var iNum2 = parseFloat(tipo_cambio.replace(',', ''));
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

            //Monto
            var monto = $('#monto_doc_md').val();
            //var numm = parseFloat(monto.replace(',', '.')).toFixed(2);   
            //var numm = parseFloat(monto.replace(',', ''));
            var numm = parseFloat(toNum(monto));
            if (numm > 0) {
                $('#MONTO_DOC_MD').val(numm);
            } else {
                $('#MONTO_DOC_MD').val(0);
                $('#monto_doc_md').val(0);
            }
            //Guardar los valores de la tabla en el modelo para enviarlos al controlador
            copiarTableControl();//Distribución
            copiarSopTableControl(); //Soporte ahora en información
            //Termina provisional
            $('#btn_guardar').click();
            M.toast({ html: "Guardando" });
        } else {
            M.toast({ html: msg });
            document.getElementById("loader").style.display = "none";//RSG 26.04.2018
        }

    });


});


//B20180625 MGC 2018.07.04 para el auto-guardado del borrador
$(document).on('mousemove keyup keypress', function () {
    clearTimeout(interval);//clear it as soon as any event occurs
    //do any process and then call the function again
    settimeout();//call it again
});

function settimeout() {
    //Aplicar nada más si el boton de borrador existe
    if ($("#btn_borradorh").length) {
        interval = setTimeout(function () {
            actiontime();
        }, borradorinac)
    }
}

function actiontime() {
    guardarBorrador(true);
}

function guardarBorrador(asyncv) {

    //Antigúo borrador
    //loadFilesf();
    //Provisional
    var tipo_cambio = $('#tipo_cambio').val();
    //var iNum = parseFloat(tipo_cambio.replace(',', '.')).toFixed(2);
    var iNum = parseFloat(toNum(tipo_cambio));

    if (iNum > 0) {
        //var num = "" + iNum;
        //num = num.replace('.', ',');
        //var numexp = num;//* 60000000000;
        $('#tipo_cambio').val(iNum);
    } else {
        $('#tipo_cambio').val(toNum(0));
    }
    tipo_cambio = $('#monto_doc_ml2').val();
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

    //Monto
    var monto = $('#monto_dis').val();
    //var numm = parseFloat(monto.replace(',', '.')).toFixed(2);   
    //var numm = parseFloat(monto.replace(',', ''));//RSG 09.07.2018
    var numm = parseFloat(toNum(monto));
    if (numm > 0) {
        $('#MONTO_DOC_MD').val(numm);
    } else {
        $('#MONTO_DOC_MD').val(0);
        $('#monto_doc_md').val(0);
    }

    //bmonto_apoyo
    $('#bmonto_apoyo').val(toNum($('#bmonto_apoyo').val()));//RSG 01.08.2018

    $('#select_negi').prop('disabled', false); //B20180618 v1 MGC 2018.06.18
    $('#select_disi').prop('disabled', false); //B20180618 v1 MGC 2018.06.18

    //Guardar los valores de la tabla en el modelo para enviarlos al controlador
    copiarTableControl("X");//Distribución //B20180625 MGC 2018.07.03
    copiarSopTableControl("X"); //Soporte ahora en información //B20180625 MGC 2018.07.03
    enviaRec("X");//RSG 28.05.2018 //B20180625 MGC 2018.07.03
    enviaRan("X");//RSG 31.10.2018

    //B20180625 MGC 2018.06.28
    //Moneda en distribución
    var moneda_dis = $('#monedadis_id').val();
    $('#moneda_dis').val("");
    $('#moneda_dis').val(moneda_dis);
    $('#moneda_dis').prop('disabled', false);

    //Enviar el parametro al controlador para tratarlo como borrador
    $('#borrador_param').val("borrador");

    //Obtener los parametros para enviar
    var form = $("#formCreate");

    var notas_soporte = $('#notas_soporte').val();
    var unafact = $('#check_facturas').val();
    var select_neg = $('#select_neg').val();
    var select_dis = $('#select_dis').val();
    var select_negi = $('#select_negi').val();
    var select_disi = $('#select_disi').val();
    var bmonto_apoyo = $('#bmonto_apoyo').val();
    var monedadis = $('#moneda_dis').val();

    //Complemento mensaje
    var comp = "";

    if (asyncv == true) {
        comp = "(Autoguardado)";
    }

    $.ajax({
        type: "POST",
        url: 'Borrador',
        dataType: "json",
        data: form.serialize() + "&notas_soporte = " + notas_soporte + "&unafact = " + unafact + "&select_neg = " + select_neg + "&select_dis = " + select_dis +
            "&select_negi = " + select_negi + "&select_disi = " + select_disi + "&bmonto_apoyo = " + bmonto_apoyo + "&monedadis = " + monedadis,
        //data: {
        //    object: form.serialize(), "notas_soporte": notas_soporte, "unafact": unafact, "select_neg": select_neg, "select_dis": select_dis,
        //    "select_negi": select_negi, "select_disi": select_disi, "bmonto_apoyo": bmonto_apoyo, "monedadis": monedadis},
        success: function (data) {

            if (data !== null || data !== "") {
                if (data == true) {
                    M.toast({ html: "Borrador Guardado " + comp });
                    $('#btn_borradore').css("display", "inline-block");  //B20180625 MGC2 2018.07.04
                } else {
                    M.toast({ html: "No se guardo el borrador" + comp });
                }
            }

        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            M.toast({ html: "No se guardo el borrador" + comp });
        },
        async: asyncv
    });
}

//B20180625 MGC2 2018.07.04
function eliminarBorrador(asyncv) {

    var user = $('#USUARIOD_ID').val();

    $.ajax({
        type: "POST",
        url: 'eliminarBorrador',
        data: { "user": user },

        success: function (data) {

            if (data !== null || data !== "") {
                if (data === "X") {
                    window.location = root + "Solicitudes/Create";
                } else {
                    document.getElementById("loader").style.display = "none";
                    M.toast({ html: "No se ha eliminado el borrador" });
                    borrador = $('#borradore').val("true");
                }
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {

            document.getElementById("loader").style.display = "none";
            M.toast({ html: "No se ha eliminado el borrador" });
            borrador = $('#borradore').val("true");
        },
        async: asyncv
    });
}

//B20180625 MGC 2018.07.02
function focusoutmonto(directo) {
    if (directo == "X") {
        $('#monto_doc_md').focusout();
    } else {

        var monto_doc_md = $('#monto_doc_md').val();
        var is_num = $.isNumeric(monto_doc_md);
        var mt = parseFloat(monto_doc_md.replace(',', '')).toFixed(2);
        //if (mt > 0 & is_num == true) {//RSG 09.07.2018
        if ((mt > 0 | ligada()) & is_num == true) {
            //Obtener la moneda en la lista
            //var MONEDA_ID = $('#moneda_id').val();
            $('#monto_doc_md').val(mt);

            //selectTcambio(MONEDA_ID, mt);
            var tipo_cambio = $('#tipo_cambio').val();
            var tc = parseFloat(tipo_cambio.replace(',', '')).toFixed(2);
            //Validar el monto en tipo de cambio
            var is_num2 = $.isNumeric(tipo_cambio);
            if (tc > 0 & is_num2 == true) {
                //$('#tipo_cambio').val(toShow(tc));
                $('#tipo_cambio').val(toShow5(tc));
                var monto = mt / tc;
                monto = parseFloat(monto).toFixed(2);
                $('#monto_doc_ml2').val(monto);
                $('#montos_doc_ml2').val(monto);
                $("label[for='montos_doc_ml2']").addClass("active");
            } else {
                $('#monto_doc_ml2').val(monto);
                $('#montos_doc_ml2').val(monto);
                $("label[for='montos_doc_ml2']").addClass("active");
                var msg = 'Tipo de cambio incorrecto';
                M.toast({ html: msg });
                e.preventDefault();
            }

        } else {
            $('#monto_doc_ml2').val(monto_doc_md);
            $('#montos_doc_ml2').val(monto_doc_md);
            $("label[for='montos_doc_ml2']").addClass("active");
            var msg = 'Monto incorrecto';
            M.toast({ html: msg });
            e.preventDefault();
        }

    }
}
function formatDate(val) {
    var vdate = "";
    try {
        vdate = val.split('/');
        //vdate = new Date(vdate[2] + "-" + vdate[1] + "-" + vdate[0]);
        vdate = new Date(vdate[2], vdate[1] - 1, vdate[0]);
    }
    catch (err) {
        vdate = "";
    }



    return vdate;
}

function formatDatef(vdate) {

    var dd = "";
    var mm = "";
    var yy = "";
    var de = true;
    var d = "";

    try {
        dd = vdate.getDate();
    }
    catch (err) {
        de = false;
    }

    try {
        mm = (vdate.getMonth() + 1);
    }
    catch (err) {
        de = false;
    }

    try {
        yy = vdate.getFullYear();
    }
    catch (err) {
        de = false;
    }

    if (de == true) {
        d = dd + "/" + mm + "/" + yy;
    } else {
        d = "";
    }

    return d;
}

$('body').on('focusout', '#monto_dis', function () {
    var neg = $("#select_neg").val();
    //Obtener la distribución
    var dis = $("#select_dis").val();


    if (neg == "P" && dis == "C") {
        //var monto = $('#monto_dis').val();//RSG 09.07.2018
        var monto = toNum($('#monto_dis').val());//RSG 09.07.2018
        monto = parseFloat(monto);

        if (monto > 0) {
            //Actualizar la tabla con los porcentajes
            updateTableCat();
        } else {
            M.toast({ html: 'El monto debe de ser mayor a 0' });
            return false;
        }
    }
    $('#monto_dis').val(toShow(toNum($('#monto_dis').val())));
    cambiaRec();//RSG 06.06.2018
});
//------------------------------------------------------------------------

function loadExcelSop(file) {

    var formData = new FormData();
    document.getElementById("loader").style.display = 'flex';
    formData.append("FileUpload", file);
    importe_fac = 0;//jemo 25-17-2018
    var table = $('#table_sop').DataTable();
    table.clear().draw();
    $.ajax({
        type: "POST",
        url: 'LoadExcelSop',
        data: formData,
        dataType: "json",
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {

            if (data !== null || data !== "") {

                $.each(data, function (i, dataj) {

                    //var fecha = new Date(parseInt(dataj.FECHA.substr(6)));
                    //var ven = new Date(parseInt(dataj.VENCIMIENTO.substr(6)));
                    var addedRow = table.row.add([
                        dataj.POS,
                        dataj.SOCIEDAD,
                        dataj.FACTURA,
                        //jemo 25-17-2018 inicio
                        "",//"" + fecha.getDate() + "/" + (fecha.getMonth() + 1) + "/" + fecha.getFullYear(),
                        dataj.BILL_DOC,
                        dataj.PROVEEDOR,
                        dataj.PROVEEDOR_TXT,
                        "",//dataj.CONTROL,
                        "",//dataj.AUTORIZACION,
                        "",//"" + ven.getDate() + "/" + (ven.getMonth() + 1) + "/" + ven.getFullYear(),
                        "",//dataj.FACTURAK,
                        //jemo 25-17-2018 inicio
                        dataj.EJERCICIOK,
                        //jemo 25-17-2018 inicio
                        dataj.PAYER,
                        dataj.DESCRIPCION,
                        //"$" + dataj.IMPORTE_FACT.toString().replace(/\D/g, "")//jemo 31-17-2018 inicio
                        //    .replace(/\B(?=(\d{3})+(?!\d)\.?)/g, ","),//jemo 31-17-2018 fin
                        toShow(dataj.IMPORTE_FACT.toString()),
                        dataj.BELNR
                        //jemo 25-17-2018 fin
                    ]).draw(false).node();

                    if (dataj.PROVEEDOR_ACTIVO == false) {
                        $(addedRow).find('td.PROVEEDOR').addClass("errorProveedor");
                    }
                    importe_fac += parseFloat(toNum(dataj.IMPORTE_FACT));//jemo inicio 25-07-2018
                    document.getElementById("loader").style.display = 'none';
                });
                //Aplicar configuración de columnas en las tablas
                ocultarColumnasTablaSoporteDatos();
                $(".table_sop").css("display", "table");
                $("#table_sop").css("display", "table");
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            document.getElementById("loader").style.display = 'none';
        },
        async: true
    });

}
//------------------------------------------------------------------------
//Obtener los materiales por categoría
function GetMaterialesCat(catid, total, m_base) {
    var vals = $('#catmat').val();
    try {
        var jsval = JSON.parse(vals);
    } catch (error) {
        M.toast('Error');
    }

    var materiales = [];

    try {
        $.each(jsval, function (i, d) {

            if (catid === d.ID) {
                if ($("#select_neg").val() !== "P") {//ADD RSG 22.11.2018
                    total = d.TOTALCAT;
                }
                materiales = GetMaterialesCatDetalle(d.MATERIALES, catid, total, m_base);
                return false;
            }
        }); //Fin de for
    } catch (error) {
        M.toast('Error');
    }

    return materiales;
}


function GetMaterialesCatDetalle(jsval, catid, total, m_base) {


    var materiales = [];
    $.each(jsval, function (i, d) {
        var t = 0;
        var v = 0;
        if (catid === d.ID_CAT) {

            var por = 0;

            try {
                por = d.VAL * 100 / total;
            } catch (error) {
                por = 0;
            }
            try {
                v = (por * m_base) / 100;
            } catch (error) {
                v = 0;
            }
            var m = {};
            m["MATNR"] = d.MATNR;
            m["DESC"] = d.DESC;
            m['POR'] = por;
            m["VAL"] = v;
            materiales.push(m);
        }
    }); //Fin de for

    return materiales;
}


//Obtener el total por categoría
function GetTotalCat(catid) {
    var vals = $('#catmat').val();
    try {
        var jsval = JSON.parse(vals);
    } catch (error) {
        M.toast('Error');
    }

    var total = 0;
    try {
        $.each(jsval, function (i, d) {

            if (catid === d.ID) {
                total = GetTotalCatDetalle(d.MATERIALES, catid);
                return false;
            }
        }); //Fin de for
    } catch (error) {
        M.toast('Error');
    }

    return total;
}


function categoriaUnica(cat) {

    var res = 0;
    var t = $('#table_dis').DataTable();
    t.rows().every(function (rowIdx, tableLoop, rowLoop) {

        var tr = this.node();
        var row = t.row(tr);

        //Obtener el id de la categoría
        var index = t.row(tr).index();
        //Categoría en el row
        var catid = t.row(index).data()[0];
        var _xxx = $.parseJSON($('#catmat').val());//LEJ 18.07.2018
        for (var i = 0; i < _xxx.length; i++) {
            if (cat === _xxx[i].ID) {
                if (_xxx[i].UNICA === true) {
                    res = _xxx[i].DESCRIPCION;
                    return res;
                }
            }
        }

        for (var i = 0; i < _xxx.length; i++) {
            if (catid === _xxx[i].ID) {
                if (_xxx[i].UNICA === true) {
                    res = _xxx[i].DESCRIPCION;
                    return res;
                }
            }
        }

    });

    return res;
}