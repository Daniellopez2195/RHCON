using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace rhcon.Models.ViewModel
{
    /// <summary>
    /// Modelo para utilizar los uploload File en Carga masivas de Centros de Trabajo y Empleados
    /// </summary>
    public class ArchivoViewModel
    {
        /// <summary>
        /// Propiedad tipo file que representa el archivo de tipo excel que se sube al servido y se procesa para realizar las cargas masivas.
        /// </summary>
        [Required]
        [DisplayName("Seleccione Archivo:")]
        public HttpPostedFileBase Archivo { get; set; }
    }
}