using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Maestro.Classes.MakerChecker
{
    public class MKCK_Function
    {
        public static string GetTaskID(string task_name)
        {
            string task_id = "";

            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT TASK_ID FROM M_TASK WHERE TASK_NAME = '" + task_name + "'";
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    task_id = dr["TASK_ID"].ToString();
                }
            }
            dbc.closeConn();

            return task_id;
        }
    }
}