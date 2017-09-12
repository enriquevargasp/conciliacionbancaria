﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Conciliacion.RunTime.ReglasDeNegocio;
//using System.Data.SqlClient;
//using System.Diagnostics;

//namespace Conciliacion.RunTime.DatosSQL
//{
//    public class ConsultaDatos : Consultas
//    {

//        public ConsultaDatos(IMensajesImplementacion implementadorMensajes)
//            : base(implementadorMensajes)
//        {
//        }

//        public override Consultas CrearObjeto()
//        {
//            return new ConsultaDatos(App.ImplementadorMensajes);
//        }

//        //public override List<ReferenciaNoConciliada> ObtieneReferenciasPorConciliacion(int corporativo, int sucursal, int año, short mes, int folio)
//        //{
//        //    List<ReferenciaNoConciliada> referencias = new List<ReferenciaNoConciliada>();

//        //    using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//        //    {
//        //        cnn.Open();
//        //        SqlCommand comando = new SqlCommand("spCBConsultaReferenciasPorConciliacion", cnn);
//        //        comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.Int).Value = 0;
//        //        comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.Int).Value = corporativo;
//        //        comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.Int).Value = sucursal;
//        //        comando.Parameters.Add("@Año", System.Data.SqlDbType.Int).Value = año;
//        //        comando.Parameters.Add("@Mes", System.Data.SqlDbType.Int).Value = mes;
//        //        comando.Parameters.Add("@Folio", System.Data.SqlDbType.Int).Value = folio;

//        //        comando.CommandType = System.Data.CommandType.StoredProcedure;
//        //        SqlDataReader reader = comando.ExecuteReader();
//        //        while (reader.Read())
//        //        {

//        //            ReferenciaNoConciliada referencia = new ReferenciaNoConciliadaDatos(Convert.ToInt32(reader["Corporativo"]), Convert.ToInt32(reader["Sucursal"]), Convert.ToInt32(reader["Año"]),
//        //            Convert.ToInt32(reader["Folio"]), Convert.ToInt32(reader["Secuencia"]), reader["Concepto"].ToString(), Convert.ToDecimal(reader["MontoConciliado"]), Convert.ToDecimal(reader["Diferencia"]),
//        //            Convert.ToDecimal(reader["Monto"]), Convert.ToInt16(reader["FormaConciliacion"]), Convert.ToInt16(reader["StatusConcepto"]), reader["statusconciliacion"].ToString(),
//        //            App.ImplementadorMensajes);

//        //            referencias.Add(referencia);
//        //        }
//        //        return referencias;
//        //    }
//        //}

//        public override DatosArchivo ObtieneArchivoExterno(int corporativo, int sucursal, int año, short mes, int folio)
//        {
//            DatosArchivo archivo = new DatosArchivoDatos(this.implementadorMensajes);
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaArchivoExterno", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.Int).Value = 0;
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.Int).Value = corporativo;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.Int).Value = sucursal;
//                    comando.Parameters.Add("@Año", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@Mes", System.Data.SqlDbType.Int).Value = mes;
//                    comando.Parameters.Add("@Folio", System.Data.SqlDbType.Int).Value = folio;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        archivo.Corporativo = Convert.ToInt32(reader["corporativo"]);
//                        archivo.Sucursal = Convert.ToInt32(reader["sucursal"]);
//                        archivo.Año = Convert.ToInt32(reader["año"]);
//                        archivo.Folio = Convert.ToInt32(reader["folio"]);
//                        archivo.CuentaBanco = reader["cuentabanco"].ToString();
//                        archivo.TipoFuenteInformacion = Convert.ToInt16(reader["tipofuenteinformacion"]);
//                        archivo.StatusConciliacion = reader["statusconciliacion"].ToString();
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//            }
//            return archivo;
//        }

//        public override List<cConciliacion> ConsultaConciliacion(int corporativo, int sucursal, int grupoconciliacion,
//                                                                 int año, short mes, short tipoconciliacion,
//                                                                 string statusconciliacion, string usuario)
//        {
//            List<cConciliacion> conciliaciones = new List<cConciliacion>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaConciliacion", cnn);
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.TinyInt).Value = corporativo;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@MesConciliacion ", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@TipoConciliacion ", System.Data.SqlDbType.SmallInt).Value =
//                        tipoconciliacion;
//                    comando.Parameters.Add("@StatusConciliacion", System.Data.SqlDbType.VarChar).Value =
//                        statusconciliacion;
//                    comando.Parameters.Add("@Usuario", System.Data.SqlDbType.VarChar).Value = usuario;
//                    comando.Parameters.Add("@GrupoConciliacion", System.Data.SqlDbType.SmallInt).Value =
//                        grupoconciliacion;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        cConciliacion conciliacion =
//                            new ConciliacionDatos(Convert.ToInt32(reader["CorporativoConciliacion"]),
//                                                  Convert.ToInt32(reader["SucursalConciliacion"]),
//                                                  Convert.ToInt32(reader["AñoConciliacion"]),
//                                                  Convert.ToInt16(reader["MesConciliacion"]),
//                                                  Convert.ToInt32(reader["FolioConciliacion"]),
//                                                  Convert.ToDateTime(reader["FInicial"]),
//                                                  Convert.ToDateTime(reader["FFinal"]),
//                                                  Convert.ToString(reader["StatusConciliacion"]),
//                                                  Convert.ToInt32(reader["GrupoConciliacion"]),
//                                                  Convert.ToInt16(reader["TipoConciliacion"]),
//                                                  Convert.ToInt32(reader["TransaccionesInternas"]),
//                                                  Convert.ToInt32(reader["ConciliadasInternas"]),
//                                                  Convert.ToInt32(reader["TransaccionesExternas"]), 0, 0,
//                                                  Convert.ToInt32(reader["ConciliadasExternas"]),
//                                                  Convert.ToString(reader["GrupoConciliacionstr"]),
//                                                  Convert.ToBoolean(reader["AccesoTotal"]),
//                                                  Convert.ToString(reader["CuentaBancoFinanciero"]),
//                                                  Convert.ToString(reader["CorporativoDes"]),
//                                                  Convert.ToString(reader["SucursalDes"]),
//                                                  Convert.ToString(reader["BancoStr"]),
//                                                  Convert.ToString(reader["UbicacionIcono"]),
//                                                  this.implementadorMensajes);

//                        conciliaciones.Add(conciliacion);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return conciliaciones;
//            }
//        }

//        public override List<ListaCombo> ConsultaAños()
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargarComboAño", cnn);

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaSucursales(ConfiguracionIden0 configuracion, int corporativo)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargarComboSucursal", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.TinyInt).Value = corporativo;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]),
//                                                         Convert.ToString(reader["Siglas"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaGruposConciliacion(ConfiguracionGrupo configuracion, string usuario)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargarComboGrupoConciliacion", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@Usuario", System.Data.SqlDbType.VarChar).Value = usuario;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaTipoConciliacion(ConfiguracionGrupo configuracion, string usuario)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargarComboTipoConciliacion", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@Usuario", System.Data.SqlDbType.VarChar).Value = usuario;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]),
//                                                         Convert.ToString(reader["Campo1"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaStatusConciliacion()
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargarComboStatusConciliacion", cnn);

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaTipoInformacionDatos(ConfiguracionTipoFuente configuracion)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargaComboTipoFuente", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]),
//                                                         Convert.ToString(reader["Campo1"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaBancos(int corporativo)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaBanco", cnn);
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.TinyInt).Value = corporativo;
//                    //comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.TinyInt).Value = sucursal;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]),
//                                                         Convert.ToString(reader["Campo1"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaCuentasBancaria(int corporativo, short banco)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaCuentaBanco", cnn);
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.TinyInt).Value = corporativo;
//                    //comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@Banco", System.Data.SqlDbType.SmallInt).Value = banco;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]),
//                                                         Convert.ToString(reader["Campo1"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaFoliosTablaDestino(int corporativo, int sucursal, int año, short mes,
//                                                                    string cuentabancaria, short tipofuenteinformacion)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargaComboTablaDestino", cnn);
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.TinyInt).Value = corporativo;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@Año", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@Mes", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@CuentaBancaria ", System.Data.SqlDbType.VarChar).Value = cuentabancaria;
//                    comando.Parameters.Add("@TipoFuenteInformacion", System.Data.SqlDbType.SmallInt).Value =
//                        tipofuenteinformacion;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]),
//                                                         Convert.ToString(reader["Campo1"]),
//                                                         Convert.ToString(reader["Campo2"]),
//                                                         Convert.ToString(reader["Campo3"]), "");
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaDestinoExterno(short tipoconciliacion)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargarComboDestino", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = 0;
//                    comando.Parameters.Add("@TipoConciliacion", System.Data.SqlDbType.SmallInt).Value = tipoconciliacion;
//                    comando.Parameters.Add("@CampoExterno", System.Data.SqlDbType.VarChar).Value = "";

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]),
//                                                         Convert.ToString(reader["Campo1"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaDestinoInterno(short tipoconciliacion, string campoexterno)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargarComboDestino", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = 1;
//                    comando.Parameters.Add("@TipoConciliacion", System.Data.SqlDbType.SmallInt).Value = tipoconciliacion;
//                    comando.Parameters.Add("@CampoExterno", System.Data.SqlDbType.VarChar).Value = campoexterno;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]),
//                                                         Convert.ToString(reader["Campo1"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaDestino()
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargarComboDestino", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = 2;
//                    comando.Parameters.Add("@TipoConciliacion", System.Data.SqlDbType.SmallInt).Value = 0;
//                    comando.Parameters.Add("@CampoExterno", System.Data.SqlDbType.VarChar).Value = "";

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]),
//                                                         Convert.ToString(reader["Campo1"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaDestinoPedido()
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargarComboDestino", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = 3;
//                    comando.Parameters.Add("@TipoConciliacion", System.Data.SqlDbType.SmallInt).Value = 0;
//                    comando.Parameters.Add("@CampoExterno", System.Data.SqlDbType.VarChar).Value = "";

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]),
//                                                         Convert.ToString(reader["Campo1"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaFormaConciliacion(short tipoconciliacion)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargarComboFormaConciliacion", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = tipoconciliacion;
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaMotivoNoConciliado()
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargarComboMotivoNoConciliado", cnn);
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaCelula(int corporativo)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargarComboCelula", cnn);
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.SmallInt).Value = corporativo;
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }


//        public override List<ImportacionAplicacion> ConsultaImportacionAplicacion(short sucursal)
//        {
//            List<ImportacionAplicacion> datos = new List<ImportacionAplicacion>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBImportacionAplicacion", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = 0;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ImportacionAplicacion dato =
//                            new ImportacionAplicacion(Convert.ToInt32(reader["ImportacionAplicacion"]),
//                                                      Convert.ToString(reader["Descripcion"]),
//                                                      Convert.ToInt16(reader["TipoFuenteInformacion"]),
//                                                      Convert.ToString(reader["Procedimiento"]),
//                                                      Convert.ToString(reader["Servidor"]),
//                                                      Convert.ToString(reader["BaseDeDatos"]),
//                                                      Convert.ToString(reader["Usuario"]),
//                                                      Convert.ToString(reader["ClaveEncriptada"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaStatusConcepto(ConfiguracionStatusConcepto configuracion)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBCargaComboStatusConcepto", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }


