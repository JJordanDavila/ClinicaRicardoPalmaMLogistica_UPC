(function() {
    var EvaluarProveedor = (function () {
        function EvaluarProveedor() { };

        EvaluarProveedor.prototype.loadPage = function () {
            gInputsFormatoFecha("txtFechaFin,txtFechaInicio");
            EvaluarProveedor.prototype.dataGrid();
            EvaluarProveedor.prototype.buscar();
            EvaluarProveedor.prototype.agregarEventos();
        };

        EvaluarProveedor.prototype.dataGrid = function () {
            $("#dgListaEvaluarProveedor").datagrid({
                title: 'RESULTADO',
                loadMsg: "Cargando...",
                columns: [[
                    {
                        field: 'RUC', title: 'RUC', align: 'center', width: 100
                    },
                    {
                        field: 'RazonSocial', title: 'Razon Social', width: 200
                    },
                    {
                        field: 'Rubro', title: 'Rubro', width: 250
                    },
                    {
                        field: 'C1', title: 'C1', width: 50
                    },
                    {
                        field: 'C2', title: 'C2', width: 50
                    },
                    {
                        field: 'C3', title: 'C3', width: 50
                    },
                    {
                        field: 'C4', title: 'C4', width: 50
                    },
                    {
                        field: 'Estado', title: 'Estado', align: 'center', width: 150,
                        formatter: function (value, row, index) {
                            var puedeSuspender = 0;
                            if (row.C1 <= 3) {
                                puedeSuspender = puedeSuspender + 1;
                            }
                            if (row.C2 <= 3) {
                                puedeSuspender = puedeSuspender + 1;
                            }
                            if (row.C3 <= 3) {
                                puedeSuspender = puedeSuspender + 1;
                            }
                            if (row.C4 <= 3) {
                                puedeSuspender = puedeSuspender + 1;
                            }
                            var result = "";
                            if (value == "SU") {
                                result = "<input type='checkbox' class='itemChkEstado' data-id='" + row.IdProveedor + ",CA" + "' checked disabled/> Suspendido";
                            } else {
                                if (puedeSuspender >= 2) {
                                    result = "<input type='checkbox' class='itemChkEstado' data-id='" + row.IdProveedor + ",SU" + "'/> Activo";
                                } else {
                                    result = "<input type='checkbox' class='itemChkEstado' disabled/> Activo";
                                }
                            }
                            return result;
                        }
                    },
                    {
                        field: 'Observacion', title: 'Observaciones', width: 200
                    }
                ]],
                rownumbers: true,
                width: '100%',
                singleSelect: true
            });
        };

        EvaluarProveedor.prototype.buscar = function () {

            var fini = $("#txtFechaInicio").val();
            var ffin = $("#txtFechaFin").val();
            var mensaje = ValidarFechaInicio_Fin(fini, ffin, 60);
            if (mensaje != "") { return gMensajeInformacion(mensaje); }

            var request = {};
            request.RUC = $("#txtRUC").val();
            request.RazonSocial = $("#txtRazonSocial").val();

            $.ajax({
                url: globalRutaServidor + "EvaluarProveedor/ListarProveedores",
                type: 'GET',
                async: false,
                data: request,
                success: function (data) {
                    gMostrarResultadoBusqueda(data.rows, "#dgListaEvaluarProveedor");

                    $(".itemChkEstado").on('click', function () {
                        //EvaluarProveedor.prototype.frmObservacion();
                        var data = $(this).attr("data-id");
                        var params = data.split(",");
                        EvaluarProveedor.prototype.ActualizarEstado(params[0], params[1]);
                    });
                },
                error: function () {
                    gMensajeErrorAjax();
                }
            });
        };

        EvaluarProveedor.prototype.agregarEventos = function () {
            $("#btnConsultar").on('click', function () {
                EvaluarProveedor.prototype.buscar();
            });
        };
        EvaluarProveedor.prototype.ActualizarEstado = function (id, estado) {            var request = {};            request.idProveedor = id;
            request.Estado = estado;            $.ajax({
                url: globalRutaServidor + "EvaluarProveedor/ActualizarEstadoProveedor",
                type: 'POST',
                data: request,
                success: function (data) {
                    if (data.statusCode == 400) {
                        gMensajeInformacion(data.mensaje);
                    } else if (data.statusCode == 200) {
                        EvaluarProveedor.prototype.buscar();
                    }

                },
                error: function () {
                    gMensajeErrorAjax();
                }
            });
        };        EvaluarProveedor.prototype.frmObservacion = function () {
            $.ajax({
                url: globalRutaServidor + "EvaluarProveedor/Observacion",
                type: "GET",
                success: function (contenido) {
                    gAbrirModal(contenido);

                    $('#btnGuardarEstado').on('click', EvaluarProveedor.prototype.guardarEstado);
                },
                error: function (request, status, error) {
                    gMensajeErrorAjax();
                }
            });
            return false;
        };        EvaluarProveedor.prototype.guardarEstado = function () {

            var archivoCargaMasiva = new FormData(document.getElementById('frmCargaMasiva_PlantaExterna'));

            $.ajax({
                url: globalRutaServidor + "PlantaExterna/PlantaExterna/CargaMasiva",
                type: 'post',
                data: archivoCargaMasiva,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data.statusCode == 400) {
                        gMensajeInformacion(data.mensaje);
                    } else if (data.statusCode == 200) {
                        var callback = function () {
                            gCerrarModal();
                            PlantaExterna.prototype.buscar();
                        };
                        gMensajeInformacionConCallback(data.mensaje, callback);
                    } else {
                        gMensajeErrorAjax();
                    }
                },
                error: function () {
                    gMensajeErrorAjax();
                }
            });

            return false;
        };
        return EvaluarProveedor;
    }());
    var evaluarProveedor = new EvaluarProveedor();
    evaluarProveedor.loadPage();
}());