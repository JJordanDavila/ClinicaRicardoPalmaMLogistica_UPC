//Funcion para convertir de Serializado a Json
(function ($) {
    $.fn.serializeFormJSON = function () {

        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };
})(jQuery);

function gFormatearFechaJson(fecha) {
    if (fecha != null) {
        var value = new Date(parseInt(fecha.replace("/Date(", "").replace(")/", ""), 10));
        var date = new Date(value), mnth = ("0" + (date.getMonth() + 1)).slice(-2), day = ("0" + date.getDate()).slice(-2);
        return [day, mnth, date.getFullYear()].join("/");
    }
}

function gMensajeErrorAjax() {
    alert("Ocurrió un error en el sistema, pónganse en contacto con el administrador del sistema.");
};

function gMostrarResultadoBusqueda(lista, idDatagrid) {
    if (lista.length > 0) {
        //$(idDatagrid).datagrid({ data: lista });
        $(idDatagrid).datagrid('loadData', lista);
    } else {
        $(idDatagrid).datagrid('unselectAll');
        //$(idDatagrid).datagrid({ data: [] });
        $(idDatagrid).datagrid('loadData', { "total": 0, "rows": [] });
    }
};

function gAbrirModal(contenido) {
    $(".seccionModal").html(contenido);
    $("#contenedorModal").modal("show");
};