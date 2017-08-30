using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Services;
using System.Web.Services;
using System.Json;
using Maestro.Classes;
using Maestro.Classes.MakerChecker;
using Maestro.Classes.Prefix;
using Newtonsoft.Json;

namespace Maestro.views.kioskMaintenance.businessDay
{
    public partial class main : System.Web.UI.Page
    {
        private static string taskName = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            taskName = Request.Params["task"];
        }

        public class Day
        {
            public string DayID { get; set; }
            public string DayName { get; set; }
            public string IsBusinessDay { get; set; }
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static Day[] getDayList()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

            cmd.CommandText = "SELECT * FROM DAY_OF_WEEK";

            SqlDataReader dr = cmd.ExecuteReader();
            List<Day> dayList = new List<Day>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Day day = new Day();
                    day.DayID = dr["DAY_OF_WEEK_ID"].ToString().Trim();
                    day.DayName = dr["DAY_OF_WEEK_NAME"].ToString().Trim();
                    day.IsBusinessDay = dr["IS_BUSINESSDAY"].ToString().Trim();

                    dayList.Add(day);
                }
            }
            dr.Close();
            dbc.closeConn();

            return dayList.ToArray();
        }

        [WebMethod, System.Web.Script.Services.ScriptMethod]
        public static McObject<Boolean> saveCalendarSetting(object[] aWeek)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();

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
                string sql = "";
                JsonObject OLD = new JsonObject();
                JsonObject NEW = new JsonObject();

                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM DAY_OF_WEEK";
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    int dayCount = 1;
                    while (dr.Read())
                    {
                        string businessDay = (dr.GetBoolean(dr.GetOrdinal("IS_BUSINESSDAY"))) ? "[Business Day]" : "";
                        OLD.Add("Day " + (dayCount++), dr["DAY_OF_WEEK_NAME"].ToString() + " " + businessDay);
                    }
                }
                else
                {
                    throw new Exception("No record found.");
                }
                dr.Close();

                int count = 1;

                foreach (Dictionary<string, object> temp in aWeek)
                {
                    object dayID;
                    object dayName;
                    object isBusinessDay;
                    temp.TryGetValue("dayID", out dayID);
                    temp.TryGetValue("dayName", out dayName);
                    temp.TryGetValue("isBusinessDay", out isBusinessDay);

                    int iDayID = int.Parse(dayID.ToString());
                    sql = sql + "UPDATE DAY_OF_WEEK SET DAY_OF_WEEK_NAME = '" + dayName.ToString() + "', IS_BUSINESSDAY = '" + isBusinessDay.ToString() + "' WHERE DAY_OF_WEEK_ID = '" + iDayID + "'";
                    sql = sql + MCDelimiter.SQL;


                    string businessDay = (Boolean.Parse(isBusinessDay.ToString())) ? "[Business Day]" : "";
                    NEW.Add("Day " + (count++), dayName.ToString() + " " + businessDay);
                }

                if (String.IsNullOrEmpty(sql))
                {
                    throw new Exception("Invalid SQL statement!");
                }

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + olddata + "', '" + newdata + "', '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();
                dbc.closeConn();

                return new McObject<bool>(true, "Successful.");
            }
            catch (Exception ex)
            {
                return new McObject<bool>(false, "Failed! " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }
    }
}