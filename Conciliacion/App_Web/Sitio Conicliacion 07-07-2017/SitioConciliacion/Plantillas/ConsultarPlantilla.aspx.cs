﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using SeguridadCB.Public;
using Conciliacion.RunTime.ReglasDeNegocio;

public partial class Plantillas_ConsultarPlantilla: System.Web.UI.Page
{
    #region "Propiedades Globales"

    private SeguridadCB.Public.Usuario usuario;
    #endregion
    #region "Propiedades privadas"
    private List<ListaCombo> listSucursales = new List<ListaCombo>();
    private List<ListaCombo> listTipoFuenteInformacion = new List<ListaCombo>();
    private List<ListaCombo> listBancos = new List<ListaCombo>();
    private List<ListaCombo> listCuentaBancaria = new List<ListaCombo>();
    private List<cFuenteInformacion> listFuenteInformacion = new List<cFuenteInformacion>();
    private DataTable tblFuenteInformacion = new DataTable("FuenteInformacion");
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
      
        try
        {
            Conciliacion.RunTime.App.ImplementadorMensajes.ContenedorActual = this;
            if (!IsPostBack)
            {
                usuario = (SeguridadCB.Public.Usuario)HttpContext.Current.Session["Usuario"];
                if (ddlEmpresa.Items.Count == 0) Carga_Corporativo();
                Carga_TipoFuenteInformacion();
                Carga_Banco(Convert.ToInt32(ddlEmpresa.SelectedItem.Value));

                this.grvFuenteInformacion.DataSource = tblFuenteInformacion;
                this.grvFuenteInformacion.DataBind();

                Consulta_FuenteInformacion();
                GenerarTablaFuenteInformacion();
            }
            LlenaGridViewFuenteInformacion();
            this.ddlEmpresa.Focus();
        }
        catch (Exception)
        {
        }
        finally
        {
        }
    }
    #region "Funciones privadas"
    /// <summary>
    /// Llena el Combo de las Empresas por Usuario
    /// </summary>
    public void Carga_Corporativo()
    {
        try
        {
            DataTable dtEmpresas = new DataTable();
            Usuario usuario;
            usuario = (Usuario)HttpContext.Current.Session["Usuario"];
            dtEmpresas = usuario.CorporativoAcceso;
            this.ddlEmpresa.DataSource = dtEmpresas;
            this.ddlEmpresa.DataValueField = "Corporativo";
            this.ddlEmpresa.DataTextField = "NombreCorporativo";
            this.ddlEmpresa.DataBind();
            dtEmpresas.Dispose();

        }
        catch
        {
        }
    }
    /// <summary>
    /// Llena el Combo de Sucurales según Corporativo
    /// </summary>
    public void Carga_SucursalCorporativo(int corporativo)
    {
        try
        {
            listSucursales = Conciliacion.RunTime.App.Consultas.ConsultaSucursales(Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConfiguracionIden0.Con0,corporativo);
            this.ddlSucursal.DataSource = listSucursales;
            this.ddlSucursal.DataValueField = "Identificador";
            this.ddlSucursal.DataTextField = "Descripcion";
            this.ddlSucursal.DataBind();
            this.ddlSucursal.Dispose();
        }
        catch
        {
        }
    }
    public void Carga_TipoFuenteInformacion()
    {
        try
        {

            listTipoFuenteInformacion = Conciliacion.RunTime.App.Consultas.ConsultaTipoInformacionDatos(Conciliacion.RunTime.ReglasDeNegocio.Consultas.ConfiguracionTipoFuente.TipoFuenteInformacion);
            this.ddlTipoFuenteInformacion.DataSource = listTipoFuenteInformacion;
            this.ddlTipoFuenteInformacion.DataValueField = "Identificador";
            this.ddlTipoFuenteInformacion.DataTextField = "Descripcion";
            this.ddlTipoFuenteInformacion.DataBind();
            this.ddlTipoFuenteInformacion.Dispose();
        }
        catch
        {
        }
    }
    /// <summary>
    /// Llena el Combo de Bancos
    /// </summary>
    public void Carga_Banco(int corporativo)
    {
        try
        {

            listBancos = Conciliacion.RunTime.App.Consultas.ConsultaBancos(corporativo);
            this.ddlBanco.DataSource = listBancos;
            this.ddlBanco.DataValueField = "Identificador";
            this.ddlBanco.DataTextField = "Descripcion";
            this.ddlBanco.DataBind();
            this.ddlBanco.Dispose();
        }
        catch
        {
        }
    }
    /// <summary>
    /// Llena el Combo de Cuenta Bancaria
    /// </summary>
    public void Carga_CuentaBancaria(int corporativo, short banco)
    {
        try
        {

            listCuentaBancaria = Conciliacion.RunTime.App.Consultas.ConsultaCuentasBancaria(corporativo, banco);
            this.ddlCuentaBancaria.DataSource = listCuentaBancaria;
            this.ddlCuentaBancaria.DataValueField = "Identificador";
            this.ddlCuentaBancaria.DataTextField = "Descripcion";
            this.ddlCuentaBancaria.DataBind();
            this.ddlCuentaBancaria.Dispose();
        }
        catch
        {
        }
    }

    public void GenerarTablaFuenteInformacion()//Genera la tabla de FuenteInformacion
    {
        tblFuenteInformacion.Columns.Add("FuenteInformacion", typeof(int));
        tblFuenteInformacion.Columns.Add("RutaArchivo", typeof(string));
        tblFuenteInformacion.Columns.Add("CuentaBancoFinanciero", typeof(string));
        tblFuenteInformacion.Columns.Add("TipoFuenteInformacion", typeof(Int16));
        tblFuenteInformacion.Columns.Add("TipoFuenteInformacionDes", typeof(string));
        tblFuenteInformacion.Columns.Add("TipoFuente", typeof(Int16));
        tblFuenteInformacion.Columns.Add("TipoFuenteDes", typeof(string));
        tblFuenteInformacion.Columns.Add("TipoArchivo", typeof(Int16));
        tblFuenteInformacion.Columns.Add("TipoArchivoDes", typeof(string));

        foreach (cFuenteInformacion cf in listFuenteInformacion)
        {
            tblFuenteInformacion.Rows.Add(cf.FuenteInformacion,
                cf.RutaArchivo,
                cf.CuentaBancoFinanciero,
                cf.TipoFuenteInformacion,
                cf.TipoFuenteInformacionDes,
                cf.TipoFuente,
                cf.TipoFuenteDes,
                cf.TipoArchivo,
                cf.TipoArchivoDes);
        }
        HttpContext.Current.Session["TABLAFUENTEINFOR"] = tblFuenteInformacion;
    }
    private void LlenaGridViewFuenteInformacion()//Llena el gridview con las Fuentes de Información Leidas
    {
        DataTable tablaFuenteInformacion = (DataTable)HttpContext.Current.Session["TABLAFUENTEINFOR"];
        grvFuenteInformacion.DataSource = tablaFuenteInformacion;
        grvFuenteInformacion.DataBind();
    }
    public void Consulta_FuenteInformacion()
    {
        System.Data.SqlClient.SqlConnection Connection = SeguridadCB.Seguridad.Conexion;
        if (Connection.State == ConnectionState.Closed)
        {
            SeguridadCB.Seguridad.Conexion.Open();
            Connection = SeguridadCB.Seguridad.Conexion;
        }
        try
        {
            listFuenteInformacion = Conciliacion.RunTime.App.Consultas.ConsultaFuenteInformacion(
                        Convert.ToInt32(ddlEmpresa.SelectedItem.Value),
                        Convert.ToInt32(ddlSucursal.SelectedItem.Value),
                        Convert.ToString(this.ddlCuentaBancaria.SelectedItem.Text),
                        Convert.ToSByte(this.ddlTipoFuenteInformacion.SelectedItem.Value));
        }
        catch (Exception)
        {

        }
        finally
        {

        }


    }

    #endregion
    protected void ddlEmpresa_DataBound(object sender, EventArgs e)
    {
        Carga_SucursalCorporativo(Convert.ToInt32(ddlEmpresa.SelectedItem.Value));
    }
    protected void ddlEmpresa_SelectedIndexChanged(object sender, EventArgs e)
    {
        Carga_SucursalCorporativo(Convert.ToInt32(ddlEmpresa.SelectedItem.Value));
    }
    protected void ddlSucursal_SelectedIndexChanged(object sender, EventArgs e)
    {
        Carga_Banco(Convert.ToInt32(ddlEmpresa.SelectedItem.Value));
    }
    protected void ddlBanco_DataBound(object sender, EventArgs e)
    {
        Carga_CuentaBancaria(Convert.ToInt32(ddlEmpresa.SelectedItem.Value), Convert.ToSByte(ddlBanco.SelectedItem.Value));
    }
    protected void ddlBanco_SelectedIndexChanged(object sender, EventArgs e)
    {
        Carga_CuentaBancaria(Convert.ToInt32(ddlEmpresa.SelectedItem.Value), Convert.ToSByte(ddlBanco.SelectedItem.Value));
    }
    protected void ddlSucursal_DataBound(object sender, EventArgs e)
    {
        Carga_Banco(Convert.ToInt32(ddlEmpresa.SelectedItem.Value));
    }
    protected void btnConsultar_Click(object sender, EventArgs e)
    {

    }
}