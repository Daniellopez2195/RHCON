using System;
using System.Collections.Generic;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class CuestionarioViewModel
    {
        public int Tipo { get; set; }
        public DateTime Fecha { get; set; }
        public string Traumatico { get; set; }
        public DateTime FechaO { get; set; }
        public string Hora { get; set; }
        public string Descripcion { get; set; }
        public HttpPostedFileBase Informe { get; set; }
        public int NPersonas { get; set; }
        public List<PersonaViewModel> Personas { get; set; }
        public string pregunta1 { get; set; }
        public string pregunta2 { get; set; }
        public string pregunta3 { get; set; }
        public string pregunta4 { get; set; }
        public string pregunta5 { get; set; }
        public string pregunta6 { get; set; }
        public string pregunta7 { get; set; }
        public string pregunta8 { get; set; }
        public string pregunta9 { get; set; }
        public string pregunta10 { get; set; }
        public string pregunta11 { get; set; }
        public string pregunta12 { get; set; }
        public string pregunta13 { get; set; }
        public string pregunta14 { get; set; }
    }
}