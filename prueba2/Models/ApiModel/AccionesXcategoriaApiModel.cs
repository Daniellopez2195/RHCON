using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ApiModel
{
    public class AccionesXcategoriaApiModel
    {
        public List<accionesJson> categoria1 { get; set; }
        public List<accionesJson> categoria2 { get; set; }
        public List<accionesJson> categoria3 { get; set; }
        public List<accionesJson> categoria4 { get; set; }
        public List<accionesJson> categoria5 { get; set; }

        public  string ObtenerProductoJson(string nombre)
        {
            return @"{
        ""Id"": 100,
        ""Nombre"": ""nombre"",
        ""Precio"": 10.50,
        ""Categorias"": [
            ""Libros"",
            ""Programación"",+
            ""Ingles""
            ]
        }";
        }
    }



    }
