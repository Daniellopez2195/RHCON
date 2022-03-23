using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rhcon.Models.ViewModel;
using rhcon.Models;
using Rotativa;
using System.IO;
using System.Data.Entity.Core.Objects;
using System.Net.Mail;

namespace rhcon.Controllers
{
    /// <summary>
    /// Controlador para módulo de cuestionario ATS
    /// </summary>
    public class CuestionarioController : Controller
    {
        /// <summary>
        /// GET: Cuestionario
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            List<ListEncabezadoCuestionRioViewModel> model = null;

            using (rhconEntities db = new rhconEntities())
            {
                model = (from c  in db.EncabezadoCuestionario
                         join h in db.cuestionario on c.idCuestionario equals h.id
                         where h.id == 3
                         select new ListEncabezadoCuestionRioViewModel
                         {
                             Id = c.id,
                             Descripcion = c.descripcion,
                             esOpcional = c.esOPcional,
                             IdCuestionario = c.idCuestionario,
                             Cuestionario = h.descripcion,
                             PreguntasList = (from d in db.cuestionarioDetalle
                                              where d.idCategoria == c.id
                                              orderby d.noReactivo
                                              select new ListPreguntasViewModel
                                              {
                                                  Id = d.id,
                                                  NoReactivo = d.noReactivo,
                                                  Reactivo = d.Reactivo,
                                                  IdPosiblesRespuestas = d.idPosiblesResp,
                                                  RespList = (from e in db.posiblesRespDetalle
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



            var empresa = (EmpresaViewModel)Session["empresa"];
            return View(model);
        }
        /// <summary>
        /// Retorna el Index enviando como parámetro un objeto formcollection y un Archivo Post
        /// </summary>
        /// <param name="Form"></param>
        /// <param name="informe"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(FormCollection Form, HttpPostedFileBase informe)
        {
            string informeNombre = "";
            int id_cuestionario = 0;
            int tipo = int.Parse(Form["tipo"]);
            var oEmpleado = (EmpleadoViewModel)Session["empleado"];
            var oEmpresa = (EmpresaViewModel)Session["empresa"];
            // tipos de intforme 
            // tipo 1 : no tiene ats 
            // tipo 2: tiene ats pero no requiere valoracion clinica 
            // tippo 3: tiene ats y requiere valoracion clinica 
            // tipo 2 y 3 usan el mismo formato de informe 
            if (int.Parse(Form["tipo"]) == 1)
            {
                using (rhconEntities db = new rhconEntities())
                {

                    CuestionarioViewModel model = new CuestionarioViewModel();
                    model.Tipo = tipo;

                    var pdf = new ViewAsPdf

                    {
                        ViewName = "~/Views/Cuestionario/informe.cshtml",
                        IsGrayScale = true,
                        Model = model,
                        FileName = "informe.pdf"
                    };


                    var fileName = "Informe-";

                    // Proceso para guardar un pdf en el servidor generado mediante rotativa 
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                    var byteArray = pdf.BuildPdf(ControllerContext);
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
                    string RutaSitio = Server.MapPath("~/");
                    Guid myuuid = Guid.NewGuid();
                    string myuuidAsString = myuuid.ToString();
                    informeNombre = fileName + oEmpleado.Email + "-" + myuuidAsString + ".pdf";
                    string fullPath = System.IO.Path.Combine(RutaSitio + "/Files/" + informeNombre);
                    var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                    fileStream.Write(byteArray, 0, byteArray.Length);
                    fileStream.Close();


                    // grabar el registro del informe generado en la base de datos 
                    var log = new logCuestionarioATS();
                    log.id_empleado = oEmpleado.Id;
                    log.informe = informeNombre;
                    log.tipo = tipo;
                    log.acontecimiento = "1";
                    log.fecha_registro = DateTime.Now;
                    db.logCuestionarioATS.Add(log);
                    db.SaveChanges();
                }


            }
            else
            {


                ObjectParameter id = new ObjectParameter("idCuestionarioATS", new int { });
                using (rhconEntities db = new rhconEntities())
                {
                    int idPregunta = 0;
                    int idRespuesta = 0;
                    int personas = 0;
                    // numero de personas involucradas
                    personas = int.Parse(Form["personas"]);
                    // fecha en que sucedio el ats
                    var fechaO = DateTime.Parse(Form["fechaO"]);
                    // registro de datos generales
                    db.cuestionatioATSInsert(Form["traumatico"], fechaO, Form["hora"], Form["descripcion"], personas, tipo, id);
                    //lista de preguntas del cuestionario
                    List<cuestionarioDetalle> lstpreguntas = db.cuestionarioDetalle.Where(d => d.idCuestionario == 3).ToList();
                    // id del registro de custionarioATS
                    id_cuestionario = Convert.ToInt32(id.Value);

                    foreach (cuestionarioDetalle preguntas in lstpreguntas)
                    {
                        idPregunta = Convert.ToInt32(Form["pre-" + preguntas.id]);
                        idRespuesta = Convert.ToInt32(Form["resp-" + preguntas.id]);
                        //registro de las preguntas referentes a ATS
                        var oTabla = new cuestionarioATSRespuestas();
                        oTabla.id_cuestionarioATS = Convert.ToInt32(id.Value);
                        oTabla.idPregunta = idPregunta;
                        oTabla.idRespuesta = idRespuesta;
                        db.cuestionarioATSRespuestas.Add(oTabla);
                        db.SaveChanges();
                    }

                    string nombre = "";
                    string apellidop = "";
                    string apellidom = "";

                    // registro de personas involucradas si se selecciono por lo menos 1 persona involucrada
                    if (personas != 0)
                    {
                        for (int cont = 1; cont <= personas; cont++)
                        {
                            var persona = new cuestionarioATSPersonas();
                            nombre = Form["nombre" + cont];
                            apellidop = Form["apellidop" + cont];
                            apellidom = Form["apellidom" + cont];
                            // si se selecciono si la persona es un empleado o no guardamos el valor seleccionado
                            if (!String.IsNullOrEmpty(Form["empleado" + cont])) { bool empleado = Form["empleado" + cont].Equals("si"); persona.empleado = empleado; }

                            //registramos de persona
                            persona.id_cuestionarioATS = Convert.ToInt32(id.Value);
                            persona.nombre = nombre;
                            persona.apellidom = apellidom;
                            persona.apellidop = apellidop;
                            db.cuestionarioATSPersonas.Add(persona);
                            db.SaveChanges();
                        }
                    }


                    // generacion del informe tipo 2 y 3
                    // se almacena en un modelo toda la informacion que el usuario contesto 

                    CuestionarioViewModel model = new CuestionarioViewModel();
                    model.Traumatico = Form["traumatico"];
                    model.Descripcion = Form["descripcion"];
                    model.FechaO = DateTime.Parse(Form["fechaO"]);
                    model.Hora = Form["hora"];
                    int per = 0;
                    per = int.Parse(Form["personas"]);
                    model.NPersonas = int.Parse(Form["personas"]);
                    model.Personas = new List<PersonaViewModel>();

                    for (int cont = 1; cont <= per; cont++)
                    {

                        var persona = new PersonaViewModel();
                        persona.nombre = "";
                        persona.apellidom = "";
                        persona.apellidop = "";
                        persona.ifempleado = "";
                        if (!Request.Params["nombre" + cont].Equals(""))
                            persona.nombre = Request.Params["nombre" + cont].ToString();
                        if (!Request.Params["apellidom" + cont].Equals(""))
                            persona.apellidom = Request.Params["apellidom" + cont].ToString();
                        if (!Request.Params["apellidop" + cont].Equals(""))
                            persona.apellidop = Request.Params["apellidop" + cont].ToString();
                        if (!Request.Params["empleado" + cont].Equals(""))
                            persona.ifempleado = Request.Params["empleado" + cont].ToString();

                        model.Personas.Add(persona);


                    }
                    // respuestas de ATS
                    model.pregunta1 = Request.Params["resp-143"].ToString();
                    model.pregunta2 = Request.Params["resp-144"].ToString();
                    model.pregunta3 = Request.Params["resp-146"].ToString();
                    model.pregunta4 = Request.Params["resp-147"].ToString();
                    model.pregunta5 = Request.Params["resp-148"].ToString();
                    model.pregunta6 = Request.Params["resp-149"].ToString();
                    model.pregunta7 = Request.Params["resp-150"].ToString();
                    model.pregunta8 = Request.Params["resp-151"].ToString();
                    model.pregunta9 = Request.Params["resp-153"].ToString();
                    model.pregunta10 = Request.Params["resp-154"].ToString();
                    model.pregunta11 = Request.Params["resp-155"].ToString();
                    model.pregunta12 = Request.Params["resp-156"].ToString();
                    model.pregunta13 = Request.Params["resp-157"].ToString();
                    model.pregunta14 = Request.Params["resp-158"].ToString();

                    int tip = int.Parse(model.Traumatico);
                    string traumatico = "";

                    if (tip == 2)
                    {
                        traumatico = "he sufrido o presenciado un accidente que tuvo como consecuencia la pérdida de un miembro o una lesión grave durante o con motivo del trabajo";
                    }
                    else if (tip == 3)
                    {
                        traumatico = "he sufrido o presenciado un asalto durante o con motivo del trabajo";
                    }
                    else if (tip == 4)
                    {
                        traumatico = "he sufrido o presenciado actos violentos que derivaron en lesiones graves durante o con motivo del trabajo";
                    }
                    else if (tip == 5)
                    {
                        traumatico = "he sufrido o presenciado un secuestro durante o con motivo del trabajo";
                    }
                    else if (tip == 6)
                    {
                        traumatico = "he sufrido o presenciado amenazas durante o con motivo del trabajo";
                    }
                    else if (tip == 7)
                    {
                        traumatico = "he sufrido o presenciado cualquier otro acto que puso en riesgo mi vida o salud, y/o la de otras personas durante o con motivo del trabajo";
                    }


                    // generacion de iforme pasando el modelo 
                    string _headerUrl = Url.Action("HeaderInforme", "Home", new
                    {
                        empresa = oEmpresa.RazonComercial,
                        empleado = oEmpleado.Nombre,
                        traumatico = traumatico,
                        fechao = model.FechaO.ToString("dd/MM/yyyy"),
                        descripcion = model.Descripcion
                    }, "https");
                    var pdf = new ViewAsPdf

                    {
                        ViewName = "~/Views/Cuestionario/informe.cshtml",
                        CustomSwitches = "--header-html " + _headerUrl + " --header-spacing 0 ",
                        IsGrayScale = true,
                        Model = model,
                        FileName = "informe.pdf"
                    };


                    var fileName = "Informe-";

                    // se guarda el informe en el servidor
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                    var byteArray = pdf.BuildPdf(ControllerContext);
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
                    string RutaSitio = Server.MapPath("~/");
                    Guid myuuid = Guid.NewGuid();
                    string myuuidAsString = myuuid.ToString();
                    informeNombre = fileName + oEmpleado.Email + "-" + myuuidAsString + ".pdf";
                    string fullPath = System.IO.Path.Combine(RutaSitio + "/Files/" + informeNombre);
                    var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                    fileStream.Write(byteArray, 0, byteArray.Length);
                    fileStream.Close();


                    // hacemos el registro con los datos del ifnrome creado
                    var log = new logCuestionarioATS();
                    log.id_empleado = oEmpleado.Id;
                    log.id_cuestionarioATS = Convert.ToInt32(id.Value);
                    log.informe = informeNombre;
                    log.tipo = tipo;
                    log.acontecimiento = Form["traumatico"];
                    log.fecha_registro = DateTime.Now;
                    db.logCuestionarioATS.Add(log);
                    db.SaveChanges();


                }

                // fin else general
            }

 


            return Redirect("~/Cuestionario/InformePdf?informe=" + informeNombre);
        }
        /// <summary>
        /// Action result que retorna el informe resultante en PDF
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult InformePdf()
        {
            // pantalla generada una vez que se envia el cuestionario 
            using (rhconEntities db = new rhconEntities())
            {
                CuestionarioViewModel model = new CuestionarioViewModel();
                //se recibe el nombre del informe generado al contestar el cuestionario
                var informe = Request.Params["informe"].ToString();
                // busqueda del registro en la DB
                var log = db.logCuestionarioATS.Where(d => d.informe == informe).First();
                //pasamos los datos correspondientes al modelo 

                int tip = int.Parse(log.acontecimiento);
                if (tip == 2)
                {
                    model.Traumatico = "has sufrido o presenciado un accidente que tuvo como consecuencia la pérdida de un miembro o una lesión grave durante o con motivo del trabajo";
                }
                else if (tip == 3)
                {
                    model.Traumatico = "has sufrido o presenciado un asalto durante o con motivo del trabajo";
                }
                else if (tip == 4)
                {
                    model.Traumatico = "has sufrido o presenciado actos violentos que derivaron en lesiones graves durante o con motivo del trabajo";
                }
                else if (tip == 5)
                {
                    model.Traumatico = "has sufrido o presenciado un secuestro durante o con motivo del trabajo";
                }
                else if (tip == 6)
                {
                    model.Traumatico = "has sufrido o presenciado amenazas durante o con motivo del trabajo";
                }
                else if (tip == 7)
                {
                    model.Traumatico = "has sufrido o presenciado cualquier otro acto que puso en riesgo su vida o salud, y/o la de otras personas durante o con motivo del trabajo";
                }
                else if (tip == 1)
                {
                    model.Traumatico = "no has sufrido o presenciado durante o por motivo de trabajo ningún Acontecimiento Traumático Severo o Violencia Laboral";
                }

                model.Fecha = log.fecha_registro.Value;
                model.Tipo = log.tipo;
                // si el informe es diferente al tipo uno buscamos la fecha del ATS en cuestionarioATS
                if (log.tipo != 1)
                {
                    var quest = db.cuestionarioATS.Where(d => d.id == log.id_cuestionarioATS).First();
                    model.FechaO = quest.fechao;
                }
                // si no recibimos el parametro del informe redirigimos al cuestionario
                if (String.IsNullOrEmpty(informe)) { return Redirect("~/Cuestionario/"); }
                ViewBag.informe = informe;

                return View(model);
            }
        }
        /// <summary>
        /// Header de l Informe PDF
        /// </summary>
        /// <returns></returns>
        public ActionResult headerInformePdf()
        {
            return View();
        }
        /// <summary>
        /// Informe PDF con dós parametros para identificar el archivo y el nombre del informe.
        /// </summary>
        /// <param name="informe"></param>
        /// <param name="nombre"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InformePdf(HttpPostedFileBase informe, string nombre)
        {

            //se guarda en el servidor el archivo subido por el usuario
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            string RutaSitio = Server.MapPath("~/");
            string nombreInforme = "informeFirmado - " + myuuidAsString + ".pdf";
            string archivo = System.IO.Path.Combine(RutaSitio + "/Files/" + nombreInforme);
            informe.SaveAs(archivo);
            var oEmpresa = (EmpresaViewModel)Session["empresa"];
            var oEmpleado = (EmpleadoViewModel)Session["empleado"];

            using (rhconEntities db = new rhconEntities())
            {
                //Busqueda del nombre del informe en la DB
                var pr = db.logCuestionarioATS.Where(d => d.informe == nombre).First();
                string acuseNombre = "";

                // generara un acuse dependiento del tipo de informe que generamos 
                if (pr.tipo == 1)
                {



                    var pdf = new ViewAsPdf("~/Views/Cuestionario/acuse1.cshtml")
                    {
                        PageSize = Rotativa.Options.Size.A5
                    };
                    var fileName = "acuse1-";

#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                    var byteArray = pdf.BuildPdf(ControllerContext);
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
                    RutaSitio = Server.MapPath("~/");
                    myuuid = Guid.NewGuid();
                    myuuidAsString = myuuid.ToString();
                    acuseNombre = fileName + "-" + myuuidAsString + ".pdf";
                    string fullPath = System.IO.Path.Combine(RutaSitio + "/Files/" + acuseNombre);
                    var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                    fileStream.Write(byteArray, 0, byteArray.Length);
                    fileStream.Close();




                    //Envio de email al encargado de la empresa
                    string EmailORigen = "rhstackcode@gmail.com";
                    string EmailDestino = oEmpleado.Email;
                    string pass = "stackcode1.";
                    var body = db.correos.Where(d => d.tipo == "acuse1").First();
                    string mensaje = body.email.ToString();
                    mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                    mensaje = mensaje.Replace("_empresa_", oEmpresa.RazonComercial);
                    mensaje = mensaje.Replace("_empleado_", oEmpleado.Nombre);
                    mensaje = mensaje.Replace("_fecha_", DateTime.Now.ToString("dd/MM/yyyy"));
                    MailMessage EmailMess = new MailMessage(
                        EmailORigen,
                        EmailDestino,
                        "Acuse de recibido",
                        mensaje
                        );
                    EmailMess.IsBodyHtml = true;

                    SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
                    oSmtpClient.EnableSsl = true;
                    oSmtpClient.UseDefaultCredentials = false;
                    oSmtpClient.Host = "smtp.gmail.com";
                    oSmtpClient.Port = 587;
                    oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailORigen, pass);
                    oSmtpClient.Send(EmailMess);
                    oSmtpClient.Dispose();

                }
                else if (pr.tipo == 2)
                {
                    var pd = db.cuestionarioATS.Where(d => d.id == pr.id_cuestionarioATS).First();
                    CuestionarioViewModel model = new CuestionarioViewModel();
                    int tip = int.Parse(pr.acontecimiento);
                    if (tip == 2)
                    {
                        model.Traumatico = "has sufrido o presenciado un accidente que tuvo como consecuencia la pérdida de un miembro o una lesión grave durante o con motivo del trabajo";
                    }
                    else if (tip == 3)
                    {
                        model.Traumatico = "has sufrido o presenciado un asalto durante o con motivo del trabajo";
                    }
                    else if (tip == 4)
                    {
                        model.Traumatico = "has sufrido o presenciado actos violentos que derivaron en lesiones graves durante o con motivo del trabajo";
                    }
                    else if (tip == 5)
                    {
                        model.Traumatico = "has sufrido o presenciado un secuestro durante o con motivo del trabajo";
                    }
                    else if (tip == 6)
                    {
                        model.Traumatico = "has sufrido o presenciado amenazas durante o con motivo del trabajo";
                    }
                    else if (tip == 7)
                    {
                        model.Traumatico = "has sufrido o presenciado cualquier otro acto que puso en riesgo su vida o salud, y/o la de otras personas durante o con motivo del trabajo";
                    }

                    model.FechaO = pd.fechao;
                    model.Fecha = pr.fecha_registro.Value;
                    



                    var pdf = new ViewAsPdf("~/Views/Cuestionario/acuse2.cshtml")
                    {
                        Model = model,
                        PageSize = Rotativa.Options.Size.A5


                    };
                    var fileName = "acuse2-";

#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                    var byteArray = pdf.BuildPdf(ControllerContext);
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
                    RutaSitio = Server.MapPath("~/");
                    myuuid = Guid.NewGuid();
                    myuuidAsString = myuuid.ToString();
                    acuseNombre = fileName + "-" + myuuidAsString + ".pdf";
                    string fullPath = System.IO.Path.Combine(RutaSitio + "/Files/" + acuseNombre);
                    var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                    fileStream.Write(byteArray, 0, byteArray.Length);
                    fileStream.Close();



                    //correo de ats
                    //Envio de email al encargado de la empresa
                    string EmailORigen = "rhstackcode@gmail.com";
                    string EmailDestino = oEmpleado.Email;
                    string pass = "stackcode1.";
                    var body = db.correos.Where(d => d.tipo == "acuse2").First();




                    string mensaje = body.email.ToString();
                    mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                    mensaje = mensaje.Replace("_empresa_", oEmpresa.RazonComercial);
                    mensaje = mensaje.Replace("_empleado_", oEmpleado.Nombre);
                    mensaje = mensaje.Replace("_traumatico_", model.Traumatico);
                    mensaje = mensaje.Replace("_fecha_", DateTime.Now.ToString("dd/MM/yyyy"));
                    mensaje = mensaje.Replace("_fechaR_", pd.fecha.ToString("dd/MM/yyyy"));
                    mensaje = mensaje.Replace("_fechao_", pd.fechao.ToString("dd/MM/yyyy"));
                    MailMessage EmailMess = new MailMessage(
                        EmailORigen,
                        EmailDestino,
                        "Acuse de recibido",
                        mensaje
                        );
                    EmailMess.IsBodyHtml = true;

                    SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
                    oSmtpClient.EnableSsl = true;
                    oSmtpClient.UseDefaultCredentials = false;
                    oSmtpClient.Host = "smtp.gmail.com";
                    oSmtpClient.Port = 587;
                    oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailORigen, pass);
                    oSmtpClient.Send(EmailMess);
                    oSmtpClient.Dispose();


                }
                else if (pr.tipo == 3)
                {
                    var pd = db.cuestionarioATS.Where(d => d.id == pr.id_cuestionarioATS).First();
                    CuestionarioViewModel model = new CuestionarioViewModel();
                    int tip = int.Parse(pr.acontecimiento);
                    if (tip == 2)
                    {
                        model.Traumatico = "has sufrido o presenciado un accidente que tuvo como consecuencia la pérdida de un miembro o una lesión grave durante o con motivo del trabajo";
                    }
                    else if (tip == 3)
                    {
                        model.Traumatico = "has sufrido o presenciado un asalto durante o con motivo del trabajo";
                    }
                    else if (tip == 4)
                    {
                        model.Traumatico = "has sufrido o presenciado actos violentos que derivaron en lesiones graves durante o con motivo del trabajo";
                    }
                    else if (tip == 5)
                    {
                        model.Traumatico = "has sufrido o presenciado un secuestro durante o con motivo del trabajo";
                    }
                    else if (tip == 6)
                    {
                        model.Traumatico = "has sufrido o presenciado amenazas durante o con motivo del trabajo";
                    }
                    else if (tip == 7)
                    {
                        model.Traumatico = "has sufrido o presenciado cualquier otro acto que puso en riesgo su vida o salud, y/o la de otras personas durante o con motivo del trabajo";
                    }
                    model.FechaO = pd.fechao;
                    model.Fecha = pr.fecha_registro.Value;







                    //Envio de email al encargado de la empresa
                    string EmailORigen = "rhstackcode@gmail.com";
                    string EmailDestino = oEmpleado.Email;
                    string pass = "stackcode1.";
                    var body = db.correos.Where(d => d.tipo == "acuse3").First();




                    string mensaje = body.email.ToString();
                    mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                    mensaje = mensaje.Replace("_empresa_", oEmpresa.RazonComercial);
                    mensaje = mensaje.Replace("_empleado_", oEmpleado.Nombre);
                    mensaje = mensaje.Replace("_traumatico_", model.Traumatico);
                    mensaje = mensaje.Replace("_fechao_", pd.fechao.ToString("dd/MM/yyyy"));
                    mensaje = mensaje.Replace("_fecha_", pd.fecha.ToString("dd/MM/yyyy"));
                    mensaje = mensaje.Replace("_fechaR_", DateTime.Now.ToString("dd/MM/yyyy"));

                    MailMessage EmailMess = new MailMessage(
                        EmailORigen,
                        EmailDestino,
                        "Acuse de recibido",
                        mensaje
                        );
                    EmailMess.IsBodyHtml = true;

                    SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
                    oSmtpClient.EnableSsl = true;
                    oSmtpClient.UseDefaultCredentials = false;
                    oSmtpClient.Host = "smtp.gmail.com";
                    oSmtpClient.Port = 587;
                    oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailORigen, pass);
                    oSmtpClient.Send(EmailMess);
                    oSmtpClient.Dispose();



                    var pdf = new ViewAsPdf("~/Views/Cuestionario/acuse3.cshtml")
                    {
                        PageSize = Rotativa.Options.Size.A5,
                        Model = model
                    };
                    var fileName = "acuse3-";

#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                    var byteArray = pdf.BuildPdf(ControllerContext);
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
                    RutaSitio = Server.MapPath("~/");
                    myuuid = Guid.NewGuid();
                    myuuidAsString = myuuid.ToString();
                    acuseNombre = fileName + "-" + myuuidAsString + ".pdf";
                    string fullPath = System.IO.Path.Combine(RutaSitio + "/Files/" + acuseNombre);
                    var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                    fileStream.Write(byteArray, 0, byteArray.Length);
                    fileStream.Close();




                }





                // Actualizamos la tabla de logcuestionarioATS con los nuevos datos del acuse 
                pr.fecha = DateTime.Now;
                pr.verificacion = nombreInforme;
                pr.acuse = acuseNombre;
                db.Entry(pr).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }





            //redirigimos al historial
            return Redirect("~/Cuestionario/Historial");
        }




        /// <summary>
        /// Vista de Historial
        /// </summary>
        /// <param name="pagina"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public ActionResult Historial(int pagina = 1, int tipo = 0)
        {
            using (rhconEntities db = new rhconEntities())
            {
                var oEmpleado = (EmpleadoViewModel)Session["empleado"];
                var oEmpresa = (EmpresaViewModel)Session["empresa"];
                var cantidadDeRegistrosPorPagina = 4;
                var modelo = new ArchivoAtsViewModel();
                // si tipo mantiene su valor de inicio sin ningun filtro  realiza la siguiente accion
                if (tipo == 0)
                {
                    //todos los registro de la tabla logcuestionarioATS ordenados del ultimo al primero
                    var registros = db.logCuestionarioATS.OrderByDescending(x => x.id).Where(d => d.id_empleado == oEmpleado.Id)
                   .Skip((pagina - 1) * cantidadDeRegistrosPorPagina)
                   .Take(cantidadDeRegistrosPorPagina).ToList();
                    // el numero total de registros que existen 
                    var totalDeRegistros = db.logCuestionarioATS.Where(d => d.id_empleado == oEmpleado.Id).Count();

                    //asignamos los valores del modelo encargado de crear la paginacion
                    //datos del registro
                    modelo.Registro = registros;
                    modelo.PaginaActual = pagina;
                    modelo.TotaldeRegistros = totalDeRegistros;
                    modelo.RegistrosPorPagina = cantidadDeRegistrosPorPagina;

                }
                else
                {
                    //si tipo es diferente de 0 se consultan los registros de logcuestionarioATS segun el tipo de registro 
                    var registros = db.logCuestionarioATS.OrderByDescending(x => x.id).Where(d => d.tipo == tipo & d.id_empleado == oEmpleado.Id)
                    .Skip((pagina - 1) * cantidadDeRegistrosPorPagina)
                    .Take(cantidadDeRegistrosPorPagina).ToList();
                    var totalDeRegistros = db.logCuestionarioATS.Where(d => d.tipo == tipo & d.id_empleado == oEmpleado.Id).Count();


                    modelo.Registro = registros;
                    modelo.PaginaActual = pagina;
                    modelo.TotaldeRegistros = totalDeRegistros;
                    modelo.RegistrosPorPagina = cantidadDeRegistrosPorPagina;
                }



                return View(modelo);
            }

        }



    }
}
