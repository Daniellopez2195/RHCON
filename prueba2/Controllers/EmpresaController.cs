using rhcon.Models;
using rhcon.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Data.Entity.Core.Objects;
using System.Net.Mail;
using rhcon.utils;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data.SqlClient;

namespace rhcon.Controllers
{
    /// <summary>
    /// Controlador para modulo "Empresa"
    /// </summary>
    [HandleError(View = "Error")]
    public class EmpresaController : Controller
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
        /// GET: Empresa
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PaginaConstruccion()
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
            List<ListEmpresaViewModel> lst = new List<ListEmpresaViewModel>();
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
                IQueryable<ListEmpresaViewModel> query =
                   (from e in db.empresa
                    join s in db.cstatus on e.idStatus equals s.id
                    select new ListEmpresaViewModel
                    {
                        Id = e.id,
                        RazonComercial = e.razonc,
                        RazonSocial = e.razons,
                        RFC = e.rfc,
                        Telefono = e.telefono,
                        Estatus = s.nombre
                    });
                //Search
                if (searchValue != "")
                    query = query.Where(d => d.RazonComercial.Contains(searchValue) || d.RazonSocial.Contains(searchValue) || d.Telefono.Contains(searchValue) || d.RFC.Contains(searchValue) || d.Estatus.Contains(searchValue));
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
                if (ex.InnerException == null)
                    throw new Exception(ex.Message);
                else
                    throw new Exception(ex.Message + "\n" + ex.InnerException.Message);
            }
        }
        /// <summary>
        /// Retorno de la Vista de Perfil para Encargado de la Empresa
        /// </summary>
        /// <returns></returns>
        public ActionResult Perfil()
        {
            try
            {
                if (TempData["Message"] != null)
                    ViewBag.Message = TempData["Message"].ToString();

                EmpresaViewModel model = null;
                //Se obtiene la empresa a partir del usuario guardado en la sesion
                //Obtenemos la sesion de usuario
                var oUser = (UserViewModel)Session["user"];

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

                        idEmpresa = encargado.IdEmpresa;

                        model = (from e in bd.empresa
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
                    Session["empresa"] = model;
                    Session["infoDisplay"] = "Empresa: " + model.RazonSocial;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    throw new Exception(ex.Message);
                else
                    throw new Exception(ex.Message + "\n" + ex.InnerException.Message);
            }

        }
        /// <summary>s
        /// Metodo para nueva empresa que recibe como parámetro el modelo y preocesa la información
        /// </summary>
        /// <param name="empresa"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Nuevo(EmpresaViewModel empresa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Se genera la instancia para la base de datos
                    rhconEntities db = new rhconEntities();

                    //Se genera un objeto de parámetro para el ID
                    ObjectParameter id = new ObjectParameter("EmpID", new int { });

                    //Se genera un objeto para guardar el Rol
                    RolViewModel rol = new RolViewModel();

                    //Se busca el rol en la base de datos
                    rol = (from d in db.rol
                           where d.nombre == "Responsable Empresa"
                           select new RolViewModel
                           {
                               Id = d.id,
                               Descripcion = d.nombre
                           }).FirstOrDefault();

                    //Generar Password
                    string password = utilerias.RandomString(12);

                    //Tratamiento para guardar logo
                    string RutaSitio = Server.MapPath("~/");
                    string pathLogo = System.IO.Path.Combine(RutaSitio + "/Assets/logos/logo-" + empresa.RFC + ".png");

                    empresa.logotipo.SaveAs(pathLogo);

                    //Tratamientto para guardar contrato
                    string RutaContrato = Server.MapPath("~/");
                    string pathContrato = System.IO.Path.Combine(RutaSitio + "/Assets/contratos/contrato-" + empresa.RFC + ".pdf");

                    empresa.contrato.SaveAs(pathContrato);

                    // Se inserta la empresa (ya el procedimiento almacenado genera el usuario con el rol asignado de manera automática)
                    db.empresaInsert(empresa.RazonComercial, empresa.RazonSocial, empresa.Email, empresa.Telefono, empresa.Direccion, empresa.Actividad, empresa.Puesto, empresa.RFC, "logo-" + empresa.RFC + ".png", 1, empresa.Responsable, empresa.Email, password, rol.Id, "contrato-" + empresa.RFC + ".pdf", empresa.nombrejerarquia, empresa.puestojerarquia, id);

                    //Envio de email al encargado de la empresa
                    string EmailORigen = "rhstackcode@gmail.com";
                    string EmailDestino = empresa.Email;
                    string pass = "stackcode1.";
                    var body = db.correos.Where(d => d.tipo == "altaempresa").First();
                    string mensaje = body.email.ToString();
                    mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                    mensaje = mensaje.Replace("_razonsocial_", empresa.RazonSocial);
                    mensaje = mensaje.Replace("_usuario_", empresa.Email);
                    mensaje = mensaje.Replace("_pass_", password);
                    mensaje = mensaje.Replace("_redireccion_", "https://bienestarlaboral.rhcon.com.mx/Home/Login?IdRol=" + rol.Id + "&email=" + empresa.Email + "&password=" + password);
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
                    oSmtpClient.UseDefaultCredentials = false;
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
                        return Redirect("~/Empresa/Perfil");
                        throw;
                    }

                    // Redirecciona al Index
                    return Redirect("~/Empresa/Index");
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
        /// Metodo para la edicion
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Editar(int Id)
        {
            // Initialize the model
            EmpresaEditViewModel model = new EmpresaEditViewModel();
            using (rhconEntities db = new rhconEntities())
            {
                var oTabla = db.empresa.Find(Id);
                model.Id = Id;
                model.RazonSocial = oTabla.razons;
                model.RazonComercial = oTabla.razonc;
                model.Email = oTabla.email;
                model.Telefono = oTabla.telefono;
                model.Direccion = oTabla.direccion;
                model.Actividad = oTabla.actividad;
                model.Puesto = oTabla.puesto;
                model.RFC = oTabla.rfc;
                model.strlogotipo = oTabla.img;

                var oUser = (UserViewModel)Session["user"];

                if (oUser != null)
                {
                    EncargadoEmpresaViewModel encargado = null;
                    encargado = (from d in db.encargadosEmpresa
                                 where d.idEmpresa == model.Id
                                 select new EncargadoEmpresaViewModel
                                 {
                                     Id = d.id,
                                     IdUsuario = d.idUsuario,
                                     IdEmpresa = d.idEmpresa
                                 }).FirstOrDefault();
                    UserViewModel responsable = (from r in db.usuario
                                                 where r.id == encargado.IdUsuario
                                                 select new UserViewModel
                                                 {
                                                     Id = r.id,
                                                     nombre = r.nombre,
                                                     email = r.email
                                                 }).FirstOrDefault();

                    model.Responsable = responsable.nombre;
                }

                string RutaSitio = Server.MapPath("~/");
                string pathLogo = System.IO.Path.Combine(RutaSitio + "/Assets/logos/logo-" + model.RFC + ".png");

                FileStream logo = new FileStream(pathLogo, FileMode.Open);
                MemoryFile fileLogo = new MemoryFile(logo, "image/png", "logo.png");

                model.logotipo = fileLogo;

                logo.Dispose();
                logo.Close();
            }
            return View(model);
        }
        /// <summary>
        /// Metodo para la edicion con modelo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Editar(EmpresaEditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var empresa = (EmpresaViewModel)Session["empresa"];
                    var user = new usuario();
                    var body = new correos();
                    // Initialize the model
                    using (rhconEntities db = new rhconEntities())
                    {
                        //Tratamiento para guardar logo
                        string RutaSitio = Server.MapPath("~/");
                        string pathLogo = System.IO.Path.Combine(RutaSitio + "/Assets/logos/logo-" + model.RFC + ".png");
                        string pathContrato = System.IO.Path.Combine(RutaSitio + "/Assets/contratos/contrato-" + model.RFC + ".pdf");

                        if (model.logotipo != null)
                            model.logotipo.SaveAs(pathLogo);
                        if (model.contrato != null)
                            model.contrato.SaveAs(pathContrato);
                        //Actualizar con el SP creado en la base de datos
                        //db.empresaUpdate(model.Id,model.RazonComercial, model.RazonSocial, model.Email, model.Telefono, model.Direccion, model.Actividad, model.Puesto, model.RFC, "logo-" + model.RFC + ".png",null,model.Responsable,model.Email,"",null,null);
                        body = db.correos.Where(d => d.tipo == "altaempresa").First();
                        empresa datos = db.empresa.Where(d => d.id == model.Id).First();
                        string password = utilerias.RandomString(12);

                        if (!model.Email.Equals(datos.email))
                        {
                            RolViewModel rol = null;
                            //Se busca el rol en la base de datos
                            rol = (from d in db.rol
                                   where d.nombre == "Responsable Empresa"
                                   select new RolViewModel
                                   {
                                       Id = d.id,
                                       Descripcion = d.nombre
                                   }).FirstOrDefault();
                            //Envio de email al encargado de la empresa
                            string EmailORigen = "rhstackcode@gmail.com";
                            string EmailDestino = model.Email;
                            string pass = "stackcode1.";
                            string mensaje = body.email.ToString();
                            mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                            mensaje = mensaje.Replace("_razonsocial_", model.RazonSocial);
                            mensaje = mensaje.Replace("_usuario_", model.Email);
                            mensaje = mensaje.Replace("_pass_", password);
                            mensaje = mensaje.Replace("_redireccion_", "https://bienestarlaboral.rhcon.com.mx/Home/Login?IdRol=" + rol.Id + "&email=" + model.Email + "&password=" + password);
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
                            oSmtpClient.UseDefaultCredentials = false;
                            oSmtpClient.Host = "smtp.gmail.com";
                            oSmtpClient.Port = 587;
                            oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailORigen, pass);
                            oSmtpClient.Send(EmailMess);
                            oSmtpClient.Dispose();
                            //db.userInsert(model.Responsable,model.Email,password,oUser.IdRol,1,1);
                        }

                        //Actualizar con el SP creado en la base de datos
                        db.empresaUpdate(model.Id, model.RazonComercial, model.RazonSocial, model.Email, model.Telefono, model.Direccion, model.Actividad, model.Puesto, model.RFC, "logo-" + model.RFC + ".png", null, model.Responsable, model.Email, "", null, null);
                        ObjectParameter id = new ObjectParameter("userID", new int { });
                        db.userInsert(model.Responsable, model.Email, password, 4, 1, id);

                        encargadosEmpresa encargado = new encargadosEmpresa();
                        encargado.idUsuario = (int)id.Value;
                        encargado.idEmpresa = model.Id;
                        db.encargadosEmpresa.Add(encargado);
                        db.SaveChanges();

                    }

                    return Redirect("~/Empresa/Index");
                }
                else return View();
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
        /// Eliminar Empresa
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
                    db.empresaDelete(Id, 3);
                }
                return Redirect("~/Empresa/Index");
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                    throw new Exception(ex.Message);
                else
                    throw new Exception(ex.Message + "\n" + ex.InnerException.Message);
            }
        }
        //Empleados
        /// <summary>
        /// Regresa la vista de empleados
        /// </summary>
        /// <returns></returns>
        public ActionResult Empleados()
        {
            if (TempData["Message"] != null)
                ViewBag.Message = TempData["Message"].ToString();

            return View();
        }
        /// <summary>
        /// Regresa objeto JSON para el llenado asincrono del grid para empleados
        /// </summary>
        /// <returns></returns>
        [HttpPost]
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

            var oEmpresa = (EmpresaViewModel)Session["empresa"];

            pageSize = length != null ? Convert.ToInt32(length) : 0;
            skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal = 0;
            //conexion con la base de datos
            using (rhconEntities db = new rhconEntities())
            {
                IQueryable<ListEmpleadoViewModel> query =
                   (from e in db.empleado
                    where e.idEmpresa == oEmpresa.Id
                    select new ListEmpleadoViewModel
                    {
                        Id = e.id,
                        Nombre = e.nombre,
                        CURP = e.curp,
                        Telefono = e.telefono,
                        Estatus = e.cstatus.nombre,
                        Nss = e.nss,
                        CentroTrabajo = e.centroTrabajo.nombre,
                        AreaFuncion = e.area_funcion,
                        email = e.usuario.email
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
        /// Retorno de vista para nueva empleado, formularo
        /// </summary>
        /// <returns></returns>
        public ActionResult NuevoEmpleado()
        {
            try
            {

                List<ListCentroTrabajoViewModel> lst;
                //Se obtiene la empresa a partir del usuario guardado en la sesion
                //Obtenemos la sesion de usuario
                var oUser = (UserViewModel)Session["user"];

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
                    }

                    idEmpresa = encargado.IdEmpresa;
                }
                using (rhconEntities db = new rhconEntities())
                {
                    lst = (from d in db.centroTrabajo
                           where d.idEmpresa == idEmpresa
                           select new ListCentroTrabajoViewModel
                           {
                               Id = d.id,
                               Nombre = d.nombre,
                           }).ToList();
                }
                //EmpleadoViewModel empleado = new EmpleadoViewModel();
                //empleado.Centros = lst;
                List<SelectListItem> centros = lst.ConvertAll(d =>
                {
                    return new SelectListItem()
                    {
                        Text = d.Nombre.ToString(),
                        Value = d.Id.ToString(),
                        Selected = false
                    };
                });



                EmpleadoViewModel model = new EmpleadoViewModel();
                model.Centros = centros;
                return View(model);
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
        /// metodo post para registrar nuevo usuario
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

                    int idEmpresa = 0;
                    string RazonSocial = "";

                    if (oUser != null)
                    {
                        EncargadoEmpresaViewModel encargado = null;
                        empresa emp = null;
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

                            emp = bd.empresa.Find(encargado.IdEmpresa);
                        }

                        idEmpresa = encargado.IdEmpresa;
                        RazonSocial = emp.razons;

                    }

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

                    db.empleadoInsert(empleado.Nombre, empleado.CURP, empleado.Nss, empleado.Telefono, empleado.Contacto, empleado.CelContacto, idEmpresa, empleado.IdCentro, idEstatus, empleado.Nombre, empleado.Email, password, rol.Id, empleado.AreaFuncion, id);


                    //Envio de email al empleado
                    string EmailORigen = "rhstackcode@gmail.com";
                    string EmailDestino = empleado.Email;
                    string pass = "stackcode1.";
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

                    return Redirect("~/Empresa/Empleados/");

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
        /// Metodo para rellenar el formulario de edicion de empleados
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult EditarEmpleado(int Id)
        {
            try
            {

                List<ListCentroTrabajoViewModel> lst;
                //Se obtiene la empresa a partir del usuario guardado en la sesion
                //Obtenemos la sesion de usuario
                var oUser = (UserViewModel)Session["user"];

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
                    }

                    idEmpresa = encargado.IdEmpresa;
                }
                using (rhconEntities db = new rhconEntities())
                {
                    lst = (from d in db.centroTrabajo
                           where d.idEmpresa == idEmpresa
                           select new ListCentroTrabajoViewModel
                           {
                               Id = d.id,
                               Nombre = d.nombre,
                           }).ToList();
                }
                //EmpleadoViewModel empleado = new EmpleadoViewModel();
                //empleado.Centros = lst;
                List<SelectListItem> centros = lst.ConvertAll(d =>
                {
                    return new SelectListItem()
                    {
                        Text = d.Nombre.ToString(),
                        Value = d.Id.ToString(),
                        Selected = false
                    };
                });



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
                                 Password = s.password,
                                 AreaFuncion = e.area_funcion


                             }).FirstOrDefault();

                }
                model.Centros = centros;
                return View(model);
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
                        register.idCentroTrabajo = empleado.IdCentro;
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
                            string EmailORigen = "rhstackcode@gmail.com";
                            string EmailDestino = empleado.Email;
                            string pass = "stackcode1.";
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

                    return Redirect("~/Empresa/Empleados");
                }
                else return View();
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
                    return Redirect("~/Empresa/Empleados");
                }
                else return View();
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
        /// Exportar a excel la lista
        /// </summary>
        public void ExportExcel()
        {
            try
            {
                var oUser = (UserViewModel)Session["user"];

                List<EmpresaViewModel> empresalist = null;

                using (rhconEntities db = new rhconEntities())
                {
                    empresalist = db.empresa.Select(x => new EmpresaViewModel
                    {
                        Id = x.id,
                        RFC = x.rfc,
                        RazonComercial = x.razonc,
                        RazonSocial = x.razons,
                        Email = x.email,
                        Telefono = x.telefono,
                        Direccion = x.direccion,
                        Actividad = x.actividad,
                        Responsable = (from d in db.usuario
                                       where d.id == (from e in db.encargadosEmpresa
                                                      where e.idEmpresa == x.id
                                                      select e.idUsuario).FirstOrDefault()
                                       select d.nombre).FirstOrDefault(),
                        estatus = x.cstatus.nombre

                    }).ToList();
                }


                // If you use EPPlus in a noncommercial context
                // according to the Polyform Noncommercial license:
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Empresa_export");
                //Titulo
                ws.Cells["A1:I1"].Merge = true;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A1"].Value = "Lista de Empresas";
                //Encabezados de columnas
                ws.Cells["A2"].Value = "RFC de la Empresa";
                ws.Cells["B2"].Value = "Razón Comercial de la Empresa";
                ws.Cells["C2"].Value = "Razón Social de la Empresa";
                ws.Cells["D2"].Value = "Nombre Completo del Responsable";
                ws.Cells["E2"].Value = "Correo Electrónico del Responsable";
                ws.Cells["F2"].Value = "Teléfono del Responsable";
                ws.Cells["G2"].Value = "Dirección de la Empresa";
                ws.Cells["H2"].Value = "Actividad de la Empresa";
                ws.Cells["I2"].Value = "Estatus de la Empresa";
                //Contenido
                int rowStart = 3;
                foreach (var item in empresalist)
                {

                    ws.Cells[string.Format("A{0}", rowStart)].Value = item.RFC;
                    ws.Cells[string.Format("B{0}", rowStart)].Value = item.RazonComercial;
                    ws.Cells[string.Format("C{0}", rowStart)].Value = item.RazonSocial;
                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.Responsable;
                    ws.Cells[string.Format("E{0}", rowStart)].Value = item.Email;
                    ws.Cells[string.Format("F{0}", rowStart)].Value = item.Telefono;
                    ws.Cells[string.Format("G{0}", rowStart)].Value = item.Direccion;
                    ws.Cells[string.Format("H{0}", rowStart)].Value = item.Actividad;
                    ws.Cells[string.Format("I{0}", rowStart)].Value = item.estatus;

                    rowStart++;
                }

                ws.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=\"Empresa_export.xlsx\"");
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
        /// Exportar lista de empleados a excel
        /// </summary>
        public void ExportExcelEmpleado(int tipo = 0)
        {
            try
            {
                var oEmpresa = (EmpresaViewModel)Session["empresa"];
                List<EmpleadoViewModel> empleadolist = null;

                using (rhconEntities db = new rhconEntities())
                {
                    if (tipo == 0)
                    {
                        empleadolist = (from d in db.empleado
                                        join u in db.usuario on d.idUsuario equals u.id
                                        where d.idEmpresa == oEmpresa.Id
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
                                            estatus = d.cstatus.nombre,
                                            verificacion = u.verificacion.ToString()
                                        }
                                    ).ToList();
                    }



                }

                // If you use EPPlus in a noncommercial context
                // according to the Polyform Noncommercial license:
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Empleados_export");
                //Titulo
                ws.Cells["A1:I1"].Merge = true;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A1"].Value = "Lista de empleados de la empresa: " + oEmpresa.RazonSocial;
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
                ws.Cells["k2"].Value = "Id";
                ws.Cells["L2"].Value = "Verificación";

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
                    ws.Cells[string.Format("k{0}", rowStart)].Value = item.Id;
                    if (item.verificacion.Equals("1"))
                    {
                        ws.Cells[string.Format("L{0}", rowStart)].Value = "Usuario Verificado";
                    }
                    else
                    {
                        ws.Cells[string.Format("L{0}", rowStart)].Value = "Usuario no Verificado";
                    }

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

        public ActionResult ReporteResultados()
        {
            var oEmpresa = (EmpresaViewModel)Session["empresa"];

            using (rhconEntities db = new rhconEntities())
            {
                List<periodosEncuesta> años = db.periodosEncuesta.Where(d => d.idEmpresa == oEmpresa.Id & d.cierre == 1).OrderByDescending(d => d.id).ToList();


                return View(años);
            }


        }


        public ActionResult Resultados()
        {
            var oEmpresa = (EmpresaViewModel)Session["empresa"];
            ViewBag.Empresa = oEmpresa.RazonSocial;
            ViewBag.idEmpresa = oEmpresa.Id;
            using (rhconEntities db = new rhconEntities())
            {

                List<centroTrabajo> centros = db.centroTrabajo.Where(d => d.idEmpresa == oEmpresa.Id).ToList();


                return View(centros);
            }

        }

        public ActionResult Acciones()
        {



            var oEmpresa = (EmpresaViewModel)Session["empresa"];
            // filtros generales
            var year = "2021";
            int empresa = oEmpresa.Id;
            //conexion a la bd
            SqlConnectionStringBuilder conect = ConexionViewModel.conectar();
            int empleados = 6;
            DataResultadosViewModel dt = new DataResultadosViewModel();



            // consulta de datos generales 

            rhconEntities db = new rhconEntities();
            var periodo = db.periodosEncuesta.Where(d => d.idEmpresa == empresa & d.year.ToString() == year.ToString() & d.cierre == 1);

            bool ternario = false;
            var datosEmpresa = db.empresa.Where(d => d.id == empresa).First();
            var idResponsable = db.encargadosEmpresa.Where(d => d.idEmpresa == oEmpresa.Id).First();
            var responsableNombre = db.usuario.Where(d => d.id == idResponsable.id).First();



            var responsable =
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

            dt.responsable = responsable.nombre;
            dt.cedula = responsable.cedula;
            var datosCentro = new centroTrabajo();


            dt.total_empleados = db.empleado.Where(d => d.idEmpresa == empresa).Count();
            dt.ternario = false;
            dt.actividades = datosEmpresa.actividad.ToString();
            dt.nombreSelect = datosEmpresa.razonc;







            using (SqlConnection connection = new SqlConnection(conect.ConnectionString))
            {
                connection.Open();


                string consulta_totalEncuesta = ConsultasViewModel.TotalRespuestasEmpleados(year, empresa.ToString(), "");
                SqlCommand command_totalEncuesta = new SqlCommand(consulta_totalEncuesta, connection);
                using (SqlDataReader reader = command_totalEncuesta.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        dt.total_encuesta = int.Parse(reader[0].ToString());
                    }
                }




                if (dt.total_encuesta > 0)
                {
                    // consulta a la vista result_categoria
                    string consulta_categorias = ConsultasViewModel.Categorias(year, empresa.ToString(), "");
                    SqlCommand command_categoria = new SqlCommand(consulta_categorias, connection);

                    // consulta a la vista result_dominio
                    string consulta_dominio = ConsultasViewModel.Dominios(year, empresa.ToString(), "");
                    SqlCommand command_dominio = new SqlCommand(consulta_dominio, connection);

                    // consulta a la vista result_dimension
                    string consulta_dimension = ConsultasViewModel.Dimensiones(year, empresa.ToString(), "");
                    SqlCommand command_dimension = new SqlCommand(consulta_dimension, connection);

                    // consulta a la vista result_nom035
                    string consulta_total = ConsultasViewModel.TotalResultado(year, empresa.ToString(), "");
                    SqlCommand command_total = new SqlCommand(consulta_total, connection);



                    // contruccion de los elementos de las categorias 
                    using (SqlDataReader reader = command_categoria.ExecuteReader())
                    {
                        /*
                          devuelve un objeto de tipo CategoriaViewModel 
                           cada categoria contine las propiedades color,text,estado
                         */
                        dt.categoriasVal = dt.categorias(reader, dt.total_encuesta);
                    }
                    // Contruccion de los elementos de los dominios
                    using (SqlDataReader reader = command_dominio.ExecuteReader())
                    {
                        /*
                           devuelve un objeto de tipo dominioViewModel 
                           cada dominio contine las propiedades color,text,estado
                        */
                        dt.dominiosVal = dt.dominios(reader, dt.total_encuesta);
                    }
                    // Contruccion de los elementos de los dominios
                    using (SqlDataReader reader = command_dimension.ExecuteReader())
                    {
                        /*
                           devuelve un objeto de tipo dominioViewModel 
                           cada dominio contine las propiedades color,text,estado
                        */
                        dt.dimensionesVal = dt.Dimensiones(reader, dt.total_encuesta);
                    }
                    // Contruccion de los elementos del total
                    using (SqlDataReader reader = command_total.ExecuteReader())
                    {

                        dt.total = dt.Total035(reader, dt.total_encuesta);

                        if (dt.total.estado.Equals("Muy alto"))
                        {
                            dt.estadoFavorable = "MUY DESFAVORABLE";
                        }
                        else if (dt.total.estado.Equals("Alto"))
                        {
                            dt.estadoFavorable = "POCO FAVORABLE";
                        }
                        else if (dt.total.estado.Equals("Medio"))
                        {
                            dt.estadoFavorable = " MODERADO";
                        }
                        else if (dt.total.estado.Equals("Bajo"))
                        {
                            dt.estadoFavorable = "FAVORABLE";
                        }
                        else if (dt.total.estado.Equals("Nulo"))
                        {
                            dt.estadoFavorable = "ALTAMENTE FAVORABLE";
                        }
                    }

                    // Contruccion de los elementos del total
                    using (SqlDataReader reader = command_total.ExecuteReader())
                    {

                        dt.totalValue = dt.valueTotal(reader, dt.total_encuesta);

                    }
                    dt.cumplimiento = dt.cumplimiento035(dt.total_empleados, dt.total_encuesta, ternario, datosEmpresa.razonc, datosCentro.nombre);


                }

            }
            return View(dt);
        }


        [HttpPost]
        public ActionResult Acciones(PlanAccionViewModel model)
        {
            return Redirect("~/Empresa/Resultados");
        }

        public ActionResult SinAcciones()
        {
            return View();
        }
        public ActionResult PanelPlanDeAccion()
        {
            return View();
        }

        public ActionResult ReporteAcciones()
        {
            return View();
        }

        public ActionResult MisActividades()
        {
            return View();
        }
    }
}