using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using Maestro.Classes;
using Maestro.Classes.Prefix;
using Maestro.Classes.MakerChecker;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using System.IO;
using System.Globalization;
using System.Json;

namespace Maestro.views.kioskMaintenance.softwareDistribution
{
    public partial class main : System.Web.UI.Page
    {
        private static string taskName = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
        }

        public class Software
        {
            public int VersionId { get; set; }
            public string VersionName { get; set; }
            public string FileName { get; set; }
            public string Checksum { get; set; }
        }
        public class UpdateType
        {
            public string SU_ID { get; set; }
            public string SU_NAME { get; set; }
        }
        public class Machine
        {
            public string M_MACH_ID { get; set; }
            public string M_BRANCH_CODE { get; set; }
            public string M_BRANCH_NAME { get; set; }
            public string M_ADDRESS1 { get; set; }
            public string M_ADDRESS2 { get; set; } 
        }
        public class SoftwareQueue
        {
            public string KioskId { get; set; }
            public string VersionId { get; set; }
            public string VersionName { get; set; }
            public string DownloadSUID { get; set; }
            public DateTime DownloadTime { get; set; }
            public DateTime? DownloadedTime { get; set; }
            public string DeploySUID { get; set; }
            public DateTime DeployTime { get; set; }
        }

        [WebMethod]
        public static McObject<List<UpdateType>> GetSoftwareUpdateType()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT M_SU_ID, M_SU_NAME FROM M_SOFTWARE_UPDATE_TYPE";
                SqlDataReader dr = cmd.ExecuteReader();
                List<UpdateType> TypeList = new List<UpdateType>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        UpdateType u = new UpdateType();
                        u.SU_ID = dr["M_SU_ID"].ToString();
                        u.SU_NAME = dr["M_SU_NAME"].ToString();

