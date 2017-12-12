(function () {
    var Parametro = (function () {
        function Parametro() { };
        Parametro.prototype.PageLoad = function () {
            //var s = suspender == undefined ? false : suspender;
            //if (!s || s == undefined) {
            gInputsFormatoFecha("FecIni,FecFin");
            //}
            Parametro.prototype.dataGrid();
            Parametro.prototype.buscar();
            Parametro.prototype.agregarEventos();
        }

        Parametro.prototype.dataGrid = function () {
            $("#dgParametros").datagrid({
                title: 'RESULTADO',
                loadMsg: "Cargando...",
                columns: [[
                    {
                        field: 'FecIni', title: 'Fecha Inicio', width: 90,
                        formatter: function (value, row, index) {
                            return gFormatearFechaJson(value);
                        }
                    },
                    {
                        field: 'FecFin', title: 'Fecha Fin', width: 90,
                        formatter: function (value, row, index) {
                            return gFormatearFechaJson(value);
                        }
                    },
                    {
                        field: 'Intervalo', title: 'Intervalo', width: 80
                    },
                    {
                        field: 'UnidadMedidaIntervalo', title: 'UnidadMedidaIntervalo', width: 100
                    },
                    {
                        field: 'FecUltPro', title: 'Fecha Ult. Proceso', width: 100,
                        formatter: function (value, row, index) {
                            return gFormatearFechaJson(value);
                        }
                    },
                    {
                        field: 'UrlServicio01', title: 'Servicio Sunat', width: 250
                    },
                    {
                        field: 'UrlServicio02', title: 'Servicio Osce', width: 250
                    },

                    {
                        field: 'EstadoServicioSUNAT', title: 'Servicio SUNAT', width: 100
                    },
                    {
                        field: 'EstadoServicioOSCE', title: 'Servicio OSCE', width: 100
                    },
                    //{
                    //    field: 'Estado', title: 'Estado', align: 'center', width: 100,
                    //    formatter: function (value, row, index) {
                    //        return value.trim() == "E" ? "Emitido" : "Suspendido";
                    //    }
                    //},
                    {
                        field: 'action', title: 'Opciones', width: 90, align: 'center',
                        formatter: function (value, row, index) {
                            var a = '<a href="' + globalRutaServidor + 'Parametro/Actualizar/' + row.ParametroId + '" ><span class="glyphicon glyphicon-pencil opciones" title="Modificar"></span></a>';
                            var b = '<a href="' + globalRutaServidor + 'Parametro/Eliminar/' + row.ParametroId + '" ><span class="glyphicon glyphicon-remove opciones" title="Eliminar"></span></a>';
                            return a + b;
                        }
                    }
                ]],
                width: '100%',
                singleSelect: true,
                rownumbers: true,
                pagination: true
            });

            var pager = $('#dgParametros').datagrid('getPager');
            $(pager).pagination({
                pageSize: 10,
                showPageList: true,
                pageList: [10, 20, 30, 40, 50],
                beforePageText: 'Página',
                afterPageText: 'de {pages}',
                displayMsg: 'Mostrando del {from} al {to} de los {total} resultados',
                onSelectPage: function (pageNumber, pageSize) {
                    Parametro.prototype.buscar();
                }
            });
        };

        Parametro.prototype.buscar = function () {

            var pageNumber_ = $('#dgParametros').datagrid('getPager').pagination('options').pageNumber;
            var pageSize_ = $('#dgParametros').datagrid('getPager').pagination('options').pageSize;

            $.ajax({
                url: globalRutaServidor + "Parametro/ListarParametros",
                type: 'GET',
                data: {
                    ////numero: $("#nroParametro").val(),
                    fini: $("#FecIniIndex").val(),
                    ffin: $("#FecFinIndex").val(),
                    page: pageNumber_,
                    pageSize: pageSize_
                },
                success: function (data) {
                    gMostrarResultadoBusqueda(data.rows, "#dgParametros");

                    $('#dgParametros').datagrid('getPager').pagination({
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

        Parametro.prototype.agregarEventos = function () {
            $("#btnConsultar").on('click', function () {
                Parametro.prototype.buscar();
            });
            $("#nroParametro").keypress(function (e) {
                return (e.keyCode >= 48 && e.keyCode <= 57)
            });
            $("#FecIniIndex").keypress(function (e) {
                return ((e.keyCode >= 48 && e.keyCode <= 57) || e.keyCode == 47)
            });
            $("#FecFinIndex").keypress(function (e) {
                return ((e.keyCode >= 48 && e.keyCode <= 57) || e.keyCode == 47)
            });
        };

        return Parametro;
    }());

    var Parametro = new Parametro();
    Parametro.PageLoad();
}());