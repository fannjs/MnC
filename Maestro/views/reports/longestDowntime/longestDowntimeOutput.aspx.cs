using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Maestro.views.reports.longestDowntime
{
    public partial class longestDowntimeOutput : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
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
                    //" WHERE (MR.M_DATE >='2014-10-27' AND MR.M_DATE <='2014-11-13') "+
                    string sql = " SELECT MR.M_MACH_ID, MR.M_DATE, MR.M_CODE, MR.M_TIME, MC.M_ERRORTYPE, MC.M_MACH_TYPE " +
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

                    sql = sql +" AND (MC.M_ERRORTYPE='ERROR' OR MC.M_ERRORTYPE='OK') " +
                    " ORDER BY MR.M_MACH_ID, MR.M_DATE, MR.M_TIME ";

                    cmd.CommandText = sql;
                    SqlDataReader dr = cmd.ExecuteReader();
                    string[] prevRecord = new string[6];
                    string machid = "";
                    string mdate = "";
                    string mcode = "";
                    string mtime = "";
                    string merrtype = "";
                    string machtype = "";

                    // Create a data table with the same name of crystal report's table.
                    DataTable longestDownTable = new DataTable("table1");
                    // Create table columns with the same fields name of crystal report's table.
                    longestDownTable.Columns.Add("machID");
                    longestDownTable.Columns.Add("machDate");
                    longestDownTable.Columns.Add("machType");
                    longestDownTable.Columns.Add("Downtime");

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

                            if (prevRecord[4] == null && merrtype == "ERROR")
                            {   // first time null, then set prevRecord
                                prevRecord[0] = machid;
                                prevRecord[1] = mdate;
                                prevRecord[2] = mcode;
                                prevRecord[3] = mtime;
                                prevRecord[4] = merrtype;
                                prevRecord[5] = machtype;
                            }
                            else if (prevRecord[4] == null && merrtype == "OK")
                            {   // first record ok, just count today downtime.
                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                double diffCurrtime = curTime.TotalHours;

                                DataRow row = longestDownTable.NewRow();
                                row["machID"] = machid;
                                row["machDate"] = mdate;
                                row["Downtime"] = diffCurrtime;
                                row["machType"] = machtype;
                                longestDownTable.Rows.Add(row);
                            }
                            else if (prevRecord[1] == mdate && merrtype == "OK" && prevRecord[0] == machid)
                            {   // count downtime in same day
                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                TimeSpan prvTime = TimeSpan.Parse(prevRecord[3]);
                                double difftime = (curTime.TotalMinutes - prvTime.TotalMinutes) / 60; // convert to hour basic

                                DataRow row = longestDownTable.NewRow();
                                // Assign values to each cell. We can assign any type of data to the cell (object type)
                                row["machID"] = machid;
                                row["machDate"] = mdate;
                                row["Downtime"] = difftime;
                                row["machType"] = machtype;
                                longestDownTable.Rows.Add(row);
                                prevRecord = new string[6];
                            }
                            else if (prevRecord[1] != mdate && merrtype == "OK" && prevRecord[0] == machid)
                            {   // same machine, count downtime in diff day.
                                // if record date no same prv date, count downtime for prv record.
                                DateTime curDate = Convert.ToDateTime(mdate);
                                DateTime prvDate = Convert.ToDateTime(prevRecord[1]);

                                double diffDays = (curDate - prvDate).TotalDays;
                                double diffDaysInHour = 0;
                                if (diffDays > 1)
                                {
                                    diffDaysInHour = ( diffDays - 1) * 24; // 24hr 
                                }

                                // find prv time
                                TimeSpan prvTime = TimeSpan.Parse(prevRecord[3]);
                                double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;

                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                double diffCurrtime = curTime.TotalHours; // convert to hour basic
                                double ttlDowntime = diffDaysInHour + diffPrvtime + diffCurrtime;

                                DataRow row = longestDownTable.NewRow();
                                row["machID"] = machid;
                                row["machDate"] = mdate;
                                row["Downtime"] = ttlDowntime;
                                row["machType"] = machtype;
                                longestDownTable.Rows.Add(row);
                                prevRecord = new string[6];
                            }
                            else if (prevRecord[4] == "ERROR" && prevRecord[0] != machid)  
                            {
                                // previous error never found Ok, mean still downtime. so, count the downtime till endDate time.
                                
                                // find diff days
                                DateTime eDate = Convert.ToDateTime(endDate);
                                DateTime prvDate = Convert.ToDateTime(prevRecord[1]);

                                double diffDays = (eDate - prvDate).TotalDays;
                                double diffDaysInHour = 0;
                                if (diffDays > 0)
                                {
                                    diffDaysInHour = diffDays * 24; // 24hr 
                                }
                                // find prv downtime
                                TimeSpan prvTime = TimeSpan.Parse(prevRecord[3]);
                                double diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;

                                double ttlDowntime = diffDaysInHour + diffPrvtime; // +diffCurrtime;

                                DataRow row = longestDownTable.NewRow();
                                row["machID"] = prevRecord[0];
                                row["machDate"] = prevRecord[1];
                                row["Downtime"] = ttlDowntime; 
                                row["machType"] = prevRecord[5];
                                longestDownTable.Rows.Add(row);

                                prevRecord = new string[6];
                                prevRecord[0] = machid;
                                prevRecord[1] = mdate;
                                prevRecord[2] = mcode;
                                prevRecord[3] = mtime;
                                prevRecord[4] = merrtype;
                                prevRecord[5] = machtype;
                            }
                        }
                    }

                    if (longestDownTable.Rows.Count > 0)
                    {
                        if (chartType == "BarChart")
                        {
                            // Create instance of the crystal report.
                            crLongestDowntime_barChart crystalReport = new crLongestDowntime_barChart();
                            // Set data source to the crystal report. If the report contains multiple
                            // datatables, then we can index each tables by using the table
                            // name. Following code explains how table name is used for indexing.
                            crystalReport.Database.Tables["DataTableLongestDowntime"].SetDataSource((DataTable)longestDownTable);
                            crystalReport.SetParameterValue("userName", userName);
                            crystalReport.SetParameterValue("startDate", startDate);
                            crystalReport.SetParameterValue("endDate", endDate);
                            crystalReport.SetParameterValue("machType", machType);
                            crystalReport.SetParameterValue("machID", machID);
                            // Assign the report to the Crystal report viewer.
                            CrystalReportViewer1.ReportSource = crystalReport;
                        }
                        else
                        {
                            crLongestDowntime_pieChart crystalReport = new crLongestDowntime_pieChart();
                            crystalReport.Database.Tables["DataTableLongestDowntime"].SetDataSource((DataTable)longestDownTable);
                            crystalReport.SetParameterValue("userName", userName);
                            crystalReport.SetParameterValue("startDate", startDate);
                            crystalReport.SetParameterValue("endDate", endDate);
                            crystalReport.SetParameterValue("machType", machType);
                            crystalReport.SetParameterValue("machID", machID);
                            CrystalReportViewer1.ReportSource = crystalReport;
                        }
                    }
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

                //// Create a data table with the same name of crystal report's table.
                //DataTable demoTable = new DataTable("table1");
                //// Create table columns with the same fields name of crystal report's table.
                //demoTable.Columns.Add("machID");
                //demoTable.Columns.Add("machType");
                //demoTable.Columns.Add("Downtime");
                ////// Adding 1000 rows to the data table
                ////for (int index = 1; index <= 1000; index++)
                ////{
                ////    Random rnd = new Random();
                ////    // Create a data row of the created table.
                ////    DataRow row = demoTable.NewRow();
                ////    // Assign values to each cell. We can assign any type of data to
                ////    // the cell (object type)
                ////    row["Name"] = "Prathap";
                ////    row["Age"] = rnd.Next(10, 100);
                ////    row["Address"] = "Trivandrum";
                ////    row["Phone"] = "954712449198";
                ////    demoTable.Rows.Add(row);
                ////}

                //// Create instance of the crystal report.
                //crLongestDowntime report = new crLongestDowntime();
                //// Set data source to the crystal report. If the report contains multiple
                //// datatables, then we can index each tables by using the table
                //// name. Following code explains how table name is used for indexing.
                //report.Database.Tables["DataTableLongestDowntime"].SetDataSource((DataTable)demoTable);
                //// Assign the report to the Crystal report viewer.
                //CrystalReportViewer1.ReportSource = report;

            }
            catch (Exception ex)
            {
                lblmsg.Text = ex.ToString();
            }

        }
    }
}