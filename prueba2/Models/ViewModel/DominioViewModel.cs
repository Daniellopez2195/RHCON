using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class DominioViewModel
    {
       public  OpcionesViewModel condiciones_ambiente_trabajo { get; set; }
        public OpcionesViewModel carga_trabajo { get; set; }
        public OpcionesViewModel falta_control_sobre_trabajo { get; set; }
        public OpcionesViewModel jornada_de_trabajo { get; set; }
        public OpcionesViewModel interferencia_relacion_trabajo_familia { get; set; }
        public OpcionesViewModel liderazgo { get; set; }
        public OpcionesViewModel relaciones_trabajo { get; set; }
        public OpcionesViewModel violencia { get; set; }
        public OpcionesViewModel reconocimiento_desempeno { get; set; }
        public OpcionesViewModel insuficiente_sentido_pertenencia_inestabilidad { get; set; }

    }
}