using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class ReporteViewModel
    {

        public string tipo { get; set; }
        public string centros { get; set; }
        public string fecha { get; set; }
        public string norma { get; set; }
        public string sexo { get; set; }
        public string edad { get; set; }
        public string estadoCivil { get; set; }
        public string antiguedad { get; set; }
        public string tiempoPuestoActual { get; set; }
        public string tiempoExperienciaLaboral { get; set; }
        public string escolaridad { get; set; }
        public string tipoJornada { get; set; }
        public string tipoContratacion { get; set; }
        public string tipoPersonal { get; set; }
        public string tieneDiscapacidad { get; set; }
        public string discapacidad { get; set; }
        public string realizaRotacion { get; set; }
        public string parteSectores { get; set; }
        public string years { get; set; }
        public string prueba { get; set; }
        public int idEmpresa { get; set; }
        public int idCentro { get; set; }
        public List<respuestaEmpleado> respuestas { get; set; }
       public   DataResultadosViewModel datosReporte { get; set; }
    }
}