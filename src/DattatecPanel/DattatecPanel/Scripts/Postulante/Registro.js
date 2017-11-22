(function () {
    var RegistroPostulante = (function () {
        function RegistroPostulante() { };

        RegistroPostulante.prototype.loadPage = function () {
            RegistroPostulante.prototype.dataGrid();
            RegistroPostulante.prototype.agregarEventos();
            RegistroPostulante.prototype.NumeroValidar();
            RegistroPostulante.prototype.ValidarCopyPaste();
        };

        RegistroPostulante.prototype.ValidarCopyPaste = function () {
            $(document).on("cut copy paste", "#RUC", function (e) {
                e.preventDefault();
            });
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

        RegistroPostulante.prototype.NumeroValidar = function () {

            var numeroRUC = $("#RUC").val();
            if (numeroRUC != undefined && numeroRUC.trim() != "") {
                if (isNaN(numeroRUC.trim())) {
                    return gMensajeInformacion("Solo se admiten numeros en el campo RUC.");
                }
            }


        };


        RegistroPostulante.prototype.Guardar = function () {

            var numeroRUC = $("#RUC").val();
            var nombreRazonSocial = $("#RazonSocial").val();
            var nDireccion = $("#Direccion").val();
            var nCorreo = $("#Correo").val();


            if (isNaN($("#RUC").val())) {
                return gMensajeInformacion("Solo se admiten numeros en el campo RUC.");
            }


            var mensaje = ValidarPostulante(numeroRUC, nombreRazonSocial, nDireccion, nCorreo);
            if (mensaje != "") { return gMensajeInformacion(mensaje); }

            gMensajeConfirmacion("¿Esta seguro de registrar?", function () {
                RegistroPostulante.prototype.guardarPostulante();
            });
            return false;
        };

        RegistroPostulante.prototype.guardarPostulante = function () {
            var frmData = new FormData(document.getElementById('frmRegistrarPostulante'));
            $.ajax({
                url: globalRutaServidor + "Postulante/RegistrarPostulante",
                type: 'POST',
                async: false,
                contentType: false,
                processData: false,
                data: frmData,
                success: function (data) {
                    if (data.statusCode == 200) {
                        if (data.mensajeInfo == "") {
                            var callback = function () {
                                $("#btnCancelar").click();
                            };
                            gMensajeInformacionConCallback(data.mensaje, callback);
                        } else {
                            gMensajeInformacion(data.mensajeInfo);
                        }
                    } else {
                        gMensajeInformacion('Ocurrio un error.');
                    }
                },
                error: function () {
                    gMensajeErrorAjax();
                }
            });
        };


        RegistroPostulante.prototype.agregarEventos = function () {


            $('#Direccion').css("background", "white");
            $('#RazonSocial').css("background", "white");


            $('#Correo').change(function (e) {
                var emailRegex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
                var EmailId = this.value;
                if (emailRegex.test(EmailId)) {
                    this.style.backgroundColor = "";
                    $('#btnGuardar').attr("disabled", false);
                }
                else {
                    this.style.backgroundColor = "LightPink";
                    gMensajeInformacion("Digite un correo válido");
                    $('#btnGuardar').attr("disabled", true);
                }
            });


            $("#btnGuardar").on('click', function () {
                RegistroPostulante.prototype.Guardar();
            });

            $("#RUC").keypress(function (e) {
                return (e.keyCode >= 48 && e.keyCode <= 57)
            });


            $("#RUC").keyup(function () {
                if ($(this).val().length > 11) {
                    $(this).val($(this).val().substr(0, 11));
                }
            });

            $("#RUC").change(function () {
                $.ajax({
                    url: globalRutaServidor + "Postulante/VerificarRUC",
                    type: 'GET',
                    data: {
                        numeroRUC: $("#RUC").val()
                    },
                    success: function (data) {

                        if (data.mensaje == 1) {

                            $("#RazonSocial").val(data.mensajeInfo);
                            $("#Direccion").val(data.mensajeDireccion);
                            $("#RazonSocial").prop("readonly", true);
                            $("#Direccion").prop("readonly", true);                     
                        }
                        else {
                            gMensajeInformacion(data.mensajeInfo);
                            $("#RazonSocial").val("");
                            $("#Direccion").val("");
                            $("#RazonSocial").prop("readonly", false);
                            $("#Direccion").prop("readonly", false);                           
                        }

                    },
                    error: function () {
                        gMensajeErrorAjax();
                        $("#RazonSocial").prop("readonly", false);
                        $("#Direccion").prop("readonly", false);    
                    }
                });
            });


        };

        return RegistroPostulante;
    }());
    var registroPostulante = new RegistroPostulante();
    registroPostulante.loadPage();
}());