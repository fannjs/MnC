using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Services;
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;
using System.Json;

namespace Maestro.views.administration.userAccess
{
    public partial class edit : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            string roleID = Request.Params["roleID"];
            hiddenRoleID.Value = roleID;
            taskName = Request.Params["task"];
        }
        public class Result
        {
            public Result(bool status, string description)
            {
                Status = status;
                Description = description;
            }
            public bool Status { get; set; }
            public string Description { get; set; }
        }
        public class UserAccessControl
        {
            public string RoleID { get; set; }
            public string RoleName { get; set; }
            public string TaskID { get; set; }
            public string TaskName { get; set; }
            public string P_Create { get; set; }
            public string P_Read { get; set; }
            public string P_Update { get; set; }
            public string P_Delete { get; set; }
            public string Role { get; set; }
        }

        [WebMethod]
        public static UserAccessControl[] getUserAccessDetails(string roleID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT R.ROLE_ID, R.ROLE_NAME, T.TASK_ID, T.TASK_NAME, RT.PERMISSION_CREATE, RT.PERMISSION_READ, " +
                                "RT.PERMISSION_UPDATE, RT.PERMISSION_DELETE " +
                                "FROM M_ROLE_TASK RT, M_ROLE R, M_TASK T " +
                                "WHERE RT.ROLE_ID = R.ROLE_ID AND RT.TASK_ID = T.TASK_ID " +
                                "AND RT.ROLE_ID = " + roleID +
                                "ORDER BY ROLE_NAME asc, ROLE_ID asc, TASK_NAME asc";

            SqlDataReader dr = cmd.ExecuteReader();
            List<UserAccessControl> uaList = new List<UserAccessControl>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    UserAccessControl uac = new UserAccessControl();
                    uac.RoleID = dr["ROLE_ID"].ToString().Trim();
                    uac.RoleName = dr["ROLE_NAME"].ToString().Trim();
                    uac.TaskID = dr["TASK_ID"].ToString().Trim();
                    uac.TaskName = dr["TASK_NAME"].ToString().Trim();
                    uac.P_Create = dr["PERMISSION_CREATE"].ToString().Trim();
                    uac.P_Read = dr["PERMISSION_READ"].ToString().Trim();
                    uac.P_Update = dr["PERMISSION_UPDATE"].ToString().Trim();
                    uac.P_Delete = dr["PERMISSION_DELETE"].ToString().Trim();

                    uaList.Add(uac);
                }
            }

            dbc.closeConn();

            return uaList.ToArray();
        }

        [WebMethod]
        public static McObject<Boolean> deleteRoleTask(string roleID, string taskID)
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
                string currentTaskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.DELETE;
                int status = MCStatus.PENDING;
                string sql = "DELETE FROM M_ROLE_TASK WHERE ROLE_ID = " + roleID + " AND TASK_ID = " + taskID;

                string newTaskName = taskName + " (Task)";

                cmd.CommandText = "SELECT ROLE_NAME, TASK_NAME FROM M_ROLE_TASK RT, M_ROLE R, M_TASK T "
                                 + "WHERE RT.ROLE_ID = R.ROLE_ID AND RT.TASK_ID = T.TASK_ID "
                                 + "AND RT.ROLE_ID = " + roleID + " AND RT.TASK_ID = " + taskID + " ";
                SqlDataReader dr = cmd.ExecuteReader();

                string oldRoleName = "", oldTaskName = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldRoleName = dr["ROLE_NAME"].ToString();
                        oldTaskName = dr["TASK_NAME"].ToString();
                    }
                }
                dr.Close();

                JsonObject OLD = new JsonObject();                
                OLD.Add("Task", oldTaskName);
                OLD.Add("From Role", oldRoleName);

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @olddata, '" + action + "', "
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

        [WebMethod]
        public static McObject<Boolean> updateRoleTask(string roleID, string roleName, object[] oldRoleTask, object[] newRoleTask)
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
                string action = MCAction.MODIFY;
                int status = MCStatus.PENDING;

                string sql = "UPDATE M_ROLE SET ROLE_NAME = '" + roleName + "' WHERE ROLE_ID = " + roleID;
                sql = sql + MCDelimiter.SQL;

                List<string> oldIdList = new List<string>();                

                foreach (Dictionary<string, object> oldTask in oldRoleTask)
                {
                    object newTaskID, create, read, update, delete;
                    oldTask.TryGetValue("id", out newTaskID);
                    oldTask.TryGetValue("c", out create);
                    oldTask.TryGetValue("r", out read);
                    oldTask.TryGetValue("u", out update);
                    oldTask.TryGetValue("d", out delete);

                    sql = sql + "UPDATE M_ROLE_TASK SET PERMISSION_CREATE = " + create + ", PERMISSION_READ = " + read + ", PERMISSION_UPDATE = " + update +
                                        ", PERMISSION_DELETE = " + delete + " WHERE ROLE_ID = " + roleID + " AND TASK_ID = " + newTaskID;
                    sql = sql + MCDelimiter.SQL;

                    oldIdList.Add(newTaskID.ToString());
                }

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

                string condition = string.Join(",", oldIdList.ToArray());
                cmd.CommandText = "SELECT RT.TASK_ID, TASK_NAME, PERMISSION_CREATE, PERMISSION_READ, PERMISSION_UPDATE, PERMISSION_DELETE " +
                                  "FROM M_ROLE_TASK RT, M_TASK T " +
                                  "WHERE RT.TASK_ID = T.TASK_ID AND ROLE_ID = " + roleID + " AND RT.TASK_ID IN("+ condition +") " +
                                  "ORDER BY TASK_NAME asc";
                dr = cmd.ExecuteReader();
                string oldOutdatedTask = "";
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        string crud = "";
                        if (dr.GetBoolean(dr.GetOrdinal("PERMISSION_CREATE")))
                        {
                            crud += "CREATE ";
                        }
                        if (dr.GetBoolean(dr.GetOrdinal("PERMISSION_READ")))
                        {
                            crud += "READ ";
                        }
                        if (dr.GetBoolean(dr.GetOrdinal("PERMISSION_UPDATE")))
                        {
                            crud += "UPDATE ";
                        }
                        if (dr.GetBoolean(dr.GetOrdinal("PERMISSION_DELETE")))
                        {
                            crud += "DELETE";
                        }
                        crud = crud.Trim().Replace(" ", ", ");
                        oldOutdatedTask += "<div>" + dr["TASK_NAME"].ToString() + " <br/>[" + crud + "]</div>";
                    }
                }
                dr.Close();

                cmd.CommandText = "SELECT TASK_ID, TASK_NAME FROM M_TASK WHERE TASK_ID IN(" + condition + ")";
                dr = cmd.ExecuteReader();
                JsonObject OldTaskJson = new JsonObject();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        OldTaskJson.Add(dr["TASK_ID"].ToString(), dr["TASK_NAME"].ToString());
                    }
                }
                dr.Close();

                string oldUpdatedTask = "";
                foreach (Dictionary<string, object> oldTask in oldRoleTask)
                {
                    object newTaskID, create, read, update, delete;
                    oldTask.TryGetValue("id", out newTaskID);
                    oldTask.TryGetValue("c", out create);
                    oldTask.TryGetValue("r", out read);
                    oldTask.TryGetValue("u", out update);
                    oldTask.TryGetValue("d", out delete);

                    string taskNameDesc = (string)OldTaskJson[newTaskID.ToString()];
                    string crud = "";
                    if (Convert.ToBoolean(int.Parse(create.ToString())))
                    {
                        crud += "CREATE ";
                    }
                    if (Convert.ToBoolean(int.Parse(read.ToString())))
                    {
                        crud += "READ ";
                    }
                    if (Convert.ToBoolean(int.Parse(update.ToString())))
                    {
                        crud += "UPDATE ";
                    }
                    if (Convert.ToBoolean(int.Parse(delete.ToString())))
                    {
                        crud += "DELETE";
                    }
                    crud = crud.Trim().Replace(" ", ", ");
                    oldUpdatedTask += "<div>" + taskNameDesc + " <br/>[" + crud + "]</div>";
                }

                string newlyAddedTask = "";
                if (newRoleTask.Length != 0)
                {
                    List<string> newIdList = new List<string>();

                    foreach (Dictionary<string, object> newTask in newRoleTask)
                    {
                        object newTaskID, create, read, update, delete;
                        newTask.TryGetValue("id", out newTaskID);
                        newTask.TryGetValue("c", out create);
                        newTask.TryGetValue("r", out read);
                        newTask.TryGetValue("u", out update);
                        newTask.TryGetValue("d", out delete);

                        sql = sql + "INSERT INTO M_ROLE_TASK VALUES(" + roleID + "," + newTaskID + "," + create + ","
                                          + read + "," + update + "," + delete + ")";
                        sql = sql + MCDelimiter.SQL;
                        newIdList.Add(newTaskID.ToString());
                    }

                    condition = string.Join(",", newIdList.ToArray());
                    cmd.CommandText = "SELECT TASK_ID, TASK_NAME FROM M_TASK WHERE TASK_ID IN(" + condition + ")";
                    dr = cmd.ExecuteReader();
                    JsonObject NewTaskJson = new JsonObject();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            NewTaskJson.Add(dr["TASK_ID"].ToString(), dr["TASK_NAME"].ToString());
                        }
                    }
                    dr.Close();
                                        
                    foreach (Dictionary<string, object> newTask in newRoleTask)
                    {
                        object newTaskID, create, read, update, delete;
                        newTask.TryGetValue("id", out newTaskID);
                        newTask.TryGetValue("c", out create);
                        newTask.TryGetValue("r", out read);
                        newTask.TryGetValue("u", out update);
                        newTask.TryGetValue("d", out delete);

                        string taskNameDesc = (string)NewTaskJson[newTaskID.ToString()];
                        string crud = "";
                        if (Convert.ToBoolean(int.Parse(create.ToString())))
                        {
                            crud += "CREATE ";
                        }
                        if (Convert.ToBoolean(int.Parse(read.ToString())))
                        {
                            crud += "READ ";
                        }
                        if (Convert.ToBoolean(int.Parse(update.ToString())))
                        {
                            crud += "UPDATE ";
                        }
                        if (Convert.ToBoolean(int.Parse(delete.ToString())))
                        {
                            crud += "DELETE";
                        }
                        crud = crud.Trim().Replace(" ", ", ");
                        newlyAddedTask += "<div>" + taskNameDesc + " <br/>[" + crud + "]</div>";
                    }
                }              
                
                JsonObject OLD = new JsonObject();
                OLD.Add("Role Name", oldRoleName);
                OLD.Add("Current Task", oldOutdatedTask);

                JsonObject NEW = new JsonObject();
                NEW.Add("Role Name", roleName);
                NEW.Add("Current Task", oldUpdatedTask);
                if (!string.IsNullOrEmpty(newlyAddedTask))
                {
                    NEW.Add("New Task", newlyAddedTask);
                }
                
                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), @olddata, @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
                cmd.Parameters.AddWithValue("newdata", newdata);
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
        public class UserAccss
        {
            public int RoleID { get; set; }
            public string RoleName { get; set; }
            public List<Task> TaskList { get; set; }
        }
        public class Task
        {
            public int TaskId { get; set; }
            public int PTaskId { get; set; }
            public bool Create { get; set; }
            public bool Read { get; set; }
            public bool Update { get; set; }
            public bool Delete { get; set; }
        }

        //Get all information
        [WebMethod]
        public static McObject<UserAccss> GetUserAccess(string roleID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {                
                cmd.CommandText = "SELECT ROLE_ID, ROLE_NAME FROM M_ROLE WHERE ROLE_ID = " + roleID;
                SqlDataReader dr = cmd.ExecuteReader();
                UserAccss UA = new UserAccss();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        UA.RoleID = dr.GetInt32(dr.GetOrdinal("ROLE_ID"));
                        UA.RoleName = dr["ROLE_NAME"].ToString();
                    }
                }
                dr.Close();

                cmd.CommandText = "SELECT T.TASK_ID, P_TASK_ID, PERMISSION_CREATE, PERMISSION_READ, PERMISSION_UPDATE, PERMISSION_DELETE " +
                                    "FROM M_ROLE_TASK RT, M_TASK T " +
                                    "WHERE RT.TASK_ID = T.TASK_ID " +
                                    "AND RT.ROLE_ID = " + roleID + " " +
                                    "ORDER BY TASK_ID asc";
                dr = cmd.ExecuteReader();
                List<Task> TaskList = new List<Task>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Task T = new Task();
                        T.TaskId = dr.GetInt32(dr.GetOrdinal("TASK_ID"));
                        T.PTaskId = dr.GetInt32(dr.GetOrdinal("P_TASK_ID"));
                        T.Create = dr.GetBoolean(dr.GetOrdinal("PERMISSION_CREATE"));
                        T.Read = dr.GetBoolean(dr.GetOrdinal("PERMISSION_READ"));
                        T.Update = dr.GetBoolean(dr.GetOrdinal("PERMISSION_UPDATE"));
                        T.Delete = dr.GetBoolean(dr.GetOrdinal("PERMISSION_DELETE"));

                        TaskList.Add(T);
                    }
                }
                dr.Close();

                UA.TaskList = TaskList;

                return new McObject<UserAccss>(true, "Successful.", UA);
            }
            catch (Exception ex)
            {
                return new McObject<UserAccss>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<Boolean> UpdateUserAccess(string roleID, string roleName, object[] TaskList, string[] CurrentTaskId)
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
                if (String.IsNullOrEmpty(taskName))
                {
                    throw new Exception("Invalid Task Name.");
                }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.MODIFY;
                int status = MCStatus.PENDING;

                string sql = "UPDATE M_ROLE SET ROLE_NAME = '" + roleName + "' WHERE ROLE_ID = " + roleID;
                sql = sql + MCDelimiter.SQL;

                cmd.CommandText = "SELECT ROLE_NAME, RT.TASK_ID, TASK_NAME, " +
                                    "PERMISSION_CREATE, PERMISSION_READ, PERMISSION_UPDATE, PERMISSION_DELETE " +
                                    "FROM M_ROLE R, M_ROLE_TASK RT, M_TASK T " +
                                    "WHERE RT.ROLE_ID = R.ROLE_ID " +
                                    "AND RT.TASK_ID = T.TASK_ID " +
                                    "AND RT.ROLE_ID = " + roleID + " " +
                                    "ORDER BY TASK_ID asc";
                dr = cmd.ExecuteReader();

                List<string> OldTaskId = new List<string>();
                JsonObject OLD = new JsonObject();
                string OldRoleName = "";
                string OldTaskDesc = "";

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        OldTaskId.Add(dr["TASK_ID"].ToString());

                        string crud = "";
                        if (dr.GetBoolean(dr.GetOrdinal("PERMISSION_CREATE")))
                        {
                            crud += "CREATE ";
                        }
                        if (dr.GetBoolean(dr.GetOrdinal("PERMISSION_READ")))
                        {
                            crud += "READ ";
                        }
                        if (dr.GetBoolean(dr.GetOrdinal("PERMISSION_UPDATE")))
                        {
                            crud += "UPDATE ";
                        }
                        if (dr.GetBoolean(dr.GetOrdinal("PERMISSION_DELETE")))
                        {
                            crud += "DELETE";
                        }
                        crud = crud.Trim().Replace(" ", ", ");
                        OldTaskDesc += "<div>" + dr["TASK_NAME"].ToString() + " <br/>[" + crud + "]</div>";

                        OldRoleName = dr["ROLE_NAME"].ToString();
                    }

                    OLD.Add("Role Name", OldRoleName);
                    OLD.Add("Task", OldTaskDesc);
                }
                else
                {
                    OLD.Add("No Task", "");
                }
                dr.Close();

                cmd.CommandText = "SELECT TASK_ID, TASK_NAME FROM M_TASK";
                dr = cmd.ExecuteReader();
                JsonObject TaskJSON = new JsonObject();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        TaskJSON.Add(dr["TASK_ID"].ToString(), dr["TASK_NAME"].ToString());
                    }
                }
                dr.Close();

                List<string> TaskToAdd = new List<string>();
                List<string> TaskToUpdate = new List<string>();
                List<string> TaskToDelete = new List<string>();

                foreach (string id in CurrentTaskId)
                {
                    if (!OldTaskId.Contains(id))
                    {
                        TaskToAdd.Add(id);
                    }
                    else
                    {
                        TaskToUpdate.Add(id);
                    }
                }
                foreach (string id in OldTaskId)
                {
                    if (!CurrentTaskId.Contains(id))
                    {
                        TaskToDelete.Add(id);
                    }
                }

                string NewTaskDesc = "";
                foreach (Dictionary<string, object> Task in TaskList)
                {
                    object TaskId, Create, Read, Update, Delete;
                    Task.TryGetValue("id", out TaskId);
                    Task.TryGetValue("c", out Create);
                    Task.TryGetValue("r", out Read);
                    Task.TryGetValue("u", out Update);
                    Task.TryGetValue("d", out Delete);

                    string TID = TaskId.ToString();
                    string taskNameDesc = (string)TaskJSON[TID];
                    string crud = "";
                    if (Convert.ToBoolean(int.Parse(Create.ToString())))
                    {
                        crud += "CREATE ";
                    }
                    if (Convert.ToBoolean(int.Parse(Read.ToString())))
                    {
                        crud += "READ ";
                    }
                    if (Convert.ToBoolean(int.Parse(Update.ToString())))
                    {
                        crud += "UPDATE ";
                    }
                    if (Convert.ToBoolean(int.Parse(Delete.ToString())))
                    {
                        crud += "DELETE";
                    }
                    crud = crud.Trim().Replace(" ", ", ");
                    NewTaskDesc += "<div>" + taskNameDesc + " <br/>[" + crud + "]</div>";


                    //Populate SQL
                    if (TaskToAdd.Contains(TID))
                    {
                        sql = sql + "INSERT INTO M_ROLE_TASK VALUES(" + roleID + "," + TID + "," + Create + ","
                                          + Read + "," + Update + "," + Delete + ", GETDATE())";
                        sql = sql + MCDelimiter.SQL;
                    }
                    else if (TaskToUpdate.Contains(TID))
                    {
                        sql = sql + "UPDATE M_ROLE_TASK SET PERMISSION_CREATE = " + Create + ", PERMISSION_READ = " + Read + ", PERMISSION_UPDATE = " + Update +
                                        ", PERMISSION_DELETE = " + Delete + " WHERE ROLE_ID = " + roleID + " AND TASK_ID = " + TID;
                        sql = sql + MCDelimiter.SQL;
                    }
                }
                
                foreach(string id in TaskToDelete)
                {
                    sql = sql + "DELETE FROM M_ROLE_TASK WHERE ROLE_ID = " + roleID + " AND TASK_ID = " + id;
                    sql = sql + MCDelimiter.SQL;
                }

                JsonObject NEW = new JsonObject();
                NEW.Add("Role Name", roleName);
                NEW.Add("Task", NewTaskDesc);

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), @olddata, @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
                cmd.Parameters.AddWithValue("newdata", newdata);
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
    }
}