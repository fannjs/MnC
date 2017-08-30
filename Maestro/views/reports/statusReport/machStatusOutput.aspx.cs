using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Maestro.views.reports.statusReport
{
    public partial class machStatusOutput : System.Web.UI.Page
    {
        string prvMachid = "";
        string prvMdate = "";
        int prvErr = 0;
        int prvWarn = 0;
        int prvOffline = 0;
        int prvNormal = 0;

        private void resetPrevRecord()
        {
            prvMachid = "";
            prvMdate = "";
            prvErr = 0;
            prvWarn = 0;
            prvOffline = 0;
            prvNormal = 0;
        }

        // Create a data table with the same name of crystal report's table.
        DataTable machStatusTable = new DataTable("table1");

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

                    //ttlHrsMach = 24 * ttlmach; // 

                    // find total of days
                    sql = "SELECT COUNT(DISTINCT mr.M_DATE) AS ttlDays " +
                            " FROM M_REPORTING_DATA AS mr INNER JOIN " +
                            " M_CODES AS mc ON mr.M_CODE = mc.M_CODE "+
                            " WHERE (MR.M_DATE >='" + startDate + "' AND MR.M_DATE <='" + endDate + "') ";

                    if (!string.IsNullOrEmpty(machID) && machID != "All")
                    {
                        sql = sql + " AND MR.M_MACH_ID = '" + machID + "' ";
                    }
                    if (!string.IsNullOrEmpty(machType) && machType != "All")
                    {
                        sql = sql + " AND MC.M_MACH_TYPE = '" + machType + "' ";
                    }

                    sql = sql + " AND (MC.M_ERRORTYPE='ERROR' OR MC.M_ERRORTYPE='OFFLINE' OR MC.M_ERRORTYPE='WARN') ";

                            //" WHERE (mr.M_DATE >= '2015-01-01') AND (mr.M_DATE <= '2015-01-31') AND (mc.M_ERRORTYPE = 'ERROR' OR
                            //" mc.M_ERRORTYPE = 'WARN')";

                    cmd.CommandText = sql;
                    dr = cmd.ExecuteReader();
                    int ttlDays = 0;
                    if (dr.HasRows)
                    {
                        if (dr.Read())
                        {
                            ttlDays = (int)dr["ttlDays"];
                        }
                    }
                    dr.Close();

                    // end find ttl of days.

                    //" WHERE (MR.M_DATE >='2014-10-25' AND MR.M_DATE <='2014-10-31') "+
                    sql = " SELECT MR.M_DATE, MR.M_MACH_ID,  MC.M_ERRORTYPE, count (MR.M_DATE) as ttlcount " +
                    " FROM M_REPORTING_DATA MR " +
                    " INNER JOIN M_CODES as MC " +
                    " ON MR.M_CODE = MC.M_CODE " +
                    " WHERE (MR.M_DATE >='" + startDate + "' AND MR.M_DATE <='" + endDate + "') ";

                    if (!string.IsNullOrEmpty(machID) && machID != "All")
                    {
                        sql = sql + " AND MR.M_MACH_ID = '" + machID + "' ";
                    }
                    if (!string.IsNullOrEmpty(machType) && machType != "All")
                    {
                        sql = sql + " AND MC.M_MACH_TYPE = '" + machType + "' ";
                    }

                    sql = sql + " AND (MC.M_ERRORTYPE='ERROR' OR MC.M_ERRORTYPE='OFFLINE' OR MC.M_ERRORTYPE='WARN') " +
                    " group by MC.M_ERRORTYPE, MR.M_MACH_ID, MR.M_DATE ";

                    cmd.CommandText = sql;
                    dr = cmd.ExecuteReader();
                    resetPrevRecord();

                    string mmachid = "";
                    string mdate = "";
                    string merrtype = "";
                    //double ttlError = 0;
                    //double ttlWarn = 0;
                    //double ttlOffline = 0;
                    //double ttlNormal = 0;
                    //int ttlRecords = 0;

                    machStatusTable.Columns.Add("Date");
                    machStatusTable.Columns.Add("KioskID");
                    machStatusTable.Columns.Add("Error");
                    machStatusTable.Columns.Add("Warning");
                    machStatusTable.Columns.Add("Offline");
                    machStatusTable.Columns.Add("Normal");

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            mdate = dr["M_DATE"].ToString();
                            mmachid = dr["M_MACH_ID"].ToString();
                            merrtype = dr["M_ERRORTYPE"].ToString();

                            //ttlRecords++;
                            //if (merrtype.Equals("ERROR"))
                            //{
                            //    ttlError++;
                            //}else if (merrtype.Equals("WARN"))
                            //{
                            //    ttlWarn++;
                            //}else if (merrtype.Equals("OFFLINE"))
                            //{
                            //    ttlOffline++;
                            //}

                            if (string.IsNullOrEmpty(prvMdate))
                            {   // first date null, then set prevRecord
                                resetPrvStatus(mmachid, mdate);
                                updatePrvStatus(merrtype);
                            }
                            else if (prvMdate == mdate && prvMachid != mmachid)
                            {   //same date, diff mach.
                                addPrvDataRows();
                                resetPrvStatus(mmachid, mdate);
                                updatePrvStatus(merrtype);
                            }
                            else if (prvMdate == mdate && prvMachid == mmachid)
                            {   //same date, same mach.
                                updatePrvStatus(merrtype);
                            }
                            else if (prvMdate != mdate)
                            {   //diff date
                                addPrvDataRows();
                                resetPrvStatus(mmachid, mdate);
                                updatePrvStatus(merrtype);
                            }
                        }
                    }

                    // last record
                    addPrvDataRows();

                    //// get total records n pass to CR.
                    //ttlError = (ttlError / ttlmach) * 100 / 3; // count 3 statetype, err, warn and offline.
                    //ttlWarn = (ttlWarn / ttlmach) * 100 / 3;
                    //ttlOffline = (ttlOffline / ttlmach) * 100 / 3;
                    //ttlNormal = 100 - ttlError - ttlWarn - ttlOffline;

                    //DataTable subMachStatusTable = new DataTable("tableSub");

                    //subMachStatusTable.Columns.Add("paramStatusType");
                    //subMachStatusTable.Columns.Add("paramValue");

                    //DataRow rowSub = subMachStatusTable.NewRow();
                    //rowSub["paramStatusType"] = "ERROR";
                    //rowSub["paramValue"] = ttlError;
                    //subMachStatusTable.Rows.Add(rowSub);

                    ////row = subMachStatusTable.NewRow();
                    ////row["paramStatusType"] = "WARN";
                    ////row["paramValue"] = ttlWarn;
                    ////subMachStatusTable.Rows.Add(row);

                    ////row = subMachStatusTable.NewRow();
                    ////row["paramStatusType"] = "OFFLINE";
                    ////row["paramValue"] = ttlOffline;
                    ////subMachStatusTable.Rows.Add(row);

                    ////row = subMachStatusTable.NewRow();
                    ////row["paramStatusType"] = "NORMAL";
                    ////row["paramValue"] = ttlNormal;
                    ////subMachStatusTable.Rows.Add(row);


                    //if (subMachStatusTable.Rows.Count > 0)
                    //{
                    //    crSubMachStatus crSub = new crSubMachStatus();
                    //    crSub.Database.Tables["DataSetSubMachStatus"].SetDataSource((DataTable)subMachStatusTable);
                    //    //crSub.SetParameterValue("paramError", ttlError);
                    //    //crSub.SetParameterValue("paramWarn", ttlWarn);
                    //    //crSub.SetParameterValue("paramOffline", ttlOffline);
                    //    //crSub.SetParameterValue("paramNormal", ttlNormal);
                    //    //CrystalReportViewer1.ReportSource = crSub;
                    //    //CrystalReportViewer1.ReuseParameterValuesOnRefresh = true;
                    //}
                    if (machStatusTable.Rows.Count > 0)
                    {
                        //if (chartType == "BarChart")
                        crMachStatus crystalReport = new crMachStatus();
                        crystalReport.Database.Tables["DateSetMachStatus"].SetDataSource((DataTable)machStatusTable);
                        crystalReport.SetParameterValue("ttlMach", ttlmach);
                        crystalReport.SetParameterValue("userName", userName);
                        crystalReport.SetParameterValue("startDate", startDate);
                        crystalReport.SetParameterValue("endDate", endDate);
                        crystalReport.SetParameterValue("machType", machType);
                        crystalReport.SetParameterValue("machID", machID);
                        //crystalReport.SetParameterValue("paraError", ttlError);
                        //crystalReport.SetParameterValue("paraWarn", ttlWarn);
                        //crystalReport.SetParameterValue("paraOffline", ttlOffline);
                        //crystalReport.SetParameterValue("paraNormal", ttlNormal);
                        CrystalReportViewer1.ReportSource = crystalReport;
                        CrystalReportViewer1.ReuseParameterValuesOnRefresh = true;
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

        private void updatePrvStatus(string errtype)
        {

            if (errtype == "WARN")
            {
                prvWarn = prvWarn + 1;
            }
            if (errtype == "ERROR")
            {
                prvErr = prvErr + 1;
            }
            if (errtype == "OFFLINE")
            {
                prvOffline = prvOffline + 1;
            }
        }

        private void resetPrvStatus(string mmachid, string mdate)
        {
            prvMdate = mdate;
            prvMachid = mmachid;
            prvWarn = 0;
            prvErr = 0;
            prvOffline = 0;
        }

        private void addPrvDataRows()
        {
            DataRow row = machStatusTable.NewRow();
            row["KioskID"] = prvMachid;
            row["Date"] = prvMdate;
            row["Error"] = prvErr;
            row["Warning"] = prvWarn;
            row["Offline"] = prvOffline;
            machStatusTable.Rows.Add(row);
        }
    }
}