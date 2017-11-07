(function () {
    var Convocatoria = (function () {
        function Convocatoria() { };
        Convocatoria.prototype.PageLoad = function () {
            gInputsFormatoFecha("FechaInicio,FechaFin,fechaInicioIndex,FechaFinIndex");
            Convocatoria.prototype.dataGrid();
            Convocatoria.prototype.buscar();
            Convocatoria.prototype.agregarEventos();
        }

        Convocatoria.prototype.dataGrid = function () {
            $("#dgConvocatoriaProveedores").datagrid({
                title: 'RESULTADO',
                loadMsg: "Cargando...",
                columns: [[
                    {
                        field: 'Numero', title: 'Número', align: 'center', width: 200
                    },
                    {
                        field: 'FechaInicio', title: 'Fecha Inicio', width: 150,
                        formatter: function (value, row, index) {
                            return gFormatearFechaJson(value);
                        }
                    },
                    {
                        field: 'FechaFin', title: 'Fecha Fin', width: 150,
                        formatter: function (value, row, index) {
                            return gFormatearFechaJson(value);
                        }
                    },
                    {
                        field: 'Descripcion', title: 'Rubro', width: 150
                    },
                    {
                        field: 'NombreCompleto', title: 'Solicitante', width: 250
                    },
                    {
                        field: 'Estado', title: 'Estado', align: 'center', width: 100
                    },
                    {
                        field: 'action', title: 'Opciones', width: 100, align: 'center',
                        formatter: function (value, row, index) {
                            var a = '<a href="' + globalRutaServidor + 'Convocatoria/Actualizar/' + row.Convocatoriaid + '" ><span class="glyphicon glyphicon-pencil opciones" title="Modificar"></span></a>';
                            var b = '<a href="' + globalRutaServidor + 'Convocatoria/Suspender/' + row.Convocatoriaid + '" ><span class="glyphicon glyphicon-remove opciones" title="Suspender"></span></a>';
                            return a + b;
                        }
                    }
                ]],
                width: '100%',
                singleSelect: true
            });
        }

        Convocatoria.prototype.buscar = function () {
            $.ajax({
                url: globalRutaServidor + "Convocatoria/ListarConvocatoriaProveedores",
                type: 'GET',
                data: {
                    numero: $("#nroConvocatoria").val(),
                    fini: $("#fechaInicioIndex").val(),
                    ffin: $("#FechaFinIndex").val()
                },
                success: function (data) {
                    gMostrarResultadoBusqueda(data.rows, "#dgConvocatoriaProveedores");

                    $(".itemSuspender").on('click', function () {
                        Convocatoria.prototype.frmSuspender();
                    });
                },
                error: function () {
                    gMensajeErrorAjax();
                }
            });
        }

        Convocatoria.prototype.Guardar = function () {
            gMensajeConfirmacion("¿Esta seguro de registrar?", function () {
                Convocatoria.prototype.GuardarConvocatoria();
            });
            return false;
        };

        Convocatoria.prototype.GuardarConvocatoria = function () {

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
                        gMensajeInformacion(data.mensaje);
                        $("#btnCancelar").click();
                    } else {
                        gMensajeInformacion('Ocurrio un error.');
                    }
                },
                error: function () {
                    gMensajeErrorAjax();
                }
            });
        };

        Convocatoria.prototype.agregarEventos = function () {
            $("#btnGuardar").on('click', function () {
                Convocatoria.prototype.Guardar();
            });
            $("#btnConsultar").on('click', function () {
                Convocatoria.prototype.buscar();
            });
            $("#btnSuspender").on('click', function () {
                Convocatoria.prototype.GuardarSuspension();
            });
        }

        Convocatoria.prototype.frmSuspender = function () {
            $.ajax({
                url: globalRutaServidor + "Convocatoria/Suspender",
                type: "GET",
                data: null,
                success: function (contenido) {
                    gAbrirModal(contenido);
                },
                error: function (request, status, error) {
                    gMensajeErrorAjax();
                }
            });
            return false;
        };

        Convocatoria.prototype.GuardarSuspension = function () {

            var convocatoria = $('#frmSuspension').serializeFormJSON();
            $.ajax({
                url: globalRutaServidor + "Convocatoria/Suspender",
                type: 'POST',
                data: { entidad: convocatoria },
                success: function (data) {
                    if (data.statusCode == 200) {
                        gMensajeInformacion(data.mensaje);
                    } else {
                        gMensajeInformacion('Ocurrio un error.');
                    }
                },
                error: function () {
                    gMensajeErrorAjax();
                }
            });

            return false;
        };

        return Convocatoria;
    }());

    var convocatoria = new Convocatoria();
    convocatoria.PageLoad();
}());