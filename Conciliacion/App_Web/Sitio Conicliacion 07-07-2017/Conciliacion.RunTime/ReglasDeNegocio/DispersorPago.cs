using System;
using System.Collections.Generic;
using Conciliacion.RunTime.DatosSQL;

namespace Conciliacion.RunTime.ReglasDeNegocio
{
    public abstract class DispersorPago : EmisorMensajes
    {

        /// <summary>
        /// �stevalor ser� utilizado para verificar que el Cliente de cada uno de los
        /// documentos por analizar respeta la relaci�n Cliente Padre &ndash; Cliente Hijo
        /// &ndash;Cliente Hermano, de esto se encargar� el m�todo <b>ValidaClientes</b>.
        /// </summary>
        private string clientereferencia;
        /// <summary>
        /// Monto remanente de MontoTotalPago tras la dispersi�n de los pagos entre los
        /// PagosPorAnalizar
        /// </summary>
        private decimal saldoafavor;
        /// <summary>
        /// Monto del pago que la clase dispersar� entre los PagosPorAnalizar
        /// </summary>
        private decimal montototalpago;
        /// <summary>
        /// Insumo de documentos propuestos para pago con un monto determinado
        /// </summary>
        private List<PagoPropuesto> pagosporanalizar;
        /// <summary>
        /// Listado de documentos con una propuesta de dispersi�n tras la ejecuci�n del
        /// m�todo DispersarPago
        /// </summary>
        private List<PagoPropuesto> pagospropuestos;


        #region Propiedades
        /// <summary>
        /// �sta propiedad ser� utilizada para verificar que el Cliente de cada uno de los
        /// documentos por analizar respeta la relaci�n Cliente Padre &ndash; Cliente Hijo
        /// &ndash;Cliente Hermano, de esto se encargar� el m�todo ValidaClientes
        /// </summary>
        public string ClienteReferencia
        {
            get { return clientereferencia; }
            set { clientereferencia = value; }
        }

        /// <summary>
        /// Monto remanente de MontoTotalPago tras la dispersi�n de los pagos entre los
        /// PagosPorAnalizar
        /// </summary>
        public decimal SaldoAFavor
        {
            get { return saldoafavor; }
            set { saldoafavor = value; }
        }

        /// <summary>
        /// Monto del pago que la clase dispersar� entre los PagosPorAnalizar
        /// </summary>
        public decimal MontoTotalPago
        {
            get { return montototalpago; }
            set { montototalpago = value; }
        }

        /// <summary>
        /// Lista de documentos propuestos para pago con un monto determinado
        /// </summary>
        public List<PagoPropuesto> PagosPorAnalizar
        {
            get { return pagosporanalizar; }
            set { pagosporanalizar = value; }
        }

        /// <summary>
        /// Listado de documentos con una propuesta de dispersi�n tras la ejecuci�n del
        /// m�todo DispersarPago
        /// </summary>
        public List<PagoPropuesto> PagosPropuestos
        {
            get { return pagospropuestos; }
            set { pagospropuestos = value; }
        }
        #endregion

        #region Contrusctores
        public void DispersarPago(string clientereferencia, decimal saldoafavor, decimal montototalpago, List<PagoPropuesto> pagosporanalizar, List<PagoPropuesto> pagospropuestos)
        {
            this.ClienteReferencia = clientereferencia;
            this.SaldoAFavor = saldoafavor;
            this.MontoTotalPago = montototalpago;
            this.PagosPorAnalizar = pagosporanalizar;
            this.PagosPropuestos = pagospropuestos;
        }

        public void DispersarPago()
        {

        }
        #endregion


        public abstract void InicializarPagos();

        public abstract bool ValidaClientes(List<PagoPropuesto> pagosavalidar, string clientereferencia, Conexion _conexion);

        public abstract bool ValidarDispersion();

        /// <summary>
        /// Ejecuta la estrategia de liquidaci�n de pagos (cubre la totalidad de un
        /// pedido)
        /// </summary>
        /// <param name="DocumentosADispersar"></param>
        public abstract List<PagoPropuesto> DispersarATotales(List<PagoPropuesto> DocumentosADispersar);

        /// <summary>
        /// Ejecuta la estrategia de abono de parcialidades  (cubre una parte del pedido)
        /// </summary>
        /// <param name="DocumentosADispersar"></param>
        public abstract List<PagoPropuesto> DispersarAParciales(List<PagoPropuesto> DocumentosADispersar);

    }

}