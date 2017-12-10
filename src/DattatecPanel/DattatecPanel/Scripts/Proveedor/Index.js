(function () {
    var Proveedor = (function () {
        function Proveedor() { };
        Proveedor.prototype.PageLoad = function () {
            Proveedor.prototype.dataGrid();
            Proveedor.prototype.buscar();
            Proveedor.prototype.agregarEventos();
        }

        Proveedor.prototype.dataGrid = function () {
            $("#dgProveedores").datagrid({
                title: 'RESULTADO',
                loadMsg: "Cargando...",
                columns: [[
                    {
                        field: 'ProveedorID', title: 'Id', align: 'center', width: 50
                    },
                    {
                        field: 'RazonSocial', title: 'Razón Social', width: 300,
                    },
                    {
                        field: 'RUC', title: 'RUC', width: 150,
                    },
                    {
                        field: 'ObservacionesSuspension', title: 'Observación', width: 300
                    },
                    {
                        field: 'Estado', title: 'Estado', align: 'center', width: 100,
                        formatter: function (value, row, index) {
                            return value.trim() == "AC" ? "ACTIVO" : "SUSPENDIDO";
                        }
                    },
                    {
                        field: 'action', title: 'Opciones', width: 100, align: 'center',
                        formatter: function (value, row, index) {
                            var a = '<a href="' + globalRutaServidor + 'Proveedor/HistorialProveedor?proveedorId=' + row.ProveedorID + '" ><span class="glyphicon glyphicon-pencil opciones" title="Ver Historial"></span></a>';
                            return a;
                        }
                    }
                ]],
                width: '100%',
                singleSelect: true,
                rownumbers: true,
                pagination: true
            });

            var pager = $('#dgProveedores').datagrid('getPager');
            $(pager).pagination({
                pageSize: 10,
                showPageList: true,
                pageList: [10, 20, 30, 40, 50],
                beforePageText: 'Página',
                afterPageText: 'de {pages}',
                displayMsg: 'Mostrando del {from} al {to} de los {total} resultados',
                onSelectPage: function (pageNumber, pageSize) {
                    Proveedor.prototype.buscar();
                }
            });
        };

        Proveedor.prototype.buscar = function () {
            var ruc = $("#rucIndex").val();
            if (ruc != undefined && ruc.trim() != "") {
                if (isNaN(ruc.trim())) {
                    return gMensajeInformacion("Solo se admiten numeros en el campo número de proveedor.");
                }
            }

            var pageNumber_ = $('#dgProveedores').datagrid('getPager').pagination('options').pageNumber;
            var pageSize_ = $('#dgProveedores').datagrid('getPager').pagination('options').pageSize;

            $.ajax({
                url: globalRutaServidor + "Proveedor/ListarProveedor",
                type: 'GET',
                data: {
                    ruc: $("#rucIndex").val(),
                    razonSocial: $("#razonSocialIndex").val()
                },
                success: function (data) {
                    gMostrarResultadoBusqueda(data.rows, "#dgProveedores");

                    $('#dgProveedores').datagrid('getPager').pagination({
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

        Proveedor.prototype.agregarEventos = function () {
            $("#btnConsultar").on('click', function () {
                Proveedor.prototype.buscar();
            });
            $("#rucIndex").keypress(function (e) {
                return (e.keyCode >= 48 && e.keyCode <= 57)
            });
        };

        return Proveedor;
    }());

    var proveedor = new Proveedor();
    proveedor.PageLoad();
}());