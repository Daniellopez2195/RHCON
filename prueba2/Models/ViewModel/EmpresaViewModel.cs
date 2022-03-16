using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    /// <summary>
    /// Modelo para utilizar en los formularios de creacion y edicion
    /// </summary>
    public class EmpresaViewModel
    {
        /// <summary>
        /// Propiedad Imagen de empresa para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Logotipo:")]
        public HttpPostedFileBase logotipo { get; set; }
        public string strlogotipo { get; set; }
        /// <summary>
        /// Propiedad Id de empresa para utilizar en los formularios crear y editar
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Propiedad Razon Comercial de empresa para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Razón Comercial de la Empresa:")]
        [StringLength(100)]
        public string RazonComercial { get; set; }
        /// <summary>
        /// Propiedad RFC de empresa para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "RFC de la Empresa:")]
        [StringLength(100)]
        public string RFC { get; set; }
        /// <summary>
        /// Propiedad Razon Social de empresa para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Razón Social de la Empresa:")]
        [StringLength(100)]
        public string RazonSocial { get; set; }
        /// <summary>
        /// Propiedad Correo de Responsable de empresa para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Correo Electrónico del Responsable:")]
        [EmailAddress]
        public string Email { get; set; }
        /// <summary>
        /// Propiedad Numero Telefonico de empresa para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Número Telefónico del Responsable:")]
        [Phone]
        public string Telefono { get; set; }
        /// <summary>
        /// Propiedad Direccion de empresa para utilizar en los formularios crear y editar
        /// </summary>
        /// 

        /// <summary>
        /// Propiedad nombrejerarquia de empresa para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Nombre del empleado de mayor jerarquía:")]
        [StringLength(100)]
        public string nombrejerarquia { get; set; }


        /// <summary>
        /// Propiedad puestojerarquia de empresa para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Puesto del empleado de mayor jerarquía:")]
        [StringLength(100)]
        public string puestojerarquia { get; set; }

        [Required]
        [Display(Name = "Dirección de la Empresa:")]
        [StringLength(150)]
        public string Direccion { get; set; }
        /// <summary>
        /// Propiedad Actividad de empresa para utilizar en los formularios crear y editar
        /// </summary>
        [Display(Name = "Actividad de la Empresa:")]
        [StringLength(200)]
        public string Actividad { get; set; }
        /// <summary>
        /// Propiedad Puesto del Responsable de empresa para utilizar en los formularios crear y editar
        /// </summary>
        [Display(Name = "Puesto del Responsable:")]
        [StringLength(100)]
        public string Puesto { get; set; }
        /// <summary>
        /// Propiedad Respondable de empresa para utilizar en los formularios crear y editar
        /// </summary>
        [Required]
        [Display(Name = "Nombre Completo del Responsable:")]
        [StringLength(75)]
        public string Responsable { get; set; }
        /// <summary>
        /// Propiedad Password de empresa para utilizar en los formularios crear y editar
        /// </summary>
        [Display(Name = "Password:")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        /// <summary>
        /// Propiedad Contrato para subir el contrato firmado de la empresa
        /// </summary>
        [Required]
        [Display(Name = "Contrato:")]
        public HttpPostedFileBase contrato { get; set; }
        public string strcontrato { get; set; }
        public string estatus { get; set; }

        public DateTime FechaAlta { get; set; }
    }
}