//        public override List<DatosArchivo> ConsultaArchivosNoReferenciados(int corporativo, int sucursal, int año,
//                                                                           short mes, short tipofuenteinformacion)
//        {
//            List<DatosArchivo> datos = new List<DatosArchivo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaTablaDestino", cnn);
//                    comando.Parameters.Add("@Corporativo ", System.Data.SqlDbType.TinyInt).Value = corporativo;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@Año", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@Mes", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@TipoFuenteInformacion", System.Data.SqlDbType.SmallInt).Value =
//                        tipofuenteinformacion;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        DatosArchivo dato = new DatosArchivoDatos(Convert.ToInt32(reader["Corporativo"]),
//                                                                  Convert.ToInt32(reader["Sucursal"]),
//                                                                  Convert.ToInt32(reader["Año"]),
//                                                                  Convert.ToInt32(reader["Folio"]),
//                                                                  Convert.ToString(reader["CuentaBancoFinanciero"]),
//                                                                  Convert.ToDateTime(reader["FInicial"]),
//                                                                  Convert.ToDateTime(reader["FFinal"]),
//                                                                  Convert.ToInt16(reader["TipoFuenteInformacion"]),
//                                                                  Convert.ToString(reader["TipoFuenteInformacionDes"]),
//                                                                  Convert.ToString(reader["StatusConciliacion"]),
//                                                                  Convert.ToInt16(reader["TipoFuente"]),
//                                                                  Convert.ToString(reader["TipoFuenteDes"]), 0, 0, 0,
//                                                                  this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<cFuenteInformacion> ConsultaFuenteInformacion(int corporativo, int sucursal,
//                                                                           string cuentabancaria,
//                                                                           short tipofuenteinformacion)
//        {
//            List<cFuenteInformacion> datos = new List<cFuenteInformacion>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaFuenteInformacion", cnn);
//                    comando.Parameters.Add("@Corporativo ", System.Data.SqlDbType.TinyInt).Value = corporativo;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@CuentaBancaria", System.Data.SqlDbType.VarChar).Value = cuentabancaria;
//                    comando.Parameters.Add("@TipoFuenteInformacion", System.Data.SqlDbType.SmallInt).Value =
//                        tipofuenteinformacion;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        cFuenteInformacion dato =
//                            new FuenteInformacionDatos(Convert.ToInt32(reader["FuenteInformacion"]),
//                                                       Convert.ToString(reader["RutaArchivo"]),
//                                                       Convert.ToString(reader["CuentaBancoFinanciero"]),
//                                                       Convert.ToInt16(reader["TipoFuenteInformacion"]),
//                                                       Convert.ToString(reader["TipoFuenteInformacionDes"]),
//                                                       Convert.ToInt16(reader["TipoFuente"]),
//                                                       Convert.ToString(reader["TipoFuenteDes"]),
//                                                       Convert.ToInt16(reader["TipoArchivo"]),
//                                                       Convert.ToString(reader["TipoArchivoDes"]),
//                                                       this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<DatosArchivoDetalle> ConsultaTablaDestinoDetalle(Configuracion configuracion,
//                                                                              int corporativo, int sucursal, int año,
//                                                                              int folio)
//        {
//            List<DatosArchivoDetalle> datos = new List<DatosArchivoDetalle>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaTablaDestinoDetalle", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@Corporativo ", System.Data.SqlDbType.TinyInt).Value = corporativo;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@Año", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@Folio", System.Data.SqlDbType.Int).Value = folio;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        DatosArchivoDetalle dato = new DatosArchivoDetalleDatos(Convert.ToInt16(reader["Corporativo"]),
//                                                                                Convert.ToInt16(reader["Sucursal"]),
//                                                                                Convert.ToInt32(reader["Año"]),
//                                                                                Convert.ToInt32(reader["Folio"]),
//                                                                                Convert.ToInt32(reader["Secuencia"]),
//                                                                                Convert.ToDateTime(reader["FOperacion"]),
//                                                                                Convert.ToDateTime(reader["FMovimiento"]),
//                                                                                Convert.ToString(reader["Referencia"]),
//                                                                                Convert.ToString(reader["Descripcion"]),
//                                                                                Convert.ToDecimal(reader["Deposito"]),
//                                                                                Convert.ToDecimal(reader["Retiro"]),
//                                                                                Convert.ToString(reader["Concepto"]),
//                                                                                this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaConciliada> ConsultaConciliarArchivosCantidad(int corporativo, int sucursal,
//                                                                                     int año, short mes, int folio,
//                                                                                     short dias, decimal centavos,
//                                                                                     int statusconcepto)
//        {
//            List<ReferenciaConciliada> datos = new List<ReferenciaConciliada>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConciliarArchivos", cnn);
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativo;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folio;
//                    comando.Parameters.Add("@Dias", System.Data.SqlDbType.SmallInt).Value = dias;
//                    comando.Parameters.Add("@Centavos", System.Data.SqlDbType.Decimal).Value = centavos;
//                    comando.Parameters.Add("@StatusConcepto", System.Data.SqlDbType.Int).Value = statusconcepto;
//                    comando.Parameters.Add("@CadenaExterno", System.Data.SqlDbType.VarChar).Value =
//                        App.Consultas.ObtenerCadenaDeEtiquetas(0, corporativo, sucursal, año, mes, folio, statusconcepto);
//                    comando.Parameters.Add("@CadenaInterno", System.Data.SqlDbType.VarChar).Value =
//                        App.Consultas.ObtenerCadenaDeEtiquetas(1, corporativo, sucursal, año, mes, folio, statusconcepto);

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaConciliada dato =
//                            new ReferenciaConciliadaDatos(Convert.ToInt16(reader["CorporativoConciliacion"]),
//                                                          Convert.ToInt32(reader["AñoConciliacion"]),
//                                                          Convert.ToInt16(reader["MesConciliacion"]),
//                                                          Convert.ToInt32(reader["FolioConciliacion"]),
//                                                          Convert.ToInt16(reader["SucursalExt"]),
//                                                          Convert.ToString(reader["SucursalExtDes"]),
//                                                          Convert.ToInt32(reader["FolioExt"]),
//                                                          Convert.ToInt32(reader["SecuenciaExt"]),
//                                                          Convert.ToString(reader["ConceptoExt"]),
//                                                          Convert.ToDecimal(reader["MontoConciliado"]),
//                                                          Convert.ToDecimal(reader["Diferencia"]),
//                                                          Convert.ToInt16(reader["FormaConciliacion"]),
//                                                          Convert.ToInt16(reader["StatusConcepto"]),
//                                                          Convert.ToString(reader["StatusConciliacion"]),
//                                                          Convert.ToDateTime(reader["FOperacionExt"]),
//                                                          Convert.ToDateTime(reader["FMovimientoExt"]),

//                                                          Convert.ToString(reader["ChequeExt"]),
//                                                          Convert.ToString(reader["ReferenciaExt"]),
//                                                          Convert.ToString(reader["DescripcionExt"]),
//                                                          Convert.ToString(reader["NombreTerceroExt"]),
//                                                          Convert.ToString(reader["RFCTerceroExt"]),
//                                                          Convert.ToDecimal(reader["DepositoExt"]),
//                                                          Convert.ToDecimal(reader["RetiroExt"]),

//                                                          Convert.ToInt16(reader["SucursalInterno"]),
//                                                          Convert.ToString(reader["SucursalIntDes"]),
//                                                          Convert.ToInt32(reader["FolioInterno"]),
//                                                          Convert.ToInt32(reader["SecuenciaInterno"]),
//                                                          Convert.ToString(reader["ConceptoInterno"]),
//                                                          Convert.ToDecimal(reader["MontoInterno"]),
//                                                          Convert.ToDateTime(reader["FOperacionInt"]),
//                                                          Convert.ToDateTime(reader["FMovimientoInt"]),

//                                                          Convert.ToString(reader["ChequeInt"]),
//                                                          Convert.ToString(reader["ReferenciaInt"]),
//                                                          Convert.ToString(reader["DescripcionInt"]),
//                                                          Convert.ToString(reader["NombreTerceroInt"]),
//                                                          Convert.ToString(reader["RFCTerceroInt"]),
//                                                          Convert.ToDecimal(reader["DepositoInt"]),
//                                                          Convert.ToDecimal(reader["RetiroInt"]),
//                                                          Convert.ToInt32(reader["AñoConciliacion"]),
//                                                          Convert.ToInt32(reader["AñoConciliacion"]),
//                                                          this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaConciliada> ConsultaConciliarCopiar(int corporativo, int sucursal, int año,
//                                                                           short mes, int folio)
//        {
//            List<ReferenciaConciliada> datos = new List<ReferenciaConciliada>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConciliacionPorCopia", cnn);
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativo;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folio;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaConciliada dato =
//                            new ReferenciaConciliadaDatos(Convert.ToInt16(reader["CorporativoConciliacion"]),
//                                                          Convert.ToInt32(reader["AñoConciliacion"]),
//                                                          Convert.ToInt16(reader["MesConciliacion"]),
//                                                          Convert.ToInt32(reader["FolioConciliacion"]),
//                                                          Convert.ToInt16(reader["SucursalExt"]),
//                                                          Convert.ToString(reader["SucursalExtDes"]),
//                                                          Convert.ToInt32(reader["FolioExt"]),
//                                                          Convert.ToInt32(reader["SecuenciaExt"]),
//                                                          Convert.ToString(reader["ConceptoExt"]),
//                                                          Convert.ToDecimal(reader["MontoConciliado"]),
//                                                          Convert.ToDecimal(reader["Diferencia"]),
//                                                          Convert.ToInt16(reader["FormaConciliacion"]),
//                                                          Convert.ToInt16(reader["StatusConcepto"]),
//                                                          Convert.ToString(reader["StatusConciliacion"]),
//                                                          Convert.ToDateTime(reader["FOperacionExt"]),
//                                                          Convert.ToDateTime(reader["FMovimientoExt"]),

//                                                          Convert.ToString(reader["ChequeExt"]),
//                                                          Convert.ToString(reader["ReferenciaExt"]),
//                                                          Convert.ToString(reader["DescripcionExt"]),
//                                                          Convert.ToString(reader["NombreTerceroExt"]),
//                                                          Convert.ToString(reader["RFCTerceroExt"]),
//                                                          Convert.ToDecimal(reader["DepositoExt"]),
//                                                          Convert.ToDecimal(reader["RetiroExt"]),

//                                                          Convert.ToInt16(reader["SucursalInterno"]),
//                                                          Convert.ToString(reader["SucursalIntDes"]),
//                                                          Convert.ToInt32(reader["FolioInterno"]),
//                                                          Convert.ToInt32(reader["SecuenciaInterno"]),
//                                                          Convert.ToString(reader["ConceptoInterno"]),
//                                                          Convert.ToDecimal(reader["MontoInterno"]),
//                                                          Convert.ToDateTime(reader["FOperacionInt"]),
//                                                          Convert.ToDateTime(reader["FMovimientoInt"]),

//                                                          Convert.ToString(reader["ChequeInt"]),
//                                                          Convert.ToString(reader["ReferenciaInt"]),
//                                                          Convert.ToString(reader["DescripcionInt"]),
//                                                          Convert.ToString(reader["NombreTerceroInt"]),
//                                                          Convert.ToString(reader["RFCTerceroInt"]),
//                                                          Convert.ToDecimal(reader["DepositoInt"]),
//                                                          Convert.ToDecimal(reader["RetiroInt"]),
//                                                          Convert.ToInt32(reader["AñoConciliacion"]),
//                                                          Convert.ToInt32(reader["AñoConciliacion"]),
//                                                          this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaConciliada> ConsultaConciliarPorReferencia(int corporativo, int sucursal, int año,
//                                                                                  short mes, int folio, short dias,
//                                                                                  decimal centavos, string campoexterno,
//                                                                                  string campointerno,
//                                                                                  int statusconcepto)
//        {
//            List<ReferenciaConciliada> datos = new List<ReferenciaConciliada>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConciliarArchivosPorReferencia", cnn);
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativo;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folio;
//                    comando.Parameters.Add("@Dias", System.Data.SqlDbType.SmallInt).Value = dias;
//                    comando.Parameters.Add("@Centavos", System.Data.SqlDbType.Decimal).Value = centavos;
//                    comando.Parameters.Add("@CampoExterno", System.Data.SqlDbType.VarChar).Value = campoexterno;
//                    comando.Parameters.Add("@CampoInterno", System.Data.SqlDbType.VarChar).Value = campointerno;
//                    comando.Parameters.Add("@StatusConcepto", System.Data.SqlDbType.Int).Value = statusconcepto;
//                    comando.Parameters.Add("@CadenaExterno", System.Data.SqlDbType.VarChar).Value =
//                        App.Consultas.ObtenerCadenaDeEtiquetas(0, corporativo, sucursal, año, mes, folio, statusconcepto);
//                    comando.Parameters.Add("@CadenaInterno", System.Data.SqlDbType.VarChar).Value =
//                        App.Consultas.ObtenerCadenaDeEtiquetas(1, corporativo, sucursal, año, mes, folio, statusconcepto);

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaConciliada dato =
//                            new ReferenciaConciliadaDatos(Convert.ToInt16(reader["CorporativoConciliacion"]),
//                                                          Convert.ToInt32(reader["AñoConciliacion"]),
//                                                          Convert.ToInt16(reader["MesConciliacion"]),
//                                                          Convert.ToInt32(reader["FolioConciliacion"]),
//                                                          Convert.ToInt16(reader["SucursalExt"]),
//                                                          Convert.ToString(reader["SucursalExtDes"]),
//                                                          Convert.ToInt32(reader["FolioExt"]),
//                                                          Convert.ToInt32(reader["SecuenciaExt"]),
//                                                          Convert.ToString(reader["ConceptoExt"]),
//                                                          Convert.ToDecimal(reader["MontoConciliado"]),
//                                                          Convert.ToDecimal(reader["Diferencia"]),
//                                                          Convert.ToInt16(reader["FormaConciliacion"]),
//                                                          Convert.ToInt16(reader["StatusConcepto"]),
//                                                          Convert.ToString(reader["StatusConciliacion"]),
//                                                          Convert.ToDateTime(reader["FOperacionExt"]),
//                                                          Convert.ToDateTime(reader["FMovimientoExt"]),

//                                                          Convert.ToString(reader["ChequeExt"]),
//                                                          Convert.ToString(reader["ReferenciaExt"]),
//                                                          Convert.ToString(reader["DescripcionExt"]),
//                                                          Convert.ToString(reader["NombreTerceroExt"]),
//                                                          Convert.ToString(reader["RFCTerceroExt"]),
//                                                          Convert.ToDecimal(reader["DepositoExt"]),
//                                                          Convert.ToDecimal(reader["RetiroExt"]),

//                                                          Convert.ToInt16(reader["SucursalInterno"]),
//                                                          Convert.ToString(reader["SucursalIntDes"]),
//                                                          Convert.ToInt32(reader["FolioInterno"]),
//                                                          Convert.ToInt32(reader["SecuenciaInterno"]),
//                                                          Convert.ToString(reader["ConceptoInterno"]),
//                                                          Convert.ToDecimal(reader["MontoInterno"]),
//                                                          Convert.ToDateTime(reader["FOperacionInt"]),
//                                                          Convert.ToDateTime(reader["FMovimientoInt"]),

//                                                          Convert.ToString(reader["ChequeInt"]),
//                                                          Convert.ToString(reader["ReferenciaInt"]),
//                                                          Convert.ToString(reader["DescripcionInt"]),
//                                                          Convert.ToString(reader["NombreTerceroInt"]),
//                                                          Convert.ToString(reader["RFCTerceroInt"]),
//                                                          Convert.ToDecimal(reader["DepositoInt"]),
//                                                          Convert.ToDecimal(reader["RetiroInt"]),
//                                                          Convert.ToInt32(reader["AñoConciliacion"]),
//                                                          Convert.ToInt32(reader["AñoConciliacion"]),
//                                                          this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaNoConciliada> ConsultaDetalleExternoPendiente(ConsultaExterno configuracion,
//                                                                                     int corporativo, short sucursal,
//                                                                                     int año, short mes, int folio,
//                                                                                     decimal diferencia,
//                                                                                     int statusconcepto)
//        {
//            bool coninterno =
//                !(configuracion == ConsultaExterno.DepositosConReferenciaPedido ||
//                  configuracion == ConsultaExterno.DepositosPedido);

