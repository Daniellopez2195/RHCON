using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    /// <summary>
    /// Clase para la lista de centros de trabajo
    /// </summary>
    public class ListCentroTrabajoViewModel
    {
        /// <summary>
        /// Propiedad Id para mostrar en la lista
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Propiedad Nombre para mostrar en la lista
        /// </summary>
        public string Nombre { get; set; } 
        /// <summary>
        /// Propedad Estatus para Mostrar en la Lista
        /// </summary>
        public string Estatus { get; set; }
    }
}