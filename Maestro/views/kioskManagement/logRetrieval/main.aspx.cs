using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;
using System.Data.SqlClient;
using System.Web.Services;
using System.Globalization;

namespace Maestro.views.kioskManagement.logRetrieval
{
    public partial class main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public class M_MACHINE_LIST
        {
            public string M_MACH_ID { get; set; }
            public string M_BRANCH_CODE { get; set; }
            public string M_BRANCH_NAME { get; set; }
            public string M_ADDRESS1 { get; set; }
            public string M_ADDRESS2 { get; set; }
        }

        public class M_LOG_LIST
        {
            public int LogID { get; set; }
            public string MachID { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public bool EventLog { get; set; }
            public bool TransLog { get; set; }
            public bool FileTransferLog { get; set; }
        }

        [WebMethod]
        public static McObject<List<M_MACHINE_LIST>> GetAllMachineList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT M_MACH_ID, ML.M_BRANCH_CODE, M_BRANCH_NAME, M_ADDRESS1, M_ADDRESS2 " +
                                    "FROM M_MACHINE_LIST ML, M_BRANCH B " +
                                    "WHERE ML.M_BRANCH_CODE = B.M_BRANCH_CODE " +
                                    "ORDER BY M_MACH_ID asc";
                SqlDataReader dr = cmd.ExecuteReader();

                List<M_MACHINE_LIST> M_MACH_LIST = new List<M_MACHINE_LIST>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        M_MACHINE_LIST M = new M_MACHINE_LIST();
                        M.M_MACH_ID = dr["M_MACH_ID"].ToString();
                        M.M_BRANCH_CODE = dr["M_BRANCH_CODE"].ToString();
                        M.M_BRANCH_NAME = dr["M_BRANCH_NAME"].ToString();
                        M.M_ADDRESS1 = dr["M_ADDRESS1"].ToString();
                        M.M_ADDRESS2 = dr["M_ADDRESS2"].ToString();

                        M_MACH_LIST.Add(M);
                    }
                }

                return new McObject<List<M_MACHINE_LIST>>(true, "Successful.", M_MACH_LIST);
            }
            catch (Exception ex)
            {
                return new McObject<List<M_MACHINE_LIST>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<Boolean> AddToList(string eventLog, string transLog, string fileTransLog, 
            string startDate, string endDate, string[] MachineArr)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                DateTime startDT = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime endDT = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                foreach (string mid in MachineArr)
                {
                    cmd.CommandText = "INSERT INTO M_LOG_LIST(M_MACH_ID,M_EVENT_LOG,M_TRANS_LOG,M_FILETRANSFER_LOG,M_START_DATE,M_END_DATE, CREATED_DATE) "
                                        + "VALUES('" + mid + "', " + eventLog + ", " + transLog + ", " + fileTransLog + ", '" + startDT.ToString("yyyy/MM/dd") + "', '" + endDT.ToString("yyyy/MM/dd") + "', GETDATE())";
                    cmd.ExecuteNonQuery();
                }

                return new McObject<bool>(true, "Successful.");
            }
            catch (Exception e)
            {
                return new McObject<bool>(false, "Failed. " + e.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<M_LOG_LIST>> GetLogList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT * FROM M_LOG_LIST";
                SqlDataReader dr = cmd.ExecuteReader();
                List<M_LOG_LIST> LogList = new List<M_LOG_LIST>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        M_LOG_LIST Log = new M_LOG_LIST();
                        Log.LogID = dr.GetInt32(dr.GetOrdinal("M_LOG_ID"));
                        Log.MachID = dr["M_MACH_ID"].ToString();
                        Log.EventLog = dr.GetBoolean(dr.GetOrdinal("M_EVENT_LOG"));
                        Log.TransLog = dr.GetBoolean(dr.GetOrdinal("M_TRANS_LOG"));
                        Log.FileTransferLog = dr.GetBoolean(dr.GetOrdinal("M_FILETRANSFER_LOG"));
                        Log.StartDate = dr.GetDateTime(dr.GetOrdinal("M_START_DATE"));
                        Log.EndDate = dr.GetDateTime(dr.GetOrdinal("M_END_DATE"));

                        LogList.Add(Log);
                    }
                }
                dr.Close();

                return new McObject<List<M_LOG_LIST>>(true, "Successful.", LogList);
            }
            catch (Exception ex)
            {
                return new McObject<List<M_LOG_LIST>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<Boolean> clearLog(string logID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "DELETE FROM M_LOG_LIST WHERE M_LOG_ID = " + logID + " ";
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
        public static McObject<Boolean> clearAllLogs()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "DELETE FROM M_LOG_LIST";
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