//            List<ReferenciaNoConciliada> datos = new List<ReferenciaNoConciliada>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConciliacionPendienteExterno", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativo;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folio;
//                    comando.Parameters.Add("@StatusConcepto", System.Data.SqlDbType.Int).Value = statusconcepto;
//                    comando.Parameters.Add("@Cadena", System.Data.SqlDbType.VarChar).Value =
//                        App.Consultas.ObtenerCadenaDeEtiquetas(0, corporativo, sucursal, año, mes, folio, statusconcepto);

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaNoConciliada dato =
//                            new ReferenciaNoConciliadaDatos(Convert.ToInt16(reader["Corporativo"]),
//                                                            Convert.ToInt16(reader["Sucursal"]),
//                                                            Convert.ToString(reader["SucursalDes"]), año,
//                                                            Convert.ToInt16(reader["Folio"]),
//                                                            Convert.ToInt16(reader["Secuencia"]),
//                                                            Convert.ToString(reader["Descripcion"]),
//                                                            Convert.ToString(reader["Concepto"]),
//                                                            Convert.ToDecimal(reader["Deposito"]),
//                                                            Convert.ToDecimal(reader["Retiro"]),
//                                                            Convert.ToInt16(reader["FormaConciliacion"]),
//                                                            Convert.ToInt16(reader["StatusConcepto"]),
//                                                            Convert.ToString(reader["StatusConciliacion"]),
//                                                            Convert.ToDateTime(reader["FOperacion"]),
//                                                            Convert.ToDateTime(reader["FMovimiento"]),
//                                                            Convert.ToString(reader["UbicacionIcono"]),
//                                                            folio, mes, diferencia, coninterno,
//                                                            Convert.ToString(reader["Cheque"]),
//                                                            Convert.ToString(reader["Referencia"]),
//                                                            Convert.ToString(reader["NombreTercero"]),
//                                                            Convert.ToString(reader["RFCTercero"]), sucursal,
//                                                            Convert.ToInt16(reader["Año"]), this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaNoConciliada> ConsultaDetalleInternoPendiente(ConciliacionInterna configuracion,
//                                                                                     int corporativoconciliacion,
//                                                                                     short sucursalconciliacion,
//                                                                                     int añoconciliacion,
//                                                                                     short mesconciliacion,
//                                                                                     int folioconciliacion,
//                                                                                     int folioexterno,
//                                                                                     int secuenciaexterno,
//                                                                                     short sucursalinterno, short dias,
//                                                                                     decimal diferencia,
//                                                                                     int statusconcepto)
//        {
//            List<ReferenciaNoConciliada> datos = new List<ReferenciaNoConciliada>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConciliacionBusquedaInterna", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativoconciliacion;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        sucursalconciliacion;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = añoconciliacion;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mesconciliacion;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folioconciliacion;
//                    comando.Parameters.Add("@Folio", System.Data.SqlDbType.Int).Value = folioexterno;
//                    comando.Parameters.Add("@Secuencia", System.Data.SqlDbType.Int).Value = secuenciaexterno;
//                    comando.Parameters.Add("@Dias", System.Data.SqlDbType.SmallInt).Value = dias;
//                    comando.Parameters.Add("@Diferencia", System.Data.SqlDbType.SmallInt).Value = diferencia;
//                    comando.Parameters.Add("@SucursalInterno", System.Data.SqlDbType.TinyInt).Value = sucursalinterno;
//                    comando.Parameters.Add("@StatusConcepto", System.Data.SqlDbType.Int).Value = statusconcepto;
//                    comando.Parameters.Add("@Cadena", System.Data.SqlDbType.VarChar).Value =
//                        App.Consultas.ObtenerCadenaDeEtiquetas(1, corporativoconciliacion, sucursalconciliacion,
//                                                               añoconciliacion, mesconciliacion, folioconciliacion,
//                                                               statusconcepto);

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaNoConciliada dato =
//                            new ReferenciaNoConciliadaDatos(Convert.ToInt16(reader["Corporativo"]),
//                                                            Convert.ToInt16(reader["Sucursal"]),
//                                                            Convert.ToString(reader["SucursalDes"]), añoconciliacion,
//                                                            Convert.ToInt16(reader["Folio"]),
//                                                            Convert.ToInt16(reader["Secuencia"]),
//                                                            Convert.ToString(reader["Descripcion"]),
//                                                            Convert.ToString(reader["Concepto"]),
//                                                            Convert.ToDecimal(reader["Deposito"]),
//                                                            Convert.ToDecimal(reader["Retiro"]),
//                                                            Convert.ToInt16(reader["FormaConciliacion"]),
//                                                            Convert.ToInt16(reader["StatusConcepto"]),
//                                                            Convert.ToString(reader["StatusConciliacion"]),
//                                                            Convert.ToDateTime(reader["FOperacion"]),
//                                                            Convert.ToDateTime(reader["FMovimiento"]),
//                                                            Convert.ToString(reader["UbicacionIcono"]),
//                                                            folioconciliacion, mesconciliacion, 0, true,

//                                                            Convert.ToString(reader["Cheque"]),
//                                                            Convert.ToString(reader["Referencia"]),
//                                                            Convert.ToString(reader["NombreTercero"]),
//                                                            Convert.ToString(reader["RFCTercero"]), sucursalconciliacion,
//                                                            Convert.ToInt16(reader["Año"]), this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaNoConciliada> ConciliacionBusqueda(BusquedaConciliacion configuracion,
//                                                                          int corporativo, short sucursal, int año,
//                                                                          short mes, int folio, String campo,
//                                                                          string operacion, string valor,
//                                                                          string tipocampo, decimal diferencia)
//        {
//            List<ReferenciaNoConciliada> datos = new List<ReferenciaNoConciliada>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    if (operacion == "LIKE")
//                    {
//                        valor = "%" + valor + "%";
//                    }

//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConciliacionBusqueda", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativo;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folio;
//                    comando.Parameters.Add("@Campo", System.Data.SqlDbType.VarChar).Value = campo;
//                    comando.Parameters.Add("@Operacion", System.Data.SqlDbType.VarChar).Value = operacion;
//                    comando.Parameters.Add("@Valor", System.Data.SqlDbType.VarChar).Value = valor;
//                    comando.Parameters.Add("@TipoCampo", System.Data.SqlDbType.VarChar).Value = tipocampo;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        ReferenciaNoConciliada dato =
//                            new ReferenciaNoConciliadaDatos(Convert.ToInt16(reader["Corporativo"]),
//                                                            Convert.ToInt16(reader["Sucursal"]),
//                                                            Convert.ToString(reader["SucursalDes"]), año,
//                                                            Convert.ToInt16(reader["Folio"]),
//                                                            Convert.ToInt16(reader["Secuencia"]),
//                                                            Convert.ToString(reader["Descripcion"]),
//                                                            Convert.ToString(reader["Concepto"]),
//                                                            Convert.ToDecimal(reader["Deposito"]),
//                                                            Convert.ToDecimal(reader["Retiro"]),
//                                                            Convert.ToInt16(reader["FormaConciliacion"]),
//                                                            Convert.ToInt16(reader["StatusConcepto"]),
//                                                            Convert.ToString(reader["StatusConciliacion"]),
//                                                            Convert.ToDateTime(reader["FOperacion"]),
//                                                            Convert.ToDateTime(reader["FMovimiento"]),
//                                                            folio, mes, diferencia, true,

//                                                            Convert.ToString(reader["Cheque"]),
//                                                            Convert.ToString(reader["Referencia"]),
//                                                            Convert.ToString(reader["NombreTercero"]),
//                                                            Convert.ToString(reader["RFCTercero"]), sucursal,
//                                                            Convert.ToInt16(reader["Año"]), this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override String ObtenerCadenaDeEtiquetas(short configuracion, int corporativo, int sucursalconciliacion,
//                                                        int añoconciliacion, short mesconciliacion,
//                                                        int folioconciliacion, int statusconcepto)
//        {
//            List<String> etiquetas = new List<String>();
//            String cadena = "";
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    string fecha = String.Empty;
//                    SqlCommand comando = new SqlCommand("spCBConsultaEtiquetas", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativo;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        sucursalconciliacion;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = añoconciliacion;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mesconciliacion;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folioconciliacion;
//                    comando.Parameters.Add("@StatusConcepto", System.Data.SqlDbType.Int).Value = statusconcepto;
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        string etiqueta;
//                        etiqueta = Convert.ToString(reader["EtiquetaStr"]);
//                        etiquetas.Add(etiqueta);
//                    }
//                    if (etiquetas.Count == 0)
//                        cadena = "";
//                    else
//                    {
//                        cadena = " AND (";
//                        for (int i = 0; i < etiquetas.Count; i++)
//                        {
//                            if (cadena != " AND (")
//                                cadena = cadena + " OR ";
//                            cadena = cadena + " TDD.concepto like '%" + etiquetas[i] + "%' OR ";
//                            cadena = cadena + " TDD.descripcion like '%" + etiquetas[i] + "%' ";
//                        }
//                        cadena = cadena + ") ";
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }

//            }
//            return cadena;
//        }

//        public override string ConsultaFechaActualInicial()
//        {
//            string fecha = String.Empty;
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaMesAñoActual", cnn);
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        fecha = Convert.ToString(reader["FechaActual"]);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return fecha;
//            }
//        }

//        public override string ConsultaFechaPermitidoConciliacion(string numMesesAnterior, string fechaMaxActual)
//        {
//            string fecha = String.Empty;
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaMesesPermitidos", cnn);
//                    comando.Parameters.Add("@NumMesesAnterior", System.Data.SqlDbType.VarChar).Value = numMesesAnterior;
//                    comando.Parameters.Add("@FechaMax ", System.Data.SqlDbType.VarChar).Value = fechaMaxActual;
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        fecha = Convert.ToString(reader["FechaMinima"]);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return fecha;
//            }
//        }

//        public override List<ReferenciaConciliadaPedido> ConsultaConciliarPedidoCantidadReferencia(int corporativo,
//                                                                                                   int sucursal, int año,
//                                                                                                   short mes, int folio,
//                                                                                                   decimal centavos,
//                                                                                                   int statusconcepto,
//                                                                                                   string campoexterno,
//                                                                                                   string campopedido)
//        {
//            List<ReferenciaConciliadaPedido> datos = new List<ReferenciaConciliadaPedido>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConciliarPedidosPorReferencia", cnn);
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativo;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folio;
//                    comando.Parameters.Add("@Centavos", System.Data.SqlDbType.Decimal).Value = centavos;
//                    comando.Parameters.Add("@StatusConcepto", System.Data.SqlDbType.Int).Value = statusconcepto;
//                    comando.Parameters.Add("@Cadena", System.Data.SqlDbType.VarChar).Value =
//                        App.Consultas.ObtenerCadenaDeEtiquetas(0, corporativo, sucursal, año, mes, folio, statusconcepto);
//                    comando.Parameters.Add("@CampoExterno", System.Data.SqlDbType.VarChar).Value = campoexterno;
//                    comando.Parameters.Add("@CampoPedido", System.Data.SqlDbType.VarChar).Value = campopedido;
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaConciliadaPedido dato =
//                            new ReferenciaConciliadaPedidoDatos(Convert.ToInt16(reader["CorporativoConciliacion"]),
//                                                                Convert.ToInt32(reader["AñoConciliacion"]),
//                                                                Convert.ToInt16(reader["MesConciliacion"]),
//                                                                Convert.ToInt32(reader["FolioConciliacion"]),
//                                                                Convert.ToInt16(reader["SucursalExt"]),
//                                                                Convert.ToString(reader["SucursalExtDes"]),
//                                                                Convert.ToInt32(reader["FolioExt"]),
//                                                                Convert.ToInt32(reader["SecuenciaExt"]),
//                                                                Convert.ToString(reader["ConceptoExt"]),
//                                                                Convert.ToDecimal(reader["MontoConciliado"]),
//                                                                Convert.ToDecimal(reader["Diferencia"]),
//                                                                Convert.ToInt16(reader["FormaConciliacion"]),
//                                                                Convert.ToInt16(reader["StatusConcepto"]),
//                                                                Convert.ToString(reader["StatusConciliacion"]),
//                                                                Convert.ToDateTime(reader["FOperacionExt"]),
//                                                                Convert.ToDateTime(reader["FMovimientoExt"]),

//                                                                Convert.ToString(reader["Cheque"]),
//                                                                Convert.ToString(reader["Referencia"]),
//                                                                Convert.ToString(reader["Descripcion"]),
//                                                                Convert.ToString(reader["NombreTercero"]),
//                                                                Convert.ToString(reader["RFCTercero"]),
//                                                                Convert.ToDecimal(reader["Deposito"]),
//                                                                Convert.ToDecimal(reader["Retiro"]),

