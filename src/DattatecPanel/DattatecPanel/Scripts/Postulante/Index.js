(function () {
    var Postulante = (function () {
        function Postulante() { };
        Postulante.prototype.loadPage = function () {
            Postulante.prototype.dataGrid();
            Postulante.prototype.buscar();
        };
        Postulante.prototype.dataGrid = function () {
            $("#dgListadoConvocatorias").datagrid({
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
                        field: 'Estado', title: 'Estado', align: 'center', width: 100,
                        formatter: function (value, row, index) {
                            return value.trim() == "E" ? "Emitido" : "Suspendido";
                        }
                    },
                    {
                        field: 'action', title: 'Opciones', width: 100, align: 'center',
                        formatter: function (value, row, index) {
                            var a = '<a href="' + globalRutaServidor + 'Postulante/RegistrarPostulante/' + row.Convocatoriaid + '" ><span class="glyphicon glyphicon-ok opciones" title="Seleccionar"></span></a>';
                            return a;
                        }
                    }
                ]],
                width: '100%',
                singleSelect: true
            });
        };

        Postulante.prototype.buscar = function () {
            $.ajax({
                url: globalRutaServidor + "Postulante/ListarConvocatorias",
                type: 'GET',
                data: null,
                success: function (data) {
                    gMostrarResultadoBusqueda(data.rows, "#dgListadoConvocatorias");
                },
                error: function () {
                    gMensajeErrorAjax();
                }
            });
        }

        return Postulante;
    }());

    var postulante = new Postulante();
    postulante.loadPage();
}());