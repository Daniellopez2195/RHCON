using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class OpcionesViewModel
    {

        public string color { get; set; }
        public string text { get; set; }
        public string estado { get; set; }
        public string tipo { get; set; }

        public List<prevenciones> prevenciones { get; set; }
    }
}