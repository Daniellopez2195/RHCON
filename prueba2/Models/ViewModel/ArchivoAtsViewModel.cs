using System.Collections.Generic;

namespace rhcon.Models.ViewModel
{
    /// <summary>
    /// Modelo para la vista de Paginación en el módulo ATS
    /// </summary>
    public class ArchivoAtsViewModel : PaginacionViewModel
    {
        /// <summary>
        /// Propiedad tipo Lista de logCuestionarioATS (Modelo de la base de datos), para utilizar en la paginación del historial.
        /// </summary>
        public List<logCuestionarioATS> Registro { get; set; }
        /// <summary>
        /// log de activacion de encuesta
        /// </summary>
        public List<Log_factores> logFac { get; set; }

    }
}