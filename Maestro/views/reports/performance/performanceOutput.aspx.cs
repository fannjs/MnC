using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Maestro.views.reports.performance
{
    public partial class performanceOutput : System.Web.UI.Page
    {
        string prvMachid = "";
        string prvMdate = "";
        string prvMcode = "";
        string prvMtime = "";
        string prvMerrtype = "";
        string prvMachtype = "";

        private void resetPrevRecord()
        {
            prvMachid = "";
            prvMdate = "";
            prvMcode = "";
            prvMtime = "";
            prvMerrtype = "";
            prvMachtype = "";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                int ttlHrsMach = 0;
                string startDate = Request.QueryString["startDate"]; //2014-10-27
                string endDate = Request.QueryString["endDate"];
                string machType = Request.QueryString["machType"];
                string machID = Request.QueryString["machID"];
                string chartType = Request.QueryString["chartType"];
                string userName = Request.QueryString["userName"];

                if (string.IsNullOrEmpty(endDate))
                {
                    endDate = DateTime.Now.ToString("yyyy-MM-dd");
                }
                if (string.IsNullOrEmpty(startDate))
                {
                    startDate = endDate;
                }

                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();

                try
                {

                    string sql = "SELECT count(*) as ttlmach FROM M_MACHINE_LIST ";

                    if (!string.IsNullOrEmpty(machType) && machType != "All")
                    {
                        sql = sql + "WHERE M_MACH_TYPE = '" + machType + "' ";

                        if (!string.IsNullOrEmpty(machID) && machID != "All")
                        {
                            sql = sql + " AND M_MACH_ID = '" + machID + "' ";
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(machID) && machID != "All")
                        {
                            sql = sql + "WHERE M_MACH_ID = '" + machID + "' ";
                        }
                    }


                    cmd.CommandText = sql;
                    SqlDataReader dr = cmd.ExecuteReader();
                    int ttlmach = 0;
                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            ttlmach = (int)dr["ttlmach"];
                        }
                    }
                    dr.Close();

                    ttlHrsMach = 24 * ttlmach; // 

                    //////////////////// count error dowmtime ///////////////
                    //" WHERE (MR.M_DATE >='2014-10-27' AND MR.M_DATE <='2014-11-13') "+
                    sql = " SELECT MR.M_DATE, MR.M_MACH_ID, MR.M_TIME, MR.M_CODE, MC.M_ERRORTYPE, MC.M_MACH_TYPE  " +
                    " FROM  M_CODES as MC " +
                    " INNER JOIN M_REPORTING_DATA as MR " +
                    " ON MC.M_CODE = MR.M_CODE " +
                    " WHERE (MR.M_DATE >='" + startDate + "' AND MR.M_DATE <='" + endDate + "') ";

                    if (!string.IsNullOrEmpty(machID) && machID != "All")
                    {
                        sql = sql + " AND MR.M_MACH_ID = '" + machID + "' ";
                    }
                    if (!string.IsNullOrEmpty(machType) && machType != "All")
                    {
                        sql = sql + " AND MC.M_MACH_TYPE = '" + machType + "' ";
                    }

                    sql = sql + " AND (MC.M_ERRORTYPE='ERROR' OR MC.M_ERRORTYPE='OK') " +
                    " ORDER BY MR.M_DATE, MR.M_MACH_ID, MR.M_TIME ";

                    cmd.CommandText = sql;
                    dr = cmd.ExecuteReader();
                    resetPrevRecord();


                    string machid = "";
                    string mdate = "";
                    string mcode = "";
                    string mtime = "";
                    string merrtype = "";
                    string machtype = "";

                    // Create a data table with the same name of crystal report's table.
                    DataTable performanceTable = new DataTable("table1");
                    // Create table columns with the same fields name of crystal report's table.
                    //performanceTable.Columns.Add("machID");
                    //performanceTable.Columns.Add("machDate");
                    //performanceTable.Columns.Add("machType");
                    //performanceTable.Columns.Add("Downtime");
                    performanceTable.Columns.Add("Date");
                    performanceTable.Columns.Add("KioskID");
                    performanceTable.Columns.Add("Error");
                    performanceTable.Columns.Add("Warning");
                    performanceTable.Columns.Add("Offline");
                    performanceTable.Columns.Add("Normal");

                    if (dr.HasRows)
                    {

                        while (dr.Read())
                        {
                            machid = dr["M_MACH_ID"].ToString();
                            mdate = dr["M_DATE"].ToString();
                            mcode = dr["M_CODE"].ToString();
                            mtime = dr["M_TIME"].ToString();
                            merrtype = dr["M_ERRORTYPE"].ToString();
                            machtype = dr["M_MACH_TYPE"].ToString();

                            if (string.IsNullOrEmpty(prvMerrtype) && merrtype == "ERROR")
                            {   // first time null, then set prevRecord
                                prvMachid = machid;
                                prvMdate = mdate;
                                prvMcode = mcode;
                                prvMtime = mtime;
                                prvMerrtype = merrtype;
                                prvMachtype = machtype;
                            }
                            else if (string.IsNullOrEmpty(prvMerrtype) && merrtype == "OK")
                            {   // first record ok, just count today downtime. 
                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                double diffCurrtime = curTime.TotalHours;

                                DataRow row = performanceTable.NewRow();
                                row["KioskID"] = machid;
                                row["Date"] = mdate;
                                row["Error"] = diffCurrtime;
                                performanceTable.Rows.Add(row);
                            }
                            else if (prvMdate == mdate && merrtype == "OK" && prvMachid == machid)
                            {   // count downtime in same day
                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double difftime = (curTime.TotalMinutes - prvTime.TotalMinutes) / 60; // convert to hour basic

                                DataRow row = performanceTable.NewRow();
                                row["KioskID"] = machid;
                                row["Date"] = mdate;
                                row["Error"] = difftime;
                                performanceTable.Rows.Add(row);
                                resetPrevRecord();
                            }
                            else if (prvMdate != mdate && merrtype == "OK" && prvMachid == machid)
                            {   // same machine, count downtime in diff day.
                                // if record date no same prv date, count downtime for prv record.
                                DateTime curDate = Convert.ToDateTime(mdate);
                                DateTime prvDate = Convert.ToDateTime(prvMdate);

                                double diffDays = (curDate - prvDate).TotalDays;
                                //double diffDaysInHour = 0;
                                DataRow row = null;
                                if (diffDays > 1)
                                {
                                    //diffDaysInHour = (diffDays - 1) * 24; // 24hr 
                                    for (int i = 0; i < diffDays; i++)
                                    {
                                        if (i == 0)
                                        {
                                            // find 1st prv record downtime
                                            TimeSpan prv1stDownTime = TimeSpan.Parse(prvMtime);
                                            double diffPrvDowntime = (1440 - prv1stDownTime.TotalMinutes) / 60;

                                            row = performanceTable.NewRow();
                                            row["KioskID"] = prvMachid;
                                            row["Date"] = prvMdate;
                                            row["Error"] = diffPrvDowntime;
                                            performanceTable.Rows.Add(row);
                                        }
                                        else
                                        {
                                            // check each date. 
                                            DateTime addDate = prvDate.AddDays(i);

                                            string format = "yyyy-MM-dd";
                                            string dateToDisplay = addDate.ToString(format);
                                            row = performanceTable.NewRow();
                                            row["KioskID"] = prvMachid;
                                            row["Date"] = dateToDisplay;
                                            row["Error"] = 24;
                                            performanceTable.Rows.Add(row);
                                        }
                                    }
                                }
                                else // diff date not more than 1 day
                                {
                                    TimeSpan prvDownTime = TimeSpan.Parse(prvMtime);
                                    double diffPrvDowntime = (1440 - prvDownTime.TotalMinutes) / 60;

                                    row = performanceTable.NewRow();
                                    row["KioskID"] = prvMachid;
                                    row["Date"] = prvMdate;
                                    row["Error"] = diffPrvDowntime;
                                    performanceTable.Rows.Add(row);
                                }

                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                double diffCurrtime = curTime.TotalHours; // convert to hour basic
                                //double ttlDowntime = diffDaysInHour + diffPrvtime + diffCurrtime;

                                row = performanceTable.NewRow();
                                row["KioskID"] = machid;
                                row["Date"] = mdate;
                                row["Error"] = diffCurrtime;
                                performanceTable.Rows.Add(row);
                                resetPrevRecord();
                            }
                            else if (prvMdate != mdate && merrtype == "OK" && prvMachid != machid)
                            {   // date change, diff machine, count current record downtime. /////

                                // have to count prev error time, b4 count curr ok.
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;
                                DataRow row = performanceTable.NewRow();
                                row["KioskID"] = prvMachid;
                                row["Date"] = prvMdate;
                                row["Error"] = diffPrvtime;
                                performanceTable.Rows.Add(row);


                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                double diffCurrtime = curTime.TotalHours; // convert to hour basic

                                row = performanceTable.NewRow();
                                row["KioskID"] = machid;
                                row["Date"] = mdate;
                                row["Error"] = diffCurrtime;
                                performanceTable.Rows.Add(row);
                                resetPrevRecord();
                            }
                            else if (prvMdate == mdate && merrtype == "ERROR" && prvMachid != machid)
                            {
                                // same day, diff mach, count prev mach downtime.

                                // find prv downtime
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;

                                DataRow row = performanceTable.NewRow();
                                row["KioskID"] = prvMachid;
                                row["Date"] = prvMdate;
                                row["Error"] = diffPrvtime;
                                performanceTable.Rows.Add(row);

                                resetPrevRecord();

                                prvMachid = machid;
                                prvMdate = mdate;
                                prvMcode = mcode;
                                prvMtime = mtime;
                                prvMerrtype = merrtype;
                                prvMachtype = machtype;
                            }
                            else if (prvMdate != mdate && merrtype == "ERROR" && prvMerrtype == "ERROR")
                            {   // diff date, same mach, 

                                // find prv downtime
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;

                                DataRow row = performanceTable.NewRow();
                                row["KioskID"] = prvMachid;
                                row["Date"] = prvMdate;
                                row["Error"] = diffPrvtime;
                                performanceTable.Rows.Add(row);

                                resetPrevRecord();

                                prvMachid = machid;
                                prvMdate = mdate;
                                prvMcode = mcode;
                                prvMtime = mtime;
                                prvMerrtype = merrtype;
                                prvMachtype = machtype;
                            }
                        }
                    }

                    // count last record if errType is error.
                    if (prvMerrtype == "ERROR")
                    {
                        TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                        double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;

                        DataRow row = performanceTable.NewRow();
                        row["KioskID"] = prvMachid;
                        row["Date"] = prvMdate;
                        row["Error"] = diffPrvtime;
                        performanceTable.Rows.Add(row);
                    }
                    dr.Close();
                    //////////////////// end, count error dowmtime ///////////////


                    //////////////////// count warning dowmtime ///////////////
                    //" WHERE (MR.M_DATE >='2014-10-27' AND MR.M_DATE <='2014-11-13') "+
                    sql = " SELECT MR.M_DATE, MR.M_MACH_ID, MR.M_TIME, MR.M_CODE, MC.M_ERRORTYPE, MC.M_MACH_TYPE  " +
                    " FROM  M_CODES as MC " +
                    " INNER JOIN M_REPORTING_DATA as MR " +
                    " ON MC.M_CODE = MR.M_CODE " +
                    " WHERE (MR.M_DATE >='" + startDate + "' AND MR.M_DATE <='" + endDate + "') ";

                    if (!string.IsNullOrEmpty(machID) && machID != "All")
                    {
                        sql = sql + " AND MR.M_MACH_ID = '" + machID + "' ";
                    }
                    if (!string.IsNullOrEmpty(machType) && machType != "All")
                    {
                        sql = sql + " AND MC.M_MACH_TYPE = '" + machType + "' ";
                    }

                    sql = sql + " AND (MC.M_ERRORTYPE='WARN' OR MC.M_ERRORTYPE='OK') " +
                    " ORDER BY MR.M_DATE, MR.M_MACH_ID, MR.M_TIME ";

                    cmd.CommandText = sql;
                    dr = cmd.ExecuteReader();
                    resetPrevRecord();


                     machid = "";
                     mdate = "";
                     mcode = "";
                     mtime = "";
                     merrtype = "";
                     machtype = "";

                    // Create a data table with the same name of crystal report's table.
        //            DataTable performanceTable = new DataTable("table1");
                    // Create table columns with the same fields name of crystal report's table.
                    //performanceTable.Columns.Add("machID");
                    //performanceTable.Columns.Add("machDate");
                    //performanceTable.Columns.Add("machType");
                    //performanceTable.Columns.Add("Downtime");
                    //performanceTable.Columns.Add("Date");
                    //performanceTable.Columns.Add("KioskID");
                    //performanceTable.Columns.Add("Error");
                    //performanceTable.Columns.Add("Warning");
                    //performanceTable.Columns.Add("Offline");
                    //performanceTable.Columns.Add("Normal");

                    if (dr.HasRows)
                    {

                        while (dr.Read())
                        {
                            machid = dr["M_MACH_ID"].ToString();
                            mdate = dr["M_DATE"].ToString();
                            mcode = dr["M_CODE"].ToString();
                            mtime = dr["M_TIME"].ToString();
                            merrtype = dr["M_ERRORTYPE"].ToString();
                            machtype = dr["M_MACH_TYPE"].ToString();

                            if (string.IsNullOrEmpty(prvMerrtype) && merrtype == "WARN")
                            {   // first time null, then set prevRecord
                                prvMachid = machid;
                                prvMdate = mdate;
                                prvMcode = mcode;
                                prvMtime = mtime;
                                prvMerrtype = merrtype;
                                prvMachtype = machtype;
                            }
                            else if (string.IsNullOrEmpty(prvMerrtype) && merrtype == "OK")
                            {   // first record ok, just count today downtime. 
                                //TimeSpan curTime = TimeSpan.Parse(mtime);
                                //double diffCurrtime = curTime.TotalHours;

                                //DataRow row = performanceTable.NewRow();
                                //row["KioskID"] = machid;
                                //row["Date"] = mdate;
                                //row["Warning"] = diffCurrtime;
                                //performanceTable.Rows.Add(row);
                            }
                            else if (prvMdate == mdate && merrtype == "OK" && prvMachid == machid)
                            {   // count warning in same day
                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double difftime = (curTime.TotalMinutes - prvTime.TotalMinutes) / 60; // convert to hour basic

                                DataRow row = performanceTable.NewRow();
                                row["KioskID"] = machid;
                                row["Date"] = mdate;
                                row["Warning"] = difftime;
                                performanceTable.Rows.Add(row);
                                resetPrevRecord();
                            }
                            else if (prvMdate != mdate && merrtype == "OK" && prvMachid == machid)
                            {   // same machine, count warning in diff day.
                                // if record date no same prv date, count downtime for prv record.
                                DateTime curDate = Convert.ToDateTime(mdate);
                                DateTime prvDate = Convert.ToDateTime(prvMdate);

                                double diffDays = (curDate - prvDate).TotalDays;
                                //double diffDaysInHour = 0;
                                DataRow row = null;
                                if (diffDays > 1)
                                {
                                    //diffDaysInHour = (diffDays - 1) * 24; // 24hr 
                                    for (int i = 0; i < diffDays; i++)
                                    {
                                        if (i == 0)
                                        {
                                            // find 1st prv record downtime
                                            TimeSpan prv1stDownTime = TimeSpan.Parse(prvMtime);
                                            double diffPrvDowntime = (1440 - prv1stDownTime.TotalMinutes) / 60;

                                            row = performanceTable.NewRow();
                                            row["KioskID"] = prvMachid;
                                            row["Date"] = prvMdate;
                                            row["Warning"] = diffPrvDowntime;
                                            performanceTable.Rows.Add(row);
                                        }
                                        else
                                        {
                                            // check each date. 
                                            DateTime addDate = prvDate.AddDays(i);

                                            string format = "yyyy-MM-dd";
                                            string dateToDisplay = addDate.ToString(format);
                                            row = performanceTable.NewRow();
                                            row["KioskID"] = prvMachid;
                                            row["Date"] = dateToDisplay;
                                            row["Warning"] = 24;
                                            performanceTable.Rows.Add(row);
                                        }
                                    }
                                }
                                else // diff date not more than 1 day
                                {
                                    TimeSpan prvDownTime = TimeSpan.Parse(prvMtime);
                                    double diffPrvDowntime = (1440 - prvDownTime.TotalMinutes) / 60;

                                    row = performanceTable.NewRow();
                                    row["KioskID"] = prvMachid;
                                    row["Date"] = prvMdate;
                                    row["Warning"] = diffPrvDowntime;
                                    performanceTable.Rows.Add(row);
                                }

                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                double diffCurrtime = curTime.TotalHours; // convert to hour basic
                                //double ttlDowntime = diffDaysInHour + diffPrvtime + diffCurrtime;

                                row = performanceTable.NewRow();
                                row["KioskID"] = machid;
                                row["Date"] = mdate;
                                row["Warning"] = diffCurrtime;
                                performanceTable.Rows.Add(row);
                                resetPrevRecord();
                            }
                            else if (prvMdate != mdate && merrtype == "OK" && prvMachid != machid)
                            {   // date change, diff machine, count current record downtime. /////

                                // have to count prev error time, b4 count curr ok.
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;
                                DataRow row = performanceTable.NewRow();
                                row["KioskID"] = prvMachid;
                                row["Date"] = prvMdate;
                                row["Warning"] = diffPrvtime;
                                performanceTable.Rows.Add(row);


                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                double diffCurrtime = curTime.TotalHours; // convert to hour basic

                                row = performanceTable.NewRow();
                                row["KioskID"] = machid;
                                row["Date"] = mdate;
                                row["Warning"] = diffCurrtime;
                                performanceTable.Rows.Add(row);
                                resetPrevRecord();
                            }
                            else if (prvMdate == mdate && merrtype == "WARN" && prvMachid != machid)
                            {
                                // same day, diff mach, count prev mach downtime.

                                // find prv downtime
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;

                                DataRow row = performanceTable.NewRow();
                                row["KioskID"] = prvMachid;
                                row["Date"] = prvMdate;
                                row["Warning"] = diffPrvtime;
                                performanceTable.Rows.Add(row);

                                resetPrevRecord();

                                prvMachid = machid;
                                prvMdate = mdate;
                                prvMcode = mcode;
                                prvMtime = mtime;
                                prvMerrtype = merrtype;
                                prvMachtype = machtype;
                            }
                            else if (prvMdate != mdate && merrtype == "WARN" && prvMerrtype == "WARN")
                            {   // diff date, same mach, 

                                // find prv downtime
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;

                                DataRow row = performanceTable.NewRow();
                                row["KioskID"] = prvMachid;
                                row["Date"] = prvMdate;
                                row["Warning"] = diffPrvtime;
                                performanceTable.Rows.Add(row);

                                resetPrevRecord();

                                prvMachid = machid;
                                prvMdate = mdate;
                                prvMcode = mcode;
                                prvMtime = mtime;
                                prvMerrtype = merrtype;
                                prvMachtype = machtype;
                            }
                        }
                    }

                    // count last record if errType is error.
                    if (prvMerrtype == "WARN")
                    {
                        TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                        double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;

                        DataRow row = performanceTable.NewRow();
                        row["KioskID"] = prvMachid;
                        row["Date"] = prvMdate;
                        row["Warning"] = diffPrvtime;
                        performanceTable.Rows.Add(row);
                    }
                    dr.Close();
                    //////////////////// end, count warning dowmtime ///////////////



                    //////////////////// count OFFLINE dowmtime ///////////////
                    //" WHERE (MR.M_DATE >='2014-10-27' AND MR.M_DATE <='2014-11-13') "+
                    sql = " SELECT MR.M_DATE, MR.M_MACH_ID, MR.M_TIME, MR.M_CODE, MC.M_ERRORTYPE, MC.M_MACH_TYPE  " +
                    " FROM  M_CODES as MC " +
                    " INNER JOIN M_REPORTING_DATA as MR " +
                    " ON MC.M_CODE = MR.M_CODE " +
                    " WHERE (MR.M_DATE >='" + startDate + "' AND MR.M_DATE <='" + endDate + "') ";

                    if (!string.IsNullOrEmpty(machID) && machID != "All")
                    {
                        sql = sql + " AND MR.M_MACH_ID = '" + machID + "' ";
                    }
                    if (!string.IsNullOrEmpty(machType) && machType != "All")
                    {
                        sql = sql + " AND MC.M_MACH_TYPE = '" + machType + "' ";
                    }

                    sql = sql + " AND (MC.M_ERRORTYPE='OFFLINE' OR MC.M_ERRORTYPE='OK') " +
                    " ORDER BY MR.M_DATE, MR.M_MACH_ID, MR.M_TIME ";

                    cmd.CommandText = sql;
                    dr = cmd.ExecuteReader();
                    resetPrevRecord();


                    machid = "";
                    mdate = "";
                    mcode = "";
                    mtime = "";
                    merrtype = "";
                    machtype = "";

                    if (dr.HasRows)
                    {

                        while (dr.Read())
                        {
                            machid = dr["M_MACH_ID"].ToString();
                            mdate = dr["M_DATE"].ToString();
                            mcode = dr["M_CODE"].ToString();
                            mtime = dr["M_TIME"].ToString();
                            merrtype = dr["M_ERRORTYPE"].ToString();
                            machtype = dr["M_MACH_TYPE"].ToString();

                            if (string.IsNullOrEmpty(prvMerrtype) && merrtype == "OFFLINE")
                            {   // first time null, then set prevRecord
                                prvMachid = machid;
                                prvMdate = mdate;
                                prvMcode = mcode;
                                prvMtime = mtime;
                                prvMerrtype = merrtype;
                                prvMachtype = machtype;
                            }
                            else if (string.IsNullOrEmpty(prvMerrtype) && merrtype == "OK")
                            {   // first record ok, just count today downtime. 
                                //TimeSpan curTime = TimeSpan.Parse(mtime);
                                //double diffCurrtime = curTime.TotalHours;

                                //DataRow row = performanceTable.NewRow();
                                //row["KioskID"] = machid;
                                //row["Date"] = mdate;
                                //row["Warning"] = diffCurrtime;
                                //performanceTable.Rows.Add(row);
                            }
                            else if (prvMdate == mdate && merrtype == "OK" && prvMachid == machid)
                            {   // count warning in same day
                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double difftime = (curTime.TotalMinutes - prvTime.TotalMinutes) / 60; // convert to hour basic

                                DataRow row = performanceTable.NewRow();
                                row["KioskID"] = machid;
                                row["Date"] = mdate;
                                row["Offline"] = difftime;
                                performanceTable.Rows.Add(row);
                                resetPrevRecord();
                            }
                            else if (prvMdate != mdate && merrtype == "OK" && prvMachid == machid)
                            {   // same machine, count warning in diff day.
                                // if record date no same prv date, count downtime for prv record.
                                DateTime curDate = Convert.ToDateTime(mdate);
                                DateTime prvDate = Convert.ToDateTime(prvMdate);

                                double diffDays = (curDate - prvDate).TotalDays;
                                //double diffDaysInHour = 0;
                                DataRow row = null;
                                if (diffDays > 1)
                                {
                                    //diffDaysInHour = (diffDays - 1) * 24; // 24hr 
                                    for (int i = 0; i < diffDays; i++)
                                    {
                                        if (i == 0)
                                        {
                                            // find 1st prv record downtime
                                            TimeSpan prv1stDownTime = TimeSpan.Parse(prvMtime);
                                            double diffPrvDowntime = (1440 - prv1stDownTime.TotalMinutes) / 60;

                                            row = performanceTable.NewRow();
                                            row["KioskID"] = prvMachid;
                                            row["Date"] = prvMdate;
                                            row["Offline"] = diffPrvDowntime;
                                            performanceTable.Rows.Add(row);
                                        }
                                        else
                                        {
                                            // check each date. 
                                            DateTime addDate = prvDate.AddDays(i);

                                            string format = "yyyy-MM-dd";
                                            string dateToDisplay = addDate.ToString(format);
                                            row = performanceTable.NewRow();
                                            row["KioskID"] = prvMachid;
                                            row["Date"] = dateToDisplay;
                                            row["Offline"] = 24;
                                            performanceTable.Rows.Add(row);
                                        }
                                    }
                                }
                                else // diff date not more than 1 day
                                {
                                    TimeSpan prvDownTime = TimeSpan.Parse(prvMtime);
                                    double diffPrvDowntime = (1440 - prvDownTime.TotalMinutes) / 60;

                                    row = performanceTable.NewRow();
                                    row["KioskID"] = prvMachid;
                                    row["Date"] = prvMdate;
                                    row["Offline"] = diffPrvDowntime;
                                    performanceTable.Rows.Add(row);
                                }

                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                double diffCurrtime = curTime.TotalHours; // convert to hour basic
                                //double ttlDowntime = diffDaysInHour + diffPrvtime + diffCurrtime;

                                row = performanceTable.NewRow();
                                row["KioskID"] = machid;
                                row["Date"] = mdate;
                                row["Offline"] = diffCurrtime;
                                performanceTable.Rows.Add(row);
                                resetPrevRecord();
                            }
                            else if (prvMdate != mdate && merrtype == "OK" && prvMachid != machid)
                            {   // date change, diff machine, count current record downtime. /////

                                // have to count prev error time, b4 count curr ok.
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;
                                DataRow row = performanceTable.NewRow();
                                row["KioskID"] = prvMachid;
                                row["Date"] = prvMdate;
                                row["Offline"] = diffPrvtime;
                                performanceTable.Rows.Add(row);


                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                double diffCurrtime = curTime.TotalHours; // convert to hour basic

                                row = performanceTable.NewRow();
                                row["KioskID"] = machid;
                                row["Date"] = mdate;
                                row["Offline"] = diffCurrtime;
                                performanceTable.Rows.Add(row);
                                resetPrevRecord();
                            }
                            else if (prvMdate == mdate && merrtype == "OFFLINE" && prvMachid != machid)
                            {
                                // same day, diff mach, count prev mach downtime.

                                // find prv downtime
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;

                                DataRow row = performanceTable.NewRow();
                                row["KioskID"] = prvMachid;
                                row["Date"] = prvMdate;
                                row["Offline"] = diffPrvtime;
                                performanceTable.Rows.Add(row);

                                resetPrevRecord();

                                prvMachid = machid;
                                prvMdate = mdate;
                                prvMcode = mcode;
                                prvMtime = mtime;
                                prvMerrtype = merrtype;
                                prvMachtype = machtype;
                            }
                            else if (prvMdate != mdate && merrtype == "OFFLINE" && prvMerrtype == "OFFLINE")
                            {   // diff date, same mach, 

                                // find prv downtime
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;

                                DataRow row = performanceTable.NewRow();
                                row["KioskID"] = prvMachid;
                                row["Date"] = prvMdate;
                                row["Offline"] = diffPrvtime;
                                performanceTable.Rows.Add(row);

                                resetPrevRecord();

                                prvMachid = machid;
                                prvMdate = mdate;
                                prvMcode = mcode;
                                prvMtime = mtime;
                                prvMerrtype = merrtype;
                                prvMachtype = machtype;
                            }
                        }
                    }

                    // count last record if errType is error.
                    if (prvMerrtype == "OFFLINE")
                    {
                        TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                        double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;

                        DataRow row = performanceTable.NewRow();
                        row["KioskID"] = prvMachid;
                        row["Date"] = prvMdate;
                        row["Offline"] = diffPrvtime;
                        performanceTable.Rows.Add(row);
                    }
                    dr.Close();
                    //////////////////// end, count offline dowmtime ///////////////

                    crPerformance_line crystalReport = new crPerformance_line();
                    crystalReport.Database.Tables["DateSetPerformance"].SetDataSource((DataTable)performanceTable);
                    crystalReport.SetParameterValue("ttlHrs", ttlHrsMach);
                    crystalReport.SetParameterValue("ttlMach", ttlmach);
                    
                    crystalReport.SetParameterValue("userName", userName);
                    crystalReport.SetParameterValue("startDate", startDate);
                    crystalReport.SetParameterValue("endDate", endDate);
                    crystalReport.SetParameterValue("machType", machType);
                    crystalReport.SetParameterValue("machID", machID);
                    CrystalReportViewer1.ReportSource = crystalReport;

                }
                catch (Exception ex)
                {
                    lblmsg.Text = ex.ToString();
                }
                finally
                {
                    if (dbc.conn != null)
                    {
                        dbc.closeConn();
                    }
                }

            }
            catch (Exception ex)
            {
                lblmsg.Text = ex.ToString();
            }

        }
    }
}