using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;
using System.Json;

namespace Maestro.views.administration.notification
{

    public partial class main : System.Web.UI.Page
    {
        //SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CSDMS"].ConnectionString);
        //SqlCommand cmd = new SqlCommand();
        //SqlDataReader dr = default(SqlDataReader);
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            checkconn();
            taskName = Request.Params["task"];
        }

        /// <summary>
        /// Check the system connection settings. 
        /// </summary>
        private void checkconn()
        {
            // Prompt user for connection settings
            int port = 1;
            //int baudRate = 9600; // We Set 9600 as our Default Baud Rate
            //int timeout = GsmCommMain.DefaultTimeout;

            ManagementObjectCollection objReturn = default(ManagementObjectCollection);
            ManagementObjectSearcher objSearch = default(ManagementObjectSearcher);

            SerialPort SePort = new SerialPort();
            try
            {
                // check from Maestro DB
                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT* FROM SystemManagement";
                SqlDataReader dr = cmd.ExecuteReader();
                String strcomport = "";
                while (dr.Read())
                {
                    //baudRate = int.Parse(dr["SMS_BaudRate"].ToString());
                    strcomport = dr["SMS_ComPort"].ToString();
                    if (strcomport.IndexOf("COM") != -1)
                    {
                        int intcom = strcomport.IndexOf("COM");
                        String strRemain = strcomport.Substring(intcom);
                        int intEmptySpace = strRemain.IndexOf(" ");
                        port = int.Parse(strRemain.Substring(3, intEmptySpace - 3));
                        //txtLogMain.Text += "Check modem connection. COM: " + port + ", baudRate: " + baudRate + ", timeout: " + timeout + "\r\n";

                        DdlComPort.Items.Add(strcomport);
                        DdlComPort.SelectedIndex = 0;
                    }
                }
                objSearch = new System.Management.ManagementObjectSearcher("Select * from Win32_POTSModem");
                objReturn = objSearch.Get();
                foreach (System.Management.ManagementObject device in objReturn)
                {
                    string fullDescPort = device["AttachedTo"] + " " + device["Name"];
                    if (!strcomport.Equals(fullDescPort))
                    {
                        SePort.PortName = device["AttachedTo"].ToString();
                        //DdlComPort.Items.Add(device["AttachedTo"] + Constants.vbTab + device["Name"]);
                        DdlComPort.Items.Add(device["AttachedTo"] + " " + device["Name"]);
                    }
                }
                objReturn = null;
                objSearch = null;
                //device = null;
                SePort = null;
                GetSysConfig();


                dr.Close();
                dbc.conn.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(this, "Error: " + ex.Message);
                MaestroModule.CreateLog("Error: " + ex.Message, "checkconn");
            }
        }

