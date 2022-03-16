using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using rhcon.Models;
using rhcon.Models.ViewModel;
using rhcon.utils;
using SpreadsheetLight;

namespace rhcon.Controllers
{
    /// <summary>
    /// Controlador para la carga masiva de Centros de Trabajo y Empleados
    /// </summary>
    [HandleError(View = "Error")]
    public class CargaMasivaController : Controller
    {
        /// <summary>
        /// GET: CargaMasiva
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Regresa la vista del centro de trabajo
        /// </summary>
        /// <returns></returns>
        public ActionResult Centro()
        {
            return View();
        }
        /// <summary>
        /// Guardar centros de trabajo a traves de un archivo de excel previamente cargado con los centros de trabajo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveCentro(ArchivoViewModel model)
        {
            try
            {
                //se cargan variables de Empresa
                var oEmpresa = (EmpresaViewModel)Session["empresa"];
                // ruta del servidor
                String RutaSitio = Server.MapPath("~/");
                // Identificador par el archivo que cargaremos en el servidor para realizar la carga masiva 
                String Nombre = "centros-" + oEmpresa.Id;

                if (!ModelState.IsValid)
                {
                    return View("Centro", model);
                }
                // ruta completa donde se subira el archivo 
                String PathArchivo = Path.Combine(RutaSitio + "/Files/" + Nombre + ".xlsx");
                // carga del archivo en el servidor 
                model.Archivo.SaveAs(PathArchivo);
                // metodo para la lectura de un archivo de excel
                SLDocument sl = new SLDocument(PathArchivo);
                // fila de inicio de lectura del excel
                int iRow = 2;
                //Se genera un objeto de parámetro para el ID
                ObjectParameter id = new ObjectParameter("ctID", new int { });

                // Validar archivo que contenga datos en las celdas
                if (string.IsNullOrEmpty(sl.GetCellValueAsString(iRow, 1)))
                {
                    ViewBag.Message = "El archivo no contiene datos";
                    return View("Centro", model);
                }
                // recorremos el arreglo mientras la fila del excel no este vacia
                while (!string.IsNullOrEmpty(sl.GetCellValueAsString(iRow, 1)))
                {
                    using (rhconEntities db = new rhconEntities())
                    {
                        // registro del centro de trabajo en la db 
                        CentroTrabajoViewModel centro = new CentroTrabajoViewModel();
                        centro.Nombre = sl.GetCellValueAsString(iRow, 1);
                        centro.Direccion = sl.GetCellValueAsString(iRow, 2);
                        centro.email = sl.GetCellValueAsString(iRow, 3);
                        centro.Encargado = sl.GetCellValueAsString(iRow, 4);
                        centro.Actividades = sl.GetCellValueAsString(iRow, 5);
                        centro.IdEmpresa = oEmpresa.Id;

                        // Se obtiene el password
                        string password = utils.utilerias.RandomString(12);
                        // Se obtiene el rol
                        // Se busca el rol en la base de datos
                        RolViewModel rol = (from d in db.rol
                                            where d.nombre == "Reponsable Centro de Trabajo"
                                            select new RolViewModel
                                            {
                                                Id = d.id,
                                                Descripcion = d.nombre
                                            }).FirstOrDefault();
                        int idEstatus = 1;
                        db.ctrabajoInsert(centro.Nombre, centro.Direccion, centro.Actividades, centro.IdEmpresa, idEstatus, centro.Encargado, centro.email, password, rol.Id, id);

                        //Envio de email al empleado
                        string EmailORigen = "bienestarlaboral@rhcon.com.mx";
                        string EmailDestino = centro.email;
                        string pass = "Bienestar2022";
                        var body = db.correos.Where(d => d.tipo == "altaEmp").First();
                        string mensaje = body.email.ToString();
                        mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                        mensaje = mensaje.Replace("_razonsocial_", oEmpresa.RazonSocial);
                        mensaje = mensaje.Replace("_usuario_", centro.email);
                        mensaje = mensaje.Replace("_pass_", password);
                        mensaje = mensaje.Replace("_redireccion_", "https://bienestarlaboral.rhcon.com.mx/Home/Login?IdRol=" + rol.Id + "&email=" + centro.email + "&password=" + password);
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
                    }
                    iRow++;
                }
                ViewBag.Message = "La Carga de los centros de trabajo se ha llevado a cabo de forma correcta";
                return View("Centro");
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
        /// Regresa la vista para carga masiva de empleados
        /// </summary>
        /// <returns></returns>
        public ActionResult Empleado()
        {
            return View();
        }

        /// <summary>
        /// Regresa la vista para carga masiva de empleados
        /// </summary>
        /// <returns></returns>
        public ActionResult EmpleadoCt()
        {
            return View();
        }
        /// <summary>
        /// Guardar Empleados a traves de un archivo de excel previamente cargado con los empleados
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveEmpleado(ArchivoViewModel model)
        {
            try
            {
                //se cargan variables de Empresa
                var oEmpresa = (EmpresaViewModel)Session["empresa"];
                // ruta del servidor
                String RutaSitio = Server.MapPath("~/");
                // Identificador par el archivo que cargaremos en el servidor para realizar la carga masiva 
                String Nombre = "empleados-" + oEmpresa.Id;
                // Validacion para determinar si se eligi[o un archivo valido
                if (!ModelState.IsValid)
                {
                    return View("Empleado", model);
                }
                // ruta completa donde se subira el archivo 
                String PathArchivo = Path.Combine(RutaSitio + "/Files/" + Nombre + ".xlsx");
                // carga del archivo en el servidor 
                model.Archivo.SaveAs(PathArchivo);
                // metodo para la lectura de un archivo de excel
                SLDocument sl = new SLDocument(PathArchivo);
                // fila de inicio de lectura del excel
                int iRow = 2;
                // Validar archivo que contenga datos en las celdas
                if (string.IsNullOrEmpty(sl.GetCellValueAsString(iRow, 1)))
                {
                    ViewBag.Message = "El archivo no contiene datos";
                    return View("Empleado", model);
                }
                //Bucle para validar el archivo de excel
                while (!string.IsNullOrEmpty(sl.GetCellValueAsString(iRow, 1)))
                {
                    using (rhconEntities db = new rhconEntities())
                    {
                        // Registro del empleado
                        EmpleadoViewModel oEmpleado = new EmpleadoViewModel();
                        //Se cargan los valores del archivo al objeto
                        oEmpleado.Nombre = sl.GetCellValueAsString(iRow, 1);
                        oEmpleado.CURP = sl.GetCellValueAsString(iRow, 2);
                        oEmpleado.Nss = sl.GetCellValueAsString(iRow, 3);
                        oEmpleado.Email = sl.GetCellValueAsString(iRow, 4);
                        oEmpleado.Telefono = sl.GetCellValueAsString(iRow, 5);
                        oEmpleado.Contacto = sl.GetCellValueAsString(iRow, 6);
                        oEmpleado.CelContacto = sl.GetCellValueAsString(iRow, 7);
                        oEmpleado.AreaFuncion = sl.GetCellValueAsString(iRow, 8);
                        // Se obtiene el centro de trabajo
                        centroTrabajo oCentroTrabajo = null;

                        if (Session["centro"] == null)
                        {
                            string nombrecentro = sl.GetCellValueAsString(iRow, 8);
                            sl.GetCellValueAsString(iRow, 8);
                            oCentroTrabajo = db.centroTrabajo.Where(c => c.nombre.Equals(nombrecentro) && c.idEmpresa == oEmpresa.Id).FirstOrDefault();
                            if (oCentroTrabajo != null)
                                oEmpleado.IdCentro = oCentroTrabajo.id;
                            else
                                throw new Exception("El Centro de trabajo " + nombrecentro + " no existe en la base de datos o se ha escrito de forma incorrecta, por favor verifique los datos del archivo, en la fila " + iRow + ", con nombre de empleado " + oEmpleado.Nombre + ", no se ha registrado ningun dato. ");
                        }
                    }
                    iRow++;
                }
                iRow = 2;
                //Bucle para recorrer las filas del archivo excel
                while (!string.IsNullOrEmpty(sl.GetCellValueAsString(iRow, 1)))
                {
                    using (rhconEntities db = new rhconEntities())
                    {
                        // Registro del empleado
                        EmpleadoViewModel oEmpleado = new EmpleadoViewModel();
                        //Se cargan los valores del archivo al objeto
                        oEmpleado.Nombre = sl.GetCellValueAsString(iRow, 1);
                        oEmpleado.CURP = sl.GetCellValueAsString(iRow, 2);
                        oEmpleado.Nss = sl.GetCellValueAsString(iRow, 3);
                        oEmpleado.Email = sl.GetCellValueAsString(iRow, 4);
                        oEmpleado.Telefono = sl.GetCellValueAsString(iRow, 5);
                        oEmpleado.Contacto = sl.GetCellValueAsString(iRow, 6);
                        oEmpleado.CelContacto = sl.GetCellValueAsString(iRow, 7);
                        oEmpleado.AreaFuncion = sl.GetCellValueAsString(iRow, 8);
                        // Se obtiene el centro de trabajo
                        if (Session["centro"] == null)
                        {
                            string nombrecentro = sl.GetCellValueAsString(iRow, 8);
                            centroTrabajo oCentroTrabajo = db.centroTrabajo.Where(c => c.nombre.Equals(nombrecentro) && c.idEmpresa == oEmpresa.Id).FirstOrDefault();
                            oEmpleado.IdCentro = oCentroTrabajo.id;
                        }
                        else
                        {
                            var oCentro = (CentroTrabajoViewModel)Session["centro"];
                            oEmpleado.IdCentro = oCentro.Id;
                        }

                        // Se obtiene el password
                        string password = utils.utilerias.RandomString(12);
                        // Se obtiene el rol
                        // Se busca el rol en la base de datos
                        RolViewModel rol = (from d in db.rol
                                            where d.nombre == "Empleado(a)"
                                            select new RolViewModel
                                            {
                                                Id = d.id,
                                                Descripcion = d.nombre
                                            }).FirstOrDefault();
                        int idEstatus = 1;
                        //Se genera un objeto de parámetro para el ID
                        ObjectParameter id = new ObjectParameter("EmplID", new int { });
                        // Se ingresan los datos en la base de datos
                        db.empleadoInsert(oEmpleado.Nombre, oEmpleado.CURP, oEmpleado.Nss, oEmpleado.Telefono, oEmpleado.Contacto, oEmpleado.CelContacto, oEmpresa.Id, oEmpleado.IdCentro, idEstatus, oEmpleado.Nombre, oEmpleado.Email, password, rol.Id, oEmpleado.AreaFuncion, id);

                        //Envio de email al empleado
                        string EmailORigen = "bienestarlaboral@rhcon.com.mx";
                        string EmailDestino = oEmpleado.Email;
                        string pass = "Bienestar2022";
                        var body = db.correos.Where(d => d.tipo == "altaEmp").First();
                        string mensaje = body.email.ToString();
                        mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                        mensaje = mensaje.Replace("_razonsocial_", oEmpresa.RazonSocial);
                        mensaje = mensaje.Replace("_usuario_", oEmpleado.Email);
                        mensaje = mensaje.Replace("_pass_", password);
                        mensaje = mensaje.Replace("_redireccion_", "https://bienestarlaboral.rhcon.com.mx/Home/Login?IdRol=" + rol.Id + "&email=" + oEmpleado.Email + "&password=" + password);
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
                    }
                    iRow++;
                }
                ViewBag.Message = "La Carga de los empleados se ha llevado a cabo de forma correcta";
                if (Session["centro"] == null)
                    return Redirect("~/Empresa/Empleados");
                else
                    return Redirect("~/CentroTrabajo/Empleados");
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                    throw new Exception(ex.Message);
                else
                    throw new Exception(ex.Message + "\n" + ex.InnerException.Message);
            }
        }

        public ActionResult EditEmpleado()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EditEmpleado(ArchivoViewModel model)
        {
            try
            {
                //se cargan variables de Empresa
                var oEmpresa = (EmpresaViewModel)Session["empresa"];
                // ruta del servidor
                String RutaSitio = Server.MapPath("~/");
                // Identificador par el archivo que cargaremos en el servidor para realizar la carga masiva 
                String Nombre = "empleadosEdit-" + oEmpresa.Id;
                // Validacion para determinar si se eligi[o un archivo valido
                if (!ModelState.IsValid)
                {
                    return View("Empleado", model);
                }
                // ruta completa donde se subira el archivo 
                String PathArchivo = Path.Combine(RutaSitio + "/Files/" + Nombre + ".xlsx");
                // carga del archivo en el servidor 
                model.Archivo.SaveAs(PathArchivo);
                // metodo para la lectura de un archivo de excel
                SLDocument sl = new SLDocument(PathArchivo);
                // fila de inicio de lectura del excel
                int iRow = 3;
                // Validar archivo que contenga datos en las celdas
                if (string.IsNullOrEmpty(sl.GetCellValueAsString(iRow, 1)))
                {
                    ViewBag.Message = "El archivo no contiene datos";
                    return View("Empleado", model);
                }
                //Bucle para validar el archivo de excel
                while (!string.IsNullOrEmpty(sl.GetCellValueAsString(iRow, 1)))
                {
                    using (rhconEntities db = new rhconEntities())
                    {
                        // Registro del empleado
                        EmpleadoViewModel oEmpleado = new EmpleadoViewModel();
                        //Se cargan los valores del archivo al objeto
                        oEmpleado.Nombre = sl.GetCellValueAsString(iRow, 2);
                        oEmpleado.CURP = sl.GetCellValueAsString(iRow, 3);
                        oEmpleado.Nss = sl.GetCellValueAsString(iRow, 1);
                        oEmpleado.Email = sl.GetCellValueAsString(iRow, 7);
                        oEmpleado.Telefono = sl.GetCellValueAsString(iRow, 4);
                        oEmpleado.Contacto = sl.GetCellValueAsString(iRow, 5);
                        oEmpleado.CelContacto = sl.GetCellValueAsString(iRow, 6);
                        oEmpleado.AreaFuncion = sl.GetCellValueAsString(iRow, 9);
                        // Se obtiene el centro de trabajo
                        centroTrabajo oCentroTrabajo = null;

                        if (Session["centro"] == null)
                        {
                            string nombrecentro = sl.GetCellValueAsString(iRow, 8);
                            sl.GetCellValueAsString(iRow, 8);
                            oCentroTrabajo = db.centroTrabajo.Where(c => c.nombre.Equals(nombrecentro) && c.idEmpresa == oEmpresa.Id).FirstOrDefault();
                            if (oCentroTrabajo != null)
                                oEmpleado.IdCentro = oCentroTrabajo.id;
                            else
                                throw new Exception("El Centro de trabajo " + nombrecentro + " no existe en la base de datos o se ha escrito de forma incorrecta, por favor verifique los datos del archivo, en la fila " + iRow + ", con nombre de empleado " + oEmpleado.Nombre + ", no se ha registrado ningun dato. ");
                        }
                    }
                    iRow++;
                }
                iRow = 3;
                //Bucle para recorrer las filas del archivo excel
                while (!string.IsNullOrEmpty(sl.GetCellValueAsString(iRow, 1)))
                {
                    using (rhconEntities db = new rhconEntities())
                    {
                        // Registro del empleado
                        int id = int.Parse(sl.GetCellValueAsString(iRow, 11));
                        empleado register = db.empleado.Where(d => d.id == id).First();

                        //Se cargan los valores del archivo al objeto
                        if (!register.nombre.Equals(sl.GetCellValueAsString(iRow, 2)))
                            register.nombre = sl.GetCellValueAsString(iRow, 2);
                        if (!register.curp.Equals(sl.GetCellValueAsString(iRow, 3)))
                            register.curp = sl.GetCellValueAsString(iRow, 3);
                        if (!register.nss.Equals(sl.GetCellValueAsString(iRow, 1)))
                            register.nss = sl.GetCellValueAsString(iRow, 1);
                        if (!register.telefono.Equals(sl.GetCellValueAsString(iRow, 4)))
                            register.telefono = sl.GetCellValueAsString(iRow, 4);
                        if (!register.contacto.Equals(sl.GetCellValueAsString(iRow, 5)))
                            register.contacto = sl.GetCellValueAsString(iRow, 5);
                        if (!register.celcontacto.Equals(sl.GetCellValueAsString(iRow, 6)))
                            register.celcontacto = sl.GetCellValueAsString(iRow, 6);
                        if (!register.area_funcion.Equals(sl.GetCellValueAsString(iRow, 9)))
                            register.area_funcion = sl.GetCellValueAsString(iRow, 9);
                        // Se obtiene el centro de trabajo
                        if (Session["centro"] == null)
                        {

                            string nombrecentro = sl.GetCellValueAsString(iRow, 8);
                            centroTrabajo oCentroTrabajo = db.centroTrabajo.Where(c => c.nombre.Equals(nombrecentro) && c.idEmpresa == oEmpresa.Id).FirstOrDefault();
                            register.idCentroTrabajo = oCentroTrabajo.id;
                        }
                        else
                        {
                            var oCentro = (CentroTrabajoViewModel)Session["centro"];
                            register.idCentroTrabajo = oCentro.Id;
                        }

                        db.Entry(register).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        string email = sl.GetCellValueAsString(iRow, 7);
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

                        if (!email.Equals(datos.email))
                        {
                            var body = new correos();
                            body = db.correos.Where(d => d.tipo == "altaEmp").First();

                            db.userUpdate(register.idUsuario, register.nombre, email, password, rol.Id);
                            //Envio de email al encargado de la empresa
                            string EmailORigen = "bienestarlaboral@rhcon.com.mx";
                            string EmailDestino = email;
                            string pass = "Bienestar2022";
                            string mensaje = body.email.ToString();
                            mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                            mensaje = mensaje.Replace("_razonsocial_", oEmpresa.RazonSocial);
                            mensaje = mensaje.Replace("_usuario_", email);
                            mensaje = mensaje.Replace("_pass_", password);
                            mensaje = mensaje.Replace("_redireccion_", "https://bienestarlaboral.rhcon.com.mx/Home/Login?IdRol=" + rol.Id + "&email=" + email + "&password=" + password);
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
                    iRow++;
                }
                ViewBag.Message = "La Carga de los empleados se ha llevado a cabo de forma correcta";
                if (Session["centro"] == null)
                    return Redirect("~/Empresa/Empleados");
                else
                    return Redirect("~/CentroTrabajo/Empleados");
            }
            catch (Exception ex)
            {

                if (ex.InnerException == null)
                    throw new Exception(ex.Message);
                else
                    throw new Exception(ex.Message + "\n" + ex.InnerException.Message);
            }
        }
    }
}