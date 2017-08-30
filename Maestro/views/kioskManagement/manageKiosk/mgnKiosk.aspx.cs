using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Json;
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;

namespace Maestro.views.kioskManagement.manageKiosk
{
    public partial class mgnKiosk : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
        }

        public class Machine
        {
            public string MachineID { get; set; }

            public string TotalMachine { get; set; }
            public string MCount { get; set; } //Count
            public string MState { get; set; }
            public string MDistrict { get; set; }
            public string MCity { get; set; }
            public string MCalendar { get; set; }
            public string MAddress1 { get; set; }
            public string MAddress2 { get; set; }
            public string MBranchNo { get; set; }
            public string MBranchName { get; set; }
            public string MTel { get; set; }
            public string MPIC { get; set; }
            public string MVendor { get; set; }

            public string CustCode { get; set; }
            public string CurrentVer { get; set; }
            public string CalendarName { get; set; }
        }

        public class Calendar
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }
        public class Vendor
        {
            public string ID { get; set; }
            public string Name { get; set; }
        }
        public class Customer
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Country { get; set; }
        }
        public class State
        {
            public string Name { get; set; }
        }
        public class District
        {
            public string Name { get; set; }
        }
        public class City
        {
            public string Name { get; set; }
        }
        public class Branch
        {
            public string Code { get; set; }
            public string Name { get; set; }

            public string ContactNo {get;set;}
            public string PIC {get;set;}
        }

        public static int getCount()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();

            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM M_MACHINE_LIST";

            int count = (Int32)cmd.ExecuteScalar();

            dbc.closeConn();

            return count;
        }

        [WebMethod, ScriptMethod]
        public static Machine[] getMachineList(string pageNumber, string pageSize)
        {
            int iPageNumber = Int32.Parse(pageNumber);
            int iPageSize = Int32.Parse(pageSize);
            int count = getCount();

            dbconn dbc = new dbconn();
            dbc.connDB();
            
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY M_MACH_ID) AS NUMBER, " +
                                "ML.M_MACH_ID, ML.M_VERSION_ID, HC.CALENDAR_NAME, MB.M_STATE, MB.M_DISTRICT, ML.M_BRANCH_CODE, MB.M_CONTACT, MB.M_BRANCH_NAME " +
                                "FROM M_MACHINE_LIST AS ML " +
                                "LEFT JOIN HOLIDAY_CALENDAR_ID AS HC ON ML.CALENDAR_ID = HC.CALENDAR_ID " +
                                "LEFT JOIN M_BRANCH AS MB ON ML.M_BRANCH_CODE = MB.M_BRANCH_CODE) AS MLIST " +
                                "WHERE NUMBER BETWEEN ((@PageNumber - 1) * @PageSize + 1) AND (@PageNumber * @PageSize) " +
                                "ORDER BY M_MACH_ID";

            cmd.Parameters.AddWithValue("PageNumber", iPageNumber);
            cmd.Parameters.AddWithValue("PageSize", iPageSize);

            SqlDataReader dr = cmd.ExecuteReader();
            List<Machine> machineList = new List<Machine>();

            Machine m = new Machine();
            m.TotalMachine = count.ToString();
            machineList.Add(m);

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Machine machine = new Machine();
                    machine.MCount = dr["NUMBER"].ToString().Trim();
                    machine.MachineID = dr["M_MACH_ID"].ToString().Trim();
                    machine.MState = dr["M_STATE"].ToString().Trim();
                    machine.MDistrict = dr["M_DISTRICT"].ToString().Trim();
                    machine.MBranchNo = dr["M_BRANCH_CODE"].ToString().Trim();
                    machine.MBranchName = dr["M_BRANCH_NAME"].ToString().Trim();
                    machine.MPIC = dr["M_CONTACT"].ToString().Trim();

                    machine.CurrentVer = dr["M_VERSION_ID"].ToString();
                    machine.CalendarName = dr["CALENDAR_NAME"].ToString().Trim();

                    machineList.Add(machine);
                }
            }
            dr.Close();

            dbc.closeConn();

            return machineList.ToArray();
        }


        [WebMethod, ScriptMethod]
        public static Machine[] editMach(string machid)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            //cmd.CommandText = "SELECT M_MACH_STATE, M_MACH_CITYDISTRICT, M_BRANCH_NO, M_CONTACT_PERSON " +
            //                    " FROM M_MACHINE_LIST WHERE M_MACH_ID = '" + machid + "'";
            cmd.CommandText = "SELECT M_VENDOR_ID, CALENDAR_ID, M_STATE, M_DISTRICT, M_ADDRESS1, M_ADDRESS2, M_TEL, M_CONTACT, " +
                                "M.M_BRANCH_CODE, M_BRANCH_NAME, B.M_CUST_CODE " +
                                "FROM M_MACHINE_LIST M, M_BRANCH B " +
                                "WHERE M.M_BRANCH_CODE = B.M_BRANCH_CODE " +
                                "AND M_MACH_ID = '" + machid + "'";

            SqlDataReader dr = cmd.ExecuteReader();
            List<Machine> details = new List<Machine>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string sBranchName = "";
                    if (!string.IsNullOrEmpty(dr["M_BRANCH_NAME"].ToString()))
                    {
                        sBranchName = dr["M_BRANCH_NAME"].ToString();
                    }
                    Machine mach = new Machine();
                    mach.MState = dr["M_STATE"].ToString();
                    mach.MDistrict = dr["M_DISTRICT"].ToString();
                    mach.MCalendar = dr["CALENDAR_ID"].ToString();
                    mach.MAddress1 = dr["M_ADDRESS1"].ToString();
                    mach.MAddress2 = dr["M_ADDRESS2"].ToString();
                    mach.MBranchNo = dr["M_BRANCH_CODE"].ToString();
                    mach.MBranchName = sBranchName;
                    mach.MTel = dr["M_TEL"].ToString();
                    mach.MPIC = dr["M_CONTACT"].ToString();
                    mach.MVendor = dr["M_VENDOR_ID"].ToString();
                    mach.CustCode = dr["M_CUST_CODE"].ToString();
                    details.Add(mach);
                }
            }
            return details.ToArray();
        }

        [WebMethod]
        public static McObject<List<Calendar>> GetCalendar()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT CALENDAR_ID, CALENDAR_NAME FROM HOLIDAY_CALENDAR_ID WHERE CALENDAR_ID NOT IN('1')";
                SqlDataReader dr = cmd.ExecuteReader();
                List<Calendar> CalendarList = new List<Calendar>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Calendar calendar = new Calendar();
                        calendar.ID = dr["CALENDAR_ID"].ToString().Trim();
                        calendar.Name = dr["CALENDAR_NAME"].ToString().Trim();
                        CalendarList.Add(calendar);
                    }
                }
                dr.Close();

                return new McObject<List<Calendar>>(true, "Successful.", CalendarList);
            }
            catch (Exception ex)
            {
                return new McObject<List<Calendar>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }
        
        [WebMethod]
        public static McObject<List<Vendor>> GetVendor()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT M_VENDOR_ID, M_VENDOR_NAME FROM M_VENDOR_LIST";
                SqlDataReader dr = cmd.ExecuteReader();
                List<Vendor> VendorList = new List<Vendor>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Vendor v = new Vendor();
                        v.ID = dr["M_VENDOR_ID"].ToString();
                        v.Name = dr["M_VENDOR_NAME"].ToString();

                        VendorList.Add(v);
                    }
                }
                dr.Close();

                return new McObject<List<Vendor>>(true, "Successful.", VendorList);
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

        [WebMethod]
        public static McObject<List<Customer>> GetCustomer()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                cmd.CommandText = "SELECT * FROM M_CUSTOMER";
                SqlDataReader dr = cmd.ExecuteReader();
                List<Customer> CustomerList = new List<Customer>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Customer c = new Customer();
                        c.Code = dr["M_CUST_CODE"].ToString();
                        c.Name = dr["M_CUST_NAME"].ToString();
                        c.Country = dr["M_MACH_COUNTRY"].ToString();

                        CustomerList.Add(c);
                    }
                }
                dr.Close();

                return new McObject<List<Customer>>(true, "Successful.", CustomerList);
            }
            catch (Exception ex)
            {
                return new McObject<List<Customer>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<State>> GetState(string CustCode)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                if (string.IsNullOrEmpty(CustCode))
                {
                    return new McObject<List<State>>(true, "Successful. No record.");
                }
                cmd.CommandText = "SELECT DISTINCT M_STATE FROM M_BRANCH WHERE M_CUST_CODE = @CustCode";
                cmd.Parameters.AddWithValue("CustCode", CustCode);
                SqlDataReader dr = cmd.ExecuteReader();
                List<State> StateList = new List<State>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        State s = new State();
                        s.Name = dr["M_STATE"].ToString();

                        StateList.Add(s);
                    }
                }
                dr.Close();

                return new McObject<List<State>>(true, "Successful.", StateList);
            }
            catch (Exception ex)
            {
                return new McObject<List<State>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<District>> GetDistrict(string CustCode, string State)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                if (string.IsNullOrEmpty(CustCode) || string.IsNullOrEmpty(State))
                {
                    return new McObject<List<District>>(true, "Successful. No record.");
                }
                cmd.CommandText = "SELECT DISTINCT M_DISTRICT FROM M_BRANCH WHERE M_CUST_CODE = @CustCode AND M_STATE = @State";
                cmd.Parameters.AddWithValue("CustCode", CustCode);
                cmd.Parameters.AddWithValue("State", State);
                SqlDataReader dr = cmd.ExecuteReader();
                List<District> DistrictList = new List<District>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        District d = new District();
                        d.Name = dr["M_DISTRICT"].ToString();

                        DistrictList.Add(d);
                    }
                }
                dr.Close();

                return new McObject<List<District>>(true, "Successful.", DistrictList);
            }
            catch (Exception ex)
            {
                return new McObject<List<District>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<City>> GetCity(string CustCode, string State, string District)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                if (string.IsNullOrEmpty(CustCode) || string.IsNullOrEmpty(State) || string.IsNullOrEmpty(District))
                {
                    return new McObject<List<City>>(true, "Successful. No record.");
                }
                cmd.CommandText = "SELECT DISTINCT M_CITY FROM M_BRANCH WHERE M_CUST_CODE = @CustCode AND M_STATE = @State AND M_DISTRICT = @District";
                cmd.Parameters.AddWithValue("CustCode", CustCode);
                cmd.Parameters.AddWithValue("State", State);
                cmd.Parameters.AddWithValue("District", District);
                SqlDataReader dr = cmd.ExecuteReader();
                List<City> CityList = new List<City>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        City c = new City();
                        c.Name = dr["M_CITY"].ToString();

                        CityList.Add(c);
                    }
                }
                dr.Close();

                return new McObject<List<City>>(true, "Successful.", CityList);
            }
            catch (Exception ex)
            {
                return new McObject<List<City>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<List<Branch>> GetBranch(string CustCode, string State, string District)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            try
            {
                if (string.IsNullOrEmpty(CustCode) || string.IsNullOrEmpty(State) || string.IsNullOrEmpty(District))
                {
                    return new McObject<List<Branch>>(true, "Successful. No record.");
                }
                cmd.CommandText = "SELECT DISTINCT M_BRANCH_CODE, M_BRANCH_NAME, M_CONTACT, M_TEL FROM M_BRANCH WHERE M_CUST_CODE = @CustCode AND M_STATE = @State AND M_DISTRICT = @District";
                cmd.Parameters.AddWithValue("CustCode", CustCode);
                cmd.Parameters.AddWithValue("State", State);
                cmd.Parameters.AddWithValue("District", District);
                //cmd.Parameters.AddWithValue("City", City);
                SqlDataReader dr = cmd.ExecuteReader();
                List<Branch> BranchList = new List<Branch>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Branch b = new Branch();
                        b.Code = dr["M_BRANCH_CODE"].ToString();
                        b.Name = dr["M_BRANCH_NAME"].ToString();
                        b.PIC = dr["M_CONTACT"].ToString();
                        b.ContactNo = dr["M_TEL"].ToString();

                        BranchList.Add(b);
                    }
                }
                dr.Close();

                return new McObject<List<Branch>>(true, "Successful.", BranchList);
            }
            catch (Exception ex)
            {
                return new McObject<List<Branch>>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }

        [WebMethod]
        public static McObject<Boolean> UpdateMachine(string MachId, string BranchCode, string VendorId, string VendorName, string CalendarId, string CalendarName)
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
                string action = MCAction.MODIFY;
                int status = MCStatus.PENDING;

                string cID = (string.IsNullOrEmpty(CalendarId)) ? "NULL" : CalendarId;
                string vID = (string.IsNullOrEmpty(VendorId)) ? "NULL" : VendorId;

                string sql = "UPDATE M_MACHINE_LIST SET M_BRANCH_CODE = '" + BranchCode + "', M_VENDOR_ID = " + vID + ", CALENDAR_ID = " + cID + " " +
                                    "WHERE M_MACH_ID = '" + MachId + "'";

                cmd.CommandText = "SELECT M_STATE, M_DISTRICT, M_BRANCH_CODE, M_BRANCH_NAME " +
                                    "FROM M_BRANCH " +
                                    "WHERE M_BRANCH_CODE = @BranchCode";
                cmd.Parameters.AddWithValue("BranchCode", BranchCode);
                dr = cmd.ExecuteReader();

                Machine m = new Machine();
                m.CalendarName = (string.IsNullOrEmpty(CalendarId)) ? "-" : CalendarName;
                m.MVendor = (string.IsNullOrEmpty(VendorId)) ? "-" : VendorName;

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {             
                        m.MState = dr["M_STATE"].ToString();
                        m.MDistrict = dr["M_DISTRICT"].ToString();
                        m.MBranchNo = dr["M_BRANCH_CODE"].ToString();
                        m.MBranchName = dr["M_BRANCH_NAME"].ToString();
                    }
                }
                dr.Close();

                JsonObject NEW = new JsonObject();
                NEW.Add("Kiosk ID", MachId);
                NEW.Add("Vendor", m.MVendor);
                NEW.Add("Calendar", m.CalendarName);
                NEW.Add("Branch", m.MBranchNo + " " + m.MBranchName);
                NEW.Add("State", m.MState);
                NEW.Add("District", m.MDistrict);
                //NEW Data populate completed


                //OLD DATA starts here
                cmd.CommandText = "SELECT M_VENDOR_NAME, ML.M_BRANCH_CODE, M_BRANCH_NAME, M_STATE, M_DISTRICT, CALENDAR_NAME " +
                                    "FROM M_MACHINE_LIST ML " +
                                    "LEFT JOIN M_VENDOR_LIST AS VL ON ML.M_VENDOR_ID = VL.M_VENDOR_ID " +
                                    "LEFT JOIN HOLIDAY_CALENDAR_ID AS HC ON ML.CALENDAR_ID = HC.CALENDAR_ID " +
                                    "LEFT JOIN M_BRANCH AS B ON ML.M_BRANCH_CODE = B.M_BRANCH_CODE " +
                                    "WHERE M_MACH_ID = @MachId";
                cmd.Parameters.AddWithValue("MachId", MachId);
                dr = cmd.ExecuteReader();
                Machine OldM = new Machine();
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        OldM.MVendor = dr["M_VENDOR_NAME"].ToString();
                        OldM.MBranchNo = dr["M_BRANCH_CODE"].ToString();
                        OldM.MBranchName = dr["M_BRANCH_NAME"].ToString();
                        OldM.MState = dr["M_STATE"].ToString();
                        OldM.MDistrict = dr["M_DISTRICT"].ToString();
                        OldM.CalendarName = dr["CALENDAR_NAME"].ToString();
                    }
                }
                dr.Close();
                

                JsonObject OLD = new JsonObject();
                OLD.Add("Kiosk ID", MachId);
                OLD.Add("Vendor", OldM.MVendor);
                OLD.Add("Calendar", OldM.CalendarName);
                OLD.Add("Branch", OldM.MBranchNo + " " + OldM.MBranchName);
                OLD.Add("State", OldM.MState);
                OLD.Add("District", OldM.MDistrict);

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), @olddata, @newdata, '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("newdata", newdata);
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
    }
}