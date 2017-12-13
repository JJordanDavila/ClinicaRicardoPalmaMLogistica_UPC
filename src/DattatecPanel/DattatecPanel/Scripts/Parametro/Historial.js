(function () {
    var Historial = (function () {
        function Historial() { };

        Historial.prototype.loadPage = function () {
            gInputsFormatoFecha("fechaInicio,fechaFin");
            Historial.prototype.dataGrid();
            Historial.prototype.buscar();
            Historial.prototype.agregarEventos();
        };

        Historial.prototype.dataGrid = function () {
            $("#dgHistorial").datagrid({
                title: 'Historial de llamados',
                loadMsg: "Cargando...",
                columns: [[
                    {
                        field: 'Descripcion', title: 'Descripcion', width: 250
                    },
                    {
                        field: 'Fecha', title: 'Fecha', width: 150,
                        formatter: function (value, row, index) {
                            return gFormatearFechaJson(value);
                        }
                    }
                ]],
                width: '100%',
                singleSelect: true,
                pagination: true
            });

            var pager = $('#dgHistorial').datagrid('getPager');
            $(pager).pagination({
                pageSize: 10,
                showPageList: true,
                pageList: [10, 20, 30, 40, 50],
                beforePageText: 'Página',
                afterPageText: 'de {pages}',
                displayMsg: 'Mostrando del {from} al {to} de los {total} resultados',
                onSelectPage: function (pageNumber, pageSize) {
                    Historial.prototype.buscar();
                }
            });
        };

        Historial.prototype.buscar = function () {
            var pageNumber_ = $('#dgHistorial').datagrid('getPager').pagination('options').pageNumber;
            var pageSize_ = $('#dgHistorial').datagrid('getPager').pagination('options').pageSize;

            $.ajax({
                url: globalRutaServidor + "Parametro/ListarHistorial",
                type: 'GET',
                data: {
                    fini: $("#fechaInicio").val(),
                    ffin: $("#fechaFin").val(),
                    page: pageNumber_,  
                    pageSize: pageSize_
                },
                success: function (data) {
                    gMostrarResultadoBusqueda(data.rows, "#dgHistorial");

                    $('#dgHistorial').datagrid('getPager').pagination({
                        total: data.total == 0 ? 1 : data.total,
                        pageSize: pageSize_,
                        pageNumber: pageNumber_
                    });
                },
                error: function () {
                    gMensajeErrorAjax();
                }
            });
        };

        Historial.prototype.agregarEventos = function () {
            $("#btnConsultarHistorial").on('click', function () {
                Historial.prototype.buscar();
            });
        };

        return Historial;
    }());
    var historial = new Historial();
    historial.loadPage();
}());