
function soportes(tsol, spras) {
    var texto = document.querySelector(".span_fileload").innerHTML;
    var backorder = document.getElementById("txt_backorder").value;
    if (backorder === undefined)
        backorder = "";
    $.ajax({
        url: "../Listas/Soportes",
        type: "POST",
        async: false,
        timeout: 30000,
        dataType: "json",
        data: { tsol: tsol, spras: spras, bo: backorder },
        success: function (data) {
            var pp = $.map(data, function (item) {
                return { tsoporte: item.TSOPORTE_ID, oblig: item.OBLIGATORIO, txt50: item.TXT50 };
            });
            $("#div_soportes").empty();
            for (var i = 0; i < pp.length; i++) {
                var input = '<label name="labels_soporte2" class="col s12">';
                if (pp[i].oblig) {
                    input += '* ';
                }
                input += pp[i].txt50 + '</label><label class"lbl_nec"></label>' +
                    '<input type="text" value="' + pp[i].txt50 + '" name="labels_soporte" hidden />' +
                    '<div class="file-field input-field col s12">' +
                    '<div class="btn-small" style="float:left;"> ' +
                    '<span class="span_fileload">'+texto+'</span > ' +
                    '<input class="file_soporte';
                if (pp[i].oblig) {
                    input += ' nec';
                }
                input += '" name="files_soporte" id="file_' + pp[i].tsoporte + '" type= "file" onchange="changeFile(this)"> ' +
                    '</div>' +
                    '<div class="file-path-wrapper"> ' +
                    '<input class="file-path validate" type="text" id="fileinput_' + pp[i].tsoporte + '" > ' +
                    '</div>' +
                    '</div>';

                $("#div_soportes").append(input);
            }
        }
    });
    $.ajax({
        url: "../Listas/TipoRecurrencia",
        type: "POST",
        async: true,
        timeout: 30000,
        //dataType: "json",
        data: { tsol: tsol },
        success: function (data) {
            if (data === "1") {
                if (ligada())
                    data = "2";
            }
            $("#txt_trec").val(data);
            if (data !== "1" && data !== "2" && data !== "3") {
                //$("#tabs_rec").addClass("disabled");
                $("#check_recurrente").prop("checked", false);
                $("#check_recurrente").attr("disabled", "disabled");
                $("#check_recurrente").trigger("change");
            } else {
                if (!isRelacionada()) {
                    //$("#tabs_rec").removeClass("disabled");
                    $("#check_recurrente").attr("disabled", false);
                }
            }
        }
    });

}

function pickerFecha(clase) {

    var script = "";
    script += "<script>";
    script += "var fechai = document.getElementById('fechai_vig').value;";
    script += "var fechaf = document.getElementById('fechaf_vig').value;";
    script += "var minDate = new Date(fechai.split('/')[2], fechai.split('/')[1]-1, fechai.split('/')[0]);";
    script += "var maxDate = new Date(fechaf.split('/')[2], fechaf.split('/')[1]-1, fechaf.split('/')[0]);";
    script += "var elems = document.querySelectorAll('" + clase + "');";
    script += "var options = {";
    script += "container: '#div_picker',";
    script += "format: 'dd/mm/yyyy'," +
        "minDate: minDate," +
        "maxDate: maxDate," +
        //"onClose: function (e) {" +
        //"var date = $('#fechad').val(); " +
        //"var periodo = date.split(" / "); " +

        //"}," +
        "i18n: {" +
        " clear: 'Limpiar', " +
        "today: 'Hoy', " +
        "done: 'Seleccionar', " +
        "previousMonth: '‹', " +
        "nextMonth: '›', " +
        "months: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'], " +
        "monthsShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'], " +
        "weekdaysShort: ['Dom', 'Lun', 'Mar', 'Mie', 'Jue', 'Vie', 'Sab'], " +
        "weekdays: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'], " +
        "weekdaysAbbrev: ['D', 'L', 'M', 'X', 'J', 'V', 'S']" +
        "}";
    script += "};";
    script += "var instances = M.Datepicker.init(elems, options); ";
    script += "</script>";
    return script;
}


function pickerFecha2(clase, div) {

    var fechai = document.getElementById('fechai_vig').value;
    var fechaf = document.getElementById('fechaf_vig').value;
    var minDate = new Date(fechai.split('/')[2], fechai.split('/')[1] - 1, fechai.split('/')[0]);
    var maxDate = new Date(fechaf.split('/')[2], fechaf.split('/')[1] - 1, fechaf.split('/')[0]);
    var elems = document.querySelectorAll(clase);
    var options = {
        container: '#div_picker' + div,
        format: 'dd/mm/yyyy',
        minDate: minDate,
        maxDate: maxDate,
        onClose: function (e) {
            ////var date = $('#fechad').val();
            ////var periodo = date.split(" / ");
            
        },
        i18n: {
            clear: 'Limpiar',
            today: 'Hoy',
            done: 'Seleccionar',
            previousMonth: '‹',
            nextMonth: '›',
            months: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            monthsShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            weekdaysShort: ['Dom', 'Lun', 'Mar', 'Mie', 'Jue', 'Vie', 'Sab'],
            weekdays: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
            weekdaysAbbrev: ['D', 'L', 'M', 'X', 'J', 'V', 'S']

        }
    };
    M.Datepicker.init(elems, options);
}