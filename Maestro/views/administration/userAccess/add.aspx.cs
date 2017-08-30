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
    public partial class add : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
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
            public string Description {get;set;}
        }

        [WebMethod]
        public static Result addNewUserAccess(string roleName, object[] taskList)
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
                string action = MCAction.ADD;
                int status = MCStatus.PENDING;

                List<string> idList = new List<string>();
                
                //Formatting SQL
                string sql = "DECLARE @id int " +
                             "INSERT INTO M_ROLE VALUES('" + roleName + "','" + roleName + "',0,GETDATE())" + // 0 is false for global access (Not sure for what)
                             "SET @id = SCOPE_IDENTITY() ";
                foreach (Dictionary<string, object> task in taskList)
                {
                    object newTaskId, create, read, update, delete;
                    task.TryGetValue("id", out newTaskId);
                    task.TryGetValue("c", out create);
                    task.TryGetValue("r", out read);
                    task.TryGetValue("u", out update);
                    task.TryGetValue("d", out delete);

                    sql = sql + "INSERT INTO M_ROLE_TASK VALUES(@id," + newTaskId + "," + create + ","
                                      + read + "," + update + "," + delete + ", GETDATE())";

                    idList.Add(newTaskId.ToString());
                }                
                //END

                string condition = string.Join(",", idList.ToArray());
                cmd.CommandText = "SELECT TASK_ID, TASK_NAME FROM M_TASK WHERE TASK_ID IN(" + condition + ")";
                SqlDataReader dr = cmd.ExecuteReader();
                JsonObject JsonTask = new JsonObject();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        JsonTask.Add(dr["TASK_ID"].ToString(), dr["TASK_NAME"].ToString());
                    }
                }
                dr.Close();

                string htmlString = "";
                foreach (Dictionary<string, object> task in taskList)
                {
                    object newTaskId, create, read, update, delete;
                    task.TryGetValue("id", out newTaskId);
                    task.TryGetValue("c", out create);
                    task.TryGetValue("r", out read);
                    task.TryGetValue("u", out update);
                    task.TryGetValue("d", out delete);

                    string taskNameDesc = (string)JsonTask[newTaskId.ToString()];
                    string crud = "";
                    if (Convert.ToBoolean(int.Parse(create.ToString())))
                    {
                        crud += "CREATE ";
                    }
                    if(Convert.ToBoolean(int.Parse(read.ToString())))
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
                    htmlString += "<div>" + taskNameDesc + " <br/>[" + crud + "]</div>";
                }

                JsonObject NEW = new JsonObject();
                NEW.Add("Role", roleName);
                NEW.Add("Task(s)", htmlString);

                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("newdata", newdata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();
                
                return new Result(true, "Successful.");
            }
            catch(Exception ex)
            {
                return new Result(false, "Failed. " + Environment.NewLine + ex.ToString());
            }
            finally
            {
                dbc.closeConn();
            }
        }
    }
}