using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.IO.Ports;
using System.Management;
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;
using System.Json;
using System.Data.SqlClient;

namespace Maestro.views.administration.statusCodeEscalation
{
    public partial class configureSMS : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            GetConn();
            taskName = Request.Params["task"];
        }

        public class SMSConfig
        {
            public string ComPort { get; set; }
            public string BaudRate { get; set; }
            public string DeviceTimeout { get; set; }
            public string SMSContent { get; set; }
        }

        private void GetConn()
        {
            ManagementObjectCollection objReturn = default(ManagementObjectCollection);
            ManagementObjectSearcher objSearch = default(ManagementObjectSearcher);

            SerialPort SePort = new SerialPort();

            try
            {
                objSearch = new System.Management.ManagementObjectSearcher("Select * from Win32_POTSModem");
                objReturn = objSearch.Get();
                foreach (System.Management.ManagementObject device in objReturn)
                {
                    string fullDescPort = device["AttachedTo"] + " " + device["Name"];
                    SePort.PortName = device["AttachedTo"].ToString();
                    selectCOMPort.Items.Add(fullDescPort);
                }
                objReturn = null;
                objSearch = null;
                SePort = null;
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Failed. " + ex.Message + "')</script>");
            }
        }

        [WebMethod]
        public static McObject<SMSConfig> GetSMSConfiguration()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT * FROM M_SMS_SERVICE";
                SqlDataReader dr = cmd.ExecuteReader();
                SMSConfig sc = null;

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        sc = new SMSConfig();
                        sc.ComPort = dr["DEVICE_COMPORT"].ToString();
                        sc.BaudRate = dr["BAUD_RATE"].ToString();
                        sc.DeviceTimeout = dr["DEVICE_TIMEOUT"].ToString();
                        sc.SMSContent = dr["SMS_CONTENT"].ToString();
                    }
                }
                dr.Close();

                return new McObject<SMSConfig>(true, "Successful.", sc);
            }
            catch (Exception ex)
            {
                return new McObject<SMSConfig>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<bool> UpdateSMSConfiguration(object SMSConfig)
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

                string newTaskName = taskName + " (SMS Configuration)";

                object ComPort, BaudRate, Timeout, SMSContent;
                Dictionary<string, object> sms = (Dictionary<string, object>)SMSConfig;
                sms.TryGetValue("ComPort", out ComPort);
                sms.TryGetValue("BaudRate", out BaudRate);
                sms.TryGetValue("Timeout", out Timeout);
                sms.TryGetValue("SMSContent", out SMSContent);

                cmd.CommandText = "SELECT COUNT(*) FROM M_SMS_SERVICE";
                int count = (int)cmd.ExecuteScalar();

                if (count == 0)
                {
                    sql = "INSERT INTO M_SMS_SERVICE VALUES (" + EscalateType.SMS + ", '" + ComPort + "', " + BaudRate + ", " + Timeout + ", '" + SMSContent + "', GETDATE())";
                    action = MCAction.ADD;
                }
                else
                {
                    sql = "UPDATE M_SMS_SERVICE SET DEVICE_COMPORT = '" + ComPort + "', BAUD_RATE = " + BaudRate + ", DEVICE_TIMEOUT = " + Timeout + ", SMS_CONTENT = '" + SMSContent + "'";
                    action = MCAction.MODIFY;
                }

                JsonObject OLD = new JsonObject();
                JsonObject NEW = new JsonObject();
                SqlDataReader dr = null;

                if (string.Equals(action, MCAction.MODIFY, StringComparison.OrdinalIgnoreCase))
                {
                    cmd.CommandText = "SELECT * FROM M_SMS_SERVICE";
                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            OLD.Add("Device COM Port", dr["DEVICE_COMPORT"].ToString());
                            OLD.Add("Baud Rate", dr["BAUD_RATE"].ToString());
                            OLD.Add("Device Timeout", dr["DEVICE_TIMEOUT"].ToString());
                            OLD.Add("SMS Content", (string.IsNullOrEmpty(dr["SMS_CONTENT"].ToString())) ? "-" : dr["SMS_CONTENT"].ToString());
                        }
                    }
                    dr.Close();
                }

                NEW.Add("Device COM Port", ComPort.ToString());
                NEW.Add("Baud Rate", BaudRate.ToString());
                NEW.Add("Device Timeout", Timeout.ToString());
                NEW.Add("SMS Content", (string.IsNullOrEmpty(SMSContent.ToString())) ? "-" : SMSContent.ToString());

                if (string.Equals(action, MCAction.MODIFY, StringComparison.OrdinalIgnoreCase))
                {
                    string olddata = OLD.ToString();
                    string newdata = NEW.ToString();

                    cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_DATA,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
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