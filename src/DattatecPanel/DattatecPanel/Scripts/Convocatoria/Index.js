(function () {
    var Convocatoria = (function () {
        function Convocatoria() { };
        Convocatoria.prototype.PageLoad = function () {
            //var s = suspender == undefined ? false : suspender;
            //if (!s || s == undefined) {
                gInputsFormatoFecha("FechaInicio,FechaFin,fechaInicioIndex,FechaFinIndex");
            //}
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
                        field: 'Estado', title: 'Estado', align: 'center', width: 100,
                        formatter: function (value, row, index) {
                            return value.trim() == "E" ? "Emitido" : "Suspendido";
                        }
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
            var nroConvocatoria = $("#nroConvocatoria").val();
            if (nroConvocatoria != undefined && nroConvocatoria.trim() != "") {
                if (isNaN(nroConvocatoria.trim())) {
                    return gMensajeInformacion("Solo se admiten numeros en el campo número de convocatoria.");
                }
            }

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
                },
                error: function () {
                    gMensajeErrorAjax();
                }
            });
        }

        Convocatoria.prototype.agregarEventos = function () {
            $("#btnConsultar").on('click', function () {
                Convocatoria.prototype.buscar();
            });
            $("#nroConvocatoria").keypress(function (e) {
                return (e.keyCode >= 48 && e.keyCode <= 57)
            });
            $("#fechaInicioIndex").keypress(function (e) {
                return ((e.keyCode >= 48 && e.keyCode <= 57) || e.keyCode == 47)
            });
            $("#FechaFinIndex").keypress(function (e) {
                return ((e.keyCode >= 48 && e.keyCode <= 57) || e.keyCode == 47)
            });
        };

        return Convocatoria;
    }());

    var convocatoria = new Convocatoria();
    convocatoria.PageLoad();
}());