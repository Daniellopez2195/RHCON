using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class ListEncabezadoCuestionRioViewModel
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool esOpcional { get; set; }
        public int IdCuestionario { get; set; }
        public string Cuestionario { get; set; }
        public string Nomenclatura { get; set; }
        public List<ListPreguntasViewModel> PreguntasList { get; set; }
    }
}