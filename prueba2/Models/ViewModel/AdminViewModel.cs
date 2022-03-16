using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace rhcon.Models.ViewModel
{
    public class AdminViewModel
    {
        public int Id { get; set; }


        [Display(Name = "NOMBRE COMPLETO DEL ADMINISTRADOR(A)")]
        [StringLength(100)]
        public string nombre { get; set; }




        [Display(Name = "NUMERO DE CEDULA DEL ADMINISTRADOR(A)")]
        [StringLength(100)]
        public string cedula { get; set; }




        [Display(Name = "CORREO ELECTRONICO DEL ADMINISTRADOR(A)")]
        [EmailAddress]
        public string Email { get; set; }




        [Display(Name = "CELULAR DEL ADMINISTRADOR(A)")]
        [Phone]
        public string celular { get; set; }




        [Display(Name = "CONTACTO DE EMERGENCIA DEL ADMINISTRADOR(A)")]
        [StringLength(100)]
        public string contacto { get; set; }



        [Display(Name = "Password:")]
        [DataType(DataType.Password)]
        public string Password { get; set; }



        [Display(Name = "NUMERO DE CONTACTO DE EMERGENCIA DEL ADMINISTRADOR(A)")]
        [Phone]
        public string celcontacto { get; set; }


        public int IdUsuario { get; set; }
    }
}