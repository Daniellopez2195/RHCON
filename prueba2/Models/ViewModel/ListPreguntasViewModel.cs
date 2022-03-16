using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class ListPreguntasViewModel
    {
        public int Id { get; set; }
        public int NoReactivo { get; set; }
        public string Reactivo { get; set; }
        public int IdPosiblesRespuestas { get; set; }    
        public List<ListRespuestasViewModel> RespList { get; set; }


    }
}