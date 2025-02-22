﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Conciliacion.RunTime;
using Conciliacion.RunTime.ReglasDeNegocio;
using CatalogoConciliacion.ReglasNegocio;
using SeguridadCB.Public;
using Consultas = Conciliacion.RunTime.ReglasDeNegocio.Consultas;


public partial class Conciliacion_FormasConciliar_UnoAVarios : System.Web.UI.Page
{
    #region "Propiedades Globales"

    private SeguridadCB.Public.Parametros parametros;
    private SeguridadCB.Public.Operaciones operaciones;
    private SeguridadCB.Public.Usuario usuario;

    public List<ReferenciaNoConciliada> listaReferenciaExternas = new List<ReferenciaNoConciliada>();
    public List<ReferenciaNoConciliada> listaReferenciaArchivosInternos = new List<ReferenciaNoConciliada>();

    public List<ReferenciaNoConciliada> listaTransaccionesConciliadas = new List<ReferenciaNoConciliada>();
    public List<ReferenciaNoConciliadaPedido> listaReferenciaPedidos = new List<ReferenciaNoConciliadaPedido>();


    private List<ListaCombo> listSucursales = new List<ListaCombo>();
    private List<ListaCombo> listCelulas = new List<ListaCombo>();
    private List<ListaCombo> listStatusConcepto = new List<ListaCombo>();
    private List<ListaCombo> listFormasConciliacion = new List<ListaCombo>();
    private List<ListaCombo> listMotivosNoConciliados = new List<ListaCombo>();

    private DataTable tblTransaccionesConciliadas;
    private DataTable tblReferenciaExternas;
    private DataTable tblReferenciaInternas;
    public List<ListaCombo> listCamposDestino = new List<ListaCombo>();
    public DataTable tblDetalleTransaccionConciliada;
    public DataTable tblReferenciaAgregadasInternas;



    #endregion

    private string DiferenciaDiasMaxima, DiferenciaDiasMinima, DiferenciaCentavosMaxima, DiferenciaCentavosMinima;
    public int corporativo, año, folio, sucursal;
    public short mes, tipoConciliacion, grupoConciliacion;
    //public int indiceExternoSeleccionado = 0;
    //public int indiceInternoSeleccionado = 0;
    public bool statusFiltro;
    public string tipoFiltro;
    public DateTime dateMin;

    public ReferenciaNoConciliada tranDesconciliar;
    public ReferenciaNoConciliada tranExternaAnteriorSeleccionada;

    private DatosArchivo datosArchivoInterno;
    private List<ListaCombo> listTipoFuenteInformacionExternoInterno = new List<ListaCombo>();
    public List<ListaCombo> listFoliosInterno = new List<ListaCombo>();
    public List<DatosArchivo> listArchivosInternos = new List<DatosArchivo>();

    private DataTable tblDestinoDetalleInterno;
    private List<DatosArchivoDetalle> listaDestinoDetalleInterno = new List<DatosArchivoDetalle>();

    public List<ListaCombo> listaCorporativoTransferencia = new List<ListaCombo>();
    public List<ListaCombo> listaSucursalTransferencia = new List<ListaCombo>();
    public List<ListaCombo> listaNombreBancoTransferencia = new List<ListaCombo>();
    public List<ListaCombo> listaCuentaBancoTransferencia = new List<ListaCombo>();


    //public List<TransferenciaBancarias> ListTransferenciasBancarias = new List<TransferenciaBancarias>();

    private int indiceExternoSeleccionado
    {
        get { return Convert.ToInt32(hdfIndiceExterno.Value); }
        set { hdfIndiceExterno.Value = value.ToString(); }
    }

    private int indiceInternoSeleccionado
    {
        get { return Convert.ToInt32(hdfIndiceInterno.Value); }
        set { hdfIndiceInterno.Value = value.ToString(); }
    }

    protected override void OnPreInit(EventArgs e)
    {
        if (HttpContext.Current.Session["Operaciones"] == null)
            Response.Redirect("~/Acceso/Acceso.aspx", true);
        else
            operaciones = (SeguridadCB.Public.Operaciones)HttpContext.Current.Session["Operaciones"];

        if (HttpContext.Current.Session["Parametros"] == null)
            Response.Redirect("~/Acceso/Acceso.aspx", true);
        else
            parametros = (SeguridadCB.Public.Parametros)HttpContext.Current.Session["Parametros"];
    }



    protected void Page_Load(object sender, EventArgs e)
    {

        Conciliacion.RunTime.App.ImplementadorMensajes.ContenedorActual = this;
        try
        {
            Conciliacion.RunTime.App.ImplementadorMensajes.ContenedorActual = this;
            if (HttpContext.Current.Request.UrlReferrer != null)
            {
                if ((!HttpContext.Current.Request.UrlReferrer.AbsoluteUri.Contains("SitioConciliacion")) ||
                    (HttpContext.Current.Request.UrlReferrer.AbsoluteUri.Contains("Acceso.aspx")))
                {
                    HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
                    HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(false);
                }
            }
            if (!Page.IsPostBack)
            {
                //Leer variables de URL
                corporativo = Convert.ToInt32(Request.QueryString["Corporativo"]);
                sucursal = Convert.ToInt16(Request.QueryString["Sucursal"]);
                año = Convert.ToInt32(Request.QueryString["Año"]);
                folio = Convert.ToInt32(Request.QueryString["Folio"]);
                mes = Convert.ToSByte(Request.QueryString["Mes"]);
                tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);
                grupoConciliacion = Convert.ToSByte(Request.QueryString["GrupoConciliacion"]);

                statusFiltro = false;
                Session["StatusFiltro"] = statusFiltro;
                tipoFiltro = String.Empty;
                Session["TipoFiltro"] = tipoFiltro;
                //INICIALIZAR QUE SE MOSTRARAN

                activarVerPendientesCanceladosExternos(true);
                activarVerPendientesCanceladosInternos(true);

                CargarRangoDiasDiferenciaGrupo(grupoConciliacion);
                Carga_StatusConcepto(Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConfiguracionStatusConcepto.ConEtiquetas);
                Carga_FormasConciliacion(tipoConciliacion);
                cargar_ComboMotivosNoConciliado();
                LlenarBarraEstado();
                //CARGAR LAS TRANSACCIONES CONCILIADAS POR EL CRITERIO DE CONCILIACION
                Consulta_TransaccionesConciliadas(corporativo, sucursal, año, mes, folio,
                                                  Convert.ToInt32(ddlCriteriosConciliacion.SelectedValue));
                GenerarTablaConciliados();
                LlenaGridViewConciliadas();
                Consulta_Externos(corporativo, sucursal, año, mes, folio, Convert.ToDecimal(txtDiferencia.Text),
                                  tipoConciliacion, Convert.ToInt32(ddlStatusConcepto.SelectedValue), EsDepositoRetiro());
                GenerarTablaExternos();
                LlenaGridViewExternos();
                ActualizarTotalesAgregados();
                if (grvExternos.Rows.Count > 0)
                {
                    //Obtener el la referencia externa seleccionada
                    if (tipoConciliacion == 2)
                    {
                        lblSucursalCelula.Text = "Celula Interna";
                        ddlCelula.Visible = lblPedidos.Visible = rdbTodosMenoresIn.Visible = true;
                        btnENPROCESOINTERNO.Visible = btnCANCELARINTERNO.Visible =
                                                                            lblVer.Visible =
                                                                            txtDias.CausesValidation =
                                                                            txtDias.Enabled = ddlSucursal.Enabled =
                                                                                              btnHistorialPendientesInterno
                                                                                                  .Visible =
                                                                                              tdEtiquetaMontoIn.Visible
                                                                                              =
                                                                                              tdMontoIn.Visible = false;
                        //imgExportar.Enabled = 
                        lblGridAP.Text = "PEDIDOS ";
                        //tdExportar.Attributes.Add("class", "iconoOpcion bg-color-grisClaro02");


                        Carga_CelulaCorporativo(corporativo);
                         
                        /**Modifico: CNSM 
                        Fecha: 08/06/2017
                        ConsultarPedidosInternos();**/

                        ConsultaInicialPedidosInternos();

                        //CHECAR SI SE DEJA EL CAMBIO DE VALIDATIONGROUP
                        btnActualizarConfig.ValidationGroup = "UnoVariosPedidos";
                        rfvDiferenciaVacio.ValidationGroup = "UnoVariosPedidos";
                        rvDiferencia.ValidationGroup = "UnoVariosPedidos";

                    }
                    else
                    {
                        lblSucursalCelula.Text = "Sucursal Interna";
                        ddlSucursal.Visible = true;
                        Carga_SucursalCorporativo(corporativo);
                        ConsultarArchivosInternos();

                        btnActualizarConfig.ValidationGroup = "UnoVarios";
                        lblArchivosInternos.Visible = true;
                        lblGridAP.Text = "INTERNOS ";
                    }
                }
                btnGuardar.Enabled = false;
                ocultarFiltroFechas(tipoConciliacion);
                ocultarAgregarPedidoDirecto(tipoConciliacion);

                Carga_TipoFuenteInformacionInterno(Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConfiguracionTipoFuente.TipoFuenteInformacionInterno);
                activarImportacion(tipoConciliacion);
            }
        }
        catch (SqlException ex)
        {
            App.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);

