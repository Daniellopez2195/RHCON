using rhcon.Controllers;
using rhcon.Models;
using rhcon.Models.ViewModel;
using System.Web;
using System.Web.Mvc;

namespace rhcon.Filters
{
    /// <summary>
    /// Clase publica heredada de la clase ActionFilterAttribute, que nos sirve para filtros y seguridad de acceso en la plataforma
    /// </summary>
    public class VerifySession : ActionFilterAttribute
    {
        /// <summary>
        /// Metodo con sobreescritura para el filtro durante la ejecucion de la aplicacion
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Obtenemos la sesion de usuario
            var oUser = (UserViewModel)HttpContext.Current.Session["user"];
            //Si la sesion es nula Redirige a Home Login
            if(oUser == null)
            {
                //Si el controller no es HomeController
                if(filterContext.Controller is HomeController == false)
                {
                    filterContext.HttpContext.Response.Redirect("~/Home/Login");
                }
            }
            else //Si la sesion no es nula
            {
                //Si el controller es HomeController
                if(filterContext.Controller is HomeController == true)
                {
                    //Dependiendo el Rol del usuario redirige a su respectiva página
                    switch (oUser.IdRol)
                    {
                        case 1://Super Usuaro
                            filterContext.HttpContext.Response.Redirect("~/Super/");
                            break;
                        case 2://Administrador
                            filterContext.HttpContext.Response.Redirect("~/Administrador/");
                            break;
                        case 3://Empleado
                            filterContext.HttpContext.Response.Redirect("~/Politica/");
                            break;
                        case 4://Responsable de Empresa
                            filterContext.HttpContext.Response.Redirect("~/Empresa/Perfil");
                            break;
                        case 5://Responsable del centro de trabajo
                            filterContext.HttpContext.Response.Redirect("~/CentroTrabajo/");
                            break;
                        case 6://Psicologo
                            filterContext.HttpContext.Response.Redirect("~/Picologo/");
                            break;
                        case 7://Nutriologo
                            filterContext.HttpContext.Response.Redirect("~/Nutriologo/");
                            break;
                        case 8://Legal
                            filterContext.HttpContext.Response.Redirect("~/Legal/");
                            break;
                        case 9://Medico
                            filterContext.HttpContext.Response.Redirect("~/Medico/");
                            break;
                    }
                }              
            }
            //continua con la ejecucion normal de la app
            base.OnActionExecuting(filterContext);
        }
    }
}