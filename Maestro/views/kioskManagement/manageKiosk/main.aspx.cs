using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data.SqlClient;

namespace Maestro.views.kioskManagement.manageKiosk
{
    public partial class main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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

        public class Machine{
            public string machID { get; set; }
            public string calID { get; set; }
        }

        public class Calendar
        {
            public string calendarID { get; set; }
            public string calendarName { get; set; }
        }

        [WebMethod]
        public static Machine[] getMachines()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT M_MACH_ID FROM M_MACHINE_LIST";
            SqlDataReader dr = cmd.ExecuteReader();
            List<Machine> machineList = new List<Machine>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Machine machine = new Machine();
                    machine.machID = dr["M_MACH_ID"].ToString().Trim();
                    machineList.Add(machine);
                }
            }

            dr.Close();
            dbc.closeConn();

            return machineList.ToArray();
        }

        [WebMethod]
        public static Machine[] searchMachine(string pattern)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT M_MACH_ID FROM M_MACHINE_LIST WHERE M_MACH_ID LIKE '" + pattern + "%'";
            SqlDataReader dr = cmd.ExecuteReader();
            List<Machine> machineList = new List<Machine>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Machine m = new Machine();
                    m.machID = dr["M_MACH_ID"].ToString().Trim();
                    machineList.Add(m);
                }
            }

            dr.Close();
            dbc.closeConn();

            return machineList.ToArray();
        }

        [WebMethod]
        public static Calendar[] getCalendar()
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT CALENDAR_ID, CALENDAR_NAME FROM HOLIDAY_CALENDAR_ID WHERE CALENDAR_ID NOT IN('1')";
            SqlDataReader dr = cmd.ExecuteReader();
            List<Calendar> Calendar = new List<Calendar>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Calendar calendar = new Calendar();
                    calendar.calendarID = dr["CALENDAR_ID"].ToString().Trim();
                    calendar.calendarName = dr["CALENDAR_NAME"].ToString().Trim();
                    Calendar.Add(calendar);
                }
            }

            dr.Close();
            dbc.closeConn();

            return Calendar.ToArray();
        }

        [WebMethod]
        public static Machine[] getMachineDetail(string mID)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();
            cmd.CommandText = "SELECT CALENDAR_ID FROM M_MACHINE_LIST WHERE M_MACH_ID = @mID";
            cmd.Parameters.AddWithValue("mID", mID);

            SqlDataReader dr = cmd.ExecuteReader();
            List<Machine> machineList = new List<Machine>();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Machine machine = new Machine();
                    machine.calID = dr["CALENDAR_ID"].ToString().Trim();
                    machineList.Add(machine);
                }
            }

            dr.Close();
            dbc.closeConn();

            return machineList.ToArray();
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
        public static Result updateMachine(string mID, string cal)
        {
            try
            {
                int isInt;
                bool existed = validateMachID(mID);

                if(!existed)
                {
                    return new Result(false, "Sorry. The update was unsuccesful due to server unable to find the machine ID. Please try again or contact system administrator.");
                }
                else if (!Int32.TryParse(cal, out isInt))
                {
                    return new Result(false, "Sorry. The update was unsuccessful due to calendar value is invalid. Please try again or contact system administrator.");
                }
                else
                {
                    dbconn dbc = new dbconn();
                    dbc.connDB();
                    SqlCommand cmd = dbc.conn.CreateCommand();
                    cmd.CommandText = "UPDATE M_MACHINE_LIST SET CALENDAR_ID = @cal WHERE M_MACH_ID = @mID";
                    cmd.Parameters.AddWithValue("cal", cal);
                    cmd.Parameters.AddWithValue("mID", mID);
                    cmd.ExecuteNonQuery();

                    return new Result(true, "Successful. The machine has been updated.");
                }
            }
            catch (Exception ex)
            {
                return new Result(false, ex.ToString());
            }
        }
    }
}