(function () {
    var DetalleConvocatoria = (function () {
        function DetalleConvocatoria() { };
        DetalleConvocatoria.prototype.PageLoad = function () {
            //var s = suspender == undefined ? false : suspender;
            //if (!s || s == undefined) {
            gInputsFormatoFecha("Fecha_Registro,Fecha_RegistroIndex");
            //}
            DetalleConvocatoria.prototype.dataGrid();
            DetalleConvocatoria.prototype.buscar();
            //////DetalleConvocatoria.prototype.agregarEventos();
        }

        DetalleConvocatoria.prototype.dataGrid = function () {
            $("#dgDetalleConvocatoriaPostulantes").datagrid({
                title: 'RESULTADO',
                loadMsg: "Cargando...",
                columns: [[
                    ////{
                    ////    field: 'ConvocatoriaId', title: 'Convocatoria ID', align: 'center', width: 0
                    ////},
                    {
                        field: 'RUC', title: 'Ruc', align: 'center', width: 150
                    },
                    {
                        field: 'RazonSocial', title: 'Razón Social', align: 'center', width: 300
                    },
                    {
                        field: 'Descripcion', title: 'Rubro', align: 'center', width: 200
                    },
                    {
                        field: 'Numero', title: 'N° Convocatoria', align: 'center', width: 200
                    },
                    ////{
                    ////    field: 'PostulanteId', title: 'Postulante Id', width: 0
                    ////},
                    {
                        field: 'Fecha_Registro', title: 'Fecha Registro', width: 150,
                        formatter: function (value, row, index) {
                            return gFormatearFechaJson(value);
                        }
                    },
                  
                    {
                        field: 'action', title: 'Opciones', width: 100, align: 'center',
                        formatter: function (value, row, index) {
                            var a = '<a href="' + globalRutaServidor + 'DetalleConvocatoria/Validar/' + row.PostulanteId + '" ><span class="glyphicon glyphicon-pencil opciones" title="Validar"></span></a>';
                            var b = '<a href="' + globalRutaServidor + 'DetalleConvocatoria/Rechazar/' + row.PostulanteId + '" ><span class="glyphicon glyphicon-remove opciones" title="Rechazar"></span></a>';
                            return a + b;
                        }
                    }
                ]],
                width: '100%',
                singleSelect: true
            });
        }

        DetalleConvocatoria.prototype.buscar = function () {
            //////var nroDetalleConvocatoria = $("#nroDetalleConvocatoria").val();
            //////if (nroDetalleConvocatoria != undefined && nroDetalleConvocatoria.trim() != "") {
            //////    if (isNaN(nroDetalleConvocatoria.trim())) {
            //////        return gMensajeInformacion("Solo se admiten numeros en el campo número de DetalleConvocatoria.");
            //////    }
            //////}

            $.ajax({
                url: globalRutaServidor + "DetalleConvocatoria/ListarDetalleConvocatoriaPostulante",
                type: 'GET',
                data: {
                    ruc: $("#RUC").val(),
                    razonSocial: $("#razonSocial").val()
                },
                success: function (data) {
                    gMostrarResultadoBusqueda(data.rows, "#dgDetalleConvocatoriaPostulantes");
                },
                error: function () {
                    gMensajeErrorAjax();
                }
            });
        }

        //////DetalleConvocatoria.prototype.agregarEventos = function () {
        //////    $("#btnConsultar").on('click', function () {
        //////        DetalleConvocatoria.prototype.buscar();
        //////    });
        //////    $("#nroDetalleConvocatoria").keypress(function (e) {
        //////        return (e.keyCode >= 48 && e.keyCode <= 57)
        //////    });
        //////    $("#fechaInicioIndex").keypress(function (e) {
        //////        return ((e.keyCode >= 48 && e.keyCode <= 57) || e.keyCode == 47)
        //////    });
        //////    $("#FechaFinIndex").keypress(function (e) {
        //////        return ((e.keyCode >= 48 && e.keyCode <= 57) || e.keyCode == 47)
        //////    });
        //////};

        return DetalleConvocatoria;
    }());

    var DetalleConvocatoria = new DetalleConvocatoria();
    DetalleConvocatoria.PageLoad();
}());