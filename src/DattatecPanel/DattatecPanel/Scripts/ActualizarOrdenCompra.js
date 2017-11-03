$(function () {
    ListaOrdenCompra();
});

function ListaOrdenCompra() {
    $("#grid").jqGrid({
        url: "/OrdenCompra/ObtenerOrdenCompra",
        datatype: "json",
        jsonReader: {
            repeatitems: false
        },
        colNames: ['ID', 'Numero', 'Moneda', 'Fecha', 'Estado'],
        colModel: [
                { name: 'TransaccionCompraID', index: 'TransaccionCompraID' },
                { name: 'Numero', index: 'Numero' },
                { name: 'Descripcion', index: 'Descripcion' },
                { name: 'Fecha', index: 'Fecha', formatter: 'date' },
                { name: 'Estado', index: 'Estado' }
        ],
        rowNum: 15,
        rowList: [15, 25, 40],
        pager: '#pager',
        height: 'auto',
        viewrecords: true,
        caption: 'Ordenes de compra',
        emptyrecords: 'No existen datos.',
        //autoencode: true,
        autowidth: true,
        //multiselect: false,
        //LoadOnce: true,
        //sortName: 'TransaccionCompraID',
        sortorder: "asc"
    });

    jQuery("#grid").jqGrid('navGrid', '#pager', { edit: false, add: false, del: false });
}