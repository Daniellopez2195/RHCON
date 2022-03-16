using rhcon.Models;
using rhcon.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Data.Entity.Core.Objects;
using rhcon.utils;
using System.Net.Mail;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data.SqlClient;

namespace rhcon.Controllers
{
    /// <summary>
    /// Controlador para Centros de trabajo
    /// </summary>
    [HandleError(View = "Error")]
    public class CentroTrabajoController : Controller
    {
        // declaración de variables para el uso de datatables
        public string draw = "";
        public string start = "";
        public string length = "";
        public string sortColumn = "";
        public string sortColumDir = "";
        public string searchValue = "";
        public int pageSize, skip, recordsTotal;

        /// <summary>
        /// GET: CentroTrabajo
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Pagina En Construccion
        /// </summary>
        /// <returns></returns>
        public ActionResult PaginaEnConstruccion()
        {
            return View();
        }
        /// <summary>
        /// Action Result de JSON para el datatable
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Json()
        {
            var oEmpresa = (EmpresaViewModel)Session["empresa"];

            List<ListCentroTrabajoViewModel> lst = new List<ListCentroTrabajoViewModel>();
            //logistica datatable
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            pageSize = length != null ? Convert.ToInt32(length) : 0;
            skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal = 0;
            //conexion con la base de datos
            using (rhconEntities db = new rhconEntities())
            {
                IQueryable<ListCentroTrabajoViewModel> query =
                   (from e in db.centroTrabajo
                    where e.idEmpresa == oEmpresa.Id
                    join s in db.cstatus on e.idStatus equals s.id
                    select new ListCentroTrabajoViewModel
                    {
                        Id = e.id,
                        Nombre = e.nombre,
                        Estatus = s.nombre
                    });
                //Search
                if (searchValue != "")
                    query = query.Where(d => d.Nombre.Contains(searchValue) || d.Estatus.Contains(searchValue));
                //Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    query = query.OrderBy(sortColumn + " " + sortColumnDir);
                }
                //Total de registros
                recordsTotal = query.Count();
                //Se ejecuta la consulta
                lst = query.Skip(skip).Take(pageSize).ToList();
            }
            return Json(new
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = lst
            });
        }

        /// <summary>
        /// Retorno de vista para nueva empresa, formularo
        /// </summary>
        /// <returns></returns>
        public ActionResult Nuevo()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Metoldo post para agregar datos en la tabla centro de trabajo
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Nuevo(CentroTrabajoViewModel ct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Se genera la instancia para la base de datos
                    rhconEntities db = new rhconEntities();

                    //Se genera un objeto de parámetro para el ID
                    ObjectParameter id = new ObjectParameter("ctID", new int { });

                    //Se genera un objeto para guardar el Rol
                    RolViewModel rol = new RolViewModel();

                    var oEmpresa = (EmpresaViewModel)Session["empresa"];

                    //Se busca el rol en la base de datos
                    rol = (from d in db.rol
                           where d.nombre.Equals("Reponsable Centro de Trabajo")
                           select new RolViewModel
                           {
                               Id = d.id,
                               Descripcion = d.nombre
                           }).FirstOrDefault();
                    // Se inserta la empresa (ya el procedimiento almacenado genera el usuario con el rol asignado de manera automática)
                    //Id Empresa Temporal

                    //Se obtiene la empresa a partir del usuario guardado en la sesion
                    //Obtenemos la sesion de usuario
                    var oUser = (UserViewModel)Session["user"];

                    if (oUser != null)
                    {
                        EncargadoEmpresaViewModel encargado = null;
                        using (rhconEntities bd = new rhconEntities())
                        {
                            encargado = (from d in db.encargadosEmpresa
                                         where d.idUsuario == oUser.Id
                                         select new EncargadoEmpresaViewModel
                                         {
                                             Id = d.id,
                                             IdUsuario = d.idUsuario,
                                             IdEmpresa = d.idEmpresa
                                         }).FirstOrDefault();
                        }

                        ct.IdEmpresa = encargado.IdEmpresa;
                    }
                    //Generar Password
                    string password = utilerias.RandomString(12);
                    db.ctrabajoInsert(ct.Nombre, ct.Direccion, ct.Actividades, ct.IdEmpresa, 1, ct.Encargado, ct.email, password, rol.Id, id);

                    //Envio de email al empleado
                    string EmailORigen = "bienestarlaboral@rhcon.com.mx";
                    string EmailDestino = ct.email;
                    string pass = "Bienestar2022";
                    var body = db.correos.Where(d => d.tipo == "altact").First();
                    string mensaje = body.email.ToString();
                    mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                    mensaje = mensaje.Replace("_razonsocial_", oEmpresa.RazonSocial);
                    mensaje = mensaje.Replace("_centrotrabajo_", ct.Nombre);
                    mensaje = mensaje.Replace("_usuario_", ct.email);
                    mensaje = mensaje.Replace("_pass_", password);
                    mensaje = mensaje.Replace("_redireccion_", "https://bienestarlaboral.rhcon.com.mx/Home/Login?IdRol=" + rol.Id + "&email=" + ct.email + "&password=" + password);
                    mensaje = mensaje.Replace("_tipousuario_", "Usuario de Centro de trabajo");
                    MailMessage EmailMess = new MailMessage(
                        EmailORigen,
                        EmailDestino,
                        "Bienvenido a RHCON",
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

                    // Redirecciona al Index
                    return Redirect("~/CentroTrabajo/Index");
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
        /// Metodo para rellenar los datos del formulario a partir del id del centro de trabajo
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Editar(int Id)
        {
            try
            {
                //Inicializar el modelo
                CentroTrabajoViewModel model = new CentroTrabajoViewModel();
                using (rhconEntities db = new rhconEntities())
                {
                    var oTabla = db.centroTrabajo.Find(Id);
                    var oUsuario = db.usuario.Find(oTabla.idUsuario);
                    model.Id = Id;
                    model.Nombre = oTabla.nombre;
                    model.Direccion = oTabla.direccion;
                    model.Actividades = oTabla.actividades;
                    model.Encargado = oUsuario.nombre;
                    model.email = oUsuario.email;
                    model.IdEmpresa = oTabla.idEmpresa;
                    model.IdStatus = oTabla.idStatus;
                    model.IdUsuario = oTabla.idUsuario;
                }
                return View(model);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Metodo post que obtiene el modelo para Actualizar los datos del Centro de trabajo
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Editar(CentroTrabajoViewModel ct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Se genera un objeto para guardar el Rol
                    RolViewModel rol = new RolViewModel();

                    var oEmpresa = (EmpresaViewModel)Session["empresa"];
                    //Inicializar el movil
                    using (rhconEntities db = new rhconEntities())
                    {
                        //Se busca el rol en la base de datos
                        rol = (from d in db.rol
                               where d.nombre.Equals("Reponsable Centro de Trabajo")
                               select new RolViewModel
                               {
                                   Id = d.id,
                                   Descripcion = d.nombre
                               }).FirstOrDefault();

                        string password = utilerias.RandomString(12);

                        //Actualizar con el SP creado en la base de datos
                        db.ctrabajoUpdate(ct.Id, ct.Nombre, ct.Direccion, ct.Actividades, ct.IdEmpresa, ct.IdStatus, ct.IdUsuario, ct.Encargado, ct.email, password, rol.Id);
                        ObjectParameter id = new ObjectParameter("userID", new int { });

                        var body = new correos();
                        body = db.correos.Where(d => d.tipo == "altact").First();
                        usuario datos = db.usuario.Where(d => d.id == ct.IdUsuario).First();

                        if (!ct.email.Equals(datos.email))
                        {
                            db.userUpdate(ct.IdUsuario, ct.Encargado, ct.email, password, rol.Id);
                            //Envio de email al encargado de la empresa
                            string EmailORigen = "bienestarlaboral@rhcon.com.mx";
                            string EmailDestino = ct.email;
                            string pass = "Bienestar2022";
                            string mensaje = body.email.ToString();
                            mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                            mensaje = mensaje.Replace("_razonsocial_", oEmpresa.RazonSocial);
                            mensaje = mensaje.Replace("_centrotrabajo_", ct.Nombre);
                            mensaje = mensaje.Replace("_usuario_", ct.email);
                            mensaje = mensaje.Replace("_pass_", password);
                            mensaje = mensaje.Replace("_redireccion_", "https://bienestarlaboral.rhcon.com.mx/Home/Login?IdRol=" + rol.Id + "&email=" + ct.email + "&password=" + password);
                            mensaje = mensaje.Replace("_tipousuario_", "Usuario Centro de Trabajo");
                            MailMessage EmailMess = new MailMessage(
                                EmailORigen,
                                EmailDestino,
                                "Bienvenido a RHCON",
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
                            //db.userInsert(model.Responsable,model.Email,password,oUser.IdRol,1,1);
                        }

                    }
                    return Redirect("~/CentroTrabajo/Index");
                }
                else
                    return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Metodo que borra de manera logica el Centro de Trabajo
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Eliminar(int Id)
        {
            try
            {
                using (rhconEntities db = new rhconEntities())
                {
                    db.ctrabajoDelete(Id, 3);
                }
                return Redirect("~/CentroTrabajo/Index");
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        //Seccion del empleado

        /// <summary>
        /// ACtiojn result que retorna la pantalla princiapal del perfil del empleado
        /// </summary>
        /// <returns></returns>
        public ActionResult Perfil()
        {
            try
            {

                CentroTrabajoViewModel centro = null;
                EmpresaViewModel emp = null;
                //Se obtiene la empresa a partir del usuario guardado en la sesion
                //Obtenemos la sesion de usuario
                var oUser = (UserViewModel)Session["user"];

                int idEmpresa = 0;

                if (oUser != null)
                {

                    using (rhconEntities bd = new rhconEntities())
                    {
                        centro = (from d in bd.centroTrabajo
                                  where d.idUsuario == oUser.Id
                                  select new CentroTrabajoViewModel
                                  {
                                      Id = d.id,
                                      IdUsuario = d.idUsuario,
                                      IdEmpresa = d.idEmpresa,
                                      Nombre = d.nombre,
                                      Actividades = d.actividades,
                                      Direccion = d.direccion
                                  }).FirstOrDefault();

                        idEmpresa = centro.IdEmpresa;

                        emp = (from e in bd.empresa
                               where e.id == idEmpresa
                               select new EmpresaViewModel
                               {
                                   Id = e.id,
                                   Responsable = oUser.nombre,
                                   Email = oUser.email,
                                   RazonSocial = e.razons,
                                   strlogotipo = e.img
                               }).FirstOrDefault();

                    }
                    Session["centro"] = centro;
                    Session["infoDisplay"] = "Centro de Trabajo: " + centro.Nombre;
                    Session["empresa"] = emp;
                }
                return View(centro);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        /// <summary>
        /// Json para grid de empleado
        /// </summary>
        /// <returns></returns>
        public ActionResult JsonEmpleado()
        {
            List<ListEmpleadoViewModel> lst = new List<ListEmpleadoViewModel>();
            //logistica datatable
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            var oCentro = (CentroTrabajoViewModel)Session["centro"];

            pageSize = length != null ? Convert.ToInt32(length) : 0;
            skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal = 0;
            //conexion con la base de datos
            using (rhconEntities db = new rhconEntities())
            {
                IQueryable<ListEmpleadoViewModel> query =
                   (from e in db.empleado
                    where e.idEmpresa == oCentro.IdEmpresa && e.idCentroTrabajo == oCentro.Id
                    join s in db.cstatus on e.idEstatus equals s.id
                    select new ListEmpleadoViewModel
                    {
                        Id = e.id,
                        Nombre = e.nombre,
                        CURP = e.curp,
                        Telefono = e.telefono,
                        Estatus = s.nombre,
                        Nss = e.nss,
                        CentroTrabajo = e.centroTrabajo.nombre,
                        AreaFuncion = e.area_funcion
                    });
                //Search
                if (searchValue != "")
                    query = query.Where(d => d.Nombre.Contains(searchValue) || d.CURP.Contains(searchValue) || d.Telefono.Contains(searchValue) || d.Estatus.Contains(searchValue));
                //Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    query = query.OrderBy(sortColumn + " " + sortColumnDir);
                }
                //Total de registros
                recordsTotal = query.Count();
                //Se ejecuta la consulta
                lst = query.Skip(skip).Take(pageSize).ToList();
            }
            return Json(new
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = lst
            });
        }
        /// <summary>
        /// Action Result de Nuevo Empleado
        /// </summary>
        /// <returns></returns>
        public ActionResult NuevoEmpleado()
        {
            return View();
        }
        /// <summary>
        /// Action Resulta para la vista de Empleados (grid)
        /// </summary>
        /// <returns></returns>
        public ActionResult Empleados()
        {
            if (TempData["Message"] != null)
                ViewBag.Message = TempData["Message"].ToString();

            return View();
        }
        /// <summary>
        /// Vista de Nuevo empleado que recibe como parametro un modelo de empleado para guardar en la base de datos
        /// </summary>
        /// <param name="empleado"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NuevoEmpleado(EmpleadoViewModel empleado)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Se genera la instancia para la base de datos
                    rhconEntities db = new rhconEntities();

                    //Se genera un objeto de parámetro para el ID
                    ObjectParameter id = new ObjectParameter("EmplID", new int { });

                    //Se genera un objeto para guardar el Rol
                    RolViewModel rol = new RolViewModel();

                    //Se obtiene la empresa a partir del usuario guardado en la sesion
                    //Obtenemos la sesion de usuario
                    var oUser = (UserViewModel)Session["user"];
                    var oEmp = (EmpresaViewModel)Session["empresa"];
                    var oCentro = (CentroTrabajoViewModel)Session["centro"];

                    int idEmpresa = oEmp.Id;
                    string RazonSocial = oEmp.RazonSocial;


                    int idEstatus = 1;

                    //Generar Password
                    string password = utilerias.RandomString(12);

                    //Se busca el rol en la base de datos
                    rol = (from d in db.rol
                           where d.nombre == "Empleado(a)"
                           select new RolViewModel
                           {
                               Id = d.id,
                               Descripcion = d.nombre
                           }).FirstOrDefault();

                    db.empleadoInsert(empleado.Nombre, empleado.CURP, empleado.Nss, empleado.Telefono, empleado.Contacto, empleado.CelContacto, idEmpresa, oCentro.Id, idEstatus, empleado.Nombre, empleado.Email, password, rol.Id, empleado.AreaFuncion, id);


                    //Envio de email al empleado
                    string EmailORigen = "bienestarlaboral@rhcon.com.mx";
                    string EmailDestino = empleado.Email;
                    string pass = "Bienestar2022";
                    var body = db.correos.Where(d => d.tipo == "altaEmp").First();
                    string mensaje = body.email.ToString();
                    mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                    mensaje = mensaje.Replace("_razonsocial_", RazonSocial);
                    mensaje = mensaje.Replace("_usuario_", empleado.Email);
                    mensaje = mensaje.Replace("_pass_", password);
                    mensaje = mensaje.Replace("_redireccion_", "https://bienestarlaboral.rhcon.com.mx/Home/Login?IdRol=" + rol.Id + "&email=" + empleado.Email + "&password=" + password);
                    mensaje = mensaje.Replace("_tipousuario_", "Usuario Empleado(a)");
                    MailMessage EmailMess = new MailMessage(
                        EmailORigen,
                        EmailDestino,
                        "Bienvenido a RHCON",
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

                    return Redirect("~/CentroTrabajo/Empleados/");

                }
                else
                    return View();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Metodo para rellenar el formulario de edicion de empleados
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult EditarEmpleado(int Id)
        {
            try
            {
                //Se obtiene la empresa a partir del usuario guardado en la sesion
                //Obtenemos la sesion de usuario
                var oUser = (UserViewModel)Session["user"];
                var oCentro = (CentroTrabajoViewModel)Session["centro"];

                int idEmpresa = oCentro.IdEmpresa;

                EmpleadoViewModel model = new EmpleadoViewModel();

                using (rhconEntities db = new rhconEntities())
                {
                    model = (from e in db.empleado
                             join s in db.usuario on e.idUsuario equals s.id
                             where e.id == Id
                             select new EmpleadoViewModel
                             {
                                 Id = e.id,
                                 Nombre = e.nombre,
                                 CURP = e.curp,
                                 Nss = e.nss,
                                 Email = s.email,
                                 Telefono = e.telefono,
                                 Contacto = e.contacto,
                                 CelContacto = e.celcontacto,
                                 IdCentro = e.idCentroTrabajo,
                                 AreaFuncion = e.area_funcion
                             }).FirstOrDefault();

                }

                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Metodo post para registrar cambios en el empleado
        /// </summary>
        /// <param name="empleado"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditarEmpleado(EmpleadoViewModel empleado)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var oCentro = (CentroTrabajoViewModel)Session["centro"];
                    var oEmpresa = (EmpresaViewModel)Session["empresa"];
                    // Initialize the model
                    using (rhconEntities db = new rhconEntities())
                    {
                        empleado register = db.empleado.Where(d => d.id == empleado.Id).First();
                        register.nombre = empleado.Nombre;
                        register.curp = empleado.CURP;
                        register.nss = empleado.Nss;
                        register.telefono = empleado.Telefono;
                        register.contacto = empleado.Contacto;
                        register.celcontacto = empleado.CelContacto;
                        register.idCentroTrabajo = oCentro.Id;
                        register.area_funcion = empleado.AreaFuncion;

                        db.Entry(register).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        var body = new correos();
                        body = db.correos.Where(d => d.tipo == "altaEmp").First();
                        usuario datos = db.usuario.Where(d => d.id == register.idUsuario).First();

                        string password = utilerias.RandomString(12);

                        RolViewModel rol = new RolViewModel();

                        //Se busca el rol en la base de datos
                        rol = (from d in db.rol
                               where d.nombre.Equals("Empleado(a)")
                               select new RolViewModel
                               {
                                   Id = d.id,
                                   Descripcion = d.nombre
                               }).FirstOrDefault();

                        if (!empleado.Email.Equals(datos.email))
                        {
                            db.userUpdate(register.idUsuario, empleado.Nombre, empleado.Email, password, rol.Id);
                            //Envio de email al encargado de la empresa
                            string EmailORigen = "bienestarlaboral@rhcon.com.mx";
                            string EmailDestino = empleado.Email;
                            string pass = "Bienestar2022";
                            string mensaje = body.email.ToString();
                            mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                            mensaje = mensaje.Replace("_razonsocial_", oEmpresa.RazonSocial);
                            mensaje = mensaje.Replace("_usuario_", empleado.Email);
                            mensaje = mensaje.Replace("_pass_", password);
                            mensaje = mensaje.Replace("_redireccion_", "https://bienestarlaboral.rhcon.com.mx/Home/Login?IdRol=" + rol.Id + "&email=" + empleado.Email + "&password=" + password);
                            mensaje = mensaje.Replace("_tipousuario_", "Usuario Empleado(a)");
                            MailMessage EmailMess = new MailMessage(
                                EmailORigen,
                                EmailDestino,
                                "Bienvenido a RHCON",
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
                            //db.userInsert(model.Responsable,model.Email,password,oUser.IdRol,1,1);
                        }
                    }
                    return Redirect("~/CentroTrabajo/Empleados");
                }
                else return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// M[etodo para borrado logico del empleado
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EliminarEmpleado(int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (rhconEntities db = new rhconEntities())
                    {
                        var oTabla = db.empleado.Find(Id);
                        oTabla.idEstatus = 3;
                        db.Entry(oTabla).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                    }
                    return Redirect("~/CentroTrabajo/Empleados");
                }
                else return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Exportar a excel la lista de empresas
        /// </summary>
        public void ExportExcel()
        {
            try
            {
                var oEmpresa = (EmpresaViewModel)Session["empresa"];
                List<CentroTrabajoViewModel> centrolist = null;

                using (rhconEntities db = new rhconEntities())
                {
                    centrolist = (from d in db.centroTrabajo
                                  join u in db.usuario on d.idUsuario equals u.id
                                  where d.idEmpresa == oEmpresa.Id
                                  select new CentroTrabajoViewModel
                                  {
                                      Id = d.id,
                                      Nombre = d.nombre,
                                      Direccion = d.direccion,
                                      Actividades = d.actividades,
                                      Encargado = u.nombre,
                                      email = u.email
                                  }
                                  ).ToList();
                }

                // If you use EPPlus in a noncommercial context
                // according to the Polyform Noncommercial license:
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("CentroTrabajo_export");
                //Titulo
                ws.Cells["A1:E1"].Merge = true;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A1"].Value = "Lista de Centros de Trabajo";
                //Encabezados de columnas
                ws.Cells["A2"].Value = "Nombre";
                ws.Cells["B2"].Value = "Dirección";
                ws.Cells["C2"].Value = "Actividades";
                ws.Cells["D2"].Value = "Responsable";
                ws.Cells["E2"].Value = "Email Responsable";
                //Contenido
                int rowStart = 3;
                foreach (var item in centrolist)
                {

                    ws.Cells[string.Format("A{0}", rowStart)].Value = item.Nombre;
                    ws.Cells[string.Format("B{0}", rowStart)].Value = item.Direccion;
                    ws.Cells[string.Format("C{0}", rowStart)].Value = item.Actividades;
                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.Encargado;
                    ws.Cells[string.Format("E{0}", rowStart)].Value = item.email;

                    rowStart++;
                }

                ws.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=\"CentroTrabajo_export.xlsx\"");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
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
        /// Exportar a excel la lista de empleados
        /// </summary>
        public void ExportExcelEmpleado()
        {
            try
            {
                var oEmpresa = (EmpresaViewModel)Session["empresa"];
                var oCt = (CentroTrabajoViewModel)Session["centro"];
                List<EmpleadoViewModel> empleadolist = null;

                using (rhconEntities db = new rhconEntities())
                {
                    empleadolist = (from d in db.empleado
                                    join u in db.usuario on d.idUsuario equals u.id
                                    where d.idEmpresa == oEmpresa.Id && d.idCentroTrabajo == oCt.Id
                                    select new EmpleadoViewModel
                                    {
                                        Id = d.id,
                                        Nss = d.nss,
                                        Nombre = d.nombre,
                                        CURP = d.curp,
                                        Telefono = d.telefono,
                                        Contacto = d.contacto,
                                        CelContacto = d.celcontacto,
                                        Email = u.email,
                                        CentroTrabajo = d.centroTrabajo.nombre,
                                        AreaFuncion = d.area_funcion,
                                        estatus = d.cstatus.nombre
                                    }
                                  ).ToList();
                }

                // If you use EPPlus in a noncommercial context
                // according to the Polyform Noncommercial license:
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Empleados_export");
                //Titulo
                ws.Cells["A1:J1"].Merge = true;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A1"].Value = "Lista de empleados de la empresa: " + oEmpresa.RazonSocial + ", centro de trabajo: " + oCt.Nombre;
                //Encabezados de columnas
                ws.Cells["A2"].Value = "Nss";
                ws.Cells["B2"].Value = "Nombre";
                ws.Cells["C2"].Value = "CURP";
                ws.Cells["D2"].Value = "Teléfono";
                ws.Cells["E2"].Value = "Contacto";
                ws.Cells["F2"].Value = "Cel. Contacto";
                ws.Cells["G2"].Value = "Email de Empleado";
                ws.Cells["H2"].Value = "Centro de Trabajo";
                ws.Cells["I2"].Value = "Area/Funcion";
                ws.Cells["J2"].Value = "Estatus";

                //Contenido
                int rowStart = 3;
                foreach (var item in empleadolist)
                {

                    ws.Cells[string.Format("A{0}", rowStart)].Value = item.Nss;
                    ws.Cells[string.Format("B{0}", rowStart)].Value = item.Nombre;
                    ws.Cells[string.Format("C{0}", rowStart)].Value = item.CURP;
                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.Telefono;
                    ws.Cells[string.Format("E{0}", rowStart)].Value = item.Contacto;
                    ws.Cells[string.Format("F{0}", rowStart)].Value = item.CelContacto;
                    ws.Cells[string.Format("G{0}", rowStart)].Value = item.Email;
                    ws.Cells[string.Format("H{0}", rowStart)].Value = item.CentroTrabajo;
                    ws.Cells[string.Format("I{0}", rowStart)].Value = item.AreaFuncion;
                    ws.Cells[string.Format("J{0}", rowStart)].Value = item.estatus;

                    rowStart++;
                }

                ws.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=\"Empleados_export.xlsx\"");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    throw new Exception(ex.Message);
                else
                    throw new Exception(ex.Message + "\n" + ex.InnerException.Message);
            }
        }

        public ActionResult Resultados()
        {


            var oCentro = (CentroTrabajoViewModel)Session["centro"];

            using (rhconEntities db = new rhconEntities())
            {
                var centro = db.centroTrabajo.Where(d => d.id == oCentro.Id).First();
                ViewBag.idCentro = centro.id;

                ReporteViewModel reporte = new ReporteViewModel();
                var years = db.periodosEncuesta.Where(d => d.idEmpresa == oCentro.IdEmpresa).OrderBy(d => d.year).ToList();
                var fecha = db.periodosEncuesta.Where(d => d.idEmpresa == oCentro.IdEmpresa & d.cierre == 1).OrderByDescending(d => d.id).First();
                string list = "";
                foreach (var year in years)
                {
                    list += year.year.ToString() + " ";

                }
                reporte.years = list;
                reporte.idEmpresa = oCentro.IdEmpresa;
                reporte.idCentro = oCentro.Id;



                SqlConnectionStringBuilder conect = ConexionViewModel.conectar();

                using (SqlConnection connection = new SqlConnection(conect.ConnectionString))
                {
                    connection.Open();



                    // consulta a la vista result_categoria
                    var cadena = " AND t1.idCentroTrabajo =" + oCentro.Id;
                    string consulta_categorias = ConsultasViewModel.Categorias(fecha.year.ToString(), oCentro.IdEmpresa.ToString(), cadena);
                    SqlCommand command_categoria = new SqlCommand(consulta_categorias, connection);
                    // contruccion de los elementos de las categorias 
                    DataResultadosViewModel dt = new DataResultadosViewModel();
                    var empleados = 0;
                    string consulta_totalEncuesta = ConsultasViewModel.TotalRespuestasEmpleados(fecha.year.ToString(), oCentro.IdEmpresa.ToString(), cadena);
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
    }
}