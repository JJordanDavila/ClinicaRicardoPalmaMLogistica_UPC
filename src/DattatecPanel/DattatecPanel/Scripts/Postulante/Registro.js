(function(){
    var RegistroPostulante = (function () {
        function RegistroPostulante() { };

        RegistroPostulante.prototype.loadPage = function () {
            RegistroPostulante.prototype.dataGrid();
            RegistroPostulante.prototype.agregarEventos();
        };

        RegistroPostulante.prototype.dataGrid = function () {
            $("#dgArchivosPostulante").datagrid({
                title: 'RESULTADO',
                loadMsg: "Cargando...",
                columns: [[
                    {
                        field: 'Archivo', title: 'Archivo', width: 600
                    }
                ]],
                rownumbers: true,
                width: '100%',
                singleSelect: true
            });
        };

        RegistroPostulante.prototype.guardarPostulante = function () {

        };

        RegistroPostulante.prototype.agregarEventos = function () {
            $("#btnDescargarRequisitos").on('click', function () {
                var data = $("#RequisitoConvocatoria").val();
                DescargarPDFPorArrayBytes(data, "Requisito");
            });
        };

        return RegistroPostulante;
    }());
    var registroPostulante = new RegistroPostulante();
    registroPostulante.loadPage();
}());