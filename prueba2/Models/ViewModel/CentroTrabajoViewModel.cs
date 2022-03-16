using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace rhcon.Models.ViewModel
{
    /// <summary>
    /// Modelo para utilizar en los formularios de Crear o Editar Centros de Trabajo
    /// </summary>
    public class CentroTrabajoViewModel
    {
        /// <summary>
        /// Propiedad Id de Centro de Trabajo para utilizar en los formularios crear y editar (hidden)
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Propiedad Nombre de Centro de Trabajo para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Nombre del Centro de Trabajo:")]
        [StringLength(100)]
        public string Nombre { get; set; }
        /// <summary>
        /// Propiedad Dirección de Centro de Trabajo para utilizar en los formularios crear y editar
        /// </summary>
        [Display(Name = "Dirección del Centro de Trabajo:")]
        [StringLength(150)]
        public string Direccion { get; set; }
        /// <summary>
        /// Propedad Actividades de centro de Trabajo para utilizar en los formularios crear y editar
        /// </summary>
        [Display(Name = "Actividad del Centro de Trabajo:")]
        [StringLength(150)]
        public string Actividades { get; set; }
        /// <summary>
        /// Propiedad Nombre del encargado de centro de Trabajo para utilizar en los formularios crear y editar
        /// </summary>
        [Display(Name = "Nombre del Encargado(a):")]
        [StringLength(75)]
        public string Encargado { get; set; }
        /// <summary>
        /// Propiedad Email del encargado de centro de Trabajo para utilizar en los formularios crear y editar
        /// </summary>
        [Display(Name = "Correo Electrónico del Encargado(a):")]
        [StringLength(75)]
        public string email { get; set; }
        /// <summary>
        /// Propiedad IdUsuario de Centro de Trabajo para utilizar en los formularios crear y editar (hidden)
        /// </summary>
        public int IdUsuario { get; set; }
        /// <summary>
        /// Propiedad Id Empresa de Centro de Trabajo para utilizar en los formularios crear y editar (hidden)
        /// </summary>
        public int IdEmpresa { get; set; }
        /// <summary>
        /// Propiedad IdEstatus de Centro de Trabajo para utilizar en los formularios crear y editar (hidden)
        /// </summary>
        public int IdStatus { get; set; }
    }
}