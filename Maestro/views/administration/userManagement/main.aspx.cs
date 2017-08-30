using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;
using System.Json;

namespace Maestro.views.administration.userManagement
{
    public partial class main : System.Web.UI.Page
    {
        private static string taskName = "";
 
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
            taskNameHidden.Value = Request.Params["task"];
        }

        public class UserRole
        {
            public string RoleID { get; set; }
            public string RoleName { get; set; }
        }

        [WebMethod]
        public static UserRole[] loadUserRole()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            try
            {
                cmd.CommandText = "SELECT ROLE_ID, ROLE_NAME FROM M_ROLE";

                SqlDataReader dr = cmd.ExecuteReader();
                List<UserRole> urList = new List<UserRole>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        UserRole ur = new UserRole();
                        ur.RoleID = dr["ROLE_ID"].ToString();
                        ur.RoleName = dr["ROLE_NAME"].ToString();
                        urList.Add(ur);
                    }
                }
                return urList.ToArray();
            }catch(Exception ex){
                return null;
            }finally{
                if (dbc.conn != null){
                    dbc.closeConn();
                }
            }
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static UserDetails[] getUserDetails()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            try
            {
                cmd.CommandText = "SELECT U.M_USER_ID, U.M_USER_NAME, U.M_USER_EMAIL, R.ROLE_NAME " +
                                    "FROM M_USER_ROLE UR, M_USER U, M_ROLE R " +
                                    "WHERE UR.M_USER_ID = U.M_USER_ID " +
                                    "AND UR.ROLE_ID = R.ROLE_ID";

                SqlDataReader dr = cmd.ExecuteReader();
                List<UserDetails> details = new List<UserDetails>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        UserDetails user = new UserDetails();
                        user.UserId = dr["M_USER_ID"].ToString();
                        user.UserName = dr["M_USER_NAME"].ToString();
                        user.RoleName = dr["ROLE_NAME"].ToString();
                        user.UserEmail = dr["M_USER_EMAIL"].ToString();
                        details.Add(user);
                    }
                }
                dr.Close();
                return details.ToArray();
            }catch(Exception ex){
                return null;
            }finally{
                if (dbc.conn != null){
                    dbc.closeConn();
                }
            }
        }

        public class UserDetails
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string RoleName { get; set; }
            public string UserEmail { get; set; }
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Boolean deleteUser(string userID)
        {
            string delimiter = MCDelimiter.SQL;      

            try
            {
                if (HttpContext.Current.Session["userId"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }
                if (HttpContext.Current.Session["userName"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }
                if (String.IsNullOrEmpty(taskName))
                {
                    throw new Exception("Invalid Task Name.");
                }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.DELETE;
                int status = MCStatus.PENDING;

                /*
                 * First: Delete from M_USER_ROLE
                 * Second: Delete from M_USER
                 */
                string sql = "DELETE M_USER_ROLE WHERE M_USER_ID = " + userID;
                sql = sql + delimiter;
                sql = sql + "DELETE M_USER WHERE M_USER_ID = " + userID;

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT U.M_USER_ID, U.M_USER_NAME, U.M_USER_EMAIL, R.ROLE_NAME "
                                + "FROM M_USER U, M_USER_ROLE UR, M_ROLE R "
                                + "WHERE U.M_USER_ID = UR.M_USER_ID AND UR.ROLE_ID = R.ROLE_ID AND U.M_USER_ID = " + userID;
                SqlDataReader dr = cmd.ExecuteReader();

                string oldUserName = "";
                string oldEmail = "";
                string oldRoleName = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldUserName = dr["M_USER_NAME"].ToString();
                        oldEmail = dr["M_USER_EMAIL"].ToString();
                        oldRoleName = dr["ROLE_NAME"].ToString();
                    }
                }
                else
                {
                    return false;
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("User Name", oldUserName);
                OLD.Add("Role", oldRoleName);
                OLD.Add("E-mail", oldEmail);

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + olddata + "','" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();
                dbc.closeConn();

                return true;
            }
            catch (Exception ex)
            {
                //log
                return false;
            }
        }
    }
}