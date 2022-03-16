using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class CabeceraViewModel
    {
        public OpcionesViewModel reclutamiento_seleccion_personas { get; set; }
        public OpcionesViewModel formacion_capacitacion { get; set; }
        public OpcionesViewModel permanencia_ascenso { get; set; }

        public OpcionesViewModel corresponsabilidad_vida_laboral_familiar { get; set; }
        public OpcionesViewModel clima_laboral_violencia    { get; set; }
        public OpcionesViewModel acoso_hostigamiento { get; set; }
        public OpcionesViewModel accesibilidad { get; set; }
        public OpcionesViewModel respeto_diversidad { get; set; }
        public OpcionesViewModel condiciones_generales_trabajo { get; set; }
        public OpcionesViewModel resultado_general { get; set; }



    }
}