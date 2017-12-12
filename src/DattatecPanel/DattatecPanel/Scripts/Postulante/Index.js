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
                singleSelect: true,
                rownumbers: true,
                pagination: true
            });

            var pager = $('#dgListadoConvocatorias').datagrid('getPager');
            $(pager).pagination({
                pageSize: 10,
                showPageList: true,
                pageList: [10, 20, 30, 40, 50],
                beforePageText: 'Página',
                afterPageText: 'de {pages}',
                displayMsg: 'Mostrando del {from} al {to} de los {total} resultados',
                onSelectPage: function (pageNumber, pageSize) {
                    Postulante.prototype.buscar();
                }
            });
        };

        Postulante.prototype.buscar = function () {

            var pageNumber_ = $('#dgListadoConvocatorias').datagrid('getPager').pagination('options').pageNumber;
            var pageSize_ = $('#dgListadoConvocatorias').datagrid('getPager').pagination('options').pageSize;

            $.ajax({
                url: globalRutaServidor + "Postulante/ListarConvocatorias",
                type: 'GET',
                data: {
                    page: pageNumber_,
                    pageSize: pageSize_
                },
                success: function (data) {
                    gMostrarResultadoBusqueda(data.rows, "#dgListadoConvocatorias");

                    $('#dgListadoConvocatorias').datagrid('getPager').pagination({
                        total: data.total == 0 ? 1 : data.total,
                        pageSize: pageSize_,
                        pageNumber: pageNumber_
                    });
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