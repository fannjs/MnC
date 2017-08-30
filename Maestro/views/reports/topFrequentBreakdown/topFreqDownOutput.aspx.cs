using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Maestro.views.reports.topFrequentBreakdown
{
    public partial class topFreqDownOutput : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string startDate = Request.QueryString["startDate"];
                string endDate = Request.QueryString["endDate"];
                string machType = Request.QueryString["machType"];
                string machID = Request.QueryString["machID"];
                string chartType = Request.QueryString["chartType"];
                string userName = Request.QueryString["userName"];
                ReportDocument crystalReport = new ReportDocument();
                string rptpath = Server.MapPath("");
                if (chartType == "BarChart")
                {
                    crystalReport.Load(rptpath + "/topFrequentDown_barChart.rpt");
                }
                else
                {
                    crystalReport.Load(rptpath + "/topFrequentDown_pieChart.rpt");
                }
                crystalReport.SetParameterValue("userName", userName);
                crystalReport.SetParameterValue("startDate", startDate);
                crystalReport.SetParameterValue("endDate", endDate);
                crystalReport.SetParameterValue("machType", machType);
                crystalReport.SetParameterValue("machID", machID);
                Configuration webConfigApp = WebConfigurationManager.OpenWebConfiguration("~");
                string odbcUid = webConfigApp.AppSettings.Settings["odbcUid"].Value;
                string odbcPwd = webConfigApp.AppSettings.Settings["odbcPwd"].Value;
                //crystalReport.SetDatabaseLogon("SSTAuto", "1qQA2wWS3eED");
                crystalReport.SetDatabaseLogon(odbcUid, odbcPwd);

                CrystalReportViewer1.ReportSource = crystalReport;
            }
            catch (Exception ex)
            {
                lblmsg.Text = ex.ToString();
            }

        }
    }
}