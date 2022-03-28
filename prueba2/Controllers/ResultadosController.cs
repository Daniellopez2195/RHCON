using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using rhcon.Models;
using rhcon.Models.ViewModel;
using Rotativa;

namespace rhcon.Controllers
{
    public class ResultadosController : Controller
    {

        public ActionResult jsonNom035(string yearData, string idCentro = "", string centro = "false", string rango = "")
        {

            var oEmpresa = (EmpresaViewModel)Session["empresa"];
            string title = "Percepción por Categoría del Nivel de Riesgo Psicosocial y Entorno Organizacional ";
            double[] blue = { 0, 0, 0, 0, 0 };
            double[] orange = { 0, 0, 0, 0, 0 };
            double[] yellow = { 0, 0, 0, 0, 0 };
            double[] green = { 0, 0, 0, 0, 0 };
            double[] red = { 0, 0, 0, 0, 0 };
            double personal = 0;
            SqlConnectionStringBuilder conect = ConexionViewModel.conectar();



            using (SqlConnection connection = new SqlConnection(conect.ConnectionString))
            {

                var year = DateTime.Now.Year;

                string Ambiente_trabajo = ConsultasViewModel.GraficarNom035("Ambiente de trabajo", oEmpresa.Id, yearData, idCentro);
                SqlCommand command = new SqlCommand(Ambiente_trabajo, connection);
                connection.Open();

                // categoria de ambiente de trabajo
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int punto = int.Parse(reader[0].ToString());

                        if (punto < 5)
                            blue[0] += 1;
                        else if (punto < 9)
                            green[0] += 1;
                        else if (punto < 11)
                            yellow[0] += 1;
                        else if (punto < 14)
                            orange[0] += 1;
                        else if (punto >= 14)
                            red[0] += 1;
                        personal++;
                    }

                    blue[0] = (blue[0] * 100) / personal;
                    green[0] = (green[0] * 100) / personal;
                    yellow[0] = (yellow[0] * 100) / personal;
                    orange[0] = (orange[0] * 100) / personal;
                    red[0] = (red[0] * 100) / personal;
                    personal = 0;
                }


                //Factores propios de la actividad
                string factores_propios = ConsultasViewModel.GraficarNom035("Factores propios de la actividad", oEmpresa.Id, yearData, idCentro);
                SqlCommand command_f = new SqlCommand(factores_propios, connection);

                using (SqlDataReader reader = command_f.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int punto = int.Parse(reader[0].ToString());

                        if (punto < 15)
                            blue[1] += 1;
                        else if (punto < 30)
                            green[1] += 1;
                        else if (punto < 45)
                            yellow[1] += 1;
                        else if (punto < 60)
                            orange[1] += 1;
                        else if (punto > 60)
                            red[1] += 1;
                        personal++;
                    }

                    blue[1] = (blue[1] * 100) / personal;
                    green[1] = (green[1] * 100) / personal;
                    yellow[1] = (yellow[1] * 100) / personal;
                    orange[1] = (orange[1] * 100) / personal;
                    red[1] = (red[1] * 100) / personal;
                    personal = 0;
                }

                //Organización de tiempo de trabajo
                string organizacion_trabajo = ConsultasViewModel.GraficarNom035("Organización del tiempo de trabajo", oEmpresa.Id, yearData, idCentro);
                SqlCommand command_o = new SqlCommand(organizacion_trabajo, connection);

                using (SqlDataReader reader = command_o.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int punto = int.Parse(reader[0].ToString());

                        if (punto < 5)
                            blue[2] += 1;
                        else if (punto < 7)
                            green[2] += 1;
                        else if (punto < 10)
                            yellow[2] += 1;
                        else if (punto < 13)
                            orange[2] += 1;
                        else if (punto >= 13)
                            red[2] += 1;
                        personal++;
                    }

