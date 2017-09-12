using System;
using System.Collections.Generic;
using System.Text;

namespace Conciliacion.Migracion.Runtime
{
    public class MensajeImplementacionConsola:IMensajesImplementacion
    {
        private object contenedor;
        public void MostrarMensaje(string texto)
        {
            string cont = (string)contenedor;
            if(this.MensajesActivos)
                System.Console.Write(texto);
        }

        public object ContenedorActual
        {
            get{return contenedor;}
            set{contenedor = value;}
        }
        private bool mensajesActivos = true;
        public bool MensajesActivos
        {
            get
            {
                return mensajesActivos;
            }
            set
            {
                mensajesActivos = value;
            }
        }


       
    }
}
