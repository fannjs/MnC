using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data.SqlClient;
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;
using System.Json;
using System.Web.Security;

namespace Maestro.views.administration.userManagement
{
    public partial class edit : System.Web.UI.Page
    {
        private static string taskName = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
        }

        public class User
        {
            public string UserID { get; set; }
            public string UserName { get; set; }
            public string UserPassword { get; set; }
            public string Email { get; set; }
            public Role UserRole { get; set; }
        }

        public class Role
        {
            public string RoleID { get; set; }
            public string RoleName { get; set; }
        }

        [WebMethod]
        public static McObject<User> getUserDetail(string userID)
        {
            try
            {
                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT U.M_USER_ID, U.M_USER_NAME, U.M_USER_PASSWORD, U.M_USER_EMAIL, UR.ROLE_ID "
                                 + "FROM M_USER U, M_USER_ROLE UR WHERE U.M_USER_ID = UR.M_USER_ID AND U.M_USER_ID = " + userID;
                SqlDataReader dr = cmd.ExecuteReader();
                User u = new User();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        u.UserID = dr["M_USER_ID"].ToString();
                        u.UserName = dr["M_USER_NAME"].ToString();
                        u.Email = dr["M_USER_EMAIL"].ToString();

                        Role r = new Role();
                        r.RoleID = dr["ROLE_ID"].ToString();

                        u.UserRole = r;
                    }
                }

                dr.Close();
                dbc.closeConn();

                return new McObject<User>(true, "Successful.", u);
            }
            catch (Exception ex)
            {
                return new McObject<User>(false, ex.Message);
            }
        }

        [WebMethod]
        public static McObject<Boolean> updateUser(string userID, string uname, string pswd, string email, string userType, string userTypeDesc)
        {            
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
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
                string action = MCAction.MODIFY;
                int status = MCStatus.PENDING;
                string sql = "";

                if (string.IsNullOrEmpty(pswd))
                {
                    sql = "UPDATE M_USER SET M_USER_EMAIL = '" + email + "' WHERE M_USER_ID = " + userID;
                }
                else
                {
                    string encryptedPw = FormsAuthentication.HashPasswordForStoringInConfigFile(pswd, "MD5");
                    sql = "UPDATE M_USER SET M_USER_PASSWORD = '" + encryptedPw + "', M_USER_EMAIL = '" + email + "' WHERE M_USER_ID = " + userID;
                }
                sql = sql + delimiter;
                sql = sql + "UPDATE M_USER_ROLE SET ROLE_ID = " + userType + " WHERE M_USER_ID = " + userID;

                
                cmd.CommandText = "SELECT U.M_USER_ID, U.M_USER_NAME, U.M_USER_PASSWORD, U.M_USER_EMAIL, R.ROLE_NAME "
                                 + "FROM M_USER U, M_USER_ROLE UR, M_ROLE R "
                                 + "WHERE U.M_USER_ID = UR.M_USER_ID AND UR.ROLE_ID = R.ROLE_ID AND U.M_USER_ID = " + userID;
                SqlDataReader dr = cmd.ExecuteReader();

                string oldUserName = "";
                string oldPassword = "";
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
                    throw new Exception("User not found.");
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("User Name", oldUserName);
                OLD.Add("Role", oldRoleName);
                OLD.Add("E-mail", oldEmail);

                JsonObject NEW = new JsonObject();
                NEW.Add("User Name", uname);
                NEW.Add("Role", userTypeDesc);
                NEW.Add("E-mail", email);

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + olddata + "', '" + newdata + "', '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();
                
                return new McObject<bool>(true, "Successful.");
            }
            catch (Exception ex)
            {
                return new McObject<bool>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }
    }
}