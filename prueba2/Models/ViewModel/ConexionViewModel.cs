using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class ConexionViewModel
    {

        public static SqlConnectionStringBuilder conectar()
        {
            //string conect = "SERVER=BIENESTARLABORA;DATABASE=rhcon;User Id=SA;Password=Hsyp*2020*;Integrated security=true";
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = "DESKTOP-D6P8TS9\\SQLEXPRESS",
                //UserID = "SA",
                //Password = "Hsyp*2020*",
                InitialCatalog = "rhcon",
                IntegratedSecurity = true


            };
            return builder;

        }

    }
}