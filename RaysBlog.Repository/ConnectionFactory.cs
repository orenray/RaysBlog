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
            var connection =new SqlConnection("server=(local);database=RaysBlogDb;uid=sa;pwd=oren");
            connection.Open();
            return connection;
        }
    }
}
