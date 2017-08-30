using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Maestro.views.reports.longestResponse
{
    public partial class longestResponseOutput : System.Web.UI.Page
    {

        string prvMachid = "";
        string prvMdate = "";
        string prvMcode = "";
        string prvMtime = "";
        string prvMerrtype = "";
        string prvMachtype = "";        


        // Create a data table with the same name of crystal report's table.
        DataTable longestResponseTable = new DataTable("table1");

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

                    //sql = sql + " AND (MC.M_ERRORTYPE='ERROR' OR MC.M_ERRORTYPE='OK' OR MC.M_ERRORTYPE='IDLE') " +
                    //" ORDER BY MR.M_MACH_ID, MR.M_DATE, MR.M_TIME ";
                    sql = sql + " AND (MC.M_ERRORTYPE='ERROR' OR MC.M_ERRORTYPE='MAIN') " +
                    " ORDER BY MR.M_MACH_ID, MR.M_DATE, MR.M_TIME ";


                    cmd.CommandText = sql;
                    SqlDataReader dr = cmd.ExecuteReader();
                    string machid = "";
                    string mdate = "";
                    string mcode = "";
                    string mtime = "";
                    string merrtype = "";
                    string machtype = "";

                    //// Create a data table with the same name of crystal report's table.
                    //DataTable longestResponseTable = new DataTable("table1");
                    // Create table columns with the same fields name of crystal report's table.
                    longestResponseTable.Columns.Add("machID");
                    longestResponseTable.Columns.Add("date");
                    longestResponseTable.Columns.Add("machType");
                    longestResponseTable.Columns.Add("responseTime");

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
                            else if (prvMachid == machid && merrtype == "MAIN")
                            {   // same mach

                                // find diff days
                                DateTime curDate = Convert.ToDateTime(mdate);
                                DateTime prvDate = Convert.ToDateTime(prvMdate);

                                double diffDays = (curDate - prvDate).TotalDays;
                                double diffDaysInHour = 0;
                                double diffPrvtime = 0;
                                double diffCurrtime = 0;
                                double ttlResponsetime = 0;

                                TimeSpan prvTime = TimeSpan.Parse(prvMtime);
                                TimeSpan curTime = TimeSpan.Parse(mtime);
                                DataRow row = longestResponseTable.NewRow();

                                if (diffDays > 0)
                                {
                                    diffDaysInHour = (diffDays - 1) * 24; // 24hr 

                                    // find prv downtime
                                    diffPrvtime = (1440 - prvTime.TotalMinutes) / 60;

                                    diffCurrtime = curTime.TotalHours;
                                    ttlResponsetime = diffDaysInHour + diffPrvtime + diffCurrtime;
                                    row["date"] = prvMdate;
                                }
                                else
                                {
                                    ttlResponsetime = curTime.TotalHours - prvTime.TotalHours;
                                    row["date"] = mdate;
                                }
                                
                                row["machID"] = machid;
                                row["responseTime"] = ttlResponsetime;
                                row["machType"] = machtype;
                                longestResponseTable.Rows.Add(row);

                                resetPrevRecord();
                            }
                            else if (prvMachid != machid && prvMerrtype == "ERROR" )
                            {   // diff mach
                                prvMachid = machid;
                                prvMdate = mdate;
                                prvMcode = mcode;
                                prvMtime = mtime;
                                prvMerrtype = merrtype;
                                prvMachtype = machtype;
                            }
                        }
                    }

                    if (longestResponseTable.Rows.Count >0)
                    {

                        if (chartType == "BarChart")
                        {
                            crLongestResponse crystalReport = new crLongestResponse();
                            crystalReport.Database.Tables["DataTableLongestResponse"].SetDataSource((DataTable)longestResponseTable);
                            crystalReport.SetParameterValue("userName", userName);
                            crystalReport.SetParameterValue("startDate", startDate);
                            crystalReport.SetParameterValue("endDate", endDate);
                            crystalReport.SetParameterValue("machType", machType);
                            //crystalReport.SetParameterValue("machID", machID);
                            CrystalReportViewer1.ReportSource = crystalReport;
                        }
                        else
                        {
                            crLongestResponse_group crystalReport = new crLongestResponse_group();
                            crystalReport.Database.Tables["DataTableLongestResponse"].SetDataSource((DataTable)longestResponseTable);
                            //crystalReport.SetParameterValue("userName", userName);
                            //crystalReport.SetParameterValue("startDate", startDate);
                            //crystalReport.SetParameterValue("endDate", endDate);
                            //crystalReport.SetParameterValue("machType", machType);
                            //crystalReport.SetParameterValue("machID", machID);
                            CrystalReportViewer1.ReportSource = crystalReport;
                        }
                    }
                    else
                    {
                        lblmsg.Text = "No record found!";
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


            }
            catch (Exception ex)
            {
                lblmsg.Text = ex.ToString();
            }

        }


        private void resetPrevRecord()
        {
            prvMachid = "";
            prvMdate = "";
            prvMcode = "";
            prvMtime = "";
            prvMerrtype = "";
            prvMachtype = "";
        }

        //private void resetPrvStatus(string mmachid, string mdate)
        //{
        //    prvMdate = mdate;
        //    prvMachid = mmachid;

        //}

        private void addPrvDataRows()
        {
            DataRow row = longestResponseTable.NewRow();
            row["KioskID"] = prvMachid;
            row["Date"] = prvMdate;

            longestResponseTable.Rows.Add(row);
        }
    }
}