//                                                                Convert.ToInt16(reader["SucursalPedido"]),
//                                                                Convert.ToString(reader["SucursalPedidoDes"]),
//                                                                Convert.ToInt32(reader["CelulaPedido"]),
//                                                                Convert.ToInt32(reader["AñoPedido"]),
//                                                                Convert.ToInt32(reader["Pedido"]),
//                                                                Convert.ToInt32(reader["RemisionPedido"]),
//                                                                Convert.ToString(reader["SeriePedido"]),
//                                                                Convert.ToInt32(reader["FolioSat"]),
//                                                                Convert.ToString(reader["SerieSat"]),
//                                                                Convert.ToString(reader["ConceptoPedido"]),
//                                                                Convert.ToDecimal(reader["Total"]),
//                                                                Convert.ToString(reader["StatusMovimiento"]),
//                                                                Convert.ToInt32(reader["Cliente"]),
//                                                                Convert.ToString(reader["Nombre"]),
//                                                                Convert.ToString(reader["PedidoReferencia"]),
//                                                                Convert.ToInt32(reader["AñoConciliacion"]),
//                                                                this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaConciliadaPedido> ConsultaConciliarPedidoCantidad(int corporativo, int sucursal,
//                                                                                         int año, short mes, int folio,
//                                                                                         decimal centavos,
//                                                                                         int statusconcepto)
//        {
//            List<ReferenciaConciliadaPedido> datos = new List<ReferenciaConciliadaPedido>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConciliarPedidos", cnn);
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativo;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folio;
//                    comando.Parameters.Add("@Centavos", System.Data.SqlDbType.Decimal).Value = centavos;
//                    comando.Parameters.Add("@StatusConcepto", System.Data.SqlDbType.Int).Value = statusconcepto;
//                    comando.Parameters.Add("@Cadena", System.Data.SqlDbType.VarChar).Value =
//                        App.Consultas.ObtenerCadenaDeEtiquetas(0, corporativo, sucursal, año, mes, folio, statusconcepto);
//                    comando.CommandTimeout = 300;
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaConciliadaPedido dato =
//                            new ReferenciaConciliadaPedidoDatos(Convert.ToInt16(reader["CorporativoConciliacion"]),
//                                                                Convert.ToInt32(reader["AñoConciliacion"]),
//                                                                Convert.ToInt16(reader["MesConciliacion"]),
//                                                                Convert.ToInt32(reader["FolioConciliacion"]),
//                                                                Convert.ToInt16(reader["SucursalExt"]),
//                                                                Convert.ToString(reader["SucursalExtDes"]),
//                                                                Convert.ToInt32(reader["FolioExt"]),
//                                                                Convert.ToInt32(reader["SecuenciaExt"]),
//                                                                Convert.ToString(reader["ConceptoExt"]),
//                                                                Convert.ToDecimal(reader["MontoConciliado"]),
//                                                                Convert.ToDecimal(reader["Diferencia"]),
//                                                                Convert.ToInt16(reader["FormaConciliacion"]),
//                                                                Convert.ToInt16(reader["StatusConcepto"]),
//                                                                Convert.ToString(reader["StatusConciliacion"]),
//                                                                Convert.ToDateTime(reader["FOperacionExt"]),
//                                                                Convert.ToDateTime(reader["FMovimientoExt"]),

//                                                                Convert.ToString(reader["Cheque"]),
//                                                                Convert.ToString(reader["Referencia"]),
//                                                                Convert.ToString(reader["Descripcion"]),
//                                                                Convert.ToString(reader["NombreTercero"]),
//                                                                Convert.ToString(reader["RFCTercero"]),
//                                                                Convert.ToDecimal(reader["Deposito"]),
//                                                                Convert.ToDecimal(reader["Retiro"]),

//                                                                Convert.ToInt16(reader["SucursalPedido"]),
//                                                                Convert.ToString(reader["SucursalPedidoDes"]),
//                                                                Convert.ToInt32(reader["CelulaPedido"]),
//                                                                Convert.ToInt32(reader["AñoPedido"]),
//                                                                Convert.ToInt32(reader["Pedido"]),
//                                                                Convert.ToInt32(reader["RemisionPedido"]),
//                                                                Convert.ToString(reader["SeriePedido"]),
//                                                                Convert.ToInt32(reader["FolioSat"]),
//                                                                Convert.ToString(reader["SerieSat"]),
//                                                                Convert.ToString(reader["ConceptoPedido"]),
//                                                                Convert.ToDecimal(reader["Total"]),
//                                                                Convert.ToString(reader["StatusMovimiento"]),
//                                                                Convert.ToInt32(reader["Cliente"]),
//                                                                Convert.ToString(reader["Nombre"]),
//                                                                Convert.ToString(reader["PedidoReferencia"]),
//                                                                Convert.ToInt32(reader["AñoConciliacion"]),
//                                                                this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        //public override List<ReferenciaNoConciliadaPedido> ConsultaDetallePedidoPendiente(ConfiguracionPedido configuracion, int corporativoconciliacion, int sucursalconciliacion, int añoconciliacion, short mesconciliacion, int folioconciliacion, int folioexterno, int secuenciaexterno, int sucursalinterno)
//        //{
//        //    List<ReferenciaNoConciliadaPedido> datos = new List<ReferenciaNoConciliadaPedido>();
//        //    using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//        //    {
//        //        cnn.Open();
//        //        SqlCommand comando = new SqlCommand("spCBConciliacionBusquedaInternaPedido", cnn);
//        //        comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.TinyInt).Value = configuracion;
//        //        comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value = corporativoconciliacion;
//        //        comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value = sucursalconciliacion;
//        //        comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = añoconciliacion;
//        //        comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mesconciliacion;
//        //        comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folioconciliacion;
//        //        comando.Parameters.Add("@Folio", System.Data.SqlDbType.Int).Value = folioexterno;
//        //        comando.Parameters.Add("@Secuencia", System.Data.SqlDbType.Int).Value = secuenciaexterno;
//        //        comando.Parameters.Add("@SucursalInterno", System.Data.SqlDbType.Int).Value = sucursalinterno;
//        //        comando.CommandType = System.Data.CommandType.StoredProcedure;
//        //        SqlDataReader reader = comando.ExecuteReader();
//        //        while (reader.Read())
//        //        {

//        //            ReferenciaNoConciliadaPedido dato = new ReferenciaNoConciliadaPedidoDatos(
//        //                                                 Convert.ToInt16(reader["Corporativo"]), Convert.ToInt16(reader["Sucursal"]),
//        //                                                 Convert.ToString(reader["SucursalDes"]),añoconciliacion,
//        //                                                 folioconciliacion,mesconciliacion,
//        //                                                 Convert.ToInt32(reader["Celula"]),Convert.ToInt32(reader["AñoPed"]),Convert.ToInt32(reader["Pedido"]),
//        //                                                 Convert.ToString(reader["Cliente"]),
//        //                                                 Convert.ToInt32(reader["RemisionPedido"]),Convert.ToString(reader["SeriePedido"]),
//        //                                                 Convert.ToInt32(reader["FolioSat"]),Convert.ToString(reader["SerieSat"]),
//        //                                                 Convert.ToString(reader["Concepto"]),Convert.ToDecimal(reader["Monto"]),
//        //                                                 Convert.ToInt16(reader["FormaConciliacion"]),Convert.ToInt16(reader["StatusConcepto"]),
//        //                                                 Convert.ToString(reader["StatusConciliacion"]), Convert.ToDateTime(reader["FOperacion"]),
//        //                                                 Convert.ToDateTime(reader["FMovimiento"]), 0, this.implementadorMensajes);
//        //            datos.Add(dato);
//        //        }
//        //        return datos;
//        //    }
//        //}


//        //public override List<ReferenciaNoConciliadaPedido> ConsultaDetalleExternoPendientePedido(ConsultaExterno configuracion, int corporativo, int sucursal, int año, short mes, int folio, decimal diferencia)
//        //{
//        //    List<ReferenciaNoConciliadaPedido> datos = new List<ReferenciaNoConciliadaPedido>();
//        //    using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//        //    {
//        //        cnn.Open();
//        //        SqlCommand comando = new SqlCommand("spCBConciliacionPendienteExterno", cnn);
//        //        comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//        //        comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value = corporativo;
//        //        comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value = sucursal;
//        //        comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//        //        comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mes;
//        //        comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folio;

//        //        comando.CommandType = System.Data.CommandType.StoredProcedure;
//        //        SqlDataReader reader = comando.ExecuteReader();
//        //        while (reader.Read())
//        //        {

//        //            ReferenciaNoConciliadaPedido dato = new ReferenciaNoConciliadaPedidoDatos(Convert.ToInt16(reader["Corporativo"]),Convert.ToInt16(reader["Sucursal"]),
//        //                                                 Convert.ToString(reader["SucursalDes"]), Convert.ToInt16(reader["Año"]),
//        //                                                 Convert.ToInt16(reader["Folio"]), Convert.ToInt16(reader["Secuencia"]),
//        //                                                 Convert.ToString(reader["Concepto"]), Convert.ToDecimal(reader["Deposito"]),
//        //                                                 Convert.ToDecimal(reader["Retiro"]), Convert.ToInt16(reader["FormaConciliacion"]),
//        //                                                 Convert.ToInt16(reader["StatusConcepto"]), Convert.ToString(reader["StatusConciliacion"]),
//        //                                                 Convert.ToDateTime(reader["FMovimiento"]), Convert.ToDateTime(reader["FOperacion"]),
//        //                                                 folio, mes, diferencia, this.implementadorMensajes);
//        //            datos.Add(dato);
//        //        }
//        //        return datos;
//        //    }
//        //}

//        public override List<ReferenciaNoConciliadaPedido> ConciliacionBusquedaPedido(BusquedaPedido configuracion,
//                                                                                      int corporativoconciliacion,
//                                                                                      int sucursalconciliacion,
//                                                                                      int añoconciliacion,
//                                                                                      short mesconciliacion,
//                                                                                      int folioconciliacion,
//                                                                                      int folioexterno,
//                                                                                      int secuenciaexterno,
//                                                                                      decimal diferencia, int celula)
//        {
//            List<ReferenciaNoConciliadaPedido> datos = new List<ReferenciaNoConciliadaPedido>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConciliacionBusquedaPedido", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativoconciliacion;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        sucursalconciliacion;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = añoconciliacion;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mesconciliacion;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folioconciliacion;
//                    comando.Parameters.Add("@Folio", System.Data.SqlDbType.Int).Value = folioexterno;
//                    comando.Parameters.Add("@Secuencia", System.Data.SqlDbType.Int).Value = secuenciaexterno;
//                    comando.Parameters.Add("@Celula", System.Data.SqlDbType.Int).Value = celula;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        ReferenciaNoConciliadaPedido dato = new ReferenciaNoConciliadaPedidoDatos(
//                            Convert.ToInt16(reader["Corporativo"]), Convert.ToInt16(reader["Sucursal"]),
//                            Convert.ToString(reader["SucursalDes"]), añoconciliacion, folioconciliacion, mesconciliacion,
//                            Convert.ToInt32(reader["Celula"]), Convert.ToInt32(reader["AñoPed"]),
//                            Convert.ToInt32(reader["Pedido"]),
//                            Convert.ToInt32(reader["Cliente"]), Convert.ToString(reader["Nombre"]),
//                            Convert.ToInt32(reader["RemisionPedido"]), Convert.ToString(reader["SeriePedido"]),
//                            Convert.ToInt32(reader["FolioSat"]), Convert.ToString(reader["SerieSat"]),
//                            Convert.ToString(reader["Concepto"]), Convert.ToDecimal(reader["Monto"]),
//                            Convert.ToInt16(reader["FormaConciliacion"]), Convert.ToInt16(reader["StatusConcepto"]),
//                            Convert.ToString(reader["StatusConciliacion"]), Convert.ToDateTime(reader["FOperacion"]),
//                            Convert.ToDateTime(reader["FMovimiento"]), diferencia, this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaNoConciliadaPedido> ConciliacionBusquedaPedidoVariosUno(
//            BusquedaPedido configuracion, int corporativoconciliacion, int sucursalconciliacion, int añoconciliacion,
//            short mesconciliacion, int folioconciliacion, int folioexterno, int secuenciaexterno, decimal diferencia,
//            int celula)
//        {
//            List<ReferenciaNoConciliadaPedido> datos = new List<ReferenciaNoConciliadaPedido>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConciliacionBusquedaPedido2", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativoconciliacion;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        sucursalconciliacion;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = añoconciliacion;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mesconciliacion;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folioconciliacion;
//                    comando.Parameters.Add("@Folio", System.Data.SqlDbType.Int).Value = folioexterno;
//                    comando.Parameters.Add("@Secuencia", System.Data.SqlDbType.Int).Value = secuenciaexterno;
//                    comando.Parameters.Add("@Celula", System.Data.SqlDbType.Int).Value = celula;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        ReferenciaNoConciliadaPedido dato = new ReferenciaNoConciliadaPedidoDatos(
//                            Convert.ToInt16(reader["Corporativo"]), Convert.ToInt16(reader["Sucursal"]),
//                            Convert.ToString(reader["SucursalDes"]), añoconciliacion, folioconciliacion, mesconciliacion,
//                            Convert.ToInt32(reader["Celula"]), Convert.ToInt32(reader["AñoPed"]),
//                            Convert.ToInt32(reader["Pedido"]),
//                            Convert.ToInt32(reader["Cliente"]), Convert.ToString(reader["Nombre"]),
//                            Convert.ToInt32(reader["RemisionPedido"]), Convert.ToString(reader["SeriePedido"]),
//                            Convert.ToInt32(reader["FolioSat"]), Convert.ToString(reader["SerieSat"]),
//                            Convert.ToString(reader["Concepto"]), Convert.ToDecimal(reader["Monto"]),
//                            /*Convert.ToInt16(reader["FormaConciliacion"])*/6, Convert.ToInt16(reader["StatusConcepto"]),
//                            Convert.ToString(reader["StatusConciliacion"]), Convert.ToDateTime(reader["FOperacion"]),
//                            Convert.ToDateTime(reader["FMovimiento"]), diferencia, this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message + "\n\r" + "Stack :" +
//                                                              ex.StackTrace);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaNoConciliadaPedido> ConciliacionBusquedaPedidoManual(
//            BusquedaPedido configuracion, int corporativoconciliacion, int sucursalconciliacion, int añoconciliacion,
//            short mesconciliacion, int folioconciliacion, int folioexterno, int secuenciaexterno, decimal diferencia,
//            int celula)
//        {
//            List<ReferenciaNoConciliadaPedido> datos = new List<ReferenciaNoConciliadaPedido>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConciliacionBusquedaPedido2", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativoconciliacion;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        sucursalconciliacion;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = añoconciliacion;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mesconciliacion;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folioconciliacion;
//                    comando.Parameters.Add("@Folio", System.Data.SqlDbType.Int).Value = folioexterno;
//                    comando.Parameters.Add("@Secuencia", System.Data.SqlDbType.Int).Value = secuenciaexterno;
//                    comando.Parameters.Add("@Celula", System.Data.SqlDbType.Int).Value = celula;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        ReferenciaNoConciliadaPedido dato = new ReferenciaNoConciliadaPedidoDatos(
//                            Convert.ToInt16(reader["Corporativo"]), Convert.ToInt16(reader["Sucursal"]),
//                            Convert.ToString(reader["SucursalDes"]), añoconciliacion, folioconciliacion, mesconciliacion,
//                            Convert.ToInt32(reader["Celula"]), Convert.ToInt32(reader["AñoPed"]),
//                            Convert.ToInt32(reader["Pedido"]),
//                            Convert.ToInt32(reader["Cliente"]), Convert.ToString(reader["Nombre"]),
//                            Convert.ToInt32(reader["RemisionPedido"]), Convert.ToString(reader["SeriePedido"]),
//                            Convert.ToInt32(reader["FolioSat"]), Convert.ToString(reader["SerieSat"]),
//                            Convert.ToString(reader["Concepto"]), Convert.ToDecimal(reader["Monto"]),
//                            Convert.ToInt16(reader["FormaConciliacion"]), Convert.ToInt16(reader["StatusConcepto"]),
//                            Convert.ToString(reader["StatusConciliacion"]), Convert.ToDateTime(reader["FOperacion"]),
//                            Convert.ToDateTime(reader["FMovimiento"]), diferencia, this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message + "\n\r" + "Stack :" +
//                                                              ex.StackTrace);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        //public override List<ReferenciaNoConciliadaPedido> ConciliacionBusquedaExternoPedido(int corporativo, int sucursal, int año, short mes, int folio, String campo, string operacion, string valor, string tipocampo, decimal diferencia)
//        //{
//        //    List<ReferenciaNoConciliadaPedido> datos = new List<ReferenciaNoConciliadaPedido>();
//        //    using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//        //    {
//        //        if (operacion == "LIKE")
//        //        {
//        //            valor = "%" + valor + "%";
//        //        }

