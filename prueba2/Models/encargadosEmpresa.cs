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
    
    public partial class encargadosEmpresa
    {
        public int id { get; set; }
        public int idUsuario { get; set; }
        public int idEmpresa { get; set; }
    
        public virtual usuario usuario { get; set; }
        public virtual empresa empresa { get; set; }
    }
}