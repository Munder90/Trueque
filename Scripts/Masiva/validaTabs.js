function filterGlobal(table, tab) {
    if ($(tab).hasClass("active")) {
        $('#tab_test' + table).DataTable().search(
            $('#global_filter').val()).draw();
    }
}


$(document).ready(function () {
    $('input.global_filter').on('keyup click', function () {//ADD RSG 29.10.2018
        filterGlobal("1","#tab_info");
        filterGlobal("2","#tab_rel");
        filterGlobal("3","#tab_mul");
        filterGlobal("4","#tab_dis");
        filterGlobal("5","#tab_arc");
    });
});

function validaTabs(num_doc, num) {
    var ban = false;
    if (num > 0)
        ban = !validH1(num_doc);
    if (ban && num > 1)
        ban = !validH2(num_doc);
    if (ban && num > 2)
        ban = !validH3(num_doc);
    if (ban && num > 3)
        ban = !validH4(num_doc);
    if (ban && num > 4)
        ban = !validH5(num_doc);
    return ban;
}

function validH1(num_doc) {

    var tablaH1 = $('#tab_test1').DataTable();
    var tabla1 = [], tabla2 = [], tabla3 = [], tabla4 = [], tabla5 = [], archivos = [];
    var banderaH1, banderaH2, banderaH3, banderaH4, banderaH5 = false;

    for (var a = 0; a < tablaH1.rows().data().length; a++) {
        var rowH1 = tablaH1.row(a).node();
        var num_docH1 = $(rowH1).children().eq(1).children().val();
        var statusH1 = $(rowH1).children().children().hasClass('rojo');

        tabla1[a] = num_docH1 + statusH1;
    }

    var cont = 0;
    for (var aa = 0; aa < tablaH1.rows().data().length; aa++) {
        var rowH11 = tablaH1.row(aa).node();
        var num_docH11 = $(rowH11).children().eq(1).children().val();
        if (num_doc === num_docH11) {

            cont++;
            //SI TIENE TRUE TIENE ERROR
            if (jQuery.inArray(num_docH11 + true, tabla1) !== -1) {
                banderaH1 = true;
            }
            else {
                banderaH1 = false;
            }

            //console.log(banderaH1, banderaH2, banderaH3, banderaH4, banderaH5, num_docH11);
            //console.log(validaErrores(banderaH1, banderaH2, banderaH3, banderaH4, banderaH5, num_docH11));
        }
    }
    if (cont === 0)
        banderaH1 = false;
    return banderaH1;
}
function validH2(num_doc) {

    var tablaH2 = $('#tab_test2').DataTable();
    var tabla1 = [], tabla2 = [], tabla3 = [], tabla4 = [], tabla5 = [], archivos = [];
    var banderaH1, banderaH2, banderaH3, banderaH4, banderaH5 = false;

    for (var b = 0; b < tablaH2.rows().data().length; b++) {
        var rowH2 = tablaH2.row(b).node();
        var num_docH2 = $(rowH2).children().eq(1).children().val();
        var statusH2 = $(rowH2).children().children().hasClass('rojo');

        tabla2[b] = num_docH2 + statusH2;
    }
    var cont = 0;
    for (var bb = 0; bb < tablaH2.rows().data().length; bb++) {
        var rowH22 = tablaH2.row(bb).node();
        var num_docH22 = $(rowH22).children().eq(1).children().val();

        if (num_doc === num_docH22) {
            cont++;
            //SI TIENE TRUE TIENE ERROR
            if (jQuery.inArray(num_docH22 + true, tabla2) !== -1) {
                banderaH2 = true;
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
    if (cont === 0)
        banderaH2 = false;
    return banderaH2;
}
function validH3(num_doc) {

    var tablaH3 = $('#tab_test3').DataTable();
    var tabla1 = [], tabla2 = [], tabla3 = [], tabla4 = [], tabla5 = [], archivos = [];
    var banderaH1, banderaH2, banderaH3, banderaH4, banderaH5 = false;

    for (var c = 0; c < tablaH3.rows().data().length; c++) {
        var rowH3 = tablaH3.row(c).node();
        var num_docH3 = $(rowH3).children().eq(1).children().val();
        var statusH3 = $(rowH3).children().children().hasClass('rojo');

        tabla3[c] = num_docH3 + statusH3;
    }


    var cont = 0;
    for (var cc = 0; cc < tablaH3.rows().data().length; cc++) {
        var rowH33 = tablaH3.row(cc).node();
        var num_docH33 = $(rowH33).children().eq(1).children().val();

        if (num_doc === num_docH33) {
            cont++;
            //SI TIENE TRUE TIENE ERROR
            if (jQuery.inArray(num_docH33 + true, tabla3) !== -1) {
                banderaH3 = true;
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
    if (cont === 0)
        banderaH3 = false;
    return banderaH3;

}
function validH4(num_doc) {
    var tablaH4 = $('#tab_test4').DataTable();
    var tabla1 = [], tabla2 = [], tabla3 = [], tabla4 = [], tabla5 = [], archivos = [];
    var banderaH1, banderaH2, banderaH3, banderaH4, banderaH5 = false;

    for (var d = 0; d < tablaH4.rows().data().length; d++) {
        var rowH4 = tablaH4.row(d).node();
        var num_docH4 = $(rowH4).children().eq(1).children().val();
        var statusH4 = $(rowH4).children().children().hasClass('rojo');

        tabla4[d] = num_docH4 + statusH4;
    }

    var cont = 0;
    for (var dd = 0; dd < tablaH4.rows().data().length; dd++) {
        var rowH44 = tablaH4.row(dd).node();
        var num_docH44 = $(rowH44).children().eq(1).children().val();

        if (num_doc === num_docH44) {
            cont++;
            //SI TIENE TRUE TIENE ERROR
            if (jQuery.inArray(num_docH44 + true, tabla4) !== -1) {
                banderaH4 = true;
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
    if (cont === 0)
        banderaH4 = false;
    return banderaH4;
}
function validH5(num_doc) {
    var tablaH5 = $('#tab_test5').DataTable();
    var tabla1 = [], tabla2 = [], tabla3 = [], tabla4 = [], tabla5 = [], archivos = [];
    var banderaH1, banderaH2, banderaH3, banderaH4, banderaH5 = false;

    for (var e = 0; e < tablaH5.rows().data().length; e++) {
        var rowH5 = tablaH5.row(e).node();
        var num_docH5 = $(rowH5).children().eq(1).children().val();
        var statusH5 = $(rowH5).children().children().hasClass('rojo');

        tabla5[e] = num_docH5 + statusH5;
    }
    var cont = 0;
    for (var ee = 0; ee < tablaH5.rows().data().length; ee++) {
        var rowH55 = tablaH5.row(ee).node();
        var num_docH55 = $(rowH55).children().eq(1).children().val();

        if (num_doc === num_docH55) {
            cont++;
            //SI TIENE TRUE TIENE ERROR
            if (jQuery.inArray(num_docH55 + true, tabla5) !== -1) {
                banderaH5 = true;
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
    if (cont === 0)
        banderaH5 = false;
    return banderaH5;
}


function clearErrorsN(num_doc) {
    $('#tab_test5 > tbody  > tr').each(function () {
        var num = $(this).find("td:eq(1) input").val();
        if (num === num_doc) {
            var file = $(this).find("td div.file-path-wrapper input");
            var td = $(this).find("td div.file-field");
            var val = $(file).val();
            var i1 = $(file).hasClass("valid") | $(file).hasClass("outRequiredfile") | val !== "";
            var i2 = validaTabs(num_doc, 4);
            if (($(file).hasClass("valid") | $(file).hasClass("outRequiredfile") | val !== "") & validaTabs(num_doc, 4)) {
                //$(td).closest('tr').children().eq(0).children().removeClass("red rojo");
                //$(td).closest('tr').children().eq(0).children().addClass("green");
                //$(td).closest('tr').children().eq(0).children().text("done");
                $(this).children().eq(0).children().removeClass("red");
                $(this).children().eq(0).children().addClass("green");
                $(this).children().eq(0).children().text("Ok");
                //clearErrors();
            } else {
                $(td).closest('tr').children().eq(0).children().removeClass("green");
                $(td).closest('tr').children().eq(0).children().addClass("red");
                $(td).closest('tr').children().eq(0).children().text("Error");
            }
        }
    });
    var bb = validaTabs(num_doc, 5);
    if (bb) {
        bb = !bb;
    }
}