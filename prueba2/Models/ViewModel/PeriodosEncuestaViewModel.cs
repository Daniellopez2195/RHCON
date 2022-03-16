using rhcon.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class PeriodosEncuestaViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Fecha de Inicio:")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicio { get; set; }
        [Required]
        [Display(Name = "Fecha Final:")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [MyDate(ErrorMessage = "No se permite una fecha menor")]
        //[DateGreaterThanAttribute(otherPropertyName = "FechaInicio", ErrorMessage = "La fecha Final debe ser mayor que la fecha de inicio")]
        public DateTime FechaFinal { get; set; }

        public int IdUsuario { get; set; }
        [Display(Name = "Cédula:")]
        public string cedula { get; set; }
        [Display(Name = "Encargado:")]
        public string Encargado { get; set; }
    }
}