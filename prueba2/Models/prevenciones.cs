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
    
    public partial class prevenciones
    {
        public int Id { get; set; }
        public string prevencion { get; set; }
        public int idDimencion { get; set; }
    
        public virtual dimencion dimencion { get; set; }
    }
}
