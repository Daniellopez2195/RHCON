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
    
    public partial class modulo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public modulo()
        {
            this.modulo1 = new HashSet<modulo>();
            this.operaciones = new HashSet<operaciones>();
        }
    
        public int id { get; set; }
        public string nombre { get; set; }
        public Nullable<int> id_padre { get; set; }
        public string controller_name { get; set; }
        public string action_name { get; set; }
        public int idStatus { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<modulo> modulo1 { get; set; }
        public virtual modulo modulo2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<operaciones> operaciones { get; set; }
        public virtual cstatus cstatus { get; set; }
    }
}
