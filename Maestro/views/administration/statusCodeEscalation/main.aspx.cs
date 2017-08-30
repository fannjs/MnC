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

using System.Net.Mail;

namespace Maestro.views.administration.statusCodeEscalation
{
    public partial class main : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
            taskNameHidden.Value = Request.Params["task"];
        }

        public class StatusCodeEscalation
        {
            public int GroupId { get; set; }
            public string KioskType { get; set; }
            public List<StatusCode> CodeList { get; set; }
            public List<EscalationUser> UserList { get; set; }
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
        public static McObject<List<StatusCodeEscalation>> GetEscalationList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT M_GROUP_ID, EU.M_USER_ID, EL.M_ESCALATE_TYPE, M_USER_NAME, M_USER_PHONE, M_USER_EMAIL " +
                                    "FROM M_ESCALATION_USERS EU, M_ESCALATION_LIST EL " +
                                    "WHERE EL.M_USER_ID = EU.M_USER_ID ORDER BY M_GROUP_ID asc, M_USER_ID asc";
                SqlDataReader dr = cmd.ExecuteReader();
                Dictionary<int, string> GroupIdDictionary = new Dictionary<int, string>();
                Dictionary<int, string> UserIdDictionary = new Dictionary<int, string>();
                List<StatusCodeEscalation> sceList = new List<StatusCodeEscalation>();
                StatusCodeEscalation sce = new StatusCodeEscalation();
                List<EscalationUser> UserList = new List<EscalationUser>();
                EscalationUser user = new EscalationUser();
                bool firstTime = true;

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        int groupId = dr.GetInt32(dr.GetOrdinal("M_GROUP_ID"));
                        int EscalationType = dr.GetInt16(dr.GetOrdinal("M_ESCALATE_TYPE"));
                        int userId = dr.GetInt16(dr.GetOrdinal("M_USER_ID"));

                        if (GroupIdDictionary.ContainsKey(groupId))
                        {
                            if (UserIdDictionary.ContainsKey(userId))
                            {
                                if (EscalationType.Equals(EscalateType.SMS))
                                {
                                    user.PhoneNo = dr["M_USER_PHONE"].ToString();
                                }
                                else if (EscalationType.Equals(EscalateType.EMAIL))
                                {
                                    user.EmailAddress = dr["M_USER_EMAIL"].ToString();
                                }
                            }
                            else
                            {
                                UserList.Add(user);

                                UserIdDictionary.Add(userId, "");
                                user = new EscalationUser();

                                user.UserName = dr["M_USER_NAME"].ToString();

                                if (EscalationType.Equals(EscalateType.SMS))
                                {
                                    user.PhoneNo = dr["M_USER_PHONE"].ToString();
                                }
                                else if (EscalationType.Equals(EscalateType.EMAIL))
                                {
                                    user.EmailAddress = dr["M_USER_EMAIL"].ToString();
                                }
                            }
                        }
                        else
                        {
                            if (firstTime)
                            {
                                firstTime = false;

                                GroupIdDictionary.Add(groupId, "");
                                UserIdDictionary.Add(userId, "");

                                user.UserName = dr["M_USER_NAME"].ToString();

                                if (EscalationType.Equals(EscalateType.SMS))
                                {
                                    user.PhoneNo = dr["M_USER_PHONE"].ToString();
                                }
                                else if (EscalationType.Equals(EscalateType.EMAIL))
                                {
                                    user.EmailAddress = dr["M_USER_EMAIL"].ToString();
                                }                                

                                sce.GroupId = groupId;
                            }
                            else
                            {
                                UserList.Add(user);
                                sce.UserList = UserList;
                                sceList.Add(sce);

                                UserIdDictionary.Clear();

                                GroupIdDictionary.Add(groupId, "");
                                UserIdDictionary.Add(userId, "");
                                sce = new StatusCodeEscalation();                                
                                UserList = new List<EscalationUser>();
                                user = new EscalationUser();

                                user.UserName = dr["M_USER_NAME"].ToString();

                                if (EscalationType.Equals(EscalateType.SMS))
                                {
                                    user.PhoneNo = dr["M_USER_PHONE"].ToString();
                                }
                                else if (EscalationType.Equals(EscalateType.EMAIL))
                                {
                                    user.EmailAddress = dr["M_USER_EMAIL"].ToString();
                                }

                                sce.GroupId = groupId;
                            }
                        }
                    }
                    UserList.Add(user);
                    sce.UserList = UserList;
                    sceList.Add(sce);

                    dr.Close();

                    foreach (StatusCodeEscalation sc in sceList)
                    {
                        cmd.CommandText = "SELECT M_CODE, M_DEVICE, M_ERRORTYPE, M_ERROR_DESCRIPTION, M_MACH_TYPE FROM M_CODES WHERE M_GROUP_ID = " + sc.GroupId;
                        dr = cmd.ExecuteReader();
                        List<StatusCode> CodeList = new List<StatusCode>();
                        string KioskType = "";

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                StatusCode MCode = new StatusCode();
                                MCode.Code = dr["M_CODE"].ToString();
                                MCode.Device = dr["M_DEVICE"].ToString();
                                MCode.Type = dr["M_ERRORTYPE"].ToString();
                                MCode.Description = dr["M_ERROR_DESCRIPTION"].ToString();

                                CodeList.Add(MCode);

                                KioskType = dr["M_MACH_TYPE"].ToString();
                            }
                        }

                        sc.KioskType = KioskType;
                        sc.CodeList = CodeList;

                        dr.Close();
                    }
                }
                dr.Close();

                return new McObject<List<StatusCodeEscalation>>(true, "Successful.", sceList);
            }
            catch (Exception ex)
            {
                return new McObject<List<StatusCodeEscalation>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<Boolean> DeleteEscalation(string kioskType, string groupId)
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
                string sql = "DELETE FROM M_ESCALATION_LIST WHERE M_GROUP_ID = " + groupId;
                sql = sql + MCDelimiter.SQL;
                sql = sql + "UPDATE M_CODES SET M_GROUP_ID = NULL WHERE M_GROUP_ID = " + groupId;

                cmd.CommandText = "SELECT M_CODE FROM M_CODES WHERE M_GROUP_ID = " + groupId;
                SqlDataReader dr = cmd.ExecuteReader();

                List<string> CodeList = new List<string>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        CodeList.Add(dr["M_CODE"].ToString());
                    }
                }
                dr.Close();

                cmd.CommandText = "SELECT M_ESCALATE_TYPE, EU.M_USER_ID, M_USER_NAME, M_USER_EMAIL, M_USER_PHONE FROM M_ESCALATION_USERS EU, M_ESCALATION_LIST EL " +
                                    "WHERE EL.M_USER_ID = EU.M_USER_ID AND EL.M_GROUP_ID = " + groupId;
                dr = cmd.ExecuteReader();
                List<int> ids = new List<int>();
                string htmlUsers = "";
                string userDesc = "";
                bool firstTime = true;

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        int escalationType = dr.GetInt16(dr.GetOrdinal("M_ESCALATE_TYPE"));

                        int UserId = dr.GetInt16(dr.GetOrdinal("M_USER_ID"));
                        string UserName = dr["M_USER_NAME"].ToString();
                        string UserEmail = dr["M_USER_EMAIL"].ToString();
                        string UserPhone = dr["M_USER_PHONE"].ToString();;

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
                    htmlUsers = htmlUsers + "<div>" + userDesc + "</div>";
                }
                dr.Close();

                string statusCodeDesc = string.Join(",", CodeList.ToArray());

                JsonObject OLD = new JsonObject();
                OLD.Add("Kiosk Type", kioskType);
                OLD.Add("Status Code", statusCodeDesc);
                OLD.Add("Escalation User", htmlUsers);

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                       + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), @olddata,'" + action + "', "
                                       + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
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

        public class ErrorKiosk
        {
            public string KioskId { get; set; }
            public string StatusCode { get; set; }
            public string StatusType { get; set; }
            public string StatusDescription { get; set; }
            public string StatusDate { get; set; }
            public string StatusTime { get; set; }
            public string BranchCode { get; set; }
            public string BranchName { get; set; }
            public string PIC { get; set; }

            public string GroupId { get; set; }
        }

        //1) Get Machine that has error from EVENT DATA - Get a list of machines with status code & Group Id if possible
        //2) Then use the group id to map with Escalation List & Escalation Users
        //3) Select Email and sendEmail, Phone send SMS
        [WebMethod]
        public static McObject<Boolean> GetErrorKiosk()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT ED.M_MACH_ID, M_DATE, M_TIME, C.M_CODE, M_ERRORTYPE, M_ERROR_DESCRIPTION, " +
                                    "M_GROUP_ID, ML.M_BRANCH_CODE, B.M_BRANCH_NAME, M_CONTACT " +
                                    "FROM M_EVENT_DATA ED, M_CODES C, M_MACHINE_LIST ML, M_BRANCH B " +
                                    "WHERE ED.M_CODE = C.M_CODE " +
                                    "AND ED.M_MACH_ID = ML.M_MACH_ID " +
                                    "AND ML.M_BRANCH_CODE = B.M_BRANCH_CODE " +
                                    "AND M_ERRORTYPE IN ('ERROR', 'WARN')";
                SqlDataReader dr = cmd.ExecuteReader();
                List<ErrorKiosk> ErrorKioskList = new List<ErrorKiosk>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ErrorKiosk ek = new ErrorKiosk();
                        ek.KioskId = dr["M_MACH_ID"].ToString();
                        ek.StatusDate = dr["M_DATE"].ToString();
                        ek.StatusTime = dr["M_TIME"].ToString();
                        ek.StatusCode = dr["M_CODE"].ToString();
                        ek.StatusType = dr["M_ERRORTYPE"].ToString();
                        ek.StatusDescription = dr["M_ERROR_DESCRIPTION"].ToString();
                        ek.BranchCode = dr["M_BRANCH_CODE"].ToString();
                        ek.BranchName = dr["M_BRANCH_NAME"].ToString();
                        ek.PIC = dr["M_CONTACT"].ToString();

                        ek.GroupId = dr["M_GROUP_ID"].ToString();

                        ErrorKioskList.Add(ek);
                    }
                }
                else
                {
                    dr.Close();
                    return new McObject<bool>(true, "No error kiosk.");
                }
                dr.Close();

                //Get Email Configuration
                cmd.CommandText = "SELECT * FROM M_MAIL_SERVICE";
                dr = cmd.ExecuteReader();
                string Host = "", Port = "", SenderEmail = "", MailUser = "", MailPassword = "", Subject = "", Content = "";
                bool SSL = false;
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        Host = dr["MAIL_SERVER"].ToString().Trim();
                        Port = dr["PORT_NO"].ToString().Trim();
                        SenderEmail = dr["SENDER_EMAIL"].ToString().Trim();
                        MailUser = dr["MAIL_USERNAME"].ToString().Trim();
                        MailPassword = dr["MAIL_PASSWORD"].ToString().Trim();
                        SSL = dr.IsDBNull(dr.GetOrdinal("IS_SSL")) ? false : dr.GetBoolean(dr.GetOrdinal("IS_SSL"));

                        Subject = dr["MAIL_SUBJECT"].ToString().Trim();
                        Content = dr["MAIL_CONTENT"].ToString().Trim();
                    }
                }
                else
                {
                    throw new Exception("Email configuration not found. Please configure first.");
                }
                dr.Close();

                //Now send email
                foreach (ErrorKiosk ek in ErrorKioskList)
                {
                    if (!string.IsNullOrEmpty(ek.GroupId))
                    {
                        cmd.CommandText = "SELECT M_ESCALATE_TYPE, M_USER_PHONE, M_USER_EMAIL " +
                                        "FROM M_ESCALATION_USERS EU, M_ESCALATION_LIST EL " +
                                        "WHERE EL.M_USER_ID = EU.M_USER_ID AND M_GROUP_ID = " + ek.GroupId;
                        dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                int escalationType = dr.GetInt16(dr.GetOrdinal("M_ESCALATE_TYPE"));
                                string userEmail = dr["M_USER_EMAIL"].ToString().Trim();
                                string userPhone = dr["M_USER_PHONE"].ToString().Trim();

                                if (escalationType.Equals(EscalateType.EMAIL))
                                {
                                    int PortNo = int.Parse(Port);
                                    Subject = Subject.Replace("<Kiosk_ID>", ek.KioskId).Replace("<Status_Type>", ek.StatusType).Replace("<Status_Date>", ek.StatusDate).Replace("<Status_Time>", ek.StatusTime);
                                    Content = Content.Replace("<Kiosk_ID>", ek.KioskId).Replace("<Status_Code>", ek.StatusCode.Trim()).Replace("<Status_Description>", ek.StatusDescription.Trim()).Replace("<Branch_Name>", ek.BranchName.Trim()).Replace("<Branch_Code>", ek.BranchCode.Trim()).Replace("<Person_In_Charge>", ek.PIC.Trim());

                                    SendEmailToUser(Host, PortNo, SSL, MailUser, MailPassword, SenderEmail, userEmail, Subject, Content, ek.StatusCode);
                                }
                                else if (escalationType.Equals(EscalateType.SMS))
                                {
                                    continue; //NOT YET
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                }
                
                return new McObject<bool>(true, "Successful.");
            }
            catch (Exception ex)
            {
                return new McObject<bool>(false, "Failed ." + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        public static Boolean SendEmailToUser(string Host, int PortNo, bool enableSSL, string SMTPUser, string SMTPPass, string SenderEmail, string UserEmail, string Subject, string Content, string StatusCode)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Host = Host;
                SmtpServer.Port = PortNo;
                SmtpServer.EnableSsl = enableSSL;
                SmtpServer.Credentials = new System.Net.NetworkCredential(SMTPUser, SMTPPass);

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(SenderEmail);
                mail.To.Add(new MailAddress(UserEmail));
                mail.Subject = Subject;
                mail.Body = Content;

                SmtpServer.Send(mail);

                cmd.CommandText = "UPDATE M_EVENT_DATA SET SEND_EMAIL = 1 WHERE M_CODE = @StateCode";
                cmd.Parameters.AddWithValue("StateCode", StatusCode);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /*
        [WebMethod]
        public static Boolean UpdateStatus()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "UPDATE M_EVENT_DATA SET SEND_EMAIL = 0";
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [WebMethod]
        public static McObject<Boolean> SendEmail()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT * FROM M_MAIL_SERVICE";
                SqlDataReader dr = cmd.ExecuteReader();
                string Host = "", Port = "", SenderEmail = "", MailUser = "", MailPassword = "";
                bool SSL = false;


                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        Host = dr["MAIL_SERVER"].ToString().Trim();
                        Port = dr["PORT_NO"].ToString().Trim();
                        SenderEmail = dr["SENDER_EMAIL"].ToString().Trim();
                        MailUser = dr["MAIL_USERNAME"].ToString().Trim();
                        MailPassword = dr["MAIL_PASSWORD"].ToString().Trim();
                        SSL = dr.IsDBNull(dr.GetOrdinal("IS_SSL")) ? false : dr.GetBoolean(dr.GetOrdinal("IS_SSL"));
                    }
                }
                else
                {
                    throw new Exception("Email configuration not found. Please configure first.");
                }
                dr.Close();

                if (string.IsNullOrEmpty(Host))
                {
                    throw new Exception("Mail server address not found.");
                }
                else if (string.IsNullOrEmpty(Port))
                {
                    throw new Exception("Port No not found.");
                }
                else if (string.IsNullOrEmpty(SenderEmail))
                {
                    throw new Exception("Sender Email Address not found.");
                }
                else if (string.IsNullOrEmpty(MailUser))
                {
                    throw new Exception("Mail Username not found.");
                }
                else if (string.IsNullOrEmpty(MailPassword))
                {
                    throw new Exception("Mail Password not found.");
                }

                int PortNo = int.Parse(Port);

                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Host = Host;
                SmtpServer.Port = PortNo;
                SmtpServer.EnableSsl = SSL;
                SmtpServer.Credentials = new System.Net.NetworkCredential(MailUser, MailPassword);

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(SenderEmail);
                mail.To.Add(new MailAddress("bs.gan@htasia.com"));
                mail.Subject = "Subject Title";
                mail.Body = "Content Message";

                SmtpServer.Send(mail);

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
         */
    }
}