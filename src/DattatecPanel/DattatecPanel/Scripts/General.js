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
function ValidarFechaInicio_Fin(fini, ffin, dias) {
    var mensaje = "";
    if (fini != "" && ffin != "") {
        var fechaini = new Date(formatdate(fini));
        var fechafin = new Date(formatdate(ffin));
        var diff = fechafin - fechaini;
        if (fechaini > fechafin) {
            //mensaje = fechaini; // fechaini;
            mensaje = "La fecha de inicio no puede ser mayor a la fecha fin.";
        }
        else {
            if (fechafin < fechaini) {
                //mensaje = fechafin;
                mensaje = "La fecha fin no puede ser menor a la fecha de inicio.";
            }
            {
                if ((diff / (1000 * 60 * 60 * 24)) < dias) {
                    //mensaje = fechaini + " | " + fechafin;
                    mensaje = "La diferencia entre la fecha inicio y fecha fin debe ser igual o mayor a " + dias + " días.";
                }
            }
        }
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

//Mostrar modales de mensajes con una funcion callback
function gMensajeInformacionConCallback(mensaje, callback) {
    bootbox.alert(mensaje, function () {
        callback();
    });
};

function base64ToArrayBuffer(base64) {
    var binaryString = window.atob(base64);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);
    for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
}

function saveByteArray8(reportName, byte) {
    var link = document.createElement('a');
    document.body.appendChild(link);
    link.style = "display: none";
    var blob = new Blob([byte], { type: "application/octet-stream" });
    var url = window.URL.createObjectURL(blob);

    link.href = url;
    var fileName = reportName + ".pdf";
    link.download = fileName;
    link.click();

    // document.body.appendChild(link);
    //window.URL.revokeObjectURL(url);



};

function saveByteArray() {
    var a = document.createElement("a");
    document.body.appendChild(a);
    a.style = "display: none";
    return function (data, name) {
        var blob = new Blob(data, { type: "octet/stream" }),
            url = window.URL.createObjectURL(blob);
        a.href = url;
        a.download = name;
        a.click();
        window.URL.revokeObjectURL(url);
    };
};

function saveByteArray5(reportName, byte) {
    var a = document.createElement("a");
    document.body.appendChild(a);
    a.style.display = "none";

    // IE
    if (window.navigator.msSaveOrOpenBlob) {
        a.onclick = ((evx) => {
            var newBlob = new Blob([new Uint8Array(xhttpGetPack.response)]);
            window.navigator.msSaveOrOpenBlob(newBlob, reportName + ".pdf");
        });
        a.click();
    }
    else //Chrome and safari
    {
        var file = URL.createObjectURL(xhttpGetPack.response);
        a.href = file;
        a["download"] = reportName + ".pdf";
        a.click();
        window.URL.revokeObjectURL(file);
    }
};




function DescargarPDFPorArrayBytes(arrayBytes, name) {
    var bytes = base64ToArrayBuffer(arrayBytes);
    saveByteArray(name, bytes);
};


function ValidarPostulante(nuevoRUC, nuevaRazonSocial, nuevaDireccion, nuevoCorreo) {

    var mensaje = "";

    if (trim(nuevoRUC) == "") {
        mensaje = "Ingrese un RUC válido.";
    }
    else {
        if (trim(nuevoRUC).length != 11) {
            mensaje = "El RUC debe contener 11 dígitos.";
        }
        else {
            if (trim(nuevaRazonSocial) == "") {
                mensaje = "Ingrese una Razón Social válida.";
            }
            else {
                if (trim(nuevaDireccion).length == 0) {
                    mensaje = "Ingrese una Dirección válida.";
                }
                else {
                    if (trim(nuevoCorreo).length == 0) {
                        mensaje = "Ingrese un correo electrónico válido.";
                    }
                }
            }
        }
    }

    return mensaje;
};


function EncriptarTexto(query) {
    var encodedData = window.btoa(query);

    return encodedData;
}
