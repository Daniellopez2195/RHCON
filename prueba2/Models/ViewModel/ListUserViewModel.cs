using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    /// <summary>
    /// Modelo para mostrar en la datatable de usuarios
    /// </summary>
    public class ListUserViewModel
    {
        /// <summary>
        /// Propiedad Id para mostrar en la lista de Usuarios
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Propiedad Nombre para mostrar en la lista de Usuarios
        /// </summary>
        public string nombre  { get; set; }
        /// <summary>
        /// Propiedad email para mostrar en la lista de Usuarios
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// Propiedad fecha para mostrar en la lista de Usuarios
        /// </summary>
        public string fecha { get; set; }
        /// <summary>
        /// Propiedad rol para mostrar en la lista de Usuarios
        /// </summary>
        public string rol { get; set; }
        /// <summary>
        /// Propiedad estatus para mostrar en la lista de Usuarios
        /// </summary>
        public string estatus { get; set; }
    }
}