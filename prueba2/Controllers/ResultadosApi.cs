using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using rhcon.Models.ViewModel;
using rhcon.Models;
using rhcon.Models.ApiModel;
namespace rhcon.Controllers
{
    public class ResultadosApi : ApiController
    {
        private rhconEntities db = new rhconEntities();
        // GET api/<controller>
        // IEnumerable<AccionesXcategoriaApiModel> getAccione
        public AccionesXcategoriaApiModel Get(int idEmpresa, string year = "", string idCentro = "")
        {

            AccionesXcategoriaApiModel acciones = new AccionesXcategoriaApiModel();
       
            string[] categorias =
            {
                    "Ambiente de trabajo",
                    "Factores propios de la actividad",
                    "Organización del tiempo de trabajo",
                    "Liderazgo y relaciones en el trabajo",
                    "Entorno organizacional"
                };

            var cat1 = categorias[0];
            var cat2 = categorias[1];
            var cat3 = categorias[2];
            var cat4 = categorias[3];
            var cat5 = categorias[4];


            var data1 = db.acciones.Where(d => d.year.ToString() == year & d.categoria.Equals(cat1) & d.idEmpresa == idEmpresa).ToList();
            var data2 = db.acciones.Where(d => d.year.ToString() == year & d.categoria.Equals(cat2) & d.idEmpresa == idEmpresa).ToList();
            var data3 = db.acciones.Where(d => d.year.ToString() == year & d.categoria.Equals(cat3) & d.idEmpresa == idEmpresa).ToList();
            var data4 = db.acciones.Where(d => d.year.ToString() == year & d.categoria.Equals(cat4) & d.idEmpresa == idEmpresa).ToList();
            var data5 = db.acciones.Where(d => d.year.ToString() == year & d.categoria.Equals(cat5) & d.idEmpresa == idEmpresa).ToList();
            bool item = false;

            if (!string.IsNullOrEmpty(idCentro))
            {
                var filt = int.Parse(idCentro);
                data1 = data1.FindAll(d => d.idCentro == filt).ToList();
                data2 = data2.FindAll(d => d.idCentro == filt).ToList();
                data3 = data3.FindAll(d => d.idCentro == filt).ToList();
                data4 = data4.FindAll(d => d.idCentro == filt).ToList();
                data5 = data5.FindAll(d => d.idCentro == filt).ToList();
                item = true;
            }
            else
            {
                data1 = data1.FindAll(d => d.idCentro == null).ToList();
                data2 = data2.FindAll(d => d.idCentro == null).ToList();
                data3 = data3.FindAll(d => d.idCentro == null).ToList();
                data4 = data4.FindAll(d => d.idCentro == null).ToList();
                data5 = data5.FindAll(d => d.idCentro == null).ToList();
            }

            acciones.categoria1 = data1;
            acciones.categoria2 = data2;
            acciones.categoria3 = data3;
            acciones.categoria4 = data4;
            acciones.categoria5 = data5;

            return acciones;


        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}