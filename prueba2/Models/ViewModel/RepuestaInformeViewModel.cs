using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class RepuestaInformeViewModel
    {

        public ResultadosInformeViewModelcs categoria_ambiente_de_trabajo { get; set; }
        public ResultadosInformeViewModelcs dominio_condicion_ambiente { get; set; }
        public ResultadosInformeViewModelcs dimension_condiciones_peligrosas { get; set; }  
        public ResultadosInformeViewModelcs dimension_condiciones_deficiontes { get; set; } 
        public ResultadosInformeViewModelcs dimension_trabajos_peligrosos { get; set; }

       public void categorias(string categoria,int valor)
        {
            switch (categoria)
            {
                case "Ambiente de trabajo":


                    if (valor < 5)
                    {
                        categoria_ambiente_de_trabajo.text = "La encuesta muestra que las  personas perciben la categoría de  condiciones en el ambiente de trabajo muy adecuadas.";
                        categoria_ambiente_de_trabajo.color = "azul";
                        categoria_ambiente_de_trabajo.valor = "Nulo";
                    }
                    else if (valor < 9)
                    {
                        categoria_ambiente_de_trabajo.text = "La encuesta muestra que las  personas  perciben la categoría de  condiciones en el ambiente de trabajo adecuadas.";
                        categoria_ambiente_de_trabajo.color = "verde";
                        categoria_ambiente_de_trabajo.valor = "Bajo";
                    }
                    else if (valor < 11)
                    {
                        categoria_ambiente_de_trabajo.text = "La encuesta muestra que las  personas  perciben la categoría de  condiciones en el ambiente de trabajo medianamente adecuadas.";
                        categoria_ambiente_de_trabajo.color = "amarillo";
                        categoria_ambiente_de_trabajo.valor = "Medio";
                    }
                    else if (valor < 14)
                    {
                        categoria_ambiente_de_trabajo.text = "La encuesta muestra que las  personas  perciben la categoría de  condiciones en el ambiente de trabajo deficientes.";
                        categoria_ambiente_de_trabajo.color = "naranja";
                        categoria_ambiente_de_trabajo.valor = "ALTO";
                    }
                    else if (valor >= 14)
                    {
                        categoria_ambiente_de_trabajo.text = "La encuesta muestra que las  personas perciben la categoría de  condiciones en el ambiente de trabajo muy deficientes.";
                        categoria_ambiente_de_trabajo.color = "rojo";
                        categoria_ambiente_de_trabajo.valor = "MUY ALTO";
                    }
                    break;

                case "Factores propios de la actividad":
                    break;
                case "Organización del tiempo de trabajo":
                    break;
                case "Liderazgo y relaciones en el trabajo":
                    break;
                case "Entorno organizacional":
                    break;
                default:
                    break;
            }
        }

    }
}