using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTestLMSProject
{
    /// <summary>
    /// A static class for storing global Unit Test Configuration Data
    /// </summary>
    static class UnitTestConfig
    {
        public static string Server { get { return "Data Source=titan.cs.weber.edu,10433"; } }
        public static string Db { get { return "Initial Catalog=LMS_ELON"; } }
        public static string User { get { return "User ID=LMS_ELON"; } }
        public static string Password { get { return "Password=$Y02X9iwsdAQ3HcDPUig"; } }

        public static string ConnectionString { get
            {
                return $"{Server};{Db};{User};{Password}";
            }
        }
    }
}
