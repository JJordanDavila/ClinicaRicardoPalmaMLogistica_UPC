(function() {
    var EvaluarProveedor = (function () {
        function EvaluarProveedor() { };

        EvaluarProveedor.prototype.loadPage = function () {
            gInputsFormatoFecha("txtFechaFin,txtFechaInicio");
            EvaluarProveedor.prototype.dataGrid();
            EvaluarProveedor.prototype.buscar();
            //EvaluarProveedor.prototype.agregarEventos();
            $("#btnConsultar").on('click', function () {
                EvaluarProveedor.prototype.buscar();
            });
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
                            var result = "";
                            if (value == "SU") {
                                result = "<input type='checkbox' class='itemChkEstado' data-id='" + row.IdProveedor+",CA"+"' checked/> Suspendido";
                            } else {
                                result = "<input type='checkbox' class='itemChkEstado' data-id='" + row.IdProveedor + ",SU" + "'/> Activo";
                            }
                            return result;
                        }
                        //editor: {
                        //    type: 'checkbox',
                        //    options: {
                        //        on: 'SU',
                        //        off: 'AC'
                        //    },
                        //    onchange: function (a,b) {
                        //        k = b;
                        //    }
                        //}
                    },
                    {
                        field: 'Observacion', title: 'Observaciones', width: 200
                    }
                ]],
                //onClickRow: function (index, row) {
                //    $('#dgListaEvaluarProveedor').datagrid('beginEdit', index);
                //}
                width: '100%',
                singleSelect: true
            });
        };

        EvaluarProveedor.prototype.buscar = function () {

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

        EvaluarProveedor.prototype.getRowIndex = function (target) {
            var tr = $(target).closest('tr.datagrid-row');
            return parseInt(tr.attr('datagrid-row-index'));
        };        EvaluarProveedor.prototype.ActualizarEstado = function (id, estado) {            var request = {};            request.idProveedor = id;
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
        }
        return EvaluarProveedor;
    }());
    var evaluarProveedor = new EvaluarProveedor();
    evaluarProveedor.loadPage();
}());