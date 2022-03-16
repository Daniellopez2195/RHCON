namespace rhcon.Models.ViewModel
{
    /// <summary>
    /// Modelo para mostar en reportes de rotativa los acuses que se envían al correo de los usuarios empleados
    /// </summary>
    public class CorreosPdfViewModel
    {
        /// <summary>
        /// Propiedad Imagen para el reporte
        /// </summary>
        public string img { get; set; }
        /// <summary>
        /// Propiedad Empresa que representa el nombre o Razon Comercial
        /// </summary>
        public string empresa { get; set; }
        /// <summary>
        /// Propiedad de Fecha de Inicio
        /// </summary>
        public string inicio { get; set; }
        /// <summary>
        /// Propiedad de Logotipo para empresa
        /// </summary>
        public string logo { get; set; }
        /// <summary>
        /// Propiedad Fecha Final
        /// </summary>
        public string fin { get; set; }
    }
}