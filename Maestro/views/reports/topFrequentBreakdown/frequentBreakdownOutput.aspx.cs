using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Maestro.views.reports.topFrequentBreakdown
{
    public partial class frequentBreakdownOutput : System.Web.UI.Page
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

                    sql = sql + " AND MC.M_ERRORTYPE='ERROR' " +
                    " ORDER BY MR.M_MACH_ID";

                    cmd.CommandText = sql;
                    SqlDataReader dr = cmd.ExecuteReader();
                    string[] prevRecord = new string[6];
                    string machid = "";
                    string merrtype = "";
                    string machtype = "";

                    // Create a data table with the same name of crystal report's table.
                    DataTable freqBreakdownTable = new DataTable("table1");
                    // Create table columns with the same fields name of crystal report's table.
                    freqBreakdownTable.Columns.Add("kioskID");
                    freqBreakdownTable.Columns.Add("kioskType");
                    freqBreakdownTable.Columns.Add("errorType");

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            machid = dr["M_MACH_ID"].ToString();
                            merrtype = dr["M_ERRORTYPE"].ToString();
                            machtype = dr["M_MACH_TYPE"].ToString();

                            DataRow row = freqBreakdownTable.NewRow();
                            row["kioskID"] = machid;
                            row["kioskType"] = machtype;
                            row["errorType"] = merrtype;
                            freqBreakdownTable.Rows.Add(row);
                        }
                    }

                    if (freqBreakdownTable.Rows.Count > 0)
                    {
                        if (chartType == "BarChart")
                        {
                            // Create instance of the crystal report.
                            crFrequentBreakdown_barChart crystalReport = new crFrequentBreakdown_barChart();
                            crystalReport.Database.Tables["DataTableFrequentBreakdown"].SetDataSource((DataTable)freqBreakdownTable);
                            crystalReport.SetParameterValue("userName", userName);
                            crystalReport.SetParameterValue("startDate", startDate);
                            crystalReport.SetParameterValue("endDate", endDate);
                            crystalReport.SetParameterValue("machType", machType);
                            //crystalReport.SetParameterValue("machID", machID);
                            CrystalReportViewer1.ReportSource = crystalReport;
                        }
                        else
                        {
                            crFrequentBreakdown_pieChart crystalReport = new crFrequentBreakdown_pieChart();
                            crystalReport.Database.Tables["DataTableFrequentBreakdown"].SetDataSource((DataTable)freqBreakdownTable);
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

            }
            catch (Exception ex)
            {
                lblmsg.Text = ex.ToString();
            }

        }
    }
}