using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Script.Services;
using System.Web.Services;
using System.Globalization;
using RototypeIntl.Database;
using System.Configuration;
using System.IO;
using System.Json;
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;

namespace Maestro.views.kioskMaintenance.holidayMaintenance
{
    public partial class main : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
        }
        
        public class HolidayCalendar
        {
            public string HolidayDate { get; set; }
            public string DayOfMonthID { get; set; }
            public string MonthOfYearID { get; set; }
            public string HolidayYear { get; set; }
            public string DayOfWeekID { get; set; }
            public string CalendarID { get; set; }
            public string CalendarName { get; set; }
            public string HolidayName { get; set; }
            public string HolidayNote { get; set; }
        }

        [WebMethod]
        public static String getFirstDayOfWeek()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            cmd.CommandText = "SELECT DAY_OF_WEEK_NAME FROM DAY_OF_WEEK WHERE DAY_OF_WEEK_ID = '1'";
            SqlDataReader dr = cmd.ExecuteReader();

            string dayOfWeekName = "";

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    dayOfWeekName = dr["DAY_OF_WEEK_NAME"].ToString().Trim();
                }
            }
            dr.Close();
            dbc.closeConn();

            return dayOfWeekName;
        }

        [WebMethod]
        public static McObject<List<HolidayCalendar>> getHolidayCalendar(string calendarName)
        {
            try
            {
                string defaultCalendar = "1"; //National
                McObject<String> mcObj = getCalendarID(calendarName);
                if (!mcObj.isSuccessful())
                {
                    throw new Exception(mcObj.getMessage());
                }

                string calendarID = mcObj.getObject();

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();

                cmd.CommandText = "SELECT HOLIDAY_DATE, CALENDAR_ID, HOLIDAY_NAME, HOLIDAY_NOTE FROM HOLIDAY_CALENDAR "
                                   + "WHERE CALENDAR_ID IN(@firstCondition, @secondCondition)";

                cmd.Parameters.AddWithValue("firstCondition", defaultCalendar); //Always grab national holiday
                cmd.Parameters.AddWithValue("secondCondition", calendarID); //State Holiday

                SqlDataReader dr = cmd.ExecuteReader();
                List<HolidayCalendar> holidayList = new List<HolidayCalendar>();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        HolidayCalendar holiday = new HolidayCalendar();

                        DateTime dt = DateTime.Parse(dr["HOLIDAY_DATE"].ToString().Trim());
                        //string holdiayDate = String.Format("{0:dd/MM/yyyy HH:mm:ss}", dt);
                        string holdiayDate = dt.ToString("dd/MM/yyyy HH:mm:ss");

                        holiday.HolidayDate = holdiayDate;
                        holiday.CalendarID = dr["CALENDAR_ID"].ToString().Trim();
                        holiday.HolidayName = dr["HOLIDAY_NAME"].ToString().Trim();
                        holiday.HolidayNote = dr["HOLIDAY_NOTE"].ToString().Trim();
                        holidayList.Add(holiday);
                    }
                }
                dr.Close();
                dbc.closeConn();

                return new McObject<List<HolidayCalendar>>(true, "Successful.", holidayList);
            }
            catch (Exception ex)
            {
                return new McObject<List<HolidayCalendar>>(false, "Failed! " + ex.Message);
            }
        }

        public static McObject<String> getCalendarID(string calendarName)
        {
            try
            {
                string calID = "";

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT CALENDAR_ID FROM HOLIDAY_CALENDAR_ID WHERE CALENDAR_NAME = @calendarName";
                cmd.Parameters.AddWithValue("calendarName", calendarName);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        calID = dr["CALENDAR_ID"].ToString().Trim();
                    }
                }

                return new McObject<String>(true, "Successful", calID);
            }
            catch(Exception ex)
            {
                return new McObject<String>(false, ex.Message);
            }
        }

        [WebMethod]
        public static McObject<Boolean> insertHoliday(string holidayDate, string dayOfMonthID, string monthOfYearID, string holidayYear,
            string dayOfWeekID, string calendarName, string holidayName, string holidayNote)
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

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.ADD;
                int status = MCStatus.PENDING;
                string sql = "";

                DateTime holidayDT = DateTime.ParseExact(holidayDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime holidayDT = Convert.ToDateTime(holidayDate);

                McObject<String> mcObj = getCalendarID(calendarName);
                if (!mcObj.isSuccessful())
                {
                    throw new Exception(mcObj.getMessage());
                }

                string calendarID = mcObj.getObject();
                
                if (String.IsNullOrEmpty(calendarID))
                {
                    sql = "DECLARE @id int " +
                          "INSERT INTO HOLIDAY_CALENDAR_ID(CALENDAR_NAME) VALUES ('" + calendarName + "') " +
                          "SET @id = SCOPE_IDENTITY() " +
                          "INSERT INTO HOLIDAY_CALENDAR " +
                          "VALUES ('" + holidayDT.ToString("yyyy/MM/dd") + "', @id, " + dayOfMonthID + ", " + monthOfYearID + ", " + holidayYear + ", " + dayOfWeekID + ", " +
                          "'" + holidayName + "', '" + holidayNote + "', GetDate())";
                }
                else
                {
                    sql = "INSERT INTO HOLIDAY_CALENDAR " +
                          "VALUES ('" + holidayDT.ToString("yyyy/MM/dd") + "', " + calendarID + ", " + dayOfMonthID + ", " + monthOfYearID + ", " + holidayYear + ", " + dayOfWeekID + ", " +
                          "'" + holidayName + "', '" + holidayNote + "', GetDate())";

                }

                JsonObject NEW = new JsonObject();
                NEW.Add("Date", holidayDT.ToString("dd/MM/yyyy"));
                NEW.Add("Calendar", calendarName);
                NEW.Add("Holdiay", holidayName);
                NEW.Add("Description", (String.IsNullOrEmpty(holidayNote)) ? "-" : holidayNote);

                string newdata = NEW.ToString();


                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(),'" + newdata + "', '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();
                dbc.closeConn();

                return new McObject<Boolean>(true, "Successful.");
            }
            catch(Exception ex)
            {
                return new McObject<Boolean>(false, "Failed! "+ ex.Message); 
            }
        }

        [WebMethod]
        public static HolidayCalendar[] getHolidayDetail(string holidayDate, string calendarID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            cmd.CommandText = "SELECT HOLIDAY_DATE, HOLIDAY_NAME, HOLIDAY_NOTE FROM HOLIDAY_CALENDAR WHERE HOLIDAY_DATE = @holidayDate AND CALENDAR_ID = @calendarID";

            DateTime holidayDT = DateTime.ParseExact(holidayDate,"dd/MM/yyyy", CultureInfo.InvariantCulture);
            cmd.Parameters.AddWithValue("holidayDate", holidayDT);
            cmd.Parameters.AddWithValue("calendarID", calendarID);

            SqlDataReader dr = cmd.ExecuteReader();
            List<HolidayCalendar> holidayList = new List<HolidayCalendar>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    HolidayCalendar holiday = new HolidayCalendar();

                    DateTime dt = DateTime.Parse(dr["HOLIDAY_DATE"].ToString().Trim());
                    string holdiayDate = dt.ToString("dd-MM-yyyy HH:mm:ss");

                    holiday.HolidayDate = holdiayDate;
                    holiday.HolidayName = dr["HOLIDAY_NAME"].ToString().Trim();
                    holiday.HolidayNote = dr["HOLIDAY_NOTE"].ToString().Trim();
                    holidayList.Add(holiday);
                }
            }
            dr.Close();
            dbc.closeConn();

            return holidayList.ToArray();
        }

        [WebMethod]
        public static McObject<Boolean> updateHoliday(string holidayDate, string calendarID, string holidayName, string holidayNote)
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

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.MODIFY;
                int status = MCStatus.PENDING;

                DateTime holidayDT = DateTime.ParseExact(holidayDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                string sql = "UPDATE HOLIDAY_CALENDAR SET HOLIDAY_NAME = '" + holidayName + "', HOLIDAY_NOTE = '" + holidayNote + "' WHERE HOLIDAY_DATE = '" + holidayDT.ToString("yyyy/MM/dd") + "' AND CALENDAR_ID = " + calendarID;                              

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT HCI.CALENDAR_NAME, HC.HOLIDAY_NAME, HC.HOLIDAY_NOTE FROM HOLIDAY_CALENDAR HC, HOLIDAY_CALENDAR_ID HCI "
                                    +"WHERE HC.CALENDAR_ID = HCI.CALENDAR_ID AND HC.CALENDAR_ID = " + calendarID + " AND HOLIDAY_DATE = '" + holidayDT.ToString("yyyy/MM/dd") + "'";
                SqlDataReader dr = cmd.ExecuteReader();

                string oldHolidayName = "", oldHolidayNote = "", calendarNameDesc = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldHolidayName = dr["HOLIDAY_NAME"].ToString();
                        oldHolidayNote = dr["HOLIDAY_NOTE"].ToString();
                        calendarNameDesc = dr["CALENDAR_NAME"].ToString();
                    }
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("Date", holidayDT.ToString("dd/MM/yyyy"));
                OLD.Add("Calendar", calendarNameDesc);
                OLD.Add("Holiday", oldHolidayName);
                OLD.Add("Description", (String.IsNullOrEmpty(oldHolidayNote)) ? "-" : oldHolidayNote);

                JsonObject NEW = new JsonObject();
                NEW.Add("Date", holidayDT.ToString("dd/MM/yyyy"));
                NEW.Add("Calendar", calendarNameDesc);
                NEW.Add("Holdiay", holidayName);
                NEW.Add("Description", (String.IsNullOrEmpty(holidayNote)) ? "-" : holidayNote);

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + olddata + "', '" + newdata + "', '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();
                dbc.closeConn();

                return new McObject<Boolean>(true, "Successful.");
            }
            catch (Exception ex)
            {
                return new McObject<Boolean>(false, "Failed! " + ex.Message);
            }
        }

        [WebMethod]
        public static McObject<Boolean> deleteHoliday(string holidayDate, string calendarID)
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

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.DELETE;
                int status = MCStatus.PENDING;

                DateTime holidayDT = DateTime.ParseExact(holidayDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                string sql = "DELETE FROM HOLIDAY_CALENDAR WHERE HOLIDAY_DATE = '" + holidayDT.ToString("yyyy/MM/dd") + "' AND CALENDAR_ID = " + calendarID;

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT HCI.CALENDAR_NAME, HC.HOLIDAY_NAME, HC.HOLIDAY_NOTE FROM HOLIDAY_CALENDAR HC, HOLIDAY_CALENDAR_ID HCI "
                                    +"WHERE HC.CALENDAR_ID = HCI.CALENDAR_ID AND HC.CALENDAR_ID = " + calendarID + " AND HOLIDAY_DATE = '" + holidayDT.ToString("yyyy/MM/dd") + "'";
                SqlDataReader dr = cmd.ExecuteReader();

                string oldHolidayName = "", oldHolidayNote = "", calendarNameDesc = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldHolidayName = dr["HOLIDAY_NAME"].ToString();
                        oldHolidayNote = dr["HOLIDAY_NOTE"].ToString();
                        calendarNameDesc = dr["CALENDAR_NAME"].ToString();
                    }
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("Date", holidayDT.ToString("dd/MM/yyyy"));
                OLD.Add("Calendar", calendarNameDesc);
                OLD.Add("Holiday", oldHolidayName);
                OLD.Add("Description", (String.IsNullOrEmpty(oldHolidayNote)) ? "-" : oldHolidayNote);

                string olddata = OLD.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + olddata + "', '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();
                dbc.closeConn();

                return new McObject<Boolean>(true, "Successful.");
            }
            catch (Exception ex)
            {
                //log
                return new McObject<Boolean>(false, "Failed! " + ex.Message) ;
            }
        }

        [WebMethod]
        public static HolidayCalendar[] getHolidayList(string holidayYear)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            cmd.CommandText = "SELECT HC.HOLIDAY_DATE, HC.HOLIDAY_NAME, HC.HOLIDAY_NOTE, HCI.CALENDAR_ID, HCI.CALENDAR_NAME FROM HOLIDAY_CALENDAR HC, HOLIDAY_CALENDAR_ID HCI "
                                +"WHERE HOLIDAY_YEAR = @holidayYear AND HC.CALENDAR_ID = HCI.CALENDAR_ID";

            cmd.Parameters.AddWithValue("holidayYear", holidayYear);

            SqlDataReader dr = cmd.ExecuteReader();
            List<HolidayCalendar> holidayList = new List<HolidayCalendar>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    HolidayCalendar holiday = new HolidayCalendar();

                    DateTime dt = DateTime.Parse(dr["HOLIDAY_DATE"].ToString().Trim());
                    //string holdiayDate = String.Format("{0:dd/MM/yyyy HH:mm:ss}", dt);
                    string holdiayDate = dt.ToString("dd/MM/yyyy HH:mm:ss");

                    holiday.HolidayDate = holdiayDate;
                    holiday.HolidayName = dr["HOLIDAY_NAME"].ToString().Trim();
                    holiday.HolidayNote = dr["HOLIDAY_NOTE"].ToString().Trim();
                    holiday.CalendarID = dr["CALENDAR_ID"].ToString().Trim();
                    holiday.CalendarName = dr["CALENDAR_NAME"].ToString().Trim();
                    holidayList.Add(holiday);
                }
            }
            dr.Close();
            dbc.closeConn();

            return holidayList.ToArray();
        }

        [WebMethod]
        public static HolidayCalendar[] getSpecificCalendar()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT CALENDAR_ID, CALENDAR_NAME FROM HOLIDAY_CALENDAR_ID WHERE CALENDAR_ID NOT IN(1)";

            SqlDataReader dr = cmd.ExecuteReader();
            List<HolidayCalendar> holidayList = new List<HolidayCalendar>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    HolidayCalendar holiday = new HolidayCalendar();
                    holiday.CalendarID = dr["CALENDAR_ID"].ToString().Trim();
                    holiday.CalendarName = dr["CALENDAR_NAME"].ToString().Trim();
                    holidayList.Add(holiday);
                }
            }

            dr.Close();
            dbc.closeConn();

            return holidayList.ToArray();
        }
    }
}