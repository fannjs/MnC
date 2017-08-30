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

namespace Maestro.views.administration.userAccess
{
    public partial class main : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
            taskNameHidden.Value = Request.Params["task"];
        }

        public class Role
        {
            public string RoleID { get; set; }
            public string RoleName { get; set; }
        }

        public class Task
        {
            public string TaskID { get; set; }
            public string TaskName { get; set; }
        }

        public class RoleTask
        {
            public string RoleID {get;set;}
            public string TaskName {get;set;}
            public bool Create {get;set;}
            public bool Read {get;set;}
            public bool Update {get;set;}
            public bool Delete {get;set;}
        }

        [WebMethod]
        public static McObject<List<Task>> getTask()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT TASK_ID, TASK_NAME FROM M_TASK ORDER BY TASK_NAME asc";
                SqlDataReader dr = cmd.ExecuteReader();
                List<Task> taskList = new List<Task>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Task task = new Task();
                        task.TaskID = dr["TASK_ID"].ToString();
                        task.TaskName = dr["TASK_NAME"].ToString();

                        taskList.Add(task);
                    }
                }
                dr.Close();

                return new McObject<List<Task>>(true, "Successful.", taskList);
            }
            catch (Exception ex)
            {
                return new McObject<List<Task>>(false, "Failed! " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<Role>> getUserAccessList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT ROLE_ID, ROLE_NAME FROM M_ROLE ORDER BY ROLE_NAME asc";
                SqlDataReader dr = cmd.ExecuteReader();
                List<Role> roleList = new List<Role>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Role role = new Role();
                        role.RoleID = dr["ROLE_ID"].ToString();
                        role.RoleName = dr["ROLE_NAME"].ToString();

                        roleList.Add(role);
                    }
                }
                dr.Close();

                return new McObject<List<Role>>(true, "Successful.", roleList);
            }
            catch (Exception ex)
            {
                return new McObject<List<Role>>(false, "Failed! " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<RoleTask>> getRoleTaskList(string roleID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT ROLE_ID, TASK_NAME, PERMISSION_CREATE, PERMISSION_READ, PERMISSION_UPDATE , PERMISSION_DELETE " +
                                    "FROM M_ROLE_TASK RT, M_TASK T WHERE RT.TASK_ID = T.TASK_ID AND ROLE_ID = " + roleID;
                SqlDataReader dr = cmd.ExecuteReader();
                List<RoleTask> rtList = new List<RoleTask>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RoleTask rt = new RoleTask();
                        rt.RoleID = dr["ROLE_ID"].ToString();
                        rt.TaskName = dr["TASK_NAME"].ToString();
                        rt.Create = dr.GetBoolean(dr.GetOrdinal("PERMISSION_CREATE"));
                        rt.Read = dr.GetBoolean(dr.GetOrdinal("PERMISSION_READ"));
                        rt.Update = dr.GetBoolean(dr.GetOrdinal("PERMISSION_UPDATE"));
                        rt.Delete = dr.GetBoolean(dr.GetOrdinal("PERMISSION_DELETE"));

                        rtList.Add(rt);
                    }
                }
                dr.Close();

                return new McObject<List<RoleTask>>(true, "Successful.", rtList);
            }
            catch (Exception ex)
            {
                return new McObject<List<RoleTask>>(false, "Failed! " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<Boolean> deleteUserAccess(string roleID)
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
                if (String.IsNullOrEmpty(taskName))
                {
                    throw new Exception("Invalid Task Name.");
                }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.DELETE;
                int status = MCStatus.PENDING;
                string sql = "DELETE FROM M_ROLE WHERE ROLE_ID = " + roleID;

                cmd.CommandText = "SELECT ROLE_NAME FROM M_ROLE WHERE ROLE_ID = " + roleID;
                SqlDataReader dr = cmd.ExecuteReader();

                string oldRoleName = "";
                
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldRoleName = dr["ROLE_NAME"].ToString();
                    }
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("Role Name", oldRoleName);

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), @olddata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                return new McObject<Boolean>(true, "Successful.");
            }
            catch (Exception ex)
            {
                return new McObject<Boolean>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        /* NEW */
        public class MainTask
        {
            public int TaskId { get; set; }
            public string TaskName { get; set; }
            public List<SubTask> SubTaskList { get; set; }
        }
        public class SubTask
        {
            public int TaskId { get; set; }
            public string TaskName { get; set; }
        }
        /**/

        [WebMethod]
        public static McObject<List<MainTask>> GetTaskList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            SqlDataReader dr = null;

            try
            {
                cmd.CommandText = "SELECT TM.P_TASK_ID, P_TASK_NAME, TASK_ID, TASK_NAME FROM M_TASK_MAIN TM, M_TASK T " +
                                    "WHERE TM.P_TASK_ID = T.P_TASK_ID " +
                                    "ORDER BY TM.P_TASK_ID asc";
                dr = cmd.ExecuteReader();

                List<MainTask> MainTaskList = new List<MainTask>();
                List<SubTask> SubTaskList = new List<SubTask>();
                List<int> IdList = new List<int>();
                MainTask MT = null;
                bool firstTime = true;

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        int PTaskID = dr.GetInt32(dr.GetOrdinal("P_TASK_ID"));

                        if (IdList.Contains(PTaskID))
                        {
                            SubTask ST = new SubTask();
                            ST.TaskId = dr.GetInt32(dr.GetOrdinal("TASK_ID"));
                            ST.TaskName = dr["TASK_NAME"].ToString();

                            SubTaskList.Add(ST);
                        }
                        else
                        {
                            if (firstTime)
                            {
                                firstTime = false;

                                IdList.Add(PTaskID);
                                MT = new MainTask();
                                MT.TaskId = PTaskID;
                                MT.TaskName = dr["P_TASK_NAME"].ToString();

                                SubTask ST = new SubTask();
                                ST.TaskId = dr.GetInt32(dr.GetOrdinal("TASK_ID"));
                                ST.TaskName = dr["TASK_NAME"].ToString();

                                SubTaskList.Add(ST);
                            }
                            else
                            {
                                MT.SubTaskList = SubTaskList;
                                MainTaskList.Add(MT);

                                //New Parent Task
                                IdList.Add(PTaskID);
                                MT = new MainTask();
                                MT.TaskId = PTaskID;
                                MT.TaskName = dr["P_TASK_NAME"].ToString();
                                                                
                                SubTask ST = new SubTask();
                                ST.TaskId = dr.GetInt32(dr.GetOrdinal("TASK_ID"));
                                ST.TaskName = dr["TASK_NAME"].ToString();

                                SubTaskList = new List<SubTask>();
                                SubTaskList.Add(ST);
                            }  
                        }
                    }

                    MT.SubTaskList = SubTaskList;
                    MainTaskList.Add(MT);
                }

                return new McObject<List<MainTask>>(true, "Successful.", MainTaskList);
            }
            catch (Exception ex)
            {
                return new McObject<List<MainTask>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }
    }
}