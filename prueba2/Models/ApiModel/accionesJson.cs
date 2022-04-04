using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ApiModel
{
    public class accionesJson
    {

        public int id { get; set; }
        public string dimension { get; set; }
        public string descripcion { get; set; }
        public string estado { get; set; }
        public string accion { get; set; }
        public string medidasPrevencion { get; set; }
        public string responsable { get; set; }
        public string date { get; set; }
        public string tipo { get; set; }
        public string color { get; set; }
        public Nullable<System.DateTime> registro { get; set; }
        public Nullable<int> idEmpresa { get; set; }
        public Nullable<bool> status { get; set; }
        public string categoria { get; set; }
        public string dominio { get; set; }
        public Nullable<short> year { get; set; }
        public Nullable<int> idCentro { get; set; }
    }
}