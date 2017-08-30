using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;
using System.Json;
using System.Web.Security;

namespace Maestro.views.administration.userManagement
{
    public partial class add : System.Web.UI.Page
    {
        private static string taskName = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
        }

        [WebMethod]
        public static McObject<Boolean> insertNewUser(string uname, string pswd, string email, string userType)
        {               
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            SqlTransaction trans = dbc.conn.BeginTransaction();
            cmd.Connection = dbc.conn;
            cmd.Transaction = trans;
            SqlDataReader dr = null;

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

                //Check if Username existed
                cmd.CommandText = "SELECT COUNT(*) FROM M_USER WHERE M_USER_NAME = @userName";
                cmd.Parameters.AddWithValue("userName", uname);
                int count = cmd.ExecuteNonQuery();
                if (count > 0)
                {
                    throw new Exception("This user name has been used. Please choose another one.");
                }

                int iUserType = int.Parse(userType);

                string userID = HttpContext.Current.Session["userId"].ToString();
                string userName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.ADD;
                int status = MCStatus.PENDING;

                //Password Encryption 
                string encryptedPw = FormsAuthentication.HashPasswordForStoringInConfigFile(pswd, "MD5");

                //string sql = "BEGIN TRANSACTION " +
                //                "BEGIN TRY " +
                //                    "DECLARE @id int " +
                //                    "INSERT INTO M_USER (M_USER_NAME, M_USER_PASSWORD, M_USER_EMAIL) " +
                //                    "OUTPUT inserted.M_USER_ID VALUES ('" + uname + "', '" + encryptedPw + "', '" + email + "') " +
                //                    "SET @id = SCOPE_IDENTITY() " +
                //                    "INSERT INTO M_USER_ROLE (M_USER_ID, ROLE_ID) VALUES (@id, " + iUserType + ") " +
                //                "COMMIT " +
                //            "END TRY " +
                //            "BEGIN CATCH " +
                //                "ROLLBACK " +
                //            "END CATCH";

                string sql = "DECLARE @id int " +
                            "INSERT INTO M_USER (M_USER_NAME, M_USER_PASSWORD, M_USER_EMAIL, CREATED_DATE) " +
                            "OUTPUT inserted.M_USER_ID VALUES ('" + uname + "', '" + encryptedPw + "', '" + email + "', GETDATE()) " +
                            "SET @id = SCOPE_IDENTITY() " +
                            "INSERT INTO M_USER_ROLE (M_USER_ID, ROLE_ID) VALUES (@id, " + iUserType + ")";
                
                cmd.CommandText = "SELECT ROLE_NAME FROM M_ROLE WHERE ROLE_ID = " + iUserType;
                dr = cmd.ExecuteReader();

                string roleName = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        roleName = dr["ROLE_NAME"].ToString();
                    }
                }
                else
                {
                    throw new Exception("Role not found.");
                }
                dr.Close();

                JsonObject NEW = new JsonObject();
                NEW.Add("User Name", uname);
                NEW.Add("Role", roleName);
                NEW.Add("E-mail", email);

                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + newdata + "', '" + action + "', "
                                    + "@sql, " + userID + ", '" + userName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                trans.Commit();
                return new McObject<bool>(true, "Successful.");
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return new McObject<bool>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }
    }
}