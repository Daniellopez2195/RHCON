using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using rhcon.Models;
using rhcon.Models.ViewModel;
using Rotativa;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace rhcon.Controllers
{
    /// <summary>
    /// Controladora para Politicas
    /// </summary>
    [HandleError(View = "Error")]
    public class PoliticaController : Controller
    {
        /// <summary>
        /// GET: Politica
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (TempData["Message"] != null)
                ViewBag.Message = TempData["Message"].ToString();

            EmpleadoViewModel empl = null;
            EmpresaViewModel emp = null;
            //Se obtiene la empresa a partir del usuario guardado en la sesion
            //Obtenemos la sesion de usuario
            var oUser = (UserViewModel)Session["user"];

            if (oUser != null)
            {
                using (rhconEntities bd = new rhconEntities())
                {


                    var politica1 = bd.politica.Where(d => d.tipo.Equals("nom035")).First();
                    var politica2 = bd.politica.Where(d => d.tipo.Equals("nom025")).First();

                    ViewBag.video1 = politica1.video;
                    ViewBag.video2 = politica2.video;


                    empl = (from e in bd.empleado
                            where e.idUsuario == oUser.Id
                            select new EmpleadoViewModel
                            {
                                Id = e.id,
                                Nombre = e.nombre,
                                CURP = e.curp,
                                Nss = e.nss,
                                Telefono = e.telefono,
                                Contacto = e.contacto,
                                CelContacto = e.celcontacto,
                                IdEmpresa = e.idEmpresa,
                                IdCentro = e.idCentroTrabajo,
                                IdUsuario = e.idUsuario,
                                AceptoPolitica = e.aceptoPolitica,
                                Email = oUser.email
                            }).FirstOrDefault();

                    EncargadoEmpresaViewModel encargado = null;
                    encargado = (from d in bd.encargadosEmpresa
                                 where d.idEmpresa == empl.IdEmpresa
                                 select new EncargadoEmpresaViewModel
                                 {
                                     Id = d.id,
                                     IdUsuario = d.idUsuario,
                                     IdEmpresa = d.idEmpresa
                                 }).FirstOrDefault();

                    UserViewModel responsable = (from r in bd.usuario
                                                 where r.id == encargado.IdUsuario
                                                 select new UserViewModel
                                                 {
                                                     Id = r.id,
                                                     nombre = r.nombre,
                                                     email = r.email
                                                 }).FirstOrDefault();


                    emp = (from d in bd.empresa
                           where d.id == empl.IdEmpresa
                           select new EmpresaViewModel
                           {
                               Id = d.id,
                               RazonComercial = d.razonc,
                               RazonSocial = d.razons,
                               Email = d.email,
                               Telefono = d.telefono,
                               Direccion = d.direccion,
                               Actividad = d.actividad,
                               Puesto = d.puesto,
                               RFC = d.rfc,
                               Responsable = responsable.nombre,
                               strlogotipo = d.img,
                               FechaAlta = d.fecha,
                               nombrejerarquia = d.nombrejerarquia,
                               puestojerarquia = d.puestojerarquia
                           }).FirstOrDefault();

                }
                empl.strlogotipo = emp.strlogotipo;
                Session["empleado"] = empl;
                Session["empresa"] = emp;

                ViewBag.empleado = empl.Nombre;
            }

            return (bool)empl.AceptoPolitica ? Redirect("~/Empleado/") : (ActionResult)View();
        }
        /// <summary>
        /// Postback para aceptar politicas y redirigir a Perfil del empleado
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(AceptaPoliticasViewModel model)
        {
            if (!model.AceptoPolitica)
            {
                @TempData["Message"] = "Debes Aceptar las Politicas para continuar";
                return RedirectToAction("Index");
            }
            var oUser = (UserViewModel)Session["user"];
            string RutaSitio = Server.MapPath("~/");
            string PathNom035 = System.IO.Path.Combine(RutaSitio + "/Files/Nom035-" + oUser.email + ".pdf");
            string PathNom025 = System.IO.Path.Combine(RutaSitio + "/Files/Nom025-" + oUser.email + ".pdf");
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            model.nom035.SaveAs(PathNom035);
            model.nom025.SaveAs(PathNom025);

            var oEmp = (EmpleadoViewModel)Session["empleado"];

            using (rhconEntities db = new rhconEntities())
            {
                empleado register = db.empleado.Find(oEmp.Id);

                register.aceptoPolitica = true;

                db.Entry(register).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            //@TempData["Message"] = "La Politica ha sido aceptada";

            return Redirect("~/Empleado");

        }
        /// <summary>
        /// M[etodo para Generar Politica en PDF a traves de la libreria .iText
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult pdf(int Id)
        {
            MemoryStream ms = new MemoryStream();

            PdfWriter pw = new PdfWriter(ms);
            PdfDocument pdfDocument = new PdfDocument(pw);
            Document doc = new Document(pdfDocument, PageSize.LETTER);
            doc.SetMargins(40, 70, 40, 70);

            using (rhconEntities db = new rhconEntities())
            {
                var textPolitica = db.politica.Find(Id);
                var titulo = textPolitica.titulo;

                var oEmpleado = (EmpleadoViewModel)Session["empleado"];
                var oEmpresa = (EmpresaViewModel)Session["empresa"];

                var texto = textPolitica.texto;
                //  razon comercial de la empresa
                var razonc = oEmpresa.RazonComercial;
                // domicilio del centro de trabajo
                var domicilio = oEmpresa.Direccion;
                // centro de trabajo al que pertenece el empleado
                var razons = oEmpresa.RazonSocial;
                //Nombre del empleado de mayor jerarquia de la empresa
                var puesto_gerarquia = oEmpresa.Responsable;
                // puesto de mayor jerarquia de la empresa
                var puesto = oEmpresa.Puesto;
                // Nombre del Empleado
                var empleado = "Yo " + oEmpleado.Nombre + " hago constar que he recibido, leido, comprendido y aceptado";

                texto = texto.Replace("_Nombre_Empresa_", razonc);
                texto = texto.Replace("_domicilio_", domicilio);
                texto = texto.Replace("_Ct_", razons);

                string[] parrafos = texto.Split('*');
                doc.Add(new Paragraph(titulo)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER));

                foreach (var parrafo in parrafos)
                {
                    doc.Add(new Paragraph(parrafo)
                        .SetTextAlignment(TextAlignment.JUSTIFIED)
                        .SetFontSize(10));
                }

                doc.Add(new Paragraph(puesto)
                 .SetTextAlignment(TextAlignment.CENTER)
                 .SetBold()
                 .SetFontSize(9));

                doc.Add(new Paragraph(puesto_gerarquia)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .SetFontSize(9));

                doc.Add(new Paragraph(" " + DateTime.Now.ToShortDateString())
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .SetFontSize(9));

                doc.Add(new Paragraph(empleado)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold()
                .SetFontSize(9));
            }

            doc.Close();

            byte[] bytesStream = ms.ToArray();
            ms = new MemoryStream();
            ms.Write(bytesStream, 0, bytesStream.Length);
            ms.Position = 0;

            return new FileStreamResult(ms, "application/pdf");
        }
        /// <summary>
        /// Rgresa la vista Descargar Politicas
        /// </summary>
        /// <returns></returns>
        public ActionResult Descargarpolitica()
        {
            var oUser = (UserViewModel)Session["user"];
            using (rhconEntities db = new rhconEntities())
            {
                var politica1 = db.politica.Where(d => d.tipo.Equals("nom035")).First();
                var politica2 = db.politica.Where(d => d.tipo.Equals("nom025")).First();

                ViewBag.video1 = politica1.video;
                ViewBag.video2 = politica2.video;

            }
            //oUser.email;

            return View(oUser);

        }
        /// <summary>
        /// Regresa Vista PDF para generar PDF de poliiticas utilizando Libreria Rotativa
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult VistaPdf(int Id)
        {
            using (rhconEntities db = new rhconEntities())
            {
                var textPolitica = db.politica.Find(Id);
                var titulo = textPolitica.titulo;

                var oEmpleado = (EmpleadoViewModel)Session["empleado"];
                var oEmpresa = (EmpresaViewModel)Session["empresa"];

                string politica = "";

                if (Id == 4)
                {
                    politica = "NOM-035-STPS-2018";
                    ViewBag.Separacion = "120%";
                }
                else
                {
                    politica = "NMX-R-025-SCFI-2015 ";
                    ViewBag.Separacion = "150%";
                }

                titulo = titulo + " - " + politica;

                var texto = textPolitica.texto;
                //  razon comercial de la empresa
                var razonc = oEmpresa.RazonComercial;
                // domicilio del centro de trabajo
                var domicilio = oEmpresa.Direccion;
                // centro de trabajo al que pertenece el empleado
                var razons = oEmpresa.RazonSocial;
                //Nombre del empleado de mayor jerarquia de la empresa
                var nombre_gerarquia = oEmpresa.nombrejerarquia;
                // puesto de mayor jerarquia de la empresa
                var puesto = oEmpresa.puestojerarquia;
                // Nombre del Empleado
                var empleado = "Yo " + oEmpleado.Nombre + " hago constar que he recibido, leído, comprendido y aceptado la presente política el día " + DateTime.Now.ToShortDateString();

                texto = texto.Replace("_Nombre_Empresa_", razons);
                texto = texto.Replace("_domicilio_", domicilio);
                texto = texto.Replace("_Ct_", razons);
                string[] parrafos = texto.Split('*');

                ViewBag.parrafos = parrafos;
                ViewBag.nombre_gerarquia = nombre_gerarquia;
                ViewBag.puesto = puesto;
                ViewBag.FechaEmpresa = oEmpresa.FechaAlta.ToShortDateString();
                ViewBag.titulo = titulo;

                var oEmp = (EmpleadoViewModel)Session["empleado"];
                ViewBag.logo = oEmp.strlogotipo;
                string footer = "--footer-center \"" + empleado + "\" --footer-line --footer-font-size \"7\" --footer-spacing 10 --footer-font-name \"arial\"";

                string _headerUrl = Url.Action("HeaderLogo", "Home", new
                {
                    logo = oEmpresa.strlogotipo,
                    titulo = titulo

                }, "https");
                //Si es la NOM 35
                if (Id == 4)
                {
                    return new ViewAsPdf("VistaPdf", new { parrafos = parrafos })
                    {
                        FileName = "Politica" + politica + ".pdf",
                        CustomSwitches = "--header-html " + _headerUrl + " --header-spacing 0 " + footer,
                        PageSize = Rotativa.Options.Size.Letter,
                        PageMargins = new Rotativa.Options.Margins(60, 20, 20, 20),
                    };
                }
                else //Si es la nom 25
                {
                    return new ViewAsPdf("VistaPdf", new { parrafos = parrafos })
                    {
                        FileName = "Politica" + politica + ".pdf",
                        CustomSwitches = "--header-html " + _headerUrl + " --header-spacing 0 " + footer,
                        PageSize = Rotativa.Options.Size.Letter,
                        PageMargins = new Rotativa.Options.Margins(55, 25, 20, 25),
                    };
                }


            }
        }
        /// <summary>
        /// Regresa Vista Para Cabecera de PDF
        /// </summary>
        /// <returns></returns>
        public ActionResult HeaderPolitica()
        {
            return View();
        }

        public ActionResult PoliticaVideo()
        {

            using (rhconEntities db = new rhconEntities())
            {
                var politica1 = db.politica.Where(d => d.tipo.Equals("nom035")).First();
                var politica2 = db.politica.Where(d => d.tipo.Equals("nom025")).First();

                ViewBag.video1 = politica1.video;
                ViewBag.video2 = politica2.video;
                return View();

            }

        }

        public ActionResult ActualizarVideo(string link1, string link2)
        {
            using (rhconEntities db = new rhconEntities())
            {
                var politica1 = db.politica.Where(d => d.tipo.Equals("nom035")).First();
                var politica2 = db.politica.Where(d => d.tipo.Equals("nom025")).First();
                politica1.video = link1;
                politica2.video = link2;
                db.Entry(politica1).State = System.Data.Entity.EntityState.Modified;
                db.Entry(politica2).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            return Redirect("~/Administrador/");
        }
    }
}