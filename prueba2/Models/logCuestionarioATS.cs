//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace rhcon.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class logCuestionarioATS
    {
        public int id { get; set; }
        public int id_empleado { get; set; }
        public Nullable<int> id_cuestionarioATS { get; set; }
        public string informe { get; set; }
        public string acuse { get; set; }
        public int tipo { get; set; }
        public string verificacion { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<System.DateTime> fecha_registro { get; set; }
        public string acontecimiento { get; set; }
    }
}