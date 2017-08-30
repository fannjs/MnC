using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Maestro
{
    /// <summary>
    /// The class contains all the establish and terminate database connection methods.
    /// </summary>
    public class dbconn
    {
        public SqlConnection conn;

        /// <summary>
        /// Opens a connection to a data source with the property settings specified by the ConnectionString.
        /// </summary>
        public void connDB()
        {
            // check from Maestro DB
            try
            {
                conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MAESTRODB"].ConnectionString);
                //cn = new SqlConnection("Data Source=localhost;database=CSDMS;User id=SSTAuto;Password=1qQA2wWS3eED;");
                conn.Open();
            }
            catch (Exception ex)
            {
                MaestroModule.CreateLog(ex.Message.ToString(), "dbconn.cs - connDB()");
                //Console.Write("Error: " + ex.Message);
                if (conn != null)
                    conn.Close();
            }
        }

        public void connDB_HTExp()
        {
            // check from Maestro DB
            try
            {
                conn = new SqlConnection(ConfigurationManager.ConnectionStrings["HTExpress"].ConnectionString);
                //cn = new SqlConnection("Data Source=localhost;database=CSDMS;User id=SSTAuto;Password=1qQA2wWS3eED;");
                conn.Open();
            }
            catch (Exception ex)
            {
                MaestroModule.CreateLog(ex.Message.ToString(), "dbconn.cs - connDB_HTExp()");
                //Console.Write("Error: " + ex.Message);
                if (conn != null)
                    conn.Close();
            }
        }

        /// <summary>
        /// Closes the connection to the data source.
        /// </summary>

        public void closeConn()
        {
            if (conn != null)
                conn.Close();
        }

        //public static SqlConnection GetDBConnection()
        //{
        //    //retrieve connection string for db from web config
        //    ///string ConnString = ConfigurationManager.ConnectionStrings["TestCenterDB"].ConnectionString;

        //    //establish a connection to the database
        //    ///Conn = new SqlConnection(ConnString);
        //    SqlConnection Conn = new SqlConnection(
        //        //"Server=localhost;Database=Dorknozzle;" +
        //        //"user id=dorknozzle; password=dorknozzle");
        //       "Server=localhost;Database=CSDMS;" +
        //       "user id=SSTAuto; password=1qQA2wWS3eED");
        //    //open db connection
        //    Conn.Open();

        //    //return db connection
        //    return Conn;
        //}
        //public static SqlConnection GetDBConnectionDorknozzle()
        //{
        //    //retrieve connection string for db from web config
        //    ///string ConnString = ConfigurationManager.ConnectionStrings["TestCenterDB"].ConnectionString;

        //    //establish a connection to the database
        //    ///Conn = new SqlConnection(ConnString);
        //    SqlConnection Conn = new SqlConnection(
        //        "Server=localhost;Database=Dorknozzle;" +
        //        "user id=dorknozzle; password=dorknozzle");

        //    Conn.Open();

        //    return Conn;
        //}

        //public static void CloseDBConnection(SqlConnection Conn)
        //{
        //    //close db connection
        //    Conn.Close();
        //}
    }
}