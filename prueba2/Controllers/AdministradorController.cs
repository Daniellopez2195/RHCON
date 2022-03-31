using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rhcon.Models.ViewModel;
using rhcon.Models;
using rhcon.utils;
using System.Net.Mail;

namespace rhcon.Controllers
{
    /// <summary>
    /// Controlador para el Panel de Administrador, así como sus funcionalidades.
    /// </summary>
    [HandleError(View = "Error")]
    public class AdministradorController : Controller
    {
        /// <summary>
        /// GET: Administrador.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Action Result que retorna la vista de página en contrucción.
        /// </summary>
        /// <returns></returns>
        public ActionResult PaginaConstruccion()
        {
            return View();
        }

        public ActionResult perfiladmin()
        {
            using (rhconEntities db = new rhconEntities())
            {
                var query =
                  (from e in db.usuario
                   join s in db.admin_perfil on e.id equals s.idUsuario
                   where e.id == 3
                   select new AdminViewModel
                   {
                       Id = e.id,
                       Email = e.email,
                       cedula = s.cedula,
                       nombre = e.nombre,
                       celular = s.celular,
                       contacto = s.contacto,
                       celcontacto = s.celcontacto,
                       Password = e.password,

                   }).First();

                return View(query);
            }

        }

        public ActionResult datosadmin()
        {

            using (rhconEntities db = new rhconEntities())
            {
                var query =
                  (from e in db.usuario
                   join s in db.admin_perfil on e.id equals s.idUsuario
                   where e.id == 3
                   select new AdminViewModel
                   {
                       Id = e.id,
                       Email = e.email,
                       cedula = s.cedula,
                       nombre = e.nombre,
                       celular = s.celular,
                       contacto = s.contacto,
                       celcontacto = s.celcontacto,
                       Password = e.password,

                   }).First();

                return View(query);
            }

        }

        [HttpPost]
        public ActionResult datosadmin(AdminViewModel model)
        {

            using (rhconEntities db = new rhconEntities())

            {



                admin_perfil consulta = db.admin_perfil.First();
                consulta.celular = model.celular;
                consulta.cedula = model.cedula;
                consulta.celcontacto = model.celcontacto;
                consulta.contacto = model.contacto;
                db.Entry(consulta).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                usuario usuario = db.usuario.Where(d => d.id == 3).First();

                

                if (!usuario.email.Equals(model.Email))
                {
                    string password = utilerias.RandomString(12);
                    db.userUpdate(3, model.nombre, model.Email, password, 2);


                    //Envio de email al encargado de la empresa
                    string EmailORigen = "rhstackcode@gmail.com";
                    string EmailDestino = model.Email;
                    string pass = "stackcode1.";
                    var body = db.correos.Where(d => d.tipo == "admin").First();
                    string mensaje = body.email.ToString();
                    mensaje = mensaje.Replace("_img_", "http://38.242.215.98/Assets/img/SVG/LOGO/rhlogo.png");
                    mensaje = mensaje.Replace("_empleado_", model.nombre);
                    mensaje = mensaje.Replace("_usuario_", model.Email);
                    mensaje = mensaje.Replace("_pass_", password);
                    mensaje = mensaje.Replace("_redireccion_", "http://38.242.215.98/Home/Login?IdRol=" + 2+ "&email=" + model.Email + "&password=" + password);
                    mensaje = mensaje.Replace("_tipousuario_", "Usuario Empresa");
                    MailMessage EmailMess = new MailMessage(
                        EmailORigen,
                        EmailDestino,
                        "Bienvenido a RHCON",
                        mensaje
                        );
                    EmailMess.IsBodyHtml = true;

                    SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
                    oSmtpClient.EnableSsl = true;
                    oSmtpClient.Host = "smtp.gmail.com";
                    oSmtpClient.Port = 587;
                    oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailORigen, pass);
                    try
                    {
                        oSmtpClient.Send(EmailMess);
                        oSmtpClient.Dispose();
                    }
                    catch (Exception)
                    {
                      
                        throw;
                    }

                }
                else
                {
                    usuario.nombre = model.nombre;
                    db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }



            }




            return Redirect("~/Administrador/perfiladmin");
        }

        public ActionResult AddAcciones()
        {
            using (rhconEntities db = new rhconEntities())
            {
                var dimensiones = db.dimencion.ToList();

                return View(dimensiones);
            }
        }


    }
}