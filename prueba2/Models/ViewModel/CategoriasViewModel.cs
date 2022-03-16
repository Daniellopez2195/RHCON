using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class CategoriasViewModel
    {
        public OpcionesViewModel ambiente_trabajo { get; set; }
        public OpcionesViewModel factores_actividad { get; set; }
        public OpcionesViewModel organizacion_trabajo { get; set; }
        public OpcionesViewModel liderazgo_trabajo { get; set; }
        public OpcionesViewModel entorno_organizacional { get; set; }

    }
}