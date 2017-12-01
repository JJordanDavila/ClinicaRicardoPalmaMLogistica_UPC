(function () {
    var NuevaParametro = (function () {
        function NuevaParametro() { };
        NuevaParametro.prototype.loadPage = function () {

            var UnidadMedidaIntervalo = $("#UnidadMedidaIntervalo").val();
            var lista = [];
            var obj = {
                id: 'mm',
                descripcion: 'mm'
            };
            lista.push(obj);
            obj = {
                id: 'hh',
                descripcion: 'hh'
            };
            lista.push(obj);
            obj = {
                id: 'dd',
                descripcion: 'dd'
            };
            lista.push(obj);
            obj = {
                id: 'mi',
                descripcion: 'mi'
            };
            lista.push(obj);
            var opciones = $("#UnidadMedidaIntervalo"); //$("#UnidadMedidaIntervalo").val()
            $.each(lista, function (a, b) {
                opciones.append($("<option />").val(b.id).text(b.descripcion));
            });
            var cbxintervals = $("#cbxMedidasIntervalos"); //$("#UnidadMedidaIntervalo").val()
            $.each(lista, function (a, b) {
                if (UnidadMedidaIntervalo != "") {
                    if (UnidadMedidaIntervalo == b.id) {
                        cbxintervals.append($("<option selected/>").val(b.id).text(b.descripcion));
                    } else {
                        cbxintervals.append($("<option/>").val(b.id).text(b.descripcion));
                    }
                }
            });
            var s = suspender == undefined ? false : suspender;
            if (!s || s == undefined) {
                gInputsFormatoFecha("FecIni,FecFin,FecIniIndex,FecFinIndex, FecUltPro, FecUltProIndex");
            }
            NuevaParametro.prototype.agregarEventos();
        };

        NuevaParametro.prototype.Guardar = function () {
            var valIntervalo = $("#Intervalo").val();
            if (valIntervalo == "") { return gMensajeInformacion("Intervalo no válido."); }
            if (valIntervalo == "0") { return gMensajeInformacion("Intervalo no válido."); }

            var cbo = $("#UnidadMedidaIntervalo").val();
            if (cbo == "") { return gMensajeInformacion("Seleccione una unidad de intervalo válido."); }

            var url1 = $("#UrlServicio01").val();
            if (url1 == "") { return gMensajeInformacion("Debe ingresar el Servicio Web Sunat Correctamente "); }

            var url2 = $("#UrlServicio02").val();
            if (url2 == "") { return gMensajeInformacion("Debe ingresar el Servicio Web Osce Correctamente "); }


            var fini = $("#FecIni").val();
            var ffin = $("#FecFin").val();
            var mensaje = ValidarFechaInicio_Fin(fini, ffin, 30);
            if (mensaje != "") { return gMensajeInformacion(mensaje); }

            gMensajeConfirmacion("¿Esta seguro de registrar los cambios?", function () {
                NuevaParametro.prototype.GuardarParametro();
            });
            return false;
        };

        NuevaParametro.prototype.GuardarParametro = function () {
            var frmData = new FormData(document.getElementById('frmSuspension'));
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
                        gMensajeInformacion('Ocurrio un error de sistema.');
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

            gMensajeConfirmacion("¿Esta seguro de modificar?", function () {
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

        NuevaParametro.prototype.Actualizar= function () {

            var obs = $("#ObservacionSuspension").val();
            if (obs == "") { return gMensajeInformacion("Ingrese una observación."); }

            var valIntervalo = $("#Intervalo").val();
            if (valIntervalo == "") { return gMensajeInformacion("Intervalo no válido."); }
            if (valIntervalo == "0") { return gMensajeInformacion("Intervalo no válido."); }

            var cbo = $("#cbxMedidasIntervalos").val();
            if (cbo == "") { return gMensajeInformacion("Ingrese un intervalo válido."); }

            var url1 = $("#UrlServicio01").val();
            if (url1 == "") { return gMensajeInformacion("Debe ingresar el Servicio Web Sunat Correctamente "); }

            var url2 = $("#UrlServicio02").val();
            if (url2 == "") { return gMensajeInformacion("Debe ingresar el Servicio Web Osce Correctamente "); }

            gMensajeConfirmacion("¿Esta seguro de modificar?", function () {
                var Parametro = $('#frmSuspension').serializeFormJSON();
                $.ajax({
                    url: globalRutaServidor + "Parametro/Actualizar",
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

        NuevaParametro.prototype.Eliminar = function () {

            var obs = $("#ObservacionSuspension").val();
            if (obs == "") { return gMensajeInformacion("Ingrese una observación."); }

            gMensajeConfirmacion("¿Esta seguro de eliminar?", function () {
                var Parametro = $('#frmSuspension').serializeFormJSON();
                $.ajax({
                    url: globalRutaServidor + "Parametro/Eliminar",
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
            $("#btnActualizar").on('click', function () {
                NuevaParametro.prototype.Actualizar();
            });
            $("#btnEliminar").on('click', function () {
                NuevaParametro.prototype.Eliminar();
            });

            $("#Intervalo").keypress(function (e) {
                return (e.keyCode >= 48 && e.keyCode <= 57)
            });
        };
        return NuevaParametro;
    }());
    var nuevaParametro = new NuevaParametro();
    nuevaParametro.loadPage();
}());