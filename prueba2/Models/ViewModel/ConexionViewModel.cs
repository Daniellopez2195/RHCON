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
            //VMI823465
            //DESKTOP-D6P8TS9\\SQLEXPRESS
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = "DESKTOP-D6P8TS9\\SQLEXPRESS",
                UserID = "BIENESTARLABORA",
                Password = "Hsyp*2020*",
                InitialCatalog = "rhcon",    

            };
            return builder;

        }

    }
}