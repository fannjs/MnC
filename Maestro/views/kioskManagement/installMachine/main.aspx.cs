using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Json;
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;

namespace Maestro.views.kioskManagement.installMachine
{
    public partial class main : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
        }
        
        public class Result
        {
            public Result(bool status, string des)
            {
                Status = status;
                Description = des;
            }
            public bool Status { get; set; }
            public string Description { get; set; }
 
        }



        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static DeployBranchInfo[] getDeployBranches(string custCode, string state, string district)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT b.M_BRANCH_CODE, b.M_BRANCH_NAME " +
                               "FROM M_CUSTOMER c, M_BRANCH b " +
                               "WHERE c.M_CUST_CODE = b.M_CUST_CODE " +
                               "AND b.M_BRANCH_NAME != '' " +
                               "AND b.M_BRANCH_CODE != '' " +  
                               "AND b.M_DISTRICT = '" + district + "' " +
                               "AND b.M_STATE = '" + state + "' " +
                               "AND b.M_CUST_CODE = '" + custCode + "' ";

            SqlDataReader dr = cmd.ExecuteReader();
            List<DeployBranchInfo> details = new List<DeployBranchInfo>();
            DeployBranchInfo dbi;
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    dbi = new DeployBranchInfo();
                    dbi.BranchCode = dr["M_BRANCH_CODE"].ToString();
                    dbi.BranchName = dr["M_BRANCH_NAME"].ToString();
                    details.Add(dbi);
                }
            }
            return details.ToArray();
        }


        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static string[] getDeployDistricts(string custCode, string state)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT b.M_DISTRICT " +
                               "FROM M_CUSTOMER c, M_BRANCH b " +
                               "WHERE c.M_CUST_CODE = b.M_CUST_CODE " +
                               "AND b.M_DISTRICT != '' " +
                               "AND b.M_STATE = '" + state + "'" +
                                "AND b.M_BRANCH_NAME != '' " +
                               "AND b.M_BRANCH_CODE != '' " +  
                               "AND b.M_CUST_CODE = '" + custCode + "'";

            SqlDataReader dr = cmd.ExecuteReader();
            List<string> details = new List<string>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    details.Add(dr["M_DISTRICT"].ToString());
                }
            }
            return details.ToArray();
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static string[] getDeployStates(string custCode)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT b.M_STATE " +
                               "FROM M_CUSTOMER c, M_BRANCH b " +
                               "WHERE c.M_CUST_CODE = b.M_CUST_CODE " +
                               "AND b.M_STATE != '' " +
                               "AND b.M_DISTRICT != '' " +
                               "AND b.M_BRANCH_NAME != '' " +
                               "AND b.M_BRANCH_CODE != '' " + 
                               "AND b.M_CUST_CODE = '" + custCode + "'";
            SqlDataReader dr = cmd.ExecuteReader();
            List<string> details = new List<string>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    details.Add(dr["M_STATE"].ToString());
                }
            }
            return details.ToArray();
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static DeploySite[] getDeploySites()
        {

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT c.* " + 
                               "FROM M_CUSTOMER c, M_BRANCH b " +
                               "WHERE c.M_CUST_CODE = b.M_CUST_CODE " +
                               "AND b.M_STATE != '' " +
                               "AND b.M_DISTRICT != '' " +
                               "AND b.M_BRANCH_NAME != '' " +
                               "AND b.M_BRANCH_CODE != '' ";
            SqlDataReader dr = cmd.ExecuteReader();
            List<DeploySite> details = new List<DeploySite>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {

                    DeploySite deploySite = new DeploySite();
                    deploySite.CustCode = dr["M_CUST_CODE"].ToString();
                    deploySite.CustName = dr["M_CUST_NAME"].ToString();
                    deploySite.Country = dr["M_MACH_COUNTRY"].ToString();
                    details.Add(deploySite);
                }
            }
            return details.ToArray();
        
        
        }

        [WebMethod]
        public static Boolean validateMachID(string machID)
        {
            try
            {
                bool existed = false;
                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM M_MACHINE_LIST WHERE M_MACH_ID = @machID";
                cmd.Parameters.AddWithValue("machID", machID);
                string count = cmd.ExecuteScalar().ToString();

                if (count != null)
                {
                    existed = true;
                }
                else
                {
                    existed = false;
                }

                return existed;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod]
        public static Boolean validateSerialNo(string serialNo)
        {
            try
            {
                bool existed = false;
                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM M_MACHINE_LIST WHERE SERIAL_NO = @serialNo";
                cmd.Parameters.AddWithValue("serialNo", serialNo);
                string count = cmd.ExecuteScalar().ToString();

                if (count != null)
                {
                    existed = true;
                }
                else
                {
                    existed = false;
                }

                return existed;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Result InstallMachine(string M_MACH_ID, string VENDOR_ID, string VENDOR_NAME, string M_MACH_TYPE,
            string M_MACH_COUNTRY,  string branchCode, string calendarID, string serialNo)
        {
            string M_CUST_CODE="";
            string M_STATE = "";
            string M_DISTRICT = "";
            string M_ADDRESS1 = "";
            string M_ADDRESS2 = "";
            string M_TEL = "";
            string M_CONTACT = "";

            dbconn DBCon = new dbconn();
            DBCon.connDB();
            SqlCommand cmd = DBCon.conn.CreateCommand();
            //SqlTransaction transaction = DBCon.conn.BeginTransaction();
            //cmd.Connection = DBCon.conn;
            //cmd.Transaction = transaction;

            try {
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

                bool machID_existed = validateMachID(M_MACH_ID);
                if (machID_existed)
                {
                    return new Result(false, "Sorry. This machine ID has being used. Please check again.");
                }

                bool serialNo_existed = validateSerialNo(serialNo);
                if (serialNo_existed)
                {
                    return new Result(false, "Sorry. This serial number has being used. Please check again.");
                }

                cmd.CommandText = "SELECT M_CUST_CODE, M_STATE,M_DISTRICT, M_ADDRESS1, M_ADDRESS2,M_TEL, M_CONTACT FROM M_BRANCH  " +
                     "WHERE M_BRANCH_CODE = '" + branchCode  + "'";
                SqlDataReader dr= cmd.ExecuteReader();

                while (dr.Read())
                {
                    M_CUST_CODE = dr.GetString(0);
                    M_STATE = dr.GetString(1);
                    M_DISTRICT = dr.GetString(2);
                    M_ADDRESS1 = dr.GetString(3);
                    M_ADDRESS2 = dr.GetString(4);
                    M_TEL = dr.GetString(5);
                    M_CONTACT = dr.GetString(6);
                }
                dr.Close();

                string cID = (string.IsNullOrEmpty(calendarID)) ? "NULL" : calendarID;
                string vID = (string.IsNullOrEmpty(VENDOR_ID)) ? "NULL" : VENDOR_ID;

                string sql = "INSERT INTO M_MACHINE_LIST (M_MACH_ID, M_VENDOR_ID, M_MACH_TYPE, " +
                    "M_BRANCH_CODE, CALENDAR_ID, M_SERIAL_NO, CREATED_DATE) " +
                    "VALUES ('" + M_MACH_ID + "'," + vID + ",'" + M_MACH_TYPE + "','" + branchCode + "'," + cID + ",'" + serialNo + "',GETDATE())";

                string calendarName = "-";

                if (!string.IsNullOrEmpty(calendarID))
                {
                    cmd.CommandText = "SELECT CALENDAR_NAME FROM HOLIDAY_CALENDAR_ID WHERE CALENDAR_ID = " + calendarID;
                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        calendarName = dr["CALENDAR_NAME"].ToString();
                    }
                    dr.Close();
                }

                JsonObject NEW = new JsonObject();
                NEW.Add("Kiosk ID", M_MACH_ID);
                NEW.Add("Vendor", VENDOR_NAME);
                NEW.Add("Kiosk Type", M_MACH_TYPE);
                NEW.Add("Country", M_MACH_COUNTRY);
                NEW.Add("Branch Code", branchCode);
                NEW.Add("Calendar", calendarName);
                NEW.Add("Serial No.", serialNo);

                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("newdata", newdata);
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();

                //transaction.Commit();
                return new Result(true, "Successful.");
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                return new Result(false, "Failed! " + ex.Message);
            }
            finally
            {
                DBCon.closeConn();
            } 
        }


        public class DeploySite
        {
            public string CustCode { get; set; }
            public string CustName { get; set; }
            public string Country { get; set; }        
        }

        public class DeployBranchInfo
        {
            public string BranchName { get; set; }
            public string BranchCode { get; set; }
        }

        public class HolidayCalendarID
        {
            public string CalendarID { get; set; }
            public string CalendarName { get; set; }
        }

        public class Vendor
        {
            public string VendorId { get; set; }
            public string VendorName {get;set;}
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static HolidayCalendarID[] getCalendarID()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();

            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT CALENDAR_ID, CALENDAR_NAME FROM HOLIDAY_CALENDAR_ID WHERE CALENDAR_ID NOT IN(1)";
            SqlDataReader dr = cmd.ExecuteReader();
            List<HolidayCalendarID> hcList = new List<HolidayCalendarID>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    HolidayCalendarID hc = new HolidayCalendarID();
                    hc.CalendarID = dr["CALENDAR_ID"].ToString().Trim();
                    hc.CalendarName = dr["CALENDAR_NAME"].ToString().Trim();
                    hcList.Add(hc);
                }
            }

            dr.Close();
            dbc.closeConn();

            return hcList.ToArray();
        }

        [WebMethod]
        public static McObject<List<Vendor>> getVendor()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();

            try
            {
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT M_VENDOR_ID, M_VENDOR_NAME FROM M_VENDOR_LIST";
                SqlDataReader dr = cmd.ExecuteReader();
                List<Vendor> vList = new List<Vendor>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Vendor v = new Vendor();
                        v.VendorId = dr["M_VENDOR_ID"].ToString();
                        v.VendorName = dr["M_VENDOR_NAME"].ToString().Trim();

                        vList.Add(v);
                    }
                }
                else
                {
                    throw new Exception("Vendor not found. Please setup at Vendor Management.");
                }

                dr.Close();
                dbc.closeConn();

                return new McObject<List<Vendor>>(true, "Successful.", vList);
            }
            catch (Exception ex)
            {
                return new McObject<List<Vendor>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }
    }
}