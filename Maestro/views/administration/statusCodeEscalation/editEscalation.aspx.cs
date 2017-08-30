using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;
using System.Json;
using System.Data.SqlClient;
using System.Web.Services;

namespace Maestro.views.administration.statusCodeEscalation
{
    public partial class editEscalation : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
            selectKioskType.InnerText = Request.Params["KioskType"];
            groupIdHidden.Value = Request.Params["GroupId"];
        }

        public class StatusCodeEscalation
        {
            public List<StatusCode> CodeList { get; set; }
            public List<EscalationUser> UserList { get; set; }
        }

        public class EscalationUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string UserEmail { get; set; }
            public string UserPhone { get; set; }
        }

        public class StatusCode
        {
            public string Code { get; set; }
        }

        [WebMethod]
        public static McObject<StatusCodeEscalation> GetEscalation(string GroupId)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            SqlDataReader dr = null;

            try
            {
                /* Load Code List */
                cmd.CommandText = "SELECT M_CODE FROM M_CODES WHERE M_GROUP_ID = " + GroupId;
                dr = cmd.ExecuteReader();
                List<StatusCode> CodeList = new List<StatusCode>();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        StatusCode MCode = new StatusCode();
                        MCode.Code = dr["M_CODE"].ToString();

                        CodeList.Add(MCode);
                    }
                }
                dr.Close();

                /* Load User List */
                cmd.CommandText = "SELECT M_ESCALATE_TYPE, EU.M_USER_ID, M_USER_NAME, M_USER_EMAIL, M_USER_PHONE FROM M_ESCALATION_USERS EU, M_ESCALATION_LIST EL " +
                                    "WHERE EL.M_USER_ID = EU.M_USER_ID AND EL.M_GROUP_ID = " + GroupId;
                dr = cmd.ExecuteReader();
                List<int> ids = new List<int>();
                EscalationUser User = new EscalationUser();
                List<EscalationUser> UserList = new List<EscalationUser>();
                bool firstTime = true;

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        int escalationType = dr.GetInt16(dr.GetOrdinal("M_ESCALATE_TYPE"));

                        int UserId = dr.GetInt16(dr.GetOrdinal("M_USER_ID"));
                        string UserName = dr["M_USER_NAME"].ToString();
                        string UserEmail = dr["M_USER_EMAIL"].ToString();
                        string UserPhone = dr["M_USER_PHONE"].ToString();

                        if (ids.Contains(UserId))
                        {
                            if (escalationType.Equals(EscalateType.EMAIL))
                            {
                                User.UserEmail = UserEmail;
                            }
                            else if (escalationType.Equals(EscalateType.SMS))
                            {
                                User.UserPhone = UserPhone;
                            }
                        }
                        else
                        {
                            if (firstTime)
                            {
                                firstTime = false;
                            }
                            else
                            {
                                UserList.Add(User);
                            }

                            ids.Add(UserId);
                            User = new EscalationUser();
                            User.UserId = UserId;
                            User.UserName = UserName;

                            if (escalationType.Equals(EscalateType.EMAIL))
                            {
                                User.UserEmail = UserEmail;
                            }
                            else if (escalationType.Equals(EscalateType.SMS))
                            {
                                User.UserPhone = UserPhone;
                            }
                        }
                    }
                    UserList.Add(User);
                }
                dr.Close();

                StatusCodeEscalation sce = new StatusCodeEscalation();
                sce.CodeList = CodeList;
                sce.UserList = UserList;

                return new McObject<StatusCodeEscalation>(true, "Successful. ", sce);
            }
            catch (Exception ex)
            {
                return new McObject<StatusCodeEscalation>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<Boolean> UnassignStatusCode(string KioskType, string GroupId, string Code)
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
                string action = MCAction.DELETE;
                int status = MCStatus.PENDING;
                string sql = "UPDATE M_CODES SET M_GROUP_ID = NULL WHERE M_CODE = '" + Code + "'";

                string newTaskName = taskName + " (Escalation Status Code)";

                /*
                cmd.CommandText = "SELECT M_ESCALATE_TYPE, EU.M_USER_ID, M_USER_NAME, M_USER_EMAIL, M_USER_PHONE FROM M_ESCALATION_USERS EU, M_ESCALATION_LIST EL " +
                                    "WHERE EL.M_USER_ID = EU.M_USER_ID AND EL.M_GROUP_ID = " + GroupId;
                dr = cmd.ExecuteReader();
                List<int> ids = new List<int>();
                string htmlUsers = "";
                string userDesc = "";
                bool firstTime = true;
                bool pending = true;

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        int escalationType = dr.GetInt16(dr.GetOrdinal("M_ESCALATE_TYPE"));

                        int UserId = dr.GetInt16(dr.GetOrdinal("M_USER_ID"));
                        string UserName = dr["M_USER_NAME"].ToString();
                        string UserEmail = dr["M_USER_EMAIL"].ToString();
                        string UserPhone = dr["M_USER_PHONE"].ToString();

                        pending = true;

                        if (ids.Contains(UserId))
                        {
                            if (escalationType.Equals(EscalateType.EMAIL))
                            {
                                userDesc = userDesc + " &#060;" + UserEmail + "&#062;";
                            }
                            else if (escalationType.Equals(EscalateType.SMS))
                            {
                                userDesc = userDesc + " &#060;" + UserPhone + "&#062;";
                            }
                        }
                        else
                        {
                            if (firstTime)
                            {
                                firstTime = false;
                            }
                            else
                            {
                                htmlUsers = htmlUsers + "<div>" + userDesc + "</div>";

                                userDesc = "";

                                pending = false;
                            }

                            ids.Add(UserId);
                            userDesc = UserName;

                            if (escalationType.Equals(EscalateType.EMAIL))
                            {
                                userDesc = userDesc + " &#060;" + UserEmail + "&#062;";
                            }
                            else if (escalationType.Equals(EscalateType.SMS))
                            {
                                userDesc = userDesc + " &#060;" + UserPhone + "&#062;";
                            }
                        }
                    }

                    if (pending)
                    {
                        htmlUsers = htmlUsers + "<div>" + userDesc + "</div>";
                    }
                }
                dr.Close();
                */
                JsonObject OLD = new JsonObject();
                OLD.Add("Kiosk Type", KioskType);
                OLD.Add("Escalation Code", Code);
                //OLD.Add("Escalation User", htmlUsers);

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                       + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @olddata,'" + action + "', "
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
        public static McObject<Boolean> UnassignUser(string KioskType, string GroupId, string UserId)
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
                string action = MCAction.DELETE;
                int status = MCStatus.PENDING;
                string sql = "DELETE FROM M_ESCALATION_LIST WHERE M_GROUP_ID = " + GroupId + " AND M_USER_ID = " + UserId;

                string newTaskName = taskName + " (Escalation User)";

                cmd.CommandText = "SELECT M_ESCALATE_TYPE, EU.M_USER_ID, M_USER_NAME, M_USER_EMAIL, M_USER_PHONE FROM M_ESCALATION_USERS EU, M_ESCALATION_LIST EL " +
                                    "WHERE EL.M_USER_ID = EU.M_USER_ID AND EL.M_GROUP_ID = " + GroupId + " AND M_USER_ID = " + UserId;
                dr = cmd.ExecuteReader();
                List<int> ids = new List<int>();
                string htmlUsers = "";
                string userDesc = "";
                bool firstTime = true;
                bool pending = true;

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        int escalationType = dr.GetInt16(dr.GetOrdinal("M_ESCALATE_TYPE"));

                        int uid = dr.GetInt16(dr.GetOrdinal("M_USER_ID"));
                        string UserName = dr["M_USER_NAME"].ToString();
                        string UserEmail = dr["M_USER_EMAIL"].ToString();
                        string UserPhone = dr["M_USER_PHONE"].ToString();

                        pending = true;

                        if (ids.Contains(uid))
                        {
                            if (escalationType.Equals(EscalateType.EMAIL))
                            {
                                userDesc = userDesc + " &#060;" + UserEmail + "&#062;";
                            }
                            else if (escalationType.Equals(EscalateType.SMS))
                            {
                                userDesc = userDesc + " &#060;" + UserPhone + "&#062;";
                            }
                        }
                        else
                        {
                            if (firstTime)
                            {
                                firstTime = false;
                            }
                            else
                            {
                                htmlUsers = htmlUsers + "<div>" + userDesc + "</div>";

                                userDesc = "";

                                pending = false;
                            }

                            ids.Add(uid);
                            userDesc = UserName;

                            if (escalationType.Equals(EscalateType.EMAIL))
                            {
                                userDesc = userDesc + " &#060;" + UserEmail + "&#062;";
                            }
                            else if (escalationType.Equals(EscalateType.SMS))
                            {
                                userDesc = userDesc + " &#060;" + UserPhone + "&#062;";
                            }
                        }
                    }

                    if (pending)
                    {
                        htmlUsers = htmlUsers + "<div>" + userDesc + "</div>";
                    }
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("Kiosk Type", KioskType);
                OLD.Add("Escalation Code", "");
                OLD.Add("Escalation User", htmlUsers);

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                       + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @olddata,'" + action + "', "
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
        public static McObject<Boolean> UpdateEscalation(string KioskType, string GroupId, string[] NewCodeList, string[] NewEmailList, string[] NewPhoneList)
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

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.MODIFY;
                int status = MCStatus.PENDING;
                string sql = "";
                string rejectSql = "";

                cmd.CommandText = "SELECT M_CODE FROM M_CODES WHERE M_GROUP_ID = " + GroupId;
                dr = cmd.ExecuteReader();
                List<string> OldCodeList = new List<string>();
                
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        //This can be reuse.
                        // 1 - Collect current M_CODE to do comparison to get which code need to add group or remove group
                        // 2 - For maker checker's view
                        OldCodeList.Add(dr["M_CODE"].ToString());
                    }
                }
                dr.Close();

                cmd.CommandText = "SELECT M_ESCALATE_TYPE, U.M_USER_ID, M_USER_NAME, M_USER_EMAIL, M_USER_PHONE " +
                                    "FROM M_ESCALATION_LIST EL, M_ESCALATION_USERS U " +
                                    "WHERE EL.M_USER_ID = U.M_USER_ID AND M_GROUP_ID = " + GroupId;
                dr = cmd.ExecuteReader();
                List<string> OldEmailList = new List<string>();
                List<string> OldPhoneList = new List<string>();
                List<string> UserIdList = new List<string>();
                string HTML_oldUsers = "";
                string userDesc = "";
                bool firstTime = true;

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        int escalateType = dr.GetInt16(dr.GetOrdinal("M_ESCALATE_TYPE"));
                        string UserId = dr["M_USER_ID"].ToString();
                        string UserName = dr["M_USER_NAME"].ToString();
                        string UserEmail = dr["M_USER_EMAIL"].ToString();
                        string UserPhone = dr["M_USER_PHONE"].ToString();

                        //This part is to collect current users in the group
                        //To compare with the new users list and the user to add or delete
                        if (escalateType.Equals(EscalateType.EMAIL))
                        {
                            OldEmailList.Add(UserId);
                        }
                        else if (escalateType.Equals(EscalateType.SMS))
                        {
                            OldPhoneList.Add(UserId);
                        }

                        //This part is to collect data and populate it nicely to show to Maker/Checker
                        if (UserIdList.Contains(UserId))
                        {
                            if (escalateType.Equals(EscalateType.EMAIL))
                            {
                                userDesc = userDesc + " &#060;" + UserEmail + "&#062;";
                            }
                            else if (escalateType.Equals(EscalateType.SMS))
                            {
                                userDesc = userDesc + " &#060;" + UserPhone + "&#062;";
                            }
                        }
                        else
                        {
                            if (firstTime)
                            {
                                firstTime = false;
                            }
                            else
                            {
                                HTML_oldUsers = HTML_oldUsers + "<div>" + userDesc + "</div>";
                                userDesc = "";
                            }

                            UserIdList.Add(UserId);
                            userDesc = UserName;

                            if (escalateType.Equals(EscalateType.EMAIL))
                            {
                                userDesc = userDesc + " &#060;" + UserEmail + "&#062;";
                            }
                            else if (escalateType.Equals(EscalateType.SMS))
                            {
                                userDesc = userDesc + " &#060;" + UserPhone + "&#062;";
                            }
                        }
                    }
                    HTML_oldUsers = HTML_oldUsers + "<div>" + userDesc + "</div>";
                    userDesc = "";
                }
                dr.Close();
                
                List<string> CodeToAdd = new List<string>();
                List<string> CodeToDelete = new List<string>();
                List<string> EmailToAdd = new List<string>();
                List<string> EmailToDelete = new List<string>();
                List<string> PhoneToAdd = new List<string>();
                List<string> PhoneToDelete = new List<string>();
                int i = 0;

                //Check Status Code List (What to Add & Delete)
                for (i = 0; i < OldCodeList.Count; i++)
                {
                    if (!NewCodeList.Contains(OldCodeList[i]))
                    {
                        CodeToDelete.Add(OldCodeList[i]);                        
                    }
                }
                for (i = 0; i < NewCodeList.Length; i++)
                {
                    if (!OldCodeList.Contains(NewCodeList[i]))
                    {
                        CodeToAdd.Add(NewCodeList[i]);
                    }
                }

                List<string> NewCodeToDelete = new List<string>();
                List<string> NewCodeToAdd = new List<string>();

                for (i = 0; i < CodeToDelete.Count; i++)
                {
                    string formatted = "'" + CodeToDelete[i] + "'";
                    NewCodeToDelete.Add(formatted);
                }
                for (i = 0; i < CodeToAdd.Count; i++)
                {
                    string formatted = "'" + CodeToAdd[i] + "'";
                    NewCodeToAdd.Add(formatted);
                }

                string CodeToDeleteCondition = string.Join(",", NewCodeToDelete.ToArray());
                string CodeToAddCondition = string.Join(",", NewCodeToAdd.ToArray());
                if(!string.IsNullOrEmpty(CodeToDeleteCondition))
                {                    
                    sql = sql + "UPDATE M_CODES SET M_GROUP_ID = NULL WHERE M_CODE IN(" + CodeToDeleteCondition + ")";
                    sql = sql + MCDelimiter.SQL;
                }

                if (!string.IsNullOrEmpty(CodeToAddCondition))
                {                    
                    sql = sql + "UPDATE M_CODES SET M_GROUP_ID = " + GroupId + ", M_IS_PENDING = 0 WHERE M_CODE IN(" + CodeToAddCondition + ")";
                    sql = sql + MCDelimiter.SQL;
                }

                //Check Email List (What to Add & Delete)
                for (i = 0; i < OldEmailList.Count; i++)
                {
                    if (!NewEmailList.Contains(OldEmailList[i]))
                    {
                        //EmailToDelete.Add(OldEmailList[i]);
                        sql = sql + "DELETE M_ESCALATION_LIST WHERE M_GROUP_ID = " + GroupId + " AND M_USER_ID = " + OldEmailList[i] + " AND M_ESCALATE_TYPE = " + EscalateType.EMAIL;
                        sql = sql + MCDelimiter.SQL;
                    }
                }
                for (i = 0; i < NewEmailList.Length; i++)
                {
                    if (!OldEmailList.Contains(NewEmailList[i]))
                    {
                        //EmailToAdd.Add(NewEmailList[i]);
                        sql = sql + "INSERT INTO M_ESCALATION_LIST VALUES (" + EscalateType.EMAIL + ", " + GroupId + ", " + NewEmailList[i] + ")";
                        sql = sql + MCDelimiter.SQL;
                    }
                }

                //Check Phone List (What to Add & Delete)
                for (i = 0; i < OldPhoneList.Count; i++)
                {
                    if (!NewPhoneList.Contains(OldPhoneList[i]))
                    {
                        //PhoneToDelete.Add(OldPhoneList[i]);
                        sql = sql + "DELETE M_ESCALATION_LIST WHERE M_GROUP_ID = " + GroupId + " AND M_USER_ID = " + OldPhoneList[i] + " AND M_ESCALATE_TYPE = " + EscalateType.SMS;
                        sql = sql + MCDelimiter.SQL;
                    }
                }
                for (i = 0; i < NewPhoneList.Length; i++)
                {
                    if (!OldPhoneList.Contains(NewPhoneList[i]))
                    {
                        //PhoneToAdd.Add(NewPhoneList[i]);
                        sql = sql + "INSERT INTO M_ESCALATION_LIST VALUES (" + EscalateType.SMS + ", " + GroupId + ", " + NewPhoneList[i] + ")";
                        sql = sql + MCDelimiter.SQL;
                    }
                }

                string[] NewUserArray = NewEmailList.Union(NewPhoneList).ToArray();
                string NewUserCondition = string.Join(",", NewUserArray.ToArray());
                cmd.CommandText = "SELECT M_USER_ID, M_USER_NAME, M_USER_PHONE, M_USER_EMAIL FROM M_ESCALATION_USERS " +
                                    "WHERE M_USER_ID IN (" + NewUserCondition + ")";
                dr = cmd.ExecuteReader();
                string HTML_newUsers = "";
                
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        string userId = dr["M_USER_ID"].ToString();
                        string userName = dr["M_USER_NAME"].ToString();
                        string userPhone = dr["M_USER_PHONE"].ToString();
                        string userEmail = dr["M_USER_EMAIL"].ToString();

                        string newUserDesc = userName;

                        if (NewEmailList.Contains(userId))
                        {
                            newUserDesc = newUserDesc + " &#060;" + userEmail + "&#062;";
                        }
                        if (NewPhoneList.Contains(userId))
                        {
                            newUserDesc = newUserDesc + " &#060;" + userPhone + "&#062;";
                        }

                        HTML_newUsers = HTML_newUsers + "<div>" + newUserDesc + "</div>";
                    }
                }
                dr.Close();

                //All ready to go. Now populate data nicely and insert to MKCK table

                string OldCodeDesc = string.Join(",", OldCodeList.ToArray());
                string NewCodeDesc = string.Join(",", NewCodeList);

                JsonObject OLD = new JsonObject();
                OLD.Add("Kiosk Type", KioskType);
                OLD.Add("Status Code", OldCodeDesc);
                OLD.Add("Escalation User", HTML_oldUsers);

                JsonObject NEW = new JsonObject();
                NEW.Add("Kiosk Type", KioskType);
                NEW.Add("Status Code", NewCodeDesc);
                NEW.Add("Escalation User", HTML_newUsers);

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                /*
                * Simple method for concurrency control
                * Not sure this is the way of doing concurrency control, will come back to improve
                * 
                * Temporary set lock to those code that pending for checker to approve. (To prevent user select the same status code)
                * 
                * Starts here
                */

                if (!string.IsNullOrEmpty(CodeToAddCondition))
                {
                    cmd.CommandText = "UPDATE M_CODES SET M_IS_PENDING = 1 WHERE M_CODE IN (" + CodeToAddCondition + ") AND M_IS_PENDING = 0";
                    int affectedRows = (int)cmd.ExecuteNonQuery();
                    if (affectedRows != CodeToAdd.Count)
                    {
                        throw new Exception("Some status code has been selected by another person. Please try again.");
                    }

                    rejectSql = "UPDATE M_CODES SET M_IS_PENDING = 0 WHERE M_CODE IN (" + CodeToAddCondition + ")";
                }
                
                /* End here */

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS, REJECT_SQL, DELETE_SQL) "
                                       + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), @olddata,@newdata,'" + action + "', "
                                       + "@approveSql, " + sessionUID + ", '" + sessionUName + "', " + status + ", @rejectSql, @deleteSql)";
                cmd.Parameters.AddWithValue("olddata", olddata);
                cmd.Parameters.AddWithValue("newdata", newdata);
                cmd.Parameters.AddWithValue("approveSql", sql);
                cmd.Parameters.AddWithValue("rejectSql", rejectSql);
                cmd.Parameters.AddWithValue("deleteSql", rejectSql);
                cmd.ExecuteNonQuery();

                trans.Commit();
                return new McObject<bool>(true, "Successful.");
            }
            catch (Exception ex)
            {
                try
                {
                    trans.Rollback();
                    return new McObject<bool>(false, "Failed. " + ex.Message);
                }
                catch
                {
                    return new McObject<bool>(false, "Failed. Unable to roll back. " + ex.Message);
                }
            }
            finally
            {
                dbc.closeConn();
            }
        }
    }
}