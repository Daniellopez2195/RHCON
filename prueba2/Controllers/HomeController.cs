using rhcon.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using rhcon.Models;
using System.Data.Entity.Core.Objects;
using System.Net.Mail;

namespace rhcon.Controllers
{
    /// <summary>
    /// Controladora para Home, contiene la pantalla inicial y los procedimientos para Login
    /// </summary>
    [HandleError(View = "Error")]
    public class HomeController : Controller
    {
        /// <summary>
        /// Regresa Vista Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Regresa Vista About
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        /// <summary>
        /// Regresa Vista Contacto
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult HeaderFactores()
        {
            return View();
        }
        /// <summary>
        /// Regresa Vista Linea Directa
        /// </summary>
        /// <returns></returns>
        public ActionResult LineDirect()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        /// <summary>
        /// Regresa Vista Login
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            try
            {
                List<RolViewModel> list = null;

                int idRol = 0;
                string nameuser = "";
                string password = "";

                //Parametros Request en la URL en caso de exisir

                if (Request.Params["IdRol"] != null)
                    if (!Request.Params["IdRol"].Equals(""))
                        idRol = Convert.ToInt32(Request.Params["IdRol"]);

                if (Request.Params["email"] != null)
                    if (!Request.Params["email"].Equals(""))
                        nameuser = Request.Params["email"].ToString();

                if (Request.Params["password"] != null)
                    if (!Request.Params["password"].Equals(""))
                        password = Request.Params["password"].ToString();

                //Conexion a la base de datos
                using (rhconEntities db = new rhconEntities())
                {
                    list =
                         (from d in db.rol
                          orderby d.orden
                          select new RolViewModel
                          {
                              Id = d.id,
                              Descripcion = d.nombre
                          }).ToList();

                }
                //Convierte la lista a SelectListItem que es la que utilizael dropdownlist
                List<SelectListItem> items = list.ConvertAll(d =>
                {
                    return new SelectListItem()
                    {
                        Text = d.Descripcion,
                        Value = d.Id.ToString(),
                        Selected = false
                    };
                });

                // Initialize the model
                LoginViewModel model = new LoginViewModel();
                model.RolList = items;
                if (idRol != 0)
                    model.IdRol = idRol;
                if (!nameuser.Equals(""))
                    model.nombre = nameuser;
                if (!password.Equals(""))
                    model.password = password;

                return View(model);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// POstback para Login, Inicio de Session e inicializacion de Variables
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Declaracion de variables para la obtención de los campos en el formulario a través de un objeto de la clase formCollection
                    int idRol = model.IdRol;
                    string usuario = model.nombre;
                    string password = model.password;
                    bool login = false;
                    int iduser = 0;
                    ObjectParameter param = new ObjectParameter("valorRetorno", login);
                    ObjectParameter param2 = new ObjectParameter("IdUsuaro", iduser);
                    //Verificar en la BD a traves del procedimiento almacenado para el Login
                    using (rhconEntities db = new rhconEntities())
                    {
                        db.userLogin(usuario, password, idRol, param, param2);
                    }
                    //Si el parametro de retorno es true, paso el login
                    if (Convert.ToBoolean(param.Value))
                    {
                        //Declaracion del objeto tipo usuyario para guardar en la session
                        UserViewModel oUser = null;
                        iduser = Convert.ToInt32(param2.Value);
                        using (rhconEntities db2 = new rhconEntities())
                        {
                            //Agregar a la sesion el objeto Usuario
                            oUser = (from u in db2.usuario
                                     where u.id == iduser
                                     select new UserViewModel
                                     {
                                         Id = u.id,
                                         nombre = u.nombre,
                                         email = u.email,
                                         IdRol = u.idRol

                                     }).FirstOrDefault();
                            if (oUser != null)
                            {
                                Session["user"] = oUser;
                                Session.Timeout = 240;
                                var data = db2.usuario.Where(d => d.id == iduser);
                                if (data.Any())
                                {
                                    var emple = data.First();
                                    emple.verificacion = 1;
                                    db2.Entry(emple).State = System.Data.Entity.EntityState.Modified;
                                    db2.SaveChanges();
                                }
                            }


                        }
                        switch (idRol)
                        {
                            case 1://Super Usuaro
                                return Redirect("~/Super/");
                            case 2://Administrador
                                {
                                    Session["infoDisplay"] = "Administrador(a): " + oUser.nombre;
                                    return Redirect("~/Administrador/");                                   
                                }                               
                            case 3://Empleado
                                {
                                    Session["infoDisplay"] = "Empleado(a): " + oUser.nombre;
                                    int historial = 0;
                                    historial = Convert.ToInt32(Request.Params["historial"]);
                                    if (historial == 1) { return Redirect("~/Cuestionario/Historial"); } else { return Redirect("~/Politica/"); }
                                }
                                
                            case 4://Responsable de Empresa
                                return Redirect("~/Empresa/Perfil");
                            case 5://Responsable del centro de trabajo
                                return Redirect("~/CentroTrabajo/Perfil");
                            case 6://Psicologo
                                return Redirect("~/Picologo/");
                            case 7://Nutriologo
                                return Redirect("~/Nutriologo/");
                            case 8://Legal
                                return Redirect("~/Legal/");
                            case 9://Medico
                                return Redirect("~/Medico/");
                        }
                        return Redirect("~/Empleado/");
                    }
                    else
                    {
                        //Se envia el mensaje de login no valido
                        ViewBag.Message = "Usuario o Contraseña inválidos";
                        return Login();
                    }
                }
                return Login();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Regresa procedimiento para Logout
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            try
            {
                Session["user"] = null;
                Session["empresa"] = null;
                Session["empleado"] = null;
                Session["centro"] = null;
                Session["infoDisplay"] = null;
                return Redirect("~/Home");
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Regresa Vista Para Cambiar Password
        /// </summary>
        /// <returns></returns>
        public ActionResult CambiarPassword()
        {
            List<RolViewModel> list = null;
            using (rhconEntities db = new rhconEntities())
            {
                list =
                     (from d in db.rol
                      select new RolViewModel
                      {
                          Id = d.id,
                          Descripcion = d.nombre
                      }).ToList();

            }
            //Convierte la lista a SelectListItem que es la que utilizael dropdownlist
            List<SelectListItem> items = list.ConvertAll(d =>
            {
                return new SelectListItem()
                {
                    Text = d.Descripcion,
                    Value = d.Id.ToString(),
                    Selected = false
                };
            });

            ViewBag.items = items;
            return View();
        }
        /// <summary>
        /// Postback para Cambiar Password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="IdRol"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CambiarPassword(string email, string password, string IdRol)
        {
            using (rhconEntities db = new rhconEntities())
            {
                //Envio de email al encargado de la empresa
                string EmailORigen = "rhstackcode@gmail.com";
                string EmailDestino = email;
                string pass = "stackcode1.";
                var body = db.correos.Where(d => d.tipo == "cambioPass").First();
                string mensaje = body.email.ToString();
                // byte[] encryted = System.Text.Encoding.Unicode.GetBytes(password);
                //string result = Convert.ToBase64String(encryted);
                mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                mensaje = mensaje.Replace("_redireccion_", "https://localhost:44351/Home/VerificarPassword?IdRol=" + IdRol + "&email=" + email + "&password=" + password);
                MailMessage EmailMess = new MailMessage(
                    EmailORigen,
                    EmailDestino,
                    "Solicitud de Cambio de Contraseña",
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


            return Redirect("~/Home/Mensaje");
        }
        /// <summary>
        /// VErificacion del Password
        /// </summary>
        /// <returns></returns>
        public ActionResult VerificarPassword()
        {

            int idRol = 0;
            string email = "";
            string password = "";

            if (Request.Params["IdRol"] != null)
                if (!Request.Params["IdRol"].Equals(""))
                    idRol = Convert.ToInt32(Request.Params["IdRol"]);

            if (Request.Params["email"] != null)
                if (!Request.Params["email"].Equals(""))
                    email = Request.Params["email"].ToString();

            if (Request.Params["password"] != null)
                if (!Request.Params["password"].Equals(""))
                    password = Request.Params["password"].ToString();


            using (rhconEntities db = new rhconEntities())
            {

                usuario user = db.usuario.Where(d => d.email == email & d.idRol == idRol).First();
                user.password = password;
                db.userUpdate(user.id, user.nombre, user.email, user.password, user.idRol);
                //db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                //db.SaveChanges();
            }
            return Redirect("~/Home/Login");
        }
        /// <summary>
        /// Regresa vista de Mensaje
        /// </summary>
        /// <returns></returns>
        public ActionResult Mensaje()
        {
            return View();
        }
        public ActionResult prueba()
        {
            return View();

        }
        public ActionResult HeaderInforme()
        {

            return View();
        }

        public ActionResult headerInforme025()
        {
            return View();
        }

        public ActionResult HeaderReporte() { 
         return View();
        }   

        public ActionResult HeaderLogo()
        {
            return View();
        }

    }
}