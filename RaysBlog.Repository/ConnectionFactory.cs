using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Data.SqlClient;

namespace RaysBlog.Repository
{
    public class ConnectionFactory
    {
        public static DbConnection GetOpenConnection()
        {
            //var connection =new SqlConnection("server=(local);database=RaysBlogDb;uid=sa;pwd=oren");
            //var  connection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;User Id=root;password=12345;Database=RaysBlogDb");
            try
            {
                var connection = new MySql.Data.MySqlClient.MySqlConnection("server=localhost;uid=root;password=wsgsboy1314;Database=raysblogdb;SslMode =none;");
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
