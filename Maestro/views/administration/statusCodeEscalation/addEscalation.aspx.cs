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
    public partial class addEscalation : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
            GetMachineType();
        }

        private void GetMachineType()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            try
            {
                cmd.CommandText = "SELECT * FROM M_MACHINE_TYPE";
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        selectKioskType.Items.Add(dr["M_MACH_TYPE"].ToString());
                    }
                }
                dr.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                dbc.closeConn();
            }
        }

        public class EscalationUser
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string PhoneNo { get; set; }
            public string EmailAddress { get; set; }
            public string Company { get; set; }
        }

        public class StatusCode
        {
            public string Code { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
            public string Device { get; set; }
        }

        [WebMethod]
        public static McObject<List<StatusCode>> GetStatusCode(string machineType, string[] Option)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            List<StatusCode> CodeList = new List<StatusCode>();

            try
            {
                //Means no option
                if (Option.Length == 0)
                {
                    return new McObject<List<StatusCode>>(true, "Successful.", CodeList);
                }
                List<string> newList = new List<string>();
                foreach (string o in Option)
                {
                    newList.Add("'" + o + "'");
                }
                string condition = string.Join(",", newList.ToArray());
                string optionDesc = "AND M_ERRORTYPE IN(" + condition + ")";

                cmd.CommandText = "SELECT M_CODE, M_DEVICE, M_ERRORTYPE, M_ERROR_DESCRIPTION FROM M_CODES WHERE M_MACH_TYPE = @machineType "
                                    + "AND M_GROUP_ID IS NULL AND M_IS_PENDING = 0 " + optionDesc;
                cmd.Parameters.AddWithValue("machineType", machineType);
                SqlDataReader dr = cmd.ExecuteReader();                

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        StatusCode sc = new StatusCode();
                        sc.Code = dr["M_CODE"].ToString();
                        sc.Device = dr["M_DEVICE"].ToString();
                        sc.Type = dr["M_ERRORTYPE"].ToString();
                        sc.Description = dr["M_ERROR_DESCRIPTION"].ToString();

                        CodeList.Add(sc);
                    }
                }
                dr.Close();

                return new McObject<List<StatusCode>>(true, "Successful.", CodeList);
            }
            catch (Exception ex)
            {
                return new McObject<List<StatusCode>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<EscalationUser>> GetEscalationUserList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT * FROM M_ESCALATION_USERS";
                SqlDataReader dr = cmd.ExecuteReader();
                List<EscalationUser> EscalationUserList = new List<EscalationUser>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        EscalationUser eu = new EscalationUser();
                        eu.UserId = dr["M_USER_ID"].ToString();
                        eu.UserName = dr["M_USER_NAME"].ToString();
                        eu.PhoneNo = dr["M_USER_PHONE"].ToString();
                        eu.EmailAddress = dr["M_USER_EMAIL"].ToString();
                        eu.Company = dr["M_USER_COMPANY"].ToString();

                        EscalationUserList.Add(eu);
                    }
                }
                dr.Close();

                return new McObject<List<EscalationUser>>(true, "Successful.", EscalationUserList);
            }
            catch
            {
                return new McObject<List<EscalationUser>>(false, "Successful.");
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<bool> AddEscalationUser(object EscalationUser)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                object UserName, PhoneNo, EmailAddress, Company;
                Dictionary<string, object> EU = (Dictionary<string, object>)EscalationUser;
                EU.TryGetValue("UserName", out UserName);
                EU.TryGetValue("PhoneNo", out PhoneNo);
                EU.TryGetValue("EmailAddress", out EmailAddress);
                EU.TryGetValue("Company", out Company);

                cmd.CommandText = "INSERT INTO M_ESCALATION_USERS (M_USER_NAME, M_USER_PHONE, M_USER_EMAIL, M_USER_COMPANY, CREATED_DATE) " +
                                    "VALUES ('" + UserName + "', '" + PhoneNo + "', '" + EmailAddress + "', '" + Company + "', GETDATE())";
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

        [WebMethod]
        public static McObject<bool> AddNewEscalation(string MachineType, string[] StatusCodeArray, string[] EmailUserArray, string[] PhoneUserArray)
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
                string approveSql = "";
                string rejectSql = "";

                int oldID = 0;

                cmd.CommandText = "SELECT M_GROUP_ID FROM M_MAX_CODEGROUP_ID";
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldID = dr.GetInt32(dr.GetOrdinal("M_GROUP_ID"));
                    }
                    else
                    {
                        throw new Exception("Unable to read M_GROUP_ID from M_MAX_CODEGROUP_ID.");
                    }
                }
                else
                {
                    throw new Exception("Group Id was not exist. Please check again.");
                }
                dr.Close();

                int newID = oldID + 1;
                cmd.CommandText = "UPDATE M_MAX_CODEGROUP_ID SET M_GROUP_ID = @newID, UPDATED_DATE = GETDATE() WHERE M_GROUP_ID = @oldID";
                cmd.Parameters.AddWithValue("newID", newID);
                cmd.Parameters.AddWithValue("oldID", oldID);

                int result = (int)cmd.ExecuteNonQuery();

                if (result == 0)
                {
                    throw new Exception("Someone has updated the M_GROUP_ID. Please try again.");
                }

                SqlTransaction trans = dbc.conn.BeginTransaction();
                cmd.Connection = dbc.conn;
                cmd.Transaction = trans;

                try
                {
                    List<string> newList = new List<string>();

                    for (int i = 0; i < StatusCodeArray.Length; i++)
                    {
                        string formatted = "'" + StatusCodeArray[i] + "'";
                        newList.Add(formatted);
                    }
                    string statusCodeCondition = string.Join(",", newList.ToArray());
                    approveSql = "UPDATE M_CODES SET M_GROUP_ID = " + newID + ", M_IS_PENDING = 0 WHERE M_CODE IN (" + statusCodeCondition + ")";
                    approveSql = approveSql + MCDelimiter.SQL;

                    foreach (string EmailUser in EmailUserArray)
                    {
                        approveSql = approveSql + "INSERT INTO M_ESCALATION_LIST VALUES (" + EscalateType.EMAIL + ", " + newID + ", " + EmailUser + ", GETDATE())";
                        approveSql = approveSql + MCDelimiter.SQL;
                    }
                    foreach (string PhoneUser in PhoneUserArray)
                    {
                        approveSql = approveSql + "INSERT INTO M_ESCALATION_LIST VALUES (" + EscalateType.SMS + ", " + newID + ", " + PhoneUser + ", GETDATE())";
                        approveSql = approveSql + MCDelimiter.SQL;
                    }

                    rejectSql = "UPDATE M_CODES SET M_IS_PENDING = 0 WHERE M_CODE IN (" + statusCodeCondition + ")";

                    string[] newUserArray = EmailUserArray.Union(PhoneUserArray).ToArray();
                    string selectUserCondition = string.Join(",", newUserArray.ToArray());
                    cmd.CommandText = "SELECT M_USER_ID, M_USER_NAME, M_USER_PHONE, M_USER_EMAIL FROM M_ESCALATION_USERS " +
                                        "WHERE M_USER_ID IN (" + selectUserCondition + ")";
                    dr = cmd.ExecuteReader();
                    string htmlUsers = "";

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string userId = dr["M_USER_ID"].ToString();
                            string userName = dr["M_USER_NAME"].ToString();
                            string userPhone = dr["M_USER_PHONE"].ToString();
                            string userEmail = dr["M_USER_EMAIL"].ToString();

                            string userDesc = userName;

                            if (EmailUserArray.Contains(userId))
                            {
                                userDesc = userDesc + " &#060;" + userEmail + "&#062;";
                            }
                            if (PhoneUserArray.Contains(userId))
                            {
                                userDesc = userDesc + " &#060;" + userPhone + "&#062;";
                            }

                            htmlUsers = htmlUsers + "<div>" + userDesc + "</div>";
                        }
                    }
                    dr.Close();

                    string statusCodeDesc = string.Join(",", StatusCodeArray);

                    JsonObject NEW = new JsonObject();
                    NEW.Add("Kiosk Type", MachineType);
                    NEW.Add("Status Code", statusCodeDesc);
                    NEW.Add("Escalation User", htmlUsers);

                    string newdata = NEW.ToString();

                    /*
                     * Simple method for concurrency control
                     * Not sure this is the way of doing concurrency control, will come back to improve
                     * 
                     * Starts here
                     */

                    cmd.CommandText = "UPDATE M_CODES SET M_IS_PENDING = 1 WHERE M_CODE IN (" + statusCodeCondition + ") AND M_IS_PENDING = 0";
                    int affectedRows = (int)cmd.ExecuteNonQuery();                    
                    if(affectedRows != newList.Count)
                    {
                        throw new Exception("Some status code has been selected by another person. Please try again.");
                    }

                    /* End here */

                    cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS, REJECT_SQL, DELETE_SQL) "
                                       + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), @newdata,'" + action + "', "
                                       + "@approveSql, " + sessionUID + ", '" + sessionUName + "', " + status + ", @rejectSql, @deleteSql)";
                    cmd.Parameters.AddWithValue("newdata", newdata);
                    cmd.Parameters.AddWithValue("approveSql", approveSql);
                    cmd.Parameters.AddWithValue("rejectSql", rejectSql);
                    cmd.Parameters.AddWithValue("deleteSql", rejectSql);
                    cmd.ExecuteNonQuery();

                    trans.Commit();
                    return new McObject<bool>(true, "Successful.");
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    return new McObject<bool>(false, "Failed. " + e.Message);
                }
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