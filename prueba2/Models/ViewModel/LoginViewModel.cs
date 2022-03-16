using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace rhcon.Models.ViewModel
{
    public class LoginViewModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Propiedad Nombre de usuario para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Usuario:")]
        [StringLength(75)]
        public string nombre { get; set; }
        /// <summary>
        /// Propiedad Password de usuario para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Contraseña:")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        /// <summary>
        /// Propiedad Rol de usuario para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Rol:")]
        public int IdRol { get; set; }
        public List<SelectListItem> RolList { get; set; }
    }
}