        //GET SMS AND EMAIL CONFIGURATION
        /// <summary>
        /// Gets the sms and email configuration settings.
        /// </summary>
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
        }

        
        //[WebMethod]
        //public Boolean updateSmsConf(string ComPort, string BautRate, string Timeout, string ChkError, string ChkWarn, string ChkOffline, string Reminder)

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Boolean updateSmsConf(string ComPort, string BaudRate, string Timeout, string ChkError, string ChkWarn, string ChkOffline, string Reminder)
        {
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

                if (string.IsNullOrEmpty(ComPort) | string.IsNullOrEmpty(BaudRate) | string.IsNullOrEmpty(Timeout))
                {
                    System.Windows.Forms.MessageBox.Show("Empty fields at Com Port, Baud Rate and Timeout are not allowed.");
                    return false;
                }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.MODIFY;
                int status = MCStatus.PENDING;
                string sql = "UPDATE SYSTEMMANAGEMENT SET SMS_COMPORT='" + ComPort + "', SMS_BAUDRATE='" + BaudRate + "', SMS_TIMEOUT='" + Timeout + "', SMS_CheckError='" + ChkError + "', SMS_CheckOffline='" + ChkOffline + "', SMS_CheckWarn='" + ChkWarn + "', SMS_Reminder = '" + Reminder + "'";

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT SMS_COMPORT, SMS_BAUDRATE, SMS_TIMEOUT, SMS_CheckError, SMS_CheckOffline,SMS_CheckWarn,SMS_Reminder "
                                    + "FROM SYSTEMMANAGEMENT";
                SqlDataReader dr = cmd.ExecuteReader();

                string oldComPort = "", oldBaudRate = "", oldTimeOut = "", oldCheckError = "", oldCheckOffline = "", oldCheckWarn = "", oldReminder = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldComPort = dr["SMS_COMPORT"].ToString();
                        oldBaudRate = dr["SMS_BAUDRATE"].ToString();
                        oldTimeOut = dr["SMS_TIMEOUT"].ToString();
                        oldCheckError = dr["SMS_CheckError"].ToString();
                        oldCheckOffline = dr["SMS_CheckOffline"].ToString();
                        oldCheckWarn = dr["SMS_CheckWarn"].ToString();
                        oldReminder = dr["SMS_Reminder"].ToString();
                    }
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("Setting", "SMS");
                OLD.Add("Com Port", oldComPort);
                OLD.Add("Baud Rate", oldBaudRate);
                OLD.Add("Timeout(ms)", oldTimeOut);
                OLD.Add("Check Error", oldCheckError);
                OLD.Add("Check Warning", oldCheckWarn);
                OLD.Add("Check Offline", oldCheckOffline);
                OLD.Add("Reminder", oldReminder);

                JsonObject NEW = new JsonObject();
                NEW.Add("Setting", "SMS");
                NEW.Add("Com Port", ComPort);
                NEW.Add("Baud Rate", BaudRate);
                NEW.Add("Timeout(ms)", Timeout);
                NEW.Add("Check Error", ChkError);
                NEW.Add("Check Warning", ChkWarn);
                NEW.Add("Check Offline", ChkOffline);
                NEW.Add("Reminder", Reminder + " minutes");

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO MKCK_LIST(TASK_ID,TASK_NAME,DT_CREATE,OLD_DATA,NEW_DATA,ACTION,SQL,MAKER_ID,MAKER_UNAME,MC_STATUS) "
                                   + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + olddata + "','" + newdata + "','" + action + "', "
                                   + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();
                dbc.closeConn();

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to update! \n Error: " + ex.Message);
                return false;
            }
        }


        //[WebMethod(EnableSession = true)]
        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Boolean updateEmailConf(string EmailFrom, string MailServer, string MailPort, string EmailUsername, string EmailPassword, 
            string EmailErr, string EmailWarn, string EmailOffline, string EmailReminder, string chkSSL)
        {
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

                if (string.IsNullOrEmpty(EmailFrom) | string.IsNullOrEmpty(EmailUsername) | string.IsNullOrEmpty(MailPort) | string.IsNullOrEmpty(MailServer))
                {
                    System.Windows.Forms.MessageBox.Show("Empty fields at Mail From, Mail Server, Mail Port and Mail Server are not allowed.");
                    return false;
                }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = "";
                string sql = "";
                int status = MCStatus.PENDING;

                // get upper case True / False
                //System.Windows.Forms.MessageBox.Show("email = " + HttpContext.Current.Session["isFT_Email"].ToString() + ", sms = " + HttpContext.Current.Session["isFT_SMS"]);
                if (EmailPassword != "")
                {
                    if (HttpContext.Current.Session["isFT_Email"].Equals("False") && HttpContext.Current.Session["isFT_SMS"].Equals("False"))
                    {
                        /*
                        cmd.CommandText = "INSERT INTO CSDMS.DBO.SYSTEMMANAGEMENT(EMAIL_MAILSERVER,EMAIL_MAILPORT,EMAIL_FROM,EMAIL_USERNAME,EMAIL_PASSWORD,EMAIL_SSL,Email_CheckError, Email_CheckOffline,Email_CheckWarn,Email_Reminder)"
                            + "VALUES('" + MailServer + "','" + MailPort + "','" + EmailFrom + "','" + EmailUsername + "','" + EmailPassword + "','" + chkSSL + "', '" + EmailErr + "', '" + EmailOffline + "', '" + EmailWarn + "', '" + EmailReminder + "')";
                        */
                        sql = "INSERT INTO CSDMS.DBO.SYSTEMMANAGEMENT(EMAIL_MAILSERVER,EMAIL_MAILPORT,EMAIL_FROM,EMAIL_USERNAME,EMAIL_PASSWORD,EMAIL_SSL,Email_CheckError, Email_CheckOffline,Email_CheckWarn,Email_Reminder)"
                            + "VALUES('" + MailServer + "','" + MailPort + "','" + EmailFrom + "','" + EmailUsername + "','" + EmailPassword + "','" + chkSSL + "', '" + EmailErr + "', '" + EmailOffline + "', '" + EmailWarn + "', '" + EmailReminder + "')";

                        action = MCAction.ADD;

                    }
                    else
                    {

                        //cmd.CommandText = "UPDATE SYSTEMMANAGEMENT SET EMAIL_MAILSERVER ='" + MailServer + "', EMAIL_MAILPORT='" + MailPort +
                        //"', EMAIL_FROM='" + EmailFrom + "', EMAIL_USERNAME='" + EmailUsername + "', EMAIL_PASSWORD='" + EmailPassword +
                        //"', EMAIL_SSL='" + chkSSL + "', Email_CheckError = '" + EmailErr + "', Email_CheckOffline = '" + EmailOffline +
                        //"', Email_CheckWarn = '" + EmailWarn + "', Email_Reminder = '" + EmailReminder + "'";

                        sql = "UPDATE SYSTEMMANAGEMENT SET EMAIL_MAILSERVER ='" + MailServer + "', EMAIL_MAILPORT='" + MailPort +
                        "', EMAIL_FROM='" + EmailFrom + "', EMAIL_USERNAME='" + EmailUsername + "', EMAIL_PASSWORD='" + EmailPassword +
                        "', EMAIL_SSL='" + chkSSL + "', Email_CheckError = '" + EmailErr + "', Email_CheckOffline = '" + EmailOffline +
                        "', Email_CheckWarn = '" + EmailWarn + "', Email_Reminder = '" + EmailReminder + "'";

                        action = MCAction.MODIFY;
                    }
                    HttpContext.Current.Session["password"] = EmailPassword;
                }
                else
                {
                    if (HttpContext.Current.Session["isFT_Email"].Equals("False") && HttpContext.Current.Session["isFT_SMS"].Equals("False"))
                    {
                        //cmd.CommandText = "INSERT INTO CSDMS.DBO.SYSTEMMANAGEMENT(EMAIL_MAILSERVER,EMAIL_MAILPORT,EMAIL_TO,EMAIL_FROM,EMAIL_CC,EMAIL_USERNAME,EMAIL_PASSWORD,EMAIL_SSL,Email_CheckError, Email_CheckOffline,Email_CheckWarn,Email_Reminder)"
                        //    + "VALUES('" + MailServer + "','" + MailPort + "','" + EmailFrom + "','" + EmailUsername + "','" + HttpContext.Current.Session["password"] + "','" + chkSSL + "', '" + EmailErr + "', '" + EmailOffline + "', '" + EmailWarn + "', '" + EmailReminder + "')";

                        sql = "INSERT INTO CSDMS.DBO.SYSTEMMANAGEMENT(EMAIL_MAILSERVER,EMAIL_MAILPORT,EMAIL_TO,EMAIL_FROM,EMAIL_CC,EMAIL_USERNAME,EMAIL_PASSWORD,EMAIL_SSL,Email_CheckError, Email_CheckOffline,Email_CheckWarn,Email_Reminder)"
                            + "VALUES('" + MailServer + "','" + MailPort + "','" + EmailFrom + "','" + EmailUsername + "','" + HttpContext.Current.Session["password"] + "','" + chkSSL + "', '" + EmailErr + "', '" + EmailOffline + "', '" + EmailWarn + "', '" + EmailReminder + "')";
                       
                        action = MCAction.ADD;
                    }
                    else
                    {
                        //cmd.CommandText = "UPDATE SYSTEMMANAGEMENT SET EMAIL_MAILSERVER ='" + MailServer + "', EMAIL_MAILPORT='" + MailPort +
                        //    "', EMAIL_FROM='" + EmailFrom + "', EMAIL_USERNAME='" + EmailUsername + "', EMAIL_PASSWORD='" + HttpContext.Current.Session["password"] +
                        //    "', EMAIL_SSL='" + chkSSL + "', Email_CheckError = '" + EmailErr + "', Email_CheckOffline = '" + EmailOffline +
                        //    "', Email_CheckWarn = '" + EmailWarn + "', Email_Reminder = '" + EmailReminder + "'";

                        sql = "UPDATE SYSTEMMANAGEMENT SET EMAIL_MAILSERVER ='" + MailServer + "', EMAIL_MAILPORT='" + MailPort +
                            "', EMAIL_FROM='" + EmailFrom + "', EMAIL_USERNAME='" + EmailUsername + "', EMAIL_PASSWORD='" + HttpContext.Current.Session["password"] +
                            "', EMAIL_SSL='" + chkSSL + "', Email_CheckError = '" + EmailErr + "', Email_CheckOffline = '" + EmailOffline +
                            "', Email_CheckWarn = '" + EmailWarn + "', Email_Reminder = '" + EmailReminder + "'";

                        action = MCAction.MODIFY;
                    }
                }

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();

                JsonObject OLD = new JsonObject();
                JsonObject NEW = new JsonObject();

                if (string.Equals(action, MCAction.MODIFY, StringComparison.OrdinalIgnoreCase))
                {
                    cmd.CommandText = "SELECT EMAIL_FROM, EMAIL_MAILSERVER, EMAIL_MAILPORT, EMAIL_USERNAME, EMAIL_PASSWORD, Email_CheckError, "
                                    + "Email_CheckWarn, Email_CheckOffline, Email_Reminder, EMAIL_SSL "
                                    + "FROM SYSTEMMANAGEMENT";
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            OLD.Add("Setting", "SMTP");
                            OLD.Add("Sender", dr["EMAIL_FROM"].ToString());
                            OLD.Add("Mail Server", dr["EMAIL_MAILSERVER"].ToString());
                            OLD.Add("Mail Port", dr["EMAIL_MAILPORT"].ToString());
                            OLD.Add("Username", dr["EMAIL_USERNAME"].ToString());
                            OLD.Add("Password", dr["EMAIL_PASSWORD"].ToString());
                            OLD.Add("Check Error", dr["Email_CheckError"].ToString());
                            OLD.Add("Check Warning", dr["Email_CheckWarn"].ToString());
                            OLD.Add("Check Offline", dr["Email_CheckOffline"].ToString());
                            OLD.Add("Reminder", dr["Email_Reminder"].ToString() + " minutes");
                            OLD.Add("SSL required", dr["EMAIL_SSL"].ToString());
                        }
                    }

                    dr.Close();
                }                
                
                NEW.Add("Setting", "SMTP");
                NEW.Add("Sender", EmailFrom);
                NEW.Add("Mail Server", MailServer);
                NEW.Add("Mail Port", MailPort);
                NEW.Add("Username", EmailUsername);
                if (EmailPassword == "")
                {
                    if (HttpContext.Current.Session["password"] == null)
                    {
                        throw new Exception("Session expired! Email password not found. Please re-login and try again.");
                    }
                    else
                    {
                        EmailPassword = HttpContext.Current.Session["password"].ToString();
                    }
                }
                NEW.Add("Password", EmailPassword);
                NEW.Add("Check Error", EmailErr);
                NEW.Add("Check Warning", EmailWarn);
                NEW.Add("Check Offline", EmailOffline);
                NEW.Add("Reminder", EmailReminder + " minutes");
                NEW.Add("SSL required", chkSSL);

                if (string.Equals(action, MCAction.MODIFY, StringComparison.OrdinalIgnoreCase))
                {
                    string olddata = OLD.ToString();
                    string newdata = NEW.ToString();

                    cmd.CommandText = "INSERT INTO MKCK_LIST(TASK_ID,TASK_NAME,DT_CREATE,OLD_DATA,NEW_DATA,ACTION,SQL,MAKER_ID,MAKER_UNAME,MC_STATUS) "
                                       + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + olddata + "','" + newdata + "','" + action + "', "
                                       + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                    
                }
                else if (string.Equals(action, MCAction.ADD, StringComparison.OrdinalIgnoreCase))
                {
                    string newdata = NEW.ToString();

                    cmd.CommandText = "INSERT INTO MKCK_LIST(TASK_ID,TASK_NAME,DT_CREATE,NEW_DATA,ACTION,SQL,MAKER_ID,MAKER_UNAME,MC_STATUS) "
                                       + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + newdata + "','" + action + "', "
                                       + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                }
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();
                dbc.closeConn();

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to update! \n Error: " + ex.Message);
                return false;
            }
        }

    }
}