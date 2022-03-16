using rhcon.Models;
using rhcon.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace rhcon.Controllers
{
    /// <summary>
    /// Controlador para Encuesta
    /// </summary>
    [HandleError(View = "Error")]
    public class EncuestaController : Controller
    {
        /// <summary>
        /// GET Encuesta()
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                var datos = (DatosDeEncuestaViewModel)Session["encuesta"];

                if(datos==null)
                {
                    TempData["MessageActivo"] = "Actualmente el periodo de evaluación de Factores de Riesgo Psicosocial, Entorno y Clima Organizacional no se encuentra activado.";
                    return Redirect("~/Empleado/");
                }
                else
                {
                    if (datos.candado)
                    {
                        TempData["Message"] = "Tu evaluación ya fue registrada";
                        return Redirect("~/Empleado/");
                    }
                    else
                    {
                        //Declaracion de lista para Encabezados
                        List<ListEncabezadoCuestionRioViewModel> model = null;
                        //Se carga la instancia de la base de datos
                        using (rhconEntities db = new rhconEntities())
                        {
                            //Llenar los encabezados
                            model = (from c in db.EncabezadoCuestionario
                                     join h in db.cuestionario on c.idCuestionario equals h.id
                                     where c.idCuestionario == 1 || c.idCuestionario == 2 
                                     orderby c.idCuestionario descending
                                     select new ListEncabezadoCuestionRioViewModel
                                     {
                                         Id = c.id,
                                         Descripcion = c.descripcion,
                                         esOpcional = c.esOPcional,
                                         IdCuestionario = c.idCuestionario,
                                         Cuestionario = h.descripcion,
                                         Nomenclatura = h.nomenclatura,
                                         PreguntasList = (from d in db.cuestionarioDetalle //Llenar La lista de Preguntas
                                                          where d.idCategoria == c.id
                                                          orderby d.noReactivo
                                                          select new ListPreguntasViewModel
                                                          {
                                                              Id = d.id,
                                                              NoReactivo = d.noReactivo,
                                                              Reactivo = d.Reactivo,
                                                              IdPosiblesRespuestas = d.idPosiblesResp,
                                                              RespList = (from e in db.posiblesRespDetalle //Llenar la lista de respuestas
                                                                          where e.idEncabezado == d.idPosiblesResp
                                                                          select new ListRespuestasViewModel
                                                                          {
                                                                              Id = e.id,
                                                                              IdEncabezado = e.idEncabezado,
                                                                              Respuesta = e.Respuesta,
                                                                              Valor = e.valor
                                                                          }).ToList()
                                                          }).ToList()
                                     }).ToList();
                        }
                        //Se obtiene la variable de Empresa a partir de la sesion
                        var empresa = (EmpresaViewModel)Session["empresa"];
                        PeriodosEncuestaViewModel periodo;
                        using (rhconEntities db = new rhconEntities())
                        {
                            periodo = (from d in db.periodosEncuesta
                                       where d.idEmpresa == empresa.Id
                                       select new PeriodosEncuestaViewModel
                                       {
                                           FechaInicio = d.fechaInicio,
                                           FechaFinal = d.fechaFinal
                                       }).FirstOrDefault();


                        }
                        //Se pasan las variables de Fechas en e lviewBag para a vista
                        ViewBag.inicio = periodo.FechaInicio.Date.ToString("dd/MM/yyyy");
                        ViewBag.fin = periodo.FechaFinal.Date.ToString("dd/MM/yyyy");
                        //Se regresa la vista co el modelo cargado
                        return View(model);
                    }
                }               
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Regresa el Index para postback y aceptar las politicas
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var oEmp = (EmpleadoViewModel)Session["empleado"];

                    int idPregunta = 0, idEmpleado = oEmp.Id;
                    int? idRespuesta = 0;
                    var oRespEmp = new respuestaEmpleado();
                    oRespEmp.sexo = formCollection["sexo"];
                    oRespEmp.edad = formCollection["edad"];
                    oRespEmp.estadoCivil = formCollection["estadoCivil"];
                    oRespEmp.antiguedad = formCollection["antiguedad"];
                    oRespEmp.tiempoPuestoActual = formCollection["tiempoPuestoActual"];
                    oRespEmp.tiempoExperienciaLaboral = formCollection["tiempoExperienciaLaboral"];
                    oRespEmp.escolaridad = formCollection["escolaridad"];
                    oRespEmp.tipoJornada = formCollection["tipoJornada"];
                    oRespEmp.tipoContratacion = formCollection["tipoContratacion"];
                    oRespEmp.tipoPersonal = formCollection["tipoPersonal"];
                    oRespEmp.tieneDiscapacidad = formCollection["tieneDiscapacidad"];
                    oRespEmp.discapacidad = formCollection["discapacidad"];
                    oRespEmp.realizaRotacion = formCollection["realizaRotacion"];

                    //seccion nueva
                    oRespEmp.parteSectores = formCollection["parteSectores"];
                    if (formCollection["sector"].Equals("otro")) { oRespEmp.sector = formCollection["otro"]; } else { oRespEmp.sector = formCollection["sector"]; }
                    oRespEmp.existePolitica = formCollection["existePolitica"];
                    oRespEmp.existeCodigoEtica = formCollection["existeCodigoEtica"];
                    oRespEmp.existeComite = formCollection["existeComite"];
                    oRespEmp.existeMecanismo = formCollection["existeMecanismo"];
                    oRespEmp.comentario = formCollection["comentario"];
                    oRespEmp.idEmpresa = oEmp.IdEmpresa;
                    oRespEmp.idCentroTrabajo = oEmp.IdCentro;
                    oRespEmp.idCuestionario = 1;
                    oRespEmp.dondeTrabajo = formCollection["dondeTrabajo"];

                    ObjectParameter id = new ObjectParameter("idRespEmpl", new int { });



                    using (rhconEntities db = new rhconEntities())
                    {
                        db.respEmplInsert(oRespEmp.sexo, oRespEmp.edad, oRespEmp.estadoCivil, oRespEmp.antiguedad, oRespEmp.tiempoPuestoActual, oRespEmp.tiempoExperienciaLaboral, oRespEmp.escolaridad, oRespEmp.tipoJornada, oRespEmp.tipoContratacion, oRespEmp.tipoPersonal, oRespEmp.tieneDiscapacidad, oRespEmp.discapacidad, oRespEmp.realizaRotacion, oRespEmp.idCuestionario, oRespEmp.idEmpresa, oRespEmp.idCentroTrabajo,oRespEmp.parteSectores, oRespEmp.sector, oRespEmp.existePolitica, oRespEmp.existeCodigoEtica, oRespEmp.existeComite, oRespEmp.existeMecanismo, oRespEmp.comentario,oEmp.Id, oRespEmp.dondeTrabajo, id);

                        List<cuestionarioDetalle> lstpreguntas = db.cuestionarioDetalle.Where(d => d.idCuestionario == 1 || d.idCuestionario == 2).ToList();
                        foreach (cuestionarioDetalle preguntas in lstpreguntas)
                        {
                            idPregunta = Convert.ToInt32(formCollection["pre-" + preguntas.id]);
                            idRespuesta = Convert.ToInt32(formCollection["resp-" + preguntas.id]);

                            var oTabla = new respuestaEmpDetalle();
                            oTabla.idResuestaEmpleado = Convert.ToInt32(id.Value);
                            oTabla.idPregunta = idPregunta;
                            oTabla.idRespuesta = idRespuesta;

                            db.respuestaEmpDetalle.Add(oTabla);
                            db.SaveChanges();
                        }
                    }

                    ViewBag.Message = "La encuesta se realizo de forma satisfactoria";
                    return Redirect("~/Encuesta/Mensaje");
                }
                return View();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Regresa Vista de Mensaje
        /// </summary>
        /// <returns></returns>
        public ActionResult Mensaje()
        {
            var empresa = (EmpresaViewModel)Session["empresa"];
            PeriodosEncuestaViewModel periodo;
            using (rhconEntities db = new rhconEntities())
            {
                periodo = (from d in db.periodosEncuesta
                           where d.idEmpresa == empresa.Id && d.year == DateTime.Now.Year
                           select new PeriodosEncuestaViewModel
                           {
                               FechaInicio = d.fechaInicio,
                               FechaFinal = d.fechaFinal
                           }).FirstOrDefault();
            }
            ViewBag.logo = empresa.strlogotipo;
            ViewBag.inicio = periodo.FechaInicio.Date.ToString("dd/MM/yyyy");
            ViewBag.fin = periodo.FechaFinal.Date.ToString("dd/MM/yyyy");
            return View();
        }
    }
}