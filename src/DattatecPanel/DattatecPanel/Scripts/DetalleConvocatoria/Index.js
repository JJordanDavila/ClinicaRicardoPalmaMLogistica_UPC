(function () {
    var DetalleConvocatoria = (function () {
        function DetalleConvocatoria() { };
        DetalleConvocatoria.prototype.PageLoad = function () {
            gInputsFormatoFecha("Fecha_Registro,Fecha_RegistroIndex");
            DetalleConvocatoria.prototype.dataGrid();
            DetalleConvocatoria.prototype.buscar();
            DetalleConvocatoria.prototype.agregarEventos();
        }

        DetalleConvocatoria.prototype.dataGrid = function () {
            $("#dgDetalleConvocatoriaPostulantes").datagrid({
                title: 'RESULTADO',
                loadMsg: "Cargando...",
                columns: [[
                    {
                        field: 'Numero', title: 'N° Convocatoria', align: 'center', width: 200
                    },
                    {
                        field: 'RUC', title: 'Ruc', align: 'center', width: 150
                    },
                    {
                        field: 'RazonSocial', title: 'Razón Social', align: 'left', width: 300
                    },
                    {
                        field: 'Descripcion', title: 'Rubro', align: 'center', width: 200
                    },
                    {
                        field: 'Fecha_Registro', title: 'Fecha de Registro', align: 'center', width: 150,
                        formatter: function (value, row, index) {
                            return gFormatearFechaJson(value);
                        }
                    },
                    {
                        field: 'action', title: 'Opciones', width: 100, align: 'center',
                        formatter: function (value, row, index) {
                            var a = '<a href="' + globalRutaServidor + 'DetalleConvocatoria/Validar/' + row.ConvocatoriaId + '/' + row.PostulanteId + '" ><span class="glyphicon glyphicon-pencil opciones" title="Validar"></span></a>';
                            var b = '<a href="' + globalRutaServidor + 'DetalleConvocatoria/Rechazar/' + row.ConvocatoriaId + '/' + row.PostulanteId + '" ><span class="glyphicon glyphicon-remove opciones" title="Rechazar"></span></a>';
                            return a + b;
                        }
                    }
                ]],
                width: '100%',
                singleSelect: true,
                rownumbers: true,
                pagination: true
            });

            var pager = $('#dgDetalleConvocatoriaPostulantes').datagrid('getPager');
            $(pager).pagination({
                pageSize: 10,
                showPageList: true,
                pageList: [10, 20, 30, 40, 50],
                beforePageText: 'Página',
                afterPageText: 'de {pages}',
                displayMsg: 'Mostrando del {from} al {to} de los {total} resultados',
                onSelectPage: function (pageNumber, pageSize) {
                    DetalleConvocatoria.prototype.buscar();
                }
            });
        };

        DetalleConvocatoria.prototype.buscar = function () {

            var pageNumber_ = $('#dgDetalleConvocatoriaPostulantes').datagrid('getPager').pagination('options').pageNumber;
            var pageSize_ = $('#dgDetalleConvocatoriaPostulantes').datagrid('getPager').pagination('options').pageSize;

            $.ajax({
                url: globalRutaServidor + "DetalleConvocatoria/ListarDetalleConvocatoriaPostulante",
                type: 'GET',
                data: {
                    numeroConvocatoria: $("#numeroConvocatoria").val(),
                    ruc: $("#RUC").val(),
                    razonSocial: $("#razonSocial").val(),
                    page: pageNumber_,
                    pageSize: pageSize_
                },
                success: function (data) {
                    gMostrarResultadoBusqueda(data.rows, "#dgDetalleConvocatoriaPostulantes");

                    $('#dgDetalleConvocatoriaPostulantes').datagrid('getPager').pagination({
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

        DetalleConvocatoria.prototype.agregarEventos = function () {
            $("#btnConsultar").on('click', function () {
                DetalleConvocatoria.prototype.buscar();
            });
            $("#RUC").keypress(function (e) {
                return (e.keyCode >= 48 && e.keyCode <= 57)
            });
            $("#numeroConvocatoria").keypress(function (e) {
                return (e.keyCode >= 48 && e.keyCode <= 57)
            });
        };

        return DetalleConvocatoria;
    }());

    var DetalleConvocatoria = new DetalleConvocatoria();
    DetalleConvocatoria.PageLoad();

}());