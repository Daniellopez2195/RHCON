using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class ListRespuestasViewModel
    {
        public int Id { get; set; }
        public int IdEncabezado { get; set; }
        public string Respuesta { get; set; }
        public int Valor { get; set; }
    }
}