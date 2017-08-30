using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;
using System.Json;
using System.Web.Security;
using System.Data.SqlClient;

namespace Maestro.views.administration.userManagement
{
    public partial class userProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public class UserAccess
        {
            public string RoleName { get; set; }
            public List<Task> TaskList { get; set; }
        }
        public class Task
        {
            public string TaskName { get; set; }
        }

        [WebMethod]
        public static McObject<UserAccess> GetUserAccess()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
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

                string userId = HttpContext.Current.Session["userId"].ToString();
                cmd.CommandText = "SELECT R.ROLE_NAME, T.TASK_NAME " +
                                    "FROM M_USER_ROLE UR, M_ROLE R, M_ROLE_TASK RT, M_TASK T " +
                                    "WHERE UR.ROLE_ID = R.ROLE_ID " +
                                    "AND R.ROLE_ID = RT.ROLE_ID " +
                                    "AND RT.TASK_ID = T.TASK_ID " +
                                    "AND M_USER_ID = " + userId;
                dr = cmd.ExecuteReader();

                UserAccess UserAccess = new UserAccess();
                List<Task> TaskList = new List<Task>();
                bool firstTime = true;

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        if (firstTime)
                        {
                            firstTime = false;
                            UserAccess.RoleName = dr["ROLE_NAME"].ToString();
                        }

                        Task T = new Task();
                        T.TaskName = dr["TASK_NAME"].ToString();
                        TaskList.Add(T);
                    }

                    UserAccess.TaskList = TaskList;
                }

                return new McObject<UserAccess>(true, "Successful.", UserAccess);
            }
            catch (Exception ex)
            {
                return new McObject<UserAccess>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<Boolean> UpdateProfile(string password)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

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

                string userId = HttpContext.Current.Session["userId"].ToString();
                string encryptedPw = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
                cmd.CommandText = "UPDATE M_USER SET M_USER_PASSWORD = @userPw WHERE M_USER_ID = @userId";
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("userPw", encryptedPw);
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