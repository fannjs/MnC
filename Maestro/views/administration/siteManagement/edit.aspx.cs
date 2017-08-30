using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Services;
using Maestro.Classes;
using Maestro.Classes.Prefix;
using Maestro.Classes.MakerChecker;
using System.Json;

namespace Maestro.views.administration.siteManagement
{
    public partial class edit : System.Web.UI.Page
    {
        private static string taskName = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            string sCustCode = Request.Params["custCode"].ToString();
            inputSiteCode.Value = sCustCode;

            taskName = Request.Params["task"];
        }

        [WebMethod]
        public static CustDetails[] getCust(string sCustCode)
        {
            List<CustDetails> details = new List<CustDetails>();
            try
            {               
                dbconn dbc = new dbconn();
                dbc.connDB();
                SqlCommand cmd = dbc.conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM M_CUSTOMER WHERE M_CUST_CODE = '" + sCustCode + "'";
                SqlDataReader dr = cmd.ExecuteReader();
                CustDetails cust;
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        cust = new CustDetails();
                        cust.CustCode = dr["M_CUST_CODE"].ToString();
                        cust.CustName = dr["M_CUST_NAME"].ToString();
                        cust.Country = dr["M_MACH_COUNTRY"].ToString();
                        details.Add(cust);
                    }
                }
                dr.Close();
                dbc.closeConn();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Failed to retriev user details! \r\n Error: " + ex.Message);
            }

            return details.ToArray();
        }

        [WebMethod]
        public static McObject<Boolean> updateCust(string siteCode, string siteName, string siteCountry)
        {
            dbconn dbc = new dbconn();
            dbc.connDB();
            SqlCommand cmd = dbc.conn.CreateCommand();

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

                string sessionUID = HttpContext.Current.Session["userId"].ToString();
                string sessionUName = HttpContext.Current.Session["userName"].ToString();
                string taskID = MKCK_Function.GetTaskID(taskName);
                string action = MCAction.MODIFY;
                int status = MCStatus.PENDING;
                string sql = "UPDATE M_CUSTOMER SET M_CUST_NAME = '" + siteName + "', M_MACH_COUNTRY = '" + siteCountry + "' WHERE M_CUST_CODE = '" + siteCode + "'";

                cmd.CommandText = "SELECT * FROM M_CUSTOMER WHERE M_CUST_CODE = " + siteCode;
                SqlDataReader dr = cmd.ExecuteReader();

                string oldSiteCode = "";
                string oldSiteName = "";
                string oldSiteCountry = "";

                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        oldSiteCode = dr["M_CUST_CODE"].ToString();
                        oldSiteName = dr["M_CUST_NAME"].ToString();
                        oldSiteCountry = dr["M_MACH_COUNTRY"].ToString();
                    }
                }
                else
                {
                    throw new Exception("Record not found.");
                }
                dr.Close();

                JsonObject OLD = new JsonObject();
                OLD.Add("Site Code", oldSiteCode);
                OLD.Add("Site Name", oldSiteName);
                OLD.Add("Country", oldSiteCountry);

                JsonObject NEW = new JsonObject();
                NEW.Add("Site Code", siteCode);
                NEW.Add("Site Name", siteName);
                NEW.Add("Country", siteCountry);

                string olddata = OLD.ToString();
                string newdata = NEW.ToString();

                cmd.CommandText = "INSERT INTO M_MAKER_CHECKER_LIST(TASK_ID,TASK_NAME,CREATED_DATE,OLD_VALUE,NEW_VALUE,ACTION_TYPE,APPROVE_SQL,MAKER_ID,MAKER_USER_NAME,APPROVAL_STATUS) "
                                    + "VALUES(" + taskID + ", '" + taskName + "', GETDATE(), '" + olddata + "', '" + newdata + "', '" + action + "', "
                                    + "@sql, " + sessionUID + ", '" + sessionUName + "', " + status + ")";
                cmd.Parameters.AddWithValue("sql", sql);
                cmd.ExecuteNonQuery();                

                return new McObject<bool>(true, "Successful.");
            }
            catch (Exception ex)
            {
                return new McObject<bool>(false, "Failed. " + ex.Message);
            }
            finally
            {
                dbc.closeConn();
            }
        }
    }
}