//        //        cnn.Open();
//        //        SqlCommand comando = new SqlCommand("spCBConciliacionBusqueda", cnn);
//        //        comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = 0;
//        //        comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value = corporativo;
//        //        comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value = sucursal;
//        //        comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//        //        comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mes;
//        //        comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folio;
//        //        comando.Parameters.Add("@Campo", System.Data.SqlDbType.VarChar).Value = campo;
//        //        comando.Parameters.Add("@Operacion", System.Data.SqlDbType.VarChar).Value = operacion;
//        //        comando.Parameters.Add("@Valor", System.Data.SqlDbType.VarChar).Value = valor;
//        //        comando.Parameters.Add("@TipoCampo", System.Data.SqlDbType.VarChar).Value = tipocampo;

//        //        comando.CommandType = System.Data.CommandType.StoredProcedure;
//        //        SqlDataReader reader = comando.ExecuteReader();
//        //        while (reader.Read())
//        //        {
//        //            ReferenciaNoConciliadaPedido dato = new ReferenciaNoConciliadaPedidoDatos(Convert.ToInt16(reader["Corporativo"]), Convert.ToInt16(reader["Sucursal"]),
//        //                                                 Convert.ToString(reader["SucursalDes"]), Convert.ToInt16(reader["Año"]),
//        //                                                 Convert.ToInt16(reader["Folio"]), Convert.ToInt16(reader["Secuencia"]),
//        //                                                 Convert.ToString(reader["Descripcion"]), Convert.ToString(reader["Concepto"]), Convert.ToDecimal(reader["Deposito"]),
//        //                                                 Convert.ToDecimal(reader["Retiro"]), Convert.ToInt16(reader["FormaConciliacion"]),
//        //                                                 Convert.ToInt16(reader["StatusConcepto"]), Convert.ToString(reader["StatusConciliacion"]),
//        //                                                 Convert.ToDateTime(reader["FMovimiento"]), Convert.ToDateTime(reader["FOperacion"]),
//        //                                                 folio, mes, diferencia, this.implementadorMensajes);
//        //            datos.Add(dato);
//        //        }
//        //        return datos;
//        //    }
//        //}

//        public override List<ReferenciaNoConciliada> ConsultaTransaccionesConciliadas(int corporativo, short sucursal,
//                                                                                      int año, short mes, int folio,
//                                                                                      int formaconciliacion)
//        {
//            List<ReferenciaNoConciliada> datos = new List<ReferenciaNoConciliada>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaTransaccionesConciliadas", cnn);
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativo;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folio;
//                    comando.Parameters.Add("@FormaConciliacion", System.Data.SqlDbType.SmallInt).Value =
//                        formaconciliacion;
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        ReferenciaNoConciliada dato =
//                            new ReferenciaNoConciliadaDatos(Convert.ToInt16(reader["Corporativo"]),
//                                                            Convert.ToInt16(reader["Sucursal"]),
//                                                            Convert.ToString(reader["SucursalDes"]), año,
//                                                            Convert.ToInt16(reader["Folio"]),
//                                                            Convert.ToInt16(reader["Secuencia"]),
//                                                            Convert.ToString(reader["Descripcion"]),
//                                                            Convert.ToString(reader["Concepto"]),
//                                                            Convert.ToDecimal(reader["Deposito"]),
//                                                            Convert.ToDecimal(reader["Retiro"]),
//                                                            Convert.ToInt16(reader["FormaConciliacion"]),
//                                                            Convert.ToInt16(reader["StatusConcepto"]),
//                                                            Convert.ToString(reader["StatusConciliacion"]),
//                                                            Convert.ToDateTime(reader["FOperacion"]),
//                                                            Convert.ToDateTime(reader["FMovimiento"]),
//                                                            Convert.ToInt16(reader["FolioConciliacion"]),
//                                                            Convert.ToInt16(reader["MesConciliacion"]),
//                                                            Convert.ToBoolean(reader["ConInterno"]),
//                                                            ConsultaTransaccionesConciliadasDetalle(
//                                                                Convert.ToInt16(reader["Corporativo"]),
//                                                                Convert.ToInt16(reader["Sucursal"]), año,
//                                                                Convert.ToInt16(reader["MesConciliacion"]),
//                                                                Convert.ToInt16(reader["FolioConciliacion"]),
//                                                                Convert.ToInt16(reader["Folio"]),
//                                                                Convert.ToInt16(reader["Secuencia"]),
//                                                                Convert.ToInt16(reader["ConInterno"]),
//                                                                Convert.ToInt16(reader["Año"])),

//                                                            Convert.ToString(reader["Cheque"]),
//                                                            Convert.ToString(reader["Referencia"]),
//                                                            Convert.ToString(reader["NombreTercero"]),
//                                                            Convert.ToString(reader["RFCTercero"]), sucursal,
//                                                            Convert.ToInt16(reader["Año"]), this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<cReferencia> ConsultaTransaccionesConciliadasDetalle(int corporativoconciliacion,
//                                                                                  int sucursalconciliacion,
//                                                                                  int añoconciliacion,
//                                                                                  short mesconciliacion,
//                                                                                  int folioconciliacion,
//                                                                                  int folioexterno, int secuenciaexterno,
//                                                                                  short cointerno, int añoexterno)
//        {

//            List<cReferencia> datos = new List<cReferencia>();

//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaTransaccionesConciliadasDetalle", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = cointerno;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativoconciliacion;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        sucursalconciliacion;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = añoconciliacion;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mesconciliacion;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folioconciliacion;
//                    comando.Parameters.Add("@AñoExterno", System.Data.SqlDbType.Int).Value = añoexterno;
//                    comando.Parameters.Add("@Folio", System.Data.SqlDbType.Int).Value = folioexterno;
//                    comando.Parameters.Add("@Secuencia", System.Data.SqlDbType.Int).Value = secuenciaexterno;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        if (cointerno == 0)
//                        {

//                            ReferenciaConciliadaPedido dato =
//                                new ReferenciaConciliadaPedidoDatos(Convert.ToInt32(reader["Corporativo"]),
//                                                                    Convert.ToInt32(reader["Año"]),
//                                                                    Convert.ToInt16(reader["Mes"]),
//                                                                    Convert.ToInt32(reader["FolioConciliacion"]),
//                                                                    Convert.ToInt16(reader["SucursalExterna"]),
//                                                                    Convert.ToString(reader["SucursalDesExt"]),
//                                                                    Convert.ToInt32(reader["Folio"]),
//                                                                    Convert.ToInt32(reader["Secuencia"]),
//                                                                    Convert.ToString(reader["Concepto"]),
//                                                                    Convert.ToDecimal(reader["MontoConciliado"]),
//                                                                    Convert.ToDecimal(reader["Diferencia"]),
//                                                                    Convert.ToInt16(reader["FormaConciliacion"]),
//                                                                    Convert.ToInt16(reader["StatusConcepto"]),
//                                                                    Convert.ToString(reader["StatusConciliacion"]),
//                                                                    Convert.ToDateTime(reader["FOperacion"]),
//                                                                    Convert.ToDateTime(reader["FMovimiento"]),

//                                                                    "", "",
//                                                                    "", "", "",
//                                                                    0, 0,

//                                                                    Convert.ToInt16(reader["SucursalInt"]),
//                                                                    Convert.ToString(reader["SucursalDesInt"]),
//                                                                    Convert.ToInt32(reader["Celula"]),
//                                                                    Convert.ToInt32(reader["Añoped"]),
//                                                                    Convert.ToInt32(reader["Pedido"]),
//                                                                    Convert.ToInt32(reader["RemisionPedido"]),
//                                                                    Convert.ToString(reader["SeriePedido"]),
//                                                                    Convert.ToInt32(reader["FolioSat"]),
//                                                                    Convert.ToString(reader["SerieSat"]),
//                                                                    Convert.ToString(reader["ConceptoInterno"]),
//                                                                    Convert.ToDecimal(reader["Total"]),
//                                                                    Convert.ToString(reader["StatusMovimiento"]),
//                                                                    Convert.ToInt32(reader["Cliente"]),
//                                                                    Convert.ToString(reader["Nombre"]),
//                                                                    Convert.ToInt32(reader["AñoExterno"]),
//                                                                    this.implementadorMensajes);
//                            datos.Add(dato);
//                        }
//                        else
//                        {

//                            ReferenciaConciliada dato =
//                                new ReferenciaConciliadaDatos(Convert.ToInt32(reader["Corporativo"]),
//                                                              Convert.ToInt32(reader["Año"]),
//                                                              Convert.ToInt16(reader["Mes"]),
//                                                              Convert.ToInt32(reader["FolioConciliacion"]),
//                                                              Convert.ToInt16(reader["SucursalExterna"]),
//                                                              Convert.ToString(reader["SucursalDesExt"]),
//                                                              Convert.ToInt32(reader["Folio"]),
//                                                              Convert.ToInt32(reader["Secuencia"]),
//                                                              Convert.ToString(reader["Concepto"]),
//                                                              Convert.ToDecimal(reader["MontoConciliado"]),
//                                                              Convert.ToDecimal(reader["Diferencia"]),
//                                                              Convert.ToInt16(reader["FormaConciliacion"]),
//                                                              Convert.ToInt16(reader["StatusConcepto"]),
//                                                              Convert.ToString(reader["StatusConciliacion"]),
//                                                              Convert.ToDateTime(reader["FOperacion"]),
//                                                              Convert.ToDateTime(reader["FMovimiento"]),

//                                                              "", "",
//                                                              "", "", "",
//                                                              0, 0,

//                                                              Convert.ToInt16(reader["SucursalInt"]),
//                                                              Convert.ToString(reader["SucursalDesInt"]),
//                                                              Convert.ToInt32(reader["Folioint"]),
//                                                              Convert.ToInt32(reader["Secuenciaint"]),
//                                                              Convert.ToString(reader["ConceptoInterno"]),
//                                                              Convert.ToDecimal(reader["MontoInterno"]),
//                                                              Convert.ToDateTime(reader["foperint"]),
//                                                              Convert.ToDateTime(reader["fmovint"]),

//                                                              "", "",
//                                                              "", "", "",
//                                                              0, 0,
//                                                              Convert.ToInt32(reader["AñoExterno"]),
//                                                              Convert.ToInt32(reader["AñoInterno"]),

//                                                              this.implementadorMensajes);
//                            datos.Add(dato);
//                        }

