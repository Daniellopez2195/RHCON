using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class PlanAccionViewModel
    {
        public int Id { get; set; }

        public string dimension { get; set; }

        public string nivelriesgo { get; set; }

        public string accion { get; set; }

        public string descripcion { get; set; }

        public string responsable { get; set; }

        public string fecha { get; set; }

    }
}