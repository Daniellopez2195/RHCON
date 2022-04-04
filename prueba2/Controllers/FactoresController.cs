using Newtonsoft.Json;
using rhcon.Models;
using rhcon.Models.ViewModel;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace rhcon.Controllers
{
    /// <summary>
    /// Controladora para Factores
    /// </summary>
    [HandleError(View = "Error")]
    public class FactoresController : Controller
    {
        /// <summary>
        /// GET: Factores
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            return View();
        }
        /// <summary>
        /// Regresa vista de Activar Periodo de encuestas
        /// </summary>
        /// <returns></returns>
        public ActionResult ActivarPeriodo(int pagina = 1)
        {
            var empresa = (EmpresaViewModel)Session["empresa"];
            using (rhconEntities db = new rhconEntities())
            {
                var modelo = new ArchivoAtsViewModel();
                var cantidadDeRegistrosPorPagina = 5;
                //todos los registro de la tabla logcuestionarioATS ordenados del ultimo al primero
                var registros = db.Log_factores.Where(l => l.id_empresa == empresa.Id)
               .OrderByDescending(x => x.id)
               .Skip((pagina - 1) * cantidadDeRegistrosPorPagina)
               .Take(cantidadDeRegistrosPorPagina).ToList();
                // el numero total de registros que existen 
                var totalDeRegistros = db.Log_factores.Count();

                //asignamos los valores del modelo encargado de crear la paginacion
                //datos del registro
                modelo.logFac = registros;
                modelo.PaginaActual = pagina;
                modelo.TotaldeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadDeRegistrosPorPagina;



                View().ViewBag.model = modelo;


                return View();
            }


        }
        /// <summary>
        /// Postback para Vista de Activar Periodo y registar en la base de datos
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActivarPeriodo(PeriodosEncuestaViewModel model)
        {
            try
            {
                var empresa = (EmpresaViewModel)Session["empresa"];
                if (ModelState.IsValid)
                {
                    //Se obtiene la empresa a partir del usuario guardado en la sesion
                    //Obtenemos la sesion de usuario
                    var oUser = (UserViewModel)Session["user"];
                    string mensaje = "";
                    string asunto = "";
                    int idEmpresa = 0;

                    if (oUser != null)
                    {
                        EncargadoEmpresaViewModel encargado = null;
                        using (rhconEntities bd = new rhconEntities())
                        {
                            encargado = (from d in bd.encargadosEmpresa
                                         where d.idUsuario == oUser.Id
                                         select new EncargadoEmpresaViewModel
                                         {
                                             Id = d.id,
                                             IdUsuario = d.idUsuario,
                                             IdEmpresa = d.idEmpresa
                                         }).FirstOrDefault();

                            if (encargado != null) idEmpresa = encargado.IdEmpresa;
                        }
                        ObjectParameter id = new ObjectParameter("periodoEncuestaID", new int { });

                        using (rhconEntities db = new rhconEntities())
                        {


                            //correos de los empleados 


                            correos body;


                            List<EmpleadoViewModel> correos;


                            //Si existe un periodo de encuesta lo extiende
                            if (db.periodosEncuesta.Where(d => d.idEmpresa == empresa.Id && d.year == DateTime.Now.Year).Any())
                            {

                                correos = (from e in db.empleado
                                           join s in db.usuario on e.idUsuario equals s.id
                                           where e.idEmpresa == empresa.Id
                                           select new EmpleadoViewModel
                                           {
                                               Email = s.email,
                                               Nombre = e.nombre
                                           }).ToList();

                                periodosEncuesta pr = db.periodosEncuesta.Where(d => d.idEmpresa == empresa.Id && d.year == DateTime.Now.Year).First();

                                if (pr.cierre != 1)
                                {
                                    pr.fechaInicio = model.FechaInicio;
                                    pr.fechaFinal = model.FechaFinal;
                                    db.Entry(pr).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();


                                    body = db.correos.Where(d => d.tipo == "extender").First();
                                    mensaje = body.email.ToString();
                                    mensaje = mensaje.Replace("_img_", "http://38.242.215.98/Assets/img/SVG/LOGO/rhlogo.png");
                                    mensaje = mensaje.Replace("_empresa_", empresa.RazonSocial);
                                    mensaje = mensaje.Replace("_inicio_", model.FechaInicio.ToString("dd/MM/yyyy"));
                                    mensaje = mensaje.Replace("_logo_", "http://38.242.215.98/Assets/img/referenciaEvaluacion.png");
                                    mensaje = mensaje.Replace("_fin_", model.FechaFinal.ToString("dd/MM/yyyy"));
                                    mensaje = mensaje.Replace("_redireccion_", "http://38.242.215.98/Home/Login");
                                    asunto = "Extención de periodo de evaluación";



                                    // pdf de Extender
                                    CorreosPdfViewModel modelpdf = new CorreosPdfViewModel();
                                    modelpdf.img = "http://38.242.215.98/Assets/img/SVG/LOGO/rhlogo.png";
                                    modelpdf.empresa = empresa.RazonSocial;
                                    modelpdf.inicio = model.FechaInicio.ToString("dd/MM/yyyy");
                                    modelpdf.logo = "http://38.242.215.98/Assets/img/et1.png";
                                    modelpdf.fin = model.FechaFinal.ToString("dd/MM/yyyy");



                                    //var pdf = new ViewAsPdf

                                    //{
                                    //    ViewName = "~/Views/Factores/Extender.cshtml",
                                    //    IsGrayScale = true,
                                    //    Model = modelpdf,
                                    //    FileName = "informe.pdf"
                                    //};

                                    var pdf = new ViewAsPdf("~/Views/Factores/Extender.cshtml")
                                    {
                                        PageSize = Rotativa.Options.Size.A5,
                                        Model = modelpdf
                                    };


#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                                    var byteArray = pdf.BuildPdf(ControllerContext);
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos

                                    string RutaSitio = Server.MapPath("~/");
                                    Guid myuuid = Guid.NewGuid();
                                    string myuuidAsString = myuuid.ToString();
                                    string nom = "Extender-" + empresa.RFC + "-" + myuuidAsString + ".pdf";
                                    string fullPath = System.IO.Path.Combine(RutaSitio + "/Files/" + nom);
                                    var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                                    fileStream.Write(byteArray, 0, byteArray.Length);
                                    fileStream.Close();

                                    Log_factores log = new Log_factores();
                                    log.id_empresa = empresa.Id;
                                    log.archivo = nom;
                                    log.tipo = 2;
                                    log.fechaInicial = model.FechaInicio;
                                    log.fechaFinal = model.FechaFinal; 
                                    log.fecha = DateTime.Now;

                                    db.Log_factores.Add(log);
                                    db.SaveChanges();


                                    TempData["Message"] = "El periodo de Cuestionarios y Encuestas se ha extendido con éxito";
                                }
                                else
                                {
                                    TempData["Message"] = "Error una vez el cierre definitivo del período se a activado";
                                    return Redirect("~/Empresa/Perfil");

                                }

                            }
                            else // Si no ingresa un nuevo periodo de encuesta
                            {
                                correos = (from e in db.empleado
                                           join s in db.usuario on e.idUsuario equals s.id
                                           where e.idEmpresa == empresa.Id
                                           select new EmpleadoViewModel
                                           {
                                               Email = s.email,
                                               Nombre = e.nombre
                                           }).ToList();

                                db.ActivarPeriodoEncuesta(idEmpresa, model.FechaInicio, model.FechaFinal, oUser.Id, "cedula", (short)model.FechaFinal.Year, "Encargado de Encuesta", id);

                                body = db.correos.Where(d => d.tipo == "activar").First();
                                mensaje = body.email.ToString();
                                mensaje = mensaje.Replace("_img_", "http://38.242.215.98/Assets/img/SVG/LOGO/rhlogo.png");

                                mensaje = mensaje.Replace("_empresa_", empresa.RazonSocial);
                                mensaje = mensaje.Replace("_inicio_", model.FechaInicio.ToString("dd/MM/yyyy"));
                                mensaje = mensaje.Replace("_logo_", "http://38.242.215.98/Assets/img/referenciaEvaluacion.png");
                                mensaje = mensaje.Replace("_fin_", model.FechaFinal.ToString("dd/MM/yyyy"));
                                mensaje = mensaje.Replace("_redireccion_", "http://38.242.215.98/Home/Login");
                                asunto = "Periodo de Evaluación";


                                // pdf de Activar
                                CorreosPdfViewModel modelpdf = new CorreosPdfViewModel();
                                modelpdf.img = "http://38.242.215.98/Assets/img/SVG/LOGO/rhlogo.png";
                                modelpdf.empresa = empresa.RazonSocial;
                                modelpdf.inicio = model.FechaInicio.ToString("dd/MM/yyyy");
                                modelpdf.logo = "http://38.242.215.98/Assets/img/et1.png";
                                modelpdf.fin = model.FechaFinal.ToString("dd/MM/yyyy");



                                var pdf = new ViewAsPdf

                                {
                                    ViewName = "~/Views/Factores/Activar.cshtml",
                                    IsGrayScale = true,
                                    Model = modelpdf,
                                    FileName = "informe.pdf"
                                };


#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                                var byteArray = pdf.BuildPdf(ControllerContext);
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos

                                string RutaSitio = Server.MapPath("~/");
                                Guid myuuid = Guid.NewGuid();
                                string myuuidAsString = myuuid.ToString();
                                string nom = "Activar-" + empresa.RFC + "-" + myuuidAsString + ".pdf";
                                string fullPath = System.IO.Path.Combine(RutaSitio + "/Files/" + nom);
                                var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                                fileStream.Write(byteArray, 0, byteArray.Length);
                                fileStream.Close();

                                Log_factores log = new Log_factores();
                                log.id_empresa = empresa.Id;
                                log.archivo = nom;
                                log.tipo = 1;
                                log.fecha = DateTime.Now;


                                db.Log_factores.Add(log);
                                db.SaveChanges();


                                TempData["Message"] = "El periodo de Cuestionarios y Encuestas fue activado con éxito";
                            }


                            // Se recorren los correos de los empleados de la empresa
                            foreach (var correo in correos)
                            {


                                //validacion de respuesta de encuesta 

                                //Envio de email al empleado
                                string EmailORigen = "rhstackcode@gmail.com";
                                string EmailDestino = correo.Email;
                                string pass = "stackcode1.";
                                mensaje = mensaje.Replace("_empleado_", correo.Nombre);

                                MailMessage EmailMess = new MailMessage(
                                    EmailORigen,
                                    EmailDestino,
                                    asunto,
                                    mensaje
                                    );
                                EmailMess.IsBodyHtml = true;

                                SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
                                oSmtpClient.EnableSsl = true;
                                //oSmtpClient.UseDefaultCredentials = false;
                                oSmtpClient.Host = "smtp.gmail.com";
                                oSmtpClient.Port = 587;
                                oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailORigen, pass);
                                oSmtpClient.Send(EmailMess);
                                oSmtpClient.Dispose();

                                mensaje = mensaje.Replace(correo.Nombre, "_empleado_");

                            }
                        }

                        return Redirect("~/Empresa/Perfil");
                    }
                    else
                    {
                        TempData["Message"] = "A ocurrido un error al activar el periodo de Cuestionarios y Encuestas, consulte al administrador del sistema";
                        return View();
                    }

                }
                else
                    return View();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    throw new Exception(ex.Message);
                else
                    throw new Exception(ex.Message + "\n" + ex.InnerException.Message);
            }

        }

        /// <summary>
        /// Regresa vista de Resultado 
        /// </summary>
        /// <returns></returns>
        public ActionResult Resultados()
        {
            var empresa = (EmpresaViewModel)Session["empresa"];
            FiltrosResultados filtro = new FiltrosResultados();
            using (rhconEntities db = new rhconEntities())
            {
                List<ListCentroTrabajoViewModel> query =
                    (from e in db.centroTrabajo
                     where e.idEmpresa == empresa.Id & e.idStatus == 1
                     select new ListCentroTrabajoViewModel
                     {
                         Id = e.id,
                         Nombre = e.nombre
                     }).ToList();

                var centros = JsonConvert.SerializeObject(query);
                var emp = db.empresa.Where(d => d.id == empresa.Id).First();
                filtro.FechaEmpresa = emp.fecha;
                filtro.Empresa = empresa.Id;
                filtro.Centros = centros;
            }

            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
            }
            else
            {
                ViewBag.Message = "0";
            }
               

            return View(filtro);
        }
        [HttpPost]
        public ActionResult Resultados(FormCollection formCollection)
        {

            var filtros = new ReporteViewModel();
            filtros.tipo = formCollection["tipo"];
            filtros.centros = formCollection["centros"];
            filtros.fecha = formCollection["fecha"];
            filtros.norma = formCollection["norma"];

            filtros.sexo = formCollection["sexo"];
            filtros.edad = formCollection["edad"];
            filtros.estadoCivil = formCollection["estadoCivil"];
            filtros.antiguedad = formCollection["antiguedad"];
            filtros.tiempoPuestoActual = formCollection["tiempoPuestoActual"];
            filtros.tiempoExperienciaLaboral = formCollection["tiempoExperienciaLaboral"];
            filtros.escolaridad = formCollection["escolaridad"];
            filtros.tipoJornada = formCollection["tipoJornada"];
            filtros.tipoContratacion = formCollection["tipoContratacion"];
            filtros.tipoPersonal = formCollection["tipoPersonal"];
            filtros.discapacidad = formCollection["discapacidad"];
            filtros.realizaRotacion = formCollection["realizaRotacion"];
            filtros.parteSectores = formCollection["parteSectores"];

            using (rhconEntities db = new rhconEntities())
            {
                string cadena = "";
                if (!String.IsNullOrEmpty(filtros.centros))
                    cadena = cadena + " AND idCentroTrabajo =" + filtros.centros;

                if (!String.IsNullOrEmpty(filtros.sexo))
                    cadena = cadena + " AND sexo = '_sexo_'";
                cadena = cadena.Replace("_sexo_", filtros.sexo);
                if (!String.IsNullOrEmpty(filtros.edad))
                    cadena = cadena + " AND edad = '_edad_'";
                cadena = cadena.Replace("_edad_", filtros.edad);
                if (!String.IsNullOrEmpty(filtros.estadoCivil))
                    cadena = cadena + " AND estadoCivil = '_estado_'";
                cadena = cadena.Replace("_estado_", filtros.estadoCivil);
                if (!String.IsNullOrEmpty(filtros.antiguedad))
                    cadena = cadena + " AND antiguedad = '_antiguedad_'";
                cadena = cadena.Replace("_antiguedad_", filtros.antiguedad);
                if (!String.IsNullOrEmpty(filtros.tiempoPuestoActual))
                    cadena = cadena + " AND tiempoPuestoActual = '_puestoActual_'";
                cadena = cadena.Replace("_puestoActual_", filtros.tiempoPuestoActual);
                if (!String.IsNullOrEmpty(filtros.tiempoExperienciaLaboral))
                    cadena = cadena + " AND tiempoExperienciaLaboral = '_experiencia_'";
                cadena = cadena.Replace("_experiencia_", filtros.tiempoExperienciaLaboral);
                if (!String.IsNullOrEmpty(filtros.escolaridad))
                    cadena = cadena + " AND escolaridad = '_escolaridad_'";
                cadena = cadena.Replace("_escolaridad_", filtros.escolaridad);
                if (!String.IsNullOrEmpty(filtros.tipoJornada))
                    cadena = cadena + " AND tipoJornada = '_jornada_'";
                cadena = cadena.Replace("_jornada_", filtros.tipoJornada);
                if (!String.IsNullOrEmpty(filtros.tipoContratacion))
                    cadena = cadena + " AND tipoContratacion = '_contratacion_'";
                cadena = cadena.Replace("_contratacion_", filtros.tipoContratacion);
                if (!String.IsNullOrEmpty(filtros.tipoPersonal))
                    cadena = cadena + " AND tipoPersonal = '_personal_'";
                cadena = cadena.Replace("_personal_", filtros.tipoPersonal);
                if (!String.IsNullOrEmpty(filtros.discapacidad))
                    cadena = cadena + " AND discapacidad = '_discapacidad_'";
                cadena = cadena.Replace("_discapacidad_", filtros.discapacidad);
                if (!String.IsNullOrEmpty(filtros.realizaRotacion))
                    cadena = cadena + " AND realizaRotacion = '_rotacion_'";
                cadena = cadena.Replace("_rotacion_", filtros.realizaRotacion);
                if (!String.IsNullOrEmpty(filtros.parteSectores))
                    cadena = cadena + " AND parteSectores = '_sectores_'";
                cadena = cadena.Replace("_sectores_", filtros.parteSectores);


                var empresa = (EmpresaViewModel)Session["empresa"];
                filtros.respuestas = db.respuestaEmpleado.SqlQuery("SELECT * FROM respuestaEmpleado WHERE idEmpresa =" + empresa.Id + cadena).Where(d => d.fecha >= DateTime.Parse("01-01-" + filtros.fecha)).ToList();


;


                var pdf = new ViewAsPdf("~/Views/Factores/Reporte.cshtml")
                {
                    PageSize = Rotativa.Options.Size.A4,
                    Model = filtros

                };


                return pdf;

            }
        }

        /// <summary>
        /// Regresa vista de Estadisticas
        /// </summary>
        /// <returns></returns>
        public ActionResult Estadisticas()
        {
            return View();
        }
        /// <summary>
        /// Regresa vista de Pagina en Construccion
        /// </summary>
        /// <returns></returns>
        public ActionResult PaginaConstruccion()
        {
            return View();
        }

        /// <summary>
        /// Regresa vista de Reforzar
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public ActionResult Reforzar()
        {
            periodosEncuesta pr;
            var empresa = (EmpresaViewModel)Session["empresa"];
            using (rhconEntities db = new rhconEntities())
            {

                pr = db.periodosEncuesta.Where(d => d.idEmpresa == empresa.Id && d.year == DateTime.Now.Year).First();

                if (pr.cierre == 1)
                {
                    TempData["Message"] = "Error una vez el cierre definitivo del periodo se a activado";
                    return Redirect("~/Empresa/Perfil");

                }

                correos body = db.correos.Where(d => d.tipo == "reforzar").First();

                List<EmpleadoViewModel> correos;

                correos = (from e in db.empleado
                           where e.idEmpresa == empresa.Id
                           join s in db.usuario on e.idUsuario equals s.id
                           select new EmpleadoViewModel
                           {
                               Email = s.email,
                               Nombre = e.nombre,
                               Id = e.id

                           }).ToList();


                foreach (var correo in correos)
                {

                    //Envio de email al empleado
                    string mensaje = body.email.ToString();
                    mensaje = mensaje.Replace("_img_", "http://38.242.215.98/Assets/img/SVG/LOGO/rhlogo.png");
                    mensaje = mensaje.Replace("_empresa_", empresa.RazonSocial);
                    mensaje = mensaje.Replace("_inicio_", pr.fechaInicio.ToString("dd/MM/yyyy"));
                    mensaje = mensaje.Replace("_logo_", "http://38.242.215.98/Assets/img/referenciaEvaluacion.png");
                    mensaje = mensaje.Replace("_fin_", pr.fechaFinal.ToString("dd/MM/yyyy"));
                    mensaje = mensaje.Replace("_redireccion_", "http://38.242.215.98/Home/Login");
                    string asunto = "Recordatorio del periodo de evaluación";

                    string EmailORigen = "rhstackcode@gmail.com";
                    string EmailDestino = correo.Email;
                    string pass = "stackcode1.";
                    mensaje = mensaje.Replace("_empleado_", correo.Nombre);

                    MailMessage EmailMess = new MailMessage(
                        EmailORigen,
                        EmailDestino,
                        asunto,
                        mensaje
                        );
                    EmailMess.IsBodyHtml = true;

                    SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
                    oSmtpClient.EnableSsl = true;
                    //oSmtpClient.UseDefaultCredentials = false;
                    oSmtpClient.Host = "smtp.gmail.com";
                    oSmtpClient.Port = 587;
                    oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailORigen, pass);
                    oSmtpClient.Send(EmailMess);
                    oSmtpClient.Dispose();

                    mensaje = mensaje.Replace(correo.Nombre, "_empleado_");
                }

            }


            // pdf de reforzamiento
            CorreosPdfViewModel modelpdf = new CorreosPdfViewModel();
            modelpdf.img = "http://38.242.215.98/Assets/img/SVG/LOGO/rhlogo.png";
            modelpdf.empresa = empresa.RazonSocial;
            modelpdf.inicio = pr.fechaInicio.ToString("dd/MM/yyyy");
            modelpdf.logo = "http://38.242.215.98/Assets/img/et1.png";
            modelpdf.fin = pr.fechaFinal.ToString("dd/MM/yyyy");





            var pdf = new ViewAsPdf("~/Views/Factores/Reforzamiento.cshtml")
            {
                PageSize = Rotativa.Options.Size.A5,
                Model = modelpdf,
            };

            var byteArray = pdf.BuildPdf(ControllerContext);

            string RutaSitio = Server.MapPath("~/");
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            string nom = "Reforzamiento-" + empresa.RFC + "-" + myuuidAsString + ".pdf";
            string fullPath = System.IO.Path.Combine(RutaSitio + "/Files/" + nom);
            var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            fileStream.Write(byteArray, 0, byteArray.Length);
            fileStream.Close();

            Log_factores log = new Log_factores();
            log.id_empresa = empresa.Id;
            log.archivo = nom;
            log.tipo = 3;
            log.fecha = DateTime.Now;

            using (rhconEntities db = new rhconEntities())
            {
                db.Log_factores.Add(log);
                db.SaveChanges();
            }

            TempData["Message"] = "El reforzamiento de Cuestionarios y Encuestas se ha enviado con éxito";
            return Redirect("~/Empresa/Perfil");

        }
        /// <summary>
        /// Cierre definitivo del periodo de Encuesta
        /// </summary>
        /// <returns></returns>
        public ActionResult Cierre()
        {
            using (rhconEntities db = new rhconEntities())
            {

                db.resultados_nom035.Where(d => d.fecha.Value.Year == 2021);
                var empresa = (EmpresaViewModel)Session["empresa"];
                if (db.periodosEncuesta.Where(d => d.idEmpresa == empresa.Id && d.year == DateTime.Now.Year).Any())
                {
                    periodosEncuesta pr = db.periodosEncuesta.Where(d => d.idEmpresa == empresa.Id && d.year == DateTime.Now.Year).First();
                    if (pr.cierre == null)
                    {
                       
                        pr.cierre = 1;
                        db.Entry(pr).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        TempData["Message"] = "El peridio de evaluación fue dado como cierre difinitivo";
                        return Redirect("~/Empresa/Perfil/");
                    }
                    else
                    {
                        TempData["Message"] = "El peridio de evaluación " + DateTime.Now.Year.ToString() + " ya se encuentra deshabilitado";
                        return Redirect("~/Empresa/Perfil/");
                    }
                }

                TempData["Message"] = "Error No existe un periodo Actual";
                return Redirect("~/Empresa/Perfil");
            }

        }


        public ActionResult menuresultados()
        {
            return View();
        }

        public ActionResult MenuEncuesta() {
            return View();
        }   

        public ActionResult Reporte()
        {
            return View();
        }

        public ActionResult ActivarP(int pagina = 1)
        {
            var periodo = new periodosEncuesta();
            using(rhconEntities db = new rhconEntities())
            {
                var empresa = (EmpresaViewModel)Session["empresa"];
                var prueba =  db.periodosEncuesta.Where(d => d.year == DateTime.Now.Year & d.idEmpresa == empresa.Id);
                if (prueba.Any())
                {
                    periodo = prueba.First();
                }
                else
                {
                    periodo = null;
                }
                var modelo = new ArchivoAtsViewModel();
                var cantidadDeRegistrosPorPagina = 5;
                //todos los registro de la tabla logcuestionarioATS ordenados del ultimo al primero
                var registros = db.Log_factores.Where(l => l.id_empresa == empresa.Id & l.tipo == 1)
               .OrderByDescending(x => x.id)
               .Skip((pagina - 1) * cantidadDeRegistrosPorPagina)
               .Take(cantidadDeRegistrosPorPagina).ToList();
                // el numero total de registros que existen 
                var totalDeRegistros = db.Log_factores.Where(l => l.id_empresa == empresa.Id).Count();

                //asignamos los valores del modelo encargado de crear la paginacion
                //datos del registro
                modelo.logFac = registros;
                modelo.PaginaActual = pagina;
                modelo.TotaldeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadDeRegistrosPorPagina;



                View().ViewBag.model = modelo;
                View().ViewBag.periodo = periodo;
                return View();
            }
        }
        [HttpPost]
        public ActionResult ActivarP(PeriodosEncuestaViewModel model)
        {
            //Se obtiene la empresa a partir del usuario guardado en la sesion
            //Obtenemos la sesion de usuario
            var empresa = (EmpresaViewModel)Session["empresa"];
            var oUser = (UserViewModel)Session["user"];
            string mensaje = "";
            string asunto = "";
            int idEmpresa = 0;

            if (oUser != null)
            {
                EncargadoEmpresaViewModel encargado = null;
                using (rhconEntities bd = new rhconEntities())
                {
                    encargado = (from d in bd.encargadosEmpresa
                                 where d.idUsuario == oUser.Id
                                 select new EncargadoEmpresaViewModel
                                 {
                                     Id = d.id,
                                     IdUsuario = d.idUsuario,
                                     IdEmpresa = d.idEmpresa
                                 }).FirstOrDefault();

                    if (encargado != null) idEmpresa = encargado.IdEmpresa;
                }
                ObjectParameter id = new ObjectParameter("periodoEncuestaID", new int { });

                using (rhconEntities db = new rhconEntities())
                {



                    correos body;


                    List<EmpleadoViewModel> correos;

                    correos = (from e in db.empleado
                               join s in db.usuario on e.idUsuario equals s.id
                               where e.idEmpresa == empresa.Id
                               select new EmpleadoViewModel
                               {
                                   Email = s.email,
                                   Nombre = e.nombre
                               }).ToList();

                    db.ActivarPeriodoEncuesta(idEmpresa, model.FechaInicio, model.FechaFinal, oUser.Id, "cedula", (short)model.FechaFinal.Year, "Encargado de Encuesta", id);

                    body = db.correos.Where(d => d.tipo == "activar").First();
                    mensaje = body.email.ToString();
                    mensaje = mensaje.Replace("_img_", "http://38.242.215.98/Assets/img/SVG/LOGO/rhlogo.png");

                    mensaje = mensaje.Replace("_empresa_", empresa.RazonSocial);
                    mensaje = mensaje.Replace("_inicio_", model.FechaInicio.ToString("dd/MM/yyyy"));
                    mensaje = mensaje.Replace("_logo_", "http://38.242.215.98/Assets/img/referenciaEvaluacion.png");
                    mensaje = mensaje.Replace("_fin_", model.FechaFinal.ToString("dd/MM/yyyy"));
                    mensaje = mensaje.Replace("_redireccion_", "http://38.242.215.98/Home/Login");
                    asunto = "Periodo de Evaluación";


                    // pdf de Activar
                    CorreosPdfViewModel modelpdf = new CorreosPdfViewModel();
                    modelpdf.img = "http://38.242.215.98/Assets/logos/bienestar.png";
                    modelpdf.empresa = empresa.RazonSocial;
                    modelpdf.inicio = model.FechaInicio.ToString("dd/MM/yyyy");
                    modelpdf.logo = "http://38.242.215.98/Assets/img/et1.png";
                    modelpdf.fin = model.FechaFinal.ToString("dd/MM/yyyy");



                    var pdf = new ViewAsPdf

                    {
                        ViewName = "~/Views/Factores/Activar.cshtml",
                        IsGrayScale = true,
                        Model = modelpdf,
                        FileName = "informe.pdf"
                    };


#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                    var byteArray = pdf.BuildPdf(ControllerContext);
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos

                    string RutaSitio = Server.MapPath("~/");
                    Guid myuuid = Guid.NewGuid();
                    string myuuidAsString = myuuid.ToString();
                    string nom = "Activar-" + empresa.RFC + "-" + myuuidAsString + ".pdf";
                    string fullPath = System.IO.Path.Combine(RutaSitio + "/Files/" + nom);
                    var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                    fileStream.Write(byteArray, 0, byteArray.Length);
                    fileStream.Close();

                    Log_factores log = new Log_factores();
                    log.id_empresa = empresa.Id;
                    log.archivo = nom;
                    log.tipo = 1;
                    log.fechaInicial = model.FechaInicio;
                    log.fechaFinal = model.FechaFinal;
                    log.fecha = DateTime.Now;


                    db.Log_factores.Add(log);
                    db.SaveChanges();


                    TempData["Message"] = "El periodo de Cuestionarios y Encuestas fue activado con éxito";


                    // Se recorren los correos de los empleados de la empresa
                    foreach (var correo in correos)
                    {


                        //validacion de respuesta de encuesta 

                        //Envio de email al empleado
                        string EmailORigen = "rhstackcode@gmail.com";
                        string EmailDestino = correo.Email;
                        string pass = "stackcode1.";
                        mensaje = mensaje.Replace("_empleado_", correo.Nombre);

                        MailMessage EmailMess = new MailMessage(
                            EmailORigen,
                            EmailDestino,
                            asunto,
                            mensaje
                            );
                        EmailMess.IsBodyHtml = true;

                        SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
                        oSmtpClient.EnableSsl = true;
                        //oSmtpClient.UseDefaultCredentials = false;
                        oSmtpClient.Host = "smtp.gmail.com";
                        oSmtpClient.Port = 587;
                        oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailORigen, pass);
                        oSmtpClient.Send(EmailMess);
                        oSmtpClient.Dispose();

                        mensaje = mensaje.Replace(correo.Nombre, "_empleado_");

                
                }

               

                }
                }

            return Redirect("~/Empresa/Perfil");

        }

        public ActionResult ReforzarP()
        {
            return View();
        }

        public ActionResult ExtenderP(int pagina = 1)
        {
            var periodo = new periodosEncuesta();
            using (rhconEntities db = new rhconEntities())
            {
                var empresa = (EmpresaViewModel)Session["empresa"];
                var prueba = db.periodosEncuesta.Where(d => d.year == DateTime.Now.Year & d.idEmpresa == empresa.Id);
                if (prueba.Any())
                {
                    periodo = prueba.First();
                }
                else
                {
                    periodo.cierre = 0;
                    
                }
                var modelo = new ArchivoAtsViewModel();
                var cantidadDeRegistrosPorPagina = 5;
                //todos los registro de la tabla logcuestionarioATS ordenados del ultimo al primero
                var registros = db.Log_factores.Where(l => l.id_empresa == empresa.Id & l.tipo == 2)
               .OrderByDescending(x => x.id)
               .Skip((pagina - 1) * cantidadDeRegistrosPorPagina)
               .Take(cantidadDeRegistrosPorPagina).ToList();
                // el numero total de registros que existen 
                var totalDeRegistros = db.Log_factores.Where(l => l.id_empresa == empresa.Id).Count();

                //asignamos los valores del modelo encargado de crear la paginacion
                //datos del registro
                modelo.logFac = registros;
                modelo.PaginaActual = pagina;
                modelo.TotaldeRegistros = totalDeRegistros;
                modelo.RegistrosPorPagina = cantidadDeRegistrosPorPagina;



                View().ViewBag.model = modelo;
                View().ViewBag.periodo = periodo;
                return View();
            }
    
        }
        [HttpPost]
        public ActionResult ExtenderP(PeriodosEncuestaViewModel model)
        {
            //Se obtiene la empresa a partir del usuario guardado en la sesion
            //Obtenemos la sesion de usuario
            var empresa = (EmpresaViewModel)Session["empresa"];
            var oUser = (UserViewModel)Session["user"];
            string mensaje = "";
            string asunto = "";
            int idEmpresa = 0;

            if (oUser != null)
            {
                EncargadoEmpresaViewModel encargado = null;
                using (rhconEntities bd = new rhconEntities())
                {
                    encargado = (from d in bd.encargadosEmpresa
                                 where d.idUsuario == oUser.Id
                                 select new EncargadoEmpresaViewModel
                                 {
                                     Id = d.id,
                                     IdUsuario = d.idUsuario,
                                     IdEmpresa = d.idEmpresa
                                 }).FirstOrDefault();

                    if (encargado != null) idEmpresa = encargado.IdEmpresa;
                }
                ObjectParameter id = new ObjectParameter("periodoEncuestaID", new int { });

                using (rhconEntities db = new rhconEntities())
                {



                    correos body;


                    List<EmpleadoViewModel> correos;

                    correos = (from e in db.empleado
                               join s in db.usuario on e.idUsuario equals s.id
                               where e.idEmpresa == empresa.Id
                               select new EmpleadoViewModel
                               {
                                   Email = s.email,
                                   Nombre = e.nombre
                               }).ToList();

                    periodosEncuesta pr = db.periodosEncuesta.Where(d => d.idEmpresa == empresa.Id && d.year == DateTime.Now.Year).First();

          
                        pr.fechaInicio = model.FechaInicio;
                        pr.fechaFinal = model.FechaFinal;
                        db.Entry(pr).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();


                        body = db.correos.Where(d => d.tipo == "extender").First();
                        mensaje = body.email.ToString();
                        mensaje = mensaje.Replace("_img_", "http://38.242.215.98/Assets/img/SVG/LOGO/rhlogo.png");
                        mensaje = mensaje.Replace("_empresa_", empresa.RazonSocial);
                        mensaje = mensaje.Replace("_inicio_", model.FechaInicio.ToString("dd/MM/yyyy"));
                        mensaje = mensaje.Replace("_logo_", "http://38.242.215.98/Assets/img/referenciaEvaluacion.png");
                        mensaje = mensaje.Replace("_fin_", model.FechaFinal.ToString("dd/MM/yyyy"));
                        mensaje = mensaje.Replace("_redireccion_", "http://38.242.215.98/Home/Login");
                        asunto = "Extención de periodo de evaluación";



                        // pdf de Extender
                        CorreosPdfViewModel modelpdf = new CorreosPdfViewModel();
                        modelpdf.img = "http://38.242.215.98/Assets/img/SVG/LOGO/rhlogo.png";
                        modelpdf.empresa = empresa.RazonSocial;
                        modelpdf.inicio = model.FechaInicio.ToString("dd/MM/yyyy");
                        modelpdf.logo = "http://38.242.215.98/Assets/img/et1.png";
                        modelpdf.fin = model.FechaFinal.ToString("dd/MM/yyyy");



                        //var pdf = new ViewAsPdf

                        //{
                        //    ViewName = "~/Views/Factores/Extender.cshtml",
                        //    IsGrayScale = true,
                        //    Model = modelpdf,
                        //    FileName = "informe.pdf"
                        //};

                        var pdf = new ViewAsPdf("~/Views/Factores/Extender.cshtml")
                        {
                            PageSize = Rotativa.Options.Size.A5,
                            Model = modelpdf
                        };


#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                        var byteArray = pdf.BuildPdf(ControllerContext);
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos

                        string RutaSitio = Server.MapPath("~/");
                        Guid myuuid = Guid.NewGuid();
                        string myuuidAsString = myuuid.ToString();
                        string nom = "Extender-" + empresa.RFC + "-" + myuuidAsString + ".pdf";
                        string fullPath = System.IO.Path.Combine(RutaSitio + "/Files/" + nom);
                        var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                        fileStream.Write(byteArray, 0, byteArray.Length);
                        fileStream.Close();

                        Log_factores log = new Log_factores();
                        log.id_empresa = empresa.Id;
                        log.archivo = nom;
                        log.tipo = 2;
                        log.fechaInicial = model.FechaInicio;
                        log.fechaFinal = model.FechaFinal;
                        log.fecha = DateTime.Now;

                        db.Log_factores.Add(log);
                        db.SaveChanges();


                        TempData["Message"] = "El periodo de Cuestionarios y Encuestas se ha extendido con éxito";


                        // Se recorren los correos de los empleados de la empresa
                        foreach (var correo in correos)
                    {


                        //validacion de respuesta de encuesta 

                        //Envio de email al empleado
                        string EmailORigen = "rhstackcode@gmail.com";
                        string EmailDestino = correo.Email;
                        string pass = "stackcode1.";
                        mensaje = mensaje.Replace("_empleado_", correo.Nombre);

                        MailMessage EmailMess = new MailMessage(
                            EmailORigen,
                            EmailDestino,
                            asunto,
                            mensaje
                            );
                        EmailMess.IsBodyHtml = true;

                        SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
                        oSmtpClient.EnableSsl = true;
                        //oSmtpClient.UseDefaultCredentials = false;
                        oSmtpClient.Host = "smtp.gmail.com";
                        oSmtpClient.Port = 587;
                        oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailORigen, pass);
                        oSmtpClient.Send(EmailMess);
                        oSmtpClient.Dispose();

                        mensaje = mensaje.Replace(correo.Nombre, "_empleado_");


                    }



                }
            }

            return Redirect("~/Empresa/Perfil");
        }

        public ActionResult CerrarP()
        {
            return View();
        }

        public ActionResult Historial(int pagina = 1)
        {
            var empresa = (EmpresaViewModel)Session["empresa"];
            using (rhconEntities db = new rhconEntities())
            {
              
                    var modelo = new ArchivoAtsViewModel();
                    var cantidadDeRegistrosPorPagina = 5;
                    //todos los registro de la tabla logcuestionarioATS ordenados del ultimo al primero
                    var registros = db.Log_factores.Where(l => l.id_empresa == empresa.Id)
                   .OrderByDescending(x => x.id)
                   .Skip((pagina - 1) * cantidadDeRegistrosPorPagina)
                   .Take(cantidadDeRegistrosPorPagina).ToList();
                    // el numero total de registros que existen 
                    var totalDeRegistros = db.Log_factores.Where(l => l.id_empresa == empresa.Id).Count();

                    //asignamos los valores del modelo encargado de crear la paginacion
                    //datos del registro
                    modelo.logFac = registros;
                    modelo.PaginaActual = pagina;
                    modelo.TotaldeRegistros = totalDeRegistros;
                    modelo.RegistrosPorPagina = cantidadDeRegistrosPorPagina;

                    var log = db.periodosEncuesta.Where(d => d.idEmpresa == empresa.Id & d.cierre == 1).ToList();
                    
                    View().ViewBag.model = modelo;
                     ViewBag.log = log;

                }
                return View();
        }


        public ActionResult Movimientos(string year)
        {
            using (rhconEntities db = new rhconEntities())
            {
                var empresa = (EmpresaViewModel)Session["empresa"];
                List<Log_factores> data =  db.Log_factores.Where(d => d.fecha.Value.Year.ToString() == year && d.id_empresa == empresa.Id).ToList();

                string _headerUrl = Url.Action("HeaderFactores", "Home", new
                {
                    logo = empresa.strlogotipo,
                    nombre = empresa.RazonSocial

                }, "https");
                var pdf = new ViewAsPdf("~/Views/Factores/movimientosPdf.cshtml", data)
                {
                    FileName = "Reporte de movimientos.pdf",
                    CustomSwitches = "--header-html " + _headerUrl + " --header-spacing 0 ",
                    PageSize = Rotativa.Options.Size.Letter,
                    PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
                };


                return pdf;
            }

            
        }
        public ActionResult Aplicacion(string year)
        {
            using (rhconEntities db = new rhconEntities())
            {
                var empresa = (EmpresaViewModel)Session["empresa"];
                var data = db.Log_factores.Where(d => d.fecha.Value.Year.ToString() == year && d.id_empresa == empresa.Id && d.tipo == 1).First();

                ViewBag.log = data;
                 var query =
                       (from e in db.resultados_por_usuario_035
                        join s in db.empleado on e.id equals s.id 
                        where e.idEmpresa == empresa.Id & e.fecha.Value.Year.ToString() == year
                        select new aplicacionPdf
                        {
                            nombre = s.nombre,
                            fecha = e.fecha.Value,

     
                        }).ToList();




              

                string _headerUrl = Url.Action("HeaderFactores", "Home", new
                {
                    logo = empresa.strlogotipo,
                    nombre = empresa.RazonSocial

                }, "https");

                var pdf = new ViewAsPdf("~/Views/Factores/aplicacion.cshtml", query)
                {
                    FileName = "Historial de aplicacion.pdf",
                    CustomSwitches = "--header-html " + _headerUrl + " --header-spacing 0 ",
                    PageSize = Rotativa.Options.Size.Letter,
                    PageMargins = new Rotativa.Options.Margins(10,10,10, 10),
                };

                return pdf;
            }
        }
    }
}