//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override cConciliacion ConsultaConciliacionDetalle(int corporativo, int sucursal, int año, short mes,
//                                                                  int folioconciliacion)
//        {
//            cConciliacion conciliacion = new ConciliacionDatos(this.implementadorMensajes);
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaConciliacionDetalle", cnn);
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.TinyInt).Value = corporativo;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@MesConciliacion ", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@FolioConciliacion ", System.Data.SqlDbType.SmallInt).Value =
//                        folioconciliacion;
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        conciliacion.Corporativo = Convert.ToInt32(reader["CorporativoConciliacion"]);
//                        conciliacion.CorporativoDes = reader["CorporativoDes"].ToString();
//                        conciliacion.Sucursal = Convert.ToInt32(reader["SucursalConciliacion"]);
//                        conciliacion.SucursalDes = reader["SucursalDes"].ToString();
//                        conciliacion.Año = Convert.ToInt32(reader["AñoConciliacion"]);
//                        conciliacion.Mes = Convert.ToInt16(reader["MesConciliacion"]);
//                        conciliacion.Folio = Convert.ToInt32(reader["FolioConciliacion"]);
//                        conciliacion.GrupoConciliacionStr = Convert.ToString(reader["GrupoConciliacionstr"]);
//                        conciliacion.TipoConciliacionStr = reader["TipoConciliacionstr"].ToString();
//                        conciliacion.StatusConciliacion = reader["StatusConciliacion"].ToString();
//                        conciliacion.TransaccionesInternas = Convert.ToInt32(reader["TransaccionesInternas"]);
//                        conciliacion.ConciliadasInternas = Convert.ToInt32(reader["ConciliadasInternas"]);
//                        conciliacion.TransaccionesExternas = Convert.ToInt32(reader["TransaccionesExternas"]);
//                        conciliacion.ConciliadasExternas = Convert.ToInt32(reader["ConciliadasExternas"]);
//                        conciliacion.MontoTotalExterno = Convert.ToDecimal(reader["MontoTotalExterno"]);
//                        conciliacion.MontoTotalInterno = Convert.ToDecimal(reader["MontoTotalInterno"]);
//                        conciliacion.CuentaBancaria = reader["CuentaBancoFinanciero"].ToString();
//                        conciliacion.BancoStr = reader["Bancostr"].ToString();
//                        conciliacion.UbicacionIcono = reader["UbicacionIcono"].ToString();

//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return conciliacion;
//            }

//        }

//        public override List<ReferenciaNoConciliada> ConsultaDetalleExternoCanceladoPendiente(
//            ConsultaExterno configuracion, int corporativoconciliacion, short sucursalconciliacion, int añoconciliacion,
//            short mesconciliacion, int folioconciliacion, short sucursalinterno, int foliointerno, int secuenciainterno,
//            int statusconcepto, decimal diferencia)
//        {
//            List<ReferenciaNoConciliada> datos = new List<ReferenciaNoConciliada>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaRegistrosCanceladosPendientesExternos", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativoconciliacion;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        sucursalconciliacion;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = añoconciliacion;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mesconciliacion;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folioconciliacion;
//                    comando.Parameters.Add("@SucursalInterno", System.Data.SqlDbType.Int).Value = sucursalinterno;
//                    comando.Parameters.Add("@FolioInterno", System.Data.SqlDbType.Int).Value = foliointerno;
//                    comando.Parameters.Add("@SecuenciaInterno", System.Data.SqlDbType.VarChar).Value = secuenciainterno;
//                    comando.Parameters.Add("@StatusConcepto", System.Data.SqlDbType.Int).Value = statusconcepto;
//                    comando.Parameters.Add("@Cadena", System.Data.SqlDbType.VarChar).Value =
//                        App.Consultas.ObtenerCadenaDeEtiquetas(0, corporativoconciliacion, sucursalconciliacion,
//                                                               añoconciliacion, mesconciliacion, folioconciliacion,
//                                                               statusconcepto);
//                    comando.Parameters.Add("@Diferencia", System.Data.SqlDbType.Decimal).Value = diferencia;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaNoConciliada dato =
//                            new ReferenciaNoConciliadaDatos(Convert.ToInt16(reader["Corporativo"]),
//                                                            Convert.ToInt16(reader["Sucursal"]),
//                                                            Convert.ToString(reader["SucursalDes"]), añoconciliacion,
//                                                            Convert.ToInt16(reader["Folio"]),
//                                                            Convert.ToInt16(reader["Secuencia"]),
//                                                            Convert.ToString(reader["Descripcion"]),
//                                                            Convert.ToString(reader["Concepto"]),
//                                                            Convert.ToDecimal(reader["Deposito"]),
//                                                            Convert.ToDecimal(reader["Retiro"]),
//                                                            Convert.ToInt16(reader["FormaConciliacion"]),
//                                                            Convert.ToInt16(reader["StatusConcepto"]),
//                                                            Convert.ToString(reader["StatusConciliacion"]),
//                                                            Convert.ToDateTime(reader["FOperacion"]),
//                                                            Convert.ToDateTime(reader["FMovimiento"]),
//                                                            Convert.ToString(reader["UbicacionIcono"]),
//                                                            Convert.ToInt32(reader["FolioConciliacion"]),
//                                                            Convert.ToInt16(reader["MesConciliacion"]), diferencia,
//                                                            Convert.ToBoolean(reader["ConInterno"]),
//                                                            Convert.ToString(reader["Cheque"]),
//                                                            Convert.ToString(reader["Referencia"]),
//                                                            Convert.ToString(reader["NombreTercero"]),
//                                                            Convert.ToString(reader["RFCTercero"]), sucursalconciliacion,
//                                                            Convert.ToInt16(reader["Año"]), this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaNoConciliada> ConsultaTransaccionesRegistradas(
//            ConfiguracionConsultaConciliaciones configuracion, int corporativoconciliacion, short sucursalconciliacion,
//            int añoconciliacion, short mesconciliacion, int folioconciliacion, int statusconcepto,
//            short formaconciliacion)
//        {
//            List<ReferenciaNoConciliada> datos = new List<ReferenciaNoConciliada>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaTransaccionesRegistradas", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativoconciliacion;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        sucursalconciliacion;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = añoconciliacion;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mesconciliacion;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folioconciliacion;
//                    comando.Parameters.Add("@StatusConcepto", System.Data.SqlDbType.SmallInt).Value = statusconcepto;
//                    comando.Parameters.Add("@FormaConciliacion", System.Data.SqlDbType.SmallInt).Value =
//                        formaconciliacion;
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        if (Convert.ToString(reader["Tipo"]) == "CANCELADO INTERNO")
//                        {
//                            ReferenciaNoConciliada dato =
//                                new ReferenciaNoConciliadaDatos(Convert.ToInt16(reader["Corporativo"]),
//                                                                Convert.ToInt16(reader["Sucursal"]),
//                                                                Convert.ToString(reader["SucursalDes"]), añoconciliacion,
//                                                                Convert.ToInt16(reader["Folio"]),
//                                                                Convert.ToInt16(reader["Secuencia"]),
//                                                                Convert.ToString(reader["Descripcion"]),
//                                                                Convert.ToString(reader["Concepto"]),
//                                                                Convert.ToDecimal(reader["Deposito"]),
//                                                                Convert.ToDecimal(reader["Retiro"]),
//                                                                Convert.ToInt16(reader["FormaConciliacion"]),
//                                                                Convert.ToInt16(reader["StatusConcepto"]),
//                                                                Convert.ToString(reader["StatusConciliacion"]),
//                                                                Convert.ToDateTime(reader["FOperacion"]),
//                                                                Convert.ToDateTime(reader["FMovimiento"]),
//                                                                Convert.ToInt16(reader["FolioConciliacion"]),
//                                                                Convert.ToInt16(reader["MesConciliacion"]),
//                                                                Convert.ToBoolean(reader["ConInterno"]),
//                                                                null,

//                                                                Convert.ToString(reader["Cheque"]),
//                                                                Convert.ToString(reader["Referencia"]),
//                                                                Convert.ToString(reader["NombreTercero"]),
//                                                                Convert.ToString(reader["RFCTercero"]),
//                                                                sucursalconciliacion, Convert.ToString(reader["Tipo"]),
//                                                                Convert.ToString(reader["UbicacionIcono"]),
//                                                                Convert.ToInt16(reader["Año"]),
//                                                                this.implementadorMensajes);
//                            datos.Add(dato);
//                        }
//                        else
//                        {
//                            ReferenciaNoConciliada dato =
//                                new ReferenciaNoConciliadaDatos(Convert.ToInt16(reader["Corporativo"]),
//                                                                Convert.ToInt16(reader["Sucursal"]),
//                                                                Convert.ToString(reader["SucursalDes"]), añoconciliacion,
//                                                                Convert.ToInt16(reader["Folio"]),
//                                                                Convert.ToInt16(reader["Secuencia"]),
//                                                                Convert.ToString(reader["Descripcion"]),
//                                                                Convert.ToString(reader["Concepto"]),
//                                                                Convert.ToDecimal(reader["Deposito"]),
//                                                                Convert.ToDecimal(reader["Retiro"]),
//                                                                Convert.ToInt16(reader["FormaConciliacion"]),
//                                                                Convert.ToInt16(reader["StatusConcepto"]),
//                                                                Convert.ToString(reader["StatusConciliacion"]),
//                                                                Convert.ToDateTime(reader["FOperacion"]),
//                                                                Convert.ToDateTime(reader["FMovimiento"]),
//                                                                Convert.ToInt16(reader["FolioConciliacion"]),
//                                                                Convert.ToInt16(reader["MesConciliacion"]),
//                                                                Convert.ToBoolean(reader["ConInterno"]),
//                                                                ConsultaTransaccionesConciliadasDetalle(
//                                                                    Convert.ToInt16(reader["Corporativo"]),
//                                                                    Convert.ToInt16(reader["Sucursal"]), añoconciliacion,
//                                                                    Convert.ToInt16(reader["MesConciliacion"]),
//                                                                    Convert.ToInt16(reader["FolioConciliacion"]),
//                                                                    Convert.ToInt16(reader["Folio"]),
//                                                                    Convert.ToInt16(reader["Secuencia"]),
//                                                                    Convert.ToInt16(reader["ConInterno"]),
//                                                                    Convert.ToInt16(reader["Año"])),

//                                                                Convert.ToString(reader["Cheque"]),
//                                                                Convert.ToString(reader["Referencia"]),
//                                                                Convert.ToString(reader["NombreTercero"]),
//                                                                Convert.ToString(reader["RFCTercero"]),
//                                                                sucursalconciliacion, Convert.ToString(reader["Tipo"]),
//                                                                Convert.ToString(reader["UbicacionIcono"]),
//                                                                Convert.ToInt16(reader["Año"]),
//                                                                this.implementadorMensajes);
//                            datos.Add(dato);
//                        }

//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaNoConciliada> ConsultaTrasaccionesExternasPendientes(int corporativoconciliacion,
//                                                                                            short sucursalconciliacion,
//                                                                                            int añoconciliacion,
//                                                                                            short mesconciliacion,
//                                                                                            int folioconciliacion,
//                                                                                            int statusconcepto)
//        {
//            List<ReferenciaNoConciliada> datos = new List<ReferenciaNoConciliada>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaTrasaccionesExternasPendientes", cnn);
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativoconciliacion;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        sucursalconciliacion;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = añoconciliacion;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mesconciliacion;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folioconciliacion;
//                    comando.Parameters.Add("@StatusConcepto", System.Data.SqlDbType.Int).Value = statusconcepto;
//                    comando.Parameters.Add("@Cadena", System.Data.SqlDbType.VarChar).Value =
//                        App.Consultas.ObtenerCadenaDeEtiquetas(0, corporativoconciliacion, sucursalconciliacion,
//                                                               añoconciliacion, mesconciliacion, folioconciliacion,
//                                                               statusconcepto);

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaNoConciliada dato =
//                            new ReferenciaNoConciliadaDatos(Convert.ToInt16(reader["Corporativo"]),
//                                                            Convert.ToInt16(reader["Sucursal"]),
//                                                            Convert.ToString(reader["SucursalDes"]), añoconciliacion,
//                                                            Convert.ToInt16(reader["Folio"]),
//                                                            Convert.ToInt16(reader["Secuencia"]),
//                                                            Convert.ToString(reader["Descripcion"]),
//                                                            Convert.ToString(reader["Concepto"]),
//                                                            Convert.ToDecimal(reader["Deposito"]),
//                                                            Convert.ToDecimal(reader["Retiro"]),
//                                                            Convert.ToInt16(reader["FormaConciliacion"]),
//                                                            Convert.ToInt16(reader["StatusConcepto"]),
//                                                            Convert.ToString(reader["StatusConciliacion"]),
//                                                            Convert.ToDateTime(reader["FOperacion"]),
//                                                            Convert.ToDateTime(reader["FMovimiento"]),
//                                                            Convert.ToString(reader["UbicacionIcono"]),
//                                                            folioconciliacion, mesconciliacion, 0, true,
//                                                            Convert.ToString(reader["Cheque"]),
//                                                            Convert.ToString(reader["Referencia"]),
//                                                            Convert.ToString(reader["NombreTercero"]),
//                                                            Convert.ToString(reader["RFCTercero"]), sucursalconciliacion,
//                                                            Convert.ToInt16(reader["Año"]), this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaNoConciliada> ConsultaTrasaccionesInternasPendientes(Configuracion configuracion,
//                                                                                            int corporativoconciliacion,
//                                                                                            short sucursalconciliacion,
//                                                                                            int añoconciliacion,
//                                                                                            short mesconciliacion,
//                                                                                            int folioconciliacion,
//                                                                                            int statusconcepto,
//                                                                                            short sucursalinterno)
//        {
//            List<ReferenciaNoConciliada> datos = new List<ReferenciaNoConciliada>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaTrasaccionesInternasPendientes", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativoconciliacion;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        sucursalconciliacion;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = añoconciliacion;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mesconciliacion;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folioconciliacion;
//                    comando.Parameters.Add("@SucursalInterno", System.Data.SqlDbType.SmallInt).Value = sucursalinterno;
//                    comando.Parameters.Add("@StatusConcepto", System.Data.SqlDbType.Int).Value = statusconcepto;
//                    comando.Parameters.Add("@Cadena", System.Data.SqlDbType.VarChar).Value =
//                        App.Consultas.ObtenerCadenaDeEtiquetas(1, corporativoconciliacion, sucursalconciliacion,
//                                                               añoconciliacion, mesconciliacion, folioconciliacion,
//                                                               statusconcepto);

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaNoConciliada dato =
//                            new ReferenciaNoConciliadaDatos(Convert.ToInt16(reader["Corporativo"]),
//                                                            Convert.ToInt16(reader["Sucursal"]),
//                                                            Convert.ToString(reader["SucursalDes"]), añoconciliacion,
//                                                            Convert.ToInt16(reader["Folio"]),
//                                                            Convert.ToInt16(reader["Secuencia"]),
//                                                            Convert.ToString(reader["Descripcion"]),
//                                                            Convert.ToString(reader["Concepto"]),
//                                                            Convert.ToDecimal(reader["Deposito"]),
//                                                            Convert.ToDecimal(reader["Retiro"]),
//                                                            Convert.ToInt16(reader["FormaConciliacion"]),
//                                                            Convert.ToInt16(reader["StatusConcepto"]),
//                                                            Convert.ToString(reader["StatusConciliacion"]),
//                                                            Convert.ToDateTime(reader["FOperacion"]),
//                                                            Convert.ToDateTime(reader["FMovimiento"]),
//                                                            Convert.ToString(reader["UbicacionIcono"]),
//                                                            folioconciliacion, mesconciliacion, 0, true,
//                                                            Convert.ToString(reader["Cheque"]),
//                                                            Convert.ToString(reader["Referencia"]),
//                                                            Convert.ToString(reader["NombreTercero"]),
//                                                            Convert.ToString(reader["RFCTercero"]), sucursalconciliacion,
//                                                            Convert.ToInt16(reader["Año"]), this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaNoConciliada> ConsultaDetalleInternoCanceladoPendiente(
//            ConciliacionInterna configuracion, int corporativoconciliacion, short sucursalconciliacion,
//            int añoconciliacion, short mesconciliacion, int folioconciliacion, int folioexterno, int secuenciaexterno,
//            decimal diferencia)
//        {
//            List<ReferenciaNoConciliada> datos = new List<ReferenciaNoConciliada>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaRegistrosCanceladosPendientesInternos", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativoconciliacion;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        sucursalconciliacion;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = añoconciliacion;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mesconciliacion;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folioconciliacion;
//                    comando.Parameters.Add("@FolioExterno", System.Data.SqlDbType.Int).Value = folioexterno;
//                    comando.Parameters.Add("@SecuenciaExterno", System.Data.SqlDbType.VarChar).Value = secuenciaexterno;
//                    comando.Parameters.Add("@Diferencia", System.Data.SqlDbType.Decimal).Value = diferencia;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaNoConciliada dato =
//                            new ReferenciaNoConciliadaDatos(Convert.ToInt16(reader["Corporativo"]),
//                                                            Convert.ToInt16(reader["Sucursal"]),
//                                                            Convert.ToString(reader["SucursalDes"]), añoconciliacion,
//                                                            Convert.ToInt16(reader["Folio"]),
//                                                            Convert.ToInt16(reader["Secuencia"]),
//                                                            Convert.ToString(reader["Descripcion"]),
//                                                            Convert.ToString(reader["Concepto"]),
//                                                            Convert.ToDecimal(reader["Deposito"]),
//                                                            Convert.ToDecimal(reader["Retiro"]),
//                                                            Convert.ToInt16(reader["FormaConciliacion"]),
//                                                            Convert.ToInt16(reader["StatusConcepto"]),
//                                                            Convert.ToString(reader["StatusConciliacion"]),
//                                                            Convert.ToDateTime(reader["FOperacion"]),
//                                                            Convert.ToDateTime(reader["FMovimiento"]),
//                                                            Convert.ToString(reader["UbicacionIcono"]),
//                                                            Convert.ToInt32(reader["FolioConciliacion"]),
//                                                            Convert.ToInt16(reader["MesConciliacion"]), 0, true,

