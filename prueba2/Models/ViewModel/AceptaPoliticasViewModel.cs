using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace rhcon.Models.ViewModel
{
    /// <summary>
    /// Modelo para la vista de aceptación de políticas
    /// </summary>
    public class AceptaPoliticasViewModel
    {
        /// <summary>
        /// Propiedad tipo File que representa el archivo de subida para la política Nom 035
        /// </summary>
        [Required]
        [DisplayName("Subir Política por un Entorno Organizacional Favorable y la Prevención de Riesgos Psicosociales firmada:")]
        public HttpPostedFileBase nom035 { get; set; }
        /// <summary>
        /// Propiedad tipo File que representa el archivo de subida para la política Nom 025
        /// </summary>
        [Required]
        [DisplayName("Subir Política de Igualdad Laboral y no Discriminación firmada:")]
        public HttpPostedFileBase nom025 { get; set; }
        /// <summary>
        /// Propiedad tipo Booleana que representa si la política ya ha sido aceptada por parte del usuario.
        /// </summary>
        [Required]
        [DisplayName("Acepto las politicas")]
        public bool AceptoPolitica { get; set; }
    }
}