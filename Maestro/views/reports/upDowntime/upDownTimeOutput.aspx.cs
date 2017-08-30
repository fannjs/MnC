using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Maestro.views.reports.upDowntime
{
    public partial class upDownTimeOutput : System.Web.UI.Page
    {

        string prvMachid = "";
        string prvMdate = "";
        string prvMcode = "";
        string prvMtime = "";
        string prvMerrtype = "";
        string prvMachtype = "";

        private void resetPrevRecord(){
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
                    DataTable upDownTimeTable = new DataTable("table1");
                    // Create table columns with the same fields name of crystal report's table.
                    upDownTimeTable.Columns.Add("machID");
                    upDownTimeTable.Columns.Add("machDate");
                    upDownTimeTable.Columns.Add("machType");
                    upDownTimeTable.Columns.Add("Downtime");

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

                                DataRow row = upDownTimeTable.NewRow();
                                row["machID"] = machid;
                                row["machDate"] = mdate;
                                row["downTime"] = diffCurrtime;
                                upDownTimeTable.Rows.Add(row);
                            }
                            else if (prvMdate == mdate && merrtype == "OK" && prvMachid == machid)
                            {   // count downtime in same day
                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double difftime = (curTime.TotalMinutes - prvTime.TotalMinutes) / 60; // convert to hour basic

                                DataRow row = upDownTimeTable.NewRow();
                                // Assign values to each cell. We can assign any type of data to the cell (object type)
                                row["machID"] = machid;
                                row["machDate"] = mdate;
                                row["downTime"] = difftime;
                                upDownTimeTable.Rows.Add(row);
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

                                            row = upDownTimeTable.NewRow();
                                            row["machID"] = prvMachid;
                                            row["machDate"] = prvMdate;
                                            row["downTime"] = diffPrvDowntime;
                                            upDownTimeTable.Rows.Add(row);
                                        }
                                        else
                                        {
                                            // check each date. 
                                            DateTime addDate = prvDate.AddDays(i);

                                            string format = "yyyy-MM-dd";
                                            string dateToDisplay = addDate.ToString(format);
                                            row = upDownTimeTable.NewRow();
                                            row["machID"] = prvMachid;
                                            row["machDate"] = dateToDisplay;
                                            row["downTime"] = 24; 
                                            upDownTimeTable.Rows.Add(row);
                                        }
                                    }
                                }
                                else // diff date not more than 1 day
                                {
                                    TimeSpan prvDownTime = TimeSpan.Parse(prvMtime);
                                    double diffPrvDowntime = (1440 - prvDownTime.TotalMinutes) / 60;

                                    row = upDownTimeTable.NewRow();
                                    row["machID"] = prvMachid;
                                    row["machDate"] = prvMdate;
                                    row["downTime"] = diffPrvDowntime;
                                    upDownTimeTable.Rows.Add(row);
                                }

                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                double diffCurrtime = curTime.TotalHours; // convert to hour basic
                                //double ttlDowntime = diffDaysInHour + diffPrvtime + diffCurrtime;

                                row = upDownTimeTable.NewRow();
                                row["machID"] = machid;
                                row["machDate"] = mdate;
                                row["downTime"] = diffCurrtime;
                                upDownTimeTable.Rows.Add(row);
                                resetPrevRecord();
                            }
                            else if (prvMdate != mdate && merrtype == "OK" && prvMachid != machid)
                            {   // date change, diff machine, count current record downtime. /////

                                // have to count prev error time, b4 count curr ok.
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;
                                DataRow row = upDownTimeTable.NewRow();
                                row["machID"] = prvMachid;
                                row["machDate"] = prvMdate;
                                row["downTime"] = diffPrvtime;
                                upDownTimeTable.Rows.Add(row);

                                
                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                double diffCurrtime = curTime.TotalHours; // convert to hour basic

                                row = upDownTimeTable.NewRow();
                                row["machID"] = machid;
                                row["machDate"] = mdate;
                                row["downTime"] = diffCurrtime;
                                upDownTimeTable.Rows.Add(row);
                                resetPrevRecord();
                            }
                            else if (prvMdate == mdate && merrtype == "ERROR" && prvMachid != machid)
                            {
                                // same day, diff mach, count prev mach downtime.

                                // find prv downtime
                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;

                                DataRow row = upDownTimeTable.NewRow();
                                row["machID"] = prvMachid;
                                row["machDate"] = prvMdate;
                                row["downTime"] = diffPrvtime;
                                upDownTimeTable.Rows.Add(row);

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

                                DataRow row = upDownTimeTable.NewRow();
                                row["machID"] = prvMachid;
                                row["machDate"] = prvMdate;
                                row["downTime"] = diffPrvtime;
                                upDownTimeTable.Rows.Add(row);

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

                        DataRow row = upDownTimeTable.NewRow();
                        row["machID"] = prvMachid;
                        row["machDate"] = prvMdate;
                        row["downTime"] = diffPrvtime;
                        upDownTimeTable.Rows.Add(row);
                    }


                    //if (chartType == "BarChart")
                    //{
                    // Create instance of the crystal report.
                    crUpDownTime crystalReport = new crUpDownTime();
                    // Set data source to the crystal report. If the report contains multiple
                    // datatables, then we can index each tables by using the table
                    // name. Following code explains how table name is used for indexing.
                    crystalReport.Database.Tables["DataSetUpDownTime"].SetDataSource((DataTable)upDownTimeTable);
                    crystalReport.SetParameterValue("ttlHrs", ttlHrsMach);
                    crystalReport.SetParameterValue("userName", userName);
                    crystalReport.SetParameterValue("startDate", startDate);
                    crystalReport.SetParameterValue("endDate", endDate);
                    crystalReport.SetParameterValue("machType", machType);
                    crystalReport.SetParameterValue("machID", machID);
                    // Assign the report to the Crystal report viewer.
                    CrystalReportViewer1.ReportSource = crystalReport;
                    //}
                    //else
                    //{
                    //    crLongestDowntime_pieChart crystalReport = new crLongestDowntime_pieChart();
                    //    crystalReport.Database.Tables["DataTableLongestDowntime"].SetDataSource((DataTable)upDownTimeTable);
                    //    crystalReport.SetParameterValue("userName", userName);
                    //    crystalReport.SetParameterValue("startDate", startDate);
                    //    crystalReport.SetParameterValue("endDate", endDate);
                    //    crystalReport.SetParameterValue("machType", machType);
                    //    crystalReport.SetParameterValue("machID", machID);
                    //    CrystalReportViewer1.ReportSource = crystalReport;
                    //}

                    dr.Close();
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