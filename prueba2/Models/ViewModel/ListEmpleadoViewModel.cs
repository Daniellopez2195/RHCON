using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class ListEmpleadoViewModel
    {
        /// <summary>
        /// Propiedad Id para mostrar en la lista de Empleado
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Propiedad Nombre para mostrar en la lista de Empleado
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Propiedad Curp para mostrar en la lista de Empleado
        /// </summary>
        public string CURP { get; set; }

        /// <summary>
        /// Propiedad Telefono para mostrar en la lista de Empleado
        /// </summary>
        public string Telefono { get; set; }
        /// <summary>
        /// Propiedad Estatus para mostrar en la lista de Empresa
        /// </summary>
        public string Estatus { get; set; }
        public string Nss { get; set; }
        public string CentroTrabajo { get; set; }
        public string AreaFuncion { get; set; }
        public string email { get; set; }
    }
}