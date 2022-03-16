using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    /// <summary>
    /// Modelo para mostrar en la datatable
    /// </summary>
    public class ListEmpresaViewModel
    {
        /// <summary>
        /// Propiedad Id para mostrar en la lista de Empresa
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Propiedad RFC para mostrar en la lista de Empresa
        /// </summary>
        public string RFC { get; set; }
        /// <summary>
        /// Propiedad Razon Comercial para mostrar en la lista de Empresa
        /// </summary>
        public string RazonComercial { get; set; }
        /// <summary>
        /// Propieda Razon Social para mostrar en la lista de Empresa
        /// </summary>
        public string RazonSocial { get; set; }
        /// <summary>
        /// Propiedad Telefono para mostrar en la lista de Empresa
        /// </summary>
        public string Telefono { get; set; }
        /// <summary>
        /// Propiedad Estatus para mostrar en la lista de Empresa
        /// </summary>
        public string Estatus { get; set; }
    }
}