            if (ex.Class >= 20)
            {
                Response.Redirect("~/Inicio.aspx", true);
            }
        }
        catch (Exception ex)
        {
            App.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);
        }
    }

    //Cargar InfoConciliacion Actual
    public void cargarInfoConciliacionActual()
    {
        try
        {
            corporativo = Convert.ToInt32(Request.QueryString["Corporativo"]);
            sucursal = Convert.ToInt16(Request.QueryString["Sucursal"]);
            año = Convert.ToInt32(Request.QueryString["Año"]);
            folio = Convert.ToInt32(Request.QueryString["Folio"]);
            mes = Convert.ToSByte(Request.QueryString["Mes"]);
            tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);
        }
        catch (Exception ex)
        {
            throw ex;
        }
       
    }
    //Limpian variables de Session
    public void limpiarVariablesSession()
    {
        //Eliminar las variables de Session utilizadas en la Vista
        HttpContext.Current.Session["StatusFiltro"] = null;
        HttpContext.Current.Session["TipoFiltro"] = null;
        HttpContext.Current.Session["CONCILIADAS"] = null;
        HttpContext.Current.Session["TAB_CONCILIADAS"] = null;
        HttpContext.Current.Session["TAB_CONCILIADAS_AX"] = null;
        HttpContext.Current.Session["TAB_INTERNOS_AX"] = null;
        HttpContext.Current.Session["TAB_INTERNOS"] = null;
        HttpContext.Current.Session["POR_CONCILIAR_INTERNO"] = null;
        HttpContext.Current.Session["POR_CONCILIAR_EXTERNO"] = null;
        HttpContext.Current.Session["TAB_EXTERNOS"] = null;
        HttpContext.Current.Session["TAB_EXTERNOS_AX"] = null;
        HttpContext.Current.Session["RepDoc"] = null;
        HttpContext.Current.Session["ParametrosReporte"] = null;
        HttpContext.Current.Session["NUEVOS_INTERNOS"] = null;
        HttpContext.Current.Session["DETALLEINTERNO"] = null;

        HttpContext.Current.Session.Remove("StatusFiltro");
        HttpContext.Current.Session.Remove("TipoFiltro");
        HttpContext.Current.Session.Remove("CONCILIADAS");
        HttpContext.Current.Session.Remove("TAB_CONCILIADAS");
        HttpContext.Current.Session.Remove("TAB_CONCILIADAS_AX");
        HttpContext.Current.Session.Remove("TAB_INTERNOS_AX");
        HttpContext.Current.Session.Remove("TAB_INTERNOS");
        HttpContext.Current.Session.Remove("POR_CONCILIAR_INTERNO");
        HttpContext.Current.Session.Remove("POR_CONCILIAR_EXTERNO");
        HttpContext.Current.Session.Remove("TAB_EXTERNOS");
        HttpContext.Current.Session.Remove("TAB_EXTERNOS_AX");
        HttpContext.Current.Session.Remove("RepDoc");
        HttpContext.Current.Session.Remove("ParametrosReporte");
        HttpContext.Current.Session.Remove("NUEVOS_INTERNOS");
        HttpContext.Current.Session.Remove("DETALLEINTERNO");

    }
    //Cargar Rango DiasMaximo-Minimio-Default
    public void CargarRangoDiasDiferenciaGrupo(short grupoC)
    {
        try
        {
            GrupoConciliacionDiasDiferencia gcd = App.GrupoConciliacionDias(grupoC);
            if (!gcd.CargarDatos())
            {
                App.ImplementadorMensajes.MostrarMensaje("Conflicto al leer Grupo Conciliación");
                return;
            }
            txtDias.Text = Convert.ToString(gcd.DiferenciaDiasDefault);
            txtDiferencia.Text = Convert.ToString(gcd.DiferenciaCentavosDefault);

            DiferenciaDiasMaxima = Convert.ToString(gcd.DiferenciaDiasMaxima);
            DiferenciaDiasMinima = Convert.ToString(gcd.DiferenciaDiasMinima);
            DiferenciaCentavosMaxima = Convert.ToString(gcd.DiferenciaCentavosMaxima);
            DiferenciaCentavosMinima = Convert.ToString(gcd.DiferenciaCentavosMinima);

            rvDias.MaximumValue = DiferenciaDiasMaxima;
            rvDias.MinimumValue = DiferenciaDiasMinima;
            rvDias.ErrorMessage = "[Dias entre " + DiferenciaDiasMinima + " - " + DiferenciaDiasMaxima + "]";

            rvDiferencia.MaximumValue = DiferenciaCentavosMaxima;
            rvDiferencia.MinimumValue = DiferenciaCentavosMinima;
            rvDiferencia.ErrorMessage = "[Diferencia entre " + DiferenciaCentavosMinima + " - " + DiferenciaCentavosMaxima +
                                        " pesos]";
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Consulta campos de Filtro y Busqueda Externo
    /// </summary>
    public void cargar_ComboCampoFiltroDestino(int tConciliacion, string filtrarEn)
    {
        try
        {
            listCamposDestino = filtrarEn.Equals("Externos") || filtrarEn.Equals("Conciliados")
                                ? Conciliacion.RunTime.App.Consultas.ConsultaDestino()
                                : (tConciliacion != 2
                                       ? Conciliacion.RunTime.App.Consultas.ConsultaDestino()
                                       : Conciliacion.RunTime.App.Consultas.ConsultaDestinoPedido());
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        

    }

    /// <summary>
    /// Cargar campos de Filtro y Busqueda externo
    /// </summary>
    public void enlazarComboCampoFiltrarDestino()
    {

        this.ddlCampoFiltrar.DataSource = listCamposDestino;
        this.ddlCampoFiltrar.DataValueField = "Identificador";
        this.ddlCampoFiltrar.DataTextField = "Descripcion";
        this.ddlCampoFiltrar.DataBind();
        this.ddlCampoFiltrar.Dispose();
    }
    /// <summary>
    /// Cargar Combo de Motivos por lo q no se Cancela la Tranasaccion
    /// </summary>
    public void cargar_ComboMotivosNoConciliado()
    {
        try
        {
            listMotivosNoConciliados = Conciliacion.RunTime.App.Consultas.ConsultaMotivoNoConciliado();
            this.ddlMotivosNoConciliado.DataSource = listMotivosNoConciliados;
            this.ddlMotivosNoConciliado.DataValueField = "Identificador";
            this.ddlMotivosNoConciliado.DataTextField = "Descripcion";
            this.ddlMotivosNoConciliado.DataBind();
            this.ddlMotivosNoConciliado.Dispose();
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }

        
    }

    /// <summary>
    /// Llena la barra de estado.
    /// </summary>
    public void LlenarBarraEstado()
    {
        try
        {
            cConciliacion c = App.Consultas.ConsultaConciliacionDetalle(corporativo, sucursal, año, mes, folio);
            lblFolio.Text = c.Folio.ToString();
            lblBanco.Text = c.BancoStr;
            lblCuenta.Text = c.CuentaBancaria;
            lblGrupoCon.Text = c.GrupoConciliacionStr;
            lblSucursal.Text = c.SucursalDes;
            lblTipoCon.Text = c.TipoConciliacionStr;
            lblMesAño.Text = c.Mes + "/" + c.Año;
            lblConciliadasExt.Text = c.ConciliadasExternas.ToString();
            lblConciliadasInt.Text = c.ConciliadasInternas.ToString();
            lblMontoTotalExterno.Text = c.MontoTotalExterno.ToString("C2");
            lblMontoTotalInterno.Text = c.MontoTotalInterno.ToString("C2");
            lblStatusConciliacion.Text = c.StatusConciliacion;
            imgStatusConciliacion.ImageUrl = c.UbicacionIcono;
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }

        
    }

    /// <summary>
    /// Llena el Combo de Sucurales según Corporativo
    /// </summary>
    public void Carga_StatusConcepto(Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConfiguracionStatusConcepto cConcepto)
    {
        try
        {
        listStatusConcepto = Conciliacion.RunTime.App.Consultas.ConsultaStatusConcepto(cConcepto);
        this.ddlStatusConcepto.DataSource = listStatusConcepto;
        this.ddlStatusConcepto.DataValueField = "Identificador";
        this.ddlStatusConcepto.DataTextField = "Descripcion";
        this.ddlStatusConcepto.DataBind();
        this.ddlStatusConcepto.Dispose();
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Llena el Combo de Sucurales según Corporativo
    /// </summary>
    public void Carga_SucursalCorporativo(int corporativo)
    {
        try
        {
            listSucursales =
                Conciliacion.RunTime.App.Consultas.ConsultaSucursales(
                    Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConfiguracionIden0.Sin0, corporativo);
            this.ddlSucursal.DataSource = this.ddlSucursalInterno.DataSource = listSucursales;
            this.ddlSucursal.DataValueField = this.ddlSucursalInterno.DataValueField = "Identificador";
            this.ddlSucursal.DataTextField = this.ddlSucursalInterno.DataTextField = "Descripcion";

            this.ddlSucursal.DataBind();
            this.ddlSucursal.Dispose();
            this.ddlSucursalInterno.DataBind();
            this.ddlSucursalInterno.Dispose();

        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Llena el Combo de Formas de Conciliacion
    /// </summary>
    public void Carga_CelulaCorporativo(int corporativo)
    {
        try
        {
        listCelulas = Conciliacion.RunTime.App.Consultas.ConsultaCelula(corporativo);
        this.ddlCelula.DataSource = listCelulas;
        this.ddlCelula.DataValueField = "Identificador";
        this.ddlCelula.DataTextField = "Descripcion";
        this.ddlCelula.DataBind();
        this.ddlCelula.Dispose();
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //Colocar el DropDown de Criterios de Evaluacion en la Actual
    public void ActualizarCriterioEvaluacion()
    {
        ddlCriteriosConciliacion.SelectedValue = ddlCriteriosConciliacion.Items.FindByText("UNO A VARIOS").Value;
    }

    /// <summary>
    /// Llena el Combo de Formas de Conciliacion
    /// </summary>
    public void Carga_FormasConciliacion(short tipoConciliacion)
    {
        try
        {
        listFormasConciliacion = Conciliacion.RunTime.App.Consultas.ConsultaFormaConciliacion(tipoConciliacion);
        this.ddlCriteriosConciliacion.DataSource = listFormasConciliacion;
        this.ddlCriteriosConciliacion.DataValueField = "Identificador";
        this.ddlCriteriosConciliacion.DataTextField = "Descripcion";
        this.ddlCriteriosConciliacion.DataBind();
        this.ddlCriteriosConciliacion.Dispose();
        ActualizarCriterioEvaluacion();
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    //Crea la paginacion para Concilidos
    protected void grvConciliadas_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Pager && (grvConciliadas.DataSource != null))
        {
            //TRAE EL TOTAL DE PAGINAS
            Label _TotalPags = (Label)e.Row.FindControl("lblTotalNumPaginas");
            _TotalPags.Text = grvConciliadas.PageCount.ToString();

            //LLENA LA LISTA CON EL NUMERO DE PAGINAS
            DropDownList list = (DropDownList)e.Row.FindControl("paginasDropDownListConciliadas");
            for (int i = 1; i <= Convert.ToInt32(grvConciliadas.PageCount); i++)
            {
                list.Items.Add(i.ToString());
            }
            list.SelectedValue = (grvConciliadas.PageIndex + 1).ToString();
        }
    }

    //Asignar Valores Css de cada Row
    protected void grvConciliadas_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes.Add("onmouseover", "this.className='bg-color-grisClaro02'");
            e.Row.Attributes.Add("onmouseout", "this.className='bg-color-blanco'");
        }

    }

    //Paginacion de los Concilidos
    protected void grvConciliadas_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            grvConciliadas.PageIndex = e.NewPageIndex;
            LlenaGridViewConciliadas();
        }
        catch (Exception)
        {
            //App.ImplementadorMensajes.MostrarMensaje(ex.Message);
        }
    }

    //Llena el dropDown de  paginacion para Conciliados
    protected void paginasDropDownListConciliadas_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList oIraPag = sender as DropDownList;
        int iNumPag;

        grvConciliadas.PageIndex = int.TryParse(oIraPag.Text.Trim(), out iNumPag) && iNumPag > 0 &&
                                   iNumPag <= grvConciliadas.PageCount
                                       ? iNumPag - 1
                                       : 0;

        LlenaGridViewConciliadas();
    }

    //Consulta transacciones conciliadas
    public void Consulta_TransaccionesConciliadas(int corporativoconciliacion, int sucursalconciliacion,
                                                  int añoconciliacion, short mesconciliacion, int folioconciliacion,
                                                  int formaconciliacion)
    {
        System.Data.SqlClient.SqlConnection connection = SeguridadCB.Seguridad.Conexion;
        if (connection.State == ConnectionState.Closed)
        {
            SeguridadCB.Seguridad.Conexion.Open();
        }
        try
        {
            listaTransaccionesConciliadas =
                Conciliacion.RunTime.App.Consultas.ConsultaTransaccionesConciliadas(corporativoconciliacion,
                                                                                    sucursalconciliacion,
                                                                                    añoconciliacion, mesconciliacion,
                                                                                    folioconciliacion,
                                                                                    Convert.ToInt16(
                                                                                        ddlCriteriosConciliacion
                                                                                            .SelectedValue));

            Session["CONCILIADAS"] = listaTransaccionesConciliadas;
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    //Genera la tabla de transacciones Conciliadas
    public void GenerarTablaConciliados()
    {
        try
        {
            tblTransaccionesConciliadas = new DataTable("TransaccionesConciladas");
            tblTransaccionesConciliadas.Columns.Add("CorporativoConciliacion", typeof(int));
            tblTransaccionesConciliadas.Columns.Add("SucursalConciliacion", typeof(int));
            tblTransaccionesConciliadas.Columns.Add("AñoConciliacion", typeof(int));
            tblTransaccionesConciliadas.Columns.Add("MesConciliacion", typeof(int));
            tblTransaccionesConciliadas.Columns.Add("FolioConciliacion", typeof(int));
            tblTransaccionesConciliadas.Columns.Add("SecuenciaExterno", typeof(int));

            tblTransaccionesConciliadas.Columns.Add("FolioExterno", typeof(int));
            tblTransaccionesConciliadas.Columns.Add("RFCTercero", typeof(string));
            tblTransaccionesConciliadas.Columns.Add("Referencia", typeof(string));
            tblTransaccionesConciliadas.Columns.Add("NombreTercero", typeof(string));
            tblTransaccionesConciliadas.Columns.Add("FMovimiento", typeof(DateTime));
            tblTransaccionesConciliadas.Columns.Add("FOperacion", typeof(DateTime));
            tblTransaccionesConciliadas.Columns.Add("MontoConciliado", typeof(decimal));
            tblTransaccionesConciliadas.Columns.Add("Retiro", typeof(decimal));
            tblTransaccionesConciliadas.Columns.Add("Deposito", typeof(decimal));
            tblTransaccionesConciliadas.Columns.Add("Cheque", typeof(string));
            tblTransaccionesConciliadas.Columns.Add("Concepto", typeof(string));
            tblTransaccionesConciliadas.Columns.Add("Descripcion", typeof(string));

            foreach (ReferenciaNoConciliada rc in listaTransaccionesConciliadas)
            {
                tblTransaccionesConciliadas.Rows.Add(
                    rc.Corporativo,
                    rc.Sucursal,
                    rc.Año,
                    rc.MesConciliacion,
                    rc.FolioConciliacion,
                    rc.Secuencia,
                    rc.Folio,
                    rc.RFCTercero,
                    rc.Referencia,
                    rc.NombreTercero,
                    rc.FMovimiento,
                    rc.FOperacion,
                    rc.Monto,
                    rc.Retiro,
                    rc.Deposito,
                    rc.Cheque,
                    rc.Concepto,
                    rc.Descripcion);
            }

            HttpContext.Current.Session["TAB_CONCILIADAS"] = tblTransaccionesConciliadas;
            HttpContext.Current.Session["TAB_CONCILIADAS_AX"] = tblTransaccionesConciliadas;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        
    }

    //Llena el Gridview Transacciones Concilidadas
    private void LlenaGridViewConciliadas()
    {
        try
        {
            DataTable tablaConciliadas = (DataTable)HttpContext.Current.Session["TAB_CONCILIADAS"];
            grvConciliadas.DataSource = tablaConciliadas;
            grvConciliadas.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    //Genera la tabla DetalleTransaccionesConciliadas
    public void GeneraTablaDetalleArchivosInternos(ReferenciaNoConciliada trConciliada)
    {
        tblDetalleTransaccionConciliada = new DataTable("DetalleTransaccionConciliada");
        if (trConciliada.ConInterno)
        {
            tblDetalleTransaccionConciliada.Columns.Add("SecuenciaInterno", typeof(int));
            tblDetalleTransaccionConciliada.Columns.Add("FolioInterno", typeof(int));
            tblDetalleTransaccionConciliada.Columns.Add("FMovimientoInt", typeof(DateTime));
            tblDetalleTransaccionConciliada.Columns.Add("FOperacionInt", typeof(DateTime));
            tblDetalleTransaccionConciliada.Columns.Add("MontoInterno", typeof(Decimal));
            tblDetalleTransaccionConciliada.Columns.Add("ConceptoInterno", typeof(string));
        }
        else
        {
            tblDetalleTransaccionConciliada.Columns.Add("Pedido", typeof(int));
            tblDetalleTransaccionConciliada.Columns.Add("PedidoReferencia", typeof(string));
            tblDetalleTransaccionConciliada.Columns.Add("AñoPed", typeof(int));
            tblDetalleTransaccionConciliada.Columns.Add("Celula", typeof(int));
            tblDetalleTransaccionConciliada.Columns.Add("Cliente", typeof(string));
            tblDetalleTransaccionConciliada.Columns.Add("Nombre", typeof(string));
            tblDetalleTransaccionConciliada.Columns.Add("Total", typeof(decimal));
            tblDetalleTransaccionConciliada.Columns.Add("ConceptoPedido", typeof(string));
        }
    }

    public void ConsultaDetalleTransaccionConciliada(ReferenciaNoConciliada trConciliada)
    {
        try
        {
            if (trConciliada.ConInterno)
            {
                foreach (ReferenciaConciliada r in trConciliada.ListaReferenciaConciliada)
                {
                    tblDetalleTransaccionConciliada.Rows.Add(
                        r.SecuenciaInterno,
                        r.FolioInterno,
                        r.FMovimientoInt,
                        r.FOperacionInt,
                        r.MontoInterno,
                        r.ConceptoInterno);
                }
            }
            else
            {
                foreach (ReferenciaConciliadaPedido r in trConciliada.ListaReferenciaConciliada)
                {
                    tblDetalleTransaccionConciliada.Rows.Add(
                        r.Pedido,
                        r.PedidoReferencia,
                        r.AñoPedido,
                        r.CelulaPedido,
                        r.Cliente,
                        r.Nombre,
                        r.Total,
                        r.ConceptoPedido
                        );
                }
            }
        }
        catch (Exception ex)
        {
            App.ImplementadorMensajes.MostrarMensaje(ex.Message);
        }
    }

    public void LlenarGridDetalleInterno(ReferenciaNoConciliada trConciliada)
    {
        if (trConciliada.ConInterno)
        {
            grvDetalleArchivoInterno.DataSource = tblDetalleTransaccionConciliada;
            grvDetalleArchivoInterno.DataBind();
        }
        else
        {
            grvDetallePedidoInterno.DataSource = tblDetalleTransaccionConciliada;
            grvDetallePedidoInterno.DataBind();
        }
    }

    protected void grvExternos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
            ((CheckBox)e.Row.FindControl("chkTodosExternos")).Enabled = hdfExternosControl.Value.Equals("PENDIENTES");

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (hdfExternosControl.Value.Equals("PENDIENTES"))
            {
                if (((Image)e.Row.FindControl("imgStatusConciliacion")).AlternateText.Equals("CONCILIACION CANCELADA"))
                    ((RadioButton)e.Row.FindControl("rdbSecuencia")).Enabled = false;
            }
            else
                ((CheckBox)e.Row.FindControl("chkExterno")).Enabled = false;

            //CheckBox cb = (CheckBox)e.Row.FindControl("chkExterno");
            //cb.Attributes.Add("onclick", "setRowBackColor(this,'" + e.Row.RowState.ToString() + "');");

        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            if (grvExternos.Rows.Count > 0)
            {
                RadioButton rdb = grvExternos.Rows[0].FindControl("rdbSecuencia") as RadioButton;
                rdb.Checked = true;
                indiceExternoSeleccionado = 0;
                pintarFilaSeleccionadaExterno(0);
            }
        }
    }

    public void pintarFilaSeleccionadaExterno(int fila)
    {

        grvExternos.Rows[fila].CssClass = "bg-color-azulClaro01 fg-color-blanco";
        grvExternos.Rows[fila].Cells[0].CssClass = "bg-color-azulClaro01";
        grvExternos.Rows[fila].Cells[1].CssClass = "bg-color-azulClaro01";
        grvExternos.Rows[fila].Cells[2].CssClass = "bg-color-azulClaro01";

    }

    public void pintarFilaSeleccionadaArchivoInterno(int fila)
    {
        grvInternos.Rows[fila].CssClass = "bg-color-amarillo";
        grvInternos.Rows[fila].Cells[0].CssClass = "bg-color-amarillo";
        grvInternos.Rows[fila].Cells[1].CssClass = "bg-color-amarillo";
        grvInternos.Rows[fila].Cells[2].CssClass = "bg-color-amarillo";
        grvInternos.Rows[fila].Cells[3].CssClass = "bg-color-amarillo";
    }

    //public void pintarFilaSeleccionadaPedido(int fila)
    //{
    //    grvPedidos.Rows[fila].CssClass = "bg-color-amarillo";
    //    grvPedidos.Rows[fila].Cells[0].CssClass = "bg-color-amarillo";
    //    grvPedidos.Rows[fila].Cells[1].CssClass = "bg-color-amarillo";
    //}

    public void despintarFilaSeleccionadaExterno(int fila)
    {

        grvExternos.Rows[fila].CssClass = "bg-color-blanco fg-color-negro";
        grvExternos.Rows[fila].Cells[0].CssClass = "bg-color-grisClaro03";
        grvExternos.Rows[fila].Cells[1].CssClass = "bg-color-grisClaro03";
        grvExternos.Rows[fila].Cells[2].CssClass = "bg-color-grisClaro03";

    }

    public void despintarFilaSeleccionadaArchivoInterno(int fila)
    {
        grvInternos.Rows[fila].CssClass = "bg-color-blanco";
        grvInternos.Rows[fila].Cells[0].CssClass = "bg-color-grisClaro03";
        grvInternos.Rows[fila].Cells[1].CssClass = "bg-color-grisClaro03";
        grvInternos.Rows[fila].Cells[2].CssClass = "bg-color-grisClaro03";
        grvInternos.Rows[fila].Cells[3].CssClass = "bg-color-grisClaro03";
    }

    public Conciliacion.RunTime.ReglasDeNegocio.Consultas.BusquedaPedido obtenerConfiguracionPedido()
    {
        return chkReferenciaIn.Checked
                   ? (rdbTodosMenoresIn.SelectedValue.Equals("TODOS")
                          ? Conciliacion.RunTime.ReglasDeNegocio.Consultas.BusquedaPedido.ConReferenciaTodos
                          : Conciliacion.RunTime.ReglasDeNegocio.Consultas.BusquedaPedido.ConReferenciaMenores)
                   : (rdbTodosMenoresIn.SelectedValue.Equals("TODOS")
                          ? Conciliacion.RunTime.ReglasDeNegocio.Consultas.BusquedaPedido.Todos
                          : Conciliacion.RunTime.ReglasDeNegocio.Consultas.BusquedaPedido.SinReferenciaMenores);
    }

    //Obtener la configuracion del la consulta de Internos
    public Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConciliacionInterna obtenerConfiguracionInterno()
    {
        return chkReferenciaIn.Checked
                   ? Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConciliacionInterna.ConReferencia
                   : Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConciliacionInterna.SinReferencia;
    }

    public void ActualizarTotalesAgregados()
    {
        try
        {
            if (grvExternos.Rows.Count > 0)
            {
                ReferenciaNoConciliada rE = leerReferenciaExternaSeleccionada();
                lblMontoAcumuladoInterno.Text = Decimal.Round(rE.MontoConciliado, 2).ToString("C2");
                lblAgregadosInternos.Text = rE.ListaReferenciaConciliada.Count.ToString();
                lblMontoResto.Text = Decimal.Round(rE.Resto, 2).ToString("C2");
            }
            else
            {
                lblMontoAcumuladoInterno.Text = Decimal.Round(0, 2).ToString("C2");
                lblAgregadosInternos.Text = "0";
                lblMontoResto.Text = Decimal.Round(0, 2).ToString("C2");
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void grvInternos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                ((CheckBox)e.Row.FindControl("chkTodosInternos")).Enabled =
                    !hdfInternosControl.Value.Equals("CANCELADOS");
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (hdfInternosControl.Value.Equals("CANCELADOS"))
                {
                    ((CheckBox)e.Row.FindControl("chkInterno")).Enabled =
                        ((RadioButton)e.Row.FindControl("rdbSecuenciaIn")).Enabled = false;
                }
                else
                {
                    bool r = !((Label)e.Row.FindControl("lblStatusConciliacion")).Text.Equals("CONCILIACION CANCELADA");
                    e.Row.FindControl("btnAgregarArchivo").Visible =
                        ((RadioButton)e.Row.FindControl("rdbSecuenciaIn")).Enabled = r;
                }

            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                if (grvInternos.Rows.Count > 0)
                {
                    RadioButton rdb;
                    if (hdfExternosControl.Value.Equals("CANCELADOS"))
                    {
                        rdb = grvInternos.Rows[indiceInternoSeleccionado].FindControl("rdbSecuenciaIn") as RadioButton;
                        rdb.Checked = true;
                    }
                    else
                    {
                        rdb = grvInternos.Rows[0].FindControl("rdbSecuenciaIn") as RadioButton;
                        rdb.Checked = true;
                        indiceInternoSeleccionado = 0;
                    }
                    pintarFilaSeleccionadaArchivoInterno(indiceInternoSeleccionado);
                }
            }
        }
        catch (Exception ex)
        {
            App.ImplementadorMensajes.MostrarMensaje("Verifique selección de referencia interna.\n" + ex.Message);
        }
    }

    protected void grvPedidos_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            grvPedidos.PageIndex = e.NewPageIndex;
            DataTable dtSortTable = HttpContext.Current.Session["TAB_INTERNOS_AX"] as DataTable;
            grvPedidos.DataSource = dtSortTable;
            grvPedidos.DataBind();
        }
        catch (Exception)
        {
        }
    }

    protected void grvAgregadosPedidos_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //try
        //{
        if (e.Row.RowType == DataControlRowType.Pager && (grvAgregadosPedidos.DataSource != null))
        {
            //TRAE EL TOTAL DE PAGINAS
            Label _TotalPags = (Label)e.Row.FindControl("lblTotalNumPaginas");
            _TotalPags.Text = grvAgregadosPedidos.PageCount.ToString();

            //LLENA LA LISTA CON EL NUMERO DE PAGINAS
            DropDownList list = (DropDownList)e.Row.FindControl("paginasDropDownList");
            for (int i = 1; i <= Convert.ToInt32(grvAgregadosPedidos.PageCount); i++)
            {
                list.Items.Add(i.ToString());
            }
            list.SelectedValue = (grvAgregadosPedidos.PageIndex + 1).ToString();

        }
        //}
        //catch (Exception ex)
        //{
        //   // App.ImplementadorMensajes.MostrarMensaje(ex.Message);
        //}
    }

    protected void grvAgregadosInternos_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow) return;
        e.Row.Attributes.Add("onmouseover", "this.className='bg-color-rojo01'");
        e.Row.Attributes.Add("onmouseout", "this.className='bg-color-blanco'");
    }

    protected void paginasDropDownListAgregadosInternos_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList oIraPag = (DropDownList)sender;
        int iNumPag;

        grvAgregadosInternos.PageIndex = int.TryParse(oIraPag.Text.Trim(), out iNumPag) && iNumPag > 0 &&
                                         iNumPag <= grvAgregadosInternos.PageCount
                                             ? iNumPag - 1
                                             : 0;

        LlenaGridViewArchivosInternos();
    }

    protected void btnGuardarUnoAVarios_Click(object sender, EventArgs e)
    {

        if (grvExternos.Rows.Count > 0)
        {

            ReferenciaNoConciliada rfExterno = leerReferenciaExternaSeleccionada();
            if (!rfExterno.Completo)
            {
                if (rfExterno.ListaReferenciaConciliada.Count > 0)
                {
                    if (rfExterno.GuardarReferenciaConciliada())
                    {
                        //Leer Variables URL 
                        cargarInfoConciliacionActual();

                        activarVerPendientesCanceladosExternos(true);
                        Consulta_Externos(corporativo, sucursal, año, mes, folio, Convert.ToDecimal(txtDiferencia.Text),
                                          tipoConciliacion, Convert.ToInt32(ddlStatusConcepto.SelectedValue),
                                          EsDepositoRetiro());
                        GenerarTablaExternos();
                        LlenaGridViewExternos();


                        //Limpiar Referncias de Externos
                        if (grvExternos.Rows.Count > 0)
                        {

                            rfExterno = leerReferenciaExternaSeleccionada();

                            LimpiarExternosReferencia(rfExterno);
                            GenerarTablaAgregadosArchivosInternos(rfExterno, tipoConciliacion);
                            ActualizarTotalesAgregados();

                            if (tipoConciliacion == 2)
                                ConsultarPedidosInternos();
                            else
                            {
                                activarVerPendientesCanceladosInternos(true);
                                ConsultarArchivosInternos();
                            }
                        }
                        else
                        {
                            LimpiarExternosTodos();
                            GenerarTablaAgregadosVacia(tipoConciliacion);
                            ActualizarTotalesAgregados();

                        }
                        //ACTUALIZAR BARRAS Y DE MAS HERRAMIENTAS
                        LlenarBarraEstado();
                        Consulta_TransaccionesConciliadas(corporativo, sucursal, año, mes, folio,
                                                          Convert.ToInt32(ddlCriteriosConciliacion.SelectedItem.Value));
                        GenerarTablaConciliados();
                        LlenaGridViewConciliadas();
                        App.ImplementadorMensajes.MostrarMensaje("TRANSACCION CONCILIADA EXITOSAMENTE");
                    }
                    //else
                    //    App.ImplementadorMensajes.MostrarMensaje("Error al guardar");
                }
                else
                    App.ImplementadorMensajes.MostrarMensaje("No se han agregado ninguna referencia interna aún");
            }
            else
                App.ImplementadorMensajes.MostrarMensaje("El archivo externo ya fue Conciliado");
        }
        else
            App.ImplementadorMensajes.MostrarMensaje("No existe ningun archivo externo a conciliar");
    }

    public void GenerarTablaArchivosInternos() //Genera la tabla Referencias a Conciliar de Archivos Internos
    {
        try
        {
            tblReferenciaInternas = new DataTable("ReferenciasInternas");
            tblReferenciaInternas.Columns.Add("Secuencia", typeof(int));
            tblReferenciaInternas.Columns.Add("Folio", typeof(int));
            tblReferenciaInternas.Columns.Add("Sucursal", typeof(int));
            tblReferenciaInternas.Columns.Add("Año", typeof(int));
            tblReferenciaInternas.Columns.Add("FMovimiento", typeof(DateTime));
            tblReferenciaInternas.Columns.Add("FOperacion", typeof(DateTime));
            tblReferenciaInternas.Columns.Add("Retiro", typeof(decimal));
            tblReferenciaInternas.Columns.Add("Deposito", typeof(decimal));
            tblReferenciaInternas.Columns.Add("Referencia", typeof(string));
            tblReferenciaInternas.Columns.Add("Descripcion", typeof(string));
            tblReferenciaInternas.Columns.Add("Monto", typeof(decimal));
            tblReferenciaInternas.Columns.Add("Concepto", typeof(string));
            tblReferenciaInternas.Columns.Add("RFCTercero", typeof(string));
            tblReferenciaInternas.Columns.Add("NombreTercero", typeof(string));
            tblReferenciaInternas.Columns.Add("Cheque", typeof(string));
            tblReferenciaInternas.Columns.Add("StatusConciliacion", typeof(string));
            tblReferenciaInternas.Columns.Add("UbicacionIcono", typeof(string));

            ReferenciaNoConciliada externoSelec = leerReferenciaExternaSeleccionada();
            foreach (
                ReferenciaNoConciliada rc in
                    listaReferenciaArchivosInternos.Where(
                        rc => !externoSelec.ExisteReferenciaConciliadaInterno(rc.Sucursal, rc.Año, rc.Folio, rc.Secuencia)))
            {
                tblReferenciaInternas.Rows.Add(
                    rc.Secuencia,
                    rc.Folio,
                    rc.Sucursal,
                    rc.Año,
                    rc.FMovimiento,
                    rc.FOperacion,
                    rc.Retiro,
                    rc.Deposito,
                    rc.Referencia,
                    rc.Descripcion,
                    rc.Monto,
                    rc.Concepto,
                    rc.RFCTercero,
                    rc.NombreTercero,
                    rc.Cheque,
                    rc.StatusConciliacion,
                    rc.UbicacionIcono
                    );
            }

            HttpContext.Current.Session["TAB_INTERNOS"] = tblReferenciaInternas;
            HttpContext.Current.Session["TAB_INTERNOS_AX"] = tblReferenciaInternas;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        
    }

    private void LlenaGridViewArchivosInternos() //Llena el gridview Referencias Internas
    {
        try
        {
            DataTable tablaReferenciasAI = (DataTable)HttpContext.Current.Session["TAB_INTERNOS"];
            grvInternos.DataSource = tablaReferenciasAI;
            grvInternos.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    public void Consulta_ArchivosInternos(int corporativoconciliacion, int sucursalconciliacion, int añoconciliacion,
                                          short mesconciliacion, int folioconciliacion, ReferenciaNoConciliada rfExterna,
                                          int sucursalinterno, short dias, decimal diferencia, int statusConcepto)
    {
        System.Data.SqlClient.SqlConnection Connection = SeguridadCB.Seguridad.Conexion;
        if (Connection.State == ConnectionState.Closed)
        {
            SeguridadCB.Seguridad.Conexion.Open();
            Connection = SeguridadCB.Seguridad.Conexion;
        }
        try
        {
            if (hdfInternosControl.Value.Equals("PENDIENTES"))
            {
                listaReferenciaArchivosInternos =
                    Conciliacion.RunTime.App.Consultas.ConsultaDetalleInternoPendiente(
                        obtenerConfiguracionInterno(),
                        corporativoconciliacion,
                        sucursalconciliacion,
                        añoconciliacion,
                        mesconciliacion,
                        folioconciliacion,
                        rfExterna.Folio,
                        rfExterna.Secuencia,
                        sucursalinterno,
                        dias, diferencia,
                        statusConcepto);
            }
            else
            {
                listaReferenciaArchivosInternos =
                    Conciliacion.RunTime.App.Consultas.ConsultaDetalleInternoCanceladoPendiente(
                        obtenerConfiguracionInterno(),
                        corporativoconciliacion,
                        sucursalconciliacion,
                        añoconciliacion,
                        mesconciliacion,
                        folioconciliacion,
                        rfExterna.Folio,
                        rfExterna.Secuencia,
                        diferencia
                        );
            }
            Session["POR_CONCILIAR_INTERNO"] = listaReferenciaArchivosInternos;
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //public void Consulta_Pedidos(int corporativoconciliacion, int sucursalconciliacion, int añoconciliacion,
    //                             short mesconciliacion, int folioconciliacion, ReferenciaNoConciliada rfExterna,
    //                             decimal diferencia, int celula, string cliente)
    //{
    //    System.Data.SqlClient.SqlConnection connection = SeguridadCB.Seguridad.Conexion;
    //    if (connection.State == ConnectionState.Closed)
    //    {
    //        SeguridadCB.Seguridad.Conexion.Open();
    //    }
    //    try
    //    {
    //        //listaReferenciaPedidos =
    //        //    Conciliacion.RunTime.App.Consultas.ConciliacionBusquedaPedido(obtenerConfiguracionPedido(),
    //        //                                                                  corporativoconciliacion,
    //        //                                                                  sucursalconciliacion, añoconciliacion,
    //        //                                                                  mesconciliacion, folioconciliacion,
    //        //                                                                  rfExterna.Folio, rfExterna.Secuencia,
    //        //                                                                  diferencia, celula, cliente);
    //        listaReferenciaPedidos =
    //           Conciliacion.RunTime.App.Consultas.ConciliacionBusquedaPedido(obtenerConfiguracionPedido(),
    //                                                                         corporativoconciliacion,
    //                                                                         sucursalconciliacion, añoconciliacion,
    //                                                                         mesconciliacion, folioconciliacion,
    //                                                                         rfExterna.Folio, rfExterna.Secuencia,
    //                                                                         diferencia, celula, cliente, clientepadre);
    //        Session["POR_CONCILIAR_INTERNO"] = listaReferenciaPedidos;
    //    }
    //    catch (Exception ex)
    //    {
    //        App.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);
    //    }
    //}
    public void Consulta_Pedidos(int corporativoconciliacion, int sucursalconciliacion, int añoconciliacion,
                                 short mesconciliacion, int folioconciliacion, ReferenciaNoConciliada rfExterna,
                                 decimal diferencia, int celula, string cliente, bool clientepadre)
    {
        System.Data.SqlClient.SqlConnection connection = SeguridadCB.Seguridad.Conexion;
        if (connection.State == ConnectionState.Closed)
        {
            SeguridadCB.Seguridad.Conexion.Open();
        }
        try
        {
            listaReferenciaPedidos =
                Conciliacion.RunTime.App.Consultas.ConciliacionBusquedaPedido(obtenerConfiguracionPedido(),
                                                                              corporativoconciliacion,
                                                                              sucursalconciliacion, añoconciliacion,
                                                                              mesconciliacion, folioconciliacion,
                                                                              rfExterna.Folio, rfExterna.Secuencia,
                                                                              diferencia, celula, cliente, clientepadre);
            Session["POR_CONCILIAR_INTERNO"] = listaReferenciaPedidos;
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void GenerarTablaPedidos() //Genera la tabla Referencias a Conciliar de Pedidos.
    {
        try
        {
            tblReferenciaInternas = new DataTable("ReferenciasInternas");
            tblReferenciaInternas.Columns.Add("Pedido", typeof(int));
            tblReferenciaInternas.Columns.Add("PedidoReferencia", typeof(int));
            tblReferenciaInternas.Columns.Add("AñoPed", typeof(int));
            tblReferenciaInternas.Columns.Add("Celula", typeof(int));
            tblReferenciaInternas.Columns.Add("Cliente", typeof(string));
            tblReferenciaInternas.Columns.Add("Nombre", typeof(string));
            tblReferenciaInternas.Columns.Add("FSuministro", typeof(DateTime));
            tblReferenciaInternas.Columns.Add("Total", typeof(decimal));
            tblReferenciaInternas.Columns.Add("Concepto", typeof(string));

            ReferenciaNoConciliada externoSelec = leerReferenciaExternaSeleccionada();
            foreach (
                ReferenciaNoConciliadaPedido rc in
                    listaReferenciaPedidos.Where(
                        rc => !externoSelec.ExisteReferenciaConciliadaPedido(rc.Pedido, rc.CelulaPedido, rc.AñoPedido)))
            {
                tblReferenciaInternas.Rows.Add(
                    rc.Pedido,
                    rc.PedidoReferencia,
                    rc.AñoPedido,
                    rc.CelulaPedido,
                    rc.Cliente,
                    rc.Nombre,
                    rc.FMovimiento,
                    rc.Total,
                    rc.Concepto
                    );
            }
            HttpContext.Current.Session["TAB_INTERNOS"] = tblReferenciaInternas;
            HttpContext.Current.Session["TAB_INTERNOS_AX"] = tblReferenciaInternas;
        }
        catch (Exception ex)
        {
            throw ex;
        }
       
    }

    private void LlenaGridViewPedidos() //Llena el gridview dePedidos
    {
        try
        {
            DataTable tablaReferenciasP = (DataTable)HttpContext.Current.Session["TAB_INTERNOS"];
            grvPedidos.PageIndex = 0;
            grvPedidos.DataSource = tablaReferenciasP;
            grvPedidos.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        
    }

    public void Consulta_ExternosPendientesCancelados(int corporativoconciliacion, int sucursalconciliacion,
                                                      int añoconciliacion, short mesconciliacion, int folioconciliacion,
                                                      int sucursalInterno, int folioInterno, int secuenciaInterno,
                                                      decimal diferencia, int statusConcepto)
    {
        System.Data.SqlClient.SqlConnection connection = SeguridadCB.Seguridad.Conexion;
        if (connection.State == ConnectionState.Closed)
        {
            SeguridadCB.Seguridad.Conexion.Open();
        }
        try
        {
            listaReferenciaExternas =
                tipoConciliacion == 2
                    ? Conciliacion.RunTime.App.Consultas.ConsultaDetalleExternoCanceladoPendiente
                          (chkReferenciaEx.Checked
                               ? Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConsultaExterno.DepositosConReferenciaPedido
                               : Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConsultaExterno.DepositosPedido,
                           corporativoconciliacion, sucursalconciliacion, añoconciliacion, mesconciliacion,
                           folioconciliacion, sucursalInterno, folioInterno, secuenciaInterno, statusConcepto,
                           diferencia)
                    : Conciliacion.RunTime.App.Consultas.ConsultaDetalleExternoCanceladoPendiente
                          (chkReferenciaEx.Checked
                               ? Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConsultaExterno.ConReferenciaInterno
                               : Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConsultaExterno.TodoInterno,
                           corporativoconciliacion, sucursalconciliacion, añoconciliacion, mesconciliacion,
                           folioconciliacion, sucursalInterno, folioInterno, secuenciaInterno, statusConcepto,
                           diferencia);

            Session["POR_CONCILIAR_EXTERNO"] = listaReferenciaExternas;
        }
        catch (Exception ex)
        {
            App.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);
        }
    }

    public void GenerarTablaAgregadosArchivosInternos(ReferenciaNoConciliada refExternaSelec, int tpConciliacion)
    //Genera la tabla Referencias (Archivos/Pedidos) agregados
    {
        try
        {
            tblReferenciaAgregadasInternas = new DataTable("ReferenciasInternas");
            if (tpConciliacion == 2)
            {
                tblReferenciaAgregadasInternas.Columns.Add("Pedido", typeof (int));
                tblReferenciaAgregadasInternas.Columns.Add("AñoPed", typeof (int));
                tblReferenciaAgregadasInternas.Columns.Add("Celula", typeof (int));
                tblReferenciaAgregadasInternas.Columns.Add("Cliente", typeof (string));
                tblReferenciaAgregadasInternas.Columns.Add("Nombre", typeof (string));
                tblReferenciaAgregadasInternas.Columns.Add("FMovimiento", typeof (DateTime));
                tblReferenciaAgregadasInternas.Columns.Add("FOperacion", typeof (DateTime));
                tblReferenciaAgregadasInternas.Columns.Add("Monto", typeof (decimal));
                tblReferenciaAgregadasInternas.Columns.Add("Concepto", typeof (string));
                //Llena GridView con lista de Agregados del Externo (PEDIDOS)
                foreach (ReferenciaConciliadaPedido rc in refExternaSelec.ListaReferenciaConciliada)
                {
                    tblReferenciaAgregadasInternas.Rows.Add(
                        rc.Pedido,
                        rc.AñoPedido,
                        rc.CelulaPedido,
                        rc.Cliente,
                        rc.Nombre,
                        rc.FMovimiento,
                        rc.FOperacion,
                        rc.Total,
                        rc.ConceptoPedido
                        );
                }
                grvAgregadosPedidos.DataSource = tblReferenciaAgregadasInternas;
                grvAgregadosPedidos.DataBind();
            }
            else
            {
                tblReferenciaAgregadasInternas.Columns.Add("Secuencia", typeof (int));
                tblReferenciaAgregadasInternas.Columns.Add("Folio", typeof (int));
                tblReferenciaAgregadasInternas.Columns.Add("Año", typeof (int));
                tblReferenciaAgregadasInternas.Columns.Add("Sucursal", typeof (int));
                tblReferenciaAgregadasInternas.Columns.Add("FMovimiento", typeof (DateTime));
                tblReferenciaAgregadasInternas.Columns.Add("FOperacion", typeof (DateTime));
                tblReferenciaAgregadasInternas.Columns.Add("Monto", typeof (decimal));
                tblReferenciaAgregadasInternas.Columns.Add("Concepto", typeof (string));

                //Llena GridView con lista de Agregados del Externo (INTERNOS)
                foreach (ReferenciaConciliada rc in refExternaSelec.ListaReferenciaConciliada)
                {
                    tblReferenciaAgregadasInternas.Rows.Add(
                        rc.SecuenciaInterno,
                        rc.FolioInterno,
                        rc.AñoConciliacion,
                        rc.SucursalInterno,
                        rc.FMovimientoInt,
                        rc.FOperacionInt,
                        rc.MontoInterno,
                        rc.ConceptoInterno
                        );

                }
                grvAgregadosInternos.DataSource = tblReferenciaAgregadasInternas;
                grvAgregadosInternos.DataBind();

            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void GenerarTablaAgregadosVacia(int tConciliacion)
    {
        try
        {
            tblReferenciaAgregadasInternas = new DataTable("ReferenciasInternas");
            if (tConciliacion == 2)
            {
                grvAgregadosPedidos.DataSource = tblReferenciaAgregadasInternas;
                grvAgregadosPedidos.DataBind();
            }
            else
            {
                grvAgregadosInternos.DataSource = tblReferenciaAgregadasInternas;
                grvAgregadosInternos.DataBind();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //Limpian ListasRefencias
    public void LimpiarExternosReferencia(ReferenciaNoConciliada rExterna)
    {
        try
        {
            listaReferenciaExternas = Session["POR_CONCILIAR_EXTERNO"] as List<ReferenciaNoConciliada>;
            if (listaReferenciaExternas != null)
                listaReferenciaExternas.Where(
                    x =>
                        x.Secuencia != rExterna.Secuencia && x.Folio == rExterna.Folio && x.Sucursal == rExterna.Sucursal &&
                        x.Año == rExterna.Año)
                    .Where(x => !x.Completo)
                    .ToList()
                    .ForEach(x => x.BorrarReferenciaConciliada());
            Session["POR_CONCILIAR_EXTERNO"] = listaReferenciaExternas;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void LimpiarExternosTodos()
    {
        try
        {
            listaReferenciaExternas = Session["POR_CONCILIAR_EXTERNO"] as List<ReferenciaNoConciliada>;
            if (listaReferenciaExternas != null) listaReferenciaExternas.ForEach(x => x.BorrarReferenciaConciliada());
            Session["POR_CONCILIAR_EXTERNO"] = listaReferenciaExternas;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //public void GenerarTablaExternos() //Genera la tabla Referencias Externas
    //{
    //    tblReferenciaExternas = new DataTable("ReferenciasExternas");
    //    tblReferenciaExternas.Columns.Add("Secuencia", typeof(int));
    //    tblReferenciaExternas.Columns.Add("Folio", typeof(int));
    //    tblReferenciaExternas.Columns.Add("Año", typeof(int));
    //    tblReferenciaExternas.Columns.Add("ConInterno", typeof(bool));
    //    tblReferenciaExternas.Columns.Add("FMovimiento", typeof(DateTime));
    //    tblReferenciaExternas.Columns.Add("FOperacion", typeof(DateTime));
    //    tblReferenciaExternas.Columns.Add("Referencia", typeof(string));
    //    tblReferenciaExternas.Columns.Add("RFCTercero", typeof(string));
    //    tblReferenciaExternas.Columns.Add("NombreTercero", typeof(string));
    //    tblReferenciaExternas.Columns.Add("Retiro", typeof(decimal));
    //    tblReferenciaExternas.Columns.Add("Deposito", typeof(decimal));
    //    tblReferenciaExternas.Columns.Add("Concepto", typeof(string));
    //    tblReferenciaExternas.Columns.Add("Cheque", typeof(string));
    //    tblReferenciaExternas.Columns.Add("Descripcion", typeof(string));
    //    tblReferenciaExternas.Columns.Add("StatusConciliacion", typeof(string));
    //    tblReferenciaExternas.Columns.Add("UbicacionIcono", typeof(string));
    //    foreach (ReferenciaNoConciliada rp in listaReferenciaExternas)
    //        tblReferenciaExternas.Rows.Add(
    //            rp.Secuencia,
    //            rp.Folio,
    //            rp.Año,
    //            rp.ConInterno,
    //            rp.FMovimiento,
    //            rp.FOperacion,
    //            rp.Referencia,
    //            rp.RFCTercero,
    //            rp.NombreTercero,
    //            rp.Retiro,
    //            rp.Deposito,
    //            rp.Concepto,
    //            rp.Cheque,
    //            rp.Descripcion,
    //            rp.StatusConciliacion,
    //            rp.UbicacionIcono);

    //    HttpContext.Current.Session["TAB_EXTERNOS"] = tblReferenciaExternas;
    //    HttpContext.Current.Session["TAB_EXTERNOS_AX"] = tblReferenciaExternas;
    //}

    public void GenerarTablaExternos() //Genera la tabla Referencias Externas
    {
        try
        {
            tblReferenciaExternas = new DataTable("ReferenciasExternas");
            tblReferenciaExternas.Columns.Add("Corporativo", typeof(int));
            tblReferenciaExternas.Columns.Add("Sucursal", typeof(int));
            tblReferenciaExternas.Columns.Add("Secuencia", typeof(int));
            tblReferenciaExternas.Columns.Add("Folio", typeof(int));
            tblReferenciaExternas.Columns.Add("Año", typeof(int));
            tblReferenciaExternas.Columns.Add("ConInterno", typeof(bool));
            tblReferenciaExternas.Columns.Add("FMovimiento", typeof(DateTime));
            tblReferenciaExternas.Columns.Add("FOperacion", typeof(DateTime));
            tblReferenciaExternas.Columns.Add("Referencia", typeof(string));
            tblReferenciaExternas.Columns.Add("RFCTercero", typeof(string));
            tblReferenciaExternas.Columns.Add("NombreTercero", typeof(string));
            tblReferenciaExternas.Columns.Add("Retiro", typeof(decimal));
            tblReferenciaExternas.Columns.Add("Deposito", typeof(decimal));
            tblReferenciaExternas.Columns.Add("Concepto", typeof(string));
            tblReferenciaExternas.Columns.Add("Cheque", typeof(string));
            tblReferenciaExternas.Columns.Add("Descripcion", typeof(string));
            tblReferenciaExternas.Columns.Add("StatusConciliacion", typeof(string));
            tblReferenciaExternas.Columns.Add("UbicacionIcono", typeof(string));
            foreach (ReferenciaNoConciliada rp in listaReferenciaExternas)
                tblReferenciaExternas.Rows.Add(
                    rp.Corporativo,
                    rp.Sucursal,
                    rp.Secuencia,
                    rp.Folio,
                    rp.Año,
                    rp.ConInterno,
                    rp.FMovimiento,
                    rp.FOperacion,
                    rp.Referencia,
                    rp.RFCTercero,
                    rp.NombreTercero,
                    rp.Retiro,
                    rp.Deposito,
                    rp.Concepto,
                    rp.Cheque,
                    rp.Descripcion,
                    rp.StatusConciliacion,
                    rp.UbicacionIcono);

            HttpContext.Current.Session["TAB_EXTERNOS"] = tblReferenciaExternas;
            HttpContext.Current.Session["TAB_EXTERNOS_AX"] = tblReferenciaExternas;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        
    }

    private void LlenaGridViewExternos() //Llena el gridview con las Referencias Externas
    {
        try
        {
            DataTable tablaReferenaciasE = (DataTable)HttpContext.Current.Session["TAB_EXTERNOS"];
            grvExternos.DataSource = tablaReferenaciasE;
            grvExternos.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }
       
    }

    public bool EsDepositoRetiro()
    {
        return rdbVerDepositoRetiro.SelectedValue.Equals("DEPOSITOS");
    }

    public void Consulta_Externos(int corporativo, int sucursal, int año, short mes, int folio, decimal diferencia,
                                  int tipoConciliacion, int statusConcepto, bool esDeposito)
    {
        System.Data.SqlClient.SqlConnection connection = SeguridadCB.Seguridad.Conexion;
        if (connection.State == ConnectionState.Closed)
        {
            SeguridadCB.Seguridad.Conexion.Open();
            /*
                        connection = SeguridadCB.Seguridad.Conexion;
            */
        }

        try
        {
            listaReferenciaExternas = tipoConciliacion == 2
                                          ? Conciliacion.RunTime.App.Consultas.ConsultaDetalleExternoPendienteDeposito
                                                (chkReferenciaEx.Checked
                                                     ? Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConsultaExterno.DepositosConReferenciaPedido
                                                     : Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConsultaExterno.DepositosPedido,
                                                 corporativo, sucursal, año, mes, folio, diferencia, statusConcepto,
                                                 esDeposito)
                                          : Conciliacion.RunTime.App.Consultas.ConsultaDetalleExternoPendienteDeposito
                                                (chkReferenciaEx.Checked
                                                     ? Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConsultaExterno.ConReferenciaInterno
                                                     : Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConsultaExterno.TodoInterno,
                                                 corporativo, sucursal, año, mes, folio, diferencia, statusConcepto,
                                                 esDeposito);

            Session["POR_CONCILIAR_EXTERNO"] = listaReferenciaExternas;

        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //Seleccion del RadioButton de Referencias Externas
    protected void rdbSecuencia_CheckedChanged(object sender, EventArgs e)
    {
        quitarSeleccionRadio("EXTERNO");
        RadioButton rdb = sender as RadioButton;
        rdb.Checked = true;
        GridViewRow grv = (GridViewRow)rdb.Parent.Parent;
        pintarFilaSeleccionadaExterno(grv.RowIndex);

        indiceExternoSeleccionado = grv.RowIndex;
        ReferenciaNoConciliada rfEx = leerReferenciaExternaSeleccionada();
        //Limpiar Listas de Referencia de demas Externos
        LimpiarExternosReferencia(rfEx);
        statusFiltro = false;
        Session["StatusFiltro"] = statusFiltro;
        tipoFiltro = String.Empty;
        Session["TipoFiltro"] = tipoFiltro;
        //Leer el tipoConciliacion URL
        tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

        if (tipoConciliacion == 2)
            ConsultarPedidosInternos();
        else
            ConsultarArchivosInternos();
        //Generar el GridView para las Referencias Internas(ARCHIVOS / PEDIDOS)
        GenerarTablaAgregadosArchivosInternos(rfEx, tipoConciliacion);
        ActualizarTotalesAgregados();

    }

    ////Seleccion del RadioButton de Referencias Pedidos
    //protected void rdbPedido_CheckedChanged(object sender, EventArgs e)
    //{
    //    quitarSeleccionRadio("PEDIDO");
    //    RadioButton rdb = sender as RadioButton;
    //    rdb.Checked = true;
    //    GridViewRow grv = (GridViewRow)rdb.Parent.Parent;
    //    pintarFilaSeleccionadaPedido(grv.RowIndex);
    //    indiceInternoSeleccionado = grv.RowIndex;
    //}
    //Seleccion del RadioButton de Referencias ArchivosInternos
    protected void rdbSecuenciaIn_CheckedChanged(object sender, EventArgs e)
    {
        quitarSeleccionRadio("ARCHIVOINTERNO");
        RadioButton rdb = sender as RadioButton;
        rdb.Checked = true;
        GridViewRow grv = (GridViewRow)rdb.Parent.Parent;
        pintarFilaSeleccionadaArchivoInterno(grv.RowIndex);
        indiceInternoSeleccionado = grv.RowIndex;
    }

    public void FiltrarInternos(string tipoFiltro)
    {
        try
        {
            //Leer el tipoConciliacion URL
            tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

            cargar_ComboCampoFiltroDestino(tipoConciliacion, ddlFiltrarEn.SelectedItem.Value);
            switch (tipoFiltro)
            {
                case "CA":
                    FiltrarCampo(valorFiltro(tipoCampoSeleccionado()), ddlFiltrarEn.SelectedItem.Value);
                    break;
                case "FO":
                    FiltrarRangoFechasFO();
                    break;
                case "FM":
                    FiltrarRangoFechasFM();
                    break;
                case "FS":
                    FiltrarRangoFechasFS();
                    break;
                default:
                    return;
                    break;
            }
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    //public void ConsultarPedidosInternos()
    //{
    //    if (grvExternos.Rows.Count > 0)
    //    {
    //        //Obtener el la referencia externa seleccionada
    //        ReferenciaNoConciliada rfEx = hdfExternosControl.Value.Equals("PENDIENTES")
    //                                          ? leerReferenciaExternaSeleccionada()
    //                                          : tranExternaAnteriorSeleccionada;
    //        //Leer Variables URL 
    //        cargarInfoConciliacionActual();

    //        Consulta_Pedidos(corporativo, sucursal, año, mes, folio, rfEx, Convert.ToDecimal(txtDiferencia.Text),
    //                         Convert.ToInt32(ddlCelula.SelectedItem.Value), rfEx.Referencia.Trim());
    //        GenerarTablaPedidos();
    //        LlenaGridViewPedidos();
    //        statusFiltro = Convert.ToBoolean(Session["StatusFiltro"]);
    //        if (statusFiltro)
    //        {
    //            //Leer el tipoConciliacion URL
    //            tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

    //            cargar_ComboCampoFiltroDestino(tipoConciliacion, ddlFiltrarEn.SelectedItem.Value);
    //            tipoFiltro = Session["TipoFiltro"] as string;
    //            FiltrarInternos(tipoFiltro);
    //        }
    //    }
    //    else
    //    {
    //        grvPedidos.DataSource = null;
    //        grvPedidos.DataBind();
    //    }
    //}
    public void ConsultaInicialPedidosInternos()
    {
        try
        {

            if (grvExternos.Rows.Count > 0)
            {
                //Obtener el la referencia externa seleccionada
                ReferenciaNoConciliada rfEx = hdfExternosControl.Value.Equals("PENDIENTES")
                    ? leerReferenciaExternaSeleccionada()
                    : tranExternaAnteriorSeleccionada;
                //Leer Variables URL 
                cargarInfoConciliacionActual();
                bool clientevalido = App.Consultas.ClienteValido(rfEx.Referencia.Trim());
                string cliente = "-1";
                if (clientevalido)
                {
                    try
                    {
                        cliente = rfEx.Referencia.Trim().Length > 2
                            ? rfEx.Referencia.Trim().Substring(0, rfEx.Referencia.Trim().Length - 1)
                            : rfEx.Referencia.Trim();
                    }
                    catch (FormatException e)
                    {
                        App.ImplementadorMensajes.MostrarMensaje(
                            "Cliente no es valido, tendra que agregar el pedido directamenete.");
                    }
                    catch (Exception e)
                    {
                        App.ImplementadorMensajes.MostrarMensaje(
                            "Cliente no es valido, tendra que agregar el pedido directamenete.");
                    }

                }
                else
                    App.ImplementadorMensajes.MostrarMensaje(
                        "Cliente no es valido, tendra que agregar el pedido directamenete.");

                Consulta_Pedidos(corporativo, sucursal, año, mes, folio, rfEx, Convert.ToDecimal(txtDiferencia.Text),
                    Convert.ToInt32(ddlCelula.SelectedItem.Value),
                    cliente, false);
                // Se agrega -1 que funje como cliente NON //ClientePadre=false para solo mandar los pedidos de ese cliente
                GenerarTablaPedidos();
                LlenaGridViewPedidos();
                statusFiltro = Convert.ToBoolean(Session["StatusFiltro"]);
                if (statusFiltro)
                {
                    //Leer el tipoConciliacion URL
                    tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

                    cargar_ComboCampoFiltroDestino(tipoConciliacion, ddlFiltrarEn.SelectedItem.Value);
                    tipoFiltro = Session["TipoFiltro"] as string;
                    FiltrarInternos(tipoFiltro);
                }
            }
            else
            {
                grvPedidos.DataSource = null;
                grvPedidos.DataBind();
            }
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public void ConsultarPedidosInternos()
    {
        try
        {
            if (grvExternos.Rows.Count > 0)
            {
                //Obtener el la referencia externa seleccionada
                ReferenciaNoConciliada rfEx = hdfExternosControl.Value.Equals("PENDIENTES")
                                                  ? leerReferenciaExternaSeleccionada()
                                                  : tranExternaAnteriorSeleccionada;
                //Leer Variables URL 
                cargarInfoConciliacionActual();
                bool clientevalido = App.Consultas.ClienteValido(rfEx.Referencia.Trim());
                string cliente = "-1";
                if (clientevalido)
                {
                    try
                    {
                        cliente = rfEx.Referencia.Trim().Length > 2
                            ? rfEx.Referencia.Trim().Substring(0, rfEx.Referencia.Trim().Length - 1)
                            : rfEx.Referencia.Trim();
                    }
                    catch (FormatException e)
                    {
                        /**Modifico: CNSM 
                         Fecha: 08/06/2017
                        App.ImplementadorMensajes.MostrarMensaje("Cliente no es valido, tendra que agregar el pedido directamenete.");
                         **/
                    }
                    catch (Exception e)
                    {
                        /**Modifico: CNSM 
                        Fecha: 08/06/2017
                        App.ImplementadorMensajes.MostrarMensaje("Cliente no es valido, tendra que agregar el pedido directamenete.");
                         **/
                    }

                }
                /**Modifico: CNSM 
                   Fecha: 08/06/2017
               else
               App.ImplementadorMensajes.MostrarMensaje("Cliente no es valido, tendra que agregar el pedido directamenete.");**/

                Consulta_Pedidos(corporativo, sucursal, año, mes, folio, rfEx, Convert.ToDecimal(txtDiferencia.Text),
                     Convert.ToInt32(ddlCelula.SelectedItem.Value),
                     cliente, false); // Se agrega -1 que funje como cliente NON //ClientePadre=false para solo mandar los pedidos de ese cliente
                GenerarTablaPedidos();
                LlenaGridViewPedidos();
                statusFiltro = Convert.ToBoolean(Session["StatusFiltro"]);
                if (statusFiltro)
                {
                    //Leer el tipoConciliacion URL
                    tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

                    cargar_ComboCampoFiltroDestino(tipoConciliacion, ddlFiltrarEn.SelectedItem.Value);
                    tipoFiltro = Session["TipoFiltro"] as string;
                    FiltrarInternos(tipoFiltro);
                }
            }
            else
            {
                grvPedidos.DataSource = null;
                grvPedidos.DataBind();
            }
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        
    }


    public void ConsultarArchivosInternos()
    {
        try
        {
            
        if (grvExternos.Rows.Count > 0)
        {
            //Obtener el la referencia externa seleccionada
            //aqui Colocar si se toma la referncia Externa Guardada cuando se veen los Cancealdos o la nommal
            ReferenciaNoConciliada rfEx = hdfExternosControl.Value.Equals("PENDIENTES")
                                              ? leerReferenciaExternaSeleccionada()
                                              : tranExternaAnteriorSeleccionada;
            //Leer Variables URL 
            cargarInfoConciliacionActual();

            Consulta_ArchivosInternos(corporativo, sucursal, año, mes,
                                      folio, rfEx, Convert.ToInt16(ddlSucursal.SelectedItem.Value),
                                      Convert.ToSByte(txtDias.Text), Convert.ToDecimal(txtDiferencia.Text),
                                      Convert.ToInt32(ddlStatusConcepto.SelectedItem.Value));
            GenerarTablaArchivosInternos();
            LlenaGridViewArchivosInternos();
            statusFiltro = Convert.ToBoolean(Session["StatusFiltro"]);
            if (statusFiltro)
            {
                //Leer el tipoConciliacion URL
                tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

                cargar_ComboCampoFiltroDestino(tipoConciliacion, ddlFiltrarEn.SelectedItem.Value);
                tipoFiltro = Session["TipoFiltro"] as string;
                FiltrarInternos(tipoFiltro);
            }
        }
        else
        {
            grvInternos.DataSource = null;
            grvInternos.DataBind();
        }
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    public void quitarSeleccionRadio(string nombreGrid)
    {
        switch (nombreGrid)
        {
            case "EXTERNO":
                foreach (
                    RadioButton rb in
                        from GridViewRow gv in grvExternos.Rows
                        select (RadioButton)grvExternos.Rows[gv.RowIndex].FindControl("rdbSecuencia"))
                {
                    rb.Checked = false;
                    despintarFilaSeleccionadaExterno(((GridViewRow)rb.Parent.Parent).RowIndex);
                }
                break;
            case "ARCHIVOINTERNO":
                foreach (
                    RadioButton rb in
                        from GridViewRow gv in grvInternos.Rows
                        select (RadioButton)grvInternos.Rows[gv.RowIndex].FindControl("rdbSecuenciaIn"))
                {
                    rb.Checked = false;
                    despintarFilaSeleccionadaArchivoInterno((rb.Parent.Parent as GridViewRow).RowIndex);
                }
                break;
        }

    }

    protected void chkReferenciaEx_CheckedChanged(object sender, EventArgs e)
    {
        //Leer Variables URL 
        cargarInfoConciliacionActual();

        if (hdfExternosControl.Value.Equals("CANCELADOS"))
            verExternosCanceladosPendientes();
        else
        {
            Consulta_Externos(corporativo, sucursal, año, mes, folio, Convert.ToDecimal(txtDiferencia.Text),
                              tipoConciliacion, Convert.ToInt32(ddlStatusConcepto.SelectedValue), EsDepositoRetiro());
            GenerarTablaExternos();
            LlenaGridViewExternos();

            //Limpiar Referencias de Externos
            if (grvExternos.Rows.Count > 0)
            {
                //Referencia Externa
                ReferenciaNoConciliada rfExterno = leerReferenciaExternaSeleccionada();
                LimpiarExternosReferencia(rfExterno);
                GenerarTablaAgregadosArchivosInternos(rfExterno, tipoConciliacion);
            }
            else
            {
                LimpiarExternosTodos();
                GenerarTablaAgregadosVacia(tipoConciliacion);
            }
            ActualizarTotalesAgregados();
            //CONSULTAR INTERNO(ARCHIVOS O PEDIDOS)
            if (tipoConciliacion == 2)
                ConsultarPedidosInternos();
            else
                ConsultarArchivosInternos();
        }
        statusFiltro = false;
        Session["StatusFiltro"] = statusFiltro;
        tipoFiltro = String.Empty;
        Session["TipoFiltro"] = tipoFiltro;
    }

    protected void grvConciliadas_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        int corporativoConciliacion =
            Convert.ToInt32(grvConciliadas.DataKeys[e.NewSelectedIndex].Values["CorporativoConciliacion"]);
        int sucursalConciliacion =
            Convert.ToInt32(grvConciliadas.DataKeys[e.NewSelectedIndex].Values["SucursalConciliacion"]);
        int añoConciliacion = Convert.ToInt32(grvConciliadas.DataKeys[e.NewSelectedIndex].Values["AñoConciliacion"]);
        int mesConciliacion = Convert.ToInt32(grvConciliadas.DataKeys[e.NewSelectedIndex].Values["MesConciliacion"]);
        int folioConciliacion = Convert.ToInt32(grvConciliadas.DataKeys[e.NewSelectedIndex].Values["FolioConciliacion"]);
        int folioExterno = Convert.ToInt32(grvConciliadas.DataKeys[e.NewSelectedIndex].Values["FolioExterno"]);
        int secuenciaExterno = Convert.ToInt32(grvConciliadas.DataKeys[e.NewSelectedIndex].Values["SecuenciaExterno"]);

        //Leer las TransaccionesConciliadas
        listaTransaccionesConciliadas = Session["CONCILIADAS"] as List<ReferenciaNoConciliada>;

        ReferenciaNoConciliada tConciliada = listaTransaccionesConciliadas.Single(
            x => x.Corporativo == corporativoConciliacion &&
                 x.Sucursal == sucursalConciliacion &&
                 x.Año == añoConciliacion &&
                 x.MesConciliacion == mesConciliacion &&
                 x.FolioConciliacion == folioConciliacion &&
                 x.Folio == folioExterno &&
                 x.Secuencia == secuenciaExterno);

        GeneraTablaDetalleArchivosInternos(tConciliada);
        ConsultaDetalleTransaccionConciliada(tConciliada);
        LlenarGridDetalleInterno(tConciliada);
        mpeLanzarDetalle.Show();
    }

    protected void grvConciliadas_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (!e.CommandName.Equals("DESCONCILIAR")) return;
        Button imgDesconciliar = e.CommandSource as Button;
        GridViewRow gRowConciliado = (GridViewRow)(imgDesconciliar).Parent.Parent;
        //Leer Variables URL 
        cargarInfoConciliacionActual();

        int corporativoConcilacion =
            Convert.ToInt32(grvConciliadas.DataKeys[gRowConciliado.RowIndex].Values["CorporativoConciliacion"]);
        int sucursalConciliacion =
            Convert.ToInt32(grvConciliadas.DataKeys[gRowConciliado.RowIndex].Values["SucursalConciliacion"]);
        int añoConciliacion = Convert.ToInt32(grvConciliadas.DataKeys[gRowConciliado.RowIndex].Values["AñoConciliacion"]);
        int mesConciliacion = Convert.ToInt32(grvConciliadas.DataKeys[gRowConciliado.RowIndex].Values["MesConciliacion"]);
        int folioConciliacion =
            Convert.ToInt32(grvConciliadas.DataKeys[gRowConciliado.RowIndex].Values["FolioConciliacion"]);
        int folioExterno = Convert.ToInt32(grvConciliadas.DataKeys[gRowConciliado.RowIndex].Values["FolioExterno"]);
        int secuenciaExterno =
            Convert.ToInt32(grvConciliadas.DataKeys[gRowConciliado.RowIndex].Values["SecuenciaExterno"]);

        //Leer las TransaccionesConciliadas
        listaTransaccionesConciliadas = Session["CONCILIADAS"] as List<ReferenciaNoConciliada>;

        tranDesconciliar = listaTransaccionesConciliadas.Single(
            x => x.Corporativo == corporativoConcilacion &&
                 x.Sucursal == sucursalConciliacion &&
                 x.Año == añoConciliacion &&
                 x.MesConciliacion == mesConciliacion &&
                 x.FolioConciliacion == folioConciliacion &&
                 x.Folio == folioExterno &&
                 x.Secuencia == secuenciaExterno);

        tranDesconciliar.DesConciliar();
        Consulta_TransaccionesConciliadas(corporativo, sucursal, año, mes, folio,
                                          Convert.ToInt32(ddlCriteriosConciliacion.SelectedValue));
        GenerarTablaConciliados();
        LlenaGridViewConciliadas();
        LlenarBarraEstado();
        //Cargo y refresco nuevamente los archvos externos
        Consulta_Externos(corporativo, sucursal, año, mes, folio, Convert.ToDecimal(txtDiferencia.Text),
                          tipoConciliacion, Convert.ToInt32(ddlStatusConcepto.SelectedValue), EsDepositoRetiro());
        GenerarTablaExternos();
        LlenaGridViewExternos();
        if (tipoConciliacion == 2)
            ConsultarPedidosInternos();
        else
            ConsultarArchivosInternos();
    }

    protected void imgFiltrar_Click(object sender, ImageClickEventArgs e)
    {
        //Leer el tipoConciliacion URL
        tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

        cargar_ComboCampoFiltroDestino(tipoConciliacion, ddlFiltrarEn.SelectedItem.Value);
        enlazarComboCampoFiltrarDestino();
        InicializarControlesFiltro();
        mpeFiltrar.Show();
    }

    protected void chkReferenciaIn_CheckedChanged(object sender, EventArgs e)
    {
        //Leer Variables URL 
        cargarInfoConciliacionActual();

        //CONSULTAR INTERNO(ARCHIVOS O PEDIDOS) TANTO PENDIENTES COMO CANCELADOS
        if (tipoConciliacion == 2)
            ConsultarPedidosInternos();
        else
            ConsultarArchivosInternos();
    }

    protected void rdbTodosMenoresIn_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Leer Variables URL 
        cargarInfoConciliacionActual();

        //CONSULTAR INTERNO(ARCHIVOS O PEDIDOS) TANTO PENDIENTES COMO CANCELADOS
        if (tipoConciliacion == 2)
            ConsultarPedidosInternos();
        else
            ConsultarArchivosInternos();
    }

    protected void btnQuitarPedidoInterno_Click(object sender, EventArgs e)
    {
        Button btnQuitarPedido = sender as Button;
        GridViewRow gRowIn = (GridViewRow)(btnQuitarPedido).Parent.Parent;
        //Leer Variables URL 
        cargarInfoConciliacionActual();

        //Leer Referencia Externa

        ReferenciaNoConciliada rcExterna = leerReferenciaExternaSeleccionada();

        //Leer Referncia (Pedido) que se va quitar
        int pedido = Convert.ToInt32(grvAgregadosPedidos.DataKeys[gRowIn.RowIndex].Values["Pedido"]);
        short celulaPedido = Convert.ToSByte(grvAgregadosPedidos.DataKeys[gRowIn.RowIndex].Values["Celula"]);
        int añoPedido = Convert.ToInt32(grvAgregadosPedidos.DataKeys[gRowIn.RowIndex].Values["AñoPed"]);

        rcExterna.QuitarReferenciaConciliada(pedido, celulaPedido, añoPedido);
        //Generar el GridView para las Referencias Internas(ARCHIVOS / PEDIDOS)
        GenerarTablaAgregadosArchivosInternos(rcExterna, tipoConciliacion);
        ActualizarTotalesAgregados();
        ConsultarPedidosInternos();
    }

    protected void btnAgregarArchivo_Click(object sender, EventArgs e)
    {
        //try
        //{
        if (grvExternos.Rows.Count > 0)
        {
            Button btnAgregarArchivo = sender as Button;
            GridViewRow gRowIn = (GridViewRow)(btnAgregarArchivo).Parent.Parent;
            //Leer Referencia Externa
            ReferenciaNoConciliada rcp = leerReferenciaExternaSeleccionada();
            //Leer Referencia (Archivo) que se va agregar
            int folioIn = Convert.ToInt32(grvInternos.DataKeys[gRowIn.RowIndex].Values["Folio"]);
            int secuenciaIn = Convert.ToInt32(grvInternos.DataKeys[gRowIn.RowIndex].Values["Secuencia"]);

            //Leer el tipoConciliacion URL
            tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);
            //Leer Referencias Internas
            listaReferenciaArchivosInternos = Session["POR_CONCILIAR_INTERNO"] as List<ReferenciaNoConciliada>;

            ReferenciaNoConciliada rnc =
                listaReferenciaArchivosInternos.Single(s => s.Secuencia == secuenciaIn && s.Folio == folioIn);

            if (!hdfExternosControl.Value.Equals("PENDIENTES"))
            {
                rcp.AgregarReferenciaConciliada(rnc);
                //Generar el GridView para las Referencias Internas(ARCHIVOS / PEDIDOS)


                GenerarTablaAgregadosArchivosInternos(rcp, tipoConciliacion);
                ActualizarTotalesAgregados();
                ConsultarArchivosInternos();


            }
            else
            {
                if (!rcp.StatusConciliacion.Equals("CONCILIACION CANCELADA"))
                {
                    rcp.AgregarReferenciaConciliada(rnc);
                    //Generar el GridView para las Referencias Internas(ARCHIVOS / PEDIDOS)
                    GenerarTablaAgregadosArchivosInternos(rcp, tipoConciliacion);
                    ActualizarTotalesAgregados();
                    ConsultarArchivosInternos();

                }
                else
                    App.ImplementadorMensajes.MostrarMensaje(
                        "NO SE PUEDE COMPLETAR LA ACCION \nLA REFERENCIA EXTERNA ESTA CANCELADA");
            }
        }
        else
            App.ImplementadorMensajes.MostrarMensaje("NO EXISTEN NINGUNA TRANSACCION EXTERNA");

        //catch (Exception ex)
        //{
        //    App.ImplementadorMensajes.MostrarMensaje(ex.Message);
        //}
    }

    protected void btnQuitarArchivoInterno_Click(object sender, EventArgs e)
    {
        Button btnQuitarArchivoInterno = sender as Button;
        GridViewRow gRowIn = (GridViewRow)(btnQuitarArchivoInterno).Parent.Parent;

        //Leer Referencia Externa
        ReferenciaNoConciliada rcExterna = leerReferenciaExternaSeleccionada();

        //Leer el tipoConciliacion URL
        tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

        //Leer Referencia (Archivo) que se va quitar   
        int folioIn = Convert.ToInt32(grvAgregadosInternos.DataKeys[gRowIn.RowIndex].Values["Folio"]);
        int secuenciaIn = Convert.ToInt32(grvAgregadosInternos.DataKeys[gRowIn.RowIndex].Values["Secuencia"]);
        int añoIn = Convert.ToInt32(grvAgregadosInternos.DataKeys[gRowIn.RowIndex].Values["Año"]);
        int sucursalIn = Convert.ToInt16(grvAgregadosInternos.DataKeys[gRowIn.RowIndex].Values["Sucursal"]);

        rcExterna.QuitarReferenciaConciliada(sucursalIn, añoIn, folioIn, secuenciaIn);
        //Generar el GridView para las Referencias Internas(ARCHIVOS / PEDIDOS)
        GenerarTablaAgregadosArchivosInternos(rcExterna, tipoConciliacion);
        ConsultarArchivosInternos();
        ActualizarTotalesAgregados();
    }

    protected void grvPedidos_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtSortTable = HttpContext.Current.Session["TAB_INTERNOS_AX"] as DataTable;
        if (dtSortTable == null) return;
        string order = getSortDirectionString(e.SortExpression);
        dtSortTable.DefaultView.Sort = e.SortExpression + " " + order;
        HttpContext.Current.Session["TAB_INTERNOS_AX"] = dtSortTable;
        grvPedidos.DataSource = HttpContext.Current.Session["TAB_INTERNOS_AX"] as DataTable;
        grvPedidos.DataBind();
    }

    public string resaltarBusqueda(string entradaTexto)
    {
        if (!txtBuscar.Text.Equals(""))
        {
            string strBuscar = txtBuscar.Text;
            Regex RegExp = new Regex(strBuscar.Replace(" ", "|").Trim(),
                                     RegexOptions.IgnoreCase);
            return RegExp.Replace(entradaTexto, pintarBusqueda);
        }
        return entradaTexto;
    }

    public string pintarBusqueda(Match m)
    {
        return "<span class=marcarBusqueda>" + m.Value + "</span>";
    }

    private string getSortDirectionString(string columna)
    {
        string sortDirection = "ASC";

        string sortExpression = ViewState["SortExpression"] as string;

        if (sortExpression != null)
        {
            if (sortExpression == columna)
            {
                string lastDirection = ViewState["SortDirection"] as string;
                if ((lastDirection != null) && (lastDirection == "ASC"))
                {
                    sortDirection = "DESC";
                }
            }
        }

        ViewState["SortDirection"] = sortDirection;
        ViewState["SortExpression"] = columna;
        return sortDirection;
    }

    private bool FiltrarCampo(string valorFiltro, string filtroEn)
    {
        bool resultado;
        try
        {
            DataTable dt = filtroEn.Equals("Externos")
                ? (DataTable) HttpContext.Current.Session["TAB_EXTERNOS"]
                : filtroEn.Equals("Internos")
                    ? (DataTable) HttpContext.Current.Session["TAB_INTERNOS"]
                    : (DataTable) HttpContext.Current.Session["TAB_CONCILIADAS"];

            DataView dv = new DataView(dt);
            string SearchExpression = String.Empty;
            if (!String.IsNullOrEmpty(valorFiltro))
            {
                SearchExpression = string.Format(
                    ddlOperacion.SelectedItem.Value == "LIKE"
                        ? "{0} {1} '%{2}%'"
                        : "{0} {1} '{2}'", ddlCampoFiltrar.SelectedItem.Text,
                    ddlOperacion.SelectedItem.Value, valorFiltro);
            }
            if (dv.Count <= 0) return false;
            dv.RowFilter = SearchExpression;

            if (filtroEn.Equals("Externos"))
            {
                //Leer Variables URL 
                cargarInfoConciliacionActual();

                HttpContext.Current.Session["TAB_EXTERNOS_AX"] = dv.ToTable();
                grvExternos.DataSource = HttpContext.Current.Session["TAB_EXTERNOS_AX"] as DataTable;
                grvExternos.DataBind();
                if (grvExternos.Rows.Count > 0)
                {
                    //Referencia Externa
                    ReferenciaNoConciliada rfExterno = leerReferenciaExternaSeleccionada();
                    LimpiarExternosReferencia(rfExterno);
                    GenerarTablaAgregadosArchivosInternos(rfExterno, tipoConciliacion);
                }
                else
                {
                    LimpiarExternosTodos();
                    GenerarTablaAgregadosVacia(tipoConciliacion);
                }
                ActualizarTotalesAgregados();
                //CONSULTAR INTERNO(ARCHIVOS O PEDIDOS)
                if (tipoConciliacion == 2)
                    ConsultarPedidosInternos();
                else
                    ConsultarArchivosInternos();
            }
            else if (filtroEn.Equals("Internos"))
            {
                HttpContext.Current.Session["TAB_INTERNOS_AX"] = dv.ToTable();
                //Leer el tipoConciliacion URL
                tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

                if (tipoConciliacion == 2)
                {
                    grvPedidos.DataSource = HttpContext.Current.Session["TAB_INTERNOS_AX"] as DataTable;
                    grvPedidos.DataBind();
                }
                else
                {
                    grvInternos.DataSource = HttpContext.Current.Session["TAB_INTERNOS_AX"] as DataTable;
                    grvInternos.DataBind();
                }

            }
            else
            {
                HttpContext.Current.Session["TAB_CONCILIADAS_AX"] = dv.ToTable();
                grvConciliadas.DataSource = HttpContext.Current.Session["TAB_CONCILIADAS_AX"] as DataTable;
                grvConciliadas.DataBind();
            }
            resultado = true;
            mpeFiltrar.Hide();
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            resultado = false;
            mpeFiltrar.Hide();
        }

        return resultado;
    }

    protected void btnIrFiltro_Click(object sender, EventArgs e)
    {
        //Leer el tipoConciliacion URL
        tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

        cargar_ComboCampoFiltroDestino(tipoConciliacion, ddlFiltrarEn.SelectedItem.Value);
        bool resultado = FiltrarCampo(valorFiltro(tipoCampoSeleccionado()), ddlFiltrarEn.SelectedItem.Value);
        statusFiltro = ddlFiltrarEn.SelectedItem.Value.Equals("Internos") && resultado;
        Session["StatusFiltro"] = statusFiltro;
        tipoFiltro = statusFiltro ? "CA" : String.Empty;
        Session["TipoFiltro"] = tipoFiltro;
        mpeFiltrar.Hide();
    }

    protected void btnIrBuscar_Click(object sender, EventArgs e)
    {
        //Leer el tipoConciliacion URL
        tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

        if (ddlBuscarEn.SelectedItem.Value.Equals("Externos"))
        {
            grvExternos.DataSource = HttpContext.Current.Session["TAB_EXTERNOS_AX"] as DataTable;
            grvExternos.DataBind();
        }
        else if (ddlBuscarEn.SelectedItem.Value.Equals("Internos"))
        {
            if (tipoConciliacion == 2)
            {
                grvPedidos.DataSource = HttpContext.Current.Session["TAB_INTERNOS_AX"] as DataTable;
                grvPedidos.DataBind();
            }
            else
            {
                grvInternos.DataSource = HttpContext.Current.Session["TAB_INTERNOS_AX"] as DataTable;
                grvInternos.DataBind();
            }
        }
        else
        {
            grvConciliadas.DataSource = HttpContext.Current.Session["TAB_CONCILIADAS_AX"] as DataTable;
            grvConciliadas.DataBind();
        }
        mpeBuscar.Hide();
    }

    //protected void imgBuscar_Click(object sender, ImageClickEventArgs e)
    //{
    //    txtBuscar.Text = String.Empty;
    //    mpeBuscar.Show();
    //}

    protected void grvAgregadosPedidos_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow) return;
        e.Row.Attributes.Add("onmouseover", "this.className='bg-color-rojo01'");
        e.Row.Attributes.Add("onmouseout", "this.className='bg-color-blanco'");
    }

    protected void OnCheckedChangedExternos(object sender, EventArgs e)
    {
        CheckBox chk = (sender as CheckBox);
        if (chk.ID == "chkTodosExternos")
            foreach (
                GridViewRow fila in
                    grvExternos.Rows.Cast<GridViewRow>().Where(fila => fila.RowType == DataControlRowType.DataRow))
                fila.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked = chk.Checked;
    }

    protected void OnCheckedChangedInternos(object sender, EventArgs e)
    {
        CheckBox chk = (sender as CheckBox);
        if (chk.ID == "chkTodosInternos")
            foreach (
                GridViewRow fila in
                    grvInternos.Rows.Cast<GridViewRow>().Where(fila => fila.RowType == DataControlRowType.DataRow))
                fila.Cells[1].Controls.OfType<CheckBox>().FirstOrDefault().Checked = chk.Checked;

    }

    public void ocultarOpcionesSeleccionadoExterno()
    {
        if ((from GridViewRow row in grvExternos.Rows
             where row.RowType == DataControlRowType.DataRow
             select row.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked).Any(estaMarcado => estaMarcado))
        {
            btnENPROCESOEXTERNO.Visible = true;
            if (!hdfExternosControl.Value.Equals("CANCELADOS"))
                btnCANCELAREXTERNO.Visible = true;
        }
        else
        {
            btnENPROCESOEXTERNO.Visible = false;
            btnCANCELAREXTERNO.Visible = false;
        }
    }

    public void ocultarOpcionesSeleccionadoInterno()
    {
        if ((from GridViewRow row in grvInternos.Rows
             where row.RowType == DataControlRowType.DataRow
             select row.Cells[1].Controls.OfType<CheckBox>().FirstOrDefault().Checked).Any(estaMarcado => estaMarcado))
        {
            btnENPROCESOINTERNO.Visible = true;
            if (!hdfInternosControl.Value.Equals("CANCELADOS"))
                btnCANCELARINTERNO.Visible = true;
        }
        else
        {
            btnENPROCESOINTERNO.Visible = false;
            btnCANCELARINTERNO.Visible = false;
        }
    }

    public List<GridViewRow> filasSeleccionadasExternos(string status)
    {
        return
            grvExternos.Rows.Cast<GridViewRow>()
                       .Where(
                           fila =>
                           fila.RowType == DataControlRowType.DataRow &&
                           (fila.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked) &&
                           (fila.FindControl("imgStatusConciliacion") as System.Web.UI.WebControls.Image).AlternateText
                                                                                                         .Equals(status))
                       .ToList();
    }

    /*******************
    * Agrego: Santiago Mendoza Carlos Nirari
    * Fecha:01/08/2014
    * Decripcion: Almacena el indice de las filas que si esten seleccionadas
    ********************/

    public List<GridViewRow> filasSeleccionadasExternos()
    {
        return
            grvExternos.Rows.Cast<GridViewRow>()
                       .Where(
                           fila =>
                           fila.RowType == DataControlRowType.DataRow &&
                           (fila.Cells[0].Controls.OfType<CheckBox>().FirstOrDefault().Checked))
                       .ToList();
    }

    public List<GridViewRow> filasSeleccionadasInternos(string status)
    {
        return
            grvInternos.Rows.Cast<GridViewRow>()
                       .Where(
                           fila =>
                           fila.RowType == DataControlRowType.DataRow &&
                           (fila.Cells[1].Controls.OfType<CheckBox>().FirstOrDefault().Checked) &&
                           (fila.FindControl("imgStatusConciliacion") as System.Web.UI.WebControls.Image).AlternateText
                                                                                                         .Equals(status))
                       .ToList();
    }

    protected void btnAceptarStatusExterno_Click(object sender, EventArgs e)
    {
        int secuenciaExterno;
        int folioExterno;
        ReferenciaNoConciliada rfExterno;
        //Leer Variables URL 
        cargarInfoConciliacionActual();

        List<GridViewRow> rowsSeleccionados = filasSeleccionadasExternos("EN PROCESO DE CONCILIACION");


        foreach (GridViewRow fila in rowsSeleccionados)
        {
            listaReferenciaExternas = Session["POR_CONCILIAR_EXTERNO"] as List<ReferenciaNoConciliada>;

            secuenciaExterno = Convert.ToInt32(grvExternos.DataKeys[fila.RowIndex].Values["Secuencia"]);
            folioExterno = Convert.ToInt32(grvExternos.DataKeys[fila.RowIndex].Values["Folio"]);
            rfExterno = listaReferenciaExternas.Single(x => x.Secuencia == secuenciaExterno && x.Folio == folioExterno);

            rfExterno.MotivoNoConciliado = Convert.ToInt32(ddlMotivosNoConciliado.SelectedItem.Value);
            rfExterno.ComentarioNoConciliado = txtComentario.Text;
            if (tipoConciliacion == 2)
                rfExterno.CancelarExternoPedido();
            else
                rfExterno.CancelarExternoInterno();
        }
        Consulta_Externos(corporativo, sucursal, año, mes, folio, Convert.ToDecimal(txtDiferencia.Text),
                          tipoConciliacion, Convert.ToInt32(ddlStatusConcepto.SelectedValue), EsDepositoRetiro());
        GenerarTablaExternos();
        LlenaGridViewExternos();
        //Limpiar Referencias de Externos
        if (grvExternos.Rows.Count > 0)
        {
            //Referencia Externa
            rfExterno = leerReferenciaExternaSeleccionada();
            LimpiarExternosReferencia(rfExterno);
            GenerarTablaAgregadosArchivosInternos(rfExterno, tipoConciliacion);
        }
        else
        {
            LimpiarExternosTodos();
            GenerarTablaAgregadosVacia(tipoConciliacion);
        }
        ActualizarTotalesAgregados();
        //CONSULTAR INTERNO(ARCHIVOS O PEDIDOS)
        if (tipoConciliacion == 2)
            ConsultarPedidosInternos();
        else
            ConsultarArchivosInternos();

        mpeStatusTransaccion.Hide();
    }

    protected void btnAceptarStatusInterno_Click(object sender, EventArgs e)
    {
        int secuenciaInt;
        int folioInt;
        ReferenciaNoConciliada rfInterna;

        List<GridViewRow> rowsSeleccionados = filasSeleccionadasInternos("EN PROCESO DE CONCILIACION");
        //Leer Referencias Internas
        listaReferenciaArchivosInternos = Session["POR_CONCILIAR_INTERNO"] as List<ReferenciaNoConciliada>;

        foreach (GridViewRow fila in rowsSeleccionados)
        {


            secuenciaInt = Convert.ToInt32(grvInternos.DataKeys[fila.RowIndex].Values["Secuencia"]);
            folioInt = Convert.ToInt32(grvInternos.DataKeys[fila.RowIndex].Values["Folio"]);
            rfInterna = listaReferenciaArchivosInternos.Single(x => x.Secuencia == secuenciaInt && x.Folio == folioInt);
            rfInterna.MotivoNoConciliado = Convert.ToInt32(ddlMotivosNoConciliado.SelectedItem.Value);
            rfInterna.ComentarioNoConciliado = txtComentario.Text;
            rfInterna.CancelarInterno();
        }
        ConsultarArchivosInternos();
        mpeStatusTransaccion.Hide();
    }

    public string tipoCampoSeleccionado()
    {
        try
        {
            return listCamposDestino[ddlCampoFiltrar.SelectedIndex].Campo1;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public string valorFiltro(string tipoCampo)
    {
        try
        {
            switch (tipoCampo)
            {
                case "Cadena":
                    return txtValor.Text;
                    break;
                case "Numero":
                    decimal num = Convert.ToDecimal(txtValor.Text);
                    return num.ToString();
                    break;
                case "Fecha":
                    DateTime fecha = Convert.ToDateTime(txtValor.Text);
                    return fecha.ToString();
                    break;
            }

        }
        catch (Exception ex)
        {
            App.ImplementadorMensajes.MostrarMensaje("Verifique:\n- Valor no valido por tipo de Campo seleccionado.");
        }
        return "";
        //return tipoCampo.Equals("Cadena")
        //           ? txtValorCadenaFiltro.Text
        //           : (tipoCampo.Equals("Fecha") ? txtValorFechaFiltro.Text : txtValorNumericoFiltro.Text);

    }

    public void InicializarControlesFiltro()
    {
        ddlCampoFiltrar.SelectedIndex = ddlOperacion.SelectedIndex = 0;
        txtValor.Text = String.Empty;
    }

    public void activarVerPendientesCanceladosInternos(bool activar)
    {
        try
        {
            if (activar)
            {
                hdfInternosControl.Value = "PENDIENTES";
                lblStatusGridInternos.Text = hdfInternosControl.Value;
                statusGridInternos.Attributes.Add("class", "bg-color-azulClaro");
                btnHistorialPendientesInterno.Visible = true;
                btnRegresarInterno.Visible = false;
                btnHistorialPendientesExterno.Visible = true;
            }
            else
            {
                hdfInternosControl.Value = "CANCELADOS";
                lblStatusGridInternos.Text = hdfInternosControl.Value.ToString();
                statusGridInternos.Attributes.Add("class", "bg-color-rojo");
                btnHistorialPendientesInterno.Visible = false;
                btnRegresarInterno.Visible = true;
                btnHistorialPendientesExterno.Visible = false;
            }
            activarOpcionesCancelarProcesoIn(activar);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void activarVerPendientesCanceladosExternos(bool activar)
    {
        try
        {
            //Leer el tipoConciliacion URL
            tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

            if (activar)
            {
                hdfExternosControl.Value = "PENDIENTES";
                lblStatusGridExternos.Text = hdfExternosControl.Value;
                statusGridExternos.Attributes.Add("class", "bg-color-azulClaro");
                btnHistorialPendientesExterno.Visible = true;
                btnRegresarExterno.Visible = false;
                btnHistorialPendientesInterno.Visible = tipoConciliacion != 2;
            }
            else
            {
                hdfExternosControl.Value = "CANCELADOS";
                lblStatusGridExternos.Text = hdfExternosControl.Value;
                statusGridExternos.Attributes.Add("class", "bg-color-rojo");
                btnHistorialPendientesExterno.Visible = false;
                btnRegresarExterno.Visible = true;
                btnHistorialPendientesInterno.Visible = false;
            }
            activarOpcionesCancelarProcesoEx(activar);
        }
        catch (Exception ex)
        {
            throw ex;
        }
       
    }

    public void activarOpcionesCancelarProcesoIn(bool activar)
    {
        btnENPROCESOINTERNO.Visible = btnCANCELARINTERNO.Visible = activar;
    }

    public void activarOpcionesCancelarProcesoEx(bool activar)
    {
        btnENPROCESOEXTERNO.Visible = btnCANCELAREXTERNO.Visible = activar;
    }

    public bool referenciaExInCancelada(string tpReferencia)
    {
        return tpReferencia.Equals("Externo")
                   ? !((Label)grvExternos.Rows[indiceExternoSeleccionado].FindControl("lblStatusConciliacion")).Text
                                                                                                                .Equals(
                                                                                                                    "CONCILIACION CANCELADA")
                   : !((Label)grvInternos.Rows[indiceInternoSeleccionado].FindControl("lblStatusConciliacion")).Text
                                                                                                                .Equals(
                                                                                                                    "CONCILIACION CANCELADA");
    }

    protected void btnHistorialPendientesInterno_Click(object sender, ImageClickEventArgs e)
    {
        if (grvExternos.Rows.Count > 0)
        {
            if (referenciaExInCancelada("Externo"))
            {
                activarVerPendientesCanceladosInternos(false);
                ConsultarArchivosInternos();
            }
            else
                App.ImplementadorMensajes.MostrarMensaje("Referencia Externa Cancelada");
        }
        else
        {
            App.ImplementadorMensajes.MostrarMensaje("NO EXISTEN TRANSACCIONES EXTERNAS");
        }
    }

    public void verExternosCanceladosPendientes()
    {
        try
        {
            //Cargar Info Actual Conciliacion
            cargarInfoConciliacionActual();
            if (tipoConciliacion == 2)
            {
                if (grvPedidos.Rows.Count <= 0)
                {
                    App.ImplementadorMensajes.MostrarMensaje("No hay pedido origen");
                    return;
                }
                Consulta_ExternosPendientesCancelados(corporativo, sucursal, año,
                                                      mes, folio, 0, 0, 0,
                                                      Convert.ToDecimal(txtDiferencia.Text),
                                                      Convert.ToInt32(ddlStatusConcepto.SelectedItem.Value));
            }
            else
            {
                if (grvInternos.Rows.Count <= 0)
                {
                    App.ImplementadorMensajes.MostrarMensaje("No hay archivo Interno seleccionado");
                    return;
                }
                ReferenciaNoConciliada rfIn = leerReferenciaInternaSeleccionada();

                if (referenciaExInCancelada("Interno"))
                    Consulta_ExternosPendientesCancelados(corporativo, sucursal, año,
                                                          mes, folio, rfIn.Sucursal, rfIn.Folio, rfIn.Secuencia,
                                                          Convert.ToDecimal(txtDiferencia.Text),
                                                          Convert.ToInt32(ddlStatusConcepto.SelectedItem.Value));
                else
                {
                    App.ImplementadorMensajes.MostrarMensaje("Referencia Interna Cancelada");
                    return;
                }

            }
            activarVerPendientesCanceladosExternos(false);
            GenerarTablaExternos();
            LlenaGridViewExternos();
            LimpiarExternosTodos();
            ActualizarTotalesAgregados();
            if (tipoConciliacion == 2)
                ConsultarPedidosInternos();
            else
                ConsultarArchivosInternos();
        }
        catch (Exception ex)
        {
            App.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.StackTrace);
        }
    }


    //Falta una validacion 
    public ReferenciaNoConciliada leerReferenciaExternaSeleccionada()
    {
        try
        {
            listaReferenciaExternas = Session["POR_CONCILIAR_EXTERNO"] as List<ReferenciaNoConciliada>;
            int secuenciaExterno = Convert.ToInt32(grvExternos.DataKeys[indiceExternoSeleccionado].Values["Secuencia"]);
            int folioExterno = Convert.ToInt32(grvExternos.DataKeys[indiceExternoSeleccionado].Values["Folio"]);
            return listaReferenciaExternas.Single(x => x.Secuencia == secuenciaExterno && x.Folio == folioExterno);
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    public ReferenciaNoConciliada leerReferenciaInternaSeleccionada()
    {
        //Leer Referencias Internas
        listaReferenciaArchivosInternos = Session["POR_CONCILIAR_INTERNO"] as List<ReferenciaNoConciliada>;

        int secuenciaInterno =
            Convert.ToInt32(grvInternos.DataKeys[indiceInternoSeleccionado].Values["Secuencia"]);
        int folioInterno = Convert.ToInt32(grvInternos.DataKeys[indiceInternoSeleccionado].Values["Folio"]);
        int sucursalInterno = Convert.ToInt16(grvInternos.DataKeys[indiceInternoSeleccionado].Values["Sucursal"]);

        return
            listaReferenciaArchivosInternos.Single(
                x => x.Secuencia == secuenciaInterno && x.Folio == folioInterno && x.Sucursal == sucursalInterno);
    }

    protected void btnHistorialPendientesExterno_Click(object sender, ImageClickEventArgs e)
    {
        tranExternaAnteriorSeleccionada = leerReferenciaExternaSeleccionada();
        verExternosCanceladosPendientes();
    }

    protected void btnENPROCESOEXTERNO_Click(object sender, ImageClickEventArgs e)
    {
        List<GridViewRow> rowsSeleccionados = filasSeleccionadasExternos("CONCILIACION CANCELADA");
        //Cargar Info Actual Conciliacion
        cargarInfoConciliacionActual();
        if (rowsSeleccionados.Count > 0)
        {
            int secuenciaExterno;
            int folioExterno;
            ReferenciaNoConciliada rfExterno = App.ReferenciaNoConciliada.CrearObjeto();
            listaReferenciaExternas = Session["POR_CONCILIAR_EXTERNO"] as List<ReferenciaNoConciliada>;
            foreach (GridViewRow fila in rowsSeleccionados)
            {
                secuenciaExterno = Convert.ToInt32(grvExternos.DataKeys[fila.RowIndex].Values["Secuencia"]);
                folioExterno = Convert.ToInt32(grvExternos.DataKeys[fila.RowIndex].Values["Folio"]);
                rfExterno =
                    listaReferenciaExternas.Single(x => x.Secuencia == secuenciaExterno && x.Folio == folioExterno);
                if (tipoConciliacion == 2)
                    rfExterno.EliminarReferenciaConciliadaPedido();
                else
                    rfExterno.EliminarReferenciaConciliada();
            }


            //Aqui ver si cargar nuevamente los cancelado pendientes o si los normales
            if (hdfExternosControl.Value.Equals("PENDIENTES"))
            {
                Consulta_Externos(corporativo, sucursal, año, mes, folio, Convert.ToDecimal(txtDiferencia.Text),
                                  tipoConciliacion, Convert.ToInt32(ddlStatusConcepto.SelectedValue), EsDepositoRetiro());
                GenerarTablaExternos();
                LlenaGridViewExternos();
            }
            else
            {
                if (tipoConciliacion == 2)
                    Consulta_ExternosPendientesCancelados(corporativo, sucursal, año,
                                                          mes, folio, 0, 0, 0,
                                                          Convert.ToDecimal(txtDiferencia.Text),
                                                          Convert.ToInt32(ddlStatusConcepto.SelectedItem.Value));
                else
                {

                    ReferenciaNoConciliada rfIn = leerReferenciaInternaSeleccionada();
                    Consulta_ExternosPendientesCancelados(corporativo, sucursal, año,
                                                          mes, folio, rfIn.Sucursal, rfIn.Folio, rfIn.Secuencia,
                                                          Convert.ToInt32(txtDiferencia.Text),
                                                          Convert.ToInt32(ddlStatusConcepto.SelectedItem.Value));
                }

                GenerarTablaExternos();
                LlenaGridViewExternos();
            }
            if (grvExternos.Rows.Count > 0)
            {
                //Referencia Externa
                rfExterno = leerReferenciaExternaSeleccionada();
                LimpiarExternosReferencia(rfExterno);
                GenerarTablaAgregadosArchivosInternos(rfExterno, tipoConciliacion);
            }
            else
            {
                LimpiarExternosTodos();
                GenerarTablaAgregadosVacia(tipoConciliacion);
            }
            ActualizarTotalesAgregados();
        }
        else
            App.ImplementadorMensajes.MostrarMensaje("Verifique su selección , siguientes razones: \n" +
                                                     "1. No existe ninguna referencia externa seleccionada. \n" +
                                                     "2. Ninguna de las referencias externas seleccionadas estan CANCELADAS");
    }

    protected void btnCANCELAREXTERNO_Click(object sender, ImageClickEventArgs e)
    {
        if (filasSeleccionadasExternos("EN PROCESO DE CONCILIACION").Count > 0)
        {
            btnAceptarStatusExterno.Visible = true;
            dvMensajeExterno.Visible = true;
            btnAceptarStatusInterno.Visible = false;
            txtComentario.Text = String.Empty;
            mpeStatusTransaccion.Show();
        }
        else
            App.ImplementadorMensajes.MostrarMensaje("Verifique su selección , siguientes razones: \n" +
                                                     "1. No existe ninguna referencia externa seleccionada. \n" +
                                                     "2. Ninguna de las referencias externas seleccionadas estan EN PROCESO DE CONCILIACIÓN");
    }

    protected void btnRegresarExterno_Click(object sender, ImageClickEventArgs e)
    {
        activarVerPendientesCanceladosExternos(true);
        //Cargar Info Actual Conciliacion
        cargarInfoConciliacionActual();

        btnHistorialPendientesInterno.Visible = tipoConciliacion != 2;

        Consulta_Externos(corporativo, sucursal, año, mes, folio, Convert.ToDecimal(txtDiferencia.Text),
                          tipoConciliacion, Convert.ToInt32(ddlStatusConcepto.SelectedValue), EsDepositoRetiro());
        GenerarTablaExternos();
        LlenaGridViewExternos();

        ReferenciaNoConciliada rfExterno = leerReferenciaExternaSeleccionada();
        //Limpiar Referncias de Externos 
        LimpiarExternosReferencia(rfExterno);

        //CONSULTAR INTERNO(ARCHIVOS O PEDIDOS)
        if (tipoConciliacion == 2)
            ConsultarPedidosInternos();
        else
            ConsultarArchivosInternos();

        GenerarTablaAgregadosArchivosInternos(rfExterno, tipoConciliacion);
        ActualizarTotalesAgregados();

    }

    protected void btnRegresarInterno_Click(object sender, ImageClickEventArgs e)
    {
        activarVerPendientesCanceladosInternos(true);
        ConsultarArchivosInternos();
    }

    protected void grvInternos_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtSortTable = HttpContext.Current.Session["TAB_INTERNOS_AX"] as DataTable;
        if (dtSortTable == null) return;
        string order = getSortDirectionString(e.SortExpression);
        dtSortTable.DefaultView.Sort = e.SortExpression + " " + order;
        grvInternos.DataSource = dtSortTable;
        grvInternos.DataBind();
    }

    protected void grvExternos_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtSortTable = HttpContext.Current.Session["TAB_EXTERNOS_AX"] as DataTable;
        if (dtSortTable != null)
        {
            string order = getSortDirectionString(e.SortExpression);
            dtSortTable.DefaultView.Sort = e.SortExpression + " " + order;
            grvExternos.DataSource = dtSortTable;
            grvExternos.DataBind();
        }

        ReferenciaNoConciliada rfExterno = leerReferenciaExternaSeleccionada();
        //Limpiar Referncias de Externos 
        LimpiarExternosReferencia(rfExterno);
        statusFiltro = false;
        Session["StatusFiltro"] = statusFiltro;
        tipoFiltro = String.Empty;
        Session["TipoFiltro"] = tipoFiltro;

        //Leer el tipoConciliacion URL
        tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

        if (tipoConciliacion == 2)
            ConsultarPedidosInternos();
        else
            ConsultarArchivosInternos();

        GenerarTablaAgregadosArchivosInternos(rfExterno, tipoConciliacion);
        ActualizarTotalesAgregados();
    }

    protected void btnENPROCESOINTERNO_Click(object sender, ImageClickEventArgs e)
    {
        List<GridViewRow> rowsSeleccionados = filasSeleccionadasInternos("CONCILIACION CANCELADA");
        if (rowsSeleccionados.Count > 0)
        {
            int secuenciaInterno;
            int folioInterno;
            ReferenciaNoConciliada rfInterno;
            //Leer Referencias Internas
            listaReferenciaArchivosInternos = Session["POR_CONCILIAR_INTERNO"] as List<ReferenciaNoConciliada>;

            foreach (GridViewRow fila in rowsSeleccionados)
            {
                secuenciaInterno = Convert.ToInt32(grvInternos.DataKeys[fila.RowIndex].Values["Secuencia"]);
                folioInterno = Convert.ToInt32(grvInternos.DataKeys[fila.RowIndex].Values["Folio"]);
                rfInterno =
                    listaReferenciaArchivosInternos.Single(
                        x => x.Secuencia == secuenciaInterno && x.Folio == folioInterno);
                rfInterno.EliminarReferenciaConciliadaInterno();
            }
            ConsultarArchivosInternos();
        }
        else
            App.ImplementadorMensajes.MostrarMensaje("Verifique su selección , siguientes razones: \n" +
                                                     "1. No existe ninguna referencia interna seleccionada. \n" +
                                                     "2. Ninguna de las referencias internas seleccionadas estan CANCELADAS");

    }

    protected void btnCANCELARINTERNO_Click(object sender, ImageClickEventArgs e)
    {
        if (filasSeleccionadasInternos("EN PROCESO DE CONCILIACION").Count > 0)
        {
            btnAceptarStatusExterno.Visible = false;
            dvMensajeExterno.Visible = false;
            btnAceptarStatusInterno.Visible = true;
            txtComentario.Text = String.Empty;
            mpeStatusTransaccion.Show();
        }
        else
            App.ImplementadorMensajes.MostrarMensaje("Verifique su selección , siguientes razones: \n" +
                                                     "1. No existe ninguna referencia interna seleccionada. \n" +
                                                     "2. Ninguna de las referencias internas seleccionadas estan EN PROCESO DE CONCILIACIÓN");

    }

    protected void grvConciliadas_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataTable dtSortTable = HttpContext.Current.Session["TAB_CONCILIADAS_AX"] as DataTable;

        if (dtSortTable == null) return;
        string order = getSortDirectionString(e.SortExpression);
        dtSortTable.DefaultView.Sort = e.SortExpression + " " + order;

        grvConciliadas.DataSource = dtSortTable;
        grvConciliadas.DataBind();
    }

    protected void btnActualizarConfig_Click(object sender, ImageClickEventArgs e)
    {
        //activarVerPendientesCanceladosExternos(true);
        //Cargar Info Actual Conciliacion
        cargarInfoConciliacionActual();
        Consulta_Externos(corporativo, sucursal, año, mes, folio, Convert.ToDecimal(txtDiferencia.Text),
                          tipoConciliacion, Convert.ToInt32(ddlStatusConcepto.SelectedValue), EsDepositoRetiro());
        GenerarTablaExternos();
        LlenaGridViewExternos();
        //Limpiar Referncias de Externos
        if (grvExternos.Rows.Count > 0)
        {
            //Referencia Externa
            ReferenciaNoConciliada rfExterno = leerReferenciaExternaSeleccionada();
            LimpiarExternosReferencia(rfExterno);
            GenerarTablaAgregadosArchivosInternos(rfExterno, tipoConciliacion);
        }
        else
        {
            LimpiarExternosTodos();
            GenerarTablaAgregadosVacia(tipoConciliacion);
        }
        //CONSULTAR INTERNO(ARCHIVOS O PEDIDOS)
        if (tipoConciliacion == 2)
            ConsultarPedidosInternos();
        else
        {
            activarVerPendientesCanceladosInternos(true);
            ConsultarArchivosInternos();
        }
        ActualizarTotalesAgregados();
        statusFiltro = false;
        Session["StatusFiltro"] = statusFiltro;
        tipoFiltro = String.Empty;
        Session["TipoFiltro"] = tipoFiltro;
    }

    protected void imgAutomatica_Click(object sender, ImageClickEventArgs e)
    {
        string criterioConciliacion = ddlCriteriosConciliacion.SelectedItem.Text.Equals("CANTIDAD CONCUERDA")
            ? "CantidadConcuerda"
            //: ddlCriteriosConciliacion.SelectedItem.Text.Equals(
            //    "CANTIDAD Y REFERENCIA CONCUERDAN")
            //      ? "CantidadYReferenciaConcuerdan"
            : ddlCriteriosConciliacion.SelectedItem.Text.Equals("CANTIDAD Y REFERENCIA CONCUERDAN")
                ? "CantidadYReferenciaConcuerdanEdificios"
                : ddlCriteriosConciliacion.SelectedItem.Text.Equals("CANTIDAD Y REFERENCIA CONCUERDAN PEDIDOS")
                    ? "CantidadYReferenciaConcuerdan"
                    : ddlCriteriosConciliacion.SelectedItem.Text.Equals("UNO A VARIOS")
                        ? "UnoAVarios"
                        : ddlCriteriosConciliacion.SelectedItem.Text.Equals("VARIOS A UNO")
                            ? "VariosAUno"
                            : ddlCriteriosConciliacion.SelectedItem.Text.Equals(
                                "COPIA DE CONCILIACION")
                                ? "CopiaDeConciliacion"
                                : "Manual";


        //Cargar Info Actual Conciliacion
        cargarInfoConciliacionActual();
        //Eliminar variables de Session
        limpiarVariablesSession();
        Response.Redirect("~/Conciliacion/FormasConciliar/" + criterioConciliacion +
                          ".aspx?Folio=" + folio + "&Corporativo=" + corporativo +
                          "&Sucursal=" + sucursal + "&Año=" + año + "&Mes=" +
                          mes + "&TipoConciliacion=" + tipoConciliacion);
    }

    protected void Nueva_Ventana(string pagina, string titulo, int ancho, int alto, int x, int y)
    {

        ScriptManager.RegisterClientScriptBlock(this.upBarraHerramientas,
                                                upBarraHerramientas.GetType(),
                                                "ventana",
                                                "ShowWindow('" + pagina + "','" + titulo + "'," + ancho + "," + alto +
                                                "," + x + "," + y + ")",
                                                true);

    }

    protected void imgExportar_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            AppSettingsReader settings = new AppSettingsReader();

            //Leer Variables URL 
            cargarInfoConciliacionActual();

            string strReporte;
            if (tipoConciliacion == 2)
                strReporte = Server.MapPath("~/") + settings.GetValue("RutaReporteRemanentesConciliacion", typeof(string));
            else
                strReporte = Server.MapPath("~/") + settings.GetValue("RutaReporteConciliacionTesoreria", typeof(string));

            if (!File.Exists(strReporte)) return;
            try
            {
                string strServer = settings.GetValue("Servidor", typeof(string)).ToString();
                string strDatabase = settings.GetValue("Base", typeof(string)).ToString();
                //Cargar Info Actual Conciliacion
                cargarInfoConciliacionActual();
                usuario = (SeguridadCB.Public.Usuario)HttpContext.Current.Session["Usuario"];
                string strUsuario = usuario.IdUsuario.Trim();
                string strPW = usuario.ClaveDesencriptada;
                ArrayList Par = new ArrayList();

                Par.Add("@Corporativo=" + corporativo);
                Par.Add("@Sucursal=" + sucursal);
                Par.Add("@AñoConciliacion=" + año);
                Par.Add("@MesConciliacion=" + mes);
                Par.Add("@FolioConciliacion=" + folio);
                ClaseReporte reporte = new ClaseReporte(strReporte, Par, strServer, strDatabase, strUsuario, strPW);
                HttpContext.Current.Session["RepDoc"] = reporte.RepDoc;
                HttpContext.Current.Session["ParametrosReporte"] = Par;
                Nueva_Ventana("../../Reporte/Reporte.aspx", "Carta", 0, 0, 0, 0);
                reporte = null;
            }
            catch (Exception ex)
            {
                App.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);
            }
        }
        catch (Exception ex)
        {
            App.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);
        }
    }

    protected void imgCerrarConciliacion_Click(object sender, ImageClickEventArgs e)
    {
        usuario = (SeguridadCB.Public.Usuario)HttpContext.Current.Session["Usuario"];
        //Cargar Info Actual Conciliacion
        cargarInfoConciliacionActual();
        cConciliacion c = App.Consultas.ConsultaConciliacionDetalle(corporativo, sucursal, año, mes, folio);
        if (c.CerrarConciliacion(usuario.IdUsuario))
        {
            App.ImplementadorMensajes.MostrarMensaje("CONCILIACIÓN CERRADA EXITOSAMENTE");
            System.Threading.Thread.Sleep(3000);
            Response.Redirect("~/Conciliacion/DetalleConciliacion.aspx?Folio=" + folio + "&Corporativo=" + corporativo +
                              "&Sucursal=" + sucursal + "&Año=" + año + "&Mes=" +
                              mes + "&TipoConciliacion=" + tipoConciliacion);
        }
        else
        {
            App.ImplementadorMensajes.MostrarMensaje("ERRORES AL CERRAR LA CONCILIACIÓN");
        }
    }

    protected void imgCancelarConciliacion_Click(object sender, ImageClickEventArgs e)
    {
        usuario = (SeguridadCB.Public.Usuario)HttpContext.Current.Session["Usuario"];
        //Cargar Info Actual Conciliacion
        cargarInfoConciliacionActual();
        cConciliacion c = App.Consultas.ConsultaConciliacionDetalle(corporativo, sucursal, año, mes, folio);
        if (c.CancelarConciliacion(usuario.IdUsuario))
        {
            App.ImplementadorMensajes.MostrarMensaje("CONCILIACIÓN CANCELADA EXITOSAMENTE");
            System.Threading.Thread.Sleep(3000);
            Response.Redirect("~/Inicio.aspx");
        }
        else
        {
            App.ImplementadorMensajes.MostrarMensaje("ERRORES AL CANCELAR LA CONCILIACIÓN");
        }
    }


    public void ocultarFiltroFechas(int tpConciliacion)
    {
        bool blVisble = tpConciliacion != 2;
        lblFOperacion.Visible =
            txtFOInicio.Visible =
            txtFOTermino.Visible =
            btnRangoFechasFO.Visible =
            rvFOInicio.Visible =
            rvFMTermino.Visible = blVisble;

        lblFMovimiento.Visible =
            txtFMInicio.Visible =
            txtFMTermino.Visible =
            btnRangoFechasFM.Visible =
            rvFMInicio.Visible =
            rvFMTermino.Visible = blVisble;

        lblFSuminstro.Visible =
            txtFSInicio.Visible =
            txtFSTermino.Visible =
            btnRangoFechasFS.Visible =
            rvFSInicio.Visible =
            rvFSTermino.Visible = !blVisble;

    }

    public void ocultarAgregarPedidoDirecto(int tpConciliacion)
    {
        lblPedidoDirecto.Visible = txtPedido.Visible = btnAgregarPedidoDirecto.Visible = tpConciliacion == 2;
    }

    public void FiltrarRangoFechasFO()
    {
        try
        {
            DataTable dt = (DataTable) HttpContext.Current.Session["TAB_INTERNOS"];
            DataView dv = new DataView(dt);

            string SearchExpression = String.Empty;
            if (!(String.IsNullOrEmpty(txtFOInicio.Text) || String.IsNullOrEmpty(txtFOTermino.Text)))
                SearchExpression = string.Format("FOperacion >= '{0}' AND FOperacion <= '{1}'", txtFOInicio.Text,
                    txtFOTermino.Text);
            if (dv.Count <= 0)
            {
                statusFiltro = false;
                Session["StatusFiltro"] = statusFiltro;
                tipoFiltro = String.Empty;
                Session["TipoFiltro"] = tipoFiltro;
                return;
            }

            dv.RowFilter = SearchExpression;
            HttpContext.Current.Session["TAB_INTERNOS_AX"] = dv.ToTable();
            grvInternos.DataSource = HttpContext.Current.Session["TAB_INTERNOS_AX"] as DataTable;
            grvInternos.DataBind();
            statusFiltro = true;
            Session["StatusFiltro"] = statusFiltro;
            tipoFiltro = "FO";
            Session["TipoFiltro"] = tipoFiltro;
          

        }
        catch (Exception ex)
        {
            throw ex;
           
        }
        finally
        {
            statusFiltro = false;
            Session["StatusFiltro"] = statusFiltro;
            tipoFiltro = String.Empty;
            Session["TipoFiltro"] = tipoFiltro;
        }
    }

    public void FiltrarRangoFechasFM()
    {
        try
        {
            DataTable dt = (DataTable) HttpContext.Current.Session["TAB_INTERNOS"];
            DataView dv = new DataView(dt);

            string SearchExpression = String.Empty;
            if (!(String.IsNullOrEmpty(txtFMInicio.Text) || String.IsNullOrEmpty(txtFMTermino.Text)))
            {
                SearchExpression = string.Format("FMovimiento >= '{0}' AND FMovimiento <= '{1}'", txtFMInicio.Text,
                    txtFMTermino.Text);
            }
            if (dv.Count <= 0)
            {
                statusFiltro = false;
                Session["StatusFiltro"] = statusFiltro;
                tipoFiltro = String.Empty;
                Session["TipoFiltro"] = tipoFiltro;
                return;
            }
            dv.RowFilter = SearchExpression;
            HttpContext.Current.Session["TAB_INTERNOS_AX"] = dv.ToTable();
            grvInternos.DataSource = HttpContext.Current.Session["TAB_INTERNOS_AX"] as DataTable;
            grvInternos.DataBind();
            statusFiltro = true;
            Session["StatusFiltro"] = statusFiltro;
            tipoFiltro = "FM";
            Session["TipoFiltro"] = tipoFiltro;
        }
        catch (Exception ex)
        {
            throw ex;
            
        }
        finally
        {
            statusFiltro = false;
            Session["StatusFiltro"] = statusFiltro;
            tipoFiltro = String.Empty;
            Session["TipoFiltro"] = tipoFiltro;
        }
    }

    public void FiltrarRangoFechasFS()
    {
        try
        {
            DataTable dt = (DataTable) HttpContext.Current.Session["TAB_INTERNOS"];
            DataView dv = new DataView(dt);

            string SearchExpression = String.Empty;
            if (!(String.IsNullOrEmpty(txtFSInicio.Text) || String.IsNullOrEmpty(txtFSTermino.Text)))
            {
                SearchExpression = string.Format("FSuministro >= '{0}' AND FSuministro <= '{1}'", txtFSInicio.Text,
                    txtFSTermino.Text);
            }
            if (dv.Count <= 0)
            {
                statusFiltro = false;
                Session["StatusFiltro"] = statusFiltro;
                tipoFiltro = String.Empty;
                Session["TipoFiltro"] = tipoFiltro;
                return;
            }
            dv.RowFilter = SearchExpression;
            HttpContext.Current.Session["TAB_INTERNOS_AX"] = dv.ToTable();
            grvPedidos.DataSource = HttpContext.Current.Session["TAB_INTERNOS_AX"] as DataTable;
            grvPedidos.DataBind();
            statusFiltro = true;
            Session["StatusFiltro"] = statusFiltro;
            tipoFiltro = "FS";
            Session["TipoFiltro"] = tipoFiltro;

        }
        catch (Exception ex)
        {
            throw ex;
        }

        finally
        {
            statusFiltro = false;
            Session["StatusFiltro"] = statusFiltro;
        }
    }

    protected void btnRangoFechasFO_Click(object sender, ImageClickEventArgs e)
    {
        FiltrarRangoFechasFO();
    }

    protected void btnRangoFechasFM_Click(object sender, ImageClickEventArgs e)
    {
        FiltrarRangoFechasFM();
    }

    protected void btnRangoFechasFS_Click(object sender, ImageClickEventArgs e)
    {
        FiltrarRangoFechasFS();
    }

    protected void btnAgregarPedidoDirecto_Click(object sender, ImageClickEventArgs e)
    {
        if (grvExternos.Rows.Count > 0)
        {
            //Leer Referencia Externa
            ReferenciaNoConciliada rce = leerReferenciaExternaSeleccionada();
            try
            {
                //Leer la InfoActual Conciliacion
                cargarInfoConciliacionActual();
                if (App.Consultas.ValidaPedidoEspecifico(rce.Corporativo, rce.Sucursal,
                    txtPedido.Text.Trim()))
                {
                    ReferenciaNoConciliadaPedido rncP = App.Consultas.ConsultaPedidoReferenciaEspecifico(corporativo,
                        sucursal, año, mes, folio, Convert.ToDecimal(txtDiferencia.Text), txtPedido.Text.Trim());
                    if (rncP != null)
                    {
                       
                        if (!rce.ExisteReferenciaConciliadaPedido(rncP.Pedido, rncP.CelulaPedido, rncP.AñoPedido))
                        {
                            agregarPedidoReferenciaExterna(rce, rncP);
                            ConsultarPedidosInternos();
                        }
                        else
                            App.ImplementadorMensajes.MostrarMensaje("El pedido ya fue agregado al Movimiento Externo");
                    }

                    else
                        App.ImplementadorMensajes.MostrarMensaje("Ocurrio algun error al leer el pedido. Consulte de nuevo.");
                }
                else
                {
                    App.ImplementadorMensajes.MostrarMensaje("El PedidoReferencia no se encuentran en Pedidos por Abonar");
                }
                //ReferenciaNoConciliadaPedido rncP =
                //        listaReferenciaPedidos.Where(
                //            rc => !rce.ExisteReferenciaConciliadaPedido(rc.Pedido, rc.CelulaPedido, rc.AñoPedido))
                //                              .Single(x => x.Pedido == Convert.ToInt32(txtPedido.Text));
                //App.Consultas.ConsultaPedido();
                //agregarPedidoReferenciaExterna(rce, rncP);
            }
            catch (Exception)
            {
                App.ImplementadorMensajes.MostrarMensaje("El pedido no se encuentra DISPONIBLE, o ya fue AGREGADO");
            }
            txtPedido.Text = String.Empty;
        }
        else
            App.ImplementadorMensajes.MostrarMensaje("NO EXISTEN NINGUNA TRANSACCION EXTERNA");
    }

    protected void btnAgregarPedido_Click(object sender, EventArgs e)
    {
        if (grvExternos.Rows.Count > 0)
        {
            Button btnAgregarPedido = sender as Button;
            GridViewRow gRowIn = (GridViewRow)(btnAgregarPedido).Parent.Parent;
            //Leer Referencia Externa
            ReferenciaNoConciliada rcp = leerReferenciaExternaSeleccionada();
            ReferenciaNoConciliadaPedido rncP = leerReferenciaPedidoSeleccionada(gRowIn.RowIndex);
            agregarPedidoReferenciaExterna(rcp, rncP);
        }
        else
            App.ImplementadorMensajes.MostrarMensaje("NO EXISTEN NINGUNA TRANSACCION EXTERNA");

    }

    public ReferenciaNoConciliadaPedido leerReferenciaPedidoSeleccionada(int rowIndex)
    {
        listaReferenciaPedidos = Session["POR_CONCILIAR_INTERNO"] as List<ReferenciaNoConciliadaPedido>;
        int pedido = Convert.ToInt32(grvPedidos.DataKeys[rowIndex].Values["Pedido"]);
        int celulaPedido = Convert.ToInt32(grvPedidos.DataKeys[rowIndex].Values["Celula"]);
        int añoPedido = Convert.ToInt32(grvPedidos.DataKeys[rowIndex].Values["AñoPed"]);
        return
            listaReferenciaPedidos.Single(
                s => s.Pedido == pedido && s.CelulaPedido == celulaPedido && s.AñoPedido == añoPedido);
    }

    public void agregarPedidoReferenciaExterna(ReferenciaNoConciliada rfExterna, ReferenciaNoConciliadaPedido rfPedido)
    {
        //Leer el tipoConciliacion URL
        tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

        if (!hdfExternosControl.Value.Equals("PENDIENTES"))
        {
            rfExterna.AgregarReferenciaConciliada(rfPedido);
            //Generar el GridView para las Referencias Internas(ARCHIVOS / PEDIDOS)
            GenerarTablaAgregadosArchivosInternos(rfExterna, tipoConciliacion);
            ActualizarTotalesAgregados();
            ConsultarPedidosInternos();
        }
        else
        {
            if (!rfExterna.StatusConciliacion.Equals("CONCILIACION CANCELADA"))
            {
                rfExterna.AgregarReferenciaConciliada(rfPedido);
                //Generar el GridView para las Referencias Internas(ARCHIVOS / PEDIDOS)
                GenerarTablaAgregadosArchivosInternos(rfExterna, tipoConciliacion);
                ActualizarTotalesAgregados();
                ConsultarPedidosInternos();
            }
            else
                App.ImplementadorMensajes.MostrarMensaje(
                    "NO SE PUEDE COMPLETAR LA ACCION \nLA REFERENCIA EXTERNA ESTA CANCELADA");
        }
    }

    protected void grvExternos_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            grvExternos.PageIndex = e.NewPageIndex;
            DataTable dtSortTable = HttpContext.Current.Session["TAB_EXTERNOS_AX"] as DataTable;
            grvExternos.DataSource = dtSortTable;
            grvExternos.DataBind();
            //Leer el tipoConciliacion URL
            tipoConciliacion = Convert.ToSByte(Request.QueryString["TipoConciliacion"]);

            ReferenciaNoConciliada rfEx = leerReferenciaExternaSeleccionada();
            //Limpiar Listas de Referencia de demas Externos
            LimpiarExternosReferencia(rfEx);
            if (tipoConciliacion == 2)
                ConsultarPedidosInternos();
            else
                ConsultarArchivosInternos();
            //Generar el GridView para las Referencias Internas(ARCHIVOS / PEDIDOS)
            GenerarTablaAgregadosArchivosInternos(rfEx, tipoConciliacion);
            ActualizarTotalesAgregados();
        }
        catch (Exception ex)
        {
            //App.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);
        }

    }

    protected void grvInternos_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            grvInternos.PageIndex = e.NewPageIndex;
            DataTable dtSortTable = HttpContext.Current.Session["TAB_INTERNOS_AX"] as DataTable;
            grvInternos.DataSource = dtSortTable;
            grvInternos.DataBind();
        }
        catch (Exception)
        {
        }
    }


    //------------------------------INICIO MODULO "AGREGAR NUEVO INTERNO"---------------------------------
    protected void imgImportar_Click(object sender, ImageClickEventArgs e)
    {

        limpiarVistaImportarInterno();
        enlazarComboFolioInterno();

        Carga_TipoFuenteInformacionInterno(Consultas.ConfiguracionTipoFuente.TipoFuenteInformacionInterno);

        LlenaGridViewFoliosAgregados();
        popUpImportarArchivos.Show();
    }
    public void limpiarVistaImportarInterno()
    {
        Session["NUEVOS_INTERNOS"] = null;
        Session.Remove("NUEVOS_INTERNOS");

        ddlTipoFuenteInfoInterno.SelectedIndex = ddlSucursalInterno.SelectedIndex = 0;
    }
    /// <summary>
    /// Llena el Combo de Tipo Fuente Informacion Externo e Interno
    /// </summary>
    public void Carga_TipoFuenteInformacionInterno(Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConfiguracionTipoFuente tipo)
    {
        try
        {

            listTipoFuenteInformacionExternoInterno = Conciliacion.RunTime.App.Consultas.ConsultaTipoInformacionDatos(tipo);
            this.ddlTipoFuenteInfoInterno.DataSource = listTipoFuenteInformacionExternoInterno;
            this.ddlTipoFuenteInfoInterno.DataValueField = "Identificador";
            this.ddlTipoFuenteInfoInterno.DataTextField = "Descripcion";
            this.ddlTipoFuenteInfoInterno.DataBind();
            this.ddlTipoFuenteInfoInterno.Dispose();
        }
        catch (SqlException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void ddlTipoFuenteInfoInterno_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Leer Variables URL 
        cargarInfoConciliacionActual();

        Carga_FoliosInternos(
                       corporativo,
                       Convert.ToInt32(ddlSucursalInterno.SelectedItem.Value),
                       año,
                       mes,
                       lblCuenta.Text,
                       Convert.ToSByte(ddlTipoFuenteInfoInterno.SelectedItem.Value)
                       );
        enlazarComboFolioInterno();

    }
    protected void ddlSucursalInterno_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Leer Variables URL 
        cargarInfoConciliacionActual();

        Carga_FoliosInternos(
                              corporativo,
                              Convert.ToInt32(ddlSucursalInterno.SelectedItem.Value),
                              año,
                              mes,
                              lblCuenta.Text,
                              Convert.ToSByte(ddlTipoFuenteInfoInterno.SelectedItem.Value)
                              );
        enlazarComboFolioInterno();
    }

    /// <summary>
    /// Consulta los Folios Internos según parametros de filtro. Agregar Interno
    /// </summary>
    public void Carga_FoliosInternos(int corporativo, int sucursal, int añoF, short mesF, string cuentabancaria, short tipofuenteinformacion)
    {
        try
        {
            listFoliosInterno = Conciliacion.RunTime.App.Consultas.ConsultaFoliosTablaDestino(corporativo, sucursal, añoF, mesF, cuentabancaria, tipofuenteinformacion);
            //HttpContext.Current.Session["listFoliosInterno"] = listFoliosInterno;
        }
        catch (Exception ex)
        {
            App.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);
        }
    }
    /// <summary>
    /// Llena el Combo de Folios Internos para Agregar Nuevo Interno
    /// </summary>
    public void enlazarComboFolioInterno()
    {
        this.ddlFolioInterno.DataSource = listFoliosInterno;
        this.ddlFolioInterno.DataValueField = "Identificador";
        this.ddlFolioInterno.DataTextField = "Descripcion";
        this.ddlFolioInterno.DataBind();
        this.ddlFolioInterno.Dispose();
    }
    protected void ddlFolioInterno_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Leer Variables URL 
        cargarInfoConciliacionActual();

        Carga_FoliosInternos(
                       corporativo,
                       Convert.ToInt32(ddlSucursalInterno.SelectedItem.Value),
                       año,
                       mes,
                       lblCuenta.Text,
                       Convert.ToSByte(ddlTipoFuenteInfoInterno.SelectedItem.Value)
                       );

        this.lblStatusFolioInterno.Text = listFoliosInterno[ddlFolioInterno.SelectedIndex].Campo1;
        this.lblUsuarioAltaEx.Text = listFoliosInterno[ddlFolioInterno.SelectedIndex].Campo3;

    }
    protected void ddlFolioInterno_DataBound(object sender, EventArgs e)
    {
        try
        {
            if (ddlFolioInterno.Items.Count <= 0)
            {
                lblUsuarioAltaEx.Text = lblStatusFolioInterno.Text = String.Empty;
                return;
            }
            this.lblStatusFolioInterno.Text = listFoliosInterno[ddlFolioInterno.SelectedIndex].Campo1;
            this.lblUsuarioAltaEx.Text = listFoliosInterno[ddlFolioInterno.SelectedIndex].Campo3;
        }
        catch (Exception)
        {
            throw;
        }

    }
    protected void btnAñadirFolio_Click(object sender, ImageClickEventArgs e)
    {
        listArchivosInternos = Session["NUEVOS_INTERNOS"] != null ?
                               Session["NUEVOS_INTERNOS"] as List<DatosArchivo> :
                               new List<DatosArchivo>();

        if (listArchivosInternos != null && listArchivosInternos.Exists(x => x.Folio == Convert.ToInt32(ddlFolioInterno.SelectedItem.Value)))
        {
            App.ImplementadorMensajes.MostrarMensaje("Este Folio Interno ya esta Agregado");
        }
        else
        {
            //Leer Variables URL 
            cargarInfoConciliacionActual();

            cConciliacion conciliacion = App.Consultas.ConsultaConciliacionDetalle(corporativo, sucursal, año, mes, folio);
            DatosArchivo datosArchivoInterno = App.DatosArchivo.CrearObjeto();//new DatosArchivoDatos(App.ImplementadorMensajes); //App.DatosArchivo

            datosArchivoInterno.FolioConciliacion = conciliacion.Folio;
            datosArchivoInterno.SucursalConciliacion = conciliacion.Sucursal;
            datosArchivoInterno.Folio = Convert.ToInt32(ddlFolioInterno.SelectedItem.Value);
            datosArchivoInterno.Corporativo = conciliacion.Corporativo;
            datosArchivoInterno.Sucursal = Convert.ToInt32(ddlSucursalInterno.SelectedItem.Value);
            datosArchivoInterno.Año = conciliacion.Año;
            datosArchivoInterno.MesConciliacion = conciliacion.Mes;
            listArchivosInternos.Add(datosArchivoInterno);
            //Guardar los arhivos interno que se estan agregando
            Session["NUEVOS_INTERNOS"] = listArchivosInternos;
            LlenaGridViewFoliosAgregados();
        }


    }
    /// <summary>
    /// Llena el GridView de Folios Internos Agregados
    /// </summary>
    private void LlenaGridViewFoliosAgregados()
    {
        listArchivosInternos = Session["NUEVOS_INTERNOS"] != null ?
                               Session["NUEVOS_INTERNOS"] as List<DatosArchivo> :
                               new List<DatosArchivo>();
        this.grvAgregados.DataSource = listArchivosInternos;
        this.grvAgregados.DataBind();
        //this.grvAgregados.Dispose();
    }
    public void activarImportacion(int tipoConciliacion)
    {
        if (tipoConciliacion == 2)
        {
            tdImportar.Attributes.Add("class", "iconoOpcion bg-color-grisClaro02");
            imgImportar.Enabled = false;
        }
    }
    protected void btnGuardarInterno_Click(object sender, EventArgs e)
    {
        bool resultado = false;
        listArchivosInternos = Session["NUEVOS_INTERNOS"] != null ?
                               Session["NUEVOS_INTERNOS"] as List<DatosArchivo> :
                               listArchivosInternos;

        if (listArchivosInternos != null && listArchivosInternos.Count > 0)
        {
            //Leer Variables URL 
            cargarInfoConciliacionActual();

            cConciliacion conciliacion = App.Consultas.ConsultaConciliacionDetalle(corporativo, sucursal, año, mes, folio);
            listArchivosInternos.ForEach(x => resultado = conciliacion.AgregarArchivo(x, cConciliacion.Operacion.Edicion));

            if (resultado)
            {
                //ACTUALIZAR GRID INTERNOS
                LlenarBarraEstado();
                ConsultarArchivosInternos();
                App.ImplementadorMensajes.MostrarMensaje("Agregado de Folios Internos exitoso.");

            }
            else
                App.ImplementadorMensajes.MostrarMensaje("Ocurrieron problemas al agregar el nuevo Folio");
            //Limpiar Remover Variable (Session) de Internos 
            limpiarVistaImportarInterno();

            popUpImportarArchivos.Hide();
            popUpImportarArchivos.Dispose();

        }
        else
        {
            App.ImplementadorMensajes.MostrarMensaje("No se ha agregado un nuevo Archivo Interno");
        }
    }
    protected void grvAgregados_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        listArchivosInternos = Session["NUEVOS_INTERNOS"] != null ?
                               Session["NUEVOS_INTERNOS"] as List<DatosArchivo> :
                               listArchivosInternos;

        int folioInterno = Convert.ToInt32(grvAgregados.DataKeys[e.RowIndex].Value);
        listArchivosInternos.RemoveAll(x => x.Folio == folioInterno);
        Session["NUEVOS_INTERNOS"] = listArchivosInternos;
        LlenaGridViewFoliosAgregados();
    }
    protected void grvAgregados_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try { grvAgregados.PageIndex = e.NewPageIndex; LlenaGridViewFoliosAgregados(); }
        catch (Exception ex) { }
    }
    protected void ddlTipoFuenteInfoInterno_DataBound(object sender, EventArgs e)
    {
        //Leer Variables URL 
        cargarInfoConciliacionActual();

        Carga_FoliosInternos(
                       corporativo,
                       Convert.ToInt32(ddlSucursalInterno.SelectedItem.Value),
                       año,
                       mes,
                       lblCuenta.Text,
                       Convert.ToSByte(ddlTipoFuenteInfoInterno.SelectedItem.Value)
                       );
        enlazarComboFolioInterno();
    }

    /// <summary>
    ///Consulta el detalle del Folio Interno
    /// </summary>
    public void Consulta_TablaDestinoDetalleInterno(Consultas.Configuracion configuracion, int empresa, int sucursal, int año, int folioInterno)
    {
        System.Data.SqlClient.SqlConnection Connection = SeguridadCB.Seguridad.Conexion;
        if (Connection.State == ConnectionState.Closed)
        {
            SeguridadCB.Seguridad.Conexion.Open();
            Connection = SeguridadCB.Seguridad.Conexion;
        }
        try
        {
            listaDestinoDetalleInterno = Conciliacion.RunTime.App.Consultas.ConsultaTablaDestinoDetalle(
                        configuracion,
                        empresa,
                        sucursal,
                        año,
                        folioInterno);
        }
        catch (Exception ex)
        {
            App.ImplementadorMensajes.MostrarMensaje("Error al Consultar Detalle.\r\nError" + ex.Message);
        }
    }
    /// <summary>
    /// Llena el VistaRapida[TablaDestinoDetalleExterno] x Folio
    /// </summary>
    public void GenerarTablaDestinoDetalleInterno()
    {
        tblDestinoDetalleInterno = new DataTable("DetalleInterno");
        tblDestinoDetalleInterno.Columns.Add("Folio", typeof(int));
        tblDestinoDetalleInterno.Columns.Add("FOperacion", typeof(DateTime));
        tblDestinoDetalleInterno.Columns.Add("FMovimiento", typeof(DateTime));
        tblDestinoDetalleInterno.Columns.Add("Referencia", typeof(string));
        tblDestinoDetalleInterno.Columns.Add("Descripcion", typeof(string));
        tblDestinoDetalleInterno.Columns.Add("Deposito", typeof(float));
        tblDestinoDetalleInterno.Columns.Add("Retiro", typeof(float));
        tblDestinoDetalleInterno.Columns.Add("Concepto", typeof(string));

        foreach (DatosArchivoDetalle da in listaDestinoDetalleInterno)
        {
            tblDestinoDetalleInterno.Rows.Add(
                da.Folio,
                da.FOperacion.ToShortDateString(),
                da.FMovimiento.ToShortDateString(),
                da.Referencia,
                da.Descripcion,
                da.Deposito,
                da.Retiro,
                da.Concepto);
        }
        HttpContext.Current.Session["DETALLEINTERNO"] = tblDestinoDetalleInterno;
    }
    /// <summary>
    ///Genera la tabla de destino detalle[Vista Rapida]
    ///  
    private void LlenaGridViewDestinoDetalleInterno()
    {
        DataTable tablaDestinoDetalleInterno = (DataTable)HttpContext.Current.Session["DETALLEINTERNO"];
        this.grvVistaRapidaInterno.DataSource = tblDestinoDetalleInterno;
        this.grvVistaRapidaInterno.DataBind();
    }
    protected void btnVerDatalleInterno_Click(object sender, ImageClickEventArgs e)
    {
        //Leer Variables URL 
        cargarInfoConciliacionActual();

        Consulta_TablaDestinoDetalleInterno(Consultas.Configuracion.Previo,
                                            corporativo,
                                            Convert.ToInt16(ddlSucursalInterno.SelectedItem.Value),
                                            año, Convert.ToInt32(ddlFolioInterno.SelectedItem.Value));
        GenerarTablaDestinoDetalleInterno();
        LlenaGridViewDestinoDetalleInterno();
        lblFolioInterno.Text = ddlFolioInterno.SelectedItem.Value;
        grvVistaRapidaInterno_ModalPopupExtender.Show();

    }

    //---FIN MODULO "AGREGAR NUEVO INTERNO"

    private void Limpiarpopup()
    {
        //lbCorporativo_.Text = string.Empty;
        //lbSucursal_.Text = string.Empty;
        txtFechaAplicacion.Text = string.Empty;
        txtDescripcion.Text = string.Empty;
        txtReferencia.Text = string.Empty;

    }

    //private void CargarDatadatepicker()
    //{
    //    string cadena = lbFMovimiento.Text.Replace('/', ',');
    //    ScriptManager.RegisterStartupScript(this.upUnoAVarios, upUnoAVarios.GetType(), "validador",
    //                                        "datapicker_modal(" + cadena + ");", true);

    //}

    //private void CargarDatadatepicker( )
    //{
    //ReferenciaNoConciliada rfEx = leerReferenciaExternaSeleccionada();
    //    string cadena = lbFMovimiento.Text == " "
    //                        ? lbFMovimiento.Text.Replace('/', ',')
    //: string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(rfEx.FMovimiento)).Replace('/', ',');

    //    ScriptManager.RegisterStartupScript(this.upUnoAVarios, upUnoAVarios.GetType(), "validador",
    //                                        "datapicker_modal(" + cadena + ");", true);
    //}

    private void CargarDatadatepicker(DateTime FMovimiento)
    {
        int diasAplicacion = int.Parse(parametros.ValorParametro(30, "DiasAplicacionTransf"));

        string cadenaMin = lbFMovimiento.Text != ""
                            ? lbFMovimiento.Text.Replace('/', ',')
                            : string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(FMovimiento)).Replace('/', ',');


         string cadenaMax = lbFMovimiento.Text != ""
                            ? string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(lbFMovimiento.Text).AddDays(diasAplicacion)).Replace('/', ',')
                            : string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(FMovimiento.AddDays(diasAplicacion))).Replace('/', ',');
        ;

        ScriptManager.RegisterStartupScript(this.upConciliar, upConciliar.GetType(), "validador",
                                            "datapicker_modal(" + cadenaMin +","+ cadenaMax + ");", true);

        
    }


    public void Carga_Corporativo()
    {

        cConciliacion c = App.Consultas.ConsultaConciliacionDetalle(corporativo, sucursal, año, mes, folio);
        try
        {
            listaCorporativoTransferencia =
                Conciliacion.RunTime.App.Consultas.ConsultaCorporativoTransferencia(
                    rdbVerDepositoRetiro.SelectedValue.Equals("DEPOSITOS") ? 0 : 1, Convert.ToInt32(c.Corporativo), Convert.ToInt32(c.Sucursal),
                    Convert.ToInt32(c.Banco), c.CuentaBancaria);
            this.cboCorporativo.DataSource = listaCorporativoTransferencia;
            this.cboCorporativo.DataValueField = "Identificador";
            this.cboCorporativo.DataTextField = "Descripcion";
            this.cboCorporativo.DataBind();
            this.cboCorporativo.Dispose();

        }
        catch (Exception ex)
        {

        }

    }

    private void Carga_Sucursal()
    {
        cConciliacion c = App.Consultas.ConsultaConciliacionDetalle(corporativo, sucursal, año, mes, folio);
        try
        {

            listaSucursalTransferencia =
                  Conciliacion.RunTime.App.Consultas.ConsultaSucursalTransferencia(
                      rdbVerDepositoRetiro.SelectedValue.Equals("DEPOSITOS") ? 2 : 3, Convert.ToInt32(c.Corporativo), Convert.ToInt32(c.Sucursal),
                        Convert.ToInt32(c.Banco), c.CuentaBancaria, Convert.ToInt32(cboCorporativo.SelectedItem.Value));
            this.cboSucursal.DataSource = listaSucursalTransferencia;
            this.cboSucursal.DataValueField = "Identificador";
            this.cboSucursal.DataTextField = "Descripcion";
            this.cboSucursal.DataBind();
            this.cboSucursal.Dispose();
        }
        catch (Exception ex)
        {
            this.cboSucursal.DataSource = new List<ListaCombo>();
            this.cboSucursal.DataBind();
            this.cboSucursal.Dispose();
        }
    }

    private void Carga_NombreBanco()
    {
        cConciliacion c = App.Consultas.ConsultaConciliacionDetalle(corporativo, sucursal, año, mes, folio);
        try
        {
            listaNombreBancoTransferencia =
                   Conciliacion.RunTime.App.Consultas.ConsultaNombreBancoTransferencia(
                       rdbVerDepositoRetiro.SelectedValue.Equals("DEPOSITOS") ? 4 : 5, Convert.ToInt32(c.Corporativo), Convert.ToInt32(c.Sucursal),
                       Convert.ToInt32(c.Banco), c.CuentaBancaria, Convert.ToInt32(cboCorporativo.SelectedItem.Value), Convert.ToInt32(cboSucursal.SelectedItem.Value));
            this.cboNombreBanco.DataSource = listaNombreBancoTransferencia;
            this.cboNombreBanco.DataValueField = "Identificador";
            this.cboNombreBanco.DataTextField = "Descripcion";
            this.cboNombreBanco.DataBind();
            this.cboNombreBanco.Dispose();
        }
        catch (Exception ex)
        {
            this.cboNombreBanco.DataSource = new List<ListaCombo>();
            this.cboNombreBanco.DataBind();
            this.cboNombreBanco.Dispose();
        }

    }

    private void Carga_CuentaBanco()
    {
        cConciliacion c = App.Consultas.ConsultaConciliacionDetalle(corporativo, sucursal, año, mes, folio);
        try
        {
            listaCuentaBancoTransferencia =
                Conciliacion.RunTime.App.Consultas.ConsultaCuentaBancoTransferencia(
                    rdbVerDepositoRetiro.SelectedValue.Equals("DEPOSITOS") ? 6 : 7, Convert.ToInt32(c.Corporativo), Convert.ToInt32(c.Sucursal),
                    Convert.ToInt32(c.Banco), c.CuentaBancaria, Convert.ToInt32(cboCorporativo.SelectedItem.Value), Convert.ToInt32(cboSucursal.SelectedItem.Value), Convert.ToInt32(cboNombreBanco.SelectedItem.Value));
            this.cboCuentaBanco.DataSource = listaCuentaBancoTransferencia;
            this.cboCuentaBanco.DataValueField = "Identificador";
            this.cboCuentaBanco.DataTextField = "Descripcion";
            this.cboCuentaBanco.DataBind();
            this.cboCuentaBanco.Dispose();
        }

        catch (Exception ex)
        {
            this.cboCuentaBanco.DataSource = new List<ListaCombo>();
            this.cboCuentaBanco.DataBind();
            this.cboCuentaBanco.Dispose();

            App.ImplementadorMensajes.MostrarMensaje(
                         "No es posible realizar un traspaso entre cuentas si el numero de la cuenta bancaria no está dado de alta en el Catalogo Cuenta Transferencia.");
        }
    }


    protected void btnAgregar_Click(object sender, ImageClickEventArgs e)
    {
        List<GridViewRow> rowSeleccionadas = filasSeleccionadasExternos();
        if (grvExternos.Rows.Count > 0 && rowSeleccionadas.Count > 0)
        {

            /*******************
             * Agrego: Santiago Mendoza Carlos Nirari
             * Fecha:01/08/2014
             * Decripcion: Se cambio la forma de seleccionar registros den el gridview
             ********************/

            List<GridViewRow> rowsSeleccionadosStatus = filasSeleccionadasExternos("CONCILIACION CANCELADA");
            //No se encontraron registros con status 'CONCILIACION CANCELADA'
            if (rowsSeleccionadosStatus.Count == 0)
            {
                Boolean resultado = false;
                cargarInfoConciliacionActual();
                cConciliacion c = App.Consultas.ConsultaConciliacionDetalle(corporativo, sucursal, año, mes, folio);

                foreach (GridViewRow row in rowSeleccionadas)
                {
                    resultado = Conciliacion.RunTime.App.Consultas.ObtieneExternosTransferencia(
                        Convert.ToSByte(grvExternos.DataKeys[row.RowIndex].Values["Corporativo"]),
                        Convert.ToSByte(grvExternos.DataKeys[row.RowIndex].Values["Sucursal"]),
                        Convert.ToInt32(grvExternos.DataKeys[row.RowIndex].Values["Año"]),
                        Convert.ToInt32(grvExternos.DataKeys[row.RowIndex].Values["Folio"]),
                        Convert.ToInt32(grvExternos.DataKeys[row.RowIndex].Values["Secuencia"]));

                    if (resultado)
                    {
                        break;
                    }
                }

                //No existen registros con ese filtrado
                if (resultado == false)
                {
                    if (rdbVerDepositoRetiro.SelectedValue.Equals("DEPOSITOS") &&
                        rowSeleccionadas.Min(
                            x =>
                            decimal.Parse((x.FindControl("lblDeposito") as Label).Text, NumberStyles.Currency,
                                          CultureInfo.GetCultureInfo("en-US"))) != 0
                        ||
                        rdbVerDepositoRetiro.SelectedValue.Equals("RETIROS") &&
                        rowSeleccionadas.Min(
                            x =>
                            decimal.Parse((x.FindControl("lblRetiro") as Label).Text, NumberStyles.Currency,
                                          CultureInfo.GetCultureInfo("en-US"))) != 0)
                    {
                        Limpiarpopup();

                        //Cargar Info Actual Conciliacion
                        lbCorporativo.Text = c.CorporativoDes;
                        lbSucursal.Text = c.SucursalDes;
                        lbNombreBanco.Text = c.BancoStr;
                        lbCuentaBanco.Text = c.CuentaBancaria;



                        txtAbono.Text = txtCargo.Text = rdbVerDepositoRetiro.SelectedValue.Equals("DEPOSITOS")
                                                            ? Convert.ToString(
                                                                rowSeleccionadas.Aggregate<GridViewRow, decimal>
                                                                    (0,
                                                                     (x, seleccionado) =>
                                                                     x +
                                                                     decimal.Parse(
                                                                         (seleccionado.FindControl("lblDeposito")
                                                                          as Label).Text, NumberStyles.Currency,
                                                                         CultureInfo.GetCultureInfo("en-US"))))
                                                            : Convert.ToString(
                                                                rowSeleccionadas.Aggregate<GridViewRow, decimal>
                                                                    (0,
                                                                     (x, seleccionado) =>
                                                                     x +
                                                                     decimal.Parse(
                                                                         (seleccionado.FindControl("lblRetiro")
                                                                          as Label).Text, NumberStyles.Currency,
                                                                         CultureInfo.GetCultureInfo("en-US"))));

                        string dateMax = string.Format("{0:dd/MM/yyyy}",
                                                       rowSeleccionadas.Max(
                                                           x =>
                                                           Convert.ToDateTime(
                                                               (x.FindControl("lblFMovimiento") as Label).Text)));
                        lbFMovimiento.Text = dateMax;

                        lbDireccion1.Text = rdbVerDepositoRetiro.SelectedValue.Equals("DEPOSITOS")
                                                ? "DESTINO"
                                                : "ORIGEN";
                        lbDireccion2.Text = rdbVerDepositoRetiro.SelectedValue.Equals("DEPOSITOS")
                                                ? "ORIGEN"
                                                : "DESTINO";
                        dateMin =
                            rowSeleccionadas.Min(
                                x => Convert.ToDateTime((x.FindControl("lblFMovimiento") as Label).Text));

                        Carga_Corporativo();
                        Carga_NombreBanco();
                        Carga_CuentaBanco();

                        CargarDatadatepicker(dateMin);
                        popUpAgregarTransfBancaria.Show();
                    }
                    else
                    {
                        App.ImplementadorMensajes.MostrarMensaje(
                            "No se posible realizar un traspaso entre cuentas sin un monto de retiro o deposito.");
                    }
                }
                else
                {

                    App.ImplementadorMensajes.MostrarMensaje(
                           "No se posible realizar un traspaso entre cuentas si un registro ya pertenece a alguna transferencia bancaria");
                }
            }
            else
            {
                App.ImplementadorMensajes.MostrarMensaje(
                "No se posible realizar un traspaso entre cuentas sobre un registro cancelado. Verifique.");
            }

        }
        else
        {

            App.ImplementadorMensajes.MostrarMensaje(
                    "No es posible realizar un traspaso entre cuentas si no se ha seleccionado una conciliación");
        }
    }

    /*protected void btnGuardar__Click(object sender, EventArgs e)
    {
        try
        {
            if (Convert.ToDecimal(txtCargo.Text) == Convert.ToDecimal(txtAbono.Text))
            {
                //Cargar Info Actual Conciliacion
                cargarInfoConciliacionActual();

                cConciliacion c = App.Consultas.ConsultaConciliacionDetalle(corporativo, sucursal, año, mes, folio);

                usuario = (SeguridadCB.Public.Usuario) HttpContext.Current.Session["Usuario"];
                TransferenciaBancarias tb = Conciliacion.RunTime.App.TransferenciaBancarias.CrearObjeto();
                TransferenciaBancariasDetalle tbd = Conciliacion.RunTime.App.TransferenciaBancariasDetalle.CrearObjeto();
                TransferenciaBancariaOrigen tbo = Conciliacion.RunTime.App.TransferenciaBancariaOrigen.CrearObjeto();

                tb.Corporativo = Convert.ToInt16(c.Corporativo);
                tb.Sucursal = Convert.ToInt16(c.Sucursal);

                tb.TipoTransferencia = (short) (rdbVerDepositoRetiro.SelectedValue.Equals("DEPOSITOS")
                    ? 3
                    : 2);

                tb.Año = c.Año;
                tb.Referencia = txtReferencia.Text;
                tb.FMovimiento = Convert.ToDateTime(lbFMovimiento.Text);
                tb.FAplicacion = Convert.ToDateTime(txtFechaAplicacion.Text);
                tb.UsuarioCaptura = usuario.IdUsuario;
                tb.Status = "CAPTURADA";
                tb.Descripcion = txtDescripcion.Text;
                if (tb.Registrar())
                {

                    //Datos Llaves Primarias
                    tbd.Corporativo = tb.Corporativo;
                    tbd.Sucursal = tb.Sucursal;
                    tbd.Año = tb.Año;
                    tbd.Folio = tb.Folio;



                    tbd.CorporativoDeatalle = Convert.ToInt16(c.Corporativo);
                    tbd.SucursalDetalle = Convert.ToInt16(c.Sucursal);
                    tbd.CuentaBanco = c.CuentaBancaria;

                    tbd.Entrada = rdbVerDepositoRetiro.SelectedValue == "DEPOSITOS"
                        ? Convert.ToInt16(1)
                        : Convert.ToInt16(0);

                    tbd.Cargo = rdbVerDepositoRetiro.SelectedValue == "DEPOSITOS"
                        ? 0
                        : decimal.Parse(txtCargo.Text, NumberStyles.Currency);

                    tbd.Abono = rdbVerDepositoRetiro.SelectedValue == "DEPOSITOS"
                        ? decimal.Parse(txtCargo.Text, NumberStyles.Currency)
                        : 0;

                    if (tbd.Registrar())
                    {
                        tbd.CorporativoDeatalle = Convert.ToInt16(cboCorporativo.SelectedItem.Value);
                        tbd.SucursalDetalle = Convert.ToInt16(cboSucursal.SelectedItem.Value);
                        tbd.CuentaBanco = cboCuentaBanco.SelectedItem.Text;


                        tbd.Entrada = rdbVerDepositoRetiro.SelectedValue == "DEPOSITOS"
                            ? Convert.ToInt16(0)
                            : Convert.ToInt16(1);

                        tbd.Cargo = rdbVerDepositoRetiro.SelectedValue == "DEPOSITOS"
                            ? decimal.Parse(txtCargo.Text, NumberStyles.Currency)
                            : 0;
                        tbd.Abono = rdbVerDepositoRetiro.SelectedValue == "DEPOSITOS"
                            ? 0
                            : decimal.Parse(txtCargo.Text, NumberStyles.Currency);
                        if (tbd.Registrar())
                        {
                            //Inserccion en la tabla TransferenciaBancariaOrigen
                            List<GridViewRow> rowSeleccionadas = filasSeleccionadasExternos();
                            foreach (GridViewRow row in rowSeleccionadas)
                            {
                                tbo.CorporativoTD =
                                    Convert.ToSByte(grvExternos.DataKeys[row.RowIndex].Values["Corporativo"]);
                                tbo.SucursalTD =
                                    Convert.ToSByte(grvExternos.DataKeys[row.RowIndex].Values["Sucursal"]);
                                tbo.AñoTD =
                                    Convert.ToInt32(grvExternos.DataKeys[row.RowIndex].Values["Año"]);
                                tbo.FolioTD =
                                    Convert.ToInt32(grvExternos.DataKeys[row.RowIndex].Values["Folio"]);
                                tbo.SecuenciaTD =
                                    Convert.ToInt32(grvExternos.DataKeys[row.RowIndex].Values["Secuencia"]);

                                tbo.Corporativo = Convert.ToInt16(c.Corporativo);
                                tbo.Sucursal = Convert.ToInt16(c.Sucursal);
                                tbo.Año = c.Año;
                                tbo.Folio = tb.Folio;
                                tbo.Registrar();

                            }
                            popUpAgregarTransfBancaria.Hide();
                            popUpAgregarTransfBancaria.Dispose();

                        }
                    }

                }
            }
            else
            {
                App.ImplementadorMensajes.MostrarMensaje(
                    "Las cantidades de el cargo y el abono deben de ser iguales");
            }
        }
        catch (Exception ex)
        {
            App.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);
        }

    }*/


    protected void btnGuardar__Click(object sender, EventArgs e)
    {
        try
        {
            if (Convert.ToDecimal(txtCargo.Text) == Convert.ToDecimal(txtAbono.Text))
            {
                //Cargar Info Actual Conciliacion
                cargarInfoConciliacionActual();

                cConciliacion c = App.Consultas.ConsultaConciliacionDetalle(corporativo, sucursal, año, mes, folio);

                usuario = (SeguridadCB.Public.Usuario)HttpContext.Current.Session["Usuario"];
                TransferenciaBancarias tb = Conciliacion.RunTime.App.TransferenciaBancarias.CrearObjeto();
               

                /*TransferenciaBancarias*/
                tb.Corporativo = Convert.ToInt16(c.Corporativo);
                tb.Sucursal = Convert.ToInt16(c.Sucursal);

                tb.TipoTransferencia = (short)(rdbVerDepositoRetiro.SelectedValue.Equals("DEPOSITOS")
                    ? 3
                    : 2);

                tb.Año = c.Año;
                tb.Referencia = txtReferencia.Text;
                tb.FMovimiento = Convert.ToDateTime(lbFMovimiento.Text);
                tb.FAplicacion = Convert.ToDateTime(txtFechaAplicacion.Text);
                tb.UsuarioCaptura = usuario.IdUsuario;
                tb.Status = "CAPTURADA";
                tb.Descripcion = txtDescripcion.Text;


                /*TransferenciaBancariasDetalle*/

                TransferenciaBancariasDetalle tbd = Conciliacion.RunTime.App.TransferenciaBancariasDetalle.CrearObjeto();

                tbd.Corporativo = tb.Corporativo;
                tbd.Sucursal = tb.Sucursal;
                tbd.Año = tb.Año;
                //tb.TransferenciaBancariasDetalle.Folio = tb.Folio;

                tbd.CorporativoDeatalle = Convert.ToInt16(c.Corporativo);
                tbd.SucursalDetalle = Convert.ToInt16(c.Sucursal);
                tbd.CuentaBanco = c.CuentaBancaria;

                tbd.Entrada = rdbVerDepositoRetiro.SelectedValue == "DEPOSITOS"
                    ? Convert.ToInt16(1)
                    : Convert.ToInt16(0);

                tbd.Cargo = rdbVerDepositoRetiro.SelectedValue == "DEPOSITOS"
                    ? 0
                    : decimal.Parse(txtCargo.Text, NumberStyles.Currency);

                tbd.Abono = rdbVerDepositoRetiro.SelectedValue == "DEPOSITOS"
                    ? decimal.Parse(txtCargo.Text, NumberStyles.Currency)
                    : 0;

                tb.ListTransferenciaBancariasDetalle.Add(tbd);

                /*TransferenciaBancariasDetalle*/

                tbd = Conciliacion.RunTime.App.TransferenciaBancariasDetalle.CrearObjeto();

                tbd.Corporativo = tb.Corporativo;
                tbd.Sucursal = tb.Sucursal;
                tbd.Año = tb.Año;

                tbd.CorporativoDeatalle = Convert.ToInt16(cboCorporativo.SelectedItem.Value);
                tbd.SucursalDetalle = Convert.ToInt16(cboSucursal.SelectedItem.Value);
                tbd.CuentaBanco = cboCuentaBanco.SelectedItem.Text;


                tbd.Entrada = rdbVerDepositoRetiro.SelectedValue == "DEPOSITOS"
                    ? Convert.ToInt16(0)
                    : Convert.ToInt16(1);

                tbd.Cargo = rdbVerDepositoRetiro.SelectedValue == "DEPOSITOS"
                    ? decimal.Parse(txtCargo.Text, NumberStyles.Currency)
                    : 0;
                tbd.Abono = rdbVerDepositoRetiro.SelectedValue == "DEPOSITOS"
                    ? 0
                    : decimal.Parse(txtCargo.Text, NumberStyles.Currency);

                tb.ListTransferenciaBancariasDetalle.Add(tbd);

                /*TransferenciaBancariaOrigen*/

                tb.ListTransferenciaBancariaOrigen= new List<TransferenciaBancariaOrigen>();

                List<GridViewRow> rowSeleccionadas = filasSeleccionadasExternos();
                foreach (GridViewRow row in rowSeleccionadas)
                {
                    TransferenciaBancariaOrigen tbo = Conciliacion.RunTime.App.TransferenciaBancariaOrigen.CrearObjeto();

                    tbo.CorporativoTD =
                        Convert.ToSByte(grvExternos.DataKeys[row.RowIndex].Values["Corporativo"]);
                    tbo.SucursalTD =
                        Convert.ToSByte(grvExternos.DataKeys[row.RowIndex].Values["Sucursal"]);
                    tbo.AñoTD =
                        Convert.ToInt32(grvExternos.DataKeys[row.RowIndex].Values["Año"]);
                    tbo.FolioTD =
                        Convert.ToInt32(grvExternos.DataKeys[row.RowIndex].Values["Folio"]);
                    tbo.SecuenciaTD =
                        Convert.ToInt32(grvExternos.DataKeys[row.RowIndex].Values["Secuencia"]);

                    tbo.Corporativo = Convert.ToInt16(c.Corporativo);
                    tbo.Sucursal = Convert.ToInt16(c.Sucursal);
                    tbo.Año = c.Año;
                    //tbo.Folio = tb.Folio;

                    tb.ListTransferenciaBancariaOrigen.Add(tbo);
                }

                tb.Guardar();
                popUpAgregarTransfBancaria.Hide();
                popUpAgregarTransfBancaria.Dispose();

            }
            else
            {
                App.ImplementadorMensajes.MostrarMensaje(
                    "Las cantidades de el cargo y el abono deben de ser iguales");
            }
        }
        catch (Exception ex)
        {
            App.ImplementadorMensajes.MostrarMensaje("Error:\n" + ex.Message);
        }

    }

    protected void img_cerrarTransfbancaria_Click(object sender, ImageClickEventArgs e)
    {
        popUpAgregarTransfBancaria.Hide();
    }


    protected void cboCorporativo_DataBound(object sender, EventArgs e)
    {
        CargarDatadatepicker(dateMin);
        Carga_Sucursal();
    }
    protected void cboCorporativo_SelectedIndexChanged(object sender, EventArgs e)
    {
        CargarDatadatepicker(dateMin);
        Carga_Sucursal();

    }
    protected void cboSucursal_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Cargar Info Actual Conciliacion
        cargarInfoConciliacionActual();

        CargarDatadatepicker(dateMin);
        Carga_NombreBanco();
        Carga_CuentaBanco();
    }
    protected void cboNombreBanco_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Cargar Info Actual Conciliacion
        cargarInfoConciliacionActual();
        CargarDatadatepicker(dateMin);
        Carga_CuentaBanco();

    }

    protected void cboCuentaBanco_SelectedIndexChanged(object sender, EventArgs e)
    {
        CargarDatadatepicker(dateMin);
    }
}