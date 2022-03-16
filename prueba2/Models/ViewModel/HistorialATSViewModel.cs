using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class HistorialATSViewModel
    {
        public string informe { get; set; }
        public string acuse { get; set; }
        public int tipo { get; set; }

        public DateTime fecha { get; set; }

        public string traumatico { get; set; }
    }
}