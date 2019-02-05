function toast(clases, dura, icon, mensaje, color, aceptar) {
    dismiss(clases);
    M.toast({
        classes: clases,
        displayLength: dura,
        html: '<span style="padding-right:15px;"><i class="material-icons ' + color + '-text">' + icon + '</i></span>  ' + mensaje
            + '<button class="btn-small btn-flat toast-action" onclick="dismiss(\'toast\')">'+aceptar+'</button> '
    });
}

function dismiss(clase) {
    var toastElement = document.querySelectorAll('.' + clase);
    for (var i = 0; i < toastElement.length; i++) {
        var toastInstance = M.Toast.getInstance(toastElement[i]);
        toastInstance.dismiss();
    }
}