                        TypeList.Add(u);
                    }
                }

                return new McObject<List<UpdateType>>(true, "Successful.", TypeList);
            }
            catch (Exception ex)
            {
                return new McObject<List<UpdateType>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<Software>> GetSoftwareList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT * FROM M_SOFTWARE_VERSION";
                SqlDataReader dr = cmd.ExecuteReader();
                List<Software> SoftwareList = new List<Software>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Software s = new Software();
                        s.VersionId = dr.GetInt16(dr.GetOrdinal("M_VERSION_ID"));
                        s.VersionName = dr.GetString(dr.GetOrdinal("M_VERSION_NAME"));
                        s.FileName = dr.GetString(dr.GetOrdinal("M_FILENAME"));
                        s.Checksum = dr.GetString(dr.GetOrdinal("M_CHECKSUM"));

                        SoftwareList.Add(s);
                    }
                }

                return new McObject<List<Software>>(true, "Successful.", SoftwareList);
            }
            catch (Exception ex)
            {
                return new McObject<List<Software>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        /*
         * Add Software Version in UploadFile.ashx
         *          
         * End */

        [WebMethod]
        public static McObject<Boolean> DeleteSoftwareVersion(string VersionId)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            SqlTransaction trans = dbc.conn.BeginTransaction();
            cmd.Connection = dbc.conn;
            cmd.Transaction = trans;

            try
            {
                if (HttpContext.Current.Session["userId"] == null)
                {
                    throw new Exception("Session timeout. User not found!");
                }

                cmd.CommandText = "SELECT COUNT(*) FROM M_SOFTWARE_VERSION_QUEUE WHERE M_VERSION_ID = " + VersionId;
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    throw new Exception("Unable to delete. This version is existed in the queue list.");
                }

                //Get FileName from database
                cmd.CommandText = "SELECT M_FILENAME FROM M_SOFTWARE_VERSION WHERE M_VERSION_ID = " + VersionId;
                SqlDataReader dr = cmd.ExecuteReader();
                string FileName = "";
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        FileName = dr["M_FILENAME"].ToString();
                    }
                }
                else
                {
                    throw new Exception("Record not found.");
                }
                dr.Close();
                
                //Delete record from database
                cmd.CommandText = "DELETE FROM M_SOFTWARE_VERSION WHERE M_VERSION_ID = " + VersionId;
                cmd.ExecuteNonQuery();

                //Delete the file
                McObject<string> McObj = GetPath();
                if (!McObj.isSuccessful())
                {
                    throw new Exception(McObj.getMessage());
                }
                string FolderPath = McObj.getObject();
                var path = Path.Combine(FolderPath, FileName);
                File.Delete(path);

                trans.Commit();
                return new McObject<bool>(true, "Successful.");
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return new McObject<bool>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        private static McObject<string> GetPath()
        {
            try
            {
                Configuration WebConfig = WebConfigurationManager.OpenWebConfiguration("~");

                if (WebConfig.AppSettings.Settings["NewSoftVerPath"] == null)
                {
                    throw new Exception("Software Path not found. Please do the setting in web.config.");
                }
                else
                {
                    return new McObject<string>(true, "Successful.", WebConfig.AppSettings.Settings["NewSoftVerPath"].Value);
                }
            }
            catch (Exception ex)
            {
                return new McObject<string>(false, ex.Message);
            }
        }

        /*
         * This require filter
         * If machine id is in the queue, do not allow user to add
         */
        [WebMethod]
        public static McObject<List<Machine>> GetAllMachineList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT M_MACH_ID, ML.M_BRANCH_CODE, M_BRANCH_NAME, M_ADDRESS1, M_ADDRESS2 " +
                                    "FROM M_MACHINE_LIST ML, M_BRANCH B " +
                                    "WHERE ML.M_BRANCH_CODE = B.M_BRANCH_CODE " +
                                    "AND M_MACH_ID NOT IN (SELECT M_MACH_ID FROM M_SOFTWARE_VERSION_QUEUE) " +
                                    "ORDER BY M_MACH_ID asc";
                SqlDataReader dr = cmd.ExecuteReader();

                List<Machine> M_MACH_LIST = new List<Machine>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Machine M = new Machine();
                        M.M_MACH_ID = dr["M_MACH_ID"].ToString();
                        M.M_BRANCH_CODE = dr["M_BRANCH_CODE"].ToString();
                        M.M_BRANCH_NAME = dr["M_BRANCH_NAME"].ToString();
                        M.M_ADDRESS1 = dr["M_ADDRESS1"].ToString();
                        M.M_ADDRESS2 = dr["M_ADDRESS2"].ToString();

                        M_MACH_LIST.Add(M);
                    }
                }

                return new McObject<List<Machine>>(true, "Successful.", M_MACH_LIST);
            }
            catch (Exception ex)
            {
                return new McObject<List<Machine>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<SoftwareQueue>> GetQueueList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT SVQ.M_VERSION_ID, M_VERSION_NAME, M_MACH_ID, M_SU_ID, M_TO_DOWNLOAD,M_DOWNLOAD_DT, M_EFFECTIVE_ID,M_EFFECTIVE_AT "
                                + "FROM M_SOFTWARE_VERSION_QUEUE SVQ, M_SOFTWARE_VERSION SV "
                                + "WHERE SVQ.M_VERSION_ID = SV.M_VERSION_ID ";
                SqlDataReader dr = cmd.ExecuteReader();
                List<SoftwareQueue> QueueList = new List<SoftwareQueue>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        SoftwareQueue sq = new SoftwareQueue();
                        sq.KioskId = dr["M_MACH_ID"].ToString();
                        sq.VersionId = dr["M_VERSION_ID"].ToString();
                        sq.VersionName = dr["M_VERSION_NAME"].ToString();
                        sq.DownloadSUID = dr["M_SU_ID"].ToString();
                        sq.DownloadTime = dr.GetDateTime(dr.GetOrdinal("M_TO_DOWNLOAD"));
                        sq.DownloadedTime = dr.IsDBNull(dr.GetOrdinal("M_DOWNLOAD_DT")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("M_DOWNLOAD_DT"));
                        sq.DeploySUID = dr["M_EFFECTIVE_ID"].ToString();
                        sq.DeployTime = dr.GetDateTime(dr.GetOrdinal("M_EFFECTIVE_AT"));

                        QueueList.Add(sq);
                    }
                }

                return new McObject<List<SoftwareQueue>>(true, "Successful.", QueueList);
            }
            catch (Exception ex)
            {
                return new McObject<List<SoftwareQueue>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<SoftwareQueue> GetQueue(string KioskId, string VersionId)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            SqlDataReader dr = null;

            try
            {
                cmd.CommandText = "SELECT M_VERSION_NAME, M_SU_ID, M_TO_DOWNLOAD,M_DOWNLOAD_DT, M_EFFECTIVE_ID,M_EFFECTIVE_AT "
                                + "FROM M_SOFTWARE_VERSION_QUEUE SVQ, M_SOFTWARE_VERSION SV "
                                + "WHERE SVQ.M_VERSION_ID = SV.M_VERSION_ID "
                                + "AND SVQ.M_VERSION_ID = @VersionId AND M_MACH_ID = @KioskId";
                cmd.Parameters.AddWithValue("VersionId", VersionId);
                cmd.Parameters.AddWithValue("KioskId", KioskId);
                dr = cmd.ExecuteReader();

                SoftwareQueue sq = new SoftwareQueue();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        sq.VersionName = dr["M_VERSION_NAME"].ToString();
                        sq.DownloadSUID = dr["M_SU_ID"].ToString();
                        sq.DownloadTime = dr.GetDateTime(dr.GetOrdinal("M_TO_DOWNLOAD"));
                        sq.DownloadedTime = dr.IsDBNull(dr.GetOrdinal("M_DOWNLOAD_DT")) ? (DateTime?)null : dr.GetDateTime(dr.GetOrdinal("M_DOWNLOAD_DT"));
                        sq.DeploySUID = dr["M_EFFECTIVE_ID"].ToString();
                        sq.DeployTime = dr.GetDateTime(dr.GetOrdinal("M_EFFECTIVE_AT"));
                    }
                }
                else
                {              
                    throw new Exception("Record not found.");
                }
                dr.Close();

                return new McObject<SoftwareQueue>(true, "Successful.", sq);
            }
            catch (Exception ex)
            {
                return new McObject<SoftwareQueue>(false, "Failed. " + ex.Message);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<Boolean> AddNewQueue(string VersionId, string VersionName, string[] KioskList, string DownloadSUID, string DLDT, string DeploySUID, string DPDT)
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

                if (String.IsNullOrEmpty(VersionId)) { throw new Exception("Invalid Version ID"); }
                if (KioskList == null) { throw new Exception("Invalid Kiosk List"); }
                if (String.IsNullOrEmpty(DownloadSUID)) { throw new Exception("Invalid Download Type"); }
                if (String.IsNullOrEmpty(DLDT)) { throw new Exception("Invalid Download Time"); }
                if (String.IsNullOrEmpty(DeploySUID)) { throw new Exception("Invalid Deploy Type"); }
                if (String.IsNullOrEmpty(DPDT)) { throw new Exception("Invalid Deploy Time"); }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.ADD;
                int status = MCStatus.PENDING;
                string sql = "";
                string newTaskName = taskName + " (Queue)";

                DateTime DownloadDateTime = DateTime.ParseExact(DLDT, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                DateTime DeployDateTime = DateTime.ParseExact(DPDT, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);

                foreach (string id in KioskList)
                {
                    sql = sql + "INSERT INTO M_SOFTWARE_VERSION_QUEUE (M_VERSION_ID, M_MACH_ID, M_SU_ID, M_TO_DOWNLOAD, M_EFFECTIVE_ID, M_EFFECTIVE_AT, CREATED_DATE) "
                              + "VALUES ('" + VersionId + "', '" + id + "', " + DownloadSUID + ", '" + DownloadDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'"
                              + ", " + DeploySUID + ", '" + DeployDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', GETDATE())";
                    sql = sql + MCDelimiter.SQL;
                }

                string formattedDownloadDT = (DownloadSUID.Equals("1")) ? DownloadDateTime.ToString("dd/MM/yyyy") + "<br/>(After CUTOFF)" : DownloadDateTime.ToString("dd/MM/yyyy H:mm tt");
                string formattedDeployDT = (DeploySUID.Equals("1")) ? DeployDateTime.ToString("dd/MM/yyyy") + "<br/>(After CUTOFF)" : DeployDateTime.ToString("dd/MM/yyyy H:mm tt");

                string KioskListDesc = string.Join(", ", KioskList);
                JsonObject NEW = new JsonObject();
                NEW.Add("Version", VersionName);
                NEW.Add("Kiosk", KioskListDesc);
                NEW.Add("Download Time", formattedDownloadDT);
                NEW.Add("Deploy Time", formattedDeployDT);

                string newdata = NEW.ToString();
                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("newdata", newdata);
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

        [WebMethod]
        public static McObject<Boolean> EditNewQueue(string VersionId, string VersionName, string KioskId, string DownloadSUID, string DLDT, string DeploySUID, string DPDT)
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

                if (String.IsNullOrEmpty(VersionId)) { throw new Exception("Invalid Version ID"); }
                if (String.IsNullOrEmpty(KioskId)) { throw new Exception("Invalid Kiosk ID"); }
                if (String.IsNullOrEmpty(DownloadSUID)) { throw new Exception("Invalid Download Type"); }
                if (String.IsNullOrEmpty(DLDT)) { throw new Exception("Invalid Download Time"); }
                if (String.IsNullOrEmpty(DeploySUID)) { throw new Exception("Invalid Deploy Type"); }
                if (String.IsNullOrEmpty(DPDT)) { throw new Exception("Invalid Deploy Time"); }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.MODIFY;
                int status = MCStatus.PENDING;
                string sql = "";
                string newTaskName = taskName + " (Queue)";

                DateTime DownloadDateTime = DateTime.ParseExact(DLDT, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                DateTime DeployDateTime = DateTime.ParseExact(DPDT, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                sql = "UPDATE M_SOFTWARE_VERSION_QUEUE SET M_SU_ID = " + DownloadSUID + ", M_TO_DOWNLOAD = '" + DownloadDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "',"
                    + "M_EFFECTIVE_ID = " + DeploySUID + ", M_EFFECTIVE_AT = '" + DeployDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' "
                    + "WHERE M_VERSION_ID = " + VersionId + " AND M_MACH_ID = '" + KioskId + "'";


                /* Getting Old Data */
                cmd.CommandText = "SELECT M_SU_ID, M_TO_DOWNLOAD, M_EFFECTIVE_ID, M_EFFECTIVE_AT " +
                                    "FROM M_SOFTWARE_VERSION_QUEUE " +
                                    "WHERE M_VERSION_ID = " + VersionId + " AND M_MACH_ID = '" + KioskId + "'";
                SqlDataReader dr = cmd.ExecuteReader();
                string OldDownloadDT = "";
                string OldDeployDT = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        OldDownloadDT = ((dr["M_SU_ID"].ToString()).Equals("1")) ? (dr.GetDateTime(dr.GetOrdinal("M_TO_DOWNLOAD"))).ToString("dd/MM/yyyy") + "<br/>(After CUTOFF)" : (dr.GetDateTime(dr.GetOrdinal("M_TO_DOWNLOAD"))).ToString("dd/MM/yyyy H:mm tt");
                        OldDeployDT = ((dr["M_EFFECTIVE_ID"].ToString()).Equals("1")) ? (dr.GetDateTime(dr.GetOrdinal("M_EFFECTIVE_AT"))).ToString("dd/MM/yyyy") + "<br/>(After CUTOFF)" : (dr.GetDateTime(dr.GetOrdinal("M_EFFECTIVE_AT"))).ToString("dd/MM/yyyy H:mm tt");
                    }
                }
                else
                {
                    throw new Exception("Record not found.");
                }
                dr.Close();
                JsonObject OLD = new JsonObject();
                OLD.Add("Version", VersionName);
                OLD.Add("Kiosk", KioskId);
                OLD.Add("Download Time", OldDownloadDT);
                OLD.Add("Deploy Time", OldDeployDT);


                /* Populating new data */
                string formattedDownloadDT = (DownloadSUID.Equals("1")) ? DownloadDateTime.ToString("dd/MM/yyyy") + "<br/>(After CUTOFF)" : DownloadDateTime.ToString("dd/MM/yyyy H:mm tt");
                string formattedDeployDT = (DeploySUID.Equals("1")) ? DeployDateTime.ToString("dd/MM/yyyy") + "<br/>(After CUTOFF)" : DeployDateTime.ToString("dd/MM/yyyy H:mm tt");
                JsonObject NEW = new JsonObject();
                NEW.Add("Version", VersionName);
                NEW.Add("Kiosk", KioskId);
                NEW.Add("Download Time", formattedDownloadDT);
                NEW.Add("Deploy Time", formattedDeployDT);

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();
                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @olddata, @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("olddata", olddata);
                cmd.Parameters.AddWithValue("newdata", newdata);
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

        [WebMethod]
        public static McObject<Boolean> DeleteQueue(string VersionId, string VersionName, string KioskId)
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

                if (String.IsNullOrEmpty(VersionId)) { throw new Exception("Invalid Version ID"); }
                if (String.IsNullOrEmpty(KioskId)) { throw new Exception("Invalid Kiosk ID"); }

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.DELETE;
                int status = MCStatus.PENDING;
                string sql = "DELETE FROM M_SOFTWARE_VERSION_QUEUE WHERE M_VERSION_ID = " + VersionId + " AND M_MACH_ID = '" + KioskId + "'";
                string newTaskName = taskName + " (Queue)";

                JsonObject OLD = new JsonObject();
                OLD.Add("Kiosk", KioskId);
                OLD.Add("Version", VersionName);

                string olddata = OLD.ToString();
                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + newTaskName + "', GETDATE(), @olddata, '" + action + "', "
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
    }
}