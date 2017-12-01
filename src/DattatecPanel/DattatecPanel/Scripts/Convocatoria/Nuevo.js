(function () {
    var NuevaConvocatoria = (function () {
        function NuevaConvocatoria() { };
        NuevaConvocatoria.prototype.loadPage = function () {
            var s = suspender == undefined ? false : suspender;
            if (!s || s == undefined) {
                gInputsFormatoFecha("FechaInicio,FechaFin,fechaInicioIndex,FechaFinIndex");
            }
            NuevaConvocatoria.prototype.agregarEventos();
        };

        NuevaConvocatoria.prototype.Guardar = function () {
            var fini = $("#FechaInicio").val();
            var ffin = $("#FechaFin").val();
            var mensaje = ValidarFechaInicio_Fin(fini, ffin, 30);
            if (mensaje != "") { return gMensajeInformacion(mensaje); }

            gMensajeConfirmacion("¿Esta seguro de registrar?", function () {
                NuevaConvocatoria.prototype.GuardarConvocatoria();
            });
            return false;
        };

        NuevaConvocatoria.prototype.GuardarConvocatoria = function () {
            var frmData = new FormData(document.getElementById('frmNuevoConvocatoria'));
            $.ajax({
                url: globalRutaServidor + "Convocatoria/Nuevo",
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

        NuevaConvocatoria.prototype.GuardarSuspension = function () {

            ////var obs = $("#ObservacionSuspension").val();
            ////if (obs == "") { return gMensajeInformacion("Ingrese una observación."); }

            gMensajeConfirmacion("¿Esta seguro de suspender?", function () {
                var convocatoria = $('#frmSuspension').serializeFormJSON();
                $.ajax({
                    url: globalRutaServidor + "Convocatoria/Suspender",
                    type: 'POST',
                    async: false,
                    data: { entidad: convocatoria },
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
            });
            return false;
        };

        NuevaConvocatoria.prototype.agregarEventos = function () {
            $("#btnGuardar").on('click', function () {
                NuevaConvocatoria.prototype.Guardar();
            });
            $("#btnSuspender").on('click', function () {
                NuevaConvocatoria.prototype.GuardarSuspension();
            });
        };
        return NuevaConvocatoria;
    }());
    var nuevaConvocatoria = new NuevaConvocatoria();
    nuevaConvocatoria.loadPage();
}());