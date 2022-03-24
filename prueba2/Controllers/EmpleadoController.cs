 using rhcon.Models;
using rhcon.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace rhcon.Controllers
{
    /// <summary>
    /// Controlador para Empleados
    /// </summary>
    [HandleError(View = "Error")]
    public class EmpleadoController : Controller
    {
        /// <summary>
        /// GET: Empleado
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
            }
            else
            {
                ViewBag.Message = 0;
            }

            if (TempData["MessageActivo"] != null)
            {
                ViewBag.MessageActivo = TempData["MessageActivo"].ToString();
            }
            else
            {
                ViewBag.MessageActivo = 0;
            }

            var oEmp = (EmpleadoViewModel)Session["empleado"];

            var empresa = (EmpresaViewModel)Session["empresa"];
            PeriodosEncuestaViewModel periodo;
            using (rhconEntities db = new rhconEntities())
            {
                periodo = (from d in db.periodosEncuesta
                           where d.idEmpresa == empresa.Id && (DateTime.Now >= d.fechaInicio && DateTime.Now <= d.fechaFinal)
                           select new PeriodosEncuestaViewModel
                           {
                               FechaInicio = d.fechaInicio,
                               FechaFinal = d.fechaFinal
                           }).FirstOrDefault();


            }

            DatosDeEncuestaViewModel datos = new DatosDeEncuestaViewModel();
            if(periodo!=null)
            {
                using (rhconEntities db = new rhconEntities())
                {
                    var dato = db.logRespuestaEmpleado.Where(d => d.idEmpleado == oEmp.Id & d.fecha.Year == DateTime.Now.Year).Any();
                    datos.respuesta = dato.ToString().ToLower();
                    datos.candado = dato;
                    datos.inicio = periodo.FechaInicio.Date.ToString("dd/MM/yyyy");
                    datos.fin = periodo.FechaFinal.Date.ToString("dd/MM/yyyy");
                    Session["encuesta"] = datos;
                }
                ViewBag.respuesta = datos.respuesta;
                ViewBag.inicio = datos.inicio;
                ViewBag.fin = datos.fin;
            }
            else
            {
                Session["encuesta"] = null;
                ViewBag.respuesta = "true";
            }
            
             
            return View(oEmp);
        }
        /// <summary>
        /// RRegresa la View de la pagina en construccion
        /// </summary>
        /// <returns></returns>
        public ActionResult PaginaContruccion()
        {
            TempData["Message"] = null;
            ViewBag.Message = 0;
            return View();
        }

        public ActionResult VideoNom035()
        {
            politicaViewModel model = new politicaViewModel();
            using (rhconEntities db = new rhconEntities())
            {
                model = (from p in db.politica
                         where p.tipo == "nom035"
                         select new politicaViewModel
                         {
                             tipo = p.tipo,
                             video = p.video
                         }).FirstOrDefault();
            }

            return View(model);
        }

        public ActionResult Resultados()
        {


            var oEmp = (EmpleadoViewModel)Session["empleado"];

            using (rhconEntities db = new rhconEntities())
            {
                var centro = db.centroTrabajo.Where(d => d.id == oEmp.IdCentro).First();
                ViewBag.idCentro = centro.id;

                ReporteViewModel reporte = new ReporteViewModel();
                var years = db.periodosEncuesta.Where(d => d.idEmpresa == oEmp.IdEmpresa).OrderBy(d => d.year).ToList();
                var fecha = db.periodosEncuesta.Where(d => d.idEmpresa == oEmp.IdEmpresa & d.cierre == 1).OrderByDescending(d => d.id).First();
                string list = "";
                foreach (var year in years)
                {
                    list += year.year.ToString() + " ";

                }
                reporte.years = list;
                reporte.idEmpresa = oEmp.IdEmpresa;
                reporte.idCentro = oEmp.IdCentro;



                SqlConnectionStringBuilder conect = ConexionViewModel.conectar();

                using (SqlConnection connection = new SqlConnection(conect.ConnectionString))
                {
                    connection.Open();
               
                 

                    // consulta a la vista result_categoria
                    var cadena =  " AND t1.idCentroTrabajo =" + oEmp.IdCentro;
                    string consulta_categorias = ConsultasViewModel.Categorias(fecha.year.ToString(), oEmp.IdEmpresa.ToString() , cadena);
                    SqlCommand command_categoria = new SqlCommand(consulta_categorias, connection);
                    // contruccion de los elementos de las categorias 
                    DataResultadosViewModel dt = new DataResultadosViewModel();
                    var empleados = 0;
                    string consulta_totalEncuesta = ConsultasViewModel.TotalRespuestasEmpleados(fecha.year.ToString(), oEmp.IdEmpresa.ToString() , cadena);
                    SqlCommand command_totalEncuesta = new SqlCommand(consulta_totalEncuesta, connection);
                    using (SqlDataReader reader = command_totalEncuesta.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            empleados = int.Parse(reader[0].ToString());
                        }
                    }
                    using (SqlDataReader reader = command_categoria.ExecuteReader())
                    {
                         /*
                          devuelve un objeto de tipo CategoriaViewModel 
                           cada categoria contine las propiedades color,text,estado
                         */
                        dt.categoriasVal = dt.categorias(reader, empleados);
                        reporte.datosReporte = dt;
                    }

                }


                return View(reporte);
            }


        }



        public ActionResult VideoNom025()
        {
            politicaViewModel model = new politicaViewModel();
            using (rhconEntities db = new rhconEntities())
            {
                model = (from p in db.politica
                         where p.tipo == "nom025"
                         select new politicaViewModel
                         {
                             tipo = p.tipo,
                             video = p.video
                         }).FirstOrDefault();
            }

            return View(model);
        }

        public ActionResult ReporteAcciones()
        {
            return View();
        }

        public ActionResult MisActividades()
        {
            using (rhconEntities db = new rhconEntities()) {

                var oEmpresa = (EmpresaViewModel)Session["empresa"];
                var empleado = (EmpleadoViewModel)Session["empleado"];
                int idEmpresa = oEmpresa.Id;
                List<acciones> acciones = db.acciones.
                    Where(d => d.idEmpresa == idEmpresa & d.registro.Value.Year == DateTime.Now.Year).ToList();

                foreach(var accion in acciones)
                {
                    int idUser = int.Parse(accion.responsable);
                    var responsable = db.usuario.Where(d => d.id == idUser).First();
                    accion.responsable = responsable.nombre;

                }
                ViewBag.idUser = empleado.Nombre;

                return View(acciones);
            }
            
        }
    }
}