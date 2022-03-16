using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class CondicionalesInformeViewModel
    {

        public OpcionesViewModel condicionales(int valor, int[] limites, string[] cadenas )
        {
            string[] colores = { "azul", "verde", "amarillo", "naranja", "rojo" };
            string[] estado = { "Nulo", "Bajo", "Medio", "Alto", "Muy alto" };
            OpcionesViewModel opciones = new OpcionesViewModel();


            if (valor < limites[0])
            {
                opciones.text = cadenas[0];
                opciones.color = colores[0];
                opciones.estado = estado[0];

            }
            else if (valor < limites[1])
            {
                opciones.text = cadenas[1];
                opciones.color = colores[1];
                opciones.estado = estado[1];
            }
            else if (valor < limites[2])
            {
                opciones.text = cadenas[2];
                opciones.color = colores[2];
                opciones.estado = estado[2];
            }
            else if (valor < limites[3])
            {
                opciones.text = cadenas[3];
                opciones.color = colores[3];
                opciones.estado = estado[3];
            }
            else if (valor >= limites[4])
            {
                opciones.text = cadenas[4];
                opciones.color = colores[4];
                opciones.estado = estado[4];
            }

            return opciones;
        }


    }
}