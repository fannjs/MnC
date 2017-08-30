using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Maestro.views.reports.fullKioskList
{
    public partial class fullKioskListOutput : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string machType = Request.QueryString["machType"];
                string userName = Request.QueryString["userName"];
                ReportDocument crystalReport = new ReportDocument();
                string rptpath = Server.MapPath("");
                crystalReport.Load(rptpath + "/crFullKioskList.rpt");
                crystalReport.SetParameterValue("userName", userName);
                crystalReport.SetParameterValue("machType", machType);
                //crystalReport.SetParameterValue("machID", machID);
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