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
    
    public partial class EncabezadoCuestionario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EncabezadoCuestionario()
        {
            this.cuestionarioDetalle = new HashSet<cuestionarioDetalle>();
        }
    
        public int id { get; set; }
        public string descripcion { get; set; }
        public bool esOPcional { get; set; }
        public int idCuestionario { get; set; }
        public string opciones_text { get; set; }
        public Nullable<int> maximo { get; set; }
    
        public virtual cuestionario cuestionario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cuestionarioDetalle> cuestionarioDetalle { get; set; }
    }
}
