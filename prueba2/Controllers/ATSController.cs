using rhcon.Models;
using rhcon.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;


namespace rhcon.Controllers
{
    /// <summary>
    /// Controlador para modulo "ATS" (Envío de mensajes)
    /// </summary>
    [HandleError(View = "Error")]
    public class ATSController : Controller
    {
        /// <summary>
        /// GET: ATS
        /// </summary>
        /// <returns></returns>
        public ActionResult Recordatorio()
        {
            if (TempData["Message"] != null)
                ViewBag.Message = TempData["Message"].ToString();

            return View();
        }
        /// <summary>
        /// Métoldo para enviar correos masivos a todos los empleados de la empresa donde se este trabajando.
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="final"></param>
        /// <returns></returns>
        public ActionResult Masiva(string inicio, string final)
        {
            try
            {
                using (rhconEntities db = new rhconEntities())
                {
                    var empresa = (EmpresaViewModel)Session["empresa"];


                    correos body = db.correos.Where(d => d.tipo == "masivo").First();                    

                    List<EmpleadoViewModel> correos;

                    correos = (from e in db.empleado
                               where e.idEmpresa == empresa.Id
                               join s in db.usuario on e.idUsuario equals s.id
                               select new EmpleadoViewModel
                               {
                                   Email = s.email,
                                   Nombre = e.nombre
                               }).ToList();


                    foreach (var correo in correos)
                    {

                        //Envio de email al empleado

                        string mensaje = body.email.ToString();
                        mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                        mensaje = mensaje.Replace("_empresa_", empresa.RazonSocial);
                        mensaje = mensaje.Replace("_logo_", "http://www.nom035.sistemascontino.com.mx/img/iconosp/at.png");
                        mensaje = mensaje.Replace("_redireccion_", "https://bienestarlaboral.rhcon.com.mx/Home/Login");
                        string asunto = "Periodo de Evaluación";

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
                        oSmtpClient.UseDefaultCredentials = false;
                        oSmtpClient.Host = "smtp.gmail.com";
                        oSmtpClient.Port = 587;
                        oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailORigen, pass);
                        oSmtpClient.Send(EmailMess);
                        oSmtpClient.Dispose();

                    }

                }
                // Redirecciona al Index
                TempData["Message"] = "El perido de Acontecimiento Traumático Severo se ha enviado con éxito a todos los empleados(as)";
                return Redirect("~/Empresa/Perfil");
            }
            catch (Exception)
            {

                throw;
            }           
        }
        /// <summary>
        /// Método para enviar correos a empleados seleccionados.
        /// </summary>
        /// <param name="lista"></param>
        /// <param name="inicio"></param>
        /// <param name="final"></param>
        /// <returns></returns>
        public ActionResult Selectiva(string lista, string inicio, string final)
        {

            try
            {
                if (lista != null)
                {
                    if (lista.Equals(""))
                    {
                        TempData["Message"] = "No ha seleccionado ningun empleado";
                        return Redirect("~/ATS/Recordatorio");
                    }
                    if (inicio.Equals(""))
                    {
                        TempData["Message"] = "No ha seleccionado fecha de inicio";
                        return Redirect("~/ATS/Recordatorio");
                    }
                    if (final.Equals(""))
                    {
                        TempData["Message"] = "No ha seleccionado fecha final";
                        return Redirect("~/ATS/Recordatorio");
                    }
                    string[] arr = lista.Split(',');
                    foreach (string dato in arr)
                    {
                        //enviar correo

                        using (rhconEntities db = new rhconEntities())
                        {
                            var empresa = (EmpresaViewModel)Session["empresa"];


                            correos body = db.correos.Where(d => d.tipo == "personal").First();
                            string mensaje = body.email.ToString();
                            mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                            mensaje = mensaje.Replace("_empresa_", empresa.RazonSocial);
                            mensaje = mensaje.Replace("_inicio_", inicio.ToString());
                            mensaje = mensaje.Replace("_fin_", final.ToString());
                            mensaje = mensaje.Replace("_logo_", "http://www.nom035.sistemascontino.com.mx/img/iconosp/at.png");
                            mensaje = mensaje.Replace("_redireccion_", "https://bienestarlaboral.rhcon.com.mx/Home/Login");
                            string asunto = "Periodo de Evaluación";


                            EmpleadoViewModel correo;

                            correo = (from e in db.empleado
                                      where e.idEmpresa == empresa.Id
                                      join s in db.usuario on e.idUsuario equals s.id
                                      where s.email == dato
                                      select new EmpleadoViewModel
                                      {
                                          Email = s.email,
                                          Nombre = e.nombre
                                      }).First();


                            //Envio de email al empleado
                            string EmailORigen = "rhstackcode@gmail.com";
                            string EmailDestino = dato;
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
                            oSmtpClient.UseDefaultCredentials = false;
                            oSmtpClient.Host = "smtp.gmail.com";
                            oSmtpClient.Port = 587;
                            oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailORigen, pass);
                            oSmtpClient.Send(EmailMess);
                            oSmtpClient.Dispose();


                        }
                    }
                }

                else
                {
                    TempData["Message"] = "No ha seleccionado ningun empleado";
                    return Redirect("~/ATS/Recordatorio");
                }

                // Redirecciona al Index
                TempData["Message"] = "El perido de Acontecimiento Traumático Severo se ha enviado con éxito a los empleados(as) seleccionados(as)";
                return Redirect("~/Empresa/Perfil");

            }
            catch (Exception)
            {
                throw;

            }

            
        }
    }
}