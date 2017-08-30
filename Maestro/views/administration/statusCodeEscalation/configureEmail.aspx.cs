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

namespace Maestro.views.administration.statusCodeEscalation
{
    public partial class configureEmail : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
        }

        public class EmailSetting
        {
            public string SenderEmail { get; set; }
            public string ServerAddress { get; set; }
            public string PortNo { get; set; }
            public string MailUsername { get; set; }
            public string MailPassword { get; set; }
            public bool? SSL { get; set; }
            public string EmailSubject { get; set; }
            public string EmailContent { get; set; }
        }

        /*
         * Old function
         *  - Not sure why need to set Email, SMS and password in the session.
         *  - Check again next time
         *  
        public void GetSysConfig()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            //"SELECT * FROM [M_CUSTOMER] ORDER BY [M_MACH_COUNTRY], [M_CUST_NAME]" 
            cmd.CommandText = "SELECT * FROM CSDMS.DBO.SYSTEMMANAGEMENT";
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                dr.Read();
                //if (!Information.IsDBNull(dr(0)))	
                //System.Windows.Forms.MessageBox.Show("IsDBNull(dr[0]) = " + System.Convert.IsDBNull(dr[0]));
                if (!System.Convert.IsDBNull(dr[0]))
                {
                    DdlComPort.Text = dr[0].ToString();
                    ddlBaudRate.Text = dr[1].ToString();
                    ddlTimeout.Text = dr[2].ToString();
                    //chkSend.Checked = Convert.ToBoolean(Strings.Trim(dr[9]));
                    ChkError.Checked = Convert.ToBoolean((dr["SMS_CheckError"].ToString()).Trim());
                    ChkOffline.Checked = Convert.ToBoolean((dr["SMS_CheckOffline"].ToString()).Trim());
                    ChkWarn.Checked = Convert.ToBoolean((dr["SMS_CheckWarn"].ToString()).Trim());
                    txtReminder.Text = dr["SMS_Reminder"].ToString();

                    chkEmailErr.Checked = Convert.ToBoolean((dr["Email_CheckError"].ToString()).Trim());
                    chkEmailOffline.Checked = Convert.ToBoolean((dr["Email_CheckOffline"].ToString()).Trim());
                    chkEmailWarn.Checked = Convert.ToBoolean((dr["Email_CheckWarn"].ToString()).Trim());
                    txtEmailReminder.Text = dr["Email_Reminder"].ToString();
                    Session["isFT_SMS"] = true;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("SMS settings has not been configured.");
                    Session["isFT_SMS"] = false;
                }

                if (!System.Convert.IsDBNull(dr[3]))
                {
                    txtMailServer.Text = dr[3].ToString();
                    txtMailPort.Text = dr[4].ToString();
                    txtEmailFrom.Text = dr[5].ToString();
                    txtEmailUsername.Text = dr[6].ToString();
                    Session["password"] = dr[7].ToString();
                    txtEmailPassword.Attributes.Add("value", dr[7].ToString());
                    chkSSL.Checked = Convert.ToBoolean(dr[8]);
                    dr.Close();
                    Session["isFT_Email"] = true;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Email settings has not been configured.");
                    Session["isFT_Email"] = false;
                    Session["password"] = "";
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Email and sms settings have not been configured.");
                Session["isFT_Email"] = false;
                Session["isFT_SMS"] = false;
                Session["password"] = "";
            }
            dbc.closeConn();

            return;
        }*/

        [WebMethod]
        public static McObject<EmailSetting> GetEmailConfiguration()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT * FROM M_MAIL_SERVICE";
                SqlDataReader dr = cmd.ExecuteReader();
                EmailSetting es = null;

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        es = new EmailSetting();
                        es.SenderEmail = dr["SENDER_EMAIL"].ToString();
                        es.ServerAddress = dr["MAIL_SERVER"].ToString();
                        es.PortNo = dr["PORT_NO"].ToString();
                        es.MailUsername = dr["MAIL_USERNAME"].ToString();
                        es.MailPassword = dr["MAIL_PASSWORD"].ToString();
                        es.SSL = dr.IsDBNull(dr.GetOrdinal("IS_SSL")) ? (bool?)null : dr.GetBoolean(dr.GetOrdinal("IS_SSL"));
                        es.EmailSubject = dr["MAIL_SUBJECT"].ToString();
                        es.EmailContent = dr["MAIL_CONTENT"].ToString();
                    }
                }
                dr.Close();

                return new McObject<EmailSetting>(true, "Successful.", es);
            }
            catch(Exception ex)
            {
                return new McObject<EmailSetting>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<bool> UpdateEmailSetting(object emailSetting)
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
                string action = "";
                int status = MCStatus.PENDING;
                string sql = "";

                string newTaskName = taskName + " (Email Configuration)";

                object senderEmail, serverAddress, portNo, mailUser, mailPassword, sslRequired, mailSubject, mailContent;
                Dictionary<string, object> emailSet = (Dictionary<string, object>)emailSetting;
                emailSet.TryGetValue("senderEmail", out senderEmail);
                emailSet.TryGetValue("serverAddress", out serverAddress);
                emailSet.TryGetValue("portNo", out portNo);
                emailSet.TryGetValue("mailUser", out mailUser);
                emailSet.TryGetValue("mailPassword", out mailPassword);
                emailSet.TryGetValue("sslRequired", out sslRequired);
                emailSet.TryGetValue("mailSubject", out mailSubject);
                emailSet.TryGetValue("mailContent", out mailContent);

                cmd.CommandText = "SELECT COUNT(*) FROM M_MAIL_SERVICE";
                int count = (int)cmd.ExecuteScalar();

                if (count == 0)
                {
                    sql = "INSERT INTO M_MAIL_SERVICE VALUES (" + EscalateType.EMAIL + ", '" + senderEmail + "', '" + serverAddress + "', '" + portNo + "', " +
                                        "'" + mailUser + "', '" + mailPassword + "', " + sslRequired + ", '" + mailSubject + "', '" + mailContent + "', GETDATE())";
                    action = MCAction.ADD;
                }
                else
                {
                    sql = "UPDATE M_MAIL_SERVICE SET SENDER_EMAIL = '" + senderEmail + "', MAIL_SERVER = '" + serverAddress + "', PORT_NO = '" + portNo + "', " +
                                        "MAIL_USERNAME = '" + mailUser + "', MAIL_PASSWORD = '" + mailPassword + "', IS_SSL = " + sslRequired + ", MAIL_SUBJECT = '" + mailSubject + "', " +
                                        "MAIL_CONTENT = '" + mailContent + "'";
                    action = MCAction.MODIFY;
                }

                JsonObject OLD = new JsonObject();
                JsonObject NEW = new JsonObject();
                SqlDataReader dr = null;
                
                if (string.Equals(action, MCAction.MODIFY, StringComparison.OrdinalIgnoreCase))
                {
                    cmd.CommandText = "SELECT * FROM M_MAIL_SERVICE";
                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            OLD.Add("Sender Email", dr["SENDER_EMAIL"].ToString());
                            OLD.Add("Mail Server", dr["MAIL_SERVER"].ToString());
                            OLD.Add("Port No", dr["PORT_NO"].ToString());
                            OLD.Add("Mail Username", dr["MAIL_USERNAME"].ToString());
                            OLD.Add("Mail Password", dr["MAIL_PASSWORD"].ToString());

                            if (!dr.IsDBNull(dr.GetOrdinal("IS_SSL")))
                            {
                                if (dr.GetBoolean(dr.GetOrdinal("IS_SSL")))
                                {
                                    OLD.Add("SSL Required", "Yes");
                                }
                                else
                                {
                                    OLD.Add("SSL Required", "No");
                                }
                            }
                            else
                            {
                                OLD.Add("SSL Required", "-");
                            }

                            OLD.Add("Mail Subject", (string.IsNullOrEmpty(dr["MAIL_SUBJECT"].ToString())) ? "-" : dr["MAIL_SUBJECT"].ToString());
                            OLD.Add("Mail Content", (string.IsNullOrEmpty(dr["MAIL_CONTENT"].ToString())) ? "-" : dr["MAIL_CONTENT"].ToString());
                        }
                    }
                    dr.Close();
                }

                NEW.Add("Sender Email", senderEmail.ToString());
                NEW.Add("Mail Server", serverAddress.ToString());
                NEW.Add("Port No", portNo.ToString());
                NEW.Add("Mail Username", mailUser.ToString());
                NEW.Add("Mail Password", mailPassword.ToString());

                if (sslRequired.Equals("1"))
                {
                    NEW.Add("SSL Required", "Yes");
                }
                else
                {
                    NEW.Add("SSL Required", "No");
                }

                NEW.Add("Mail Subject", (string.IsNullOrEmpty(mailSubject.ToString())) ? "-" : mailSubject.ToString());
                NEW.Add("Mail Content", (string.IsNullOrEmpty(mailContent.ToString())) ? "-" : mailContent.ToString());

                if (string.Equals(action, MCAction.MODIFY, StringComparison.OrdinalIgnoreCase))
                {
                    string olddata = OLD.ToString();
                    string newdata = NEW.ToString();

                    cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                       + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @olddata, @newdata,'" + action + "', "
                                       + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                    cmd.Parameters.AddWithValue("olddata", olddata);
                    cmd.Parameters.AddWithValue("newdata", newdata);
                }
                else if (string.Equals(action, MCAction.ADD, StringComparison.OrdinalIgnoreCase))
                {
                    string newdata = NEW.ToString();

                    cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                       + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @newdata,'" + action + "', "
                                       + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                    cmd.Parameters.AddWithValue("newdata", newdata);
                }
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