//                                                            Convert.ToString(reader["Cheque"]),
//                                                            Convert.ToString(reader["Referencia"]),
//                                                            Convert.ToString(reader["NombreTercero"]),
//                                                            Convert.ToString(reader["RFCTercero"]), sucursalconciliacion,
//                                                            Convert.ToInt16(reader["Año"]), this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }



//        public override List<ReferenciaConciliadaPedido> ConsultaPagosPorAplicar(int corporativo, short sucursal,
//                                                                                 int año, short mes, int folio)
//        {
//            List<ReferenciaConciliadaPedido> datos = new List<ReferenciaConciliadaPedido>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaPagosPorAplicar", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = 0;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativo;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folio;
//                    comando.Parameters.Add("@Cliente", System.Data.SqlDbType.Int).Value = 0;
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaConciliadaPedido dato =
//                            new ReferenciaConciliadaPedidoDatos(Convert.ToInt16(reader["CorporativoConciliacion"]),
//                                                                Convert.ToInt32(reader["AñoConciliacion"]),
//                                                                Convert.ToInt16(reader["MesConciliacion"]),
//                                                                Convert.ToInt32(reader["FolioConciliacion"]),
//                                                                Convert.ToInt16(reader["SucursalExt"]),
//                                                                Convert.ToString(reader["SucursalExtDes"]),
//                                                                Convert.ToInt32(reader["FolioExt"]),
//                                                                Convert.ToInt32(reader["SecuenciaExt"]),
//                                                                Convert.ToString(reader["ConceptoExt"]),
//                                                                Convert.ToDecimal(reader["MontoConciliado"]),
//                                                                Convert.ToDecimal(reader["Diferencia"]),
//                                                                Convert.ToInt16(reader["FormaConciliacion"]),
//                                                                Convert.ToInt16(reader["StatusConcepto"]),
//                                                                Convert.ToString(reader["StatusConciliacion"]),
//                                                                Convert.ToDateTime(reader["FOperacionExt"]),
//                                                                Convert.ToDateTime(reader["FMovimientoExt"]),

//                                                                Convert.ToString(reader["Cheque"]),
//                                                                Convert.ToString(reader["Referencia"]),
//                                                                Convert.ToString(reader["Descripcion"]),
//                                                                Convert.ToString(reader["NombreTercero"]),
//                                                                Convert.ToString(reader["RFCTercero"]),
//                                                                Convert.ToDecimal(reader["Deposito"]),
//                                                                Convert.ToDecimal(reader["Retiro"]),

//                                                                Convert.ToInt16(reader["SucursalPedido"]),
//                                                                Convert.ToString(reader["SucursalPedidoDes"]),
//                                                                Convert.ToInt32(reader["CelulaPedido"]),
//                                                                Convert.ToInt32(reader["AñoPedido"]),
//                                                                Convert.ToInt32(reader["Pedido"]),
//                                                                Convert.ToInt32(reader["RemisionPedido"]),
//                                                                Convert.ToString(reader["SeriePedido"]),
//                                                                Convert.ToInt32(reader["FolioSat"]),
//                                                                Convert.ToString(reader["SerieSat"]),
//                                                                Convert.ToString(reader["ConceptoPedido"]),
//                                                                Convert.ToDecimal(reader["Total"]),
//                                                                Convert.ToString(reader["StatusMovimiento"]),
//                                                                Convert.ToInt32(reader["Cliente"]),
//                                                                Convert.ToString(reader["Nombre"]),
//                                                                Convert.ToString(reader["PedidoReferencia"]),
//                                                                Convert.ToInt32(reader["AñoExterno"]),
//                                                                this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaConciliadaPedido> ConsultaPagosPorAplicarCliente(int corporativo, short sucursal,
//                                                                                        int año, short mes, int folio,
//                                                                                        int cliente)
//        {
//            List<ReferenciaConciliadaPedido> datos = new List<ReferenciaConciliadaPedido>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaPagosPorAplicar", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = 1;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativo;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folio;
//                    comando.Parameters.Add("@Cliente", System.Data.SqlDbType.Int).Value = cliente;
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaConciliadaPedido dato =
//                            new ReferenciaConciliadaPedidoDatos(Convert.ToInt16(reader["CorporativoConciliacion"]),
//                                                                Convert.ToInt32(reader["AñoConciliacion"]),
//                                                                Convert.ToInt16(reader["MesConciliacion"]),
//                                                                Convert.ToInt32(reader["FolioConciliacion"]),
//                                                                Convert.ToInt16(reader["SucursalExt"]),
//                                                                Convert.ToString(reader["SucursalExtDes"]),
//                                                                Convert.ToInt32(reader["FolioExt"]),
//                                                                Convert.ToInt32(reader["SecuenciaExt"]),
//                                                                Convert.ToString(reader["ConceptoExt"]),
//                                                                Convert.ToDecimal(reader["MontoConciliado"]),
//                                                                Convert.ToDecimal(reader["Diferencia"]),
//                                                                Convert.ToInt16(reader["FormaConciliacion"]),
//                                                                Convert.ToInt16(reader["StatusConcepto"]),
//                                                                Convert.ToString(reader["StatusConciliacion"]),
//                                                                Convert.ToDateTime(reader["FOperacionExt"]),
//                                                                Convert.ToDateTime(reader["FMovimientoExt"]),

//                                                                Convert.ToString(reader["Cheque"]),
//                                                                Convert.ToString(reader["Referencia"]),
//                                                                Convert.ToString(reader["Descripcion"]),
//                                                                Convert.ToString(reader["NombreTercero"]),
//                                                                Convert.ToString(reader["RFCTercero"]),
//                                                                Convert.ToDecimal(reader["Deposito"]),
//                                                                Convert.ToDecimal(reader["Retiro"]),

//                                                                Convert.ToInt16(reader["SucursalPedido"]),
//                                                                Convert.ToString(reader["SucursalPedidoDes"]),
//                                                                Convert.ToInt32(reader["CelulaPedido"]),
//                                                                Convert.ToInt32(reader["AñoPedido"]),
//                                                                Convert.ToInt32(reader["Pedido"]),
//                                                                Convert.ToInt32(reader["RemisionPedido"]),
//                                                                Convert.ToString(reader["SeriePedido"]),
//                                                                Convert.ToInt32(reader["FolioSat"]),
//                                                                Convert.ToString(reader["SerieSat"]),
//                                                                Convert.ToString(reader["ConceptoPedido"]),
//                                                                Convert.ToDecimal(reader["Total"]),
//                                                                Convert.ToString(reader["StatusMovimiento"]),
//                                                                Convert.ToInt32(reader["Cliente"]),
//                                                                Convert.ToString(reader["Nombre"]),
//                                                                Convert.ToString(reader["PedidoReferencia"]),
//                                                                Convert.ToInt32(reader["AñoExterno"]),
//                                                                this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<Cobro> ConsultaChequeTarjetaAltaModifica(int corporativo, short sucursal, int año,
//                                                                      short mes, int folio)
//        {
//            List<Cobro> datos = new List<Cobro>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaChequeTarjetaAltaModifica", cnn);
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.Int).Value = corporativo;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.Int).Value = sucursal;
//                    comando.Parameters.Add("@Año", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@Mes", System.Data.SqlDbType.Int).Value = mes;
//                    comando.Parameters.Add("@Folio", System.Data.SqlDbType.Int).Value = folio;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        Cobro dato = new CobroDatos(Convert.ToInt16(reader["AñoCobro"]),
//                                                    Convert.ToInt32(reader["Cobro"]),
//                                                    Convert.ToString(reader["NumeroCheque"]),
//                                                    Convert.ToDecimal(reader["Total"]),
//                                                    Convert.ToDecimal(reader["Saldo"]),
//                                                    Convert.ToString(reader["NumeroCuenta"]),
//                                                    Convert.ToString(reader["NumeroCuentaDestino"]),
//                                                    Convert.ToDateTime(reader["FCheque"]),
//                                                    Convert.ToInt32(reader["Cliente"]), Convert.ToInt16(reader["Banco"]),
//                                                    Convert.ToInt16(reader["BancoOrigen"]),
//                                                    Convert.ToString(reader["Observaciones"]),
//                                                    Convert.ToString(reader["Status"]),
//                                                    Convert.ToInt16(reader["TipoCobro"]),
//                                                    Convert.ToBoolean(reader["Alta"]),
//                                                    Convert.ToString(reader["Usuario"]),
//                                                    Convert.ToBoolean(reader["SaldoAFavor"]),
//                                                    ConsultaPagosPorAplicarCliente(corporativo, sucursal, año, mes,
//                                                                                   folio,
//                                                                                   Convert.ToInt32(reader["Cliente"])),
//                                                    this.implementadorMensajes);
//                        datos.Add(dato);



//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override MovimientoCaja ConsultaMovimientoCajaAlta(int corporativo, short sucursal, int año, short mes,
//                                                                  int folio)
//        {
//            MovimientoCaja movimiento = new MovimientoCajaDatos(this.implementadorMensajes);
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaMovimientoCajaAlta", cnn);
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.Int).Value = corporativo;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.Int).Value = sucursal;
//                    comando.Parameters.Add("@Año", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@Mes", System.Data.SqlDbType.Int).Value = mes;
//                    comando.Parameters.Add("@Folio", System.Data.SqlDbType.Int).Value = folio;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        movimiento.Caja = Convert.ToInt16(reader["Caja"]);
//                        movimiento.FOperacion = Convert.ToDateTime(reader["FOperacion"]);
//                        movimiento.Consecutivo = Convert.ToInt16(reader["Consecutivo"]);
//                        movimiento.Folio = Convert.ToInt32(reader["Folio"]);
//                        movimiento.FMovimiento = Convert.ToDateTime(reader["FMovimiento"]);
//                        movimiento.Total = Convert.ToDecimal(reader["Total"]);
//                        movimiento.Usuario = reader["Usuario"].ToString();
//                        movimiento.Empleado = Convert.ToInt32(reader["Empleado"]);
//                        movimiento.Observaciones = reader["Observaciones"].ToString();
//                        movimiento.SaldoAFavor = Convert.ToDecimal(reader["SaldoAFavor"]);
//                        movimiento.ListaCobros = ConsultaChequeTarjetaAltaModifica(corporativo, sucursal, año, mes,
//                                                                                   folio);
//                        movimiento.ListaPedidos = ConsultaPagosPorAplicar(corporativo, sucursal, año, mes, folio);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Error al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//            }
//            return movimiento;
//        }

