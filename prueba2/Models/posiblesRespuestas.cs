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
    
    public partial class posiblesRespuestas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public posiblesRespuestas()
        {
            this.cuestionarioDetalle = new HashSet<cuestionarioDetalle>();
            this.posiblesRespDetalle = new HashSet<posiblesRespDetalle>();
        }
    
        public int id { get; set; }
        public string grupo { get; set; }
        public string descripcion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cuestionarioDetalle> cuestionarioDetalle { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<posiblesRespDetalle> posiblesRespDetalle { get; set; }
    }
}
