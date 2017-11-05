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

//Cargar input en formato fecha
//se debe enviar el parametro: "idInput1,idInput2,..."
function gInputsFormatoFecha(ids) {
    var listaInput = ids.split(",");

    for (var i = 0; i < listaInput.length; i++) {
        $('#' + trim(listaInput[i])).datepicker({
            dateFormat: "dd/mm/yy",
            changeMonth: true,
            changeYear: true
        });
    }
};

//Elimina campos vacios
function trim(value) {
    return value.replace(/^\s+|\s+$/g, "");
};

//Validar si la fecha Inicio es mayor a la fecha fin o viveversa
function ValidarFechaInicio_Fin(fini, ffin) {
    var mensaje = "";
    var fechaini = new Date(formatdate(fini));
    var fechafin = new Date(formatdate(ffin));
    if (fechaini > fechafin) {
        mensaje = "La fecha de inicio no puede ser mayor a la fecha fin.";
    }
    if (fechafin < fechaini) {
        mensaje = "La fecha fin no puede ser menor a la fecha de inicio.";
    }
    return mensaje;
};

function formatdate(fecha) {
    var result = "";
    var partsDate = fecha.split("/");
    return result = partsDate[1] + "-" + partsDate[0] + "-" + partsDate[2]
}

function dialogoConfirmacion(dialogText, okFunc, dialogTitle) {
    $('<div style="padding: 10px; max-width: 500px; word-wrap: break-word;">' + dialogText + '</div>').dialog({
        draggable: false,
        modal: true,
        resizable: false,
        width: 'auto',
        title: dialogTitle || 'Confirm',
        minHeight: 75,
        buttons: {
            OK: function () {
                if (typeof (okFunc) == 'function') {
                    setTimeout(okFunc, 50);
                }
                $(this).dialog('destroy');
            },
            Cancel: function () {
                $(this).dialog('destroy');
            }
        }
    });
}

function gMensajeConfirmacion(mensaje, llamada) {
    bootbox.confirm({
        message: mensaje,
        buttons: {
            confirm: {
                label: '&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp Si &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp',
                className: 'btn-success'
            },
            cancel: {
                label: '&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp No &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp',
                className: 'btn-danger'
            }
        },
        callback: function (result) {
            if (result) {
                llamada();
            }
        }
    });
};

function gMensajeInformacion(mensaje) {
    bootbox.alert(mensaje);
};