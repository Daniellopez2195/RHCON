using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class ConsultasViewModel
    {

        public static string  GraficarNom035(string categoria, int idEmpresa, string year, string idCentro = "")
        {
            string  condicion = "";
            if (!string.IsNullOrEmpty(idCentro))
            {
                condicion = "and idCentroTrabajo = " + idCentro;
            }
            string queryString = "select SUM(valor) suma  from dbo.resultados_nom035 " +
                     "where categoria_descripcion = '"+categoria+"'  and idEmpresa = "+idEmpresa+condicion+" and fecha between '"+year+ "-01-01T00:00:00' and '" + year+ "-12-31T00:00:00' group by idResuestaEmpleado";

            return queryString;
        }

        public static string GraficarNom025(string categoria, int idEmpresa, string year, string idCentro = "")
        {
            string condicion = "";
            if (!string.IsNullOrEmpty(idCentro))
            {
                condicion = "and idCentroTrabajo = " + idCentro;
            }
            string queryString = "select SUM(valor) suma  from dbo.resultados_nom025 where cabecera_descripcion like '%"+categoria+"%' and  idEmpresa = "+idEmpresa + condicion+" and fecha between '" + year+ "-01-01T00:00:00' and '" + year+ "-12-31T00:00:00' group by idResuestaEmpleado";

            return queryString;
        }

        public static string Categorias(string year, string idEmpresa, string filtros = "")
        {
            string queryString = "select sum(suma) valor, categoria_descripcion from dbo.result_categoria " +
                "where idResuestaEmpleado in  (select distinct(t1.id) from dbo.respuestaEmpleado t1 join dbo.resultados_nom035 t2 on t1.id = t2.idResuestaEmpleado" +
                " where t1.idEmpresa = "+idEmpresa+" and t2.year = "+year+ filtros+ ") and year = "+year+"  group by categoria_descripcion";
       

            return queryString;

        }

        public static string Dominios(string year, string idEmpresa, string filtros = "")
        {
            string queryString = "select sum(suma) valor, dominio_descripcion from dbo.result_dominio  where " +
                "idResuestaEmpleado in  (select distinct(t1.id) from dbo.respuestaEmpleado t1 join dbo.resultados_nom035 t2 on t1.id = t2.idResuestaEmpleado  " +
                "where t1.idEmpresa = "+idEmpresa+" and t2.year = "+year+filtros+") and year = "+year+"  group by dominio_descripcion";


            return queryString;

        }

        public static string Dimensiones(string year, string idEmpresa, string filtros = "")
        {
            string queryString = "select sum(suma) valor, dimencion_descripcion,idDimencion from dbo.result_dimension  where " +
                "idResuestaEmpleado in  (select distinct(t1.id) from dbo.respuestaEmpleado t1 join dbo.resultados_nom035 t2 on t1.id = t2.idResuestaEmpleado  " +
                "where t1.idEmpresa = " + idEmpresa + " and t2.year = " + year + filtros + ") and year = " + year + "  group by dimencion_descripcion,idDimencion";


            return queryString;

        }

        public static string Cabeceras(string year, string idEmpresa, string filtros = "")
        {
            string queryString = "select sum(suma) valor, cabecera_descripcion from dbo.result_cabecera  where  " +
                "idResuestaEmpleado in  (select distinct(t1.id) from dbo.respuestaEmpleado t1 join dbo.resultados_nom025 t2 on t1.id = t2.idResuestaEmpleado" +
                " where t1.idEmpresa = "+idEmpresa+" and t2.year = "+year+filtros+") and year = "+year+" group by cabecera_descripcion";

            return queryString;

        }

        public static string TotalRespuestasEmpleados(string year, string idEmpresa, string filtros = "")
        {
            string queryString = "select count(distinct(id)) total from dbo.respuestaEmpleado " +
                "where id in (select id from dbo.respuestaEmpleado t1 where YEAR(t1.fecha) = "+year+" and t1.idEmpresa = "+idEmpresa+ filtros+")";

            return queryString;

        }

        public static string TotalResultado(string year, string idEmpresa, string filtros = "")
        {
            string queryString = "select sum(valor) suma from dbo.resultados_nom035 t1 where idEmpresa = "+idEmpresa+" and year = "+year + filtros+" ";

            return queryString;

        }
    }
}