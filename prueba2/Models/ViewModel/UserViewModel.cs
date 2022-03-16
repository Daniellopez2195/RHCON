using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace rhcon.Models.ViewModel
{
    /// <summary>
    /// Clase para utilizar como mode lo en los frmularios de creacion y edicion
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// Propiedad Id de usuario para utilizar en los formularios crear y editar
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Propiedad Nombre de usuario para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Nombre:")]
        [StringLength(75)]
        public string nombre { get; set; }
        /// <summary>
        /// Propiedad Password de usuario para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Password:")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        /// <summary>
        /// Propiedad Email de usuario para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "E-Mail:")]
        [EmailAddress]
        public string email { get; set; }
        /// <summary>
        /// Propiedad Rol de usuario para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Rol:")]
        public int IdRol { get; set; }
        public List<SelectListItem>  RolList { get; set; }

    }
}