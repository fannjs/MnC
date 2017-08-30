using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace Maestro
{
	public partial class index : System.Web.UI.Page
	{
		public static UserDetails SessionUser;
		protected void Page_Load(object sender, EventArgs e)
		{            
			if (HttpContext.Current.Session["userName"] != null)
			{
                userName.InnerText = HttpContext.Current.Session["userName"].ToString();

                SessionUser = new UserDetails()
                {
                    UserId = Session["userId"].ToString(),
                    UserName = Session["userName"].ToString(),
                    UserRole = Session["userRole"].ToString(),
                    UserAccessList = GetUserAccess(Session["userId"].ToString())
                };
			}
			else
			{
				SessionUser = null;
				Response.Redirect("../login.aspx");
			}
		}

        private List<UserAccess> GetUserAccess(string userID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            cmd.CommandText = "SELECT T.P_TASK_ID,P_TASK_NAME, T.TASK_ID, TASK_NAME, " +
                                "PERMISSION_CREATE, PERMISSION_READ, PERMISSION_UPDATE, PERMISSION_DELETE " +
                                "FROM M_USER_ROLE UR, M_ROLE_TASK RT, M_TASK T, M_TASK_MAIN TM " +
                                "WHERE UR.ROLE_ID = RT.ROLE_ID " +
                                "AND T.TASK_ID = RT.TASK_ID " +
                                "AND T.P_TASK_ID = TM.P_TASK_ID " +
                                "AND M_USER_ID = " + userID;

            SqlDataReader dr = cmd.ExecuteReader();

            List<UserAccess> UserAccessList = new List<UserAccess>();
            UserAccess UA = null;
            List<TaskPermission> TaskList = null;
            TaskPermission TP = null;
            List<int> PTaskIdList = new List<int>();
            bool firstTime = true;
            bool pending = false;

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    pending = true;

                    int PTaskId = dr.GetInt32(dr.GetOrdinal("P_TASK_ID"));
                    string PTaskName = dr["P_TASK_NAME"].ToString();

                    if (PTaskIdList.Contains(PTaskId))
                    {
                        TP = new TaskPermission();
                        TP.TaskId = dr.GetInt32(dr.GetOrdinal("TASK_ID"));
                        TP.TaskName = dr["TASK_NAME"].ToString();
                        TP.Create = dr.GetBoolean(dr.GetOrdinal("PERMISSION_CREATE"));
                        TP.Read = dr.GetBoolean(dr.GetOrdinal("PERMISSION_READ"));
                        TP.Update = dr.GetBoolean(dr.GetOrdinal("PERMISSION_UPDATE"));
                        TP.Delete = dr.GetBoolean(dr.GetOrdinal("PERMISSION_DELETE"));

                        TaskList.Add(TP);
                    }
                    else
                    {
                        if (firstTime)
                        {
                            firstTime = false;
                            PTaskIdList.Add(PTaskId);

                            UA = new UserAccess();
                            UA.PTaskId = PTaskId;
                            UA.PTaskName = PTaskName;

                            TP = new TaskPermission();
                            TP.TaskId = dr.GetInt32(dr.GetOrdinal("TASK_ID"));
                            TP.TaskName = dr["TASK_NAME"].ToString();
                            TP.Create = dr.GetBoolean(dr.GetOrdinal("PERMISSION_CREATE"));
                            TP.Read = dr.GetBoolean(dr.GetOrdinal("PERMISSION_READ"));
                            TP.Update = dr.GetBoolean(dr.GetOrdinal("PERMISSION_UPDATE"));
                            TP.Delete = dr.GetBoolean(dr.GetOrdinal("PERMISSION_DELETE"));

                            TaskList = new List<TaskPermission>();
                            TaskList.Add(TP);
                        }
                        else
                        {
                            /* Add the subtask to main task object 
                                Add main task object into a List
                                */
                            UA.SubTaskList = TaskList;
                            UserAccessList.Add(UA);


                            /*Another main task*/
                            PTaskIdList.Add(PTaskId);
                            UA = new UserAccess();
                            UA.PTaskId = PTaskId;
                            UA.PTaskName = PTaskName;

                            TP = new TaskPermission();
                            TP.TaskId = dr.GetInt32(dr.GetOrdinal("TASK_ID"));
                            TP.TaskName = dr["TASK_NAME"].ToString();
                            TP.Create = dr.GetBoolean(dr.GetOrdinal("PERMISSION_CREATE"));
                            TP.Read = dr.GetBoolean(dr.GetOrdinal("PERMISSION_READ"));
                            TP.Update = dr.GetBoolean(dr.GetOrdinal("PERMISSION_UPDATE"));
                            TP.Delete = dr.GetBoolean(dr.GetOrdinal("PERMISSION_DELETE"));

                            TaskList = new List<TaskPermission>();
                            TaskList.Add(TP);
                        }
                    }
                }

                if (pending)
                {
                    UA.SubTaskList = TaskList;
                    UserAccessList.Add(UA);
                }
            }
            dr.Close();
            dbc.closeConn();

            return UserAccessList;
        }

        [WebMethod]
        public static UserDetails GetCurrentUserDetails()
        {
            return SessionUser;
        }
	}

    public class UserDetails
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public List<UserAccess> UserAccessList { get; set; }
    }
    public class UserAccess
    {
        public int PTaskId { get; set; }
        public string PTaskName { get; set; }
        public List<TaskPermission> SubTaskList { get; set; }
    }
    public class TaskPermission
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public bool Create { get; set; }
        public bool Read { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
    }
}