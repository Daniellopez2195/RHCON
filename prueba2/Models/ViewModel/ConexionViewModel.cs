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
        
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = "DESKTOP-PK2CDF5\\SQLEXPRESS",
                //UserID = "BIENESTARLABORA",
                //Password = "Hsyp*2020*",
                InitialCatalog = "rhcon",    
                IntegratedSecurity = true

            };
            return builder;

        }

    }
}