using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace rhcon.Models.ViewModel
{
    public class EmpleadoViewModel
    {
        /// <summary>
        /// Propiedad Id del empleado para utilizar en los formular  ios crear y editar
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Propiedad del nombre  del empleado para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Nombre Completo del Empleado(a):")]
        [StringLength(100)]
        public string Nombre { get; set; }
        /// <summary>
        /// Propiedad CURP del empleado para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "CURP:")]
        [StringLength(100)]
        public string CURP { get; set; }
        /// <summary>
        /// Propiedad Nss del empleado para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "NSS:")]
        [StringLength(100)]
        public string Nss { get; set; }
        /// <summary>
        /// Propiedad Correo de Responsable del empleado para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Correo Electrónico:")]
        [EmailAddress]
        public string Email { get; set; }
        /// <summary>
        /// Propiedad Numero Telefonico del empleado para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Número Telefónico:")]
        [Phone]
        public string Telefono { get; set; }
        /// <summary>
        /// Propiedad Contacto del empleado para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Nombre del Contacto de Emergencia:")]
        [StringLength(150)]
        public string Contacto { get; set; }
        /// <summary>
        /// Propiedad de Celular de contacto de Empleado para utilizar en los formularios crear y editar
        /// </summary>
        [Display(Name = "Número Telefónico del Contacto de Emergencia:")]
        public string CelContacto { get; set; }
        /// <summary>
        /// Propiedad Centro de Trabajo del Empleado para utilizar en los formularios crear y editar
        /// </summary>

        [Display(Name = "Centro de Trabajo:")]
        public int IdCentro { get; set; }

        [Display(Name = "Área/Función del Empleado(a):")]
        public string AreaFuncion { get; set; }

        /// <summary>
        /// Propiedad Password del Empleado para utilizar en los formularios crear y editar
        /// </summary>

        [Display(Name = "Password:")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        /// <summary>
        /// Propiedad que representra la lista de centros de trabajo para mostrar en el formulario del empleado.
        /// </summary>
        public List<SelectListItem> Centros { get; set; }
        /// <summary>
        /// Propiedad de ID Empresa
        /// </summary>
        public int IdEmpresa { get; set; }
        /// <summary>
        /// Propiedad ID Usuario
        /// </summary>
        public int IdUsuario { get; set; }
        /// <summary>
        /// Propiedad Acepto política que determina si el empleado ha aceptado las politicas o no
        /// </summary>
        public bool? AceptoPolitica { get; set; }
        /// <summary>
        /// Propiedad que representa el logotipo apar ala empresa
        /// </summary>
        public string strlogotipo { get; set; }
        /// <summary>
        /// Propiedad que representa el centro de Trabajo
        /// </summary>
        public string CentroTrabajo { get; set; }
        /// <summary>
        /// Propiedad estatus, que representa que estatus guarda en la BD
        /// </summary>
        public string estatus { get; set; }

        public string verificacion { get; set; }
    }
}