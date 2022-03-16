using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using rhcon.Models;
using rhcon.Models.ViewModel;
using System.Linq.Dynamic;
using System.Data.Entity.Core.Objects;

namespace rhcon.Controllers
{
    /// <summary>
    /// Controlador para Usuarios del sistema
    /// </summary>
    [HandleError(View = "Error")]
    public class UsuariosController : Controller
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
        /// Devuelve la vistade Usuarios
        /// </summary>
        /// <returns></returns>
        public ActionResult Users()
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
            List<ListUserViewModel> lst = new List<ListUserViewModel>();
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
            using (rhconEntities db =
                new rhconEntities())
            {
                IQueryable<ListUserViewModel> query =
                   (from u in db.usuario
                    join s in db.cstatus on u.idStatus equals s.id
                    join r in db.rol on u.idRol equals r.id
                    select new ListUserViewModel
                    {
                        Id = u.id,
                        nombre = u.nombre,
                        email = u.email,
                        fecha = u.fecha.ToString(),
                        estatus = s.nombre,
                        rol = r.nombre
                    });
                //Search
                if (searchValue != "")
                    query = query.Where(d => d.nombre.Contains(searchValue) || d.email.Contains(searchValue) || d.rol.Contains(searchValue) || d.fecha.Contains(searchValue) || d.estatus.Contains(searchValue));
                //Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    query = query.OrderBy(sortColumn + " " + sortColumnDir);
                }
                //Total de registros
                recordsTotal = query.Count();

                lst = query.Skip(skip).Take(pageSize).ToList();
            }
            return Json(new {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = lst
            });
        }
        /// <summary>
        /// Action Result para mostar el formulario para agregar nuevo Usuario, llena el dorpdownlist
        /// </summary>
        /// <returns></returns>
        public ActionResult Nuevo()
        {
            try
            {
                List<RolViewModel> list = null;

                //Conexion a la base de datos
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
                    return new SelectListItem() {
                        Text = d.Descripcion,
                        Value = d.Id.ToString(),
                        Selected = false
                    };
                });

                // Initialize the model
                UserViewModel model = new UserViewModel();
                model.RolList = items;

                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Controller para insertar nuevo usuario
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Nuevo(UserViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    rhconEntities db = new rhconEntities();

                    ObjectParameter id = new ObjectParameter("UserID", new int { });

                    db.userInsert(user.nombre, user.email, user.password, user.IdRol, 1, id);

                    return Redirect("~/Usuarios/Users");
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
        /// Método para regresar la vista de edición llena con los del usuario
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Editar(int Id)
        {
            List<RolViewModel> list = null;

            //Conexion a la base de datos
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

            // Initialize the model
            UserViewModel model = new UserViewModel();
            using (rhconEntities db = new rhconEntities())
            {
                var oTabla = db.usuario.Find(Id);
                model.nombre = oTabla.nombre;
                model.email = oTabla.email;
                model.IdRol = oTabla.idRol;
                model.Id = oTabla.id;
                model.RolList = items;
            }
            return View(model);
        }
        /// <summary>
        /// Funcion para guardar los cambios de la edición del formulario
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Editar(UserViewModel model)
        {
           
            try
            {
                if (ModelState.IsValid)
                {
                    // Initialize the model
                    using (rhconEntities db = new rhconEntities())
                    {
                        //Actualizar con el SP creado en la base de datos
                        db.userUpdate(model.Id, model.nombre, model.email, model.password, model.IdRol);
                    }

                    return Redirect("~/Usuarios/Users");
                }
                else return View();
            }  
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Metodo para eliminar Usuariosi puedes
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Eliminar(int Id)
        {

            try
            {
                using(rhconEntities db = new rhconEntities())
                {
                    db.userDelete(Id, 3);
                }
                return Redirect("~/Usuarios/Users");
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}