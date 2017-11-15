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
                        field: 'FecIni', title: 'Fecha Inicio', width: 150,
                        formatter: function (value, row, index) {
                            return gFormatearFechaJson(value);
                        }
                    },
                    {
                        field: 'FecFin', title: 'Fecha Fin', width: 150,
                        formatter: function (value, row, index) {
                            return gFormatearFechaJson(value);
                        }
                    },
                    {
                        field: 'Intervalo', title: 'Intervalo', width: 150
                    },
                    {
                        field: 'UnidadMedidaIntervalo', title: 'UnidadMedidaIntervalo', width: 250
                    },
                    {
                        field: 'FecUltPro', title: 'Fecha Ult. Proceso', width: 150,
                         formatter: function (value, row, index) {
                             return gFormatearFechaJson(value);
                         }
                    },
                    {
                        field: 'UrlServicio01', title: 'Servicio Sunat', width: 250
                    },
                    {
                        field: 'UrlServicio02', title: 'Servicio Osce', width: 250
                    }
                    //{
                    //    field: 'Estado', title: 'Estado', align: 'center', width: 100,
                    //    formatter: function (value, row, index) {
                    //        return value.trim() == "E" ? "Emitido" : "Suspendido";
                    //    }
                    //},
                    //{
                        ////field: 'action', title: 'Opciones', width: 100, align: 'center',
                        ////formatter: function (value, row, index) {
                        ////    var a = '<a href="' + globalRutaServidor + 'Parametro/Actualizar/' + row.Parametroid + '" ><span class="glyphicon glyphicon-pencil opciones" title="Modificar"></span></a>';
                        ////    var b = '<a href="' + globalRutaServidor + 'Parametro/Suspender/' + row.Parametroid + '" ><span class="glyphicon glyphicon-remove opciones" title="Suspender"></span></a>';
                        ////    return a + b;
                        ////}
                    //}
                ]],
                width: '100%',
                singleSelect: true
            });
        }

        Parametro.prototype.buscar = function () {
            ////var nroParametro = $("#nroParametro").val();
            ////if (nroParametro != undefined && nroParametro.trim() != "") {
            ////    if (isNaN(nroParametro.trim())) {
            ////        return gMensajeInformacion("Solo se admiten numeros en el campo número de Parametro.");
            ////    }
            ////}

            $.ajax({
                url: globalRutaServidor + "Parametro/ListarParametros",
                type: 'GET',
                data: {
                    ////numero: $("#nroParametro").val(),
                    fini: $("#FecIniIndex").val(),
                    ffin: $("#FecFinIndex").val()
                },
                success: function (data) {
                    gMostrarResultadoBusqueda(data.rows, "#dgParametros");
                },
                error: function () {
                    gMensajeErrorAjax();
                }
            });
        }

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