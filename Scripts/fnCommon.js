var fnCommon = {

    materializeInit: function (type, spras_id) {
        var options = {};
        switch (type) {
            case 'select':
                 options = {};
                var selects = document.querySelectorAll('select');
                M.FormSelect.init(selects, options);
                break;
            case 'datepicker':
                 options = { format: 'dd/mm/yyyy' };
                if (spras_id==='ES'){
                    options.i18n = {
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
                    };
                }
                var datepickers = document.querySelectorAll('.datepicker');
                M.Datepicker.init(datepickers, options);
                break;
            case 'timepicker':
                 options = {
                    twelveHour: false,
                    autoClose: true
                };
                var timepickers = document.querySelectorAll('.timepicker');
                var instances = M.Timepicker.init(timepickers, options);
                break;
            case 'tabs':
                 options = {};
                var tabs = document.querySelectorAll('.tabs');
                M.Tabs.init(tabs, options);
                break;
            case 'collapsible':
                 options = {};
                var collapsibles = document.querySelectorAll('.collapsible');
                M.Collapsible.init(collapsibles, options);
                break;
            case 'modal':
                options = {};
                var modals = document.querySelectorAll('.modal');
                M.Modal.init(modals, options);
                break;
            default:
                break;
        }
    },
    selectRequired: function () {
        var combos = document.querySelectorAll('.select-dropdown.dropdown-trigger');
        var i = 0;
        combos.forEach(function (combo) {
            combo.setAttribute('required', '');
            combo.setAttribute('name', 'select_' + i);
            i++;
        });
    },
    formValidation: function (idForm) {
        $.validator.setDefaults({
            errorClass: 'invalid',
            validClass: 'valid'
        });
        $.validator.addMethod(
            'required',
            function (value, element, requiredValue) {
                return !(value === '');
            },
            ''
        );
        $('#' + idForm).validate();
    },
   
    dateRangeValidation: function (idFechaIni, idFechaFin,_idFechaIni, _idFechaFin) {

        fnCommon.setDateRange(idFechaIni, idFechaFin, null);
        fnCommon.setDateRange(idFechaFin, _idFechaIni, idFechaIni);
        if (_idFechaIni && _idFechaFin) { fnCommon.setDateRange(_idFechaIni, _idFechaFin, idFechaFin); }
        if (_idFechaFin && _idFechaFin) { fnCommon.setDateRange(_idFechaFin, null, _idFechaIni); }

       
    },
    setDateRange: function (idFechaFSelect, idFechaFMin, idFechaFMax) {
        var fechaFSelect = $('#' + idFechaFSelect),
            fechaFSelectM = M.Datepicker.getInstance(fechaFSelect),

            fechaFMin = idFechaFMin ? $('#' + idFechaFMin) : null,
            fechaFMinM = fechaFMin ? M.Datepicker.getInstance(fechaFMin) : null,

            fechaFMax = idFechaFMax ? $('#' + idFechaFMax) : null,
            fechaFMaxM = fechaFMax ? M.Datepicker.getInstance(fechaFMax) : null;
        
        fechaFSelectM.options.onSelect = function (val) {
            if (fechaFMinM) { fechaFMinM.options.minDate = val; }
            if (fechaFMaxM) { fechaFMaxM.options.maxDate = val; }
            };
        
          

        if (fechaFSelect.val()) {
            var date = moment(fechaFSelect.val(), 'DD/MM/YYYY');
            if (fechaFMinM) { fechaFMinM.options.minDate = date; }
            if (fechaFMaxM) { fechaFMaxM.options.maxDate = date; }
        }
    },
    configurarTableNoPagNoBusq: function (idTable, scrollY, scrollX, urlLanguage, targets) {
     $('#' + idTable).DataTable({
            scrollY: scrollY,
            scrollX: scrollX,
            scrollCollapse: true,
            paging: false,
            info: false,
            searching: false,
            ordering: false,
            language: {
                url: urlLanguage
            },
            columnDefs: [{
                targets: targets ? targets : [0, 1, 2, 4, 5, 6],
                className: 'mdl-data-table__cell--non-numeric'
            }]
        });
    },
    configurarTable: function (idTable, scrollY, scrollX, urlLanguage,idSelectPag,idGFilter,targets) {
        var table =  $('#'+idTable).DataTable({
            scrollY: scrollY,
            scrollX: scrollX,
            scrollCollapse: true,
            language: {
                url: urlLanguage
            },
            columnDefs: [{
                targets: targets ? targets:[0, 1, 2,4,5,6],
                className: 'mdl-data-table__cell--non-numeric'
            }]
        });

        var selectPagVal = $('#' + idSelectPag).val();
        table.page.len(selectPagVal).draw();
        $('#' + idSelectPag).on('change', function () {
            table.page.len(this.value).draw();
        });

       

        $('input.global_filter').on('keyup click', function () {
            fnCommon.filterGlobal(idTable, idGFilter);
        });
    },
    filterGlobal: function (idTable, idGFilter) {
        var filterVal = $('#' + idGFilter).val();
        $('#' + idTable).DataTable().search(filterVal).draw();
    },
    fillOptionsInSelect: function (idSelect, url, idSelectToFill,callBack) {
        $('#' + idSelect).change(function () {
            var val = $('#' + idSelect).val();
            if (val === "") {
                var options = '<option value></option>';
                $('#' + idSelectToFill).html(options);
                fnCommon.materializeInit( 'select');
                fnCommon.selectRequired();
            } else {
                $.ajax({
                    url: url,
                    data: { val: val },
                    cache: false,
                    type: 'POST',
                    success: function (selectItems) {
                        var options = '<option value></option>';
                        selectItems.forEach(function (selectItem) {
                            options += '<option value=' + selectItem.Value  + '>' + selectItem.Text + '</option>';
                        });
                        $('#' + idSelectToFill).html(options);
                        fnCommon.materializeInit( 'select');
                        fnCommon.selectRequired();
                        if (callBack) {
                            callBack();
                        }
                    }
                });
            }
        });
    },
    showProcess: function (show) {
        if (show){
            document.getElementById("loader").style.display = "flex";
        } else {
            document.getElementById("loader").style.display = "none";
        }
    },
    autoAjax: function (url, response, params, fnData) {
        $ = $ === undefined ? auto : $;
        return $.ajax({
            url: url,
            type: "GET",
            dataType: "json",
            data: params,
            success: function (data) {
                response($.map(data, function (item) {
                    return fnData(item);
                }));
            }
        });
    }


};