                    blue[2] = (blue[2] * 100) / personal;
                    green[2] = (green[2] * 100) / personal;
                    yellow[2] = (yellow[2] * 100) / personal;
                    orange[2] = (orange[2] * 100) / personal;
                    red[2] = (red[2] * 100) / personal;
                    personal = 0;
                }

                //Liderazgo y relaciones en el trabajo
                string liderazgo_trabajo = ConsultasViewModel.GraficarNom035("Liderazgo y relaciones en el trabajo", oEmpresa.Id, yearData, idCentro);
                SqlCommand command_l = new SqlCommand(liderazgo_trabajo, connection);
                using (SqlDataReader reader = command_l.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int punto = int.Parse(reader[0].ToString());

                        if (punto < 14)
                            blue[3] += 1;
                        else if (punto < 29)
                            green[3] += 1;
                        else if (punto < 42)
                            yellow[3] += 1;
                        else if (punto < 58)
                            orange[3] += 1;
                        else if (punto >= 58)
                            red[3] += 1;
                        personal++;
                    }

                    blue[3] = (blue[3] * 100) / personal;
                    green[3] = (green[3] * 100) / personal;
                    yellow[3] = (yellow[3] * 100) / personal;
                    orange[3] = (orange[3] * 100) / personal;
                    red[3] = (red[3] * 100) / personal;
                    personal = 0;
                }


                //Entorno Organizacional
                string entorno_organizacional = ConsultasViewModel.GraficarNom035("Entorno organizacional", oEmpresa.Id, yearData, idCentro);
                SqlCommand command_e = new SqlCommand(entorno_organizacional, connection);
                using (SqlDataReader reader = command_e.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int punto = int.Parse(reader[0].ToString());

                        if (punto < 10)
                            blue[4] += 1;
                        else if (punto < 14)
                            green[4] += 1;
                        else if (punto < 18)
                            yellow[4] += 1;
                        else if (punto < 23)
                            orange[4] += 1;
                        else if (punto >= 23)
                            red[4] += 1;
                        personal++;
                    }

                    blue[4] = (blue[4] * 100) / personal;
                    green[4] = (green[4] * 100) / personal;
                    yellow[4] = (yellow[4] * 100) / personal;
                    orange[4] = (orange[4] * 100) / personal;
                    red[4] = (red[4] * 100) / personal;
                    personal = 0;
                }

                connection.Close();
            }


            using (rhconEntities db = new rhconEntities())
            {
                if (!string.IsNullOrEmpty(idCentro))
                {
                    var id = int.Parse(idCentro);
                    var c = db.centroTrabajo.Where(d => d.id == id).First();
                    title += "en el centro de trabajo " + c.nombre.ToString();

                }
                else
                {
                    title += "en " + oEmpresa.RazonSocial;
                }
            }


            return Json(
           new
           {
               title = title,
               data = yearData,
               red = red,
               orange = orange,
               yellow = yellow,
               green = green,
               blue = blue

           });
        }



        public ActionResult jsonNom025(string yearData, string idCentro = "", bool centro = false)
        {

            var oEmpresa = (EmpresaViewModel)Session["empresa"];
            string title = "Percepción por Apartados del Nivel Riesgos de Clima Laboral y no Discriminación ";
            double[] blue = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] orange = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] yellow = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] green = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] red = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int personal = 0;
            SqlConnectionStringBuilder conect = ConexionViewModel.conectar();



            using (SqlConnection connection = new SqlConnection(conect.ConnectionString))
            {

                var year = DateTime.Now.Year;

                string reclutamiento_seleccion = ConsultasViewModel.GraficarNom025("reclutamiento y selección de personal", oEmpresa.Id, yearData, idCentro);
                SqlCommand command = new SqlCommand(reclutamiento_seleccion, connection);
                connection.Open();

                // categoria de ambiente de trabajo
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int punto = int.Parse(reader[0].ToString());
                        punto = punto / 4;

                        if (punto >= 2.5)
                            blue[0] += 1;
                        else if (punto >= 1.5)
                            green[0] += 1;
                        else if (punto >= 1)
                            orange[0] += 1;
                        else if (punto < 1)
                            red[0] += 1;
                        personal++;
                    }




                    blue[0] = (blue[0] * 100) / personal;
                    green[0] = (green[0] * 100) / personal;
                    yellow[0] = (yellow[0] * 100) / personal;
                    orange[0] = (orange[0] * 100) / personal;
                    red[0] = (red[0] * 100) / personal;
                    personal = 0;
                }

                string formacion_capacitacion = ConsultasViewModel.GraficarNom025("Formación y capacitación", oEmpresa.Id, yearData, idCentro);
                SqlCommand command_f = new SqlCommand(formacion_capacitacion, connection);


                // categoria de ambiente de trabajo
                using (SqlDataReader reader = command_f.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int punto = int.Parse(reader[0].ToString());
                        punto = punto / 4;

                        if (punto >= 2.5)
                            blue[1] += 1;
                        else if (punto >= 1.5)
                            green[1] += 1;
                        else if (punto >= 1)
                            orange[1] += 1;
                        else if (punto < 1)
                            red[1] += 1;
                        personal++;
                    }




                    blue[1] = (blue[1] * 100) / personal;
                    green[1] = (green[1] * 100) / personal;
                    yellow[1] = (yellow[1] * 100) / personal;
                    orange[1] = (orange[1] * 100) / personal;
                    red[1] = (red[1] * 100) / personal;
                    personal = 0;
                }


                string pertenecia_ascenso = ConsultasViewModel.GraficarNom025("Permanencia y ascenso", oEmpresa.Id, yearData, idCentro);
                SqlCommand command_p = new SqlCommand(pertenecia_ascenso, connection);


                // categoria de ambiente de trabajo
                using (SqlDataReader reader = command_p.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int punto = int.Parse(reader[0].ToString());
                        punto = punto / 6;

                        if (punto >= 2.5)
                            blue[2] += 1;
                        else if (punto >= 1.5)
                            green[2] += 1;
                        else if (punto >= 1)
                            orange[2] += 1;
                        else if (punto < 1)
                            red[2] += 1;
                        personal++;
                    }




                    blue[2] = (blue[2] * 100) / personal;
                    green[2] = (green[2] * 100) / personal;
                    yellow[2] = (yellow[2] * 100) / personal;
                    orange[2] = (orange[2] * 100) / personal;
                    red[2] = (red[2] * 100) / personal;
                    personal = 0;
                }

                string familiar_personal = ConsultasViewModel.GraficarNom025("corresponsabilidad en la vida laboral, familiar y personal", oEmpresa.Id, yearData, idCentro);
                SqlCommand command_fp = new SqlCommand(familiar_personal, connection);


                // categoria de ambiente de trabajo
                using (SqlDataReader reader = command_fp.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int punto = int.Parse(reader[0].ToString());
                        punto = punto / 9;

                        if (punto >= 2.5)
                            blue[3] += 1;
                        else if (punto >= 1.5)
                            green[3] += 1;
                        else if (punto >= 1)
                            orange[3] += 1;
                        else if (punto < 1)
                            red[3] += 1;
                        personal++;
                    }




                    blue[3] = (blue[3] * 100) / personal;
                    green[3] = (green[3] * 100) / personal;
                    yellow[3] = (yellow[3] * 100) / personal;
                    orange[3] = (orange[3] * 100) / personal;
                    red[3] = (red[3] * 100) / personal;
                    personal = 0;
                }

                string clima_laboral = ConsultasViewModel.GraficarNom025("Clima laboral libre de violencia", oEmpresa.Id, yearData, idCentro);
                SqlCommand command_cl = new SqlCommand(clima_laboral, connection);


                // categoria de ambiente de trabajo
                using (SqlDataReader reader = command_cl.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int punto = int.Parse(reader[0].ToString());
                        punto = punto / 14;

                        if (punto >= 2.5)
                            blue[4] += 1;
                        else if (punto >= 1.5)
                            green[4] += 1;
                        else if (punto >= 1)
                            orange[4] += 1;
                        else if (punto < 1)
                            red[4] += 1;
                        personal++;
                    }




                    blue[4] = (blue[4] * 100) / personal;
                    green[4] = (green[4] * 100) / personal;
                    yellow[4] = (yellow[4] * 100) / personal;
                    orange[4] = (orange[4] * 100) / personal;
                    red[4] = (red[4] * 100) / personal;
                    personal = 0;
                }

                string acoso_hostigamiento = ConsultasViewModel.GraficarNom025("Acoso y Hostigamiento", oEmpresa.Id, yearData, idCentro);
                SqlCommand command_ah = new SqlCommand(acoso_hostigamiento, connection);


                // categoria de ambiente de trabajo
                using (SqlDataReader reader = command_ah.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int punto = int.Parse(reader[0].ToString());
                        punto = punto / 8;

                        if (punto >= 2.5)
                            blue[5] += 1;
                        else if (punto >= 1.5)
                            green[5] += 1;
                        else if (punto >= 1)
                            orange[5] += 1;
                        else if (punto < 1)
                            red[5] += 1;
                        personal++;
                    }




                    blue[5] = (blue[5] * 100) / personal;
                    green[5] = (green[5] * 100) / personal;
                    yellow[5] = (yellow[5] * 100) / personal;
                    orange[5] = (orange[5] * 100) / personal;
                    red[5] = (red[5] * 100) / personal;
                    personal = 0;
                }


                string accesibilidad = ConsultasViewModel.GraficarNom025("Accesibilidad", oEmpresa.Id, yearData, idCentro);
                SqlCommand command_a = new SqlCommand(accesibilidad, connection);


                // categoria de ambiente de trabajo
                using (SqlDataReader reader = command_a.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int punto = int.Parse(reader[0].ToString());
                        punto = punto / 5;

                        if (punto >= 2.5)
                            blue[6] += 1;
                        else if (punto >= 1.5)
                            green[6] += 1;
                        else if (punto >= 1)
                            orange[6] += 1;
                        else if (punto < 1)
                            red[6] += 1;
                        personal++;
                    }




                    blue[6] = (blue[6] * 100) / personal;
                    green[6] = (green[6] * 100) / personal;
                    yellow[6] = (yellow[6] * 100) / personal;
                    orange[6] = (orange[6] * 100) / personal;
                    red[6] = (red[6] * 100) / personal;
                    personal = 0;
                }

                string respeto_diversidad = ConsultasViewModel.GraficarNom025("Respeto a la diversidad", oEmpresa.Id, yearData, idCentro);
                SqlCommand command_rd = new SqlCommand(respeto_diversidad, connection);


                // categoria de ambiente de trabajo
                using (SqlDataReader reader = command_rd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int punto = int.Parse(reader[0].ToString());
                        punto = punto / 2;

                        if (punto >= 2.5)
                            blue[7] += 1;
                        else if (punto >= 1.5)
                            green[7] += 1;
                        else if (punto >= 1)
                            orange[7] += 1;
                        else if (punto < 1)
                            red[7] += 1;
                        personal++;
                    }




                    blue[7] = (blue[7] * 100) / personal;
                    green[7] = (green[7] * 100) / personal;
                    yellow[7] = (yellow[7] * 100) / personal;
                    orange[7] = (orange[7] * 100) / personal;
                    red[7] = (red[7] * 100) / personal;
                    personal = 0;
                }


                string condiciones_generales = ConsultasViewModel.GraficarNom025("Condiciones generales de trabajo", oEmpresa.Id, yearData, idCentro);
                SqlCommand command_cg = new SqlCommand(condiciones_generales, connection);


                // categoria de ambiente de trabajo
                using (SqlDataReader reader = command_cg.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        int punto = int.Parse(reader[0].ToString());
                        punto = punto / 4;

                        if (punto >= 2.5)
                            blue[8] += 1;
                        else if (punto >= 1.5)
                            green[8] += 1;
                        else if (punto >= 1)
                            orange[8] += 1;
                        else if (punto < 1)
                            red[8] += 1;
                        personal++;
                    }




                    blue[8] = (blue[8] * 100) / personal;
                    green[8] = (green[8] * 100) / personal;
                    yellow[8] = (yellow[8] * 100) / personal;
                    orange[8] = (orange[8] * 100) / personal;
                    red[8] = (red[8] * 100) / personal;
                    personal = 0;
                }


            }


            using (rhconEntities db = new rhconEntities())
            {
                if (!string.IsNullOrEmpty(idCentro))
                {
                    var id = int.Parse(idCentro);
                    var c = db.centroTrabajo.Where(d => d.id == id).First();
                    title += "en el centro de trabajo " + c.nombre.ToString();

                }
                else
                {
                    title += "en " + oEmpresa.RazonSocial;
                }
            }


            return Json(
           new
           {
               title = title,
               data = yearData,
               red = red,
               orange = orange,
               yellow = yellow,
               green = green,
               blue = blue

           });
        }




        [HttpPost]
        public ActionResult activar()
        {
            var oEmpresa = (EmpresaViewModel)Session["empresa"];
            DateTime year = DateTime.Now;

            using (rhconEntities db = new rhconEntities())
            {
                var activo = db.periodosEncuesta.Where(d => d.idEmpresa == oEmpresa.Id & d.cierre != null);

                if (activo.Any())
                {
                    var data = activo.OrderByDescending(d => d.id).First();

                    List<int> intermediate_list = new List<int>();

                    foreach (var d in activo.ToList())
                    {

                        intermediate_list.Add(d.year);


                    }

                    int[] arr_sample = intermediate_list.ToArray();

                    return Json(new
                    {
                        validacion = "true",
                        fecha = data.year.ToString(),
                        fechas = arr_sample
                    });
                }
                else
                {
                    return Json(new
                    {
                        validacion = "false"
                    });
                }

            }



        }

        public ActionResult Informe035(FormCollection formCollection)
        {

            var oEmpresa = (EmpresaViewModel)Session["empresa"];
            // filtros generales
            var year = formCollection["fecha"];
            int empresa = int.Parse(formCollection["idEmpresa"]);
            //conexion a la bd
            SqlConnectionStringBuilder conect = ConexionViewModel.conectar();
            int empleados = 6;
            DataResultadosViewModel dt = new DataResultadosViewModel();



            // consulta de datos generales 

            rhconEntities db = new rhconEntities();
            bool ternario = false;
            var datosEmpresa = db.empresa.Where(d => d.id == empresa).First();
            var idResponsable = db.encargadosEmpresa.Where(d => d.idEmpresa == oEmpresa.Id).First();
            var responsableNombre = db.usuario.Where(d => d.id == idResponsable.id).First();

            if (Session["empleado"] == null & Session["centro"] == null)
            {

                if (!String.IsNullOrEmpty(formCollection["centro"]))
                {
                    int ct_id = int.Parse(formCollection["centro"].ToString());
                    dt.comentarios = db.respuestaEmpleado.Where(d => d.idEmpresa == datosEmpresa.id & d.idCentroTrabajo == ct_id & d.fecha.Value.Year.ToString() == year).ToList();
                    dt.isEmpresa = true;
                }
                else
                {
                    dt.comentarios = db.respuestaEmpleado.Where(d => d.idEmpresa == datosEmpresa.id & d.fecha.Value.Year.ToString() == year).ToList();
                    dt.isEmpresa = true;
                }
            }
            else
            {
                dt.isEmpresa = false;
            }

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

            // informacion del centro de trabajo 

            if (!String.IsNullOrEmpty(formCollection["centro"]))
            {
                int centro = int.Parse(formCollection["centro"]);
                datosCentro = db.centroTrabajo.Where(d => d.id == centro).First();
                ternario = true;
                dt.ternario = true;
                dt.total_empleados = db.empleado.Where(d => d.idEmpresa == empresa & d.idCentroTrabajo == centro).Count();
                dt.actividades = datosCentro.actividades.ToString();
                dt.nombreSelect = datosCentro.nombre;
            }
            else
            {

                dt.total_empleados = db.empleado.Where(d => d.idEmpresa == empresa).Count();
                dt.ternario = false;
                dt.actividades = datosEmpresa.actividad.ToString();
                dt.nombreSelect = datosEmpresa.razonc;

            }
            var periodo = db.periodosEncuesta.Where(d => d.idEmpresa == empresa & d.year.ToString() == year.ToString()).OrderByDescending(d => d.id).First();






            var filtros = new ReporteViewModel();
            filtros.centros = formCollection["centro"];
            filtros.sexo = formCollection["sexo"];
            filtros.edad = formCollection["edad"];
            filtros.estadoCivil = formCollection["estadoCivil"];
            filtros.antiguedad = formCollection["antiguedad"];
            filtros.tiempoPuestoActual = formCollection["tiempoPuestoActual"];
            filtros.tiempoExperienciaLaboral = formCollection["tiempoExperienciaLaboral"];
            filtros.escolaridad = formCollection["escolaridad"];
            filtros.tipoJornada = formCollection["tipoJornada"];
            filtros.tipoContratacion = formCollection["tipoContratacion"];
            filtros.tipoPersonal = formCollection["tipoPersonal"];
            filtros.discapacidad = formCollection["discapacidad"];
            filtros.realizaRotacion = formCollection["realizaRotacion"];
            filtros.parteSectores = formCollection["parteSectores"];


            string cadena = "";
            if (!String.IsNullOrEmpty(filtros.centros))
                cadena = cadena + " AND t1.idCentroTrabajo =" + filtros.centros;

            if (!String.IsNullOrEmpty(filtros.sexo))
                cadena = cadena + " AND t1.sexo = '_sexo_'";
            cadena = cadena.Replace("_sexo_", filtros.sexo);
            if (!String.IsNullOrEmpty(filtros.edad))
                cadena = cadena + " AND t1.edad = '_edad_'";
            cadena = cadena.Replace("_edad_", filtros.edad);
            if (!String.IsNullOrEmpty(filtros.estadoCivil))
                cadena = cadena + " AND t1.estadoCivil = '_estado_'";
            cadena = cadena.Replace("_estado_", filtros.estadoCivil);
            if (!String.IsNullOrEmpty(filtros.antiguedad))
                cadena = cadena + " AND t1.antiguedad = '_antiguedad_'";
            cadena = cadena.Replace("_antiguedad_", filtros.antiguedad);
            if (!String.IsNullOrEmpty(filtros.tiempoPuestoActual))
                cadena = cadena + " AND t1.tiempoPuestoActual = '_puestoActual_'";
            cadena = cadena.Replace("_puestoActual_", filtros.tiempoPuestoActual);
            if (!String.IsNullOrEmpty(filtros.tiempoExperienciaLaboral))
                cadena = cadena + " AND t1.tiempoExperienciaLaboral = '_experiencia_'";
            cadena = cadena.Replace("_experiencia_", filtros.tiempoExperienciaLaboral);
            if (!String.IsNullOrEmpty(filtros.escolaridad))
                cadena = cadena + " AND t1.escolaridad = '_escolaridad_'";
            cadena = cadena.Replace("_escolaridad_", filtros.escolaridad);
            if (!String.IsNullOrEmpty(filtros.tipoJornada))
                cadena = cadena + " AND t1.tipoJornada = '_jornada_'";
            cadena = cadena.Replace("_jornada_", filtros.tipoJornada);
            if (!String.IsNullOrEmpty(filtros.tipoContratacion))
                cadena = cadena + " AND t1.tipoContratacion = '_contratacion_'";
            cadena = cadena.Replace("_contratacion_", filtros.tipoContratacion);
            if (!String.IsNullOrEmpty(filtros.tipoPersonal))
                cadena = cadena + " AND t1.tipoPersonal = '_personal_'";
            cadena = cadena.Replace("_personal_", filtros.tipoPersonal);
            if (!String.IsNullOrEmpty(filtros.discapacidad))
                cadena = cadena + " AND t1.discapacidad = '_discapacidad_'";
            cadena = cadena.Replace("_discapacidad_", filtros.discapacidad);
            if (!String.IsNullOrEmpty(filtros.realizaRotacion))
                cadena = cadena + " AND t1.realizaRotacion = '_rotacion_'";
            cadena = cadena.Replace("_rotacion_", filtros.realizaRotacion);
            if (!String.IsNullOrEmpty(filtros.parteSectores))
                cadena = cadena + " AND t1.parteSectores = '_sectores_'";
            cadena = cadena.Replace("_sectores_", filtros.parteSectores);





            using (SqlConnection connection = new SqlConnection(conect.ConnectionString))
            {
                connection.Open();


                string consulta_totalEncuesta = ConsultasViewModel.TotalRespuestasEmpleados(year, empresa.ToString(), cadena);
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
                    string consulta_categorias = ConsultasViewModel.Categorias(year, empresa.ToString(), cadena);
                    SqlCommand command_categoria = new SqlCommand(consulta_categorias, connection);

                    // consulta a la vista result_dominio
                    string consulta_dominio = ConsultasViewModel.Dominios(year, empresa.ToString(), cadena);
                    SqlCommand command_dominio = new SqlCommand(consulta_dominio, connection);

                    // consulta a la vista result_dimension
                    string consulta_dimension = ConsultasViewModel.Dimensiones(year, empresa.ToString(), cadena);
                    SqlCommand command_dimension = new SqlCommand(consulta_dimension, connection);

                    // consulta a la vista result_nom035
                    string consulta_total = ConsultasViewModel.TotalResultado(year, empresa.ToString(), cadena);
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

                    string _headerUrl = "";
                    if (ternario)
                    {
                        //Generacion del reporte 
                        _headerUrl = Url.Action("HeaderReporte", "Home", new
                        {
                            empresa = datosEmpresa.razons,
                            titulo = "IDENTIFICACIÓN Y ANÁLISIS DE LOS FACTORES DE RIESGO PSICOSOCIAL, Y EVALUACIÓN DEL ENTORNO ORGANIZACIONAL",
                            norma = "NOM-035-STPS-2018",
                            fecha = periodo.fechaFinal.ToString("dd/MM/yyyy"),
                            centro = datosCentro.nombre,
                            domicilio = datosCentro.direccion,
                            ternario = "true",
                            actividad = datosEmpresa.actividad
                        }, "https");
                    }
                    else
                    {
                        //Generacion del reporte 
                        _headerUrl = Url.Action("HeaderReporte", "Home", new
                        {
                            empresa = datosEmpresa.razons,
                            titulo = "IDENTIFICACIÓN Y ANÁLISIS DE LOS FACTORES DE RIESGO PSICOSOCIAL, Y EVALUACIÓN DEL ENTORNO ORGANIZACIONAL",
                            norma = "NOM-035-STPS-2018",
                            fecha = periodo.fechaFinal.ToString("dd/MM/yyyy"),
                            domicilio = datosEmpresa.direccion,
                            ternario = "false",
                            actividad = datosEmpresa.actividad

                        }, "https");
                    }


                    var pdf = new ViewAsPdf("~/Views/Resultados/Informe035.cshtml")
                    {
                        PageSize = Rotativa.Options.Size.A4,
                        CustomSwitches = "--header-html " + _headerUrl + " --header-spacing 5 ",
                        Model = dt
                    };


                    return pdf;

                }
                else
                {
                    TempData["Message"] = "no se encontraron datos ";
                    return Redirect("~/Factores/Resultados");
                }
            }

        }

        public ActionResult Informe025(FormCollection formCollection)
        {


            var oEmpresa = (EmpresaViewModel)Session["empresa"];
            // inicializacion de variables
            var year = formCollection["fecha"];
            int empresa = int.Parse(formCollection["idEmpresa"]);
            rhconEntities db = new rhconEntities();
            var filtros = new ReporteViewModel();
            var datosCentro = new centroTrabajo();
            DataResultadosViewModel dt = new DataResultadosViewModel();
            var datosEmpresa = db.empresa.Where(d => d.id == empresa).First();
            bool ternario = false;
            var periodo = db.periodosEncuesta.Where(d => d.idEmpresa == empresa & d.year.ToString() == year.ToString()).OrderByDescending(d => d.id).First();
            //conexion a la bd
            SqlConnectionStringBuilder conect = ConexionViewModel.conectar();

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

            // informacion del centro de trabajo 

            if (!String.IsNullOrEmpty(formCollection["centro"]))
            {
                int centro = int.Parse(formCollection["centro"]);
                datosCentro = db.centroTrabajo.Where(d => d.id == centro).First();
                ternario = true;
                dt.ternario = true;
                dt.total_empleados = db.empleado.Where(d => d.idEmpresa == empresa & d.idCentroTrabajo == centro).Count();
            }
            else
            {
                dt.total_empleados = db.empleado.Where(d => d.idEmpresa == empresa).Count();
                dt.ternario = false;
            }




            // informacion del formulario

            filtros.centros = formCollection["centro"];
            filtros.sexo = formCollection["sexo"];
            filtros.edad = formCollection["edad"];
            filtros.estadoCivil = formCollection["estadoCivil"];
            filtros.antiguedad = formCollection["antiguedad"];
            filtros.tiempoPuestoActual = formCollection["tiempoPuestoActual"];
            filtros.tiempoExperienciaLaboral = formCollection["tiempoExperienciaLaboral"];
            filtros.escolaridad = formCollection["escolaridad"];
            filtros.tipoJornada = formCollection["tipoJornada"];
            filtros.tipoContratacion = formCollection["tipoContratacion"];
            filtros.tipoPersonal = formCollection["tipoPersonal"];
            filtros.discapacidad = formCollection["discapacidad"];
            filtros.realizaRotacion = formCollection["realizaRotacion"];
            filtros.parteSectores = formCollection["parteSectores"];

            // concatenacion de filtros
            string cadena = "";
            if (!String.IsNullOrEmpty(filtros.centros))
                cadena = cadena + " AND t1.idCentroTrabajo =" + filtros.centros;
            if (!String.IsNullOrEmpty(filtros.sexo))
                cadena = cadena + " AND t1.sexo = '_sexo_'";
            cadena = cadena.Replace("_sexo_", filtros.sexo);
            if (!String.IsNullOrEmpty(filtros.edad))
                cadena = cadena + " AND t1.edad = '_edad_'";
            cadena = cadena.Replace("_edad_", filtros.edad);
            if (!String.IsNullOrEmpty(filtros.estadoCivil))
                cadena = cadena + " AND t1.estadoCivil = '_estado_'";
            cadena = cadena.Replace("_estado_", filtros.estadoCivil);
            if (!String.IsNullOrEmpty(filtros.antiguedad))
                cadena = cadena + " AND t1.antiguedad = '_antiguedad_'";
            cadena = cadena.Replace("_antiguedad_", filtros.antiguedad);
            if (!String.IsNullOrEmpty(filtros.tiempoPuestoActual))
                cadena = cadena + " AND t1.tiempoPuestoActual = '_puestoActual_'";
            cadena = cadena.Replace("_puestoActual_", filtros.tiempoPuestoActual);
            if (!String.IsNullOrEmpty(filtros.tiempoExperienciaLaboral))
                cadena = cadena + " AND t1.tiempoExperienciaLaboral = '_experiencia_'";
            cadena = cadena.Replace("_experiencia_", filtros.tiempoExperienciaLaboral);
            if (!String.IsNullOrEmpty(filtros.escolaridad))
                cadena = cadena + " AND t1.escolaridad = '_escolaridad_'";
            cadena = cadena.Replace("_escolaridad_", filtros.escolaridad);
            if (!String.IsNullOrEmpty(filtros.tipoJornada))
                cadena = cadena + " AND t1.tipoJornada = '_jornada_'";
            cadena = cadena.Replace("_jornada_", filtros.tipoJornada);
            if (!String.IsNullOrEmpty(filtros.tipoContratacion))
                cadena = cadena + " AND t1.tipoContratacion = '_contratacion_'";
            cadena = cadena.Replace("_contratacion_", filtros.tipoContratacion);
            if (!String.IsNullOrEmpty(filtros.tipoPersonal))
                cadena = cadena + " AND t1.tipoPersonal = '_personal_'";
            cadena = cadena.Replace("_personal_", filtros.tipoPersonal);
            if (!String.IsNullOrEmpty(filtros.discapacidad))
                cadena = cadena + " AND t1.discapacidad = '_discapacidad_'";
            cadena = cadena.Replace("_discapacidad_", filtros.discapacidad);
            if (!String.IsNullOrEmpty(filtros.realizaRotacion))
                cadena = cadena + " AND t1.realizaRotacion = '_rotacion_'";
            cadena = cadena.Replace("_rotacion_", filtros.realizaRotacion);
            if (!String.IsNullOrEmpty(filtros.parteSectores))
                cadena = cadena + " AND t1.parteSectores = '_sectores_'";
            cadena = cadena.Replace("_sectores_", filtros.parteSectores);





            using (SqlConnection connection = new SqlConnection(conect.ConnectionString))
            {
                connection.Open();



                string consulta_totalEncuesta = ConsultasViewModel.TotalRespuestasEmpleados(year, empresa.ToString(), cadena);
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
                    // consulta a la vista result_cabecera 
                    string consulta_cabecera = ConsultasViewModel.Cabeceras(year, empresa.ToString(), cadena);
                    SqlCommand command_cabecera = new SqlCommand(consulta_cabecera, connection);

                    // Contruccion de los elementos de las cabeceras
                    using (SqlDataReader reader = command_cabecera.ExecuteReader())
                    {
                        /*
                           devuelve un objeto de tipo dominioViewModel 
                           cada dominio contine las propiedades color,text,estado
                        */
                        dt.cabeceraVal = dt.Cabecera(reader, dt.total_encuesta);
                    }


                    string _headerUrl = "";
                    if (ternario)
                    {
                        //Generacion del reporte 
                        _headerUrl = Url.Action("headerInforme025", "Home", new
                        {
                            empresa = datosEmpresa.razons,
                            titulo = "PERCEPCIÓN DE CLIMA LABORAL Y NO DISCRIMINACIÓN",
                            norma = "NMX-R-025-SCFI-2015",
                            fecha = periodo.fechaFinal.ToString("dd/MM/yyyy"),
                            centro = datosCentro.nombre,
                            domicilio = datosCentro.direccion,
                            ternario = "true",
                            actividad = datosEmpresa.actividad,
                            responsable = responsable.nombre,
                            cedula = responsable.cedula

                        }, "https");


                    }
                    else
                    {
                        //Generacion del reporte 
                        _headerUrl = Url.Action("headerInforme025", "Home", new
                        {
                            empresa = datosEmpresa.razons,
                            titulo = "PERCEPCIÓN DE CLIMA LABORAL Y NO DISCRIMINACIÓN",
                            norma = "NMX-R-025-SCFI-2015",
                            fecha = periodo.fechaFinal.ToString("dd/MM/yyyy"),
                            domicilio = datosEmpresa.direccion,
                            ternario = "false",
                            actividad = datosEmpresa.actividad,
                            responsable = responsable.nombre,
                            cedula = responsable.cedula


                        }, "https");

                    }

                    var pdf = new ViewAsPdf("~/Views/Resultados/Informe025.cshtml")
                    {
                        PageSize = Rotativa.Options.Size.A4,
                        CustomSwitches = "--header-html " + _headerUrl + " --header-spacing 1 ",
                        Model = dt,
                        PageMargins = new Rotativa.Options.Margins(50, 10, 10, 10),

                    };



                    return pdf;

                }
                else
                {
                    TempData["Message"] = "no se encontraron datos ";
                    return Redirect("~/Factores/Resultados");
                }






            }
        }
        [ValidateInput(false)]
        public ActionResult AddSugerencia(string tipo, string cadenaSugerencia, string rol)
        {

            using (rhconEntities db = new rhconEntities())
            {
                EncuestaSugerencias nuevo = new EncuestaSugerencias();
                nuevo.categoria = tipo;
                nuevo.sugerencia = cadenaSugerencia;
                nuevo.year = short.Parse(DateTime.Now.Year.ToString());
                nuevo.rol = rol;
                db.EncuestaSugerencias.Add(nuevo);
                db.SaveChanges();
            }

            return Redirect("~/" + rol + "/Resultados");
        }




        public ActionResult Reporte035(int year)
        {
            var oEmpresa = (EmpresaViewModel)Session["empresa"];

            using (rhconEntities db = new rhconEntities())
            {
                List<resultados_por_usuario_035> respuesta = db.resultados_por_usuario_035.Where(d => d.fecha.Value.Year == year & d.idEmpresa ==
                oEmpresa.Id).ToList();

                return new ViewAsPdf("~/Views/Resultados/Reporte035.cshtml")
                {

                    PageSize = Rotativa.Options.Size.A4,

                    PageOrientation = Rotativa.Options.Orientation.Landscape,

                    Model = respuesta
                };
            }


        }


        public ActionResult Reporte025(int year)
        {
            var oEmpresa = (EmpresaViewModel)Session["empresa"];

            using (rhconEntities db = new rhconEntities())
            {
                List<resultados_por_usuario_025> respuesta = db.resultados_por_usuario_025.Where(d => d.fecha.Value.Year == year & d.idEmpresa ==
                oEmpresa.Id).ToList();

                return new ViewAsPdf("~/Views/Resultados/Reporte025.cshtml")
                {

                    PageSize = Rotativa.Options.Size.A4,

                    PageOrientation = Rotativa.Options.Orientation.Landscape,

                    Model = respuesta
                };
            }

        }



        public ActionResult AddAcciones(acciones[] elements)
        {

            var oEmpresa = (EmpresaViewModel)Session["empresa"];
            Dictionary<string, int> wordsCount = new Dictionary<string, int>();
            var ejemeplo = elements[0].dimension;

            using (rhconEntities db = new rhconEntities())
            {
                foreach (var element in elements)
                {
                        acciones addAccion = new acciones();

       
                        addAccion.accion = element.accion;
                        addAccion.color = element.color;
                        addAccion.date = element.date;
                        addAccion.descripcion = element.descripcion;
                        addAccion.dimension = element.dimension;
                        addAccion.estado = element.estado;
                        addAccion.idEmpresa = oEmpresa.Id;
                        addAccion.medidasPrevencion = element.medidasPrevencion;
                        addAccion.registro = DateTime.Now;
                        addAccion.responsable = element.responsable;
                        addAccion.status = false;
                        addAccion.tipo = "Nom-035-stps";
                        var idUser = int.Parse(element.responsable);
                        var correo = db.usuario.Where(d => d.id == idUser).First().email;

                        if (wordsCount.ContainsKey(correo))
                        {
                            wordsCount[correo] = wordsCount[correo] + 1;
                        }
                        else
                        {
                            wordsCount.Add(correo, 1);
                        }



                }

                foreach (var usuario in wordsCount)
                {
                 

                    //Envio de email al encargado de la empresa
                    string EmailORigen = "rhstackcode@gmail.com";
                    string EmailDestino = usuario.Key.ToString();
                    string pass = "stackcode1.";
                    var body = db.correos.Where(d => d.tipo == "acciones").First();
                    string mensaje = body.email.ToString();
                    mensaje = mensaje.Replace("_img_", "https://bienestarlaboral.rhcon.com.mx/Assets/img/SVG/LOGO/rhlogo.png");
                    mensaje = mensaje.Replace("_empresa_", oEmpresa.RazonSocial);
                    mensaje = mensaje.Replace("_fecha_", DateTime.Now.ToString("dd/MM/yyyy"));
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

                        throw;
                    }

                }

                return Redirect("~/Empresa/Acciones");
            }
        }

        [HttpPost]
        public ActionResult Example(usuario user)
        {
            var name = user.nombre;
            return Redirect("~/Empresa/Resultados");
        }


    }
}