//        public override bool BorrarTransaccionesNoCorrespondidas(int corporativoconciliacion, short sucursalconciliacion,
//                                                                 int añoconciliacion, short mesconciliacion,
//                                                                 int folioconciliacion)
//        {
//            bool resultado = false;
//            try
//            {
//                using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBActualizaConciliacion", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = 3;
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.TinyInt).Value =
//                        corporativoconciliacion;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.TinyInt).Value = sucursalconciliacion;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = añoconciliacion;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mesconciliacion;
//                    comando.Parameters.Add("@FolioConciliacion ", System.Data.SqlDbType.Int).Value = folioconciliacion;
//                    comando.Parameters.Add("@FolioExterno", System.Data.SqlDbType.Int).Value = 0;
//                    comando.Parameters.Add("@UsuarioAlta", System.Data.SqlDbType.VarChar).Value = "";
//                    comando.Parameters.Add("@GrupoConciliacion", System.Data.SqlDbType.SmallInt).Value = 0;
//                    comando.Parameters.Add("@CuentaBancoFinanciero", System.Data.SqlDbType.VarChar).Value = "";
//                    comando.Parameters.Add("@TipoConciliacion", System.Data.SqlDbType.SmallInt).Value = 0;
//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    comando.ExecuteNonQuery();
//                    resultado = true;
//                }

//            }
//            catch (SqlException ex)
//            {
//                stackTrace = new StackTrace();
//                this.ImplementadorMensajes.MostrarMensaje("No se pudo borrar el registro.\n\rClase :" +
//                                                          this.GetType().Name + "\n\r" + "Metodo :" +
//                                                          stackTrace.GetFrame(0).GetMethod().Name + "\n\r" + "Error :" +
//                                                          ex.Message);
//                stackTrace = null;
//            }
//            return resultado;
//        }

//        public override List<ReferenciaNoConciliada> ConsultaDetalleExternoPendienteDeposito(
//            ConsultaExterno configuracion, int corporativo, short sucursal, int año, short mes, int folio,
//            decimal diferencia, int statusconcepto, bool deposito)
//        {
//            bool coninterno =
//                !(configuracion == ConsultaExterno.DepositosConReferenciaPedido ||
//                  configuracion == ConsultaExterno.DepositosPedido);

//            List<ReferenciaNoConciliada> datos = new List<ReferenciaNoConciliada>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConciliacionPendienteExternoTipo", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativo;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value = sucursal;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folio;
//                    comando.Parameters.Add("@StatusConcepto", System.Data.SqlDbType.Int).Value = statusconcepto;
//                    comando.Parameters.Add("@Cadena", System.Data.SqlDbType.VarChar).Value =
//                        App.Consultas.ObtenerCadenaDeEtiquetas(0, corporativo, sucursal, año, mes, folio, statusconcepto);
//                    comando.Parameters.Add("@Deposito", System.Data.SqlDbType.Bit).Value = deposito;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaNoConciliada dato =
//                            new ReferenciaNoConciliadaDatos(Convert.ToInt16(reader["Corporativo"]),
//                                                            Convert.ToInt16(reader["Sucursal"]),
//                                                            Convert.ToString(reader["SucursalDes"]), año,
//                                                            Convert.ToInt16(reader["Folio"]),
//                                                            Convert.ToInt16(reader["Secuencia"]),
//                                                            Convert.ToString(reader["Descripcion"]),
//                                                            Convert.ToString(reader["Concepto"]),
//                                                            Convert.ToDecimal(reader["Deposito"]),
//                                                            Convert.ToDecimal(reader["Retiro"]),
//                                                            Convert.ToInt16(reader["FormaConciliacion"]),
//                                                            Convert.ToInt16(reader["StatusConcepto"]),
//                                                            Convert.ToString(reader["StatusConciliacion"]),
//                                                            Convert.ToDateTime(reader["FOperacion"]),
//                                                            Convert.ToDateTime(reader["FMovimiento"]),
//                                                            Convert.ToString(reader["UbicacionIcono"]),
//                                                            folio, mes, diferencia, coninterno,
//                                                            Convert.ToString(reader["Cheque"]),
//                                                            Convert.ToString(reader["Referencia"]),
//                                                            Convert.ToString(reader["NombreTercero"]),
//                                                            Convert.ToString(reader["RFCTercero"]), sucursal,
//                                                            Convert.ToInt16(reader["Año"]), this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ReferenciaNoConciliada> ConsultaDetalleInternoPendienteDeposito(
//            ConciliacionInterna configuracion, int corporativoconciliacion, short sucursalconciliacion,
//            int añoconciliacion, short mesconciliacion, int folioconciliacion, short sucursalinterno, short dias,
//            decimal diferencia, int statusconcepto, decimal monto, bool deposito, DateTime fminima, DateTime fmaxima)
//        {
//            List<ReferenciaNoConciliada> datos = new List<ReferenciaNoConciliada>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConciliacionBusquedaInternaTipo", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@CorporativoConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        corporativoconciliacion;
//                    comando.Parameters.Add("@SucursalConciliacion", System.Data.SqlDbType.TinyInt).Value =
//                        sucursalconciliacion;
//                    comando.Parameters.Add("@AñoConciliacion", System.Data.SqlDbType.Int).Value = añoconciliacion;
//                    comando.Parameters.Add("@MesConciliacion", System.Data.SqlDbType.SmallInt).Value = mesconciliacion;
//                    comando.Parameters.Add("@FolioConciliacion", System.Data.SqlDbType.Int).Value = folioconciliacion;
//                    comando.Parameters.Add("@Dias", System.Data.SqlDbType.SmallInt).Value = dias;
//                    comando.Parameters.Add("@Diferencia", System.Data.SqlDbType.SmallInt).Value = diferencia;
//                    comando.Parameters.Add("@SucursalInterno", System.Data.SqlDbType.TinyInt).Value = sucursalinterno;
//                    comando.Parameters.Add("@StatusConcepto", System.Data.SqlDbType.Int).Value = statusconcepto;
//                    comando.Parameters.Add("@Cadena", System.Data.SqlDbType.VarChar).Value =
//                        App.Consultas.ObtenerCadenaDeEtiquetas(1, corporativoconciliacion, sucursalconciliacion,
//                                                               añoconciliacion, mesconciliacion, folioconciliacion,
//                                                               statusconcepto);
//                    comando.Parameters.Add("@Monto", System.Data.SqlDbType.Decimal).Value = monto;
//                    comando.Parameters.Add("@Deposito", System.Data.SqlDbType.Bit).Value = deposito;
//                    comando.Parameters.Add("@FMinima", System.Data.SqlDbType.DateTime).Value = fminima;
//                    comando.Parameters.Add("@FMaxima", System.Data.SqlDbType.DateTime).Value = fmaxima;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ReferenciaNoConciliada dato =
//                            new ReferenciaNoConciliadaDatos(Convert.ToInt16(reader["Corporativo"]),
//                                                            Convert.ToInt16(reader["Sucursal"]),
//                                                            Convert.ToString(reader["SucursalDes"]), añoconciliacion,
//                                                            Convert.ToInt16(reader["Folio"]),
//                                                            Convert.ToInt16(reader["Secuencia"]),
//                                                            Convert.ToString(reader["Descripcion"]),
//                                                            Convert.ToString(reader["Concepto"]),
//                                                            Convert.ToDecimal(reader["Deposito"]),
//                                                            Convert.ToDecimal(reader["Retiro"]),
//                                                            Convert.ToInt16(reader["FormaConciliacion"]),
//                                                            Convert.ToInt16(reader["StatusConcepto"]),
//                                                            Convert.ToString(reader["StatusConciliacion"]),
//                                                            Convert.ToDateTime(reader["FOperacion"]),
//                                                            Convert.ToDateTime(reader["FMovimiento"]),
//                                                            Convert.ToString(reader["UbicacionIcono"]),
//                                                            folioconciliacion, mesconciliacion, 0, true,

//                                                            Convert.ToString(reader["Cheque"]),
//                                                            Convert.ToString(reader["Referencia"]),
//                                                            Convert.ToString(reader["NombreTercero"]),
//                                                            Convert.ToString(reader["RFCTercero"]), sucursalconciliacion,
//                                                            Convert.ToInt16(reader["Año"]), this.implementadorMensajes);
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<TransferenciaBancarias> ObtenieneTransferenciasBancarias(short corporativoOrigen,
//                                                                                      short sucursalOrigen,
//                                                                                      string cuentabancoOrigen, int año,
//                                                                                      short mes, string status)
//        {
//            List<TransferenciaBancarias> datos = new List<TransferenciaBancarias>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {

//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaTransferenciaBancaria", cnn);
//                    comando.Parameters.Add("@CorporativoOrigen", System.Data.SqlDbType.TinyInt).Value =
//                        corporativoOrigen;
//                    comando.Parameters.Add("@SucursalOrigen", System.Data.SqlDbType.TinyInt).Value = sucursalOrigen;
//                    comando.Parameters.Add("@CuentaBancoOrigen", System.Data.SqlDbType.VarChar).Value =
//                        cuentabancoOrigen;
//                    comando.Parameters.Add("@Año", System.Data.SqlDbType.Int).Value = año;
//                    comando.Parameters.Add("@Mes", System.Data.SqlDbType.SmallInt).Value = mes;
//                    comando.Parameters.Add("@Status", System.Data.SqlDbType.VarChar).Value = status;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        TransferenciaBancarias dato =
//                            new TransferenciaBancariasDatos(Convert.ToInt16(reader["Corporativo"]),
//                                                            Convert.ToInt16(reader["Sucursal"]),
//                                                            Convert.ToInt32(reader["Año"]),
//                                                            Convert.ToInt32(reader["Folio"]),
//                                                            Convert.ToSByte(reader["TipoTransferencia"]),
//                                                            Convert.ToString(reader["Referencia"]),
//                                                            Convert.ToDateTime(reader["FMovimiento"]),
//                                                            Convert.ToDateTime(reader["FAplicacion"]),
//                                                            Convert.ToString(reader["UsuarioCaptura"]),
//                                                            Convert.ToDateTime(reader["FCaptura"]),
//                                                            Convert.ToString(reader["UsuarioAprobo"]),
//                                                            Convert.ToDateTime(reader["FAprobado"]),
//                                                            Convert.ToString(reader["Status"]),
//                                                            Convert.ToString(reader["Descripcion"]),
//                                                            Convert.ToString(reader["BancoNombreOrigen"]),
//                                                            Convert.ToString(reader["CuentaBancoOrigen"]),
//                                                            Convert.ToDecimal(reader["Cargo"]),
//                                                            Convert.ToString(reader["BancoNombreDestino"]),
//                                                            Convert.ToString(reader["CuentaBancoDestino"]),
//                                                            Convert.ToDecimal(reader["Abono"]),
//                                                            this.implementadorMensajes);
//                        datos.Add(dato);
//                    }

//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }

//        }

//        public override List<ListaCombo> ConsultaCorporativoTransferencia(int configuracion, int corporativo)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaTransferenciaBancaria", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.SmallInt).Value = corporativo;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.SmallInt).Value = 0;
//                    comando.Parameters.Add("@Banco", System.Data.SqlDbType.SmallInt).Value = 0;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaSucursalTransferencia(int configuracion, int corporativo)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaTransferenciaBancaria", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.SmallInt).Value = corporativo;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.SmallInt).Value = 0;
//                    comando.Parameters.Add("@Banco", System.Data.SqlDbType.SmallInt).Value = 0;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaNombreBancoTransferencia(int configuracion, int corporativo)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaTransferenciaBancaria", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.SmallInt).Value = corporativo;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.SmallInt).Value = 0;
//                    comando.Parameters.Add("@Banco", System.Data.SqlDbType.SmallInt).Value = 0;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//        public override List<ListaCombo> ConsultaCuentaBancoTransferencia(int configuracion, int corporativo, int banco)
//        {
//            List<ListaCombo> datos = new List<ListaCombo>();
//            using (SqlConnection cnn = new SqlConnection(App.CadenaConexion))
//            {
//                try
//                {
//                    cnn.Open();
//                    SqlCommand comando = new SqlCommand("spCBConsultaTransferenciaBancaria", cnn);
//                    comando.Parameters.Add("@Configuracion", System.Data.SqlDbType.SmallInt).Value = configuracion;
//                    comando.Parameters.Add("@Corporativo", System.Data.SqlDbType.SmallInt).Value = corporativo;
//                    comando.Parameters.Add("@Sucursal", System.Data.SqlDbType.SmallInt).Value = 0;
//                    comando.Parameters.Add("@Banco", System.Data.SqlDbType.SmallInt).Value = banco;

//                    comando.CommandType = System.Data.CommandType.StoredProcedure;
//                    SqlDataReader reader = comando.ExecuteReader();
//                    while (reader.Read())
//                    {

//                        ListaCombo dato = new ListaCombo(Convert.ToInt32(reader["Identificador"]),
//                                                         Convert.ToString(reader["Descripcion"]));
//                        datos.Add(dato);
//                    }
//                }
//                catch (SqlException ex)
//                {
//                    stackTrace = new StackTrace();
//                    this.ImplementadorMensajes.MostrarMensaje("Erros al consultar la informacion.\n\rClase :" +
//                                                              this.GetType().Name + "\n\r" + "Metodo :" +
//                                                              stackTrace.GetFrame(0).GetMethod().Name + "\n\r" +
//                                                              "Error :" + ex.Message);
//                    stackTrace = null;
//                }
//                return datos;
//            }
//        }

//    }
//}
