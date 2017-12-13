if ($.fn.pagination) {
    $.fn.pagination.defaults.beforePageText = 'Page';
    $.fn.pagination.defaults.afterPageText = 'of {pages}';
    $.fn.pagination.defaults.displayMsg = 'Displaying {from} to {to} of {total} items';
}
if ($.fn.datagrid) {
    $.fn.datagrid.defaults.loadMsg = 'Processing, please wait ...';
}
if ($.messager) {
    $.messager.defaults.ok = 'Ok';
    $.messager.defaults.cancel = 'Cancel';
}
if ($.fn.validatebox) {
    $.fn.validatebox.defaults.missingMessage = 'This field is required.';
    $.fn.validatebox.defaults.rules.email.message = 'Please enter a valid email address.';
    $.fn.validatebox.defaults.rules.url.message = 'Please enter a valid URL.';
    $.fn.validatebox.defaults.rules.length.message = 'Please enter a value between {0} and {1}.';
}
if ($.fn.numberbox) {
    $.fn.numberbox.defaults.missingMessage = 'This field is required.';
}
if ($.fn.combobox) {
    $.fn.combobox.defaults.missingMessage = 'This field is required.';
}
if ($.fn.combotree) {
    $.fn.combotree.defaults.missingMessage = 'This field is required.';
}
if ($.fn.calendar) {
    $.fn.calendar.defaults.weeks = ['D', 'L', 'M', 'M', 'J', 'V', 'S'];
    $.fn.calendar.defaults.months = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'];
}
if ($.fn.datebox) {
    $.fn.datebox.defaults.currentText = 'Hoy';
    $.fn.datebox.defaults.closeText = 'Cerrar';
    $.fn.datebox.defaults.okText = 'Ok';
    $.fn.datebox.defaults.missingMessage = 'Campo requerido';
}
if ($.fn.datetimebox && $.fn.datebox) {
    $.extend($.fn.datetimebox.defaults, {
        currentText: $.fn.datebox.defaults.currentText,
        closeText: $.fn.datebox.defaults.closeText,
        okText: $.fn.datebox.defaults.okText,
        missingMessage: $.fn.datebox.defaults.missingMessage
    });
}

//Adicionales Extesion funcion datebox
$.fn.datebox.defaults.formatter = function (date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return (d < 10 ? ('0' + d) : d) + "/" + (m < 10 ? ('0' + m) : m) + "/" + y;
};
$.fn.datebox.defaults.parser = function (s) {
    if (!s) return new Date();
    var ss = s.split('/');
    var y = parseInt(ss[0], 10);
    var m = parseInt(ss[1], 10);
    var d = parseInt(ss[2], 10);
    if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
        return new Date(d, m - 1, y);
    } else {
        return new Date();
    }
};
