﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Principal.master" AutoEventWireup="true"
    CodeFile="ReporteConciliacionI.aspx.cs" Inherits="ReportesConciliacion_ReporteConciliacionI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="titulo" runat="Server">
    Reporte Tesoreria I
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
    <!--MsDropdown CSS-->
    <link href="../App_Scripts/msdropdown/dd.css" rel="stylesheet" type="text/css" />
    <!--Libreria jQuery-->
    <script src="../App_Scripts/jQueryScripts/jquery.min.js" type="text/javascript"></script>
    <script src="../App_Scripts/jQueryScripts/jquery-ui.min.js" type="text/javascript"></script>
    <script src="../App_Scripts/jQueryScripts/jquery.ui.datepicker-es.js" type="text/javascript"></script>
    <link href="../App_Scripts/jQueryScripts/css/custom-theme/jquery-ui-1.10.2.custom.min.css"
        rel="stylesheet" type="text/css" />
    <!-- Script se utiliza para el Scroll del GridView-->
    <link href="../App_Scripts/ScrollGridView/GridviewScroll.css" rel="stylesheet" type="text/css" />
    <script src="../App_Scripts/ScrollGridView/gridviewScroll.min.js" type="text/javascript"></script>
    <script src="../App_Scripts/Common.js" type="text/javascript"></script>
    <!--MsDropdown -->
    <script src="../App_Scripts/msdropdown/js/jquery.dd.js" type="text/javascript"></script>
    <script type="text/javascript">
        function pageLoad() {
            gridviewScroll();
            //FInicio - FFinal
            activarDatePickers();
            CargarEventoCheckBox();

        }

        function activarDatePickers() {
            //DataPicker Rango-Fechas 

            //DatePicker FOperacion
            $("#<%= txtFInicial.ClientID%>").datepicker({
                defaultDate: "+1w",
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    $("#<%=txtFFinal.ClientID%>").datepicker("option", "minDate", selectedDate);
                }
            });
            $("#<%=txtFFinal.ClientID%>").datepicker({
                defaultDate: "+1w",
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    $("#<%=txtFInicial.ClientID%>").datepicker("option", "maxDate", selectedDate);
                }
            });
            $("#<%=txtFechaFactura.ClientID%>").datepicker({
                defaultDate: "+1w",
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    $("#<%=txtFInicial.ClientID%>").datepicker("option", "minDate", selectedDate);
                }
                        });

                $("#<%=txtFechaFacturaBusqueda.ClientID%>").datepicker({
                defaultDate: "+1w",
                changeMonth: true,
                changeYear: true,

                onClose: function (selectedDate) {
                    $("#<%=txtFInicial.ClientID%>").datepicker("option", "minDate", selectedDate);
                }
            });

        }

        function gridviewScroll() {
            $('#<%=grvConciliacionCompartida.ClientID%>').gridviewScroll({
                width: 1200,
                height: 500,
                freezesize: 5,
                arrowsize: 30,
                varrowtopimg: '../App_Scripts/ScrollGridView/Images/arrowvt.png',
                varrowbottomimg: '../App_Scripts/ScrollGridView/Images/arrowvb.png',
                harrowleftimg: '../App_Scripts/ScrollGridView/Images/arrowhl.png',
                harrowrightimg: '../App_Scripts/ScrollGridView/Images/arrowhr.png',
                headerrowcount: 1,
                startVertical: $("#<%=hfCompartidaSV.ClientID%>").val(),
                startHorizontal: $("#<%=hfCompartidaSH.ClientID%>").val(),
                onScrollVertical: function (delta) { $("#<%=hfCompartidaSV.ClientID%>").val(delta); },
                onScrollHorizontal: function (delta) { $("#<%=hfCompartidaSH.ClientID%>").val(delta); }
            });
        }
    </script>
 <script type="text/javascript">
     function grvPedidosScroll(){
          $('#<%=grvPedidos.ClientID%>').gridviewScroll({
                width:580,
                height: 300,
                freezesize: 2,
                arrowsize: 30,
                varrowtopimg: '../App_Scripts/ScrollGridView/Images/arrowvt.png',
                varrowbottomimg: '../App_Scripts/ScrollGridView/Images/arrowvb.png',
                harrowleftimg: '../App_Scripts/ScrollGridView/Images/arrowhl.png',
                harrowrightimg: '../App_Scripts/ScrollGridView/Images/arrowhr.png',
                headerrowcount: 1
                
            });
     }
 </script>
 <script type="text/javascript">
     function FormatToCurrency(num) {
         num = num.toString().replace(/\$|\,/g, '');
         if (isNaN(num))
             num = "0";
         sign = (num == (num = Math.abs(num)));
         num = Math.floor(num * 100 + 0.50000000001);
         cents = num % 100;
         num = Math.floor(num / 100).toString();
         if (cents < 10)
             cents = "0" + cents;
         for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
             num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));
         return (((sign) ? '' : '-') + '$' + num + '.' + cents);
     }
  </script>
    <!-- Validar: solo numeros -->
    <script type="text/javascript">
        function ValidNum(e) {
            var tecla = document.all ? tecla = e.keyCode : tecla = e.which;
            return ((tecla > 47 && tecla < 58) || tecla == 8);
        }
        function ValidNumDecimal(e) {
            var tecla = document.all ? tecla = e.keyCode : tecla = e.which;
            return ((tecla > 47 && tecla < 58) || tecla == 46);
        }
    </script>
    <!-- Sumar los elementos seleccionados (CHECKBOX) -->
    <script type="text/javascript">
        var total = 10;

        function CargarEventoCheckBox() {
            $("#<%=grvPedidos.ClientID%> [id*='chkSeleccionado']").change(function () {
                //                CalcularTotal(montoexterno);
                var montoseleccionado = document.getElementById('<%=lblMontoExterno.ClientID%>');
                var monto = parseFloat((montoseleccionado.innerText).replace(/[^0-9-.]/g, ''));
                CalcularTotal(monto);
            });

            function CalcularTotal(montoexterno) {

                var total = 0;

                $('#<%=grvPedidos.ClientID%> tr:not(:last)').each(function () {
                    var checkBox = $(this).find("input[type='checkbox']");
                    if ($(checkBox).is(':checked')) {
                        $(this).attr("checked", "checked");
                        var coltotal = parseFloat($("td:eq(3) span", this).html());
                        if (!isNaN(coltotal)) {
                            total += coltotal;
                        }
                    }
                });
                var resultado = montoexterno - total;
                $('#<%=lblMontoResto.ClientID%>').html(FormatToCurrency(resultado));
                //            $('#<%=grvPedidos.ClientID%> tr:last td:eq(3) span').html(total);
            }

        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contenidoPrincipal" runat="Server">
    <asp:ScriptManager runat="server" ID="smDetalleConciliacion" AsyncPostBackTimeout="600"
        EnableScriptGlobalization="True">
    </asp:ScriptManager>
    <script src="../App_Scripts/jsUpdateProgress.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">

        function ShowModalPopup() {
            $find("ModalBehaviour").show();
        }

        function HideModalPopup() {
            $find("ModalBehaviour").hide();
        }
        function HideModalPopupConciliarMovPedido() {
            $find("ModalBehaviourConciliarMovPedido").hide();
        }
        function HideModalPopupFacturasPedido() {
            $find("ModalBehaviourBusquedaFactura").hide();
            $('#grvPedidosFacturados td').text(" ");
        }
        function HideModalPopupTipoCliente() {
            $find("ModalBehaviourTipoCliente").hide();
        }
        function HideModalPopupMesAño() {
            $find("ModalBehaviourMesAño").hide();
        }
    </script>
    <script type="text/javascript" language="javascript">
        var ModalProgress = '<%=mpeLoading.ClientID%>';        
    </script>
    <asp:UpdatePanel runat="server" ID="upConciliacionCompartida" UpdateMode="Always">
        <ContentTemplate>
            <table id="BarraHerramientas" class="bg-color-grisClaro01" style="width: 100%; vertical-align: top">
                <tr>
                    <td style="padding: 3px 3px 3px 0px; vertical-align: top; width: 70%">
                        <table class="etiqueta opcionBarra">
                            <tr>
                                <td class="iconoOpcion  bg-color-azulClaro" rowspan="2">
                                    <asp:ImageButton ID="btnActualizarConfig" runat="server" Height="25px" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/ActualizarConfig.png"
                                        ToolTip="ACTUALIZAR CONFIGURACION" Width="25px" ValidationGroup="Configuracion"
                                        OnClick="btnActualizarConfig_Click" />
                                </td>
                                <td class="lineaVertical" style="width: 25%">
                                    Empresa
                                    <asp:HiddenField ID="hdfCorporativo" runat="server" />
                                </td>
                                <td class="lineaVertical" style="width: 20%">
                                    Sucursal
                                    <asp:HiddenField ID="hdfSucursal" runat="server" />
                                </td>
                                <td class="lineaVertical" style="width: 20%">
                                    Banco
                                </td>
                                <td class="lineaVertical" style="width: 15%">
                                    Cuenta Bancaria
                                    <asp:HiddenField ID="hdfCuentaBancaria" runat="server" />
                                </td>
                                <td class="lineaVertical" style="width: 10%">
                                    Fecha Inicial
                                    <asp:HiddenField ID="hdfFInicial" runat="server" />
                                </td>
                                <td style="width: 10%">
                                    Fecha Final
                                    <asp:HiddenField ID="hdfFFinal" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="lineaVertical" style="width: 25%">
                                    <asp:DropDownList ID="ddlEmpresa" runat="server" Width="100%" SkinID="DropDownList"
                                        CssClass="dropDown" AutoPostBack="True" OnDataBound="ddlEmpresa_DataBound" OnSelectedIndexChanged="ddlEmpresa_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                                <td class="lineaVertical" style="width: 20%">
                                    <asp:DropDownList ID="ddlSucursal" runat="server" Width="100%" CssClass="dropDown"
                                        AutoPostBack="False">
                                    </asp:DropDownList>
                                </td>
                                <td class="lineaVertical" style="width: 20%">
                                    <asp:DropDownList ID="ddlBanco" runat="server" Width="100%" CssClass="dropDown" AutoPostBack="True"
                                        OnDataBound="ddlBanco_DataBound" OnSelectedIndexChanged="ddlBanco_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                                <td class="lineaVertical" style="width: 15%">
                                    <asp:DropDownList ID="ddlCuentaBancaria" runat="server" Width="100%" CssClass="dropDown"
                                        AutoPostBack="False">
                                    </asp:DropDownList>
                                </td>
                                <td class="lineaVertical" style="width: 10%">
                                    <asp:TextBox runat="server" ID="txtFInicial" CssClass="cajaTexto" Font-Size="10px"
                                        Width="85%"></asp:TextBox>
                                </td>
                                <td style="width: 10%">
                                    <asp:TextBox runat="server" ID="txtFFinal" CssClass="cajaTexto" Font-Size="10px"
                                        Width="85%"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 1%; padding: 3px 3px 3px 0px; vertical-align: top">
                        <table class="etiqueta opcionBarra">
                            <tr>
                                <td class="iconoOpcion bg-color-purpura" style="height: 30px" rowspan="2">
                                    <asp:ImageButton ID="btnFiltrar" runat="server" Height="25px" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Filtrar.png"
                                        ToolTip="FILTRAR" Width="25px" Style="height: 30px" OnClick="btnFiltrar_Click" />
                                </td>
                                <td>
                                    Filtrar en
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlFiltrarEn" runat="server" CssClass="etiqueta dropDownPequeño"
                                        Style="margin-bottom: 3px; margin-right: 3px" Width="85px">
                                        <asp:ListItem Value="Externos" Selected="True">Externos</asp:ListItem>
                                        <asp:ListItem Value="Internos">Internos</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="padding: 3px 3px 3px 0px; vertical-align: top; width: 1%">
                        <table class="etiqueta opcionBarra">
                            <tr>
                                <td class="iconoOpcion bg-color-naranja" style="height: 30px">
                                    <asp:ImageButton ID="imgBuscar" runat="server" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Buscar.png"
                                        ToolTip="BUSCAR" Width="25px" OnClick="imgBuscar_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 1%; padding: 3px 3px 3px 0px; vertical-align: top">
                        <table class="etiqueta opcionBarra">
                            <tr>
                                <td id="tdExportar" runat="server" class="iconoOpcion bg-color-rojo" style="height: 30px">
                                    <asp:ImageButton ID="imgExportar" runat="server" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Exportar.png"
                                        ToolTip="EXPORTAR RESULTADOS" Width="25px" OnClick="imgExportar_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 1%; padding: 3px 3px 3px 0px; vertical-align: top">
                        <table class="etiqueta opcionBarra">
                            <tr>
                                <td class="iconoOpcion bg-color-azulOscuro" style="height: 30px">
                                    <asp:ImageButton ID="imgGuardar" runat="server" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Guardar.png"
                                        ToolTip="GUARDAR VISTA" Width="25px" OnClick="btnGuardarVista_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 1%; padding: 3px 3px 3px 0px; vertical-align: top">
                        <table class="etiqueta opcionBarra">
                            <tr>
                                <td class="iconoOpcion bg-color-grisOscuro" style="height: 30px">
                                    <asp:ImageButton ID="imgCerrarMesConciliacion" runat="server" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Cerrar.png"
                                        ToolTip="CERRAR MES" Width="25px" OnClick="imgCerrarMesConciliacion_Click" OnClientClick="return confirm('¿Esta seguro de CERRAR el MES de Conciliaciones.?\nNota: Verificar antes de que no existan CONCILIACIONES abiertas de este mes.')" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <%--       <td style="width: 1%; padding: 3px 3px 3px 0px; vertical-align: top">
                        <table class="etiqueta opcionBarra">
                            <tr>
                                <td class="iconoOpcion bg-color-grisClaro01" style="height: 30px">
                                    <asp:ImageButton ID="imgCancelarConciliacion" runat="server" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Cancelar.png"
                                        ToolTip="CANCELAR CONCILIACION" Width="25px" OnClientClick="return confirm('¿Esta seguro de CANCELAR la conciliación.?')" />
                                </td>
                            </tr>
                        </table>
                    </td>--%>
                </tr>
            </table>
            <table style="width: 100%">
                <tr>
                    <td style="vertical-align: middle; padding: 5px 5px 5px 5px" class="etiqueta centradoJustificado fg-color-blanco bg-color-azulClaro">
                        Transacciones Conciliadas
                    </td>
                </tr>
                <tr style="width: 100%">
                    <td colspan="2">
                        <asp:HiddenField ID="hfCompartidaSV" runat="server" />
                        <asp:HiddenField ID="hfCompartidaSH" runat="server" />
                        <asp:HiddenField ID="hfStatusConciliacionFD" runat="server" />
                        <asp:HiddenField ID="hfStatusConceptoFiltro" runat="server" />
                        <asp:GridView ID="grvConciliacionCompartida" runat="server" AutoGenerateColumns="False"
                            Width="100%" AllowPaging="True" ShowHeaderWhenEmpty="True" CssClass="grvResultadoConsultaCss"
                            DataKeyNames="FolioConciliacion,CorporativoConciliacion,SucursalConciliacion,AñoConciliacion,MesConciliacion,
                            Folio,Corporativo,Sucursal,Año,Secuencia,ConsecutivoFlujo" PageSize="50" OnPageIndexChanging="grvConciliacionCompartida_PageIndexChanging"
                            OnRowDataBound="grvConciliacionCompartida_RowDataBound" OnRowCreated="grvConciliacionCompartida_RowCreated"
                            AllowSorting="True" OnSorting="grvConciliacionCompartida_Sorting">
                            <%-- <EmptyDataTemplate>
                                <asp:Label ID="lblvacio" runat="server" CssClass="etiqueta fg-color-rojo" Text="No existe ninguna conciliación de acuerdo a los Parámetros del Filtrado. "></asp:Label>
                            </EmptyDataTemplate>--%>
                            <HeaderStyle HorizontalAlign="Center" />
                            <Columns>
                                <asp:TemplateField HeaderText="  #  " SortExpression="ConsecutivoFlujo">
                                    <ItemTemplate>
                                        <asp:Label ID="lblConsecutivoFlujo" runat="server" Text='<%# Eval("ConsecutivoFlujo") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="15px" />
                                    <ItemStyle HorizontalAlign="Center" Width="15px" BackColor="#ebecec" ForeColor="Black" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="FE" SortExpression="Folio">
                                    <ItemTemplate>
                                        <asp:Label ID="lblFolio" runat="server" Text='<%# resaltarBusqueda(Eval("Folio").ToString()) %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="15px" />
                                    <ItemStyle HorizontalAlign="Center" Width="15px" BackColor="#ebecec" ForeColor="Black" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="SE" SortExpression="Secuencia">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSecuencia" runat="server" Text='<%#  resaltarBusqueda(Eval("Secuencia").ToString())  %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="15px" />
                                    <ItemStyle HorizontalAlign="Center" Width="15px" BackColor="#ebecec" ForeColor="Black" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status" SortExpression="StatusConciliacion">
                                    <ItemTemplate>
                                        <asp:Image runat="server" ID="imgStatusConciliacion" ImageUrl='<%# Eval("UbicacionIcono") %>'
                                            Width="15px" Height="15px" CssClass="icono border-color-grisOscuro centradoMedio"
                                            ToolTip='<%# Eval("StatusConciliacion") %>' AlternateText='<%# Eval("StatusConciliacion") %>' />
                                    </ItemTemplate>
                                    <HeaderTemplate>
                                        <asp:DropDownList runat="server" ID="ddlStatusMovimiento" Width="70px" CssClass="dropDownPequeño bg-color-verdeAgua fg-color-blanco"
                                            AutoPostBack="True" Style="margin: 0 0 0 0;" AppendDataBoundItems="True" OnSelectedIndexChanged="ddlStatusMovimiento_SelectedIndexChanged">
                                            <asp:ListItem Value="-1" Text="TODOS"></asp:ListItem>
                                        </asp:DropDownList>
                                    </HeaderTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="80px" />
                                    <ItemStyle HorizontalAlign="Center" Width="80px" BackColor="#ebecec" ForeColor="Black" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Fecha" SortExpression="FOperacion">
                                    <ItemTemplate>
                                        <asp:Label ID="lblFechaOperacion" runat="server" Text='<%#  resaltarBusqueda(Eval("FOperacion", "{0:d}").ToString())  %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                                    <ItemStyle HorizontalAlign="Center" Width="50px" BackColor="#ebecec" ForeColor="Black" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Descripcion" SortExpression="Descripcion">
                                    <ItemTemplate>
                                        <div class="parrafoTexto" style="margin-left: 10px">
                                            <asp:Label ID="lblDescripcionExterno" runat="server" Text='<%# resaltarBusqueda( Eval("Descripcion").ToString())   %>'></asp:Label>
                                        </div>
                                        <asp:HoverMenuExtender ID="hmeDescripcionExterno" runat="server" TargetControlID="lblDescripcionExterno"
                                            PopupControlID="pnlPopUpDescripcionExterno" PopDelay="20" OffsetX="-30" OffsetY="-10">
                                        </asp:HoverMenuExtender>
                                        <asp:Panel ID="pnlPopUpDescripcionExterno" runat="server" CssClass="border-color-grisOscuro ocultar"
                                            Width="200px" Style="padding: 5px 5px 5px 5px; margin: 0 0 0 0; position: absolute"
                                            BackColor="White" BorderColor="gray" Wrap="True">
                                            <asp:Label ID="lblTLDescripcionExterno" runat="server" Text='<%#  resaltarBusqueda( Eval("Descripcion").ToString())  %>'
                                                CssClass="etiqueta" Width="95%" Font-Size="11px" />
                                        </asp:Panel>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="200px" />
                                    <ItemStyle HorizontalAlign="Justify" Width="200px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Caja" SortExpression="Caja">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCaja" runat="server" Text='<%#  resaltarBusqueda( Eval("Caja").ToString())  %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="30px" />
                                    <ItemStyle HorizontalAlign="Center" Width="30px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Sucursal Bancaria" SortExpression="SucursalBancaria">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSucursalBancaria" runat="server" Text='<%# resaltarBusqueda(Eval("SucursalBancaria").ToString()) %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Referencia" SortExpression="Referencia">
                                    <ItemTemplate>
                                        <asp:Label ID="lblReferencia" runat="server" Text='<%# resaltarBusqueda(Eval("Referencia").ToString())  %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Retiro" SortExpression="Retiro">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblRetiro" Text='<%# resaltarBusqueda(Eval("Retiro","{0:c2}").ToString()) %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="80px" />
                                    <ItemStyle HorizontalAlign="Center" Width="80px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Deposito" SortExpression="Deposito">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblDeposito" Text='<%# resaltarBusqueda(Eval("Deposito","{0:c2}").ToString()) %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="80px" />
                                    <ItemStyle HorizontalAlign="Center" Width="80px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Saldo" SortExpression="Saldo">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblSaldo" Text='<%# resaltarBusqueda(Eval("Saldo","{0:c2}").ToString()) %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="80px" />
                                    <ItemStyle HorizontalAlign="Center" Width="80px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="" SortExpression="">
                                    <HeaderTemplate>
                                        <table width="100%" style="border-style: none" cellpadding="0px" cellspacing="0px">
                                            <tr>
                                                <td style="width: 150px">
                                                    <%--<asp:Label ID="ttlStatusConcepto" Text="StatusConcepto" CssClass="etiqueta" runat="server"></asp:Label>--%>
                                                    <asp:DropDownList runat="server" ID="ddlStatusConcepto" Width="100px" CssClass="dropDownPequeño fg-color-blanco bg-color-verdeAgua"
                                                        AutoPostBack="True" Style="margin: 0 0 0 0;" AppendDataBoundItems="True" OnSelectedIndexChanged="ddlStatusConcepto_SelectedIndexChanged">
                                                        <asp:ListItem Value="-1" Text="TODOS"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="width: 80px">
                                                    <asp:Label ID="ttlCliente" Text="Cliente" CssClass="etiqueta" runat="server"></asp:Label>
                                                </td>
                                                <td style="width: 30px">
                                                </td>
                                                <td style="width: 150px">
                                                    <asp:Label ID="ttlDescripcionInterna" Text="Descripcion Interna" CssClass="etiqueta"
                                                        runat="server"></asp:Label>
                                                </td>
                                                <td style="width: 150px">
                                                    <asp:Label ID="ttlConceptoInterno" Text="Concepto Interno" CssClass="etiqueta" runat="server"></asp:Label>
                                                </td>
                                                <td style="width: 250px">
                                                    <asp:Label ID="ttlMotivoNoConciliado" Text="Motivo No Conciliado" CssClass="etiqueta"
                                                        runat="server"></asp:Label>
                                                </td>
                                                <td style="width: 250px">
                                                    <asp:Label ID="ttlComentarioNoConciliado" Text="Comentario No Conciliado" CssClass="etiqueta"
                                                        runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:GridView ID="grvMovimientosConciliadosMovExterno" runat="server" AutoGenerateColumns="False"
                                            Width="950px" AllowPaging="False" ShowHeaderWhenEmpty="False" ShowHeader="False"
                                            OnRowDataBound="grvMovimientosConciliadosMovExterno_RowDataBound" CssClass="grvAnidadoCss"
                                            DataKeyNames="FolioConciliacion,CorporativoConciliacion,SucursalConciliacion,AñoConciliacion,MesConciliacion,SecuenciaRelacion,
                                            Folio,Corporativo,Sucursal,Año,Secuencia" Style="margin-bottom: -5px; padding: 0"
                                            CellPadding="0" CellSpacing="0" BorderStyle="None">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <Columns>
                                                <%--  <asp:TemplateField HeaderText="Status" SortExpression="StatusConciliacion">
                                                    <ItemTemplate>
                                                        <asp:Image runat="server" ID="imgStatusConciliacion" ImageUrl='<%# Eval("UbicacionIcono") %>'
                                                            Width="15px" Height="15px" CssClass="icono border-color-grisOscuro centradoMedio"
                                                            ToolTip='<%# Eval("StatusConciliacionMovimiento") %>' AlternateText='<%# Eval("StatusConciliacionMovimiento") %>' />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="40px" />
                                                    <ItemStyle HorizontalAlign="Center" Width="40px" BackColor="#ebecec" ForeColor="Black" />
                                                </asp:TemplateField>--%>
                                                <asp:TemplateField HeaderText="Status Concepto" SortExpression="StatusConcepto">
                                                    <ItemTemplate>
                                                        <asp:DropDownList runat="server" ID="ddlStatusConcepto" Width="100px" CssClass="dropDown"
                                                            AutoPostBack="False" Style="margin: 0 0 0 0;" />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                                                    <ItemStyle HorizontalAlign="Center" Width="150px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cliente" SortExpression="Cliente">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtCliente" runat="server" Width="80px" CssClass="cajaTextoEditar centrado"
                                                            Text='<%# Eval("Cliente") %>' onkeypress="return ValidNum(event)" Visible="True"
                                                            Font-Size="10px" Enabled="True" Style="margin: 0 0 0 0"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="80px" />
                                                    <ItemStyle HorizontalAlign="Center" Width="80px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:ImageButton runat="server" ID="btnBuscarPedido" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Buscar.png"
                                                            Width="15px" Height="15px" Visible="True" Enabled="True" CssClass="icono bg-color-grisOscuro centradoMedio"
                                                            ToolTip="BUSCAR PEDIDOS" OnClick="btnBuscarPedido_Click" ValidationGroup="Cliente" />
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="30px" />
                                                    <ItemStyle HorizontalAlign="Center" Width="30px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Descripcion Interna" SortExpression="DescripcionInterno">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtDescripcionInterna" runat="server" Width="90%" CssClass="cajaTextoEditar"
                                                            Text='<%# Eval("DescripcionInterno") %>' Style="margin: 0 0 0 0" Font-Size="10px"></asp:TextBox>
                                                        <asp:HoverMenuExtender ID="hmeDescripcionInterno" runat="server" TargetControlID="txtDescripcionInterna"
                                                            PopupControlID="pnlPopUpDescripcionInterno" PopDelay="20" OffsetX="-20" OffsetY="-30">
                                                        </asp:HoverMenuExtender>
                                                        <asp:Panel ID="pnlPopUpDescripcionInterno" runat="server" CssClass="border-color-grisOscuro ocultar"
                                                            Width="200px" Style="padding: 5px 5px 5px 5px; margin: 0 0 0 0; position: absolute"
                                                            BackColor="White" BorderColor="gray" Wrap="True">
                                                            <table width="100%" style="margin-top: -5px">
                                                                <tr>
                                                                    <td class="lineaHorizontal">
                                                                        Descripcion Interna
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtToolTipDescripcionInterno" runat="server" Text='<%# Eval("DescripcionInterno").ToString() %>'
                                                                            CssClass="cajaTexto" Width="95%" Font-Size="12px" MaxLength="500" Rows="4" TextMode="MultiLine"
                                                                            Style="resize: none" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </asp:Panel>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                                                    <ItemStyle HorizontalAlign="Center" Width="150px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Concepto Interno" SortExpression="ConceptoInterno">
                                                    <ItemTemplate>
                                                        <div class="parrafoTexto">
                                                            <asp:Label runat="server" ID="lblConceptoInterno" Text='<%# Eval("ConceptoInterno") %>'
                                                                Font-Size="10px"></asp:Label></div>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                                                    <ItemStyle HorizontalAlign="Center" Width="150px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Motivo No Conciliado" SortExpression="MotivoNoConciliado">
                                                    <ItemTemplate>
                                                        <div class="parrafoTexto">
                                                            <asp:Label runat="server" ID="lblMotivoNoConciliado" Text='<%# Eval("MotivoNoConciliado") %>'
                                                                Font-Size="10px"></asp:Label>
                                                        </div>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="250px" />
                                                    <ItemStyle HorizontalAlign="Center" Width="250px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Comentario No Conciliado" SortExpression="ComentarioNoConciliado">
                                                    <ItemTemplate>
                                                        <div class="parrafoTexto">
                                                            <asp:Label runat="server" ID="lblComentarioNoConciliado" Text='<%# Eval("ComentarioNoConciliado") %>'
                                                                Font-Size="10px"></asp:Label>
                                                        </div>
                                                        <asp:HoverMenuExtender ID="hmeComentarioNoConciliado" runat="server" TargetControlID="lblComentarioNoConciliado"
                                                            PopupControlID="pnlPopUpComentarioNoConciliado" PopDelay="20" OffsetX="-30" OffsetY="-10">
                                                        </asp:HoverMenuExtender>
                                                        <asp:Panel ID="pnlPopUpComentarioNoConciliado" runat="server" CssClass="border-color-grisOscuro ocultar"
                                                            Width="200px" Style="padding: 5px 5px 5px 5px; margin: 0 0 0 0; position: absolute"
                                                            BackColor="White" BorderColor="gray" Wrap="True">
                                                            <asp:Label ID="lblTLComentarioNoConciliado" runat="server" Text='<%# Eval("ComentarioNoConciliado").ToString() %>'
                                                                CssClass="etiqueta" Width="95%" Font-Size="11px" />
                                                        </asp:Panel>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" Width="250px" />
                                                    <ItemStyle HorizontalAlign="Justify" Width="250px" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="950px" />
                                    <ItemStyle HorizontalAlign="Center" Width="950px" BackColor="#f5ff8d" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Traspaso" SortExpression="FolioTraspaso">
                                    <ItemTemplate>
                                        <b>
                                            <asp:Label runat="server" ID="lblFolioTraspaso" ForeColor="white" Text='<%# String.Format("FL: {0}/{1}", Eval("AñoTraspaso").ToString(), Eval("FolioTraspaso").ToString()) %>'></asp:Label></b>
                                        <asp:Image ID="imgTipo" ImageUrl='<%# String.Format("~/App_Themes/GasMetropolitanoSkin/Iconos/In_out/{0}.png", Eval("TipoTraspaso").ToString()) %>'
                                            runat="server" ToolTip='<%# Eval("TipoTraspaso") %>' Width="15px" Heigth="15px"
                                            CssClass="icono" ImageAlign="Middle" Style="margin: 0 0 0 0" />
                                        <asp:Label runat="server" ID="lblMontoTraspaso" Text='<%# Eval("MontoTraspaso","{0:c2}") %>'
                                            ForeColor="white"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                                    <ItemStyle HorizontalAlign="Center" Width="150px" BackColor="#f1ff58" />
                                </asp:TemplateField>
                            </Columns>
                            <PagerStyle CssClass="grvPaginacionScroll" />
                        </asp:GridView>
                    </td>
                </tr>
            </table>
            <asp:HiddenField runat="server" ID="hdfCerrarFiltro" />
            <asp:ModalPopupExtender ID="mpeFiltrar" runat="server" BackgroundCssClass="ModalBackground"
                DropShadow="False" EnableViewState="false" PopupControlID="pnlFiltrar" TargetControlID="hdfCerrarFiltro"
                CancelControlID="btnCerrar">
            </asp:ModalPopupExtender>
            <asp:Panel ID="pnlFiltrar" runat="server" CssClass="ModalPopup" Width="500px" Style="display: none">
                <table style="width: 100%;">
                    <tr class="bg-color-grisOscuro">
                        <td colspan="4" style="padding: 5px 5px 5px 5px" class="etiqueta">
                            <div class="floatDerecha">
                                <asp:ImageButton runat="server" ID="btnCerrar" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Cerrar.png"
                                    CssClass="iconoPequeño bg-color-rojo" />
                            </div>
                            <div class="fg-color-blanco">
                                FILTRAR
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 5px 5px 5px 5px; width: 50%">
                            <div class="etiqueta">
                                Campo
                            </div>
                            <asp:DropDownList runat="server" ID="ddlCampoFiltrar" CssClass="dropDown etiqueta"
                                ClientIDMode="Static" ValidationGroup="Filtrar" Width="100%" Font-Size="10px"
                                AutoPostBack="False" />
                            <%--OnSelectedIndexChanged="ddlCampoFiltrar_SelectedIndexChanged"--%>
                        </td>
                        <td style="padding: 5px 5px 5px 5px; width: 30%">
                            <div class="etiqueta">
                                Operacion
                            </div>
                            <asp:DropDownList runat="server" ID="ddlOperacion" CssClass="dropDown centradoMedio etiqueta"
                                ValidationGroup="Filtrar" Width="100%" Font-Size="10px">
                                <asp:ListItem Selected="True" Value="=">IGUAL</asp:ListItem>
                                <asp:ListItem Value="&gt;">MAYOR</asp:ListItem>
                                <asp:ListItem Value="&lt;">MENOR</asp:ListItem>
                                <asp:ListItem Value="&gt;=">MAYOR O IGUAL</asp:ListItem>
                                <asp:ListItem Value="&lt;=">MENOR O IGUAL</asp:ListItem>
                                <asp:ListItem Value="&lt;&gt;">DIFERENTE</asp:ListItem>
                                <asp:ListItem Value="LIKE">LIKE</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 5px 5px 5px 5px; width: 15%" colspan="2">
                            <div class="etiqueta">
                                Valor
                            </div>
                            <asp:TextBox ID="txtValor" runat="server" CssClass="cajaTexto" Font-Size="12px" Width="98%">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 10%;" class="centradoDerecha" colspan="3">
                            <asp:Button ID="btnIrFiltro" runat="server" CssClass="boton fg-color-blanco bg-color-azulClaro"
                                Text="Filtrar" ValidationGroup="Filtro" OnClick="btnIrFiltro_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="bg-color-grisClaro01" colspan="4">
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:HiddenField runat="server" ID="hdfCerrarBuscar" />
            <asp:ModalPopupExtender ID="mpeBuscar" runat="server" BackgroundCssClass="ModalBackground"
                DropShadow="False" EnableViewState="false" PopupControlID="pnlBuscar" TargetControlID="hdfCerrarBuscar"
                CancelControlID="btnCerrarBuscar">
            </asp:ModalPopupExtender>
            <asp:Panel ID="pnlBuscar" runat="server" CssClass="ModalPopup" Width="400px" Style="display: none">
                <table style="width: 100%;">
                    <tr class="bg-color-grisOscuro">
                        <td colspan="4" style="padding: 5px 5px 5px 5px" class="etiqueta">
                            <div class="floatDerecha">
                                <asp:ImageButton runat="server" ID="btnCerrarBuscar" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Cerrar.png"
                                    CssClass="iconoPequeño bg-color-rojo" />
                            </div>
                            <div class="fg-color-blanco">
                                Buscar
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 5px 5px 5px 5px; width: 85%">
                            <div class="etiqueta">
                                Valor
                            </div>
                            <asp:TextBox ID="txtBuscar" runat="server" CssClass="cajaTexto" Font-Size="12px"
                                Width="98%">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 5%" class="centradoDerecha">
                            <asp:Button ID="btnIrBuscar" runat="server" CssClass="boton fg-color-blanco bg-color-azulClaro"
                                Text="BUSCAR" OnClick="btnIrBuscar_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="bg-color-grisClaro01" colspan="4">
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:HiddenField runat="server" ID="hdfConciliarMovPedido" />
    <asp:HiddenField ID="hfPedidoSV" runat="server" />
    <asp:HiddenField ID="hfPedidoSH" runat="server" />
    <asp:ModalPopupExtender ID="popUpConciliarMovPedido" runat="server" PopupControlID="pnlConciliarMovPedido"
        TargetControlID="hdfConciliarMovPedido" BehaviorID="ModalBehaviourConciliarMovPedido"
        BackgroundCssClass="ModalBackground">
    </asp:ModalPopupExtender>
    <%--Style="display: none"--%>
<asp:Panel ID="pnlConciliarMovPedido" runat="server" BackColor="#FFFFFF" Width="600px">
        <asp:UpdatePanel ID="upConciliarMovPedido" runat="server">
            <ContentTemplate>
                <table style="width: 100%;">
                    <tr class="bg-color-grisOscuro">
                        <td colspan="4" style="padding: 5px 5px 5px 5px" class="etiqueta">
                            <div class="floatDerecha">
                                <asp:ImageButton runat="server" ID="img_cerrarConciliarMovPedido" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Cerrar.png"
                                    CssClass="iconoPequeño bg-color-rojo" OnClientClick="HideModalPopupConciliarMovPedido();" /><%--OnClick="imgCerrarImportar_Click"--%>
                            </div>
                            <div class="fg-color-blanco">
                                CONCILIAR MOVIMIENTO.
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="datos-estilo;bg-color-grisClaro03" style="padding: 10px 10px 10px 10px">
                            <div class="etiqueta lineaHorizontal">
                                <table width="100%">
                                    <tr class="etiqueta centradoJustificado fg-color-blanco bg-color-azulClaro">
                                    <td style="width: 70%; padding: 5px 5px 5px 5px">
                                    <asp:RadioButtonList ID="rblClienteTipo" runat="server" RepeatColumns="2" RepeatDirection="Horizontal"
                                        Width="70%">
                                        <asp:ListItem Value="NORMA" Selected="True">Cliente Normal</asp:ListItem>
                                        <asp:ListItem Value="PADREL">Cliente Padre</asp:ListItem>
                                    </asp:RadioButtonList>
                                    </td>
                                    <td style="width: 90%; padding: 5px 5px 5px 5px"  colspan="2" align="left">
                                            Fecha Factura:
                                            <asp:TextBox ID="txtFechaFactura" runat="server" CssClass="cajaTexto" Font-Size="12px"
                                            Width="90%">
                                            </asp:TextBox>
                                   </td>
                                    </td>
                                    <td style="width: 20%; padding: 5px 5px 5px 5px" colspan="3">
                                            Factura:
                                           <asp:TextBox ID="txtFactura" runat="server" CssClass="cajaTexto" Font-Size="12px"
                                            Width="90%">
                                            </asp:TextBox>
                                    </td>
                                       
                                    </tr>
                                    <tr>
                                        <td class="etiqueta lineaVertical centradoMedio" style="width: 40%; padding: 5px 5px 5px 5px">
                                            CLIENTE:
                                            <asp:Label ID="lblCliente" runat="server" CssClass="fg-color-negro"></asp:Label>
                                        </td>
                                        <td class="etiqueta lineaVertical centradoMedio" style="width: 15%; padding: 5px 5px 5px 5px">
                                            Monto Conciliar:
                                        </td>
                                        <td class="etiqueta lineaVertical centradoMedio bg-color-grisOscuro fg-color-blanco"
                                            style="width: 15%; padding: 5px 5px 5px 5px">
                                            <asp:Label runat="server" ID="lblMontoExterno" Text="$ 0.00"></asp:Label>
                                        </td>
                                        <td class="etiqueta lineaVertical centradoMedio" style="width: 15%; padding: 5px 5px 5px 5px">
                                            Resto:
                                        </td>
                                        <td class="etiqueta lineaVertical centradoMedio bg-color-azul fg-color-blanco" style="width: 15%;
                                            padding: 5px 5px 5px 5px">
                                            <asp:Label runat="server" ID="lblMontoResto"></asp:Label>
                                        </td>

                                        <td class="iconoOpcion bg-color-naranja" rowspan="2">
                                            <asp:ImageButton ID="imgBuscarPedido" runat="server" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Buscar.png"
                                                ToolTip="BUSCAR" style="width: 25px; padding: 10px 2px 2px 2px"  OnClick="imgBuscarPedido_Click" />
                                            <%--Width="25px"--%>
                                        </td>

                                    </tr>
                                    </table>
                                    <table width="100%">
                                    <tr>


                                        <asp:ImageButton runat="server" ID="btnBusquFact" 
                                        Width="150px" Height="25px" Visible="True" Enabled="True" CssClass="iconoOpcion bg-color-azulOscuro centradoMedio fg-color-blanco"
                                        ToolTip="BUSCAR FACTURAS" OnClick="btnBuscarFactura_Click" />
                                    </tr>
                                </table>
                            </div>
                            <br />
                            <div class="etiqueta">
                                PEDIDOS
                            </div>
                            <div >
                                <asp:GridView ID="grvPedidos" runat="server" AutoGenerateColumns="False" ShowHeader="True"
                                    CssClass="grvResultadoConsultaCss" AllowSorting="True" ShowFooter="False" Width="100%"
                                    ShowHeaderWhenEmpty="True" DataKeyNames="Celula,Pedido,AñoPed,Cliente" AllowPaging="False"
                                    PageSize="10" OnPageIndexChanging="grvPedidos_PageIndexChanging">
                                  
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkSeleccionado" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="25px" BackColor="#ebecec"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" Width="25px"></HeaderStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Pedido" SortExpression="Pedido">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPedido" runat="server" Text='<%# Eval("Pedido") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="70px" BackColor="#ebecec"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" Width="70px"></HeaderStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ped.Referencia" SortExpression="PedReferencia">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPedidoReferencia" runat="server" Text='<%# Eval("PedReferencia") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="70px" BackColor="#ebecec"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" Width="70px"></HeaderStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Remisión" SortExpression="Remision">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRemision" runat="server" Text='<%# Eval("Remision") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="70px" BackColor="#ebecec"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" Width="70px"></HeaderStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Factura" SortExpression="Factura">
                                            <ItemTemplate>
                                                <asp:Label ID="lblFactura" runat="server" Text='<%# Eval("Factura") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="70px" BackColor="#ebecec"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" Width="70px"></HeaderStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="FSuministro" SortExpression="FSuministro">
                                            <ItemTemplate>
                                                <asp:Label ID="lblFSuministro" runat="server" Text='<%# Eval("FSuministro","{0:d}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ControlStyle CssClass="centradoMedio" />
                                            <ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" Width="100px"></HeaderStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Litros" SortExpression="Litros">
                                            <ItemTemplate>
                                                <asp:Label ID="lblLitros" runat="server" Text='<%# Eval("Litros") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="70px" BackColor="#ebecec"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" Width="70px"></HeaderStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Monto" SortExpression="Total">
                                            <ItemTemplate>
                                                <b>
                                                    <asp:Label ID="lblMontoPedido" runat="server" Text='<%# Eval("Total") %>'></asp:Label></b>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="120px"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Nom. Cliente" SortExpression="Nombre">
                                            <ItemTemplate>
                                                <div class="parrafoTexto">
                                                    <asp:Label ID="lblCliente" runat="server" Text='<%# Eval("Nombre") %>'>
                                                    </asp:Label>
                                                </div>
                                                <asp:HoverMenuExtender ID="hmeCliente" runat="server" TargetControlID="lblCliente"
                                                    PopupControlID="pnlPopUpCliente" PopDelay="20" OffsetX="-30" OffsetY="-10">
                                                </asp:HoverMenuExtender>
                                                <asp:Panel ID="pnlPopUpCliente" runat="server" CssClass="grvResultadoConsultaCss ocultar"
                                                    Width="250px" Style="padding: 5px 5px 5px 5px" BackColor="White" Wrap="True">
                                                    <asp:Label ID="lblToolTipCliente" runat="server" Text='<%#Eval("Nombre") %>' CssClass="etiqueta "
                                                        Font-Size="10px" /></asp:Panel>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Justify" Width="150px"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" Width="150px"></HeaderStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Concepto" SortExpression="Concepto">
                                            <ItemTemplate>
                                                <div class="parrafoTexto">
                                                    <asp:Label ID="lblConcepto" runat="server" Text='<%# Eval("Concepto") %>'></asp:Label>
                                                </div>
                                                <asp:HoverMenuExtender ID="hmeConcepto" runat="server" TargetControlID="lblConcepto"
                                                    PopupControlID="pnlPopUpConcepto" PopDelay="20" OffsetX="-30" OffsetY="-10">
                                                </asp:HoverMenuExtender>
                                                <asp:Panel ID="pnlPopUpConcepto" runat="server" CssClass="grvResultadoConsultaCss ocultar"
                                                    Width="120px" Style="padding: 5px 5px 5px 5px" BackColor="White" Wrap="True">
                                                    <asp:Label ID="lblToolTipConcepto" runat="server" Text='<%# Eval("Concepto") %>'
                                                        CssClass="etiqueta " Font-Size="10px" /></asp:Panel>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="150px"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" Width="150px"></HeaderStyle>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle CssClass="grvPaginacionScroll" />
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="centradoMedio">
                            <asp:Button ID="btnGuardar" runat="server" CssClass="boton bg-color-azulClaro fg-color-blanco"
                                Text="GUARDAR" OnClick="btnGuardar_Click" Width="100px" />
                            <asp:Button ID="btnCancelarConciliar" runat="server" CssClass="boton bg-color-grisClaro01 fg-color-blanco"
                                Text="CANCELAR" OnClientClick="HideModalPopupConciliarMovPedido();" Width="100px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="bg-color-grisClaro01" colspan="2">
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:HiddenField runat="server" ID="hdfTipoCliente" />
    <asp:ModalPopupExtender ID="mpeTipoCliente" runat="server" PopupControlID="pnlTipoCliente"
        TargetControlID="hdfTipoCliente" BehaviorID="ModalBehaviourTipoCliente" BackgroundCssClass="ModalBackground">
    </asp:ModalPopupExtender>
    
    
    <%----%>
    <asp:Panel ID="pnlTipoCliente" runat="server" BackColor="#FFFFFF" Width="300px" Style="display: none">
        <asp:UpdatePanel ID="updTipoCliente" runat="server">
            <ContentTemplate>
                <table style="width: 100%;">
                    <tr class="bg-color-grisOscuro">
                        <td colspan="4" style="padding: 5px 5px 5px 5px" class="etiqueta">
                            <div class="floatDerecha">
                                <asp:ImageButton runat="server" ID="ImageButton1" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Cerrar.png"
                                    CssClass="iconoPequeño bg-color-rojo" OnClientClick="HideModalPopupTipoCliente();" /><%--OnClick="imgCerrarImportar_Click"--%>
                            </div>
                            <div class="fg-color-blanco">
                                BUSCAR CLIENTE COMO:<asp:HiddenField runat="server" ID="hdfClienteBuscar" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="datos-estilo;bg-color-grisClaro03;centradoMedio" style="padding: 10px 10px 10px 10px">
                            <div class="centradoMedio" style="width: 100%">
                                <asp:RadioButtonList ID="rdbTipoCliente" runat="server" RepeatColumns="2" RepeatDirection="Horizontal"
                                    Width="100%">
                                    <asp:ListItem Value="NORMA" Selected="True">Cliente Normal</asp:ListItem>
                                    <asp:ListItem Value="PADREL">Cliente Padre</asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="centradoMedio">
                            <asp:Button ID="btnBuscarPedidosCliente" runat="server" CssClass="boton bg-color-verdeClaro fg-color-blanco"
                                Text="BUSCAR" OnClick="btnBuscarPedidosCliente_Click" Width="100px" />
                            <asp:Button ID="btnCancelarBuscarPedidosCliente" runat="server" CssClass="boton bg-color-grisClaro01 fg-color-blanco"
                                Text="CANCELAR" OnClientClick="HideModalPopupTipoCliente();" Width="100px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="bg-color-grisClaro01" colspan="2">
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>


            <asp:HiddenField runat="server" ID="hdfBusquedaFactura" />
    <asp:ModalPopupExtender ID="mpeBusquedaFactura" runat="server" PopupControlID="pnlBusquedaFactura"
        TargetControlID="hdfBusquedaFactura" BehaviorID="ModalBehaviourBusquedaFactura" BackgroundCssClass="ModalBackground">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnlBusquedaFactura" runat="server" BackColor="#FFFFFF" Width="600px">
        <asp:UpdatePanel ID="upBusquedaFactura" runat="server">
            <ContentTemplate>
                <table style="width: 100%;">
                    <tr class="bg-color-grisOscuro">
                        <td colspan="4" style="padding: 5px 5px 5px 5px" class="etiqueta">
                            <div class="floatDerecha">
                                <asp:ImageButton runat="server" ID="ImageButton2" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Cerrar.png"
                                    CssClass="iconoPequeño bg-color-rojo" OnClientClick="HideModalPopupConciliarMovPedido();" /><%--OnClick="imgCerrarImportar_Click"--%>
                            </div>
                            <div class="fg-color-blanco">
                                BUSQUEDA FACTURAS.
                            </div>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%;">   
                    <tr>
                        <td class="datos-estilo;bg-color-grisClaro03" style="padding: 10px 10px 10px 10px">
                            <div class="etiqueta lineaHorizontal">
                                <table width="100%">
                                    <tr class="etiqueta centradoJustificado fg-color-blanco bg-color-azulClaro">
                                    <td style="width: 50%; padding: 5px 5px 5px 5px" colspan="2">
                                    <asp:RadioButtonList ID="rblTipoClienteFactura" runat="server" RepeatColumns="2" RepeatDirection="Horizontal"
                                        Width="70%">
                                        <asp:ListItem Value="NORMA" Selected="True">Cliente Normal</asp:ListItem>
                                        <asp:ListItem Value="PADREL">Cliente Padre</asp:ListItem>
                                    </asp:RadioButtonList>
                                    </td>  
                                    <td style="width: 25%; padding: 5px 5px 5px 5px"  colspan="2" align="left">
                                            Fecha Factura:
                                            <asp:TextBox ID="txtFechaFacturaBusqueda" runat="server"  CssClass="cajaTexto" Font-Size="12px"
                                            Width="90%">
                                            </asp:TextBox>
									</td>
								    <td style="width: 25%; padding: 5px 5px 5px 5px" colspan="2">
                                            Factura:
                                           <asp:TextBox ID="txtFacturaBusuqeda" runat="server" CssClass="cajaTexto" Font-Size="12px"
                                            Width="90%">
                                            </asp:TextBox>
                                    </td>                                
                                    </tr>
                                    <tr>
                                        <td class="etiqueta lineaVertical centradoMedio" style="width: 40%; padding: 5px 5px 5px 5px">
                                            CLIENTE:
                                            <asp:Label ID="lblClienteFactura" runat="server" CssClass="fg-color-negro"></asp:Label>
                                        </td>
                                        <td class="etiqueta lineaVertical centradoMedio" style="width: 15%; padding: 5px 5px 5px 5px">
                                            Monto Conciliar:
                                        </td>
                                        <td class="etiqueta lineaVertical centradoMedio bg-color-grisOscuro fg-color-blanco"
                                            style="width: 15%; padding: 5px 5px 5px 5px">
                                            <asp:Label runat="server" ID="lblMontoExternoFactura" Text="$ 0.00"></asp:Label>
                                        </td>
                                        <td class="etiqueta lineaVertical centradoMedio" style="width: 15%; padding: 5px 5px 5px 5px">
                                            Resto:
                                        </td>
                                        <td class="etiqueta lineaVertical centradoMedio bg-color-azul fg-color-blanco" style="width: 15%;
                                            padding: 5px 5px 5px 5px">
                                            <asp:Label runat="server" ID="lblMontoRestoFactrura"></asp:Label>
                                        </td>

                                        <td class="iconoOpcion bg-color-naranja" rowspan="2">
                                            <asp:ImageButton ID="imgBotonBuscarFacturasManual" runat="server" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Buscar.png"
                                                ToolTip="BUSCAR" style="width: 25px; padding: 10px 2px 2px 2px"  OnClick="imgBotonBuscarFacturasManual_Click" />
                                            <%--Width="25px"--%>
                                        </td>

                                    </tr>
                                    </table>
                            </div>
                            </td>
                        </tr>
                    <tr>
                        <td>

                            <div style="width:100%; height:200px; overflow: scroll;">
                                <asp:GridView ID="grvPedidosFacturados" runat="server" AutoGenerateColumns="False" ShowHeader="True"
                                    CssClass="grvResultadoConsultaCssE" AllowSorting="True" ShowFooter="False" Width="100%"
                                    ShowHeaderWhenEmpty="True" DataKeyNames="Cliente" AllowPaging="False"
                                    PageSize="10" OnPageIndexChanging="grvFacturasManuales_PageIndexChanging">
                                  
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <Columns>
                                         <asp:TemplateField HeaderText="Factura" SortExpression="FoliFactura">
                                            <ItemTemplate>
                                                <asp:Label ID="lblFolioFacturaFACT" runat="server" Text='<%# Eval("FolioFactura") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="70px" BackColor="#ebecec"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" Width="70px"></HeaderStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cliente" SortExpression="Cliente">
                                            <ItemTemplate>
                                                <asp:Label ID="lblClienteFACT" runat="server" Text='<%# Eval("Cliente") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="70px" BackColor="#ebecec"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" Width="70px"></HeaderStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Nombre" SortExpression="Nombre">
                                            <ItemTemplate>
                                                <asp:Label ID="lblNombreClienteFACT" runat="server" Text='<%# Eval("NombreCliente") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="70px" BackColor="#ebecec"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" Width="70px"></HeaderStyle>
                                        </asp:TemplateField>
   
                                         <asp:TemplateField HeaderText="Fecha" SortExpression="Fecha">
                                            <ItemTemplate>
                                                <asp:Label ID="lblFechaFACT" runat="server" Text='<%# Eval("FechaFactura","{0:d}") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" Width="70px" BackColor="#ebecec"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" Width="70px"></HeaderStyle>
                                        </asp:TemplateField>
      
                                    </Columns>
                                    <PagerStyle CssClass="grvPaginacionScroll" />
                                </asp:GridView>
                            </div>

                        </td>
                    </tr>
                    <tr>
                        <td class="centradoMedio">
                            <asp:Button ID="btnSalir" runat="server" CssClass="boton bg-color-azulClaro fg-color-blanco"
                                OnClick="btnSalir_Click" 
                                OnClientClick="HideModalPopupFacturasPedido();"
                                Text="SALIR" Width="100px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="bg-color-grisClaro01" colspan="2">
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>

    <asp:HiddenField runat="server" ID="hdfMesAño" />
    <asp:ModalPopupExtender ID="mpeMesAño" runat="server" PopupControlID="pnlMesAño"
        TargetControlID="hdfMesAño" BehaviorID="ModalBehaviourMesAño" BackgroundCssClass="ModalBackground">
    </asp:ModalPopupExtender>
    <%----%>
    <asp:Panel ID="pnlMesAño" runat="server" BackColor="#FFFFFF" Width="300px" Style="display: none">
        <asp:UpdatePanel ID="upMesAño" runat="server">
            <ContentTemplate>
                <table style="width: 100%;">
                    <tr class="bg-color-grisOscuro">
                        <td colspan="4" style="padding: 5px 5px 5px 5px" class="etiqueta">
                            <div class="floatDerecha">
                                <asp:ImageButton runat="server" ID="imgCerrarMesAño" ImageUrl="~/App_Themes/GasMetropolitanoSkin/Iconos/Cerrar.png"
                                    CssClass="iconoPequeño bg-color-rojo" OnClientClick="HideModalPopupMesAño();" /><%--OnClick="imgCerrarImportar_Click"--%>
                            </div>
                            <div class="fg-color-blanco">
                                SELECCIONAR MES /AÑO CIERRE
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="datos-estilo;bg-color-grisClaro03;centradoMedio" style="padding: 10px 10px 10px 10px">
                            <div class="centradoMedio" style="width: 100%">
                                <asp:DropDownList runat="server" ID="ddlMesAño" CssClass="dropDown centradoMedio etiqueta"
                                    Width="100%" Font-Size="12px">
                                </asp:DropDownList>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="centradoMedio">
                            <asp:Button ID="btnCerrarMesAño" runat="server" CssClass="boton bg-color-verdeClaro fg-color-blanco"
                                Text="CERRAR" OnClick="btnMesAñoCierre_Click" Width="100px" />
                            <asp:Button ID="btnCancelarMesAño" runat="server" CssClass="boton bg-color-grisClaro01 fg-color-blanco"
                                Text="CANCELAR" OnClientClick="HideModalPopupMesAño();" Width="100px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="bg-color-grisClaro01" colspan="2">
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <asp:UpdateProgress ID="panelBloqueo" runat="server" AssociatedUpdatePanelID="upConciliacionCompartida">
        <ProgressTemplate>
            <asp:Image ID="imgLoad" runat="server" CssClass="icono bg-color-blanco" Height="40px"
                ImageUrl="~/App_Themes/GasMetropolitanoSkin/Imagenes/LoadPage.gif" Width="40px" />
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:ModalPopupExtender ID="mpeLoading" runat="server" BackgroundCssClass="ModalBackground"
        PopupControlID="panelBloqueo" TargetControlID="panelBloqueo">
    </asp:ModalPopupExtender>
</asp:Content>
