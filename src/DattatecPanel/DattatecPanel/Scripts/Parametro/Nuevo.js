(function () {
    var NuevaParametro = (function () {
        function NuevaParametro() { };
        NuevaParametro.prototype.loadPage = function () {
            var s = suspender == undefined ? false : suspender;
            if (!s || s == undefined) {
                gInputsFormatoFecha("FecIni,FecFin,FecIniIndex,FecFinIndex");
            }
            NuevaParametro.prototype.agregarEventos();
        };

        NuevaParametro.prototype.Guardar = function () {
            var fini = $("#FecIni").val();
            var ffin = $("#FecFin").val();
            var mensaje = ValidarFechaInicio_Fin(fini, ffin, 30);
            if (mensaje != "") { return gMensajeInformacion(mensaje); }

            gMensajeConfirmacion("¿Esta seguro de registrar?", function () {
                NuevaParametro.prototype.GuardarParametro();
            });
            return false;
        };

        NuevaParametro.prototype.GuardarParametro = function () {
            var frmData = new FormData(document.getElementById('frmNuevoParametro'));
            $.ajax({
                url: globalRutaServidor + "Parametro/Nuevo",
                type: 'POST',
                async: false,
                contentType: false,
                processData: false,
                data: frmData,
                success: function (data) {
                    if (data.statusCode == 200) {
                        if (data.mensajeInfo == "") {
                            var callback = function () {
                                $("#btnCancelar").click();
                            };
                            gMensajeInformacionConCallback(data.mensaje, callback);
                        } else {
                            gMensajeInformacion(data.mensajeInfo);
                        }
                    } else {
                        gMensajeInformacion('Ocurrio un error.');
                    }
                },
                error: function () {
                    gMensajeErrorAjax();
                }
            });
        };

        NuevaParametro.prototype.GuardarSuspension = function () {

            var obs = $("#ObservacionSuspension").val();
            if (obs == "") { return gMensajeInformacion("Ingrese una observación."); }

            gMensajeConfirmacion("¿Esta seguro de suspender?", function () {
                var Parametro = $('#frmSuspension').serializeFormJSON();
                $.ajax({
                    url: globalRutaServidor + "Parametro/Suspender",
                    type: 'POST',
                    async: false,
                    data: { entidad: Parametro },
                    success: function (data) {
                        if (data.statusCode == 200) {
                            var callback = function () {
                                $("#btnCancelar").click();
                            };
                            gMensajeInformacionConCallback(data.mensaje, callback);
                        } else {
                            gMensajeInformacion('Ocurrio un error.');
                        }
                    },
                    error: function () {
                        gMensajeErrorAjax();
                    }
                });
            });
            return false;
        };

        NuevaParametro.prototype.agregarEventos = function () {
            $("#btnGuardar").on('click', function () {
                NuevaParametro.prototype.Guardar();
            });
            $("#btnSuspender").on('click', function () {
                NuevaParametro.prototype.GuardarSuspension();
            });
        };
        return NuevaParametro;
    }());
    var nuevaParametro = new NuevaParametro();
    nuevaParametro.loadPage();
}());